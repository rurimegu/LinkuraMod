[English](README.md) | [简体中文](README_zh.md)

# LinkuraMod

一个致敬 **Link! Like! LoveLive!** 的 [杀戮尖塔 2 (Slay the Spire 2)](https://store.steampowered.com/app/2868840/Slay_the_Spire_2/) MOD，添加了全新的可选角色 —— **日野下花帆** (Hinoshita Kaho) —— 以及她的卡牌、遗物和药水。

---

## 特性

### 角色：日野下花帆

花帆是一个全新的角色，其核心玩法围绕独特的资源机制：**❤️ (心)** 和 **爆心 (Burst)** 关键词展开。

### 核心机制

| 机制     | 说明                                                   |
| -------- | ------------------------------------------------------ |
| **❤️**   | 花帆的资源，上限默认为 9。卡牌和能力会在回合中收集❤️。 |
| **爆心** | 获得❤️，但不超过❤️上限。                               |
| **收心** | 对随机敌人造成等同于当前❤️数量的伤害，然后失去所有❤️。 |
| **幕后** | 当卡牌在手牌中且满足特定条件时触发的被动效果关键词。   |

---

## 安装

### 前置条件

- [杀戮尖塔 2 (Slay the Spire 2)](https://store.steampowered.com/app/2868840/Slay_the_Spire_2/) (最新稳定版)
- [BaseLib](https://github.com/Alchyr/Sts2BaseLib) MOD 框架

### 步骤

1. 从 [Releases](https://github.com/rurimegu/LinkuraMod/releases) 页面下载最新版本。
2. 根据版本说明，下载推荐版本的 [BaseLib](https://github.com/Alchyr/BaseLib-StS2/releases)。虽然可能兼容其他版本，但不保证稳定性。
3. 将 MOD 文件放入你的 STS2 mods 文件夹（如果不存在则创建一个）：
   ```
   <STS2 install dir>/mods/LinkuraMod/
   ```
4. 将 BaseLib 文件放入你的 STS2 mods 文件夹（如果不存在则创建一个）：
   ```
   <STS2 install dir>/mods/BaseLib/
   ```
5. 启动《杀戮尖塔 2》并在 MOD 列表中启用 **LinkuraMod**。

### 皮肤

本 MOD 仅附带花帆的默认皮肤。要使用备用皮肤，你需要下载并将皮肤文件放入以下目录：

```
<STS2 install dir>/mods/LinkuraMod/skins/
```

目录结构如下所示：

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
│   └── ...
└── ...
```

皮肤下载地址（2026/04/19更新，修复了部分皮肤）：
[谷歌盘](https://drive.google.com/file/d/1scaiE-_R99QQn1v0PcD73tc_lekBaME0/view?usp=sharing) | [直链](https://files.rurino.dev/linkuramod/LinkuraMod_all_skins_v0.1.2.zip)

---

## 从源码构建

需要 [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) 和支持 .NET 的 [Godot 4](https://godotengine.org/)。

1. **使用 Godot 导出 `LinkuraMod.pck` 文件。**
   在 Godot 中打开项目，前往 `Project -> Export -> Export PCK/ZIP` 并保存为 `LinkuraMod.pck`。
2. **使用 .NET CLI 构建 MOD。**
   在该仓库根目录的终端中运行以下命令：

   ```bash
   # Debug 构建
   dotnet build LinkuraMod.sln --configuration Debug

   # Release 构建
   dotnet build LinkuraMod.sln --configuration ExportRelease
   ```

3. **将构建的文件复制到 STS2 mods 文件夹。**
   构建完成后，将以下文件复制到你的 STS2 mods 文件夹 (`<STS2 install dir>/mods/LinkuraMod/`)：
   - `.godot/mono/temp/bin/Debug/LinkuraMod.dll`
   - `LinkuraMod.pck`
   - `LinkuraMod.json` (你可以在仓库根目录找到它)

---

## 鸣谢

- **作者：** KCFindstr
- **灵感来源：** Link! Like! LoveLive!
- **依赖：** [BaseLib](https://github.com/Alchyr/Sts2BaseLib) (作者：Alchyr)
- **特别鸣谢：** [密友@Bilibili](https://space.bilibili.com/383983658)

> 这是一个同人 MOD，与 ODD No.、Bandai Namco 或 LoveLive! 系列无关，也不受其认可。
