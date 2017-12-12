using System;
using System.IO;
using DG.Tweening;
using Sanford.Multimedia.Midi;
using SharpConfig;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DJControllerControl
{
    LeftJog,
    LeftSync,
    LeftCue,
    LeftPlayPause,
    Rec,
    Mode,
    LeftPad1,
    LeftPad2,
    LeftPad3,
    LeftPad4,
    RightJog,
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
            Configuration = SharpConfig.Configuration.LoadFromFile("config.ini");
        }
        else
        {
            Log.Info("No settings file found. Creating one...");

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

    private void Start()
    {
    }
}