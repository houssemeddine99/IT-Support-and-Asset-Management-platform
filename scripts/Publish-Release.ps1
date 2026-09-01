param([string]$OutputDirectory = (Join-Path $PSScriptRoot '..\artifacts\release'))
$ErrorActionPreference = 'Stop'
$project = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\src\ITSupportAssetManagement.Web\ITSupportAssetManagement.Web.vbproj'))
$publishPath = [IO.Path]::GetFullPath($OutputDirectory)
[IO.Directory]::CreateDirectory($publishPath) | Out-Null
$packagePath = Join-Path $publishPath 'ITSupportAssetManagement.Web.zip'
$msbuild = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe'
if (-not (Test-Path -LiteralPath $msbuild)) { throw 'MSBuild was not found. Install Visual Studio with ASP.NET and web development.' }
& $msbuild $project /t:Rebuild,Package /p:Configuration=Release /p:WebPublishMethod=Package "/p:PackageLocation=$packagePath"
if ($LASTEXITCODE -ne 0) { throw "Release publish failed with exit code $LASTEXITCODE." }
if (-not (Test-Path -LiteralPath $packagePath)) { throw 'MSBuild completed but the deployment package was not created.' }
Write-Host "Release package created: $packagePath"
