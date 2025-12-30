# Generate documentation for all libraries
param(
    [string]$LibrariesPath = "libraries",
    [string]$DocsPath = "docs/libraries"
)

$ErrorActionPreference = "Stop"

# Get repository root
$repoRoot = Get-Location

# Shame messages for missing documentation
$readmeShameMessages = @(
    "The author was too busy coding to write a readme. We appreciate the hustle, but... really?",
    "Apparently the author thinks their code is self-documenting. Narrator: It wasn't.",
    "The author has left you to divine the purpose of this library through spiritual meditation and stack traces.",
    "The author believes in the `"sink or swim`" approach to documentation. Good luck!",
    "No readme? The author clearly enjoys living dangerously.",
    "The author forgot that future-them is also a user. Past-them is laughing. Present-them should write a readme.",
    "This package is SO good, the author thought it didn't need documentation. Confidence: 100. Documentation: 0.",
    "The author rolled a natural 1 on their documentation check.",
    "The author subscribes to the `"readme files are for the weak`" philosophy. Bold.",
    "The author aimed for minimalism and achieved mysticism."
)

$releaseNotesShameMessages = @(
    "The author updates packages like a ninja - silently, in the dark, with no witnesses.",
    "Breaking changes? New features? The author wants to keep you guessing! Surprise mechanics!",
    "The author will document changes right after they finish that other thing they've been putting off since 2019.",
    "The author expects you to git diff between versions like some kind of archaeologist.",
    "`"What changed?`" you ask. `"Everything and nothing,`" whispers the author mysteriously.",
    "The author believes version numbers speak for themselves. They do not.",
    "Surprise! The author has made changes. What changes? That's half the fun!",
    "The author treats each version like a mystery box. Will it break your code? Open it and find out!",
    "Version bumps without changelogs: Because the author lives for danger.",
    "The author's release process: Ship it and let users discover the changes through trial and error.",
    "`"Move fast and break things`" - The author, probably, while not writing release notes.",
    "The changelog is stored in the author's brain. Good luck accessing that archive."
)

function Get-RandomShameMessage {
    param([string[]]$Messages)
    return $Messages | Get-Random
}

# Create docs directory if it doesn't exist
if (-not (Test-Path $DocsPath)) {
    New-Item -Path $DocsPath -ItemType Directory | Out-Null
}

$currentDate = Get-Date -Format "yyyy-MM-dd"
$libraries = @()

# Find all libraries by looking for folders with metadata files
Write-Host "Scanning for libraries..." -ForegroundColor Cyan
$libraryFolders = Get-ChildItem -Path $LibrariesPath -Directory | ForEach-Object {
    $parentFolder = $_
    # Look for nested folder with same name (the actual shared project folder)
    $sharedFolder = Get-ChildItem -Path $parentFolder.FullName -Directory | Where-Object {
        $_.Name -eq $parentFolder.Name
    } | Select-Object -First 1
    
    if ($sharedFolder) {
        # Check if it has the required _version file (marker that this is a valid library)
        $versionFile = Join-Path $sharedFolder.FullName "_version"
        if (Test-Path $versionFile) {
            # Also verify it has other required metadata
            $hasAuthors = Test-Path (Join-Path $sharedFolder.FullName "_authors")
            $hasDescription = Test-Path (Join-Path $sharedFolder.FullName "_description")
            
            if ($hasAuthors -and $hasDescription) {
                return $sharedFolder
            } else {
                Write-Warning "Library '$($sharedFolder.Name)' is missing required metadata files (_authors or _description)"
            }
        }
    }
}

Write-Host "Found $($libraryFolders.Count) libraries" -ForegroundColor Green
Write-Host ""

foreach ($projectDir in $libraryFolders) {
    $packageName = $projectDir.Name
    
    Write-Host "Processing: $packageName" -ForegroundColor Yellow
    
    # Read metadata
    $version = (Get-Content (Join-Path $projectDir "_version")).Trim()
    $authors = (Get-Content (Join-Path $projectDir "_authors")) -join ", "
    $description = (Get-Content (Join-Path $projectDir "_description")).Trim()
    
    # Check for optional files (case-insensitive)
    $readmePath = Get-ChildItem -Path $projectDir -File | Where-Object { $_.Name -match "^readme\.md$" } | Select-Object -First 1
    $releaseNotesPath = Get-ChildItem -Path $projectDir -File | Where-Object { $_.Name -match "^_releasenotes$" } | Select-Object -First 1
    
    # Generate main documentation file
    $mainDocPath = Join-Path $DocsPath "$packageName.md"
    $installDocPath = Join-Path $DocsPath "$packageName-Installation.md"
    $releaseNotesDocPath = Join-Path $DocsPath "$packageName-ReleaseNotes.md"
    
    # Build main doc
    $mainDoc = @"
# $packageName

> [!NOTE]
> **Version:** $version  
> **Authors:** $authors  
> **Package:** ``$packageName``
> 
> **Description:** $description
>
> **[Installation Instructions →](./$packageName-Installation.md)** | **[Release Notes →](./$packageName-ReleaseNotes.md)**

---

"@
    
    # Add readme content or shame message
    if ($readmePath) {
        $readmeContent = Get-Content $readmePath.FullName -Raw
        $mainDoc += $readmeContent
    } else {
        $shameMessage = Get-RandomShameMessage -Messages $readmeShameMessages
        $mainDoc += @"
> [!WARNING]
> **Missing Documentation**
> 
> *$shameMessage*  
> \- Malware

"@
    }
    
    $mainDoc += @"

---

*Documentation auto-generated from package metadata. Last updated: $currentDate*
"@
    
    Set-Content -Path $mainDocPath -Value $mainDoc -Encoding UTF8
    Write-Host "  ✓ Generated $packageName.md" -ForegroundColor Green
    
    # Generate installation doc
    $installDoc = @"
# Installing $packageName

**Quick Links:** [Visual Studio](#visual-studio) | [JetBrains Rider](#jetbrains-rider) | [Command Line](#command-line--manual)

---

## Visual Studio

1. Right-click on your project in Solution Explorer
2. Select **Manage NuGet Packages**
3. Click on **Settings** (gear icon)
4. Add a new package source:
   - Name: ``Malforge``
   - Source: ``https://nuget.pkg.github.com/malforge/index.json``
5. Click **OK** to save
6. Search for ``$packageName``
7. Click **Install**

---

## JetBrains Rider

1. Go to **File → Settings → NuGet → Sources**
2. Click **+** to add a new source
   - Name: ``Malforge``
   - URL: ``https://nuget.pkg.github.com/malforge/index.json``
3. Click **OK** to save
4. Open the **NuGet** tool window
5. Search for ``$packageName``
6. Click **Install**

---

## Command Line / Manual

### Add Package Source (one time only)

``````bash
dotnet nuget add source https://nuget.pkg.github.com/malforge/index.json --name Malforge
``````

### Install the Package

``````bash
dotnet add package $packageName
``````

### Or Edit .csproj Directly

Add this to your ``.csproj`` file:

``````xml
<PackageReference Include="$packageName" Version="$version" />
``````

---

*Installation instructions auto-generated. Last updated: $currentDate*
"@
    
    Set-Content -Path $installDocPath -Value $installDoc -Encoding UTF8
    Write-Host "  ✓ Generated $packageName-Installation.md" -ForegroundColor Green
    
    # Generate release notes doc
    if ($releaseNotesPath) {
        $releaseNotesRaw = Get-Content $releaseNotesPath.FullName -Raw
        
        $releaseNotesDoc = @"
# Release Notes - $packageName

````````````````text
$releaseNotesRaw
````````````````

---

*Release notes auto-generated from ``_releasenotes`` file. Last updated: $currentDate*
"@
    } else {
        $shameMessage = Get-RandomShameMessage -Messages $releaseNotesShameMessages
        $releaseNotesDoc = @"
# Release Notes - $packageName

> [!WARNING]
> **Missing Release Notes**
> 
> *$shameMessage*  
> \- Malware

---

*Last updated: $currentDate*
"@
    }
    
    Set-Content -Path $releaseNotesDocPath -Value $releaseNotesDoc -Encoding UTF8
    Write-Host "  ✓ Generated $packageName-ReleaseNotes.md" -ForegroundColor Green
    
    # Add to library list for index
    $libraries += @{
        Name = $packageName
        Version = $version
        Description = $description
        Authors = $authors
    }
    
    Write-Host ""
}

# Generate main README.md
Write-Host "Generating main README.md..." -ForegroundColor Cyan

$templatePath = Join-Path $repoRoot "docs\README.template"
if (-not (Test-Path $templatePath)) {
    Write-Host "ERROR: README template not found at $templatePath" -ForegroundColor Red
    exit 1
}

$template = Get-Content -Path $templatePath -Raw

# Build library table
$libraryTable = ""
foreach ($lib in $libraries | Sort-Object Name) {
    $docPath = "./docs/libraries/$($lib.Name).md"
    $libraryTable += "| [$($lib.Name)]($docPath) | $($lib.Version) | $($lib.Authors) | $($lib.Description) |`n"
}

