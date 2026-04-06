public struct Room
{
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }

    // Calculated properties
    public int Right => X + Width;
    public int Bottom => Y + Height;
    public int CenterX => X + (Width / 2);
    public int CenterY => Y + (Height / 2);

    public Room(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}

public enum TileType
{
    Void,      // Unreachable space
    Floor,     // Inside a room
    Wall,      // Room boundary
    Corridor,  // Connection between rooms
    Door       // Entry point
}


public class Leaf

{
    public int X, Y, Width, Height;
    public Leaf? LeftChild, RightChild;
    public Room Room; // Only null if this leaf was split further

    public Leaf(int x, int y, int width, int height)
    {
        X = x; Y = y; Width = width; Height = height;
    }

    public bool Split(int currentDepth, int maxDepth, Random rng)
    {
        // BASE CASE: Stop if we are too deep or the area is too small
        if (currentDepth >= maxDepth || Width < 10 || Height < 10) 
            return false; 

        // Determine direction 
        bool splitH = rng.NextDouble() > 0.5;
        if (Width > Height && Width / Height >= 1.25) splitH = false;
        else if (Height > Width && Height / Width >= 1.25) splitH = true;

        // Calculate the cut (Using MinSize)
        int max = (splitH ? Height : Width) - 6; // 6 is min room size
        if (max <= 6) return false; // Area too small to split safely

        int split = rng.Next(6, max);

        // Create the children
        if (splitH)
        {
            LeftChild = new Leaf(X, Y, Width, split);
            RightChild = new Leaf(X, Y + split, Width, Height - split);
        }
        else
        {
            LeftChild = new Leaf(X, Y, split, Height);
            RightChild = new Leaf(X + split, Y, Width - split, Height);
        }

        // Dive deeper 
        LeftChild.Split(currentDepth + 1, maxDepth, rng);
        RightChild.Split(currentDepth + 1, maxDepth, rng);

        return true; //successful split
    }

    public Room GetRoom()
    {
        if (Room.Width > 0) return Room;
        Room leftRoom = LeftChild?.GetRoom() ?? default;
        Room rightRoom = RightChild?.GetRoom() ?? default;

        if (leftRoom.Width > 0) return leftRoom;
        if (rightRoom.Width > 0) return rightRoom;

        return default; // No room found
    }
}

public class BSP
    {
        public void CreateRooms(Leaf node, DungeonMap map, Random rng)
        {
            if (node.LeftChild != null || node.RightChild != null)
            {
                // This is not a leaf, so keep diving down
                if (node.LeftChild != null) CreateRooms(node.LeftChild, map, rng);
                if (node.RightChild != null) CreateRooms(node.RightChild, map, rng);
                // After creating rooms in children, connect them with a corridor
                if (node.RightChild != null && node.LeftChild != null)
            {
                if (node.LeftChild.GetRoom().Width > 0 && node.RightChild.GetRoom().Width > 0) 
                CreateCorridor(node.LeftChild.GetRoom(), node.RightChild.GetRoom(), map, rng);
            }
            }
                
            else
            {
                // This IS a leaf! Let's put a room in it.
                // 1. Create a Room object with a random size inside this leaf
                // 2. Tell the map to draw it
                int RoomX = node.X + 1; // Leave a 1-tile border for walls
                int RoomY = node.Y + 1;
                int RoomWidth = Math.Max(6, node.Width - 2); // Ensure at least 1 tile for floor
                int RoomHeight = Math.Max(6, node.Height - 2);
                node.Room = new Room(RoomX, RoomY, RoomWidth, RoomHeight);
                map.DrawRoom(node.Room);
            }
        }
    
        public void CreateCorridor(Room roomA, Room roomB, DungeonMap map, Random rng)
        {
            // Simple L-shaped corridor
            int x1 = roomA.CenterX;
            int y1 = roomA.CenterY;
            int x2 = roomB.CenterX;
            int y2 = roomB.CenterY;

            if (rng.NextDouble() < 0.5)
            {
                // Horizontal first
                for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                    if (map.GetTile(x,y1) == TileType.Void) map.SetTile(x, y1, TileType.Corridor);
                for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                    if (map.GetTile(x2, y) == TileType.Void) map.SetTile(x2, y, TileType.Corridor);
            }
            else
            {
                // Vertical first
                for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                    if (map.GetTile(x1, y) == TileType.Void) map.SetTile(x1, y, TileType.Corridor);
                for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                    if (map.GetTile(x, y2) == TileType.Void) map.SetTile(x, y2, TileType.Corridor);
            }
        }

        
    }