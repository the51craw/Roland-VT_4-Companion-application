using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
//using Windows.UI.ApplicationSettings;

namespace VT_4
{
    public sealed partial class MainPage : Page
    {
        public void InitTextBlocks(double width)
        {
            lblRobot.TextBlock.Text = vt4MenuItems[2][0][VT4.TemporaryPatch.ROBOT_VARIATION].TextBlock.Text;
            //pmbRobot.TextBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 130, 210, 180));

            lblMegaphone.TextBlock.Text = vt4MenuItems[3][0][VT4.TemporaryPatch.MEGAPHONE_VARIATION].TextBlock.Text;
            //pmbMegaphone.TextBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 130, 210, 180));

            lblVocoder.TextBlock.Text = vt4MenuItems[0][0][VT4.TemporaryPatch.VOCODER_VARIATION].TextBlock.Text;
            //pmbVocoder.TextBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 130, 210, 180));

            lblHarmony.TextBlock.Text = vt4MenuItems[1][0][VT4.TemporaryPatch.HARMONY_VARIATION].TextBlock.Text;
            //pmbHarmony.TextBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 130, 210, 180));

            lblReverb.TextBlock.Text = vt4MenuItems[4][0][VT4.TemporaryPatch.ROBOT_VARIATION].TextBlock.Text;
            //pmbReverb.TextBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 130, 210, 180));

            //SetTextBlockFontSize(width);

            //UpdateTextBlocks();
        }

        public void UpdatePanelLabels()
        {
            //if (PatchIndex == -1)
            //{
            //    pmbRobot.TextBlock.Text = vt4MenuTemplates[0][0][VT4.TemporaryPatch.ROBOT_VARIATION].TextBlock.Text;
            //    pmbMegaphone.TextBlock.Text = vt4MenuTemplates[1][0][VT4.TemporaryPatch.MEGAPHONE_VARIATION].Text;
            //    pmbVocoder.TextBlock.Text = vt4MenuTemplates[8][0][VT4.TemporaryPatch.VOCODER_VARIATION].Text;
            //    pmbHarmony.TextBlock.Text = vt4MenuTemplates[9][0][VT4.TemporaryPatch.HARMONY_VARIATION].Text;
            //    pmbReverb.TextBlock.Text = vt4MenuTemplates[7][0][VT4.TemporaryPatch.REVERB_VARIATION].Text;

            //    //pmbRobot.TextBlock.Text = vt4MenuItems[0][0][VT4.TemporaryPatch.ROBOT_VARIATION].pmbMenuText.Text;
            //    //pmbMegaphone.TextBlock.Text = vt4MenuItems[1][0][VT4.TemporaryPatch.MEGAPHONE_VARIATION].pmbMenuText.Text;
            //    //pmbVocoder.TextBlock.Text = vt4MenuItems[8][0][VT4.TemporaryPatch.VOCODER_VARIATION].pmbMenuText.Text;
            //    //pmbHarmony.TextBlock.Text = vt4MenuItems[9][0][VT4.TemporaryPatch.HARMONY_VARIATION].pmbMenuText.Text;
            //    //pmbReverb.TextBlock.Text = vt4MenuItems[7][0][VT4.TemporaryPatch.REVERB_VARIATION].pmbMenuText.Text;
            //}
            //else
            //{
            //    pmbRobot.TextBlock.Text = vt4MenuTemplates[0][0][VT4.UserPatch[PatchIndex].ROBOT_VARIATION].Text;
            //    pmbMegaphone.TextBlock.Text = vt4MenuTemplates[1][0][CurrentPatch.MEGAPHONE_VARIATION].Text;
            //    pmbVocoder.TextBlock.Text = vt4MenuTemplates[8][0][VT4.UserPatch[PatchIndex].VOCODER_VARIATION].Text;
            //    pmbHarmony.TextBlock.Text = vt4MenuTemplates[9][0][VT4.UserPatch[PatchIndex].HARMONY_VARIATION].Text;
            //    pmbReverb.TextBlock.Text = vt4MenuTemplates[7][0][VT4.UserPatch[PatchIndex].REVERB_VARIATION].Text;

            //    //pmbRobot.TextBlock.Text = vt4MenuItems[0][0][VT4.Patch[PatchIndex].ROBOT_VARIATION].pmbMenuText.Text;
            //    //pmbMegaphone.TextBlock.Text = vt4MenuItems[1][0][VT4.Patch[PatchIndex].MEGAPHONE_VARIATION].pmbMenuText.Text;
            //    //pmbVocoder.TextBlock.Text = vt4MenuItems[8][0][VT4.Patch[PatchIndex].VOCODER_VARIATION].pmbMenuText.Text;
            //    //pmbHarmony.TextBlock.Text = vt4MenuItems[9][0][VT4.Patch[PatchIndex].HARMONY_VARIATION].pmbMenuText.Text;
            //    //pmbReverb.TextBlock.Text = vt4MenuItems[7][0][VT4.Patch[PatchIndex].REVERB_VARIATION].pmbMenuText.Text;
            //}
            lblRobot.TextBlock.Text = vt4MenuItems[2][0][VT4.TemporaryPatch.ROBOT_VARIATION].TextBlock.Text;
            lblMegaphone.TextBlock.Text = vt4MenuItems[3][0][VT4.TemporaryPatch.MEGAPHONE_VARIATION].TextBlock.Text;
            lblPatch.TextBlock.Text = VT4.TemporaryPatch.Name;
            lblVocoder.TextBlock.Text = vt4MenuItems[0][0][VT4.TemporaryPatch.VOCODER_VARIATION].TextBlock.Text;
            lblHarmony.TextBlock.Text = vt4MenuItems[1][0][VT4.TemporaryPatch.HARMONY_VARIATION].TextBlock.Text;
            lblReverb.TextBlock.Text = vt4MenuItems[4][0][VT4.TemporaryPatch.REVERB_VARIATION].TextBlock.Text;
        }

        public void SetTextBlockFontSize(double width)
        {
            //pmbRobot.TextBlock.FontSize = width * 0.014;
            //pmbMegaphone.TextBlock.FontSize = width * 0.014;
            //pmbVocoder.TextBlock.FontSize = width * 0.014;
            //pmbHarmony.TextBlock.FontSize = width * 0.014;
            //pmbReverb.TextBlock.FontSize = width * 0.014;
        }
    }
}
