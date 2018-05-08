namespace Grooph
{
    class Path<TSeed> : IPath<TSeed>
        where TSeed : struct
    {
        public Path(Vertex[] vertexes, Edge[] edges, TSeed[] seeds, int depth)
        {
            Vertexes = vertexes;
            Edges = edges;
            Seeds = seeds;
            Depth = depth;
            FinalSeed = seeds[seeds.Length - 1];
        }

        public Vertex[] Vertexes { get; }
        public Edge[] Edges { get; }
        public TSeed[] Seeds { get; }
        public int Depth { get; }
        public TSeed FinalSeed { get; }

        public override string ToString() => this.Print(false, false);
    }
}
