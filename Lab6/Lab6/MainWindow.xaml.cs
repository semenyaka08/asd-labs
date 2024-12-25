using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphDataStructure;

namespace Lab6;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly Graph _graph = new(false);

    private readonly Dictionary<Edge, Point> _middlePoint = new();
    
    private readonly Dictionary<(Vertex, Vertex), Line> _drawnLines = new();

    private readonly Dictionary<Vertex, Ellipse> _drawnCircles = new();
    
    private readonly Graph _minSpanningTree = new(false);
    
    private int _index;
    
    public MainWindow()
    {
        InitializeComponent();
        _graph.AddVertex(1);
        _graph.AddVertex(2);
        _graph.AddVertex(3);
        _graph.AddVertex(4);
        _graph.AddVertex(5);
        _graph.AddVertex(6);
        _graph.AddVertex(7);
        _graph.AddVertex(8);
        _graph.AddVertex(9);
        _graph.AddVertex(10);
        _graph.AddVertex(11);
        _graph.AddVertex(12);
        
        _graph.GenerateMatrix();
        
        DisplayMatrix(_graph.GetMatrix());
        
        var dictionary = new Dictionary<Vertex, Coordinates>();
        
        ArrangeVerticesInCircle(683, 352, 300, dictionary);
        
        foreach (var vertex in _graph.Vertices)
        {
            foreach (var edge in vertex.Edges)
            {
                DrawEdge(edge, dictionary);
            }
        }
        
        var weightMatrix= _graph.GenerateWeightMatrix();

        var drawSpanningTree = new Button
        {
            Content = "draw spanning tree",
            Width = 115,
            Height = 50,
        };

        drawSpanningTree.Click += drawSpanningTree_Click;
        
        Canvas.SetLeft(drawSpanningTree, 50);
        Canvas.SetTop(drawSpanningTree, 50);
        
        Canvas.Children.Add(drawSpanningTree);
        
        foreach (var edge in _graph.Edges)
        {
            if(edge.From.CurrentId != edge.To.CurrentId)
                DrawWeight(edge);
        }

        DisplayMatrix(weightMatrix);

        FindMinimumSpanningTree();
    }

    private void drawSpanningTree_Click(object sender, RoutedEventArgs e)
    {
        if (_index < _minSpanningTree.Edges.Count)
        {
            _drawnLines[(_minSpanningTree.Edges[_index].From, _minSpanningTree.Edges[_index].To)].Stroke =
                Brushes.Fuchsia;
            _drawnLines[(_minSpanningTree.Edges[_index].To, _minSpanningTree.Edges[_index].From)].Stroke =
                Brushes.Fuchsia;
            _drawnCircles[_minSpanningTree.Vertices[_index]].Fill = Brushes.Fuchsia;
            _drawnCircles[_minSpanningTree.Vertices[_index+1]].Fill = Brushes.Fuchsia;
            
            _index++;
        }
    }
    
    private void FindMinimumSpanningTree()
    {
        _minSpanningTree.AddVertex(_graph.Vertices[0]);
        
        var lessWeightEdge = _graph.Vertices[0].Edges[0];

        while (_minSpanningTree.Vertices.Count != _graph.Vertices.Count)
        {
            for (int i = 0; i < _minSpanningTree.Vertices.Count; i++)
            {
                foreach (var edge in _minSpanningTree.Vertices[i].Edges)
                {
                    if (lessWeightEdge.Weight > edge.Weight && !_minSpanningTree.Vertices.Contains(edge.To) && edge.Weight != 0)
                    {
                        lessWeightEdge = edge;
                    }
                }
            }

            _minSpanningTree.AddVertex(lessWeightEdge.To);
            _minSpanningTree.AddEdge(lessWeightEdge);
            lessWeightEdge = _graph.Vertices[0].Edges[0];
        }
    }
    
    private void DisplayMatrix<T>(T[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write(matrix[i, j] + "  ");
            }
            Console.WriteLine();
        }
    }
    
    private void ArrangeVerticesInCircle(double centreX,double centreY,int radius, Dictionary<Vertex, Coordinates> dictionary)
    {
        int angleIncrement = 360 / _graph.Vertices.Count;
        for (int i = 0; i < _graph.Vertices.Count; i++)
        {
            double angle = i * angleIncrement;
            double angleRad = angle * Math.PI / 180; 

            double x = centreX + radius * Math.Cos(angleRad);
            double y = centreY + radius * Math.Sin(angleRad);

            Coordinates coordinates = new Coordinates(x, y);
            dictionary.Add(_graph.Vertices.First(p=>p.CurrentId == i+1), coordinates);
            DrawVertex(coordinates, _graph.Vertices.First(p=>p.CurrentId == i+1));
        }
    }
    
    private void DrawVertex(Coordinates coordinates, Vertex vertex)
    {
        var ellipse = new Ellipse
        {
            Width = 30,
            Height = 30,
            Fill = Brushes.LightBlue,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };
        
        _drawnCircles.Add(vertex, ellipse);
        
        Canvas.SetLeft(ellipse, coordinates.X - 15);
        Canvas.SetTop(ellipse, coordinates.Y - 15);
        Canvas.Children.Add(ellipse);
        
        var textBlock = new TextBlock
        {
            Text = vertex.Value.ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        Canvas.Children.Add(textBlock);
        
        Canvas.SetLeft(textBlock, (coordinates.X + ellipse.Width / 2 - textBlock.ActualWidth / 2)-19);
        Canvas.SetTop(textBlock, (coordinates.Y + ellipse.Height / 2 - textBlock.ActualHeight / 2)-24);
    }

    private void DrawEdge(Edge edge, Dictionary<Vertex, Coordinates> dictionary)
    {
        if (edge.From.CurrentId == edge.To.CurrentId)
        {
            var ellipse = new Ellipse()
            {
                Width = 15,
                Height = 15,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            
            const double koef = 322.5 / 300;
        
            Canvas.SetLeft(ellipse, (683 +(dictionary[edge.To].X-683)*koef)-7.5);
            Canvas.SetTop(ellipse, (352 + (dictionary[edge.To].Y-352)*koef)-7.5);
            Canvas.Children.Add(ellipse);
            return;
        }
        
        double lineLength = Math.Sqrt(Math.Pow((dictionary[edge.To].X - dictionary[edge.From].X), 2) +
                                      Math.Pow(dictionary[edge.To].Y - dictionary[edge.From].Y, 2));
        double k = 15 / lineLength;

        
        if (_graph.IsDirected && _graph.Edges.Any( z => z.To == edge.From && z.From == edge.To))
        {
                Path path = new Path();
                path.Stroke = Brushes.Black;
                path.StrokeThickness = 2;
                Point startPoint = new Point(dictionary[edge.From].X + k*(dictionary[edge.To].X - dictionary[edge.From].X), dictionary[edge.From].Y + k*(dictionary[edge.To].Y - dictionary[edge.From].Y));
                Point endPoint = new Point(dictionary[edge.To].X + k*(dictionary[edge.From].X - dictionary[edge.To].X), dictionary[edge.To].Y + k*(dictionary[edge.From].Y - dictionary[edge.To].Y));
                Point center = new Point((startPoint.X + endPoint.X)/2, (startPoint.Y + endPoint.Y)/2);
                double dx = endPoint.X - startPoint.X;
                double dy = startPoint.Y - endPoint.Y;
                double length = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                double xGrow = dx / length;
                double yGrow = dy / length;
                
                Point middlePoint = new Point(center.X - (50 * yGrow), center.Y - (50*xGrow));
                
                PathGeometry pathGeometry = new PathGeometry();
                
                PathFigure pathFigure = new PathFigure();
                
                pathFigure.StartPoint = startPoint; 
            
                BezierSegment bezierSegment = new BezierSegment(
                    startPoint,
                    middlePoint,
                    endPoint,
                    true);
                
                pathFigure.Segments.Add(bezierSegment);
                pathGeometry.Figures.Add(pathFigure);
                path.Data = pathGeometry;
                Canvas.Children.Add(path);
        }

        else
        {
            var line = new Line
            {
                X1 = dictionary[edge.From].X + k * (dictionary[edge.To].X - dictionary[edge.From].X),
                Y1 = dictionary[edge.From].Y + k * (dictionary[edge.To].Y - dictionary[edge.From].Y),
                X2 = dictionary[edge.To].X + k * (dictionary[edge.From].X - dictionary[edge.To].X),
                Y2 = dictionary[edge.To].Y + k * (dictionary[edge.From].Y - dictionary[edge.To].Y),
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            _drawnLines.Add((edge.From, edge.To), line);
            
            Point startPoint = new Point(dictionary[edge.From].X + k*(dictionary[edge.To].X - dictionary[edge.From].X), dictionary[edge.From].Y + k*(dictionary[edge.To].Y - dictionary[edge.From].Y));
            Point endPoint = new Point(dictionary[edge.To].X + k*(dictionary[edge.From].X - dictionary[edge.To].X), dictionary[edge.To].Y + k*(dictionary[edge.From].Y - dictionary[edge.To].Y));
            Point center = new Point((startPoint.X + endPoint.X)/2, (startPoint.Y + endPoint.Y)/2);
            
            _middlePoint.Add(edge, center);
            
            Canvas.Children.Add(line);
        }

        if (_graph.IsDirected)
        {
            double y2 = dictionary[edge.To].Y + k * (dictionary[edge.From].Y - dictionary[edge.To].Y);
            double y1 = dictionary[edge.From].Y + k * (dictionary[edge.To].Y - dictionary[edge.From].Y);
            double x1 = dictionary[edge.From].X + k * (dictionary[edge.To].X - dictionary[edge.From].X);
            double x2 = dictionary[edge.To].X + k * (dictionary[edge.From].X - dictionary[edge.To].X);
            
            double angle = Math.Atan2(y2 - y1, x2 - x1) * (180 / Math.PI);
            if (_graph.IsDirected && _graph.Edges.Any(z => z.To == edge.From && z.From == edge.To))
            {
                angle -= 15;
            }
            
            Polygon arrowhead = new Polygon
            {
                Points = new PointCollection { new Point(x2, y2), new Point(x2 - 10, y2 - 5), new Point(x2 - 10, y2 + 5) }, 
                Fill = Brushes.Black, 
                StrokeThickness = 0,
                RenderTransform = new RotateTransform(angle, x2, y2)
            };

            Canvas.Children.Add(arrowhead);
        }
    }

    private void DrawWeight(Edge edge)
    {
        var textBlock = new TextBlock
        {
            Text = edge.Weight.ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        Canvas.Children.Add(textBlock);
        
        Canvas.SetLeft(textBlock, _middlePoint[edge].X);
        Canvas.SetTop(textBlock, _middlePoint[edge].Y);
    }
}

public readonly struct Coordinates(double x, double y)
{
    public double X { get; } = x;

    public double Y { get; } = y;
}