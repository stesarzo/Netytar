﻿using NeeqDMIs.ATmega;
using NeeqDMIs.Eyetracking.Eyetribe;
using NeeqDMIs.Eyetracking.Tobii;
using NeeqDMIs.Keyboard;
using NeeqDMIs.MIDI;
using NeeqDMIs.Music;
using NeeqDMIs.NithSensors;
using System.Windows.Controls;

namespace Netytar.DMIbox
{
    /// <summary>
    /// DMIBox for Netytar, implementing the internal logic of the instrument.
    /// </summary>
    public class NetytarDMIBox
    {
        private const _BreathControlModes DEFAULT_BREATHCONTROLMODE = _BreathControlModes.Dynamic;

        private const _ModulationControlModes DEFAULT_MODULATIONCONTROLMODE = _ModulationControlModes.On;

        private const _NetytarControlModes DEFAULT_NETYTARCONTROLMODE = _NetytarControlModes.Keyboard;

        private const _SharpNotesModes DEFAULT_SHARPNOTESMODE = _SharpNotesModes.On;

        private bool hasAButtonGaze = false;

        private Button lastGazedButton = new Button();

        private string testString;

        public NetytarDMIBox()
        {
            StartingScale = ScalesFactory.Cmaj;
            LastScale = StartingScale;
            SelectedScale = StartingScale;
        }

        public bool CursorHidden { get; set; } = false;
        public _Eyetracker Eyetracker { get; set; } = _Eyetracker.Tobii;
        public EyeTribeModule EyeTribeModule { get; set; }
        public bool HasAButtonGaze { get => hasAButtonGaze; set => hasAButtonGaze = value; }
        public KeyboardModule KeyboardModule { get; set; }
        public Button LastGazedButton { get => lastGazedButton; set => lastGazedButton = value; }
        public IMidiModule MidiModule { get; set; }
        public string TestString { get => testString; set => testString = value; }
        public TobiiModule TobiiModule { get; set; }
        private Scale LastScale { get; set; }
        private Scale SelectedScale { get; set; }
        private Scale StartingScale { get; set; }

        #region Switchable

        private _BreathControlModes breathControlMode = DEFAULT_BREATHCONTROLMODE;

        public _BreathControlModes BreathControlMode
        { get => breathControlMode; set { breathControlMode = value; ResetModulationAndPressure(); } }

        #endregion Switchable

        #region Instrument logic

        private bool blow = false;
        private int modulation = 0;
        private MidiNotes nextNote = MidiNotes.C5;
        private int pressure = 127;
        private MidiNotes selectedNote = MidiNotes.C5;
        private int velocity = 127;

        public bool Blow
        {
            get { return blow; }
            set
            {
                switch (R.UserSettings.SlidePlayMode)
                {
                    case _SlidePlayModes.On:
                        if (value != blow)
                        {
                            blow = value;
                            if (blow == true)
                            {
                                PlaySelectedNote();
                            }
                            else
                            {
                                StopSelectedNote();
                            }
                        }
                        break;

                    case _SlidePlayModes.Off:
                        if (value != blow)
                        {
                            blow = value;
                            if (blow == true)
                            {
                                selectedNote = nextNote;
                                PlaySelectedNote();
                            }
                            else
                            {
                                StopSelectedNote();
                            }
                        }
                        break;
                }
            }
        }

        public int Modulation
        {
            get { return modulation; }
            set
            {
                if (R.UserSettings.ModulationControlMode == _ModulationControlModes.On)
                {
                    if (value < 50 && value > 1)
                    {
                        modulation = 50;
                    }
                    else if (value > 127)
                    {
                        modulation = 127;
                    }
                    else if (value == 0)
                    {
                        modulation = 0;
                    }
                    else
                    {
                        modulation = value;
                    }
                    SetModulation();
                }
                else if (R.UserSettings.ModulationControlMode == _ModulationControlModes.Off)
                {
                    modulation = 0;
                    SetModulation();
                }
            }
        }

        public int Pressure
        {
            get { return pressure; }
            set
            {
                if (BreathControlMode == _BreathControlModes.Dynamic)
                {
                    if (value < 50 && value > 1)
                    {
                        pressure = 50;
                    }
                    else if (value > 127)
                    {
                        pressure = 127;
                    }
                    else if (value == 0)
                    {
                        pressure = 0;
                    }
                    else
                    {
                        pressure = value;
                    }
                    SetPressure();
                }
                if (BreathControlMode == _BreathControlModes.Switch)
                {
                    pressure = 127;
                    SetPressure();
                }
            }
        }

