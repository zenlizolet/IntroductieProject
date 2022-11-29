using Engine;
using IntroductieProject.Code.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace IntroductieProject
{
    public class OfficeBaby : ExtendedGameWithLevels
    {
        public const float Depth_Background = 0; // for background images
        public const float Depth_UIBackground = 0.9f; // for UI elements with text on top of them
        public const float Depth_UIForeground = 1; // for UI elements in front
        public const float Depth_LevelTiles = 0.5f; // for tiles in the level
        public const float Depth_LevelObjects = 0.6f; // for all game objects except the player
        public const float Depth_LevelPlayer = 0.7f; // for the player

        private SpriteBatch _spriteBatch;

        [STAThread]
        static void Main()
        {
            using (var game = new OfficeBaby())
                game.Run();
        } 
        public OfficeBaby()
        {
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // set a custom world and window size
            worldSize = new Point(1440, 825);
            windowSize = new Point(1024, 586);

            // to let these settings take effect, we need to set the FullScreen property again
            FullScreen = false;

            // load the player's progress from a file

            // add the game states
            GameStateManager.AddGameState(StateName_Title, new TitleMenuState());
            GameStateManager.AddGameState(StateName_LevelMenu, new LevelMenuState());
            GameStateManager.AddGameState(StateName_HelpState, new HelpState());
            GameStateManager.AddGameState(StateName_Playing, new PlayingState());

            // start at the title screen
            GameStateManager.SwitchTo(StateName_Title);

            // play background music
            AssetManager.PlaySong("Sounds/snd_music", true);
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }


    }
}
