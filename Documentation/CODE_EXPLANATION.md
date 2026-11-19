# ROOM 0: THE SHIFT — Code Explanation

> **Status:** `PlayerController.cs` is implemented. The other scripts described below are still planned and do not exist yet.

## 0. Current State After Phase 1 and Task 2

Phase 1 built the scene using **only Unity built-in components**. Task 2 added the first gameplay script.

- Room geometry uses standard `Cube` primitives (`MeshFilter`, `MeshRenderer`, `BoxCollider`).
- The three room materials use the built-in `Standard` shader with plain color values.
- The player is a tagged empty GameObject with a manually configured `CharacterController`.
- The camera is the original `Main Camera`, reparented under `Player` and renamed `PlayerCamera`.
- Three point lights were added because the sealed ceiling blocks the directional light.
- `Assets/Scripts/PlayerController.cs` now moves the player and rotates the camera using the new Input System.

CharacterController values: `m_Height = 1.8`, `m_Radius = 0.35`, `m_Center = (0, 0.9, 0)`, `m_SlopeLimit = 45`, `m_StepOffset = 0.3`, `m_MinMoveDistance = 0`.

The goal is a small number of small scripts, each with one clear job.

## 1. Scripts

### PlayerController.cs — implemented

**File:** `Assets/Scripts/PlayerController.cs`
**Job:** Move the player, rotate the camera, and handle jumping and gravity.

It is attached to the `Player` GameObject. It requires a `CharacterController` (`[RequireComponent]`) and finds the child camera automatically.

**Input wiring (new Input System only):**
- The Inspector field `inputActions` references `Assets/InputSystem_Actions.inputactions`.
- In `Awake()`, the script finds the `"Player"` action map and the `Move`, `Look`, and `Jump` actions from it.
- `OnEnable()` enables the three actions; `OnDisable()` disables them.
- No legacy `Input.GetKey` or `Input.GetAxis` is used anywhere.

**Fields and what they mean:**

| Field | Value | Purpose |
|---|---|---|
| `inputActions` | Input actions asset | Source of the Move/Look/Jump actions |
| `moveSpeed` | 4.5 | Walking speed in metres per second |
| `jumpHeight` | 0.8 | Desired jump height in metres |
| `gravity` | -9.81 | Downward acceleration in m/s² |
| `lookSensitivity` | 0.12 | Degrees of rotation per pixel of mouse movement |
| `maxLookAngle` | 80 | Clamp for looking up/down |
| `controller` | — | Cached `CharacterController` reference |
| `playerCamera` | — | Cached child camera |
| `moveAction` / `lookAction` / `jumpAction` | — | Cached actions from the asset |
| `verticalVelocity` | 0 | Current up/down speed |
| `cameraPitch` | 0 | Current up/down look angle |

**Methods:**

| Method | Purpose |
|---|---|
| `Awake()` | Caches the controller and camera, finds the action map and actions, and logs an error if the asset is missing. |
| `OnEnable()` / `OnDisable()` | Enables/disables the three input actions. |
| `Start()` | Locks and hides the cursor so mouse look works. |
| `Update()` | Calls cursor handling, mouse look, and movement once per frame. |
| `HandleCursor()` | Esc unlocks the cursor; a left click locks it again. |
| `SetCursorLocked(bool)` | Sets `Cursor.lockState` and `Cursor.visible` in one place. |
| `HandleMouseLook()` | Reads the `Look` action and rotates the body and camera. |
| `HandleMovement()` | Reads `Move`, applies gravity and jumping, and calls `CharacterController.Move`. |

**Movement (simple explanation):**
1. Read `Move` as a `Vector2` (X = strafe, Y = forward/back).
2. Build a world direction from `transform.right * x + transform.forward * y`. Because this uses the player's own transform, W always moves in the direction the player faces.
3. Normalize diagonal input so diagonal walking is not faster.
4. Multiply by `moveSpeed * Time.deltaTime` so speed is the same on every machine.
5. Send the result to `CharacterController.Move`, which handles collisions and slopes.

