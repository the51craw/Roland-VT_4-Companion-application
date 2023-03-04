using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using static VT_4.MainPage;

namespace VT_4
{
    public class Settings
    {
        public ApplicationDataContainer LocalSettings;

        private MainPage mainPage;

        public Settings(MainPage mainPage)
        {
            this.mainPage = mainPage;
            LocalSettings = ApplicationData.Current.LocalSettings;
        }

        public void Load()
        {
            //if (LocalSettings.Values.ContainsKey("Robot"))
            //{
            //    try
            //    {
            //        string[] robotVariants = ((string)LocalSettings.Values["Robot"]).Split('|');
            //        if (robotVariants.Length == mainPage.vt4MenuItems[2][0].Count)
            //        {
            //            for (int i = 0; i < mainPage.vt4MenuItems[2][0].Count; i++)
            //            {
            //                mainPage.vt4MenuItems[2][0][i].TextBlock.Text = robotVariants[i];
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        InitVariationText(2);
            //    }
            //}
            //else
            //{
            //    InitVariationText(2);
            //}

            //if (LocalSettings.Values.ContainsKey("Megaphone"))
            //{
            //    try
            //    {
            //        string[] megaphoneVariants = ((string)LocalSettings.Values["Megaphone"]).Split('|');
            //        if (megaphoneVariants.Length == mainPage.vt4MenuItems[3][0].Count)
            //        {
            //            for (int i = 0; i < mainPage.vt4MenuItems[3][0].Count; i++)
            //            {
            //                mainPage.vt4MenuItems[3][0][i].TextBlock.Text = megaphoneVariants[i];
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        InitVariationText(3);
            //    }
            //}
            //else
            //{
            //    InitVariationText(3);
            //}

            //if (LocalSettings.Values.ContainsKey("Vocoder"))
            //{
            //    try
            //    {
            //        string[] vocoderVariants = ((string)LocalSettings.Values["Vocoder"]).Split('|');
            //        if (vocoderVariants.Length == mainPage.vt4MenuItems[0][0].Count)
            //        {
            //            for (int i = 0; i < mainPage.vt4MenuItems[0][0].Count; i++)
            //            {
            //                mainPage.vt4MenuItems[0][0][i].TextBlock.Text = vocoderVariants[i];
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        InitVariationText(0);
            //    }
            //}
            //else
            //{
            //    InitVariationText(0);
            //}

            //if (LocalSettings.Values.ContainsKey("Harmony"))
            //{
            //    try
            //    {
            //        string[] harmonyVariants = ((string)LocalSettings.Values["Harmony"]).Split('|');
            //        if (harmonyVariants.Length == mainPage.vt4MenuItems[1][0].Count)
            //        {
            //            for (int i = 0; i < mainPage.vt4MenuItems[1][0].Count; i++)
            //            {
            //                mainPage.vt4MenuItems[1][0][i].TextBlock.Text = harmonyVariants[i];
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        InitVariationText(1);
            //    }
            //}
            //else
            //{
            //    InitVariationText(1);
            //}

            //if (LocalSettings.Values.ContainsKey("Reverb"))
            //{
            //    try
            //    {
            //        string[] reverbVariants = ((string)LocalSettings.Values["Reverb"]).Split('|');
            //        if (reverbVariants.Length == mainPage.vt4MenuItems[4][0].Count)
            //        {
            //            for (int i = 0; i < mainPage.vt4MenuItems[4][0].Count; i++)
            //            {
            //                mainPage.vt4MenuItems[4][0][i].TextBlock.Text = reverbVariants[i];
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        InitVariationText(4);
            //    }
            //}
            //else
            //{
            //    InitVariationText(4);
            //}

            Save();
        }

