using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Sanford.Multimedia.Midi;
using UnityEngine;

public class MIDIManager : Singleton<MIDIManager>
{
    private InputDevice _inputDevice = null;
    private OutputDevice _outputDevice = null;
    private SynchronizationContext _context;

    [SerializeField]
    private Dictionary<DJControllerControl, string> _activeCommands = new Dictionary<DJControllerControl, string>();

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
            AppManager.Instance.PushToLog("No MIDI input devices available", AppManager.LogType.Error);
        }
        else
        {
            try
            {
                _context = SynchronizationContext.Current;
                var deviceId = AppManager.Instance.Settings["MIDI Input Device ID"].IntValue;
                _inputDevice = new InputDevice(deviceId);
                AppManager.Instance.PushToLog("Connected to " + InputDevice.GetDeviceCapabilities(deviceId).name,
                    AppManager.LogType.Info);
                _inputDevice.ChannelMessageReceived += HandleChannelMessageReceived;
                _inputDevice.StartRecording();
            }
            catch (Exception ex)
            {
                AppManager.Instance.PushToLog(ex.Message, AppManager.LogType.Error);
                throw new ApplicationException("Error: " + ex);
            }
        }

        if (OutputDevice.DeviceCount == 0)
        {
            Log.Error("No MIDI output devices available");
            AppManager.Instance.PushToLog("No MIDI output devices available", AppManager.LogType.Error);
        }
        else
        {
            try
            {
                _context = SynchronizationContext.Current;
                var deviceId = AppManager.Instance.Settings["MIDI Output Device ID"].IntValue;
                _outputDevice = new OutputDevice(deviceId);
                AppManager.Instance.PushToLog("Using output device: " +
                                              OutputDevice.GetDeviceCapabilities(deviceId).name,
                    AppManager.LogType.Info);
                Greet();
            }
            catch (Exception ex)
            {
                AppManager.Instance.PushToLog(ex.Message, AppManager.LogType.Error);
                throw new ApplicationException("Error: " + ex);
            }
        }
    }

    private void WriteOut(IList<DJControllerControl> data, bool on = true)
    {
        for (var i = 0; i < data.Count; i++)
        {
            var builder = new ChannelMessageBuilder
            {
                Command = ChannelCommand.NoteOn,
                Data1 = (int) data[i],
                Data2 = (on ? 127 : 0)
            };

            builder.Build();

            _outputDevice.Send(builder.Result);
        }

        _outputDevice.Error += (sender, eventArgs) =>
        {
            AppManager.Instance.PushToLog(eventArgs.Error.Message, AppManager.LogType.Error);
            Debug.LogException(eventArgs.Error);
        };
    }

    private void Greet()
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
            DJControllerControl.LeftSync,
            DJControllerControl.LeftCue,
            DJControllerControl.LeftPlayPause,
            DJControllerControl.LeftPad1,
            DJControllerControl.LeftPad2,
            DJControllerControl.LeftPad3,
            DJControllerControl.LeftPad4,
            DJControllerControl.Rec,
            DJControllerControl.RightPad1,
            DJControllerControl.RightPad2,
            DJControllerControl.RightPad3,
            DJControllerControl.RightPad4,
            DJControllerControl.ScratchAtomix,
            DJControllerControl.RightSync,
            DJControllerControl.RightCue,
            DJControllerControl.RightPlayPause,
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
        WriteOut(new[] {DJControllerControl.LeftPad1,});

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {DJControllerControl.LeftPad2});
        WriteOut(new[] {DJControllerControl.LeftPad1}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {DJControllerControl.LeftPad4});
        WriteOut(new[] {DJControllerControl.LeftPad2}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {DJControllerControl.LeftPad3});
        WriteOut(new[] {DJControllerControl.LeftPad4}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {DJControllerControl.LeftPad1});
        WriteOut(new[] {DJControllerControl.LeftPad3}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {DJControllerControl.LeftPad1}, false);
    }

    private IEnumerator ILightsPlayRight()
    {
        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {DJControllerControl.RightPad2});

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {DJControllerControl.RightPad1});
        WriteOut(new[] {DJControllerControl.RightPad2}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {DJControllerControl.RightPad3});
        WriteOut(new[] {DJControllerControl.RightPad1}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {DJControllerControl.RightPad4});
        WriteOut(new[] {DJControllerControl.RightPad3}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {DJControllerControl.RightPad2});
        WriteOut(new[] {DJControllerControl.RightPad4}, false);

        yield return new WaitForSeconds(.1f);
        WriteOut(new[] {DJControllerControl.RightPad2}, false);
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
        WriteOut(new[] {DJControllerControl.ScratchAtomix});

        yield return new WaitForSeconds(.1f);

        WriteOut(new[] {DJControllerControl.ScratchAtomix}, false);
    }

    private void HandleChannelMessageReceived(object sender, ChannelMessageEventArgs e)
    {
        _context.Post(delegate(object dummy)
        {
            var logMsg = "<b>" + e.Message.Data1 + "</b>" +
                         " | " +
                         e.Message.Data2 +
                         " | " +
                         e.Message.Data2.ToString("X") +
                         " | " +
                         e.Message.Command.GetHashCode().ToString() +
                         " | " +
                         e.Message.Command.ToString();

            Log.Info("<color=blue><b>" + logMsg + "</b></color>");

            var channelCommand = e.Message.Command;

            if (!_isRadioOn && e.Message.Data1 != 43)
            {
                AppManager.Instance.PushToLog("Press REC button to enable communication...");
                Greet();
                return;
            }

            AppManager.Instance.PushToLog(logMsg);

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
            if (_activeCommands.ContainsKey(DJControllerControl.LeftPad4))
            {
                if (_activeCommands[DJControllerControl.LeftPad4] == "FT1;")
                {
                    _activeCommands[DJControllerControl.LeftPad4] = "FT0;";
                    _isSplitActive = false;
                    control = 0;
                }
                else
                {
                    _activeCommands[DJControllerControl.LeftPad4] = "FT1;";
                    _isSplitActive = true;
                }
            }
            else
            {
                _activeCommands[DJControllerControl.LeftPad4] = "FT1;";
                _isSplitActive = true;
            }
        }
        else
        {
            if (_activeCommands.ContainsKey(DJControllerControl.LeftPad4))
            {
                if (_activeCommands[DJControllerControl.LeftPad4] == "FT0;")
                {
                    _activeCommands[DJControllerControl.LeftPad4] = "FT1;";
                    _isSplitActive = false;
                    control = 0;
                }
                else
                {
                    _activeCommands[DJControllerControl.LeftPad4] = "FT0;";
                    _isSplitActive = true;
                }
            }
            else
            {
                _activeCommands[DJControllerControl.LeftPad4] = "FT0;";
                _isSplitActive = true;
            }
        }

        SendToRadio(DJControllerControl.LeftPad4, _activeCommands[DJControllerControl.LeftPad4], control);
    }


    private void ChangeVFO(string vfo)
    {
        var command = "";
        DJControllerControl note = DJControllerControl.None;
        DJControllerControl[] switchOff = null;

        // Reset split state
        _isSplitActive = false;

        switch (vfo)
        {
            case "A":
                command = "FR0;";
                _vfo = VFO.A;
                note = DJControllerControl.LeftSync;
                switchOff = new[] {DJControllerControl.LeftCue, DJControllerControl.LeftPad4};
                AppManager.Instance.ActivateButton(DJControllerControl.LeftSync);
                _activeCommands.Remove(DJControllerControl.LeftPad4);
                break;

            case "B":
                command = "FR1;";
                _vfo = VFO.B;
                note = DJControllerControl.LeftCue;
                switchOff = new[] {DJControllerControl.LeftSync, DJControllerControl.LeftPad4};
                AppManager.Instance.ActivateButton(DJControllerControl.LeftCue);
                _activeCommands.Remove(DJControllerControl.LeftPad4);
                break;
        }

        SendToRadio(note, command, switchOff);
    }

    private void ChangeRadioMode(string mode)
    {
        var command = "";
        DJControllerControl note = DJControllerControl.None;
        DJControllerControl[] switchOff = null;

        switch (mode)
        {
            case "CW":
                command = "MD3;MD";
                _radioMode = RadioMode.CW;
                note = DJControllerControl.RightPad3;
                switchOff = new[]
                {
                    DJControllerControl.RightPad2,
                    DJControllerControl.RightPad1,
                    DJControllerControl.RightPad4
                };
                break;

            case "LSB":
                command = "MD1;MD";
                _radioMode = RadioMode.LSB;
                note = DJControllerControl.RightPad1;
                switchOff = new[]
                {
                    DJControllerControl.RightPad2,
                    DJControllerControl.RightPad3,
                    DJControllerControl.RightPad4
                };
                break;

            case "USB":
                command = "MD2;MD";
                _radioMode = RadioMode.USB;
                note = DJControllerControl.RightPad2;
                switchOff = new[]
                {
                    DJControllerControl.RightPad1,
                    DJControllerControl.RightPad3,
                    DJControllerControl.RightPad4
                };
                break;

            case "FSK":
                command = "MD6;MD";
                _radioMode = RadioMode.FSK;
                note = DJControllerControl.RightPad4;
                switchOff = new[]
                {
                    DJControllerControl.RightPad2,
                    DJControllerControl.RightPad3,
                    DJControllerControl.RightPad1
                };
                break;
        }

        SendToRadio(note, command, switchOff);
    }

    private void HandleController(ChannelMessageEventArgs e)
    {
        var note = e.Message.Data1;
        var control = e.Message.Data2;
        var key = (DJControllerControl) note;

        switch (key)
        {
            // VFO Tune
            case DJControllerControl.LeftJog:
                AppManager.Instance.MoveJog(control, DJControllerControl.LeftJog);
                SendToRadio(control < 64 ? "UP;" : "DN;");
                break;

            // RIT Tune
            case DJControllerControl.RightJog:
                AppManager.Instance.MoveJog(control, DJControllerControl.RightJog);
                SendToRadio(control < 64 ? "RU;" : "RD;");
                break;

            // CW Speed
            case DJControllerControl.XFader:
                var keySpeed = Mathf.FloorToInt(4 + (60 - 4) * control / 127);
                SendToRadio("KS" + keySpeed.ToString("D3") + ";");
                break;

            // DSP Low
            case DJControllerControl.LeftMedium:
                var dspLow = Mathf.FloorToInt(control / 11);
                SendToRadio("SL" + dspLow.ToString("D2") + ";");
                break;

            // DSP High
            case DJControllerControl.RightMedium:
                var dspHigh = Mathf.FloorToInt(control / 11);
                SendToRadio("SH" + dspHigh.ToString("D2") + ";");
                break;

            // DSP Shift    
            case DJControllerControl.RightBass:
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
            case DJControllerControl.LeftVolume:
                var afValue = Mathf.FloorToInt(0 + (255 - 0) * control / 127);
                SendToRadio("AG0" + afValue.ToString("D3") + ";");
                break;

            // RF Gain
            case DJControllerControl.RightVolume:
                var gain = Mathf.FloorToInt(0 + (255 - 0) * control / 127);
                SendToRadio("RG" + gain.ToString("D3") + ";");
                break;

            // Width
            case DJControllerControl.LeftBass:
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
        SendToRadio(DJControllerControl.RightCue, "RT0;");
        // Set default VFO
        ChangeVFO(AppManager.Instance.Settings["Default VFO"].StringValue);
        _isRadioOn = true;

        AppManager.Instance.PushToLog("Communication enabled", AppManager.LogType.Info);
    }

    private void HandleNote(ChannelMessageEventArgs e)
    {
        var note = e.Message.Data1;
        var control = e.Message.Data2;
        var key = (DJControllerControl) note;

        switch (key)
        {
            // ON/OFF
            case DJControllerControl.Rec:
                if (control == 127)
                {
                    if (_isRadioOn)
                    {
                        // RIT Off
                        SendToRadio("RT0;");
                        SendToRadio("PS0;");
                        ControlLights(false);
                        _isRadioOn = false;
                        AppManager.Instance.PushToLog("Communication disabled", AppManager.LogType.Info);
                    }
                    else
                    {
                        SendToRadio(DJControllerControl.Rec, "PS1;");
                        StartCoroutine(WakeUpRadio());
                    }
                }
                break;

            // VFO A
            case DJControllerControl.LeftSync:
                ChangeVFO("A");
                break;

            // VFO B
            case DJControllerControl.LeftCue:
                ChangeVFO("B");
                break;

            // A = B
            case DJControllerControl.LeftPlayPause:
                SendToRadio(DJControllerControl.LeftPlayPause, "VV;", control);
                break;

            // AT Tune
            case DJControllerControl.LeftPad1:
                SendToRadio(DJControllerControl.LeftPad1, "AC111;", control);
                break;

            // A/B
            case DJControllerControl.LeftPad2:
                if (control == 127)
                {
                    if (_vfo == VFO.A)
                    {
                        if (_isSplitActive)
                        {
                            SendToRadio(DJControllerControl.LeftPad2, "FR1;FT0;", control, true);
                            _activeCommands[DJControllerControl.LeftPad4] = "FT0;";
                        }
                        else
                        {
                            SendToRadio(DJControllerControl.LeftPad2, "FR1;FT1;", control, true);
                            _activeCommands[DJControllerControl.LeftPad4] = "FT1;";
                        }

                        _vfo = VFO.B;

                        WriteOut(new[] {DJControllerControl.LeftCue});
                        WriteOut(new[] {DJControllerControl.LeftSync}, false);
                    }
                    else
                    {
                        if (_isSplitActive)
                        {
                            SendToRadio(DJControllerControl.LeftPad2, "FR0;FT1;", control, true);
                            _activeCommands[DJControllerControl.LeftPad4] = "FT1;";
                        }
                        else
                        {
                            SendToRadio(DJControllerControl.LeftPad2, "FR0;FT0;", control, true);
                            _activeCommands[DJControllerControl.LeftPad4] = "FT0;";
                        }
                        _vfo = VFO.A;

                        WriteOut(new[] {DJControllerControl.LeftSync});
                        WriteOut(new[] {DJControllerControl.LeftCue}, false);
                    }
                }
                break;

            // SEND
            case DJControllerControl.LeftPad3:
                SendToRadio(DJControllerControl.LeftPad3, "TX0;", "RX;", control);
                break;

            // SPLIT
            case DJControllerControl.LeftPad4:
                SetSplit(control);
                break;

            // LSB    
            case DJControllerControl.RightPad1:
                ChangeRadioMode("LSB");
                break;

            // USB
            case DJControllerControl.RightPad2:
                ChangeRadioMode("USB");
                break;

            // CW   
            case DJControllerControl.RightPad3:
                ChangeRadioMode("CW");
                break;

            // FSK
            case DJControllerControl.RightPad4:
                ChangeRadioMode("FSK");
                break;

            // RIT ON
            case DJControllerControl.RightSync:
                SendToRadio(DJControllerControl.RightSync, "RT1;", new[] {DJControllerControl.RightCue});
                break;

            // RIT OFF
            case DJControllerControl.RightCue:
                SendToRadio(DJControllerControl.RightCue, "RT0;", new[] {DJControllerControl.RightSync});
                break;

            // RIT CLEAR
            case DJControllerControl.RightPlayPause:
                SendToRadio(DJControllerControl.RightPlayPause, "RC;", control);
                break;
        }
    }


    private void SendToRadio(string message)
    {
        SerialManager.Instance.Write(message);
    }

    private void SendToRadio(DJControllerControl key, string onMessage, string offMessage, int control)
    {
        if (control == 127)
        {
            if (_activeCommands.ContainsKey(key))
            {
                if (_activeCommands[key] == onMessage)
                {
                    _activeCommands[key] = offMessage;
                    SerialManager.instance.Write(offMessage);
                    WriteOut(new[] {key}, false);
                }
                else
                {
                    _activeCommands[key] = onMessage;
                    SerialManager.instance.Write(onMessage);
                    WriteOut(new[] {key});
                }
            }
            else
            {
                _activeCommands.Add(key, onMessage);
                SerialManager.instance.Write(onMessage);
                WriteOut(new[] {key});
            }
        }
    }

    private void SendToRadio(DJControllerControl key, string message, int control, bool forceClose = false)
    {
        StartCoroutine(ISendToRadio(key, message, control, forceClose));
    }

    private IEnumerator ISendToRadio(DJControllerControl key, string message, int control, bool forceClose)
    {
        WriteOut(new[] {key});

        SerialManager.Instance.Write(message);

        yield return new WaitForSeconds(.3f);

        if (control == 0 || forceClose)
            WriteOut(new[] {key}, false);
    }

    private void SendToRadio(DJControllerControl key, string message, DJControllerControl[] switchOffCodes = null)
    {
        WriteOut(new[] {key});

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