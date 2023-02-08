
$myPath = split-path -parent $MyInvocation.MyCommand.Definition
$cli = Join-Path $myPath /DotnetStarter.Cli/bin/Debug/net7.0/DotnetStarter.Cli.exe
& $cli $args