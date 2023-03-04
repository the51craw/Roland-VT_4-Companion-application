using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.UI.Xaml.Media;

namespace VT_4
{
	/// <summary>
	/// VT4 holds all memory data available in the VT-4 unit.
	/// Note that there are eight user effects available for
	/// each effect type. even though the MIDI implementation
	/// states that there are only four.
	/// 
	/// The VT-4 operats from temporary effect objects that
	/// are populated from User effect objects when a variation
	/// is selected.
	/// 
	/// The TV-4 holds eight sets of settings, called 'Scenes'.
	/// When a scene i selected, the corresponding effect settings
	/// are copied to the user temporary patches, and effect
	/// global, or common, settings are copied to temporary patch.
	/// </summary>
	public class VT4
    {
		public System System { get; set; }
        public Patch TemporaryPatch { get; set; }
        public Patch[] UserPatch { get; set; }
        public Robot TemporaryRobot { get; set; }
        public Robot[] UserRobot { get; set; }
        public Harmony TemporaryHarmony { get; set; }
        public Harmony[] UserHarmony { get; set; }
        public Vocoder TemporaryVocoder { get; set; }
        public Vocoder[] UserVocoder { get; set; }
        public Megaphone TemporaryMegaphone { get; set; }
        public Megaphone[] UserMegaphone { get; set; }
        public Reverb TemporaryReverb { get; set; }
        public Reverb[] UserReverb { get; set; }
        public Equalizer TemporaryEqualizer { get; set; }
		public Equalizer UserEqualizer { get; set; }

		public VT4()
        {
			System = new System();

			TemporaryPatch = new Patch();
			UserPatch = new Patch[8];
			TemporaryRobot = new Robot();
			UserRobot = new Robot[8];
			TemporaryHarmony = new Harmony();
			UserHarmony = new Harmony[8];
			TemporaryVocoder = new Vocoder();
			UserVocoder = new Vocoder[8];
			TemporaryMegaphone = new Megaphone();
			UserMegaphone = new Megaphone[8];
			TemporaryReverb = new Reverb();
			UserReverb = new Reverb[8];

            UserPatch = new Patch[8];
			UserRobot = new Robot[8];
			UserHarmony = new Harmony[8];
			UserVocoder = new Vocoder[8];
			UserMegaphone = new Megaphone[8];
			UserReverb = new Reverb[8];

			for (Int32 i = 0; i < 8; i++)
			{
				UserPatch[i] = new Patch();
			}

            for (Int32 i = 0; i < 8; i++)
			{
                UserRobot[i] = new Robot();
                UserMegaphone[i] = new Megaphone();
				UserVocoder[i] = new Vocoder();
				UserHarmony[i] = new Harmony();
				UserReverb[i] = new Reverb();
			}

            TemporaryEqualizer = new Equalizer();
            UserEqualizer = new Equalizer();
        }
    }

	/// <summary>
	/// System holds system specific settings that are
	/// not specific to any effect in particular.
	/// </summary>
	public class System
	{
		public byte MIDI_CH { get; set; }
		public byte GATE_LEVEL { get; set; }
		public byte LOW_CUT { get; set; }
		public byte ENHANCER { get; set; }
		public byte FORMANT_DEPTH { get; set; }
		public byte MONITOR_MODE { get; set; }
		public byte EXTERNAL_CARRIER { get; set; }
		public byte USB_MIXING { get; set; }
		public byte MIDI_IN_MODE { get; set; }
		public byte PITCH_AND_FORMANT_ROUTING { get; set; }
		public byte MUTE_MODE { get; set; }

		public System()
		{
			this.MIDI_CH = 0x00;
			this.GATE_LEVEL = 0x00;
			this.LOW_CUT = 0x00;
			this.ENHANCER = 0x00;
			this.FORMANT_DEPTH = 0x00;
			this.MONITOR_MODE = 0x00;
			this.EXTERNAL_CARRIER = 0x00;
			this.USB_MIXING = 0x00;
			this.MIDI_IN_MODE = 0x00;
			this.PITCH_AND_FORMANT_ROUTING = 0x00;
			this.MUTE_MODE = 0x00;
		}

		public System(byte MIDI_CH, byte GATE_LEVEL, byte LOW_CUT, byte ENHANCER, byte FORMANT_DEPTH, byte MONITOR_MODE,
				byte EXTERNAL_CARRIER, byte USB_MIXING, byte MIDI_IN_MODE, byte PITCH_AND_FORMANT_ROUTING, byte MUTE_MODE)
		{
			this.MIDI_CH = MIDI_CH;
			this.GATE_LEVEL = GATE_LEVEL;
			this.LOW_CUT = LOW_CUT;
			this.ENHANCER = ENHANCER;
			this.FORMANT_DEPTH = FORMANT_DEPTH;
			this.MONITOR_MODE = MONITOR_MODE;
			this.EXTERNAL_CARRIER = EXTERNAL_CARRIER;
			this.USB_MIXING = USB_MIXING;
			this.MIDI_IN_MODE = MIDI_IN_MODE;
			this.PITCH_AND_FORMANT_ROUTING = PITCH_AND_FORMANT_ROUTING;
			this.MUTE_MODE = MUTE_MODE;
		}

		public System(byte[] buffer)
		{
			this.MIDI_CH = buffer[12];
			this.GATE_LEVEL = buffer[13];
			this.LOW_CUT = buffer[14];
			this.ENHANCER = buffer[15];
			this.FORMANT_DEPTH = buffer[16];
			this.MONITOR_MODE = buffer[17];
			this.EXTERNAL_CARRIER = buffer[18];
			this.USB_MIXING = buffer[19];
			this.MIDI_IN_MODE = buffer[20];
			this.PITCH_AND_FORMANT_ROUTING = buffer[21];
			this.MUTE_MODE = buffer[22];
		}

		public System(System System)
		{
			this.MIDI_CH = System.MIDI_CH;
			this.GATE_LEVEL = System.GATE_LEVEL;
			this.LOW_CUT = System.LOW_CUT;
			this.ENHANCER = System.ENHANCER;
			this.FORMANT_DEPTH = System.FORMANT_DEPTH;
			this.MONITOR_MODE = System.MONITOR_MODE;
			this.EXTERNAL_CARRIER = System.EXTERNAL_CARRIER;
			this.USB_MIXING = System.USB_MIXING;
			this.MIDI_IN_MODE = System.MIDI_IN_MODE;
			this.PITCH_AND_FORMANT_ROUTING = System.PITCH_AND_FORMANT_ROUTING;
			this.MUTE_MODE = System.MUTE_MODE;
		}

		public byte[] AsBuffer()
		{
			return new byte[]
			{
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
					MUTE_MODE
			};
		}
	}

	/// <summary>
	/// Patch holds global settings for effects, 
	/// on/off switches, variation numbers and
	/// also front panel adjustable controls like
	/// slider and knob values.
	/// </summary>
	public class Patch
	{
		public byte ROBOT { get; set; }
		public byte HARMONY { get; set; }
		public byte VOCODER { get; set; }
		public byte MEGAPHONE { get; set; }
		public byte ROBOT_VARIATION { get; set; }
		public byte HARMONY_VARIATION { get; set; }
		public byte VOCODER_VARIATION { get; set; }
		public byte MEGAPHONE_VARIATION { get; set; }
		public byte REVERB_VARIATION { get; set; }
		public Int32 PITCH { get; set; }
		public Int32 FORMANT { get; set; }
		public Int32 BALANCE { get; set; }
		public Int32 REVERB { get; set; }
		public Int32 AUTO_PITCH { get; set; }
		public byte KEY { get; set; }
		public Int32 GLOBAL_LEVEL { get; set; }
		public Int32 NAME_00_03 { get; set; }
		public Int32 NAME_04_07 { get; set; }
        public String Name { get; set; }

