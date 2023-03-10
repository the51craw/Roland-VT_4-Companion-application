using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Midi;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static VT_4.MainPage;

namespace VT_4
{
    public sealed partial class MainPage : Page
    {
        // When loading 
        public enum CurrentOperation
        {
            NONE,
            LOADING_USER_PATCH,
            SAVING_USER_PATCH,
            COPYING_USER_PATCH,
        }

        /// <summary>
        /// ButtonStates is used do know how to handle incoming
        /// messages from the VT-4. All buttons sends CC messages
        /// with id in second byte. Third byte is 0x7f for button
        /// down and 0x00 for button up. However, Manual button 
        /// does NOT send button up. 
        /// One press on MANUAL enters buttons 1 - 4 are used
        /// as buttons 5 - 8. A subsequent press on MANUAL without
        /// intermediate other event means MANUAL button up.
        /// Intermediate PC message means that either that buttons
        /// 1 - 4 are used as buttons 5 - 8, or, if an effect button
        /// is down, that variation for the effect is selected thus
        /// allowing to select a variation 5 - 8 rather than 1 - 4.
        /// </summary>
        public bool manualButtonIsDown = false;
        public int sceneButtonPressed = -1;
        public bool robotButtonIsDown = false;
        public bool megaphoneButtonIsDown = false;
        public bool vocoderButtonIsDown = false;
        public bool harmonyButtonIsDown = false;
        public bool reverbButtonIsDown = false;

        public bool measureRobotButtonDownTime = false;
        public bool measureMegaphoneButtonDownTime = false;
        public bool measureVocoderButtonDownTime = false;
        public bool measureHarmonyButtonDownTime = false;
        public bool measureReverbButtonDownTime = false;

        // Do NOT reorder items in PendingMidiRequest!
        // They are used as Integers in some cases.
        // New items MUST be added at bottom of list!
        public enum PendingMidiRequest
        {
            SAVE_PATCH_1,
            SAVE_PATCH_2,
            SAVE_PATCH_3,
            SAVE_PATCH_4,
            SAVE_PATCH_5,
            SAVE_PATCH_6,
            SAVE_PATCH_7,
            SAVE_PATCH_8,
            COPY_TO_PATCH_1,
            COPY_TO_PATCH_2,
            COPY_TO_PATCH_3,
            COPY_TO_PATCH_4,
            COPY_TO_PATCH_5,
            COPY_TO_PATCH_6,
            COPY_TO_PATCH_7,
            COPY_TO_PATCH_8,
            READ_SYSTEM,
            READ_TEMPORARY_PATCH,
            READ_PATCH_1,
            READ_PATCH_2,
            READ_PATCH_3,
            READ_PATCH_4,
            READ_PATCH_5,
            READ_PATCH_6,
            READ_PATCH_7,
            READ_PATCH_8,
            READ_USER_PATCH,
            READ_TEMPORARY_ROBOT,
            READ_ROBOT_1,
            READ_ROBOT_2,
            READ_ROBOT_3,
            READ_ROBOT_4,
            READ_ROBOT_5,
            READ_ROBOT_6,
            READ_ROBOT_7,
            READ_ROBOT_8,
            READ_USER_ROBOT,
            READ_TEMPORARY_MEGAPHONE,
            READ_MEGAPHONE_1,
            READ_MEGAPHONE_2,
            READ_MEGAPHONE_3,
            READ_MEGAPHONE_4,
            READ_MEGAPHONE_5,
            READ_MEGAPHONE_6,
            READ_MEGAPHONE_7,
            READ_MEGAPHONE_8,
            READ_MEGAPHONE_VARIATION,
            READ_TEMPORARY_VOCODER,
            READ_VOCODER_1,
            READ_VOCODER_2,
            READ_VOCODER_3,
            READ_VOCODER_4,
            READ_VOCODER_5,
            READ_VOCODER_6,
            READ_VOCODER_7,
            READ_VOCODER_8,
            READ_VOCODER_VARIATION,
            READ_TEMPORARY_HARMONY,
            READ_HARMONY_1,
            READ_HARMONY_2,
            READ_HARMONY_3,
            READ_HARMONY_4,
            READ_HARMONY_5,
            READ_HARMONY_6,
            READ_HARMONY_7,
            READ_HARMONY_8,
            READ_USER_HARMONY,
            READ_TEMPORARY_REVERB,
            READ_REVERB_1,
            READ_REVERB_2,
            READ_REVERB_3,
            READ_REVERB_4,
            READ_REVERB_5,
            READ_REVERB_6,
            READ_REVERB_7,
            READ_REVERB_8,
            READ_REVERB_VARIATION,
            READ_TEMPORARY_EQUALIZER,
            READ_EQUALIZER,
            CHECKING,
            DONE,
        }

