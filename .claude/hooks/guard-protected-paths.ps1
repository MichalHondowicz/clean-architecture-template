# PreToolUse hook (Edit, Write): the implementer may not write a protected path listed in docs/agents/dod.json.
# Protocol: read the hook event JSON from stdin; print a PreToolUse "deny" decision to refuse the tool call.
# Writes through Bash never reach this hook; the verifier's check-diff.ps1 catches them in the ticket's diff.

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'dod-common.ps1')

$hookInput = [Console]::In.ReadToEnd() | ConvertFrom-Json
$root = Find-DodRoot $hookInput.cwd
if (-not $root) {
    exit 0
}

$dod = Read-Dod $root
if ($hookInput.agent_type -ne $dod.implementerAgent) {
    exit 0
}

$relativePath = Get-RepositoryRelativePath $root $hookInput.tool_input.file_path

# Outside the repository (another drive, or e.g. the main checkout next to a worktree) the ticket's diff
# cannot see the change, so it is refused outright.
$outside = [IO.Path]::IsPathRooted($relativePath) -or $relativePath -eq '..' -or $relativePath.StartsWith('../')
$pattern = $outside ? $null : (Find-ProtectedPattern $relativePath $dod.protectedPaths)

$reason = $null
if ($outside) {
    $reason = "'$($hookInput.tool_input.file_path)' is outside the repository $root. The implementer writes only " +
        'inside its own worktree.'
}
elseif ($pattern) {
    $reason = "'$relativePath' is protected by '$pattern' in $DodRelativePath. The implementer may not change it; " +
        'if the ticket needs this change, finish with status stuck and say why.'
}

if ($reason) {
    @{
        hookSpecificOutput = @{
            hookEventName = 'PreToolUse'
            permissionDecision = 'deny'
            permissionDecisionReason = $reason
        }
    } | ConvertTo-Json -Compress
}

exit 0
