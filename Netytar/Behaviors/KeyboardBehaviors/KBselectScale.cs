using NITHdmis.Keyboard;
using NITHdmis.Music;
using RawInputProcessor;

namespace Netytar.DMIbox.KeyboardBehaviors
{
    class KBselectScale : IKeyboardBehavior
    {
        private const VKeyCodes keyMaj = VKeyCodes.Add;
        private const VKeyCodes keyMin = VKeyCodes.Subtract;

        public int ReceiveEvent(RawInputEventArgs e)
        {
            if (e.VirtualKey == (ushort)keyMaj && e.KeyPressState == KeyPressState.Down)
            {
                R.NDB.NetytarSurface.Scale = new Scale(R.NDB.NetytarSurface.CheckedButton.Note.ToAbsNote(), ScaleCodes.maj);
                return 1;
            }
            if (e.VirtualKey == (ushort)keyMaj && e.KeyPressState == KeyPressState.Up)
            {
                R.NDB.NetytarSurface.Scale = new Scale(R.NDB.NetytarSurface.CheckedButton.Note.ToAbsNote(), ScaleCodes.min);
            };
            return 0;
        }
    }
}
