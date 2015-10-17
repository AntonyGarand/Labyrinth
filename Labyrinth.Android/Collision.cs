using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
{
    static class Collision
    {
        /// <summary>
        /// Checks if the center of the ball is within the range of a circle
        /// </summary>
        /// <param name="circleCenter"> Current position of the center of the circle</param>
        /// <param name="circleRadius">Radius of the circle</param>
        /// <param name="ballCoords">Coordinates of the ball</param>
        /// <param name="ballPosition">Position within the tile of the ball</param>
        /// <returns></returns>
        static public bool checkCircularCollision(Vector2 circleCenter, int circleRadius,Vector2 ballCoords, Vector2 ballPosition)
        {
            int ballX = (int)(ballCoords.X + ballPosition.X);
            int ballY = (int)(ballCoords.Y + ballPosition.Y);
            int dist = (int)findDist(circleCenter, new Vector2(ballX, ballY));
            return dist <= circleRadius;
        }
        /// <summary>
        /// /Cheks the collision between a wall and the ball
        /// </summary>
        /// <param name="wallCoords">Wall coordinates</param>
        /// <param name="spriteSize">Size of the sprites</param>
        /// <param name="ballCoords">Ball coordinates</param>
        /// <param name="ballPosition">Ball position within the coordinates</param>
        /// <param name="ballRadius">Ball radius</param>
        /// <returns>The collision status</returns>
        static public bool checkWallCollision(Vector2 wallCoords, int spriteSize, Vector2 ballCoords, Vector2 ballPosition, int ballRadius)
        {
            //TODO: Fix the collision detection. Currently not working 
            /* Checking left collision */
            if (wallCoords.X + 1 == ballCoords.X)
            {
                if (ballPosition.X - ballRadius <= 0)
                {
                    return true;
                }
            }
            /* Checking right collision */
            else if (wallCoords.X - 1 == ballCoords.X)
            {
                if (ballPosition.X + ballRadius >= spriteSize)
                {
                    return true;
                }
            }
            /* Checking top collision */
            else if (wallCoords.Y + 1 == ballCoords.Y)
            {
                if(ballPosition.Y - ballRadius <= 0)
                {
                    return true;
                }
            }
            /* Checking bot collision */
            else if (wallCoords.Y - 1 == ballCoords.Y)
            {
                if (ballPosition.Y + ballRadius >= spriteSize)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find the distance between two points
        /// </summary>
        /// <param name="from">Starting point</param>
        /// <param name="to">Ending point</param>
        /// <returns>Double distance</returns>
        static private double findDist(Vector2 from, Vector2 to)
        {
            double a2 = Math.Pow(from.X - to.X, 2);
            double b2 = Math.Pow(from.Y - to.Y,2);
            double dist = Math.Sqrt(a2 + b2);
            return dist;
        }
    }
}