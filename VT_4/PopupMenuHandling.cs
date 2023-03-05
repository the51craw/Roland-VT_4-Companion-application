using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using UwpControlsLibrary;

namespace VT_4
{
    public sealed partial class MainPage : Page
    {
        Int32 CopyFrom;
        Int32 Variation;
        PopupMenuButton currentPopupMenuButton = null;
        PopupMenuButton[] currentPopupMenu = null;

        // Menu items:
        public List<List<List<PopupMenuButton>>> vt4MenuItems; // First dimension: Button that is associated with the menu items, event is 'Tapped'.
                                                               // Second dimension: 0 = 'RightTapped' meny, 1 = 'DoubleTapped', Variation menu.
                                                               // Third dimension: Lines in popup menu, 0 = first line 1 is second line etc.
        public int currentMenuId = -1;
        public int currentSubMenuId = -1;

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// Helpers
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        public async Task ShowManual()
        {
            try
            {
                Windows.System.LauncherOptions options = new Windows.System.LauncherOptions();
                options.ContentType = "application/pdf";
                StorageFile manual = await StorageFile.GetFileFromApplicationUriAsync
                    (new Uri(@"ms-appx:///Documents/Roland_VT-4_Companion_Application.pdf", UriKind.Absolute));
                await Windows.System.Launcher.LaunchFileAsync(manual);
            }
            catch (Exception e)
            {
                ContentDialog error = new Error("It seems like you do not have Acrobate Reader installed.\n"
                    + "You need Acrobat reader to read the manual.\nGo to adobe.com to install it.");
                await error.ShowAsync();
            }
        }

        public void SetSystemParameterTexts()
        {
            try
            {
                // Just don't ask! I get either 0 - 19 or 0 - 21...
                // The rigth limit is a little too much to the left. It is for the Robot right margin and
                // can therefore not be changed.
                Int32 value = vt4MenuItems[10][2][4].Value;
                value = value > 20 ? 20 : value;
                vt4MenuItems[10][2][4].Value = value;

                vt4MenuItems[10][2][0].TextBlock.Text = "Gate level " + (vt4MenuItems[10][2][0].Value + 1).ToString();
                vt4MenuItems[10][2][1].TextBlock.Text = "Formant dept " + (vt4MenuItems[10][2][1].Value + 1).ToString();
                vt4MenuItems[10][2][2].TextBlock.Text = "Low cut " + (vt4MenuItems[10][2][2].Value + 1).ToString();
                vt4MenuItems[10][2][3].TextBlock.Text = "Enhancer " + (vt4MenuItems[10][2][3].Value + 1).ToString();
                vt4MenuItems[10][2][4].TextBlock.Text = "Usb Mixing " + vt4MenuItems[10][2][4].Value.ToString();
                vt4MenuItems[10][3][0].TextBlock.Text = "Monitor mode " + (vt4MenuItems[10][3][0].Value == 0 ? "off" : "on");
                vt4MenuItems[10][3][1].TextBlock.Text = "External carrier " + (vt4MenuItems[10][3][1].Value == 0 ? "off" : "on");
                vt4MenuItems[10][3][2].TextBlock.Text = "Midi in " + (vt4MenuItems[10][3][2].Value == 0 ? "off" : "on");
                vt4MenuItems[10][3][3].TextBlock.Text = "Pitch/Form rout. " + (vt4MenuItems[10][3][3].Value == 0 ? "off" : "on");
                vt4MenuItems[10][3][4].TextBlock.Text = "Mute mode " + (vt4MenuItems[10][3][4].Value == 0 ? "off" : "on");
                vt4MenuItems[10][4][0].TextBlock.Text = "Equalizer " + (vt4MenuItems[10][4][0].Value == 0 ? "off" : "on");
            }
            catch { }
        }

        public void SetParameterTexts(int id)
        {
            switch (id)
            {
                case 0x08:
                    SetVocoderParameterTexts();
                    break;
                case 0x09:
                    SetHarmonyParameterTexts();
                    break;
                case 0x0a:
                    SetRobotParameterTexts();
                    break;
                case 0x0b:
                    SetMegaphoneParameterTexts();
                    break;
                case 0x0c:
                    SetReverbParameterTexts();
                    break;
            }
        }

