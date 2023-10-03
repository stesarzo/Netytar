using NITHdmis.Eyetracking.PointFilters;
using NITHdmis.Eyetracking.Utils;
using NITHdmis.Keyboard;
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
                R.NDB.TobiiModule.MouseEmulator.Enabled = false;
                R.NDB.TobiiModule.MouseEmulator.CursorVisible = true;

                return 0;
            }

            return 1;
        }
    }
}