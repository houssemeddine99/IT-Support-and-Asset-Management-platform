param(
    [string]$ConnectionString = 'Data Source=(localdb)\SilianaIT;Initial Catalog=ITSupportAssetManagement;Integrated Security=True;TrustServerCertificate=True',
    [string]$OutputDirectory = (Join-Path $PSScriptRoot '..\database\backups')
)
$ErrorActionPreference = 'Stop'
$resolvedOutput = [IO.Path]::GetFullPath($OutputDirectory)
[IO.Directory]::CreateDirectory($resolvedOutput) | Out-Null
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$backupPath = Join-Path $resolvedOutput "ITSupportAssetManagement-$stamp.bak"
Add-Type -AssemblyName System.Data
$connection = [System.Data.SqlClient.SqlConnection]::new($ConnectionString)
try {
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandTimeout = 600
    $command.CommandText = 'BACKUP DATABASE [ITSupportAssetManagement] TO DISK = @BackupPath WITH COPY_ONLY, CHECKSUM, INIT; RESTORE VERIFYONLY FROM DISK = @BackupPath WITH CHECKSUM;'
    $command.Parameters.Add('@BackupPath', [System.Data.SqlDbType]::NVarChar, 4000).Value = $backupPath
    $command.ExecuteNonQuery() | Out-Null
    Write-Host "Verified backup created: $backupPath"
} finally {
    $connection.Dispose()
}
