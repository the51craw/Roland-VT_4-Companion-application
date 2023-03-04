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
            //VOCODER_VARIATION_1,
            //VOCODER_VARIATION_2,
            //VOCODER_VARIATION_3,
            //VOCODER_VARIATION_4,
            //VOCODER_VARIATION_5,
            //VOCODER_VARIATION_6,
            //VOCODER_VARIATION_7,
            //VOCODER_VARIATION_8,
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

        /// <summary>
        /// Writes entire memory to VT-4
        /// </summary>
        public void WriteVt4()
        {
            WriteSystem();
            WriteTemporaryPatch();
            WriteTemporaryRobot();
            WriteTemporaryHarmony();
            WriteTemporaryMegaphone();
            WriteTemporaryReverb();
            WriteTemporaryVocoder();
            WriteTemporaryEqualizer();

            for (Int32 i = 0; i < 8; i++)
            {
                VariationIndex = i;
                WriteUserPatch(i);
            }

            for (Int32 i = 0; i < 8; i++)
            {
                VariationIndex = i;
                WriteRobotVariation(i);
                WriteHarmonyVariation(i);
                WriteMegaphoneVariation(i);
                WriteReverbVariation(i);
                WriteVocoderVariation(i);
            }

            VariationIndex = 0;
            WriteEqualizer();
        }

        public void WriteVt4(Int32 patch)
        {
            WriteUserPatch(patch);
            WriteRobotVariation(VT4.TemporaryPatch.ROBOT_VARIATION);
            WriteHarmonyVariation(VT4.TemporaryPatch.HARMONY_VARIATION);
            WriteMegaphoneVariation(VT4.TemporaryPatch.MEGAPHONE_VARIATION);
            WriteReverbVariation(VT4.TemporaryPatch.REVERB_VARIATION);
            WriteVocoderVariation(VT4.TemporaryPatch.VOCODER_VARIATION);
        }

        /// <summary>
        /// Reads all the temporary and stores in app memory
        /// </summary>
        //public async void ReadTemporary()
        //{
        //    ReadTemporaryPatch();
        //    ReadTemporaryRobot();
        //    ReadTemporaryHarmony();
        //    ReadTemporaryMegaphone();
        //    ReadTemporaryReverb();
        //    ReadTemporaryVocoder();
        //    ReadTemporaryEqualizer();

        //    ReadTemporaryRobot();
        //    ReadTemporaryHarmony();
        //    ReadTemporaryMegaphone();
        //    ReadTemporaryReverb();
        //    ReadTemporaryVocoder();
        //}

        /// <summary>
        /// Writes temporary to VT-4
        /// </summary>
        public void WriteTemporary()
        {
            WriteTemporaryRobot();
            WriteTemporaryMegaphone();
            WriteTemporaryVocoder();
            WriteTemporaryHarmony();
            WriteTemporaryReverb();
            WriteTemporaryEqualizer();
            WriteTemporaryPatch();
        }

        //public void StoreUserPatch(Int32 PatchNumber)
        //{
        //    WriteUserPatch(PatchNumber);
        //    WriteRobotVariation(VT4.TemporaryPatch.ROBOT_VARIATION);
        //    WriteHarmonyVariation(VT4.TemporaryPatch.HARMONY_VARIATION);
        //    WriteMegaphoneVariation(VT4.TemporaryPatch.MEGAPHONE_VARIATION);
        //    WriteReverbVariation(VT4.TemporaryPatch.REVERB_VARIATION);
        //    WriteVocoderVariation(VT4.TemporaryPatch.VOCODER_VARIATION);
        //    WriteEqualizer();
        //}

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

        public void WriteUserEqualizer()
        {
            byte[] address = new byte[] { 0x63, 0x00, 0x00, 0x00 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryEqualizer.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        //public void WriteUserPatch(Int32 PatchNumber)
        //{
        //    byte[] address = new byte[] { 0x11, (byte)PatchNumber, 0x00, 0x00 };
        //    byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.Patch[PatchNumber].AsBuffer());
        //    byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryPatch.AsBuffer());
        //    midi.SendSystemExclusive(midi.PortPairs[0], message);
        //}

        //public void WriteRobotVariation(Int32 PatchNumber)
        //{
        //    byte[] address = new byte[] { 0x21, (byte)PatchNumber, 0x00, 0x00 };
        //    byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.Robot[PatchNumber].AsBuffer());
        //    byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryRobot.AsBuffer());
        //    midi.SendSystemExclusive(midi.PortPairs[0], message);
        //}

        //public void WriteHarmonyVariation(Int32 PatchNumber)
        //{
        //    byte[] address = new byte[] { 0x31, (byte)PatchNumber, 0x00, 0x00 };
        //    byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.Harmony[PatchNumber].AsBuffer());
        //    byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryHarmony.AsBuffer());
        //    midi.SendSystemExclusive(midi.PortPairs[0], message);
        //}

        //public void WriteMegaphoneVariation(Int32 PatchNumber)
        //{
        //    byte[] address = new byte[] { 0x41, (byte)PatchNumber, 0x00, 0x00 };
        //    byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.Megaphone[PatchNumber].AsBuffer());
        //    byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryMegaphone.AsBuffer());
        //    midi.SendSystemExclusive(midi.PortPairs[0], message);
        //}

        //public void WriteReverbVariation(Int32 PatchNumber)
        //{
        //    byte[] address = new byte[] { 0x51, (byte)PatchNumber, 0x00, 0x00 };
        //    byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.Reverb[PatchNumber].AsBuffer());
        //    byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryReverb.AsBuffer());
        //    midi.SendSystemExclusive(midi.PortPairs[0], message);
        //}

        //public void WriteVocoderVariation(Int32 PatchNumber)
        //{
        //    byte[] address = new byte[] { 0x61, (byte)PatchNumber, 0x00, 0x00 };
        //    byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.Vocoder[PatchNumber].AsBuffer());
        //    byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.TemporaryVocoder.AsBuffer());
        //    midi.SendSystemExclusive(midi.PortPairs[0], message);
        //}

        public void WriteEqualizer(byte userPatch = 0)
        {
            byte addr0Offset = (byte)(userPatch > 0 ? 1 : 0);
            byte[] address = new byte[] { 0x63, 0x00, 0x00, 0x00 };
            address = new byte[] { (byte)(0x60 + addr0Offset), 0x0, 0x0, 0x0 };
            byte[] message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, VT4.UserEqualizer.AsBuffer());
            midi.SendSystemExclusive(midi.PortPairs[0], message);
        }

        //public byte[] Int32ToBytes(Int32 value)
        //{
        //    if (value < 128)
        //    {
        //        // Single byte, just convert:
        //        return new byte[] { (byte)value };
        //    }
        //    else if (value < 256)
        //    {
        //        // Two byte value:
        //        return new byte[] { (byte)(value % 128), (byte)(value / 128) };
        //    }
        //    else
        //    {
        //        // Larger than 255 are nibblewise:
        //        List<byte> bytes = new List<byte>();
        //        Int32 count = 0;
        //        while (value > 0)
        //        {
        //            bytes.Add((byte)(value % 16));
        //            value /= 16;
        //            count++;
        //        }
        //        byte[] result = new byte[count];
        //        while (count > 0)
        //        {
        //            count--;
        //            result[count] = bytes[0];
        //            bytes.RemoveAt(0);
        //        }
        //        return result;
        //    }
        //}

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
            //byte[] menuItemToEqualizerParameter = new byte[]
            //{
            //    VT4.UserEqualizer.EQUALIZER_SWITCH,
            //    VT4.UserEqualizer.EQUALIZER_LOW_SHELF_FREQUENCY,
            //    VT4.UserEqualizer.EQUALIZER_LOW_SHELF_GAIN,
            //    VT4.UserEqualizer.EQUALIZER_LOW_MID_FREQUENCY,
            //    VT4.UserEqualizer.EQUALIZER_LOW_MID_Q,
            //    VT4.UserEqualizer.EQUALIZER_LOW_MID_GAIN,
            //    VT4.UserEqualizer.EQUALIZER_HIGH_MID_FREQUENCY,
            //    VT4.UserEqualizer.EQUALIZER_HIGH_MID_Q,
            //    VT4.UserEqualizer.EQUALIZER_HIGH_MID_GAIN,
            //    VT4.UserEqualizer.EQUALIZER_HIGH_SHELF_FREQUENCY,
            //    VT4.UserEqualizer.EQUALIZER_HIGH_SHELF_GAIN
            //};

            //////////////////////////////////////////////////////////////////////////////////
            /// Knobs
            //////////////////////////////////////////////////////////////////////////////////
            if (control.GetType() == typeof(Knob))
            {
                midi.SendControlChange(midi.PortPairs[0],
                    idToCcNumberMap[control.Id], (byte)((Knob)control).Value);
                if (control.Id > 1 && sceneIndex > -1)
                {
                    sceneEdited[sceneIndex] = true;
                }
            }

            //////////////////////////////////////////////////////////////////////////////////
            /// Sliders
            //////////////////////////////////////////////////////////////////////////////////
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

            //////////////////////////////////////////////////////////////////////////////////
            /// ImageButtons
            //////////////////////////////////////////////////////////////////////////////////
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

            //////////////////////////////////////////////////////////////////////////////////
            /// PopupMenuButtons
            //////////////////////////////////////////////////////////////////////////////////
            else if ((control.GetType() == typeof(PopupMenuButton)))
            {
                /// Effect menus /////////////////////////////////////////////////////////////
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
                        parameterAddress = effectParameterOffset[control.Id];
                        int i = effectParameterOffset[control.Id];
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
                            Parameters[((PopupMenuButton)control).MenuItemNumber].GetType() == typeof(int))
                        {
                            // Numeric variable:
                            if ((int)parms.ParameterList[control.Id].
                                Parameters[((PopupMenuButton)control).MenuItemNumber] > 127)
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
                            0x00, 0x00, (byte)(((PopupMenuButton)control).MenuItemNumber) };
                        //userEffectAddress = new byte[] { (byte)(baseAddress + 1),
                        //    (byte)VariationIndex, 0x00, (byte)(parameterAddress) };

                        // Send to temporary effect:
                        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], temporaryEffectAddress, data.ToArray());
                        midi.SendSystemExclusive(midi.PortPairs[0], message);

                        //// Send to user effect:
                        //message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], userEffectAddress, data.ToArray());
                        //midi.SendSystemExclusive(midi.PortPairs[0], message);

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
                /// Number button menus /////////////////////////////////////////////////////////////
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
                            //PopupMenuButton scene = (PopupMenuButton)Controls.ControlsList[topLevelMenuLocations[control.Id - 14]];
                            //PopupMenuButton save = (PopupMenuButton)Controls.ControlsList[topLevelMenuLocations[control.Id - 14 + 1]];
                            //PopupMenuButton copyFrom = (PopupMenuButton)Controls.ControlsList[topLevelMenuLocations[control.Id - 14 + 2]];

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
                /// Roland logo button //////////////////////////////////////////////////////////////
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
                            //int item = ((PopupMenuButton)control).MenuItemNumber;
                            //if (item == 0)
                            //{
                            //    menuItemToEqualizerParameter[item] =
                            //        (byte)(((PopupMenuButton)control).IsOn ? 1 : 0);
                            //}
                            //else
                            //{
                            //    menuItemToEqualizerParameter[item] =
                            //        (byte)pmbEqualizerSettings[item].Value;
                            //}
                            //WriteTemporaryEqualizer();
                            //WriteUserEqualizer();
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
                        else if (((PopupMenuButton)control).Parent == pmbSave)
                        {
                            await WriteJsonFile();
                        }
                        else if (((PopupMenuButton)control).Parent == pmbLoad)
                        {
                            await ReadJsonFile();
                        }
                    }
                    //if (((PopupMenuButton)control).MenuNumber == 0)
                    //{
                    //    if (((PopupMenuButton)control).Parent.MenuNumber == -1)
                    //    {
                    //        switch (((PopupMenuButton)control).MenuItemNumber)
                    //        {
                    //            case 0:
                    //                // MIDI channel selection
                    //                VT4.System.MIDI_CH = (byte)((PopupMenuButton)control).MenuItemNumber;
                    //                WriteSystem();
                    //                midi.PortPairs[0].OutChannel = (byte)((PopupMenuButton)control).MenuItemNumber;
                    //                midi.PortPairs[0].InChannel = (byte)((PopupMenuButton)control).MenuItemNumber;
                    //                foreach (PopupMenuButton pmb in pmbRoland.Children[0])
                    //                {
                    //                    pmb.Set(false);
                    //                }
                    //                pmbMidiChannels[(byte)((PopupMenuButton)control).MenuItemNumber].Set(true);
                    //                break;
                    //            case 1:
                    //                break;
                    //            case 2:
                    //                break;
                    //            case 3:
                    //                break;
                    //            case 4:
                    //                break;
                    //            case 5:
                    //                break;
                    //            case 6:
                    //                break;
                    //            case 7:
                    //                break;
                    //        }
                    //    }
                    //}
                    //((PopupMenuButton)control).MenuItemNumber ==)
                }
            }
        }

        public void SendSingleSysEx(Area control, byte[] data)
        {
            SendSingleSysExToParameter(control, data, -1);                  // Send to temporary effect
            SendSingleSysExToParameter(control, data, VariationIndex);      // Send to user effect

            if (sceneIndex > -1)
            {
                SendSingleSysExToPatch(control, data, sceneIndex);          // Send to user patch
            }
            SendSingleSysExToPatch(control, data, -1);                      // Send to temporary patch
        }

        public void SendSingleSysExToParameter(Area control, byte[] data, int variationIndex)
        {
            byte[] address = null;
            byte[] message = null;
            byte addr0Offset = 0;    // User patches offsets 1 from temporary patch in address[0]
            if (variationIndex > -1)
            {
                addr0Offset = 1;
            }
            else
            {
                variationIndex = 0;
            }

            switch (control)
            {
            //    case Area.ROBOT_PARAMETER_1:
            //        address = new byte[] { (byte)(0x20 + addr0Offset), (byte)variationIndex, 0x0, 0x00 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, new byte[] { data[0] });
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.ROBOT_PARAMETER_2:
            //        address = new byte[] { (byte)(0x20 + addr0Offset), (byte)variationIndex, 0x0, 0x01 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, new byte[] { data[0] });
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.ROBOT_PARAMETER_3:
            //        address = new byte[] { (byte)(0x20 + addr0Offset), (byte)variationIndex, 0x0, 0x02 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.ROBOT_PARAMETER_4:
            //        address = new byte[] { (byte)(0x20 + addr0Offset), (byte)variationIndex, 0x0, 0x04 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.MEGAPHONE_TYPE:
            //        address = new byte[] { (byte)(0x40 + addr0Offset), (byte)variationIndex, 0x0, 0x0 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;
            //    case Area.MEGAPHONE_PARAMETER_1:
            //        address = new byte[] { (byte)(0x40 + addr0Offset), (byte)variationIndex, 0x0, 0x01 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.MEGAPHONE_PARAMETER_2:
            //        address = new byte[] { (byte)(0x40 + addr0Offset), (byte)variationIndex, 0x0, 0x03 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.MEGAPHONE_PARAMETER_3:
            //        address = new byte[] { (byte)(0x40 + addr0Offset), (byte)variationIndex, 0x0, 0x05 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.MEGAPHONE_PARAMETER_4:
            //        address = new byte[] { (byte)(0x40 + addr0Offset), (byte)variationIndex, 0x0, 0x07 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;
            //    case Area.HARMONY_1_LEVEL:
            //    case Area.HARMONY_PARAMETER_1:
            //        address = new byte[] { (byte)(0x30 + addr0Offset), (byte)variationIndex, 0x0, 0x0 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.HARMONY_2_LEVEL:
            //    case Area.HARMONY_PARAMETER_2:
            //        address = new byte[] { (byte)(0x30 + addr0Offset), (byte)variationIndex, 0x0, 0x02 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.HARMONY_3_LEVEL:
            //    case Area.HARMONY_PARAMETER_3:
            //        address = new byte[] { (byte)(0x30 + addr0Offset), (byte)variationIndex, 0x0, 0x04 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.HARMONY_1_KEY:
            //    case Area.HARMONY_PARAMETER_4:
            //        address = new byte[] { (byte)(0x30 + addr0Offset), (byte)variationIndex, 0x0, 0x06 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.HARMONY_2_KEY:
            //    case Area.HARMONY_PARAMETER_5:
            //        address = new byte[] { (byte)(0x30 + addr0Offset), (byte)variationIndex, 0x0, 0x07 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.HARMONY_3_KEY:
            //    case Area.HARMONY_PARAMETER_6:
            //        address = new byte[] { (byte)(0x30 + addr0Offset), (byte)variationIndex, 0x0, 0x08 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.HARMONY_1_GENDER:
            //    case Area.HARMONY_PARAMETER_7:
            //        address = new byte[] { (byte)(0x30 + addr0Offset), (byte)variationIndex, 0x0, 0x09 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.HARMONY_2_GENDER:
            //    case Area.HARMONY_PARAMETER_8:
            //        address = new byte[] { (byte)(0x30 + addr0Offset), (byte)variationIndex, 0x0, 0x0b };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.HARMONY_3_GENDER:
            //    case Area.HARMONY_PARAMETER_9:
            //        address = new byte[] { (byte)(0x30 + addr0Offset), (byte)variationIndex, 0x0, 0x0d };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;
            //    case Area.REVERB_TYPE:
            //        address = new byte[] { (byte)(0x50 + addr0Offset), (byte)variationIndex, 0x0, 0x0 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            case Area.REVERB_PARAMETER_1:
                address = new byte[] { (byte)(0x50 + addr0Offset), (byte)(0x0 + VariationIndex), 0x0, 0x01 };
                message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                midi.SendSystemExclusive(midi.PortPairs[0], message);
                break;

            case Area.REVERB_PARAMETER_2:
                address = new byte[] { (byte)(0x50 + addr0Offset), (byte)variationIndex, 0x0, 0x03 };
                message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                midi.SendSystemExclusive(midi.PortPairs[0], message);
                break;

            //    case Area.REVERB_PARAMETER_3:
            //        address = new byte[] { (byte)(0x50 + addr0Offset), (byte)variationIndex, 0x0, 0x05 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.REVERB_PARAMETER_4:
            //        address = new byte[] { (byte)(0x50 + addr0Offset), (byte)variationIndex, 0x0, 0x07 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.VOCODER_TYPE:
            //        address = new byte[] { (byte)(0x60 + addr0Offset), (byte)variationIndex, 0x0, 0x0 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.VOCODER_PARAMETER_1:
            //        address = new byte[] { (byte)(0x60 + addr0Offset), (byte)variationIndex, 0x0, 0x01 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.VOCODER_PARAMETER_2:
            //        address = new byte[] { (byte)(0x60 + addr0Offset), (byte)variationIndex, 0x0, 0x03 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.VOCODER_PARAMETER_3:
            //        address = new byte[] { (byte)(0x60 + addr0Offset), (byte)variationIndex, 0x0, 0x05 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.VOCODER_PARAMETER_4:
            //        address = new byte[] { (byte)(0x60 + addr0Offset), (byte)variationIndex, 0x0, 0x07 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.EQUALIZER:
            //        address = new byte[] { (byte)(0x62 + addr0Offset), (byte)variationIndex, 0x0, 0x0 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.EQUALIZER_LOW_SHELF_FREQUENCY:
            //        address = new byte[] { (byte)(0x62 + addr0Offset), (byte)variationIndex, 0x0, 0x01 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.EQUALIZER_LOW_SHELF_GAIN:
            //        address = new byte[] { (byte)(0x62 + addr0Offset), (byte)variationIndex, 0x0, 0x02 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.EQUALIZER_LOW_MID_FREQUENCY:
            //        address = new byte[] { (byte)(0x62 + addr0Offset), (byte)variationIndex, 0x0, 0x03 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.EQUALIZER_LOW_MID_Q:
            //        address = new byte[] { (byte)(0x62 + addr0Offset), (byte)variationIndex, 0x0, 0x04 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.EQUALIZER_LOW_MID_GAIN:
            //        address = new byte[] { (byte)(0x62 + addr0Offset), (byte)variationIndex, 0x0, 0x05 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.EQUALIZER_HIGH_MID_FREQUENCY:
            //        address = new byte[] { (byte)(0x62 + addr0Offset), (byte)variationIndex, 0x0, 0x06 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.EQUALIZER_HIGH_MID_Q:
            //        address = new byte[] { (byte)(0x62 + addr0Offset), (byte)variationIndex, 0x0, 0x07 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.EQUALIZER_HIGH_MID_GAIN:
            //        address = new byte[] { (byte)(0x62 + addr0Offset), (byte)variationIndex, 0x0, 0x08 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.EQUALIZER_HIGH_SHELF_FREQUENCY:
            //        address = new byte[] { (byte)(0x62 + addr0Offset), (byte)variationIndex, 0x0, 0x09 };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;

            //    case Area.EQUALIZER_HIGH_SHELF_GAIN:
            //        address = new byte[] { (byte)(0x62 + addr0Offset), (byte)variationIndex, 0x0, 0x0a };
            //        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
            //        midi.SendSystemExclusive(midi.PortPairs[0], message);
            //        break;
            }
        }

        public void SendSingleSysExToPatch(Area control, byte[] data, int userPatchIndex)
        {
            byte[] address = null;
            byte[] message = null;
            byte addr0Offset = 0;    // User patches offsets 1 from temporary patch in address[0]
            if (userPatchIndex > -1)
            {
                addr0Offset = 1;
                userPatchIndex = (byte)sceneIndex;
            }

            switch (control)
            {
                case Area.MIDI_CH:
                    address = new byte[] { 0x0, 0x0, 0x0, 0x0 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.GATE_LEVEL:
                    address = new byte[] { 0x0, 0x0, 0x0, 0x1 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.LOW_CUT:
                    address = new byte[] { 0x0, 0x0, 0x0, 0x2 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.ENHANCER:
                    address = new byte[] { 0x0, 0x0, 0x0, 0x3 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.FORMANT_DEPTH:
                    address = new byte[] { 0x0, 0x0, 0x0, 0x4 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.MONITOR_MODE:
                    address = new byte[] { 0x0, 0x0, 0x0, 0x5 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.EXTERNAL_CARRIER:
                    address = new byte[] { 0x0, 0x0, 0x0, 0x6 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.USB_MIXING:
                    address = new byte[] { 0x0, 0x0, 0x0, 0x7 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.MIDI_IN_MODE:
                    address = new byte[] { 0x0, 0x0, 0x0, 0x8 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.PITCH_AND_FORMANT_ROUTING:
                    address = new byte[] { 0x0, 0x0, 0x0, 0x9 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.MUTE_MODE:
                    address = new byte[] { 0x0, 0x0, 0x0, 0xA };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.ROBOT:
                    if (Variation == -1)
                    {
                        address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x0 };
                        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                        midi.SendSystemExclusive(midi.PortPairs[0], message);
                    }
                    else
                    {
                        address = new byte[] { (byte)(0x10 + addr0Offset),
                            (byte)(0x0 + userPatchIndex), 0x0, 0x04 };
                        message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                        midi.SendSystemExclusive(midi.PortPairs[0], message);
                    }
                    break;

                case Area.HARMONY:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x01 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.VOCODER:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x02 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.MEGAPHONE:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x03 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.ROBOT_VARIATION:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x04 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                //case Area.HARMONY_VARIATION:
                //    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x05 };
                //    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                //    midi.SendSystemExclusive(midi.PortPairs[0], message);
                //    break;

                //case Area.VOCODER_VARIATION:
                //    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x06 };
                //    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                //    midi.SendSystemExclusive(midi.PortPairs[0], message);
                //    break;

                //case Area.MEGAPHONE_VARIATION:
                //    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x07 };
                //    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                //    midi.SendSystemExclusive(midi.PortPairs[0], message);
                //    break;

                case Area.REVERB_VARIATION:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x08 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.PITCH:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x09 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data);
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.FORMANT:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x0b };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.BALANCE:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x0d };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.REVERB_SLIDER:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x0f };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.AUTO_PITCH:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x11 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.KEY:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x13 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.GLOBAL_LEVEL:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x14 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.NAME_00_03:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x16 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;

                case Area.NAME_04_07:
                    address = new byte[] { (byte)(0x10 + addr0Offset), (byte)(0x0 + userPatchIndex), 0x0, 0x16 };
                    message = midi.SystemExclusiveDT1Message(midi.PortPairs[0], address, data.ToArray());
                    midi.SendSystemExclusive(midi.PortPairs[0], message);
                    break;





            }
        }
    }
}
