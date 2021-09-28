using NeeqDMIs.Eyetracking.PointFilters;
using NeeqDMIs.Eyetracking.Utils;
using NeeqDMIs.Keyboard;
using Netytar;
using RawInputProcessor;

namespace Netytar
{
    public class KBemulateMouse : IKeyboardBehavior
    {
        private VKeyCodes keyAction = VKeyCodes.Q;

        public int ReceiveEvent(RawInputEventArgs e)
        {
            if (e.VirtualKey == (ushort)keyAction)
            {
                Rack.DMIBox.TobiiModule.MouseEmulator = new MouseEmulator(new PointFilterBypass());
                Rack.DMIBox.TobiiModule.MouseEmulator.EyetrackerToMouse = true;
                Rack.DMIBox.TobiiModule.MouseEmulator.CursorVisible = false;

                return 0;
            }

            return 1;
        }
    }
}