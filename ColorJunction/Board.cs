using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ColorJunction
{
    class Board
    {
        Rectangle[,] _rectangle = new Rectangle[10, 10];

        public Board() 
        {
            int blues = 0;
            int greens = 0;
            int yellows = 0;
            int reds = 0;

            Random rnd = new Random();

            for (int i = 0; i < 100; i++)
            {
                int rndInt = rnd.Next(0, 4);

                Brush col;
                switch (rndInt)
                {
                    case 0:
                        col = Brushes.Blue;
                        blues++;
                        break;
                    case 1:
                        col = Brushes.Green;
                        greens++;
                        break;
                    case 2:
                        col = Brushes.Yellow;
                        yellows++;
                        break;
                    case 3:
                        col = Brushes.Red;
                        reds++;
                        break;
                    default:
                        col = Brushes.Black;
                        break;
                }
            }
        }
    }
}
