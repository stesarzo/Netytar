using NeeqDMIs.Keyboard;
using Netytar.DMIbox;
using RawInputProcessor;
using System.Windows;
using System.Windows.Controls;

namespace Netytar
{
    public class KBsimulateBlow : IKeyboardBehavior
    {
        private VKeyCodes keyBlow = VKeyCodes.Space;

        private bool blowing = false;
        int returnVal = 0;

        public int ReceiveEvent(RawInputEventArgs e)
        {
            returnVal = 0;

            if(R.UserSettings.NetytarControlMode == _NetytarControlModes.Keyboard)
            {
                if (e.VirtualKey == (ushort)keyBlow && e.KeyPressState == KeyPressState.Down)
                {
                    blowing = true;
                    returnVal = 1;
                    R.NDB.BreathValue = 127;
                    R.NDB.Velocity = 127;
                    R.NDB.Pressure = 127;
                }
                else if (e.VirtualKey == (ushort)keyBlow && e.KeyPressState == KeyPressState.Up)
                {
                    blowing = false;
                    returnVal = 1;
                    R.NDB.BreathValue = 0;
                    R.NDB.Velocity = 0;
                    R.NDB.Pressure = 0;
                }
                R.NDB.Blow = blowing;
            }

            return returnVal;
        }
    }
}
