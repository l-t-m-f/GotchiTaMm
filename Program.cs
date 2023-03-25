using System.ComponentModel;
using System.Reflection;

using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace GotchiTaMm
{
    public delegate void VoidDelegate();
    public delegate void KeysymDelegate(SDL_Keysym keysym);
    public delegate void MouseButtonEventDelegate(SDL_MouseButtonEvent mouseButtonEvent);
    public delegate void MouseMotionEventDelegate(SDL_MouseMotionEvent mouseMotionEvent);

    internal class Program
    {
        internal const int WINDOW_W = 480;
        internal const int WINDOW_H = 320;

        static internal IntPtr Window;
        static internal IntPtr Renderer;        

        public static event KeysymDelegate? KeyDownEvent;
        public static event KeysymDelegate? KeyUpEvent;
        public static event MouseButtonEventDelegate? MouseDownEvent;
        public static event MouseButtonEventDelegate? MouseUpEvent;
        public static event MouseMotionEventDelegate? MouseMotionEvent;


        // OTHER THREADS

        static Thread? Clock;
        static Thread? Animate;


        internal static bool Continue = true;


        static void Main(string[] args)
        {

            Setup();

            Clock = new Thread(() => ClockThread());
            Animate = new Thread(() => AnimateThread());

            Task<SaveState> LoadData = SaveSystem.Instance.LoadGame();
            SaveSystem.Instance.SavedGame = LoadData.Result;

            if (SaveSystem.Instance.SavedGame.LastTime == DateTime.MinValue)
            {
                Console.WriteLine("This is the first time the program has been run.");
            }
            else
            {
                Console.WriteLine($"The program was last shutdown at: {SaveSystem.Instance.SavedGame.LastTime}");
            }

            Clock.Start();
            Animate.Start();

            KeyDownEvent += InputSystem.Instance.OnKeyDown;
            KeyUpEvent += InputSystem.Instance.OnKeyUp;
            MouseDownEvent += InputSystem.Instance.OnMouseDown;
            MouseUpEvent += InputSystem.Instance.OnMouseUp;
            MouseMotionEvent += InputSystem.Instance.OnMouseMove;

            while (Continue)
            {
                Input();
                Render();
                Logic();
            }

            QuitGame(0);
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
                WINDOW_W, WINDOW_H, SDL_WindowFlags.SDL_WINDOW_SHOWN);

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

            if (TTF_Init() < 0)
            {
                Console.WriteLine(
                    $"There was an issue starting SDL_ttf:\n{SDL_GetError()}!");
            }

            IMG_InitFlags img_flags = IMG_InitFlags.IMG_INIT_PNG;
            if (IMG_Init(img_flags) != (int)img_flags)
            {
                Console.WriteLine(
                    $"There was an issue starting SDL_image:\n{SDL_GetError()}!");
            }
        }
        static void Render()
        {
            SDL_SetRenderDrawColor(Renderer, 155, 155, 155, 255);
            SDL_RenderClear(Renderer);

            Game.Instance.scene.Draw();
            UserInterface.Instance.Draw();

            SDL_RenderPresent(Renderer);
        }

        static void Input()
        {
            while (SDL_PollEvent(out SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        QuitGame(0);
                        break;
                    case SDL_EventType.SDL_MOUSEMOTION:
                        MouseMotionEvent?.Invoke(e.motion);
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        MouseDownEvent?.Invoke(e.button);
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                        MouseUpEvent?.Invoke(e.button);
                        break;
                    case SDL_EventType.SDL_KEYDOWN:
                        KeyDownEvent?.Invoke(e.key.keysym);
                        break;
                    case SDL_EventType.SDL_KEYUP:
                        KeyUpEvent?.Invoke(e.key.keysym);
                        break;
                    default:
                        // error...
                        break;
                }
            }
        }

        static void Logic()
        {
            if (InputSystem.Instance.mouse.buttons[1] == 1)
            {
                foreach(Button b in UserInterface.Instance.buttonsDictio.Values)
                {
                    if (b.TestMouseOverlap() == true)
                    {
                        b.Activate();
                        InputSystem.Instance.mouse.buttons[1] = 0;
                        break;
                    }

                }
            }
        }

        public static void ClockThread()
        {
            while (true)
            {
                Thread.Sleep(1000);

                Console.WriteLine("A second has passed.");

                if (Game.Instance.clock is not null)
                {
                    Game.Instance.clock.IncrementTime(0, 0, 1);
                    Console.WriteLine($"Clock incremented: {Game.Instance.clock.GetElapsedTime()}");
                }
            }
        }

        public static void AnimateThread()
        {
            while(true)
            {
                Thread.Sleep((1 / 60) * 100);
                Game.Instance.pet.Animate();
            }
        }

        internal static void QuitGame(sbyte ProgramCode)
        {
            // Release unsafe pointer
            //Marshal.FreeHGlobal(rectangle_ptr);

            SaveSystem.Instance.SaveGame(DateTime.Now);
            TTF_Quit();
            SDL_Quit();

            Console.WriteLine("Program exited successfully!");
            Environment.Exit(ProgramCode);
        }
    }

    public static class Util
    {
        public static string GetDescription<T>(this T e) where T : Enum
        {
            var descriptionAttribute = e.GetType()
                                          .GetMember(e.ToString())
                                          .FirstOrDefault()
                                          ?.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttribute?.Description ?? e.ToString();
        }

        public static void DrawEllipsoid(IntPtr renderer, SDL_Rect circle)
        {
            double pih = Math.PI / 2;
            const int prec = 300; // precision value; value of 1 will draw a diamond, 27 makes pretty smooth circles.
            double theta = 0; // angle that will be increased each loop

            int x = (int)(circle.w * Math.Cos(theta));//start point
            int y = (int)(circle.h * Math.Sin(theta));//start point
            int x1 = x;
            int y1 = y;

            double step = pih / prec; // amount to add to theta each time (degrees)
            for (theta = step ; theta <= pih ; theta += step)//step through only a 90 arc (1 quadrant)
            {
                //get new point location
                x1 = (int)(circle.w * Math.Cos(theta) + 0.5); //new point (+.5 is a quick rounding method)
                y1 = (int)(circle.h * Math.Sin(theta) + 0.5); //new point (+.5 is a quick rounding method)

                //draw line from previous point to new point, ONLY if point incremented
                if ((x != x1) || (y != y1))//only draw if coordinate changed
                {
                    SDL_RenderDrawLine(renderer, circle.x + x, circle.y - y, circle.x + x1, circle.y - y1);//quadrant TR
                    SDL_RenderDrawLine(renderer, circle.x - x, circle.y - y, circle.x - x1, circle.y - y1);//quadrant TL
                    SDL_RenderDrawLine(renderer, circle.x - x, circle.y + y, circle.x - x1, circle.y + y1);//quadrant BL
                    SDL_RenderDrawLine(renderer, circle.x + x, circle.y + y, circle.x + x1, circle.y + y1);//quadrant BR
                }
                //save previous points
                x = x1;//save new previous point
                y = y1;//save new previous point
            }
            //arc did not finish because of rounding, so finish the arc 
            if (x != 0)
            {
                x = 0;
                SDL_RenderDrawLine(renderer, circle.x + x, circle.y - y, circle.x + x1, circle.y - y1);//quadrant TR
                SDL_RenderDrawLine(renderer, circle.x - x, circle.y - y, circle.x - x1, circle.y - y1);//quadrant TL
                SDL_RenderDrawLine(renderer, circle.x - x, circle.y + y, circle.x - x1, circle.y + y1);//quadrant BL
                SDL_RenderDrawLine(renderer, circle.x + x, circle.y + y, circle.x + x1, circle.y + y1);//quadrant BR
            }
        }
        public static void FillEllipsoid(IntPtr renderer, SDL_Rect circle)
        {
            double pih = Math.PI / 2;
            const int prec = 300; // precision value; value of 1 will draw a diamond, 27 makes pretty smooth circles.
            double theta = 0; // angle that will be increased each loop

            int x = (int)(circle.w * Math.Cos(theta));//start point
            int y = (int)(circle.h * Math.Sin(theta));//start point
            int x1 = x;
            int y1 = y;

            SDL_RenderDrawLine(renderer, circle.x - x1, circle.y - y1, circle.x + x1, circle.y - y1);
            SDL_RenderDrawLine(renderer, circle.x - x1, circle.y + y1, circle.x + x1, circle.y + y1);

            double step = pih / prec; // amount to add to theta each time (degrees)
            for (; theta <= pih + step ; theta += step)//step through only a 90 arc (1 quadrant)
            {
                //get new point location
                x1 = (int)(circle.w * Math.Cos(theta) + 0.5); //new point (+.5 is a quick rounding method)
                y1 = (int)(circle.h * Math.Sin(theta) + 0.5); //new point (+.5 is a quick rounding method)

                //draw line from previous point to new point, ONLY if point incremented
                if ((x != x1) || (y != y1))//only draw if coordinate changed
                {
                    SDL_RenderDrawLine(renderer, circle.x - x1, circle.y - y1, circle.x + x1, circle.y - y1);
                    SDL_RenderDrawLine(renderer, circle.x - x1, circle.y + y1, circle.x + x1, circle.y + y1);
                }
                //save previous points
                x = x1;//save new previous point
                y = y1;//save new previous point
            }
            //arc did not finish because of rounding, so finish the arc
            if (x != 0)
            {
                x = 0;
                SDL_RenderDrawLine(renderer, circle.x - x1, circle.y - y1, circle.x + x1, circle.y - y1);
                SDL_RenderDrawLine(renderer, circle.x - x1, circle.y + y1, circle.x + x1, circle.y + y1);
            }
        }


        internal static void Blit(IntPtr renderer, IntPtr texture, int x, int y)
        {
            SDL_Rect destination;
            destination.x = x;
            destination.y = y;
            SDL_QueryTexture(texture, out uint format, out int access, out destination.w, out destination.h);
            SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref destination);
        }

        internal static void BlitRect(IntPtr renderer, IntPtr texture, SDL_Rect rect)
        {
            SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref rect);
        }
    }

    public static class StringExt
    {
        public static string DropLastChar(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length == 1 ? "" : value.Substring(0, value.Length-1);
        }
    }
}