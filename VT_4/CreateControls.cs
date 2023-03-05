using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpControlsLibrary;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace VT_4
{
    public sealed partial class MainPage : Page
    {

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // Object declarations
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        // The Controls object:
        Controls Controls;

        // Controls:
        private PopupMenuButton pmbRoland;
        private PopupMenuButton pmbMidiChannel;
        private PopupMenuButton[] pmbMidiChannels;
        private PopupMenuButton pmbEqualizer;
        private PopupMenuButton[] pmbEqualizerSettings;
        private PopupMenuButton pmbLevels;
        private PopupMenuButton[] pmbLevelSettings;
        private PopupMenuButton pmbSwitches;
        private PopupMenuButton[] pmbSwitchSettings;
        private PopupMenuButton pmbSave;
        private PopupMenuButton pmbLoad;
        private ImageButton btnVT4;
        private ImageButton btnBypass;
        //private ImageButton btnReverb;
        private PopupMenuButton pmb1;
        private PopupMenuButton pmb2;
        private PopupMenuButton pmb3;
        private PopupMenuButton pmb4;
        private PopupMenuButton pmb5;
        private PopupMenuButton pmb6;
        private PopupMenuButton pmb7;
        private PopupMenuButton pmb8;
        private PopupMenuButton pmbManual;
        private PopupMenuButton pmbTo1;
        private PopupMenuButton pmbTo2;
        private PopupMenuButton pmbTo3;
        private PopupMenuButton pmbTo4;
        private PopupMenuButton pmbTo5;
        private PopupMenuButton pmbTo6;
        private PopupMenuButton pmbTo7;
        private PopupMenuButton pmbTo8;
        private Knob knobVolume;
        private Knob knobMicSens;
        private Knob knobKey;
        private Knob knobAutoPitch;
        private VerticalSlider slPitch;
        private VerticalSlider slFormant;
        private VerticalSlider slBalance;
        private VerticalSlider slReverb;
        private PopupMenuButton pmbRobot;
        private PopupMenuButton pmbMegaphone;
        private PopupMenuButton pmbVocoder;
        private PopupMenuButton pmbHarmony;
        private PopupMenuButton pmbReverb;

        /// <summary>
        /// All sub menus and menu items gets the same Id as its parent.
        /// The actual position in Controls.ControlsList is stored in
        /// topLevelMenuLocations. There are 6 label, 4 knobs and 4
        /// sliders preceeding the first menu in Controls.ControlsList
        /// so the first menu is at position 15. Then all sub menus
        /// and items fills up locations in Controls.ControlsList before
        /// next top level menu is stored. Here we store the locations
        /// of top level menus indexed by their Id. The sub menus and
        /// items are then stored after that position.
        /// </summary>
        private List<int> topLevelMenuLocations;

        private Label lblVocoder;
        private Label lblHarmony;
        private Label lblRobot;
        private Label lblMegaphone;
        private Label lblReverb;
        private Label lblPatch;

        private SolidColorBrush menuTextColorOn;
        private SolidColorBrush menuTextColorOff;

        // Default font settings:
        private ControlBase.ControlTextWeight fontWeightNormal =
            ControlBase.ControlTextWeight.NORMAL;
        private ControlBase.ControlTextAlignment textAlignmentLeft =
            ControlBase.ControlTextAlignment.LEFT;
        int fontSize = 16;

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // Controls creation
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Creating PopupMenuButton menus in VT-4:
        /// PopupMenuButton has three Id fields:
        /// Id - Main Id field. All controls in the same menu has the same Id.
        /// MenuNumber - One PopupMenuButton can hold one or more menus. In TV-4 those
        /// are used for effects to be 0 for variation menu and 1 for parameter menu.
        /// The effect button is the PopupMenuButton that acts to turn effect on or off
        /// and to open variation and parameter menu. It has MenuNumber set to -1.
        /// MenuItemNumber - The item number of a menu item. Selects variation or parameter
        /// to use, and is set by an int sub that is set to zero before each menu creation
        /// and incremented for each item using sub++.
        /// 
        /// Since menus covers other controls, and controls might cover menus, all menus are
        /// created last in order not tobe covered by other controls, and by position in 
        /// Y-order from window bottom and up. Note that the use of the integer i and the
        /// operation i++ to set Id also defined the order of Area definitions!
        /// </summary>
        private void CreateControls()
        {
            // Create and initiate the controls object:
            Controls = new Controls(Window.Current.Bounds, imgClickArea);
            Controls.Init(gridControls);
            topLevelMenuLocations = new List<int>();

            // Menu text color:
            menuTextColorOn = new SolidColorBrush(Color.FromArgb(255, 0x99, 0xE7, 0xCF));
            menuTextColorOff = new SolidColorBrush(Color.FromArgb(255, 0x62, 0x86, 0xA5));

            // Create menu lists:
            vt4MenuItems = new List<List<List<PopupMenuButton>>>();

            // Index couters:
            int i = 0;
            int sub;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Variant labels, no Id
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            // Create labels:
            lblVocoder = Controls.AddLabel(-1, gridControls, new Rect(376, 445, 93, 30),
                " ", fontSize, TextAlignment.Center, fontWeightNormal,
                TextWrapping.NoWrap, menuTextColorOn);

            lblHarmony = Controls.AddLabel(-1, gridControls, new Rect(661, 445, 93, 30),
                " ", fontSize, TextAlignment.Center, fontWeightNormal,
                TextWrapping.NoWrap, menuTextColorOn);

            lblRobot = Controls.AddLabel(-1, gridControls, new Rect(74, 295, 93, 30),
                " ", fontSize, TextAlignment.Center, fontWeightNormal,
                TextWrapping.NoWrap, menuTextColorOn);

            lblMegaphone = Controls.AddLabel(-1, gridControls, new Rect(219, 295, 93, 30),
                " ", fontSize, TextAlignment.Center, fontWeightNormal,
                TextWrapping.NoWrap, menuTextColorOn);

            lblPatch = Controls.AddLabel(-1, gridControls, new Rect(516, 295, 93, 30),
                " ", fontSize, TextAlignment.Center, fontWeightNormal,
                TextWrapping.NoWrap, menuTextColorOn);

            lblReverb = Controls.AddLabel(-1, gridControls, new Rect(962, 295, 93, 30),
                " ", fontSize, TextAlignment.Center, fontWeightNormal,
                TextWrapping.NoWrap, menuTextColorOn);

            i = 0;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Knobs, Id 0 - 3
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            knobVolume = Controls.AddKnob(i++, gridControls, new Image[] { imgSmallKnob },
                new Point(120, 158), 0, 127, 30, 330);
            knobMicSens = Controls.AddKnob(i++, gridControls, new Image[] { imgSmallKnob },
                new Point(267, 158), 0, 127, 30, 330);
            knobKey = Controls.AddKnob(i++, gridControls, new Image[] { imgKeyKnob },
                new Point(1008, 158), 0, 11, 0, 360, 1, true);
            knobAutoPitch = Controls.AddKnob(i++, gridControls, new Image[] { imgLargeKnob },
                new Point(566, 576), 0, 127, 30, 330, 1);

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Sliders, Id 4 - 7
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            slPitch = Controls.AddVerticalSlider(i++, gridControls,
                new Image[] { imgSliderHandle }, new Rect(66, 376, 110, 364), 0, 127);
            slFormant = Controls.AddVerticalSlider(i++, gridControls,
                new Image[] { imgSliderHandle }, new Rect(212, 376, 110, 364), 0, 127);
            slBalance = Controls.AddVerticalSlider(i++, gridControls,
                new Image[] { imgSliderHandle }, new Rect(807, 376, 110, 364), 0, 127);
            slReverb = Controls.AddVerticalSlider(i++, gridControls,
                new Image[] { imgSliderHandle }, new Rect(953, 376, 110, 364), 0, 127);

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Vocoder button and menus, Id 8
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            sub = 0;
            double xPos = (imgVocoderOn.ActualWidth - imgMenuBackground.ActualWidth) / imgVocoderOn.ActualWidth / 2;
            double ySpacing = -(imgVocoderOn.ActualHeight - imgMenuBackground.ActualHeight) / imgVocoderOn.ActualHeight;
            double yPos = imgMenuBackground.ActualHeight / imgVocoderOn.ActualHeight * 2;
            pmbVocoder = Controls.AddPopupMenuButton(i, gridControls,
                new Image[] { imgVocoderOn }, new Point(367, 336),
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT,
                    ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                0, 127, null, fontSize, false,
                fontWeightNormal,
                textAlignmentLeft,
                menuTextColorOn, menuTextColorOff);
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmbVocoder.AddMenu();
            vt4MenuItems.Add(new List<List<PopupMenuButton>>());
            vt4MenuItems[vt4MenuItems.Count - 1].Add(new List<PopupMenuButton>());
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
                pmbVocoder.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
                pmbVocoder.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
                pmbVocoder.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
                pmbVocoder.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
                pmbVocoder.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
                pmbVocoder.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
                pmbVocoder.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
                pmbVocoder.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            pmbVocoder.AddMenu();
            sub = 0;
            vt4MenuItems[vt4MenuItems.Count - 1].Add(new List<PopupMenuButton>());
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
                pmbVocoder.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT },
                "Type", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbVocoder.Children[1][pmbVocoder.Children[1].Count - 1].MaxValue = 3;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
                pmbVocoder.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter 1", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbVocoder.Children[1][pmbVocoder.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
                pmbVocoder.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter 2", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbVocoder.Children[1][pmbVocoder.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbVocoder.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter 3", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbVocoder.Children[1][pmbVocoder.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbVocoder.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter 4", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbVocoder.Children[1][pmbVocoder.Children[1].Count - 1].MaxValue = 255;
            //vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            //pmbVocoder.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
            //    ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
            //    { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
            //    "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
            //    xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            i++;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Harmony button and menus, Id 9
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            sub = 0;
            xPos = (imgHarmonyOn.ActualWidth - imgMenuBackground.ActualWidth) / imgHarmonyOn.ActualWidth / 2;
            ySpacing = -1.117 * (imgHarmonyOn.ActualHeight - imgMenuBackground.ActualHeight) / imgHarmonyOn.ActualHeight;
            yPos = imgMenuBackground.ActualHeight / imgHarmonyOn.ActualHeight * 2;
            pmbHarmony = Controls.AddPopupMenuButton(i, gridControls,
                new Image[] { imgHarmonyOn }, new Point(653, 336),
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT,
                    ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                0, 127, null, fontSize, false,
                fontWeightNormal,
                textAlignmentLeft,
                menuTextColorOn, menuTextColorOff);
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmbHarmony.AddMenu();
            vt4MenuItems.Add(new List<List<PopupMenuButton>>());
            vt4MenuItems[vt4MenuItems.Count - 1].Add(new List<PopupMenuButton>());
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbHarmony.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbHarmony.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbHarmony.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbHarmony.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbHarmony.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbHarmony.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbHarmony.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbHarmony.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOff));
            pmbHarmony.AddMenu();
            sub = 0;
            vt4MenuItems[vt4MenuItems.Count - 1].Add(new List<PopupMenuButton>());
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbHarmony.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbHarmony.Children[1][pmbHarmony.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbHarmony.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbHarmony.Children[1][pmbHarmony.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbHarmony.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbHarmony.Children[1][pmbHarmony.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbHarmony.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbHarmony.Children[1][pmbHarmony.Children[1].Count - 1].MaxValue = 10;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbHarmony.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbHarmony.Children[1][pmbHarmony.Children[1].Count - 1].MaxValue = 10;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbHarmony.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbHarmony.Children[1][pmbHarmony.Children[1].Count - 1].MaxValue = 10;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbHarmony.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbHarmony.Children[1][pmbHarmony.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbHarmony.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbHarmony.Children[1][pmbHarmony.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbHarmony.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, yPos, ySpacing, menuTextColorOn, menuTextColorOn));
            pmbHarmony.Children[1][pmbHarmony.Children[1].Count - 1].MaxValue = 255;
            i++;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Robot button and menus, Id 0x0a (10)
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            sub = 0;
            pmbRobot = Controls.AddPopupMenuButton(i, gridControls,
                new Image[] { imgRobotOn }, new Point(68, 250),
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT,
                    ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                0, 127, null, fontSize, false,
                fontWeightNormal,
                textAlignmentLeft,
                menuTextColorOn, menuTextColorOff);
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmbRobot.AddMenu();
            vt4MenuItems.Add(new List<List<PopupMenuButton>>());
            vt4MenuItems[vt4MenuItems.Count - 1].Add(new List<PopupMenuButton>());
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbRobot.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            //vt4MenuItems[vt4MenuItems.Count - 1][0][sub++];
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbRobot.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            //vt4MenuItems[vt4MenuItems.Count - 1][0][sub++];
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbRobot.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            //vt4MenuItems[vt4MenuItems.Count - 1][0][sub++];
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbRobot.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            //vt4MenuItems[vt4MenuItems.Count - 1][0][sub++];
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbRobot.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            //vt4MenuItems[vt4MenuItems.Count - 1][0][sub++];
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbRobot.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            //vt4MenuItems[vt4MenuItems.Count - 1][0][sub++];
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbRobot.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            //vt4MenuItems[vt4MenuItems.Count - 1][0][sub++];
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbRobot.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            //vt4MenuItems[vt4MenuItems.Count - 1][0][sub++];
            pmbRobot.AddMenu();
            sub = 0;
            vt4MenuItems[vt4MenuItems.Count - 1].Add(new List<PopupMenuButton>());
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbRobot.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbRobot.Children[1][pmbRobot.Children[1].Count - 1].MaxValue = 3;
            //vt4MenuItems[vt4MenuItems.Count - 1][1][sub++];
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbRobot.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbRobot.Children[1][pmbRobot.Children[1].Count - 1].MaxValue = 1;
            //vt4MenuItems[vt4MenuItems.Count - 1][1][sub++];
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbRobot.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbRobot.Children[1][pmbRobot.Children[1].Count - 1].MaxValue = 255;
            //vt4MenuItems[vt4MenuItems.Count - 1][1][sub++];
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbRobot.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbRobot.Children[1][pmbRobot.Children[1].Count - 1].MaxValue = 255;
            //vt4MenuItems[vt4MenuItems.Count - 1][1][sub++];
            i++;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Megaphone button and menus, Id 0x0b (11)
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            sub = 0;
            pmbMegaphone = Controls.AddPopupMenuButton(i, gridControls,
                new Image[] { imgMegaphoneOn }, new Point(213, 250),
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT,
                    ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                0, 127, null, fontSize, false,
                fontWeightNormal,
                textAlignmentLeft,
                menuTextColorOn, menuTextColorOff);
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmbMegaphone.AddMenu();
            vt4MenuItems.Add(new List<List<PopupMenuButton>>());
            vt4MenuItems[vt4MenuItems.Count - 1].Add(new List<PopupMenuButton>());
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbMegaphone.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbMegaphone.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbMegaphone.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbMegaphone.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbMegaphone.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbMegaphone.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbMegaphone.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbMegaphone.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff));
            pmbMegaphone.AddMenu();
            sub = 0;
            vt4MenuItems[vt4MenuItems.Count - 1].Add(new List<PopupMenuButton>());
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbMegaphone.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Type", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbMegaphone.Children[1][pmbMegaphone.Children[1].Count - 1].MaxValue = 3;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbMegaphone.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbMegaphone.Children[1][pmbMegaphone.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbMegaphone.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbMegaphone.Children[1][pmbMegaphone.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbMegaphone.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbMegaphone.Children[1][pmbMegaphone.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbMegaphone.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbMegaphone.Children[1][pmbMegaphone.Children[1].Count - 1].MaxValue = 255;
            i++;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Reverb button and menus, Id 0x0c (12)
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            sub = 0;
            xPos = (imgReverbOn.ActualWidth - imgMenuBackground.ActualWidth) / imgReverbOn.ActualWidth;
            pmbReverb = Controls.AddPopupMenuButton(i, gridControls,
                new Image[] { imgReverbOn }, new Point(958, 250),
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT,
                    ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                0, 127, null, fontSize, false,
                fontWeightNormal,
                textAlignmentLeft,
                menuTextColorOn, menuTextColorOff);
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmbReverb.AddMenu();
            vt4MenuItems.Add(new List<List<PopupMenuButton>>());
            vt4MenuItems[vt4MenuItems.Count - 1].Add(new List<PopupMenuButton>());
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbReverb.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbReverb.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbReverb.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbReverb.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbReverb.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbReverb.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbReverb.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOff));
            vt4MenuItems[vt4MenuItems.Count - 1][0].Add(
            pmbReverb.AddMenuItem(0, sub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.ITEM, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Variant", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOff));
            pmbReverb.AddMenu();
            sub = 0;
            vt4MenuItems[vt4MenuItems.Count - 1].Add(new List<PopupMenuButton>());
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbReverb.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Type", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbReverb.Children[1][pmbReverb.Children[1].Count - 1].MaxValue = 5;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbReverb.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbReverb.Children[1][pmbReverb.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbReverb.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbReverb.Children[1][pmbReverb.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbReverb.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbReverb.Children[1][pmbReverb.Children[1].Count - 1].MaxValue = 255;
            vt4MenuItems[vt4MenuItems.Count - 1][1].Add(
            pmbReverb.AddMenuItem(1, sub++, new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT, ControlBase.PointerButton.OTHER },
                "Parameter", fontSize, false, fontWeightNormal, textAlignmentLeft,
                xPos, 1, 0, menuTextColorOn, menuTextColorOn));
            pmbReverb.Children[1][pmbReverb.Children[1].Count - 1].MaxValue = 255;
            i++;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Manual button, scene buttons and menus
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            pmbManual = Controls.AddPopupMenuButton(i++, gridControls,
            new Image[] { imgManualOn }, new Point(812, 250),
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT });
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmbManual.IsOn = false;

            int numberButtonStartX = 356;
            int numberButtonStartY = 198;
            int numberButtonSpacingX = 113;
            int numberButtonSpacingY = 53;
            int numberButtonX = 0;
            int numberButtonY = 1;

            sub = 0;
            pmb5 = Controls.AddPopupMenuButton(i++, gridControls, new Image[] { imgFiveOn },
                new Point(numberButtonStartX + numberButtonSpacingX * numberButtonX++,
                numberButtonStartY + numberButtonSpacingY * numberButtonY),
                ControlBase.PopupMenuButtonStyle.BUTTON, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT,
                    ControlBase.PointerButton.OTHER });
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmb5.AddMenu();
            pmb5.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Save", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            pmbTo5 = pmb5.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Copy to...", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);

            int subSub = 0;
            pmbTo5.AddMenu();
            while (subSub < 8)
            {
                pmbTo5.AddMenuItem(0, subSub++, new Image[] { imgNumberButtonBackground },
                    ControlBase.PopupMenuButtonStyle.MENU,
                    new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                    "... Scene " + subSub.ToString(), fontSize, false,
                    fontWeightNormal, textAlignmentLeft,
                    0, 1, 0, menuTextColorOn, menuTextColorOn);
            }

            sub = 0;
            pmb6 = Controls.AddPopupMenuButton(i++, gridControls, new Image[] { imgSixOn },
                new Point(numberButtonStartX + numberButtonSpacingX * numberButtonX++,
                numberButtonStartY + numberButtonSpacingY * numberButtonY),
                ControlBase.PopupMenuButtonStyle.BUTTON, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT,
                    ControlBase.PointerButton.OTHER });
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmb6.AddMenu();
            pmb6.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Save", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            pmbTo6 = pmb6.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Copy to...", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            subSub = 0;
            pmbTo6.AddMenu();
            while (subSub < 8)
            {
                pmbTo6.AddMenuItem(0, subSub++, new Image[] { imgNumberButtonBackground },
                    ControlBase.PopupMenuButtonStyle.MENU,
                    new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                    "... Scene " + subSub.ToString(), fontSize, false,
                    fontWeightNormal, textAlignmentLeft,
                    0, 1, 0, menuTextColorOn, menuTextColorOn);
            }

            sub = 0;
            pmb7 = Controls.AddPopupMenuButton(i++, gridControls, new Image[] { imgSevenOn },
                new Point(numberButtonStartX + numberButtonSpacingX * numberButtonX++,
                numberButtonStartY + numberButtonSpacingY * numberButtonY),
                ControlBase.PopupMenuButtonStyle.BUTTON, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT,
                    ControlBase.PointerButton.OTHER });
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmb7.AddMenu();
            pmb7.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Save", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            pmbTo7 = pmb7.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Copy to...", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            subSub = 0;
            pmbTo7.AddMenu();
            while (subSub < 8)
            {
                pmbTo7.AddMenuItem(0, subSub++, new Image[] { imgNumberButtonBackground },
                    ControlBase.PopupMenuButtonStyle.MENU,
                    new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                    "... Scene " + subSub.ToString(), fontSize, false,
                    fontWeightNormal, textAlignmentLeft,
                    0, 1, 0, menuTextColorOn, menuTextColorOn);
            }

            sub = 0;
            pmb8 = Controls.AddPopupMenuButton(i++, gridControls, new Image[] { imgEightOn },
                new Point(numberButtonStartX + numberButtonSpacingX * numberButtonX++,
                numberButtonStartY + numberButtonSpacingY * numberButtonY),
                ControlBase.PopupMenuButtonStyle.BUTTON, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT,
                    ControlBase.PointerButton.OTHER });
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmb8.AddMenu();
            pmb8.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Save", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            pmbTo8 = pmb8.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Copy to...", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            subSub = 0;
            pmbTo8.AddMenu();
            while (subSub < 8)
            {
                pmbTo8.AddMenuItem(0, subSub++, new Image[] { imgNumberButtonBackground },
                    ControlBase.PopupMenuButtonStyle.MENU,
                    new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                    "... Scene " + subSub.ToString(), fontSize, false,
                    fontWeightNormal, textAlignmentLeft,
                    0, 1, 0, menuTextColorOn, menuTextColorOn);
            }

            numberButtonX = 0;
            numberButtonY = 0;
            // Add scene 1 button:
            pmb1 = Controls.AddPopupMenuButton(i++, gridControls, new Image[] { imgOneOn },
                new Point(numberButtonStartX + numberButtonSpacingX * numberButtonX++,
                numberButtonStartY + numberButtonSpacingY * numberButtonY),
                ControlBase.PopupMenuButtonStyle.BUTTON, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT,
                    ControlBase.PointerButton.OTHER });
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            sub = 0;
            pmb1.AddMenu();
            // Add 'Save' menu item
            pmb1.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Save", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            // Add 'Copy to...' menu item
            pmbTo1 = pmb1.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Copy to...", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            // Add '...Scene' menu items 
            subSub = 0;
            pmbTo1.AddMenu();
            while (subSub < 8)
            {
                pmbTo1.AddMenuItem(0, subSub++, new Image[] { imgNumberButtonBackground },
                    ControlBase.PopupMenuButtonStyle.MENU,
                    new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                    "... Scene " + subSub.ToString(), fontSize, false,
                    fontWeightNormal, textAlignmentLeft,
                    0, 1, 0, menuTextColorOn, menuTextColorOn);
            }

            sub = 0;
            pmb2 = Controls.AddPopupMenuButton(i++, gridControls, new Image[] { imgTwoOn },
                new Point(numberButtonStartX + numberButtonSpacingX * numberButtonX++,
                numberButtonStartY + numberButtonSpacingY * numberButtonY),
                ControlBase.PopupMenuButtonStyle.BUTTON, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT,
                    ControlBase.PointerButton.OTHER });
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmb2.AddMenu();
            pmb2.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Save", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            pmbTo2 = pmb2.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Copy to...", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            subSub = 0;
            pmbTo2.AddMenu();
            while (subSub < 8)
            {
                pmbTo2.AddMenuItem(0, subSub++, new Image[] { imgNumberButtonBackground },
                    ControlBase.PopupMenuButtonStyle.MENU,
                    new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                    "... Scene " + subSub.ToString(), fontSize, false,
                    fontWeightNormal, textAlignmentLeft,
                    0, 1, 0, menuTextColorOn, menuTextColorOn);
            }

            sub = 0;
            pmb3 = Controls.AddPopupMenuButton(i++, gridControls, new Image[] { imgThreeOn },
                new Point(numberButtonStartX + numberButtonSpacingX * numberButtonX++,
                numberButtonStartY + numberButtonSpacingY * numberButtonY),
                ControlBase.PopupMenuButtonStyle.BUTTON, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT,
                    ControlBase.PointerButton.OTHER });
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmb3.AddMenu();
            pmb3.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Save", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            pmbTo3 = pmb3.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Copy to...", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            subSub = 0;
            pmbTo3.AddMenu();
            while (subSub < 8)
            {
                pmbTo3.AddMenuItem(0, subSub++, new Image[] { imgNumberButtonBackground },
                    ControlBase.PopupMenuButtonStyle.MENU,
                    new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                    "... Scene " + subSub.ToString(), fontSize, false,
                    fontWeightNormal, textAlignmentLeft,
                    0, 1, 0, menuTextColorOn, menuTextColorOn);
            }

            sub = 0;
            pmb4 = Controls.AddPopupMenuButton(i++, gridControls, new Image[] { imgFourOn },
                new Point(numberButtonStartX + numberButtonSpacingX * numberButtonX++,
                numberButtonStartY + numberButtonSpacingY * numberButtonY),
                ControlBase.PopupMenuButtonStyle.BUTTON, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT, ControlBase.PointerButton.RIGHT,
                    ControlBase.PointerButton.OTHER });

            numberButtonX = 0;
            numberButtonY++;
            topLevelMenuLocations.Add(Controls.ControlsList.Count - 1);
            pmb4.AddMenu();
            pmb4.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Save", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            pmbTo4 = pmb4.AddMenuItem(0, sub++, new Image[] { imgNumberButtonBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT }, "Copy to...", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOn);
            subSub = 0;
            pmbTo4.AddMenu();
            while (subSub < 8)
            {
                pmbTo4.AddMenuItem(0, subSub++, new Image[] { imgNumberButtonBackground },
                    ControlBase.PopupMenuButtonStyle.MENU,
                    new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                    "... Scene " + subSub.ToString(), fontSize, false,
                    fontWeightNormal, textAlignmentLeft,
                    0, 1, 0, menuTextColorOn, menuTextColorOn);
            }

            sub = 0;

            pmb1.Set(false);
            pmb2.Set(false);
            pmb3.Set(false);
            pmb4.Set(false);
            pmb5.Set(false);
            pmb6.Set(false);
            pmb7.Set(false);
            pmb8.Set(false);

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Bypass button
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            btnBypass = Controls.AddImageButton(i++, gridControls,
            new Image[] { imgBypassOn }, new Point(812, 198),
                ControlBase.ImageButtonFunction.TOGGLE);
            btnBypass.IsOn = false;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Logo buttons
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            pmbRoland = Controls.AddPopupMenuButton(i++, gridControls,
                new Image[] { imgRolandLogo }, new Point(14, 13),
                ControlBase.PopupMenuButtonStyle.MENU, new ControlBase.PointerButton[]
                { ControlBase.PointerButton.LEFT });
            btnVT4 = Controls.AddImageButton(i++, gridControls,
                new Image[] { imgVT_4_Logo }, new Point(844, 23),
                ControlBase.ImageButtonFunction.MOMENTARY);

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Roland button menus
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            subSub = 0;
            // Main menu button image (imgMenuBackground) is higher than the menu item 
            // background image, and that screwes up y spacing for the menu items, thus:
            ySpacing = (imgMenuBackground.ActualHeight - imgRolandLogo.ActualHeight) /
                imgRolandLogo.ActualHeight;
            pmbRoland.AddMenu();
            pmbMidiChannel = pmbRoland.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Set MIDI channel", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, ySpacing, menuTextColorOn, menuTextColorOn);

            pmbEqualizer = pmbRoland.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Equalizer settings", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, ySpacing, menuTextColorOn, menuTextColorOn);

            pmbLevels = pmbRoland.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Set system levels", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, ySpacing, menuTextColorOn, menuTextColorOn);

            pmbSwitches = pmbRoland.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.MENU,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Set system switches", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, ySpacing, menuTextColorOn, menuTextColorOn);

            pmbSave = pmbRoland.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Save VT-4 to file", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, ySpacing, menuTextColorOn, menuTextColorOn);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbLoad = pmbRoland.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Load VT-4 from file", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, ySpacing, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // MIDI channel menuItems
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            pmbMidiChannels = new PopupMenuButton[17];

            subSub = 0;
            pmbMidiChannel.AddMenu();
            ySpacing = -imgRolandLogo.ActualHeight / imgMenuBackground.ActualHeight;
            pmbMidiChannels[0] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 1", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[1] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 2", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[2] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 3", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[3] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 4", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[4] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 5", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[5] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 6", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[6] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 7", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[7] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 8", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[8] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 9", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[9] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 10", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[10] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 11", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[11] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 12", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[12] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 13", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[13] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 14", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[14] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 15", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[15] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI channel 16", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            pmbMidiChannels[16] = pmbMidiChannel.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "MIDI Omni", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, ySpacing, -0.1, menuTextColorOn, menuTextColorOff);

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Equalizer menuItems
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            pmbEqualizerSettings = new PopupMenuButton[11];

            subSub = 0;
            pmbEqualizer.AddMenu();
            pmbEqualizerSettings[0] = pmbEqualizer.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Equalizer", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbEqualizerSettings[1] = pmbEqualizer.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Low Shelf freq ", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbEqualizerSettings[2] = pmbEqualizer.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Low Shelf gain", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            pmbEqualizerSettings[2].MaxValue = 40;
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbEqualizerSettings[3] = pmbEqualizer.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Low mid frequency", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbEqualizerSettings[4] = pmbEqualizer.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Low mid Q freq", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbEqualizerSettings[5] = pmbEqualizer.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Low mid gain", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;
            pmbEqualizerSettings[5].MaxValue = 40;

            pmbEqualizerSettings[6] = pmbEqualizer.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "High mid freq", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbEqualizerSettings[7] = pmbEqualizer.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "High mid Q", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbEqualizerSettings[8] = pmbEqualizer.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "High mid gain", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;
            pmbEqualizerSettings[8].MaxValue = 40;

            pmbEqualizerSettings[9] = pmbEqualizer.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "High Shelf Q freq", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbEqualizerSettings[10] = pmbEqualizer.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "High Shelf gain", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;
            pmbEqualizerSettings[10].MaxValue = 40;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Levels menuItems
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            pmbLevelSettings = new PopupMenuButton[5];

            subSub = 0;
            pmbLevels.AddMenu();
            pmbLevelSettings[0] = pmbLevels.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Gate level ", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            pmbLevelSettings[0].MaxValue = 4;
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbLevelSettings[1] = pmbLevels.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Formant depth ", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            pmbLevelSettings[1].MaxValue = 4;
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbLevelSettings[2] = pmbLevels.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Low cut ", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            pmbLevelSettings[2].MaxValue = 4;
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbLevelSettings[3] = pmbLevels.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Enhancer ", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            pmbLevelSettings[3].MaxValue = 4;
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbLevelSettings[4] = pmbLevels.AddMenuItem(0, subSub++,
                new Image[] { imgMenuBackground, imgMenuItemSliderHandle },
                ControlBase.PopupMenuButtonStyle.SLIDER,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Usb mixing ", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            pmbLevelSettings[4].MaxValue = 20;
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Switches menuItems
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            pmbSwitchSettings = new PopupMenuButton[5];

            subSub = 0;
            pmbSwitches.AddMenu();
            pmbSwitchSettings[0] = pmbSwitches.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Monitor mode  ", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbSwitchSettings[1] = pmbSwitches.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "External carrier ", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbSwitchSettings[2] = pmbSwitches.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Midi in ", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbSwitchSettings[3] = pmbSwitches.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Pitch & Format routing", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            pmbSwitchSettings[4] = pmbSwitches.AddMenuItem(0, subSub++, new Image[] { imgMenuBackground },
                ControlBase.PopupMenuButtonStyle.BUTTON,
                new ControlBase.PointerButton[] { ControlBase.PointerButton.LEFT },
                "Mute mode ", fontSize, false,
                fontWeightNormal, textAlignmentLeft,
                0, 1, 0, menuTextColorOn, menuTextColorOff);
            ((PopupMenuButton)Controls.ControlsList[Controls.ControlsList.Count - 1]).IsOn = true;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Controls finalization
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            // Make sure all controls has the correct size and position:
            Controls.ResizeControls(gridControls, Window.Current.Bounds);
            Controls.SetControlsUniform(gridControls);
            UpdateLayout();
        }
    }
}
