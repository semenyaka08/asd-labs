namespace GraphDataStructure;

public class Edge(Vertex vertex1, Vertex vertex2)
{
    public Vertex From { get; set; } = vertex1;

    public Vertex To { get; set; } = vertex2;

    public int Weight { get; set; } = 1;
}