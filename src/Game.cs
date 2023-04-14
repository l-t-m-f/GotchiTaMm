using System;

namespace GotchiTaMm;

internal class Game
    {
        internal Game_State State { get; set; }
        internal Game_Scene Scene { get; set; }
        internal Clock? Clock { get; set; }

        //Singleton
        internal static readonly Lazy<Game> Lazy_Instance = new(() => new Game());
        private Game()
            {
                this.Pet = new Gotchi_Pet();
                this.State = new Game_State_Start();
                this.Scene = new Game_Scene();
            }

        public static Game Instance => Lazy_Instance.Value;

        //private Game(SaveState save)
        //{
        //    Pet = save.Pet;
        //    GameState = save.GameState;
        //}


        internal Gotchi_Pet Pet { get; set; }

        internal void Compare_Age_Since_Last_Shutdown()
            {

            }


    }