        public Patch()
		{
			this.ROBOT = 0x00;
			this.HARMONY = 0x00;
			this.VOCODER = 0x00;
			this.MEGAPHONE = 0x00;
			this.ROBOT_VARIATION = 0x00;
			this.HARMONY_VARIATION = 0x00;
			this.VOCODER_VARIATION = 0x00;
			this.MEGAPHONE_VARIATION = 0x00;
			this.REVERB_VARIATION = 0x00;
			this.PITCH = 0;
			this.FORMANT = 0;
			this.BALANCE = 0;
			this.REVERB = 0;
			this.AUTO_PITCH = 0;
			this.KEY = 0x00;
			this.GLOBAL_LEVEL = 0;
			this.NAME_00_03 = 0;
			this.NAME_04_07 = 0;
		}

		public Patch(byte ROBOT, byte HARMONY, byte VOCODER, byte MEGAPHONE, byte ROBOT_VARIATION, byte HARMONY_VARIATION,
				byte VOCODER_VARIATION, byte MEGAPHONE_VARIATION, byte REVERB_VARIATION, Int32 PITCH, Int32 FORMANT, Int32 BALANCE,
				Int32 REVERB, Int32 AUTO_PITCH, byte KEY, Int32 GLOBAL_LEVEL, Int32 NAME_00_03, Int32 NAME_04_07)
		{
			this.ROBOT = ROBOT;
			this.HARMONY = HARMONY;
			this.VOCODER = VOCODER;
			this.MEGAPHONE = MEGAPHONE;
			this.ROBOT_VARIATION = ROBOT_VARIATION;
			this.HARMONY_VARIATION = HARMONY_VARIATION;
			this.VOCODER_VARIATION = VOCODER_VARIATION;
			this.MEGAPHONE_VARIATION = MEGAPHONE_VARIATION;
			this.REVERB_VARIATION = REVERB_VARIATION;
			this.PITCH = PITCH;
			this.FORMANT = FORMANT;
			this.BALANCE = BALANCE;
			this.REVERB = REVERB;
			this.AUTO_PITCH = AUTO_PITCH;
			this.KEY = KEY;
			this.GLOBAL_LEVEL = GLOBAL_LEVEL;
			this.NAME_00_03 = NAME_00_03;
			this.NAME_04_07 = NAME_04_07;
            this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
        }

        /// <summary>
        /// All 'two nibble' values are 0 - 255 in order to adapt
        /// to CC events. They are in the form 0000 aaaa, 0000 bbbb
        /// int the MIDI sepecification and are here read as bytes
        /// n+1 + n*16, e.g. this.PITCH = buffer[22] + 16 * buffer[21];
        /// </summary>
        /// <param name="buffer"></param>
        public Patch(byte[] buffer)
		{
			this.ROBOT = buffer[12];
			this.HARMONY = buffer[13];
			this.VOCODER = buffer[14];
			this.MEGAPHONE = buffer[15];
			this.ROBOT_VARIATION = buffer[16];
			this.HARMONY_VARIATION = buffer[17];
			this.VOCODER_VARIATION = buffer[18];
			this.MEGAPHONE_VARIATION = buffer[19];
			this.REVERB_VARIATION = buffer[20];
			this.PITCH = buffer[22] + 16 * buffer[21];
			this.FORMANT = buffer[24] + 16 * buffer[23];
            this.BALANCE = buffer[26] + 16 * buffer[25];
			this.REVERB = buffer[28] + 16 * buffer[27];
			this.AUTO_PITCH = buffer[30] + 16 * buffer[29];
			this.KEY = buffer[31];
			this.GLOBAL_LEVEL = buffer[33] + 16 * buffer[32];
			this.NAME_00_03 = buffer[41] + 16 * buffer[40] + 256 * buffer[39] + 4096 * buffer[38] + 65536 * buffer[37] + 1048576 * buffer[36] + 16777216 * buffer[35] + 268435456 * buffer[34];
			this.NAME_04_07 = buffer[49] + 16 * buffer[48] + 256 * buffer[47] + 4096 * buffer[46] + 65536 * buffer[45] + 1048576 * buffer[44] + 16777216 * buffer[43] + 268435456 * buffer[42];
            this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
        }

        public Patch(Patch Patch)
		{
			this.ROBOT = Patch.ROBOT;
			this.HARMONY = Patch.HARMONY;
			this.VOCODER = Patch.VOCODER;
			this.MEGAPHONE = Patch.MEGAPHONE;
			this.ROBOT_VARIATION = Patch.ROBOT_VARIATION;
			this.HARMONY_VARIATION = Patch.HARMONY_VARIATION;
			this.VOCODER_VARIATION = Patch.VOCODER_VARIATION;
			this.MEGAPHONE_VARIATION = Patch.MEGAPHONE_VARIATION;
			this.REVERB_VARIATION = Patch.REVERB_VARIATION;
			this.PITCH = Patch.PITCH;
			this.FORMANT = Patch.FORMANT;
			this.BALANCE = Patch.BALANCE;
			this.REVERB = Patch.REVERB;
			this.AUTO_PITCH = Patch.AUTO_PITCH;
			this.KEY = Patch.KEY;
			this.GLOBAL_LEVEL = Patch.GLOBAL_LEVEL;
			this.NAME_00_03 = Patch.NAME_00_03;
			this.NAME_04_07 = Patch.NAME_04_07;
            this.Name = Patch.Name;
        }

        public byte[] AsBuffer()
		{
			return new byte[]
			{
					ROBOT,
					HARMONY,
					VOCODER,
					MEGAPHONE,
					ROBOT_VARIATION,
					HARMONY_VARIATION,
					VOCODER_VARIATION,
					MEGAPHONE_VARIATION,
					REVERB_VARIATION,
				(byte)((PITCH / 16) % 0x10),
				(byte)((PITCH / 1) % 0x10),
				(byte)((FORMANT / 16) % 0x10),
				(byte)((FORMANT / 1) % 0x10),
				(byte)((BALANCE / 16) % 0x10),
				(byte)((BALANCE / 1) % 0x10),
				(byte)((REVERB / 16) % 0x10),
				(byte)((REVERB / 1) % 0x10),
				(byte)((AUTO_PITCH / 16) % 0x10),
				(byte)((AUTO_PITCH / 1) % 0x10),
					KEY,
				(byte)((GLOBAL_LEVEL / 16) % 0x10),
				(byte)((GLOBAL_LEVEL / 1) % 0x10),
				(byte)((NAME_00_03 / 268435456) % 0x10),
				(byte)((NAME_00_03 / 16777216) % 0x10),
				(byte)((NAME_00_03 / 1048576) % 0x10),
				(byte)((NAME_00_03 / 65536) % 0x10),
				(byte)((NAME_00_03 / 4096) % 0x10),
				(byte)((NAME_00_03 / 256) % 0x10),
				(byte)((NAME_00_03 / 16) % 0x10),
				(byte)((NAME_00_03 / 1) % 0x10),
				(byte)((NAME_04_07 / 268435456) % 0x10),
				(byte)((NAME_04_07 / 16777216) % 0x10),
				(byte)((NAME_04_07 / 1048576) % 0x10),
				(byte)((NAME_04_07 / 65536) % 0x10),
				(byte)((NAME_04_07 / 4096) % 0x10),
				(byte)((NAME_04_07 / 256) % 0x10),
				(byte)((NAME_04_07 / 16) % 0x10),
				(byte)((NAME_04_07 / 1) % 0x10)
			};
		}

