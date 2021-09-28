using Netytar.DMIbox;

namespace Netytar
{
    internal static class Rack
    {
        private static NetytarDMIBox dmibox = new NetytarDMIBox();
        public static NetytarDMIBox DMIBox { get => dmibox; set => dmibox = value; }

        public static IUserSettings UserSettings { get; set; } = new DefaultUserSettings();
        public static IColorCode ColorCode { get; set; } = new DefaultColorCode();
        public static IButtonsSettings ButtonsSettings { get; set; } = new DefaultButtonSettings();
        public static NetytarSurfaceLineModes DrawMode { get; set; } = NetytarSurfaceLineModes.OnlyScaleLines;
    }
}