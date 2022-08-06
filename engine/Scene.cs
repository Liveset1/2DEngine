using OpenTK.Windowing.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlexileEngine.engine.Renderer;

namespace FlexileEngine.engine
{
    public abstract class Scene
    {
        internal Renderer.Renderer Renderer;
        public Camera Camera { get; set; }
        private bool isRunning = false;
        protected ArrayList gameObjects;

        public Scene()
        {
            gameObjects = new ArrayList();
            Renderer = new Renderer.Renderer();
        }

        public void Start()
        {
            foreach (GameObject go in gameObjects)
            {
                go.Start();
                Renderer.Add(go);
            }
            isRunning = true;
        }

        // On Activating scene
        public virtual void Init()
        {

        }

        public void AddGameObject(GameObject go)
        {
            if(!isRunning)
            {
                gameObjects.Add(go);
            } else
            {
                gameObjects.Add(go);
                go.Start();
                Renderer.Add(go);
            }
        }

        public abstract void Update(FrameEventArgs e, float dt);
    }
}
