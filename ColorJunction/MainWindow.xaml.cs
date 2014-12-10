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

namespace ColorJunction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            fillGrid();
        }

        private void fillGrid() 
        {
            for (int i = 0; i < 100; i++)
            {
                Rectangle rect = new Rectangle();

                rect.Height = 20;
                rect.Width = 20;

                Brush col;
                switch (i%4)
                {
                    case 0:
                        col = Brushes.Blue;
                        break;
                    case 1:
                        col = Brushes.Yellow;
                        break;
                    case 2:
                        col = Brushes.Green;
                        break;
                    default:
                        col = Brushes.Red;
                        break;
                }

                rect.Fill = col;

                gameCanvas.Children.Add(rect);

                double top = gameCanvas.Height - (20 * ((i % 10) + 1));
                double left = 0;

                Canvas.SetTop(rect, top);
                Canvas.SetLeft(rect, left);
            }
        }
    }
}