		public Boolean IsSameAs(Patch patch)
        {
			return
				this.ROBOT == patch.ROBOT &&
				this.MEGAPHONE == patch.MEGAPHONE &&
				this.VOCODER == patch.VOCODER &&
				this.HARMONY == patch.HARMONY &&
				this.REVERB == patch.REVERB &&
				this.ROBOT_VARIATION == patch.ROBOT_VARIATION &&
				this.MEGAPHONE_VARIATION == patch.MEGAPHONE_VARIATION &&
				this.VOCODER_VARIATION == patch.VOCODER_VARIATION &&
				this.HARMONY_VARIATION == patch.HARMONY_VARIATION &&
				this.REVERB_VARIATION == patch.REVERB_VARIATION;
		}
	}

	/// <summary>
	/// The Robot class holds one set of settings for 
	/// the Robot effect. it is used for all eight
	/// user Robot objects, with their different
	/// settings, and for the temporary Robot which
	/// holds the data that is currently in effect.
	/// </summary>
	public class Robot
	{
		public byte OCTAVE { get; set; }
		public byte FEEDBACK_SWITCH { get; set; }
		public Int32 FEEDBACK_RESONANCE { get; set; }
		public Int32 FEEDBACK_LEVEL { get; set; }
		public Int32 NAME_00_03 { get; set; }
		public Int32 NAME_04_07 { get; set; }
		public String Name { get; set; }

		public Robot()
		{
			this.OCTAVE = 0x00;
			this.FEEDBACK_SWITCH = 0x00;
			this.FEEDBACK_RESONANCE = 0;
			this.FEEDBACK_LEVEL = 0;
			this.NAME_00_03 = 20202020;
			this.NAME_04_07 = 20202020;
			this.Name = "        ";
		}

		public Robot(byte OCTAVE, byte FEEDBACK_SWITCH, Int32 FEEDBACK_RESONANCE, Int32 FEEDBACK_LEVEL, Int32 NAME_00_03, Int32 NAME_04_07, String Name)
		{
			this.OCTAVE = OCTAVE;
			this.FEEDBACK_SWITCH = FEEDBACK_SWITCH;
			this.FEEDBACK_RESONANCE = FEEDBACK_RESONANCE;
			this.FEEDBACK_LEVEL = FEEDBACK_LEVEL;
			this.NAME_00_03 = NAME_00_03;
			this.NAME_04_07 = NAME_04_07;
			this.Name = Name;
		}

		public Robot(byte OCTAVE, byte FEEDBACK_SWITCH, Int32 FEEDBACK_RESONANCE, Int32 FEEDBACK_LEVEL, Int32 NAME_00_03, Int32 NAME_04_07)
		{
			this.OCTAVE = OCTAVE;
			this.FEEDBACK_SWITCH = FEEDBACK_SWITCH;
			this.FEEDBACK_RESONANCE = FEEDBACK_RESONANCE;
			this.FEEDBACK_LEVEL = FEEDBACK_LEVEL;
			this.NAME_00_03 = NAME_00_03;
			this.NAME_04_07 = NAME_04_07;
			this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
		}

        /// <summary>
        /// All 'two nibble' values are 0 - 255 in order to adapt
        /// to CC events. They are in the form 0000 aaaa, 0000 bbbb
        /// int the MIDI sepecification and are here read as bytes
        /// n+1 + n*16, e.g. this.PITCH = buffer[22] + 16 * buffer[21];
        /// </summary>
        /// <param name="buffer"></param>
		public Robot(byte[] buffer)
		{
			this.OCTAVE = buffer[12];
			this.FEEDBACK_SWITCH = buffer[13];
			this.FEEDBACK_RESONANCE = buffer[15] + 16 * buffer[14];
			this.FEEDBACK_LEVEL = buffer[17] + 16 * buffer[16];
			this.NAME_00_03 = buffer[26] + 16 * buffer[25] + 256 * buffer[24] + 4096 * buffer[23] + 65536 * buffer[22] + 1048576 * buffer[21] + 16777216 * buffer[20] + 268435456 * buffer[19];
			this.NAME_04_07 = buffer[34] + 16 * buffer[33] + 256 * buffer[32] + 4096 * buffer[31] + 65536 * buffer[30] + 1048576 * buffer[29] + 16777216 * buffer[28] + 268435456 * buffer[27];
            this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
        }

        public Robot(Robot Robot)
		{
			this.OCTAVE = Robot.OCTAVE;
			this.FEEDBACK_SWITCH = Robot.FEEDBACK_SWITCH;
			this.FEEDBACK_RESONANCE = Robot.FEEDBACK_RESONANCE;
			this.FEEDBACK_LEVEL = Robot.FEEDBACK_LEVEL;
			this.NAME_00_03 = Robot.NAME_00_03;
			this.NAME_04_07 = Robot.NAME_04_07;
			this.Name = Robot.Name;
		}

		public byte[] AsBuffer()
		{
			return new byte[]
			{
					OCTAVE,
					FEEDBACK_SWITCH,
                (byte)((FEEDBACK_RESONANCE / 16) % 0x10),
                (byte)((FEEDBACK_RESONANCE / 1) % 0x10),
                (byte)((FEEDBACK_LEVEL / 16) % 0x10),
                (byte)((FEEDBACK_LEVEL / 1) % 0x10),
                    0x00,
                (byte)(NAME_00_03 / 0x10000000 & 0x0f),
                (byte)(NAME_00_03 / 0x1000000 & 0x0f),
                (byte)(NAME_00_03 / 0x100000 & 0x0f),
                (byte)(NAME_00_03 / 0x10000 & 0x0f),
                (byte)(NAME_00_03 / 0x1000 & 0x0f),
                (byte)(NAME_00_03 / 0x100 & 0x0f),
                (byte)(NAME_00_03 / 0x10 & 0x0f),
                (byte)(NAME_00_03 / 0x1 & 0x0f),
                (byte)(NAME_04_07 / 0x10000000 & 0x0f),
                (byte)(NAME_04_07 / 0x1000000 & 0x0f),
                (byte)(NAME_04_07 / 0x100000 & 0x0f),
                (byte)(NAME_04_07 / 0x10000 & 0x0f),
                (byte)(NAME_04_07 / 0x1000 & 0x0f),
                (byte)(NAME_04_07 / 0x100 & 0x0f),
                (byte)(NAME_04_07 / 0x10 & 0x0f),
                (byte)(NAME_04_07 / 0x1 & 0x0f),
			};
		}
	}

    /// <summary>
    /// The Harmony class holds one set of settings for 
    /// the Harmony effect. it is used for all eight
    /// user Harmony objects, with their different
    /// settings, and for the temporary Harmony which
    /// holds the data that is currently in effect.
    /// </summary>
    public class Harmony
	{
		public Int32 HARMONY_1_LEVEL { get; set; }
		public Int32 HARMONY_2_LEVEL { get; set; }
		public Int32 HARMONY_3_LEVEL { get; set; }
		public byte HARMONY_1_KEY { get; set; }
		public byte HARMONY_2_KEY { get; set; }
		public byte HARMONY_3_KEY { get; set; }
		public Int32 HARMONY_1_GENDER { get; set; }
		public Int32 HARMONY_2_GENDER { get; set; }
		public Int32 HARMONY_3_GENDER { get; set; }
		public Int32 NAME_00_03 { get; set; }
		public Int32 NAME_04_07 { get; set; }
		public String Name { get; set; }

		public Harmony()
		{
			this.HARMONY_1_LEVEL = 0;
			this.HARMONY_2_LEVEL = 0;
			this.HARMONY_3_LEVEL = 0;
			this.HARMONY_1_KEY = 0x00;
			this.HARMONY_2_KEY = 0x00;
			this.HARMONY_3_KEY = 0x00;
			this.HARMONY_1_GENDER = 0;
			this.HARMONY_2_GENDER = 0;
			this.HARMONY_3_GENDER = 0;
			this.NAME_00_03 = 0;
			this.NAME_04_07 = 0;
            this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
        }

