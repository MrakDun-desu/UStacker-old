
/************************************
SimultaneousDasBehavior.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.ComponentModel;

namespace UStacker.GlobalSettings.Enums
{
    public enum SimultaneousDasBehavior : byte
    {
        [Description("Don't cancel DAS")] DontCancel = 0,

        [Description("Cancel first DAS direction")]
        CancelFirstDirection = 1,

        [Description("Cancel both DAS directions")]
        CancelBothDirections = 2
    }
}
/************************************
end SimultaneousDasBehavior.cs
*************************************/
