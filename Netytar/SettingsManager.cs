using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Netytar
{
    public static class SettingsManager
    {
        public static Backgrounds Background = Backgrounds.Panwood;
        public static void ApplySettings()
        {

            switch (Background)
            {
                case Backgrounds.Lightwood:
                    break;
                case Backgrounds.Panwood:
                    Rack.DMIBox.NetytarMainWindow.canvasNetytar.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/myapp;component/Images/Backgrounds/Panwood.png")));
                    break;
                case Backgrounds.Darkwood:
                    break;
                case Backgrounds.Midwood:
                    break;
            }
            
        }
    }

    public enum Backgrounds
    {
        Lightwood,
        Panwood,
        Darkwood,
        Midwood
    }
}
