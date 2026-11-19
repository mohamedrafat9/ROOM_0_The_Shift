# ROOM 0: THE SHIFT — Implementation Log

Chronological record of work performed in this project. Only real actions are listed. Nothing here should be read as a play-tested result unless it says so.

---

## 2026-11-19 — Project setup (pre-existing commits)

Repository already contained two commits before this documentation task:

- `83ab413` Initial check-in
- `be3b780` setup

The project is a fresh Unity 6 project, not yet a game:
- One scene: `Assets/Scenes/SampleScene.unity`.
- Default objects only: `Main Camera`, `Directional Light`.
- No C# scripts in `Assets/`.
- No materials, prefabs, or custom assets.
- Default `Assets/InputSystem_Actions.inputactions`.

## 2026-11-19 — AGENTS.md created (earlier task)

Created root-level `AGENTS.md` describing project context, development rules, and constraints.
No scenes, assets, scripts, or project settings were touched.

## 2026-11-19 — MCP authoring smoke test (earlier task)

Purpose: confirm Unity MCP authoring works end to end.

Actions:
1. Created one Cube in the open scene via Unity MCP.
2. Named it `MCP_Test_Cube`.
3. Set its local position to `(0, 1, 0)`, rotation `(0, 0, 0)`, scale `(1, 1, 1)`.
4. Verified with `gameobject find` and by reading the `Transform` component.

Actual result:
- `MCP_Test_Cube` exists in the open scene at hierarchy path `/MCP_Test_Cube`.
- Transform verified: position `(0, 1, 0)`, rotation identity, scale `(1, 1, 1)`.
- The scene was **not saved**. The test cube exists only in the open, unsaved editing session. `SampleScene.unity` on disk still contains only the camera and light.

Note: the test cube is intentionally preserved. It is a temporary test object, not a game object.

## 2026-11-19 — Full inspection (read-only)

Unity MCP:
- `editor status`, `scene list_open`, `scene get_hierarchy`.
- `component get_fields` on `Transform` (camera, light, test cube) and `Camera`.
- `settings tags_layers get`, `build get_settings`, `console status`.

Filesystem (read-only):
- `Packages/manifest.json`, `ProjectSettings/ProjectVersion.txt`.
- `ProjectSettings/ProjectSettings.asset`, `GraphicsSettings.asset`, `QualitySettings.asset`, `TagManager.asset`, `DynamicsManager.asset`, `TimeManager.asset`, `EditorBuildSettings.asset`.
- `Assets/InputSystem_Actions.inputactions` (action map names).

Actual results are summarized in `SCENE_HIERARCHY.md` and in the final inspection report.

## 2026-11-19 — Documentation created

Created under `Documentation/`:
- `GAME_DESIGN.md`
- `IMPLEMENTATION_LOG.md` (this file)
- `CODE_EXPLANATION.md`
- `SCENE_HIERARCHY.md`
- `PRESENTATION_NOTES.md`

Updated root `AGENTS.md` with the development workflow and current project state.
No Unity objects, scenes, scripts, assets, or project settings were created or modified by this step.

## 2026-11-19 — Phase 1 / Task 1: Scene and Room Blockout

Approved scope: remove the test cube, prepare the scene, add it to Build Settings, build the 12×18×5 m room, provide an exit opening, create simple materials, and create the Player hierarchy with a CharacterController and child camera. No scripts, movement, dimension switching, puzzles, UI, or audio.

Actions:
1. Deleted the temporary `MCP_Test_Cube` (explicitly approved for this task).
2. Created `Assets/Materials/` with three `Standard` materials: `M_Room_Floor`, `M_Room_Wall`, `M_Room_Accent`.
3. Created root `Environment` and added primitives for floor, ceiling, four walls, three exit-frame parts, and an empty `Exit_Opening` marker.
4. Assigned the materials to the matching MeshRenderers.
5. Added three point lights inside the sealed room (`Light_South`, `Light_Center`, `Light_North`) with shadows off.
6. Created root `Player` at `(0, 0, -7.5)`, tagged `Player`, added and configured a `CharacterController`.
7. Reparented the original `Main Camera` under `Player`, renamed it `PlayerCamera`, and set its local position to `(0, 1.6, 0)`. This avoids duplicate cameras and duplicate AudioListeners.
8. Added `SampleScene` to Build Settings (`buildIndex: 0`, enabled).
9. Saved `SampleScene.unity`.

