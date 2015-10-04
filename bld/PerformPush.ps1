if(-not ($Env:NugetKey)) {
	Write-Host "Please set NugetKey environment variable to use this script"
	exit 1;
}

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
	& $nugetPath push $pack.FullName -ApiKey $Env:NugetKey 
}

