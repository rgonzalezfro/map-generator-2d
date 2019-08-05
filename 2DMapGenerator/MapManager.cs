using System;
using System.Collections.Generic;
using System.Linq;

namespace MapGenerator
{
    public class MapManager
    {
        #region Properties

        public int HorizontalSize { get; set; }
        public int VerticalSize { get; set; }
        public int CreationPoints { get; set; }
        public int CreationIterations { get; set; }

        private Random _random = new Random();
        private char[,] _grid;
        private Position2D[] _creationPositionArray;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of a map
        /// </summary>
        public MapManager(int vSize = 20, int hSize = 20, int creationPoints = 2, int creationIterations = 30)
        {
            UpdateProperties(vSize, hSize, creationPoints, creationIterations);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the properties and resets the map
        /// </summary>
        /// <param name="vSize"></param>
        /// <param name="hSize"></param>
        /// <param name="creationPoints"></param>
        /// <param name="creationIterations"></param>
        public void UpdateProperties(int vSize, int hSize, int creationPoints, int creationIterations)
        {
            VerticalSize = vSize;
            HorizontalSize = hSize;
            CreationPoints = creationPoints;
            CreationIterations = creationIterations;

            Initialize();
            GenerateCreationPoints(CreationPoints);
        }

        /// <summary>
        /// Resets the map
        /// </summary>
        public void Reinitialize()
        {
            Initialize();
        }

        /// <summary>
        /// Returns a copy of the map
        /// </summary>
        /// <returns></returns>
        public char[,] GetMap()
        {
            return (char[,])_grid.Clone();
        }

        /// <summary>
        /// Tries to generate new tiles of terrain by CreationIterations
        /// </summary>
        public void CreateTerrain()
        {
            MoveCreationPoints(CreationIterations);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Initializes an empty grid
        /// </summary>
        private void Initialize()
        {
            _grid = new char[HorizontalSize, VerticalSize];
            for (int hIndex = 0; hIndex < HorizontalSize; hIndex++)
            {
                for (int vIndex = 0; vIndex < VerticalSize; vIndex++)
                {
                    _grid[hIndex, vIndex] = '_';
                }
            }
        }

        /// <summary>
        /// Generates the points that will create terrain tiles
        /// </summary>
        /// <param name="quantity"></param>
        private void GenerateCreationPoints(int quantity)
        {
            _creationPositionArray = new Position2D[quantity];
            for (int creationPoint = 0; creationPoint < quantity; creationPoint++)
            {
                var xCoord = _random.Next(HorizontalSize);
                var yCoord = _random.Next(VerticalSize);
                if (_grid[xCoord, yCoord] == '_')
                {
                    _grid[xCoord, yCoord] = 'X';
                }
                _creationPositionArray[creationPoint] = new Position2D(xCoord, yCoord);
            }
        }

        /// <summary>
        /// Move the creation points and creates terreain in each move by the specified amount of times
        /// </summary>
        /// <param name="movements"></param>
        private void MoveCreationPoints(int movements)
        {
            for (int iteration = 0; iteration < movements; iteration++)
            {
                for (int creationPointIndex = 0; creationPointIndex < CreationPoints; creationPointIndex++)
                {
                    var pos = _creationPositionArray[creationPointIndex];
                    var surrounding = GetSurroundingTiles(pos);

                    var maxTile = GetSurroundingMovingProbability(surrounding).OrderByDescending(t => t.Probability).First();

                    _grid[pos.x, pos.y] = 'X';
                    _grid[maxTile.Pos.x, maxTile.Pos.y] = 'O';
                    _creationPositionArray[creationPointIndex] = maxTile.Pos;
                }
            }
        }

        /// <summary>
        /// Calculate the probability to move to each nearby tile
        /// </summary>
        /// <param name="surroundingTiles"></param>
        /// <returns></returns>
        private IEnumerable<ProbabilityTile> GetSurroundingMovingProbability(IEnumerable<ProbabilityTile> surroundingTiles)
        {
            foreach (var tile in surroundingTiles)
            {
                double probability = 0;
                var leftIndex = (tile.Index > 0) ? tile.Index - 1 : 7;
                var leftTile = surroundingTiles.Where(tl => tl.Index == leftIndex).FirstOrDefault();

                var rightIndex = (tile.Index < 7) ? tile.Index + 1 : 0;
                var rightTile = surroundingTiles.Where(tl => tl.Index == rightIndex).FirstOrDefault();

                probability += (tile.Content != 'X') ? _random.Next(1, 19) * 0.01 : _random.Next(40, 60) * (-0.01);
                if (leftTile != null && leftTile.Content == 'X')
                {
                    probability += _random.Next(20, 40) * 0.01;
                }
                if (rightTile != null && rightTile.Content == 'X')
                {
                    probability += _random.Next(20, 40) * 0.01;
                }
                tile.Probability = probability;
            }
            return surroundingTiles;
        }

        /// <summary>
        /// Validate the tile at the coordinates provided exists and add it to the list passed by parameter if possible
        /// </summary>
        /// <param name="probabilityTiles"></param>
        /// <param name="xCoord"></param>
        /// <param name="yCoord"></param>
        /// <param name="tileIndex"></param>
        private void AddSurroundingTile(List<ProbabilityTile> probabilityTiles, int xCoord, int yCoord, int tileIndex)
        {
            var inBoundX = xCoord >= 0 && xCoord < HorizontalSize;
            var inBoundY = yCoord >= 0 && yCoord < VerticalSize;
            if (inBoundX && inBoundY)
            {
                var tile = new ProbabilityTile(tileIndex, new Position2D(xCoord, yCoord), _grid[xCoord, yCoord]);
                probabilityTiles.Add(tile);
            }
        }

        /// <summary>
        /// Get an array with surrounding values (null for tiles outside grid)
        /// </summary>
        /// <param name="creationPosition"></param>
        /// <returns></returns>
        private IEnumerable<ProbabilityTile> GetSurroundingTiles(Position2D creationPosition)
        {
            var surroundingTiles = new List<ProbabilityTile>();
            var surroundIndex = 0;

            int xSearchIndex = creationPosition.x - 1;
            int ySearchIndex = creationPosition.y - 1;

            for (int x = xSearchIndex; x <= creationPosition.x + 1; x++)
            {
                xSearchIndex = x;
                AddSurroundingTile(surroundingTiles, xSearchIndex, ySearchIndex, surroundIndex);
                surroundIndex++;
            }
            ySearchIndex++;
            for (int y = ySearchIndex; y <= creationPosition.y + 1; y++)
            {
                ySearchIndex = y;
                AddSurroundingTile(surroundingTiles, xSearchIndex, ySearchIndex, surroundIndex);
                surroundIndex++;
            }
            xSearchIndex--;
            for (int x = xSearchIndex; x >= creationPosition.x - 1; x--)
            {
                xSearchIndex = x;
                AddSurroundingTile(surroundingTiles, xSearchIndex, ySearchIndex, surroundIndex);
                surroundIndex++;
            }
            ySearchIndex--;
            for (int y = ySearchIndex; y > creationPosition.y - 1; y--)
            {
                ySearchIndex = y;
                AddSurroundingTile(surroundingTiles, xSearchIndex, ySearchIndex, surroundIndex);
                surroundIndex++;
            }
            return surroundingTiles;
        }

        #endregion

    }
}
