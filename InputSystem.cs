using System.Collections.Generic;
using System.ComponentModel;
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
        internal int appInLimit = 5;

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
                if (Game.Instance.state is GameStartState)
                {

                    if (keysym.scancode == SDL_Scancode.SDL_SCANCODE_BACKSPACE)
                    {
                        TrimAppIn(true, TextVarNameType.TimeStart, true);
                    }

                    if (char.IsAsciiDigit((char)keysym.sym))
                    {
                        PushIntoAppIn(keysym, true, TextVarNameType.TimeStart, true);
                    }

                    Console.WriteLine(appIn);
                }
                else if (Game.Instance.state is TimeSetPauseState)
                {
                }
                else if (Game.Instance.state is GotchiPetViewState)
                {
                }
                else if (Game.Instance.state is GotchiPetEvolveState)
                {
                }
                else if (Game.Instance.state is GotchiGameState)
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

        internal void TrimAppIn(bool render = false, TextVarNameType renderTarget = 0, bool isTime = false)
        {
            if (appIn.Length == 0) return;

            if (isTime)
            {
                if (Instance.appIn.Length == 4)
                {
                    appIn = appIn.DropLastChar();
                }
            }

            appIn = appIn.DropLastChar();

            if (!render) return;

            UserInterface.Instance.SetOrUpdateTextVar(renderTarget, appIn, FontNameType.RainyHearts, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });
        }

        internal void PushIntoAppIn(SDL_Keysym keysym, bool render = false, TextVarNameType renderTarget = 0, bool isTime = false)
        {
            if (appIn.Length >= appInLimit) return;

            if (isTime)
            {
                if (Instance.appIn.Length == 2)
                {
                    Instance.appIn += ':';
                }
            }

            appIn += (char)keysym.sym;

            if (!render) return;

            UserInterface.Instance.SetOrUpdateTextVar(renderTarget, appIn, FontNameType.RainyHearts, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });

        }


        public void SelectButtonPressed()
        {
            Console.WriteLine("Select!");

            if (Game.lazyInstance is null) return;

            if (Game.Instance.state is GameStartState)
            {
                UserInterface.Instance.TryInputTime();
            }
            else if (Game.Instance.state is GotchiPetViewState)
            {
                UserInterface.Instance.pictoSelection.SelectNext();
            }

        }
        public void ExecuteButtonPressed()
        {
            Console.WriteLine(value: "Execute!");
        }
        public void CancelButtonPressed()
        {
            Console.WriteLine("Cancel!");
            if (Game.Instance.state is GotchiPetViewState)
            {
                UserInterface.Instance.pictoSelection.ClearSelect();
            }
        }
    }
}
