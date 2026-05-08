# FHIR Query Builder Deployment Script
# This script builds and packages the application for deployment

param(
    [Parameter(Mandatory=$false)]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath = ".\publish",
    
    [Parameter(Mandatory=$false)]
    [string]$Runtime = "win-x64",
    
    [Parameter(Mandatory=$false)]
    [switch]$SelfContained = $true,
    
    [Parameter(Mandatory=$false)]
    [switch]$CreateInstaller = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$RunTests = $true
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Get script directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectDir = Split-Path -Parent $ScriptDir
$SolutionDir = Split-Path -Parent $ProjectDir

Write-Host "=== FHIR Query Builder Deployment ===" -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Output Path: $OutputPath" -ForegroundColor Yellow
Write-Host "Runtime: $Runtime" -ForegroundColor Yellow
Write-Host "Self-Contained: $SelfContained" -ForegroundColor Yellow
Write-Host ""

try {
    # Step 1: Clean previous builds
    Write-Host "Step 1: Cleaning previous builds..." -ForegroundColor Cyan
    if (Test-Path $OutputPath) {
        Remove-Item -Path $OutputPath -Recurse -Force
        Write-Host "Cleaned output directory: $OutputPath" -ForegroundColor Green
    }
    
    # Clean bin and obj directories
    Get-ChildItem -Path $ProjectDir -Include "bin", "obj" -Recurse -Directory | Remove-Item -Recurse -Force
    Write-Host "Cleaned bin and obj directories" -ForegroundColor Green
    
    # Step 2: Restore dependencies
    Write-Host "Step 2: Restoring dependencies..." -ForegroundColor Cyan
    Set-Location $ProjectDir
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to restore dependencies"
    }
    Write-Host "Dependencies restored successfully" -ForegroundColor Green
    
    # Step 3: Run tests (if requested)
    if ($RunTests) {
        Write-Host "Step 3: Running tests..." -ForegroundColor Cyan
        Set-Location $SolutionDir
        
        if (Test-Path "FHIRQueryBuilder.Tests") {
            dotnet test "FHIRQueryBuilder.Tests" --configuration $Configuration --logger "console;verbosity=minimal"
            if ($LASTEXITCODE -ne 0) {
                throw "Tests failed"
            }
            Write-Host "All tests passed" -ForegroundColor Green
        } else {
            Write-Host "No test project found, skipping tests" -ForegroundColor Yellow
        }
    } else {
        Write-Host "Step 3: Skipping tests (as requested)" -ForegroundColor Yellow
    }
    
    # Step 4: Build application
    Write-Host "Step 4: Building application..." -ForegroundColor Cyan
    Set-Location $ProjectDir
    
    $buildArgs = @(
        "build"
        "--configuration", $Configuration
        "--no-restore"
        "-p:Platform=x64"
    )
    
    & dotnet @buildArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed"
    }
    Write-Host "Build completed successfully" -ForegroundColor Green
    
    # Step 5: Publish application
    Write-Host "Step 5: Publishing application..." -ForegroundColor Cyan
    
    $publishArgs = @(
        "publish"
        "--configuration", $Configuration
        "--runtime", $Runtime
        "--output", $OutputPath
        "--no-build"
    )
    
    if ($SelfContained) {
        $publishArgs += "--self-contained", "true"
        $publishArgs += "-p:PublishSingleFile=true"
        $publishArgs += "-p:PublishTrimmed=true"
    } else {
        $publishArgs += "--self-contained", "false"
    }
    
    & dotnet @publishArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Publish failed"
    }
    Write-Host "Application published successfully" -ForegroundColor Green
    
    # Step 6: Copy additional files
    Write-Host "Step 6: Copying additional files..." -ForegroundColor Cyan
    
    # Copy configuration files
    $configFiles = @("appsettings.json", "appsettings.Production.json")
    foreach ($configFile in $configFiles) {
        $sourcePath = Join-Path $ProjectDir $configFile
        if (Test-Path $sourcePath) {
            Copy-Item -Path $sourcePath -Destination $OutputPath -Force
            Write-Host "Copied $configFile" -ForegroundColor Green
        }
    }
    
    # Copy README and documentation
    $docFiles = @("README.md", "LICENSE")
    foreach ($docFile in $docFiles) {
        $sourcePath = Join-Path $ProjectDir $docFile
        if (Test-Path $sourcePath) {
            Copy-Item -Path $sourcePath -Destination $OutputPath -Force
            Write-Host "Copied $docFile" -ForegroundColor Green
        }
    }
    
    # Create directories for user data
    $userDirs = @("QueryHistory", "Exports", "ConfigBackups")
    foreach ($dir in $userDirs) {
        $dirPath = Join-Path $OutputPath $dir
        New-Item -Path $dirPath -ItemType Directory -Force | Out-Null
        Write-Host "Created directory: $dir" -ForegroundColor Green
    }
    
    # Step 7: Create installer (if requested)
    if ($CreateInstaller) {
        Write-Host "Step 7: Creating installer..." -ForegroundColor Cyan
        
        # Check if NSIS is available
        $nsisPath = Get-Command "makensis.exe" -ErrorAction SilentlyContinue
        if ($nsisPath) {
            $installerScript = Join-Path $ScriptDir "installer.nsi"
            if (Test-Path $installerScript) {
                & makensis.exe $installerScript
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "Installer created successfully" -ForegroundColor Green
                } else {
                    Write-Host "Failed to create installer" -ForegroundColor Red
                }
            } else {
                Write-Host "Installer script not found: $installerScript" -ForegroundColor Yellow
            }
        } else {
            Write-Host "NSIS not found, skipping installer creation" -ForegroundColor Yellow
            Write-Host "Install NSIS from https://nsis.sourceforge.io/ to create installers" -ForegroundColor Yellow
        }
    } else {
        Write-Host "Step 7: Skipping installer creation" -ForegroundColor Yellow
    }
    
    # Step 8: Generate deployment info
    Write-Host "Step 8: Generating deployment info..." -ForegroundColor Cyan
    
    $deploymentInfo = @{
        BuildDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss UTC"
        Configuration = $Configuration
        Runtime = $Runtime
        SelfContained = $SelfContained
        Version = (Get-Content (Join-Path $ProjectDir "FHIRQueryBuilder.csproj") | Select-String "<Version>" | ForEach-Object { $_.Line -replace ".*<Version>(.*)</Version>.*", '$1' }).Trim()
        Files = (Get-ChildItem -Path $OutputPath -Recurse -File | Measure-Object).Count
        TotalSize = [math]::Round((Get-ChildItem -Path $OutputPath -Recurse -File | Measure-Object -Property Length -Sum).Sum / 1MB, 2)
    }
    
    $deploymentInfo | ConvertTo-Json -Depth 2 | Out-File -FilePath (Join-Path $OutputPath "deployment-info.json") -Encoding UTF8
    Write-Host "Deployment info saved" -ForegroundColor Green
    
    # Step 9: Create ZIP package
    Write-Host "Step 9: Creating ZIP package..." -ForegroundColor Cyan
    
    $zipPath = Join-Path (Split-Path $OutputPath) "FHIRQueryBuilder-$Runtime-$(Get-Date -Format 'yyyyMMdd-HHmmss').zip"
    Compress-Archive -Path "$OutputPath\*" -DestinationPath $zipPath -Force
    Write-Host "ZIP package created: $zipPath" -ForegroundColor Green
    
    # Summary
    Write-Host ""
    Write-Host "=== Deployment Summary ===" -ForegroundColor Green
    Write-Host "Configuration: $Configuration" -ForegroundColor White
    Write-Host "Runtime: $Runtime" -ForegroundColor White
    Write-Host "Output Path: $OutputPath" -ForegroundColor White
    Write-Host "Package: $zipPath" -ForegroundColor White
    Write-Host "Total Files: $($deploymentInfo.Files)" -ForegroundColor White
    Write-Host "Total Size: $($deploymentInfo.TotalSize) MB" -ForegroundColor White
    Write-Host ""
    Write-Host "Deployment completed successfully!" -ForegroundColor Green
    
} catch {
    Write-Host ""
    Write-Host "=== Deployment Failed ===" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Location: $($_.InvocationInfo.ScriptName):$($_.InvocationInfo.ScriptLineNumber)" -ForegroundColor Red
    exit 1
}

# Return to original location
Set-Location $ScriptDir
