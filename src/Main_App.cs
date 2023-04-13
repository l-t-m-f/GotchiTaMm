using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace GotchiTaMm;

public delegate void VoidDelegate();

internal class Main_App
    {
        private const string _APP_TITLE = "GotchiTaMm!!!";
        internal const int WINDOW_W = 480;
        internal const int WINDOW_H = 320;
        private static IntPtr Window;
        internal static IntPtr Renderer;
        
        // OTHER THREADS
        private static Thread? Clock;
        private static Thread? Animate;

        private static bool Continue = true;

        static void Main(string[] args)
            {

                Setup();

                Clock = new Thread(() => ClockThread());
                Animate = new Thread(() => AnimateThread());

                Task<Save_State> load_data = Subsystem_Serialization.Instance.LoadGame();
                Subsystem_Serialization.Instance.SavedGame = load_data.Result;

                Console.WriteLine(
                    Subsystem_Serialization.Instance.SavedGame.LastTime ==
                    DateTime.MinValue
                        ? "This is the first time the program has been run."
                        : $"The program was last shutdown at: {Subsystem_Serialization.Instance.SavedGame.LastTime}");

                Clock.Start();
                Animate.Start();

                while (Continue)
                    {
                        Input();
                        Render();
                        Logic();
                    }

                QuitGame(0);
            }

        private static void Setup()
            {
                const uint SDL_FLAGS = SDL_INIT_VIDEO;
                const SDL_WindowFlags WINDOW_FLAGS = SDL_WindowFlags.SDL_WINDOW_SHOWN;
                const SDL_RendererFlags RENDERER_FLAGS =
                    SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                    SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
                const IMG_InitFlags IMG_FLAGS = IMG_InitFlags.IMG_INIT_PNG;
                
                if (SDL_Init(SDL_FLAGS) < 0)
                    {
                        Console.WriteLine(
                            $"There was an issue starting SDL:\n{SDL_GetError()}!");
                    }

                Window = SDL_CreateWindow(_APP_TITLE,
                    SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
                    WINDOW_W, WINDOW_H, WINDOW_FLAGS);

                if (Window == IntPtr.Zero)
                    {
                        Console.WriteLine(
                            $"There was an issue creating the window:\n{SDL_GetError()}");
                    }

                Renderer = SDL_CreateRenderer(Window, -1, RENDERER_FLAGS);

                if (Renderer == IntPtr.Zero)
                    {
                        Console.WriteLine(
                            $"There was an issue creating the renderer:\n{SDL_GetError()}");
                    }

                if (TTF_Init() < 0)
                    {
                        Console.WriteLine(
                            $"There was an issue starting SDL_ttf:\n{SDL_GetError()}!");
                    }

                if (IMG_Init(IMG_FLAGS) != (int)IMG_FLAGS)
                    {
                        Console.WriteLine(
                            $"There was an issue starting SDL_image:\n{SDL_GetError()}!");
                    }
                
                Subsystem_Imaging.instance.Make_Atlas();
                
            }
        
        
        private static void Render()
            {
                SDL_SetRenderDrawColor(Renderer, 155, 155, 155, 255);
                SDL_RenderClear(Renderer);

                Game.Instance.scene.Draw();
                Subsystem_UI.Instance.Draw();

                SDL_RenderPresent(Renderer);
            }
        
        private static void Input()
            {
                while (SDL_PollEvent(out SDL_Event e) == 1)
                    {
                        switch (e.type)
                            {
                                case SDL_EventType.SDL_QUIT:
                                    QuitGame(0);
                                    break;
                                case SDL_EventType.SDL_MOUSEMOTION:
                                    Subsystem_Input.Instance.OnMouseMove(e.motion);
                                    break;
                                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                                    Subsystem_Input.Instance.OnMouseDown(e.button);
                                    break;
                                case SDL_EventType.SDL_MOUSEBUTTONUP:
                                    Subsystem_Input.Instance.OnMouseUp(e.button);
                                    break;
                                case SDL_EventType.SDL_KEYDOWN:
                                    Subsystem_Input.Instance.OnKeyDown(e.key.keysym);
                                    break;
                                case SDL_EventType.SDL_KEYUP:
                                    Subsystem_Input.Instance.OnKeyUp(e.key.keysym);
                                    break;
                                default:
                                    // error...
                                    break;
                            }
                    }
            }


        private static void Logic()
            {
                if (Subsystem_Input.Instance.mouse.buttons[1] != 1)
                    {
                        return;
                    }

                foreach(Button b in Subsystem_UI.Instance.buttonsDictio.Values)
                    {
                        if (b.TestMouseOverlap() != true)
                            {
                                continue;
                            }

                        b.Activate();
                        Subsystem_Input.Instance.mouse.buttons[1] = 0;
                        break;

                    }
            }

        internal static void QuitGame(sbyte program_code)
            {
                // Release unsafe pointer
                //Marshal.FreeHGlobal(rectangle_ptr);

                Subsystem_Serialization.Instance.SaveGame(DateTime.Now);
                TTF_Quit();
                SDL_Quit();

                Console.WriteLine("Program exited successfully!");
                Environment.Exit(program_code);
            }

        private static void ClockThread()
            {
                while (true)
                    {
                        Thread.Sleep(1000);

                        Console.WriteLine("A second has passed.");

                        if (Game.Instance.clock is not null)
                            {
                                Game.Instance.clock.GameSecond();
                                Console.WriteLine($"Clock incremented!");
                            }
                    }
            }

        private static void AnimateThread()
            {
                while(true)
                    {
                        Thread.Sleep((1 / 60) * 100);
                        Game.Instance.pet.Animate();
                    }
            }

    }
