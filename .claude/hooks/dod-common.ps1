# Shared by the DoD hooks and scripts; dot-source it. Not a hook itself.

$DodRelativePath = 'docs/agents/dod.json'

# The repository root is the nearest directory at or above $start that holds the DoD file; $null when none does.
function Find-DodRoot([string]$start) {
    $root = $start
    while ($root -and -not (Test-Path (Join-Path $root $DodRelativePath))) {
        $root = Split-Path -Parent $root
    }
    $root
}

function Read-Dod([string]$root) {
    Get-Content -Raw -Path (Join-Path $root $DodRelativePath) | ConvertFrom-Json
}

# Output of a DoD step is quoted to an agent, so only its tail is kept.
$DodOutputTailLines = 100

# Runs one DoD entry ({name, run}) in a child pwsh found on PATH, not the current process path: when pwsh is
# installed as a dotnet tool, the current process is dotnet.exe.
function Invoke-DodStep($step) {
    $lines = & pwsh -NoProfile -NonInteractive -Command $step.run 2>&1
    [pscustomobject]@{
        ExitCode = $LASTEXITCODE
        Output = ($lines | Select-Object -Last $DodOutputTailLines | Out-String)
    }
}

# Repository-relative path with forward slashes; '..' segments are resolved first, so they cannot dodge a pattern.
function Get-RepositoryRelativePath([string]$root, [string]$path) {
    $full = [IO.Path]::GetFullPath($path, $root)
    [IO.Path]::GetRelativePath($root, $full).Replace('\', '/')
}

# The first protected pattern that matches the path, or $null. '**' spans folders, '*' stays within one;
# matching ignores case, because Windows paths do.
function Find-ProtectedPattern([string]$relativePath, $patterns) {
    foreach ($pattern in $patterns) {
        $regex = '^' + ([regex]::Escape($pattern) -replace '\\\*\\\*', '.*' -replace '\\\*', '[^/]*') + '$'
        if ($relativePath -match $regex) {
            return $pattern
        }
    }
    $null
}
