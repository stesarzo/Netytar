using NITHdmis.Keyboard;
using RawInputProcessor;

namespace Netytar
{
    public class KBstopAutoScroller : IKeyboardBehavior
    {
        const VKeyCodes keyAction = VKeyCodes.S;

        public int ReceiveEvent(RawInputEventArgs e)
        {
            if (e.VirtualKey == (ushort)keyAction && e.KeyPressState == KeyPressState.Down)
            {
                SetStuff();

                return 0;
            }

            return 1;
        }

        private void SetStuff()
        {
            R.NDB.AutoScroller.Enabled = false;
        }
    }
}