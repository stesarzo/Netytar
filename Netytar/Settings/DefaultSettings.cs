﻿using NeeqDMIs.Music;
using Netytar.DMIbox;
using System;

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
            NetytarControlMode = _NetytarControlModes.BreathSensor;
            SlidePlayMode = _SlidePlayModes.On;
            SensorPort = 4;
            MIDIPort = 1;
            RootNote = AbsNotes.C;
            ScaleCode = ScaleCodes.maj;
            NoteNamesVisualized = false;
        }
    }
}