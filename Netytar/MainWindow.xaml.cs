using NAudio.Wave;
using NeeqDMIs.Music;
using Netytar.DMIbox;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Tobii.Interaction.Wpf;

namespace Netytar
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int BreathMax = 340;
        private readonly SolidColorBrush ActiveBrush = new SolidColorBrush(Colors.LightGreen);
        private readonly SolidColorBrush WarningBrush = new SolidColorBrush(Colors.DarkRed);
        private readonly SolidColorBrush BlankBrush = new SolidColorBrush(Colors.Black);
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        private int breathSensorValue = 0;
        private Scale StartingScale = ScalesFactory.Cmaj;
        private Scale lastScale;
        private Scale selectedScale;
        private bool NetytarStarted = false;
        private Timer updater;
        private double velocityBarMaxHeight = 0;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            updater = new Timer();
            updater.Interval = 10;
            updater.Tick += UpdateWindow;
            updater.Start();

            lastScale = StartingScale;
            SelectedScale = StartingScale;
        }

        public int BreathSensorValue { get => breathSensorValue; set => breathSensorValue = value; }

        public Scale SelectedScale { get => selectedScale; set => selectedScale = value; }

        public int SensorPort
        {
            get { return Rack.UserSettings.SensorPort; }
            set
            {
                if (value > 0)
                {
                    Rack.UserSettings.SensorPort = value;
                }
            }
        }

        public void ReceiveNoteChange()
        {
            txtPitch.Text = Rack.DMIBox.SelectedNote.ToPitchValue().ToString();
            txtNoteName.Text = Rack.DMIBox.SelectedNote.ToStandardString();
        }

        public void ReceiveBlowingChange()
        {
            if (Rack.DMIBox.Blow)
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
            Rack.DMIBox.NetytarSurface.Scale = new Scale(Rack.DMIBox.SelectedNote.ToAbsNote(), scaleCode);
        }

        private void eyeGazeHandler(object sender, HasGazeChangedRoutedEventArgs e)
        {
            if (e.HasGaze)
            {
                Rack.DMIBox.HasAButtonGaze = true;
                Rack.DMIBox.LastGazedButton = (System.Windows.Controls.Button)sender;
            }
            else
            {
                Rack.DMIBox.HasAButtonGaze = false;
            }
        }

        private void UpdateWindow(object sender, EventArgs e)
        {
            if (NetytarStarted)
            {
                VelocityBar.Height = (velocityBarMaxHeight * breathSensorValue) / BreathMax;

                if (SelectedScale.GetName().Equals(lastScale.GetName()) == false)
                {
                    lastScale = selectedScale;
                    Rack.DMIBox.NetytarSurface.Scale = selectedScale;
                }

                txtNoteName.Text = Rack.DMIBox.SelectedNote.ToStandardString();
                txtPitch.Text = Rack.DMIBox.SelectedNote.ToPitchValue().ToString();
                if (Rack.DMIBox.Blow)
                {
                    txtIsBlowing.Text = "B";
                }
                else
                {
                    txtIsBlowing.Text = "_";
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
        }

        private void StartNetytar(object sender, RoutedEventArgs e)
        {
            AddScaleListItems();

            NetytarSetup netytarSetup = new NetytarSetup(this);
            netytarSetup.Setup();

            //wpfInteractorAgent = NetytarRack.DMIBox.TobiiModule.TobiiHost.InitializeWpfAgent();

            InitializeVolumeBar();
            InitializeSensorPortText();

            if (Rack.UserSettings.NetytarControlMode == _NetytarControlModes.Keyboard)
            {
                indCtrlKeyboard.Background = ActiveBrush;
            }

            if (Rack.UserSettings.NetytarControlMode == _NetytarControlModes.BreathSensor)
            {
                indCtrlBreath.Background = ActiveBrush;
            }

            btnStart.IsEnabled = false;
            btnStart.Foreground = new SolidColorBrush(Colors.Black);

            CheckMidiPort();

            breathSensorValue = 0;

            UpdateIndicators();

            NetytarStarted = true; // LEAVE AT THE END!

            
        }

        private void UpdateIndicators()
        {
            switch (Rack.UserSettings.NetytarControlMode)
            {
                case _NetytarControlModes.BreathSensor:
                    indCtrlKeyboard.Background = BlankBrush;
                    indCtrlBreath.Background = ActiveBrush;
                    indCtrlEyePos.Background = BlankBrush;
                    indCtrlEyeVel.Background = BlankBrush;
                    break;

                case _NetytarControlModes.EyePos:
                    indCtrlKeyboard.Background = BlankBrush;
                    indCtrlBreath.Background = BlankBrush;
                    indCtrlEyePos.Background = ActiveBrush;
                    indCtrlEyeVel.Background = BlankBrush;
                    break;

                case _NetytarControlModes.Keyboard:
                    indCtrlKeyboard.Background = ActiveBrush;
                    indCtrlBreath.Background = BlankBrush;
                    indCtrlEyePos.Background = BlankBrush;
                    indCtrlEyeVel.Background = BlankBrush;
                    break;

                case _NetytarControlModes.EyeVel:
                    indCtrlKeyboard.Background = BlankBrush;
                    indCtrlBreath.Background = BlankBrush;
                    indCtrlEyePos.Background = BlankBrush;
                    indCtrlEyeVel.Background = ActiveBrush;
                    break;
            }

            switch (Rack.DMIBox.ModulationControlMode)
            {
                case _ModulationControlModes.On:
                    indModulationControl.Background = ActiveBrush;
                    break;

                case _ModulationControlModes.Off:
                    indModulationControl.Background = BlankBrush;
                    break;
            }

            switch (Rack.DMIBox.BreathControlMode)
            {
                case _BreathControlModes.Switch:
                    indBreathSwitch.Background = ActiveBrush;
                    break;

                case _BreathControlModes.Dynamic:
                    indBreathSwitch.Background = BlankBrush;
                    break;
            }

            switch (Rack.UserSettings.SharpNotesMode)
            {
                case _SharpNotesModes.On:
                    indShowSharps.Background = ActiveBrush;
                    break;

                case _SharpNotesModes.Off:
                    indShowSharps.Background = WarningBrush;
                    break;
            }

            switch (Rack.UserSettings.BlinkSelectScaleMode)
            {
                case _BlinkSelectScaleMode.On:
                    indBlinkSelectScale.Background = ActiveBrush;
                    break;

                case _BlinkSelectScaleMode.Off:
                    indBlinkSelectScale.Background = WarningBrush;
                    break;
            }

            switch (Rack.UserSettings.SlidePlayMode)
            {
                case _SlidePlayModes.On:
                    indSlidePlay.Background = ActiveBrush;
                    break;

                case _SlidePlayModes.Off:
                    indSlidePlay.Background = WarningBrush;
                    break;
            }

            sldDistance.Value = Rack.UserSettings.HorizontalSpacer;

            /* MIDI */
            lblMIDIch.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();
            CheckMidiPort();
        }

        private void CheckMidiPort()
        {
            if (Rack.DMIBox.MidiModule.IsMidiOk())
            {
                lblMIDIch.Foreground = ActiveBrush;
            }
            else
            {
                lblMIDIch.Foreground = WarningBrush;
            }
        }

        private void InitializeSensorPortText()
        {
            txtSensorPort.Foreground = WarningBrush;
            txtSensorPort.Text = "COM" + SensorPort;
            UpdateSensorConnection();
        }

        private void InitializeVolumeBar()
        {
            velocityBarMaxHeight = VelocityBarBorder.ActualHeight;
            VelocityBar.Height = 0;
            MaxBar.Height = VelocityBar.Height = (velocityBarMaxHeight * 127) / BreathMax;
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            Rack.DMIBox.NetytarSurface.DrawScale();
        }

        private void AddScaleListItems()
        {
            foreach (Scale scale in ScalesFactory.GetList())
            {
                ListBoxItem item = new ListBoxItem() { Content = scale.GetName() };
                lstScaleChanger.Items.Add(item);
            }
        }

        private void BtnScroll_Click(object sender, RoutedEventArgs e)
        {
            Rack.DMIBox.AutoScroller.Enabled = !Rack.DMIBox.AutoScroller.Enabled;
        }

        private void BtnFFBTest_Click(object sender, RoutedEventArgs e)
        {
            //Rack.DMIBox.FfbModule.FlashFFB();
        }

        private void LstScaleChanger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Rack.DMIBox.NetytarSurface.Scale = Scale.FromString(((ListBoxItem)lstScaleChanger.SelectedItem).Content.ToString());
        }

        private void btnMIDIchMinus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                Rack.UserSettings.MIDIPort--;
                Rack.DMIBox.MidiModule.OutDevice = Rack.UserSettings.MIDIPort;
                //lblMIDIch.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();

                //CheckMidiPort();
                UpdateIndicators();
            }
        }

        private void btnMIDIchPlus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                Rack.UserSettings.MIDIPort++;
                Rack.DMIBox.MidiModule.OutDevice = Rack.UserSettings.MIDIPort;
                //lblMIDIch.Text = "MP" + Rack.DMIBox.MidiModule.OutDevice.ToString();

                //CheckMidiPort();
                UpdateIndicators();
            }
        }

        private void btnCtrlKeyboard_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                Rack.UserSettings.NetytarControlMode = _NetytarControlModes.Keyboard;
                Rack.DMIBox.ResetModulationAndPressure();

                breathSensorValue = 0;

                UpdateIndicators();
            }
        }

        private void btnCtrlBreath_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                Rack.UserSettings.NetytarControlMode = _NetytarControlModes.BreathSensor;
                Rack.DMIBox.ResetModulationAndPressure();

                breathSensorValue = 0;

                UpdateIndicators();
            }
        }

        private void btnSensorPortPlus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                SensorPort++;
                UpdateSensorConnection();
            }
        }

        private void UpdateSensorConnection()
        {
            txtSensorPort.Text = "COM" + SensorPort.ToString();

            if (Rack.DMIBox.SensorReader.Connect(SensorPort))
            {
                txtSensorPort.Foreground = ActiveBrush;
            }
            else
            {
                txtSensorPort.Foreground = WarningBrush;
            }
        }

        private void btnSensorPortMinus_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                SensorPort--;
                UpdateSensorConnection();
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Rack.DMIBox.Dispose();
            Close();
        }

        private void btnCtrlEyePos_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                Rack.UserSettings.NetytarControlMode = _NetytarControlModes.EyePos;
                Rack.DMIBox.ResetModulationAndPressure();

                breathSensorValue = 0;

                UpdateIndicators();
            }
        }

        private void btnExit_Activate(object sender, RoutedEventArgs e)
        {
        }

        private void btnCalibrateHeadPose_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (NetytarStarted)
            {
                //btnCalibrateHeadPose.Background = new SolidColorBrush(Colors.LightGreen);

                if (Rack.DMIBox.TobiiModule.LastHeadPoseData != null && Rack.DMIBox.TobiiModule.LastHeadPoseData.HasHeadPosition)
                {
                    Rack.DMIBox.HeadPoseBaseX = Rack.DMIBox.TobiiModule.LastHeadPoseData.HeadRotation.X;
                    Rack.DMIBox.HeadPoseBaseY = Rack.DMIBox.TobiiModule.LastHeadPoseData.HeadRotation.Y;
                    Rack.DMIBox.HeadPoseBaseZ = Rack.DMIBox.TobiiModule.LastHeadPoseData.HeadRotation.Z;
                }

                Rack.DMIBox.CalibrateGyroBase();
                Rack.DMIBox.CalibrateAccBase();
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
                if (Rack.DMIBox.ModulationControlMode == _ModulationControlModes.Off)
                {
                    Rack.DMIBox.ModulationControlMode = _ModulationControlModes.On;
                }
                else if (Rack.DMIBox.ModulationControlMode == _ModulationControlModes.On)
                {
                    Rack.DMIBox.ModulationControlMode = _ModulationControlModes.Off;
                }
            }

            breathSensorValue = 0;

            UpdateIndicators();
        }

        private void btnBreathControlSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if (Rack.DMIBox.BreathControlMode == _BreathControlModes.Switch)
                {
                    Rack.DMIBox.BreathControlMode = _BreathControlModes.Dynamic;
                }
                else if (Rack.DMIBox.BreathControlMode == _BreathControlModes.Dynamic)
                {
                    Rack.DMIBox.BreathControlMode = _BreathControlModes.Switch;
                }
            }

            breathSensorValue = 0;

            UpdateIndicators();
        }

        private void btnCtrlEyeVel_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                Rack.UserSettings.NetytarControlMode = _NetytarControlModes.EyeVel;
                Rack.DMIBox.ResetModulationAndPressure();

                breathSensorValue = 0;

                UpdateIndicators();
            }
        }

        private void Button_Play(object sender, RoutedEventArgs e)
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
            if (audioFile == null)
            {
                audioFile = new AudioFileReader(@"C:\Users\Entica\Documents\GitHub\Netytar_CustomInterface\Netytar_CustomInterface\Netytar\Audio\80.mp3");
                outputDevice.Init(audioFile);
            }
            outputDevice.Play();
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            outputDevice?.Stop();
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            outputDevice = null;
            audioFile.Dispose();
            audioFile = null;
        }

        private void sldDistance_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (NetytarStarted)
            {
                Rack.UserSettings.HorizontalSpacer = (int)sldDistance.Value;
                Rack.UserSettings.VerticalSpacer = -(int)(sldDistance.Value / 2);
                Rack.DMIBox.NetytarSurface.DrawButtons();
            }
        }

        private void btnRemoveSharps_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                if (Rack.UserSettings.SharpNotesMode == _SharpNotesModes.Off)
                {
                    Rack.UserSettings.SharpNotesMode = _SharpNotesModes.On;
                }
                else if (Rack.UserSettings.SharpNotesMode == _SharpNotesModes.On)
                {
                    Rack.UserSettings.SharpNotesMode = _SharpNotesModes.Off;
                }

                UpdateIndicators();
                Rack.DMIBox.NetytarSurface.DrawButtons();
            }
        }

        private void btnBlinkSelectScale_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                switch (Rack.UserSettings.BlinkSelectScaleMode)
                {
                    case _BlinkSelectScaleMode.Off:
                        Rack.UserSettings.BlinkSelectScaleMode = _BlinkSelectScaleMode.On;
                        break;

                    case _BlinkSelectScaleMode.On:
                        Rack.UserSettings.BlinkSelectScaleMode = _BlinkSelectScaleMode.Off;
                        break;
                }

                UpdateIndicators();
            }
        }

        private void btnSlidePlay_Click(object sender, RoutedEventArgs e)
        {
            if (NetytarStarted)
            {
                switch (Rack.UserSettings.SlidePlayMode)
                {
                    case _SlidePlayModes.Off:
                        Rack.UserSettings.SlidePlayMode = _SlidePlayModes.On;
                        break;

                    case _SlidePlayModes.On:
                        Rack.UserSettings.SlidePlayMode = _SlidePlayModes.Off;
                        break;
                }

                UpdateIndicators();
            }
        }
    }
}