Verified with Unity MCP reads:
- Scene hierarchy has 3 roots: `Directional Light`, `Environment`, `Player`.
- `Environment/Floor` local position `(0, -0.1, 0)`, scale `(12, 0.2, 18)`; floor material `M_Room_Floor`.
- `Environment/Wall_West` local position `(-6.1, 2.5, 0)`, scale `(0.2, 5, 18)`.
- `Environment/ExitFrame_Top` uses material `M_Room_Accent`.
- `Player` local position `(0, 0, -7.5)`, identity rotation, tag `Player`.
- CharacterController values: height 1.8, radius 0.35, center `(0, 0.9, 0)`, slope limit 45, step offset 0.3, skin width 0.08.
- `PlayerCamera` local position `(0, 1.6, 0)`; still tagged `MainCamera` and keeps `Camera` + `AudioListener`.
- `Light_Center`: type Point, color `(0.85, 0.92, 1)`, intensity 1.1, range 12, shadows `No Shadows`, render mode `Not Important`.
- Build Settings contains exactly one scene: `Assets/Scenes/SampleScene.unity`, enabled.
- Scene reported `isDirty: false` after saving.
- Rendered the scene through `PlayerCamera` (640×360) to confirm the interior is visible and the exit opening is present.

Follow-up adjustment: the player root was moved from `(0, 0.05, -7.5)` to `(0, 0, -7.5)` so the camera sits at exactly 1.6 m above the floor. The scene was saved again and verified `isDirty: false`.

Result: Task 1 complete. Scene saved. No scripts, UI, puzzles, dimensions, kill zone, or door logic exist.

Note: five temporary errors appeared in the Unity console during authoring, all caused by rejected MCP parameter formats (`m_Shadows`, `m_RenderMode`, `m_Center`). They were command-validation errors, not project or code errors, and the console was cleared afterwards. Final console counts: 0 errors, 0 warnings.

Note: Unity created `ProjectSettings/SceneTemplateSettings.json` automatically while saving the scene. It was not authored manually and was not deleted.

## 2026-11-19 — Phase 1 / Task 2: Player Controller

Approved scope: WASD movement, mouse look with vertical limits, jump when grounded, gravity, CharacterController movement, cursor handling, new Input System only, no extra movement features.

Actions:
1. Created `Assets/Scripts/` and `Assets/Scripts/PlayerController.cs`.
2. Wired the script to the existing `Player` GameObject.
3. Assigned `Assets/InputSystem_Actions.inputactions` to the script's `inputActions` field.
4. Saved `SampleScene.unity`.

Implementation summary:
- The script uses `InputActionAsset.FindActionMap("Player")` and the existing `Move`, `Look`, and `Jump` actions.
- `Update()` handles cursor state, mouse look, and movement.
- Movement is built from `transform.right` and `transform.forward`, so it is relative to the player's facing.
- Gravity accumulates in `verticalVelocity`; jumping uses `Mathf.Sqrt(jumpHeight * -2f * gravity)` and is allowed only while `CharacterController.isGrounded`.
- Mouse look rotates the player for yaw and the child camera for pitch, clamped to ±80°.
- Cursor starts locked; Esc unlocks; a left click re-locks.

Play Mode tests (Unity MCP, actual measured values):
- Entered Play Mode; editor reported `playMode: playing`, console 0 errors / 0 warnings.
- **Walk speed:** 2.702 m in 0.6 s ≈ **4.5 m/s** (target 4.5).
- **Facing-relative movement:** after rotating the player 90°, forward input moved the player by `dx = +1.811`, `dz = -0.047`, i.e. along the new facing direction.
- **Jump height:** peak `maxY = 0.870` from `startY = 0.080`, rise **0.790 m** (target 0.8).
- **Gravity/landing:** position returned to `y = 0.080` after the jump.
- **Grounded-only jump:** a second Space press in mid-air left `isGrounded = false` and changed `verticalVelocity` from `-1.23` to `-2.75` (only gravity, no jump reset).
- **Mouse yaw:** a 100 px horizontal delta produced a **12.00°** yaw change (0.12°/px).
- **Pitch limits:** large vertical deltas clamped at **+80.0°** and **-80.0°**.
- **Cursor:** locked on start; Esc produced `CursorLockMode.None`; a left click returned it to `CursorLockMode.Locked`.
- Left Play Mode; scene reported `isDirty: false`; final console 0 errors / 0 warnings.

