using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VT_4.MainPage;
using UwpControlsLibrary;
using Windows.UI.Xaml.Controls;

namespace VT_4
{
    public sealed partial class MainPage : Page
    {
        #region Header
        /// <summary>
        /// The VT-4 always works in temporary patch and user effect areas.
        /// Selecting a Patch involves copying from one of the patches 1 - 8 to the temporary patch.
        /// Selecting a Variation involves copying the user effect area to the temporary effect area.
        /// Updating a parameter involves changing it in both temporary area and the current effect area.
        /// If control is an effect, patch is updated.
        /// If control is a parameter, user effect is updated.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="data"></param>

        public async void HandleControlEvent(ControlBase control)
        {
            byte offset;
            byte baseAddress;
            byte parameterAddress;
            byte[] temporaryEffectAddress;
            byte[] userEffectAddress;
            byte[] temporaryPatchAddress;
            byte[] userPatchAddress;
            byte[] data;
            byte[] message;
            byte[] idToCcNumberMap = new byte[] { 46, 47, 48, 55, 0, 54, 56, 57, 6, 5, 4, 7, 8 };
            byte[] idToEffectOnOffMap = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 0, 3 };
            byte[] patchId = new byte[] { 0x60, 0x30, 0x20, 0x40, 0x50, 0x62 };
            byte[] effectParameterOffset = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1 };
            byte[] menuItemToEqualizerParameter = new byte[]
            {
                VT4.TemporaryEqualizer.EQUALIZER_SWITCH,
                VT4.TemporaryEqualizer.EQUALIZER_LOW_SHELF_FREQUENCY,
                VT4.TemporaryEqualizer.EQUALIZER_LOW_SHELF_GAIN,
                VT4.TemporaryEqualizer.EQUALIZER_LOW_MID_FREQUENCY,
                VT4.TemporaryEqualizer.EQUALIZER_LOW_MID_Q,
                VT4.TemporaryEqualizer.EQUALIZER_LOW_MID_GAIN,
                VT4.TemporaryEqualizer.EQUALIZER_HIGH_MID_FREQUENCY,
                VT4.TemporaryEqualizer.EQUALIZER_HIGH_MID_Q,
                VT4.TemporaryEqualizer.EQUALIZER_HIGH_MID_GAIN,
                VT4.TemporaryEqualizer.EQUALIZER_HIGH_SHELF_FREQUENCY,
                VT4.TemporaryEqualizer.EQUALIZER_HIGH_SHELF_GAIN
            };
            #endregion Header
            //////////////////////////////////////////////////////////////////////////////////
            /// Knobs
            //////////////////////////////////////////////////////////////////////////////////
            #region Knobs
            if (control.GetType() == typeof(Knob))
            {
                midi.SendControlChange(midi.PortPairs[0],
                    idToCcNumberMap[control.Id], (byte)((Knob)control).Value);
                if (control.Id > 1 && sceneIndex > -1)
                {
                    sceneEdited[sceneIndex] = true;
                }
            }
            #endregion Knobs
            //////////////////////////////////////////////////////////////////////////////////
            /// Sliders
            //////////////////////////////////////////////////////////////////////////////////
            #region Sliders
            else if (control.GetType() == typeof(VerticalSlider))
            {
                switch (control.Id)
                {
                    case 4:
                        midi.SendPitchBender(midi.PortPairs[0],
                            ((VerticalSlider)control).Value * 128);
                        VT4.TemporaryPatch.PITCH = ((VerticalSlider)control).Value * 2;
                        break;
                    case 5:
                        midi.SendControlChange(midi.PortPairs[0],
                            idToCcNumberMap[control.Id], (byte)((VerticalSlider)control).Value);
                        VT4.TemporaryPatch.FORMANT = ((VerticalSlider)control).Value * 2;
                        break;
                    case 6:
                        midi.SendControlChange(midi.PortPairs[0],
                            idToCcNumberMap[control.Id], (byte)((VerticalSlider)control).Value);
                        VT4.TemporaryPatch.BALANCE = ((VerticalSlider)control).Value * 2;
                        break;
                    case 7:
                        midi.SendControlChange(midi.PortPairs[0],
                            idToCcNumberMap[control.Id], (byte)((VerticalSlider)control).Value);
                        VT4.TemporaryPatch.REVERB = ((VerticalSlider)control).Value * 2;
                        pmbReverb.Set(((VerticalSlider)control).Value > 0);
                        break;
                }

                if (sceneIndex > -1)
                {
                    sceneEdited[sceneIndex] = true;
                }
            }
            #endregion Sliders
            //////////////////////////////////////////////////////////////////////////////////
            /// ImageButtons
            //////////////////////////////////////////////////////////////////////////////////
            #region ImageButtons
            else if ((control.GetType() == typeof(ImageButton)))
            {
                if (control.Id == 22)
                {
                    // Bypass clicked:
                    midi.SendControlChange(midi.PortPairs[0], 51, (byte)(((ImageButton)control).IsOn ? 127 : 0));
                }
                if (control.Id == 24)
                {
                    await ShowManual();
                }
            }
            #endregion ImageButtons
            //////////////////////////////////////////////////////////////////////////////////
            /// PopupMenuButtons
            //////////////////////////////////////////////////////////////////////////////////
            #region PopupMenuButtons
            else if ((control.GetType() == typeof(PopupMenuButton)))
            {
                /// Effect menus /////////////////////////////////////////////////////////////
                #region Effects
                if (control.Id >= 8 && control.Id <= 12)
                {
                    /// Top level button -----------------------------------------------------
                    if (((PopupMenuButton)control).MenuNumber == -1)
                    {
                        // Menu button pressed (i.e. not a menu item):
                        if (Controls.PointerButtonStates.Contains(ControlBase.PointerButton.LEFT))
                        {
                            // Send on/off for the clicked effects unless it is Reverb:
                            if (control.Id >= 8 && control.Id <= 11)
                            {
                                midi.SendControlChange(midi.PortPairs[0],
                                    ControlToCcNumber(((PopupMenuButton)control)),
                                    (byte)(((PopupMenuButton)control).IsOn ? 127 : 0));

                                if (sceneIndex > -1)
                                {
                                    sceneEdited[sceneIndex] = true;
                                }
                            }
                            else if (control.Id == 12)
                            {
                                // Send reverb amount from slider or 0:
                                midi.SendControlChange(midi.PortPairs[0], 57,
                                    (byte)(((PopupMenuButton)control).IsOn ?
                                        (byte)(slReverb.Value) : 0));
                            }

                            // Update VT4:
                            switch (control.Id)
                            {
                                case 8:
                                    VT4.TemporaryPatch.VOCODER =
                                        (byte)(((PopupMenuButton)control).IsOn ? 1 : 0);
                                    break;
                                case 9:
                                    VT4.TemporaryPatch.HARMONY =
                                        (byte)(((PopupMenuButton)control).IsOn ? 1 : 0);
                                    break;
                                case 10:
                                    VT4.TemporaryPatch.ROBOT =
                                        (byte)(((PopupMenuButton)control).IsOn ? 1 : 0);
                                    break;
                                case 11:
                                    VT4.TemporaryPatch.MEGAPHONE =
                                        (byte)(((PopupMenuButton)control).IsOn ? 1 : 0);
                                    break;
                                case 12:
                                    VT4.TemporaryPatch.REVERB =
                                        (byte)(((PopupMenuButton)control).IsOn ? slReverb.Value * 2 : 0);
                                    break;
                            }

                        }
                    }
                    /// Variation menu ----------------------------------------------------------
                    else if (((PopupMenuButton)control).MenuNumber == 0)
                    {
                        // Handle first popup menu items:
                        if (((PopupMenuButton)control).MenuNumber == 0)
                        {
                            data = new byte[] { (byte)(((PopupMenuButton)control).MenuItemNumber) };
                            VariationIndex = data[0];

                            // We need an offset not to have duplicate cases in ControlToCcNumber, chosen is 4.
                            byte cc = ControlToCcNumber(((PopupMenuButton)control), 4);
                            message = new byte[] { 0xb0, cc, data[0] };
                            midi.SendControlChange(midi.PortPairs[0],
                                ControlToCcNumber(((PopupMenuButton)control), 4), data[0]);

                            // The VT-4 unit has now copied the user areas to 
                            // temporary areas. Read them into VT4. 
                            // HBE ReadTemporaryPatch();
                            ReadTemporaryRobot();
                            ReadTemporaryHarmony();
                            ReadTemporaryMegaphone();
                            ReadTemporaryReverb();
                            ReadTemporaryVocoder();
                            // Robot parameters are not copied (bug in VT-4 firmware), so copy them here:
                            VT4.TemporaryRobot.OCTAVE = VT4.UserRobot[VariationIndex].OCTAVE;
                            VT4.TemporaryRobot.FEEDBACK_SWITCH = VT4.UserRobot[VariationIndex].FEEDBACK_SWITCH;
                            VT4.TemporaryRobot.FEEDBACK_RESONANCE = VT4.UserRobot[VariationIndex].FEEDBACK_RESONANCE;
                            VT4.TemporaryRobot.FEEDBACK_LEVEL = VT4.UserRobot[VariationIndex].FEEDBACK_LEVEL;
                            VT4.TemporaryRobot.NAME_00_03 = VT4.UserRobot[data[0]].NAME_00_03;
                            VT4.TemporaryRobot.NAME_04_07 = VT4.UserRobot[data[0]].NAME_04_07;
                            VT4.TemporaryRobot.Name = VT4.UserRobot[data[0]].Name;

                            // The VT-4 unit has copied the effect settings to the 
                            // temporary pach. Read them into VT4:
                            ReadTemporaryPatch();

                            // Update item and label names and indicate selected item:
                            SetVariationNames();
                            SetControlValues();
                            SetParameterTexts(control.Id);

                            if (sceneIndex > -1)
                            {
                                sceneEdited[sceneIndex] = true;
                            }
                        }
                    }
                    /// Parameters menu ----------------------------------------------------------
                    // Handle second popup menu items:
                    else if (((PopupMenuButton)control).MenuNumber == 1)
                    {
                        baseAddress = (byte)(parms.ParameterList[control.Id]).Id;
                        // Right button menu is used to set variations which all
                        // starts in Patch at address 5, but are not in the same
                        // order as in this app. Use variation array to obtain address.

                        // Some controls has variation as first parameter. 
                        // We handle those variations in the right button menu.
                        // Set an offset to skip that parameter for the other button menu, and
                        // some parameters use 2 bytes. Calculate position of selected parameter:
                        int i = 0;
                        parameterAddress = 0;
                        while (i <= ((PopupMenuButton)control).MenuItemNumber)
                        {
                            if (parms.ParameterList[control.Id].Parameters[i].GetType() == typeof(int))
                            {
                                if ((int)parms.ParameterList[control.Id].Parameters[i] > 127)
                                {
                                    parameterAddress += 1;
                                }
                            }
                            parameterAddress += 1;
                            i++;
                        }

                        // Parameter values over 127 takes two 'nibble bytes', else one byte.
                        if (parms.ParameterList[control.Id].
                            Parameters[((PopupMenuButton)control).
                            MenuItemNumber].GetType() == typeof(int))
                        {
                            // Numeric variable:
                            if ((int)parms.ParameterList[control.Id].
                                Parameters[((PopupMenuButton)control).
                                MenuItemNumber] > 127)
                            {
                                data = new byte[] { (byte)(((PopupMenuButton)control).Value / 16),
                                    (byte)(((PopupMenuButton)control).Value % 16) };
                            }
                            else
                            {
                                data = new byte[] { (byte)(((PopupMenuButton)control).Value) };
                            }
                        }
                        else
                        {
                            // String list variable, numeric value is 0 - number of strings - 1:
                            data = new byte[] { (byte)(((PopupMenuButton)control).Value) };
                        }

                        // Assemble address:
                        temporaryEffectAddress = new byte[] { baseAddress,
                            0x00, 0x00, (byte)(((PopupMenuButton)control).
                            MenuItemNumber) };

                        // Send to temporary effect:
                        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], temporaryEffectAddress, data.ToArray());
                        midi.SendSystemExclusive(midi.PortPairs[0], message);

                        // Store in VT4:
                        switch (control.Id)
                        {
                            case 0x08:
                                switch (((PopupMenuButton)control).MenuItemNumber)
                                {
                                    case 0:
                                        VT4.TemporaryVocoder.VOCODER_PARAMETER_1 = ((PopupMenuButton)control).Value;
                                        break;
                                    case 1:
                                        VT4.TemporaryVocoder.VOCODER_PARAMETER_2 = ((PopupMenuButton)control).Value;
                                        break;
                                    case 2:
                                        VT4.TemporaryVocoder.VOCODER_PARAMETER_3 = ((PopupMenuButton)control).Value;
                                        break;
                                    case 3:
                                        VT4.TemporaryVocoder.VOCODER_PARAMETER_4 = ((PopupMenuButton)control).Value;
                                        break;
                                }
                                break;
                            case 0x09:
                                switch (((PopupMenuButton)control).MenuItemNumber)
                                {
                                    case 0:
                                        VT4.TemporaryHarmony.HARMONY_1_LEVEL = ((PopupMenuButton)control).Value;
                                        break;
                                    case 1:
                                        VT4.TemporaryHarmony.HARMONY_2_LEVEL = ((PopupMenuButton)control).Value;
                                        break;
                                    case 2:
                                        VT4.TemporaryHarmony.HARMONY_3_LEVEL = ((PopupMenuButton)control).Value;
                                        break;
                                    case 3:
                                        VT4.TemporaryHarmony.HARMONY_1_KEY = (byte)((PopupMenuButton)control).Value;
                                        break;
                                    case 4:
                                        VT4.TemporaryHarmony.HARMONY_2_KEY = (byte)((PopupMenuButton)control).Value;
                                        break;
                                    case 5:
                                        VT4.TemporaryHarmony.HARMONY_3_KEY = (byte)((PopupMenuButton)control).Value;
                                        break;
                                    case 6:
                                        VT4.TemporaryHarmony.HARMONY_1_GENDER = ((PopupMenuButton)control).Value;
                                        break;
                                    case 7:
                                        VT4.TemporaryHarmony.HARMONY_2_GENDER = ((PopupMenuButton)control).Value;
                                        break;
                                    case 8:
                                        VT4.TemporaryHarmony.HARMONY_3_GENDER = ((PopupMenuButton)control).Value;
                                        break;
                                }
                                break;
                            case 0x0a:
                                switch (((PopupMenuButton)control).MenuItemNumber)
                                {
                                    case 0:
                                        VT4.TemporaryRobot.OCTAVE = (byte)((PopupMenuButton)control).Value;
                                        break;
                                    case 1:
                                        VT4.TemporaryRobot.FEEDBACK_SWITCH = (byte)((PopupMenuButton)control).Value;
                                        break;
                                    case 2:
                                        VT4.TemporaryRobot.FEEDBACK_RESONANCE = ((PopupMenuButton)control).Value;
                                        break;
                                    case 3:
                                        VT4.TemporaryRobot.FEEDBACK_LEVEL = ((PopupMenuButton)control).Value;
                                        break;
                                }
                                break;
                            case 0x0b:
                                switch (((PopupMenuButton)control).MenuItemNumber)
                                {
                                    case 0:
                                        VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_1 = ((PopupMenuButton)control).Value;
                                        break;
                                    case 1:
                                        VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_2 = ((PopupMenuButton)control).Value;
                                        break;
                                    case 2:
                                        VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_3 = ((PopupMenuButton)control).Value;
                                        break;
                                    case 3:
                                        VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_4 = ((PopupMenuButton)control).Value;
                                        break;
                                }
                                break;
                            case 0x0c:
                                switch (((PopupMenuButton)control).MenuItemNumber)
                                {
                                    case 0:
                                        VT4.TemporaryReverb.REVERB_PARAMETER_1 = ((PopupMenuButton)control).Value;
                                        break;
                                    case 1:
                                        VT4.TemporaryReverb.REVERB_PARAMETER_2 = ((PopupMenuButton)control).Value;
                                        break;
                                    case 2:
                                        VT4.TemporaryReverb.REVERB_PARAMETER_3 = ((PopupMenuButton)control).Value;
                                        break;
                                    case 3:
                                        VT4.TemporaryReverb.REVERB_PARAMETER_4 = ((PopupMenuButton)control).Value;
                                        break;
                                }
                                break;
                        }

                        // Calculate SysEx address offset from menu item number:
                        if (parms.ParameterList[((PopupMenuButton)control).Id].
                            Parameters[((PopupMenuButton)control).MenuItemNumber].GetType() == typeof(int))
                        {
                            try
                            {
                                if (control.TextBlock.Text.LastIndexOf(' ') > -1)
                                {
                                    control.TextBlock.Text =
                                            control.TextBlock.Text.Remove(control.TextBlock.Text.LastIndexOf(' ') + 1) +
                                            ((PopupMenuButton)control).Value.ToString();
                                }
                                else
                                {
                                    control.TextBlock.Text =
                                            control.TextBlock.Text + " " +
                                            ((PopupMenuButton)control).Value.ToString();
                                }
                            }
                            catch { }
                        }

                        if (sceneIndex > -1)
                        {
                            sceneEdited[sceneIndex] = true;
                        }
                    }
                    else
                    {
                        if (sceneIndex > -1)
                        {
                            sceneEdited[sceneIndex] = true;
                        }
                    }
                }
                #endregion Effects
                /// Scene button menus /////////////////////////////////////////////////////////////
                #region Scene buttons
                if (control.Id >= 13 && control.Id <= 21)
                {
                    // Select scene button ----------------------------------------------------------
                    if (((PopupMenuButton)control).MenuNumber == -1)
                    {
                        /// Number buttons and MANUAL acts as radio buttons:
                        if (Controls.PointerButtonStates.Contains(ControlBase.PointerButton.LEFT))
                        {
                            CloseAllPopupMenus();

                            // Turn off all number buttons and the manual button:
                            for (int i = 0; i < 9; i++)
                            {
                                ((PopupMenuButton)Controls.ControlsList[topLevelMenuLocations[i + 5]]).Set(false);
                                if (i > 0)
                                {
                                    // And set the scene to that it has not been edited:
                                    sceneEdited[i - 1] = false;
                                }
                            }

                            // Turn on the selected scene or the manual button:
                            if (control == pmbManual)
                            {
                                sceneIndex = -1;
                                pmbManual.Set(true);
                            }
                            else
                            {
                                sceneIndex = control.Id > 17 ? control.Id - 18 : control.Id - 10;
                                ((PopupMenuButton)Controls.ControlsList[topLevelMenuLocations[control.Id - 8]]).Set(true);
                            }

                            // Send to VT-4
                            midi.SendProgramChange(midi.PortPairs[0], (byte)(sceneIndex + 1));
                            ReadTemporaryPatch();
                            ReadTemporaryVocoder();
                            ReadTemporaryHarmony();
                            ReadTemporaryRobot();
                            ReadTemporaryMegaphone();
                            ReadTemporaryReverb();
                            if (sceneIndex == 0)
                            {
                                manualOn = false;
                            }

                            // The VT-4 unit fails to read out some parameter values,
                            // so we copy them between user areas and temporary areas
                            // in VT4 here:
                            CopyUserToTemporary();
                            SetControlValues();
                            UpdateGui();
                        }
                    }
                    // Save scene button ------------------------------------------------------------
                    else if (Controls.PointerButtonStates.Contains(ControlBase.PointerButton.LEFT))
                    {
                        if (((PopupMenuButton)control).MenuNumber == 0)
                        {
                            // Menus and items are stored from a top level position
                            // in Controls.ControlsList in the following order:
                            // Top level, Save, Copy to..., ...Scene 1, ...Scene 2 etc. to ...Scene 8
                            // But, top level buttons comes in order 5, 6, 7, 8, 1, 2, 3, 4 just not 
                            // to have 5 - 8 menus cover 1 - 4 menus.

                            if (((PopupMenuButton)control).Parent.MenuNumber == -1
                                && ((PopupMenuButton)control).MenuNumber == 0
                                && ((PopupMenuButton)control).MenuItemNumber == 0)
                            {
                                // Save scene:
                                int scene = control.Id > 17 ? control.Id - 18 : control.Id - 10;
                                SaveScene(scene);
                                CloseAllSceneMenus();
                                sceneEdited[scene] = false;
                            }
                            else if ((((PopupMenuButton)control).Parent.MenuNumber == 0
                                && ((PopupMenuButton)control).Parent.MenuItemNumber == 1)
                                && ((PopupMenuButton)control).MenuNumber == 0)
                            {
                                // Copy scene:
                                int to = ((PopupMenuButton)control).MenuItemNumber;
                                int from = control.Id > 17 ? control.Id - 18 : control.Id - 10;
                                CopyScene(from, to);
                                CloseAllSceneMenus();
                            }
                        }
                    }
                }
                #endregion Scene buttons
                /// Roland logo button //////////////////////////////////////////////////////////////
                #region Roland logo button
                else if (control.Id == 23)
                {
                    // There is only one top level menu, the Roland logo.
                    if (((PopupMenuButton)control).Parent != null
                        && ((PopupMenuButton)control).Parent.Parent != null)
                    {
                        if (((PopupMenuButton)control).Parent == pmbMidiChannel)
                        {
                            foreach (PopupMenuButton pmb in pmbRoland.Children[0][0].Children[0])
                            {
                                if (pmb.MenuItemNumber < 16)
                                {
                                    pmb.Set(false);
                                }
                            }
                            if ((byte)((PopupMenuButton)control).MenuItemNumber < 16)
                            {
                                // MIDI channel selection
                                VT4.System.MIDI_CH = (byte)((PopupMenuButton)control).MenuItemNumber;
                                WriteSystem();
                                midi.PortPairs[0].OutChannel = (byte)((PopupMenuButton)control).MenuItemNumber;
                                midi.PortPairs[0].InChannel = (byte)((PopupMenuButton)control).MenuItemNumber;
                                pmbMidiChannels[(byte)((PopupMenuButton)control).MenuItemNumber].Set(true);
                            }
                            else
                            {
                                // Toggle Omni on/off:
                                if (((PopupMenuButton)control).IsOn)
                                {
                                    data = new byte[] { 0x11 };
                                }
                                else
                                {
                                    data = new byte[] { midi.PortPairs[0].OutChannel };
                                    pmbRoland.Children[0][0].Children[0][midi.PortPairs[0].OutChannel].Set(true);
                                }
                                byte[] address = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                                message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data);
                                midi.SendSystemExclusive(midi.PortPairs[0], message);
                            }
                        }
                        else if (((PopupMenuButton)control).Parent == pmbEqualizer)
                        {
                            byte parameter = (byte)((PopupMenuButton)control).MenuItemNumber;
                            byte[] address = new byte[] { 0x62, 0x00, 0x00, parameter };
                            if (parameter == 0)
                            {
                                data = new byte[] { (byte)(((PopupMenuButton)control).IsOn ? 1 : 0) };
                            }
                            else
                            {
                                data = new byte[] { (byte)((PopupMenuButton)control).Value };
                            }
                            message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data);
                            midi.SendSystemExclusive(midi.PortPairs[0], message);
                            SetEqualizerParameterTexts();
                        }
                        else if (((PopupMenuButton)control).Parent == pmbLevels)
                        {
                            byte value = (byte)((PopupMenuButton)control).Value;
                            switch (((PopupMenuButton)control).MenuItemNumber)
                            {
                                case 0:
                                    VT4.System.GATE_LEVEL = value;
                                    break;
                                case 1:
                                    VT4.System.LOW_CUT = value;
                                    break;
                                case 2:
                                    VT4.System.ENHANCER = value;
                                    break;
                                case 3:
                                    VT4.System.FORMANT_DEPTH = value;
                                    break;
                                case 4:
                                    VT4.System.USB_MIXING = value;
                                    break;
                            }
                            WriteSystem();
                            SetSystemParameterTexts();
                        }
                        else if (((PopupMenuButton)control).Parent == pmbSwitches)
                        {
                            byte value = (byte)(((PopupMenuButton)control).IsOn ? 1 : 0);
                            switch (((PopupMenuButton)control).MenuItemNumber)
                            {
                                case 0:
                                    VT4.System.MONITOR_MODE = value;
                                    break;
                                case 1:
                                    VT4.System.EXTERNAL_CARRIER = value;
                                    break;
                                case 2:
                                    VT4.System.MIDI_IN_MODE = value;
                                    break;
                                case 3:
                                    VT4.System.PITCH_AND_FORMANT_ROUTING = value;
                                    break;
                                case 4:
                                    VT4.System.MUTE_MODE = value;
                                    break;
                            }
                            WriteSystem();
                            SetSystemParameterTexts();
                        }
                    }
                    else if (((PopupMenuButton)control).Parent != null)
                    {

                        if (((PopupMenuButton)control) == pmbSave)
                        {
                            await WriteJsonFile();
                        }
                        else if (((PopupMenuButton)control) == pmbLoad)
                        {
                            await ReadJsonFile();
                        }
                    }
                    #endregion Roland logo button
                }
                #endregion PopupMenuButtons
            }
        }
    }
}
