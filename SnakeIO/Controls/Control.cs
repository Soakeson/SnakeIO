using Microsoft.Xna.Framework.Input;
using System.Runtime.Serialization;

namespace Controls
{
    [DataContract(Name = "Control")]
    public class Control
    {
        [DataMember()]
        public ControlContext cc { get; private set; }
        [DataMember()]
        public Scenes.SceneContext sc { get; private set; }
        [DataMember()]
        public Keys key { get; set; }
        [DataMember()]
        public bool keyPressOnly { get; set; }

        public Control(Scenes.SceneContext sc, ControlContext cc, Keys key, bool keyPressOnly)
        {
            this.sc = sc;
            this.cc = cc;
            this.key = key;
            this.keyPressOnly = keyPressOnly;
        }
    }
}
