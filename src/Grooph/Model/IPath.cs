namespace Grooph
{
    public interface IPath
    {
        Vertex[] Vertexes { get; }
        Edge[] Edges { get; }
        int Depth { get; }
    }


    public interface IPath<out TSeed> : IPath
        where TSeed : struct
    {
        TSeed[] Seeds { get; }
        TSeed FinalSeed { get; }
    }
}
