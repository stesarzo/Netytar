using Netytar.DMIbox;
using NITHdmis.NithSensors;

namespace Netytar.Behaviors.NITHheadBehaviors
{
    public class HByawPlay : INithSensorBehavior
    {
        private readonly _NetytarControlModes associatedMode = _NetytarControlModes.NeeqHTYaw;


        // NEW ==============================================



        public void HandleData(NithSensorData nithData)
        {
            // Check associated control mode is selected
            if(R.UserSettings.NetytarControlMode == associatedMode)
            {

            }
            R.HThelper.Acceleration =

            throw new System.NotImplementedException();
        }
    }
}