# Manual package build script
# Builds NuGet packages for shared projects in the packages/ directory

param(
    [Parameter(Mandatory=$false)]
    [string]$PackageName,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath = ".\package-output",
    
    [Parameter(Mandatory=$false)]
    [switch]$UseLocalPackager,
    
    [Parameter(Mandatory=$false)]
    [switch]$Help
)

if ($Help) {
    Write-Host @"
Usage: .\build-packages.ps1 [-PackageName <name>] [-OutputPath <path>] [-UseLocalPackager]

Parameters:
  -PackageName      Build only the specified package (folder name in packages/)
                    If not specified, builds all packages
  
  -OutputPath       Output directory for built packages (default: .\package-output)
  
  -UseLocalPackager Use locally built packager instead of the global tool
                    (requires packager to be built first)
  
  -Help             Show this help message

Examples:
  .\build-packages.ps1
  .\build-packages.ps1 -PackageName Mal.IngameScript.Coroutines
  .\build-packages.ps1 -UseLocalPackager
"@
    exit 0
}

$ErrorActionPreference = "Stop"

Write-Host "=== MDK2 Package Builder ===" -ForegroundColor Cyan
Write-Host ""

# Create output directory
if (-not (Test-Path $OutputPath)) {
    New-Item -Path $OutputPath -ItemType Directory -Force | Out-Null
    Write-Host "Created output directory: $OutputPath" -ForegroundColor Green
}

# Determine which packages to build
$packagesToBuild = @()
if ($PackageName) {
    $packagePath = Join-Path "packages" $PackageName
    if (-not (Test-Path $packagePath)) {
        Write-Error "Package '$PackageName' not found at: $packagePath"
        exit 1
    }
    $packagesToBuild += $PackageName
    Write-Host "Building single package: $PackageName" -ForegroundColor Yellow
} else {
    $packageDirs = Get-ChildItem -Path "packages" -Directory | Where-Object { $_.Name -ne ".idea" }
    $packagesToBuild = $packageDirs | ForEach-Object { $_.Name }
    Write-Host "Building all packages: $($packagesToBuild -join ', ')" -ForegroundColor Yellow
}

Write-Host ""

# Determine packager executable
if ($UseLocalPackager) {
    $packagerExe = "packager\Mal.Mdk2.SharedNugetBuilder\bin\Release\net8.0\Mal.Mdk2.SharedNugetBuilder.exe"
    
    if (-not (Test-Path $packagerExe)) {
        Write-Host "Local packager not found. Building it first..." -ForegroundColor Yellow
        Push-Location packager\Mal.Mdk2.SharedNugetBuilder
        try {
            dotnet build -c Release
            if ($LASTEXITCODE -ne 0) {
                throw "Failed to build packager"
            }
        } finally {
            Pop-Location
        }
    }
    
    Write-Host "Using local packager: $packagerExe" -ForegroundColor Cyan
} else {
    # Check if global tool is installed
    $toolList = dotnet tool list --global | Select-String "mal.mdk2.sharednugetbuilder"
    if (-not $toolList) {
        Write-Error @"
Packager tool 'Mal.Mdk2.SharedNugetBuilder' is not installed as a global tool.

Install it with:
  dotnet tool install --global Mal.Mdk2.SharedNugetBuilder --add-source https://nuget.pkg.github.com/malforge/index.json

Or use the -UseLocalPackager flag to build and use the local version.
"@
        exit 1
    }
    
    $packagerExe = "mdk2-sharednuget"
    Write-Host "Using global tool: $packagerExe" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "=== Building Packages ===" -ForegroundColor Cyan
Write-Host ""

$successCount = 0
$failCount = 0
$results = @()

foreach ($package in $packagesToBuild) {
    Write-Host "Processing: $package" -ForegroundColor Yellow
    
    # Find .shproj file
    $shprojFile = Get-ChildItem -Path "packages\$package" -Recurse -Filter "*.shproj" | Select-Object -First 1
    
    if (-not $shprojFile) {
        Write-Host "  [SKIP] No .shproj file found" -ForegroundColor DarkYellow
        $results += [PSCustomObject]@{
            Package = $package
            Status = "SKIPPED"
            Reason = "No .shproj file found"
        }
        continue
    }
    
    Write-Host "  Found: $($shprojFile.Name)" -ForegroundColor Gray
    
    # Run packager
    try {
        if ($UseLocalPackager) {
            & $packagerExe --shared $shprojFile.FullName --output $OutputPath
        } else {
            & $packagerExe --shared $shprojFile.FullName --output $OutputPath
        }
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  [SUCCESS]" -ForegroundColor Green
            $successCount++
            $results += [PSCustomObject]@{
                Package = $package
                Status = "SUCCESS"
                Reason = ""
            }
        } else {
            Write-Host "  [FAILED] Exit code: $LASTEXITCODE" -ForegroundColor Red
            $failCount++
            $results += [PSCustomObject]@{
                Package = $package
                Status = "FAILED"
                Reason = "Exit code: $LASTEXITCODE"
            }
        }
    } catch {
        Write-Host "  [FAILED] $($_.Exception.Message)" -ForegroundColor Red
        $failCount++
        $results += [PSCustomObject]@{
            Package = $package
            Status = "FAILED"
            Reason = $_.Exception.Message
        }
    }
    
    Write-Host ""
}

# Summary
Write-Host "=== Build Summary ===" -ForegroundColor Cyan
Write-Host ""
$results | Format-Table -AutoSize
Write-Host ""
Write-Host "Total: $($packagesToBuild.Count) | Success: $successCount | Failed: $failCount" -ForegroundColor $(if ($failCount -eq 0) { "Green" } else { "Yellow" })
Write-Host ""

if ($successCount -gt 0) {
    Write-Host "Output packages are in: $OutputPath" -ForegroundColor Green
    $nupkgs = Get-ChildItem -Path $OutputPath -Filter "*.nupkg"
    if ($nupkgs) {
        Write-Host "Packages created:" -ForegroundColor Cyan
        $nupkgs | ForEach-Object { Write-Host "  $($_.Name)" -ForegroundColor Gray }
    }
}

if ($failCount -gt 0) {
    exit 1
}
