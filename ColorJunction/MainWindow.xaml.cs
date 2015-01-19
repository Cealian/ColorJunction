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
using System.Windows.Media.Animation;
namespace ColorJunction
{

    struct rectCoords {
        public rectCoords(int c, int r) {
            column = c;
            row = r;
        }
        public int column, row;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Rectangle[,] rects; // Rectangles are saved here.
        Brush borderColor = Brushes.Gray; // Set border color here

        public MainWindow()
        {
            InitializeComponent();
            fillGrid(10);
        }


        /* Fills the gameCanvas with boardSize*boardSize rectangles */
        private void fillGrid(int boardSize) 
        {
            Random rnd = new Random();

            rects = new Rectangle[boardSize, boardSize];

            int rectSize = 200 / boardSize; // canvas is 200 wide.

            for (int column = 0; column < boardSize; column++)
            {
                for (int row = 0; row < boardSize; row++)
                {
                    rects[column, row] = new Rectangle();
                    
                    rects[column, row].Height = rectSize;
                    rects[column, row].Width = rectSize;

                    /* Randomize color for the rectangle */
                    Brush col;
                    ImageBrush recImageBrush = new ImageBrush(); 
                    int rndInt = rnd.Next(0, 4);
                    switch (rndInt)
                    {
                        case 0:
                            recImageBrush.ImageSource = new BitmapImage(new Uri("../../media/RecBlå.png", UriKind.Relative));
                            col = Brushes.Blue;
                            break;
                        case 1:
                            recImageBrush.ImageSource = new BitmapImage(new Uri("../../media/RecGul.png", UriKind.Relative));
                            col = Brushes.Yellow;
                            break;
                        case 2:
                            recImageBrush.ImageSource = new BitmapImage(new Uri("../../media/RecGrön.png", UriKind.Relative)); 
                            col = Brushes.Green;
                            break;
                        case 3:
                            recImageBrush.ImageSource = new BitmapImage(new Uri("../../media/RecRöd.png", UriKind.Relative));
                            col = Brushes.Red;
                            break;
                        default: // Default not possible...
                            col = Brushes.Black;
                            break;
                    }
                    rects[column, row].Stroke = col; // Set color
                    rects[column, row].Fill = recImageBrush;

                    /* Calculate and set top-left corner position */
                    double top = gameCanvas.Height - (rectSize * row) - rectSize;
                    double left = rectSize * column;

                    Canvas.SetTop(rects[column, row], top);
                    Canvas.SetLeft(rects[column, row], left);

                    
                    rects[column, row].Cursor = Cursors.Hand; // Hand cursor on hover

                    // Border styling
                    rects[column, row].StrokeThickness = 0.5;

                    rects[column, row].MouseUp += rect_MouseUp; // Add click event
                    rects[column, row].MouseEnter += rect_MouseEnter;//Add MouseEnter event
                    rects[column, row].MouseLeave += rect_MouseLeave;//Add MouseLeave event
                    gameCanvas.Children.Add(rects[column, row]); // Add rectangle to canvas
                }
            }
        }


        /* Drops down blocks with no block directly underneath */
        void dropBlocks(int boardSize) {

            int rectSize = 200 / boardSize; // Calc rectangle size for later use

            for (int i = 0; i < boardSize; i++ )
            {
                for (int column = 0; column < boardSize; column++)
                {
                    for (int row = 1; row < boardSize; row++)
                    {
                        if (rects[column, row - 1].Fill == gameCanvas.Background) // If rectangle under is background colored (no block)
                        {
                            /* Copy rectangle one down */
                            rects[column, row - 1].Fill = rects[column, row].Fill;
                            rects[column, row - 1].Stroke = rects[column, row].Stroke;

                            /* Hide rectangle copied from */
                            rects[column, row].Fill = gameCanvas.Background;
                            rects[column, row].Stroke = gameCanvas.Background;
                        }
                    }
                }
            }

            /* Set cursor.arrow to all hidden rectangles */
            for (int column = 0; column < boardSize; column++)
            {
                for (int row = 0; row < boardSize; row++)
                {
                    if (rects[column,row].Fill == gameCanvas.Background)
                    {
                        rects[column, row].Cursor = Cursors.Arrow;
                    }
                    else
                    {
                        rects[column, row].Cursor = Cursors.Hand;
                    }
                }
            }
        }

