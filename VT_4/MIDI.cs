using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Popups;
using static VT_4.MainPage;
using System.Diagnostics;
using Windows.Devices.Bluetooth;
//using Newtonsoft.Json.Linq;

namespace VT_4
{
    public class MIDI
    {
        //public CoreDispatcher coreDispatcher;
        //public IMidiOutPort midiOutPort;
        //public MidiInPort midiInPort;
        //public byte MidiOutPortChannel { get; set; }
        //public byte MidiInPortChannel { get; set; }
        //public Int32 MidiOutPortSelectedIndex { get; set; }
        //public Int32 MidiInPortSelectedIndex { get; set; }
        //public Boolean VenderDriverPresent = false;
        public MainPage mainPage { get; set; }

        public List<PortPair> PortPairs { get; set; }
        //public List<String> MidiDevices { get; set; }

        public List<PortPair> PortPairsToKeep; // Ports for which no respons on ID request message was received should be removed.

        public byte expectingAnswer = 0;

        public MIDI(MainPage mainPage)
        {
            this.mainPage = mainPage;
        }

        //~MIDI()
        //{
        //    try
        //    {
        //        foreach (PortPair portPair in PortPairs)
        //        {
        //            if (portPair.OutPort != null)
        //            {
        //                portPair.OutPort.Dispose();
        //                portPair.OutPort = null;
        //            }
        //            if (portPair.InPort != null)
        //            {
        //                portPair.InPort.Dispose();
        //                portPair.InPort = null;
        //            }
        //            GC.Collect();
        //        }
        //        PortPairs.Clear();
        //    }
        //    catch { }
        //}

        //public void ResetMidi()
        //{
        //    try
        //    {
        //        foreach (PortPair portPair in PortPairs)
        //        {
        //            if (portPair.OutPort != null)
        //            {
        //                portPair.OutPort.Dispose();
        //            }
        //            if (portPair.InPort != null)
        //            {
        //                portPair.InPort.Dispose();
        //            }
        //            portPair.OutPort = null;
        //            portPair.InPort = null;
        //            GC.Collect();
        //        }
        //        PortPairs.Clear();
        //    }
        //    catch { }
        //}

        public async Task Init(List<String> deviceNames)
        {
            try
            {
                PortPairs = new List<PortPair>();
                PortPairsToKeep = new List<PortPair>();

                DeviceInformationCollection midiOutputDevices = await DeviceInformation.FindAllAsync(MidiOutPort.GetDeviceSelector());
                DeviceInformationCollection midiInputDevices = await DeviceInformation.FindAllAsync(MidiInPort.GetDeviceSelector());
                DeviceInformation midiOutDevInfo = null;
                DeviceInformation midiInDevInfo = null;

                foreach (String deviceName in deviceNames)
                {
                    foreach (DeviceInformation device in midiOutputDevices)
                    {
                        if (device.Name.Contains(deviceName) && !device.Name.Contains("CTRL"))
                        {
                            midiOutDevInfo = device;
                            break;
                        }
                    }

                    PortPair portPair = new PortPair();
                    portPair.Name = deviceName;

                    if (midiOutDevInfo != null)
                    {
                        portPair.OutPort = await MidiOutPort.FromIdAsync(midiOutDevInfo.Id);
                    }

                    foreach (DeviceInformation device in midiInputDevices)
                    {
                        if (device.Name.Contains(deviceName) && !device.Name.Contains("CTRL"))
                        {
                            midiInDevInfo = device;
                            break;
                        }
                    }

                    if (midiInDevInfo != null)
                    {
                        portPair.InPort = await MidiInPort.FromIdAsync(midiInDevInfo.Id);
                    }

                    PortPairs.Add(portPair);

                    if (portPair.OutPort == null)
                    {
                        Debug.WriteLine("Unable to create MidiOutPort from output device");
                    }
                    else if (portPair.InPort == null)
                    {
                        Debug.WriteLine("Unable to create MidiInPort from input device");
                    }
                    else
                    {
                        portPair.InPort.MessageReceived += mainPage.InPort_MessageReceived;
                        SendIdRequests(portPair);
                        Debug.WriteLine("SendIdRequests(portPair) expectingAnswer = " + 
                            expectingAnswer.ToString());
                        Task.Delay(10).Wait();
                        expectingAnswer++;
                        portPair.IsConnected = true;
                    }
                }
            } catch { }
        }

