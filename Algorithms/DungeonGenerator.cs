public class DungeonGenerator
{
    public DungeonMap Generate(int width, int height, Random rng, int maxDepth = 4)
    {
        DungeonMap map = new DungeonMap(width, height);
        Leaf root = new Leaf(0, 0, width, height);
        root.Split(0, maxDepth, rng);

        BSP bsp = new BSP();
        bsp.CreateRooms(root, map, rng);

         map.GenerateWalls(); 
        map.GenerateDoors();
        return map;
    }
}