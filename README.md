#  Dungeon Architect

> A procedural dungeon generator built in C# that uses **Binary Space Partitioning (BSP)** to recursively carve a 2D tile map into rooms, corridors, walls, and doors — producing a unique, fully connected dungeon layout from a simple text seed.

---

##  Example Output

```
####################
# . . . . # . . . #
# . . . . D + + + #
# . . . . # .   . #
######D####  .   . #
      #    . . . . #
      # + + D . . #
      #   . . . . #
      ##############
```

*(Console-rendered with colour-coded tiles: `.` Floor · `#` Wall · `+` Corridor · `D` Door)*

---

## Analysis & A-Level STEM Significance

This project implements a **procedural content generation (PCG)** algorithm — a topic sitting at the intersection of **Computer Science, Discrete Mathematics, and Game Engineering**.

The core technique, Binary Space Partitioning, is the same family of spatial data structures used in:

- **Compiler theory** — expression trees and recursive descent parsing
- **Computer graphics** — BSP trees were the technology behind *Doom* (1993) for hidden-surface removal
- **Computational geometry** — spatial indexing and collision detection in game engines
- **Operating systems** — memory allocators (buddy system) use an almost identical recursive halving strategy

At A-Level, this project demonstrates **recursion**, **tree traversal**, **2D array manipulation**, **enumerations**, **structs**, and **separation of concerns** — all within a single, self-contained C# console application.

---

## Technical Depth

### 1. Binary Space Partitioning — Recursive Tree Building

The map starts as one large rectangle. The `Leaf.Split()` method recursively divides it into a **binary tree** of sub-regions:

```
Root (full map)
├── LeftChild  (top half)
│   ├── LeftChild  (top-left)  ← room placed here
│   └── RightChild (top-right) ← room placed here
└── RightChild (bottom half)
    ├── LeftChild  (bottom-left)  ← room placed here
    └── RightChild (bottom-right) ← room placed here
```

Each split is randomised, but biased — if a region is 1.25× wider than it is tall, it splits **vertically** (and vice versa), preventing long, thin, unplayable slivers. This is a key heuristic that distinguishes a good PCG algorithm from a naïve one.

**Base cases** stop the recursion when:
- The region is smaller than a minimum tile threshold (10×10), OR
- The maximum recursion depth is reached (`maxDepth`, default `4`)

```csharp
if (currentDepth >= maxDepth || Width < 10 || Height < 10)
    return false;
```

This is a classic example of a **bounded recursive algorithm**, analogous to merge sort's depth limit.

### 2. Post-Order Tree Traversal — Room & Corridor Placement

`BSP.CreateRooms()` uses **post-order traversal**: it recurses down to the leaf nodes first, places a room, then connects sibling rooms with a corridor on the way back up.

```
Visit Left Child → Visit Right Child → Connect them
```

This guarantees **full connectivity** — every room is reachable from every other room, since corridors are laid at every level of the tree.

### 3. L-Shaped Corridor Generation

Corridors between rooms are drawn as **L-shaped paths** connecting the centre points of two rooms. The horizontal-or-vertical-first choice is randomised (50/50), which produces natural-looking junctions and prevents a grid-like feel.

```csharp
// Horizontal leg first, then vertical
for (int x = Min(x1,x2); x <= Max(x1,x2); x++) SetTile(x, y1, Corridor);
for (int y = Min(y1,y2); y <= Max(y1,y2); y++) SetTile(x2, y, Corridor);
```

Crucially, the code only overwrites `Void` tiles — it never overwrites already-carved floor space, preventing corridors from destroying rooms they pass through.

### 4. Tile-Neighbour Wall & Door Generation

After rooms and corridors are carved, walls and doors are generated in a single `O(W × H)` scan of the 2D grid:

- **Walls** — any `Void` tile adjacent (including diagonals) to a `Floor` or `Corridor` tile becomes a `Wall`
- **Doors** — any `Corridor` tile that is orthogonally adjacent to a `Floor` tile becomes a `Door`

This **post-processing pass** cleanly separates *carving* (the BSP phase) from *decorating* (the wall/door phase), a key principle of good software architecture.

### 5. Seeded Determinism

```csharp
Random rng = new Random(seed.GetHashCode());
```

By hashing a text string into an integer seed, the same dungeon can be reproduced exactly from the same string — a standard technique in procedural generation (used in Minecraft's world seeds, for example).

---

## Project Structure

```
Dungeon-Architect/
├── Algorithms/
│   ├── BSPBuilder.cs        # Leaf struct, BSP class — the recursive engine
│   ├── DungeonGenerator.cs  # Orchestrates the full generation pipeline
│   ├── DungeonMap.cs        # 2D tile grid, wall/door passes, console renderer
│   └── CorridorGenerator.cs # (reserved for future corridor strategies)
├── Core/
│   └── Program.cs           # Entry point — seed input, display
└── DungeonArchitect.csproj  # .NET 8 project file
```

---

## Installation & Usage

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run

```bash
git clone https://github.com/77natsu77/Dungeon-Architect.git
cd Dungeon-Architect
dotnet run
```

### Change the Dungeon

Edit the seed string in `Core/Program.cs` to get a different — but reproducible — layout:

```csharp
string seed = "MyPersonalDungeon"; // Change this to any string
```

Or change the map dimensions and BSP depth:

```csharp
DungeonMap map = architect.Generate(
    width:    40,   // Map width  (tiles)
    height:   30,   // Map height (tiles)
    rng:      rng,
    maxDepth: 5     // More depth = more, smaller rooms
);
```

### Console Legend

| Symbol | Tile Type | Colour |
|--------|-----------|--------|
| `.`    | Floor     | Gray   |
| `#`    | Wall      | Dark Gray |
| `+`    | Corridor  | Yellow |
| `D`    | Door      | Green  |
| ` `    | Void      | Black  |

---

## Learning Outcomes

Building this project developed mastery in the following areas:

**Algorithms & Data Structures**
- Implementing a **binary tree** from scratch using linked node objects (`Leaf`)
- Understanding **post-order tree traversal** and why the order matters for dependent operations (you must have rooms before you can connect them)
- Applying a **bounded recursive algorithm** with multiple base cases

**Procedural Generation Theory**
- The difference between *carving* and *decorating* phases in map generation
- Using **aspect-ratio heuristics** to produce balanced spatial partitions
- How **seeded randomness** enables reproducibility without sacrificing variety

**C# & .NET**
- Using `struct` vs `class` for value-type semantics (`Room` as a struct)
- Nullable reference types (`Leaf? LeftChild`) and null-coalescing operators
- `enum` for type-safe tile categorisation
- Separating concerns across files and namespaces in a multi-file project

**Software Design**
- The **pipeline pattern**: generate → decorate → render as distinct, testable stages
- Writing **bounds-checked accessors** to prevent index-out-of-range bugs in 2D arrays
- Keeping rendering logic (`Display()`) separate from generation logic

---

## Possible Extensions

- [ ] Add a `Display` folder / class with colour themes or ASCII art tile sets
- [ ] Implement a proper `CorridorGenerator` strategy interface for winding paths
- [ ] Add enemy/item spawn points at room centres
- [ ] Export to JSON for use in a Unity or Godot game engine
- [ ] Implement A* pathfinding to verify all rooms are reachable

---

## License

MIT — free to use, extend, and learn from.
