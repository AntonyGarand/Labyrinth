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
        /// <summary>
        /// Constructor of the game activity
        /// </summary>
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
        /// <summary>
        /// Load the game content
        /// </summary>
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
        /// <summary>
        /// Update the game to its next state. 
        /// </summary>
        /// <param name="gameTime">current GameTime</param>
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
        /// <summary>
        /// Draw the current game status
        /// </summary>
        /// <param name="gameTime">current GameTime</param>
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
        /// <summary>
        /// Checks the collision of the player with holes and walls.
        /// Updates the player speed accordingly
        /// </summary>
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
            
            /* Checking for the exit */
            if (map.findTile(playerCoordX, playerCoordY).Name == "sortie")
            {
                win.Play();
                status++;
                createLevel();
                Log.Debug("Main", "Sortie!");
            }
            player.setSpeed(newSpeed);

            /* Checking for a hole */
            if (map.findTile(playerCoordX, playerCoordY).Name == "trou") {
                if (Collision.checkCircularCollision(new Vector2(playerCoordX + map.TileWidth / 2, playerCoordY + map.TileHeight / 2), 20, new Vector2(playerCoordX, playerCoordY), new Vector2(playerPosX, playerPosY))){
                    fall.Play();
                    resetPlayerPosition();
                    Log.Debug("Main", "Trou!");
                }
            }


        } /* End of checkPlayerCollision */

        /// <summary>
        /// Replace the current map with the next level.
        /// Level is found using the "status" variable.
        /// </summary>
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
                        int[,] walls = { { 6, 1 }, { 14, 1 }, { 15, 1 }, { 16, 1 }, { 17, 1 }, { 18, 1 }, { 19, 1 }, { 20, 1 }, { 1, 2 }, { 6, 2 }, { 14, 2 }, { 15, 2 }, { 16, 2 }, { 1, 3 }, { 2, 3 }, { 3, 3 }, { 6, 3 }, { 9, 3 }, { 10, 3 }, { 11, 3 }, { 14, 3 }, { 15, 3 }, { 16, 3 }, { 22, 3 }, { 1, 4 }, { 2, 4 }, { 3, 4 }, { 6, 4 }, { 9, 4 }, { 14, 4 }, { 15, 4 }, { 16, 4 }, { 19, 4 }, { 20, 4 }, { 21, 4 }, { 22, 4 }, { 1, 5 }, { 2, 5 }, { 3, 5 }, { 6, 5 }, { 9, 5 }, { 14, 5 }, { 15, 5 }, { 16, 5 }, { 19, 5 }, { 1, 6 }, { 2, 6 }, { 3, 6 }, { 6, 6 }, { 9, 6 }, { 12, 6 }, { 13, 6 }, { 14, 6 }, { 15, 6 }, { 16, 6 }, { 19, 6 }, { 1, 7 }, { 2, 7 }, { 6, 7 }, { 9, 7 }, { 12, 7 }, { 19, 7 }, { 22, 7 }, { 23, 7 }, { 6, 8 }, { 9, 8 }, { 12, 8 }, { 19, 8 }, { 22, 8 }, { 23, 8 }, { 4, 9 }, { 5, 9 }, { 6, 9 }, { 9, 9 }, { 12, 9 }, { 15, 9 }, { 16, 9 }, { 17, 9 }, { 18, 9 }, { 19, 9 }, { 3, 10 }, { 4, 10 }, { 5, 10 }, { 6, 10 }, { 9, 10 }, { 12, 10 }, { 15, 10 }, { 16, 10 }, { 17, 10 }, { 18, 10 }, { 19, 10 }, { 9, 11 }, { 15, 11 }, { 16, 11 }, { 17, 11 }, { 18, 11 }, { 19, 11 }, { 20, 11 }, { 21, 11 }, { 9, 12 }, { 15, 12 }, { 16, 12 }, { 17, 12 }, { 18, 12 }, { 19, 12 }, { 20, 12 }, { 21, 12 }, { 22, 12 } };
                        int[,] holes = { { 5, 1 }, { 11, 1 }, { 23, 1 }, { 3, 2 }, { 9, 2 }, { 20, 2 }, { 7, 3 }, { 17, 3 }, { 4, 4 }, { 8, 5 }, { 10, 5 }, { 20, 5 }, { 22, 5 }, { 5, 6 }, { 18, 6 }, { 7, 7 }, { 10, 7 }, { 13, 7 }, { 1, 8 }, { 4, 8 }, { 17, 8 }, { 21, 8 }, { 8, 9 }, { 11, 9 }, { 22, 9 }, { 2, 10 }, { 14, 10 }, { 20, 10 }, { 3, 11 }, { 12, 11 }, { 1, 12 }, { 6, 12 } };
                        int[] spawn = { 1, 1 };
                        int[] end = { 23, 12 };
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
                        int[,] walls = { };
                        int[,] holes = { { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 1 }, { 8, 1 }, { 9, 1 }, { 10, 1 }, { 11, 1 }, { 12, 1 }, { 13, 1 }, { 14, 1 }, { 15, 1 }, { 16, 1 }, { 17, 1 }, { 18, 1 }, { 19, 1 }, { 20, 1 }, { 21, 1 }, { 22, 1 }, { 23, 1 }, { 1, 2 }, { 3, 2 }, { 4, 2 }, { 5, 2 }, { 6, 2 }, { 7, 2 }, { 8, 2 }, { 9, 2 }, { 10, 2 }, { 11, 2 }, { 12, 2 }, { 13, 2 }, { 14, 2 }, { 15, 2 }, { 16, 2 }, { 17, 2 }, { 18, 2 }, { 19, 2 }, { 20, 2 }, { 21, 2 }, { 22, 2 }, { 23, 2 }, { 1, 3 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 3 }, { 6, 3 }, { 7, 3 }, { 8, 3 }, { 9, 3 }, { 10, 3 }, { 11, 3 }, { 12, 3 }, { 13, 3 }, { 14, 3 }, { 15, 3 }, { 16, 3 }, { 17, 3 }, { 18, 3 }, { 19, 3 }, { 20, 3 }, { 21, 3 }, { 22, 3 }, { 23, 3 }, { 1, 4 }, { 2, 4 }, { 3, 4 }, { 4, 4 }, { 5, 4 }, { 6, 4 }, { 7, 4 }, { 8, 4 }, { 9, 4 }, { 10, 4 }, { 11, 4 }, { 12, 4 }, { 13, 4 }, { 14, 4 }, { 15, 4 }, { 16, 4 }, { 17, 4 }, { 18, 4 }, { 19, 4 }, { 20, 4 }, { 21, 4 }, { 22, 4 }, { 23, 4 }, { 1, 5 }, { 2, 5 }, { 3, 5 }, { 4, 5 }, { 5, 5 }, { 6, 5 }, { 7, 5 }, { 8, 5 }, { 9, 5 }, { 10, 5 }, { 11, 5 }, { 12, 5 }, { 13, 5 }, { 14, 5 }, { 15, 5 }, { 16, 5 }, { 17, 5 }, { 18, 5 }, { 19, 5 }, { 20, 5 }, { 21, 5 }, { 22, 5 }, { 23, 5 }, { 1, 6 }, { 2, 6 }, { 3, 6 }, { 4, 6 }, { 5, 6 }, { 6, 6 }, { 7, 6 }, { 8, 6 }, { 9, 6 }, { 10, 6 }, { 11, 6 }, { 12, 6 }, { 13, 6 }, { 14, 6 }, { 15, 6 }, { 16, 6 }, { 17, 6 }, { 18, 6 }, { 19, 6 }, { 20, 6 }, { 21, 6 }, { 22, 6 }, { 23, 6 }, { 1, 7 }, { 2, 7 }, { 3, 7 }, { 4, 7 }, { 5, 7 }, { 6, 7 }, { 7, 7 }, { 8, 7 }, { 9, 7 }, { 10, 7 }, { 11, 7 }, { 12, 7 }, { 13, 7 }, { 14, 7 }, { 15, 7 }, { 16, 7 }, { 17, 7 }, { 18, 7 }, { 19, 7 }, { 20, 7 }, { 21, 7 }, { 22, 7 }, { 23, 7 }, { 1, 8 }, { 2, 8 }, { 3, 8 }, { 4, 8 }, { 5, 8 }, { 6, 8 }, { 7, 8 }, { 8, 8 }, { 9, 8 }, { 10, 8 }, { 11, 8 }, { 12, 8 }, { 13, 8 }, { 14, 8 }, { 15, 8 }, { 16, 8 }, { 17, 8 }, { 18, 8 }, { 19, 8 }, { 20, 8 }, { 21, 8 }, { 22, 8 }, { 23, 8 }, { 1, 9 }, { 2, 9 }, { 3, 9 }, { 4, 9 }, { 5, 9 }, { 6, 9 }, { 7, 9 }, { 8, 9 }, { 9, 9 }, { 10, 9 }, { 11, 9 }, { 12, 9 }, { 13, 9 }, { 14, 9 }, { 15, 9 }, { 16, 9 }, { 17, 9 }, { 18, 9 }, { 19, 9 }, { 20, 9 }, { 21, 9 }, { 22, 9 }, { 23, 9 }, { 1, 10 }, { 2, 10 }, { 3, 10 }, { 4, 10 }, { 5, 10 }, { 6, 10 }, { 7, 10 }, { 8, 10 }, { 9, 10 }, { 10, 10 }, { 11, 10 }, { 12, 10 }, { 13, 10 }, { 14, 10 }, { 15, 10 }, { 16, 10 }, { 17, 10 }, { 18, 10 }, { 19, 10 }, { 20, 10 }, { 21, 10 }, { 22, 10 }, { 23, 10 }, { 1, 11 }, { 2, 11 }, { 3, 11 }, { 4, 11 }, { 5, 11 }, { 6, 11 }, { 7, 11 }, { 8, 11 }, { 9, 11 }, { 10, 11 }, { 11, 11 }, { 12, 11 }, { 13, 11 }, { 14, 11 }, { 15, 11 }, { 16, 11 }, { 17, 11 }, { 18, 11 }, { 19, 11 }, { 20, 11 }, { 21, 11 }, { 23, 11 }, { 1, 12 }, { 2, 12 }, { 3, 12 }, { 4, 12 }, { 5, 12 }, { 6, 12 }, { 7, 12 }, { 8, 12 }, { 9, 12 }, { 10, 12 }, { 11, 12 }, { 12, 12 }, { 13, 12 }, { 14, 12 }, { 15, 12 }, { 16, 12 }, { 17, 12 }, { 18, 12 }, { 19, 12 }, { 20, 12 }, { 21, 12 }, { 22, 12 }, { 23, 12 } };
                        int[] spawn = { 2, 2 };
                        int[] end = { 22, 11 };
                        map.CreateWalls(walls);
                        map.CreateHoles(holes);
                        map.setEnd(end);
                        originalPosition = new Vector2(spawn[0] * map.TileWidth, spawn[1] * map.TileHeight);
                        player.setPosition(originalPosition);
                        break;
                    }
                default:
                    status =1;
                    goto case 1;
            }
            
        }

        /// <summary>
        /// Resets the player position and speed to the original one.
        /// </summary>
        private void resetPlayerPosition()
        {
            player.setPosition(originalPosition);
            player.setSpeed(new Vector2(0, 0));
        }
	}
}