        public void SetRobotParameterTexts()
        {
            try
            {
                pmbRobot.Children[1][0].TextBlock.Text = "Octave " + (pmbRobot.Children[1][0].Value - 2).ToString();
                if (VT4.TemporaryPatch.ROBOT_VARIATION > 2)
                {
                    pmbRobot.Children[1][1].TextBlock.Text = "Feedback Switch " + (pmbRobot.Children[1][1].Value == 0 ? "Off" : "On");
                }
                else
                {
                    pmbRobot.Children[1][1].TextBlock.Text = "Feedback is unavailable";
                }
                pmbRobot.Children[1][2].TextBlock.Text = "FB Resonance " + pmbRobot.Children[1][2].Value.ToString();
                pmbRobot.Children[1][3].TextBlock.Text = "FB Level " + pmbRobot.Children[1][3].Value.ToString();
            }
            catch { }
        }

        public void SetMegaphoneParameterTexts()
        {
            string[] type = new string[] { "Megaphon", "Radio", "BbdChors", "Strobo" };
            try
            {
                switch (pmbMegaphone.Children[1][0].Value)
                {
                    case 0:
                        pmbMegaphone.Children[1][0].TextBlock.Text = type[pmbMegaphone.Children[1][0].Value];
                        pmbMegaphone.Children[1][1].TextBlock.Text = "Clip Gain " + pmbMegaphone.Children[1][1].Value.ToString();
                        pmbMegaphone.Children[1][2].TextBlock.Text = "Direct Level " + pmbMegaphone.Children[1][2].Value.ToString();
                        pmbMegaphone.Children[1][3].TextBlock.Text = "Volume " + pmbMegaphone.Children[1][3].Value.ToString();
                        pmbMegaphone.Children[1][4].TextBlock.Text = "" + pmbMegaphone.Children[1][4].Value.ToString();
                        pmbMegaphone.Children[1][4].Set(false);
                        break;
                    case 1:
                        pmbMegaphone.Children[1][0].TextBlock.Text = type[pmbMegaphone.Children[1][0].Value];
                        pmbMegaphone.Children[1][1].TextBlock.Text = "Drive " + pmbMegaphone.Children[1][1].Value.ToString();
                        pmbMegaphone.Children[1][2].TextBlock.Text = "Sampling Rate " + pmbMegaphone.Children[1][2].Value.ToString();
                        pmbMegaphone.Children[1][3].TextBlock.Text = "Low Cut " + pmbMegaphone.Children[1][3].Value.ToString();
                        pmbMegaphone.Children[1][4].TextBlock.Text = "High Cut " + pmbMegaphone.Children[1][4].Value.ToString();
                        pmbMegaphone.Children[1][4].Set(true);
                        break;
                    case 2:
                        pmbMegaphone.Children[1][0].TextBlock.Text = type[pmbMegaphone.Children[1][0].Value];
                        pmbMegaphone.Children[1][1].TextBlock.Text = "Mode " + pmbMegaphone.Children[1][1].Value.ToString();
                        pmbMegaphone.Children[1][2].TextBlock.Text = "Depth " + pmbMegaphone.Children[1][2].Value.ToString();
                        pmbMegaphone.Children[1][3].TextBlock.Text = "Effect Level " + pmbMegaphone.Children[1][3].Value.ToString();
                        pmbMegaphone.Children[1][4].TextBlock.Text = "Noice Level " + pmbMegaphone.Children[1][4].Value.ToString();
                        pmbMegaphone.Children[1][4].Set(true);
                        break;
                    case 3:
                        pmbMegaphone.Children[1][0].TextBlock.Text = type[pmbMegaphone.Children[1][0].Value];
                        pmbMegaphone.Children[1][1].TextBlock.Text = "Wave Shape " + pmbMegaphone.Children[1][1].Value.ToString();
                        pmbMegaphone.Children[1][2].TextBlock.Text = "Rate " + pmbMegaphone.Children[1][2].Value.ToString();
                        pmbMegaphone.Children[1][3].TextBlock.Text = "Depth " + pmbMegaphone.Children[1][3].Value.ToString();
                        pmbMegaphone.Children[1][4].TextBlock.Text = "Level " + pmbMegaphone.Children[1][4].Value.ToString();
                        pmbMegaphone.Children[1][4].Set(true);
                        break;
                }
            }
            catch { }
        }

