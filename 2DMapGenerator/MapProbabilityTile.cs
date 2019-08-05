namespace MapGenerator
{
    internal class MapProbabilityTile
    {
        public int Index { get; set; }
        public MapPosition2D Pos { get; set; }
        public char Content { get; set; }
        public double Probability { get; set; }

        public MapProbabilityTile(int index, MapPosition2D pos, char content, double probability = 0)
        {
            Index = index;
            Pos = pos;
            Content = content;
            Probability = probability;
        }
    }
}
