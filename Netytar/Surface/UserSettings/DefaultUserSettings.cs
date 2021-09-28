using Netytar.DMIbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netytar
{
    class DefaultUserSettings : IUserSettings
    {
        public int HighlightStrokeDim { get; set; } = 5;
        public int HighlightRadius { get; set; } = 65;
        public int VerticalSpacer { get; set; } = -80;
        public int HorizontalSpacer { get; set; } = 180;
        public int OccluderOffset { get; set; } = 35;
        public int EllipseRadius { get; set; } = 23;
        public int LineThickness { get; set; } = 3;
        public _SharpNotesModes SharpNotesMode { get; set; } = _SharpNotesModes.On;
        public _BlinkSelectScaleMode BlinkSelectScaleMode { get; set; } = _BlinkSelectScaleMode.On;
        public _BreathControlModes BreathControlMode { get; set; } = _BreathControlModes.Dynamic;
        public _NetytarControlModes NetytarControlMode { get; set; } = _NetytarControlModes.BreathSensor;
        public _SlidePlayModes SlidePlayMode { get; set; } = _SlidePlayModes.On;
        public int SensorPort { get; set; } = 4;
        public int MIDIPort { get; set; } = 1;
    }
}
