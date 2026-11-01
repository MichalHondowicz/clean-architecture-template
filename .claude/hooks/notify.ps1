# Script for /ship and the Notification hook: sends one line to the user's ntfy topic, because the app's push
# notifications reach the phone only when its app is opened.
# Usage: pwsh -File notify.ps1 -Message <text>   (/ship, next to each of its pushes)
#        pwsh -File notify.ps1                   (Notification hook: the event JSON on stdin, its `message` is sent)
# The topic's full URL comes from the user's environment variable CLAUDE_NTFY_URL, never from the repository.
# Exits 0 when sent, and when no topic is set (prints {"sent":false,...}: the fallback is optional); 1 when the
# server rejects the message or cannot be reached; 2 when there is no message to send.

param(
    [string]$Message
)

$ErrorActionPreference = 'Stop'

if (-not $Message) {
    $hookInput = [Console]::In.ReadToEnd()
    if ($hookInput.Trim()) {
        $Message = ($hookInput | ConvertFrom-Json).message
    }
}
if (-not $Message) {
    [Console]::Error.WriteLine('Nothing to send: pass -Message, or the hook event JSON with a message on stdin.')
    exit 2
}

$topicUrl = $env:CLAUDE_NTFY_URL
if (-not $topicUrl) {
    [ordered]@{ sent = $false; reason = 'CLAUDE_NTFY_URL is not set' } | ConvertTo-Json -Compress
    exit 0
}

try {
    $response = Invoke-WebRequest -Uri $topicUrl -Method Post -SkipHttpErrorCheck -TimeoutSec 15 `
        -ContentType 'text/plain; charset=utf-8' -Body ([Text.Encoding]::UTF8.GetBytes($Message))
}
catch {
    [Console]::Error.WriteLine("The topic could not be reached: $($_.Exception.Message)")
    exit 1
}
if ($response.StatusCode -lt 200 -or $response.StatusCode -ge 300) {
    [Console]::Error.WriteLine("The topic answered $($response.StatusCode).")
    exit 1
}

[ordered]@{ sent = $true } | ConvertTo-Json -Compress
exit 0
