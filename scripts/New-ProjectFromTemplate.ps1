<#
.SYNOPSIS
    Rebrands this BaseTemplate solution (backend + frontend) into a new project.

.DESCRIPTION
    Replaces every "BaseTemplate" / "basetemplate" / "base-template" token found
    across source, config, and project files with the new name you supply, and
    renames matching files and folders (the .sln, every .csproj, and each
    project directory under backend/src and backend/tests).

    Also regenerates the API project's <UserSecretsId>, so a project cloned
    from this template doesn't share a user-secrets store with the template
    itself or with other projects generated from it.

    This script intentionally does NOT touch the "Powered by MagdyTech
    Solutions" footer (AppFooterComponent) or its logo — that attribution
    stays fixed across every project generated from this template.

.PARAMETER NewName
    The new project name in PascalCase, e.g. "Acme" or "AcmeShop".
    Used to derive every generated name:
      PascalCase   Acme.Domain, Acme.Application, Acme.sln, AcmeDb, ...
      lowercase    acme-sqlserver, acme-api (docker-compose container names)
      kebab-case   acme (npm package name / Angular project name)

.PARAMETER Path
    Repository root to operate on. Defaults to the parent of this script's
    folder (i.e. run it from scripts/ inside a checkout of this template).

.PARAMETER WhatIf
    Preview every file edit and rename without changing anything on disk.

.EXAMPLE
    pwsh ./scripts/New-ProjectFromTemplate.ps1 -NewName Acme

.EXAMPLE
    pwsh ./scripts/New-ProjectFromTemplate.ps1 -NewName AcmeShop -WhatIf

.NOTES
    Run this once, right after cloning the template for a new project, before
    you've made any other changes — it rewrites a large number of files.
    Commit the template as-is first if you want an easy diff to review after.
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[A-Z][A-Za-z0-9]*$', ErrorMessage = 'NewName must be PascalCase: start with an uppercase letter, letters and digits only (e.g. Acme, AcmeShop).')]
    [string]$NewName,

    [string]$Path = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = 'Stop'

# --- derive naming variants from -NewName, mirroring how the template's own
#     "BaseTemplate" / "basetemplate" / "base-template" variants were used ---
$Pascal = $NewName
$Lower = $NewName.ToLowerInvariant()
$Kebab = ([regex]::Replace($NewName, '(?<!^)(?=[A-Z])', '-')).ToLowerInvariant()

$OldPascal = 'BaseTemplate'
$OldLower = 'basetemplate'
$OldKebab = 'base-template'

$repoRoot = (Resolve-Path -LiteralPath $Path).Path

Write-Host "Rebranding template at: $repoRoot" -ForegroundColor Cyan
Write-Host "  $OldPascal -> $Pascal"
Write-Host "  $OldLower  -> $Lower"
Write-Host "  $OldKebab  -> $Kebab"
Write-Host ''

# Directories we never descend into.
$excludeDirNames = @('.git', 'node_modules', 'bin', 'obj', 'dist', '.angular', '.vs', '.idea')

# Extensions we never open as text (renamed if their name matches, but content is left alone).
$binaryExtensions = @('.ico', '.png', '.jpg', '.jpeg', '.gif', '.bmp', '.webp', '.woff', '.woff2', '.ttf', '.eot', '.pdf', '.zip')

function Test-ExcludedPath {
    param([Parameter(Mandatory)][string]$FullName)
    $relative = $FullName.Substring($repoRoot.Length).TrimStart([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar)
    $segments = $relative -split '[\\/]'
    foreach ($segment in $segments) {
        if ($excludeDirNames -contains $segment) { return $true }
    }
    return $false
}

# --- 1) Replace tokens inside file contents ---
$allFiles = Get-ChildItem -LiteralPath $repoRoot -Recurse -File -Force |
    Where-Object { -not (Test-ExcludedPath $_.FullName) -and ($binaryExtensions -notcontains $_.Extension.ToLowerInvariant()) }

$changedFiles = 0
foreach ($file in $allFiles) {
    $content = Get-Content -LiteralPath $file.FullName -Raw -ErrorAction SilentlyContinue
    if ($null -eq $content) { continue }
    if ($content -notmatch $OldPascal -and $content -notmatch $OldLower -and $content -notmatch $OldKebab) {
        continue
    }

    $updated = $content -creplace $OldPascal, $Pascal -creplace $OldLower, $Lower -creplace $OldKebab, $Kebab

    if ($updated -ne $content) {
        if ($PSCmdlet.ShouldProcess($file.FullName, 'Replace BaseTemplate tokens')) {
            Set-Content -LiteralPath $file.FullName -Value $updated -NoNewline
        }
        $changedFiles++
    }
}
Write-Host "Updated content in $changedFiles file(s)." -ForegroundColor Green

# --- 2) Rename files/folders whose name contains the old token.
#        Deepest paths first, so renaming a parent folder doesn't invalidate
#        the FullName we already captured for its children. ---
$itemsToRename = Get-ChildItem -LiteralPath $repoRoot -Recurse -Force |
    Where-Object { -not (Test-ExcludedPath $_.FullName) -and $_.Name -cmatch $OldPascal } |
    Sort-Object { $_.FullName.Length } -Descending

$renamedItems = 0
foreach ($item in $itemsToRename) {
    # NOTE: deliberately not named $newName — PowerShell validation attributes
    # on a script parameter re-trigger on ANY assignment to that variable name,
    # and variable names are case-insensitive, so $newName would collide with
    # the -NewName parameter's [ValidatePattern] and fail on values like
    # "Acme.Infrastructure.csproj".
    $renamedTo = $item.Name -creplace $OldPascal, $Pascal
    if ($renamedTo -ne $item.Name) {
        if ($PSCmdlet.ShouldProcess($item.FullName, "Rename to $renamedTo")) {
            Rename-Item -LiteralPath $item.FullName -NewName $renamedTo
        }
        $renamedItems++
    }
}
Write-Host "Renamed $renamedItems file/folder name(s)." -ForegroundColor Green

# --- 3) Regenerate the API project's UserSecretsId ---
$apiCsproj = Get-ChildItem -LiteralPath $repoRoot -Recurse -Filter "$Pascal.API.csproj" -File | Select-Object -First 1
if ($apiCsproj) {
    $content = Get-Content -LiteralPath $apiCsproj.FullName -Raw
    $newGuid = [guid]::NewGuid().ToString()
    $updated = [regex]::Replace($content, '<UserSecretsId>.*?</UserSecretsId>', "<UserSecretsId>$newGuid</UserSecretsId>")
    if ($updated -ne $content) {
        if ($PSCmdlet.ShouldProcess($apiCsproj.FullName, 'Regenerate UserSecretsId')) {
            Set-Content -LiteralPath $apiCsproj.FullName -Value $updated -NoNewline
        }
        Write-Host "Regenerated UserSecretsId: $newGuid" -ForegroundColor Green
    }
}
else {
    Write-Warning "Could not find $Pascal.API.csproj to regenerate UserSecretsId (already renamed, or -WhatIf was used)."
}

Write-Host ''
Write-Host 'Done. Next steps:' -ForegroundColor Cyan
Write-Host "  cd backend && dotnet restore && dotnet build && dotnet test"
Write-Host "  cd frontend && npm install && npm run build"
Write-Host '  Review README.md, appsettings*.json, and docker-compose.yml for anything else project-specific (DB password, JWT signing key, CORS origins).'
Write-Host '  The "Powered by MagdyTech Solutions" footer was left untouched on purpose.'
