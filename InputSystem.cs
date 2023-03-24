using static GotchiTaMm.Program;
using static SDL2.SDL;

namespace GotchiTaMm
{
    internal class InputSystem
    {
        // Inner classes

        internal class Mouse
        {
            internal SDL_Point position = new SDL_Point();
            internal int[] buttons = new int[4];
        }

        internal class Keyboard
        {
            internal int[] state = new int[255];
        }

        //

        internal Mouse mouse;
        internal Keyboard keyboard;
        internal string appIn = "";

        //Singleton

        internal static readonly Lazy<InputSystem> lazyInstance = new Lazy<InputSystem>(() => new InputSystem());
        private InputSystem()
        {
            mouse = new Mouse();
            keyboard = new Keyboard();
        }

        public static InputSystem Instance {
            get {
                return lazyInstance.Value;
            }
        }

        //

        public void OnKeyDown(SDL_Keysym keysym)
        {
            Console.WriteLine($"Key down: {keysym.scancode}");
            keyboard.state[(int)keysym.scancode] = 1;

            if (keysym.scancode == SDL_Scancode.SDL_SCANCODE_ESCAPE)
            {
                QuitGame(0);
            }
            else
            {
                if (Game.Instance.gameState is GameStartState)
                {

                    if (keysym.scancode == SDL_Scancode.SDL_SCANCODE_BACKSPACE)
                    {
                        UserInterface.Instance.RemoveOne();
                    }

                    if (appIn.Length > 4) return;

                    if (char.IsAsciiDigit((char)keysym.sym))
                    {
                        UserInterface.Instance.AddOne(keysym);
                    }

                    Console.WriteLine(appIn);
                }
                else if (Game.Instance.gameState is TimeSetPauseState)
                {
                }
                else if (Game.Instance.gameState is GotchiPetViewState)
                {
                }
                else if (Game.Instance.gameState is GotchiPetEvolveState)
                {
                }
                else if (Game.Instance.gameState is GotchiGameState)
                {
                }
            }

        }

        public void OnKeyUp(SDL_Keysym keysym)
        {
            //Console.WriteLine($"Key up: {keysym.scancode}");
            keyboard.state[(int)keysym.scancode] = 0;
        }

        public void OnMouseDown(SDL_MouseButtonEvent mouseButtonEvent)
        {
            Console.WriteLine($"Mouse click: {mouseButtonEvent.button} at {mouseButtonEvent.x}, {mouseButtonEvent.y}");
            if (mouseButtonEvent.button <= 3)
            {
                Instance.mouse.buttons[mouseButtonEvent.button] = 1;
            }

        }

        public void OnMouseUp(SDL_MouseButtonEvent mouseButtonEvent)
        {
            //Console.WriteLine($"Mouse released: {mouseButtonEvent.button} at {mouseButtonEvent.x}, {mouseButtonEvent.y}");
            if (mouseButtonEvent.button <= 3)
            {
                Instance.mouse.buttons[mouseButtonEvent.button] = 0;
            }
        }
        public void OnMouseMove(SDL_MouseMotionEvent mouseMotionEvent)
        {
            //Console.WriteLine($"Mouse moving at {mouseMotionEvent.x}, {mouseMotionEvent.y}");
            Instance.mouse.position.x = mouseMotionEvent.x;
            Instance.mouse.position.y = mouseMotionEvent.y;
        }
    }
}
