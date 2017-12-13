using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using DG.Tweening;
using Sanford.Multimedia.Midi;
using SharpConfig;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control mapping on Hercules DJControl Compact USB Controller
/// </summary>
public enum DJControllerControl
{
    None = 0,
    LeftJog = 48,
    LeftSync = 35,
    LeftCue = 34,
    LeftPlayPause = 33,
    Rec = 43,
    Mode = 48,
    LeftPad1 = 1,
    LeftPad2 = 2,
    LeftPad3 = 3,
    LeftPad4 = 4,
    XFader = 54,
    LeftVolume = 57,
    LeftMedium = 59,
    LeftBass = 60,
    RightMedium = 63,
    RightBass = 64,
    RightVolume = 61,
    ScratchAtomix = 45,
    Shift = 47,
    RightPad1 = 49,
    RightPad2 = 50,
    RightPad3 = 51,
    RightPad4 = 52,
    RightJog = 49,
    RightSync = 83,
    RightCue = 82,
    RightPlayPause = 81
}

public class AppManager : Singleton<AppManager>
{
    public Configuration Configuration = new Configuration();
    public Section Settings;

    private const string MidiControllerStringId = "DJControl Compact";

    [Header("DJ Controller Components")]
    // Big Jogs
    [SerializeField] private Image _leftJog;
    [SerializeField] private Image _rightJog;

    // Text Buttons
    [SerializeField] private TMP_Text _leftSync;

    [SerializeField] private TMP_Text _leftCue;
    [SerializeField] private TMP_Text _leftPausePlay;

    // Log lines
    [SerializeField] private TMP_Text _logLines;
    [SerializeField] private string[] _logQueue = new string[3];

    protected override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = 30;
        DOTween.Init();

        LoadOrCreateConfig();
    }

    private void LoadOrCreateConfig()
    {
        if (File.Exists("config.ini"))
        {
            Log.Info("Loading settings from file");
            AppManager.Instance.PushToLog("Loading settings from config.ini");
            Configuration = SharpConfig.Configuration.LoadFromFile("config.ini");
        }
        else
        {
            Log.Info("No settings file found. Creating one...");
            AppManager.Instance.PushToLog("No settings file found. Creating one...");

            Configuration["Settings"]["Remote Host"].StringValue = "127.0.0.1";
            Configuration["Settings"]["Remote Port"].IntValue = 1028;
            Configuration["Settings"]["COM Port"].StringValue = "COM1";
            Configuration["Settings"]["MIDI Input Device ID"].IntValue = 0;
            Configuration["Settings"]["MIDI Output Device ID"].IntValue = 0;
            Configuration["Settings"]["Default Radio Mode"].StringValue = "LSB";
            Configuration["Settings"]["Default VFO"].StringValue = "A";

            for (var i = 0; i < InputDevice.DeviceCount; i++)
            {
                if (InputDevice.GetDeviceCapabilities(i).name == MidiControllerStringId)
                    Configuration["Settings"]["Midi Input Device ID"].IntValue = i;
            }

            for (var i = 0; i < OutputDevice.DeviceCount; i++)
            {
                if (OutputDevice.GetDeviceCapabilities(i).name == MidiControllerStringId)
                    Configuration["Settings"]["Midi Output Device ID"].IntValue = i;
            }

            // Get MIDI input devices
            for (var i = 0; i < InputDevice.DeviceCount; i++)
            {
                Configuration["MIDI Input Devices"][InputDevice.GetDeviceCapabilities(i).name].IntValue = i;
            }

            // Get MIDI output devices
            for (var i = 0; i < OutputDevice.DeviceCount; i++)
            {
                Configuration["MIDI Output Devices"][OutputDevice.GetDeviceCapabilities(i).name].IntValue = i;
            }

            Configuration.SaveToFile("config.ini");
        }

        Settings = Configuration["Settings"];
    }

    public void MoveJog(int control, DJControllerControl controlJog)
    {
        Image jog = null;

        jog = controlJog == DJControllerControl.LeftJog ? _leftJog : _rightJog;
        jog.transform.DOLocalRotate(control < 64 ? new Vector3(0, 0, -1) : new Vector3(0, 0, 1), 0f,
            RotateMode.LocalAxisAdd);
    }

    public void ActivateButton(DJControllerControl control)
    {
        TMP_Text textButton = null;

        switch (control)
        {
            case DJControllerControl.LeftSync:
                textButton = _leftSync;
                break;

            case DJControllerControl.LeftCue:

                Log.Error("asdasd");
                textButton = _leftCue;
                break;
        }

        textButton.color = ColorManager.Instance.Blue;
        textButton.fontStyle = FontStyles.Bold | FontStyles.Italic;
    }

    public enum LogType
    {
        Standard,
        Info,
        Error
    }

    public void PushToLog(string message, LogType type = LogType.Standard)
    {
        string textColor = "";

        switch (type)
        {
            case LogType.Info:
                textColor = "#5FC2D7";
                break;

            case LogType.Error:
                textColor = "#DD6B85";
                break;

            case LogType.Standard:
                textColor = "#7A6763";
                break;
        }


        var output = "<color=#E4D9AC>[" + DateTime.Now.ToString("HH:mm:ss.fff") + "]</color> <color=" +
                     textColor + ">" + message + "</color><br>";

        _logQueue[0] = _logQueue[1];
        _logQueue[1] = _logQueue[2];
        _logQueue[2] = output;

        _logLines.text = _logQueue[0] + _logQueue[1] + _logQueue[2];
    }
}