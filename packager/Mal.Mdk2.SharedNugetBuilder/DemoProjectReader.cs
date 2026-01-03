using System.IO.Compression;
using System.Xml.Linq;

namespace Mal.Mdk2.SharedNugetBuilder;

public class SharedProjectReference
{
    public required FileInfo ProjectFile { get; init; }
    public required string PackageId { get; init; }
}

public class DemoProjectReader
{
    public static SharedProjectReference[] FindSharedProjectReferences(FileInfo demoProjectFile)
    {
        if (!demoProjectFile.Exists)
            throw new FileNotFoundException($"Demo project file not found: {demoProjectFile.FullName}");

        if (!demoProjectFile.Name.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"File is not a C# project (.csproj): {demoProjectFile.Name}");

        var csprojDoc = XDocument.Load(demoProjectFile.FullName);
        var sharedProjects = new List<SharedProjectReference>();

        // Find all <Import Project="..." Label="Shared" /> elements
        var imports = csprojDoc.Descendants("Import")
            .Where(e => e.Attribute("Label")?.Value == "Shared");

        foreach (var import in imports)
        {
            var projectAttr = import.Attribute("Project");
            if (projectAttr == null)
                continue;

            var importPath = projectAttr.Value;

            // Resolve relative path
            var projectDir = demoProjectFile.Directory!;
            var fullPath = Path.GetFullPath(Path.Combine(projectDir.FullName, importPath));

            // If it's a .projitems file, convert to .shproj
            if (fullPath.EndsWith(".projitems", StringComparison.OrdinalIgnoreCase)) fullPath = fullPath.Substring(0, fullPath.Length - ".projitems".Length) + ".shproj";

            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"Warning: Referenced shared project not found: {fullPath}");
                continue;
            }

            // Get package ID from .shproj filename
            var packageId = Path.GetFileNameWithoutExtension(fullPath);

            sharedProjects.Add(new SharedProjectReference
            {
                ProjectFile = new FileInfo(fullPath),
                PackageId = packageId
            });
        }

        return sharedProjects.ToArray();
    }

    public static string[] DetectSharedProjectDependencies(FileInfo sharedProjectFile, DirectoryInfo nugetSourceDir)
    {
        var projectDir = sharedProjectFile.Directory!;
        var projItemsFile = Path.Combine(projectDir.FullName,
            Path.GetFileNameWithoutExtension(sharedProjectFile.Name) + ".projitems");

        if (!File.Exists(projItemsFile))
            return Array.Empty<string>();

        var projItemsDoc = XDocument.Load(projItemsFile);
        var dependencies = new List<string>();

        // Look for PackageReference elements
        var packageRefs = projItemsDoc.Descendants()
            .Where(e => e.Name.LocalName == "PackageReference");

        foreach (var packageRef in packageRefs)
        {
            var includeAttr = packageRef.Attribute("Include");
            var versionAttr = packageRef.Attribute("Version");

            if (includeAttr == null || versionAttr == null)
                continue;

            var packageId = includeAttr.Value;
            var version = versionAttr.Value;

            // Check if this is a shared project package
            if (IsSharedProjectPackage(packageId, version, nugetSourceDir)) dependencies.Add($"{packageId}:{version}");
        }

        return dependencies.ToArray();
    }

    static bool IsSharedProjectPackage(string packageId, string version, DirectoryInfo nugetSourceDir)
    {
        // Look for the package in the NuGet source directory
        var packageFileName = $"{packageId}.{version}.nupkg";
        var packagePath = Path.Combine(nugetSourceDir.FullName, packageFileName);

        if (!File.Exists(packagePath))
            return false;

        try
        {
            // Open the package and check for our marker
            using var zip = ZipFile.OpenRead(packagePath);
            var nuspecEntry = zip.Entries.FirstOrDefault(e => e.Name.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));

            if (nuspecEntry == null)
                return false;

            using var stream = nuspecEntry.Open();
            using var reader = new StreamReader(stream);
            var nuspecContent = reader.ReadToEnd();

            // Check for our mdk-mixin tag marker
            return nuspecContent.Contains("<tags>") && nuspecContent.Contains("mdk-mixin");
        }
        catch
        {
            return false;
        }
    }

    public static DirectoryInfo? FindSolutionRoot(FileInfo projectFile)
    {
        var dir = projectFile.Directory;

        while (dir != null)
        {
            // Look for .sln or .slnx files
            var solutionFiles = dir.GetFiles("*.sln").Concat(dir.GetFiles("*.slnx")).ToArray();
            if (solutionFiles.Length > 0)
                return dir;

            dir = dir.Parent;
        }

        return null;
    }
}