using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UwpControlsLibrary;
//using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VT_4
{
    public sealed partial class MainPage : Page
    {
        public MIDI midi;
        public MainPage mainPage;
        public Double WindowSize;
        public List<String> deviceNames;
        public Int32 MicSens;
        public Double ResizedHeight;
        public Double ResizedWidth;
        public Double ExtraMargin;
        public Boolean bypass = false;
        public ControlBase currentControl;

        public VT4 VT4 { get; set; }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // Global variables
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        PointerPoint mousePoint;
        Area currentArea;

        List<MidiEvents> timerAction;

        public Boolean portFound = false;

        DispatcherTimer EventTimer;
        DispatcherTimer MidiWatchTimer;
        DispatcherTimer BlinkTimer;
        DispatcherTimer ReleaseManualButtonTimer;
        DispatcherTimer ReleaseEffectButtonTimer;

        Boolean blinking = false;
        Boolean fast = false;
        Boolean slow = false;
        int variantCheck = -1;
        DispatcherTimer HandsOffTimer;

        byte[] midiInBuffer;
        List<MidiEvents> MidiEvents;

        Paramters parms = new Paramters();

        NameConverter[] Names = new NameConverter[8];

        private bool initDone = false;

        public Settings settings;
        public int sceneIndex = -1;    // -1 => Manual, 0 - 7 => scene 1 - 8
        public int VariationIndex = 0;
        //public byte sceneIndex = 0;
        public int manualBlinkCounter;
        bool manualOn;
        bool[] sceneEdited = new bool[8];
    }
}
