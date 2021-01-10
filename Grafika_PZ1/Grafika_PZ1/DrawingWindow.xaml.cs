using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Grafika_PZ1
{
    /// <summary>
    /// Interaction logic for DrawingWindow.xaml
    /// </summary>
    public partial class DrawingWindow : Window
    {
        public string WindowTitle { get; set; }
        double X = 0;
        double Y = 0;
        Shape shape = Shape.None;
        PointCollection coordinatesList = null;
        System.Windows.Shapes.Shape DrawShape = null;
        System.Windows.Shapes.Shape ModifyShape = null;
        Image DrawImage = null;
        Image ModifyImage = null;
        Canvas canvas = null;
        public string DrawingImage { get; set; }
        public string NotDrawingImage { get; set; }
        static OpenFileDialog op2 = new OpenFileDialog();
        BitmapImage image;
        MainWindow mainWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
        int id = 0;

        #region Constructors
        public DrawingWindow(string title, Coordinates coordinates, Canvas canvas)
        {
            InitializeComponent();
            DataContext = this;
            WindowTitle = title;
            cmbFillColor.ItemsSource = typeof(Colors).GetProperties();
            cmbFillColor.SelectedIndex = 0;
            cmbBorderColor.ItemsSource = typeof(Colors).GetProperties();
            cmbBorderColor.SelectedIndex = 0;
            X = coordinates.X;
            Y = coordinates.Y;
            shape = coordinates.shape;
            this.canvas = canvas;
            if (shape != Shape.Image)
            {
                DrawingImage = "Hidden";
                NotDrawingImage = "Visible";
            }
            else
            {
                DrawingImage = "Visible";
                NotDrawingImage = "Hidden";
            }
        }
        public DrawingWindow(string title, PointCollection coordinates, Canvas canvas)
        {
            InitializeComponent();
            WindowTitle = title;
            cmbFillColor.ItemsSource = typeof(Colors).GetProperties();
            cmbFillColor.SelectedIndex = 0;
            cmbBorderColor.ItemsSource = typeof(Colors).GetProperties();
            cmbBorderColor.SelectedIndex = 0;
            this.coordinatesList = coordinates;
            this.canvas = canvas;
            this.shape = Shape.Polygon;
            ChooseImageButton.Visibility = Visibility.Hidden;
            ChooseImageLabel.Visibility = Visibility.Hidden;
            shapeWidth.Visibility = Visibility.Collapsed;
            shapeHeight.Visibility = Visibility.Collapsed;
            widthLabel.Visibility = Visibility.Collapsed;
            heightLabel.Visibility = Visibility.Collapsed;
            if (title.Contains("-"))
                DrawButton.Content = "Modify";
        }

        public DrawingWindow(string title, System.Windows.Shapes.Shape shape, Canvas canvas)
        {
            InitializeComponent();
            WindowTitle = title;

            cmbFillColor.ItemsSource = typeof(Colors).GetProperties();
            var color = (Color)ColorConverter.ConvertFromString(shape.Fill.ToString());
            cmbFillColor.SelectedItem = typeof(Colors).GetProperties().FirstOrDefault(x => x.GetValue(null,null).ToString() == color.ToString());

            cmbBorderColor.ItemsSource = typeof(Colors).GetProperties();
            color = (Color)ColorConverter.ConvertFromString(shape.Stroke.ToString());
            cmbBorderColor.SelectedItem = typeof(Colors).GetProperties().FirstOrDefault(x => x.GetValue(null, null).ToString() == color.ToString());

            shapeBorderThickness.Text = shape.StrokeThickness.ToString();
            this.canvas = canvas;
            ModifyShape = shape;
            ChooseImageButton.Visibility = Visibility.Hidden;
            ChooseImageLabel.Visibility = Visibility.Hidden;
            shapeWidth.Visibility = Visibility.Collapsed;
            shapeHeight.Visibility = Visibility.Collapsed;
            widthLabel.Visibility = Visibility.Collapsed;
            heightLabel.Visibility = Visibility.Collapsed;
            DrawButton.Content = "Modify";
        }

        public DrawingWindow(string title, Image image, Canvas canvas)
        {
            InitializeComponent();
            WindowTitle = title;
            this.canvas = canvas;
            ModifyImage = image;
            DrawButton.Content = "Modify";
            shapeWidth.Visibility = Visibility.Collapsed;
            shapeHeight.Visibility = Visibility.Collapsed;
            widthLabel.Visibility = Visibility.Collapsed;
            heightLabel.Visibility = Visibility.Collapsed;
            cmbBorderColor.Visibility = Visibility.Collapsed;
            cmbFillColor.Visibility = Visibility.Collapsed;
            fillLabel.Visibility = Visibility.Collapsed;
            borderThicknessLabel.Visibility = Visibility.Collapsed;
            shapeBorderThickness.Visibility = Visibility.Collapsed;
            borderColorLabel.Visibility = Visibility.Collapsed;
            Image previewImage = new Image()
            {
                Width = 251,
                Height = 170,
                Source = image.Source
            };
            Canvas.SetLeft(previewImage, 0);
            Canvas.SetTop(previewImage, 0);
            ImagePreviewCanvas.Children.Add(previewImage);

            DrawingImage = "Visible";
            NotDrawingImage = "Hidden";
        }
        #endregion


        private void DrawingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainWindow.label.Content = " ";
        }
        
        private void cmbFillColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFillColor.SelectedItem != null)
            {
                var selectedItem = (PropertyInfo)cmbFillColor.SelectedItem;
                var color = (Color)selectedItem.GetValue(null, null);
            }
        }

        private void cmbBorderColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBorderColor.SelectedItem != null)
            {
                var selectedItem = (PropertyInfo)cmbBorderColor.SelectedItem;
                var color = (Color)selectedItem.GetValue(null, null);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingWindow_Closing(null, null);
            this.Close();
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            bool validated = false;
            if ((string)DrawButton.Content != "Modify")
            {
                switch (shape)
                {
                    case Shape.Elipse:
                        validated = !String.IsNullOrEmpty(shapeHeight.Text) && !String.IsNullOrEmpty(shapeWidth.Text) && !String.IsNullOrEmpty(shapeBorderThickness.Text);
                        if (!validated)
                            break;
                        DrawShape = new Ellipse();
                        DrawShape.Height = double.Parse(shapeHeight.Text);
                        DrawShape.Width = double.Parse(shapeWidth.Text);
                        DrawShape.StrokeThickness = double.Parse(shapeBorderThickness.Text);

                        var col = cmbFillColor.Text;
                        var col1 = cmbBorderColor.Text;

                        DrawShape.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(col.Split(' ')[1]);
                        DrawShape.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(col1.Split(' ')[1]);

                        Canvas.SetLeft(DrawShape, X);

                        Canvas.SetTop(DrawShape, Y);

                        canvas.Children.Add(DrawShape);
                        mainWindow.isUndo.Add(false);
                        mainWindow.UndoObjectList.Add(DrawShape);
                        break;
                    case Shape.Rectangle:
                        validated = !String.IsNullOrEmpty(shapeHeight.Text) && !String.IsNullOrEmpty(shapeWidth.Text) && !String.IsNullOrEmpty(shapeBorderThickness.Text);
                        if (!validated)
                            break;
                        DrawShape = new Rectangle();
                        DrawShape.Height = double.Parse(shapeHeight.Text);
                        DrawShape.Width = double.Parse(shapeWidth.Text);
                        DrawShape.StrokeThickness = double.Parse(shapeBorderThickness.Text);

                        var col2 = cmbFillColor.Text;
                        var col3 = cmbBorderColor.Text;

                        DrawShape.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(col2.Split(' ')[1]);
                        DrawShape.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(col3.Split(' ')[1]);

                        Canvas.SetLeft(DrawShape, X);

                        Canvas.SetTop(DrawShape, Y);

                        canvas.Children.Add(DrawShape);
                        mainWindow.isUndo.Add(false);
                        mainWindow.UndoObjectList.Add(DrawShape);
                        break;

                    case Shape.Polygon:
                        validated = !String.IsNullOrEmpty(shapeBorderThickness.Text);
                        if (!validated)
                            break;
                        DrawShape = new Polygon()
                        {
                            Points = coordinatesList
                        };

                        DrawShape.StrokeThickness = double.Parse(shapeBorderThickness.Text);

                        var col4 = cmbFillColor.Text;
                        var col5 = cmbBorderColor.Text;

                        DrawShape.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(col4.Split(' ')[1]);
                        DrawShape.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(col5.Split(' ')[1]);

                        canvas.Children.Add(DrawShape);
                        mainWindow.isUndo.Add(false);
                        mainWindow.UndoObjectList.Add(DrawShape);
                        break;
                    case Shape.Image:
                        validated = !String.IsNullOrEmpty(shapeWidth.Text) && !String.IsNullOrEmpty(shapeHeight.Text) && ImagePreviewCanvas.Children.Count != 0;
                        if (!validated)
                            break;
                        DrawImage = new Image()
                        {
                            Width = double.Parse(shapeWidth.Text),
                            Height = double.Parse(shapeHeight.Text),
                            Source = image,
                        };
                        Canvas.SetLeft(DrawImage, X);
                        Canvas.SetTop(DrawImage, Y);
                        canvas.Children.Add(DrawImage);
                        mainWindow.isUndo.Add(false);
                        mainWindow.UndoObjectList.Add(DrawImage);
                        break;

                }
            }
            else
            {
                if (ModifyShape != null)
                {
                    validated = !String.IsNullOrEmpty(shapeBorderThickness.Text);

                    ModificatedShape mShape = new ModificatedShape();
                    mShape.shape = ModifyShape;
                    mShape.changes = new Ellipse()
                    {
                        Fill = ModifyShape.Fill,
                        Stroke = ModifyShape.Stroke,
                        StrokeThickness = ModifyShape.StrokeThickness
                    };

                    System.Windows.Shapes.Shape ModifiedShape = ModifyShape;
                    
                    var col4 = cmbFillColor.Text;
                    var col5 = cmbBorderColor.Text;

                    ModifiedShape.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(col4.Split(' ')[1]);
                    ModifiedShape.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString(col5.Split(' ')[1]);
                    ModifiedShape.StrokeThickness = double.Parse(shapeBorderThickness.Text);
                    
                    canvas.Children.Remove(ModifyShape);
                    canvas.Children.Add(ModifiedShape);
                    mainWindow.isUndo.Add(false);
                    mainWindow.UndoObjectList.Add(mShape);
                    mainWindow.UndoObjectList.Add("m");
                    ModifyShape = null;
                }
                else if(ModifyImage != null)
                {
                    validated = ImagePreviewCanvas.Children.Count != 0;

                    ModificatedImage mImage = new ModificatedImage();
                    mImage.image = new Image() { Source = ModifyImage.Source };
                    mImage.change = ModifyImage;
                    
                    mainWindow.UndoObjectList.Add(mImage);
                    mainWindow.UndoObjectList.Add("m");

                    Image ModifiedImage = ModifyImage;
                    ModifiedImage.Source = image;   //ovde doslo do promene
                    canvas.Children.Remove(ModifyImage);
                    canvas.Children.Add(ModifiedImage);
                    mainWindow.isUndo.Add(false);
                    ModifyImage = null;
                }
                
            }
            if(validated)
                this.Close();
        }

        
        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            image = null;
            if (op2.ShowDialog() == true)
            {
                image = new BitmapImage(new Uri(op2.FileName));
            }
            Image previewImage = new Image()
            {
                Width = 251,                
                Height = 170,
                Source = image
            };
            Canvas.SetLeft(previewImage, 0);
            Canvas.SetTop(previewImage, 0);
            ImagePreviewCanvas.Children.Clear();
            ImagePreviewCanvas.Children.Add(previewImage);
        }

        
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!double.TryParse((sender as TextBox).Text, out double res) && !String.IsNullOrEmpty((sender as TextBox).Text))
            {
                MessageBox.Show("Only numbers are permited for this field!", "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
                (sender as TextBox).Text = "";
            }
        }
    }
}
