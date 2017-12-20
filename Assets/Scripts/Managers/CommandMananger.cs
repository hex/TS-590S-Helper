using System;
using System.Collections.Generic;
using System.IO;
using SharpConfig;

public class CommandMananger : Singleton<CommandMananger>
{
    public Dictionary<string, ControlCommand> ControlCommands = new Dictionary<string, ControlCommand>();
    public Configuration Configuration = new Configuration();
    
    protected override void Awake()
    {
        base.Awake();

        if (File.Exists("commands.ini"))
        {
            AppManager.Instance.PushToLog("Loading commands from commands.ini");
            Configuration = Configuration.LoadFromFile("commands.ini");
            
            foreach (var control in Enum.GetValues(typeof(DJControllerControl)))
            {
                Configuration["Command Mappings"][control.ToString()].StringValue = new ATTune().Name;
            }
            
            Configuration.SaveToFile("commands.ini");
        }
    }
}