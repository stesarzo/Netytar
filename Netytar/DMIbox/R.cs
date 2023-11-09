using NITHdmis.NithSensors;
using Netytar.DMIbox;
using Netytar.Settings;
using NITHdmis.Headtracking.NeeqHT;
using Netytar.Behaviors;
using Netytar.DMIbox.NithBSBehaviors;

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
        public static NithModule NithModule { get; set; } = new NithModule();
        public static NithModule NithModuleBS { get; set; } = new NithModule();
        public static NithModule NithModuleTPS { get; set; } = new NithModule();
        public static NithModule NithModuleKey { get; set; } = new NithModule();
        public static SavingSystem SavingSystem { get; set; } = new SavingSystem("Settings");
        public static bool RaiseClickEvent { get; internal set; } = false;
        public static NetytarSettings UserSettings { get; set; } = new DefaultSettings();

        public static HeadtrackerCenteringHelper HThelper { get; set; } = new HeadtrackerCenteringHelper();

        public static TeethPressureCalibrationHelper TPhelper { get; set; } = new TeethPressureCalibrationHelper();

        public static SavingCalibration SavingCalibration { get; set; } = new SavingCalibration();

    }
} 