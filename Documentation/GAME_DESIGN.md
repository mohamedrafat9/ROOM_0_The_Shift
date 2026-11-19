# ROOM 0: THE SHIFT — Game Design

Status: design document, written before implementation. Nothing in this file is implemented yet unless explicitly stated.

## 1. Concept

**Title:** ROOM 0: THE SHIFT
**Genre:** First-person puzzle adventure
**Engine:** Unity 6 (6000.6.9f1), C#
**Setting:** One enclosed, open-chamber sci-fi room.
**Goal:** Collect 3 Energy Fragments and solve 3 dimension-based puzzles to unlock the exit and escape.

The game is intentionally small: one room, one exit, three fragments, three puzzles, two dimensions. There is no combat, timer, inventory, save system, or multiplayer.

## 2. Core Mechanic

The player presses **Q** to switch between two versions of the same room:

- **Dimension A — Stable Reality**
- **Dimension B — Hybrid Reality**

Both dimensions use the same room shell and the same scene. There are **not** two scenes. Two groups of GameObjects are toggled with `SetActive()`.

Switching is immediate. A switch is **blocked** when the destination would place the player inside solid geometry, or above a gap with no support. When blocked, the switch does not happen and the HUD shows a short **"SHIFT BLOCKED"** message.

## 3. Controls

| Input | Action |
|---|---|
| W A S D | Move |
| Mouse | Look / camera rotation |
| Space | Jump |
| E | Interact (exit door) |
| Q | Switch dimension |
| Esc | Pause / resume |

No sprint, no crouch.

## 4. Player Settings (initial values)

| Setting | Value |
|---|---|
| Controller type | `CharacterController` |
| Movement speed | 4.5 m/s |
| Jump height | ~0.8 m |
| Character height | 1.8 m |
| Camera height | 1.6 m |
| Gravity | -9.81 m/s² |

These are starting values only; they may be tuned after play-testing.

Note: a 0.8 m jump under -9.81 m/s² gravity needs an initial vertical speed of roughly `sqrt(2 * 9.81 * 0.8) ≈ 3.96 m/s`.

## 5. Dimensions

### Dimension A — Stable Reality
- Light gray / white environment.
- Cool white lighting.
- Cyan and light-blue details.
- Normal platforms and obstacles.

### Dimension B — Hybrid Reality
- Same room architecture.
- Dark navy and charcoal accents.
- Blue and violet lighting.
- Different platforms and paths.
- Some objects appear or disappear between dimensions.

Only visual/mesh objects change. The player, camera, UI, and global systems stay active in both dimensions.

## 6. Room Layout

Approximate dimensions: **12 m wide (X) × 18 m long (Z) × 5 m high (Y)**.

- Player starts at the **south** end (low Z).
- Exit is at the **north** end (high Z).
- A lower kill zone sits beneath the main floor; falling into it triggers the Lose Screen.
- Puzzle areas are distributed between the south and north ends.

Exact platform coordinates, puzzle trigger positions, and prop placement are **undecided** until scene construction begins.

## 7. Puzzles

### Puzzle 1 — The Missing Path
A platform needed to cross a gap exists only in the other dimension. The player must switch dimensions to make the platform appear, then cross.

### Puzzle 2 — The Hidden Passage
A barrier blocks the direct route here, but switching dimensions reveals an alternative route around it.

### Puzzle 3 — The Final Switch
The last fragment requires alternating dimensions and simple platform navigation to reach.

Fragment placement per puzzle is **undecided**; the plan is one fragment per puzzle area.

## 8. Game Flow

1. **Start Screen** — title, short English instructions, Start button.
2. **Gameplay** — first-person exploration and puzzle solving.
3. **HUD** — fragment counter (0/3), current dimension indicator (A/B), temporary "SHIFT BLOCKED" message.
4. **Pause Menu** — opened with Esc; resume and restart options.
5. **Exit Door** — locked until all 3 fragments are collected. Interact with E.
6. **Win Screen** — "You Escaped Room 0."
7. **Lose Screen** — shown when the player falls into the lower kill zone; includes Restart.

Fragments are collected automatically on touch (trigger collider). E is used only for the exit interaction.

## 9. Visual Direction

- Clean Sci-Fi / Liminal.
- Simple primitive geometry (cubes, planes, cylinders) with simple materials.
- Strong color contrast between dimensions: A = white/cyan, B = navy/violet.
- Lighting carries most of the mood; avoid heavy post-processing.
- Readable silhouettes and clear paths matter more than detail.

## 10. Explicit Non-Goals

- No timer, scoring system, inventory, crafting, or save/load.
- No enemies or combat.
- No multiple rooms or level streaming.
- No external art packs or high-poly assets.
- No complex shaders, custom render pipeline, or post-processing stack.
- No third-party packages beyond what the project already contains.

## 11. Open / Undecided Items

- Exact room blockout and platform coordinates.
- Puzzle placement and how each fragment is gated.
- UI visual style (fonts, panel colors) and whether the Start Screen lives in the same scene.
- Whether restart reloads the scene or resets state manually.
- Dimension-switch safety test implementation details (see `CODE_EXPLANATION.md`).