# Replace placeholders
$readmeContent = $template -replace '{{LIBRARY_TABLE}}', $libraryTable.TrimEnd()
$readmeContent = $readmeContent -replace '{{TIMESTAMP}}', $currentDate

Set-Content -Path "README.md" -Value $readmeContent -Encoding UTF8
Write-Host "✓ Generated README.md" -ForegroundColor Green
Write-Host ""

# Cleanup: Remove documentation for libraries that no longer exist
Write-Host "Cleaning up documentation for removed libraries..." -ForegroundColor Cyan
$validPackageNames = $libraries | ForEach-Object { $_.Name }
$docsFiles = Get-ChildItem -Path $DocsPath -Filter "*.md" -ErrorAction SilentlyContinue

$removedCount = 0
foreach ($docFile in $docsFiles) {
    # Extract package name from doc filename
    $fileName = $docFile.Name
    
    # Check if this is a main doc, installation doc, or release notes doc
    if ($fileName -match '^(.+?)(-Installation|-ReleaseNotes)?\.md$') {
        $packageName = $matches[1]
        
        # If package no longer exists, remove the doc
        if ($packageName -notin $validPackageNames) {
            Remove-Item $docFile.FullName -Force
            Write-Host "  ✗ Removed $fileName (library no longer exists)" -ForegroundColor Red
            $removedCount++
        }
    }
}

if ($removedCount -gt 0) {
    Write-Host "Removed $removedCount documentation file(s)" -ForegroundColor Yellow
} else {
    Write-Host "No cleanup needed" -ForegroundColor Green
}

Write-Host ""
Write-Host "Documentation generation complete!" -ForegroundColor Green
Write-Host "Generated documentation for $($libraries.Count) libraries" -ForegroundColor Cyan
