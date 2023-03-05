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
        public void UpdatePanelLabels()
        {
            lblRobot.TextBlock.Text = vt4MenuItems[2][0][VT4.TemporaryPatch.ROBOT_VARIATION].TextBlock.Text;
            lblMegaphone.TextBlock.Text = vt4MenuItems[3][0][VT4.TemporaryPatch.MEGAPHONE_VARIATION].TextBlock.Text;
            lblPatch.TextBlock.Text = VT4.TemporaryPatch.Name;
            lblVocoder.TextBlock.Text = vt4MenuItems[0][0][VT4.TemporaryPatch.VOCODER_VARIATION].TextBlock.Text;
            lblHarmony.TextBlock.Text = vt4MenuItems[1][0][VT4.TemporaryPatch.HARMONY_VARIATION].TextBlock.Text;
            lblReverb.TextBlock.Text = vt4MenuItems[4][0][VT4.TemporaryPatch.REVERB_VARIATION].TextBlock.Text;
        }
    }
}