        /// <summary>
        /// Parameter names are not included in VT-4 memory so
        /// it is set here instead.
        /// </summary>
        /// <param name="index"></param>
        //public void InitVariationText(int index)
        //{
        //    switch (index)
        //    {
        //        case 0:
        //            mainPage.vt4MenuItems[0][0][0].TextBlock.Text = "Advanced";
        //            mainPage.vt4MenuItems[0][0][1].TextBlock.Text = "VP-330  ";
        //            mainPage.vt4MenuItems[0][0][2].TextBlock.Text = "Talk box";
        //            mainPage.vt4MenuItems[0][0][3].TextBlock.Text = "SpellToy";
        //            mainPage.vt4MenuItems[0][0][4].TextBlock.Text = "AdvnOsc2";
        //            mainPage.vt4MenuItems[0][0][5].TextBlock.Text = "Vp3HiCut";
        //            mainPage.vt4MenuItems[0][0][6].TextBlock.Text = "TkbxOsc2";
        //            mainPage.vt4MenuItems[0][0][7].TextBlock.Text = "SpTyOsc2";
        //            break;
        //        case 1:
        //            mainPage.vt4MenuItems[1][0][0].TextBlock.Text = "+5      ";
        //            mainPage.vt4MenuItems[1][0][1].TextBlock.Text = "+3      ";
        //            mainPage.vt4MenuItems[1][0][2].TextBlock.Text = "+3&-4   ";
        //            mainPage.vt4MenuItems[1][0][3].TextBlock.Text = "+3&+5   ";
        //            mainPage.vt4MenuItems[1][0][4].TextBlock.Text = "+4      ";
        //            mainPage.vt4MenuItems[1][0][5].TextBlock.Text = "Man&Wmn ";
        //            mainPage.vt4MenuItems[1][0][6].TextBlock.Text = "With Kid";
        //            mainPage.vt4MenuItems[1][0][7].TextBlock.Text = "+O1&-O1 ";
        //            break;
        //        case 2:
        //            mainPage.vt4MenuItems[2][0][0].TextBlock.Text = "Normal  ";
        //            mainPage.vt4MenuItems[2][0][1].TextBlock.Text = "Octave-1";
        //            mainPage.vt4MenuItems[2][0][2].TextBlock.Text = "Octave+1";
        //            mainPage.vt4MenuItems[2][0][3].TextBlock.Text = "Feedback";
        //            mainPage.vt4MenuItems[2][0][4].TextBlock.Text = "Octave-2";
        //            mainPage.vt4MenuItems[2][0][5].TextBlock.Text = "FB&Oct-1";
        //            mainPage.vt4MenuItems[2][0][6].TextBlock.Text = "FB&Oct+1";
        //            mainPage.vt4MenuItems[2][0][7].TextBlock.Text = "FB&Res  ";
        //            break;
        //        case 3:
        //            mainPage.vt4MenuItems[3][0][0].TextBlock.Text = "Megaphon";
        //            mainPage.vt4MenuItems[3][0][1].TextBlock.Text = "Radio   ";
        //            mainPage.vt4MenuItems[3][0][2].TextBlock.Text = "BbdChors";
        //            mainPage.vt4MenuItems[3][0][3].TextBlock.Text = "Strobo  ";
        //            mainPage.vt4MenuItems[3][0][4].TextBlock.Text = "DistMgph";
        //            mainPage.vt4MenuItems[3][0][5].TextBlock.Text = "DistRdio";
        //            mainPage.vt4MenuItems[3][0][6].TextBlock.Text = "Vibrate ";
        //            mainPage.vt4MenuItems[3][0][7].TextBlock.Text = "SlowStrb";
        //            break;
        //        case 4:
        //            mainPage.vt4MenuItems[4][0][0].TextBlock.Text = "Reverb  ";
        //            mainPage.vt4MenuItems[4][0][1].TextBlock.Text = "Echo    ";
        //            mainPage.vt4MenuItems[4][0][2].TextBlock.Text = "TempoDly";
        //            mainPage.vt4MenuItems[4][0][3].TextBlock.Text = "Dub echo";
        //            mainPage.vt4MenuItems[4][0][4].TextBlock.Text = "Wide Rvb";
        //            mainPage.vt4MenuItems[4][0][5].TextBlock.Text = "LongEcho";
        //            mainPage.vt4MenuItems[4][0][6].TextBlock.Text = "PanDelay";
        //            mainPage.vt4MenuItems[4][0][7].TextBlock.Text = "PanDubEc";
        //            break;
        //    }
        //}

        //public void SetRobotParameterTexts()
        //{
        //    try
        //    {
        //        mainPage.vt4MenuItems[2][1][0].TextBlock.Text = "Octave " + (mainPage.vt4MenuItems[2][1][0].Value - 2).ToString();
        //        mainPage.vt4MenuItems[2][1][1].TextBlock.Text = "Feedback Switch " + (mainPage.vt4MenuItems[2][1][1].Value == 0 ? "Off" : "On");
        //        mainPage.vt4MenuItems[2][1][2].TextBlock.Text = "FB Resonance " + mainPage.vt4MenuItems[2][1][2].Value.ToString();
        //        mainPage.vt4MenuItems[2][1][3].TextBlock.Text = "FB Level " + mainPage.vt4MenuItems[2][1][3].Value.ToString();
        //    }
        //    catch { }
        //}

        public void Save()
        {
            // HBE Byt mot json!!!
            string variants;

            try
            {
                variants = "";
                for (int i = 0; i < mainPage.vt4MenuItems[2][0].Count; i++)
                {
                    variants += mainPage.vt4MenuItems[2][0][i].TextBlock.Text + "|";
                }
                variants = variants.TrimEnd('|');
                LocalSettings.Values["Robot"] = variants;

                variants = "";
                for (int i = 0; i < mainPage.vt4MenuItems[3][0].Count; i++)
                {
                    variants += mainPage.vt4MenuItems[3][0][i].TextBlock.Text + "|";
                }
                variants = variants.TrimEnd('|');
                LocalSettings.Values["Megaphone"] = variants;

                variants = "";
                for (int i = 0; i < mainPage.vt4MenuItems[0][0].Count; i++)
                {
                    variants += mainPage.vt4MenuItems[0][0][i].TextBlock.Text + "|";
                }
                variants = variants.TrimEnd('|');
                LocalSettings.Values["Vocoder"] = variants;

                variants = "";
                for (int i = 0; i < mainPage.vt4MenuItems[1][0].Count; i++)
                {
                    variants += mainPage.vt4MenuItems[1][0][i].TextBlock.Text + "|";
                }
                variants = variants.TrimEnd('|');
                LocalSettings.Values["Harmony"] = variants;

                variants = "";
                for (int i = 0; i < mainPage.vt4MenuItems[4][0].Count; i++)
                {
                    variants += mainPage.vt4MenuItems[4][0][i].TextBlock.Text + "|";
                }
                variants = variants.TrimEnd('|');
                LocalSettings.Values["Reverb"] = variants;
            }
            catch { }
        }

        public void Save(String parameter, byte value)
        {
            LocalSettings.Values[parameter] = value;
        }
    }
}
