using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Labyrinth
{
    class Tile
    {
        private Texture2D texture;
        private string name;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public Tile(GraphicsDevice graphicsDevice, string name)
        {
            using (var stream = TitleContainer.OpenStream("Content/sol.jpg"))
            {
                texture = Texture2D.FromStream(graphicsDevice, stream);
            }
        }

        public Tile(GraphicsDevice graphicsDevice, Texture2D sprite)
        {
            texture = sprite;
        }

        public Tile(GraphicsDevice graphicsDevice, string sprite, string tileName)
        {
            this.name = tileName;
            using (var stream = TitleContainer.OpenStream("Content/" + sprite + ".jpg"))
            {
                texture = Texture2D.FromStream(graphicsDevice, stream);
            }
        }

        public Texture2D Texture()
        {
            return texture;
        }
    }
}
