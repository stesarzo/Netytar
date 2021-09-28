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
    /// Logica di interazione per Starter2.xaml
    /// </summary>
    public partial class Starter2 : Window
    {
        public Starter2()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Init win1 = new Init();
            win1.Show();
            this.Close();
        }
    }
}
