
$root_dir = $args[0]
$changeset = $args[1]
$dev_env = $args[2]

$version_file = $root_dir + "CommonLib\Version.cs"
$version_template = $version_file + ".template"

if($changeset -match "\d+")
{
	$revision =  $matches[0]
}
else
{
	$exepath = $dev_env + "TF.exe"
#"C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\TF.exe"
	$result = & $exepath history $root_dir /r /noprompt /stopafter:1 /version:W
	if("$result" -match "\d+")
	{
		$revision =  $matches[0]
	}
	else 
	{
# get revision without version control
		$startday = get-date -year 2000 -month 1 -day 1
		$revision = [int]($(New-TimeSpan $startday $(get-date)).TotalHours / 2) % 32768
	}
}

cat $version_template | % { $_ -replace '\$WCREV\$', $revision } > $version_file