cd d:\Project\LinkuraMod\linkuramod\images

# Card portraits
mkdir -Force card_portraits\kaho\big
Get-ChildItem -Path card_portraits -File -Depth 0 | ForEach-Object { git mv $_.FullName card_portraits\kaho\ }
Get-ChildItem -Path card_portraits\big -File -Depth 0 | ForEach-Object { git mv $_.FullName card_portraits\kaho\big\ }

# Powers
mkdir -Force powers\kaho
Get-ChildItem -Path powers -File -Depth 0 | ForEach-Object { git mv $_.FullName powers\kaho\ }
# Move globals back
$globals = @("auto_burst", "hearts", "max_hearts", "true_face_pixel")
foreach ($g in $globals) {
    if (Test-Path "powers\kaho\.png") { git mv "powers\kaho\.png" powers\ }
    if (Test-Path "powers\kaho\.png.import") { git mv "powers\kaho\.png.import" powers\ }
}

# Relics
mkdir -Force relics\kaho\big
Get-ChildItem -Path relics -File -Depth 0 | ForEach-Object { git mv $_.FullName relics\kaho\ }
Get-ChildItem -Path relics\big -File -Depth 0 | ForEach-Object { git mv $_.FullName relics\kaho\big\ }

# CharUI
git mv charui\big_energy.png charui\kaho\big_energy.png
git mv charui\big_energy.png.import charui\kaho\big_energy.png.import
git mv charui\text_energy.png charui\kaho\text_energy.png
git mv charui\text_energy.png.import charui\kaho\text_energy.png.import
