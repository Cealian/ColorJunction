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
        int _score = 0;

        public MainWindow()
        {
            InitializeComponent();
            fillGrid(4);
        }

        private void fillGrid(int columns)
        {
            _score = 0;

            lblScore.Content = "Score: 0";

            Random rnd = new Random();
            GridLength gridLength = new GridLength(200 / columns);

            ImageBrush canvasBackground = new ImageBrush();
            ImageBrush gameBoardBackground = new ImageBrush();

            canvasBackground.ImageSource = new BitmapImage(new Uri("../../media/Background.png", UriKind.Relative));
           gameBoardBackground.ImageSource = new BitmapImage(new Uri("../../media/SquaresBackground1.png", UriKind.Relative));

           gameGrid.Background = gameBoardBackground;
           ViewBoxBackground.Background = canvasBackground;

           BitmapImage blueSquare = new BitmapImage(new Uri("../../media/Small test sheet.png", UriKind.Relative));
           BitmapImage redSquare = new BitmapImage(new Uri("../../media/Small test sheet Red.png",UriKind.Relative));
           BitmapImage yellowSquare = new BitmapImage(new Uri("../../media/Small test sheet Yellow.png", UriKind.Relative));
           BitmapImage greenSquare = new BitmapImage(new Uri("../../media/Small test sheet Green.png", UriKind.Relative));

            Sprite blue = new Sprite(blueSquare, 32, 50, 50);
            Sprite red = new Sprite(redSquare, 32, 50, 50);
            Sprite yellow = new Sprite(yellowSquare, 32, 50, 50);
            Sprite green = new Sprite(greenSquare, 32, 50, 50);
            

            for (int i = 0; i < columns; i++)
            {

                ColumnDefinition col = new ColumnDefinition();
                col.Width = gridLength;
                gameGrid.ColumnDefinitions.Add(col);

                RowDefinition row = new RowDefinition();
                row.Height = gridLength;
                gameGrid.RowDefinitions.Add(row);

                for (int j = 0; j < columns; j++)
                {
                    System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();

                    ImageBrush fill = new ImageBrush();
                    

                    switch (rnd.Next(0, 4))
                    {
                        case 0:
                            rect.Fill = blue.Brush;                          
                            break;
                        case 1:
                            rect.Fill = yellow.Brush;
                            break;
                        case 2:
                            rect.Fill = green.Brush;
                            break;
                        case 3:
                            rect.Fill = red.Brush;
                            break;
                        default: // Default not possible...
                            fill.ImageSource = new BitmapImage(new Uri("../../media/RecBlå.png", UriKind.Relative));
                           
                            break;
                    }


                    
                    rect.Cursor = Cursors.Hand;
                    rect.Width = gameGrid.Width / columns;
                    rect.Height = rect.Width;

                    rect.MouseUp += rect_MouseUp;
                    rect.MouseEnter += rect_MouseEnter;
                    rect.MouseLeave += rect_MouseLeave;

                    Grid.SetColumn(rect, j);
                    Grid.SetRow(rect, i);

                    gameGrid.Children.Add(rect);
                }
            }

                    checkPossibleMoves();
                 }

                 void rect_MouseLeave(object sender, MouseEventArgs e)
                 {
                     int columns = gameGrid.ColumnDefinitions.Count;

                     for (int column = 0; column < columns; column++)
                     {
                         for (int row = 0; row < columns; row++)
                         {
                            System.Windows.Shapes.Rectangle r = getRectangle(column, row);

                             if (r != null)
                             {
                                 r.Opacity = 1;
                             }
                         }
                     }
                 }

                 void rect_MouseEnter(object sender, MouseEventArgs e)
                 {
                     rect_MouseLeave(null, null);

                     System.Windows.Shapes.Rectangle hRectangle = sender as System.Windows.Shapes.Rectangle;

                     if (!isValidMove(hRectangle))
                     {
                         return;
                     }

                     int hColumn = Grid.GetColumn(hRectangle);
                     int hRow = Grid.GetRow(hRectangle);

                     Queue<System.Windows.Shapes.Rectangle> que = new Queue<System.Windows.Shapes.Rectangle>();

                     que.Enqueue(hRectangle);

                     while (que.Count > 0)
                     {
                         System.Windows.Shapes.Rectangle currentRect = que.Dequeue();

                         int currentRow = Grid.GetRow(currentRect);
                         int currentColumn = Grid.GetColumn(currentRect);

                         if (currentRect.Stroke == hRectangle.Stroke)
                         {
                             currentRect.Opacity = 0.7;
                             System.Windows.Shapes.Rectangle r;

                             r = getRectangle(currentColumn, currentRow + 1); // Up
                             if (r != null && r.Opacity == 1) { que.Enqueue(r); }

                             r = getRectangle(currentColumn, currentRow - 1); // Down
                             if (r != null && r.Opacity == 1) { que.Enqueue(r); }

                             r = getRectangle(currentColumn - 1, currentRow); // Left
                             if (r != null && r.Opacity == 1) { que.Enqueue(r); }

                             r = getRectangle(currentColumn + 1, currentRow); // Right
                             if (r != null && r.Opacity == 1) { que.Enqueue(r); }

                         }
                     }

                 }

                 void rect_MouseUp(object sender, MouseButtonEventArgs e)
                 {
                     System.Windows.Shapes.Rectangle clickedRect = sender as System.Windows.Shapes.Rectangle;

                     if (!isValidMove(clickedRect))
                     {
                         return;
                     }

                     int clickedColumn = Grid.GetColumn(clickedRect);
                     int clickedRow = Grid.GetRow(clickedRect);

                     Queue<System.Windows.Shapes.Rectangle> que = new Queue<System.Windows.Shapes.Rectangle>();

                     que.Enqueue(clickedRect);

                     int removedRects = 0;

                     while (que.Count > 0)
                     {
                         System.Windows.Shapes.Rectangle currentRect = que.Dequeue();

                         int currentRow = Grid.GetRow(currentRect);
                         int currentColumn = Grid.GetColumn(currentRect);

                         if (currentRect.Stroke == clickedRect.Stroke)
                         {
                             gameGrid.Children.Remove(currentRect);
                             removedRects++;
                             System.Windows.Shapes.Rectangle r;

                             r = getRectangle(currentColumn, currentRow + 1); // Up
                             if (r != null) { que.Enqueue(r); }

                             r = getRectangle(currentColumn, currentRow - 1); // Down
                             if (r != null) { que.Enqueue(r); }

                             r = getRectangle(currentColumn - 1, currentRow); // Left
                             if (r != null) { que.Enqueue(r); }

                             r = getRectangle(currentColumn + 1, currentRow); // Right
                             if (r != null) { que.Enqueue(r); }

                         }
                     }

                     int points = (removedRects - 1) * 2;
                     _score += points;

                     lblScore.Content = "Score: " + _score + "(+" + points + ")";

                     dropBlocks();
                     slideBlocks();
                     checkPossibleMoves();
                 }

                 void checkPossibleMoves()
                 {
                     int columns = gameGrid.ColumnDefinitions.Count;

                     bool movesPossible = false;

                     for (int column = 0; column < columns && !movesPossible; column++)
                     {
                         for (int row = 0; row < columns && !movesPossible; row++)
                         {
                             System.Windows.Shapes.Rectangle r = getRectangle(column, row);
                             if (r != null && isValidMove(r))
                             {
                                 movesPossible = true;
                             }
                         }
                     }

                     if (movesPossible)
                     {
                         gameGrid.Opacity = 1;
                     }
                     else
                     {
                         gameGrid.Opacity = 0.5;
                     }
                 }

                 void slideBlocks()
                 {
                     int columns = gameGrid.ColumnDefinitions.Count;

                     for (int x = 0; x < columns; x++)
                     {
                         for (int column = columns; column > 0; column--)
                         {
                             if (getRectangle(column, 0) == null)
                             {
                                 for (int i = column; i < columns; i++)
                                 {
                                     for (int j = 0; j < columns; j++)
                                     {
                                         System.Windows.Shapes.Rectangle r = getRectangle(i, j);
                                         if (r != null)
                                         {
                                             Grid.SetColumn(r, i - 1);
                                         }
                                     }
                                 }
                             }
                         }
                     }
                 }

                 void dropBlocks()
                 {
                     int columns = gameGrid.ColumnDefinitions.Count;

                     for (int i = 0; i < columns; i++)
                     {
                         for (int row = columns - 1; row > 0; row--)
                         {
                             for (int column = 0; column < columns; column++)
                             {
                                 System.Windows.Shapes.Rectangle r = getRectangle(column, row);

                                 if (r != null && getRectangle(column, row - 1) == null)
                                 {
                                     Grid.SetRow(r, row - 1);
                                 }
                             }
                         }
                     }
                 }

                 bool isValidMove(System.Windows.Shapes.Rectangle testRectangle)
                 {

                     int row = Grid.GetRow(testRectangle);
                     int column = Grid.GetColumn(testRectangle);
                     System.Windows.Shapes.Rectangle r;

                     r = getRectangle(column, row + 1); // Up
                     if (r != null && r.Stroke == testRectangle.Stroke) { return true; }

                     r = getRectangle(column, row - 1); // Down
                     if (r != null && r.Stroke == testRectangle.Stroke) { return true; }

                     r = getRectangle(column - 1, row); // Left
                     if (r != null && r.Stroke == testRectangle.Stroke) { return true; }

                     r = getRectangle(column + 1, row); // Right
                     if (r != null && r.Stroke == testRectangle.Stroke) { return true; }

                     return false;
                 }

                 System.Windows.Shapes.Rectangle getRectangle(int column, int row)
                 {
                     var selectedItems = gameGrid.Children.Cast<System.Windows.Shapes.Rectangle>().Where(i => Grid.GetRow(i) == row && Grid.GetColumn(i) == column);

                     if (selectedItems.Count() < 1)
                     {
                         return null;
                     }

                     return selectedItems.ElementAt(0);
                 }

                 private void Window_KeyUp(object sender, KeyEventArgs e)
                 {

                     if (e.Key == Key.F5)
                     {
                         // Empty grid
                         gameGrid.Children.RemoveRange(0, gameGrid.Children.Count);
                         gameGrid.ColumnDefinitions.RemoveRange(0, gameGrid.ColumnDefinitions.Count);
                         gameGrid.RowDefinitions.RemoveRange(0, gameGrid.RowDefinitions.Count);

                         fillGrid(10);
                     }
                     else if (e.Key == Key.D1)
                     {
                         gameGrid.Children.Remove(getRectangle(5, 5));
                     }
                     else if (e.Key == Key.D2)
                     {
                         getRectangle(0, 0).Fill = System.Windows.Media.Brushes.Black;
                     }
                     else if (e.Key == Key.D3)
                     {
                         // Empty grid
                         gameGrid.Children.RemoveRange(0, gameGrid.Children.Count);
                         gameGrid.ColumnDefinitions.RemoveRange(0, gameGrid.ColumnDefinitions.Count);
                         gameGrid.RowDefinitions.RemoveRange(0, gameGrid.RowDefinitions.Count);

                         fillGrid(15);
                     }
                     else if (e.Key == Key.D4)
                     {
                         int columns = gameGrid.ColumnDefinitions.Count;

                         for (int row = 0; row < columns; row++)
                         {
                             for (int column = 0; column < columns; column++)
                             {
                                 System.Windows.Shapes.Rectangle r = getRectangle(column, row);

                                 if (r != null && isValidMove(r))
                                 {
                                     if (r.Opacity < 1)
                                     {
                                         rect_MouseUp(r, null);
                                     }
                                     else
                                     {
                                         rect_MouseEnter(r, null);
                                     }
                                     return;
                                 }
                             }
                         }
                     }
                     else if (e.Key == Key.D5)
                     {
                         int columns = gameGrid.ColumnDefinitions.Count;

                         for (int column = 0; column < columns; column++)
                         {
                             for (int row = 0; row < columns; row++)
                             {
                                 System.Windows.Shapes.Rectangle r = getRectangle(column, row);

                                 if (r != null && isValidMove(r))
                                 {
                                     if (r.Opacity < 1)
                                     {
                                         rect_MouseUp(r, null);
                                     }
                                     else
                                     {
                                         rect_MouseEnter(r, null);
                                     }
                                     return;
                                 }
                             }
                         }
                     }
                     else if (e.Key == Key.D6)
                     {
                         int columns = gameGrid.ColumnDefinitions.Count;

                         for (int row = columns; row >= 0; row--)
                         {
                             for (int column = columns; column >= 0; column--)
                             {
                                 System.Windows.Shapes.Rectangle r = getRectangle(column, row);

                                 if (r != null && isValidMove(r))
                                 {
                                     if (r.Opacity < 1)
                                     {
                                         rect_MouseUp(r, null);
                                     }
                                     else
                                     {
                                         rect_MouseEnter(r, null);
                                     }
                                     return;
                                 }
                             }
                         }
                     }
                     else if (e.Key == Key.D7)
                     {
                         int columns = gameGrid.ColumnDefinitions.Count;

                         System.Windows.Shapes.Rectangle bestComboRect = null;
                         int bestCombo = 0;

                         for (int row = 0; row < columns; row++)
                         {
                             for (int column = 0; column < columns; column++)
                             {
                                 System.Windows.Shapes.Rectangle r = getRectangle(column, row);
                                 int c = getComboSize(r);

                                 if (r != null && c > bestCombo)
                                 {
                                     bestComboRect = r;
                                     bestCombo = c;
                                 }
                             }
                         }

                         if (bestComboRect != null)
                         {
                             if (bestComboRect.Opacity < 1)
                             {
                                 rect_MouseUp(bestComboRect, null);
                             }
                             else
                             {
                                 rect_MouseEnter(bestComboRect, null);
                             }
                         }
                     }
                     else if (e.Key == Key.D8)
                     {
                         lblScore.Content = getComboSize(getRectangle(5, 5));
                     }
                 }

                 int getComboSize(System.Windows.Shapes.Rectangle tRectangle)
                 {

                     if (tRectangle == null)
                     {
                         return 0;
                     }

                     int clickedColumn = Grid.GetColumn(tRectangle);
                     int clickedRow = Grid.GetRow(tRectangle);

                     Queue<System.Windows.Shapes.Rectangle> que = new Queue<System.Windows.Shapes.Rectangle>();

                     que.Enqueue(tRectangle);

                     int combo = 0;

                     while (que.Count > 0)
                     {
                         System.Windows.Shapes.Rectangle currentRect = que.Dequeue();

                         currentRect.Opacity = 0;

                         int currentRow = Grid.GetRow(currentRect);
                         int currentColumn = Grid.GetColumn(currentRect);

                         if (currentRect.Stroke == tRectangle.Stroke)
                         {
                             combo++;
                             System.Windows.Shapes.Rectangle r;

                             r = getRectangle(currentColumn, currentRow + 1); // Up
                             if (r != null && r.Opacity > 0) { que.Enqueue(r); }

                             r = getRectangle(currentColumn, currentRow - 1); // Down
                             if (r != null && r.Opacity > 0) { que.Enqueue(r); }

                             r = getRectangle(currentColumn - 1, currentRow); // Left
                             if (r != null && r.Opacity > 0) { que.Enqueue(r); }

                             r = getRectangle(currentColumn + 1, currentRow); // Right
                             if (r != null && r.Opacity > 0) { que.Enqueue(r); }

                         }
                     }
            
                     rect_MouseLeave(null, null);

                     return combo;
                 }

                 public void btnHint_Click(object sender, RoutedEventArgs e)
                 {
                     int columns = gameGrid.ColumnDefinitions.Count;

                     for (int row = columns; row >= 0; row--)
                     {
                         for (int column = columns; column >= 0; column--)
                         {
                             System.Windows.Shapes.Rectangle r = getRectangle(column, row);

                             if (r != null && isValidMove(r))
                             {
                                 rect_MouseEnter(r, null);
                                 return;
                             }
                         }
                     }
        }

        //this.RegisterName(gameRectangle.Name, gameRectangle);

        //DoubleAnimation myDoubleAnimation = new DoubleAnimation();
        //myDoubleAnimation.From = Canvas.GetTop(gameRectangle);
        //myDoubleAnimation.To = 0;
        //myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
        //Storyboard.SetTargetName(myDoubleAnimation, gameRectangle.Name);
        //Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.TopProperty));

        //Storyboard myStoryboard = new Storyboard();

        //myStoryboard.Children.Add(myDoubleAnimation);
        //myStoryboard.Begin(this);
    }
}
