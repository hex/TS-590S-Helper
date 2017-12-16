# TS-590S-Helper
This is simple application that allows you to use the Hercules DJ Control Compact to control the Kenwood TS-590S transceiver. This is inspired by a similar script written by [Patrick Egloff](http://www.egloff.eu).

<img src="https://i.imgur.com/QLxO3Bo.gif" width="100%">

## Download
Check the [Releases](https://github.com/hex/TS-590S-Helper/releases) section for the [latest build](https://github.com/hex/TS-590S-Helper/releases/latest).

## Setup
When the application starts the first time it will create a `config.ini` file. Close the application and edit the contents of the newly created configuration file.
#### Example `config.ini` file:
```
[Settings]
Enable Remote=False
Remote Host=127.0.0.1
Remote Port=1028
COM Port=COM1
MIDI Input Device ID=0
MIDI Output Device ID=1
Default Radio Mode=LSB
Default VFO=A

[MIDI Input Devices]
DJControl Compact=0

[MIDI Output Devices]
Microsoft GS Wavetable Synth=0
DJControl Compact=1
```

| Setting | Values | Description |
|---------|:------:|-------------|
|Enable Remote|True/False | Disables or enables remote host verification. Defaults to **True**. Set this to **False** if not using a remote computer.|
|Remote Host | 127.0.0.1|IP address of remote computer. *Not needed if not using a remote computer.*|
|Remote Port | 1024| Remote port. *Not needed if not using a remote computer.*|
|COM Port | COM1 | Transciever communication port|
|MIDI Input Device | 0 | Device ID of MIDI Input, check the list [MIDI Input Devices] in config.ini for available input devices|
|MIDI Output Device | 0 | Device ID of MIDI Input, check the list [MIDI Output Devices] in config.ini for available output devices|
|Default Radio Mode | LSB | Default mode for startup. Available options: **CW**, **LSB**, **USB**, **FSK** | 
|Default VFO | A | Default VFO for startup. Available options: **A**, **B** | 

## TODO
* Add configurable actions for controller
* Remote setup tutorial