        public MidiNotes SelectedNote
        {
            get { return selectedNote; }
            set
            {
                switch (R.UserSettings.SlidePlayMode)
                {
                    case _SlidePlayModes.On:
                        if (value != selectedNote)
                        {
                            StopSelectedNote();
                            selectedNote = value;
                            if (blow)
                            {
                                PlaySelectedNote();
                            }
                        }
                        break;

                    case _SlidePlayModes.Off:
                        if (value != selectedNote)
                        {
                            nextNote = value;
                        }
                        break;
                }
            }
        }

        public int Velocity
        {
            get { return velocity; }
            set
            {
                if (value < 0)
                {
                    velocity = 0;
                }
                else if (value > 127)
                {
                    velocity = 127;
                }
                else
                {
                    velocity = value;
                }
            }
        }

        public void ResetModulationAndPressure()
        {
            Blow = false;
            Modulation = 0;
            Pressure = 127;
            Velocity = 127;
        }

        private void PlaySelectedNote()
        {
            MidiModule.PlayNote((int)selectedNote, velocity);
        }

        private void SetModulation()
        {
            MidiModule.SetModulation(Modulation);
        }

        private void SetPressure()
        {
            MidiModule.SetPressure(pressure);
        }

        private void StopSelectedNote()
        {
            MidiModule.StopNote((int)selectedNote);
        }

        #endregion Instrument logic

        #region Graphic components

        private AutoScroller autoScroller;
        private NetytarSurface netytarSurface;
        public AutoScroller AutoScroller { get => autoScroller; set => autoScroller = value; }
        public NetytarSurface NetytarSurface { get => netytarSurface; set => netytarSurface = value; }

        #endregion Graphic components

        #region Shared values

        private int accBaseX = 0;
        private int accBaseY = 0;
        private int accBaseZ = 0;
        private int accX = 0;
        private int accY = 0;
        private int accZ = 0;
        private double eyePosBaseX = 0;
        private double eyePosBaseY = 0;
        private double eyePosBaseZ = 0;
        private int gyroBaseX = 0;
        private int gyroBaseY = 0;
        private int gyroBaseZ = 0;
        private int gyroX = 0;
        private int gyroY = 0;
        private int gyroZ = 0;
        public int AccBaseX { get => accBaseX; set => accBaseX = value; }
        public int AccBaseY { get => accBaseY; set => accBaseY = value; }
        public int AccBaseZ { get => accBaseZ; set => accBaseZ = value; }
        public int AccCalibX { get => accX - GyroBaseX; }
        public int AccCalibY { get => accY - GyroBaseY; }
        public int AccCalibZ { get => accZ - GyroBaseZ; }
        public int AccX { get => accX; set => accX = value; }
        public int AccY { get => accY; set => accY = value; }
        public int AccZ { get => accZ; set => accZ = value; }
        public int GyroBaseX { get => gyroBaseX; set => gyroBaseX = value; }
        public int GyroBaseY { get => gyroBaseY; set => gyroBaseY = value; }
        public int GyroBaseZ { get => gyroBaseZ; set => gyroBaseZ = value; }
        public int GyroCalibX { get => gyroX - GyroBaseX; }
        public int GyroCalibY { get => gyroY - GyroBaseY; }
        public int GyroCalibZ { get => gyroZ - GyroBaseZ; }
        public int GyroX { get => gyroX; set => gyroX = value; }
        public int GyroY { get => gyroY; set => gyroY = value; }
        public int GyroZ { get => gyroZ; set => gyroZ = value; }
        public double HeadPoseBaseX { get => eyePosBaseX; set => eyePosBaseX = value; }
        public double HeadPoseBaseY { get => eyePosBaseY; set => eyePosBaseY = value; }
        public double HeadPoseBaseZ { get => eyePosBaseZ; set => eyePosBaseZ = value; }
        public double BreathValue { get; internal set; } = 0;

        public void CalibrateAccBase()
        {
            R.NDB.accBaseX = accX;
            R.NDB.accBaseY = accY;
            R.NDB.accBaseZ = accZ;
        }

        public void CalibrateGyroBase()
        {
            R.NDB.gyroBaseX = gyroX;
            R.NDB.gyroBaseY = gyroY;
            R.NDB.gyroBaseZ = gyroZ;
        }

        public void Dispose()
        {
            try
            {
                TobiiModule.Dispose();
            }
            catch
            {
            }
            try
            {
                R.NithBSModule.Disconnect();
            }
            catch
            {
            }
        }

        #endregion Shared values
    }
}