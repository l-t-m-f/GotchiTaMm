#define DEBUG

using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace GotchiTaMm;

public delegate void VoidDelegate();

internal class Main_App
    {
        private const string _APP_TITLE = "GotchiTaMm!!!";
        internal const int WINDOW_W = 128;
        internal const int WINDOW_H = 92;
        internal const float SCREEN_RATIO = 3.0f;
        private static IntPtr Window;
        internal static IntPtr Renderer;
        
        // OTHER THREADS
        private static Thread? Clock;
        private static Thread? Animate;
        private static Thread? PetWalk;

        private static bool Continue = true;

        static void Main(string[] args)
            {

                Setup();

                Clock = new Thread(Clock_Thread);
                Animate = new Thread(Animate_Thread);
                PetWalk = new Thread(Walk_Thread);

                Task<Save_State> load_data = Subsystem_Serialization.Instance.Load_Game();
                Subsystem_Serialization.Instance.SavedGame = load_data.Result;

                Console.WriteLine(
                    Subsystem_Serialization.Instance.SavedGame.Last_Time ==
                    DateTime.MinValue
                        ? "This is the first time the program has been run."
                        : $"The program was last shutdown at: {Subsystem_Serialization.Instance.SavedGame.Last_Time}");

                Clock.Start();
                Animate.Start();
                PetWalk.Start();

                while (Continue)
                    {
                        Input();
                        Render();
                        Logic();
                    }

                Quit_Game(0);
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
                    (int)(WINDOW_W * SCREEN_RATIO), (int)(WINDOW_H * 
                    SCREEN_RATIO), WINDOW_FLAGS);

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

                SDL_ShowCursor(0);
                
                Subsystem_Imaging.Instance.Make_Atlas();
                Subsystem_UI.Instance.Init();
                Subsystem_Imaging.Instance.Font_Atlas.Make_Sheets_For
                (Font_Name_Type.RAINY_HEARTS);
                Subsystem_Imaging.Instance.Animation_Loader.Make_Animations();
                

            }
        
        
        private static void Render()
            {
                SDL_SetRenderDrawColor(Renderer, 155, 155, 155, 255);
                SDL_RenderClear(Renderer);
                // SDL_RenderCopy(Renderer, SDL_CreateTextureFromSurface
                // (Renderer, Subsystem_Imaging.Instance
                // .Sprite_Atlas.Master_Surface), 
                // IntPtr.Zero, ref 
                // Subsystem_Imaging.Instance
                // .background_rect);
                
                Game.Instance.Scene.Draw();
                Game.Instance.Pet.Draw();
                Subsystem_UI.Instance.Draw();

                
                Subsystem_Imaging.Instance.Font_Atlas.Draw_With_Sheet(
                    "Hello, world", 4, 100, 100, new SDL_Color { r = 255, 
                        g = 0, b = 0 }, Font_Name_Type.RAINY_HEARTS, 10);

                
                SDL_RenderPresent(Renderer);
            }
        
        private static void Input()
            {
                while (SDL_PollEvent(out SDL_Event e) == 1)
                    {
                        switch (e.type)
                            {
                                case SDL_EventType.SDL_QUIT:
                                    Quit_Game(0);
                                    break;
                                case SDL_EventType.SDL_MOUSEMOTION:
                                    Subsystem_Input.Instance.On_Mouse_Move(e.motion);
                                    break;
                                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                                    Subsystem_Input.Instance.On_Mouse_Down(e.button);
                                    break;
                                case SDL_EventType.SDL_MOUSEBUTTONUP:
                                    Subsystem_Input.Instance.On_Mouse_Up(e.button);
                                    break;
                                case SDL_EventType.SDL_KEYDOWN:
                                    Subsystem_Input.Instance.OnKeyDown(e.key.keysym);
                                    break;
                                case SDL_EventType.SDL_KEYUP:
                                    Subsystem_Input.Instance.On_Key_Up(e.key.keysym);
                                    break;
                                default:
                                    // error...
                                    break;
                            }
                    }
            }


        private static void Logic()
            {
                if (Subsystem_Input.Instance.Mouse.Buttons[1] != 1)
                    {
                        return;
                    }

                foreach (Button b in Subsystem_UI.Instance.Buttons_Dictionary.Values.Where(b => b.TestMouseOverlap()))
                    {
                        b.Activate();
                        Subsystem_Input.Instance.Mouse.Buttons[1] = 0;
                        break;
                    }
            }

        internal static void Quit_Game(sbyte program_code)
            {
                // Release unsafe pointer
                //Marshal.FreeHGlobal(rectangle_ptr);

                Subsystem_Serialization.Instance.Save_Game(DateTime.Now);
                TTF_Quit();
                SDL_Quit();

                Console.WriteLine("Program exited successfully!");
                Environment.Exit(program_code);
            }

        private static void Clock_Thread()
            {
                while (true)
                    {
                        Thread.Sleep(1000);

                        Console.WriteLine("A second has passed.");

                        if (Game.Instance.Clock is not null)
                            {
                                Game.Instance.Clock.GameSecond();
                                Console.WriteLine($"Clock incremented!");
                            }
                    }
            }

        private static void Animate_Thread()
            {
                while(true)
                    {
                        Thread.Sleep((1 / 60) * 100);
                        Game.Instance.Pet.Animate();
                    }
            }

        private static void Walk_Thread()
            {
                while (true)
                    {
                        Game.Instance.Pet.Walk();
                        Game.Instance.Pet.Update_Render_Rect();
                        Thread.Sleep(500);
                    }
            }

    }
