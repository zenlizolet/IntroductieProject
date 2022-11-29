using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroductieProject.Code.GameStates
{
    internal class PlayingState : GameState
    {
        public PlayingState()
        {
            // load the level menu  screen
            SpriteGameObject PlayingScreen = new SpriteGameObject("Sprites/Gen_Office_Background", OfficeBaby.Depth_Background);
            PlayingScreen.width = 1440; //OfficeBaby.cs Window size x value
            PlayingScreen.height = 825;
            gameObjects.AddChild(PlayingScreen);
        }
    }
}
