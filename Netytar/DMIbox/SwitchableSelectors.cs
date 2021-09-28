using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netytar.DMIbox
{

    public enum _Eyetracker
    {
        Tobii,
        Eyetribe
    }

    public enum _NetytarControlModes
    {
        Keyboard,
        BreathSensor,
        EyePos,
        EyeVel
    }

    public enum _ModulationControlModes
    {
        On,
        Off
    }

    public enum _BreathControlModes
    {
        Dynamic,
        Switch
    }

    public enum _SharpNotesModes
    {
        On,
        Off
    }

    public enum _BlinkSelectScaleMode
    {
        On,
        Off
    }

    public enum _SlidePlayModes
    {
        On,
        Off
    }
}
