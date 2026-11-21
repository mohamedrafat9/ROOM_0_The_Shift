# AGENTS.md — ROOM 0: THE SHIFT

Project-specific instructions for anyone (human or AI agent) working in this repository.

## Project Context

- **Game**: ROOM 0: THE SHIFT — a small first-person 3D puzzle game.
- **Engine**: Unity 6 (current project version: 6000.6.9f1), C#.
- **Setting**: A single room. The entire game takes place in this one space.
- **Core mechanic**: The player switches between two dimensions by pressing **Q**.
- **Objective**: Collect **3 energy fragments** and solve **3 puzzles** to unlock the exit.
- **Art style**: Simple 3D primitives (cubes, spheres, planes, etc.) and simple materials. No complex or high-poly assets.
- **Target hardware**: Development on a MacBook Air M2 with 8 GB RAM. Keep the project lightweight:
  - Prefer primitives and simple materials over imported models/textures.
  - Avoid heavy post-processing, large texture sets, and unnecessary packages.
  - Keep scenes small and draw calls low.

## Development Rules

1. Work incrementally — implement one feature at a time.
2. Do not implement features that have not been requested.
3. Prefer simple, readable C# code a beginner can understand and explain. Favor clear names and small methods over clever abstractions.
4. Avoid unnecessary architecture, dependencies, and complex design patterns (no ECS, DI containers, event buses, etc. unless explicitly requested).
5. Before modifying existing files, inspect them and understand their purpose.
6. Do not delete or overwrite existing project files without explicit approval.
7. Use Unity MCP tools for Unity Editor operations whenever possible (GameObjects, components, scenes, assets, verification).
8. Verify changes after executing them (e.g., confirm the GameObject/component state with MCP tools or a compile check).
9. Explain important implementation decisions and potential side effects.
10. Do not enter Play Mode or modify project settings unless explicitly requested.
11. Keep the game scope aligned with the original project brief described above.

## Working Notes

- **Language/runtime**: Unity 6, C# (Mono scripting backend unless told otherwise).
- **Scene authoring**: Prefer building from Unity primitives; name objects clearly.
- **Dimensions**: The two-world mechanic is central — keep dimension switching behavior simple and explicit (e.g., toggling object sets or visibility groups driven by a single input handler).
- **Scope guard**: 3 fragments, 3 puzzles, 1 exit, 1 room. Do not expand the scope without approval.
- **Performance**: Assume an 8 GB RAM machine. Avoid unbounded allocations in `Update`, prefer cached references, and keep asset sizes small.
- **Verification**: After edits, confirm with Unity MCP (inspect components/fields) rather than assuming success.

## Do Not

- Do not add materials, scripts, packages, or assets unrelated to the requested task.
- Do not modify scenes, project settings, or save scenes unless the task explicitly asks for it.
- Do not introduce third-party dependencies without explicit approval.

## Documentation

Design and planning documents live in `Documentation/` at the project root:

- `Documentation/GAME_DESIGN.md` — concept, controls, dimensions, puzzles, flow, visual direction.
- `Documentation/IMPLEMENTATION_LOG.md` — chronological log of real actions, tests, and known issues.
- `Documentation/CODE_EXPLANATION.md` — planned scripts and beginner-level Unity concepts.
- `Documentation/SCENE_HIERARCHY.md` — current scene contents and planned hierarchy.
- `Documentation/PRESENTATION_NOTES.md` — explanation, systems, and likely Q&A.

Read the relevant document before changing gameplay, scene layout, or UI.

## Verified Project Baseline

- Unity `6000.6.9f1`, built-in render pipeline (no URP).
- Input handling: new Input System only (`activeInputHandler: 1`). Use `UnityEngine.InputSystem`; legacy `Input.GetKey` will not work.
- One scene: `Assets/Scenes/SampleScene.unity`. It is saved and enabled in Build Settings at index 0.
- Phase 1 room blockout exists: `Environment` root (12 × 18 × 5 m room, exit opening, three point lights) and `Player` root (CharacterController + `PlayerCamera` + `PlayerController` + `DimensionSwitcher`).
- Task 3 dimension system exists: `Environment/DimensionA_Root` (active) and `Environment/DimensionB_Root` (inactive), Q switching with grounded-only collision/floor safety, and four placeholder test cubes.
- Materials: `Assets/Materials/M_Room_Floor.mat`, `M_Room_Wall.mat`, `M_Room_Accent.mat`, `M_DimensionB_Accent.mat` (Standard shader).
- The temporary `MCP_Test_Cube` was removed in Phase 1 with approval.
- Tags available: `Respawn`, `Finish`, `MainCamera`, `Player`, `GameController`.
- Existing input actions: `Move`, `Look`, `Jump`, `Interact`, `Cancel` (plus unused actions such as `Attack`, `Crouch`, `Sprint`).
- `Assets/Scripts/PlayerController.cs` implements first-person movement, mouse look, jump, gravity, and cursor locking (Task 2). `Assets/Scripts/DimensionSwitcher.cs` implements Q dimension switching with `TrySwitch(out string reason)` (Task 3). Prefabs, UI, puzzles, collectibles, door, and kill zone do not exist yet.

## Implementation Workflow

1. Restate the single requested feature and its acceptance check.
2. Inspect affected files, objects, and settings first.
3. Implement the smallest change using Unity built-in components and existing packages.
4. Use Unity MCP for Editor operations: create/configure GameObjects, add components, set fields, read values back.
5. Do not save the scene or assets unless the task explicitly asks for it.
6. Verify with MCP reads (hierarchy, components, fields) and a console check; report what was actually verified.
7. Record real actions, results, and new issues in `Documentation/IMPLEMENTATION_LOG.md`.
8. Stop and wait for approval before starting the next feature.

## Remaining Scope (approved design, not yet built)

- Puzzles: missing platform, hidden passage, final switch, added inside the existing dimension roots.
- Gameplay: 3 fragments, locked exit with E interaction, kill zone, win/lose, pause, start screen.
- Scripts: at most about six small scripts total (two exist: `PlayerController`, `DimensionSwitcher`; next: `Collectible`, `GameManager`, `DoorController`, `UIManager`).
