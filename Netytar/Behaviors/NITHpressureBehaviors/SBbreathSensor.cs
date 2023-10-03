using NITHdmis.ATmega;
using System.Globalization;

namespace Netytar.DMIbox.NithBSBehaviors
{
    [System.Obsolete("Deprecated, use PressureBasedBehavior.cs instead")]
    public class SBbreathSensor : ISensorBehavior
    {
        private int v = 1;
        private int offThresh;
        private int onThresh;
        private float sensitivity;

        public SBbreathSensor(int offThresh, int onThresh, float sensitivity)
        {
            this.offThresh = offThresh;
            this.onThresh = onThresh;
            this.sensitivity = sensitivity;
        }

        public void ReceiveSensorRead(string val)
        {
            if(R.UserSettings.NetytarControlMode == _NetytarControlModes.NeeqBS)
            {
                float b = 0;

                try
                {
                    b = float.Parse(val, CultureInfo.InvariantCulture.NumberFormat);
                }
                catch
                {

                }

                v = (int)(b / 3);

                R.NetytarMainWindow.BreathSensorValue = v;
                R.NDB.Pressure = (int)(v * 2 * sensitivity);
                R.NDB.Modulation = (int)(v / 8 * sensitivity);

                if (v > onThresh && R.NDB.Blow == false)
                {
                    R.NDB.Blow = true;
                    //NetytarRack.DMIBox.Pressure = 110;
                }

                if (v < offThresh)
                {
                    R.NDB.Blow = false;
                }
            }
            
        }
    }
}
