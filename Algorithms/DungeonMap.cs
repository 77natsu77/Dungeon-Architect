public class DungeonMap
{
    public int Width, Height;
    public TileType[,] Tiles;

    public DungeonMap(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new TileType[width, height];
        // Initialize all tiles as Void
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                Tiles[x, y] = TileType.Void;
    }

    public void SetTile(int x, int y, TileType type)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            Tiles[x, y] = type;
    }

    public TileType GetTile(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            return Tiles[x, y];
        return TileType.Void; // Out of bounds treated as Void
    }

    public bool IsWalkable(int x, int y)
    {
        var tile = GetTile(x, y);
        return tile == TileType.Floor || tile == TileType.Corridor || tile == TileType.Door;
    }

    public void DrawRoom(Room room)
    {
        // We only carve the floor. 
        // We trust the "Wall Generator" to wrap it in stone later.
        for (int x = room.X; x < room.X + room.Width; x++)
        {
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                SetTile(x, y, TileType.Floor);
            }
        }
    }

    public void GenerateWalls()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (GetTile(x, y) == TileType.Floor || GetTile(x, y) == TileType.Corridor)
                {
                    // Check neighbors
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            if (GetTile(x + dx, y + dy) == TileType.Void)
                            {
                                SetTile(x + dx, y + dy, TileType.Wall);
                            }
                        }
                    }
                }
            }
        }
    }

    public void GenerateDoors()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (GetTile(x, y) == TileType.Corridor)
                {
                    // If a corridor tile is touching a floor tile, it's a doorway!
                    if (IsAdjacentTo(x, y, TileType.Floor))
                    {
                        SetTile(x, y, TileType.Door);
                    }
                }
            }
        }
    }

    // Helper method for the check
    private bool IsAdjacentTo(int x, int y, TileType type)
    {
        return GetTile(x - 1, y) == type || GetTile(x + 1, y) == type ||
            GetTile(x, y - 1) == type || GetTile(x, y + 1) == type;
    }

    public void Display()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                switch (GetTile(x, y))
                {
                    case TileType.Void:
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(' ');
                        break;
                    case TileType.Floor:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write('.');
                        break;
                    case TileType.Wall:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write('#');
                        break;
                    case TileType.Corridor:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write('+');
                        break;
                    case TileType.Door:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write('D');
                        break;
                }
            }
            Console.WriteLine();
        }
        Console.ResetColor();
    }
}