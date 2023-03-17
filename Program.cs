using static SDL2.SDL;

using System.Runtime.InteropServices;

namespace GotchiTaMm
{
    internal class Program
    {

        static IntPtr Window;
        static IntPtr Renderer;
        static bool Continue = true;

        static void Main(string[] args)
        {
            Setup();

            // Structure
            SDL_Rect my_rectangle = new SDL_Rect { x = 50, y = 50, w = 200, h = 60 };

            // Structure & Pointer to it
            SDL_Rect my_rectangle2 = new SDL_Rect { x = 250, y = 250, w = 20, h = 60 };
            int size = Marshal.SizeOf(my_rectangle2);
            IntPtr rectangle_ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(my_rectangle2, rectangle_ptr, false);

            while (Continue)
            {
                SDL_SetRenderDrawColor(Renderer, 155, 155, 155, 255);
                SDL_RenderClear(Renderer);

                SDL_SetRenderDrawColor(Renderer, 125, 205, 235, 255);
                SDL_RenderDrawLine(Renderer, 5, 5, 300, 310);


                SDL_SetRenderDrawColor(Renderer, 255, 0, 0, 255);
                // Draw with structure ref
                SDL_RenderFillRect(Renderer, ref my_rectangle);

                SDL_SetRenderDrawColor(Renderer, 0, 255, 0, 255);
                // Draw with pointer to structure
                SDL_RenderFillRect(Renderer, rectangle_ptr);

                SDL_RenderPresent(Renderer);
            }

            Marshal.FreeHGlobal(rectangle_ptr);

            Console.WriteLine("Program exited successfully!");
        }

        static void Setup()
        {
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine(
                    $"There was an issue starting SDL:\n{SDL_GetError()}!");
            }

            Window = SDL_CreateWindow("GotchiTaMm!!!",
                SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
                640, 480, SDL_WindowFlags.SDL_WINDOW_SHOWN);

            if (Window == IntPtr.Zero)
            {
                Console.WriteLine(
                    $"There was an issue creating the window:\n{SDL_GetError()}");
            }

            Renderer = SDL_CreateRenderer(Window, -1,
                SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            if (Renderer == IntPtr.Zero)
            {
                Console.WriteLine(
                    $"There was an issue creating the renderer:\n{SDL_GetError()}");
            }
        }
    }
}