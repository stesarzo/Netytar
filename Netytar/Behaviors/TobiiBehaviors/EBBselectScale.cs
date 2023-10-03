using NITHdmis.Eyetracking.Tobii;
using NITHdmis.Music;

namespace Netytar.DMIbox.TobiiBehaviors
{
    public class EBBselectScale : ATobiiBlinkBehavior
    {
        private MainWindow mainWindow;

        public EBBselectScale(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            LCThresh = 40;
            RCThresh = 40;
        }

        public override void Event_doubleClose() { }

        public override void Event_doubleOpen() { }

        public override void Event_leftClose()
        {
            if(R.UserSettings.BlinkSelectScaleMode == _BlinkSelectScaleMode.On)
            {
                R.NetytarMainWindow.SelectedScale = new Scale(R.NDB.NetytarSurface.CheckedButton.Note.ToAbsNote(), ScaleCodes.maj);
            }
        }

        public override void Event_leftOpen() { }

        public override void Event_rightClose()
        {
            if (R.UserSettings.BlinkSelectScaleMode == _BlinkSelectScaleMode.On)
            {
                R.NetytarMainWindow.SelectedScale = new Scale(R.NDB.NetytarSurface.CheckedButton.Note.ToAbsNote(), ScaleCodes.min);
            }
        }
        public override void Event_rightOpen() { }
    }
}
