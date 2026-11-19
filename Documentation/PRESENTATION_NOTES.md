# ROOM 0: THE SHIFT — Presentation Notes

Use these notes to explain the project clearly. They describe the **design plus the current implementation state**; room blockout and the first-person controller are built, while dimensions, puzzles, and UI are still planned.

## 1. One-Sentence Summary

ROOM 0: THE SHIFT is a short first-person puzzle game set in a single sci-fi room where the player presses Q to switch between two versions of the same space in order to collect three Energy Fragments and unlock the exit.

## 2. The Idea

- **One room, two realities.** Dimension A is bright and stable; Dimension B is dark and hybrid.
- The same architecture exists in both, but platforms, barriers, and props differ.
- The player must think in two spaces at once: a path missing here exists there.
- Three fragments, three puzzles, one exit. Small scope on purpose.

## 3. Walkthrough of the Flow

1. Start Screen with title and short instructions.
2. Player spawns at the south end of the room.
3. HUD shows fragment counter (0/3) and current dimension.
4. Puzzle 1: a missing platform appears in the other dimension.
5. Puzzle 2: a barrier has a bypass only in the other dimension.
6. Puzzle 3: a final route requiring alternating switches.
7. Third fragment collected: the exit unlocks.
8. Player reaches the exit and presses E: Win Screen, "You Escaped Room 0."
9. Falling into the lower kill zone: Lose Screen with Restart.

## 4. How Each System Works

### Player Controller
- Uses Unity's `CharacterController` component, not a Rigidbody.
- WASD moves relative to where the camera is looking.
- The mouse rotates the player body left/right and the camera up/down.
- Gravity is applied manually and Space adds a jump impulse.
- Simple and predictable, which is important for platform puzzles.

### Dimension Switching
- One scene, two root groups of GameObjects: `DimensionA_Root` and `DimensionB_Root`.
- Q calls `SetActive(true)` on one group and `SetActive(false)` on the other.
- The switch is immediate.
- Before switching, the game checks whether the player would be inside geometry or over a gap in the destination dimension.
- If unsafe, the switch is cancelled and the HUD briefly shows "SHIFT BLOCKED."

### Collectibles
- Each fragment has a trigger collider.
- When the player enters, the fragment reports to `GameManager` and disappears.
- The counter updates to 1/3, 2/3, 3/3.

### Game Manager
- Stores the fragment count and whether the exit is unlocked.
- Decides win, lose, and restart.
- Keeping game state in one place avoids scattering logic across objects.

### Exit Door
- Locked until all three fragments are collected.
- E near the door interacts; if locked, it shows a short hint.
- Passing through after unlocking triggers the Win Screen.

### UI
- One Canvas with a HUD and four panels: Start, Pause, Win, Lose.
- Esc pauses and sets `Time.timeScale` to 0; resuming restores it to 1.

## 5. Technical Concepts to Mention

- **One scene, not two.** Less loading, fewer duplicated objects, and switching is instant.
- **`SetActive` groups.** Simple and readable compared to custom culling systems.
- **CharacterController.** Beginner-friendly first-person movement with built-in ground checks and collision.
- **Trigger colliders.** Used for pickups, exit zone, and kill zone.
- **Physics queries** (`CheckCapsule`, `Raycast`) for the safe-switch test.
- **New Input System.** The project is configured for it, so input goes through `UnityEngine.InputSystem`.
- **Primitive art + lighting.** Atmosphere comes from color and light rather than expensive assets.
- **Performance scope.** One small room, no post-processing stack, simple materials: suitable for an 8 GB MacBook Air M2.

## 6. Likely Instructor Questions and Answers

**Why use two dimensions instead of two levels?**
Because the design is one room. Reusing the same shell in one scene keeps the project small, makes switching instant, and forces the puzzle design to focus on spatial thinking rather than level count.

**How do you switch dimensions technically?**
Two parent GameObjects hold the objects for each dimension. Pressing Q toggles them with `SetActive()`. Shared geometry lives outside both groups so it stays visible.

**Why a CharacterController instead of a Rigidbody?**
It gives consistent walking, jumping, and slope behavior without physics tuning. Platform puzzles need predictable movement more than realistic physics.

**How do you prevent the player from switching into a wall or into thin air?**
Before the switch, the game temporarily enables the destination group and runs a capsule overlap check at the player's position plus a short downward ray for ground support. If either fails, it reverts the groups and shows "SHIFT BLOCKED."

**What does "SHIFT BLOCKED" mean?**
The destination is unsafe: there is no room for the player, or nothing to stand on.

**Why only three fragments and three puzzles?**
Scope control. A small number of well-made puzzles is better than a large unfinished game, especially for a solo university project.

**How is the UI implemented?**
With Unity's built-in uGUI: a Canvas, text elements for counter and dimension, and panels for the menus. Buttons call simple methods on the UI or game manager.

**How do you restart after falling?**
Either reload the scene or reset player position, fragments, and counters. Reloading is the simpler and safer option; the final choice is made during implementation.

**How did you keep performance low?**
Primitive meshes, a handful of materials, a few lights, one room, no post-processing, no imported art packs, and cached references instead of repeated lookups.

**What was the hardest part?**
The dimension safety check is the most subtle part, because the world changes while the player is standing in it. That is why it is isolated in a single clearly named method.

**What would you improve with more time?**
Better puzzle readability, a short audio pass, smoother dimension transition effects, and more play-testing. These are explicitly out of scope for now.

## 7. Demo Checklist

Before presenting:
- Confirm the scene is saved and added to Build Settings.
- Confirm the console has no errors.
- Test Q near a wall, over a gap, and in open space.
- Test all three fragments and the locked/unlocked door.
- Test Esc pause and resume.
- Test the kill zone and Restart.
- Have a backup: screenshots or a short recording in case of a live demo problem.

## 8. Honest Limitations

- No audio yet unless added later.
- No save system; the game is short by design.
- Dimension switching is a visual swap, not a shader transition.
- The safe-switch check is intentionally simple and may need tuning around edges.
- Room blockout and puzzle placement may change during implementation.
