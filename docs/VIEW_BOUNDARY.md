# PrototypeUI View Boundary

This document records the current boundary for PrototypeUI after the first real extraction and test-suite migration.

## Decision

PrototypeUI should keep a Godot-facing layer for reusable control scenes and view primitives.

PrototypeUI should not own full composed gameplay HUDs or screen-specific root views at this stage.

In practice, that means:

- keep reusable Godot controls and `.tscn` templates in the plugin
- keep game-owned `CanvasLayer`, HUD root, and scene composition in the game project
- keep runtime-specific snapshot shaping and text generation in the game project

## Why The Plugin Should Keep Godot Views

Leaving PrototypeUI as core-only would push layout and control duplication back into each game.

The current extracted value is specifically the reusable Godot-side presentation layer:

- `HudSectionCard`
- `HudListView`
- `HudStatMeter`
- `PrototypeHotbarButtonView`
- `CooldownOverlayView`

These are not game rules. They are reusable visual controls with stable Godot node structure, editor-authored layout, and focused tests.

If they moved back to game-level implementation, each prototype would need to rebuild the same `.tscn` structure, styling, and behavior glue.

## Why Full HUD Roots Should Stay Game-Owned

Full composed views like `PlayableHudView` still encode game-specific decisions:

- which panels exist and in what order
- which tabs are visible and how they toggle
- how runtime snapshots are translated into strings and list entries
- what counts as friendly, hostile, inventory, intel, logs, or faction detail
- which runtime interface is used and where event wiring lives

Those are composition and policy decisions, not reusable primitives.

Moving them into PrototypeUI now would make the plugin depend on one game's screen model instead of exposing a stable reusable control layer.

## Current Boundary

### Keep In PrototypeUI

- Godot control scripts and reusable `.tscn` resources
- control-level state mapping from generic contracts into visuals
- hotbar presentation primitives and cooldown visuals
- focused plugin-owned Godot tests for those controls

### Keep In Game Projects

- root HUD scenes such as `PlayableHudView`
- panel arrangement, tab ordering, and scene-specific composition
- runtime event subscription and snapshot routing
- text generation from game state and world rules
- project-specific input policy and UX decisions

## Extraction Rule For Full Views

Only extract a full Godot view into PrototypeUI when all of the following are true:

1. At least two projects need materially the same root layout.
2. The snapshot contract is already game-agnostic.
3. The tab structure, panel ordering, and interaction policy are shared instead of project-specific.
4. The plugin can expose a stable composition API without depending on Thistletide concepts.

Until then, the right architecture is:

- plugin-owned reusable Godot controls
- game-owned composition roots

## Immediate Implication

PrototypeUI should continue to improve its Godot-side control layer and tests.

The next extraction work should target adapter seams like CharacterControl and AudioSystem rather than pulling `PlayableHudView` into the plugin prematurely.
