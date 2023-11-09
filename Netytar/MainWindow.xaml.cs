using NITHdmis.ErrorLogging;
using NITHdmis.Music;
using Netytar.DMIbox;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System;
using System.IO;
using Tobii.Interaction.Wpf;
using NITHdmis.NithSensors;
using System.Drawing.Drawing2D;
using Netytar.DMIbox.NithBSBehaviors;
using System.Windows.Input;

namespace Netytar
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SolidColorBrush ActiveBrush = new SolidColorBrush(Colors.LightGreen);
        private readonly SolidColorBrush WarningBrush = new SolidColorBrush(Colors.DarkRed);
        private readonly SolidColorBrush BlankBrush = new SolidColorBrush(Colors.Black);

        private readonly SolidColorBrush GazeButtonColor = new SolidColorBrush(Colors.DarkGoldenrod);
        private bool NetytarStarted = false;
        private DispatcherTimer updater;
        private double velocityBarMaxHeight = 0;

        private Scale lastScale = ScalesFactory.Cmaj;

        public Button LastSettingsGazedButton { get; set; } = null;

        private Brush LastGazedBrush = null;
        private DispatcherTimer timer;

        private DispatcherTimer TimerClick;
        private bool isLongClick = false;

        public MainWindow()
        {
            InitializeComponent();

            // Debugger
            TraceAdder.AddTrace();
            DataContext = this;

            // GUI updater
            updater = new DispatcherTimer();
            updater.Interval = new TimeSpan(1000);
            updater.Tick += UpdateTimedVisuals;
            updater.Start();

        }

        public int BreathSensorValue { get; set; } = 0;

        public Scale SelectedScale { get; set; } = ScalesFactory.Cmaj;

        public int SensorPort
        {
            get { return R.UserSettings.SensorPort; }
            set
            {
                if (value > 0)
                {
                    R.UserSettings.SensorPort = value;
                }
            }
        }

        public int TPS_SenorPort
        {
            get { return R.UserSettings.TPS_SensorPort; }
            set
            {
                if (value > 0)
                {
                    R.UserSettings.TPS_SensorPort = value;
                }
            }
        }

        public int BS_SenorPort
        {
            get { return R.UserSettings.BS_SensorPort; }
            set
            {
                if (value > 0)
                {
                    R.UserSettings.BS_SensorPort = value;
                }
            }
        }

        public void ReceiveNoteChange()
        {
            UpdateGUIVisuals();
        }

        public void ReceiveBlowingChange()
        {
            if (R.NDB.Blow)
            {
                txtIsBlowing.Text = "B";
            }
            else
            {
                txtIsBlowing.Text = "_";
            }
        }

        internal void ChangeScale(ScaleCodes scaleCode)
        {
            R.NDB.NetytarSurface.Scale = new Scale(R.NDB.SelectedNote.ToAbsNote(), scaleCode);
        }

        private void eyeGazeHandler(object sender, HasGazeChangedRoutedEventArgs e)
        {
            if (e.HasGaze)
            {
                R.NDB.HasAButtonGaze = true;
                R.NDB.LastGazedButton = (System.Windows.Controls.Button)sender;
            }
            else
            {
                R.NDB.HasAButtonGaze = false;
            }
        }

        private void UpdateTimedVisuals(object sender, EventArgs e)
        {
            if (NetytarStarted)
            {
                if (SelectedScale.GetName().Equals(lastScale.GetName()) == false)
                {
                    lastScale = SelectedScale;
                    R.NDB.NetytarSurface.Scale = SelectedScale;
                    UpdateGUIVisuals();
                }

                txtNoteName.Text = R.NDB.SelectedNote.ToStandardString();
                txtPitch.Text = R.NDB.SelectedNote.ToPitchValue().ToString();
                if (R.NDB.Blow)
                {
                    txtIsBlowing.Text = "B";
                }
                else
                {
                    txtIsBlowing.Text = "_";
                }

                prbBreathSensor.Value = R.NDB.BreathValue;

                if (R.RaiseClickEvent)
                {
                    LastSettingsGazedButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    R.RaiseClickEvent = false;
                }

                /*
                try
                {
                    txtEyePosX.Text = NetytarRack.DMIBox.EyeXModule.LastEyePosition.LeftEye.X.ToString();
                    txtEyePosY.Text = NetytarRack.DMIBox.EyeXModule.LastEyePosition.LeftEye.Y.ToString();
                    txtEyePosZ.Text = NetytarRack.DMIBox.EyeXModule.LastEyePosition.LeftEye.Z.ToString();
                }
                catch
                {
                }*/

                //txtTest.Text = Rack.DMIBox.TestString;
            }
            ValueSensor_txt.Text = R.NDB.pressureSensor;
            Pitchbend_txt.Text = R.NDB.PitchBend.ToString();
            PitchbendBS_txt.Text = R.NDB.PitchBend.ToString();
        }

        private void StartNetytar()
        {

            // EventHandler for all buttons
            EventManager.RegisterClassHandler(typeof(Button), Button.MouseEnterEvent, new RoutedEventHandler(Global_SettingsButton_MouseEnter));

            R.UserSettings = R.SavingSystem.LoadSettings();
            NetytarSetup netytarSetup = new NetytarSetup(this);
            netytarSetup.Setup();

            // Checks the selected MIDI port is available
            CheckMidiPort();

            // wpfInteractorAgent = NetytarRack.DMIBox.TobiiModule.TobiiHost.InitializeWpfAgent();

            // Hide Settings
            brdSettings.Visibility = Visibility.Hidden;
            brdPress.Visibility = Visibility.Hidden;
            brdBS.Visibility = Visibility.Hidden;
            
            // LEAVE AT THE END!
            NetytarStarted = true;
            UpdateGUIVisuals();
            UpdateSensorBSConnection();
            UpdateSensorTPSConnection();
        }

        private void UpdateGUIVisuals()
        {
            // TEXT
            txtMIDIch.Text = "MP" + R.UserSettings.MIDIPort.ToString();
            txtSensorPressPort.Text = "COM" + R.UserSettings.TPS_SensorPort.ToString();
            txtSensorBSPort.Text = "COM" + R.UserSettings.BS_SensorPort.ToString();
            txtRootNote.Text = R.UserSettings.RootNote.ToString();
            txtSpacing.Text = R.UserSettings.HorizontalSpacer.ToString();

            /// INDICATORS
            indBreath.Background = R.UserSettings.BS_activateBreath == true ? ActiveBrush : BlankBrush;
            indTeeth.Background = R.UserSettings.TPS_activateTeeth == true ? ActiveBrush : BlankBrush;
            indHeadYaw.Background = R.UserSettings.HT_activateHT == true ? ActiveBrush : BlankBrush;
            //indKeyboard.Background = R.UserSettings.NetytarControlMode == _NetytarControlModes.Keyboard ? ActiveBrush : BlankBrush;
            indRootNoteColor.Background = R.ColorCode.FromAbsNote(R.UserSettings.RootNote);
            indScaleMajor.Background = (R.UserSettings.ScaleCode == ScaleCodes.maj) ? ActiveBrush : BlankBrush;
            indScaleMinor.Background = (R.UserSettings.ScaleCode == ScaleCodes.min) ? ActiveBrush : BlankBrush;
            indMod.Background = R.UserSettings.ModulationBind == ControlModes.NithTPS ? ActiveBrush: BlankBrush;
            indPress.Background = R.UserSettings.PressureBind == ControlModes.NithTPS ? ActiveBrush : BlankBrush;
            indPitchBenad.Background = R.UserSettings.PitchBendBind == ControlModes.NithTPS ? ActiveBrush : BlankBrush;

            indModBS.Background = R.UserSettings.ModulationBind == ControlModes.NithBS ? ActiveBrush : BlankBrush;
            indPressBS.Background = R.UserSettings.PressureBind == ControlModes.NithBS ? ActiveBrush : BlankBrush;
            indPitchBenadBS.Background = R.UserSettings.PitchBendBind == ControlModes.NithBS ? ActiveBrush : BlankBrush;

            indBSwitch.Background = R.UserSettings.BreathControlMode == _BreathControlModes.Switch ? ActiveBrush : BlankBrush;
            indSharpNotes.Background = R.UserSettings.SharpNotesMode == _SharpNotesModes.On ? ActiveBrush : BlankBrush;
            indSlidePlay.Background = R.UserSettings.SlidePlayMode == _SlidePlayModes.On ? ActiveBrush : BlankBrush;
            indToggleCursor.Background = R.NDB.CursorHidden ? ActiveBrush: BlankBrush;
            indToggleAutoScroll.Background = R.NDB.AutoScroller.Enabled ? ActiveBrush : BlankBrush;
            indToggleEyeTracker.Background = R.NDB.TobiiModule.MouseEmulator.Enabled ? ActiveBrush : BlankBrush;
            indSettings.Background = IsSettingsShown ? ActiveBrush : BlankBrush;
            indNoteNames.Background = R.NDB.NetytarSurface.NoteNamesVisualized ? ActiveBrush : BlankBrush;

            /* MIDI */
            txtMIDIch.Text = "MP" + R.NDB.MidiModule.OutDevice.ToString();
            CheckMidiPort();

        }

        private void CheckMidiPort()
        {
            if (R.NDB.MidiModule.IsMidiOk())
            {
                txtMIDIch.Foreground = ActiveBrush;
            }
            else
            {
                txtMIDIch.Foreground = WarningBrush;
            }
        }

        //private void InitializeVolumeBar()
        //{
        //    velocityBarMaxHeight = VelocityBarBorder.ActualHeight;
        //    VelocityBar.Height = 0;
        //    // MaxBar.Height = VelocityBar.Height = (velocityBarMaxHeight * 127) / BreathMax;
        //}

        private void Test(object sender, RoutedEventArgs e)
        {
            R.NDB.NetytarSurface.DrawScale();
        }

        private void BtnScroll_Click(object sender, RoutedEventArgs e)
        {
            R.NDB.AutoScroller.Enabled = !R.NDB.AutoScroller.Enabled;
        }

        private void BtnFFBTest_Click(object sender, RoutedEventArgs e)
        {
            //Rack.DMIBox.FfbModule.FlashFFB();
        }

        private void BtnMIDIchMinus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.UserSettings.MIDIPort--;
                R.NDB.MidiModule.OutDevice = R.UserSettings.MIDIPort;
                //lblMIDIch.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();

                //CheckMidiPort();
                UpdateGUIVisuals();
            }
        }

        private void BtnMIDIchPlus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.UserSettings.MIDIPort++;
                R.NDB.MidiModule.OutDevice = R.UserSettings.MIDIPort;
                //lblMIDIch.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();

                //CheckMidiPort();
                UpdateGUIVisuals();
            }
        }


        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            R.SavingSystem.SaveSettings(R.UserSettings);
            R.NDB.Dispose();
            Close();
        }

        private void btnCtrlEyePos_Click(object sender, RoutedEventArgs e)
        {
           // if (NetytarStarted)
           // {
           //     R.UserSettings.NetytarControlMode = _NetytarControlModes.EyePos;
           //     R.NDB.ResetModulationAndPressure();
           //
           //     BreathSensorValue = 0;
           //
           //     UpdateGUIVisuals();
           // }
        }

        private void btnExit_Activate(object sender, RoutedEventArgs e)
        {
        }

        private void btnCalibrateHeadPose_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (NetytarStarted)
            {
                //btnCalibrateHeadPose.Background = new SolidColorBrush(Colors.LightGreen);

                if (R.NDB.TobiiModule.LastHeadPoseData != null && R.NDB.TobiiModule.LastHeadPoseData.HasHeadPosition)
                {
                    R.NDB.HeadPoseBaseX = R.NDB.TobiiModule.LastHeadPoseData.HeadRotation.X;
                    R.NDB.HeadPoseBaseY = R.NDB.TobiiModule.LastHeadPoseData.HeadRotation.Y;
                    R.NDB.HeadPoseBaseZ = R.NDB.TobiiModule.LastHeadPoseData.HeadRotation.Z;
                }

                R.NDB.CalibrateGyroBase();
                R.NDB.CalibrateAccBase();
            }
        }

        private void btnCalibrateHeadPose_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //btnCalibrateHeadPose.Background = new SolidColorBrush(Colors.Black);
        }

        private void btnTestClick(object sender, RoutedEventArgs e)
        {
            throw (new NotImplementedException("Test button is not set!"));
        }

        private void btnModulationControlSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.UserSettings.ModulationControlMode = R.UserSettings.ModulationControlMode == _ModulationControlModes.On ? _ModulationControlModes.Off : _ModulationControlModes.On;
                UpdateGUIVisuals();
            }
        }


        private void btnPitchBendTPS_Click(object sender, RoutedEventArgs e) //aggiunto per pitch bend 
        {
            if (NetytarStarted)
            {
              
                    if (R.UserSettings.PitchBendBind != ControlModes.NithTPS)
                    {
                        R.UserSettings.PitchBendBind = ControlModes.NithTPS;
                    }
                    else
                    {
                        R.UserSettings.PitchBendBind = ControlModes.Inactive;
                    }
                    UpdateGUIVisuals();

                
            }
        }

        private void btnBreathControlSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if (R.NDB.BreathControlMode == _BreathControlModes.Switch)
                {
                    R.NDB.BreathControlMode = _BreathControlModes.Dynamic;
                }
                else if (R.NDB.BreathControlMode == _BreathControlModes.Dynamic)
                {
                    R.NDB.BreathControlMode = _BreathControlModes.Switch;
                }
            }

            BreathSensorValue = 0;

            UpdateGUIVisuals();
        }

        private void btnCtrlEyeVel_Click(object sender, RoutedEventArgs e)
        {
           // if (NetytarStarted)
           // {
           //     R.UserSettings.NetytarControlMode = _NetytarControlModes.EyeVel;
           //     R.NDB.ResetModulationAndPressure();
           //
           //     BreathSensorValue = 0;
           //
           //     UpdateGUIVisuals();
           // }
        }

        private void btnRemoveSharps_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if (R.UserSettings.SharpNotesMode == _SharpNotesModes.Off)
                {
                    R.UserSettings.SharpNotesMode = _SharpNotesModes.On;
                }
                else if (R.UserSettings.SharpNotesMode == _SharpNotesModes.On)
                {
                    R.UserSettings.SharpNotesMode = _SharpNotesModes.Off;
                }

                UpdateGUIVisuals();
                R.NDB.NetytarSurface.DrawButtons();
            }
        }

        private void btnBlinkSelectScale_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                switch (R.UserSettings.BlinkSelectScaleMode)
                {
                    case _BlinkSelectScaleMode.Off:
                        R.UserSettings.BlinkSelectScaleMode = _BlinkSelectScaleMode.On;
                        break;

                    case _BlinkSelectScaleMode.On:
                        R.UserSettings.BlinkSelectScaleMode = _BlinkSelectScaleMode.Off;
                        break;
                }

                UpdateGUIVisuals();
            }
        }

        private void btnSlidePlay_Click(object sender, RoutedEventArgs e)
        {
            // TODO Rifare (?)

            if (NetytarStarted)
            {
                switch (R.UserSettings.SlidePlayMode)
                {
                    case _SlidePlayModes.Off:
                        R.UserSettings.SlidePlayMode = _SlidePlayModes.On;
                        break;

                    case _SlidePlayModes.On:
                        R.UserSettings.SlidePlayMode = _SlidePlayModes.Off;
                        break;
                }

                UpdateGUIVisuals();
            }
        }

        private void btnNeutral_Click(object sender, RoutedEventArgs e)
        {
        }


        private void btnNoCursor_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.NDB.CursorHidden = !R.NDB.CursorHidden;
                Cursor = R.NDB.CursorHidden ? System.Windows.Input.Cursors.None : System.Windows.Input.Cursors.Arrow;
            }

            UpdateGUIVisuals();
        }

        private void btnToggleEyeTracker_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.NDB.TobiiModule.MouseEmulator.Enabled = !R.NDB.TobiiModule.MouseEmulator.Enabled;
            }

            UpdateGUIVisuals();
        }

        private void btnToggleAutoScroll_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.NDB.AutoScroller.Enabled = !R.NDB.AutoScroller.Enabled;
            }

            UpdateGUIVisuals();
        }

        public bool IsSettingsShown { get; set; } = false;
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            switch (IsSettingsShown)
            {
                case false:
                    IsSettingsShown = true;
                    brdSettings.Visibility = Visibility.Visible;
                    break;

                case true:
                    IsSettingsShown = false;
                    brdSettings.Visibility = Visibility.Hidden;
                    brdPress.Visibility = Visibility.Hidden;
                    break;
            }

            UpdateGUIVisuals();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartNetytar();
        }

        private void btnScaleMajor_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.UserSettings.ScaleCode = ScaleCodes.maj;
                R.NDB.NetytarSurface.Scale = new Scale(R.UserSettings.RootNote, R.UserSettings.ScaleCode);
                UpdateGUIVisuals();
            }
        }

        private void btnScaleMinor_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.UserSettings.ScaleCode = ScaleCodes.min;
                R.NDB.NetytarSurface.Scale = new Scale(R.UserSettings.RootNote, R.UserSettings.ScaleCode);
                UpdateGUIVisuals();
            }
        }

        private void btnModTPS_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                    if (R.UserSettings.ModulationBind != ControlModes.NithTPS)
                    {
                        R.UserSettings.ModulationBind = ControlModes.NithTPS;
                    }
                    else
                    {
                        R.UserSettings.ModulationBind = ControlModes.Inactive;
                    }

                    UpdateGUIVisuals();            
            }
        }

        private void btnBSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.UserSettings.BreathControlMode = R.UserSettings.BreathControlMode == _BreathControlModes.Dynamic ? _BreathControlModes.Switch : _BreathControlModes.Dynamic;
                UpdateGUIVisuals();
            }
        }

        private void btnSharpNotes_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.UserSettings.SharpNotesMode = R.UserSettings.SharpNotesMode == _SharpNotesModes.On ? _SharpNotesModes.Off : _SharpNotesModes.On;
                R.NDB.NetytarSurface.DrawButtons();
                UpdateGUIVisuals();
            }
        }

        private void btnBlinkPlay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSlidePlay_Click_1(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.UserSettings.SlidePlayMode = R.UserSettings.SlidePlayMode == _SlidePlayModes.On ? _SlidePlayModes.Off : _SlidePlayModes.On;
                UpdateGUIVisuals();
            }
        }

        private void btnSpacingMinus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if (R.UserSettings.HorizontalSpacer > R.HORIZONTALSPACING_MIN)
                {
                    R.UserSettings.HorizontalSpacer -= 10;
                    R.UserSettings.VerticalSpacer = -(R.UserSettings.HorizontalSpacer / 2);
                }

                R.NDB.NetytarSurface.DrawButtons();
                UpdateGUIVisuals();
            }
        }

        private void btnSpacingPlus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if(R.UserSettings.HorizontalSpacer < R.HORIZONTALSPACING_MAX)
                {
                    R.UserSettings.HorizontalSpacer += 10;
                    R.UserSettings.VerticalSpacer = -(R.UserSettings.HorizontalSpacer / 2);
                }

                R.NDB.NetytarSurface.DrawButtons();
                UpdateGUIVisuals();
            }
        }


        private void btnBreath_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if (R.UserSettings.BS_activateBreath == true)
                {
                    R.UserSettings.BS_activateBreath = false;
                }
                else if (R.UserSettings.BS_activateBreath == false)
                {
                    R.UserSettings.BS_activateBreath = true;
                }
                //R.UserSettings.TPS_activateTeeth=true;
                UpdateGUIVisuals();
            }
        }

        private void btnTeeth_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if (R.UserSettings.TPS_activateTeeth == true)
                {
                    R.UserSettings.TPS_activateTeeth = false;
                }
                else if (R.UserSettings.TPS_activateTeeth == false)
                {
                    R.UserSettings.TPS_activateTeeth = true;
                }
                //R.UserSettings.TPS_activateTeeth=true;
                UpdateGUIVisuals();
            }
        }

        private void btnNoteNames_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.UserSettings.NoteNamesVisualized = !R.UserSettings.NoteNamesVisualized;
                R.NDB.NetytarSurface.NoteNamesVisualized = R.UserSettings.NoteNamesVisualized;
                UpdateGUIVisuals();
            }
        }

        #region Global SettingsButtons

        public void Global_NetytarButtonMouseEnter()
        {
            if (NetytarStarted)
            {
                if (LastSettingsGazedButton != null)
                {
                    // Reset Previous Button
                    LastSettingsGazedButton.Background = LastGazedBrush;
                    LastSettingsGazedButton = null;
                }
            }
        }

        private void Global_SettingsButton_MouseEnter(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if (LastSettingsGazedButton != null)
                {
                    // Reset Previous Button
                    LastSettingsGazedButton.Background = LastGazedBrush;
                }

                LastSettingsGazedButton = (Button)sender;
                LastGazedBrush = LastSettingsGazedButton.Background;
                LastSettingsGazedButton.Background = GazeButtonColor;
            }
        }

        #endregion Global SettingsButtons

        private void btnRootNotePlus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.UserSettings.RootNote = R.UserSettings.RootNote.Next();
                R.NDB.NetytarSurface.Scale = new Scale(R.UserSettings.RootNote, R.UserSettings.ScaleCode);
                UpdateGUIVisuals();
            }
        }

        private void btnRootNoteMinus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                R.UserSettings.RootNote = R.UserSettings.RootNote.Previous();
                R.NDB.NetytarSurface.Scale = new Scale(R.UserSettings.RootNote, R.UserSettings.ScaleCode);
                UpdateGUIVisuals();
            }
        }

        private void btnHeadYaw_Click(object sender, RoutedEventArgs e)
        {
           if (NetytarStarted)
           {
               R.UserSettings.NetytarControlMode = ControlModes.NithHT;
                if (R.UserSettings.HT_activateHT == false)
                {
                    R.UserSettings.HT_activateHT = true;
                }
                else if (R.UserSettings.HT_activateHT == true)
                {
                    R.UserSettings.HT_activateHT = false;
                }
                UpdateGUIVisuals();
           }
           brdPress.Visibility = Visibility.Hidden;
        }

        private void btnCalibrateMin_Click(object sender, RoutedEventArgs e)
        {
            R.TPhelper.CalibrateMinValue = R.NDB.BreathValue;
            R.TPhelper.MinSet = true;
            CalibrateMin_txt.Text = R.TPhelper.CalibrateMinValue.ToString();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5); // Imposta l'intervallo di tempo in secondi
            timer.Tick += Timer_TickMax;
            timer.Start();
            Instruction.Text = "Mettere il sensore in bocca, \n esercitare la massima pressione possibile,\n premere il pulsante quando appare. \n Cerca di rimanere stabile durante la calibraizone";
        }

        private void btnCalibrateMax_Click(object sender, RoutedEventArgs e)
        {
           R.TPhelper.CalibrateMaxValue = int.Parse(R.NDB.pressureSensor);
            R.TPhelper.MaxSet = true;
            CalibrateMax_txt.Text = R.TPhelper.CalibrateMaxValue.ToString();
            R.SavingCalibration.SaveCalibration(R.TPhelper.CalibrateMinValue.ToString(), R.TPhelper.CalibrateMaxValue.ToString());
        }

        private void btnEnableCalibration_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5); // Imposta l'intervallo di tempo in secondi
            timer.Tick += Timer_TickMin;
            timer.Start();
            Instruction.Text = "Mettere il sensore in bocca, \n esercitare la minima pressione possibile,\n premere il pulsante quando appare. \n Cerca di rimanere stabile durante la calibraizone";
            //R.TPhelper.CalibrateMaxValue = 1024;
            //R.TPhelper.CalibrateMaxValue = 0;
        }

        private void Timer_TickMin(object sender, EventArgs e)
        {
            CalibrateMin.Visibility = Visibility.Visible; // Abilita il pulsante
            timer.Stop(); // Ferma il timer (opzionale)
        }

        private void Timer_TickMax(object sender, EventArgs e)
        {
            CalibrateMax.Visibility = Visibility.Visible; // Abilita il pulsante
            timer.Stop(); // Ferma il timer (opzionale)
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            brdPress.Visibility = Visibility.Hidden;
        }



      

        private void BtnSensorPortTPSMinus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                TPS_SenorPort--;
                UpdateSensorTPSConnection();
            }
        }

        private void BtnSensorPortTPSPlus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                TPS_SenorPort++;
                UpdateSensorTPSConnection();
            }
        }

        private void UpdateSensorTPSConnection()
        {
            txtSensorPressPort.Text = "COM" + TPS_SenorPort.ToString();

            if (R.NithModuleTPS.Connect(TPS_SenorPort))
            {
                txtSensorPressPort.Foreground = ActiveBrush;
            }
            else
            {
                txtSensorPressPort.Foreground = WarningBrush;
            }
        }

        private void BtnSensorBSPortMinus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                BS_SenorPort--;
                UpdateSensorBSConnection();
            }
        }

        private void BtnSensorBSPortPlus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                BS_SenorPort++;
                UpdateSensorBSConnection();
            }
        }

        private void UpdateSensorBSConnection() 
        {
            txtSensorBSPort.Text = "COM" + BS_SenorPort.ToString();

            if (R.NithModuleBS.Connect(BS_SenorPort))
            {
                txtSensorBSPort.Foreground = ActiveBrush;
            }
            else
            {
                txtSensorBSPort.Foreground = WarningBrush;
            }
        }

        private void btnHeadYawSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTeethSettings_Click(object sender, RoutedEventArgs e)
        {
            if (brdPress.Visibility == Visibility.Visible)
            {
                brdPress.Visibility = Visibility.Hidden;
            }
            else if (brdPress.Visibility==Visibility.Hidden)
            {
                brdPress.Visibility = Visibility.Visible;
                brdBS.Visibility = Visibility.Hidden;
            }
        }

        private void btnBreathSettings_Click(object sender, RoutedEventArgs e)
        {
            if (brdBS.Visibility == Visibility.Visible)
            {
                brdBS.Visibility = Visibility.Hidden;
            }
            else if (brdBS.Visibility == Visibility.Hidden)
            {
                brdPress.Visibility = Visibility.Hidden;
                brdBS.Visibility = Visibility.Visible;
            }
        }

        private void btnPressBS_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if (R.UserSettings.PressureBind != ControlModes.NithBS)
                {
                    R.UserSettings.PressureBind = ControlModes.NithBS;
                }
                else
                {
                    R.UserSettings.PressureBind = ControlModes.Inactive;
                }

                UpdateGUIVisuals();
                

            }
        }

        private void btnPressTPS_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if (R.UserSettings.PressureBind != ControlModes.NithTPS)
                {
                    R.UserSettings.PressureBind = ControlModes.NithTPS;
                }
                else
                {
                    R.UserSettings.PressureBind = ControlModes.Inactive;
                }
          
                //R.UserSettings.PressureControlMode = R.UserSettings.PressureControlMode == _PressureControlModes.On ? _PressureControlModes.Off : _PressureControlModes.On;
                //R.UserSettings.PressureControlMode = _PressureControlModes.On;
                UpdateGUIVisuals();
                //MessageBox.Show(R.UserSettings.PressureBind.ToString());
            }
        }

        private void btnModBS_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if (R.UserSettings.ModulationBind != ControlModes.NithBS)
                {
                    R.UserSettings.ModulationBind = ControlModes.NithBS;
                }
                else
                {
                    R.UserSettings.ModulationBind = ControlModes.Inactive;
                }
                UpdateGUIVisuals();

            }
        }

        private void btnPitchBendBS_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if (R.UserSettings.PitchBendBind != ControlModes.NithBS)
                {
                    R.UserSettings.PitchBendBind = ControlModes.NithBS;
                }
                else
                {
                    R.UserSettings.PitchBendBind = ControlModes.Inactive;
                }
                UpdateGUIVisuals();

            }
        }
    }
}