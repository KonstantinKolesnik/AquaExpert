
namespace SmartNetwork.Hardware
{
    public enum CommandType : byte
    {
        GetControlLinesCount, // no params
        GetControlLineInfo, // [idx]
        GetControlLineMode, // [idx]
        SetControlLineMode, // [idx, mode]
        GetControlLineState, // [idx]
        SetControlLineState // [idx, state]
    }
}
