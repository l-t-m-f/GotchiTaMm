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


        // RENDER

        // Structure
        static SDL_Rect my_rectangle = new SDL_Rect { x = 50, y = 50, w = 200, h = 60 };

        // Structure & Pointer to it
        static SDL_Rect my_rectangle2 = new SDL_Rect { x = 250, y = 250, w = 20, h = 60 };
        static IntPtr rectangle_ptr = IntPtr.Zero;

        static SDL_Rect my_circle = new SDL_Rect { x = Program.WINDOW_W / 2, y = Program.WINDOW_H / 2, w = 100, h = 80 };
    }
}
