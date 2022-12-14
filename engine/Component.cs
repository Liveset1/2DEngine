using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexileEngine.engine
{
    public abstract class Component
    {
        public GameObject GameObject { get; set; }
        public abstract void Update(float delta);
        public abstract void Start();
    }
}
