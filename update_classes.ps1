$cardFiles = Get-ChildItem -Path d:\Project\LinkuraMod\core\cards\kaho -Recurse -Filter *.cs
foreach ($f in $cardFiles) {
    if ($f.Name -match "^Kaho(Card|InHandTriggerCard)\.cs$") { continue }
    $content = Get-Content $f.FullName -Raw
    $changed = $false
    if ($content -match ': LinkuraCard(?: |\()') {
        $content = $content -replace ': LinkuraCard\b', ': KahoCard'
        $changed = $true
    }
    if ($content -match ': InHandTriggerCard(?: |\()') {
        $content = $content -replace ': InHandTriggerCard\b', ': KahoInHandTriggerCard'
        $changed = $true
    }
    if ($changed) {
        Set-Content -Path $f.FullName -Value $content -Encoding UTF8 -NoNewline
    }
}

$powerFiles = Get-ChildItem -Path d:\Project\LinkuraMod\core\powers\kaho -Recurse -Filter *.cs
foreach ($f in $powerFiles) {
    if ($f.Name -eq "KahoPower.cs") { continue }
    $content = Get-Content $f.FullName -Raw
    if ($content -match ': LinkuraPower\b') {
        $content = $content -replace ': LinkuraPower\b', ': KahoPower'
        Set-Content -Path $f.FullName -Value $content -Encoding UTF8 -NoNewline
    }
}
