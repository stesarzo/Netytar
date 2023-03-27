using NeeqDMIs.Eyetracking.Tobii;
using System.Windows;
using Tobii.Interaction;

namespace Netytar.DMIbox.TobiiBehaviors
{
    public class HPBpitchPlay : ITobiiHeadPoseBehavior
    {
        private int offThresh;
        private int onThresh;
        private float maxSwing;
        private float sensitivity;

        private float b;
        public float B
        {
            get { return b; }
            set
            {
                if(value < 0)
                {
                    b = 0;
                }
                else if(value > maxSwing)
                {
                    b = 50;
                }
                else if(value == 0) { }
                else if (value < -50) { }
                else
                {
                    b = value;
                }

            }
        }

        public HPBpitchPlay(int offThresh, int onThresh, float sensitivity, float maxSwing)
        {
            this.offThresh = offThresh;
            this.onThresh = onThresh;
            this.sensitivity = sensitivity;
            this.maxSwing = maxSwing;
        }



        public void ReceiveHeadPoseData(HeadPoseData data)
        {

            if (R.UserSettings.NetytarControlMode == _NetytarControlModes.EyePos)
            {
                if (data.HasHeadPosition)
                {
                    B = (float)(data.HeadRotation.Y - R.NDB.HeadPoseBaseY);
                    int Bnorm = (int)((B * 127f) / maxSwing);

                    R.NetytarMainWindow.BreathSensorValue = Bnorm;
                    R.NDB.Pressure = (int)(Bnorm * 2 * sensitivity);
                    //NetytarRack.DMIBox.Modulation = (int)(Bnorm / 8 * sensitivity);

                    if (Bnorm > onThresh && R.NDB.Blow == false)
                    {
                        R.NDB.Blow = true;
                        //NetytarRack.DMIBox.Pressure = 110;
                    }

                    if (Bnorm < offThresh)
                    {
                        R.NDB.Blow = false;
                    }
                }
            }

        }
    }
}
