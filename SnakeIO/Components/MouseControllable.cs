using System.Collections.Generic;
using Controls;

namespace Components
{
    /// <summary>
    /// This component is responsible for keeping mouse control data of
    /// entites that are controllable using a keyboard.
    /// </summary>
    public class MouseControllable : Component
    {
        public Dictionary<ControlContext, ControlDelegatePosition> controls = new Dictionary<ControlContext, ControlDelegatePosition>();
        public MouseControllable(
                Controls.ControlManager cm,
                (Controls.ControlContext, ControlDelegatePosition)[] actions
                )
        {
            foreach ((ControlContext con, ControlDelegatePosition del) in actions)
            {
                controls.Add(con, del);
            }
        }
    }
}
