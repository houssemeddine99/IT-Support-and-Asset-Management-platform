param(
    [Parameter(Mandatory=$true)][string]$ConnectionString,
    [string]$MigrationDirectory = (Join-Path $PSScriptRoot '..\database')
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data
$files = Get-ChildItem -LiteralPath $MigrationDirectory -Filter '*.sql' -File | Sort-Object Name
if ($files.Count -eq 0) { throw 'No SQL migrations were found.' }
$connection = [System.Data.SqlClient.SqlConnection]::new($ConnectionString)
try {
    $connection.Open()
    foreach ($file in $files) {
        Write-Host "Applying $($file.Name)..."
        $command = $connection.CreateCommand()
        $command.CommandTimeout = 600
        $command.CommandText = [IO.File]::ReadAllText($file.FullName)
        $command.ExecuteNonQuery() | Out-Null
    }
    Write-Host 'All migrations completed successfully.'
} finally {
    $connection.Dispose()
}
