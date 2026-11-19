# ROOM 0: THE SHIFT — Scene Hierarchy

## 1. Current Scene (after Phase 1, saved)

Scene: `Assets/Scenes/SampleScene.unity`
Status: **saved** and added to Build Settings as build index 0.
Root count: 3 (`Directional Light`, `Environment`, `Player`).

```
SampleScene
├── Directional Light        (default light, unchanged)
├── Environment              (Transform only, position 0,0,0)
│   ├── Floor                cube, pos (0, -0.1, 0), scale (12, 0.2, 18)
│   ├── Ceiling              cube, pos (0, 5.1, 0), scale (12, 0.2, 18)
│   ├── Wall_West            cube, pos (-6.1, 2.5, 0), scale (0.2, 5, 18)
│   ├── Wall_East            cube, pos (6.1, 2.5, 0), scale (0.2, 5, 18)
│   ├── Wall_South           cube, pos (0, 2.5, -9.1), scale (12, 5, 0.2)
│   ├── Wall_North_Left      cube, pos (-3.6, 2.5, 9.1), scale (4.8, 5, 0.2)
│   ├── Wall_North_Right     cube, pos (3.6, 2.5, 9.1), scale (4.8, 5, 0.2)
│   ├── Wall_North_Top       cube, pos (0, 3.8, 9.1), scale (2.4, 2.4, 0.2)
│   ├── ExitFrame_Left       cube, pos (-1.3, 1.3, 8.95), scale (0.2, 2.6, 0.1)
│   ├── ExitFrame_Right      cube, pos (1.3, 1.3, 8.95), scale (0.2, 2.6, 0.1)
│   ├── ExitFrame_Top        cube, pos (0, 2.7, 8.95), scale (2.8, 0.2, 0.1)
│   ├── Exit_Opening         empty marker, pos (0, 0, 9)
│   ├── Light_South          point light, pos (0, 4.4, -6)
│   ├── Light_Center         point light, pos (0, 4.4, 0)
│   └── Light_North          point light, pos (0, 4.4, 6)
└── Player                   tag "Player", pos (0, 0, -7.5), CharacterController + PlayerController
    └── PlayerCamera         tag "MainCamera", local pos (0, 1.6, 0)
```

### Room measurements (verified from transforms)

- Interior width: X `-6` to `+6` = **12 m**.
- Interior length: Z `-9` to `+9` = **18 m**.
- Interior height: Y `0` to `5` = **5 m**.
- Wall/floor/ceiling thickness: 0.2 m.
- Exit opening: **2.4 m wide × 2.6 m high**, centered on the north wall at Z = 9.
- Frame inner faces align with the opening edges (X ±1.2, top 2.6).

### Player

- `Player` is an empty GameObject at `(0, 0, -7.5)` at the south end.
- `PlayerController` (from `Assets/Scripts/PlayerController.cs`) handles WASD movement, mouse look, jump, gravity, and cursor locking. Its `inputActions` field references `Assets/InputSystem_Actions.inputactions`.
- `CharacterController`: height 1.8, radius 0.35, center `(0, 0.9, 0)`, slope limit 45, step offset 0.3, skin width 0.08, min move distance 0.
- `PlayerCamera` is the original `Main Camera`, reparented and renamed. Local position `(0, 1.6, 0)` → world height **1.6 m**. Camera settings (FOV 60, near 0.3, far 1000, HDR, MSAA) are unchanged.

### Materials (Assets/Materials)

| Material | Used by | Color | Smoothness |
|---|---|---|---|
| `M_Room_Floor.mat` | Floor | dark slate `(0.32, 0.34, 0.39)` | 0.45 |
| `M_Room_Wall.mat` | Ceiling + all walls | light gray `(0.78, 0.80, 0.83)` | 0.25 |
| `M_Room_Accent.mat` | Exit frame (3 parts) | cyan `(0.12, 0.78, 0.86)` | 0.60 |

All use the built-in `Standard` shader, no textures, no post-processing.

### Lighting

- `Directional Light` remains as the original default.
- Three point lights (`Light_South`, `Light_Center`, `Light_North`) at Y = 4.4, color `(0.85, 0.92, 1)`, intensity 1.1, range 12, **no shadows**, render mode `Not Important`. They exist because the sealed ceiling blocks the directional light; a lit interior is needed even at blockout stage.

### Temporary test object

`MCP_Test_Cube` was removed in Phase 1 with explicit approval. It no longer exists in the scene or on disk.

## 2. Planned Hierarchy (next phases)

```
SampleScene
├── Systems
│   ├── GameManager
│   ├── DimensionSwitcher
│   └── UI_Canvas              (HUD, Start, Pause, Win, Lose panels)
├── Player
│   ├── CharacterController + PlayerController.cs
│   └── PlayerCamera
├── Environment_Common         (existing Environment becomes this)
├── DimensionA_Root            Dimension A objects only
├── DimensionB_Root            Dimension B objects only
└── Hazards
    └── KillZone               trigger volume below the floor
```

### Dimension organization rules

- One root per dimension. Puzzle objects belonging to a dimension live inside its root.
- Only geometry, props, and lights go in dimension roots. Never the Player, camera, UI, or managers.
- Objects that must exist in both dimensions stay in `Environment_Common` (the current room shell and exit).
- Each puzzle gets a parallel pair, e.g. `Puzzle1_Platform` under A and under B.
- Fragments that exist in only one dimension live in that dimension's root; shared fragments can live in `Environment_Common`.

### Layers (undecided)

- `DimensionA` / `DimensionB` layers are recommended only if the safe-switch check uses layer-based physics queries.
- Adding layers changes project settings and needs approval.

## 3. Current vs Planned

| Item | Current (Phase 1 + Task 2) | Planned |
|---|---|---|
| Scenes | `SampleScene`, saved, build index 0 | Same single scene |
| Player | CharacterController + camera child + `PlayerController.cs` | Final tuning only |
| Scripts | 1 (`PlayerController.cs`) | 4–6 small scripts |
| Materials | 3 Standard materials | Dimension-themed materials |
| UI | None | One Canvas with HUD + menus |
| Dimensions | Not created | `DimensionA_Root`, `DimensionB_Root` |
| Build Settings | `SampleScene` enabled at index 0 | Unchanged |
| Test object | Removed | — |

## 4. Notes and Risks

- The room is a sealed box with one exit opening; there is currently no door mesh and no exit trigger.
- There is no kill zone and no gap in the floor yet.
- The interior is lit by three unshadowed point lights; expect flat but readable lighting.
- The camera starts at the south end facing north (identity rotation), which matches the intended start direction.
- `ProjectSettings/SceneTemplateSettings.json` appeared in the project during scene saving as an auto-generated Unity file; it is untracked and was not authored manually.
