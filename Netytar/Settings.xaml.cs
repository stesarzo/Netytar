using NeeqDMIs.Music;
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
    /// Logica di interazione per Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private int elli_H = Rack.UserSettings.EllipseRadius * 2;
        private int bordi_H = Rack.UserSettings.OccluderOffset * 2;
        private int elli_W = Rack.UserSettings.EllipseRadius * 2;
        private int bordi_W = Rack.UserSettings.OccluderOffset * 2;
        private int line_Tick = Rack.UserSettings.LineThickness;
        private int verSpacer = Rack.UserSettings.VerticalSpacer;
        private int horSpacer = Rack.UserSettings.HorizontalSpacer;
        private int highlightStrokeDim = Rack.UserSettings.HighlightStrokeDim;
     
        public void LoadUserSettings()
        {
            elli_H = Rack.UserSettings.EllipseRadius * 2;
            bordi_H = Rack.UserSettings.OccluderOffset * 2;
            elli_W = Rack.UserSettings.EllipseRadius * 2;
            bordi_W = Rack.UserSettings.OccluderOffset * 2;
            line_Tick = Rack.UserSettings.LineThickness;
            verSpacer = Rack.UserSettings.VerticalSpacer;
            horSpacer = Rack.UserSettings.HorizontalSpacer;
            highlightStrokeDim = Rack.UserSettings.HighlightStrokeDim;
    }

        public Settings()
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
           
            if (elli.Height > 45 && elli.Height <= 60)
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
            if (elli.Height > 45 && elli.Height <= 59)
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

        private void Btn_Esci(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Btn_Salva(object sender, RoutedEventArgs e)
        {
            IUserSettings userSettings = new CustomUserSettings(verSpacer, horSpacer, bordi_W / 2, elli_H / 2, line_Tick, highlightStrokeDim, elli_H + 19);
            Rack.UserSettings = userSettings;
            Rack.DMIBox.NetytarSurface.LoadSettings();
            Rack.DMIBox.NetytarSurface.DrawButtons();
            Rack.DMIBox.NetytarSurface.Scale = ScalesFactory.Cmaj;
            this.Close();
            
        }
    }

    public class CustomUserSettings : IUserSettings
    {
        public CustomUserSettings(int verticalSpacer, int horizontalSpacer, int occluderOffset, int ellipseRadius, int lineThickness, int highlightStrokeDim, int highlightRadius)
        {
            VerticalSpacer = verticalSpacer;
            HorizontalSpacer = horizontalSpacer;
            OccluderOffset = occluderOffset;
            EllipseRadius = ellipseRadius;
            LineThickness = lineThickness;
            HighlightStrokeDim = highlightStrokeDim;
            HighlightRadius = highlightRadius;
            
        }

        public int VerticalSpacer { get; set; }

        public int HorizontalSpacer { get; set; }

        public int OccluderOffset { get; set; }

        public int EllipseRadius { get; set; }

        public int LineThickness { get; set; }

        public int HighlightStrokeDim { get; set; }

        public int HighlightRadius { get; set; }
        
        public _SharpNotesModes SharpNotesMode { get; set; }
    }
}

