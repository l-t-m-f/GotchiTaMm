using System;

namespace GotchiTaMm;

internal class Game
    {
        internal Game_State state { get; set; }
        internal Game_Scene scene { get; set; }
        internal Clock? clock { get; set; }

        //Singleton
        internal static readonly Lazy<Game> lazyInstance = new Lazy<Game>(() => new Game());
        private Game()
            {
                this.pet = new Gotchi_Pet();
                this.state = new GameStartState();
                this.scene = new Game_Scene();
            }

        public static Game Instance {
                get {
                        return lazyInstance.Value;
                    }
            }
                
        //private Game(SaveState save)
        //{
        //    Pet = save.Pet;
        //    GameState = save.GameState;
        //}


        internal Gotchi_Pet pet { get; set; }

        internal void CompareAgeSinceLastShutdown()
            {

            }


    }