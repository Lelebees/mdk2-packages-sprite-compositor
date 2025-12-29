namespace Mdk.SharedNuGet;

public record SharedProjectMetadata
{
    public required string PackageId { get; init; }
    public required string Version { get; init; }
    public required string Description { get; init; }
    public required string[] Authors { get; init; }
    public string? TargetFramework { get; init; }
    public string? License { get; init; }
    public string? ProjectUrl { get; init; }
    public string[]? Tags { get; init; }
    public string? ReadmePath { get; init; }
}

public class SharedProjectReader
{
    public static SharedProjectMetadata ReadMetadata(FileInfo sharedProjectFile)
    {
        if (!sharedProjectFile.Exists)
            throw new FileNotFoundException($"Shared project file not found: {sharedProjectFile.FullName}");

        if (!sharedProjectFile.Name.EndsWith(".shproj", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"File is not a shared project (.shproj): {sharedProjectFile.Name}");

        var projectDir = sharedProjectFile.Directory!;
        var packageId = Path.GetFileNameWithoutExtension(sharedProjectFile.Name);

        var errors = new List<string>();
        
        var version = tryReadRequiredFile(projectDir, "_version", "version", errors);
        var description = tryReadRequiredFile(projectDir, "_description", "description", errors);
        var authors = tryReadRequiredFileLines(projectDir, "_authors", "authors", errors);

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "Validation failed. Please fix the following issues:\n\n" +
                string.Join("\n\n", errors.Select((e, i) => $"{i + 1}. {e}")));
        }

        var targetFramework = readOptionalFile(projectDir, "_targetframework");
        var license = readOptionalFile(projectDir, "_license");
        var projectUrl = readOptionalFile(projectDir, "_projecturl");
        var tags = readOptionalFileLines(projectDir, "_tags");
        var readmePath = findReadmeFile(projectDir);

        return new SharedProjectMetadata
        {
            PackageId = packageId,
            Version = version!,
            Description = description!,
            Authors = authors!,
            TargetFramework = targetFramework,
            License = license,
            ProjectUrl = projectUrl,
            Tags = tags,
            ReadmePath = readmePath
        };
    }

    static string? tryReadRequiredFile(DirectoryInfo dir, string fileName, string fieldName, List<string> errors)
    {
        var filePath = Path.Combine(dir.FullName, fileName);
        if (!File.Exists(filePath))
        {
            errors.Add(
                $"Missing required metadata file: {fileName}\n" +
                $"   Location: {dir.FullName}\n" +
                $"   Please create this file containing the {fieldName} for the NuGet package.");
            return null;
        }

        var content = File.ReadAllText(filePath).Trim();
        if (string.IsNullOrWhiteSpace(content))
        {
            errors.Add(
                $"Required metadata file is empty: {fileName}\n" +
                $"   Location: {filePath}\n" +
                $"   Please add the {fieldName} to this file.");
            return null;
        }

        return content;
    }

    static string[]? tryReadRequiredFileLines(DirectoryInfo dir, string fileName, string fieldName, List<string> errors)
    {
        var filePath = Path.Combine(dir.FullName, fileName);
        if (!File.Exists(filePath))
        {
            errors.Add(
                $"Missing required metadata file: {fileName}\n" +
                $"   Location: {dir.FullName}\n" +
                $"   Please create this file containing the {fieldName} for the NuGet package, one per line.");
            return null;
        }

        var lines = File.ReadAllLines(filePath)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToArray();

        if (lines.Length == 0)
        {
            errors.Add(
                $"Required metadata file is empty: {fileName}\n" +
                $"   Location: {filePath}\n" +
                $"   Please add the {fieldName} to this file.");
            return null;
        }

        return lines;
    }

    static string? readOptionalFile(DirectoryInfo dir, string fileName)
    {
        var filePath = Path.Combine(dir.FullName, fileName);
        if (!File.Exists(filePath))
            return null;

        var content = File.ReadAllText(filePath).Trim();
        return string.IsNullOrWhiteSpace(content) ? null : content;
    }

    static string[]? readOptionalFileLines(DirectoryInfo dir, string fileName)
    {
        var filePath = Path.Combine(dir.FullName, fileName);
        if (!File.Exists(filePath))
            return null;

        var lines = File.ReadAllLines(filePath)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToArray();

        return lines.Length == 0 ? null : lines;
    }

    static string? findReadmeFile(DirectoryInfo dir)
    {
        var readmeFiles = new[] { "readme.md", "README.md", "Readme.md", "ReadMe.md" };
        
        foreach (var fileName in readmeFiles)
        {
            var filePath = Path.Combine(dir.FullName, fileName);
            if (File.Exists(filePath))
                return filePath;
        }

        return null;
    }

    public static string BumpVersion(FileInfo sharedProjectFile, string currentVersion)
    {
        var projectDir = sharedProjectFile.Directory!;
        var versionFilePath = Path.Combine(projectDir.FullName, "_version");

        // Parse version (supports major.minor.build or major.minor.build.revision)
        var parts = currentVersion.Split('.');
        if (parts.Length < 2 || parts.Length > 4)
            throw new InvalidOperationException($"Version format not supported for auto-bump: {currentVersion}");

        // Increment the last component (build or revision)
        var lastIndex = parts.Length - 1;
        if (!int.TryParse(parts[lastIndex], out var lastNumber))
            throw new InvalidOperationException($"Cannot auto-bump non-numeric version component: {parts[lastIndex]}");

        parts[lastIndex] = (lastNumber + 1).ToString();
        var newVersion = string.Join(".", parts);

        // Write back to file
        File.WriteAllText(versionFilePath, newVersion);

        return newVersion;
    }
}
