using NeeqDMIs.ATmega;
using NeeqDMIs.Eyetracking.Eyetribe;
using NeeqDMIs.Eyetracking.PointFilters;
using NeeqDMIs.Eyetracking.Tobii;
using NeeqDMIs.Eyetracking.Utils;
using NeeqDMIs.Keyboard;
using NeeqDMIs.MIDI;
using NeeqDMIs.Music;
using NeeqDMIs.NithSensors;
using Netytar.Behaviors.TobiiBehaviors;
using Netytar.DMIbox.KeyboardBehaviors;
using Netytar.DMIbox.NithBSBehaviors;
using Netytar.DMIbox.TobiiBehaviors;
using RawInputProcessor;
using System;
using System.Collections.Generic;
using System.Windows.Interop;
using Tobii.Interaction.Framework;

namespace Netytar.DMIbox
{
    public class NetytarSetup
    {
        public NetytarSetup(MainWindow window)
        {
            R.NetytarMainWindow = window;
        }

        public void Setup()
        {
            IntPtr windowHandle = new WindowInteropHelper(R.NetytarMainWindow).Handle;

            R.NDB.KeyboardModule = new KeyboardModule(windowHandle, RawInputCaptureMode.Foreground);

            // MIDI
            R.NDB.MidiModule = new MidiModuleNAudio(1, 1);
            //MidiDeviceFinder midiDeviceFinder = new MidiDeviceFinder(Rack.DMIBox.MidiModule);
            //midiDeviceFinder.SetToLastDevice();
            R.NDB.MidiModule.OutDevice = 1;

            // EYETRACKER
            if (R.NDB.Eyetracker == _Eyetracker.Tobii)
            {
                R.NDB.TobiiModule = new TobiiModule(GazePointDataMode.Unfiltered);
                R.NDB.TobiiModule.HeadPoseBehaviors.Add(new HPBpitchPlay(10, 15, 1.5f, 30f));
                R.NDB.TobiiModule.HeadPoseBehaviors.Add(new HPBvelocityPlay(8, 12, 2f, 120f, 0.2f));
            }

            if (R.NDB.Eyetracker == _Eyetracker.Eyetribe)
            {
                R.NDB.EyeTribeModule = new EyeTribeModule();
                R.NDB.EyeTribeModule.Start();
                R.NDB.EyeTribeModule.MouseEmulatorGazeMode = GazeMode.Raw;
            }

            // NITHBS SENSOR INIT
            R.NithBSModule = new NithModule();
            R.NithBSModule.ExpectedArguments = new List<NithArguments> { NithArguments.press };

            // BEHAVIORS
            //Rack.DMIBox.KeyboardModule.KeyboardBehaviors.Add(new KBemulateMouse());
            R.NDB.KeyboardModule.KeyboardBehaviors.Add(new KBautoScroller());
            R.NDB.KeyboardModule.KeyboardBehaviors.Add(new KBemulateMouse());
            R.NDB.KeyboardModule.KeyboardBehaviors.Add(new KBstopAutoScroller());
            R.NDB.KeyboardModule.KeyboardBehaviors.Add(new KBstopEmulateMouse());
            R.NDB.KeyboardModule.KeyboardBehaviors.Add(new KBsimulateBlow());
            R.NDB.KeyboardModule.KeyboardBehaviors.Add(new KBselectScale());

            R.NDB.TobiiModule.BlinkBehaviors.Add(new EBBselectScale(R.NetytarMainWindow));
            R.NDB.TobiiModule.BlinkBehaviors.Add(new EBBrepeatNote());
            R.NDB.TobiiModule.BlinkBehaviors.Add(new EBBdoubleCloseClick());

            R.NithBSModule.SensorBehaviors.Add(new NSBnoteDynamics(8, 1, 8));
            //R.NithBSModule.SensorBehaviors.Add(new SBbreathSensor(20, 28, 1.5f)); // 15 20
            //Rack.DMIBox.SensorReader.Behaviors.Add(new SBaccelerometerTest());
            //R.NDB.SensorReader.Behaviors.Add(new SBreadSerial());

            // SURFACE INIT
            R.NDB.AutoScroller = new AutoScroller_ButtonScroller(R.NetytarMainWindow.scrlNetytar, 0, 130, new PointFilterMAExpDecaying(0.07f)); // OLD was 100, 0.1f
            R.NDB.NetytarSurface = new NetytarSurface(R.NetytarMainWindow.canvasNetytar, R.DrawMode);

            R.NDB.NetytarSurface.DrawButtons();
            R.NDB.NetytarSurface.Scale = new Scale(R.UserSettings.RootNote, R.UserSettings.ScaleCode);
        }
    }
}