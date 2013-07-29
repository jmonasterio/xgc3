using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace xgc3.GameObjects
{
    public class GameTimer : GameObject
    {
        GameTime m_gameTime;

        public GameTimer()
        {
            m_gameTime = null;
        }

        public void Step( GameTime gameTime)
        {
            this.m_gameTime = gameTime;

            // Calculate FPS -- assume we are called once per step.
            float elapsed = (float)gameTime.ElapsedRealTime.TotalSeconds;
            m_frameCount++;
            m_timeSinceLastUpdate += elapsed;

            if (m_timeSinceLastUpdate > m_updateInterval)
            {
                m_fps = m_frameCount / m_timeSinceLastUpdate; //mean m_fps over updateIntrval
                m_frameCount = 0;
                m_timeSinceLastUpdate = 0.0F;
            }
        }

        // Summary:
        //     The amount of elapsed game time since the last update.
        //
        // Returns:
        //     Elapsed game time since the last update.
        public TimeSpan ElapsedGameTime { get { return m_gameTime.ElapsedGameTime; } }
        //
        // Summary:
        //     The amount of elapsed real time (wall clock) since the last frame.
        //
        // Returns:
        //     Elapsed real time since the last frame.
        public TimeSpan ElapsedRealTime { get { return m_gameTime.ElapsedRealTime; } }
        //
        // Summary:
        //     Gets a value indicating that the game loop is taking longer than its Game.TargetElapsedTime.
        //      In this case, the game loop can be considered to be running too slowly and
        //     should do something to "catch up."
        //
        // Returns:
        //     true if the game loop is taking too long; false otherwise.
        public bool IsRunningSlowly { get { return m_gameTime.IsRunningSlowly; } }
        //
        // Summary:
        //     The amount of game time since the start of the game.
        //
        // Returns:
        //     Game time since the start of the game.
        public TimeSpan TotalGameTime { get { return m_gameTime.TotalGameTime; } }
        //
        // Summary:
        //     The amount of real time (wall clock) since the start of the game.
        //
        // Returns:
        //     Real time since the start of the game.
        public TimeSpan TotalRealTime { get { return m_gameTime.TotalRealTime; } }

        private float m_updateInterval = 1.0f;
        private float m_timeSinceLastUpdate = 0.0f;
        private float m_frameCount = 0.0f;
        private float m_fps = 0.0f;

        public float FPS { get { return m_fps; } }
    }
}