        //public async Task AddUnknown(List<String> deviceNames)
        //{
        //    DeviceInformationCollection midiOutputDevices = await DeviceInformation.FindAllAsync(MidiOutPort.GetDeviceSelector());
        //    DeviceInformationCollection midiInputDevices = await DeviceInformation.FindAllAsync(MidiInPort.GetDeviceSelector());
        //    DeviceInformation midiOutDevInfo = null;
        //    DeviceInformation midiInDevInfo = null;

        //    foreach (DeviceInformation outputDevice in midiOutputDevices)
        //    {
        //        if (!Exclude(deviceNames, outputDevice.Name) && !outputDevice.Name.Contains("CTRL"))
        //        {
        //            midiOutDevInfo = outputDevice;
        //            PortPair portPair = new PortPair();

        //            portPair.Name = outputDevice.Name;
        //            String compareName = portPair.Name;
        //            Int32 pos = compareName.IndexOf('[');
        //            if (pos > -1)
        //            {
        //                compareName = compareName.Remove(outputDevice.Name.IndexOf('[') - 1);
        //            }

        //            if (midiOutDevInfo != null)
        //            {
        //                portPair.OutPort = await MidiOutPort.FromIdAsync(midiOutDevInfo.Id);
        //            }

        //            foreach (DeviceInformation inputDevice in midiInputDevices)
        //            {
        //                if (inputDevice.Name.Contains(compareName) && !inputDevice.Name.Contains("CTRL"))
        //                {
        //                    midiInDevInfo = inputDevice;
        //                    break;
        //                }
        //            }

        //            if (midiInDevInfo != null)
        //            {
        //                portPair.InPort = await MidiInPort.FromIdAsync(midiInDevInfo.Id);
        //            }

        //            if (portPair.OutPort == null)
        //            {
        //                //System.Diagnostics.Debug.WriteLine("Unable to create MidiOutPort from output device");
        //            }

        //            if (portPair.InPort == null)
        //            {
        //                //System.Diagnostics.Debug.WriteLine("Unable to create MidiInPort from input device");
        //            }

        //            if (portPair.InPort != null && portPair.OutPort != null)
        //            {
        //                PortPairs.Add(portPair);
        //            }
        //        }
        //    }
        //}

        //private Boolean Exclude(List<String> Names, String Name)
        //{
        //    Boolean exclude = false;
        //    foreach (String name in Names)
        //    if (Name.Contains(name))
        //    {
        //            exclude = true;
        //    }
        //    return exclude;
        //}

        //public List<String> GetMidiDeviceList()
        //{
        //    return MidiDevices;
        //}

        //public async Task MakeMidiDeviceList()
        //{
        //    DeviceInformationCollection midiInputDevices = await DeviceInformation.FindAllAsync(MidiInPort.GetDeviceSelector());
        //    MidiDevices = new List<String>();
        //    foreach (DeviceInformation device in midiInputDevices)
        //    {
        //        if (device.Name.Contains("["))
        //        {
        //            MidiDevices.Add(device.Name.Remove(device.Name.IndexOf('[')));
        //        }
        //        else
        //        {
        //            MidiDevices.Add(device.Name);
        //        }
        //    }
        //}

        //public Boolean MidiIsReady()
        //{
        //    foreach (PortPair portPair in PortPairs)
        //    {
        //        if (portPair.InPort == null || portPair.OutPort == null)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public byte[] AddressFromSysExRespons(byte[] bytes)
        //{
        //    if (bytes.Length > 11)
        //    {
        //        byte[] bAddress = new byte[4];
        //        for (Int32 i = 0; i < 4; i++)
        //        {
        //            bAddress[i] = bytes[i + 7];
        //        }
        //        return bAddress;
        //    }
        //    else return null;
        //}

        //public byte[] DataBytesFromSysExRespons(byte[] bytes)
        //{
        //    if (bytes.Length > 11)
        //    {
        //        return new byte[] { bytes[11] };
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //public Boolean IsIdRequestResponse(byte[] bytes)
        //{
        //    return bytes[0] == 0xf0 && bytes[1] == 0x7e && bytes[3] == 0x06;
        //}

