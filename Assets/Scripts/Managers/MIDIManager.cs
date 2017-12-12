using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Sanford.Multimedia.Midi;
using UnityEngine;

public class MIDIManager : Singleton<MIDIManager>
{
    private const int SysExBufferSize = 128;
    private InputDevice _inputDevice = null;
    private OutputDevice _outputDevice = null;
    private SynchronizationContext _context;

    [SerializeField]
    private Dictionary<int, string> activeCommands = new Dictionary<int, string>();

    public string GetDeviceName()
    {
        return InputDevice.GetDeviceCapabilities(_inputDevice.DeviceID).name;
    }

    public int GetDeviceCount()
    {
        return InputDevice.DeviceCount;
    }

    private void Start()
    {
        SerialManager.Instance.SerialDataReceived += HandleSerialDataReceived;
        if (InputDevice.DeviceCount == 0)
        {
            Log.Error("No MIDI input devices available");
        }
        else
        {
            try
            {
                _context = SynchronizationContext.Current;
                _inputDevice = new InputDevice(AppManager.Instance.Settings["MIDI Input Device ID"].IntValue);
                _inputDevice.ChannelMessageReceived += HandleChannelMessageReceived;
                _inputDevice.StartRecording();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error: " + ex);
            }
        }

        if (OutputDevice.DeviceCount == 0)
        {
            Log.Error("No MIDI output devices available");
        }
        else
        {
            try
            {
                _context = SynchronizationContext.Current;
                _outputDevice = new OutputDevice(AppManager.Instance.Settings["MIDI Output Device ID"].IntValue);
                Greet();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error: " + ex);
            }
        }
    }

    private void WriteOut(int[] data, bool on = true)
    {
        for (var i = 0; i < data.Length; i++)
        {
            var builder = new ChannelMessageBuilder
            {
                Command = ChannelCommand.NoteOn,
                Data1 = data[i],
                Data2 = (on ? 127 : 0)
            };

            builder.Build();

            _outputDevice.Send(builder.Result);
        }

        _outputDevice.Error += (sender, eventArgs) => { Debug.LogException(eventArgs.Error); };
    }

    public void Greet()
    {
        StopAllCoroutines();
        StartCoroutine(IGreet());
    }

    private IEnumerator IGreet()
    {
        yield return new WaitForSeconds(.1f);
        ControlLights();
        yield return new WaitForSeconds(.2f);
        ControlLights(false);
        yield return new WaitForSeconds(.1f);
        LightsPlay();
    }

    private void ControlLights(bool on = true)
    {
        WriteOut(new[]
        {
            35,
            34,
            33,
            1,
            2,
            3,
            4,
            43,
            49,
            50,
            51,
            52,
            45,
            83,
            82,
            81
        }, on);
    }

    private void LightsPlay()
    {
        StartCoroutine(ILightsPlayLeft());
        StartCoroutine(ILightsPlayRight());
    }

