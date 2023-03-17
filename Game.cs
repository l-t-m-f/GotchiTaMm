using static SDL2.SDL;

namespace GotchiTaMm
{
    

    internal class Game
    {

        internal bool Continue = true;

        internal GotchiPet Pet { get; set; }

        public Game()
        {
            Pet = new GotchiPet();
        }
    }
}
