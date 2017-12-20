/// <summary>
/// Computer Control Commands
/// A computer control command is composed of
/// a 2 letter alphabetical command name, a set of
/// parameters, and the terminator that signals the end of
/// the command.
/// 
/// Example: Command to set VFO A to 7 MHz
/// “FA00007000000;”
/// 
/// More details on: http://www.kenwood.com/i/products/info/amateur/pdf/ts_590_pc_command_e.pdf
/// </summary>
public abstract class ControlCommand
{
    /// <summary>
    /// Human friendly name for the Control Command
    /// </summary>
    public abstract string Name { get; }
    /// <summary>
    /// The Control Command - http://www.kenwood.com/i/products/info/amateur/pdf/ts_590_pc_command_e.pdf
    /// </summary>
    public abstract string Command { get; }

    public virtual void OnExecute()
    {
        
    }
    
}