        public Harmony(Int32 HARMONY_1_LEVEL, Int32 HARMONY_2_LEVEL, Int32 HARMONY_3_LEVEL, byte HARMONY_1_KEY, byte HARMONY_2_KEY, byte HARMONY_3_KEY,
				Int32 HARMONY_1_GENDER, Int32 HARMONY_2_GENDER, Int32 HARMONY_3_GENDER, Int32 NAME_00_03, Int32 NAME_04_07)
		{
			this.HARMONY_1_LEVEL = HARMONY_1_LEVEL;
			this.HARMONY_2_LEVEL = HARMONY_2_LEVEL;
			this.HARMONY_3_LEVEL = HARMONY_3_LEVEL;
			this.HARMONY_1_KEY = HARMONY_1_KEY;
			this.HARMONY_2_KEY = HARMONY_2_KEY;
			this.HARMONY_3_KEY = HARMONY_3_KEY;
			this.HARMONY_1_GENDER = HARMONY_1_GENDER;
			this.HARMONY_2_GENDER = HARMONY_2_GENDER;
			this.HARMONY_3_GENDER = HARMONY_3_GENDER;
			this.NAME_00_03 = NAME_00_03;
			this.NAME_04_07 = NAME_04_07;
            this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
        }

        public Harmony(Int32 HARMONY_1_LEVEL, Int32 HARMONY_2_LEVEL, Int32 HARMONY_3_LEVEL, byte HARMONY_1_KEY, byte HARMONY_2_KEY, byte HARMONY_3_KEY,
				Int32 HARMONY_1_GENDER, Int32 HARMONY_2_GENDER, Int32 HARMONY_3_GENDER, Int32 NAME_00_03, Int32 NAME_04_07, String Name)
		{
			this.HARMONY_1_LEVEL = HARMONY_1_LEVEL;
			this.HARMONY_2_LEVEL = HARMONY_2_LEVEL;
			this.HARMONY_3_LEVEL = HARMONY_3_LEVEL;
			this.HARMONY_1_KEY = HARMONY_1_KEY;
			this.HARMONY_2_KEY = HARMONY_2_KEY;
			this.HARMONY_3_KEY = HARMONY_3_KEY;
			this.HARMONY_1_GENDER = HARMONY_1_GENDER;
			this.HARMONY_2_GENDER = HARMONY_2_GENDER;
			this.HARMONY_3_GENDER = HARMONY_3_GENDER;
			this.NAME_00_03 = NAME_00_03;
			this.NAME_04_07 = NAME_04_07;
			this.Name = Name;
		}

        /// <summary>
        /// All 'two nibble' values are 0 - 255 in order to adapt
        /// to CC events. They are in the form 0000 aaaa, 0000 bbbb
        /// int the MIDI sepecification and are here read as bytes
        /// n+1 + n*16, e.g. this.PITCH = buffer[22] + 16 * buffer[21];
        /// </summary>
        /// <param name="buffer"></param>
		public Harmony(byte[] buffer)
		{
			this.HARMONY_1_LEVEL = buffer[13] + 16 * buffer[12];
			this.HARMONY_2_LEVEL = buffer[15] + 16 * buffer[14];
			this.HARMONY_3_LEVEL = buffer[17] + 16 * buffer[16];
			this.HARMONY_1_KEY = buffer[18];
			this.HARMONY_2_KEY = buffer[19];
			this.HARMONY_3_KEY = buffer[20];
			this.HARMONY_1_GENDER = buffer[22] + 16 * buffer[21];
			this.HARMONY_2_GENDER = buffer[24] + 16 * buffer[23];
			this.HARMONY_3_GENDER = buffer[26] + 16 * buffer[25];
			this.NAME_00_03 = buffer[38] + 16 * buffer[37] + 256 * buffer[36] + 4096 * buffer[35] + 65536 * buffer[34] + 1048576 * buffer[33] + 16777216 * buffer[32] + 268435456 * buffer[31];
			this.NAME_04_07 = buffer[46] + 16 * buffer[45] + 256 * buffer[44] + 4096 * buffer[43] + 65536 * buffer[42] + 1048576 * buffer[41] + 16777216 * buffer[40] + 268435456 * buffer[39];
			this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
		}

		public Harmony(Harmony Harmony)
		{
			this.HARMONY_1_LEVEL = Harmony.HARMONY_1_LEVEL;
			this.HARMONY_2_LEVEL = Harmony.HARMONY_2_LEVEL;
			this.HARMONY_3_LEVEL = Harmony.HARMONY_3_LEVEL;
			this.HARMONY_1_KEY = Harmony.HARMONY_1_KEY;
			this.HARMONY_2_KEY = Harmony.HARMONY_2_KEY;
			this.HARMONY_3_KEY = Harmony.HARMONY_3_KEY;
			this.HARMONY_1_GENDER = Harmony.HARMONY_1_GENDER;
			this.HARMONY_2_GENDER = Harmony.HARMONY_2_GENDER;
			this.HARMONY_3_GENDER = Harmony.HARMONY_3_GENDER;
			this.NAME_00_03 = Harmony.NAME_00_03;
			this.NAME_04_07 = Harmony.NAME_04_07;
			this.Name = Harmony.Name;
		}

		public byte[] AsBuffer()
		{
			return new byte[]
			{
				(byte)((HARMONY_1_LEVEL / 16) % 0x10),
				(byte)((HARMONY_1_LEVEL / 1) % 0x10),
				(byte)((HARMONY_2_LEVEL / 16) % 0x10),
				(byte)((HARMONY_2_LEVEL / 1) % 0x10),
				(byte)((HARMONY_3_LEVEL / 16) % 0x10),
				(byte)((HARMONY_3_LEVEL / 1) % 0x10),
					HARMONY_1_KEY,
					HARMONY_2_KEY,
					HARMONY_3_KEY,
				(byte)((HARMONY_1_GENDER / 16) % 0x10),
				(byte)((HARMONY_1_GENDER / 1) % 0x10),
				(byte)((HARMONY_2_GENDER / 16) % 0x10),
				(byte)((HARMONY_2_GENDER / 1) % 0x10),
				(byte)((HARMONY_3_GENDER / 16) % 0x10),
				(byte)((HARMONY_3_GENDER / 1) % 0x10),
					0x00,
					0x00,
					0x00,
					0x00,
				(byte)((NAME_00_03 / 268435456) % 0x10),
				(byte)((NAME_00_03 / 16777216) % 0x10),
				(byte)((NAME_00_03 / 1048576) % 0x10),
				(byte)((NAME_00_03 / 65536) % 0x10),
				(byte)((NAME_00_03 / 4096) % 0x10),
				(byte)((NAME_00_03 / 256) % 0x10),
				(byte)((NAME_00_03 / 16) % 0x10),
				(byte)((NAME_00_03 / 1) % 0x10),
				(byte)((NAME_04_07 / 268435456) % 0x10),
				(byte)((NAME_04_07 / 16777216) % 0x10),
				(byte)((NAME_04_07 / 1048576) % 0x10),
				(byte)((NAME_04_07 / 65536) % 0x10),
				(byte)((NAME_04_07 / 4096) % 0x10),
				(byte)((NAME_04_07 / 256) % 0x10),
				(byte)((NAME_04_07 / 16) % 0x10),
				(byte)((NAME_04_07 / 1) % 0x10)
			};
		}
	}

    /// <summary>
    /// The Megaphone class holds one set of settings for 
    /// the Megaphone effect. it is used for all eight
    /// user Megaphone objects, with their different
    /// settings, and for the temporary Megaphone which
    /// holds the data that is currently in effect.
    /// </summary>
	public class Megaphone
	{
		public byte MEGAPHONE_TYPE { get; set; }
		public Int32 MEGAPHONE_PARAMETER_1 { get; set; }
		public Int32 MEGAPHONE_PARAMETER_2 { get; set; }
		public Int32 MEGAPHONE_PARAMETER_3 { get; set; }
		public Int32 MEGAPHONE_PARAMETER_4 { get; set; }
		public Int32 NAME_00_03 { get; set; }
		public Int32 NAME_04_07 { get; set; }
		public String Name { get; set; }

