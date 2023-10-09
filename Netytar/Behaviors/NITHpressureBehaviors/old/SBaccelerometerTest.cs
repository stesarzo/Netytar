using NITHdmis.ATmega;
using System.Globalization;

namespace Netytar.DMIbox.NithBSBehaviors
{
    public class SBaccelerometerTest : ISensorBehavior
    {
        string[] para = new string[3];

        public SBaccelerometerTest()
        {

        }

        public void ReceiveSensorRead(string val)
        {
            para = val.Split('/');

            if(para.Length == 6)
            {
                R.NDB.GyroX = int.Parse(para[0]);
                R.NDB.GyroY = int.Parse(para[1]);
                R.NDB.GyroZ = int.Parse(para[2]);
                R.NDB.AccX = int.Parse(para[3]);
                R.NDB.AccY = int.Parse(para[4]);
                R.NDB.AccZ = int.Parse(para[5]);

                PrintIndicators();

                R.NDB.MidiModule.SetPitchBend((R.NDB.GyroCalibX / 2 + 8192));
            }
            else
            {
                // missing values
            }
            
        }

        private void PrintIndicators()
        {
            R.NDB.TestString = "X: " + R.NDB.AccCalibX + "\nY: " + R.NDB.AccCalibY + "\nZ: " + R.NDB.AccCalibZ;
        }

        private int ReadValue(string val)
        {
            return int.Parse(val, CultureInfo.InvariantCulture.NumberFormat);
        }

        /*
         * Gyro max values: 32767, -32768
         */
        
    }
}
