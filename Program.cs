using static SDL2.SDL;

using System.Runtime.InteropServices;
using System.Drawing;

namespace GotchiTaMm
{
    internal class Program
    {

        static IntPtr Window;
        static IntPtr Renderer;
        static bool Continue = true;
        static int[] Keyboard = new int[255];

        // Structure
        static SDL_Rect my_rectangle = new SDL_Rect { x = 50, y = 50, w = 200, h = 60 };

        // Structure & Pointer to it
        static SDL_Rect my_rectangle2 = new SDL_Rect { x = 250, y = 250, w = 20, h = 60 };
        static IntPtr rectangle_ptr = IntPtr.Zero;

        // For input
        public delegate void KeySymbolDelegate(SDL_Keysym keysym);
        public static event KeySymbolDelegate KeyDownEvent;
        public static event KeySymbolDelegate KeyUpEvent;


        static void Main(string[] args)
        {
            Setup();

            int size = Marshal.SizeOf(my_rectangle2);
            rectangle_ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(my_rectangle2, rectangle_ptr, false);

            // Subscribe the respective methods to the events
            // Methods have to respect the delegate definition
            KeyDownEvent += OnKeyDown;
            KeyUpEvent += OnKeyUp;

            while (Continue)
            {
                Input();
                Render();
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

        static void Render()
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

        static void Input()
        {
            while(SDL_PollEvent(out SDL_Event e) == 1)
            {
                switch(e.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        SDL_Quit();
                        Environment.Exit(0);
                        break;
                    case SDL_EventType.SDL_KEYDOWN:
                        KeyDownEvent?.Invoke(e.key.keysym);
                        Keyboard[(int)e.key.keysym.scancode] = 1;
                        break;
                    case SDL_EventType.SDL_KEYUP:
                        KeyUpEvent?.Invoke(e.key.keysym);
                        Keyboard[(int)e.key.keysym.scancode] = 0;
                        break;
                    default:
                        // error...
                        break;

                }
            }
        }

        public static void OnKeyDown(SDL_Keysym keysym)
        {
            Console.WriteLine($"Key down: {keysym.scancode}");

            if(keysym.scancode == SDL_Scancode.SDL_SCANCODE_ESCAPE)
            {
                SDL_Quit();
                Environment.Exit(0);
            }
        }

        public static void OnKeyUp(SDL_Keysym keysym)
        {
            Console.WriteLine($"Key up: {keysym.scancode}");
        }
    }
}