        public void SetReverbParameterTexts()
        {
            string[] type = new string[] { "Reverb", "Echo", "Delay", "Dub echo", "Deep reverb", "VT reverb" };
            try
            {
                switch (pmbReverb.Children[1][0].Value)
                {
                    case 0:
                    case 1:
                    case 3:
                    case 4:
                    case 5:
                        pmbReverb.Children[1][0].TextBlock.Text = type[pmbReverb.Children[1][0].Value];                        
                        pmbReverb.Children[1][1].TextBlock.Text = "Feedback " + pmbReverb.Children[1][1].Value.ToString();
                        pmbReverb.Children[1][2].TextBlock.Text = "Pre Delay " + pmbReverb.Children[1][2].Value.ToString();
                        pmbReverb.Children[1][3].TextBlock.Text = "Low Cut " + pmbReverb.Children[1][3].Value.ToString();
                        pmbReverb.Children[1][4].TextBlock.Text = "High Cut " + pmbReverb.Children[1][4].Value.ToString();
                        break;
                    case 2:
                        pmbReverb.Children[1][0].TextBlock.Text = type[pmbReverb.Children[1][0].Value];
                        pmbReverb.Children[1][1].TextBlock.Text = "Mode " + (pmbReverb.Children[1][1].Value).ToString();
                        pmbReverb.Children[1][2].TextBlock.Text = "Sync Note " + pmbReverb.Children[1][2].Value.ToString();
                        pmbReverb.Children[1][3].TextBlock.Text = "Low Cut " + pmbReverb.Children[1][3].Value.ToString();
                        pmbReverb.Children[1][4].TextBlock.Text = "High Cut " + pmbReverb.Children[1][4].Value.ToString();
                        break;
                }
            }
            catch { }
        }

        public void SetVocoderParameterTexts()
        {
            string[] type = new string[] { "Vintage", "Advanced", "Talk box", "Speel toy" };
            try
            {
                switch (pmbVocoder.Children[1][0].Value)
                {
                    case 0:
                        pmbVocoder.Children[1][0].TextBlock.Text = type[pmbVocoder.Children[1][0].Value];
                        pmbVocoder.Children[1][1].TextBlock.Text = "Release " + pmbVocoder.Children[1][1].Value.ToString();
                        pmbVocoder.Children[1][2].TextBlock.Text = "Tone " + pmbVocoder.Children[1][2].Value.ToString();
                        pmbVocoder.Children[1][3].TextBlock.Text = "Consonant " + pmbVocoder.Children[1][3].Value.ToString();
                        pmbVocoder.Children[1][4].TextBlock.Text = "Effect Level " + pmbVocoder.Children[1][4].Value.ToString();
                        break;
                    case 1:
                    case 3:
                        pmbVocoder.Children[1][0].TextBlock.Text = type[pmbVocoder.Children[1][0].Value];
                        pmbVocoder.Children[1][1].TextBlock.Text = "Release " + pmbVocoder.Children[1][1].Value.ToString();
                        pmbVocoder.Children[1][2].TextBlock.Text = "Tone " + pmbVocoder.Children[1][2].Value.ToString();
                        pmbVocoder.Children[1][3].TextBlock.Text = "OSC Color " + pmbVocoder.Children[1][3].Value.ToString();
                        pmbVocoder.Children[1][4].TextBlock.Text = "Effect Level " + pmbVocoder.Children[1][4].Value.ToString();
                        break;
                    case 2:
                        pmbVocoder.Children[1][0].TextBlock.Text = type[pmbVocoder.Children[1][0].Value];
                        pmbVocoder.Children[1][1].TextBlock.Text = "Release " + pmbVocoder.Children[1][1].Value.ToString();
                        pmbVocoder.Children[1][2].TextBlock.Text = "Formant Depth " + pmbVocoder.Children[1][2].Value.ToString();
                        pmbVocoder.Children[1][3].TextBlock.Text = "OSC Color " + pmbVocoder.Children[1][3].Value.ToString();
                        pmbVocoder.Children[1][4].TextBlock.Text = "Effect Level " + pmbVocoder.Children[1][4].Value.ToString();
                        break;
                }
            }
            catch { }
        }