**Jumping and gravity:**
- `controller.isGrounded` tells us whether the capsule is on the floor.
- While grounded, `verticalVelocity` is held at `-2` so the controller stays pressed to the ground.
- On Space, only if grounded: `verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity)`. This is the standard physics formula `v = sqrt(2 * g * h)`; it gives about `3.96 m/s` for a 0.8 m jump.
- Every frame, `verticalVelocity += gravity * Time.deltaTime` makes a jump rise, slow down, and fall.
- Horizontal speed and `verticalVelocity` are combined into one `Move` call, so movement and gravity never fight each other.

**Mouse look (simple explanation):**
- The `Look` action returns mouse movement for the frame (not a speed).
- Horizontal input rotates the whole player around the Y (up) axis.
- Vertical input changes a stored `cameraPitch`, which is clamped to ±80° so the camera cannot flip over.
- The camera's local X rotation is set from `cameraPitch`; the player handles left/right and the camera handles up/down. This separation avoids tilted movement.
- Mouse movement is **not** multiplied by `Time.deltaTime` because it is already a per-frame amount.

**Cursor handling:**
- On start the cursor is locked and hidden.
- Esc unlocks and shows it (useful in the editor and before the pause menu exists).
- Left click re-locks it and hides it again.
- Mouse look is skipped while the cursor is unlocked.

**Tested values (Play Mode, measured):**
- Jump rise: 0.790 m (target 0.8).
- Walking distance: 2.70 m in 0.6 s ≈ 4.5 m/s.
- Mouse: 100 px produced 12° of yaw (`0.12°/px`).
- Pitch clamped at exactly +80° and -80°.
- A second Space press in mid-air did not reset vertical velocity, confirming the grounded check.
- Cursor lock, Esc unlock, and click re-lock all worked.

Why `CharacterController` instead of a `Rigidbody`: it gives predictable walking, stair stepping, and simple ground checks without physics tuning.

### DimensionSwitcher.cs
**Job:** Handle Q, toggle the two dimension groups, and refuse unsafe switches.

Planned responsibilities:
- Keep references to `DimensionA_Root` and `DimensionB_Root`.
- Track the current dimension.
- On Q: test whether switching is safe, then `SetActive()` the new group.
- Tell the UI to show the dimension name and the "SHIFT BLOCKED" message.

Planned methods:
- `TrySwitch()`
- `bool IsSwitchSafe(Vector3 position)`
- `SetDimension(bool useA)`

Simple safety approach (planned):
1. Compute the player's feet and head points from the `CharacterController`.
2. Temporarily enable the destination group and disable the current one.
3. Call `Physics.SyncTransforms()` so physics queries see the change immediately.
4. Run `Physics.CheckCapsule` at the player position and a short downward `Physics.Raycast` for ground support.
5. If blocked, immediately revert the group states and show "SHIFT BLOCKED".

Trade-off: this is the easiest version to read and explain. It briefly changes active states inside one frame; if flicker or ordering problems appear, a later alternative is layer-based queries without toggling. Both dimensions need distinct layers for the cleaner version, which is **undecided**.

### Collectible.cs
**Job:** Represent one Energy Fragment.

Planned responsibilities:
- Detect the player entering its trigger.
- Tell `GameManager` that a fragment was collected.
- Hide or destroy itself once collected.
- Optionally spin/bob for visibility (visual only).

Planned methods:
- `OnTriggerEnter(Collider other)` — check for the Player tag, call the game manager, then disable the object.

Requirements: a `Collider` with **Is Trigger** enabled, and the player object tagged `Player`.

### GameManager.cs
**Job:** Own the game state.

Planned responsibilities:
- Count collected fragments (0 to 3).
- Unlock the exit when the count reaches 3.
- Trigger Win when the player reaches the exit.
- Trigger Lose when the player enters the kill zone.
- Restart the level.

Planned methods:
- `AddFragment()`
- `bool IsExitUnlocked()`
- `Win()` / `Lose()`
- `Restart()`

Planned state: one `int fragmentsCollected`, one `bool exitUnlocked`, and a reference to the UI.

Restart can either reload the scene (`SceneManager.LoadScene`) or reset positions and counters manually. Reloading is simpler and less error-prone; **decided at implementation time**.

### DoorController.cs
**Job:** The exit door.