		public Megaphone()
		{
			this.MEGAPHONE_TYPE = 0x00;
			this.MEGAPHONE_PARAMETER_1 = 0;
			this.MEGAPHONE_PARAMETER_2 = 0;
			this.MEGAPHONE_PARAMETER_3 = 0;
			this.MEGAPHONE_PARAMETER_4 = 0;
			this.NAME_00_03 = 0;
			this.NAME_04_07 = 0;
			this.Name = "";
		}

		public Megaphone(byte MEGAPHONE_TYPE, Int32 MEGAPHONE_PARAMETER_1, Int32 MEGAPHONE_PARAMETER_2, Int32 MEGAPHONE_PARAMETER_3, Int32 MEGAPHONE_PARAMETER_4, Int32 NAME_00_03,
				Int32 NAME_04_07, String Name)
		{
			this.MEGAPHONE_TYPE = MEGAPHONE_TYPE;
			this.MEGAPHONE_PARAMETER_1 = MEGAPHONE_PARAMETER_1;
			this.MEGAPHONE_PARAMETER_2 = MEGAPHONE_PARAMETER_2;
			this.MEGAPHONE_PARAMETER_3 = MEGAPHONE_PARAMETER_3;
			this.MEGAPHONE_PARAMETER_4 = MEGAPHONE_PARAMETER_4;
			this.NAME_00_03 = NAME_00_03;
			this.NAME_04_07 = NAME_04_07;
			this.Name = Name;
		}

		public Megaphone(byte MEGAPHONE_TYPE, Int32 MEGAPHONE_PARAMETER_1, Int32 MEGAPHONE_PARAMETER_2, Int32 MEGAPHONE_PARAMETER_3, Int32 MEGAPHONE_PARAMETER_4, Int32 NAME_00_03,
				Int32 NAME_04_07)
		{
			this.MEGAPHONE_TYPE = MEGAPHONE_TYPE;
			this.MEGAPHONE_PARAMETER_1 = MEGAPHONE_PARAMETER_1;
			this.MEGAPHONE_PARAMETER_2 = MEGAPHONE_PARAMETER_2;
			this.MEGAPHONE_PARAMETER_3 = MEGAPHONE_PARAMETER_3;
			this.MEGAPHONE_PARAMETER_4 = MEGAPHONE_PARAMETER_4;
			this.NAME_00_03 = NAME_00_03;
			this.NAME_04_07 = NAME_04_07;
			this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
		}

        /// <summary>
        /// All 'two nibble' values are 0 - 255 in order to adapt
        /// to CC events. They are in the form 0000 aaaa, 0000 bbbb
        /// int the MIDI sepecification and are here read as bytes
        /// n+1 + n*16, e.g. this.PITCH = buffer[22] + 16 * buffer[21];
        /// </summary>
        /// <param name="buffer"></param>
		public Megaphone(byte[] buffer)
		{
			this.MEGAPHONE_TYPE = buffer[12];
			this.MEGAPHONE_PARAMETER_1 = buffer[14] + 16 * buffer[13];
			this.MEGAPHONE_PARAMETER_2 = buffer[16] + 16 * buffer[15];
			this.MEGAPHONE_PARAMETER_3 = buffer[18] + 16 * buffer[17];
			this.MEGAPHONE_PARAMETER_4 = buffer[20] + 16 * buffer[19];
			this.NAME_00_03 = buffer[31] + 16 * buffer[30] + 256 * buffer[29] + 4096 * buffer[28] + 65536 * buffer[27] + 1048576 * buffer[26] + 16777216 * buffer[25] + 268435456 * buffer[24];
			this.NAME_04_07 = buffer[39] + 16 * buffer[38] + 256 * buffer[37] + 4096 * buffer[36] + 65536 * buffer[35] + 1048576 * buffer[34] + 16777216 * buffer[33] + 268435456 * buffer[32];
			this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
		}

		public Megaphone(Megaphone Megaphone)
		{
			this.MEGAPHONE_TYPE = Megaphone.MEGAPHONE_TYPE;
			this.MEGAPHONE_PARAMETER_1 = Megaphone.MEGAPHONE_PARAMETER_1;
			this.MEGAPHONE_PARAMETER_2 = Megaphone.MEGAPHONE_PARAMETER_2;
			this.MEGAPHONE_PARAMETER_3 = Megaphone.MEGAPHONE_PARAMETER_3;
			this.MEGAPHONE_PARAMETER_4 = Megaphone.MEGAPHONE_PARAMETER_4;
			this.NAME_00_03 = Megaphone.NAME_00_03;
			this.NAME_04_07 = Megaphone.NAME_04_07;
			this.Name = Megaphone.Name;
		}

		public byte[] AsBuffer()
		{
			return new byte[]
			{
					MEGAPHONE_TYPE,
				(byte)((MEGAPHONE_PARAMETER_1 / 16) % 0x10),
				(byte)((MEGAPHONE_PARAMETER_1 / 1) % 0x10),
				(byte)((MEGAPHONE_PARAMETER_2 / 16) % 0x10),
				(byte)((MEGAPHONE_PARAMETER_2 / 1) % 0x10),
				(byte)((MEGAPHONE_PARAMETER_3 / 16) % 0x10),
				(byte)((MEGAPHONE_PARAMETER_3 / 1) % 0x10),
				(byte)((MEGAPHONE_PARAMETER_4 / 16) % 0x10),
				(byte)((MEGAPHONE_PARAMETER_4 / 1) % 0x10),
					0x00,
					0x00,
					0x00,
				(byte)((NAME_00_03 / 268435456) % 0x10),
				(byte)((NAME_00_03 / 16777216) % 0x10),
				(byte)((NAME_00_03 / 1048576) % 0x10),
				(byte)((NAME_00_03 / 65536) % 0x10),
				(byte)((NAME_00_03 / 4096) % 0x10),
				(byte)((NAME_00_03 / 256) % 0x10),
				(byte)((NAME_00_03 / 16) % 0x10),
				(byte)((NAME_00_03 / 1) % 0x10),
				(byte)((NAME_04_07 / 268435456) % 0x10),
				(byte)((NAME_04_07 / 16777216) % 0x10),
				(byte)((NAME_04_07 / 1048576) % 0x10),
				(byte)((NAME_04_07 / 65536) % 0x10),
				(byte)((NAME_04_07 / 4096) % 0x10),
				(byte)((NAME_04_07 / 256) % 0x10),
				(byte)((NAME_04_07 / 16) % 0x10),
				(byte)((NAME_04_07 / 1) % 0x10)
			};
		}
	}

    /// <summary>
    /// The Reverb class holds one set of settings for 
    /// the Reverb effect. it is used for all eight
    /// user Reverb objects, with their different
    /// settings, and for the temporary Reverb which
    /// holds the data that is currently in effect.
    /// </summary>
	public class Reverb
	{
		public byte REVERB_TYPE { get; set; }
		public Int32 REVERB_PARAMETER_1 { get; set; }
		public Int32 REVERB_PARAMETER_2 { get; set; }
		public Int32 REVERB_PARAMETER_3 { get; set; }
		public Int32 REVERB_PARAMETER_4 { get; set; }
		public Int32 NAME_00_03 { get; set; }
		public Int32 NAME_04_07 { get; set; }
		public String Name { get; set; }

		public Reverb()
		{
			this.REVERB_TYPE = 0x00;
			this.REVERB_PARAMETER_1 = 0;
			this.REVERB_PARAMETER_2 = 0;
			this.REVERB_PARAMETER_3 = 0;
			this.REVERB_PARAMETER_4 = 0;
			this.NAME_00_03 = 0;
			this.NAME_04_07 = 0;
			this.Name = "";
		}

