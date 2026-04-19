[English](README.md) | [简体中文](README_zh.md)

# LinkuraMod

A [Slay the Spire 2](https://store.steampowered.com/app/2868840/Slay_the_Spire_2/) mod inspired by **Link! Like! LoveLive!**, adding a new playable character — **Hinoshita Kaho** (日野下花帆) — along with her full card set, and relics, and potions.

---

## Features

### Character: Hinoshita Kaho

Kaho is a new playable character whose gameplay revolves around a unique resource mechanic: **Hearts (❤️)** and the **Burst** keyword.

### Core Mechanics

| Mechanic        | Description                                                                                                                   |
| --------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| **❤️ (Hearts)** | Kaho's personal resource, capped at a Max ❤️ limit (default: 9). Cards and powers fill your hearts over the course of a turn. |
| **Burst**       | Increase ❤️ up to Max ❤️.                                                                                                     |
| **Collect**     | Deal damage to a random enemy equal to current ❤️, then lose all ❤️.                                                          |
| **Backstage**   | A keyword for cards that trigger a passive effect while held in hand, whenever a stated condition is met.                     |

---

## Installation

### Prerequisites

- [Slay the Spire 2](https://store.steampowered.com/app/2868840/Slay_the_Spire_2/) (Latest stable)
- [BaseLib](https://github.com/Alchyr/Sts2BaseLib) mod framework

### Steps

1. Download the latest release from the [Releases](https://github.com/rurimegu/LinkuraMod/releases) page.
2. Based on the release notes, download the recommended version of [BaseLib](https://github.com/Alchyr/BaseLib-StS2/releases). The mod might be compatible with other versions, but no guarantees.
3. Place the mod files into your STS2 mods folder (create it if it doesn't exist):
   ```
   <STS2 install dir>/mods/LinkuraMod/
   ```
4. Place the BaseLib files into your STS2 mods folder (create it if it doesn't exist):
   ```
   <STS2 install dir>/mods/BaseLib/
   ```
5. Launch Slay the Spire 2 and enable **LinkuraMod** in the mod list.

### Skins

The mod only comes with the default skin for Kaho. To get the alternate skins, you need to download and put the skin files in the following directory:

```
<STS2 install dir>/mods/LinkuraMod/skins/
```

So the directory structure should look like this:

```
<STS2 install dir>/mods/LinkuraMod/
├── LinkuraMod.dll
├── LinkuraMod.json
├── LinkuraMod.pck
├── skins/
│   ├── ingame_chara_sd_spine_1021_001
│   │   ├── spine_metadata.rurimegu
│   │   ├── stage_idol_model_1021_001.atlas
│   │   ├── stage_idol_model_1021_001.png
│   │   └── stage_idol_model_1021_001_json.skel
│   ├── ingame_chara_sd_spine_1021_002
│   │   └── ...
|   └── ...
└── ...
```

---

## Building from Source

Requires [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) and [Godot 4](https://godotengine.org/) with .NET support.

1. **Export the `LinkuraMod.pck` file using Godot.**
   Open the project in Godot, go to `Project -> Export -> Export PCK/ZIP` and save it as `LinkuraMod.pck`.
2. **Build the mod using the .NET CLI.**
   Run the following command in the terminal at the root directory of this repo:

   ```bash
   # Debug build
   dotnet build LinkuraMod.sln --configuration Debug

   # Release build
   dotnet build LinkuraMod.sln --configuration ExportRelease
   ```

3. **Copy the built files to the STS2 mods folder.**
   After building, copy the following files to your STS2 mods folder (`<STS2 install dir>/mods/LinkuraMod/`):
   - `.godot/mono/temp/bin/Debug/LinkuraMod.dll`
   - `LinkuraMod.pck`
   - `LinkuraMod.json` (you can find it in the root directory of the repo)

---

## Credits

- **Author:** KCFindstr
- **Inspired by:** Link! Like! LoveLive!
- **Depends on:** [BaseLib](https://github.com/Alchyr/Sts2BaseLib) by Alchyr
- **Special Thanks:** [密友@Bilibili](https://space.bilibili.com/383983658)

> This is a fan-made mod and is not affiliated with or endorsed by ODD No., Bandai Namco, or the LoveLive! Series.