        //public async void NoteOn(PortPair portPair, byte noteNumber, byte velocity)
        //{
        //    try
        //    {
        //        if (portPair.OutPort != null)
        //        {
        //            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(portPair.OutChannel, noteNumber, velocity);
        //            portPair.OutPort.SendMessage(midiMessageToSend);
        //        }
        //    }
        //    catch
        //    {
        //        MessageDialog warning = new MessageDialog("Communication with your " + portPair.Name + " has been lost. Please verify connection and restart the app.");
        //        warning.Title = "Warning!";
        //        warning.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
        //        var response = await warning.ShowAsync();
        //    }
        //}

        //public async void NoteOff(PortPair portPair, byte currentChannel, byte noteNumber)
        //{
        //    try
        //    {
        //        if (portPair.OutPort != null)
        //        {
        //            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(portPair.OutChannel, noteNumber, 0);
        //            portPair.OutPort.SendMessage(midiMessageToSend);
        //        }
        //    }
        //    catch
        //    {
        //        MessageDialog warning = new MessageDialog("Communication with your " + portPair.Name + " has been lost. Please verify connection and restart the app.");
        //        warning.Title = "Warning!";
        //        warning.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
        //        var response = await warning.ShowAsync();
        //    }
        //}

        public void SendControlChange(PortPair portPair, byte controller, byte value)
        {
            if (portPair.IsConnected)
            {
                try
                {
                    if (portPair.OutPort != null)
                    {
                        IMidiMessage midiMessageToSend = new MidiControlChangeMessage(portPair.OutChannel, controller, value);
                        portPair.OutPort.SendMessage(midiMessageToSend);
                    }
                }
                catch
                {
                    portPair.IsConnected = false;
                }
            }
        }

        public void SendProgramChange(PortPair portPair, byte value)
        {
            if (portPair.IsConnected)
            {
                try
                {
                    if (portPair.OutPort != null)
                    {
                        IMidiMessage midiMessageToSend = new MidiProgramChangeMessage(portPair.OutChannel, value);
                        portPair.OutPort.SendMessage(midiMessageToSend);
                    }
                }
                catch
                {
                    portPair.IsConnected = false;
                }
            }
        }

        public async void SendPitchBender(PortPair portPair, Int32 value)
        {
            try
            {
                if (portPair.OutPort != null)
                {
                    //IMidiMessage midiMessageToSend = new MidiPitchBendChangeMessage(portPair.OutChannel, (UInt16)value);
                    byte[] msg = new byte[] { (byte)(0xe0 + portPair.OutChannel), (byte)(value % 128), (byte)(value / 128) };
                    portPair.OutPort.SendBuffer(msg.AsBuffer());
                }
            }
            catch
            {
                MessageDialog warning = new MessageDialog("Communication with your VT-4 has been lost. Please verify connection and restart the app.");
                warning.Title = "Warning!";
                warning.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                var response = await warning.ShowAsync();
            }
        }

        //public async void SetVolume(PortPair portPair, byte currentChannel, byte volume)
        //{
        //    try
        //    {
        //        if (portPair.OutPort != null)
        //        {
        //            IMidiMessage midiMessageToSend = new MidiControlChangeMessage(portPair.OutChannel, 0x07, volume);
        //            portPair.OutPort.SendMessage(midiMessageToSend);
        //        }
        //    }
        //    catch
        //    {
        //        MessageDialog warning = new MessageDialog("Communication with your VT-4 has been lost. Please verify connection and restart the app.");
        //        warning.Title = "Warning!";
        //        warning.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
        //        var response = await warning.ShowAsync();
        //    }
        //}

        //public async void AllNotesOff(PortPair portPair, byte currentChannel)
        //{
        //    try
        //    {
        //        if (portPair.OutPort != null)
        //        {
        //            IMidiMessage midiMessageToSend = new MidiControlChangeMessage(portPair.OutChannel, 0x78, 0);
        //            portPair.OutPort.SendMessage(midiMessageToSend);
        //        }
        //    }
        //    catch
        //    {
        //        MessageDialog warning = new MessageDialog("Communication with your VT-4 has been lost. Please verify connection and restart the app.");
        //        warning.Title = "Warning!";
        //        warning.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
        //        var response = await warning.ShowAsync();
        //    }
        //}

