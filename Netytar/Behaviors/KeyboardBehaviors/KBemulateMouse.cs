using NITHdmis.Keyboard;
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
                R.NDB.TobiiModule.MouseEmulator.Enabled = true;
                R.NDB.TobiiModule.MouseEmulator.Enabled = false;

                return 0;
            }

            return 1;
        }
    }
}