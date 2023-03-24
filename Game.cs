using static GotchiTaMm.Program;
using static SDL2.SDL;

namespace GotchiTaMm
{

    internal class Game
    {
        internal GameState gameState { get; set; }

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

        internal class Clock
        {
            internal TimeSpan currentTime;
            internal TimeSpan elapsedTime;

            internal Clock(int h, int m, int s) {
                currentTime= new TimeSpan(h, m, s);
                elapsedTime = new TimeSpan(0, 0, 0);            
            }

            // Method to increment the elapsed time
            internal void IncrementTime(int hours, int minutes, int seconds)
            {
                elapsedTime += new TimeSpan(hours, minutes, seconds);
            }

            // Get the elapsed time in hours
            internal int GetElapsedHours()
            {
                return (int)elapsedTime.TotalHours;
            }

            // Get the elapsed time in minutes
            internal int GetElapsedMinutes()
            {
                return (int)elapsedTime.TotalMinutes;
            }

            // Get the elapsed time in seconds
            internal int GetElapsedSeconds()
            {
                return (int)elapsedTime.TotalSeconds;
            }

            // Get the elapsed time as a formatted string
            internal string GetElapsedTime()
            {
                return $"{elapsedTime.Hours:D2}:{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}";
            }

        }

        // TIME-KEEPING
        internal byte inputed_minutes = 0;
        internal byte inputed_hours = 0;

        internal GotchiPet pet { get; set; }

        internal void Draw()
        {
            if (gameState is GameStartState)
            {
                    Blit(UserInterface.Instance.TextImages.GetValueOrDefault(1), 260, 120);

                    if (UserInterface.Instance.TextVars.Count > 0)
                    {
                        Blit(UserInterface.Instance.TextVars.GetValueOrDefault(TextVarNameType.TimeStart), 275, 144);
                    }
                
                SDL_Rect circle = new SDL_Rect {
                    x = 150, y = 150, w = 50, h = 70,
                };

                SDL_SetRenderDrawColor(Renderer, 255, 255, 0, 255);
                FillEllipsoid(circle);

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
