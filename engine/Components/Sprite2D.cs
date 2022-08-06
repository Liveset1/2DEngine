using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexileEngine.engine.Components
{
    public class Sprite2D : Component
    {
        public Vector4 Color { get; set; }

        public Sprite2D(Vector4 color)
        {
            Color = color;
        }

        public override void start()
        {

        }

        public override void update(float delta)
        {
            
        }
    }
}
