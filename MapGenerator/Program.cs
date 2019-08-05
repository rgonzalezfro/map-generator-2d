using MapGenerator;
using System;

namespace MapCreatorApp
{
    class Program
    {
        static int _hSize = 15;
        static int _vSize = 20;
        static int _creationPoints = 1;
        static int _creationIterations = 1;

        static void Main(string[] args)
        {
            var map = new MapManager();
            var selection = 2;
            while (selection != 4)
            {
                Console.Clear();
                Console.WriteLine($"1- Edit variables: \n\tHorizontal size: {map.HorizontalSize} \n\tVertical size: {map.VerticalSize} \n\tLand creation points: {map.CreationPoints} \n\tMoves per iteration: {map.CreationIterations}");
                Console.WriteLine($"2- Regenerate grid");
                Console.WriteLine($"3- Fill grid");
                Console.WriteLine($"4- Exit\n");
                switch (selection)
                {
                    case 1:
                        EditVariables();
                        map.UpdateProperties(_vSize, _hSize, _creationPoints, _creationIterations);
                        break;
                    case 2:
                        map.Reinitialize();
                        DisplayGrid(map.GetMap());
                        break;
                    case 3:
                        map.CreateTerrain();
                        DisplayGrid(map.GetMap());
                        break;
                }
                Console.WriteLine($"\nSelect an option");
                selection = GetNewInt(selection);
            }
        }

        /// <summary>
        /// Get new vaules for the map
        /// </summary>
        private static void EditVariables()
        {
            Console.WriteLine("Enter horizontal size:");
            _hSize = GetNewInt(_hSize);
            Console.WriteLine("Enter vertical size:");
            _vSize = GetNewInt(_vSize);
            Console.WriteLine("Enter amount of land creation points:");
            _creationPoints = GetNewInt(_creationPoints);
            Console.WriteLine("Enter amount of moves per iteration:");
            _creationIterations = GetNewInt(_creationIterations);
        }

        /// <summary>
        /// Writes the grid to the console
        /// </summary>
        private static void DisplayGrid(char[,] grid)
        {
            int hSize = grid.GetLength(0);
            int vSize = grid.GetLength(1);

            for (int vIndex = 0; vIndex < vSize; vIndex++)
            {
                for (int hIndex = 0; hIndex < hSize; hIndex++)
                {
                    Console.Write(" " + grid[hIndex, vIndex]);
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Tries to get a new value from the console, if there is an exception returns the value passed by parameter
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        private static int GetNewInt(int oldValue)
        {
            try
            {
                return Int32.Parse(Console.ReadLine());
            }
            catch
            {
                return oldValue;
            }
        }
    }
}
