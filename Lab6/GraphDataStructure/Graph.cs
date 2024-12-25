namespace GraphDataStructure;

public class Graph
{
    public Graph(bool isDirected)
    {
        IsDirected = isDirected;
    }
    
    public readonly bool IsDirected;
    
    public readonly List<Vertex> Vertices = [];
    
    public readonly List<Edge> Edges = [];
    
    public int[,] GetMatrix()
    {
        var matrix = new int[Vertices.Count, Vertices.Count];

        foreach (var edge in Edges)
        {
            matrix[edge.From.CurrentId - 1, edge.To.CurrentId - 1] = 1;

            if (!IsDirected) matrix[edge.To.CurrentId - 1, edge.From.CurrentId - 1] = 1;
        }
        
        return matrix;
    }
    
    public void AddVertex(int value)
    {
        Vertex newVertex = new Vertex(value);
        Vertices.Add(newVertex);
    }
    public void AddVertex(Vertex vertex)
    {
        Vertices.Add(vertex);
    }

    public void AddEdge(Vertex v1, Vertex v2)
    {
        if (Edges.Any(edge => edge.From == v1 && edge.To == v2))
            return;
        
        Edge firstEdge = new Edge(v1, v2);
        Edges.Add(firstEdge);
        v1.Edges.Add(firstEdge);

        if (!IsDirected)
        {
            if(v1 == v2) return;
            Edge secondEdge = new Edge(v2, v1);
            Edges.Add(secondEdge);
            v2.Edges.Add(secondEdge);
        }
    }
    public void AddEdge(Edge edge)
    {
        if (Edges.Any(k => k.From == edge.From && k.To == edge.To))
            return;
        
        Edges.Add(edge);
    }

    private void MatrixGenerationGraph(double[,] matrix)
    {
        for (int i = 0;i<matrix.GetLength(0);i++)
        {
            for (int z =0;z<matrix.GetLength(1);z++)
            {
                if (matrix[i,z] == 1)
                {
                    AddEdge(Vertices.First(p=>p.CurrentId == i+1), Vertices.First(p=>p.CurrentId == z+1));
                }
            }
        }
    }

    private double[,] MatrixForDirected(double k)
    {
        int variant = 3421;
        
        int n = 12; 
        double[,] adjacencyMatrix = new double[n, n];
        Random rnd = new Random(variant);
        
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                adjacencyMatrix[i, j] =  rnd.NextDouble() * 2.0 * k;
                if (k != 1)
                    adjacencyMatrix[i, j] = Round(adjacencyMatrix[i, j]);
            }
        }
        
        return adjacencyMatrix;
    }

    private double[,] MatrixForUnDirected(double[,] adjacencyMatrix)
    {
        int n = 12;
        double[,] undirectedAdjMatrix = new double[n, n]; 

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (adjacencyMatrix[i, j] == 1 )
                {
                    undirectedAdjMatrix[i, j] = 1;
                    undirectedAdjMatrix[j, i] = 1;
                }
            }
        }

        return undirectedAdjMatrix;
    }

    public int[,] GenerateWeightMatrix()
    {
        double[,] b = MatrixForDirected(1);
        double[,] a = MatrixForUnDirected(MatrixForDirected(1.0 - 2 * 0.01 - 1 * 0.005 - 0.05));
        int[,] c = new int[12,12];
        for (int i = 0;i<c.GetLength(1);i++)
        {
            for (int j = 0; j < c.GetLength(0); j++)
            {
                c[i, j] = (int)Math.Round(b[i,j] * 100 * a[i,j] );
            }
        }

        int[,] d = new int[12, 12];
        for (int i = 0;i<d.GetLength(0);i++)
        {
            for (int j = 0; j < d.GetLength(1); j++)
            {
                if (c[i, j] > 0)
                    d[i, j] = 1;
            }
        }

        int[,] h = new int[12, 12];
        for (int i = 0;i<h.GetLength(0);i++)
        {
            for (int j = 0;j<h.GetLength(1);j++)
            {
                if (d[i, j] != d[j, i])
                    h[i, j] = 1;
            }
        }

        int[,] tr = new int[12, 12];
        for (int i = 0;i<tr.GetLength(0);i++)
        {
            for (int j = i;j<tr.GetLength(1);j++)
            {
                tr[i, j] = 1;
            }
        }

        int[,] weight = new int[12, 12];
        for (int i = 0;i<weight.GetLength(0);i++)
        {
            for (int j = i, z = 0;j<weight.GetLength(1);j++)
            {
                weight[i, j] =  (d[i,j] + h[i,j] * tr[i, j]) * c[i, j];
                weight[j, i] = weight[i, j];
            }
        }
        
        for (int i = 0;i<weight.GetLength(0);i++)
        {
            for (int j = 0;j<weight.GetLength(1);j++)
            {
                if (weight[i, j] != 0)
                {
                    Edge edge = Edges.First(p => p.From.CurrentId == i + 1 && p.To.CurrentId == j + 1);
                    edge.Weight = weight[i, j];
                }
            }
        }

        return weight;
    }
    
    public void GenerateMatrix()
    {
        double k = 1.0 - 2 * 0.01 - 1 * 0.005 - 0.05;
        if (IsDirected) MatrixGenerationGraph(MatrixForDirected(k));
        else MatrixGenerationGraph(MatrixForUnDirected(MatrixForDirected(k)));
    }
    
    static int Round(double num)
    {
        if (num < 1) return 0;
        return 1;
    }
}