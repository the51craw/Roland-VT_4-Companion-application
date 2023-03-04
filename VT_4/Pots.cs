using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UwpControlsLibrary;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;

namespace VT_4
{
    public sealed partial class MainPage : Page
    {
        private void SetPotPositions()
        {
            //HandlePot(Area.VOLUME, lineY[1], 127);
            //HandlePot(Area.MIC_SENS, lineY[1], 127);
            //HandlePot(Area.KEY, lineY[1], 11);
            //HandlePot(Area.AUTO_PITCH, lineY[5], 255);
        }

        private byte HandlePot(int id, int value)
        {
            Area area = (Area)id;
            switch ((Area)id)
            {
                case Area.VOLUME:
                    // HBE VT4.TemporaryPatch.GLOBAL_LEVEL
                    knobVolume.Value = value;
                    SendSingleSysEx(area, new byte[] { (byte)(value / 16), (byte)(value % 16) });
                    SendControlChange(area, (byte)value);
                    break;
                case Area.MIC_SENS:
                    SendSingleSysEx(area, new byte[] { (byte)(value / 16), (byte)(value % 16) });
                    SendControlChange(area, (byte)value);
                    break;
                case Area.KEY:
                    SendSingleSysEx(area, new byte[] { (byte)(value / 16), (byte)(value % 16) });
                    break;
                case Area.AUTO_PITCH:
                    SendSingleSysEx(area, new byte[] { (byte)(value / 16), (byte)(value % 16) });
                    break;
            }
            return (byte)value;
        }

        private void SetPotHandles()
        {
            SetPotHandle(Area.VOLUME, VT4.TemporaryPatch.GLOBAL_LEVEL);
            SetPotHandle(Area.MIC_SENS, MicSens);
            SetPotHandle(Area.KEY, VT4.TemporaryPatch.KEY);
            SetPotHandle(Area.AUTO_PITCH, VT4.TemporaryPatch.AUTO_PITCH);
        }

        private void HandlePotFromWheel(Area area, int step)
        {
            switch (area)
            {
                case Area.VOLUME:
                    VT4.TemporaryPatch.GLOBAL_LEVEL += step;
                    VT4.TemporaryPatch.GLOBAL_LEVEL = VT4.TemporaryPatch.GLOBAL_LEVEL < 0 ? 0 : VT4.TemporaryPatch.GLOBAL_LEVEL;
                    VT4.TemporaryPatch.GLOBAL_LEVEL = VT4.TemporaryPatch.GLOBAL_LEVEL > 255 ? 255 : VT4.TemporaryPatch.GLOBAL_LEVEL;
                    SetPotHandle(area, VT4.TemporaryPatch.GLOBAL_LEVEL / 2);
                    SendSingleSysEx(area, new byte[] { (byte)(VT4.TemporaryPatch.GLOBAL_LEVEL / 16), (byte)(VT4.TemporaryPatch.GLOBAL_LEVEL % 16) });
                    SendControlChange(area, (byte)(VT4.TemporaryPatch.GLOBAL_LEVEL / 2));
                    break;
                case Area.MIC_SENS:
                    MicSens += step;
                    MicSens = MicSens < 0 ? 0 : MicSens;
                    MicSens = MicSens > 255 ? 255 : MicSens;
                    SetPotHandle(area, MicSens / 2);
                    SendSingleSysEx(area, new byte[] { (byte)(MicSens / 16), (byte)(MicSens % 16) });
                    SendControlChange(area, (byte)(MicSens / 2));
                    break;
                case Area.AUTO_PITCH:
                    VT4.TemporaryPatch.AUTO_PITCH += step;
                    VT4.TemporaryPatch.AUTO_PITCH = VT4.TemporaryPatch.AUTO_PITCH < 0 ? 0 : VT4.TemporaryPatch.AUTO_PITCH;
                    VT4.TemporaryPatch.AUTO_PITCH = VT4.TemporaryPatch.AUTO_PITCH > 255 ? 255 : VT4.TemporaryPatch.AUTO_PITCH;
                    SetPotHandle(area, VT4.TemporaryPatch.AUTO_PITCH);
                    SendControlChange(area, (byte)(VT4.TemporaryPatch.AUTO_PITCH));
                    SendSysEx(area, (byte)(VT4.TemporaryPatch.AUTO_PITCH));
                    break;
                case Area.KEY:
                    int value = VT4.TemporaryPatch.KEY;
                    value += step;
                    value = value < 0 ? value + 12 : value;
                    value = value > 11 ? value - 12 : value;
                    VT4.TemporaryPatch.KEY = (byte)value;
                    SetPotHandle(area, VT4.TemporaryPatch.KEY);
                    SendControlChange(area, (byte)(VT4.TemporaryPatch.KEY));
                    SendSysEx(area, (byte)(VT4.TemporaryPatch.KEY));
                    break;
            }
        }

