using FlexileEngine.engine.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexileEngine.engine.Renderer
{
    public class Renderer
    {
        private const int MAX_BATCH_SIZE = 1000;
        private ArrayList batches;

        public Renderer()
        {
            this.batches = new ArrayList();
        }

        public void Add(GameObject go)
        {
            Sprite2D? spr = go.GetComponent<Sprite2D>();
            if (spr != null)
            {
                add(spr);
            }
        }

        private void add(Sprite2D sprite)
        {
            bool added = false;
            foreach(RenderBatch batch in batches)
            {
                if (batch.hasRoom)
                {
                    batch.AddSprite(sprite);
                    added = true;
                    break;
                }
            }

            if (!added)
            {
                RenderBatch newBatch = new RenderBatch(MAX_BATCH_SIZE);
                newBatch.Start();
                batches.Add(newBatch);
                newBatch.AddSprite(sprite);
            }
        }

        public void Render()
        {
            foreach(RenderBatch batch in batches)
            {
                batch.Render();
            }
        }
    }
}
