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

## 2026-11-20 — Phase 1 / Task 3: Dimension System

Approved scope: one `DimensionSwitcher.cs`, Q toggles Dimension A (Stable Reality) and Dimension B (Hybrid Reality), per-dimension environment objects, blocked switches when the target capsule overlaps solids or has no floor, no UI, no puzzles or collectibles.

Actions:
1. Created `Assets/Scripts/DimensionSwitcher.cs` and added it to `Player`.
2. Created `Environment/DimensionA_Root` (active) and `Environment/DimensionB_Root` (inactive, empty at first).
3. Added test objects:
   - `A_Pad_Test` at `(-3, 0.15, 1)`, scale `(2, 0.3, 2)`, cyan `M_Room_Accent`.
   - `A_HighPad_Test` at `(3, 0.15, -3)`, scale `(2, 0.3, 2)`, cyan.
   - `B_Pad_Test` at `(-3, 0.15, 1)`, same size, new violet `M_DimensionB_Accent`.
   - `B_Blocker_Test` at `(0, 1, -4)`, scale `(1.5, 2, 1.5)`, violet.
4. Created `Assets/Materials/M_DimensionB_Accent.mat` (Standard shader, color `(0.22, 0.12, 0.46)`, smoothness 0.55, metallic 0.1).
5. Assigned both roots on the switcher and saved `SampleScene.unity`.

Implementation summary:
- Q is read directly with `Keyboard.current.qKey.wasPressedThisFrame`; switching only runs while the cursor is locked.
- `Start()` forces Dimension A active and Dimension B inactive, so the scene always starts consistently.
- `TrySwitch(out string reason)` is public and returns success/failure. On failure it restores the old dimension and returns a short reason.
- `LastSwitchSucceeded` and `LastBlockedReason` are public so a future UI can display "SHIFT BLOCKED" without any new architecture.
- The target root is activated inside the same `Update` before checking, because inactive colliders cannot be queried. `Physics.SyncTransforms()` makes the new colliders visible to queries immediately.
- The body test builds the capsule from `transform.TransformPoint(controller.center)` and the controller's height/radius, then calls `Physics.OverlapCapsuleNonAlloc` with a cached array and a 5 cm radius shrink. The player's own collider and children are skipped.
- The support test uses five rays around the feet (centre + four points 20 cm apart) with a 30 cm length. Any hit counts, so ledges are not punished; a larger drop is blocked.
- Switching is grounded-only. The player position, rotation, and camera pitch are never changed.

Play Mode tests (Unity MCP):
- Console after entering Play Mode: 0 errors, 0 warnings.
- **Q on shared floor:** `grounded=True`, switched A→B, `LastSwitchSucceeded=True`.
- **Q again on shared floor:** switched B→A, `LastSwitchSucceeded=True`.
- **Edge of platform:** player placed at `(-2.1, 0.38, 1)`, 0.1 m inside `A_Pad_Test`; `grounded=True`; switch to B succeeded because the matching `B_Pad_Test` supports the feet.
- **No supporting floor:** player on `A_HighPad_Test` at `(3, 0.38, -3)`; B has nothing within 30 cm below; switch refused with `"No supporting floor in the other dimension."`; A stayed active.
- **Solid obstacle:** player at `(0, 0.08, -4)`; `B_Blocker_Test` occupies that space in B; switch refused with `"Solid geometry in the other dimension."`; A stayed active.
- **Airborne:** after a jump, `grounded=False`, `y=0.82`; Q refused with `"No stable footing."`; A stayed active.
- **Visual check:** captured the Game view in both states. A shows cyan pads; B hides them and shows the violet pad/blocker. Material check: `M_Room_Accent` vs `M_DimensionB_Accent`.
- Left Play Mode; scene reported `isDirty: false` with Dimension A active and Dimension B inactive; final console 0 errors, 0 warnings.

Test note: the Unity Editor did not keep OS focus during automation, which stopped normal keyboard input. The temporary harness set the Input System's editor background behavior to `IgnoreFocus`, injected the key state on the normal frame update, and restored the settings afterwards. Project settings were not changed. The harness files lived in `Temp/` and were deleted; no test scripts were added to `Assets/`.

Not tested:
- Human gamepad or keyboard play (Q only via injected key state).
- Switching on slopes, steps, or moving platforms (none exist).
- Puzzles, collectibles, UI, pause, kill zone, or door (out of scope).
- Human comfort or how obvious the dimension change feels in motion.

Known limitations:
- Switching is intentional only while grounded; mid-air switching is refused.
- The support test allows a step down of about 0.2 m and refuses larger drops.
- A foot-area ray hit is enough for support; standing half over a ledge is treated according to the controller's grounded state.
- Triggers are ignored by the safety checks, so fragments must not be placed on intended switch spots without re-testing.
- There is no UI message yet, only `Debug.Log` and the public result fields.
- The four test objects are placeholders, not puzzle content.

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
| DimensionSwitcher compiles | Pass — no errors |
| Q switches A → B on shared floor | Pass — B active, result `True` |
| Q switches B → A on shared floor | Pass — A active, result `True` |
| Switch near a platform edge | Pass — 0.1 m inside the edge, grounded, switched |
| Switch blocked by solid geometry | Pass — refused, A stayed active, clear reason |
| Switch blocked with no floor below | Pass — refused, A stayed active, clear reason |
| Switch blocked while airborne | Pass — refused at y = 0.82, clear reason |
| A/B visual distinction | Pass — cyan vs violet materials visible in captured Game view |
| Puzzles, collectibles, UI, audio | **Not performed** — out of scope |

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
12. **Dimension test objects are placeholders.** `A_Pad_Test`, `A_HighPad_Test`, `B_Pad_Test`, and `B_Blocker_Test` are for verification only and should be replaced or repositioned when puzzles are built.
13. **No on-screen feedback for a blocked shift.** The switcher exposes `LastSwitchSucceeded` / `LastBlockedReason` and logs to the console; the HUD message is still planned.
14. **Switching is grounded-only and tolerates only a small step down.** Mid-air switches and larger drops are refused by design.
15. **Scene and new assets are uncommitted.** The task did not request a commit.
