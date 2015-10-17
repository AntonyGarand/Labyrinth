using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.Content;
using Android.Hardware;
using Microsoft.Devices.Sensors;
using Android.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
{
    class Ball
    {

        static Accelerometer accelerometer;

        private float X;
        private float Y;

        private float speedX;
        private float speedY;

        private float speedCap;
        private float resistance;

        private float accelerationSpeed;

        private Texture2D sprite;


        public Ball(GraphicsDevice graphicsDevice)
        {
            X = 0;
            Y = 0;

            speedX = 0.5f;
            speedY = 0.5f;

            accelerationSpeed = 2.0f;

            speedCap = 10f;
            resistance = 0.3f;


            if (accelerometer == null)
            {
                accelerometer = new Accelerometer();

                accelerometer.Start();

            }
            using (var stream = TitleContainer.OpenStream("Content/balle.png"))
            {
                sprite = Texture2D.FromStream(graphicsDevice, stream);
            }

        }
        public Ball(GraphicsDevice graphicsDevice, int x, int y)
        {
            X = x;
            Y = y;
            accelerationSpeed = 1.0f;
            speedCap = 10f;
            resistance = 0.1f;
            if (accelerometer == null)
            {
                accelerometer = new Accelerometer();

                accelerometer.Start();

            }
            using (var stream = TitleContainer.OpenStream("Content/balle.png"))
            {
                sprite = Texture2D.FromStream(graphicsDevice, stream);
            }
        }
        public void Update()
        {
            UpdateSpeed();
            Log.Debug("Ball", "Position: " + X + " " + Y);
        }

        private void UpdateSpeed()
        {

            float orientationX = accelerometer.CurrentValue.Acceleration.Y;
            float orientationY = accelerometer.CurrentValue.Acceleration.X;

            orientationX = (float)(Math.Round((double)orientationX, 1));
            orientationY = (float)(Math.Round((double)orientationY, 1));


            if (orientationX == 0)
            {
                if (speedX < -resistance || speedX > resistance)
                {
                    speedX += speedX > 0 ? -resistance : resistance;
                }
                else {
                    speedX = 0;
                }
            }
            else
            {
                speedX += orientationX * accelerationSpeed;
            }

            if (orientationY == 0)
            {
                if (speedY < -resistance || speedY > resistance)
                {
                    speedY += speedY > 0 ? -resistance : resistance;
                }
                else {
                    speedY = 0;
                }
            }
            else
            {
                speedY += orientationY * accelerationSpeed;
            }

            speedX = speedX > speedCap ? speedCap : speedX < -speedCap ? -speedCap : speedX;
            speedY = speedY > speedCap ? speedCap : speedY < -speedCap ? -speedCap : speedY;

            X = X % 1280;
            Y = Y % 720;
        }

        public void Move()
        {
            X += speedX;
            Y += speedY;
        }

        public void setSpeed(Vector2 speed)
        {
            speedX = speed.X;
            speedY = speed.Y;
        }

        public Vector2 getSpeed()
        {
            return new Vector2(speedX, speedY);
        }

        public void setPosition(Vector2 position)
        {
            X = position.X;
            Y = position.Y;
            speedX = 0;
            speedY = 0;
        }

        public Vector2 Coords()
        {
            return new Vector2(X, Y);
        }
        public int Radius()
        {
            return sprite.Width / 2;
        }

		public void Draw (SpriteBatch spriteBatch)
		{
            spriteBatch.Draw(sprite, new Vector2(X, Y), Color.White);
		}
    }
}