    private IEnumerator ILightsPlayLeft()
    {
        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {1});

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {2});
        WriteOut(new[] {1}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {4});
        WriteOut(new[] {2}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {3});
        WriteOut(new[] {4}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {1});
        WriteOut(new[] {3}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {1}, false);
    }

    private IEnumerator ILightsPlayRight()
    {
        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {50});

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {49});
        WriteOut(new[] {50}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {51});
        WriteOut(new[] {49}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {52});
        WriteOut(new[] {51}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {50});
        WriteOut(new[] {52}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {50}, false);
    }

    private void HandleSerialDataReceived(object sender, SerialMessageEventArgs e)
    {
//        Log.Info("<color=grey><b>" + e.Message + "</b></color>");

        if (!_isRadioOn) return;

        StopCoroutine("ActivityLight");
        StartCoroutine(ActivityLight());
    }

    private IEnumerator ActivityLight()
    {
        WriteOut(new[] {45});

        yield return new WaitForSeconds(.1f);

        WriteOut(new[] {45}, false);
    }

    private void HandleChannelMessageReceived(object sender, ChannelMessageEventArgs e)
    {
        _context.Post(delegate(object dummy)
        {
            Log.Info("<color=blue><b>" +
                     e.Message.Data1 +
                     " | " +
                     e.Message.Data2 +
                     " | " +
                     e.Message.Data2.ToString("X") +
                     " | " +
                     e.Message.Command.GetHashCode().ToString() +
                     " | " +
                     e.Message.Command.ToString() +
                     "</b></color>");

            var channelCommand = e.Message.Command;

            if (!_isRadioOn && e.Message.Data1 != 43)
            {
                Greet();
                return;
            }

            switch (channelCommand)
            {
                case ChannelCommand.NoteOn:
                    HandleNote(e);
                    break;
                case ChannelCommand.Controller:
                    HandleController(e);
                    break;
            }
        }, null);
    }

    enum RadioMode
    {
        CW,
        LSB,
        USB,
        FSK
    }

    enum VFO
    {
        A,
        B
    }

    [SerializeField] private RadioMode _radioMode;
    [SerializeField] private VFO _vfo;
    [SerializeField] private bool _isRadioOn;
    [SerializeField] private bool _isSplitActive;

    private void SetSplit(int control)
    {
        if (control != 127) return;
        
        if (_vfo == VFO.A)
        {
            if (activeCommands.ContainsKey(4))
            {
                if (activeCommands[4] == "FT1;")
                {
                    activeCommands[4] = "FT0;";
                    _isSplitActive = false;
                    control = 0;
                }
                else
                {
                    activeCommands[4] = "FT1;";
                    _isSplitActive = true;
                }
            }
            else
            {
                activeCommands[4] = "FT1;";
                _isSplitActive = true;
            }
        }
        else
        {
            if (activeCommands.ContainsKey(4))
            {
                if (activeCommands[4] == "FT0;")
                {
                    activeCommands[4] = "FT1;";
                    _isSplitActive = false;
                    control = 0;
                }
                else
                {
                    activeCommands[4] = "FT0;";
                    _isSplitActive = true;
                }
            }
            else
            {
                activeCommands[4] = "FT0;";
                _isSplitActive = true;
            }
        }

        SendToRadio(4, activeCommands[4], control);
    }


    private void ChangeVFO(string vfo)
    {
        var command = "";
        int note = 0;
        int[] switchOff = null;
        
        // Reset split state
        _isSplitActive = false;

        switch (vfo)
        {
            case "A":
                command = "FR0;";
                _vfo = VFO.A;
                note = 35;
                switchOff = new[] {34, 4};
                AppManager.Instance.ActivateButton(DJControllerControl.LeftSync);
                activeCommands.Remove(4);
                break;

            case "B":
                command = "FR1;";
                _vfo = VFO.B;
                note = 34;
                switchOff = new[] {35, 4};
                AppManager.Instance.ActivateButton(DJControllerControl.LeftCue);
                activeCommands.Remove(4);
                break;
        }

        SendToRadio(note, command, switchOff);
    }

    private void ChangeRadioMode(string mode)
    {
        var command = "";
        var note = 0;
        int[] switchOff = null;

        switch (mode)
        {
            case "CW":
                command = "MD3;MD";
                _radioMode = RadioMode.CW;
                note = 51;
                switchOff = new[] {50, 49, 52};
                break;

            case "LSB":
                command = "MD1;MD";
                _radioMode = RadioMode.LSB;
                note = 49;
                switchOff = new[] {50, 51, 52};
                break;

            case "USB":
                command = "MD2;MD";
                _radioMode = RadioMode.USB;
                note = 50;
                switchOff = new[] {49, 51, 52};
                break;

            case "FSK":
                command = "MD6;MD";
                _radioMode = RadioMode.FSK;
                note = 52;
                switchOff = new[] {50, 51, 49};
                break;
        }

        SendToRadio(note, command, switchOff);
    }

    private void HandleController(ChannelMessageEventArgs e)
    {
        var note = e.Message.Data1;
        var control = e.Message.Data2;

        switch (note)
        {
            // VFO Tune
            case 48:
                AppManager.Instance.MoveJog(control, DJControllerControl.LeftJog);
                SendToRadio(control < 64 ? "UP;" : "DN;");
                break;

            // RIT Tune
            case 49:
                AppManager.Instance.MoveJog(control, DJControllerControl.RightJog);
                SendToRadio(control < 64 ? "RU;" : "RD;");
                break;

            // CW Speed
            case 54:
                var keySpeed = Mathf.FloorToInt(4 + (60 - 4) * control / 127);
                SendToRadio("KS" + keySpeed.ToString("D3") + ";");
                break;

            // DSP Low
            case 59:
                var dspLow = Mathf.FloorToInt(control / 11);
                SendToRadio("SL" + dspLow.ToString("D2") + ";");
                break;

            // DSP High
            case 63:
                var dspHigh = Mathf.FloorToInt(control / 11);
                SendToRadio("SH" + dspHigh.ToString("D2") + ";");
                break;

            // DSP Shift    
            case 64:
                var value = 0;

                switch (_radioMode)
                {
                    case RadioMode.CW:
                        value = Mathf.FloorToInt(100 + (1000 - 100) * control / 127);
                        break;
                    case RadioMode.FSK:
                        value = Mathf.FloorToInt(250 + (1500 - 250) * control / 127);
                        break;
                }

                SendToRadio("FW" + value.ToString("D4") + ";");
                break;

            // AF Gain
            case 57:
                var afValue = Mathf.FloorToInt(0 + (255 - 0) * control / 127);
                SendToRadio("AG0" + afValue.ToString("D3") + ";");
                break;

            // RF Gain
            case 61:
                var gain = Mathf.FloorToInt(0 + (255 - 0) * control / 127);
                SendToRadio("RG" + gain.ToString("D3") + ";");
                break;

            // Width
            case 60:
                var width = Mathf.FloorToInt(300 + (1000 - 300) * control / 127);
                SendToRadio("IS " + width.ToString("D4") + ";");
                break;
        }
    }

    private IEnumerator WakeUpRadio()
    {
        yield return new WaitForSeconds(.9f);

        ChangeRadioMode(AppManager.Instance.Settings["Default Radio Mode"].StringValue);
        // RIT Off
        SendToRadio(82, "RT0;");
        // Set default VFO
        ChangeVFO(AppManager.Instance.Settings["Default VFO"].StringValue);
        _isRadioOn = true;
    }

    private void HandleNote(ChannelMessageEventArgs e)
    {
        var note = e.Message.Data1;
        var control = e.Message.Data2;

        switch (note)
        {
            // ON/OFF
            case 43:
                if (control == 127)
                {
                    if (_isRadioOn)
                    {
                        // RIT Off
                        SendToRadio("RT0;");
                        SendToRadio("PS0;");
                        ControlLights(false);
                        _isRadioOn = false;
                    }
                    else
                    {
                        SendToRadio(43, "PS1;");
                        StartCoroutine(WakeUpRadio());
                    }
                }
                break;

            // VFO A
            case 35:
                ChangeVFO("A");
                break;

            // VFO B
            case 34:
                ChangeVFO("B");
                break;

            // A = B
            case 33:
                SendToRadio(33, "VV;", control);
                break;

            // AT Tune
            case 1:
                SendToRadio(1, "AC111;", control);
                break;

            // A/B
            case 2:
                if (control == 127)
                {
                    if (_vfo == VFO.A)
                    {
                        if (_isSplitActive)
                        {
                            SendToRadio(2, "FR1;FT0;", control, true);
                            activeCommands[4] = "FT0;";
                        }
                        else
                        {
                            SendToRadio(2, "FR1;FT1;", control, true);
                            activeCommands[4] = "FT1;";
                            
                        }
               
                        _vfo = VFO.B;
                        
                        WriteOut(new[] {34});
                        WriteOut(new[] {35}, false);
                    }
                    else
                    {
                        if (_isSplitActive)
                        {
                            SendToRadio(2, "FR0;FT1;", control, true);
                            activeCommands[4] = "FT1;";
                        }
                        else
                        {
                            SendToRadio(2, "FR0;FT0;", control, true);
                            activeCommands[4] = "FT0;";
                        }
                        _vfo = VFO.A;
                        
                        WriteOut(new[] {35});
                        WriteOut(new[] {34}, false);
                    }
                }
                break;

            // SEND
            case 3:
                SendToRadio(3, "TX0;", "RX;", control);
                break;

            // SPLIT
            case 4:
                SetSplit(control);
                break;

            // LSB    
            case 49:
                ChangeRadioMode("LSB");
                break;

            // USB
            case 50:
                ChangeRadioMode("USB");
                break;

            // CW   
            case 51:
                ChangeRadioMode("CW");
                break;

            // FSK
            case 52:
                ChangeRadioMode("FSK");
                break;

            // RIT ON
            case 83:
                SendToRadio(83, "RT1;", new[] {82});
                break;

            // RIT OFF
            case 82:
                SendToRadio(82, "RT0;", new[] {83});
                break;

            // RIT CLEAR
            case 81:
                SendToRadio(81, "RC;", control);
                break;
        }
    }


    private void SendToRadio(string message)
    {
        SerialManager.Instance.Write(message);
    }

    private void SendToRadio(int code, string onMessage, string offMessage, int control)
    {
        if (control == 127)
        {
            if (activeCommands.ContainsKey(code))
            {
                if (activeCommands[code] == onMessage)
                {
                    activeCommands[code] = offMessage;
                    SerialManager.instance.Write(offMessage);
                    WriteOut(new[] {code}, false);
                }
                else
                {
                    activeCommands[code] = onMessage;
                    SerialManager.instance.Write(onMessage);
                    WriteOut(new[] {code});
                }
            }
            else
            {
                activeCommands.Add(code, onMessage);
                SerialManager.instance.Write(onMessage);
                WriteOut(new[] {code});
            }
        }
    }

    private void SendToRadio(int code, string message, int control, bool forceClose = false)
    {
        StartCoroutine(ISendToRadio(code, message, control, forceClose));
    }

    private IEnumerator ISendToRadio(int code, string message, int control, bool forceClose)
    {
        WriteOut(new[] {code});

        SerialManager.Instance.Write(message);

        yield return new WaitForSeconds(.3f);

        if (control == 0 || forceClose)
            WriteOut(new[] {code}, false);
    }

    private void SendToRadio(int code, string message, int[] switchOffCodes = null)
    {
        WriteOut(new[] {code});

        if (switchOffCodes != null)
            WriteOut(switchOffCodes, false);

        SerialManager.Instance.Write(message);
    }

    private void OnApplicationQuit()
    {
        ControlLights(false);
        Log.Info("Closing the MIDI input device");
        _outputDevice.Close();
        _inputDevice.Close();
    }
}