        public void SetHarmonyParameterTexts()
        {
            try
            {
                pmbHarmony.Children[1][0].TextBlock.Text = "Level 1: " + pmbHarmony.Children[1][0].Value.ToString();
                pmbHarmony.Children[1][1].TextBlock.Text = "Level 2: " + pmbHarmony.Children[1][1].Value.ToString();
                pmbHarmony.Children[1][2].TextBlock.Text = "Level 3: " + pmbHarmony.Children[1][2].Value.ToString();
                pmbHarmony.Children[1][3].TextBlock.Text = "Key 1: " + IndexToKey(VT4.TemporaryHarmony.HARMONY_1_KEY);
                pmbHarmony.Children[1][4].TextBlock.Text = "Key 2: " + IndexToKey(VT4.TemporaryHarmony.HARMONY_2_KEY);
                pmbHarmony.Children[1][5].TextBlock.Text = "Key 3: " + IndexToKey(VT4.TemporaryHarmony.HARMONY_3_KEY);
                pmbHarmony.Children[1][6].TextBlock.Text = "Gender 1: " + pmbHarmony.Children[1][6].Value.ToString();
                pmbHarmony.Children[1][7].TextBlock.Text = "Gender 2: " + pmbHarmony.Children[1][7].Value.ToString();
                pmbHarmony.Children[1][8].TextBlock.Text = "Gender 3: " + vt4MenuItems[1][1][8].Value.ToString();
            }
            catch { }
        }

        public void SetEqualizerParameterTexts()
        {
            try
            {
                //pmbEqualizerSettings[0].TextBlock.Text = "Equalizer is "   + (pmbEqualizerSettings[0].Value > 0 ? "on" : "off");
                pmbEqualizerSettings[1].TextBlock.Text = "Low shelf freq "   + MakeFrequencyString(pmbEqualizerSettings[1].Value);
                pmbEqualizerSettings[2].TextBlock.Text = "Low shelf gain "   + MakeGainString(pmbEqualizerSettings[2].Value);
                pmbEqualizerSettings[3].TextBlock.Text = "Low mid freq "     + MakeFrequencyString(pmbEqualizerSettings[3].Value);
                pmbEqualizerSettings[4].TextBlock.Text = "Low mid Q "        + MakeQString(pmbEqualizerSettings[4].Value);
                pmbEqualizerSettings[5].TextBlock.Text = "Low mid gain "     + MakeGainString(pmbEqualizerSettings[5].Value);
                pmbEqualizerSettings[6].TextBlock.Text = "High mid freq "    + MakeFrequencyString(pmbEqualizerSettings[6].Value);
                pmbEqualizerSettings[7].TextBlock.Text = "High mid Q "       + MakeQString(pmbEqualizerSettings[7].Value);
                pmbEqualizerSettings[8].TextBlock.Text = "High mid gain "    + MakeGainString(pmbEqualizerSettings[8].Value);
                pmbEqualizerSettings[9].TextBlock.Text = "High shelf freq "  + MakeFrequencyString(pmbEqualizerSettings[9].Value);
                pmbEqualizerSettings[10].TextBlock.Text = "High shelf gain " + MakeGainString(pmbEqualizerSettings[10].Value);
            }
            catch { }
        }

        private string MakeFrequencyString(int frequency)
        {
            return ((int)(30 + (double)frequency * 9970.0 / 127.0)).ToString();
        }

        private string MakeGainString(int gain)
        {
            return ((int)(gain - 20)).ToString();
        }

