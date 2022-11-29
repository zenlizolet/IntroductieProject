using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IntroductieProject
{
   class Shirt : SpriteGameObject
    {
        Level level;
        protected float bounce;
        Vector2 startPosition;

        public Shirt(Level level, Vector2 startPosition) : base("Sprites/shirt1", OfficeBaby.Depth_LevelObjects)
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

            // check if the player collects this water drop
            if (Visible && level.Player.CanCollideWithObjects && HasPixelPreciseCollision(level.Player))
            {
                Visible = false;
                ExtendedGame.AssetManager.PlaySoundEffect("Sounds/snd_watercollected");
            }

        }

        public override void Reset()
        {
            localPosition = startPosition;
            Visible = true;
        }
    }
}

