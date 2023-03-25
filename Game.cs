namespace GotchiTaMm
{
    internal class Game
    {
        internal GameState state { get; set; }
        internal GameScene scene { get; set; }
        internal Clock? clock { get; set; }

        //Singleton
        internal static readonly Lazy<Game> lazyInstance = new Lazy<Game>(() => new Game());
        private Game()
        {
            pet = new GotchiPet();
            state = new GameStartState();
            scene = new GameScene();
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

        // LIGHTS
        internal bool LightsOn = true;

        internal GotchiPet pet { get; set; }

    }
}
