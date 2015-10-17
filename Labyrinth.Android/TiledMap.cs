using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Labyrinth;
using Android.Util;

namespace Labyrinth
{
    class TiledMap
    {
        private const int tileWidth = 50;
        private const int tileHeight = 50;
        private const int mapWidth = 25;
        private const int mapHeight = 14;

        private Dictionary<string, Tile> tiles;
        private Tile[,] map;

        public int TileWidth
        {
            get
            {
                return tileWidth;
            }
        }

        public int TileHeight
        {
            get
            {
                return tileHeight;
            }
        }

        public int MapWidth
        {
            get
            {
                return mapWidth;
            }
        }

        public int MapHeight
        {
            get
            {
                return mapHeight;
            }
        }

        public TiledMap(GraphicsDevice graphicsDevice)
        {
            map = new Tile[mapWidth, mapHeight];
            tiles = new Dictionary<string, Tile>();
            /* Creating the tiles */
            tiles.Add("mur", new Tile(graphicsDevice, "mur", "mur"));
            tiles.Add("sol", new Tile(graphicsDevice, "sol", "sol"));
            tiles.Add("sortie", new Tile(graphicsDevice, "sortie", "sortie"));
            tiles.Add("trou", new Tile(graphicsDevice, "trou", "trou"));

            /* Surrounding walls */
            for (int x = 0; x < MapWidth; x++)
            {
                map[x, 0] = tiles["mur"];
                map[x, MapHeight - 1] = tiles["mur"];
            }
            for (int y = 0; y < MapHeight; y++)
            {
                map[0, y] = tiles["mur"];
                map[MapWidth - 1, y] = tiles["mur"];
            }

            /* Floor */
            for (int x = 1; x < MapWidth - 1; x++)
            {
                for (int y = 1; y < MapHeight - 1; y++)
                {
                    map[x, y] = tiles["sol"];
                }
            }
        }

        public void draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    spriteBatch.Draw(map[x, y].Texture(), new Vector2((float)(x * TileWidth), (float)(y * TileHeight)), Color.White);
                }
            }
        }

        public Tile findTile(int x, int y)
        {
            return map[x, y];
        }

        public bool setTile(Vector2 position, string name)
        {
            if (position.X >= 0 && position.X < MapWidth && position.Y >= 0 && position.Y < MapHeight && tiles.ContainsKey(name))
            {
                map[(int)position.X, (int)position.Y] = tiles[name];
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateHoles(int[,] position)
        {
            for (int x = 0; x < position.GetLength(0); x++)
            {
                map[position[x, 0], position[x, 1]] = tiles["trou"];
            }
        }
        public void CreateWalls(int[,] position)
        {
            for (int x = 0; x < position.GetLength(0); x++)
            {
                map[position[x, 0], position[x, 1]] = tiles["mur"];
            }
        }
        public void setEnd(int[] position)
        {
            map[position[0], position[1]] = tiles["sortie"];
        }
    }
}
