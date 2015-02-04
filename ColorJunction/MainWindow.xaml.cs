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
using System.Windows.Media.Animation;
using System.Reflection;

namespace ColorJunction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    struct Score
    {
        public Score(string name, int points)
        {
            this.name = name;
            this.points = points;
        }

        public string name;
        public int points;
    }

    public partial class MainWindow : Window
    {
        int _score = 0;
        public int tutorialstep = 0;
        Sprite[] spriteArray = new Sprite[4];
        ImageBrush[] imageBrushArray = new ImageBrush[4];

        public MainWindow()
        {
            InitializeComponent();
        }

        /* Fills the grid with columns*columns blocks */
        public void fillGrid(int columns)
        {
            if (tutorialstep == 1) 
            {
                btnRestart.IsEnabled = false;
                tutorialText.Text = "Let's learn how to play Color Junction! \n First;\n Please try holding your cursor over the squares.";
            }
            _score = 0;

            lblScore.Content = "Score: 0";

            Random rnd = new Random();
            GridLength gridLength = new GridLength(200 / columns);
            
            imageBrushArray[0] = new ImageBrush();
            imageBrushArray[1] = new ImageBrush();
            imageBrushArray[2] = new ImageBrush();
            imageBrushArray[3] = new ImageBrush();

            imageBrushArray[0].ImageSource = new BitmapImage(new Uri("pack://application:,,,/media/RecBlå.png"));
            imageBrushArray[1].ImageSource = new BitmapImage(new Uri("pack://application:,,,/media/RecGulTest.png"));
            imageBrushArray[2].ImageSource = new BitmapImage(new Uri("pack://application:,,,/media/RecGrön.png"));
            imageBrushArray[3].ImageSource = new BitmapImage(new Uri("pack://application:,,,/media/RecRöd.png"));

            string blueSource = "pack://application:,,,/media/Blue Square/";
            string redSource = "pack://application:,,,/media/Red Square/";
            string greenSource = "pack://application:,,,/media/Green Square/";
            string yellowSource = "pack://application:,,,/media/Yellow Square/";

            BitmapImage[] blue = createBitmapArray(30, blueSource, "Blue__", 3);
            BitmapImage[] red = createBitmapArray(30, redSource, "Red__", 3);
            BitmapImage[] green = createBitmapArray(30, greenSource, "Green__", 3);
            BitmapImage[] yellow = createBitmapArray(30, yellowSource, "Yellow__", 3);

            spriteArray[0] = new Sprite(blue);
            spriteArray[1] =  new Sprite(red);
            spriteArray[2] = new Sprite(green);
            spriteArray[3] = new Sprite(yellow);

            int random;

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
                    Rectangle rect = new Rectangle();

                    ImageBrush fill = new ImageBrush();
                    Brush stroke = Brushes.Black;

                    if (tutorialstep == 1)
                    {
                        random = 0;
                    }
                    else if (tutorialstep == 2)
                    {
                        random = 1;
                        if(i>=2)
                        {
                            random = 0;                        
                        }
                    }
                    else if (tutorialstep == 4) 
                    {
                        random = 0;
                        if (j < 2) 
                        {
                            random = 2;
                        }
                        if (i % 2 == 0)
                        {
                            if (j == 3 || j == 5)
                            {
                                random = 3;
                            }
                            else
                            {
                                random = 1;
                            }
                        }
                    }
                    else
                    {
                        random = rnd.Next(0, 4);
                    }
                    
                    switch (random)
                    {
                        case 0:
                            fill = imageBrushArray[0];
                            stroke = Brushes.Blue;
                            break;
                        case 1:
                            fill = imageBrushArray[1];
                            stroke = Brushes.Yellow;
                            break;
                        case 2:
                            fill = imageBrushArray[2];
                            stroke = Brushes.Green;
                            break;
                        case 3:
                            fill = imageBrushArray[3];
                            stroke = Brushes.Red;
                            break;
                        default: // Default not possible...
                            fill.ImageSource = new BitmapImage(new Uri("../../media/RecBlå.png", UriKind.Relative));
                            stroke = Brushes.Black;
                            break;
                    }

                    rect.Fill = fill;
                    rect.Stroke = stroke;
                    rect.StrokeThickness = 0.0;
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
            updateHighScore();
        }

        /* Creates array of bitmapimages */
        public BitmapImage[] createBitmapArray(int noOfImages, string sourcePath, string name, int numberLength = 0, string imageFormat = ".png")
        {
            BitmapImage[] BtArray = new BitmapImage[noOfImages];

            for (int i = 0; i < noOfImages - 1; i++)
            {
                string imgNo = Convert.ToString(i);
                while (imgNo.Length < numberLength)
                {
                    imgNo = "0" + imgNo;
                }
                BitmapImage bmi = new BitmapImage();
                string fullSource = sourcePath + name + imgNo + imageFormat;
                bmi.BeginInit();
                bmi.UriSource = new Uri(fullSource);
                bmi.EndInit();             
                BtArray[i] = bmi;
            }
            return BtArray;
        }

        /* Restores the squares to their original images */ 
        void rect_MouseLeave(object sender, MouseEventArgs e)
        {
            if (tutorialstep == 1)
            {
                tutorialText.Text = "Please try holding your cursor over the squares.";
            }
            int columns = gameGrid.ColumnDefinitions.Count;

            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < columns; row++)
                {
                    Rectangle r = getRectangle(column, row);

                    if (r != null)
                    {
                        switch (r.Stroke.ToString()) //Lägger en animerad sprite i rektangeln
                        {
                            case "#FF0000FF": r.Fill = imageBrushArray[0];//if Blue
                                break;
                             case "#FFFFFF00": r.Fill = imageBrushArray[1];//if Yellow
                                break;                            
                            case "#FF008000": r.Fill = imageBrushArray[2];//if Green
                                break;
                            case "#FFFF0000": r.Fill = imageBrushArray[3];//if Red
                                break;
                            default:
                                break; 
                        }

                        r.Opacity = 1;
                    }
                }
            }
        }

        /* Show animated sprites if the squares hovered over are a valid combo */
        void rect_MouseEnter(object sender, MouseEventArgs e)
        {
            rect_MouseLeave(null, null);
            
            Rectangle hRectangle = sender as Rectangle;

            if (!isValidMove(hRectangle))
            {
                return;
            }

            int hColumn = Grid.GetColumn(hRectangle);
            int hRow = Grid.GetRow(hRectangle);

            Queue<Rectangle> que = new Queue<Rectangle>();

            que.Enqueue(hRectangle);

            if (tutorialstep == 1) 
            {
                tutorialText.Text = "Just like that! Notice how the squares pulsate?\n Try clicking on them.";
            }
            else if (tutorialstep == 4) 
            {
                tutorialText.Text = "Here things get a little tricky.\nYou can NOT click on single squares, thus the red\nsquares can't be removed for now. If you clicked\nthe blue pair between them though...";
            }

            while (que.Count > 0)
            {
                Rectangle currentRect = que.Dequeue();

                int currentRow = Grid.GetRow(currentRect);
                int currentColumn = Grid.GetColumn(currentRect);

                if (currentRect.Stroke == hRectangle.Stroke)
                {
                    switch (currentRect.Stroke.ToString()) //Lägger en animerad sprite i rektangeln
                    {
                        case "#FF0000FF": currentRect.Fill = spriteArray[0].Animation;//if Blue
                            break;
                        case "#FFFF0000": currentRect.Fill = spriteArray[1].Animation;//if Red
                            break;
                        case "#FF008000": currentRect.Fill = spriteArray[2].Animation;//if Green
                            break;
                        case "#FFFFFF00": currentRect.Fill = spriteArray[3].Animation;//if Yellow
                            break;
                        default:
                            break;
                    }
                                        
                    currentRect.Opacity = 0.7;
                    Rectangle r;

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

        /* Rectangle clicked: remove, drop & slide. Tutorial is in here. */
        void rect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle clickedRect = sender as Rectangle;

            if (!isValidMove(clickedRect))
            {
                return;
            }

            int clickedColumn = Grid.GetColumn(clickedRect);
            int clickedRow = Grid.GetRow(clickedRect);

            Queue<Rectangle> que = new Queue<Rectangle>();

            que.Enqueue(clickedRect);

            int removedRects = 0;

            while (que.Count > 0)
            {
                Rectangle currentRect = que.Dequeue();

                int currentRow = Grid.GetRow(currentRect);
                int currentColumn = Grid.GetColumn(currentRect);

                if (currentRect.Stroke == clickedRect.Stroke)
                {
                    gameGrid.Children.Remove(currentRect);
                    removedRects++;
                    Rectangle r;

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
            lblScore.Content = "Score: " + _score;

            popupPoints(points);
            dropBlocks();
            slideBlocks();         

            if (tutorialstep == 1)
            {
                tutorialstep = 2;
                gameGrid.Children.RemoveRange(0, gameGrid.Children.Count);
                gameGrid.ColumnDefinitions.RemoveRange(0, gameGrid.ColumnDefinitions.Count);
                gameGrid.RowDefinitions.RemoveRange(0, gameGrid.RowDefinitions.Count);

                fillGrid(3);
                tutorialText.Height = 70;
                tutorialText.Text = "Good!\nDid you notice the little orange number\npoping up after you clicked the squares?\n Try clicking either the yellow or the blue squares.";
            }
            else if (tutorialstep == 2)
            {
                tutorialstep = 3;
                tutorialText.Text = "Great!\n The orange number is the number of points you\nget after removing a group of squares.\nThe more squares, the more points.";
            }
            else if (tutorialstep == 3)
            {
                tutorialstep = 4;
                tutorialText.Text = "You can keep track of all your points\nby looking at the number next to 'Score'.\n Your goal is to get as many points as possible.\n ";
                
                gameGrid.Children.RemoveRange(0, gameGrid.Children.Count);
                gameGrid.ColumnDefinitions.RemoveRange(0, gameGrid.ColumnDefinitions.Count);
                gameGrid.RowDefinitions.RemoveRange(0, gameGrid.RowDefinitions.Count);

                fillGrid(4);
            }
            else if(tutorialstep == 4)
            {
                    tutorialstep = 5;
                    tutorialText.Text = "Clicking a group of squares can change the entire\ngame-board and clearing the whole board will \nprovide 50 bonuspoints. Also, see the Hint button?\nIf you're stuck but the game isn't over, pressing it\nwill show a valid combo.";                    
                  
            }
            else 
            {
                checkPossibleMoves();
            }
        }

        /* Ends the game if there are no possible moves left */
        void checkPossibleMoves()
        {
            int columns = gameGrid.ColumnDefinitions.Count;

            bool movesPossible = false;

            for (int column = 0; column < columns && !movesPossible; column++)
            {
                for (int row = 0; row < columns && !movesPossible; row++)
                {
                    Rectangle r = getRectangle(column, row);
                    if (r != null && isValidMove(r))
                    {
                        movesPossible = true;
                    }
                }
                
            }
            string postTutorial = "\n\nNow you can;\nClick restart to start a new real game.\nClick Menu and return to the main menu\nClick Exit to exit the game.";
            
            
             if (!movesPossible && tutorialstep == 5) 
            {
                tutorialstep = 0;
                tutorialText.Height = 90;
                btnRestart.IsEnabled = true;
                
                if (gameGrid.Children.Count != 0)
                {
                    tutorialText.Text = "It didn't work this time but now you've got\nthe hang of it! " + postTutorial;
                    gameGrid.Opacity = 0.5;
                }
                else 
                {
                    tutorialText.Text = "Congrats! You've got the hang of it now!" + postTutorial;
                    popupPoints(50);
                    _score = _score + 50;
                    lblScore.Content = "Score: " + _score;
                    gameGrid.Opacity = 0.5;
                }   
            } 
            else if (movesPossible)
            {
                gameGrid.Opacity = 1;
            }
            else
            {
                if (gameGrid.Children.Count == 0) 
                {
                    popupPoints(50);
                    _score = _score + 50;
                    lblScore.Content = "Score: " + _score;
                }
                gameGrid.Opacity = 0.5;
                movelbl();
                GameOverTxt.Text = "Game Over";
                gameOver();
            }
        }

        /* Slide blocks to the left if needed */
        void slideBlocks()
        {
            int columns = gameGrid.ColumnDefinitions.Count;

            for (int column = 0; column < columns; column++)
            {
                if (getRectangle(column, 0) == null)
                {
                    slideCol(column + 1);
                }
            }
        }

        /* Drop blocks down if needed */
        void dropBlocks()
        {
            int columns = gameGrid.ColumnDefinitions.Count;

            for (int column = 0; column < columns; column++)
            {
                int dropHeight = 01;
                for (int row = 0; row < columns; row++)
                {
                    if (getRectangle(column, row)==null)
                    {
                        if (!dropDownRect(column, row + 1, dropHeight)) 
                        {
                            dropHeight++;
                        }
                    }
                }
            }
        }

        /* End game, save highscore? */
        private void gameOver()
        {
            txtEnterName.Text = "Enter your name: ";
            txtnameinput.Visibility = Visibility.Visible;
            btnSubmit.Visibility = Visibility.Visible;
            btnHint.Visibility = Visibility.Hidden;
        }

        /* Save score in "Highscore" if it's in top 5 */
        private void submitHighscore()
        {

            int newScore = _score;
            restatbtn_Click(null, null);

            if (txtnameinput.Text.Contains(":") || txtnameinput.Text.Contains(";") || txtnameinput.Text.Length < 1 || txtnameinput.Text.Length > 10)
            {
                txtnameinput.Text = "Player";
            }

            if (!File.Exists("Highscore"))
            {
                File.WriteAllText("Highscore", txtnameinput.Text + ":" + newScore + ";");
                updateHighScore();
                return;
            }

            string highScore = File.ReadAllText("Highscore");
            string[] entries = highScore.Split(';');
            Score[] scores = new Score[entries.Length+1];

            int i;
            for (i = 0; i < entries.Length; i++)
            {
                if (entries[i].Length > 0)
                {
                    string[] entry = entries[i].Split(':');

                    scores[i].name = entry[0];
                    scores[i].points = Int32.Parse(entry[1]);
                }
            }

            scores[i].name = txtnameinput.Text;
            scores[i].points = newScore;


            // Sort hihgscore
            for (int j = 0; j < scores.Length; j++)
            {
                for (int k = j; k < scores.Length; k++)
                {
                    if (scores[k].points > scores[j].points)
                    {
                        Score tmp = scores[j];
                        scores[j] = scores[k];
                        scores[k] = tmp;
                    }
                }
            }


            string output = "";
            for (int j = 0; j < scores.Length && j < 5; j++)
            {
                if (scores[j].points > 0)
                {
                    output += scores[j].name + ":" + scores[j].points + ";";
                }
            }

            File.WriteAllText("Highscore", output);

            updateHighScore();

        }

        /* Display highscore to the left */
        private void updateHighScore() 
        {
            txtHighscore.Text = "Highscore\n";

            if (!File.Exists("Highscore"))
            {
                return;
            }

            string highScore = File.ReadAllText("Highscore");
            string[] entries = highScore.Split(';');

            int place = 1;
            foreach (string entry in entries)
            {
                if (entry.Length > 0)
                {
                    string[] score = entry.Split(':');
                    txtHighscore.Text += place + ". " + score[0] + " - " + score[1] + "\n";
                    place++;
                }
            }


        }

        /* Returns true if passed rectangle is a valid move */
        bool isValidMove(Rectangle testRectangle)
        {

            int row = Grid.GetRow(testRectangle);
            int column = Grid.GetColumn(testRectangle);
            Rectangle r;

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

        /* Returns Rectangle at passed column and row */
        Rectangle getRectangle(int column, int row)
        {
            var selectedItems = gameGrid.Children.Cast<Rectangle>().Where(i => Grid.GetRow(i) == row && Grid.GetColumn(i) == column);

            if (selectedItems.Count() < 1)
            {
                return null;
            }

            return selectedItems.ElementAt(0);
        }

        /* Test function, useless in release */
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            return; // Comment to activate test keys (don't)
            if (e.Key == Key.F5)
            {
                restatbtn_Click(null, null);
            }
            else if (e.Key == Key.D1)
            {
                slideCol(9);
            }
            else if (e.Key == Key.D2)
            {
                updateHighScore();
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
                        Rectangle r = getRectangle(column, row);

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
                        Rectangle r = getRectangle(column, row);

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
                        Rectangle r = getRectangle(column, row);

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
                // 7
            }
            else if (e.Key == Key.D8)
            {
                // 8
            }
            else if (e.Key == Key.D9)
            {
                int columns = gameGrid.ColumnDefinitions.Count;

                // Empty grid
                gameGrid.Children.RemoveRange(0, gameGrid.Children.Count);
                gameGrid.ColumnDefinitions.RemoveRange(0, gameGrid.ColumnDefinitions.Count);
                gameGrid.RowDefinitions.RemoveRange(0, gameGrid.RowDefinitions.Count);

                fillGrid(columns-1);
            }
            else if (e.Key == Key.D0)
            {
                int columns = gameGrid.ColumnDefinitions.Count;

                // Empty grid
                gameGrid.Children.RemoveRange(0, gameGrid.Children.Count);
                gameGrid.ColumnDefinitions.RemoveRange(0, gameGrid.ColumnDefinitions.Count);
                gameGrid.RowDefinitions.RemoveRange(0, gameGrid.RowDefinitions.Count);

                fillGrid(columns+1);
            }
        }

        /* Displays a clickable combo (top right) */
        private void btnHint_Click(object sender, RoutedEventArgs e)
        {
            int columns = gameGrid.ColumnDefinitions.Count;

            for (int row = columns; row >= 0; row--)
            {
                for (int column = columns; column >= 0; column--)
                {
                    Rectangle r = getRectangle(column, row);

                    if (r != null && isValidMove(r))
                    {
                        rect_MouseEnter(r, null);
                        return;
                    }
                }
            }
        }

        /* Pops up points at mouse location */
        private void popupPoints(int points)
        {
            scrollScore.Text = Convert.ToString(points);
            Point p = Mouse.GetPosition(gamecanvas);
            Canvas.SetLeft(scrollScore, p.X - scrollScore.Width / 2);
            Canvas.SetTop(scrollScore, p.Y - scrollScore.Height / 2);

            DoubleAnimation animation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(1.0)));
            
            movePopupPoints(p);
            
            scrollScore.BeginAnimation(Rectangle.OpacityProperty, animation);
            
        }
        
        /* Popped points animation */
        private void movePopupPoints(Point p)
        {                       
            var moveAnimY = new DoubleAnimation(p.Y-50, p.Y -100, new Duration(TimeSpan.FromSeconds(5.0)));            
            scrollScore.BeginAnimation(Canvas.TopProperty, moveAnimY);        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /* Exits the application */
        private void btnexit_Click(object sender, RoutedEventArgs e)
        {
            tutorialText.Height = 40;
            Application.Current.Shutdown();
        }

        /* Goes back to the menu */
        private void menubtn_Click(object sender, RoutedEventArgs e)
        {
            tutorialText.Height = 40;
            Splash s = new Splash();
            this.Close();
            s.Show();
        }

        /* Restarts the game */
        private void restatbtn_Click(object sender, RoutedEventArgs e) 
        {
            tutorialText.Height = 40;
            tutorialText.Text = "";
            gameGrid.Children.RemoveRange(0, gameGrid.Children.Count);
            gameGrid.ColumnDefinitions.RemoveRange(0, gameGrid.ColumnDefinitions.Count);
            gameGrid.RowDefinitions.RemoveRange(0, gameGrid.RowDefinitions.Count);

            fillGrid(10);
            GameOverTxt.Text = "";
           // Canvas.GetTop(lblScore);
           // Canvas.GetLeft(lblScore);
            Canvas.SetTop(lblScore, 10);
            Canvas.SetLeft(lblScore, 10);
            txtEnterName.Text = "";
            txtnameinput.Visibility = Visibility.Hidden;
            btnSubmit.Visibility = Visibility.Hidden;
            btnHint.Visibility = Visibility.Visible;
        }

        /* Drops down the rectangle at the passed column/row. dropHeight is how far to drop it */
        private bool dropDownRect(int column, int row, int dropHeight) 
        {
            Rectangle rect = getRectangle(column, row);

            if (rect == null)
                return false;

            Grid.SetRow(rect, row - dropHeight);

            double height = rect.Height;

            var T = new TranslateTransform(0, height * dropHeight);
            rect.RenderTransform = T;
            Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 200 * dropHeight));
            DoubleAnimation anim = new DoubleAnimation(0, duration);
            anim.AccelerationRatio = 1.0;
            T.BeginAnimation(TranslateTransform.YProperty, anim);

            return true;

        }

        /* Centers the score label */
        private void movelbl()
        {
            var top = Canvas.GetTop(lblScore);
            var left = Canvas.GetLeft(lblScore);

            double newTop = top + 90;
            double newLeft = left + 30;

            var moveAnimTop = new DoubleAnimation(top, newTop , new Duration(TimeSpan.FromSeconds(1.0)));
            var moveAnimLeft = new DoubleAnimation(left, newLeft, new Duration(TimeSpan.FromSeconds(1.0)));

            moveAnimLeft.FillBehavior = FillBehavior.Stop;
            moveAnimTop.FillBehavior = FillBehavior.Stop;

            Canvas.SetTop(lblScore, newTop);
            Canvas.SetLeft(lblScore, newLeft);

            lblScore.BeginAnimation(Canvas.TopProperty, moveAnimTop);
            lblScore.BeginAnimation(Canvas.LeftProperty, moveAnimLeft);            
        }

        /* Slides the passed column to the left */
        private void slideCol(int column)
        {
            Rectangle rect = getRectangle(column, 0);
            int rows = gameGrid.RowDefinitions.Count;

            if (rect == null || column < 1)
                return;

            int slideDistance = 1;

            while(column-slideDistance-1 >= 0 && getRectangle(column-slideDistance-1, 0) == null)
            {
                slideDistance++;
            }

            for (int row = 0; row < rows; row++)
            {
                rect = getRectangle(column, row);

                if (rect == null)
                    break;

                Grid.SetColumn(rect, column - slideDistance);

                double width = rect.Width;

                var T = new TranslateTransform(width * slideDistance, 0);
                rect.RenderTransform = T;
                Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 200 * slideDistance));
                DoubleAnimation anim = new DoubleAnimation(0, duration);
                T.BeginAnimation(TranslateTransform.XProperty, anim);
            }
        }

        /* ... */
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            submitHighscore();
        }
    }
}
