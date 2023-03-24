using static GotchiTaMm.Program;
using static GotchiTaMm.Util;
using static SDL2.SDL;


namespace GotchiTaMm
{

    internal class Game
    {
        internal GameState gameState { get; set; }
        internal Clock? clock { get; set; }

        //Singleton
        internal static readonly Lazy<Game> lazyInstance = new Lazy<Game>(() => new Game());
        private Game()
        {
            pet = new GotchiPet();
            gameState = new GameStartState();
        }

        public static Game Instance {
            get {
                return lazyInstance.Value;
            }
        }

        //

        //private Game(SaveState save)
        //{
        //    Pet = save.Pet;
        //    GameState = save.GameState;
        //}


        // LIGHTS
        internal bool LightsOn = true;                

        internal GotchiPet pet { get; set; }

        internal void Draw()
        {
            if (gameState is GameStartState)
            {
                    Blit(Renderer, UserInterface.Instance.textImagesDictio.GetValueOrDefault(1), 260, 120);

                    if (UserInterface.Instance.textVarsDictio.Count > 0)
                    {
                        Blit(Renderer, UserInterface.Instance.textVarsDictio.GetValueOrDefault(TextVarNameType.TimeStart), 275, 144);
                    }
                
                SDL_Rect circle = new SDL_Rect {
                    x = 150, y = 150, w = 50, h = 70,
                };

                SDL_SetRenderDrawColor(Renderer, 255, 255, 0, 255);
                FillEllipsoid(Renderer, circle);

            }
            else if (gameState is TimeSetPauseState)
            {
            }
            else if (gameState is GotchiPetViewState)
            {
            }
            else if (gameState is GotchiPetEvolveState)
            {
            }
            else if (gameState is GotchiGameState)
            {
            }
        }        
    }
}
