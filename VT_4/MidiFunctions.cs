using ClassLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using UwpControlsLibrary;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace VT_4
{
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Read and write methods transfers data between VT-4 and class VT4 instance VT4.
        /// Changes, done in GUI interface or via incoming MIDI messages, are stored in VT4.
        /// Changes done in GUI interface are also stored in VT-4 temporary areas.
        /// 
        /// Variation user areas are only 4, but all effects can have up to 8 different
        /// variations. We only store the 4 first variations in VT-4, but VT4 contains
        /// all 8 variations for each effect. Now the user can call up all 8 variations
        /// from VT4 and we do not need to fetch from VT-4.
        /// </summary>
        /// 
        public enum Area
        {
            // Knobs
            VOLUME,
            MIC_SENS,
            KEY,
            AUTO_PITCH,

            // Sliders
            PITCH,
            FORMANT,
            BALANCE,
            REVERB_SLIDER,

            // Effect buttons
            VOCODER,
            HARMONY,
            ROBOT,
            MEGAPHONE,
            REVERB,



            ROBOT_VARIATION,
            REVERB_VARIATION,
            REVERB_TYPE,

            MANUAL,
            BYPASS,
            
            
            MIDI_CH,
            GATE_LEVEL,
            LOW_CUT,
            ENHANCER,
            FORMANT_DEPTH,
            MONITOR_MODE,
            EXTERNAL_CARRIER,
            USB_MIXING,
            MIDI_IN_MODE,
            PITCH_AND_FORMANT_ROUTING,
            MUTE_MODE,
            VOCODER_VARIATION,
            VOCODER_TYPE,

            BUTTON1,
            BUTTON2,
            BUTTON3,
            BUTTON4,
            BUTTON5,
            BUTTON6,
            BUTTON7,
            BUTTON8,

            ROLAND,
            VT_4,

            GLOBAL_LEVEL,
            NAME_00_03,
            NAME_04_07,
            EQUALIZER,
            EQUALIZER_LOW_SHELF_FREQUENCY,
            EQUALIZER_LOW_SHELF_GAIN,
            EQUALIZER_LOW_MID_FREQUENCY,
            EQUALIZER_LOW_MID_Q,
            EQUALIZER_LOW_MID_GAIN,
            EQUALIZER_HIGH_MID_FREQUENCY,
            EQUALIZER_HIGH_MID_Q,
            EQUALIZER_HIGH_MID_GAIN,
            EQUALIZER_HIGH_SHELF_FREQUENCY,
            EQUALIZER_HIGH_SHELF_GAIN,
            NONE,


            REVERB_PARAMETER_1,
            REVERB_PARAMETER_2,
            REVERB_PARAMETER_3,
            REVERB_PARAMETER_4,

        }

        public enum VocoderArea
        {
            VOCODER_PARAMETER_1,
            VOCODER_PARAMETER_2,
            VOCODER_PARAMETER_3,
            VOCODER_PARAMETER_4,
            VOCODER_PARAMETER_5,
            VOCODER_PARAMETER_6,
            VOCODER_PARAMETER_7,
            VOCODER_PARAMETER_8,
        }

        public enum HarmonyArea
        {
            HARMONY_PARAMETER_1,
            HARMONY_PARAMETER_2,
            HARMONY_PARAMETER_3,
            HARMONY_PARAMETER_4,
            HARMONY_PARAMETER_5,
            HARMONY_PARAMETER_6,
            HARMONY_PARAMETER_7,
            HARMONY_PARAMETER_8,
            HARMONY_PARAMETER_9,
            HARMONY_1_LEVEL,
            HARMONY_2_LEVEL,
            HARMONY_3_LEVEL,
            HARMONY_1_KEY,
            HARMONY_2_KEY,
            HARMONY_3_KEY,
            HARMONY_1_GENDER,
            HARMONY_2_GENDER,
            HARMONY_3_GENDER,
        }

        public enum RobotArea
        {
            ROBOT_PARAMETER_1,
            ROBOT_PARAMETER_2,
            ROBOT_PARAMETER_3,
            ROBOT_PARAMETER_4,
            ROBOT_PARAMETER_5,
            ROBOT_PARAMETER_6,
            ROBOT_PARAMETER_7,
            ROBOT_PARAMETER_8,
        }

        public enum MegaphoneArea
        {
            MEGAPHONE_VARIATION,
            MEGAPHONE_TYPE,
            MEGAPHONE_PARAMETER_1,
            MEGAPHONE_PARAMETER_2,
            MEGAPHONE_PARAMETER_3,
            MEGAPHONE_PARAMETER_4,
        }

        public enum ReverbArea
        {
            REVERB_PARAMETER_1,
            REVERB_PARAMETER_2,
            REVERB_PARAMETER_3,
            REVERB_PARAMETER_4,
        }

        /// <summary>
        /// Reads entire memory from VT-4 and stores in app memory
        /// </summary>
        public void ReadVt4()
        {
            // Read system:
            ReadSystem();
            if (pendingMidiRequest != PendingMidiRequest.DONE)
            {
                return;
            }

            // Read all temporary areas:
            ReadTemporaryPatch();
            if (pendingMidiRequest != PendingMidiRequest.DONE)
            {
                return;
            }
            ReadTemporaryRobot();
            if (pendingMidiRequest != PendingMidiRequest.DONE)
            {
                return;
            }
            ReadTemporaryHarmony();
            if (pendingMidiRequest != PendingMidiRequest.DONE)
            {
                return;
            }
            ReadTemporaryMegaphone();
            if (pendingMidiRequest != PendingMidiRequest.DONE)
            {
                return;
            }
            ReadTemporaryReverb();
            if (pendingMidiRequest != PendingMidiRequest.DONE)
            {
                return;
            }
            ReadTemporaryVocoder();
            if (pendingMidiRequest != PendingMidiRequest.DONE)
            {
                return;
            }
            ReadTemporaryEqualizer();
            if (pendingMidiRequest != PendingMidiRequest.DONE)
            {
                return;
            }

            // Read all user patches:
            for (byte i = 0; i < 8; i++)
            {
                ReadPatch(i);
                if (pendingMidiRequest != PendingMidiRequest.DONE)
                {
                    return;
                }
            }

            // Read all user areas:
            for (byte i = 0; i < 8; i++)
            {
                ReadRobot(i);
                if (pendingMidiRequest != PendingMidiRequest.DONE)
                {
                    return;
                }
                ReadHarmony(i);
                if (pendingMidiRequest != PendingMidiRequest.DONE)
                {
                    return;
                }
                ReadMegaphone(i);
                if (pendingMidiRequest != PendingMidiRequest.DONE)
                {
                    return;
                }
                ReadReverb(i);
                if (pendingMidiRequest != PendingMidiRequest.DONE)
                {
                    return;
                }
                ReadVocoder(i);
                if (pendingMidiRequest != PendingMidiRequest.DONE)
                {
                    return;
                }
            }

            // Equalizer has only one user area, read it:
            ReadTemporaryEqualizer();
            if (pendingMidiRequest != PendingMidiRequest.DONE)
            {
                return;
            }

            pendingMidiRequest = PendingMidiRequest.DONE;
            for (Int32 i = 0; i < 8 && pendingMidiRequest == PendingMidiRequest.DONE; i++)
            {
                VariationIndex = i;
                ReadUserPatch(i);
                ReadUserRobot(i);
                ReadUserHarmony(i);
                ReadUserMegaphone(i);
                ReadUserReverb(i);
                ReadVocoderVariation(i);
            }

            if (pendingMidiRequest == PendingMidiRequest.DONE)
            {
                VariationIndex = 0;
                ReadEqualizer();
            }
        }

        public void WriteUserPatch(Int32 PatchNumber)
        {
            byte[] address = new byte[] { 0x11, (byte)PatchNumber, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.UserPatch[PatchNumber].AsBuffer());
            //byte[] message = midi.SystemExclusiveDT1Message(DeviceInformation, address, VT4.TemporaryPatch.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            //SendProgramChange((byte)(PatchNumber + 1));
        }

        public void WriteRobotVariation(Int32 PatchNumber)
        {
            //NameConverter nameConverter = new NameConverter(vt4MenuItems[2][0][PatchNumber].TextBlock.Text);
            //VT4.UserRobot[PatchNumber].NAME_00_03 = nameConverter.Numeric1;
            //VT4.UserRobot[PatchNumber].NAME_04_07 = nameConverter.Numeric2;
            byte[] address = new byte[] { 0x21, (byte)PatchNumber, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.UserRobot[PatchNumber].AsBuffer());
            //byte[] message = midi.SystemExclusiveDT1Message(DeviceInformation, address, VT4.TemporaryRobot.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteHarmonyVariation(Int32 PatchNumber)
        {
            byte[] address = new byte[] { 0x31, (byte)PatchNumber, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.UserHarmony[PatchNumber].AsBuffer());
            //byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryHarmony.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteMegaphoneVariation(Int32 PatchNumber)
        {
            byte[] address = new byte[] { 0x41, (byte)PatchNumber, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.UserMegaphone[PatchNumber].AsBuffer());
            //byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryMegaphone.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteReverbVariation(Int32 PatchNumber)
        {
            byte[] address = new byte[] { 0x51, (byte)PatchNumber, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.UserReverb[PatchNumber].AsBuffer());
            //byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryReverb.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteVocoderVariation(Int32 PatchNumber)
        {
            byte[] address = new byte[] { 0x61, (byte)PatchNumber, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.UserVocoder[PatchNumber].AsBuffer());
            //byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryVocoder.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        //////////////////////////////////////////////////////////////////////////
        // Helpers
        //////////////////////////////////////////////////////////////////////////

        public async void ReadSystem()
        {
            pendingMidiRequest = PendingMidiRequest.READ_SYSTEM;
            byte[] address = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x10 };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadTemporaryPatch()
        {
            pendingMidiRequest = PendingMidiRequest.READ_TEMPORARY_PATCH;
            byte[] address = new byte[] { 0x10, 0x00, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x26 };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadTemporaryRobot()
        {
            pendingMidiRequest = PendingMidiRequest.READ_TEMPORARY_ROBOT;
            byte[] address = new byte[] { 0x20, 0x00, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x17 };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadTemporaryHarmony()
        {
            pendingMidiRequest = PendingMidiRequest.READ_TEMPORARY_HARMONY;
            byte[] address = new byte[] { 0x30, 0x00, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x23 };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadTemporaryMegaphone()
        {
            pendingMidiRequest = PendingMidiRequest.READ_TEMPORARY_MEGAPHONE;
            byte[] address = new byte[] { 0x40, 0x00, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x1c };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadTemporaryReverb()
        {
            pendingMidiRequest = PendingMidiRequest.READ_TEMPORARY_REVERB;
            byte[] address = new byte[] { 0x50, 0x00, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x1c };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadTemporaryVocoder()
        {
            pendingMidiRequest = PendingMidiRequest.READ_TEMPORARY_VOCODER;
            byte[] address = new byte[] { 0x60, 0x00, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x1c };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadPatch(byte userNumber)
        {
            pendingMidiRequest = PendingMidiRequest.READ_PATCH_1 + userNumber;
            byte[] address = new byte[] { 0x11, userNumber, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x26 };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadRobot(byte userNumber)
        {
            pendingMidiRequest = PendingMidiRequest.READ_ROBOT_1 + userNumber;
            byte[] address = new byte[] { 0x21, userNumber, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x17 };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadHarmony(byte userNumber)
        {
            pendingMidiRequest = PendingMidiRequest.READ_HARMONY_1 + userNumber;
            byte[] address = new byte[] { 0x31, userNumber, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x23 };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadMegaphone(byte userNumber)
        {
            pendingMidiRequest = PendingMidiRequest.READ_MEGAPHONE_1 + userNumber;
            byte[] address = new byte[] { 0x41, userNumber, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x1c };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadReverb(byte userNumber)
        {
            pendingMidiRequest = PendingMidiRequest.READ_REVERB_1 + userNumber;
            byte[] address = new byte[] { 0x51, userNumber, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x1c };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadVocoder(byte userNumber)
        {
            pendingMidiRequest = PendingMidiRequest.READ_VOCODER_1 + userNumber;
            byte[] address = new byte[] { 0x61, userNumber, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x1c };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadTemporaryEqualizer()
        {
            pendingMidiRequest = PendingMidiRequest.READ_TEMPORARY_EQUALIZER;
            byte[] address = new byte[] { 0x62, 0x00, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x20 };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadEqualizer()
        {
            pendingMidiRequest = PendingMidiRequest.READ_EQUALIZER;
            byte[] address = new byte[] { 0x63, 0x00, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x20 };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadUserPatch(Int32 PatchNumber)
        {
            pendingMidiRequest = PendingMidiRequest.READ_USER_PATCH;
            byte[] address = new byte[] { 0x11, (byte)PatchNumber, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x26 };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadUserRobot(Int32 RobotNumber)
        {
            pendingMidiRequest = PendingMidiRequest.READ_USER_ROBOT;
            byte[] address = null;
            if (RobotNumber > -1)
            {
                address = new byte[] { 0x21, (byte)RobotNumber, 0x00, 0x00 };
            }
            else
            {
                address = new byte[] { 0x20, 0x00, 0x00, 0x00 };
            }
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x17 };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadUserHarmony(Int32 HarmonyNumber)
        {
            pendingMidiRequest = PendingMidiRequest.READ_USER_HARMONY;
            byte[] address = new byte[] { 0x31, (byte)HarmonyNumber, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x23 };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadUserMegaphone(Int32 MegaphoneNumber)
        {
            pendingMidiRequest = PendingMidiRequest.READ_MEGAPHONE_VARIATION;
            byte[] address = new byte[] { 0x41, (byte)MegaphoneNumber, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x1c };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadUserReverb(Int32 ReverbNumber)
        {
            pendingMidiRequest = PendingMidiRequest.READ_REVERB_VARIATION;
            byte[] address = new byte[] { 0x51, (byte)ReverbNumber, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x1c };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public async void ReadVocoderVariation(Int32 VariationIndex)
        {
            pendingMidiRequest = PendingMidiRequest.READ_VOCODER_VARIATION;
            byte[] address = new byte[] { 0x61, (byte)VariationIndex, 0x00, 0x00 };
            byte[] length = new byte[] { 0x00, 0x00, 0x00, 0x1c };
            byte[] message = midi.SystemExclusiveRQ1Message(midi.PortPairs[0], address, length);
            midi.SendSystemExclusive(midi.PortPairs[0], message);
            await WaitForMidiRequestAnswered();
        }

        public void WriteSystem()
        {
            byte[] address = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.System.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteTemporaryPatch()
        {
            byte[] address = new byte[] { 0x10, 0x00, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryPatch.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteTemporaryRobot()
        {
            byte[] address = new byte[] { 0x20, 0x00, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryRobot.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteTemporaryHarmony()
        {
            byte[] address = new byte[] { 0x30, 0x00, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryHarmony.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteTemporaryMegaphone()
        {
            byte[] address = new byte[] { 0x40, 0x00, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryMegaphone.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteTemporaryReverb()
        {
            byte[] address = new byte[] { 0x50, 0x00, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryReverb.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteTemporaryVocoder()
        {
            byte[] address = new byte[] { 0x60, 0x00, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryVocoder.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteTemporaryEqualizer()
        {
            byte[] address = new byte[] { 0x62, 0x00, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryEqualizer.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteUserRobot(int RobotNumber)
        {
            byte[] address = new byte[] { 0x21, (byte)RobotNumber, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryRobot.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteUserHarmony(int HarmonyNumber)
        {
            byte[] address = new byte[] { 0x31, (byte)HarmonyNumber, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryHarmony.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteUserMegaphone(int MegaphoneNumber)
        {
            byte[] address = new byte[] { 0x41, (byte)MegaphoneNumber, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryMegaphone.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteUserReverb(int ReverbNumber)
        {
            byte[] address = new byte[] { 0x51, (byte)ReverbNumber, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryReverb.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteUserVocoder(int VocoderNumber)
        {
            byte[] address = new byte[] { 0x61, (byte)VocoderNumber, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryVocoder.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        public void WriteEqualizer(byte userPatch = 0)
        {
            byte addr0Offset = (byte)(userPatch > 0 ? 1 : 0);
            byte[] address = new byte[] { 0x63, 0x00, 0x00, 0x00 };
            address = new byte[] { (byte)(0x60 + addr0Offset), 0x0, 0x0, 0x0 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.UserEqualizer.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }
    }
}
