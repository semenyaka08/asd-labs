namespace GraphDataStructure;

public class Edge
{
    public Edge(Vertex vertex1, Vertex vertex2)
    {
        From = vertex1;
        To = vertex2;
    }

    public Edge()
    {}
    
    public Vertex From { get; }

    public Vertex To { get; }

    public int Weight { get; set; }
}