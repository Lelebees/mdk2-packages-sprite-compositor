# MDK2 Shared Project NuGet Packager

A tool for packaging .NET Shared Projects (.shproj) as NuGet packages, enabling easy distribution and reuse of shared
code libraries.

## Quick Start

### Setting Up Your Shared Project

1. **Create or use an existing shared project** (`.shproj`)

2. **Add required metadata files** to your shared project directory (same location as the `.shproj` file):

    - `_version` - Package version (e.g., `1.0.0`)
    - `_description` - One-line description of your package
    - `_authors` - Author names, one per line

3. **Add optional metadata files**:

    - `_targetframework` - Target framework (e.g., `net8.0`, `netstandard2.0`)
    - `_license` - License identifier (e.g., `MIT`, `Apache-2.0`)
    - `_projecturl` - Project or repository URL
    - `_tags` - Package tags for discoverability, one per line
    - `readme.md` - Package documentation (automatically displayed in NuGet Package Manager)

### Example Shared Project Structure

```
MySharedLibrary/
├── MySharedLibrary.shproj
├── _version              (1.0.0)
├── _description          (A reusable shared library)
├── _authors              (Your Name)
├── readme.md             (Optional: Package documentation)
├── Class1.cs
└── Class2.cs
```

## Using the Packager

### Direct Mode (Manual Packaging)

Package a specific shared project:

```bash
mdk2-nuget --shared path/to/YourProject.shproj --output packages/
```

With dependencies:

```bash
mdk2-nuget --shared path/to/YourProject.shproj \
           --dependency SomePackage:1.0.0 \
           --dependency AnotherPackage:2.5.0 \
           --output packages/
```

### Demo-Driven Mode (Automatic - Coming Soon)

Automatically package all shared projects referenced by a demo project:

```bash
mdk2-nuget --demo path/to/DemoProject.csproj
```

Packages will be created in a `packages/` folder at your solution root.

## CLI Options

| Option                   | Description                                                               |
|--------------------------|---------------------------------------------------------------------------|
| `--shared <path.shproj>` | Package a specific shared project (direct mode)                           |
| `--demo <path.csproj>`   | Package all shared projects referenced by demo (auto mode)                |
| `--output <dir>`         | Output directory for .nupkg files (required for `--shared`)               |
| `--dependency <Id:Ver>`  | Add package dependency (repeatable)                                       |
| `--force`                | Force rebuild even if package already exists                              |
| `--auto-bump`            | Automatically increment the build/revision version (e.g., 1.0.0 -> 1.0.1) |
| `--dry-run`              | Preview what would be packaged without creating files                     |

## MSBuild Integration (Automatic Packaging)

When you install the `Malforge.Mdk.SharedNuGet` package in your demo project, it automatically packages all referenced
shared projects on Release builds.

### Configuration Properties

Add these properties to your demo project's `.csproj` file to control the packager:

```xml
<PropertyGroup>
  <!-- Auto-bump version on every Release build -->
  <MdkAutoBumpVersion>true</MdkAutoBumpVersion>
  
  <!-- Force rebuild packages even if they already exist -->
  <MdkForceRebuild>true</MdkForceRebuild>
  
  <!-- Disable automatic packaging completely -->
  <MdkDisableAutoPackage>true</MdkDisableAutoPackage>
</PropertyGroup>
```

### How It Works

1. Add the package reference to your demo project:
   ```xml
   <PackageReference Include="Malforge.Mdk.SharedNuGet" Version="1.0.0">
     <PrivateAssets>all</PrivateAssets>
   </PackageReference>
   ```

2. Build your project in Release configuration:
   ```bash
   dotnet build -c Release
   ```

3. Packages are automatically created in `YourSolution/packages/` folder

### Platform Support

The packager includes native executables for:

- ✅ **Windows** (win-x64)
- ✅ **Linux** (linux-x64)

The correct executable is automatically selected based on your OS.

## What Gets Packaged?

### ✅ Included in Package:

- All `.cs` source files
- `.shproj` and `.projitems` files
- Other project files (`.xml`, `.resx`, etc.)
- `readme.md` (placed at package root for NuGet display)

### ❌ Excluded from Package:

- Metadata files (`_version`, `_description`, `_authors`, etc.)
- Build artifacts (`bin/`, `obj/` folders)

## Using Generated Packages

1. **Add the package to your project** via NuGet Package Manager or CLI:
   ```bash
   dotnet add package YourPackageName
   ```

2. **The shared files are automatically included** in your project's compilation through MSBuild integration

3. **Files appear in your IDE** under a `Shared/` folder in Solution Explorer

## Metadata File Details

### Required Files

- **`_version`**: Semantic version (e.g., `1.0.0`, `2.1.3-beta`)
- **`_description`**: Brief package description (one line)
- **`_authors`**: Package authors, one per line

### Optional Files

- **`_targetframework`**: If not specified, defaults to `any` in direct mode, or matches the demo project's target
  framework in demo-driven mode
- **`_license`**: SPDX license identifier or expression (e.g., `MIT`, `Apache-2.0`, `GPL-3.0`)
- **`_projecturl`**: Link to your project's homepage or repository
- **`_tags`**: Keywords for package discovery, one per line
- **`readme.md`**: Markdown documentation shown in NuGet Package Manager

## Examples

### Example 1: Simple Shared Library

```
MyUtilities/
├── MyUtilities.shproj
├── _version              → 1.0.0
├── _description          → Common utility functions
├── _authors              → John Doe
├── StringHelpers.cs
└── MathHelpers.cs
```

Package it:

```bash
mdk2-nuget --shared MyUtilities/MyUtilities.shproj --output packages/
```

### Example 2: With Dependencies

```
AdvancedLibrary/
├── AdvancedLibrary.shproj
├── _version              → 2.0.0
├── _description          → Advanced features building on MyUtilities
├── _authors              → Jane Smith
├── _license              → MIT
├── _projecturl           → https://github.com/example/AdvancedLibrary
├── readme.md
└── AdvancedFeatures.cs
```

Package with dependency:

```bash
mdk2-nuget --shared AdvancedLibrary/AdvancedLibrary.shproj \
           --dependency MyUtilities:1.0.0 \
           --output packages/
```

## Troubleshooting

### "Missing required metadata file"

- Ensure all required files (`_version`, `_description`, `_authors`) exist in the same directory as your `.shproj` file
- Files must not be empty

### "Package already exists"

- Use `--force` to rebuild existing packages
- Or use `--auto-bump` to automatically create a new version
- Or manually delete the old package from the output directory

### "Version format not supported for auto-bump"

- Auto-bump requires numeric version components (e.g., `1.0.0`, `2.1.5.3`)
- Pre-release versions with labels (e.g., `1.0.0-beta`) are not supported for auto-bump

### Files not appearing in package

- Check the build output - it lists all included and excluded files
- Ensure files aren't in `bin/` or `obj/` directories
- Metadata files (starting with `_`) are intentionally excluded

## Support

For issues, feature requests, or contributions, please visit the project repository.

---

**Happy packaging!** 📦
