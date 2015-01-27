﻿using System;
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
            fillGrid(10);
        }

        private void fillGrid(int columns)
        {
            _score = 0;

            lblScore.Content = "Score: 0";

            Random rnd = new Random();
            GridLength gridLength = new GridLength(200 / columns);

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

                    switch (rnd.Next(0, 4))
                    {
                        case 0:
                            fill.ImageSource = new BitmapImage(new Uri("../../media/RecBlå.png", UriKind.Relative));
                            stroke = Brushes.Blue;
                            break;
                        case 1:
                            fill.ImageSource = new BitmapImage(new Uri("../../media/RecGul.png", UriKind.Relative));
                            stroke = Brushes.Yellow;
                            break;
                        case 2:
                            fill.ImageSource = new BitmapImage(new Uri("../../media/RecGrön.png", UriKind.Relative));
                            stroke = Brushes.Green;
                            break;
                        case 3:
                            fill.ImageSource = new BitmapImage(new Uri("../../media/RecRöd.png", UriKind.Relative));
                            stroke = Brushes.Red;
                            break;
                        default: // Default not possible...
                            fill.ImageSource = new BitmapImage(new Uri("../../media/RecBlå.png", UriKind.Relative));
                            stroke = Brushes.Black;
                            break;
                    }


                    rect.Fill = fill;
                    rect.Stroke = stroke;
                    rect.StrokeThickness = 0.5;
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
                    Rectangle r = getRectangle(column, row);

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

            Rectangle hRectangle = sender as Rectangle;

            if (!isValidMove(hRectangle))
            {
                return;
            }

            int hColumn = Grid.GetColumn(hRectangle);
            int hRow = Grid.GetRow(hRectangle);

            Queue<Rectangle> que = new Queue<Rectangle>();

            que.Enqueue(hRectangle);

            while (que.Count > 0)
            {
                Rectangle currentRect = que.Dequeue();

                int currentRow = Grid.GetRow(currentRect);
                int currentColumn = Grid.GetColumn(currentRect);

                if (currentRect.Stroke == hRectangle.Stroke)
                {
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
                    Rectangle r = getRectangle(column, row);
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
                                Rectangle r = getRectangle(i, j);
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
                        Rectangle r = getRectangle(column, row);

                        if (r != null && getRectangle(column, row - 1) == null)
                        {
                            Grid.SetRow(r, row - 1);
                        }
                    }
                }
            }
        }

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

        Rectangle getRectangle(int column, int row)
        {
            var selectedItems = gameGrid.Children.Cast<Rectangle>().Where(i => Grid.GetRow(i) == row && Grid.GetColumn(i) == column);

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
                getRectangle(0, 0).Fill = Brushes.Black;
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
                int columns = gameGrid.ColumnDefinitions.Count;

                Rectangle bestComboRect = null;
                int bestCombo = 0;

                for (int row = 0; row < columns; row++)
                {
                    for (int column = 0; column < columns; column++)
                    {
                        Rectangle r = getRectangle(column, row);
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

        int getComboSize(Rectangle tRectangle) {

            if (tRectangle == null)
            {
                return 0;
            }

            int clickedColumn = Grid.GetColumn(tRectangle);
            int clickedRow = Grid.GetRow(tRectangle);

            Queue<Rectangle> que = new Queue<Rectangle>();

            que.Enqueue(tRectangle);

            int combo = 0;

            while (que.Count > 0)
            {
                Rectangle currentRect = que.Dequeue();

                currentRect.Opacity = 0;

                int currentRow = Grid.GetRow(currentRect);
                int currentColumn = Grid.GetColumn(currentRect);

                if (currentRect.Stroke == tRectangle.Stroke)
                {
                    combo++;
                    Rectangle r;

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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnexit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
