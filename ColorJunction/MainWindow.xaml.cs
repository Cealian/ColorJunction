using System;
using System.Collections.Generic;
using System.IO;
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
using System.Drawing;

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
                ImageBrush recImageBrush = new ImageBrush(); 
                int rndInt = rnd.Next(0, 4);
                switch (rndInt)
                {
                    case 0:
                        recImageBrush.ImageSource = new BitmapImage(new Uri("../../media/RecBlå.png", UriKind.Relative));
                        break;
                    case 1:
                        recImageBrush.ImageSource = new BitmapImage(new Uri("../../media/RecGul.png", UriKind.Relative));
                        break;
                    case 2:
                        recImageBrush.ImageSource = new BitmapImage(new Uri("../../media/RecGrön.png", UriKind.Relative));                       
                        break;
                    case 3:
                        recImageBrush.ImageSource = new BitmapImage(new Uri("../../media/RecRöd.png", UriKind.Relative));
                        break;
                    default:
                        //col = Brushes.Black;
                        break;
                }

                rect.Fill = recImageBrush;

                double top = gameCanvas.Height - (20 * ((i % 10) + 1));
                double left = (i/10)*20;

                Canvas.SetTop(rect, top);
                Canvas.SetLeft(rect, left);

                rect.Cursor = Cursors.Hand;

                rect.MouseUp += rect_MouseUp;

                gameCanvas.Children.Add(rect);
            }
        }

        void rect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle clickedRect = sender as Rectangle;
            gameCanvas.Children.Remove(clickedRect);
        }
    }
}
