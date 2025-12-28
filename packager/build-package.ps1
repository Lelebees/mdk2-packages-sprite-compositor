# Build for multiple platforms and create NuGet package

Write-Host "Building Mdk.SharedNuGet for multiple platforms..." -ForegroundColor Cyan

# Build for Windows
Write-Host "`nBuilding for Windows (win-x64)..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true

# Build for Linux
Write-Host "`nBuilding for Linux (linux-x64)..." -ForegroundColor Yellow
dotnet publish -c Release -r linux-x64 --self-contained false -p:PublishSingleFile=true

# Create NuGet package
Write-Host "`nCreating NuGet package..." -ForegroundColor Yellow
dotnet pack -c Release --no-build -o .

Write-Host "`nDone! Package created in current directory." -ForegroundColor Green
