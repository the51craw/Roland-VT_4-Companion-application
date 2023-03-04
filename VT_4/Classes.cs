using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    ///////////////////////////////////////////////////////////////////////////////// 
    /// Enumerations
    ///////////////////////////////////////////////////////////////////////////////// 
    #region enumerations

    //Parameters are often spread out over multiple lines.
    //While parsing, it is necessary to know what the parser
    // has found so far.This enum is used both to keep track
    //of what the parser has found so far, and what the current
    // line turned out to contain, in order to decide what to
    // do next.
    public enum ParameterStatus
    {
        NOTHING,
        NAME,
        VALUES,
        EXPLANATION,
        NAME_AND_VALUES,
        NAME_AND_EXPLANATION,
        VALUES_AND_EXPLANATION,
        NAME_VALUES_AND_EXPLANATION
    }

    public enum ControlType
    {
        NONE,
        POT,
        BUTTON,
        TOGGLEBUTTON,
        PAD,
        SLIDER,
        PITCH_BEND,
        XY_JOYSTICK,
        KEYBOARD,
    }

    #endregion enumerations

    ///////////////////////////////////////////////////////////////////////////////// 
    /// Classes
    ///////////////////////////////////////////////////////////////////////////////// 
    #region classes

    public static class HexInt
    {
        public static String hexChars = "0123456789ABCDEF";
        public static Int32 ToInt(String hex)
        {
            Int32 result = 0;
            char[] c = hex.ToUpper().ToCharArray();
            for (Int32 i = 0; i < c.Length; i++)
            {
                result *= 16;
                result += hexChars.IndexOf(c[i]);
            }
            return result;
        }

        public static byte ToByte(String hex)
        {
            byte result = 0;
            char[] c = hex.ToUpper().ToCharArray();
            for (Int32 i = 0; i < c.Length; i++)
            {
                result *= 16;
                result += (byte)hexChars.IndexOf(c[i]);
            }
            return result;
        }
    }

    public class Address
    {
        public String[] sAddress;
        public byte[] bAddress;
        public Int32 iAddress;

        public Address()
        {
            iAddress = 0;
            sAddress = new String[4];
        }

        public Address(String line)
        {
            String[] addressParts = line.Split(' ');
            Int32 i = 0;
            String address = "";
            while (i < addressParts.Length)
            {
                if (Hex.IsHex(addressParts[i]))
                {
                    address += addressParts[i] + " ";
                }
                i++;
            }

            if (i > 1)
            {
                SaveAddress(address.Trim().Split(' '));
            }
        }

        public Address(byte[] addressParts)
        {
            Int32 i = 0;
            Int32 value = 0;
            while (i < addressParts.Length)
            {
                value += addressParts[addressParts.Length - i - 1] * (Int32)Math.Pow(128, i);
                i++;
            }
            iAddress = value;
            sAddress = iAddressTosAddress(iAddress);
            bAddress = sAddressTobAddress(sAddress);
        }

        public Address(Address address)
        {
            this.sAddress = address.sAddress;
            this.iAddress = address.iAddress;
            this.bAddress = address.bAddress;
        }

        public Address(String[] address)
        {
            SaveAddress(address);
        }

        public Address(Int32 address)
        {
            iAddress = address;
            sAddress = iAddressTosAddress(iAddress);
            bAddress = sAddressTobAddress(sAddress);
        }

        public static Address operator +(Address a, Address b)
        {
            return new Address(a.iAddress + b.iAddress);
        }

        public static Address operator -(Address a, Address b)
        {
            return new Address(a.iAddress - b.iAddress);
        }

        public void SaveAddress(String[] address)
        {
            sAddress = new String[] { "00", "00", "00", "00" };
            Int32 j = 3;
            for (Int32 i = address.Length - 1; i >= 0; i--)
            {
                if (address[i].Length == 2 && Hex.IsHex(address[i]) && j < 4)
                {
                    sAddress[j--] = address[i];
                }
                else
                {
                    sAddress[j--] = "00";
                }
            }
            sAddressToiAddress(sAddress);
            bAddress = sAddressTobAddress(sAddress);
        }

        public void sAddressToiAddress(String[] address)
        {
            iAddress = 0;
            for (Int32 i = 0; i < address.Length; i++)
            {
                iAddress *= 128;
                iAddress += HexInt.ToInt(address[i]);
            }
        }

        public byte[] sAddressTobAddress(String[] address)
        {
            byte[] bytes = new byte[address.Length];
            for (Int32 i = 0; i < address.Length; i++)
            {
                bytes[i] = HexInt.ToByte(address[i]);
            }
            return bytes;
        }

        public String[] iAddressTosAddress(Int32 address)
        {
            String[] s = { "00", "00", "00", "00" };
            Int32 digit1 = address / (128 * 128 * 128);
            address -= digit1 * 128 * 128 * 128;
            Int32 digit2 = address / (128 * 128);
            address -= digit2 * 128 * 128;
            Int32 digit3 = address / (128);
            address -= digit3 * 128;
            Int32 digit4 = address;
            s[0] = digit1.ToString("X");
            s[1] = digit2.ToString("X");
            s[2] = digit3.ToString("X");
            s[3] = digit4.ToString("X");
            return s;
        }

        public override string ToString()
        {
            String temp = "";
            foreach (String s in sAddress)
            {
                temp += "0x" + s + ", ";
            }
            return temp.Trim(' ').TrimEnd(',');
        }

        public String ToSimpleString()
        {
            String temp = "";
            foreach (String s in sAddress)
            {
                if (s.Length < 2)
                {
                    temp += "0" + s + " ";
                }
                else
                {
                    temp += s + " ";
                }
            }
            return temp.Trim(' ');
        }
    }

    public class ValueRange
    {
        public String LowSign { get; set; }
        public String HighSign { get; set; }
        public Int32 LowValue { get; set; }
        public Int32 HighValue { get; set; }
        public Boolean IsValid { get; set; }

        public ValueRange(Int32 from, Int32 to)
        {
            LowSign = "";
            HighSign = "";
            LowValue = from;
            HighValue = to;
            IsValid = true;
        }

        public ValueRange(String from, String to)
        {
            IsValid = true;
            Int32 ifrom = Hex.KeyToNumber(from);
            Int32 ito = Hex.KeyToNumber(to);
            if (ifrom > 0)

                if (from.Contains("-"))
                {
                    from = from.Trim('-');
                    LowSign = "-";
                }
                else if (from.Contains("L"))
                {
                    from = from.Trim('L');
                    LowSign = "L";
                }
                else
                {
                    LowSign = " ";
                    LowValue = 0;
                }
            try
            {
                LowValue = Int32.Parse(from);
            }
            catch
            {
                try
                {
                    LowValue = Hex.KeyToNumber(from);
                }
                catch
                {
                    IsValid = false;
                }
            }

            if (to.Contains("-"))
            {
                to = to.Trim('-');
                HighSign = "-";
            }
            else if (to.Contains("L"))
            {
                to = to.Trim('L');
                HighSign = "L";
            }
            try
            {
                HighValue = Int32.Parse(to);
            }
            catch
            {
                try
                {
                    HighValue = Hex.KeyToNumber(to);
                }
                catch
                {
                    IsValid = false;
                }
            }
        }

        public override String ToString()
        {
            if (IsValid)
            {
                return LowSign + LowValue + "-" + HighSign + HighValue;
            }
            else
            {
                return "";
            }
        }
    }

    public static class Hex
    {
        public static Boolean IsHex(String s)
        {
            s = s.Trim().ToLower();
            foreach (char c in s)
            {
                if (!IsHexChar(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static Boolean IsHexChar(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f');
        }

        // This returns a key number from a key name string, e.g. C3, E-1 G#2 and Eb4
        // Where C-1 = key 0 and G8 =key 127
        public static Int32 KeyToNumber(String s)
        {
            String scale = "CDEFGAB";
            String digits = "012345678";
            Int32 adjustment = 0;
            Int32 octaveAdjustment = 1;

            if (s.Contains("#"))
            {
                s.Replace("#", "");
                adjustment = 1;
            }
            else if (s.Contains("Bb"))
            {
                s.Replace("b", "");
                adjustment = -1;
            }
            if (s.Contains("-"))
            {
                s.Replace("-", "");
                octaveAdjustment = -1;
            }
            Char[] parts = s.ToCharArray();
            if (!scale.Contains(parts[0].ToString()) || !digits.Contains(parts[1].ToString()))
            {
                return -1;
            }
            else
            {
                return ((Int32)parts[1] + 1) * octaveAdjustment + scale.IndexOf(parts[0]) + adjustment;
            }
        }
    }

    public static class Numerics
    {
        public static Int32 ExtractNumericValue(String s)
        {
            Int32 result = 0;
            String digits = "0123456789";
            Boolean found = false;
            for (Int32 i = 0; i < s.Length; i++)
            {
                Int32 pos = digits.IndexOf(s.ToCharArray()[i]);
                if (pos > -1)
                {
                    found = true;
                    result *= 10;
                    result += Int32.Parse(s.ToCharArray()[i].ToString());
                }
            }
            if (found)
            {
                return result;
            }
            else
            {
                return -1;
            }
        }

        public static String RemoveTrailingNumbers(String s)
        {
            String digits = "0123456789";
            Boolean done = false;

            while (!done && s.Length > 0)
            {
                if (digits.Contains(s.Remove(0, s.Length - 1)))
                {
                    s = s.Remove(s.Length - 1);
                }
                else
                {
                    done = true;
                }
            }
            return s;
        }
    }

    public class ControlTag
    {
        public Int32 Id { get { return id; } }
        public String Name { get; set; }
        public String Label { get; set; }
        public Int32 CC { get; set; }
        public Int32 Note { get; set; }
        public String SysExAddress { get; set; }
        public String SysExRange { get; set; }
        public Double Maximum { get; set; }
        public Double Minimum { get; set; }
        public Double Value { get; set; }
        public String Address { get; set; }
        public String Range { get; set; }
        public byte LowKey { get; set; }
        public byte HighKey { get; set; }
        public Boolean AutoReturn { get; set; }

        private Int32 id;

        public static Int32 NextId = 0;

        public ControlTag(String Name, Int32 Id = -1)
        {
            if (Id > -1)
            {
                id = Id;
            }
            else
            {
                this.id = NextId++;
            }
            this.Name = Name;
            this.Label = "";
            this.CC = -1;
            this.Note = -1;
            this.SysExAddress = "";
            this.SysExRange = "";
            this.Minimum = 0;
            this.Maximum = 127;
            this.Address = "";
            this.Range = "";
            this.Value = 0;
        }
    }

    public class PanelContent
    {
        public Int32 RowCount { get; set; }
        public Int32 ColumnCount { get; set; }
        public byte[] ModelID { get; set; }
        public String DeviceName { get; set; }
        public List<ControlInfo> Controls { get; set; }

        public PanelContent()
        {
            Controls = new List<ControlInfo>();
        }
    }
    public class ControlInfo
    {
        public Int32 Row { get; set; }
        public Int32 Column { get; set; }
        public Int32 RowSpan { get; set; }
        public Int32 ColumnSpan { get; set; }
        public String Name { get; set; }
        public String XAMLName { get; set; }
        public List<ControlTag> ControlTags { get; set; }
        public ControlType ControlType { get; set; }

        public ControlInfo()
        {
            ControlTags = new List<ControlTag>();
        }
    }

    public class EventHandlerSwitch
    {
        public Boolean HandleControlEvents { get { return handleControlEvents;  } }

        private Boolean handleControlEvents = true;
        private List<Boolean> previousHandleControlEvents = new List<Boolean>();

        public void PushHandleControlEvents(Boolean setHandleControlEvents = false)
        {
            previousHandleControlEvents.Add(handleControlEvents);
            handleControlEvents = setHandleControlEvents;
        }

        public void PopHandleControlEvents()
        {
            if (previousHandleControlEvents.Count > 0)
            {
                handleControlEvents = previousHandleControlEvents[previousHandleControlEvents.Count - 1];
                previousHandleControlEvents.RemoveAt(previousHandleControlEvents.Count - 1);
            }
            else
            {
                handleControlEvents = true;
            }
        }
    }

    public class KeyEvent
    {
        public byte KeyNumber { get; set; }
        public byte Velocity { get; set; }
        public Boolean KeyOn { get; set; }

        public KeyEvent(byte KeyNumber, byte Velocity = 64, Boolean KeyOn = true)
        {
            this.KeyNumber = KeyNumber;
            this.Velocity = Velocity;
            this.KeyOn = KeyOn;
        }
    }

    public class KeyTypeAndPositionList
    {
        public List<KeyTypeAndPosition> KeyTypeAndPositions;

        public KeyTypeAndPositionList()
        {
            KeyTypeAndPositions = new List<KeyTypeAndPosition>();
            KeyTypeAndPositions.Add(new KeyTypeAndPosition("C", 0));
            KeyTypeAndPositions.Add(new KeyTypeAndPosition("C#", 30));
            KeyTypeAndPositions.Add(new KeyTypeAndPosition("D", 50));
            KeyTypeAndPositions.Add(new KeyTypeAndPosition("D#", 90));
            KeyTypeAndPositions.Add(new KeyTypeAndPosition("E", 100));
            KeyTypeAndPositions.Add(new KeyTypeAndPosition("F", 150));
            KeyTypeAndPositions.Add(new KeyTypeAndPosition("F#", 180));
            KeyTypeAndPositions.Add(new KeyTypeAndPosition("G", 200));
            KeyTypeAndPositions.Add(new KeyTypeAndPosition("G#", 236));
            KeyTypeAndPositions.Add(new KeyTypeAndPosition("A", 250));
            KeyTypeAndPositions.Add(new KeyTypeAndPosition("A#", 290));
            KeyTypeAndPositions.Add(new KeyTypeAndPosition("H", 300));
        }
    }

    public class KeyTypeAndPosition
    {
        public String KeyName { get; set; }          // C, C#, etc. within one octave
        public Int32 HorizontalOffset { get; set; } // Distance from left edge

        public KeyTypeAndPosition(String KeyName, Int32 HorizontalOffset)
        {
            this.KeyName = KeyName;
            this.HorizontalOffset = HorizontalOffset;
        }
    }
    #endregion classes
}