        //public async void ProgramChange(PortPair portPair, byte currentChannel, String smsb, String slsb, String spc)
        //{
        //    try
        //    {
        //        MidiControlChangeMessage controlChangeMsb = new MidiControlChangeMessage(portPair.OutChannel, 0x00, (byte)(UInt16.Parse(smsb)));
        //        MidiControlChangeMessage controlChangeLsb = new MidiControlChangeMessage(portPair.OutChannel, 0x20, (byte)(UInt16.Parse(slsb)));
        //        MidiProgramChangeMessage programChange = new MidiProgramChangeMessage(portPair.OutChannel, (byte)(UInt16.Parse(spc) - 1));
        //        portPair.OutPort.SendMessage(controlChangeMsb);
        //        portPair.OutPort.SendMessage(controlChangeLsb);
        //        portPair.OutPort.SendMessage(programChange);
        //    }
        //    catch
        //    {
        //        MessageDialog warning = new MessageDialog("Communication with your VT-4 has been lost. Please verify connection and restart the app.");
        //        warning.Title = "Warning!";
        //        warning.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
        //        var response = await warning.ShowAsync();
        //    }
        //}

        //public async void ProgramChange(PortPair portPair, byte currentChannel, byte msb, byte lsb, byte pc)
        //{
        //    try
        //    {
        //        MidiControlChangeMessage controlChangeMsb = new MidiControlChangeMessage(portPair.OutChannel, 0x00, msb);
        //        MidiControlChangeMessage controlChangeLsb = new MidiControlChangeMessage(portPair.OutChannel, 0x20, lsb);
        //        MidiProgramChangeMessage programChange = new MidiProgramChangeMessage(currentChannel, (byte)(pc - 1));
        //        portPair.OutPort.SendMessage(controlChangeMsb);
        //        portPair.OutPort.SendMessage(controlChangeLsb);
        //        portPair.OutPort.SendMessage(programChange);
        //    }
        //    catch
        //    {
        //        MessageDialog warning = new MessageDialog("Communication with your VT-4 has been lost. Please verify connection and restart the app.");
        //        warning.Title = "Warning!";
        //        warning.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
        //        var response = await warning.ShowAsync();
        //    }
        //}

        //public async void SetInControl(PortPair portPair)
        //{
        //    try
        //    {
        //        //MidiNoteOnMessage InControlMesssage = new MidiNoteOnMessage(0x0f, 0x00, 0x7f);
        //        //portPair.OutPort.SendMessage(InControlMesssage);
        //        byte[] bytes = { 0x9f, (byte)(portPair.OutChannel + 0x0c), 0x7f };
        //        IBuffer buffer = bytes.AsBuffer();
        //        portPair.OutPort.SendBuffer(buffer);
        //    }
        //    catch (Exception e)
        //    {
        //        MessageDialog warning = new MessageDialog("Communication with your VT-4 has been lost. Please verify connection and restart the app.");
        //        warning.Title = "Warning!";
        //        warning.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
        //        var response = await warning.ShowAsync();
        //    }
        //}

        //public async void ResetInControl(PortPair portPair)
        //{
        //    try
        //    {
        //        MidiControlChangeMessage InControlMesssage = new MidiControlChangeMessage(0x9f, 0x00, 0x00);
        //        portPair.OutPort.SendMessage(InControlMesssage);
        //    }
        //    catch
        //    {
        //        MessageDialog warning = new MessageDialog("Communication with your VT-4 has been lost. Please verify connection and restart the app.");
        //        warning.Title = "Warning!";
        //        warning.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
        //        var response = await warning.ShowAsync();
        //    }
        //}

        private void SendIdRequests(PortPair portPair)
        {
            for (byte i = 0x10; i < 0x80 && portPair.DeviceId == 0; i++)
            {
                portPair.OutPort.SendBuffer((new byte[] { 0xf0, 0x7e, i, 0x06, 0x01, 0xf7 }).AsBuffer());
                Task.Delay(10);
            }
        }

        public void SendSystemExclusive(PortPair portPair, byte[] bytes)
        {
            if (portPair.IsConnected)
            {
                try
                {
                    IBuffer buffer = bytes.AsBuffer();
                    portPair.OutPort.SendBuffer(buffer);
                }
                catch
                {
                    portPair.IsConnected = false;
                    //MessageDialog warning = new MessageDialog("Communication with your VT-4 has been lost. Please verify connection and restart the app.");
                    //warning.Title = "Warning!";
                    //warning.Commands.Add(new UICommand { Label = "Ok", Id = 0 });
                    //var response = await warning.ShowAsync();
                }
            }
        }