        private void SetPotHandle(Area area, Int32 value)
        {
            switch (area)
            {
                case Area.VOLUME:
                    value = knobVolume.Value;
                    VT4.TemporaryPatch.GLOBAL_LEVEL = value;
                    SendControlChange(area, (byte)value);
                    break;
                case Area.MIC_SENS:
                    value = knobMicSens.Value;
                    SendControlChange(area, (byte)value);
                    break;
                case Area.KEY:
                    value = knobKey.Value;
                    VT4.TemporaryPatch.KEY = (byte)value;
                    SendSysEx(area, (byte)value);
                    break;
                case Area.AUTO_PITCH:
                    value = knobAutoPitch.Value;
                    VT4.TemporaryPatch.AUTO_PITCH = value;
                    SendSysEx(area, (byte)value);
                    break;
            }
        }

        //private void CreateIndicator(Area area, SpriteVisual indicator, Int32 value, Int32 maximum)
        //{
        //    switch (area)
        //    {
        //        case Area.VOLUME:
        //            indicator.Size = new Vector2((float)(imgClickArea.ActualWidth / 200), (float)(imgClickArea.ActualWidth / 80));
        //            indicator.Offset = new Vector3((float)(imgClickArea.ActualWidth * volumePosX), (float)(imgClickArea.ActualHeight * volumePosY), 0);
        //            indicator.CenterPoint = new Vector3((float)(imgClickArea.ActualWidth / 800), (float)(imgClickArea.ActualWidth / 46), 0);
        //            indicator.RotationAngleInDegrees = (float)((value * 300 / maximum) - 150);
        //            break;
        //        case Area.MIC_SENS:
        //            indicator.Size = new Vector2((float)(imgClickArea.ActualWidth / 200), (float)(imgClickArea.ActualWidth / 80));
        //            indicator.Offset = new Vector3((float)(imgClickArea.ActualWidth * micSensePosX), (float)(imgClickArea.ActualHeight * micSensePosY), 0);
        //            indicator.CenterPoint = new Vector3((float)(imgClickArea.ActualWidth / 800), (float)(imgClickArea.ActualWidth / 46), 0);
        //            indicator.RotationAngleInDegrees = (float)((value * 300 / maximum) - 150);
        //            break;
        //        case Area.KEY:
        //            indicator.Size = new Vector2((float)(imgClickArea.ActualWidth / 200), (float)(imgClickArea.ActualWidth / 80));
        //            indicator.Offset = new Vector3((float)(imgClickArea.ActualWidth * keyPosX), (float)(imgClickArea.ActualHeight * keyPosY), 0);
        //            indicator.CenterPoint = new Vector3((float)(imgClickArea.ActualWidth / 800), (float)(imgClickArea.ActualWidth / 46), 0);
        //            indicator.RotationAngleInDegrees = (float)((value * 330 / maximum) - 180);
        //            break;
        //        case Area.AUTO_PITCH:
        //            indicator.Size = new Vector2((float)(imgClickArea.ActualWidth / 200), (float)(imgClickArea.ActualWidth / 80));
        //            indicator.Offset = new Vector3((float)(imgClickArea.ActualWidth * autoPitchPosX), (float)(imgClickArea.ActualHeight * autoPitchPosY), 0);
        //            indicator.CenterPoint = new Vector3((float)(imgClickArea.ActualWidth / 680), (float)(imgClickArea.ActualWidth / 15), 0);
        //            //indicator.Offset = new Vector3(98.0f, 25.0f, 0);
        //            //indicator.CenterPoint = new Vector3(2.0f, 75.0f, 0);
        //            indicator.RotationAngleInDegrees = (float)((value * 300 / maximum) - 150);
        //            break;
        //    }
        //    indicator.Brush = compositor.CreateColorBrush(Colors.White);
        //    root.Children.InsertAtTop(indicator);
        //}
    }
}
