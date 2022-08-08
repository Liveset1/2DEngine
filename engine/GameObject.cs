using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace FlexileEngine.engine
{
    public class GameObject
    {
        public string Name { get; set; }
        private ArrayList Components { get; set; }
        public Transform Transform { get; set; }
        
        public GameObject(string name)
        {
            Init(name, new Transform());
        }

        public GameObject(string name, Transform transform)
        {
            Init(name, transform);
        }

        private void Init(string name, Transform transform)
        {
            Name = name;
            Components = new ArrayList();
            Transform = transform;
        }

        public T? GetComponent<T>() where T : Component
        {
            foreach (Component comp in Components)
            {
                if (typeof(T).IsAssignableFrom(comp.GetType()))
                {
                    return (T)comp;
                }
            }
            return null;
        }

        public void AddComponent<T>(T component) where T : Component
        {
            Components.Add(component);
            component.GameObject = this;
        }

        public void RemoveComponent<T>(T component) where T : Component
        {
            if (Components.Contains(component))
            {
                Components.Remove(component);
            }
        }

        public void Update(float delta)
        {
            foreach (Component c in Components)
            {
                c.Update(delta);
            }
        }

        public void Start()
        {
            foreach (Component c in Components)
            {
                c.Start();
            }
        }
    }
}
