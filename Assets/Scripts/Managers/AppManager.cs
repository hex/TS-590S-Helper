using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using DG.Tweening;
using Sanford.Multimedia.Midi;
using SharpConfig;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    LeftPausePlay = 33,
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
    RightPausePlay = 81
}

public class AppManager : Singleton<AppManager>
{
    public Configuration Configuration = new Configuration();
    public Section Settings;

    private const string MidiControllerStringId = "DJControl Compact";

    [Header("DJ Controller Components")] [Header("Jogs")]
    // Big Jogs
    [SerializeField] private Image _leftJog;

    [SerializeField] private Image _rightJog;

    // Small Jogs
    [SerializeField] private Image _leftVolumeJog;

    [SerializeField] private Image _rightVolumeJog;
    [SerializeField] private Image _leftMediumJog;
    [SerializeField] private Image _leftBassJog;
    [SerializeField] private Image _rightMediumJog;
    [SerializeField] private Image _rightBassJog;
    
    // XFader
    [SerializeField] private Slider _xFaderSlider;

    // Text Buttons
    [Header("Buttons Text")] [SerializeField] private TMP_Text _leftSync;

    [SerializeField] private TMP_Text _leftCue;
    [SerializeField] private TMP_Text _leftPausePlay;
    [SerializeField] private TMP_Text _rec;
    [SerializeField] private TMP_Text _leftPad1;
    [SerializeField] private TMP_Text _leftPad2;
    [SerializeField] private TMP_Text _leftPad3;
    [SerializeField] private TMP_Text _leftPad4;
    [SerializeField] private TMP_Text _scratchAtomix;
    [SerializeField] private TMP_Text _rightPad1;
    [SerializeField] private TMP_Text _rightPad2;
    [SerializeField] private TMP_Text _rightPad3;
    [SerializeField] private TMP_Text _rightPad4;
    [SerializeField] private TMP_Text _rightSync;
    [SerializeField] private TMP_Text _rightCue;
    [SerializeField] private TMP_Text _rightPausePlay;

    [Header("Misc stuff")] [SerializeField] private TMP_FontAsset _regularFont;
    [SerializeField] private TMP_FontAsset _boldFont;
    [SerializeField] private TMP_Text _logLines;
    [SerializeField] private string[] _logQueue = new string[3];
    [SerializeField] private UIStatusBoxes _statusBoxes;

    protected override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = 30;
        DOTween.Init();

        LoadOrCreateConfig();
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public void Refresh()
    {
       _statusBoxes.CheckBoxes();
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

    public void MoveXFader(float val)
    {
        _xFaderSlider.value = val;
    }

    public void MoveJog(int control, DJControllerControl controlJog, bool isRelative = false)
    {
        Image jog = GetJogImage(controlJog);

        if (isRelative)
        {
            jog.transform.DOLocalRotate(control < 64 ? new Vector3(0, 0, -1) : new Vector3(0, 0, 1), 0f,
                RotateMode.LocalAxisAdd);
        }
        else
        {
            jog.transform.DOLocalRotate(new Vector3(0, 0, -control), 0f, RotateMode.FastBeyond360);
        }
    }

    Image GetJogImage(DJControllerControl control)
    {
        switch (control)
        {
            case DJControllerControl.LeftJog:
                return _leftJog;
            case DJControllerControl.RightJog:
                return _rightJog;
            case DJControllerControl.LeftVolume:
                return _leftVolumeJog;
            case DJControllerControl.RightVolume:
                return _rightVolumeJog;
            case DJControllerControl.LeftMedium:
                return _leftMediumJog;
            case DJControllerControl.LeftBass:
                return _leftBassJog;
            case DJControllerControl.RightMedium:
                return _rightMediumJog;
            case DJControllerControl.RightBass:
                return _rightBassJog;
            default:
                return null;
        }
    }

    TMP_Text GetButtonText(DJControllerControl control)
    {
        switch (control)
        {
            case DJControllerControl.LeftSync:
                return _leftSync;
            case DJControllerControl.LeftCue:
                return _leftCue;
            case DJControllerControl.LeftPausePlay:
                return _leftPausePlay;
            case DJControllerControl.Rec:
                return _rec;
            case DJControllerControl.LeftPad1:
                return _leftPad1;
            case DJControllerControl.LeftPad2:
                return _leftPad2;
            case DJControllerControl.LeftPad3:
                return _leftPad3;
            case DJControllerControl.LeftPad4:
                return _leftPad4;
            case DJControllerControl.ScratchAtomix:
                return _scratchAtomix;
            case DJControllerControl.RightPad1:
                return _rightPad1;
            case DJControllerControl.RightPad2:
                return _rightPad2;
            case DJControllerControl.RightPad3:
                return _rightPad3;
            case DJControllerControl.RightPad4:
                return _rightPad4;
            case DJControllerControl.RightSync:
                return _rightSync;
            case DJControllerControl.RightCue:
                return _rightCue;
            case DJControllerControl.RightPausePlay:
                return _rightPausePlay;
            default:
                return null;
        }
    }

    public void ChangeButtonState(DJControllerControl control, bool isActive)
    {
        TMP_Text textButton = GetButtonText(control);

        if (textButton == null)
            return;

        if (isActive)
        {
            textButton.color = ColorManager.Instance.Blue;
            textButton.font = _boldFont;
            
            if (control == DJControllerControl.Rec)
            {
                textButton.text = "Off";
                textButton.color = ColorManager.Instance.Red;
            }
        }
        else
        {
            textButton.color = ColorManager.Instance.MidBrown;
            textButton.font = _regularFont;
            
            if (control == DJControllerControl.Rec)
            {
                textButton.text = "On";
                textButton.color = ColorManager.Instance.Green;
            }
        }
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