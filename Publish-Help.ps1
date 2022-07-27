dotnet tool restore
dotnet clean
dotnet build --configuration Release
Get-ChildItem -Path .\src\ -Directory -Exclude *.Tests | ForEach-Object -Process {
   $assemblyFilePath = ".\src\$($_.Name)\bin\Release\net48\$($_.Name).dll"
   $outputDirectoryPath = Join-Path -Path 'C:\Files\Projects\be.stateless\BizTalk.Factory.Documentation\Help\' -ChildPath ($($_.Name) -replace 'Be\.Stateless\.' , '' -replace '\.', '\')
   dotnet defaultdocumentation --ConfigurationFilePath .\DefaultDocumentation.json --AssemblyFilePath $assemblyFilePath --OutputDirectoryPath $outputDirectoryPath
}
