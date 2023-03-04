using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UwpControlsLibrary;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using static VT_4.MainPage;

namespace VT_4
{
    public sealed partial class MainPage : Page
    {
        private async void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (initDone && Controls != null)
            {
                //if (args.VirtualKey == Windows.System.VirtualKey.Enter)
                //{
                //    if (currentPopupMenuButton != null)
                //    {
                //        settings.Save();
                //    }
                //}
                //else
                if (args.VirtualKey == Windows.System.VirtualKey.F1)
                {
                    await ShowManual();
                }
            }
        }

        // When the pointer is moved over the click-area, ask the Controls
        // object if, and if so which control the pointer is over:
        private void imgClickArea_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (initDone)
            {
                currentControl = (ControlBase)Controls.PointerMoved(sender, e);
                if (currentControl != null)
                {
                    currentArea = ((Area)((ControlBase)currentControl).Id);
                    if (currentControl.GetType() == typeof(PopupMenuButton)
                        && ((PopupMenuButton)currentControl).Visibility == Visibility.Visible)
                    {
                        SetVariationNames();
                        if (Controls.PointerButtonStates.Contains(ControlBase.PointerButton.LEFT)) // HBE is this the right way to do it?
                            HandleControlEvent((PopupMenuButton)currentControl);
                    }
                    else if (Controls.PointerButtonStates.Contains(ControlBase.PointerButton.LEFT))
                    {
                        HandleControlEvent((ControlBase)currentControl);
                    }
                }
            }
        }

        private void imgClickArea_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (initDone)
            {
                currentControl = (ControlBase)Controls.PointerPressed(sender, e);
                if (currentControl != null)
                {
                    currentArea = ((Area)((ControlBase)currentControl).Id);
                    if (currentControl.GetType() == typeof(PopupMenuButton))
                    {
                        SetParameterTexts(((ControlBase)currentControl).Id);
                        HandleControlEvent((PopupMenuButton)currentControl);
                    }
                    if (currentControl.GetType() == typeof(ImageButton))
                    {
                        HandleControlEvent((ImageButton)currentControl);
                    }
                }
                else
                {
                    CloseAllPopupMenus();
                }
            }
        }

        private void imgClickArea_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (initDone)
            {
                Controls.PointerReleased(sender, e);
            }
        }

        private void imgClickArea_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (initDone && currentControl != null)
            {
                Controls.PointerWheelChanged(sender, e);
                if (currentControl.GetType() == typeof(PopupMenuButton))
                {
                    SetParameterTexts(((ControlBase)currentControl).Id);
                    HandleControlEvent((PopupMenuButton)currentControl);
                }
                else if (currentControl.GetType() == typeof(Knob))
                {
                    HandleControlEvent((Knob)currentControl);
                }
                else if (currentControl.GetType() == typeof(VerticalSlider))
                {
                    HandleControlEvent((VerticalSlider)currentControl);
                }
            }
        }

        // When app size is changed, all controls must also be resized:
        private void gridControls_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (initDone)
            {
                Controls.ResizeControls(gridMain, Window.Current.Bounds);
            }
        }

        private void UpdateGui()
        {
            pmbManual.Set(sceneIndex == -1);
            pmb1.Set(sceneIndex == 0);
            pmb2.Set(sceneIndex == 1);
            pmb3.Set(sceneIndex == 2);
            pmb4.Set(sceneIndex == 3);
            pmb5.Set(sceneIndex == 4);
            pmb6.Set(sceneIndex == 5);
            pmb7.Set(sceneIndex == 6);
            pmb8.Set(sceneIndex == 7);

            pmbRobot.Set(VT4.TemporaryPatch.ROBOT > 0);
            pmbHarmony.Set(VT4.TemporaryPatch.HARMONY > 0);
            pmbVocoder.Set(VT4.TemporaryPatch.VOCODER > 0);
            pmbMegaphone.Set(VT4.TemporaryPatch.MEGAPHONE > 0);

            knobVolume.Value = VT4.TemporaryPatch.GLOBAL_LEVEL;
            knobAutoPitch.Value = VT4.TemporaryPatch.AUTO_PITCH / 2;

            if (settings.LocalSettings.Values["Mic sens"] != null)
            {
                knobMicSens.Value = (byte)settings.LocalSettings.Values["Mic sens"];
            }
            else
            {
                knobMicSens.Value = 63;
                settings.LocalSettings.Values["Mic sense"] = 0x63;
            }

            slPitch.Value = VT4.TemporaryPatch.PITCH / 2;
            slFormant.Value = VT4.TemporaryPatch.FORMANT / 2;
            slBalance.Value = VT4.TemporaryPatch.BALANCE / 2;
            slReverb.Value = VT4.TemporaryPatch.REVERB / 2;
            pmbReverb.Set(VT4.TemporaryPatch.REVERB > 0);

            UpdatePanelLabels();
        }

        private byte ControlToCcNumber(PopupMenuButton control, int offset = 0)
        {
            switch (control.Id + offset)
            {
                case 0: return 46; // VOLUME
                case 1: return 47; // MIC SENS
                case 2: return 48; // KEY
                case 3: return 55; // AUTO PITCH
                case 5: return 54; // FORMANT
                case 6: return 56; // BALANCE
                case 7: return 57; // REVERB
                case 8: return 52; // VOCODER
                case 9: return 53; // HARMONY
                case 10: return 49; // ROBOT
                case 11: return 50; // MEGAPHONE
                //case : return 51; // BYPASS
                //case : return 58; // LINE OUT SELECT
                //case : return 76; // MODULATION RATE
                case 14: return 79; // ROBOT VARIATION
                case 15: return 80; // MEGAPHONE VARIATION
                case 12: return 81; // VOCODER VARIATION
                case 13: return 82; // HARMONY VARIATION
                case 16: return 83; // REVERB VARIATION
            }
            return 255;
        }
    }
}
