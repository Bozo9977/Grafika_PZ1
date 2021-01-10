using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grafika_PZ1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string DrawingWindowTitle { get; set; }
        private Shape selectedShape = Shape.None;
        PointCollection coordinatesList = new PointCollection();
        Point point;
        public List<Object> UndoObjectList = null;
        public List<Object> RedoObjectList = null;
        public List<bool> isUndo = null;

        public MainWindow()
        {
            InitializeComponent();
            UndoObjectList = new List<object>();
            RedoObjectList = new List<object>();
            isUndo = new List<bool>();
        }

        private void EllipseButton_Click(object sender, RoutedEventArgs e)
        {
            label.Content = DrawingWindowTitle= "Drawing Ellipse!";
            selectedShape = Shape.Elipse;
        }

        private void RectangleButton_Click(object sender, RoutedEventArgs e)
        {
            label.Content = DrawingWindowTitle = "Drawing Rectangle!";
            selectedShape = Shape.Rectangle;
        }

        private void PolygonButton_Click(object sender, RoutedEventArgs e)
        {
            label.Content = DrawingWindowTitle = "Drawing Polygon!";
            selectedShape = Shape.Polygon;
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            label.Content = DrawingWindowTitle = "Inserting Image!";
            selectedShape = Shape.Image;
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Coordinates coordinates = null;

            switch (selectedShape)
            {
                case Shape.Elipse:
                    coordinates = new Coordinates(selectedShape, e.GetPosition(canvas).X, e.GetPosition(canvas).Y);
                    Window we = new DrawingWindow(DrawingWindowTitle, coordinates, canvas);
                    we.Show();
                    selectedShape = Shape.None;
                    break;
                case Shape.Rectangle:
                    coordinates = new Coordinates(selectedShape, e.GetPosition(canvas).X, e.GetPosition(canvas).Y);
                    Window wr = new DrawingWindow(DrawingWindowTitle, coordinates, canvas);
                    wr.Show();
                    selectedShape = Shape.None;
                    break;
                case Shape.Polygon:
                    point = new Point(e.GetPosition(canvas).X, e.GetPosition(canvas).Y);
                    coordinatesList.Add(point);
                    break;
                case Shape.Image:
                    coordinates = new Coordinates(selectedShape, e.GetPosition(canvas).X, e.GetPosition(canvas).Y);
                    Window wi = new DrawingWindow(DrawingWindowTitle, coordinates, canvas);
                    wi.Show();
                    selectedShape = Shape.None;
                    break;
                default:
                    selectedShape = Shape.None;
                    return;
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object s = e.OriginalSource;
            if (e.OriginalSource is System.Windows.Shapes.Shape && selectedShape != Shape.Polygon)
            {
                Shape shape = Shape.None;
                switch (e.OriginalSource.GetType().Name)
                {
                    case "Ellipse":
                        shape = Shape.Elipse;
                        break;
                    case "Rectangle":
                        shape = Shape.Rectangle;
                        break;
                    case "Polygon":
                        shape = Shape.Polygon;
                        break;
                }
                var v = new DrawingWindow(shape + " - Modification", (System.Windows.Shapes.Shape)e.OriginalSource, canvas);
                v.Show();
            }
            if(e.OriginalSource is Image && selectedShape != Shape.Polygon)
            {
                Window w = new DrawingWindow("Image - Modification", (Image)e.OriginalSource, canvas);
                w.Show();
            }
            if (selectedShape == Shape.Polygon && coordinatesList.Count() < 3)
            {
                MessageBox.Show("Polygon must have at least 3 points!");
            }

            if (selectedShape == Shape.Polygon && coordinatesList.Count() >= 3 && coordinatesList != null)
            {
                var w = new DrawingWindow(DrawingWindowTitle, coordinatesList, canvas);
                w.ShowDialog();
                coordinatesList = new PointCollection();
                selectedShape = Shape.None;
            }
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (UndoObjectList.Count != 0)
            {
                isUndo.Add(true);
                if (UndoObjectList.Last() is int)
                {
                    int number = (int)UndoObjectList.Last();
                    UndoObjectList.Remove(UndoObjectList.Last());
                    for(int i =0; i < number; i++)
                    {
                        canvas.Children.Add((UIElement)UndoObjectList.Last());
                        RedoObjectList.Add(UndoObjectList.Last());
                        UndoObjectList.RemoveAt(UndoObjectList.Count - 1);
                    }
                    RedoObjectList.Add(number);
                }
                else if(UndoObjectList.Last() is string)
                {
                    UndoObjectList.RemoveAt(UndoObjectList.Count - 1);
                    if(UndoObjectList.Last() is ModificatedShape)
                    {
                        System.Windows.Shapes.Shape temp = ((ModificatedShape)UndoObjectList.Last()).shape;
                        canvas.Children.Remove(temp);
                        var fill = temp.Fill;
                        var stroke = temp.Stroke;
                        var strokeThickness = temp.StrokeThickness;
                        temp.Fill = ((ModificatedShape)UndoObjectList.Last()).changes.Fill;
                        temp.Stroke = ((ModificatedShape)UndoObjectList.Last()).changes.Stroke;
                        temp.StrokeThickness = ((ModificatedShape)UndoObjectList.Last()).changes.StrokeThickness;

                        ((ModificatedShape)UndoObjectList.Last()).changes.Fill = fill;
                        ((ModificatedShape)UndoObjectList.Last()).changes.Stroke = stroke;
                        ((ModificatedShape)UndoObjectList.Last()).changes.StrokeThickness = strokeThickness;

                        RedoObjectList.Add(UndoObjectList.Last());
                        RedoObjectList.Add("m");
                        UndoObjectList.RemoveAt(UndoObjectList.Count - 1);
                        canvas.Children.Add(temp);
                    }
                    else
                    {
                        //samo source stare slike
                        var image = ((ModificatedImage)UndoObjectList.Last()).image;
                        //modifikovana slika 
                        var change = ((ModificatedImage)UndoObjectList.Last()).change;
                        var temp = change.Source;

                        canvas.Children.Remove(change);
                        change.Source = image.Source;
                        canvas.Children.Add(change);
                        ((ModificatedImage)UndoObjectList.Last()).image.Source = temp;
                        RedoObjectList.Add(UndoObjectList.Last());
                        RedoObjectList.Add("m");
                        UndoObjectList.RemoveAt(UndoObjectList.Count - 1);
                    }
                    
                }
                else
                {
                    RedoObjectList.Add(UndoObjectList.Last());
                    canvas.Children.Remove((UIElement)UndoObjectList.Last());
                    UndoObjectList.Remove(UndoObjectList.Last());
                }
                    
            }
            else
            {
                MessageBox.Show("Nothing to undo!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            if(RedoObjectList.Count() != 0 && isUndo.Last() == true)
            {
                isUndo.RemoveAt(isUndo.Count - 1);
                if(RedoObjectList.Last() is int)
                {
                    int number = (int)RedoObjectList.Last();
                    RedoObjectList.RemoveAt(RedoObjectList.Count - 1);
                    for (int i = 0; i < number; i++)
                    {
                        canvas.Children.Remove((UIElement)RedoObjectList.Last());
                        UndoObjectList.Add(RedoObjectList.Last());
                        RedoObjectList.RemoveAt(RedoObjectList.Count - 1);
                    }
                    UndoObjectList.Add(number);
                }
                else if (RedoObjectList.Last() is string)
                {
                    RedoObjectList.RemoveAt(RedoObjectList.Count - 1);

                    if(RedoObjectList.Last() is ModificatedShape)
                    {
                        System.Windows.Shapes.Shape temp = ((ModificatedShape)RedoObjectList.Last()).shape;
                        canvas.Children.Remove(temp);

                        var fill = temp.Fill;
                        var stroke = temp.Stroke;
                        var strokeThickness = temp.StrokeThickness;
                        temp.Fill = ((ModificatedShape)RedoObjectList.Last()).changes.Fill;
                        temp.Stroke = ((ModificatedShape)RedoObjectList.Last()).changes.Stroke;
                        temp.StrokeThickness = ((ModificatedShape)RedoObjectList.Last()).changes.StrokeThickness;

                        ((ModificatedShape)RedoObjectList.Last()).changes.Fill = fill;
                        ((ModificatedShape)RedoObjectList.Last()).changes.Stroke = stroke;
                        ((ModificatedShape)RedoObjectList.Last()).changes.StrokeThickness = strokeThickness;

                        UndoObjectList.Add(RedoObjectList.Last());
                        UndoObjectList.Add("m");
                        RedoObjectList.RemoveAt(RedoObjectList.Count - 1);
                        canvas.Children.Add(temp);
                    }
                    else
                    {
                        //samo source stare slike
                        var image = ((ModificatedImage)RedoObjectList.Last()).image;
                        //modifikovana slika 
                        var change = ((ModificatedImage)RedoObjectList.Last()).change;
                        var temp = change.Source;

                        canvas.Children.Remove(change);
                        change.Source = image.Source;
                        canvas.Children.Add(change);
                        ((ModificatedImage)RedoObjectList.Last()).image.Source = temp;
                        UndoObjectList.Add(RedoObjectList.Last());
                        UndoObjectList.Add("m");
                        RedoObjectList.RemoveAt(RedoObjectList.Count - 1);


                    }
                    
                }
                else
                {
                    UndoObjectList.Add(RedoObjectList.Last());
                    canvas.Children.Add((UIElement)RedoObjectList.Last());
                    RedoObjectList.RemoveAt(RedoObjectList.Count - 1);
                }
            }
            else
            {
                MessageBox.Show("Nothing to redo!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if(canvas.Children.Count != 0)
            {
                foreach (var item in canvas.Children)
                {
                    UndoObjectList.Add(item);
                }
                UndoObjectList.Add(canvas.Children.Count);
                isUndo.Add(false);
                canvas.Children.Clear();
            }
        }

    }
}
