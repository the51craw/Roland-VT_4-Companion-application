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
        Boolean handleControlEvents = true;
        List<Boolean> previousHandleControlEvents = new List<Boolean>();

        private void PushHandleControlEvents(Boolean setHandleControlEvents = false)
        {
            previousHandleControlEvents.Add(handleControlEvents);
            handleControlEvents = setHandleControlEvents;
        }

        private void PopHandleControlEvents()
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
}
