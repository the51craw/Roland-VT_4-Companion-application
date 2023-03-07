using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using UwpControlsLibrary;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VT_4
{
    public sealed partial class MainPage : Page
    {
        public enum ControlId
        {
            STATICIMAGE,        // 0 
        }

        public enum TimerAction
        {
            NONE,
            WAIT_FOR_INITIALIZATION_DONE,
            UPDATE_GUI_FROM_PATCH,
            HANDLE_EFFECT_BUTTON_PRESS,
            HANDLE_CC_MESSAGE,
            HANDLE_PB_MESSAGE,
            HANDLE_ROBOT_VARIATION_CHANGE,
            HANDLE_MEGAPHONE_VARIATION_CHANGE,
            HANDLE_VOCODER_VARIATION_CHANGE,
            HANDLE_HARMONY_VARIATION_CHANGE,
            HANDLE_REVERB_VARIATION_CHANGE,
            SELECT_PATCH_1,
            SELECT_PATCH_2,
            SELECT_PATCH_3,
            SELECT_PATCH_4,
            SELECT_PATCH_5,
            SELECT_PATCH_6,
            SELECT_PATCH_7,
            SELECT_PATCH_8,
            SELECT_TEMPORARY_PATCH,
            TURN_ON_ROBOT,
            NO_MIDI_RESPONSE,
        }

        public MainPage()
        {
            this.InitializeComponent();

            // Fix the title bar:
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Color.FromArgb(0, 128, 128, 128);
            titleBar.ButtonForegroundColor = Color.FromArgb(0, 128, 128, 128);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // When imgClickArea is opened it has also got its size, so now
        // we can create the controls object:
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        private async void imgClickArea_ImageOpened(object sender, RoutedEventArgs e)
        {
            CreateControls();
            VT4 = new VT4();
            MidiEvents = new List<MidiEvents>();
            timerAction = new List<MidiEvents>();

            await InitMidi();
            SendProgramChange(1);
            sceneIndex = 0;

            midi.SendControlChange(midi.PortPairs[0], 51, 0);
            ReadVt4();
            byte channel = (byte)(VT4.System.MIDI_CH); 
            channel = (byte)(channel > 0 ? channel - 1 : 0);   // Minus one because 0 is OFF, which we disregard.
            channel = (byte)(channel > 15 ? 15 : channel);     // Because above 15 is ALL which we also disregard.
            pmbMidiChannels[channel].Set(true);
            midi.PortPairs[0].OutChannel = channel;
            midi.PortPairs[0].InChannel = channel;
            SetControlValues();
            SetEqualizerParameterTexts();
            SetVariationNames();
            SetParameterTexts(0x08);
            SetParameterTexts(0x09);
            SetParameterTexts(0x0a);
            SetParameterTexts(0x0b);
            SetParameterTexts(0x0c);

            //midi.SendControlChange(midi.PortPairs[0], 51, 0);
            sceneIndex = 0;
            sceneEdited[sceneIndex] = true;

            // Megaphone sometimes has only three parameters. When calling
            // SetParameterTexts(0x0b) the extra one might be set to visible,
            // set it to collapsed again:
            pmbMegaphone.Children[1][3].Visibility = Visibility.Collapsed;

            while (VT4.UserPatch[0].Name == null)
            {
                await DisplayConnectionProblem(1);
            }

            lblPatch.TextBlock.Text = VT4.TemporaryPatch.Name;
            settings = new Settings(this);
            //settings.Load();

            // There is aproblem with UserRobot's names.
            // They always reads as 0 (not spaces) even
            // if just written to. I do not know if this
            // is an issue with my VT-4 only, or if it
            // is something Roland abandoned since they
            // do not use the names. However, they do
            // have names for all other effect variations
            // even though they do not use them either.
            // Maybe I accidentally wiped the UserRobot's
            // names from my TV-4 when testing the menu
            // item name change, but I also did that with
            // megaphone, and those names are still there.
            // Anyway, I fetch from hardcoded instead:
            pmbRobot.Children[0][0].TextBlock.Text = "Normal  ";
            pmbRobot.Children[0][1].TextBlock.Text = "Octave-1";
            pmbRobot.Children[0][2].TextBlock.Text = "Octave+1";
            pmbRobot.Children[0][3].TextBlock.Text = "Feedback";
            pmbRobot.Children[0][4].TextBlock.Text = "Octave-2";
            pmbRobot.Children[0][5].TextBlock.Text = "FB&Oct-1";
            pmbRobot.Children[0][6].TextBlock.Text = "FB&Oct+1";
            pmbRobot.Children[0][7].TextBlock.Text = "FB&Res  ";

            VT4.UserRobot[0].OCTAVE = 2;
            VT4.UserRobot[0].FEEDBACK_SWITCH = 0;
            VT4.UserRobot[0].FEEDBACK_RESONANCE = 0;
            VT4.UserRobot[0].FEEDBACK_LEVEL = 0;
            VT4.UserRobot[0].NAME_00_03 = 1383031407;
            VT4.UserRobot[0].NAME_04_07 = 1948262432;
            VT4.UserRobot[0].Name = "Normal  ";

            VT4.UserRobot[1].OCTAVE = 1;
            VT4.UserRobot[1].FEEDBACK_SWITCH = 0;
            VT4.UserRobot[1].FEEDBACK_RESONANCE = 0;
            VT4.UserRobot[1].FEEDBACK_LEVEL = 0;
            VT4.UserRobot[1].NAME_00_03 = 1331917921;
            VT4.UserRobot[1].NAME_04_07 = 1986342193;
            VT4.UserRobot[1].Name = "Octave-1";

            VT4.UserRobot[2].OCTAVE = 3;
            VT4.UserRobot[2].FEEDBACK_SWITCH = 0;
            VT4.UserRobot[2].FEEDBACK_RESONANCE = 0;
            VT4.UserRobot[2].FEEDBACK_LEVEL = 0;
            VT4.UserRobot[2].NAME_00_03 = 1331917921;
            VT4.UserRobot[2].NAME_04_07 = 1986341681;
            VT4.UserRobot[2].Name = "Octave+1";

            VT4.UserRobot[3].OCTAVE = 2;
            VT4.UserRobot[3].FEEDBACK_SWITCH = 1;
            VT4.UserRobot[3].FEEDBACK_RESONANCE = 131;
            VT4.UserRobot[3].FEEDBACK_LEVEL = 150;
            VT4.UserRobot[3].NAME_00_03 = 1181050212;
            VT4.UserRobot[3].NAME_04_07 = 1650549611;
            VT4.UserRobot[3].Name = "Feedback";

            VT4.UserRobot[4].OCTAVE = 0;
            VT4.UserRobot[4].FEEDBACK_SWITCH = 0;
            VT4.UserRobot[4].FEEDBACK_RESONANCE = 0;
            VT4.UserRobot[4].FEEDBACK_LEVEL = 0;
            VT4.UserRobot[4].NAME_00_03 = 1331917921;
            VT4.UserRobot[4].NAME_04_07 = 1986342194;
            VT4.UserRobot[4].Name = "Octave-2";

            VT4.UserRobot[5].OCTAVE = 1;
            VT4.UserRobot[5].FEEDBACK_SWITCH = 1;
            VT4.UserRobot[5].FEEDBACK_RESONANCE = 131;
            VT4.UserRobot[5].FEEDBACK_LEVEL = 150;
            VT4.UserRobot[5].NAME_00_03 = 1180975713;
            VT4.UserRobot[5].NAME_04_07 = 1667968305;
            VT4.UserRobot[5].Name = "FB&Oct-1";

            VT4.UserRobot[6].OCTAVE = 3;
            VT4.UserRobot[6].FEEDBACK_SWITCH = 1;
            VT4.UserRobot[6].FEEDBACK_RESONANCE = 131;
            VT4.UserRobot[6].FEEDBACK_LEVEL = 150;
            VT4.UserRobot[6].NAME_00_03 = 1180975713;
            VT4.UserRobot[6].NAME_04_07 = 1667967793;
            VT4.UserRobot[6].Name = "FB&Oct+1";

            VT4.UserRobot[7].OCTAVE = 2;
            VT4.UserRobot[7].FEEDBACK_SWITCH = 1;
            VT4.UserRobot[7].FEEDBACK_RESONANCE = 200;
            VT4.UserRobot[7].FEEDBACK_LEVEL = 100;
            VT4.UserRobot[7].NAME_00_03 = 1282371175;
            VT4.UserRobot[7].NAME_04_07 = 1180983915;
            VT4.UserRobot[7].Name = "FB&Res  ";

            // Knobs and sliders are not reported when
            // reading the VT4, so we give them standard
            // values and update the VT4:
            SetKnobsAndSliders();
            SendVolume();
            SendMicSens();

            HandsOffTimer = new DispatcherTimer();
            HandsOffTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            HandsOffTimer.Tick += HandsOffTimer_Tick;

            MidiWatchTimer = new DispatcherTimer();
            MidiWatchTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            MidiWatchTimer.Tick += MidiWatchTimer_Tick;

            BlinkTimer = new DispatcherTimer();
            BlinkTimer.Interval = new TimeSpan(0, 0, 0, 0, 235);
            BlinkTimer.Tick += BlinkTimer_Tick;

            EventTimer = new DispatcherTimer();
            EventTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            EventTimer.Tick += EventTimer_Tick;

            ReleaseManualButtonTimer = new DispatcherTimer();
            ReleaseManualButtonTimer.Interval = new TimeSpan(0, 0, 0, 0, 2);
            ReleaseManualButtonTimer.Tick += ReleaseManualButtonTimer_Tick;

            ReleaseEffectButtonTimer = new DispatcherTimer();
            ReleaseEffectButtonTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            ReleaseEffectButtonTimer.Tick += ReleaseEffectButtonTimer_Tick;

            MidiWatchTimer.Start();
            BlinkTimer.Start();
            EventTimer.Start();

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            // Make sure all controls has the correct size and position:
            Controls.ResizeControls(gridControls, Window.Current.Bounds);
            UpdateLayout();

            timerAction.Add(new MidiEvents(TimerAction.WAIT_FOR_INITIALIZATION_DONE, null));
        }

        public async Task InitMidi()
        {
            portFound = false;
            int i = 50;
            deviceNames = new List<String>() { "VT-4" };
            midi = new MIDI(this);
            await midi.Init(deviceNames);
            while (i > 0 && !portFound)
            {
                Thread.Sleep(1);
                Debug.WriteLine(i.ToString());
                if (midi.PortPairs[0] != null && midi.PortPairs[0].InPort != null)
                {
                    portFound = true;
                }
                i--;
            }
            if (!portFound)
            {
                await DisplayConnectionProblem(0);
            }
            else
            {
                midi.PortPairs[0].Manufacturer = 0x41; // Roland
                midi.PortPairs[0].Model = new byte[] { 0x10, 0x00, 0x00, 0x00, 0x51 };
            }
        }

        public async Task DisplayConnectionProblem(Int32 type)
        {
            MessageDialog problem;

            portFound = false;

            while (!portFound)
            {
                if (type == 0)
                {
                    problem = new MessageDialog(
                        "Please connect your VT-4 and turn it on.\nWait at least 30 seconds, then press Retry, \n"
                        + "or press \"Close VT-4 application\".\n"
                        + "If you are using phantom power, the USB port might be unavailable until the phantom\n"
                        + "power has become stable. In such case, wait until you can hear that the microphone\n"
                        + "works, plus a few seconds more.");
                    problem.Title = "Waiting for VT-4";
                    problem.Commands.Add(new UICommand { Label = "Close VT-4 application", Id = 0 });
                    problem.Commands.Add(new UICommand { Label = "Retry", Id = 1 });
                    var response = await problem.ShowAsync();
                    if ((Int32)response.Id == 1)
                    {
                        await InitMidi();
                        Thread.Sleep(500);
                    }
                    else
                    {
                        Application.Current.Exit();
                    }
                }
                else if (type == 1)
                {
                    problem = new MessageDialog(
                        "Communication with your VT-4 has been lost.\nPlease verify connection and restart the app and the VT-4."
                        + "\nFirst turn off the app and the VT-4, then turn on the VT-4.\nWait at least 30 seconds before you start the app."
                        + "If you are using phantom power, the USB port might be unavailable until the phantom\n"
                        + "power has become stable. In such case, wait until you can hear that the microphone\n"
                        + "works, plus a few seconds more.");
                    problem.Title = "Waiting for VT-4";
                    problem.Commands.Add(new UICommand { Label = "Close VT-4 application", Id = 0 });
                    problem.Commands.Add(new UICommand { Label = "Retry", Id = 1 });
                    var response = await problem.ShowAsync();
                    pendingMidiRequest = PendingMidiRequest.DONE;
                    if ((Int32)response.Id == 1)
                    {
                        await InitMidi();
                    }
                    else
                    {
                        Application.Current.Exit();
                    }
                }
            }
        }

        public void SetKnobsAndSliders()
        {
            //// Volume
            //VT4.TemporaryPatch.GLOBAL_LEVEL = 0x80;
            //midi.SendControlChange(midi.PortPairs[0], 46, 0x80);
            //((Knob)knobVolume).Value = 0x80;
            //// Mic sens (not stored in VT4):
            //midi.SendControlChange(midi.PortPairs[0], 47, 0x80);
            //((Knob)knobMicSens).Value = 0x80;
            //// Key
            //VT4.TemporaryPatch.KEY = 0;
            //midi.SendControlChange(midi.PortPairs[0], 48, 0);
            //((Knob)knobKey).Value = 0;
            //// Auto pitch
            VT4.TemporaryPatch.AUTO_PITCH = 0;
            midi.SendControlChange(midi.PortPairs[0], 55, 0);
            ((Knob)knobAutoPitch).Value = 0;
            // Pitch
            VT4.TemporaryPatch.PITCH = 0x80;
            midi.SendPitchBender(midi.PortPairs[0], 0x40);
            ((VerticalSlider)slPitch).Value = 0x40;
            // Formant
            VT4.TemporaryPatch.FORMANT = 0x80;
            midi.SendControlChange(midi.PortPairs[0], 48, 0x40);
            ((VerticalSlider)slFormant).Value = 0x40;
            // Balance
            VT4.TemporaryPatch.BALANCE = 0xff;
            midi.SendControlChange(midi.PortPairs[0], 48, 0x7f);
            ((VerticalSlider)slBalance).Value = 0x7f;
            // Reverb
            VT4.TemporaryPatch.REVERB = 0x00;
            midi.SendControlChange(midi.PortPairs[0], 48, 0);
            ((VerticalSlider)slReverb).Value = 0;
        }

        public void SendVolume()
        {
            if (settings.LocalSettings.Values["Volume"] != null)
            {
                midi.SendControlChange(midi.PortPairs[0], (byte)26, (byte)settings.LocalSettings.Values["Volume"]);
            }
        }

        public void SendMicSens()
        { 
            if (settings.LocalSettings.Values["Mic sens"] != null)
            {
                midi.SendControlChange(midi.PortPairs[0], (byte)27, (byte)settings.LocalSettings.Values["Mic sens"]);
            }
        }

        private void CopyVT4ToPopupMenus()
        {
            foreach (List<List<PopupMenuButton>> vt4MenuItem in vt4MenuItems)
            {
                foreach (List<PopupMenuButton> vt4Menus in vt4MenuItem)
                {
                    foreach (PopupMenuButton vt4Menu in vt4Menus)
                    {
                        switch ((Area)vt4Menu.Id)
                        {
                            case Area.EQUALIZER_LOW_SHELF_FREQUENCY:
                                vt4Menu.Value = VT4.TemporaryEqualizer.EQUALIZER_LOW_SHELF_FREQUENCY;
                                break;
                            case Area.EQUALIZER_LOW_SHELF_GAIN:
                                vt4Menu.Value = VT4.TemporaryEqualizer.EQUALIZER_LOW_SHELF_GAIN;
                                break;
                            case Area.EQUALIZER_LOW_MID_FREQUENCY:
                                vt4Menu.Value = VT4.TemporaryEqualizer.EQUALIZER_LOW_MID_FREQUENCY;
                                break;
                            case Area.EQUALIZER_LOW_MID_GAIN:
                                vt4Menu.Value = VT4.TemporaryEqualizer.EQUALIZER_LOW_MID_GAIN;
                                break;
                            case Area.EQUALIZER_LOW_MID_Q:
                                vt4Menu.Value = VT4.TemporaryEqualizer.EQUALIZER_LOW_MID_Q;
                                break;
                            case Area.EQUALIZER_HIGH_SHELF_FREQUENCY:
                                vt4Menu.Value = VT4.TemporaryEqualizer.EQUALIZER_HIGH_SHELF_FREQUENCY;
                                break;
                            case Area.EQUALIZER_HIGH_SHELF_GAIN:
                                vt4Menu.Value = VT4.TemporaryEqualizer.EQUALIZER_HIGH_SHELF_GAIN;
                                break;
                            case Area.ROLAND:
                                Int32 firstIndex = vt4MenuItems.IndexOf(vt4MenuItem);
                                Int32 secondIndex = vt4MenuItem.IndexOf(vt4Menus);
                                Int32 thirdIndex = vt4Menus.IndexOf(vt4Menu);
                                switch (firstIndex)
                                {
                                    case 10:
                                        switch (secondIndex)
                                        {
                                            case 2:
                                                switch (thirdIndex)
                                                {
                                                    case 0:
                                                        vt4Menu.Value = VT4.System.GATE_LEVEL;
                                                        break;
                                                    case 1:
                                                        vt4Menu.Value = VT4.System.FORMANT_DEPTH;
                                                        break;
                                                    case 2:
                                                        vt4Menu.Value = VT4.System.LOW_CUT;
                                                        break;
                                                    case 3:
                                                        vt4Menu.Value = VT4.System.ENHANCER;
                                                        break;
                                                    case 4:
                                                        vt4Menu.Value = VT4.System.USB_MIXING;
                                                        break;
                                                }
                                                break;
                                            case 3:
                                                switch (thirdIndex)
                                                {
                                                    case 0:
                                                        vt4Menu.Value = VT4.System.MONITOR_MODE;
                                                        break;
                                                    case 1:
                                                        vt4Menu.Value = VT4.System.EXTERNAL_CARRIER;
                                                        break;
                                                    case 2:
                                                        vt4Menu.Value = VT4.System.MIDI_IN_MODE;
                                                        break;
                                                    case 3:
                                                        vt4Menu.Value = VT4.System.PITCH_AND_FORMANT_ROUTING;
                                                        break;
                                                    case 4:
                                                        vt4Menu.Value = VT4.System.MUTE_MODE;
                                                        break;
                                                }
                                                break;
                                            case 4:
                                                switch (thirdIndex)
                                                {
                                                    case 0:
                                                        vt4Menu.Value = VT4.UserEqualizer.EQUALIZER_SWITCH;
                                                        break;
                                                    case 1:
                                                        vt4Menu.Value = VT4.UserEqualizer.EQUALIZER_LOW_SHELF_FREQUENCY;
                                                        break;
                                                    case 2:
                                                        vt4Menu.Value = VT4.UserEqualizer.EQUALIZER_LOW_SHELF_GAIN;
                                                        break;
                                                    case 3:
                                                        vt4Menu.Value = VT4.UserEqualizer.EQUALIZER_LOW_MID_FREQUENCY;
                                                        break;
                                                    case 4:
                                                        vt4Menu.Value = VT4.UserEqualizer.EQUALIZER_LOW_MID_GAIN;
                                                        break;
                                                    case 5:
                                                        vt4Menu.Value = VT4.UserEqualizer.EQUALIZER_LOW_MID_Q;
                                                        break;
                                                    case 6:
                                                        vt4Menu.Value = VT4.UserEqualizer.EQUALIZER_HIGH_SHELF_FREQUENCY;
                                                        break;
                                                    case 7:
                                                        vt4Menu.Value = VT4.UserEqualizer.EQUALIZER_HIGH_SHELF_GAIN;
                                                        break;
                                                    case 8:
                                                        vt4Menu.Value = VT4.UserEqualizer.EQUALIZER_HIGH_MID_FREQUENCY;
                                                        break;
                                                    case 9:
                                                        vt4Menu.Value = VT4.UserEqualizer.EQUALIZER_HIGH_MID_GAIN;
                                                        break;
                                                    case 10:
                                                        vt4Menu.Value = VT4.UserEqualizer.EQUALIZER_HIGH_MID_Q;
                                                        break;
                                                }
                                                break;
                                        }
                                        break;
                                }
                                break;
                        }
                        switch ((VocoderArea)vt4Menu.Id)
                        {
                            case VocoderArea.VOCODER_PARAMETER_1:
                                vt4Menu.Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_1;
                                break;
                            case VocoderArea.VOCODER_PARAMETER_2:
                                vt4Menu.Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_2;
                                break;
                            case VocoderArea.VOCODER_PARAMETER_3:
                                vt4Menu.Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_3;
                                break;
                            case VocoderArea.VOCODER_PARAMETER_4:
                                vt4Menu.Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_4;
                                break;
                        }
                        switch ((HarmonyArea)vt4Menu.Id)
                        {
                            case HarmonyArea.HARMONY_1_GENDER:
                                vt4Menu.Value = VT4.TemporaryHarmony.HARMONY_1_GENDER;
                                break;
                            case HarmonyArea.HARMONY_2_GENDER:
                                vt4Menu.Value = VT4.TemporaryHarmony.HARMONY_2_GENDER;
                                break;
                            case HarmonyArea.HARMONY_3_GENDER:
                                vt4Menu.Value = VT4.TemporaryHarmony.HARMONY_3_GENDER;
                                break;
                            case HarmonyArea.HARMONY_1_KEY:
                                vt4Menu.Value = VT4.TemporaryHarmony.HARMONY_1_KEY;
                                break;
                            case HarmonyArea.HARMONY_2_KEY:
                                vt4Menu.Value = VT4.TemporaryHarmony.HARMONY_2_KEY;
                                break;
                            case HarmonyArea.HARMONY_3_KEY:
                                vt4Menu.Value = VT4.TemporaryHarmony.HARMONY_3_KEY;
                                break;
                            case HarmonyArea.HARMONY_1_LEVEL:
                                vt4Menu.Value = VT4.TemporaryHarmony.HARMONY_1_LEVEL;
                                break;
                            case HarmonyArea.HARMONY_2_LEVEL:
                                vt4Menu.Value = VT4.TemporaryHarmony.HARMONY_2_LEVEL;
                                break;
                            case HarmonyArea.HARMONY_3_LEVEL:
                                vt4Menu.Value = VT4.TemporaryHarmony.HARMONY_3_LEVEL;
                                break;
                        }
                        switch ((RobotArea)vt4Menu.Id)
                        {
                            case RobotArea.ROBOT_PARAMETER_1:
                                vt4Menu.Value = VT4.TemporaryRobot.OCTAVE;
                                break;
                            case RobotArea.ROBOT_PARAMETER_2:
                                vt4Menu.Value = VT4.TemporaryRobot.FEEDBACK_SWITCH;
                                break;
                            case RobotArea.ROBOT_PARAMETER_3:
                                vt4Menu.Value = VT4.TemporaryRobot.FEEDBACK_RESONANCE;
                                break;
                            case RobotArea.ROBOT_PARAMETER_4:
                                vt4Menu.Value = VT4.TemporaryRobot.FEEDBACK_LEVEL;
                                break;
                        }
                        switch ((MegaphoneArea)vt4Menu.Id)
                        {
                            case MegaphoneArea.MEGAPHONE_PARAMETER_1:
                                vt4Menu.Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_1;
                                break;
                            case MegaphoneArea.MEGAPHONE_PARAMETER_2:
                                vt4Menu.Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_2;
                                break;
                            case MegaphoneArea.MEGAPHONE_PARAMETER_3:
                                vt4Menu.Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_3;
                                break;
                            case MegaphoneArea.MEGAPHONE_PARAMETER_4:
                                vt4Menu.Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_4;
                                break;
                        }
                        switch ((ReverbArea)vt4Menu.Id)
                        {
                            case ReverbArea.REVERB_PARAMETER_1:
                                vt4Menu.Value = VT4.TemporaryReverb.REVERB_PARAMETER_1;
                                break;
                            case ReverbArea.REVERB_PARAMETER_2:
                                vt4Menu.Value = VT4.TemporaryReverb.REVERB_PARAMETER_2;
                                break;
                            case ReverbArea.REVERB_PARAMETER_3:
                                vt4Menu.Value = VT4.TemporaryReverb.REVERB_PARAMETER_3;
                                break;
                            case ReverbArea.REVERB_PARAMETER_4:
                                vt4Menu.Value = VT4.TemporaryReverb.REVERB_PARAMETER_4;
                                break;
                        }
                    }
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // Handlers
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        private async void MidiWatchTimer_Tick(object sender, object e)
        {
            MidiWatchTimer.Stop();
            try
            {
                foreach (PortPair portPair in midi.PortPairs)
                {
                    if (!portPair.IsConnected)
                    {
                        await DisplayConnectionProblem(1);
                    }
                    byte[] address = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x01 };
                    byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    await WaitForMidiRequestAnswered();
                }
            }
            catch { }
            MidiWatchTimer.Start();
        }

        private void BlinkTimer_Tick(object sender, object e)
        {
            fast = !fast;
            if (fast)
            {
                slow = !slow;
            }
            if (!manualOn)
            {
                if (blinking)
                {
                    pmbManual.Set(slow);
                }
                else
                {
                    imgManualOn.Visibility = Visibility.Collapsed;
                }
            }
            if (variantCheck > -1)
            {
                pmb1.Set(true);
                pmb2.Set(true);
                pmb3.Set(true);
                pmb4.Set(true);
                pmb5.Set(true);
                pmb6.Set(true);
                pmb7.Set(true);
                pmb8.Set(true);
                switch (variantCheck)
                {
                    case 0: pmb1.Set(fast); break;
                    case 1: pmb2.Set(fast); break;
                    case 2: pmb3.Set(fast); break;
                    case 3: pmb4.Set(fast); break;
                    case 4: pmb5.Set(fast); break;
                    case 5: pmb6.Set(fast); break;
                    case 6: pmb7.Set(fast); break;
                    case 7: pmb8.Set(fast); break;
                }
            }
            else if (sceneIndex > -1 && sceneEdited[sceneIndex])
            {
                ((PopupMenuButton)Controls.ControlsList[topLevelMenuLocations[6 + (sceneIndex + 12) % 8]]).Set(slow);
            }
        }

        private void HandsOffTimer_Tick(object sender, object e)
        {
            HandsOffTimer.Stop();
            handleControlEvents = true;
        }

        private async void EventTimer_Tick(object sender, object e)
        {
            EventTimer.Stop();
            if (ActualHeight > 0 && ActualWidth > 0)
            {
                try
                {
                    while (timerAction.Count > 0)
                    {
                        // Skip intermidate messages of the same type:
                        while (timerAction.Count > 1
                            && (timerAction[0].EventData == null
                                || (timerAction[0].EventData != null && timerAction[1].EventData != null
                                    && timerAction[0].EventData[0] == timerAction[1].EventData[1])))
                        {
                            timerAction.RemoveAt(0);
                        }
                        switch (timerAction[0].EventType)
                        {
                            case TimerAction.WAIT_FOR_INITIALIZATION_DONE:
                                CopyVT4ToPopupMenus();
                                UpdateGui();
                                initDone = true;
                                break;
                            case TimerAction.HANDLE_EFFECT_BUTTON_PRESS:
                                ReleaseEffectButtonTimer.Start();
                                break;
                            case TimerAction.HANDLE_CC_MESSAGE:
                                HandleCcMessage(timerAction[0].EventData);
                                break;
                            case TimerAction.HANDLE_PB_MESSAGE:
                                HandlePbMessage();
                                break;
                            case TimerAction.HANDLE_ROBOT_VARIATION_CHANGE:
                                SetVariation(timerAction[0].EventData[1] - 1, Area.ROBOT);
                                break;
                            case TimerAction.HANDLE_MEGAPHONE_VARIATION_CHANGE:
                                SetVariation(timerAction[0].EventData[1] - 1, Area.MEGAPHONE);
                                break;
                            case TimerAction.HANDLE_VOCODER_VARIATION_CHANGE:
                                SetVariation(timerAction[0].EventData[1] - 1, Area.VOCODER);
                                break;
                            case TimerAction.HANDLE_HARMONY_VARIATION_CHANGE:
                                SetVariation(timerAction[0].EventData[1] - 1, Area.HARMONY);
                                break;
                            case TimerAction.HANDLE_REVERB_VARIATION_CHANGE:
                                SetVariation(timerAction[0].EventData[1] - 1, Area.REVERB);
                                break;
                            //switch (ChangingVariationFor)
                            //{
                            //    case 0x31:
                            //        SetVariation(ChangingVariationNumber, Area.ROBOT);
                            //        break;
                            //    case 0x32:
                            //        SetVariation(ChangingVariationNumber, Area.MEGAPHONE);
                            //        break;
                            //    case 0x33:
                            //        SetVariation(ChangingVariationNumber, Area.REVERB);
                            //        break;
                            //    case 0x34:
                            //        SetVariation(ChangingVariationNumber, Area.VOCODER);
                            //        break;
                            //    case 0x35:
                            //        SetVariation(ChangingVariationNumber, Area.HARMONY);
                            //        break;
                            //}
                            //ChangingVariationFor = -1;
                            //break;
                            case TimerAction.SELECT_TEMPORARY_PATCH:
                                sceneIndex = -1;
                                currentArea = Area.MANUAL;
                                //CurrentPatch = VT4.TemporaryPatch;
                                blinking = false;
                                UpdateGui();
                                manualOn = true;
                                manualBlinkCounter = 40;
                                ReleaseManualButtonTimer.Start();
                                break;
                            case TimerAction.SELECT_PATCH_1:
                                sceneIndex = timerAction[0].EventData[0];
                                pmb1.Set(sceneIndex == 0);
                                pmb2.Set(sceneIndex == 1);
                                pmb3.Set(sceneIndex == 2);
                                pmb4.Set(sceneIndex == 3);
                                pmb5.Set(sceneIndex == 4);
                                pmb6.Set(sceneIndex == 5);
                                pmb7.Set(sceneIndex == 6);
                                pmb8.Set(sceneIndex == 7);
                                if (sceneIndex > 3)
                                {
                                    blinking = true;
                                }
                                else
                                {
                                    blinking = false;
                                }
                                if (sceneIndex > -1)
                                {
                                    VT4.TemporaryPatch = new Patch(VT4.UserPatch[sceneIndex]);
                                }

                                // Turn off scene button changed blinking:
                                if (sceneIndex > -1)
                                {
                                    sceneEdited[sceneIndex] = false;
                                }
                                UpdateGui();
                                break;
                            case TimerAction.NO_MIDI_RESPONSE:
                                await DisplayConnectionProblem(1);
                                break;
                            case TimerAction.TURN_ON_ROBOT:
                                pmbRobot.Set(true);
                                VT4.TemporaryPatch.ROBOT = 2; // 0 = off, 1 = on, 2 = on and MIDI in
                                break;
                        }
                        timerAction.RemoveAt(0);
                    }
                }
                catch
                {
                    EventTimer.Start();
                }
            }
            //coordinates.Text = timerAction.Count.ToString();
            EventTimer.Start();
        }

        private void ReleaseManualButtonTimer_Tick(object sender, object e)
        {
            if (manualBlinkCounter > 0)
            {
                switch (15 - manualBlinkCounter % 16)
                {
                    case 0:
                        imgOneOn.Visibility = Visibility.Visible;
                        break;
                    case 1:
                        imgTwoOn.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        imgThreeOn.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        imgFourOn.Visibility = Visibility.Visible;
                        break;
                    case 4:
                        imgFiveOn.Visibility = Visibility.Visible;
                        break;
                    case 5:
                        imgSixOn.Visibility = Visibility.Visible;
                        break;
                    case 6:
                        imgSevenOn.Visibility = Visibility.Visible;
                        break;
                    case 7:
                        imgEightOn.Visibility = Visibility.Visible;
                        break;
                    case 8:
                        imgOneOn.Visibility = Visibility.Collapsed;
                        break;
                    case 9:
                        imgTwoOn.Visibility = Visibility.Collapsed;
                        break;
                    case 10:
                        imgThreeOn.Visibility = Visibility.Collapsed;
                        break;
                    case 11:
                        imgFourOn.Visibility = Visibility.Collapsed;
                        break;
                    case 12:
                        imgFiveOn.Visibility = Visibility.Collapsed;
                        break;
                    case 13:
                        imgSixOn.Visibility = Visibility.Collapsed;
                        break;
                    case 14:
                        imgSevenOn.Visibility = Visibility.Collapsed;
                        break;
                    case 15:
                        imgEightOn.Visibility = Visibility.Collapsed;
                        break;
                }
                manualBlinkCounter--;
            }
            else
            {
                ReleaseManualButtonTimer.Stop();
                manualOn = true;
                manualButtonIsDown = false;
                imgOneOn.Visibility = Visibility.Collapsed;
                imgTwoOn.Visibility = Visibility.Collapsed;
                imgThreeOn.Visibility = Visibility.Collapsed;
                imgFourOn.Visibility = Visibility.Collapsed;
                imgFiveOn.Visibility = Visibility.Collapsed;
                imgSixOn.Visibility = Visibility.Collapsed;
                imgSevenOn.Visibility = Visibility.Collapsed;
                imgEightOn.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// When the user presses an effect button (Robot, Megaphone, Vocoder or Harmony)
        /// the GUI needs to know if the user wants to turn the effect on/off or keeps
        /// the button down more than 500 ms in order to see what variation is in effect.
        /// The application must know how to react when the key is released, is it released
        /// before or after the 500 ms? If before, we have an on/off event else a check
        /// variation event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReleaseEffectButtonTimer_Tick(object sender, object e)
        {
            ReleaseEffectButtonTimer.Stop();

            if (measureRobotButtonDownTime)
            {
                if (robotButtonIsDown)
                {
                    // Button was not released and  is on, set fast blinking on variant:
                    variantCheck = VT4.TemporaryPatch.ROBOT_VARIATION;
                    if (VT4.TemporaryPatch.ROBOT == 0)
                    {
                        // Button was not released and  is off, turn it on:
                        VT4.TemporaryPatch.ROBOT = 0x01;
                        pmbRobot.Set(true);
                    }
                }
                else
                {
                    // Perform on/off action
                    VT4.TemporaryPatch.ROBOT = (byte)(VT4.TemporaryPatch.ROBOT > 0 ? 0 : 1);
                    pmbRobot.Set(VT4.TemporaryPatch.ROBOT > 0);
                }
                measureRobotButtonDownTime = false;
            }
            else if (measureMegaphoneButtonDownTime)
            {
                if (megaphoneButtonIsDown)
                {
                    // Button was not released and  is on, set fast blinking on variant:
                    variantCheck = VT4.TemporaryPatch.MEGAPHONE_VARIATION;
                    if (VT4.TemporaryPatch.MEGAPHONE == 0)
                    {
                        // Button was not released and  is off, turn it on:
                        VT4.TemporaryPatch.MEGAPHONE = 0x01;
                        pmbMegaphone.Set(true);
                    }
                }
                else
                {
                    // Perform on/off action
                    VT4.TemporaryPatch.MEGAPHONE = (byte)(VT4.TemporaryPatch.MEGAPHONE > 0 ? 0 : 1);
                    pmbMegaphone.Set(VT4.TemporaryPatch.MEGAPHONE > 0);
                }
                measureMegaphoneButtonDownTime = false;
            }
            else if (measureVocoderButtonDownTime)
            {
                if (vocoderButtonIsDown)
                {
                    // Button was not released and  is on, set fast blinking on variant:
                    variantCheck = VT4.TemporaryPatch.VOCODER_VARIATION;
                    if (VT4.TemporaryPatch.VOCODER == 0)
                    {
                        // Button was not released and  is off, turn it on:
                        VT4.TemporaryPatch.VOCODER = 0x01;
                        pmbVocoder.Set(true);
                    }
                }
                else
                {
                    // Perform on/off action
                    VT4.TemporaryPatch.VOCODER = (byte)(VT4.TemporaryPatch.VOCODER > 0 ? 0 : 1);
                    pmbVocoder.Set(VT4.TemporaryPatch.VOCODER > 0);
                }
                measureVocoderButtonDownTime = false;
            }
            else if (measureHarmonyButtonDownTime)
            {
                if (harmonyButtonIsDown)
                {
                    // Button was not released and  is on, set fast blinking on variant:
                    variantCheck = VT4.TemporaryPatch.HARMONY_VARIATION;
                    if (VT4.TemporaryPatch.HARMONY == 0)
                    {
                        // Button was not released and  is off, turn it on:
                        VT4.TemporaryPatch.HARMONY = 0x01;
                        pmbHarmony.Set(true);
                    }
                }
                else
                {
                    // Perform on/off action
                    VT4.TemporaryPatch.HARMONY = (byte)(VT4.TemporaryPatch.HARMONY > 0 ? 0 : 1);
                    pmbHarmony.Set(VT4.TemporaryPatch.HARMONY > 0);
                }
                measureHarmonyButtonDownTime = false;
            }
            else if (measureReverbButtonDownTime)
            {
                if (reverbButtonIsDown)
                {
                    // Reverb variant check is performed by pressing the bypass button.
                    // Button was not released and  is on, set fast blinking on variant:
                    variantCheck = VT4.TemporaryPatch.REVERB_VARIATION;
                    measureReverbButtonDownTime = false;
                    if (VT4.TemporaryPatch.REVERB == 0)
                    {
                        // Button was not released and  is off, turn it on:
                        //VT4.TemporaryPatch.REVERB = 0x01;
                        //pmbReverb.Set(true);
                    }
                }
                else
                {
                    // Perform on/off action. Note that this will be the bypass!
                    btnBypass.IsOn = !btnBypass.IsOn;
                }
            }
            else
            {
                // Button was released, clear the blinking:
                pmb1.Set(false);
                pmb2.Set(false);
                pmb3.Set(false);
                pmb4.Set(false);
                pmb5.Set(false);
                pmb6.Set(false);
                pmb7.Set(false);
                pmb8.Set(false);
                variantCheck = -1;
            }
        }

        private void SendSysEx(Area area, byte value)
        {
            if (initDone)
            {
                byte[] address = null;
                byte[] data = null;
                byte[] message = null;

                switch (area)
                {
                    case Area.AUTO_PITCH:
                        address = new byte[] { 0x10, 0x00, 0x00, 0x11 };
                        data = new byte[] { (byte)(value / 16), (byte)(value % 16) };
                        break;
                    case Area.KEY:
                        address = new byte[] { 0x10, 0x00, 0x00, 0x13 };
                        data = new byte[] { (byte)value };
                        break;
                }
                message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data);
                midi.SendSystemExclusive(midi.PortPairs[0], message);
            }
        }

        private void SendControlChange(Area area, byte value)
        {
            if (initDone)
            {
                switch (area)
                {
                    case Area.VOLUME:
                        midi.SendControlChange(midi.PortPairs[0], 46, value);
                        break;
                    case Area.MIC_SENS:
                        midi.SendControlChange(midi.PortPairs[0], 47, value);
                        break;
                    case Area.BYPASS:
                        midi.SendControlChange(midi.PortPairs[0], 51, value);
                        break;
                    case Area.PITCH:
                        midi.SendPitchBender(midi.PortPairs[0], value * 128);
                        break;
                    case Area.FORMANT:
                        midi.SendControlChange(midi.PortPairs[0], 54, value);
                        break;
                    case Area.BALANCE:
                        midi.SendControlChange(midi.PortPairs[0], 56, value);
                        break;
                    case Area.REVERB_SLIDER:
                        midi.SendControlChange(midi.PortPairs[0], 57, value);
                        break;
                    case Area.ROBOT:
                        midi.SendControlChange(midi.PortPairs[0], 49, value);
                        break;
                }
            }
        }

        //private void SendPitchBender(Area area, byte value)
        //{
        //    if (initDone)
        //    {
        //        midi.SendPitchBender(midi.PortPairs[0], 0, value);
        //    }
        //}

        private void SendProgramChange(byte value)
        {
            if (midi.PortPairs[0].IsConnected)
            {
                midi.SendProgramChange(midi.PortPairs[0], value);
            }
        }

        private void SendSystemExclusive(Area area, Int32 value)
        {
            List<byte> data = new List<byte>();
            data.Add((byte)(value % 128));
            value -= value % 128;
            while (value > 0)
            {
                data.Add((byte)(value % 128));
                value -= value % 128;
            }

            if (initDone)
            {
                byte[] address;
                byte[] message;

                switch (area)
                {
                    case Area.KEY:
                        address = new byte[] { 0x10, 0x00, 0x00, 0x13 };
                        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                        midi.SendSystemExclusive(midi.PortPairs[0], message);
                        break;
                    case Area.ROBOT:
                        address = new byte[] { 0x10, 0x00, 0x00, 0x00 };
                        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                        midi.SendSystemExclusive(midi.PortPairs[0], message);
                        break;
                    case Area.MEGAPHONE:
                        address = new byte[] { 0x10, 0x00, 0x00, 0x03 };
                        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                        midi.SendSystemExclusive(midi.PortPairs[0], message);
                        break;
                    case Area.VOCODER:
                        address = new byte[] { 0x10, 0x0, 0x0, 0x2 };
                        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                        midi.SendSystemExclusive(midi.PortPairs[0], message);
                        break;

                    case Area.HARMONY:
                        address = new byte[] { 0x10, 0x0, 0x0, 0x1 };
                        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                        midi.SendSystemExclusive(midi.PortPairs[0], message);
                        break;
                }
            }
        }

        private void SetVariationNames()
        {
            for (int i = 0; i < 8; i++)
            {
                pmbVocoder.Children[0][i].Set(false);
                pmbHarmony.Children[0][i].Set(false);
                pmbRobot.Children[0][i].Set(false);
                pmbMegaphone.Children[0][i].Set(false);
                pmbReverb.Children[0][i].Set(false);
            }
            lblPatch.TextBlock.Text = VT4.TemporaryPatch.Name;
            lblVocoder.TextBlock.Text = VT4.UserVocoder[VT4.TemporaryPatch.VOCODER_VARIATION].Name;
            lblHarmony.TextBlock.Text = VT4.UserHarmony[VT4.TemporaryPatch.HARMONY_VARIATION].Name;
            lblRobot.TextBlock.Text = VT4.UserRobot[VT4.TemporaryPatch.ROBOT_VARIATION].Name;
            lblMegaphone.TextBlock.Text = VT4.UserMegaphone[VT4.TemporaryPatch.MEGAPHONE_VARIATION].Name;
            lblReverb.TextBlock.Text = VT4.UserReverb[VT4.TemporaryPatch.REVERB_VARIATION].Name;

            pmbVocoder.Children[0][VT4.TemporaryPatch.VOCODER_VARIATION].Set(true);
            pmbHarmony.Children[0][VT4.TemporaryPatch.HARMONY_VARIATION].Set(true);
            pmbRobot.Children[0][VT4.TemporaryPatch.ROBOT_VARIATION].Set(true);
            pmbMegaphone.Children[0][VT4.TemporaryPatch.MEGAPHONE_VARIATION].Set(true);
            pmbReverb.Children[0][VT4.TemporaryPatch.REVERB_VARIATION].Set(true);

            for (int i = 0; i < pmbVocoder.Children[0].Count; i++)
            {
                pmbVocoder.Children[0][i].TextBlock.Text = VT4.UserVocoder[i].Name;
            }

            for (int i = 0; i < pmbHarmony.Children[0].Count; i++)
            {
                pmbHarmony.Children[0][i].TextBlock.Text = VT4.UserHarmony[i].Name;
            }

            for (int i = 0; i < pmbRobot.Children[0].Count; i++)
            {
                pmbRobot.Children[0][i].TextBlock.Text = VT4.UserRobot[i].Name;
            }

            for (int i = 0; i < pmbMegaphone.Children[0].Count; i++)
            {
                pmbMegaphone.Children[0][i].TextBlock.Text = VT4.UserMegaphone[i].Name;
            }

            for (int i = 0; i < pmbReverb.Children[0].Count; i++)
            {
                pmbReverb.Children[0][i].TextBlock.Text = VT4.UserReverb[i].Name;
            }
        }
    }
}