Not tested:
- Human feel/comfort of the sensitivity (only numeric response measured).
- Gamepad or other input devices.
- Slopes and step offsets (no such geometry in the room yet).
- Multiple resolutions or builds.
- Anything related to dimensions, puzzles, UI, audio, or pause.

Temporary test harness files were created under `Temp/` (outside `Assets/`) and deleted after testing. No test scripts were added to the project.

Known issue: with the current skin width, the controller rests at about `y = 0.080` above the floor plane rather than exactly `0`. This is normal CharacterController behaviour and is visually negligible.

Known conflict to resolve later: Esc currently unlocks the cursor, but Esc is also the planned pause key. The pause system will need to take over this input cleanly.

---

## Tests Performed

| Test | Result |
|---|---|
| Unity Editor reachable via MCP | Pass — status `ready`, not compiling |
| MCP can create and edit a GameObject | Pass — in the earlier smoke test, and again for Phase 1 objects |
| MCP can read Transform values | Pass — values matched what was set |
| Room dimensions readable from transforms | Pass — 12 (X) × 18 (Z) × 5 (Y) interior |
| Player + CharacterController configured | Pass — values verified by MCP read |
| Camera reparented and positioned | Pass — local `(0, 1.6, 0)`, tag `MainCamera` |
| Build Settings contains the scene | Pass — one enabled scene at index 0 |
| Scene saved | Pass — `isDirty: false` after save |
| Visual sanity render through player camera | Pass — interior and exit opening visible |
| Console clean after authoring | Pass — 0 errors, 0 warnings after clearing command-validation errors |
| PlayerController compiles | Pass — recompile completed with no errors |
| Walk speed in Play Mode | Pass — 2.702 m / 0.6 s ≈ 4.5 m/s |
| Facing-relative movement | Pass — 90° turn changed forward motion to +X |
| Jump height in Play Mode | Pass — 0.790 m measured (target 0.8) |
| Gravity and landing | Pass — returned to y = 0.080 |
| Jump only when grounded | Pass — mid-air press did not reset velocity |
| Mouse look yaw | Pass — 12.00° per 100 px |
| Mouse pitch limits | Pass — clamped at +80° and -80° |
| Cursor lock / Esc / click | Pass — locked, unlocked, re-locked |
| Play Mode console | Pass — 0 errors, 0 warnings |
| Dimension switching, puzzles, UI, audio | **Not performed** — not implemented |

## Known Issues and Risks

1. **Movement is implemented but not human play-tested.** Speed, jump, look, and cursor behaviour were verified numerically; comfort and feel were not evaluated.
2. **No door mesh or exit trigger.** The north wall has a 2.4 × 2.6 m opening and a cyan frame, but the opening is empty.
3. **No kill zone and no floor gap.** The floor is solid.
4. **Interior lighting is flat.** Three unshadowed point lights keep the sealed room readable; there is no bounce lighting.
5. **Input handling is set to the new Input System only** (`activeInputHandler: 1`). Scripts must use `UnityEngine.InputSystem`; legacy `Input.GetKey` will fail at runtime.
6. **Quality level is `Ultra`** on the built-in render pipeline. Heavier than needed for an M2 / 8 GB target; changing it is out of scope unless approved.
7. **No render pipeline asset** — built-in pipeline. Do not assume URP shaders or features.
8. **`ProjectSettings/SceneTemplateSettings.json` is an untracked file** generated by Unity during scene saving.
9. **Scene changes are uncommitted** (`Assets/Scenes/SampleScene.unity`, `Assets/Materials/`, `Assets/Scripts/`, `ProjectSettings/EditorBuildSettings.asset`).
10. **Esc will conflict with the future pause menu.** The cursor handler currently owns Esc and must be integrated with pause later.
11. **The player rests about 8 cm above the floor** due to CharacterController skin width; this is expected and visually minor.
