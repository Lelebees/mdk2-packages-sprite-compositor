using System.Text;
using System.Xml.Linq;
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace Mdk.SharedNuGet;

public class PackageDependency
{
    public required string Id { get; init; }
    public required string Version { get; init; }
}

public class NuGetPackageGenerator
{
    public static async Task<string> GeneratePackage(
        FileInfo sharedProjectFile,
        SharedProjectMetadata metadata,
        DirectoryInfo outputDir,
        PackageDependency[] dependencies,
        bool dryRun = false,
        bool force = false)
    {
        var projectDir = sharedProjectFile.Directory!;
        var packageFileName = $"{metadata.PackageId}.{metadata.Version}.nupkg";
        var packagePath = Path.Combine(outputDir.FullName, packageFileName);

        if (!dryRun)
        {
            if (!outputDir.Exists)
                outputDir.Create();

            if (File.Exists(packagePath) && !force)
            {
                Console.WriteLine($"Package already exists: {packagePath}");
                Console.WriteLine("Use --force to rebuild.");
                return packagePath;
            }
        }

        var tempDir = Path.Combine(Path.GetTempPath(), $"mdk-nuget-{Guid.NewGuid()}");

        try
        {
            if (!dryRun)
                Directory.CreateDirectory(tempDir);

            // 1. Generate .nuspec file
            var nuspecContent = generateNuspec(metadata, dependencies);
            var nuspecPath = Path.Combine(tempDir, $"{metadata.PackageId}.nuspec");

            if (dryRun)
            {
                Console.WriteLine("\n=== Generated .nuspec ===");
                Console.WriteLine(nuspecContent);
                if (metadata.ReadmePath != null)
                {
                    Console.WriteLine($"\n✓ Would include readme: {Path.GetFileName(metadata.ReadmePath)}");
                }
            }
            else
            {
                await File.WriteAllTextAsync(nuspecPath, nuspecContent);
                Console.WriteLine($"✓ Created .nuspec: {nuspecPath}");
                
                // Copy readme to package root
                if (metadata.ReadmePath != null)
                {
                    var readmeDestPath = Path.Combine(tempDir, "readme.md");
                    File.Copy(metadata.ReadmePath, readmeDestPath, true);
                    Console.WriteLine($"✓ Included readme.md");
                }
            }

            // 2. Copy shared project files
            var contentDir = Path.Combine(tempDir, "contentFiles", "cs", "any", "Shared");
            if (!dryRun)
            {
                Directory.CreateDirectory(contentDir);
                copySharedProjectFiles(projectDir, contentDir, sharedProjectFile.Name);
                Console.WriteLine($"✓ Copied shared project files to content");
            }
            else
            {
                Console.WriteLine($"\n=== Files to include ===");
                var files = getSharedProjectFiles(projectDir, sharedProjectFile.Name);
                foreach (var file in files)
                {
                    var relativePath = Path.GetRelativePath(projectDir.FullName, file);
                    Console.WriteLine($"  - {relativePath}");
                }
            }

            // Show files being included (including metadata and readme)
            if (!dryRun)
            {
                var allFiles = getAllProjectFiles(projectDir);
                var includedFiles = getSharedProjectFiles(projectDir, sharedProjectFile.Name);
                var excludedFiles = allFiles.Except(includedFiles).ToArray();

                Console.WriteLine("\nFiles in shared project directory:");
                Console.WriteLine("  Included in package:");
                foreach (var file in includedFiles)
                {
                    var relativePath = Path.GetRelativePath(projectDir.FullName, file);
                    Console.WriteLine($"    ✓ {relativePath}");
                }

                if (excludedFiles.Length > 0)
                {
                    Console.WriteLine("  Excluded (metadata/readme/build artifacts):");
                    foreach (var file in excludedFiles)
                    {
                        var relativePath = Path.GetRelativePath(projectDir.FullName, file);
                        var fileName = Path.GetFileName(file);
                        var reason = fileName.StartsWith("_") ? "metadata" :
                                   fileName.Equals("readme.md", StringComparison.OrdinalIgnoreCase) ? "readme (added to package root)" :
                                   file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}") || 
                                   file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}") ? "build artifact" :
                                   "excluded";
                        Console.WriteLine($"    - {relativePath} ({reason})");
                    }
                }
            }

            // 3. Generate MSBuild integration files
            var propsContent = generatePropsFile(metadata);
            var targetsContent = generateTargetsFile(metadata);
            var buildDir = Path.Combine(tempDir, "build");

            if (dryRun)
            {
                Console.WriteLine("\n=== Generated .props ===");
                Console.WriteLine(propsContent);
                Console.WriteLine("\n=== Generated .targets ===");
                Console.WriteLine(targetsContent);
            }
            else
            {
                Directory.CreateDirectory(buildDir);
                await File.WriteAllTextAsync(Path.Combine(buildDir, $"{metadata.PackageId}.props"), propsContent);
                await File.WriteAllTextAsync(Path.Combine(buildDir, $"{metadata.PackageId}.targets"), targetsContent);
                Console.WriteLine($"✓ Created MSBuild integration files");
            }

            // 4. Create .nupkg
            if (!dryRun)
            {
                await createNupkg(tempDir, nuspecPath, packagePath);
                Console.WriteLine($"✓ Created package: {packagePath}");
            }
            else
            {
                Console.WriteLine($"\nWould create package: {packagePath}");
            }