		public Reverb(byte REVERB_TYPE, Int32 REVERB_PARAMETER_1, Int32 REVERB_PARAMETER_2, Int32 REVERB_PARAMETER_3, Int32 REVERB_PARAMETER_4, Int32 NAME_00_03,
				Int32 NAME_04_07, String Name)
		{
			this.REVERB_TYPE = REVERB_TYPE;
			this.REVERB_PARAMETER_1 = REVERB_PARAMETER_1;
			this.REVERB_PARAMETER_2 = REVERB_PARAMETER_2;
			this.REVERB_PARAMETER_3 = REVERB_PARAMETER_3;
			this.REVERB_PARAMETER_4 = REVERB_PARAMETER_4;
			this.NAME_00_03 = NAME_00_03;
			this.NAME_04_07 = NAME_04_07;
			this.Name = Name;
		}

		public Reverb(byte REVERB_TYPE, Int32 REVERB_PARAMETER_1, Int32 REVERB_PARAMETER_2, Int32 REVERB_PARAMETER_3, Int32 REVERB_PARAMETER_4, Int32 NAME_00_03,
				Int32 NAME_04_07)
		{
			this.REVERB_TYPE = REVERB_TYPE;
			this.REVERB_PARAMETER_1 = REVERB_PARAMETER_1;
			this.REVERB_PARAMETER_2 = REVERB_PARAMETER_2;
			this.REVERB_PARAMETER_3 = REVERB_PARAMETER_3;
			this.REVERB_PARAMETER_4 = REVERB_PARAMETER_4;
			this.NAME_00_03 = NAME_00_03;
			this.NAME_04_07 = NAME_04_07;
			this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
		}

        /// <summary>
        /// All 'two nibble' values are 0 - 255 in order to adapt
        /// to CC events. They are in the form 0000 aaaa, 0000 bbbb
        /// int the MIDI sepecification and are here read as bytes
        /// n+1 + n*16, e.g. this.PITCH = buffer[22] + 16 * buffer[21];
        /// </summary>
        /// <param name="buffer"></param>
		public Reverb(byte[] buffer)
		{
			this.REVERB_TYPE = buffer[12];
			this.REVERB_PARAMETER_1 = buffer[14] + 16 * buffer[13];
			this.REVERB_PARAMETER_2 = buffer[16] + 16 * buffer[15];
			this.REVERB_PARAMETER_3 = buffer[18] + 16 * buffer[17];
			this.REVERB_PARAMETER_4 = buffer[20] + 16 * buffer[19];
			this.NAME_00_03 = buffer[31] + 16 * buffer[30] + 256 * buffer[29] + 4096 * buffer[28] + 65536 * buffer[27] + 1048576 * buffer[26] + 16777216 * buffer[25] + 268435456 * buffer[24];
			this.NAME_04_07 = buffer[39] + 16 * buffer[38] + 256 * buffer[37] + 4096 * buffer[36] + 65536 * buffer[35] + 1048576 * buffer[34] + 16777216 * buffer[33] + 268435456 * buffer[32];
			this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
		}

		public Reverb(Reverb Reverb)
		{
			this.REVERB_TYPE = Reverb.REVERB_TYPE;
			this.REVERB_PARAMETER_1 = Reverb.REVERB_PARAMETER_1;
			this.REVERB_PARAMETER_2 = Reverb.REVERB_PARAMETER_2;
			this.REVERB_PARAMETER_3 = Reverb.REVERB_PARAMETER_3;
			this.REVERB_PARAMETER_4 = Reverb.REVERB_PARAMETER_4;
			this.NAME_00_03 = Reverb.NAME_00_03;
			this.NAME_04_07 = Reverb.NAME_04_07;
			this.Name = Reverb.Name;
		}

		public byte[] AsBuffer()
		{
			return new byte[]
			{
					REVERB_TYPE,
				(byte)((REVERB_PARAMETER_1 / 16) % 0x10),
				(byte)((REVERB_PARAMETER_1 / 1) % 0x10),
				(byte)((REVERB_PARAMETER_2 / 16) % 0x10),
				(byte)((REVERB_PARAMETER_2 / 1) % 0x10),
				(byte)((REVERB_PARAMETER_3 / 16) % 0x10),
				(byte)((REVERB_PARAMETER_3 / 1) % 0x10),
				(byte)((REVERB_PARAMETER_4 / 16) % 0x10),
				(byte)((REVERB_PARAMETER_4 / 1) % 0x10),
					0x00,
					0x00,
					0x00,
				(byte)((NAME_00_03 / 268435456) % 0x10),
				(byte)((NAME_00_03 / 16777216) % 0x10),
				(byte)((NAME_00_03 / 1048576) % 0x10),
				(byte)((NAME_00_03 / 65536) % 0x10),
				(byte)((NAME_00_03 / 4096) % 0x10),
				(byte)((NAME_00_03 / 256) % 0x10),
				(byte)((NAME_00_03 / 16) % 0x10),
				(byte)((NAME_00_03 / 1) % 0x10),
				(byte)((NAME_04_07 / 268435456) % 0x10),
				(byte)((NAME_04_07 / 16777216) % 0x10),
				(byte)((NAME_04_07 / 1048576) % 0x10),
				(byte)((NAME_04_07 / 65536) % 0x10),
				(byte)((NAME_04_07 / 4096) % 0x10),
				(byte)((NAME_04_07 / 256) % 0x10),
				(byte)((NAME_04_07 / 16) % 0x10),
				(byte)((NAME_04_07 / 1) % 0x10)
			};
		}
	}

    /// <summary>
    /// The Vocoder class holds one set of settings for 
    /// the Vocoder effect. it is used for all eight
    /// user Vocoder objects, with their different
    /// settings, and for the temporary Vocoder which
    /// holds the data that is currently in effect.
    /// </summary>
    public class Vocoder
	{
		public byte VOCODER_TYPE { get; set; }
		public Int32 VOCODER_PARAMETER_1 { get; set; }
		public Int32 VOCODER_PARAMETER_2 { get; set; }
		public Int32 VOCODER_PARAMETER_3 { get; set; }
		public Int32 VOCODER_PARAMETER_4 { get; set; }
		public Int32 NAME_00_03 { get; set; }
		public Int32 NAME_04_07 { get; set; }
		public String Name { get; set; }

		public Vocoder()
		{
			this.VOCODER_TYPE = 0x00;
			this.VOCODER_PARAMETER_1 = 0;
			this.VOCODER_PARAMETER_2 = 0;
			this.VOCODER_PARAMETER_3 = 0;
			this.VOCODER_PARAMETER_4 = 0;
			this.NAME_00_03 = 0;
			this.NAME_04_07 = 0;
			this.Name = "";
		}

		public Vocoder(byte VOCODER_TYPE, Int32 VOCODER_PARAMETER_1, Int32 VOCODER_PARAMETER_2, Int32 VOCODER_PARAMETER_3, Int32 VOCODER_PARAMETER_4, Int32 NAME_00_03,
				Int32 NAME_04_07, String Name)
		{
			this.VOCODER_TYPE = VOCODER_TYPE;
			this.VOCODER_PARAMETER_1 = VOCODER_PARAMETER_1;
			this.VOCODER_PARAMETER_2 = VOCODER_PARAMETER_2;
			this.VOCODER_PARAMETER_3 = VOCODER_PARAMETER_3;
			this.VOCODER_PARAMETER_4 = VOCODER_PARAMETER_4;
			this.NAME_00_03 = NAME_00_03;
			this.NAME_04_07 = NAME_04_07;
			this.Name = Name;
		}

