param
(
	[string]$apiUrl,
	[string]$apiKey
)

$scriptPath 		= $MyInvocation.MyCommand.Path
$scriptDir 			= Split-Path -Parent $scriptPath
$nugetPath 			= $scriptDir + "\nuget.exe"
$scanDirectory 		= $scriptDir + "\..\src\";
$outputDirectory 	= $scriptDir + "\..\bld\";

Write-Host "Pushing packages to nuget server"

$nupacks = Get-ChildItem -Recurse -Path $outputDirectory -Filter *.nupkg 
foreach ($pack in $nupacks)
{
	Write-Host "   Pushing nupkg file:" + $pack.FullName
	if ($apiUrl -eq "default") {
		& $nugetPath push $pack.FullName -ApiKey $apiKey 
	} else {
		
		& $nugetPath push $pack.FullName -ApiKey $apiKey -Source $apiUrl
	}
}

