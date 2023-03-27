using NeeqDMIs.Music;
using Netytar.DMIbox;
using System;

namespace Netytar.Settings
{
    [Serializable]
    public class NetytarSettings
    {
        public NetytarSettings()
        {

        }

        public NetytarSettings(int verticalSpacer, int horizontalSpacer, int occluderOffset, int ellipseRadius, int lineThickness, int highlightStrokeDim, int highlightRadius, _SharpNotesModes sharpNotesMode, _BlinkSelectScaleMode blinkSelectScaleMode, _BreathControlModes breathControlMode, _NetytarControlModes netytarControlMode, _ModulationControlModes modulationControlMode, _SlidePlayModes slidePlayMode, int sensorPort, int mIDIPort, AbsNotes rootNote, ScaleCodes scaleCode, bool noteNamesVisualized)
        {
            VerticalSpacer = verticalSpacer;
            HorizontalSpacer = horizontalSpacer;
            OccluderOffset = occluderOffset;
            EllipseRadius = ellipseRadius;
            LineThickness = lineThickness;
            HighlightStrokeDim = highlightStrokeDim;
            HighlightRadius = highlightRadius;
            SharpNotesMode = sharpNotesMode;
            BlinkSelectScaleMode = blinkSelectScaleMode;
            BreathControlMode = breathControlMode;
            NetytarControlMode = netytarControlMode;
            ModulationControlMode = modulationControlMode;
            SlidePlayMode = slidePlayMode;
            SensorPort = sensorPort;
            MIDIPort = mIDIPort;
            RootNote = rootNote;
            ScaleCode = scaleCode;
            NoteNamesVisualized = noteNamesVisualized;
        }

        public int VerticalSpacer { get; set; }
        public int HorizontalSpacer { get; set; }
        public int OccluderOffset { get; set; }
        public int EllipseRadius { get; set; }
        public int LineThickness { get; set; }
        public int HighlightStrokeDim { get; set; }
        public int HighlightRadius { get; set; }
        public _SharpNotesModes SharpNotesMode { get; set; }
        public _BlinkSelectScaleMode BlinkSelectScaleMode { get; set; }
        public _BreathControlModes BreathControlMode { get; set; }
        public _NetytarControlModes NetytarControlMode { get; set; }
        public _ModulationControlModes ModulationControlMode { get; set; }
        public _SlidePlayModes SlidePlayMode { get; set; }
        public int SensorPort { get; set; }
        public int MIDIPort { get; set; }
        public AbsNotes RootNote { get; set; }
        public ScaleCodes ScaleCode { get; set; }
        public bool NoteNamesVisualized { get; set; }
    }
}