		public Vocoder(byte VOCODER_TYPE, Int32 VOCODER_PARAMETER_1, Int32 VOCODER_PARAMETER_2, Int32 VOCODER_PARAMETER_3, Int32 VOCODER_PARAMETER_4, Int32 NAME_00_03,
				Int32 NAME_04_07)
		{
			this.VOCODER_TYPE = VOCODER_TYPE;
			this.VOCODER_PARAMETER_1 = VOCODER_PARAMETER_1;
			this.VOCODER_PARAMETER_2 = VOCODER_PARAMETER_2;
			this.VOCODER_PARAMETER_3 = VOCODER_PARAMETER_3;
			this.VOCODER_PARAMETER_4 = VOCODER_PARAMETER_4;
			this.NAME_00_03 = NAME_00_03;
			this.NAME_04_07 = NAME_04_07;
			this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
		}

        /// <summary>
        /// All 'two nibble' values are 0 - 255 in order to adapt
        /// to CC events. They are in the form 0000 aaaa, 0000 bbbb
        /// int the MIDI sepecification and are here read as bytes
        /// n+1 + n*16, e.g. this.PITCH = buffer[22] + 16 * buffer[21];
        /// </summary>
        /// <param name="buffer"></param>
		public Vocoder(byte[] buffer)
		{
			this.VOCODER_TYPE = buffer[12];
			this.VOCODER_PARAMETER_1 = buffer[14] + 16 * buffer[13];
			this.VOCODER_PARAMETER_2 = buffer[16] + 16 * buffer[15];
			this.VOCODER_PARAMETER_3 = buffer[18] + 16 * buffer[17];
			this.VOCODER_PARAMETER_4 = buffer[20] + 16 * buffer[19];
			this.NAME_00_03 = buffer[31] + 16 * buffer[30] + 256 * buffer[29] + 4096 * buffer[28] + 65536 * buffer[27] + 1048576 * buffer[26] + 16777216 * buffer[25] + 268435456 * buffer[24];
			this.NAME_04_07 = buffer[39] + 16 * buffer[38] + 256 * buffer[37] + 4096 * buffer[36] + 65536 * buffer[35] + 1048576 * buffer[34] + 16777216 * buffer[33] + 268435456 * buffer[32];
			this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
		}

		public Vocoder(Vocoder Vocoder)
		{
			this.VOCODER_TYPE = Vocoder.VOCODER_TYPE;
			this.VOCODER_PARAMETER_1 = Vocoder.VOCODER_PARAMETER_1;
			this.VOCODER_PARAMETER_2 = Vocoder.VOCODER_PARAMETER_2;
			this.VOCODER_PARAMETER_3 = Vocoder.VOCODER_PARAMETER_3;
			this.VOCODER_PARAMETER_4 = Vocoder.VOCODER_PARAMETER_4;
			this.NAME_00_03 = Vocoder.NAME_00_03;
			this.NAME_04_07 = Vocoder.NAME_04_07;
			this.Name = Vocoder.Name;
		}

		public byte[] AsBuffer()
		{
			return new byte[]
			{
					VOCODER_TYPE,
				(byte)((VOCODER_PARAMETER_1 / 16) % 0x10),
				(byte)((VOCODER_PARAMETER_1 / 1) % 0x10),
				(byte)((VOCODER_PARAMETER_2 / 16) % 0x10),
				(byte)((VOCODER_PARAMETER_2 / 1) % 0x10),
				(byte)((VOCODER_PARAMETER_3 / 16) % 0x10),
				(byte)((VOCODER_PARAMETER_3 / 1) % 0x10),
				(byte)((VOCODER_PARAMETER_4 / 16) % 0x10),
				(byte)((VOCODER_PARAMETER_4 / 1) % 0x10),
					0x00,
					0x00,
					0x00,
				(byte)((NAME_00_03 / 268435456) % 0x10),
				(byte)((NAME_00_03 / 16777216) % 0x10),
				(byte)((NAME_00_03 / 1048576) % 0x10),
				(byte)((NAME_00_03 / 65536) % 0x10),
				(byte)((NAME_00_03 / 4096) % 0x10),
				(byte)((NAME_00_03 / 256) % 0x10),
				(byte)((NAME_00_03 / 16) % 0x10),
				(byte)((NAME_00_03 / 1) % 0x10),
				(byte)((NAME_04_07 / 268435456) % 0x10),
				(byte)((NAME_04_07 / 16777216) % 0x10),
				(byte)((NAME_04_07 / 1048576) % 0x10),
				(byte)((NAME_04_07 / 65536) % 0x10),
				(byte)((NAME_04_07 / 4096) % 0x10),
				(byte)((NAME_04_07 / 256) % 0x10),
				(byte)((NAME_04_07 / 16) % 0x10),
				(byte)((NAME_04_07 / 1) % 0x10)
			};
		}
	}

	/// <summary>
	/// The Equalizer class holds all equalizer settings.
	/// Even though there is only one equlizer, it is used
	/// in User Equalizer as well as Temporary equalizer.
	/// </summary>
	public class Equalizer
	{
		public byte EQUALIZER_SWITCH { get; set; }
		public byte EQUALIZER_LOW_SHELF_FREQUENCY { get; set; }
		public byte EQUALIZER_LOW_SHELF_GAIN { get; set; }
		public byte EQUALIZER_LOW_MID_FREQUENCY { get; set; }
		public byte EQUALIZER_LOW_MID_Q { get; set; }
		public byte EQUALIZER_LOW_MID_GAIN { get; set; }
		public byte EQUALIZER_HIGH_MID_FREQUENCY { get; set; }
		public byte EQUALIZER_HIGH_MID_Q { get; set; }
		public byte EQUALIZER_HIGH_MID_GAIN { get; set; }
		public byte EQUALIZER_HIGH_SHELF_FREQUENCY { get; set; }
		public byte EQUALIZER_HIGH_SHELF_GAIN { get; set; }
		public Int32 NAME_00_03 { get; set; }
		public Int32 NAME_04_07 { get; set; }
		public string Name { get; set; }

		public Equalizer()
		{
			this.EQUALIZER_SWITCH = 0x00;
			this.EQUALIZER_LOW_SHELF_FREQUENCY = 0x00;
			this.EQUALIZER_LOW_SHELF_GAIN = 0x00;
			this.EQUALIZER_LOW_MID_FREQUENCY = 0x00;
			this.EQUALIZER_LOW_MID_Q = 0x00;
			this.EQUALIZER_LOW_MID_GAIN = 0x00;
			this.EQUALIZER_HIGH_MID_FREQUENCY = 0x00;
			this.EQUALIZER_HIGH_MID_Q = 0x00;
			this.EQUALIZER_HIGH_MID_GAIN = 0x00;
			this.EQUALIZER_HIGH_SHELF_FREQUENCY = 0x00;
			this.EQUALIZER_HIGH_SHELF_GAIN = 0x00;
			this.NAME_00_03 = 0;
			this.NAME_04_07 = 0;
			this.Name = "";
		}

		public Equalizer(byte EQUALIZER, byte EQUALIZER_LOW_SHELF_FREQUENCY, byte EQUALIZER_LOW_SHELF_GAIN, byte EQUALIZER_LOW_MID_FREQUENCY, byte EQUALIZER_LOW_MID_Q, byte EQUALIZER_LOW_MID_GAIN,
				byte EQUALIZER_HIGH_MID_FREQUENCY, byte EQUALIZER_HIGH_MID_Q, byte EQUALIZER_HIGH_MID_GAIN, byte EQUALIZER_HIGH_SHELF_FREQUENCY, byte EQUALIZER_HIGH_SHELF_GAIN, Int32 NAME_00_03,
				Int32 NAME_04_07)
		{
			this.EQUALIZER_SWITCH = EQUALIZER;
			this.EQUALIZER_LOW_SHELF_FREQUENCY = EQUALIZER_LOW_SHELF_FREQUENCY;
			this.EQUALIZER_LOW_SHELF_GAIN = EQUALIZER_LOW_SHELF_GAIN;
			this.EQUALIZER_LOW_MID_FREQUENCY = EQUALIZER_LOW_MID_FREQUENCY;
			this.EQUALIZER_LOW_MID_Q = EQUALIZER_LOW_MID_Q;
			this.EQUALIZER_LOW_MID_GAIN = EQUALIZER_LOW_MID_GAIN;
			this.EQUALIZER_HIGH_MID_FREQUENCY = EQUALIZER_HIGH_MID_FREQUENCY;
			this.EQUALIZER_HIGH_MID_Q = EQUALIZER_HIGH_MID_Q;
			this.EQUALIZER_HIGH_MID_GAIN = EQUALIZER_HIGH_MID_GAIN;
			this.EQUALIZER_HIGH_SHELF_FREQUENCY = EQUALIZER_HIGH_SHELF_FREQUENCY;
			this.EQUALIZER_HIGH_SHELF_GAIN = EQUALIZER_HIGH_SHELF_GAIN;
			this.NAME_00_03 = NAME_00_03;
			this.NAME_04_07 = NAME_04_07;
            this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
        }

