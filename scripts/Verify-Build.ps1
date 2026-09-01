param([switch]$SkipAspNetPrecompile)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$solution = Join-Path $repoRoot 'ITSupportAssetManagement.sln'
$project = Join-Path $repoRoot 'src\ITSupportAssetManagement.Web'
$candidates = @(
    'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe',
    'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe',
    'C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe'
)
$msbuild = $candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
if (-not $msbuild) { throw 'MSBuild was not found. Install Visual Studio with ASP.NET and web development.' }
& $msbuild $solution /t:Build /p:Configuration=Debug /m
if ($LASTEXITCODE -ne 0) { throw "Solution build failed with exit code $LASTEXITCODE." }
if (-not $SkipAspNetPrecompile) {
    $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler.exe'
    if (-not (Test-Path -LiteralPath $compiler)) { throw 'ASP.NET compiler was not found.' }
    $target = Join-Path ([IO.Path]::GetTempPath()) ('SilianaIT-precompile-' + [Guid]::NewGuid().ToString('N'))
    try {
        & $compiler -v / -p $project $target -f
        if ($LASTEXITCODE -ne 0) { throw "ASP.NET precompilation failed with exit code $LASTEXITCODE." }
    } finally {
        $tempRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
        $resolvedTarget = [IO.Path]::GetFullPath($target)
        if (-not $resolvedTarget.StartsWith($tempRoot, [StringComparison]::OrdinalIgnoreCase)) { throw 'Refusing to clean a precompile path outside the temporary directory.' }
        if (Test-Path -LiteralPath $resolvedTarget) { Remove-Item -LiteralPath $resolvedTarget -Recurse -Force }
    }
}
Write-Host 'Verification passed: solution build and ASP.NET page compilation are clean.'
