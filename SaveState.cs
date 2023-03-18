namespace GotchiTaMm
{
    [Serializable]
    public class SaveState
    {
        public DateTime LastTime { get; set; }

        internal GotchiPet Pet { get; set; }

        internal GameState GameState { get; set; }

        public SaveState()
        {
            Pet = new GotchiPet();
            GameState = new GameStartState();
        }
    }
}