        private string MakeQString(int q)
        {
            return (Math.Truncate(100 * ((double)(0.1 + q * 17.9 / 127))) / 100).ToString("f");
        }

        private String IndexToKey(byte value)
        {
            switch (value)
            {
                case 0:
                    return "-Oct";
                case 1:
                    return "-7";
                case 2:
                    return "-6";
                case 3:
                    return "-5";
                case 4:
                    return "-3";
                case 5:
                    return "0";
                case 6:
                    return "+3";
                case 7:
                    return "+5";
                case 8:
                    return "+6";
                case 9:
                    return "+7";
                case 10:
                    return "+Oct";
            }
            return "";
        }

        public void CloseAllPopupMenus()
        {
            foreach (PopupMenuButton pmb in pmbVocoder.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmbHarmony.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmbRobot.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmbMegaphone.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmbReverb.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmbVocoder.Children[1])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmbHarmony.Children[1])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmbRobot.Children[1])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmbMegaphone.Children[1])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmbReverb.Children[1])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmbRoland.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            for (int i = 0; i < pmbRoland.Children[0].Count; i++)
            {
                if (pmbRoland.Children[0][i].Children.Count > 0)
                {
                    foreach (PopupMenuButton pmb in pmbRoland.Children[0][i].Children[0])
                    {
                        pmb.Visibility = Visibility.Collapsed;
                    }
                }
            }

            CloseAllSceneMenus();
        }

        public void CloseAllSceneMenus()
        {
            foreach (PopupMenuButton pmb in pmb1.Children[0][1].Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb1.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb2.Children[0][1].Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb2.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb3.Children[0][1].Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb3.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb4.Children[0][1].Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb4.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb5.Children[0][1].Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb5.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb6.Children[0][1].Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb6.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb7.Children[0][1].Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb7.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb8.Children[0][1].Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
            foreach (PopupMenuButton pmb in pmb8.Children[0])
            {
                pmb.Visibility = Visibility.Collapsed;
            }
        }

        public void SaveScene(int patchNumber)
        {
            VT4.UserRobot[patchNumber] = new Robot(VT4.TemporaryRobot);
            WriteRobotVariation(patchNumber);
            VT4.UserMegaphone[patchNumber] = new Megaphone(VT4.TemporaryMegaphone);
            WriteMegaphoneVariation(patchNumber);
            VT4.UserVocoder[patchNumber] = new Vocoder(VT4.TemporaryVocoder);
            WriteVocoderVariation(patchNumber);
            VT4.UserHarmony[patchNumber] = new Harmony(VT4.TemporaryHarmony);
            WriteHarmonyVariation(patchNumber);
            VT4.UserReverb[patchNumber] = new Reverb(VT4.TemporaryReverb);
            WriteReverbVariation(patchNumber);
            VT4.UserPatch[patchNumber] = new Patch(VT4.TemporaryPatch);
            WriteUserPatch(patchNumber);
        }

        public void CopyScene(int from, int to)
        {
            VT4.UserRobot[to] = new Robot(VT4.UserRobot[from]);
            WriteUserRobot(to);
            VT4.UserMegaphone[to] = new Megaphone(VT4.UserMegaphone[from]);
            WriteUserMegaphone(to);
            VT4.UserVocoder[to] = new Vocoder(VT4.UserVocoder[from]);
            WriteUserVocoder(to);
            VT4.UserHarmony[to] = new Harmony(VT4.UserHarmony[from]);
            WriteUserHarmony(to);
            VT4.UserReverb[to] = new Reverb(VT4.UserReverb[from]);
            WriteUserReverb(to);
            VT4.UserPatch[to] = new Patch(VT4.UserPatch[from]);
            WriteUserPatch(to);
        }

        public void CopyUserToTemporary()
        {
            if (sceneIndex > -1)
            {
                VT4.TemporaryRobot = new Robot(VT4.UserRobot[sceneIndex]);
                VT4.TemporaryMegaphone = new Megaphone(VT4.UserMegaphone[sceneIndex]);
                VT4.TemporaryVocoder = new Vocoder(VT4.UserVocoder[sceneIndex]);
                VT4.TemporaryHarmony = new Harmony(VT4.UserHarmony[sceneIndex]);
                VT4.TemporaryReverb = new Reverb(VT4.UserReverb[sceneIndex]);
                VT4.TemporaryPatch = new Patch(VT4.UserPatch[sceneIndex]);
            }
        }

        public void SetVariation(int Variation, Area area)
        {
            switch (area)
            {
                case Area.ROBOT:
                    VT4.TemporaryPatch.ROBOT_VARIATION = (byte)Variation;
                    vt4MenuItems[0][1][0].Value = VT4.TemporaryRobot.OCTAVE;
                    vt4MenuItems[0][1][1].Value = VT4.TemporaryRobot.FEEDBACK_SWITCH;
                    vt4MenuItems[0][1][2].Value = VT4.TemporaryRobot.FEEDBACK_RESONANCE;
                    vt4MenuItems[0][1][3].Value = VT4.TemporaryRobot.FEEDBACK_LEVEL;
                    WriteUserRobot(Variation);
                    UpdatePanelLabels();
                    break;
                case Area.MEGAPHONE:
                    VT4.TemporaryPatch.MEGAPHONE_VARIATION = (byte)Variation;
                    VT4.TemporaryMegaphone.MEGAPHONE_TYPE = (byte)Variation;
                    vt4MenuItems[1][1][0].Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_1;
                    vt4MenuItems[1][1][1].Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_2;
                    vt4MenuItems[1][1][2].Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_3;
                    vt4MenuItems[1][1][3].Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_4;
                    UpdatePanelLabels();
                    break;
                case Area.REVERB: // This is the reverb
                    VT4.TemporaryPatch.REVERB_VARIATION = (byte)Variation;
                    VT4.TemporaryReverb.REVERB_TYPE = (byte)Variation;
                    vt4MenuItems[7][1][0].Value = VT4.TemporaryReverb.REVERB_PARAMETER_1;
                    vt4MenuItems[7][1][1].Value = VT4.TemporaryReverb.REVERB_PARAMETER_2;
                    vt4MenuItems[7][1][2].Value = VT4.TemporaryReverb.REVERB_PARAMETER_3;
                    vt4MenuItems[7][1][3].Value = VT4.TemporaryReverb.REVERB_PARAMETER_4;
                    UpdatePanelLabels();
                    break;
                case Area.VOCODER:
                    VT4.TemporaryPatch.VOCODER_VARIATION = (byte)Variation;
                    VT4.TemporaryVocoder.VOCODER_TYPE = (byte)Variation;
                    vt4MenuItems[0][1][0].Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_1;
                    vt4MenuItems[0][1][1].Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_2;
                    vt4MenuItems[0][1][2].Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_3;
                    vt4MenuItems[0][1][3].Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_4;
                    UpdatePanelLabels();
                    break;
                case Area.HARMONY:
                    VT4.TemporaryPatch.HARMONY_VARIATION = (byte)Variation;
                    vt4MenuItems[1][1][0].Value = VT4.TemporaryHarmony.HARMONY_1_LEVEL;
                    vt4MenuItems[1][1][1].Value = VT4.TemporaryHarmony.HARMONY_2_LEVEL;
                    vt4MenuItems[1][1][2].Value = VT4.TemporaryHarmony.HARMONY_3_LEVEL;
                    vt4MenuItems[1][1][3].Value = VT4.TemporaryHarmony.HARMONY_1_KEY;
                    vt4MenuItems[1][1][4].Value = VT4.TemporaryHarmony.HARMONY_2_KEY;
                    vt4MenuItems[1][1][5].Value = VT4.TemporaryHarmony.HARMONY_3_KEY;
                    vt4MenuItems[1][1][6].Value = VT4.TemporaryHarmony.HARMONY_1_GENDER;
                    vt4MenuItems[1][1][7].Value = VT4.TemporaryHarmony.HARMONY_2_GENDER;
                    vt4MenuItems[1][1][8].Value = VT4.TemporaryHarmony.HARMONY_3_GENDER;
                    UpdatePanelLabels();
                    break;
            }
            CloseAllSceneMenus();
        }
    }
}
