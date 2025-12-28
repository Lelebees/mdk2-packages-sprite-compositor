# MDK2 NuGet Package Generator - Requirements

## Overview
A tool that generates NuGet packages for .NET Shared Projects (.shproj). Can work standalone or integrate with demo projects via MSBuild.

## Design Decisions

### Tool Format
- Standalone CLI executable for manual invocation
- MSBuild targets for automatic packaging during builds
  - Packaging target runs after successful compilation (depends on Build target)
  - If compilation fails, packaging does not run
- Distributable as a NuGet package

### Project Structure & Modes
**Demo-driven mode:**
- Demo project references the packager as a NuGet package
- Tool automatically detects all directly-referenced shared projects and packages each one on build
- Automatically detects shared project NuGet package references as dependencies
- No manual configuration required
- Default output: `packages/` folder at solution root (creates if doesn't exist)

**Direct mode:**
- Manually specify shared project path via CLI
- Manually specify dependencies via CLI arguments
- Output location must be specified via `--output`
- No validation - user responsible for ensuring shared project is valid

### Metadata
- **Package ID**: Derived from shared project name (`.shproj` filename)
- **Metadata files**: Simple text files with underscore prefix and no extension in shared project directory
  - `_version` - Required. Package version (e.g., `1.0.0`)
  - `_description` - Required. Package description
  - `_authors` - Required. One author per line
  - `_targetframework` - Optional. Target framework moniker (e.g., `net8.0`, `netstandard2.0`). If not specified, defaults to demo project's TFM in demo-driven mode, or `any` in direct mode.
  - `_license` - Optional. License identifier or URL
  - `_projecturl` - Optional. Project/repository URL
  - `_tags` - Optional. One tag per line
- **Format**: Plain text, UTF-8 encoding
- **Missing required files**: Tool errors with clear instructions on which files to create and where

### Versioning
- Manual only (user edits `version.txt`)

### CLI Interface
**Demo-driven:**
```
--demo <path.csproj>     Analyze demo and package all referenced shared projects
--output <dir>           Output directory for .nupkg files
```

**Direct:**
```
--shared <path.shproj>   Package specific shared project
--dependency <Id:Ver>    Specify dependency (repeatable)
--output <dir>           Output directory for .nupkg
```

**Common:**
```
--force                  Force rebuild
--dry-run               Preview without creating files
```

## Example Structure
```
MyCoroutinesLibrary/
├── MyCoroutinesLibrary.sln
├── Coroutines.Shared/
│   ├── Coroutines.shproj
│   ├── _version           (1.0.0)
│   ├── _description       (Coroutine support for C#)
│   ├── _authors           (Malforge)
│   └── ... source files ...
├── CoroutinesDemo/
│   ├── CoroutinesDemo.csproj  (references Coroutines.Shared + packager NuGet)
│   └── Program.cs
└── packages/
    └── Coroutines.Shared.1.0.0.nupkg  (generated on build)
```

## Usage Examples

**Automatic (MSBuild integration):**
- Install the packager NuGet package in your demo project
- Build the demo project
- Shared project packages are automatically generated

**Manual (demo-driven):**
```
mdk2-nuget --demo CoroutinesDemo/CoroutinesDemo.csproj --output packages/
```

**Manual (direct):**
```
mdk2-nuget --shared Coroutines.Shared/Coroutines.shproj --output packages/
mdk2-nuget --shared StateManagement.Shared/StateManagement.shproj --dependency Coroutines.Shared:1.0.0 --output packages/
```

## Implementation Progress

### Core Features
- [x] CLI argument parsing (--demo, --shared, --output, --dependency, --force, --dry-run, --auto-bump)
- [x] Shared project metadata validation
  - [x] Read and validate required files (_version, _description, _authors)
  - [x] Read optional files (_targetframework, _license, _projecturl, _tags)
  - [x] Read optional readme.md
  - [x] Comprehensive error reporting (all issues at once)
- [x] NuGet package generation
  - [x] Create .nuspec file from metadata
  - [x] Package shared project files
  - [x] Include .shproj and source files
  - [x] Generate .props/.targets for MSBuild integration
  - [x] Create .nupkg file with proper NuGet format
  - [x] Include readme in package
- [x] Demo-driven mode
  - [x] Parse .csproj to find shared project references
  - [x] Detect shared project NuGet package dependencies (ignore regular NuGet packages)
  - [x] Find solution root for default output location
  - [x] Package all referenced shared projects
- [x] Direct mode dependency handling
  - [x] Parse --dependency arguments
  - [x] Add to package metadata
- [x] Dry-run mode implementation
- [x] Force rebuild logic
- [x] Auto-bump version feature

### MSBuild Integration (Future)
- [ ] Create MSBuild .targets file
- [ ] Hook into build pipeline (AfterBuild target)
- [ ] Package as NuGet tool package
- [x] Create readme.md for packager NuGet explaining setup
- [ ] Test with real demo projects

### Testing
- [ ] Create test shared projects
- [ ] Test all CLI options
- [ ] Test error cases
- [ ] Integration tests with real projects
