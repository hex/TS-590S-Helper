public class ATTune : ControlCommand
{
    public override string Name
    {
        get { return "AT Tune"; }
    }

    public override string Command
    {
        get { return "AC111;"; }
    }

    public override void OnExecute()
    {
        SerialManager.Instance.Write(Command);
    }
}