using Netytar.DMIbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netytar
{
    public interface IUserSettings
    {
        int VerticalSpacer { get; set; }
        int HorizontalSpacer { get; set; }
        int OccluderOffset { get; set; }
        int EllipseRadius { get; set; }
        int LineThickness { get; set; }
        int HighlightStrokeDim { get; set; }
        int HighlightRadius { get; set; }
        _SharpNotesModes SharpNotesMode { get; set; }
        _BlinkSelectScaleMode BlinkSelectScaleMode { get; set; }
        _BreathControlModes BreathControlMode { get; set; }
        _NetytarControlModes NetytarControlMode { get; set; }
        _SlidePlayModes SlidePlayMode { get; set; }
        int SensorPort { get; set; }
        int MIDIPort { get; set; }
    }
}
