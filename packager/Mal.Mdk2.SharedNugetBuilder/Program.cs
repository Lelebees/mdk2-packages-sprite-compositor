using System.CommandLine;
using Mdk.SharedNuGet;

var rootCommand = new RootCommand("Mdk.SharedNuGet - Generate NuGet packages for .NET Shared Projects");

var demoOption = new Option<FileInfo?>(
    "--demo",
    "Path to demo project (.csproj) to analyze and package all referenced shared projects");

var sharedOption = new Option<FileInfo?>(
    "--shared",
    "Path to shared project (.shproj) to package directly");

var outputOption = new Option<DirectoryInfo?>(
    "--output",
    "Output directory for generated .nupkg files");

var dependencyOption = new Option<string[]>(
    "--dependency",
    "Specify dependency as PackageId:Version (repeatable)")
{ AllowMultipleArgumentsPerToken = true };

var forceOption = new Option<bool>(
    "--force",
    "Force rebuild even if nothing changed");

var dryRunOption = new Option<bool>(
    "--dry-run",
    "Preview what would be packaged without creating files");

var autoBumpOption = new Option<bool>(
    "--auto-bump",
    "Automatically increment the build version number (e.g., 1.0.0 -> 1.0.1)");

var ideOutputOption = new Option<bool>(
    "--ide-output",
    "Format output for IDE integration (MSBuild-compatible errors/warnings)");

rootCommand.AddOption(demoOption);
rootCommand.AddOption(sharedOption);
rootCommand.AddOption(outputOption);
rootCommand.AddOption(dependencyOption);
rootCommand.AddOption(forceOption);
rootCommand.AddOption(dryRunOption);
rootCommand.AddOption(autoBumpOption);
rootCommand.AddOption(ideOutputOption);

rootCommand.SetHandler(handleCommand, demoOption, sharedOption, outputOption, dependencyOption, forceOption, dryRunOption, autoBumpOption, ideOutputOption);

return await rootCommand.InvokeAsync(args);

