using NITHdmis.Music;
using Netytar.DMIbox;
using System;
using System.Windows.Forms;
using Netytar.DMIbox.NithBSBehaviors;

namespace Netytar.Settings
{
    [Serializable]
    public class NetytarSettings
    {
        public NetytarSettings()
        {

        }

        public NetytarSettings(int verticalSpacer, int horizontalSpacer, int occluderOffset, int ellipseRadius, int lineThickness, int highlightStrokeDim, int highlightRadius, _SharpNotesModes sharpNotesMode, _BlinkSelectScaleMode blinkSelectScaleMode, _BreathControlModes breathControlMode, ControlModes netytarControlMode, _ModulationControlModes modulationControlMode, _PitchBendControlModes pitchbendControlMode, _SlidePlayModes slidePlayMode, int sensorPort, int mIDIPort, AbsNotes rootNote, ScaleCodes scaleCode, bool noteNamesVisualized, bool tps_activateTeeth, int tps_SensorPort, int tps_Pressure, int tps_CalibationMax, int tps_CalibrationMin, bool bs_activatebs, int bs_SensorPort, int bs_pressure, _PressureControlModes pressurecontrolmode, bool ht_activateht, ControlModes pressurebind, ControlModes modulationbind, ControlModes pitchcbendbind, Keyboard keybordmode)
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
            PitchBendControlMode = pitchbendControlMode;
            SlidePlayMode = slidePlayMode;
            SensorPort = sensorPort;
            MIDIPort = mIDIPort;
            RootNote = rootNote;
            ScaleCode = scaleCode;
            NoteNamesVisualized = noteNamesVisualized;
            TPS_activateTeeth = tps_activateTeeth;
            TPS_SensorPort = tps_SensorPort;
            TPS_Pressure = tps_Pressure;
            TPS_CalibationMax = tps_CalibationMax;
            TPS_CalibrationMin = tps_CalibrationMin;
            BS_activateBreath = bs_activatebs;
            BS_SensorPort = bs_SensorPort;
            BS_Pressure = bs_pressure;
            PressureControlMode = pressurecontrolmode;
            HT_activateHT = ht_activateht;
            PressureBind = pressurebind;
            ModulationBind = modulationbind;
            PitchBendBind = pitchcbendbind;
            keyBoardMode = keybordmode;



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
        public ControlModes NetytarControlMode { get; set; }
        public _ModulationControlModes ModulationControlMode { get; set; }
        public _PitchBendControlModes PitchBendControlMode { get; set; }

        public _PressureControlModes PressureControlMode { get; set; }
        public _SlidePlayModes SlidePlayMode { get; set; }
        public int SensorPort { get; set; }
        public int MIDIPort { get; set; }
        public AbsNotes RootNote { get; set; }
        public ScaleCodes ScaleCode { get; set; }
        public bool NoteNamesVisualized { get; set; }


        public bool TPS_activateTeeth { get; set; }
        public int TPS_SensorPort { get; set; }
        public int TPS_Pressure { get; set; }
        public int TPS_CalibationMax { get; set; }
        public int TPS_CalibrationMin { get; set; }

        public bool BS_activateBreath { get; set; }
        public int BS_SensorPort { get; set; }
        public int BS_Pressure { get; set; }

        public bool HT_activateHT { get; set; }


        // user settings
        public ControlModes PressureBind { get; set; }

        public ControlModes ModulationBind { get; set; }

        public ControlModes PitchBendBind { get; set; }

        public Keyboard keyBoardMode {get;set;} 








        //public int BS_CalibationMax { get; set; }
        //public int BS_CalibrationMin { get; set; }


    }


}
