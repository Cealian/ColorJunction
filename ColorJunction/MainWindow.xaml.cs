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
            Random rnd = new Random();
            
            for (int i = 0; i < 100; i++)
            {
                Rectangle rect = new Rectangle();

                rect.Height = 20;
                rect.Width = 20;

                Brush col;
                int rndInt = rnd.Next(0, 4);
                switch (rndInt)
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
                    case 3:
                        col = Brushes.Red;
                        break;
                    default:
                        col = Brushes.Black;
                        break;
                }

                rect.Fill = col;

                gameCanvas.Children.Add(rect);

                double top = gameCanvas.Height - (20 * ((i % 10) + 1));
                double left = (i/10)*20;

                Canvas.SetTop(rect, top);
                Canvas.SetLeft(rect, left);

                rect.Cursor = Cursors.Hand;

                rect.MouseUp += rect_MouseUp;
            }
        }

        void rect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle clickedRect = sender as Rectangle;
            gameCanvas.Children.Remove(clickedRect);
        }
    }
}
