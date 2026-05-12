param(
    [string]$HostName = "localhost",
    [int]$Port = 5432,
    [string]$Database = "inventory_db",
    [string]$Username = "postgres",
    [string]$Password = "",
    [switch]$SeedData
)

$ErrorActionPreference = "Stop"

if (-not (Get-Command psql -ErrorAction SilentlyContinue)) {
    throw "No se encontró 'psql' en el PATH. Instala PostgreSQL client tools o agrega psql al PATH."
}

if ([string]::IsNullOrWhiteSpace($Password)) {
    $Password = $env:DB_PASSWORD
}

if ([string]::IsNullOrWhiteSpace($Password)) {
    throw "Debes indicar -Password o definir DB_PASSWORD en el entorno."
}

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$sqlFile = Join-Path $scriptPath "init-db.sql"

if (-not (Test-Path $sqlFile)) {
    throw "No se encontró el archivo SQL en: $sqlFile"
}

$env:PGPASSWORD = $Password

try {
    & psql -v ON_ERROR_STOP=1 -h $HostName -p $Port -U $Username -d $Database -f $sqlFile

    if ($SeedData.IsPresent) {
        $seedSql = @"
INSERT INTO productos (id, nombre, cantidad)
VALUES
  (1, 'Teclado', 10),
  (2, 'Mouse', 15),
  (3, 'Monitor', 5)
ON CONFLICT (id) DO NOTHING;
"@
        & psql -v ON_ERROR_STOP=1 -h $HostName -p $Port -U $Username -d $Database -c $seedSql
    }

    Write-Host "Inicialización completada correctamente." -ForegroundColor Green
}
finally {
    Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
}
