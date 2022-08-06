using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexileEngine.engine
{
    public class Transform
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }


        public Transform()
        {
            Init(new Vector3(), new Vector3());
        }

        public Transform(Vector3 position, Vector3 scale)
        {
            Init(position, scale);
        }

        private void Init(Vector3 position, Vector3 scale)
        {
            Position = position;
            Scale = scale;

        }
    }
}
