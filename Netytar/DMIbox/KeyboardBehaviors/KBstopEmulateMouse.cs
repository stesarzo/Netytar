using NeeqDMIs.Eyetracking.PointFilters;
using NeeqDMIs.Eyetracking.Utils;
using NeeqDMIs.Keyboard;
using Netytar;
using RawInputProcessor;

namespace Netytar
{
    public class KBstopEmulateMouse : IKeyboardBehavior
    {
        private VKeyCodes keyAction = VKeyCodes.A;

        public int ReceiveEvent(RawInputEventArgs e)
        {
            if (e.VirtualKey == (ushort)keyAction)
            {
                Rack.DMIBox.TobiiModule.MouseEmulator.EyetrackerToMouse = false;
                Rack.DMIBox.TobiiModule.MouseEmulator.CursorVisible = true;

                return 0;
            }

            return 1;
        }
    }
}