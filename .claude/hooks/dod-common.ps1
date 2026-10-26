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

# What makes a DoD file unusable: an empty one would pass every check, and one without the implementer's name
# would switch the hooks off. Returns the problems, none when the file is usable.
function Get-DodProblems($dod) {
    if (-not "$($dod.implementerAgent)".Trim()) { 'implementerAgent is missing' }
    if (@($dod.commands).Where({ $_.run }).Count -eq 0) { 'commands has no command with a run' }
    if ($null -eq $dod.environmentChecks) { 'environmentChecks is missing (use [] for none)' }
    if ($null -eq $dod.protectedPaths) { 'protectedPaths is missing (use [] for none)' }
}

# Runs $action while holding a machine-wide mutex for this repository root, and returns its output. The DoD gate
# and run-dod.ps1 both build and test; two runs over the same output folders lock each other's DLLs and turn a
# green DoD red (seen in the first /ship run).
function Invoke-DodExclusive([string]$root, [scriptblock]$action) {
    $key = [IO.Path]::GetFullPath($root).TrimEnd('\', '/').ToLowerInvariant()
    $hash = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($key)))
    $mutex = [Threading.Mutex]::new($false, "Global\dod-$($hash.Substring(0, 16))")
    $owned = $false
    try {
        try {
            $owned = $mutex.WaitOne([TimeSpan]::FromMinutes(30))
        }
        catch [Threading.AbandonedMutexException] {
            # The previous holder died mid-run; the mutex is ours now.
            $owned = $true
        }
        if (-not $owned) {
            throw [TimeoutException]::new("Another DoD run on $root held the lock for 30 minutes.")
        }
        & $action
    }
    finally {
        if ($owned) {
            $mutex.ReleaseMutex()
        }
        $mutex.Dispose()
    }
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