        private enum CcMessages
        {
            VOLUME = 46,
            MIC_SENS,
            KEY,
            ROBOT,
            MEGAPHONE,
            BYPASS,
            VOCODER,
            HARMONY,
            FORMANT,
            AUTO_PITCH,
            BALANCE,
            REVERB_SLIDER,
            LINE_OUT_SELECT,
        }

        public PendingMidiRequest pendingMidiRequest;
        private byte[] MidiInBuffer;
        //private Boolean ManualButtonIsDown = false;

        // The VT-4 keeps hammering CC's amd PB's once the handle has been moved.
        // This doed not stop until some other manipulation has occurred.
        // Therefore we need to remember last incoming Pitch Bend in order
        // to ignore all foolowing that are the same.
        byte[] lastPB = null;
        byte[] lastCC = null;

        public void InPort_MessageReceived(Windows.Devices.Midi.MidiInPort sender, Windows.Devices.Midi.MidiMessageReceivedEventArgs args)
        {
            if (args.Message.Type == MidiMessageType.SystemExclusive)
            {
                MidiInBuffer = args.Message.RawData.ToArray();

                switch (MidiInBuffer[1])  // Contains message ID
                {
                    case 0x7e:      // Identity request message
                        Debug.WriteLine("midi.PortPairs[" + midi.expectingAnswer.ToString() +
                            "].DeviceId = " + MidiInBuffer[2].ToString());
                        if (midi.expectingAnswer < midi.PortPairs.Count)
                        {
                            midi.PortPairs[midi.expectingAnswer].DeviceId = MidiInBuffer[2];
                            midi.PortPairs[midi.expectingAnswer].InChannel = (byte)(MidiInBuffer[0] & 0x0f);
                            midi.PortPairs[midi.expectingAnswer].OutChannel = (byte)(MidiInBuffer[0] & 0x0f);
                        }
                        break;
                    case 0x41:      // Roland SysEx message
                        switch (pendingMidiRequest)
                        {
                            case PendingMidiRequest.READ_SYSTEM:
                                VT4.System = new System(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_TEMPORARY_PATCH:
                                VT4.TemporaryPatch = new Patch(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_TEMPORARY_ROBOT:
                                VT4.TemporaryRobot = new Robot(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_TEMPORARY_MEGAPHONE:
                                VT4.TemporaryMegaphone = new Megaphone(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_TEMPORARY_VOCODER:
                                VT4.TemporaryVocoder = new Vocoder(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_TEMPORARY_HARMONY:
                                VT4.TemporaryHarmony = new Harmony(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_TEMPORARY_REVERB:
                                VT4.TemporaryReverb = new Reverb(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_TEMPORARY_EQUALIZER:
                                VT4.TemporaryEqualizer = new Equalizer(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_USER_PATCH:
                                VT4.UserPatch[VariationIndex] = new Patch(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_USER_ROBOT:
                                VT4.UserRobot[VariationIndex] = new Robot(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_MEGAPHONE_VARIATION:
                                VT4.UserMegaphone[VariationIndex] = new Megaphone(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_VOCODER_VARIATION:
                                VT4.UserVocoder[VariationIndex] = new Vocoder(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_USER_HARMONY:
                                VT4.UserHarmony[VariationIndex] = new Harmony(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_REVERB_VARIATION:
                                VT4.UserReverb[VariationIndex] = new Reverb(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_EQUALIZER:
                                VT4.UserEqualizer = new Equalizer(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_PATCH_1:
                            case PendingMidiRequest.READ_PATCH_2:
                            case PendingMidiRequest.READ_PATCH_3:
                            case PendingMidiRequest.READ_PATCH_4:
                            case PendingMidiRequest.READ_PATCH_5:
                            case PendingMidiRequest.READ_PATCH_6:
                            case PendingMidiRequest.READ_PATCH_7:
                            case PendingMidiRequest.READ_PATCH_8:
                                VT4.UserPatch[pendingMidiRequest - PendingMidiRequest.READ_PATCH_1] = new Patch(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_ROBOT_1:
                            case PendingMidiRequest.READ_ROBOT_2:
                            case PendingMidiRequest.READ_ROBOT_3:
                            case PendingMidiRequest.READ_ROBOT_4:
                            case PendingMidiRequest.READ_ROBOT_5:
                            case PendingMidiRequest.READ_ROBOT_6:
                            case PendingMidiRequest.READ_ROBOT_7:
                            case PendingMidiRequest.READ_ROBOT_8:
                                VT4.UserRobot[pendingMidiRequest - PendingMidiRequest.READ_ROBOT_1] = new Robot(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_MEGAPHONE_1:
                            case PendingMidiRequest.READ_MEGAPHONE_2:
                            case PendingMidiRequest.READ_MEGAPHONE_3:
                            case PendingMidiRequest.READ_MEGAPHONE_4:
                            case PendingMidiRequest.READ_MEGAPHONE_5:
                            case PendingMidiRequest.READ_MEGAPHONE_6:
                            case PendingMidiRequest.READ_MEGAPHONE_7:
                            case PendingMidiRequest.READ_MEGAPHONE_8:
                                VT4.UserMegaphone[pendingMidiRequest - PendingMidiRequest.READ_MEGAPHONE_1] = new Megaphone(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_VOCODER_1:
                            case PendingMidiRequest.READ_VOCODER_2:
                            case PendingMidiRequest.READ_VOCODER_3:
                            case PendingMidiRequest.READ_VOCODER_4:
                            case PendingMidiRequest.READ_VOCODER_5:
                            case PendingMidiRequest.READ_VOCODER_6:
                            case PendingMidiRequest.READ_VOCODER_7:
                            case PendingMidiRequest.READ_VOCODER_8:
                                VT4.UserVocoder[pendingMidiRequest - PendingMidiRequest.READ_VOCODER_1] = new Vocoder(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_HARMONY_1:
                            case PendingMidiRequest.READ_HARMONY_2:
                            case PendingMidiRequest.READ_HARMONY_3:
                            case PendingMidiRequest.READ_HARMONY_4:
                            case PendingMidiRequest.READ_HARMONY_5:
                            case PendingMidiRequest.READ_HARMONY_6:
                            case PendingMidiRequest.READ_HARMONY_7:
                            case PendingMidiRequest.READ_HARMONY_8:
                                VT4.UserHarmony[pendingMidiRequest - PendingMidiRequest.READ_HARMONY_1] = new Harmony(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.READ_REVERB_1:
                            case PendingMidiRequest.READ_REVERB_2:
                            case PendingMidiRequest.READ_REVERB_3:
                            case PendingMidiRequest.READ_REVERB_4:
                            case PendingMidiRequest.READ_REVERB_5:
                            case PendingMidiRequest.READ_REVERB_6:
                            case PendingMidiRequest.READ_REVERB_7:
                            case PendingMidiRequest.READ_REVERB_8:
                                VT4.UserReverb[pendingMidiRequest - PendingMidiRequest.READ_REVERB_1] = new Reverb(MidiInBuffer);
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            case PendingMidiRequest.CHECKING:
                                pendingMidiRequest = PendingMidiRequest.DONE;
                                break;
                            default:
                                break; // Set breakpoint here to identify any unhandled messages in pendingMidiRequest.
                        }
                        break;
                }
            }
            else if (args.Message.Type == MidiMessageType.ProgramChange)
            {
                // Buttons 1 - 4 and MANUAL all behaves as radio buttons.
                // When one of the buttons are pressed, all the other are released.
                midiInBuffer = args.Message.RawData.ToArray();
                if (robotButtonIsDown)
                {
                    timerAction.Add(new MidiEvents(TimerAction.HANDLE_ROBOT_VARIATION_CHANGE, midiInBuffer));
                }
                if (megaphoneButtonIsDown)
                {
                    timerAction.Add(new MidiEvents(TimerAction.HANDLE_MEGAPHONE_VARIATION_CHANGE, midiInBuffer));
                }
                if (vocoderButtonIsDown)
                {
                    timerAction.Add(new MidiEvents(TimerAction.HANDLE_VOCODER_VARIATION_CHANGE, midiInBuffer));
                }
                if (harmonyButtonIsDown)
                {
                    timerAction.Add(new MidiEvents(TimerAction.HANDLE_HARMONY_VARIATION_CHANGE, midiInBuffer));
                }
                if (reverbButtonIsDown)
                {
                    timerAction.Add(new MidiEvents(TimerAction.HANDLE_REVERB_VARIATION_CHANGE, midiInBuffer));
                }
                if (!robotButtonIsDown && !megaphoneButtonIsDown && !vocoderButtonIsDown && !harmonyButtonIsDown && !reverbButtonIsDown)
                {
                    if (midiInBuffer[1] == 0)
                    {
                        sceneButtonPressed = - 1;
                        manualButtonIsDown = true;
                    }
                    else
                    {
                        sceneButtonPressed = midiInBuffer[1] - 1;
                        if (manualButtonIsDown)
                        {
                            sceneButtonPressed += 4;
                        }
                        manualButtonIsDown = false;
                        timerAction.Add(new MidiEvents(TimerAction.SELECT_PATCH_1, new byte[] { (byte)sceneButtonPressed }));
                    }
                }
            }
            else if (args.Message.Type == MidiMessageType.ControlChange)
            {
                // To avoid VT-4 PB hammering, always ignore if same id and value as last message:
                midiInBuffer = args.Message.RawData.ToArray();
                if (midiInBuffer[1] >= 0x31 && midiInBuffer[1] <= 0x35 && midiInBuffer[2] == 0x7f)
                {
                    switch (midiInBuffer[1])
                    {
                        case 0x31:
                            robotButtonIsDown = true;
                            measureRobotButtonDownTime = true;
                            timerAction.Add(new MidiEvents(TimerAction.HANDLE_EFFECT_BUTTON_PRESS, midiInBuffer));
                            break;
                        case 0x32:
                            megaphoneButtonIsDown = true;
                            measureMegaphoneButtonDownTime = true;
                            timerAction.Add(new MidiEvents(TimerAction.HANDLE_EFFECT_BUTTON_PRESS, midiInBuffer));
                            break;
                        case 0x33:
                            reverbButtonIsDown = true;
                            measureReverbButtonDownTime = true;
                            timerAction.Add(new MidiEvents(TimerAction.HANDLE_EFFECT_BUTTON_PRESS, midiInBuffer));
                            break;
                        case 0x34:
                            vocoderButtonIsDown = true;
                            measureVocoderButtonDownTime = true;
                            timerAction.Add(new MidiEvents(TimerAction.HANDLE_EFFECT_BUTTON_PRESS, midiInBuffer));
                            break;
                        case 0x35:
                            harmonyButtonIsDown = true;
                            measureHarmonyButtonDownTime = true;
                            timerAction.Add(new MidiEvents(TimerAction.HANDLE_EFFECT_BUTTON_PRESS, midiInBuffer));
                            break;
                    }
                }
                else if (midiInBuffer[1] != 48 && midiInBuffer[2] == 0x00)
                {
                    switch (midiInBuffer[1])
                    {
                        case 0x31:
                            robotButtonIsDown = false;
                            if (VT4.TemporaryPatch.ROBOT > 0)
                            {
                                timerAction.Add(new MidiEvents(TimerAction.HANDLE_EFFECT_BUTTON_PRESS, midiInBuffer));
                            }
                            break;
                        case 0x32:
                            megaphoneButtonIsDown = false;
                            if (VT4.TemporaryPatch.MEGAPHONE > 0)
                            {
                                timerAction.Add(new MidiEvents(TimerAction.HANDLE_EFFECT_BUTTON_PRESS, midiInBuffer));
                            }
                            break;
                        case 0x33:
                            reverbButtonIsDown = false;
                            if (VT4.TemporaryPatch.REVERB > 0)
                            {
                                timerAction.Add(new MidiEvents(TimerAction.HANDLE_EFFECT_BUTTON_PRESS, midiInBuffer));
                            }
                            break;
                        case 0x34:
                            vocoderButtonIsDown = false;
                            if (VT4.TemporaryPatch.VOCODER > 0)
                            {
                                timerAction.Add(new MidiEvents(TimerAction.HANDLE_EFFECT_BUTTON_PRESS, midiInBuffer));
                            }
                            break;
                        case 0x35:
                            harmonyButtonIsDown = false;
                            if (VT4.TemporaryPatch.HARMONY > 0)
                            {
                                timerAction.Add(new MidiEvents(TimerAction.HANDLE_EFFECT_BUTTON_PRESS, midiInBuffer));
                            }
                            break;
                    }
                }
                else
                { 
                    if (lastCC == null)
                    {
                        lastCC = new byte[3];
                        timerAction.Add(new MidiEvents(TimerAction.HANDLE_CC_MESSAGE, midiInBuffer));
                    }
                    else if (!(lastCC[0] == midiInBuffer[0] && lastCC[1] == midiInBuffer[1] && lastCC[2] == midiInBuffer[2])
                        || midiInBuffer[1] == 49 || midiInBuffer[1] == 50 || midiInBuffer[1] == 51)
                    {
                        timerAction.Add(new MidiEvents(TimerAction.HANDLE_CC_MESSAGE, midiInBuffer));
                    }
                    lastCC[0] = midiInBuffer[0];
                    lastCC[1] = midiInBuffer[1];
                    lastCC[2] = midiInBuffer[2];
                }
            }
            else if (args.Message.Type == MidiMessageType.PitchBendChange)
            {
                // To avoid VT-4 PB hammering, always ignore if same id and value as last message:
                midiInBuffer = args.Message.RawData.ToArray();
                if (lastPB == null)
                {
                    lastPB = new byte[3];
                    timerAction.Add(new MidiEvents(TimerAction.HANDLE_PB_MESSAGE, midiInBuffer));
                }
                else if (!(lastPB[0] == midiInBuffer[0] && lastPB[1] == midiInBuffer[1] && lastPB[2] == midiInBuffer[2]))
                {
                    timerAction.Add(new MidiEvents(TimerAction.HANDLE_PB_MESSAGE, midiInBuffer));
                }
                lastPB[0] = midiInBuffer[0];
                lastPB[1] = midiInBuffer[1];
                lastPB[2] = midiInBuffer[2];
            }
            else if (args.Message.Type == MidiMessageType.NoteOn)
            {
                // Receiving note on turns Robot on, so we need to indicate that:
                timerAction.Add(new VT_4.MidiEvents(TimerAction.TURN_ON_ROBOT, new byte[] { 0x02 }));
            }
        }

        private void HandleCcMessage(byte[] midiInBuffer)
        {
            switch (midiInBuffer[1])
            {
                case (Int32)CcMessages.VOLUME:
                    // Volume is not stored in memory, but saved in settings:
                    settings.Save("Volume", midiInBuffer[2]);
                    VT4.TemporaryPatch.GLOBAL_LEVEL = midiInBuffer[2];
                    knobVolume.Value= midiInBuffer[2];
                    break;
                case (Int32)CcMessages.MIC_SENS:
                    // Mic sens is not stored in memory, but saved in settings:
                    settings.Save("Mic sens", midiInBuffer[2]);
                    knobMicSens.Value= midiInBuffer[2];
                    break;
                case (Int32)CcMessages.KEY:
                    VT4.TemporaryPatch.KEY = midiInBuffer[2];
                    knobKey.Value = midiInBuffer[2];
                    break;
                case (Int32)CcMessages.AUTO_PITCH:
                    VT4.TemporaryPatch.AUTO_PITCH = midiInBuffer[2] * 2;
                    knobAutoPitch.Value = midiInBuffer[2];
                    break;
                case (Int32)CcMessages.FORMANT:
                    VT4.TemporaryPatch.FORMANT = midiInBuffer[2] * 2;
                    slFormant.Value = midiInBuffer[2];
                    break;
                case (Int32)CcMessages.BALANCE:
                    VT4.TemporaryPatch.BALANCE = midiInBuffer[2] * 2;
                    slBalance.Value = midiInBuffer[2];
                    break;
                case (Int32)CcMessages.REVERB_SLIDER:
                    VT4.TemporaryPatch.REVERB = midiInBuffer[2] * 2;
                    slReverb.Value = midiInBuffer[2];
                    pmbReverb.Set(slReverb.Value > 1);
                    break;
                case (Int32)CcMessages.BYPASS:
                    if (midiInBuffer[2] > 0)
                    {
                        bypass = !bypass;
                        imgBypassOn.Visibility = bypass ? Visibility.Visible : Visibility.Collapsed;
                    }
                    break;
            }

            // Turn on scene button changed blinking:
            if (sceneIndex > -1)
            {
                sceneEdited[sceneIndex] = true;
            }

        }

        private void HandlePbMessage()
        {
            if (handleControlEvents)
            {
                slPitch.Value = midiInBuffer[2];
            }
        }

        private async Task WaitForMidiRequestAnswered()
        {
            Int32 counter = 2000;
            while (counter-- > 0 && pendingMidiRequest != PendingMidiRequest.DONE)
            {
                Thread.Sleep(10);
            }
            if (pendingMidiRequest != PendingMidiRequest.DONE)
            {
                await DisplayConnectionProblem(1);
            }
        }
    }

    public class MidiEvents
    {
        public TimerAction EventType { get; set; }
        public byte[] EventData { get; set; }

        public MidiEvents(TimerAction EventType, byte[] EventData)
        {
            this.EventType = EventType;
            this.EventData = EventData;
        }
    }
}
