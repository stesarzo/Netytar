using NITHdmis.ATmega;

namespace Netytar.DMIbox.NithBSBehaviors
{
    public class SBreadSerial : ISensorBehavior
    {
        private string cose = "";
            
        public void ReceiveSensorRead(string val)
        {
           cose = val;
           R.NDB.TestString = cose.Replace("$", "\n");
        }

        /*
         * Gyro max values: 32767, -32768
         */
        
    }
}
