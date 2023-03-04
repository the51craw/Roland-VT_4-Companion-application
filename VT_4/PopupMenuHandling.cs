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

        private Int32 AreaToMenuNumber(Area Area)
        {
            Int32 menuNumber = -1;

            switch (Area)
            {
                case Area.ROBOT:
                    menuNumber = 2;
                    break;
                case Area.MEGAPHONE:
                    menuNumber = 3;
                    break;
            //    case Area.BUTTON1:
            //        menuNumber = 2;
            //        break;
            //    case Area.BUTTON2:
            //        menuNumber = 3;
            //        break;
            //    case Area.BUTTON3:
            //        menuNumber = 4;
            //        break;
            //    case Area.BUTTON4:
            //        menuNumber = 5;
            //        break;
            //    case Area.BUTTON5:
            //        menuNumber = 11;
            //        break;
            //    case Area.BUTTON6:
            //        menuNumber = 12;
            //        break;
            //    case Area.BUTTON7:
            //        menuNumber = 13;
            //        break;
            //    case Area.BUTTON8:
            //        menuNumber = 14;
            //        break;
            //    case Area.MANUAL:
            //        menuNumber = 6;
            //        break;
            //    case Area.BYPASS:
                case Area.REVERB:
                    if (VT4.TemporaryReverb != null)
                    {
                        SetReverbParameterTexts();
                    }
                    menuNumber = 4;
                    break;
                case Area.VOCODER:
                    menuNumber = 0;
                    break;
                case Area.HARMONY:
                    menuNumber = 1;
                    break;
            //    case Area.ROLAND:
            //    case Area.USB_MIXING:
            //        menuNumber = 10;
            //        break;
            }

            return menuNumber;
        }

        private void ShowButtonMenu(Area button, Int32 menu)
        {
            //// In case another menu is already open:
            //CloseAllMenuItems();

            //// Open all items for current menu:
            //Int32 menuButton = AreaToMenuNumber(button);
            //if (menuButton > -1 && vt4MenuItems[menuButton] != null && vt4MenuItems[menuButton].Count > menu)
            //{
            //    foreach (PopupMenuButton vt4MenuItem in vt4MenuItems[menuButton][menu])
            //    {
            //        //vt4MenuItem.Visibility = Visibility.Visible;
            //        vt4MenuItem.Set(true);
            //    }
            //}
        }

        private void GetMenuValues(Area area)
        {
            CopyVT4ToPopupMenus(); // HBE Do we need this?
            // HBE UpdatePositions();
            //if (PatchIndex == -1)
            //{
            //    ReadTemporaryRobot(); // Variation -1 is the Temporary stuff.
            //}
            //else
            //{
            //    ReadUserRobot(PatchIndex);
            //}
            //vt4MenuItems[0][1][0].Value.ToString();
            switch (area)
            {
                case Area.VOCODER:
                    ReadTemporaryVocoder();
                    break;
                case Area.HARMONY:
                    ReadTemporaryHarmony();
                    break;
                case Area.ROBOT:
                    ReadTemporaryRobot();
                    break;
                case Area.MEGAPHONE:
                    ReadTemporaryMegaphone();
                    break;
                case Area.REVERB:
                    ReadTemporaryReverb();
                    break;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// Popup menu handlers (note that there are also button handlers, not to confuse!)
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        private void HandleRolandMenu(object sender, PointerRoutedEventArgs e)
        {
            Vt4MenuItem itemSelected = (Vt4MenuItem)((Image)sender).Tag;
            ////VariationIndex = VariationNumberFromArea(itemSelected.Area);
            //Int32 value = itemSelected.GetSliderPosition(e);
            //itemSelected.Value = Exceptions(itemSelected, value);
            //itemSelected.SetSliderPosition();

            switch ((int)itemSelected.MouseButton)
            {
                case 1: // MIDI channel selection
                    SetMidiChannel(itemSelected, new EventArgs());
                    break;
                case 2: // Levels
                    switch (itemSelected.MenuRow)
                    {
                        case 0: // Gate level
                            SetGateLevel(itemSelected, new EventArgs());
                            break;
                        case 1: // Formant depth
                            SetFormantDepthLevel(itemSelected, new EventArgs());
                            break;
                        case 2: // Low cut
                            SetLowCutLevel(itemSelected, new EventArgs());
                            break;
                        case 3: // Enhancer
                            SetEnhancerLevel(itemSelected, new EventArgs());
                            break;
                        case 4: // Usb Mixing
                            SetUsbMixingLevel(itemSelected, new EventArgs());
                            break;
                    }
                    break;
                case 3: // Switches
                    switch (itemSelected.MenuRow)
                    {
                        case 0: // Monitor mode
                            SetMonitorMode(itemSelected, new EventArgs());
                            break;
                        case 1: // External carrier
                            SetExternalCarrier(itemSelected, new EventArgs());
                            break;
                        case 2: // Midi in
                            SetMidiIn(itemSelected, new EventArgs());
                            break;
                        case 3: // Pitch/Form routing
                            SetPitchAndFormantRouting(itemSelected, new EventArgs());
                            break;
                        case 4: // Mute mode
                            SetMuteMode(itemSelected, new EventArgs());
                            break;
                    }
                    break;
                case 4: // Equalizer
                    switch (itemSelected.MenuRow)
                    {
                        case 0: // Switch
                            SetEqualizerSwitch(itemSelected, new EventArgs());
                            break;
                        case 1: // Low shelf frequency
                            EventArgs eventArgs = new EventArgs();
                            //eventArgs = (int)e;
                            SetEqualizerLowShelfFrequency(itemSelected, new EventArgs());
                            break;
                        case 2: // Low shelf gain
                            SetEqualizerLowShelfGain(itemSelected, new EventArgs());
                            break;
                        case 3: // Low mid frequency
                            SetEqualizerLowMidFrequency(itemSelected, new EventArgs());
                            break;
                        case 4: // Low mid Q frequency
                            SetEqualizerLowMidQ(itemSelected, new EventArgs());
                            break;
                        case 5: // Low mid gain
                            SetEqualizerLowMidGain(itemSelected, new EventArgs());
                            break;
                        case 6: // High shelf frequency
                            SetEqualizerHighShelfFrequency(itemSelected, new EventArgs());
                            break;
                        case 7: // High shelf gain
                            SetEqualizerHighShelfGain(itemSelected, new EventArgs());
                            break;
                        case 8: // High mid frequency
                            SetEqualizerHighMidFrequency(itemSelected, new EventArgs());
                            break;
                        case 9: // High mid Q frequency
                            SetEqualizerHighMidQ(itemSelected, new EventArgs());
                            break;
                        case 10: // High mid gain
                            SetEqualizerHighMidGain(itemSelected, new EventArgs());
                            break;
                    }
                    break;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// Helpers
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        //public Int32 Exceptions(Vt4MenuItem itemSelected, Int32 value)
        //{
        //    switch (itemSelected.Area)
        //    {
                //case Area.REVERB_PARAMETER_1:
                //    if (VT4.TemporaryReverb.REVERB_TYPE == 2 || VT4.TemporaryReverb.REVERB_TYPE == 3)
                //    {
                //        value = (Int32)((Double)value / 64);
                //        itemSelected.TextBlock.Text = itemSelected.TextBlock.Text.Remove(itemSelected.TextBlock.Text.LastIndexOf(' ')) + " " + (value + 1).ToString();
                //    }
                //    break;
                //case Area.REVERB_PARAMETER_2:
                //    if (VT4.TemporaryReverb.REVERB_TYPE == 2 || VT4.TemporaryReverb.REVERB_TYPE == 3)
                //    {
                //        value = (Int32)((Double)value / 2);
                //        itemSelected.TextBlock.Text = itemSelected.TextBlock.Text.Remove(itemSelected.TextBlock.Text.LastIndexOf(' ')) + " " + value.ToString();
                //    }
                //    break;
        //        case Area.ROLAND:
        //            //value = (Int32)((Double)value / 2);
        //            itemSelected.TextBlock.Text = itemSelected.TextBlock.Text.Remove(itemSelected.TextBlock.Text.LastIndexOf(' ')) + " " + value.ToString();
        //            break;
        //        default:
        //            itemSelected.TextBlock.Text = itemSelected.TextBlock.Text.Remove(itemSelected.TextBlock.Text.LastIndexOf(' ')) + " " + value.ToString();
        //            break;
        //    }

        //    return value;
        //}

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

        //public Int32 GetItemValue(Vt4MenuItem vt4MenuItem)
        //{
        //    switch (vt4MenuItem.Area)
        //    {
        //        case Area.ROBOT_PARAMETER_1:
        //            vt4MenuItem.Value = VT4.TemporaryRobot.OCTAVE;
        //            break;
        //        case Area.ROBOT_PARAMETER_2:
        //            vt4MenuItem.Value = VT4.TemporaryRobot.FEEDBACK_SWITCH;
        //            break;
        //        case Area.ROBOT_PARAMETER_3:
        //            vt4MenuItem.Value = VT4.TemporaryRobot.FEEDBACK_RESONANCE;
        //            break;
        //        case Area.ROBOT_PARAMETER_4:
        //            vt4MenuItem.Value = VT4.TemporaryRobot.FEEDBACK_LEVEL;
        //            break;
        //        case Area.MEGAPHONE_PARAMETER_1:
        //            vt4MenuItem.Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_1;
        //            break;
        //        case Area.MEGAPHONE_PARAMETER_2:
        //            vt4MenuItem.Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_2;
        //            break;
        //        case Area.MEGAPHONE_PARAMETER_3:
        //            vt4MenuItem.Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_3;
        //            break;
        //        case Area.MEGAPHONE_PARAMETER_4:
        //            vt4MenuItem.Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_4;
        //            break;
        //        case Area.REVERB_PARAMETER_1:
        //            vt4MenuItem.Value = VT4.TemporaryReverb.REVERB_PARAMETER_1;
        //            break;
        //        case Area.REVERB_PARAMETER_2:
        //            vt4MenuItem.Value = VT4.TemporaryReverb.REVERB_PARAMETER_2;
        //            break;
        //        case Area.REVERB_PARAMETER_3:
        //            vt4MenuItem.Value = VT4.TemporaryReverb.REVERB_PARAMETER_3;
        //            break;
        //        case Area.REVERB_PARAMETER_4:
        //            vt4MenuItem.Value = VT4.TemporaryReverb.REVERB_PARAMETER_4;
        //            break;
        //    }
        //    return vt4MenuItem.Value;
        //}

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
            try
            {
                switch (VT4.TemporaryPatch.MEGAPHONE_VARIATION)
                {
                    case 0:
                    case 4:
                        pmbMegaphone.Children[1][0].TextBlock.Text = "Clip Gain " + pmbMegaphone.Children[1][0].Value.ToString();
                        pmbMegaphone.Children[1][1].TextBlock.Text = "Direct Level " + pmbMegaphone.Children[1][1].Value.ToString();
                        pmbMegaphone.Children[1][2].TextBlock.Text = "Volume " + pmbMegaphone.Children[1][2].Value.ToString();
                        pmbMegaphone.Children[1][3].Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                    case 5:
                        pmbMegaphone.Children[1][0].TextBlock.Text = "Drive " + pmbMegaphone.Children[1][0].Value.ToString();
                        pmbMegaphone.Children[1][1].TextBlock.Text = "Sampling Rate " + pmbMegaphone.Children[1][1].Value.ToString();
                        pmbMegaphone.Children[1][2].TextBlock.Text = "Low Cut " + pmbMegaphone.Children[1][2].Value.ToString();
                        pmbMegaphone.Children[1][3].TextBlock.Text = "High Cut " + pmbMegaphone.Children[1][3].Value.ToString();
                        break;
                    case 2:
                        pmbMegaphone.Children[1][0].TextBlock.Text = "Mode " + pmbMegaphone.Children[1][0].Value.ToString();
                        pmbMegaphone.Children[1][1].TextBlock.Text = "Depth " + pmbMegaphone.Children[1][1].Value.ToString();
                        pmbMegaphone.Children[1][2].TextBlock.Text = "Effect Level " + pmbMegaphone.Children[1][2].Value.ToString();
                        pmbMegaphone.Children[1][3].TextBlock.Text = "Noice Level " + pmbMegaphone.Children[1][3].Value.ToString();
                        break;
                    case 3:
                    case 7:
                        pmbMegaphone.Children[1][0].TextBlock.Text = "Wave Shape " + pmbMegaphone.Children[1][0].Value.ToString();
                        pmbMegaphone.Children[1][1].TextBlock.Text = "Rate " + pmbMegaphone.Children[1][1].Value.ToString();
                        pmbMegaphone.Children[1][2].TextBlock.Text = "Depth " + pmbMegaphone.Children[1][2].Value.ToString();
                        pmbMegaphone.Children[1][3].TextBlock.Text = "Level " + pmbMegaphone.Children[1][3].Value.ToString();
                        break;
                    case 6:
                        pmbMegaphone.Children[1][0].TextBlock.Text = "Parameter 1 " + pmbMegaphone.Children[1][0].Value.ToString();
                        pmbMegaphone.Children[1][1].TextBlock.Text = "Parameter 2 " + pmbMegaphone.Children[1][1].Value.ToString();
                        pmbMegaphone.Children[1][2].TextBlock.Text = "Parameter 3 " + pmbMegaphone.Children[1][2].Value.ToString();
                        pmbMegaphone.Children[1][3].TextBlock.Text = "Parameter 4 " + pmbMegaphone.Children[1][3].Value.ToString();
                        break;
                }
            }
            catch { }
        }

        public void SetReverbParameterTexts()
        {
            try
            {
                switch (VT4.TemporaryPatch.REVERB_VARIATION)
                {
                    case 0:
                    case 1:
                    case 3:
                    case 4:
                    case 5:
                    case 6: // Experimentera fram denna!
                    case 7: // Experimentera fram denna!
                        pmbReverb.Children[1][0].TextBlock.Text = "Pre Delay " + pmbReverb.Children[1][0].Value.ToString();
                        pmbReverb.Children[1][1].TextBlock.Text = "Feedback " + pmbReverb.Children[1][1].Value.ToString();
                        pmbReverb.Children[1][2].TextBlock.Text = "Low Cut " + pmbReverb.Children[1][2].Value.ToString();
                        pmbReverb.Children[1][3].TextBlock.Text = "High Cut " + pmbReverb.Children[1][3].Value.ToString();
                        break;
                    //case 1:
                    //    pmbReverb.Children[1][0].TextBlock.Text = "Pre Delay " + pmbReverb.Children[1][0].Value.ToString();
                    //    pmbReverb.Children[1][1].TextBlock.Text = "Delay " + pmbReverb.Children[1][1].Value.ToString();
                    //    pmbReverb.Children[1][2].TextBlock.Text = "Low Cut " + pmbReverb.Children[1][2].Value.ToString();
                    //    pmbReverb.Children[1][3].TextBlock.Text = "High Cut " + pmbReverb.Children[1][3].Value.ToString();
                    //    break;
                    //case 4:
                    //    pmbReverb.Children[1][0].TextBlock.Text = "Mode " + pmbReverb.Children[1][0].Value.ToString();
                    //    pmbReverb.Children[1][1].TextBlock.Text = "Sync Note " + pmbReverb.Children[1][1].Value.ToString();
                    //    pmbReverb.Children[1][2].TextBlock.Text = "Low Cut " + pmbReverb.Children[1][2].Value.ToString();
                    //    pmbReverb.Children[1][3].TextBlock.Text = "High Cut " + pmbReverb.Children[1][3].Value.ToString();
                    //    break;
                    //case 5:
                    //    pmbReverb.Children[1][0].TextBlock.Text = "Mode " + pmbReverb.Children[1][0].Value.ToString();
                    //    pmbReverb.Children[1][1].TextBlock.Text = "Feedback " + pmbReverb.Children[1][1].Value.ToString();
                    //    pmbReverb.Children[1][2].TextBlock.Text = "Low Cut " + pmbReverb.Children[1][2].Value.ToString();
                    //    pmbReverb.Children[1][3].TextBlock.Text = "High Cut " + pmbReverb.Children[1][3].Value.ToString();
                    //    break;
                    case 2:
                        pmbReverb.Children[1][0].TextBlock.Text = "Mode " + (pmbReverb.Children[1][0].Value).ToString();
                        pmbReverb.Children[1][1].TextBlock.Text = "Sync Note " + pmbReverb.Children[1][1].Value.ToString();
                        pmbReverb.Children[1][2].TextBlock.Text = "Low Cut " + pmbReverb.Children[1][2].Value.ToString();
                        pmbReverb.Children[1][3].TextBlock.Text = "High Cut " + pmbReverb.Children[1][3].Value.ToString();
                        break;
                    //case 3:
                    //    pmbReverb.Children[1][0].TextBlock.Text = "Mode " + (pmbReverb.Children[1][0].Value + 1).ToString();
                    //    pmbReverb.Children[1][1].TextBlock.Text = "Feedback " + pmbReverb.Children[1][1].Value.ToString();
                    //    pmbReverb.Children[1][2].TextBlock.Text = "Low Cut " + pmbReverb.Children[1][2].Value.ToString();
                    //    pmbReverb.Children[1][3].TextBlock.Text = "High Cut " + pmbReverb.Children[1][3].Value.ToString();
                    //    break;
                }
            }
            catch { }
        }

        public void SetVocoderParameterTexts()
        {
            try
            {
                switch (VT4.TemporaryPatch.REVERB_VARIATION)
                {
                    case 0:
                        pmbVocoder.Children[1][0].TextBlock.Text = "Release " + pmbVocoder.Children[1][0].Value.ToString();
                        pmbVocoder.Children[1][1].TextBlock.Text = "Tone " + pmbVocoder.Children[1][1].Value.ToString();
                        pmbVocoder.Children[1][2].TextBlock.Text = "Consonant " + pmbVocoder.Children[1][2].Value.ToString();
                        pmbVocoder.Children[1][3].TextBlock.Text = "Effect Level " + pmbVocoder.Children[1][3].Value.ToString();
                        break;
                    case 1:
                    case 3:
                        pmbVocoder.Children[1][0].TextBlock.Text = "Release " + pmbVocoder.Children[1][0].Value.ToString();
                        pmbVocoder.Children[1][1].TextBlock.Text = "Tone " + pmbVocoder.Children[1][1].Value.ToString();
                        pmbVocoder.Children[1][2].TextBlock.Text = "OSC Color " + pmbVocoder.Children[1][2].Value.ToString();
                        pmbVocoder.Children[1][3].TextBlock.Text = "Effect Level " + pmbVocoder.Children[1][3].Value.ToString();
                        break;
                    case 2:
                        pmbVocoder.Children[1][0].TextBlock.Text = "Release " + pmbVocoder.Children[1][0].Value.ToString();
                        pmbVocoder.Children[1][1].TextBlock.Text = "Formant Depth " + pmbVocoder.Children[1][1].Value.ToString();
                        pmbVocoder.Children[1][2].TextBlock.Text = "OSC Color " + pmbVocoder.Children[1][2].Value.ToString();
                        pmbVocoder.Children[1][3].TextBlock.Text = "Effect Level " + pmbVocoder.Children[1][3].Value.ToString();
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

        public void ShowMenuSliderHandles(List<Vt4MenuItem> item)
        {
            // Iterate the items of the menu:
            foreach (Vt4MenuItem menuItem in item)
            {
                menuItem.ShowHandle = true;
            }
        }

        //public void LoadScene(object sender, EventArgs e)
        //{
        //    byte patch = (byte)((vt4EventArgs)e).IntValue;

        //    SendProgramChange(patch, VT4.System.MIDI_CH);

        //    //LoadUserPatch(patch);
        //    WriteTemporary();

        //    imgRobotOn.Visibility = VT4.TemporaryPatch.ROBOT > 0 ? Visibility.Visible : Visibility.Collapsed;
        //    imgMegaphoneOn.Visibility = VT4.TemporaryPatch.MEGAPHONE > 0 ? Visibility.Visible : Visibility.Collapsed;
        //    imgVocoderOn.Visibility = VT4.TemporaryPatch.VOCODER > 0 ? Visibility.Visible : Visibility.Collapsed;
        //    imgHarmonyOn.Visibility = VT4.TemporaryPatch.HARMONY > 0 ? Visibility.Visible : Visibility.Collapsed;

        //    SetSliderHandle(Area.ROBOT, VT4.TemporaryPatch.PITCH, false);
        //    SetSliderHandle(Area.FORMANT, VT4.TemporaryPatch.FORMANT, false);
        //    SetSliderHandle(Area.BALANCE, VT4.TemporaryPatch.BALANCE, false);
        //    SetSliderHandle(Area.REVERB, VT4.TemporaryPatch.REVERB, false);

        //    SetPotHandles();

        //    // Hide the menuitem and its siblings:
        //    CloseAllMenuItems();

        //    // Manual button should now be off:
        //    imgManualOn.Visibility = Visibility.Collapsed;
        //}

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
            VT4.TemporaryRobot = new Robot(VT4.UserRobot[sceneIndex]);
            VT4.TemporaryMegaphone = new Megaphone(VT4.UserMegaphone[sceneIndex]);
            VT4.TemporaryVocoder = new Vocoder(VT4.UserVocoder[sceneIndex]);
            VT4.TemporaryHarmony = new Harmony(VT4.UserHarmony[sceneIndex]);
            VT4.TemporaryReverb = new Reverb(VT4.UserReverb[sceneIndex]);
            VT4.TemporaryPatch = new Patch(VT4.UserPatch[sceneIndex]);
        }

        public void CopyFromPatch(object sender, EventArgs e)
        {
            CopyFrom = (Int32)((vt4EventArgs)e).IntValue;
            Area button = ((Vt4MenuItem)sender).Area;

            // In case another menu is already open:
            CloseAllSceneMenus();

            // Open all items for current menu:
            Int32 menuButton = AreaToMenuNumber(button);
            if (menuButton > -1 && vt4MenuItems[menuButton] != null && vt4MenuItems[menuButton].Count > 1)
            {
                foreach (PopupMenuButton vt4MenuItem in vt4MenuItems[menuButton][2])
                {
                    vt4MenuItem.Visibility = Visibility.Visible;
                }
            }
        }

        public void CopyToPatch(object sender, EventArgs e)
        {
            ReadPatch((byte)CopyFrom);
            Int32 copyTo = ((vt4EventArgs)e).IntValue;
            VT4.UserPatch[copyTo] = new Patch(VT4.TemporaryPatch);
            VT4.UserRobot[copyTo] = new Robot(VT4.TemporaryRobot);
            VT4.UserMegaphone[copyTo] = new Megaphone(VT4.TemporaryMegaphone);
            VT4.UserVocoder[copyTo] = new Vocoder(VT4.TemporaryVocoder);
            VT4.UserHarmony[copyTo] = new Harmony(VT4.TemporaryHarmony);
            VT4.UserReverb[copyTo] = new Reverb(VT4.TemporaryReverb);
            WriteVt4(copyTo);

            // Hide the menuitem and its siblings:
            CloseAllSceneMenus();
        }

        /// <summary>
        /// Setting a variation involves copying the corresponding user area to the
        /// temporary area for the effect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetVariation(object sender, EventArgs e)
        {
            Variation = ((PopupMenuButton)currentControl).MenuItemNumber; //(Int32)((vt4EventArgs)e).IntValue;
            //Area area = ((Vt4MenuItem)sender).Area;
            switch (currentArea)
            {
                case Area.ROBOT:
                    VT4.TemporaryRobot = new Robot(VT4.UserRobot[Variation]);
                    //WriteUserRobot(Variation);
                    WriteTemporaryRobot();
                    break;
                case Area.MEGAPHONE:
                    VT4.TemporaryMegaphone = new Megaphone(VT4.UserMegaphone[Variation]);
                    //WriteUserMegaphone(Variation);
                    WriteTemporaryMegaphone();
                    break;
                case Area.VOCODER:
                    VT4.TemporaryVocoder = new Vocoder(VT4.UserVocoder[Variation]);
                    //WriteUserVocoder(Variation);
                    WriteTemporaryVocoder();
                    break;
                case Area.HARMONY:
                    VT4.TemporaryHarmony = new Harmony(VT4.UserHarmony[Variation]);
                    //WriteUserHarmony(Variation);
                    WriteTemporaryHarmony();
                    break;
                case Area.REVERB:
                    VT4.TemporaryReverb = new Reverb(VT4.UserReverb[Variation]);
                    //WriteUserReverb(Variation);
                    WriteTemporaryReverb();
                    break;
                case Area.EQUALIZER:
                    VT4.TemporaryEqualizer = new Equalizer(VT4.UserEqualizer);
                    WriteUserEqualizer();
                    break;
            }
            //SendVariation(Variation, area);
            SendVariation(Variation, currentArea);
        }

        public void SendVariation(int variation, Area area)
        {
            switch (area)
            {
                case Area.ROBOT:
                    SendSingleSysEx(Area.ROBOT_VARIATION, new byte[] { (byte)Variation });
                    break;
                //case Area.MEGAPHONE:
                //    SendSingleSysEx(Area.MEGAPHONE_VARIATION, new byte[] { (byte)Variation });
                //    break;
                case Area.REVERB: // This is the reverb
                    SendSingleSysEx(Area.REVERB_TYPE, new byte[] { (byte)Variation });
                    break;
                case Area.VOCODER:
                    SendSingleSysEx(Area.VOCODER_VARIATION, new byte[] { (byte)Variation });
                    break;
                //case Area.HARMONY:
                //    SendSingleSysEx(Area.HARMONY_VARIATION, new byte[] { (byte)Variation });
                //    break;
            }
        }

        public void SetVariation(int Variation, Area area)
        {
            switch (area)
            {
                case Area.ROBOT:
                    //VT4.TemporaryRobot = new Robot(VT4.Robot[Variation]);
                    //WriteTemporaryRobot();
                    VT4.TemporaryPatch.ROBOT_VARIATION = (byte)Variation;
                    vt4MenuItems[0][1][0].Value = VT4.TemporaryRobot.OCTAVE;
                    vt4MenuItems[0][1][1].Value = VT4.TemporaryRobot.FEEDBACK_SWITCH;
                    vt4MenuItems[0][1][2].Value = VT4.TemporaryRobot.FEEDBACK_RESONANCE;
                    vt4MenuItems[0][1][3].Value = VT4.TemporaryRobot.FEEDBACK_LEVEL;
                    WriteUserRobot(Variation);
                    //SetRobotParameterTexts();
                    UpdatePanelLabels();
                    break;
                case Area.MEGAPHONE:
                    //VT4.TemporaryMegaphone = new Megaphone(VT4.Megaphone[Variation]);
                    //WriteTemporaryMegaphone();
                    VT4.TemporaryPatch.MEGAPHONE_VARIATION = (byte)Variation;
                    //VT4.TemporaryMegaphone = VT4.UserMegaphone[Variation];
                    VT4.TemporaryMegaphone.MEGAPHONE_TYPE = (byte)Variation;
                    vt4MenuItems[1][1][0].Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_1;
                    vt4MenuItems[1][1][1].Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_2;
                    vt4MenuItems[1][1][2].Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_3;
                    vt4MenuItems[1][1][3].Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_4;
                    //SetMegaphoneParameterTexts();
                    UpdatePanelLabels();
                    break;
                case Area.REVERB: // This is the reverb
                    //VT4.TemporaryReverb = new Reverb(VT4.Reverb[Variation]);
                    //WriteTemporaryReverb();
                    VT4.TemporaryPatch.REVERB_VARIATION = (byte)Variation;
                    VT4.TemporaryReverb.REVERB_TYPE = (byte)Variation;
                    vt4MenuItems[7][1][0].Value = VT4.TemporaryReverb.REVERB_PARAMETER_1;
                    vt4MenuItems[7][1][1].Value = VT4.TemporaryReverb.REVERB_PARAMETER_2;
                    vt4MenuItems[7][1][2].Value = VT4.TemporaryReverb.REVERB_PARAMETER_3;
                    vt4MenuItems[7][1][3].Value = VT4.TemporaryReverb.REVERB_PARAMETER_4;
                    //SetReverbParameterTexts();
                    UpdatePanelLabels();
                    break;
                case Area.VOCODER:
                    //VT4.TemporaryVocoder = new Vocoder(VT4.Vocoder[Variation]);
                    //WriteTemporaryVocoder();
                    VT4.TemporaryPatch.VOCODER_VARIATION = (byte)Variation;
                    VT4.TemporaryVocoder.VOCODER_TYPE = (byte)Variation;
                    vt4MenuItems[0][1][0].Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_1;
                    vt4MenuItems[0][1][1].Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_2;
                    vt4MenuItems[0][1][2].Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_3;
                    vt4MenuItems[0][1][3].Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_4;
                    //SetVocoderParameterTexts();
                    UpdatePanelLabels();
                    break;
                case Area.HARMONY:
                    VT4.TemporaryPatch.HARMONY_VARIATION = (byte)Variation;
                    //VT4.TemporaryHarmony = new Harmony(VT4.Harmony[Variation]);
                    //WriteTemporaryHarmony();
                    vt4MenuItems[1][1][0].Value = VT4.TemporaryHarmony.HARMONY_1_LEVEL;
                    vt4MenuItems[1][1][1].Value = VT4.TemporaryHarmony.HARMONY_2_LEVEL;
                    vt4MenuItems[1][1][2].Value = VT4.TemporaryHarmony.HARMONY_3_LEVEL;
                    vt4MenuItems[1][1][3].Value = VT4.TemporaryHarmony.HARMONY_1_KEY;
                    vt4MenuItems[1][1][4].Value = VT4.TemporaryHarmony.HARMONY_2_KEY;
                    vt4MenuItems[1][1][5].Value = VT4.TemporaryHarmony.HARMONY_3_KEY;
                    vt4MenuItems[1][1][6].Value = VT4.TemporaryHarmony.HARMONY_1_GENDER;
                    vt4MenuItems[1][1][7].Value = VT4.TemporaryHarmony.HARMONY_2_GENDER;
                    vt4MenuItems[1][1][8].Value = VT4.TemporaryHarmony.HARMONY_3_GENDER;
                    //SetHarmonyParameterTexts();
                    UpdatePanelLabels();
                    break;
            }
            CloseAllSceneMenus();
        }

        //public void SetSliderPositions()
        //{
        //    foreach (List<List<Vt4MenuItem>> menuItemsList in vt4MenuItems)
        //    {
        //        foreach (List<Vt4MenuItem> menuItems in menuItemsList)
        //        {
        //            foreach (Vt4MenuItem menuItem in menuItems)
        //            {
        //                if (menuItem.ShowHandle)
        //                {
        //                    switch (menuItem.Area)
        //                    {
        //                        case Area.ROBOT_PARAMETER_1:
        //                            menuItem.Value = VT4.TemporaryRobot.OCTAVE;
        //                            break;
        //                        case Area.ROBOT_PARAMETER_2:
        //                            menuItem.Value = VT4.TemporaryRobot.FEEDBACK_SWITCH;
        //                            break;
        //                        case Area.ROBOT_PARAMETER_3:
        //                            menuItem.Value = VT4.TemporaryRobot.FEEDBACK_RESONANCE;
        //                            break;
        //                        case Area.ROBOT_PARAMETER_4:
        //                            menuItem.Value = VT4.TemporaryRobot.FEEDBACK_LEVEL;
        //                            break;
        //                        case Area.MEGAPHONE_PARAMETER_1:
        //                            menuItem.Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_1;
        //                            break;
        //                        case Area.MEGAPHONE_PARAMETER_2:
        //                            menuItem.Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_2;
        //                            break;
        //                        case Area.MEGAPHONE_PARAMETER_3:
        //                            menuItem.Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_3;
        //                            break;
        //                        case Area.MEGAPHONE_PARAMETER_4:
        //                            menuItem.Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_4;
        //                            break;
        //                        case Area.REVERB_PARAMETER_1:
        //                            menuItem.Value = VT4.TemporaryReverb.REVERB_PARAMETER_1;
        //                            break;
        //                        case Area.REVERB_PARAMETER_2:
        //                            menuItem.Value = VT4.TemporaryReverb.REVERB_PARAMETER_2;
        //                            break;
        //                        case Area.REVERB_PARAMETER_3:
        //                            menuItem.Value = VT4.TemporaryReverb.REVERB_PARAMETER_3;
        //                            break;
        //                        case Area.REVERB_PARAMETER_4:
        //                            menuItem.Value = VT4.TemporaryReverb.REVERB_PARAMETER_4;
        //                            break;
        //                        case Area.VOCODER_PARAMETER_1:
        //                            menuItem.Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_1;
        //                            break;
        //                        case Area.VOCODER_PARAMETER_2:
        //                            menuItem.Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_2;
        //                            break;
        //                        case Area.VOCODER_PARAMETER_3:
        //                            menuItem.Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_3;
        //                            break;
        //                        case Area.VOCODER_PARAMETER_4:
        //                            menuItem.Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_4;
        //                            break;
        //                        case Area.HARMONY_PARAMETER_1:
        //                            menuItem.Value = VT4.TemporaryHarmony.HARMONY_1_LEVEL;
        //                            break;
        //                        case Area.HARMONY_PARAMETER_2:
        //                            menuItem.Value = VT4.TemporaryHarmony.HARMONY_2_LEVEL;
        //                            break;
        //                        case Area.HARMONY_PARAMETER_3:
        //                            menuItem.Value = VT4.TemporaryHarmony.HARMONY_3_LEVEL;
        //                            break;
        //                        case Area.HARMONY_PARAMETER_4:
        //                            menuItem.Value = VT4.TemporaryHarmony.HARMONY_1_KEY;
        //                            break;
        //                        case Area.HARMONY_PARAMETER_5:
        //                            menuItem.Value = VT4.TemporaryHarmony.HARMONY_2_KEY;
        //                            break;
        //                        case Area.HARMONY_PARAMETER_6:
        //                            menuItem.Value = VT4.TemporaryHarmony.HARMONY_3_KEY;
        //                            break;
        //                        case Area.HARMONY_PARAMETER_7:
        //                            menuItem.Value = VT4.TemporaryHarmony.HARMONY_1_GENDER;
        //                            break;
        //                        case Area.HARMONY_PARAMETER_8:
        //                            menuItem.Value = VT4.TemporaryHarmony.HARMONY_2_GENDER;
        //                            break;
        //                        case Area.HARMONY_PARAMETER_9:
        //                            menuItem.Value = VT4.TemporaryHarmony.HARMONY_3_GENDER;
        //                            break;
        //                    }
        //                    menuItem.SetSliderPosition();
        //                }
        //            }
        //        }
        //    }
        //}

        public void SetParameter(object sender, EventArgs e)
        {
        }

        public void ShowMidiChannelMenu(object sender, EventArgs e)
        {
            CloseAllSceneMenus();

            if (vt4MenuItems[10] != null && vt4MenuItems[10].Count > 1 && vt4MenuItems[10][1] != null && vt4MenuItems[10][1].Count > 0)
            {
                foreach (PopupMenuButton vt4MenuItem in vt4MenuItems[10][1])
                {
                    vt4MenuItem.Visibility = Visibility.Visible;
                }
            }
        }

        public void ShowEqualizerMenu(object sender, EventArgs e)
        {
            CloseAllSceneMenus();

            if (vt4MenuItems[10] != null && vt4MenuItems[10].Count > 1 && vt4MenuItems[10][4] != null && vt4MenuItems[10][4].Count > 0)
            {
                foreach (PopupMenuButton vt4MenuItem in vt4MenuItems[10][4])
                {
                    vt4MenuItem.Visibility = Visibility.Visible;
                }
            }
        }

        public void ShowLevelsMenu(object sender, EventArgs e)
        {
            CloseAllSceneMenus();

            if (vt4MenuItems[10] != null && vt4MenuItems[10].Count > 2 && vt4MenuItems[10][2] != null && vt4MenuItems[10][2].Count > 0)
            {
                foreach (PopupMenuButton vt4MenuItem in vt4MenuItems[10][2])
                {
                    vt4MenuItem.Visibility = Visibility.Visible;
                }
            }
        }

        public void ShowSwitchesMenu(object sender, EventArgs e)
        {
            CloseAllSceneMenus();

            if (vt4MenuItems[10] != null && vt4MenuItems[10].Count > 3 && vt4MenuItems[10][3] != null && vt4MenuItems[10][3].Count > 0)
            {
                foreach (PopupMenuButton vt4MenuItem in vt4MenuItems[10][3])
                {
                    vt4MenuItem.Visibility = Visibility.Visible;
                }
            }
        }

        public void ShowSaveLoadMenu(object sender, EventArgs e)
        {
            CloseAllSceneMenus();

            if (vt4MenuItems[10] != null && vt4MenuItems[10].Count > 4 && vt4MenuItems[10][4] != null && vt4MenuItems[10][4].Count > 0)
            {
                foreach (PopupMenuButton vt4MenuItem in vt4MenuItems[10][4])
                {
                    vt4MenuItem.Visibility = Visibility.Visible;
                }
            }
        }

        public void SetMidiChannel(object sender, EventArgs e)
        {
            VT4.System.MIDI_CH = (byte)((vt4EventArgs)e).IntValue;
            WriteSystem();
            CloseAllSceneMenus();
            //midi.PortPairs[0].InChannel = (byte)(((vt4EventArgs)e).IntValue - 1);
            //midi.PortPairs[0].OutChannel = (byte)(((vt4EventArgs)e).IntValue - 1);
        }

        private void SetLowCutLevel(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.LOW_CUT, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetEnhancerLevel(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.ENHANCER, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetGateLevel(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.GATE_LEVEL, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetFormantDepthLevel(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.FORMANT_DEPTH, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetUsbMixingLevel(object sender, EventArgs e)
        {
            ((Vt4MenuItem)sender).SetSliderPosition();
            Int32 value = (byte)((Vt4MenuItem)sender).Value;
            value = value > 20 ? 20 : value;
            SetSystemParameterTexts();
            SendSingleSysEx(Area.USB_MIXING, new byte[] { (byte)value });
        }

        private void SetMonitorMode(object sender, EventArgs e)
        {
            VT4.System.MONITOR_MODE = (byte)((VT4.System.MONITOR_MODE + 1) % 2);
            ((Vt4MenuItem)sender).Value = VT4.System.MONITOR_MODE;
            SetSystemParameterTexts();
            SendSingleSysEx(Area.MONITOR_MODE, new byte[] { (byte)(VT4.System.MONITOR_MODE) });
        }

        private void SetExternalCarrier(object sender, EventArgs e)
        {
            VT4.System.EXTERNAL_CARRIER = (byte)((VT4.System.EXTERNAL_CARRIER + 1) % 2);
            ((Vt4MenuItem)sender).Value = VT4.System.EXTERNAL_CARRIER;
            SetSystemParameterTexts();
            SendSingleSysEx(Area.EXTERNAL_CARRIER, new byte[] { (byte)(VT4.System.EXTERNAL_CARRIER) });
        }

        private void SetMidiIn(object sender, EventArgs e)
        {
            VT4.System.MIDI_IN_MODE = (byte)((VT4.System.MIDI_IN_MODE + 1) % 2);
            ((Vt4MenuItem)sender).Value = VT4.System.MIDI_IN_MODE;
            SetSystemParameterTexts();
            SendSingleSysEx(Area.MIDI_IN_MODE, new byte[] { (byte)(VT4.System.MIDI_IN_MODE) });
        }

        private void SetPitchAndFormantRouting(object sender, EventArgs e)
        {
            VT4.System.PITCH_AND_FORMANT_ROUTING = (byte)((VT4.System.PITCH_AND_FORMANT_ROUTING + 1) % 2);
            ((Vt4MenuItem)sender).Value = VT4.System.PITCH_AND_FORMANT_ROUTING;
            SetSystemParameterTexts();
            SendSingleSysEx(Area.PITCH_AND_FORMANT_ROUTING, new byte[] { (byte)(VT4.System.PITCH_AND_FORMANT_ROUTING) });
        }

        private void SetMuteMode(object sender, EventArgs e)
        {
            VT4.System.MUTE_MODE = (byte)((VT4.System.MUTE_MODE + 1) % 2);
            ((Vt4MenuItem)sender).Value = VT4.System.MUTE_MODE;
            SetSystemParameterTexts();
            SendSingleSysEx(Area.MUTE_MODE, new byte[] { (byte)(VT4.System.MUTE_MODE) });
        }

        private void SetEqualizerSwitch(object sender, EventArgs e)
        {
            //VT4.TemporaryEqualizer.EQUALIZER_SWITCH = (byte)((VT4.TemporaryEqualizer.EQUALIZER_SWITCH + 1) % 2);
            //vt4MenuItems[10][4][0].Value = VT4.TemporaryEqualizer.EQUALIZER_SWITCH;
            //SetEqualizerParameterTexts();
            SendSingleSysEx(Area.EQUALIZER, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetEqualizerLowShelfFrequency(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.EQUALIZER_LOW_SHELF_FREQUENCY, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetEqualizerLowShelfGain(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.EQUALIZER_LOW_SHELF_GAIN, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetEqualizerLowMidFrequency(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.EQUALIZER_LOW_MID_FREQUENCY, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetEqualizerLowMidQ(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.EQUALIZER_LOW_MID_Q, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetEqualizerLowMidGain(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.EQUALIZER_LOW_MID_GAIN, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetEqualizerHighShelfFrequency(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.EQUALIZER_HIGH_SHELF_FREQUENCY, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetEqualizerHighShelfGain(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.EQUALIZER_HIGH_SHELF_GAIN, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetEqualizerHighMidFrequency(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.EQUALIZER_HIGH_MID_FREQUENCY, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetEqualizerHighMidQ(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.EQUALIZER_HIGH_MID_Q, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private void SetEqualizerHighMidGain(object sender, EventArgs e)
        {
            SendSingleSysEx(Area.EQUALIZER_HIGH_MID_GAIN, new byte[] { (byte)((Vt4MenuItem)sender).Value });
        }

        private async void SaveJsonFile(object sender, EventArgs e)
        {
            //ReadVt4();
            await WriteJsonFile();
            CloseAllSceneMenus();
        }

        private async void LoadJsonFile(object sender, EventArgs e)
        {
            await ReadJsonFile();
            WriteVt4();
            CloseAllSceneMenus();
        }

        public void SetTimeout(object sender, EventArgs e)
        {
            Int32 timeout = (byte)((Vt4MenuItem)sender).Value;
            byte[] address = new byte[] { 0x00, 0x00, 0x00, 0x0f };
            byte[] value = new byte[] { (byte)timeout };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, value);
            midi.SendSystemExclusive(midi.PortPairs[0], message);

            // Hide the menuitem and its siblings:
            CloseAllSceneMenus();
        }

        public void SetGate(object sender, EventArgs e)
        {
            byte[] address = new byte[] { 0x00, 0x00, 0x00, 0x01 };
            byte[] value = new byte[] { (byte)((Vt4MenuItem)sender).Value };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, value);
            midi.SendSystemExclusive(midi.PortPairs[0], message);

            // Hide the menuitem and its siblings:
            CloseAllSceneMenus();
        }

        public void SetFormantDepth(object sender, EventArgs e)
        {
            byte[] address = new byte[] { 0x00, 0x00, 0x00, 0x04 };
            byte[] value = new byte[] { (byte)((Vt4MenuItem)sender).Value };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, value);
            midi.SendSystemExclusive(midi.PortPairs[0], message);

            // Hide the menuitem and its siblings:
            CloseAllSceneMenus();
        }

        public void SetLowCut(object sender, EventArgs e)
        {
            byte[] address = new byte[] { 0x00, 0x00, 0x00, 0x02 };
            byte[] value = new byte[] { (byte)((Vt4MenuItem)sender).Value };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, value);
            midi.SendSystemExclusive(midi.PortPairs[0], message);

            // Hide the menuitem and its siblings:
            CloseAllSceneMenus();
        }

        public void SetEnhancer(object sender, EventArgs e)
        {
            byte[] address = new byte[] { 0x00, 0x00, 0x00, 0x03 };
            byte[] value = new byte[] { (byte)((Vt4MenuItem)sender).Value };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, value);
            midi.SendSystemExclusive(midi.PortPairs[0], message);

            // Hide the menuitem and its siblings:
            CloseAllSceneMenus();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// Classes
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        public enum MouseButton
        {
            LEFT,
            RIGHT
        }

        public class vt4EventArgs : EventArgs
        {
            public Int32 IntValue { get; set; }
            public Int32[] Int32Array { get; set; }

            vt4EventArgs(int IntValue)
            {
                this.IntValue = IntValue;
            }

            vt4EventArgs(Int32[] Int32Array)
            {
                this.Int32Array = new Int32[Int32Array.Length];
                for (Int32 i = 0; i < Int32Array.Length; i++)
                {
                    this.Int32Array[i] = Int32Array[i];
                }
            }

            public static explicit operator vt4EventArgs(Int32 v)
            {
                return new vt4EventArgs(v);
            }

            public static explicit operator vt4EventArgs(Int32[] v)
            {
                return new vt4EventArgs(v);
            }
        }

        public class Vt4MenuItemTemplate
        {
            public String Text { get; set; }
            public Area Area { get; set; }
            public EventHandler EventHandler { get; set; }
            public vt4EventArgs e { get; set; }
            public Boolean ShowHandle { get; set; }

            public Vt4MenuItemTemplate(Area Area, String Text, EventHandler EventHandler, vt4EventArgs e, Boolean ShowHandle = false)
            {
                this.Area = Area;
                this.Text = Text;
                this.EventHandler = EventHandler;
                this.e = e;
                this.ShowHandle = ShowHandle;
            }

            public Vt4MenuItemTemplate(Area Area, String Text, EventHandler EventHandler, Int32 e, Boolean ShowHandle = false)
            {
                this.Area = Area;
                this.Text = Text;
                this.EventHandler = EventHandler;
                this.e = (vt4EventArgs)e;
                this.ShowHandle = ShowHandle;
            }

            public Vt4MenuItemTemplate(Area Area, String Text, EventHandler EventHandler, Int32[] e, Boolean ShowHandle = false)
            {
                this.Area = Area;
                this.Text = Text;
                this.EventHandler = EventHandler;
                this.ShowHandle = ShowHandle;
                this.e = (vt4EventArgs)e;
            }
        }

        public class Vt4MenuItem : Grid
        {
            public Area Area { get; set; }               // The control that uses this menu
            public MouseButton MouseButton { get; set; }    // The mouse button the menu is for
            public Int32 MenuRow { get; set; }              // The menu row
            public Grid GridMenu { get; set; }              // The Grid it is based on
            public EventHandler EventHandler { get; set; }  // The final event that will be trigged
            public EventArgs e { get; set; }                // Any argument for generic use in handler
            public Boolean ShowHandle { get; set; }         // Shows a slider handle if needed
            public Boolean LeftButtonIsDown { get; set; }   // Indicates that pointer move should set value
            public PointerPoint pointerPoint { get; set; }  // Mouse position within menu image
            public TextBlock TextBlock { get; set; }       // The text on the button
            public TextBox tbMenuEditText { get; set; }     // User editable name box
            public String UserEditedName { get; set; }      // User edited variant name (used only for variation buttons)
            public Int32 Value { get; set; }                // Current value
            public object Control { get; set; }             // The control holding the value for this menu item 

            Image imgMenuBackground;
            public Image imgPointerEventArea;
            public Image imgMenuBackSliderHandle;
            public Int32 maxValue = 127;

            public Vt4MenuItem(Area Control, MouseButton MouseButton, Int32 MenuRow, Grid gridMain, String text, Double width,
                PointerEventHandler pointerEntered, PointerEventHandler pointerExited,
                PointerEventHandler pointerPressed, PointerEventHandler pointerReleased,
                PointerEventHandler pointerMoved, PointerEventHandler pointerWheelChanged,
                EventHandler EventHandler, EventArgs e, Boolean ShowHandle = false)
            {
                // Id is used in handlers to identify sender:
                this.Area = Control;

                // Button is the button that opens the menu. This is used for positioning:
                this.MouseButton = MouseButton;

                // MenuRow is the row within the menu. This is used for positioning:
                this.MenuRow = MenuRow;

                this.EventHandler = EventHandler;

                this.e = e;

                this.Value = ((vt4EventArgs)e).IntValue;    // Used if this i a mouse button 2 menu

                this.maxValue = ((vt4EventArgs)e).IntValue; // Used if this i a mouse button > 2 menu

                this.ShowHandle = ShowHandle;

                // Create grid:
                GridMenu = new Grid();
                GridMenu.Name = "gridMenu_" + Control.ToString() + "_" + MouseButton.ToString() + "_" + MenuRow.ToString();
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(1, GridUnitType.Star);
                GridMenu.RowDefinitions.Add(rd);
                rd = new RowDefinition();
                rd.Height = new GridLength(8, GridUnitType.Star);
                GridMenu.RowDefinitions.Add(rd);
                rd = new RowDefinition();
                rd.Height = new GridLength(1, GridUnitType.Star);
                GridMenu.RowDefinitions.Add(rd);
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                GridMenu.ColumnDefinitions.Add(cd);
                cd = new ColumnDefinition();
                cd.Width = new GridLength(15, GridUnitType.Star);
                GridMenu.ColumnDefinitions.Add(cd);
                cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                GridMenu.ColumnDefinitions.Add(cd);

                // Create image:
                imgMenuBackground = new Image();
                imgMenuBackground.Name = "imgMenuBackground_" + Control.ToString() + "_" + MouseButton.ToString() + "_" + MenuRow.ToString();
                imgMenuBackground.Source = new BitmapImage(new Uri("ms-appx:///Images/MenuBackground.png"));
                Grid.SetRowSpan(imgMenuBackground, 3);
                Grid.SetColumnSpan(imgMenuBackground, 3);
                Grid.SetRow(imgMenuBackground, 0);
                Grid.SetColumn(imgMenuBackground, 0);

                // Create slider handle:
                //Grid gridHandle = new Grid();
                if (ShowHandle)
                {
                    imgMenuBackSliderHandle = new Image();
                    imgMenuBackSliderHandle.Name = "imgMenuSliderHandle_" + Control.ToString() + "_" + MouseButton.ToString() + "_" + MenuRow.ToString();
                    imgMenuBackSliderHandle.Source = new BitmapImage(new Uri("ms-appx:///Images/MenuItemSliderHandle.png"));
                    imgMenuBackSliderHandle.Opacity = 1;
                    imgMenuBackSliderHandle.Visibility = Visibility.Visible;
                    Grid.SetRowSpan(imgMenuBackSliderHandle, 3);
                    Grid.SetColumnSpan(imgMenuBackSliderHandle, 3);
                    Grid.SetRow(imgMenuBackSliderHandle, 0);
                    Grid.SetColumn(imgMenuBackSliderHandle, 0);
                }

                // Create textblock:
                TextBlock = new TextBlock();
                TextBlock.Name = "TextBlock_" + Control.ToString() + "_" + MouseButton.ToString() + "_" + MenuRow.ToString();
                if (!String.IsNullOrEmpty(UserEditedName))
                {
                    TextBlock.Text = UserEditedName;

                }
                else
                {
                    TextBlock.Text = text;
                }
                TextBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 130, 210, 180));
                TextBlock.FontSize = width * 0.014;
                TextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                TextBlock.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRowSpan(TextBlock, 1);
                Grid.SetColumnSpan(TextBlock, 1);
                Grid.SetRow(TextBlock, 1);
                Grid.SetColumn(TextBlock, 1);

                // Create textbox:
                tbMenuEditText = new TextBox();
                tbMenuEditText.Name = "tbMenuEditText_" + Control.ToString() + "_" + MouseButton.ToString() + "_" + MenuRow.ToString();
                if (!String.IsNullOrEmpty(UserEditedName))
                {
                    tbMenuEditText.Text = UserEditedName;

                }
                else
                {
                    tbMenuEditText.Text = text;
                }
                tbMenuEditText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 130, 210, 180));
                tbMenuEditText.FontSize = width * 0.014;
                tbMenuEditText.HorizontalAlignment = HorizontalAlignment.Stretch;
                tbMenuEditText.VerticalAlignment = VerticalAlignment.Center;
                tbMenuEditText.Visibility = Visibility.Collapsed;
                tbMenuEditText.KeyDown += PopupMenu_KeyDown;
                Grid.SetRowSpan(tbMenuEditText, 1);
                Grid.SetColumnSpan(tbMenuEditText, 1);
                Grid.SetRow(tbMenuEditText, 1);
                Grid.SetColumn(tbMenuEditText, 1);

                // Create clickArea:
                imgPointerEventArea = new Image();
                imgPointerEventArea.Source = new BitmapImage(new Uri("ms-appx:///Images/MenuBackground.png"));
                imgPointerEventArea.Opacity = 0;
                Grid.SetRowSpan(imgPointerEventArea, 3);
                Grid.SetColumnSpan(imgPointerEventArea, 3);
                Grid.SetRow(imgPointerEventArea, 0);
                Grid.SetColumn(imgPointerEventArea, 0);

                // Tag clickArea:
                imgPointerEventArea.Tag = this;

                // Add handlers:
                imgPointerEventArea.PointerEntered += pointerEntered;
                imgPointerEventArea.PointerExited += pointerExited;
                imgPointerEventArea.PointerPressed += pointerPressed;
                imgPointerEventArea.PointerReleased += pointerReleased;
                imgPointerEventArea.PointerWheelChanged += pointerWheelChanged;
                imgPointerEventArea.PointerMoved += pointerMoved;

                // Assemble popup menu:
                GridMenu.Children.Add(imgMenuBackground);
                if (ShowHandle)
                {
                    GridMenu.Children.Add(imgMenuBackSliderHandle);
                }
                GridMenu.Children.Add(TextBlock);
                GridMenu.Children.Add(tbMenuEditText);
                GridMenu.Children.Add(imgPointerEventArea);
                gridMain.Children.Add(GridMenu);

                // Initially hide the menu:
                GridMenu.Visibility = Visibility.Collapsed;
            }

            public void PopupMenu_KeyDown(object sender, KeyRoutedEventArgs e)
            {
                if (e.Key == Windows.System.VirtualKey.Escape)
                {
                    tbMenuEditText.Visibility = Visibility.Collapsed;
                    TextBlock.Visibility = Visibility.Visible;
                }
                else if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    MainPage mainPage = (MainPage)((Grid)((Grid)((TextBox)sender).Parent).Parent).Parent;
                    mainPage.vt4MenuItems[mainPage.AreaToMenuNumber(Area)][0][MenuRow].TextBlock.Text = tbMenuEditText.Text;
                    UserEditedName = tbMenuEditText.Text;
                    TextBlock.Text = UserEditedName;
                    switch (Area)
                    {
                        case Area.ROBOT:
                            if (MenuRow == mainPage.VT4.TemporaryPatch.ROBOT_VARIATION)
                            {
                                mainPage.pmbRobot.TextBlock.Text = UserEditedName;
                            }
                            break;
                        case Area.MEGAPHONE:
                            if (MenuRow == mainPage.VT4.TemporaryPatch.MEGAPHONE_VARIATION)
                            {
                                mainPage.pmbMegaphone.TextBlock.Text = UserEditedName;
                            }
                            break;
                        case Area.VOCODER:
                            if (MenuRow == mainPage.VT4.TemporaryPatch.VOCODER_VARIATION)
                            {
                                mainPage.pmbVocoder.TextBlock.Text = UserEditedName;
                            }
                            break;
                        case Area.HARMONY:
                            if (MenuRow == mainPage.VT4.TemporaryPatch.HARMONY_VARIATION)
                            {
                                mainPage.pmbHarmony.TextBlock.Text = UserEditedName;
                            }
                            break;
                        case Area.REVERB:
                            if (MenuRow == mainPage.VT4.TemporaryPatch.REVERB_VARIATION)
                            {
                                mainPage.pmbReverb.TextBlock.Text = UserEditedName;
                            }
                            break;
                    }
                    tbMenuEditText.Visibility = Visibility.Collapsed;
                    TextBlock.Visibility = Visibility.Visible;
                    mainPage.settings.Save();
                    mainPage.CloseAllSceneMenus();
                }
                else
                {
                    UserEditedName = tbMenuEditText.Text;
                    TextBlock.Text = UserEditedName;
                }
            }

            // HBE better use this? public void Show()
            //{
            //    GridMenu.Visibility = Visibility.Visible;
            //    if (ShowHandle)
            //    {
            //        SetSliderPosition();
            //    }
            //}

            public void LowLite()
            {
                TextBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 130, 210, 180));
            }

            public void HighLite()
            {
                TextBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 160, 255, 240));
            }

            public void SetFontSize(Double width)
            {
                TextBlock.FontSize = width * 0.014;
            }

            public Int32 GetSliderPosition(PointerRoutedEventArgs e)
            {
                PointerPoint position = e.GetCurrentPoint(this.imgMenuBackground);
                // HBE double menuWidth = 171.0 * MainPage.AppWidth / 1126.0;
                // HBE Int32 value = (Int32)(position.Position.X * 296 / menuWidth);
                // HBE value = value > maxValue ? maxValue : value;
                // HBE SetSliderPosition();
                // HBE return value;
                return 0; // HBE 
            }

            public void SetSliderPosition()
            {
                double menuWidth = imgMenuBackground.ActualWidth;
                // HBE double handleWidth = 34 * MainPage.AppWidth / 1126.0;
                // HBE double left = ((double)Value / (double)maxValue) * (menuWidth - handleWidth);
                // HBE double right = menuWidth - left - handleWidth;
                // HBE imgMenuBackSliderHandle.Margin = new Thickness(left, 0, right, 0);
            }

            public Int32 GetWeightedSliderPosition(PointerRoutedEventArgs e, double maximum)
            {
                PointerPoint position = e.GetCurrentPoint(this.imgMenuBackground);
                // HBE double menuWidth = 171.0 * MainPage.AppWidth / 1126.0;
                // HBE Int32 value = (Int32)(position.Position.X * 296 / menuWidth);
                // HBE value = (int)(value * maximum / 256.0);
                // HBE value = value > maxValue ? maxValue : value;
                // HBE SetWeightedSliderPosition(maximum);
                // HBE return value;
                return 0; // HBE
            }

            public void SetWeightedSliderPosition(double maximum)
            {
                double menuWidth = imgMenuBackground.ActualWidth;
                // HBE double handleWidth = 34 * MainPage.AppWidth / 1126.0;
                // HBE double left = ((double)Value * (maximum / 256.0) / (double)maxValue) * (menuWidth - handleWidth);
                // HBE double right = menuWidth - left - handleWidth;
                // HBE imgMenuBackSliderHandle.Margin = new Thickness(left, 0, right, 0);
            }
        }
    }
}