        /// <summary>
        /// All 'two nibble' values are 0 - 255 in order to adapt
        /// to CC events. They are in the form 0000 aaaa, 0000 bbbb
        /// int the MIDI sepecification and are here read as bytes
        /// n+1 + n*16, e.g. this.PITCH = buffer[22] + 16 * buffer[21];
        /// </summary>
        /// <param name="buffer"></param>
        public Equalizer(byte[] buffer)
		{
			this.EQUALIZER_SWITCH = buffer[12];
			this.EQUALIZER_LOW_SHELF_FREQUENCY = buffer[13];
			this.EQUALIZER_LOW_SHELF_GAIN = buffer[14];
			this.EQUALIZER_LOW_MID_FREQUENCY = buffer[15];
			this.EQUALIZER_LOW_MID_Q = buffer[16];
			this.EQUALIZER_LOW_MID_GAIN = buffer[17];
			this.EQUALIZER_HIGH_MID_FREQUENCY = buffer[18];
			this.EQUALIZER_HIGH_MID_Q = buffer[19];
			this.EQUALIZER_HIGH_MID_GAIN = buffer[20];
			this.EQUALIZER_HIGH_SHELF_FREQUENCY = buffer[21];
			this.EQUALIZER_HIGH_SHELF_GAIN = buffer[22];
			this.NAME_00_03 = buffer[35] + 16 * buffer[34] + 256 * buffer[33] + 4096 * buffer[32] + 65536 * buffer[31] + 1048576 * buffer[30] + 16777216 * buffer[29] + 268435456 * buffer[28];
			this.NAME_04_07 = buffer[43] + 16 * buffer[42] + 256 * buffer[41] + 4096 * buffer[40] + 65536 * buffer[39] + 1048576 * buffer[38] + 16777216 * buffer[37] + 268435456 * buffer[36];
            this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
        }

        public Equalizer(Equalizer Equalizer)
		{
			this.EQUALIZER_SWITCH = Equalizer.EQUALIZER_SWITCH;
			this.EQUALIZER_LOW_SHELF_FREQUENCY = Equalizer.EQUALIZER_LOW_SHELF_FREQUENCY;
			this.EQUALIZER_LOW_SHELF_GAIN = Equalizer.EQUALIZER_LOW_SHELF_GAIN;
			this.EQUALIZER_LOW_MID_FREQUENCY = Equalizer.EQUALIZER_LOW_MID_FREQUENCY;
			this.EQUALIZER_LOW_MID_Q = Equalizer.EQUALIZER_LOW_MID_Q;
			this.EQUALIZER_LOW_MID_GAIN = Equalizer.EQUALIZER_LOW_MID_GAIN;
			this.EQUALIZER_HIGH_MID_FREQUENCY = Equalizer.EQUALIZER_HIGH_MID_FREQUENCY;
			this.EQUALIZER_HIGH_MID_Q = Equalizer.EQUALIZER_HIGH_MID_Q;
			this.EQUALIZER_HIGH_MID_GAIN = Equalizer.EQUALIZER_HIGH_MID_GAIN;
			this.EQUALIZER_HIGH_SHELF_FREQUENCY = Equalizer.EQUALIZER_HIGH_SHELF_FREQUENCY;
			this.EQUALIZER_HIGH_SHELF_GAIN = Equalizer.EQUALIZER_HIGH_SHELF_GAIN;
			this.NAME_00_03 = Equalizer.NAME_00_03;
			this.NAME_04_07 = Equalizer.NAME_04_07;
            this.Name = new NameConverter(NAME_00_03, NAME_04_07).Name;
        }

        public byte[] AsBuffer()
		{
			return new byte[]
			{
					EQUALIZER_SWITCH,
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
					0x00,
					0x00,
					0x00,
					0x00,
					0x00,
				(byte)((NAME_00_03 / 268435456) % 0x10),
				(byte)((NAME_00_03 / 16777216) % 0x10),
				(byte)((NAME_00_03 / 1048576) % 0x10),
				(byte)((NAME_00_03 / 65536) % 0x10),
				(byte)((NAME_00_03 / 4096) % 0x10),
				(byte)((NAME_00_03 / 256) % 0x10),
				(byte)((NAME_00_03 / 16) % 0x10),
				(byte)((NAME_00_03 / 1) % 0x10),
				(byte)((NAME_04_07 / 268435456) % 0x10),
				(byte)((NAME_04_07 / 16777216) % 0x10),
				(byte)((NAME_04_07 / 1048576) % 0x10),
				(byte)((NAME_04_07 / 65536) % 0x10),
				(byte)((NAME_04_07 / 4096) % 0x10),
				(byte)((NAME_04_07 / 256) % 0x10),
				(byte)((NAME_04_07 / 16) % 0x10),
				(byte)((NAME_04_07 / 1) % 0x10)
			};
		}
	}

	public class NameConverter
    {
		public String Name { get; set; }
		public Int32 Numeric1 { get; set; }
		public Int32 Numeric2 { get; set; }

		private byte[] b1;
		private byte[] b2;
		private String s1;
		private String s2;

		public NameConverter()
        {
			Name = "        ";
			Numeric1 = 538976288;
			Numeric2 = 538976288;
		}

		public NameConverter(Int32 Numeric1, Int32 Numeric2)
        {
			this.Numeric1 = Numeric1;
			this.Numeric2 = Numeric2;
			NumericToString();
		}

		public NameConverter(String Name)
        {
			this.Name = Name;
			if (this.Name.Length > 8)
            {
				this.Name = this.Name.Remove(this.Name.Length - 1);
            }
			while (this.Name.Length < 8)
            {
				this.Name += " ";
            }
			StringToNumeric();
        }

		private void NumericToString()
        {
			b1 = Unpack(Numeric1);
			b2 = Unpack(Numeric2);
			s1 = "";
			s2 = "";
			for (Int32 i = 0; i < 4; i++)
            {
				s1 += (char)b1[i];
				s2 += (char)b2[i];
			}
			Name = s1 + s2;
		}

		private void StringToNumeric()
        {
			char[] c1 = Name.ToCharArray(0, 4);
			char[] c2 = Name.ToCharArray(4, 4);
			b1 = new byte[4];
			b2 = new byte[4];
			Numeric1 = 0;
			Numeric2 = 0;
			for (Int32 i = 0; i < 4; i++)
            {
				b1[i] = (byte)c1[i];
				b2[i] = (byte)c2[i];
				Numeric1 *= 256;
				Numeric2 *= 256;
				Numeric1 += (Int32)b1[i];
				Numeric2 += (Int32)b2[i];
			}

		}

		private byte[] Unpack(Int32 value)
        {
			byte[] b = new byte[4];
			for (Int32 i = 3; i > -1; i--)
			{
				b[i] = (byte)(value % 256);
				value /= 256;
			}
			return b;
        }
	}
}

