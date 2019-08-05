namespace MapGenerator
{
    internal class ProbabilityTile
    {
        public int Index { get; set; }
        public Position2D Pos { get; set; }
        public char Content { get; set; }
        public double Probability { get; set; }

        public ProbabilityTile(int index, Position2D pos, char content, double probability = 0)
        {
            Index = index;
            Pos = pos;
            Content = content;
            Probability = probability;
        }
    }
}