        //public byte[] SystemExclusiveRQ1Message(DeviceInfo device, byte[] Address, byte[] Length)
        //{
        //    byte[] result = new byte[17];
        //    result[0] = 0xf0; // Start of exclusive message
        //    result[1] = device.CompanyNumber;
        //    result[2] = device.UnitNumber;
        //    result[3] = device.SysExId[0];
        //    result[4] = device.SysExId[1];
        //    result[5] = device.SysExId[2];
        //    result[6] = 0x11; // Command (RQ1)
        //    result[7] = Address[0];
        //    result[8] = Address[1];
        //    result[9] = Address[2];
        //    result[10] = Address[3];
        //    result[11] = Length[0];
        //    result[12] = Length[1];
        //    result[13] = Length[2];
        //    result[14] = Length[3];
        //    result[15] = 0x00; // Filled out by CheckSum but present here to avoid confusion about index 15 missing.
        //    result[16] = 0xf7; // End of sysex
        //    CheckSum(ref result);
        //    return (result);
        //}

        public byte[] SystemExclusiveRQ1Message(PortPair portPair, byte[] Address, byte[] Length)
        {
            byte[] result = new byte[18];
            int i = 0;
            result[i++] = 0xf0; // Start of exclusive message
            result[i++] = portPair.Manufacturer;
            for (int j = 0; j < portPair.Model.Length; j++)
            {
                result[i++] = portPair.Model[j];
            }
            result[i++] = 0x11;         // Command (RQ1)
            result[i++] = Address[0];
            result[i++] = Address[1];
            result[i++] = Address[2];
            result[i++] = Address[3];
            result[i++] = Length[0];
            result[i++] = Length[1];
            result[i++] = Length[2];
            result[i++] = Length[3];
            result[i++] = CheckSum(result, portPair.Model.Length + 2); // Filled out by CheckSum but present here to avoid confusion about index 15 missing.
            result[i++] = 0xf7; // End of sysex
            //CheckSum(ref result);
            return (result);
        }

        public byte[] SystemExclusiveDT1Message(PortPair portPair, byte[] Address, byte[] DataToTransmit)
        {
            Int32 length = 9 + portPair.Model.Length + DataToTransmit.Length; // 9 = SysEx start (f0), Manufacturer, Command (12), address, checksum and SysEx end (f7) counted
            byte[] result = new byte[length];
            int i = 0;
            result[i++] = 0xf0; // Start of exclusive message
            result[i++] = portPair.Manufacturer;
            for (int j = 0; j < portPair.Model.Length; j++)
            {
                result[i++] = portPair.Model[j];
            }
            result[i++] = 0x12; // Command (DT1)
            result[i++] = Address[0];
            result[i++] = Address[1];
            result[i++] = Address[2];
            result[i++] = Address[3];
            for (int j = 0; j < DataToTransmit.Length; j++)
            {
                result[i++] = DataToTransmit[j];
            }
            result[i++] = CheckSum(result, portPair.Model.Length + 2); // 2 = Manufacturer and DeviceId counted.
            result[i++] = 0xf7; // End of sysex
            
            return (result);
        }

        public byte CheckSum(byte[] bytes, int deviceInfoLength)
        {
            byte chksum = 0;
            for (Int32 i = deviceInfoLength + 1; i < bytes.Length - 1; i++) // 2 = SysEx start and Command skipped
            {
                chksum += bytes[i];
            }
            return chksum = (byte)((0x80 - (chksum & 0x7f)));
        }
    }

    public class PortPair
    {
        public Int32 ID { get;set; }
        public String Name { get; set; }
        public MidiInPort InPort;
        public byte DeviceId = 0;
        public byte Manufacturer;
        public byte[] Model;
        public bool IsConnected = false;
        public byte InChannel { get; set; }
        //public Int32 MidiInPortSelectedIndex { get; set; }
        public IMidiOutPort OutPort;
        public byte OutChannel { get; set; }
        //public Int32 MidiOutPortSelectedIndex { get; set; }
    }
}
