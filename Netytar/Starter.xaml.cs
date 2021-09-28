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

namespace Netytar
{
    /// <summary>
    /// Logica di interazione per Starter.xaml
    /// </summary>
    public partial class Starter : Window
    {
        public Starter()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Starter2 win1 = new Starter2();
            win1.Show();
            this.Close();

                 
        }
    }
}
