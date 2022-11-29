using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroductieProject.Code.GameStates
{
    class LevelMenuState : GameState
    {
        public LevelMenuState()
        {
            // load the level menu  screen
            SpriteGameObject lvlMenuScreen = new SpriteGameObject("Sprites/Gen_Office_Background", OfficeBaby.Depth_Background);
            gameObjects.AddChild(lvlMenuScreen);
        }
    }
}
