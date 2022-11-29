using Engine;
using System;
using Microsoft.Xna.Framework;


namespace IntroductieProject.Code.LevelObjects
{
    internal class Hearts : SpriteGameObject
    {
        Level level;
        protected float bounce;
        Vector2 startPosition;

        public Hearts(Level level, Vector2 startPosition) : base("Content/Sprites/heart", OfficeBaby.Depth_UIForeground)
        {
            this.level = level;
            this.startPosition = startPosition;

            SetOriginToCenter();

            Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            double t = gameTime.TotalGameTime.TotalSeconds * 3.0f + LocalPosition.X;
            bounce = (float)Math.Sin(t) * 0.2f;
            localPosition.Y += bounce;

        }
    }
}