            return packagePath;
        }
        finally
        {
            if (!dryRun && Directory.Exists(tempDir))
            {
                try { Directory.Delete(tempDir, true); }
                catch { /* Ignore cleanup errors */ }
            }
        }
    }

    static string generateNuspec(SharedProjectMetadata metadata, PackageDependency[] dependencies)
    {
        var nuspec = new XDocument(
            new XElement("package",
                new XElement("metadata",
                    new XElement("id", metadata.PackageId),
                    new XElement("version", metadata.Version),
                    new XElement("description", metadata.Description),
                    new XElement("authors", string.Join(",", metadata.Authors))
                )
            )
        );

        var metadataElement = nuspec.Root!.Element("metadata")!;

        if (metadata.ReadmePath != null)
            metadataElement.Add(new XElement("readme", "readme.md"));

        if (metadata.License != null)
            metadataElement.Add(new XElement("license", new XAttribute("type", "expression"), metadata.License));

        if (metadata.ProjectUrl != null)
            metadataElement.Add(new XElement("projectUrl", metadata.ProjectUrl));

        // Always add mdk-mixin tag to identify these packages
        var tags = metadata.Tags != null && metadata.Tags.Length > 0 
            ? string.Join(" ", metadata.Tags.Append("mdk-mixin"))
            : "mdk-mixin";
        metadataElement.Add(new XElement("tags", tags));

        // Add dependencies
        if (dependencies.Length > 0)
        {
            var depsGroup = new XElement("group");
            if (metadata.TargetFramework != null)
                depsGroup.Add(new XAttribute("targetFramework", metadata.TargetFramework));

            foreach (var dep in dependencies)
            {
                depsGroup.Add(new XElement("dependency",
                    new XAttribute("id", dep.Id),
                    new XAttribute("version", dep.Version)));
            }

            metadataElement.Add(new XElement("dependencies", depsGroup));
        }

        // Add contentFiles configuration
        metadataElement.Add(new XElement("contentFiles",
            new XElement("files",
                new XAttribute("include", "cs/any/Shared/**/*.*"),
                new XAttribute("buildAction", "None"),
                new XAttribute("copyToOutput", "false")
            )
        ));

        return nuspec.ToString();
    }

    static string generatePropsFile(SharedProjectMetadata metadata)
    {
        var props = new XDocument(
            new XElement("Project",
                new XElement("PropertyGroup",
                    new XElement($"{metadata.PackageId}Imported", "true")
                )
            )
        );
        return props.ToString();
    }

    static string generateTargetsFile(SharedProjectMetadata metadata)
    {
        var targets = new XDocument(
            new XElement("Project",
                new XElement("Import",
                    new XAttribute("Project", $"$(MSBuildThisFileDirectory)..\\contentFiles\\cs\\any\\Shared\\{metadata.PackageId}.projitems"),
                    new XAttribute("Label", "Shared")
                )
            )
        );
        return targets.ToString();
    }

    static string[] getSharedProjectFiles(DirectoryInfo projectDir, string shprojName)
    {
        var files = Directory.GetFiles(projectDir.FullName, "*.*", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}") && 
                        !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
            .Where(f => !Path.GetFileName(f).StartsWith("_"))
            .Where(f => !Path.GetFileName(f).Equals("readme.md", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        return files;
    }

    static string[] getAllProjectFiles(DirectoryInfo projectDir)
    {
        var files = Directory.GetFiles(projectDir.FullName, "*.*", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}") && 
                        !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
            .ToArray();
        return files;
    }

    static void copySharedProjectFiles(DirectoryInfo sourceDir, string targetDir, string shprojName)
    {
        var files = getSharedProjectFiles(sourceDir, shprojName);

        foreach (var file in files)
        {
            var relativePath = Path.GetRelativePath(sourceDir.FullName, file);
            var targetPath = Path.Combine(targetDir, relativePath);
            var targetFileDir = Path.GetDirectoryName(targetPath)!;

            if (!Directory.Exists(targetFileDir))
                Directory.CreateDirectory(targetFileDir);

            File.Copy(file, targetPath, true);
        }
    }

    static async Task createNupkg(string workingDir, string nuspecPath, string outputPath)
    {
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        var builder = new PackageBuilder();
        
        using (var nuspecStream = File.OpenRead(nuspecPath))
        {
            var manifest = Manifest.ReadFrom(nuspecStream, validateSchema: false);
            builder.Populate(manifest.Metadata);
        }

        // Add all files from the working directory
        var baseDir = new DirectoryInfo(workingDir);
        foreach (var file in Directory.GetFiles(workingDir, "*.*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(workingDir, file);
            
            // Skip the nuspec file itself
            if (relativePath.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase))
                continue;

            var physicalFile = new PhysicalPackageFile
            {
                SourcePath = file,
                TargetPath = relativePath.Replace("\\", "/")
            };
            builder.Files.Add(physicalFile);
        }

        // Create the package
        using (var outputStream = File.Create(outputPath))
        {
            builder.Save(outputStream);
        }
        
        await Task.CompletedTask;
    }

    public static PackageDependency[] ParseDependencies(string[] dependencyArgs)
    {
        var dependencies = new List<PackageDependency>();

        foreach (var dep in dependencyArgs)
        {
            var parts = dep.Split(':', 2);
            if (parts.Length != 2)
                throw new ArgumentException($"Invalid dependency format: '{dep}'. Expected format: PackageId:Version");

            dependencies.Add(new PackageDependency
            {
                Id = parts[0].Trim(),
                Version = parts[1].Trim()
            });
        }

        return dependencies.ToArray();
    }
}