async Task handleCommand(FileInfo? demo, FileInfo? shared, DirectoryInfo? output, string[] dependencies, bool force, bool dryRun, bool autoBump, bool ideOutput)
{
    IOutput outputter = ideOutput ? new IdeOutput() : new CliOutput();
    if (demo == null && shared == null)
    {
        outputter.Error("Either --demo or --shared must be specified.");
        Environment.ExitCode = 1;
        return;
    }

    if (demo != null && shared != null)
    {
        outputter.Error("Cannot specify both --demo and --shared.");
        Environment.ExitCode = 1;
        return;
    }

    if (shared != null && output == null)
    {
        outputter.Error("--output is required when using --shared mode.");
        Environment.ExitCode = 1;
        return;
    }

    Console.WriteLine($"Mode: {(demo != null ? "Demo-driven" : "Direct")}");
    if (demo != null) Console.WriteLine($"Demo project: {demo.FullName}");
    if (shared != null) Console.WriteLine($"Shared project: {shared.FullName}");
    if (output != null) Console.WriteLine($"Output directory: {output.FullName}");
    if (dependencies.Length > 0) Console.WriteLine($"Dependencies: {string.Join(", ", dependencies)}");
    if (force) Console.WriteLine("Force rebuild: enabled");
    if (dryRun) Console.WriteLine("Dry run: enabled");
    if (autoBump) Console.WriteLine("Auto-bump version: enabled");
    Console.WriteLine();

    try
    {
        if (shared != null)
        {
            var metadata = SharedProjectReader.ReadMetadata(shared);
            
            // Auto-bump version if requested
            if (autoBump && !dryRun)
            {
                var newVersion = SharedProjectReader.BumpVersion(shared, metadata.Version);
                Console.WriteLine($"Version bumped: {metadata.Version} -> {newVersion}");
                metadata = metadata with { Version = newVersion };
            }
            
            Console.WriteLine($"Package ID: {metadata.PackageId}");
            Console.WriteLine($"Version: {metadata.Version}");
            Console.WriteLine($"Description: {metadata.Description}");
            Console.WriteLine($"Authors: {string.Join(", ", metadata.Authors)}");
            if (metadata.TargetFramework != null) Console.WriteLine($"Target Framework: {metadata.TargetFramework}");
            if (metadata.License != null) Console.WriteLine($"License: {metadata.License}");
            if (metadata.ProjectUrl != null) Console.WriteLine($"Project URL: {metadata.ProjectUrl}");
            if (metadata.Tags != null) Console.WriteLine($"Tags: {string.Join(", ", metadata.Tags)}");
            
            Console.WriteLine("\n✓ Metadata validation passed");
            
            // Parse dependencies
            var deps = NuGetPackageGenerator.ParseDependencies(dependencies);
            if (deps.Length > 0)
            {
                Console.WriteLine($"Dependencies: {string.Join(", ", deps.Select(d => $"{d.Id}:{d.Version}"))}");
            }
            
            // Generate package
            Console.WriteLine();
            var packagePath = await NuGetPackageGenerator.GeneratePackage(
                shared, metadata, output!, deps, dryRun, force);
            
            if (!dryRun)
            {
                Console.WriteLine($"\n✓ Package created successfully: {packagePath}");
            }
        }
        else
        {
            // Demo-driven mode
            var sharedProjects = DemoProjectReader.FindSharedProjectReferences(demo);
            
            if (sharedProjects.Length == 0)
            {
                Console.WriteLine("No shared project references found in demo project.");
                return;
            }

            Console.WriteLine($"Found {sharedProjects.Length} shared project reference(s):");
            foreach (var sp in sharedProjects)
            {
                Console.WriteLine($"  - {sp.PackageId}");
            }
            Console.WriteLine();

            // Determine output directory
            DirectoryInfo outputDir;
            if (output != null)
            {
                outputDir = output;
            }
            else
            {
                var solutionRoot = DemoProjectReader.FindSolutionRoot(demo);
                if (solutionRoot == null)
                {
                    Console.Error.WriteLine("Error: Could not find solution root. Please specify --output.");
                    Environment.ExitCode = 1;
                    return;
                }
                outputDir = new DirectoryInfo(Path.Combine(solutionRoot.FullName, "packages"));
                Console.WriteLine($"Output directory: {outputDir.FullName} (solution root)");
            }

            if (!outputDir.Exists && !dryRun)
                outputDir.Create();

            // Package each shared project
            foreach (var sharedProject in sharedProjects)
            {
                Console.WriteLine($"\n=== Packaging {sharedProject.PackageId} ===");
                
                var metadata = SharedProjectReader.ReadMetadata(sharedProject.ProjectFile);
                
                // Auto-bump version if requested
                if (autoBump && !dryRun)
                {
                    var newVersion = SharedProjectReader.BumpVersion(sharedProject.ProjectFile, metadata.Version);
                    Console.WriteLine($"Version bumped: {metadata.Version} -> {newVersion}");
                    metadata = metadata with { Version = newVersion };
                }
                
                Console.WriteLine($"Version: {metadata.Version}");
                Console.WriteLine($"Description: {metadata.Description}");

                // Detect dependencies
                var detectedDeps = DemoProjectReader.DetectSharedProjectDependencies(
                    sharedProject.ProjectFile, outputDir);
                
                if (detectedDeps.Length > 0)
                {
                    Console.WriteLine($"Detected dependencies: {string.Join(", ", detectedDeps)}");
                }

                var deps = NuGetPackageGenerator.ParseDependencies(detectedDeps);

                if (!dryRun)
                {
                    var packagePath = await NuGetPackageGenerator.GeneratePackage(
                        sharedProject.ProjectFile, metadata, outputDir, deps, dryRun, force);
                    
                    Console.WriteLine($"✓ Package created: {packagePath}");
                }
            }

            if (!dryRun)
            {
                Console.WriteLine($"\n✓ Successfully packaged {sharedProjects.Length} shared project(s)");
            }
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"\n✗ Error: {ex.Message}");
        Environment.ExitCode = 1;
    }
    
    await Task.CompletedTask;
}
