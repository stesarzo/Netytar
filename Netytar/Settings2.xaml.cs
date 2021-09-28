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
    /// Logica di interazione per Settings2.xaml
    /// </summary>
    public partial class Settings2 : Window
    {
        private int elli_H;
        private int bordi_H;
        private int elli_W;
        private int bordi_W;
        private int line_Tick;

        public Settings2()
        {
            InitializeComponent();
        }

        private void btnPanwood_Click(object sender, RoutedEventArgs e)
        {
            rbtPanwood.IsChecked = true;
            SettingsManager.Background = Backgrounds.Panwood;
        }

        private void btnLightwood_Click(object sender, RoutedEventArgs e)
        {
            rbtLightwood.IsChecked = true;
        }

        private void btnMidwood_Click(object sender, RoutedEventArgs e)
        {
            rbtMidwood.IsChecked = true;
        }

        private void btnDarkwood_Click(object sender, RoutedEventArgs e)
        {
            rbtDarkwood.IsChecked = true;
        }

        private void Btn_Secondo_Click(object sender, RoutedEventArgs e)
        {
            if (line.StrokeThickness < 17)
            {
                line.StrokeThickness += 1;
            }
            line_Tick = (int)line.StrokeThickness;
        }

        private void Btn_Primo_Click(object sender, RoutedEventArgs e)
        {
            if (line.StrokeThickness >= 3)
            {
                line.StrokeThickness -= 1;
            }
            line_Tick = (int)line.StrokeThickness;
        }

        private void Btn_Prim_One_Click(object sender, RoutedEventArgs e)
        {


            if (elli.Height >= 46 && elli.Height <= 57)
            {
                bordi.Width -= 1;
                bordi.Height -= 1;
                elli.Height -= 1;
                elli.Width -= 1;
            }
            elli_H = (int)elli.Height;
            bordi_H = (int)bordi.Height;
            elli_W = (int)elli.Width;
            bordi_W = (int)bordi.Width;
        }

        private void Btn_Second_Two_Click(object sender, RoutedEventArgs e)
        {
            if (elli.Height >= 45 && elli.Height <= 56)
            {
                bordi.Width += 1;
                bordi.Height += 1;
                elli.Height += 1;
                elli.Width += 1;
            }
            elli_H = (int)elli.Height;
            bordi_H = (int)bordi.Height;
            elli_W = (int)elli.Width;
            bordi_W = (int)bordi.Width;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
         
            this.Close();
        }
    }
}