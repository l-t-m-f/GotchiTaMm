using System;

using static SDL2.SDL;

namespace GotchiTaMm;

internal class Game
    {
        internal Game_State State { get; set; }
        internal Game_Scene Scene { get; set; }
        internal Clock? Clock { get; set; }

        //Singleton
        private static readonly Lazy<Game> _Lazy_Instance =
            new(() => new Game());

        private Game()
            {
                this.Pet = new Gotchi_Pet();
                this.State = new Game_State_Start();
                this.Scene = new Game_Scene();
            }

        public static Game Instance => _Lazy_Instance.Value;

        //private Game(SaveState save)
        //{
        //    Pet = save.Pet;
        //    GameState = save.GameState;
        //}


        internal Gotchi_Pet Pet { get; set; }

        internal void Compare_Age_Since_Last_Shutdown()
            {
            }

        internal void Test()
            {
                string? input = Console.ReadLine();
                if (input is null)
                    {
                        SDL_LogError(SDL_LOG_CATEGORY_APPLICATION, "Input was null");
                        return;
                    }
                
                
                
            }
    }