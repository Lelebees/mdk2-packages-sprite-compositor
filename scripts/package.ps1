# Package Shared Projects into NuGet Packages

# Define paths
$repoRoot   = Join-Path $PSScriptRoot ".."
$sourcePath = Join-Path $repoRoot "Source"
$outputPath = Join-Path $repoRoot "NuGetPackages"

# Ensure the output folder exists
if (-not (Test-Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath | Out-Null
}

# Enumerate all potential package folders, skipping "Template"
$packageDirs = Get-ChildItem -Path $sourcePath -Directory | Where-Object {
    $_.Name -ne "Template"
}

foreach ($pkg in $packageDirs) {
    Write-Host "Processing package: $($pkg.Name)"

    $packageFolder = Join-Path $pkg.FullName "Mixin"
    if (-not (Test-Path $packageFolder)) {
        Write-Error "Skipping $($pkg.Name): No 'Mixin' folder found."
        continue
    }

    # Expected files
    $shprojFile       = Get-ChildItem -Path $packageFolder -Filter "*.shproj"    | Select-Object -First 1
    $projItemsFile    = Get-ChildItem -Path $packageFolder -Filter "*.projitems" | Select-Object -First 1
    $versionFile      = Join-Path $packageFolder "PackageVersion.txt"
    $releaseNotesFile = Join-Path $packageFolder "ReleaseNotes.txt"
    $readmeFile       = Join-Path $packageFolder "ReadMe.md"
    $summaryFile      = Join-Path $packageFolder "Summary.txt"
    $licenseFile      = Join-Path $packageFolder "LICENSE"

    # Validate required files
    if (-not $shprojFile)       { Write-Error "Missing .shproj in $($pkg.Name). Skipping..."; continue }
    if (-not $projItemsFile)    { Write-Error "Missing .projitems in $($pkg.Name). Skipping..."; continue }
    if (-not (Test-Path $versionFile))      { Write-Error "Missing PackageVersion.txt in $($pkg.Name). Skipping..."; continue }
    if (-not (Test-Path $releaseNotesFile)) { Write-Error "Missing ReleaseNotes.txt in $($pkg.Name). Skipping..."; continue }
    if (-not (Test-Path $readmeFile))       { Write-Error "Missing ReadMe.md in $($pkg.Name). Skipping..."; continue }
    if (-not (Test-Path $summaryFile))      { Write-Error "Missing Summary.txt in $($pkg.Name). Skipping..."; continue }

    # Read metadata
    $packageVersion = (Get-Content $versionFile -Raw).Trim()
    $releaseNotes   = Get-Content $releaseNotesFile -Raw
    $summary        = (Get-Content $summaryFile -Raw).Trim()
    $packageId      = $pkg.Name

    # Determine license approach
    if (Test-Path $licenseFile) {
        $licenseType   = "file"
        $licenseTarget = "LICENSE"
    }
    else {
        $licenseType   = "expression"
        $licenseTarget = "MIT"
    }

    # Project URL
    $packageUrl = "https://github.com/malforge/mdk2-packages/Source/$packageId"

    # Create folder for generated props
    $nugetBuildFolder = Join-Path $packageFolder "nuget_build"
    if (-not (Test-Path $nugetBuildFolder)) {
        New-Item -ItemType Directory -Path $nugetBuildFolder | Out-Null
    }
    
    $projFileName = [System.IO.Path]::GetFileName($projItemsFile.Name)
    # Generate props file for automatic reference
    $propsContent = @"
<?xml version="1.0" encoding="utf-8"?>
<Project>
    <Import Project="`$(MSBuildThisFileDirectory)..\$projFileName" Label="Shared" />
</Project>
"@

    $propsFileName  = "$packageId.props"
    $propsFilePath  = Join-Path $nugetBuildFolder $propsFileName
    $propsContent | Out-File -FilePath $propsFilePath -Encoding utf8

    Write-Host "Generated $propsFileName for $packageId"

    # Generate .nuspec
    $nuspecContent = @"
<?xml version="1.0"?>
<package>
  <metadata>
    <id>$packageId</id>
    <version>$packageVersion</version>
    <authors>Your Name or Organization</authors>
    <owners>Your Name or Organization</owners>
    <license type="$licenseType">$licenseTarget</license>
    <projectUrl>$packageUrl</projectUrl>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <description>$summary</description>
    <releaseNotes><![CDATA[
$releaseNotes
    ]]></releaseNotes>
    <readme>ReadMe.md</readme>
  </metadata>
  <files>
    <file src="nuget_build\*.props" target="build" />
    <file src="**\*.*" exclude="nuget_build\*.props" target="" />
  </files>
</package>
"@

    $nuspecPath = Join-Path $packageFolder "$packageId.nuspec"
    $nuspecContent | Out-File -FilePath $nuspecPath -Encoding utf8

    Write-Host "Generated $($packageId).nuspec"

    # Pack using nuget.exe (assumes nuget.exe is in PATH)
    Write-Host "Packing $packageId..."
    nuget pack $nuspecPath -OutputDirectory $outputPath
}
