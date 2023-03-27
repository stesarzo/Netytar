using NeeqDMIs.NithSensors;
using NeeqDMIs.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Netytar.DMIbox.NithBSBehaviors
{
    internal class NSBnoteDynamics : INithSensorBehavior
    {
        private ValueMapperDouble inputMapper;
        private float pressureMultiplier;
        private float modulationDivider;
        private float sensitivity;
        private const float UPPERTHRESH = 5f;
        private const float LOWERTHRESH = 1f;
        public NSBnoteDynamics(float modulationDivider = 8f, float pressureMultiplier = 2f, float sensitivity = 2f)
        {
            inputMapper = new ValueMapperDouble(100, 127);
            this.modulationDivider = modulationDivider;
            this.pressureMultiplier = pressureMultiplier;
            this.sensitivity = sensitivity;
        }
        public void HandleData(NithSensorData nithData)
        {
            if(R.UserSettings.NetytarControlMode == _NetytarControlModes.BreathSensor)
            {
                if(nithData.GetValue(NithArguments.press) != null)
                {
                    double input = double.Parse(nithData.GetValue(NithArguments.press), CultureInfo.InvariantCulture);
                    input = input * sensitivity;
                    if (input > 100) input = 100;
                    //MessageBox.Show(input.ToString());
                    R.NDB.BreathValue = input;
                    input = inputMapper.Map(input);
                    R.NDB.Pressure = (int)(input * pressureMultiplier);
                    R.NDB.Modulation = (int)(input / modulationDivider);
                    

                    if ((int)input > UPPERTHRESH && !R.NDB.Blow)
                    {
                        R.NDB.Blow = true;
                    }

                    if ((int)input == LOWERTHRESH)
                    {
                        R.NDB.Blow = false;
                    }
                }
            }
            
        }
    }
}
