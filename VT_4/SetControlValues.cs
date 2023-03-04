using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace VT_4
{
    public sealed partial class MainPage : Page
    {
        public void SetControlValues()
        {
            SetVocoderControlValues();
            SetHarmonyControlValues();
            SetRobotControlValues();
            SetMegaphoneControlValues();
            SetReverbControlValues();
            SetEqualizerControlValues();
            SetSystemControlValues();
        }

        public void SetVocoderControlValues()
        {
            pmbVocoder.Children[1][0].Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_1;
            pmbVocoder.Children[1][1].Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_2;
            pmbVocoder.Children[1][2].Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_3;
            pmbVocoder.Children[1][3].Value = VT4.TemporaryVocoder.VOCODER_PARAMETER_4;
        }

        public void SetHarmonyControlValues()
        {
            pmbHarmony.Children[1][0].Value = VT4.TemporaryHarmony.HARMONY_1_LEVEL;
            pmbHarmony.Children[1][1].Value = VT4.TemporaryHarmony.HARMONY_2_LEVEL;
            pmbHarmony.Children[1][2].Value = VT4.TemporaryHarmony.HARMONY_3_LEVEL;
            pmbHarmony.Children[1][3].Value = VT4.TemporaryHarmony.HARMONY_1_KEY;
            pmbHarmony.Children[1][4].Value = VT4.TemporaryHarmony.HARMONY_2_KEY;
            pmbHarmony.Children[1][5].Value = VT4.TemporaryHarmony.HARMONY_3_KEY;
            pmbHarmony.Children[1][6].Value = VT4.TemporaryHarmony.HARMONY_1_GENDER;
            pmbHarmony.Children[1][7].Value = VT4.TemporaryHarmony.HARMONY_2_GENDER;
            pmbHarmony.Children[1][8].Value = VT4.TemporaryHarmony.HARMONY_3_GENDER;
        }

        public void SetRobotControlValues()
        {
            pmbRobot.Children[1][0].Value = VT4.TemporaryRobot.OCTAVE;
            pmbRobot.Children[1][1].Value = VT4.TemporaryRobot.FEEDBACK_SWITCH;
            pmbRobot.Children[1][2].Value = VT4.TemporaryRobot.FEEDBACK_RESONANCE;
            pmbRobot.Children[1][3].Value = VT4.TemporaryRobot.FEEDBACK_LEVEL;
        }

        public void SetMegaphoneControlValues()
        {
            pmbMegaphone.Children[1][0].Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_2;
            pmbMegaphone.Children[1][1].Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_3;
            pmbMegaphone.Children[1][2].Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_4;
            pmbMegaphone.Children[1][3].Value = VT4.TemporaryMegaphone.MEGAPHONE_PARAMETER_4;
        }

        public void SetReverbControlValues()
        {
            pmbReverb.Children[1][0].Value = VT4.TemporaryReverb.REVERB_PARAMETER_1;
            pmbReverb.Children[1][1].Value = VT4.TemporaryReverb.REVERB_PARAMETER_2;
            pmbReverb.Children[1][2].Value = VT4.TemporaryReverb.REVERB_PARAMETER_3;
            pmbReverb.Children[1][3].Value = VT4.TemporaryReverb.REVERB_PARAMETER_4;
        }

        public void SetEqualizerControlValues()
        {
            pmbEqualizer.Children[0][0].Set(VT4.TemporaryEqualizer.EQUALIZER_SWITCH > 0);
            pmbEqualizer.Children[0][1].Value = VT4.TemporaryEqualizer.EQUALIZER_LOW_SHELF_FREQUENCY;   // 64 => 4985
            pmbEqualizer.Children[0][2].Value = VT4.TemporaryEqualizer.EQUALIZER_LOW_SHELF_GAIN;        // 20 => 0
            pmbEqualizer.Children[0][3].Value = VT4.TemporaryEqualizer.EQUALIZER_LOW_MID_FREQUENCY;     // 32 => 2492
            pmbEqualizer.Children[0][4].Value = VT4.TemporaryEqualizer.EQUALIZER_LOW_MID_Q;             // 64 => 8.95
            pmbEqualizer.Children[0][5].Value = VT4.TemporaryEqualizer.EQUALIZER_LOW_MID_GAIN;          // 20 => 0
            pmbEqualizer.Children[0][6].Value = VT4.TemporaryEqualizer.EQUALIZER_HIGH_MID_FREQUENCY;    // 96 => 7559
            pmbEqualizer.Children[0][7].Value = VT4.TemporaryEqualizer.EQUALIZER_HIGH_MID_Q;            // 64 => 8.95
            pmbEqualizer.Children[0][8].Value = VT4.TemporaryEqualizer.EQUALIZER_HIGH_MID_GAIN;         // 20 => 0
            pmbEqualizer.Children[0][9].Value = VT4.TemporaryEqualizer.EQUALIZER_HIGH_SHELF_FREQUENCY;  // 64 => 4985
            pmbEqualizer.Children[0][10].Value = VT4.TemporaryEqualizer.EQUALIZER_HIGH_SHELF_GAIN;      // 20 => 0
        }   

        public void SetSystemControlValues()
        {
            pmbLevels.Children[0][0].Value = VT4.System.GATE_LEVEL;
            pmbLevels.Children[0][1].Value = VT4.System.LOW_CUT;
            pmbLevels.Children[0][2].Value = VT4.System.ENHANCER;
            pmbLevels.Children[0][3].Value = VT4.System.FORMANT_DEPTH;
            pmbLevels.Children[0][4].Value = VT4.System.USB_MIXING;

            pmbSwitches.Children[0][0].Set(VT4.System.MONITOR_MODE > 0);
            pmbSwitches.Children[0][1].Set(VT4.System.EXTERNAL_CARRIER > 0);
            pmbSwitches.Children[0][2].Set(VT4.System.MIDI_IN_MODE > 0);
            pmbSwitches.Children[0][3].Set(VT4.System.PITCH_AND_FORMANT_ROUTING > 0);
            pmbSwitches.Children[0][4].Set(VT4.System.MUTE_MODE > 0);
        }
    }
}
