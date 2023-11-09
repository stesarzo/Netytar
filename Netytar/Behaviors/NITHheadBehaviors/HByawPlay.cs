using Netytar.DMIbox;
using NITHdmis.Filters.ValueFilters;
using NITHdmis.NithSensors;
using NITHdmis.Utils;
using System;
using System.Globalization;

namespace Netytar.Behaviors.NITHheadBehaviors
{
    public class HByawPlay : INithSensorBehavior
    {

        private const int DEADSPEED = 40;
        private const double PRESSURE_MULTIPLIER = 10f;
        private const double VELOCITY_MULTIPLIER = 2.5f;
        //private readonly _NetytarControlModes associatedMode = _NetytarControlModes.NeeqHTYaw;
        private readonly ValueMapperDouble PressureMapper = new ValueMapperDouble(0.5f, 127);
        private readonly IDoubleFilter SpeedFilter = new DoubleFilterMAExpDecaying(0.8f);
        private int currentDirection = 1;
        private int previousDirection = 1;
        private double yawSpeed = 0;
        private double yawSpeedFiltered;

        ///<summary>
        ///Handles the NithSensorData by checking the associated control mode and calling the HTStrum_ElaboratePosition method if the NithSensorData contains the "acc_yaw" argument.
        ///</summary>
        ///<param name="nithData">The NithSensorData object to handle.</param>
        public void HandleData(NithSensorData nithData)
        {
            // Check associated control mode is selected
            
            //if (R.UserSettings.NetytarControlMode == associatedMode)
           // {
                if (nithData.ContainsArgument(NithArguments.acc_yaw))
                {
                    HTStrum_ElaboratePosition(nithData);
                }
            //}
        }
        

        public void HTStrum_ElaboratePosition(NithSensorData nithData)
        {
            yawSpeed = double.Parse(nithData.GetArgument(NithArguments.acc_yaw).Value.Base, CultureInfo.InvariantCulture);
            previousDirection = currentDirection;
            currentDirection = Math.Sign(yawSpeed);

            // Perché tutto 'sto casino?
            SpeedFilter.Push(yawSpeed);
            yawSpeedFiltered = PressureMapper.Map(SpeedFilter.Pull());
            yawSpeedFiltered = Math.Log(yawSpeedFiltered, 1.5f) * PRESSURE_MULTIPLIER;

            R.NDB.Pressure = (int)yawSpeedFiltered - DEADSPEED;
            //R.NDB.Expression = (int)yawSpeedFiltered - DEADSPEED;
            //Mon_Speed = MusicParameters.Pressure;
            //MusicParameters.Pressure_Set();
            //MusicParameters.Expression_Set();

            if (currentDirection != previousDirection && R.NDB.Pressure > 0)
            {
                R.NDB.Velocity = (int)(yawSpeedFiltered * VELOCITY_MULTIPLIER - DEADSPEED);
                R.NDB.Blow = false;
                R.NDB.Blow = true;
            }
        }
    }
}