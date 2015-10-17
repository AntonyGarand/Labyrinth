using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using Android.Util;
using Android.Runtime;
using Microsoft.Xna.Framework.Input.Touch;

namespace Labyrinth
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SpriteFont font;
        private SoundEffect win;
        private SoundEffect fall;

        private Ball player;
        private TiledMap map;

        private Vector2 originalPosition;

        int status;  /* From 0 to 10, according to the level. 0 = menu. */

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");
            win = Content.Load<SoundEffect>("victoire");
            fall = Content.Load<SoundEffect>("trou");

            map = new TiledMap(this.GraphicsDevice);
            player = new Ball(this.GraphicsDevice, 50, 50);

            status = 0;
            createLevel();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (status == 0)
            {

                TouchCollection touchCollection = TouchPanel.GetState();

                if (touchCollection.Count > 0)
                {
                    status++;
                    createLevel();
                }
            }
            player.Update();
            checkPlayerCollision();
            player.Move();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            map.draw(spriteBatch);
            player.Draw(spriteBatch);

            if(status == 0)
            {
                spriteBatch.DrawString(font, "Press anywhere to start", new Vector2(500, 500), Color.Black);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void checkPlayerCollision()
        {
            Vector2 playerPos = player.Coords();
            int playerRadius = player.Radius();
            int playerCoordX = (int)(playerPos.X + playerRadius) / map.TileWidth;
            int playerCoordY = (int)(playerPos.Y + playerRadius) / map.TileHeight;
            int playerPosX = (int)(playerPos.X + playerRadius) % map.TileWidth;
            int playerPosY = (int)(playerPos.Y + playerRadius) % map.TileHeight;
            Log.Debug("Main", "Coords: " + playerPos.ToString());
            Vector2 newSpeed = player.getSpeed();
            /* Checking for wall collision */
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    Vector2 coords = new Vector2(playerCoordX + x, playerCoordY + y);
                    /* Checking if we're within range */
                    if (coords.X >= 0 && coords.X < map.MapWidth && coords.Y >= 0 && coords.Y < map.MapHeight)
                    {
                        /* Checking if it is a wall */
                        if (map.findTile((int)coords.X, (int)coords.Y).Name == "mur")
                        {
                            /* Checking if there is a collision */
                            if (Collision.checkWallCollision(coords, map.TileWidth, new Vector2(playerCoordX, playerCoordY), new Vector2(playerPosX, playerPosY), playerRadius))
                            {
                                /* If the wall was left */
                                if (x == -1 && y == 0)
                                {
                                    Log.Debug("Collision", "Left");
                                    newSpeed.X = newSpeed.X < 0 ? 0 : newSpeed.X;
                                }
                                /* If the wall was right */
                                else if (x == 1 && y == 0)
                                {
                                    Log.Debug("Collision", "Right");
                                    newSpeed.X = newSpeed.X > 0 ? 0 : newSpeed.X;
                                }

                                /* If the wall was top */
                                else if (y == -1 && x == 0)
                                {
                                    Log.Debug("Collision", "Top");
                                    newSpeed.Y = newSpeed.Y < 0 ? 0 : newSpeed.Y;
                                }
                                /* If the wall was bot */
                                else if (y == 1 && x == 0)
                                {
                                    Log.Debug("Collision", "Bot");
                                    newSpeed.Y = newSpeed.Y > 0 ? 0 : newSpeed.Y;
                                }

                            } /* End of collision check */
                        } /* End of wall check */
                    }/* End of range check */
                } /* end of for y */
            }/* end of for x */

            /* Checking for a hole */
            if (map.findTile(playerCoordX, playerCoordY).Name == "trou") {
                if (Collision.checkCircularCollision(new Vector2(playerCoordX + map.TileWidth / 2, playerCoordY + map.TileHeight / 2), 20, new Vector2(playerCoordX, playerCoordY), new Vector2(playerPosX, playerPosY))){
                    fall.Play();
                    player.setPosition(originalPosition);
                    Log.Debug("Main", "Trou!");
                }
            }

            /* Checking for the exit */
            else if (map.findTile(playerCoordX, playerCoordY).Name == "sortie")
            {
                win.Play();
                status++;
                createLevel();
                Log.Debug("Main", "Sortie!");
            }
            player.setSpeed(newSpeed);

        } /* End of checkPlayerCollision */

        private void createLevel()
        {
            switch (status)
            {
                /* Main menu */
                case 0:
                    originalPosition = new Vector2(250, 250);
                    break;
                /* Level 1 */
                case 1:
                    {
                        map = new TiledMap(this.GraphicsDevice);
                        int[,] walls = { { 2, 2 }, { 3, 2 }, { 4, 2 }, { 5, 2 }, { 6, 2 }, { 7, 2 }, { 8, 2 }, { 9, 2 }, { 10, 2 }, { 11, 2 }, { 12, 2 }, { 13, 2 }, { 14, 2 }, { 15, 2 }, { 2, 3 }, { 15, 3 }, { 2, 4 }, { 3, 4 }, { 4, 4 }, { 5, 4 }, { 6, 4 }, { 10, 4 }, { 11, 4 }, { 12, 4 }, { 13, 4 }, { 14, 4 }, { 15, 4 }, { 6, 5 }, { 10, 5 }, { 6, 6 }, { 10, 6 }, { 6, 7 }, { 7, 7 }, { 8, 7 }, { 9, 7 }, { 10, 7 } };
                        int[,] holes = { { 8, 3 }, { 8, 4 }, { 8, 5 } };
                        int[] end = { 14, 3 };
                        int[] spawn = { 3, 3 };
                        map.CreateWalls(walls);
                        map.CreateHoles(holes);
                        map.setEnd(end);
                        originalPosition = new Vector2((spawn[0] * map.TileWidth) + (map.TileWidth / 2) - player.Radius(), (spawn[1] * map.TileHeight) + (map.TileHeight / 2) - player.Radius());
                        player.setPosition(originalPosition);
                        break;
                    }
                case 2:
                    {
                        map = new TiledMap(this.GraphicsDevice);
                        int[,] walls = { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 }, { 1, 1 }, { 9, 1 }, { 1, 2 }, { 5, 2 }, { 6, 2 }, { 7, 2 }, { 9, 2 }, { 1, 3 }, { 5, 3 }, { 7, 3 }, { 9, 3 }, { 1, 4 }, { 2, 4 }, { 3, 4 }, { 4, 4 }, { 5, 4 }, { 7, 4 }, { 9, 4 }, { 7, 5 }, { 9, 5 }, { 6, 6 }, { 7, 6 }, { 9, 6 }, { 10, 6 }, { 5, 7 }, { 6, 7 }, { 10, 7 }, { 5, 8 }, { 10, 8 }, { 4, 9 }, { 5, 9 }, { 10, 9 }, { 4, 10 }, { 10, 10 }, { 4, 11 }, { 9, 11 }, { 10, 11 }, { 4, 12 }, { 8, 12 }, { 9, 12 }, { 4, 13 }, { 5, 13 }, { 6, 13 }, { 7, 13 }, { 8, 13 } };
                        int[,] holes = { { 3, 1 }, { 3, 2 }, { 7, 7 }, { 9, 7 }, { 6, 8 }, { 7, 8 }, { 6, 9 }, { 7, 9 }, { 8, 9 }, { 5, 10 }, { 6, 10 }, { 5, 11 }, { 8, 11 }, { 5, 12 }, { 6, 12 }, { 7, 12 } };
                        int[] end = { 6, 11 };
                        int[] spawn = { 2, 1 };
                        map.CreateWalls(walls);
                        map.CreateHoles(holes);
                        map.setEnd(end);
                        originalPosition = new Vector2(spawn[0] * map.TileWidth, spawn[1] * map.TileHeight);
                        player.setPosition(originalPosition);
                        break;
                    }
                case 3:
                    {
                        map = new TiledMap(this.GraphicsDevice);
                        int[,] walls = { { 3, 3 }, { 4, 3 }, { 5, 3 }, { 6, 3 }, { 7, 3 }, { 8, 3 }, { 9, 3 }, { 10, 3 }, { 11, 3 }, { 12, 3 }, { 13, 3 }, { 3, 4 }, { 16, 4 }, { 3, 5 }, { 4, 5 }, { 5, 5 }, { 6, 5 }, { 7, 5 }, { 12, 5 }, { 13, 5 }, { 14, 5 }, { 7, 6 }, { 11, 6 }, { 7, 7 }, { 8, 7 }, { 9, 7 }, { 10, 7 }, { 11, 7 } };
                        int[,] holes = { { 10, 4 }, { 9, 5 } };
                        int[] end = { 14, 4 };
                        int[] spawn = { 4, 4 };
                        map.CreateWalls(walls);
                        map.CreateHoles(holes);
                        map.setEnd(end);
                        originalPosition = new Vector2(spawn[0] * map.TileWidth, spawn[1] * map.TileHeight);
                        player.setPosition(originalPosition);
                        break;
                    }
            }
            
        }

        private void resetPlayerPosition()
        {
            player.setPosition(originalPosition);
        }
	}
}