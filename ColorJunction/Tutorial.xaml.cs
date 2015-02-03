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
using System.Windows.Shapes;
using System.IO;

namespace ColorJunction
{
    /// <summary>
    /// Interaction logic for Tutorial.xaml
    /// </summary>
    public partial class Tutorial : Window
    {
        Splash s = new Splash();
        MainWindow m = new MainWindow();

        public Tutorial()
        {
            InitializeComponent();
            ShowText("ENG");
        }

        private void menubtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            s.Show();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void techbtn_Click(object sender, RoutedEventArgs e) 
        {
            this.Close();
            m.Show();
            m.tutorialstep = 1;
            m.fillGrid(2);
        }

        //Reads the textfile and prints out the tutorial text
        private void ShowText(string lang)
        {
                tuttxt.Text = "Welcome to ColorJunction! \n\nThis game is about removing clusters of colored cubes. \nClick on clusters of two or more of the same color to remove them from the board.\nBigger clusters give you more points. Good luck!";
        }
    }
}