Planned responsibilities:
- Know whether the exit is unlocked.
- On E while the player is near, open/allow exit if unlocked.
- If locked, show a short message such as "COLLECT ALL FRAGMENTS".
- Report arrival to `GameManager` when the player passes through.

Planned methods:
- `Interact()`
- `OnTriggerEnter(Collider other)` (exit zone)

E interaction can be handled by the player script calling `DoorController.Interact()`, or by the door checking proximity and input itself. Only one of these should be used to avoid duplicate input handling.

### UIManager.cs
**Job:** All on-screen text and menus.

Planned responsibilities:
- Fragment counter (0/3).
- Dimension indicator (A / B).
- Temporary "SHIFT BLOCKED" message.
- Start Screen with instructions.
- Pause Menu.
- Win Screen: "You Escaped Room 0."
- Lose Screen with Restart.

Planned methods:
- `SetFragmentCount(int count)`
- `SetDimension(string label)`
- `ShowBlockedMessage()`
- `ShowStart()`, `ShowPause(bool)`, `ShowWin()`, `ShowLose()`

Key ideas:
- Uses Unity UI (`uGUI`, already installed).
- Buttons call public methods on the UI manager or game manager.
- `Time.timeScale = 0` while paused or on end screens; remember to restore it to 1.

### Possible reduction
Six scripts are a suggestion. A simpler split is acceptable, for example merging `DoorController` into `GameManager`, or keeping UI logic in one `UIManager`. What matters is that unrelated responsibilities are not mixed.

## 2. Unity Concepts Used (beginner notes)

- **MonoBehaviour** — base class for scripts attached to GameObjects. `Update()` runs once per frame.
- **`Time.deltaTime`** — seconds since the last frame; multiply speeds by it so movement is frame-rate independent.
- **CharacterController** — a capsule-based mover with `Move()`, `isGrounded`, and collision handling. Not a `Rigidbody`.
- **Collider / Trigger** — colliders detect contact; `Is Trigger` colliders call `OnTriggerEnter` instead of physically blocking.
- **Tag** — a string label such as `Player` used for quick identification in collisions.
- **Layer** — a 32-slot bitmask used for physics filtering and optional dimension checks.
- **`SetActive(bool)`** — turns a GameObject (and its children) on or off. Used for dimension switching.
- **`Physics.SyncTransforms()`** — forces the physics engine to see transform/active changes immediately.
- **`Physics.CheckCapsule` / `Physics.Raycast`** — overlap and ground queries used for the safe-switch test.
- **Quaternion / `Transform.Rotate`** — rotation representation; mouse look uses Euler angles with clamped pitch.
- **Input System** — this project uses the new Input System only. Use `UnityEngine.InputSystem` (`Keyboard.current`, `Mouse.current`, or the generated actions asset).
- **SceneManager** — loads the scene for restart.
- **uGUI** — Unity's built-in UI system for text, buttons, and panels.

## 3. Planned Input Actions

The existing `InputSystem_Actions` asset already provides `Move`, `Look`, `Jump`, `Interact`, and `Cancel`. Two actions still need to be added during implementation:

- `SwitchDimension` — bound to Q.
- `Pause` — bound to Esc.

Existing unused actions (`Attack`, `Crouch`, `Sprint`, `Previous`, `Next`) are not part of this game. Leave them or remove them only when the input work is actually being done.

## 4. Data Flow (planned)

```
Input (WASD / Mouse / Space / E / Q / Esc)
        |
        v
PlayerController ---- Q ----> DimensionSwitcher ----> Dimension A/B SetActive
        |                            |
        |                            v
        |                        UIManager (dimension, SHIFT BLOCKED)
        |
        +---- touch trigger ----> Collectible ----> GameManager ----> UIManager (0/3)
        |
        +---- E near door ------> DoorController ---> GameManager (win if unlocked)
        |
        +---- kill zone --------> GameManager (lose) -> UIManager (Lose Screen)
```

## 5. What Is Not Decided

- Whether restart reloads the scene or resets state in place.
- Whether E is polled by the player or by the door.
- Whether dimension safety uses temporary toggling or dedicated layers.
- Final method and field names.
