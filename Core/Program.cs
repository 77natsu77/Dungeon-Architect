using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("--- THE DUNGEON ARCHITECT ---");
        
        string seed = "MyPersonalDungeon"; // Console.ReadLine();
        Random rng = new Random(seed.GetHashCode()); //could enter seed to generate the same dungeon
 
        DungeonGenerator architect = new DungeonGenerator();

        DungeonMap map = architect.Generate(20, 20, rng, 4);
    
        map.Display();

        Console.ReadLine();
    }
}