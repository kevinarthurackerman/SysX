$key = Read-Host 'Enter nuget API key'

Get-ChildItem ..\nupkg | ForEach-Object {
    dotnet nuget push $_.FullName --api-key $key --source https://api.nuget.org/v3/index.json
}