using NITHdmis.Music;
using Netytar.DMIbox;
using System;
using System.Windows.Forms;

namespace Netytar.Settings
{
    [Serializable]
    internal class DefaultSettings : NetytarSettings
    {
        public DefaultSettings() : base()
        {
            HighlightStrokeDim = 5;
            HighlightRadius = 65;
            VerticalSpacer = -100;
            HorizontalSpacer = 200;
            OccluderOffset = 35;
            EllipseRadius = 23;
            LineThickness = 3;
            SharpNotesMode = _SharpNotesModes.On;
            BlinkSelectScaleMode = _BlinkSelectScaleMode.On;
            BreathControlMode = _BreathControlModes.Dynamic;
            ModulationControlMode = _ModulationControlModes.Off;
            PressureControlMode = _PressureControlModes.Off;
            SlidePlayMode = _SlidePlayModes.On;
            SensorPort = 4;
            MIDIPort = 1;
            RootNote = AbsNotes.C;
            ScaleCode = ScaleCodes.maj;
            NoteNamesVisualized = false;
            TPS_SensorPort = 0;
            TPS_Pressure = 0;
            TPS_CalibationMax = 0;
            TPS_CalibrationMin = 0;
            TPS_activateTeeth = false;
            BS_activateBreath = false;
            BS_Pressure = 0;
            BS_SensorPort = 0;
            keyBoardMode = Keyboard.On;
        }
    }
}