using NeeqDMIs.Eyetracking.Tobii;

namespace Netytar.Behaviors.TobiiBehaviors
{
    class EBBdoubleCloseClick : ATobiiBlinkBehavior
    {
        public EBBdoubleCloseClick() : base()
        {
            DCThresh = 5;
        }
        public override void Event_doubleClose()
        {
            //MessageBox.Show(R.NDB.MainWindow.LastSettingsGazedButton.ToString());
            if (R.NetytarMainWindow.LastSettingsGazedButton != null && R.NDB.TobiiModule.MouseEmulator.EyetrackerToMouse)
            {
                //MessageBox.Show("Im In!");
                R.RaiseClickEvent = true;
            }
        }

        public override void Event_doubleOpen()
        {
        }

        public override void Event_leftClose()
        {
        }

        public override void Event_leftOpen()
        {
        }

        public override void Event_rightClose()
        {
        }

        public override void Event_rightOpen()
        {
        }
    }
}
