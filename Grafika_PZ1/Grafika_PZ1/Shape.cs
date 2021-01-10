using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;

namespace Grafika_PZ1
{
    public enum Shape
    {
        None,
        Elipse,
        Rectangle,
        Polygon,
        Image
    }

    public class Coordinates
    {
        public Shape shape { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public Coordinates(Shape shape1, double x, double y)
        {
            this.shape = shape1;
            this.X = x;
            this.Y = y;
        }
    }

    public class ModificatedShape
    {
        public System.Windows.Shapes.Shape shape;
        public System.Windows.Shapes.Shape changes;
        
    }

    public class ModificatedImage
    {
        public Image image;
        public Image change;
    }
}
