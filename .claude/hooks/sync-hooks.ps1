# Maintenance script, not a hook: copies this folder's scripts into another project's .claude/hooks and records
# the commit they came from. This template is the only place where the hooks have tests, so a change is made
# here and synced out; hand copies drift.
# Usage: pwsh -File sync-hooks.ps1 -Target <project root> [-Source <hooks folder>]
#   -Source: the folder to copy from; this script's own folder by default, another one in the tests.
# Prints {"source":<commit>,"copied":[name]}; exits 1 when the source scripts have uncommitted changes (the
# recorded commit would not describe the copies), 2 when the target or the source cannot be used.
# The target's .claude/settings.json is not touched: a new script still needs its registration or allowlist entry.

param(
    [Parameter(Mandatory)] [string]$Target,
    [string]$Source = $PSScriptRoot
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $Target -PathType Container)) {
    [Console]::Error.WriteLine("Target '$Target' is not a folder.")
    exit 2
}

$commit = & git -C $Source rev-parse HEAD 2>$null
if ($LASTEXITCODE -ne 0) {
    [Console]::Error.WriteLine("Source '$Source' is not inside a git repository with a commit.")
    exit 2
}

$dirty = & git -C $Source status --porcelain -- .
if ($dirty) {
    [Console]::Error.WriteLine("Source '$Source' has uncommitted changes; commit them first:`n$($dirty -join "`n")")
    exit 1
}

$destination = Join-Path $Target '.claude/hooks'
New-Item -ItemType Directory -Force -Path $destination | Out-Null

$copied = [Collections.Generic.List[string]]::new()
foreach ($script in (Get-ChildItem -LiteralPath $Source -Filter '*.ps1' -File | Sort-Object Name)) {
    Copy-Item -LiteralPath $script.FullName -Destination (Join-Path $destination $script.Name) -Force
    $copied.Add($script.Name)
}

# One line, so a later sync shows up as a one-line diff in the target's commit.
Set-Content -LiteralPath (Join-Path $destination 'SOURCE') -Value "$commit" -NoNewline:$false

[ordered]@{ source = "$commit"; copied = @($copied) } | ConvertTo-Json -Compress
exit 0
