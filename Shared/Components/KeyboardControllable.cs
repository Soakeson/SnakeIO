using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Shared.Components
{
    /// <summary>
    /// This component is responsible for keeping keyboard control data of
    /// entites that are controllable using a keyboard.
    /// </summary>

    public class KeyboardControllable : Shared.Components.Component
    {
        public Dictionary<Shared.Controls.ControlContext, Shared.Controls.ControlDelegate> controls = new Dictionary<Shared.Controls.ControlContext, Shared.Controls.ControlDelegate>();
        public Type type;
        public bool enable;
        public Keys? keyPress;

        public KeyboardControllable(
                bool enable,
                Type type,
                Dictionary<Shared.Controls.ControlContext, Shared.Controls.ControlDelegate> controls 
                )
        {
            this.enable = enable;
            this.type = type;
            this.controls = controls;
            this.keyPress = null;
        }

        // Input will be changing, do this with changed input
        public override void Serialize(ref List<byte> data)
        {
            data.AddRange(BitConverter.GetBytes(enable));
            data.AddRange(BitConverter.GetBytes(type.ToString().Length));
            data.AddRange(Encoding.UTF8.GetBytes(type.ToString()));
        }

        public Keys? ConsumeKeyPress()
        {
            if (keyPress != null)
            {
                Keys value = (Keys)keyPress;
                keyPress = null;
                return value;
            }
            return null;
        }
    }
}
