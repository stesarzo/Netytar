using NeeqDMIs.NithSensors;
using Netytar.DMIbox;
using Netytar.Settings;

namespace Netytar
{
    internal static class R
    {
        public const int HORIZONTALSPACING_MAX = 300;
        public const int HORIZONTALSPACING_MIN = 80;
        public static IButtonsSettings ButtonsSettings { get; set; } = new DefaultButtonSettings();
        public static IColorCode ColorCode { get; set; } = new DefaultColorCode();
        public static NetytarSurfaceLineModes DrawMode { get; set; } = NetytarSurfaceLineModes.OnlyScaleLines;
        public static NetytarDMIBox NDB { get; set; } = new NetytarDMIBox();
        public static MainWindow NetytarMainWindow { get; set; }
        public static NithModule NithBSModule { get; set; } = new NithModule();
        public static SavingSystem SavingSystem { get; set; } = new SavingSystem("Settings");
        public static bool RaiseClickEvent { get; internal set; } = false;
        public static NetytarSettings UserSettings { get; set; } = new DefaultSettings();
    }
}