using System;

namespace Controls
{
    [Flags]
    public enum ControlContext 
    {
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        MoveTowards,
        MenuUp,
        MenuDown,
        Confirm,
        MouseClick,
        MouseDown,
        MouseUp,
        MouseMove,
    }
}
