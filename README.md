# TokkiTopokki

TokkiTopokki is a console-based puzzle game written in F# for .NET 10. The player controls Tokki, a rabbit who collects all topokki in each level, opens the exit, and escapes while dealing with boxes, spikes, and teleporters.

## Run

From the repository root:

```bash
dotnet run
```

The game loads files from `levels/` and, optionally, `tutorial/`, so it should be run from the project root directory.

## Controls

| Key | Action |
|---|---|
| Arrow keys | Move |
| R | Restart current level |
| Q | Quit |
| Y/N | Answer prompts |

## Symbols

| Symbol | Meaning |
|---|---|
| `T` | Tokki |
| `O` | Box |
| `.` | Floor |
| `#` | Grass / wall |
| `=` | Fence / wall |
| `~` | Topokki |
| `E` | Exit |
| `_` | Deactivated spike |
| `▲` | Activated spike |
| `A`, `B`, `C` | Teleport pairs |

## Rules

Collect every `~` to open the exit. Step on `E` after the exit opens to complete the level.

Tokki cannot move through walls, boxes, the edge of the board, or a closed exit. Boxes can be pushed onto floor, spikes, and unblocked teleporters, but not into walls, topokki, exits, Tokki, or other boxes.

A deactivated spike `_` is safe, but after Tokki leaves it, it becomes activated. Stepping on an activated spike `▲` kills Tokki. Boxes can toggle spikes when moved off them.

Teleport cells with the same letter are paired. Entering one moves the entity to the other if the destination is not blocked.

## Level Files

Main levels are stored in `levels/` as `.txt` files and are played in alphabetical order.

Tutorial levels are stored in `tutorial/`. A tutorial may also have a matching guide file named with `_guide.txt`.

Each level must be rectangular and contain exactly one `T`.

## Requirement Changes

The final version mostly follows the proposal. The following small changes were made:

- Deactivated and activated spikes are rendered as `_` and `▲` for better visibility.
- Optional tutorial levels were added.
- `R` was added to restart the current level at any time.

## Author

Nurtilek Duishobaev  
CS-20200 Programming Principles Term Project, Spring 2026
