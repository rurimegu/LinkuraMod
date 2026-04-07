# LinkuraMod (WIP)

A [Slay the Spire 2](https://store.steampowered.com/app/2868840/Slay_the_Spire_2/) mod inspired by **Link! Like! LoveLive!**, adding a new playable character — **Hinoshita Kaho** (日野下花帆) — along with her full card set, powers, and relic.

---

## Features

### Character: Hinoshita Kaho

Kaho is a new playable character whose gameplay revolves around a unique resource mechanic: **Hearts (❤️)** and the **Burst** keyword.

### Core Mechanics

| Mechanic | Description |
|---|---|
| **❤️ (Hearts)** | Kaho's personal resource, capped at a Max ❤️ limit (default: 9). Cards and powers fill your hearts over the course of a turn. |
| **Burst** | Add ❤️. When hearts overflow past the limit, the overflow is dealt as damage to a random enemy. |
| **Collect** | Deal damage to a random enemy equal to current ❤️, then lose all ❤️. |
| **Backstage** | A keyword for cards that trigger a passive effect while held in hand, whenever a stated condition is met. |

---

## Installation

### Prerequisites

- [Slay the Spire 2](https://store.steampowered.com/app/2868840/Slay_the_Spire_2/) (Early Access)
- [BaseLib](https://github.com/Alchyr/Sts2BaseLib) mod framework (v0.2.0+)

### Steps

1. Download the latest release from the [Releases](https://github.com/rurimegu/LinkuraMod/releases) page.
2. Place the mod files into your STS2 mods folder:
   ```
   <STS2 install dir>/mods/LinkuraMod/
   ```
3. Launch Slay the Spire 2 and enable **LinkuraMod** in the mod list.

You also need to install [BaseLib](https://github.com/Alchyr/BaseLib-StS2/releases) v0.2.0 as a dependency.

---

## Building from Source

Requires [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) and [Godot 4](https://godotengine.org/) with .NET support.

```bash
# Debug build
dotnet build LinkuraMod.sln --configuration Debug

# Release build
dotnet build LinkuraMod.sln --configuration ExportRelease
```

---

## Credits

- **Author:** KCFindstr
- **Inspired by:** Link! Like! LoveLive!
- **Depends on:** [BaseLib](https://github.com/Alchyr/Sts2BaseLib) by Alchyr

> This is a fan-made mod and is not affiliated with or endorsed by ODD No., Bandai Namco, or the LoveLive! Series.
