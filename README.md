# Overview of "TokkiTopokki"

**TokkiTopokki** is a single-player, keyboard-controlled, console puzzle game written in **F#** using **.NET 10**. The player controls a rabbit named **Tokki** who explores a series of small grid-based levels collecting **Topokki** (rice cake) to recreate a lost recipe.

Each level is a fixed two-dimensional grid loaded from a plain text file. The player must collect all Topokkis in order to open the level's exit and then make their way into the exit, while avoiding spike traps, navigating paired teleporters, and pushing boxes in clever ways.

The game ends when the player completes all available levels, dies on a trap, or chooses to quit.

This project was developed for the **CS-20200 Programming Principles Spring 2026 Term Project**.

---

## 1 How to Run

To build and run the game you must have:

- .NET 10 SDK,
- a terminal that supports keyboard input and Unicode output.

The implementation is written in F# and no external libraries are required.

From the root directory of the repository, run:

```bash
dotnet run
```

The project file copies level files from the following directories into the build output:

```text
levels/
tutorial/
```

A normal repository layout should look like this:

```text
TokkiTopokki/
├── Board.fs
├── Loader.fs
├── Game.fs
├── Program.fs
├── TokkiTopokki.fsproj
├── levels/
│   ├── level1.txt
│   ├── level2.txt
│   └── ...
└── tutorial/
    ├── tutorial1_guide.txt
    ├── tutorial1.txt
    ├── tutorial2_guide.txt
    ├── tutorial2.txt
    └── ...
```

---

## 2 How to Play

When the game starts, it asks whether the player wants to play the tutorial levels first.

- If the player chooses to play tutorial, tutorial files from `tutorial/` are played first.
- Tutorial levels have text instructions from files `<tutorial_name>_guide.txt` displayed at the top.
- After all tutorial levels are complete, the game continues to the main levels from `levels/`.
- Player can refuse to go through tutorial, in which case they will immediately start playing levels from `levels/`.

After the final level, the game prints the total time across all completed levels.

---

## 3 Controls

| Key | Action |
|---|---|
| Arrow Up | Move Tokki up |
| Arrow Down | Move Tokki down |
| Arrow Left | Move Tokki left |
| Arrow Right | Move Tokki right |
| Q | Quit the game |
| R | Restart the level |
| y/n | Answer prompts |
| (any key) | When asked to press any key |

---

## 4 Board Legend

Each level is a two-dimensional grid loaded from a plain text file.

| Symbol | Meaning |
|---|---|
| `T` | Tokki, the player character |
| `O` | Box |
| `.` | Floor |
| `#` | Grass / wall-like non-walkable background |
| `=` | Fence / non-walkable internal barrier |
| `~` | Topokki collectible |
| `E` | Exit |
| `_` | Deactivated spike |
| `▲` | Activated spike |
| `A`, `B`, `C` | Teleport cells |

Only one dynamic object may occupy a cell at a time. Tokki and boxes are dynamic objects. Floor, grass, fences, spikes, exits, Topokkis, and teleports are static objects.

---

## 5 Rules

### 5.1 Movement

Tokki can:

- move one tile at a time,
- activate spikes and step on spikes (results in death),
- collect Topokkis,
- move a box one tile,
- teleport between open teleports,
- exit the level once exit is open.

Tokki cannot:

- leave the grid,
- enter grass (`#`),
- enter fences (`=`),
- enter a closed exit,
- push more than one box,
- enter a teleport blocked on the other end.

### 5.2 Topokki Collection

When Tokki steps onto a Topokki (cells shown as `~`):

1. the Topokki is collected,
2. the cell becomes floor,
3. the collected counter increases.

### 5.3 Exit

The exit (cell shown as `E`) is closed until every Topokki in the current level has been collected. Once all Topokkis are collected, the exit becomes open.

### 5.4 Spikes

There are two spike states:

- `_` deactivated and safe. Tokki survives stepping on a deactivated spike, but after Tokki leaves that spike, it becomes activated.
- `▲` activated and lethal. Tokki dies immediately when stepping on an activated spike. 

When a box is pushed onto a spike and later pushed off that spike, the spike toggles between deactivated and activated.

### 5.5 Boxes

When Tokki attempts to move into a box (cells shown as `O`), Tokki pushes the box one tile in the same direction. Tokki does not move into the box’s old position during that push. A box push succeeds only if the destination cell is valid for a box. If a box push fails, neither Tokki nor the box moves.

Valid destinations for a box are:

- floor,
- spikes (both activated and deactivated),
- unblocked teleport cells.

Invalid destinations for box are:

- Tokki,
- grass,
- fences,
- the exit (both open and closed),
- Topokki,
- another box,
- blocked teleporter cells.

### 5.6 Teleports

Teleport cells are represented by uppercase letters:

```text
A B C
```

Two cells with the same letter form a teleport pair.

When Tokki or a box enters one end of a teleport pair, it is relocated to the matching cell. If the matching cell is blocked, the move is refused and the entity stays in its original position.

For boxes, teleporting happens only when a move places the box onto a teleport cell. A box already standing on a teleport does not repeatedly teleport again without a new push.

### 5.7 Timing

Each level has an independent timer. The timer starts when the level begins and stops when Tokki reaches the open exit. After completing a level, the game displays the level time in this format:

```text
MM:SS.mmm
```

After the final level, the game displays the total time across all completed levels in the same format.

---

## 6 Changes from the Submitted Proposal

The final implementation mostly follows the subimtted proposal, except a few features:

### 6.1 Spike symbols were changed:

The symbols used for spike was changed from `^` into `▲` for better visibility. 

### 6.2 Optional tutorial mode was added:

Easy levels are boring, while interesting levels are hard. 

Which is why an optional quick tutorial was added to understand special interactions between objects.

### 6.3 Restart action button was added:

When a player want to restart the game they had to Quit (Q) and replay all levels from start.

Because in challenging levels Tokki is often "trapped", Restart (R) action was added to only restart current level.

---

## 7 LLM Usage

I have used LLM to assist me with:

- looking up the F# documentation (e.g. "Does F# have a function for time formatting?"),
- suggestions on consistent variable and function naming,
- improving the structure and wording of this README and checking that it explicitly matches the spec's requirements.

Still, most of things had to be done manually, such as: 

- the actual F# code was reviewed manually,
- level design for both tutorial and main levels,
- the final README wording was adjusted to avoid redundency and make it more readable. 


The main thing the LLM was not able to do correctly was verify whether the whole game actually satisfied my proposal. It could suggest explanations and wording, but it could not reliably test the gameplay experience, judge whether the levels were solvable and fair, or confirm that the implemented mechanics worked together as intended. Those parts required manual playtesting, manual inspection of the F# code, and adjustments to the level design.

---