        /* Returns true if any nearby rctangle is same color as the clicked one, else false */
        bool checkNearbyBlocks(int row, int column, int boardSize, Brush clickedColor) {

            if (row + 1 < boardSize && rects[column, row+1].Stroke == clickedColor) // Check above
            {
                return true;
            }
            if (column + 1 < boardSize && rects[column+1, row].Stroke == clickedColor) // Check right
            {
                return true;
            }
            if (row - 1 >= 0 && rects[column, row-1].Stroke == clickedColor) // Check below
            {
                return true;
            }
            if (column - 1 >= 0 && rects[column-1, row].Stroke == clickedColor) // Check  left
            {
                return true;
            }

            return false; // No rectangles with same color
        }

        void rect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle clickedRect = sender as Rectangle;
            Brush clickedColor = clickedRect.Stroke;
            int boardSize = (int)Math.Sqrt(rects.Length);

            if (clickedColor == gameCanvas.Background) 
            {
                return; // Hidden rectangle clicked, do nothing
            }

            /* Check all rectangles to get column and row of the clicked rectangle */
            int column = -1, row = -1;
            for (int i = 0; i < boardSize && row < 0; i++)
            {
                for (int j = 0; j < boardSize && row < 0; j++)
                {
                    if (rects[i,j] == clickedRect)
                    {
                        column = i;
                        row = j;
                    }
                }
            }

            if (!checkNearbyBlocks(row, column, boardSize, clickedColor))
            {
                return; // Single rectangle clicked, do nothing
            }

            /* Queue the clicked rectangle and check all nearby for rectangles with same color */
            Queue<rectCoords> que = new Queue<rectCoords>();
            que.Enqueue(new rectCoords(column, row));

            int points = 0;

            while(que.Count > 0){
                rectCoords rc = que.Dequeue();

                if (rects[rc.column, rc.row].Stroke != clickedColor)
	            {
		            continue; // Current rectangle not same color, skip this one.
	            }

                /* Hide rectangle since it's the same color */
                rects[rc.column, rc.row].Fill = gameCanvas.Background;
                rects[rc.column, rc.row].Stroke = gameCanvas.Background;


                points++;

                /* Queue nearby rectangles */
                if (rc.column+1 < boardSize)
                {
                    que.Enqueue(new rectCoords(rc.column+1, rc.row));
                }
                if (rc.row+1 < boardSize)
                {
                    que.Enqueue(new rectCoords(rc.column, rc.row + 1));
                }
                if (rc.column-1 >= 0)
                {
                    que.Enqueue(new rectCoords(rc.column - 1, rc.row));
                }
                if (rc.row-1 >= 0)
                {
                    que.Enqueue(new rectCoords(rc.column, rc.row - 1));
                }
            }

            dropBlocks(boardSize); // Drop down rectangles with hidden rectangles directly below

            lblOutput.Content = points.ToString() + " (" + Math.Round(Math.Pow(1.5,points)) + " p)"; // Test score.


        }

        /* Right click resets with a new grid */
        private void gameCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            gameCanvas.Children.RemoveRange(1, gameCanvas.Children.Count);
            fillGrid(10);

            lblOutput.Content = "";
        }

        /*On MouseOver rectangle fade */
       void rect_MouseEnter(object Sender, MouseEventArgs e)
       {           
               Rectangle rec = (Rectangle)Sender;
               DoubleAnimation animation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
               animation.AutoReverse = true;
               animation.RepeatBehavior = new RepeatBehavior(TimeSpan.FromHours(1));
               rec.BeginAnimation(Rectangle.OpacityProperty, animation);
       }

        /*On MouseLeave rectangle show */
       void rect_MouseLeave(object Sender, MouseEventArgs e)
       {
           Rectangle rec = (Rectangle)Sender;
           DoubleAnimation animation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));
           rec.BeginAnimation(Rectangle.OpacityProperty, animation);
       }
    }
}
