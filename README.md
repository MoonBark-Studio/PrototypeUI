# PrototypeUI Plugin

A reusable Godot C# plugin for prototype HUDs and lightweight presentation scaffolding.

PrototypeUI exists for the common UI layer that shows up across gameplay prototypes before a project commits to a fully custom UI stack.

## Features

- reusable HUD card container
- reusable text list view
- reusable stat meter row
- reusable hotbar button with optional cooldown overlay
- small core contracts for hotbar slot data and activation sinks

## Intended Use

Use this plugin when a project needs:

- a fast prototype HUD built from real `.tscn` resources
- snapshot-driven UI rendering without game-specific scene code baked into the controls
- a reusable hotbar surface that does not depend on one game's runtime types

Use a game project for:

- panel composition specific to that game's layout
- text generation from game state
- lore, faction, item, or rules-specific UI content

## Architecture

```text
PrototypeUI/
├── Core/                      # Godot-independent UI contracts
├── UI/                        # Godot controls and reusable scenes
├── PrototypeUI.Godot.csproj   # Godot-facing assembly
├── PrototypeUIPlugin.cs       # Godot addon entrypoint
└── plugin.cfg                 # Godot addon metadata
```

### Core

The core project defines generic data contracts:

- `PrototypeHotbarSlot`
- `PrototypeHotbarSlotKind`
- `IPrototypeHotbarActionSink`

### Godot addon

The Godot-facing layer provides reusable controls and scenes directly from the addon root:

- `HudSectionCard`
- `HudListView`
- `HudStatMeter`
- `PrototypeHotbarButtonView`
- `CooldownOverlayView`

## Current Status

Initial scaffold, now laid out so projects can consume it directly as `res://addons/PrototypeUI`.

