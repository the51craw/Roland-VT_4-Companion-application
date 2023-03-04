using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.Devices.Bluetooth.Advertisement;

namespace VT_4
{
    /// <summary>
    /// All effects has eight variations.
    /// Some effects has different sound engines for some variations,
    /// thus has different sets of parameters depending on variation.
    /// This is a map to make it possible to handle all parameter
    /// values the same way, i.e. it holds address values and value
    /// range lists for all effect parameters.
    /// 
    /// All effects has an Id, that's the main key.
    /// All parameters har a sub Id, that's the kay to the parameter.
    /// Then we need the value range, and maybe a separate handling
    /// of on/off parameters and other parameters that should present
    /// a text rather than a numeric value.
    /// </summary>
    /// 
    public class Paramters
    {
        public List<Parameter> ParameterList;
        public List<List<string>> ParameterNameList;

        public Paramters()
        {
            ParameterList = new List<Parameter>();
            ParameterNameList = new List<List<string>>();

            // Knobs
            ParameterList.Add(new Parameter(0x60, new object[] { 255 }));
            ParameterList.Add(new Parameter(0x60, new object[] { 255 }));
            ParameterList.Add(new Parameter(0x60, new object[] { 10 }));
            ParameterList.Add(new Parameter(0x60, new object[] { 255 }));

            // Sliders
            ParameterList.Add(new Parameter(0x60, new object[] { 255 }));
            ParameterList.Add(new Parameter(0x60, new object[] { 255 }));
            ParameterList.Add(new Parameter(0x60, new object[] { 255 }));
            ParameterList.Add(new Parameter(0x60, new object[] { 255 }));

            // Vocoder
            ParameterList.Add(new Parameter(0x60, new object[] {
                255, 255, 255, 255 }));

            // Harmony
            ParameterList.Add(new Parameter(0x30, new object[] {
                255, 255, 255, 10, 10, 10, 255, 255, 255 }));

            // Robot
            ParameterList.Add(new Parameter(0x20, new object[] {
                new string[] { "2OctDown", "OctDown", "NoTransp", "OctUp" },
                new string[] { "FB off", "FB on" },
                255, 255 }));

            // Megaphone
            ParameterList.Add(new Parameter(0x40, new object[] { 
                new string[] { "FB off", "FB on" },
                255, 255, 255, 255 }));

            // Reverb
            ParameterList.Add(new Parameter(0x50, new object[] { 
                255, 255, 255, 255 }));

            // Patch
            ParameterList.Add(new Parameter(-1, new object[] {
                new string[] { "Off", "On", "MIDI in" },
                new string[] { "Off", "On" },
                new string[] { "Off", "On" },
                new string[] { "Off", "On" },
                7, 7, 7, 7, 7, 255, 255, 255, 255, 255, 11, 255 }));

            ParameterNameList.Add((new string[] { "Vintage", "Advanced", "Talk box", "Spell toy", "AdvcOsc2", "Vp3HiCut", "TkbxOsc2", "SpTyOsc2" }).ToList());
            ParameterNameList.Add((new string[] { "+5", "+3", "+3&-4", "+3&+5", "+4", "Man&Wmn", "With Kid", "+O1&-O1" }).ToList());
            ParameterNameList.Add((new string[] { "Normal", "Octave-1", "Octave+1", "Feedback", "Octave-2", "FB&Oct-1", "FB&Oct+1", "FB&Res  " }).ToList());
            ParameterNameList.Add((new string[] { "Megaphon", "Radio", "BbdChors", "Strobo", "DistMgph", "DistRdio", "", "" }).ToList());
            ParameterNameList.Add((new string[] { "", "", "", "", "", "", "", "" }).ToList());
        }

        public class Parameter
        {
            public int Id;
            public List<object> Parameters;
            //public int ItemNumber;
            //public NumericParameter NumericParameter;
            //public TextParameter TextParameter;

            public Parameter(int id, object[] parameters)
            {
                Id = id;
                Parameters = new List<object>();
                foreach (var param in parameters.ToList())
                {
                    Parameters.Add(param);
                }
                //ItemNumber = item;
                //NumericParameter = new NumericParameter(maxValue);
                //if (strings == null)
                //{
                //    TextParameter = null;
                //}
                //else
                //{
                //    TextParameter = new TextParameter(strings);
                //}
            }
        }

        public class NumericParameter
        {
            public int MaxValue;

            public NumericParameter(int maxValue)
            {
                MaxValue = maxValue;
            }
        }

        public class TextParameter
        {
            public List<string> Texts;

            public TextParameter(string[] texts)
            {
                Texts = new List<string>();
                foreach (string text in texts.ToList())
                { 
                    Texts.Add(text);
                }
            }
        }
    }
}
