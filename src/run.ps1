$solutionDir = Get-Location
$command = $args[0]

if ($args.length -gt 1) {
  $arguments = $args[1..($args.length-1)]
}

if ($command  -eq $null) {
  write-host "No command argument provided."
} elseif ($command -eq 'cleanup') {
  if (Test-Path -Path %solutionDir%\Sysx.DevTools.Cleanup\bin\Release\net6.0\Sysx.DevTools.Cleanup.exe -PathType Leaf) {
    & $solutionDir\Sysx.DevTools.Cleanup\bin\Release\net6.0\Sysx.DevTools.Cleanup.exe $arguments
  } else {
    & $solutionDir\Sysx.DevTools.Cleanup\bin\Debug\net6.0\Sysx.DevTools.Cleanup.exe $arguments
  }
} else {
  write-host "'${command}' is not a command."
}