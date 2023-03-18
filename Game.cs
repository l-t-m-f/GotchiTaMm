using static SDL2.SDL;

namespace GotchiTaMm
{ 

    internal class Game
    {

        internal static Game? Instance;

        internal bool Continue = true;

        // LIGHTS
        internal bool LightsOn = true;

        internal GotchiPet Pet { get; set; }

        internal GameState GameState { get; set; }

        internal Scene Scene = new Scene();

        public static Game Get()
        {
            if (Instance == null)
            {
                Instance = new Game();
            }
            return Instance;
        }

        public static Game Get(SaveState save)
        {
            if (Instance == null)
            {
                Instance = new Game(save);
            }
            return Instance;
        }

        private Game()
        {
            Pet = new GotchiPet();
            GameState = new GameStartState();
        }

        private Game(SaveState save)
        {
            Pet = save.Pet;
            GameState = save.GameState;
        }
    }

    internal abstract class GameState
    {

    }

    internal class GameStartState : GameState
    {

    }

    internal class TimeSetPauseState : GameState
    {

    }

    internal class GotchiPetViewState : GameState
    {

    }

    internal class GotchiPetEvolveState : GameState
    {

    }

    internal class GotchiGameState : GameState
    {

    }


}
