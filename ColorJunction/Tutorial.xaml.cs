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
        public Tutorial()
        {
            InitializeComponent();
            ShowText("SWE");
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

        private void ShowText(string lang)
        {
            try
            {
                using (StreamReader sr = new StreamReader("../../Tut_"+ lang +".txt"))
                {
                    String line = sr.ReadToEnd();
                    tuttxt.Text = line;
                }
            }
            catch (Exception e)
            {
                tuttxt.Text = "Lang filen kan inte läsas, " + e.Message;
            }
           
        }
    }
}
