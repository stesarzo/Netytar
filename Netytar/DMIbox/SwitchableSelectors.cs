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
        //NeeqBS,
        //NeeqTPS,
        EyePos,
        EyeVel,
        NeeqHTYaw
    }
    public enum _PressureControlModes
    {
        On,
        Off
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
    public enum _PitchBendControlModes
    {
        On,
        Off
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
 
