using static GotchiTaMm.Program;
using static SDL2.SDL;

namespace GotchiTaMm
{

    internal class Game
    {

        internal static Game? Instance;

        internal bool Continue = true;

        // LIGHTS
        internal bool LightsOn = true;

        // INPUT
        internal string current_input = "";

        // TIME-KEEPING
        internal byte inputed_minutes = 0;
        internal byte inputed_hours = 0;

        internal GotchiPet Pet { get; set; }

        internal GameState GameState { get; set; }

        public static Game Get()
        {
            if (Instance == null)
            {
                Instance = new Game();
            }
            return Instance;
        }

        public static Game Get(SaveState save)
        {
            if (Instance == null)
            {
                Instance = new Game(save);
            }
            return Instance;
        }

        private Game()
        {
            Pet = new GotchiPet();
            GameState = new GameStartState();
        }

        private Game(SaveState save)
        {
            Pet = save.Pet;
            GameState = save.GameState;
        }

        internal void Draw()
        {
            if (GameState is GameStartState)
            {
                if (UI != null)
                {
                    Blit(UI.TextImages.GetValueOrDefault(1), 260, 120);

                    if (UI.TextVars.Count > 0)
                    {
                        Blit(UI.TextVars.GetValueOrDefault(TextVarNameType.TimeStart), 275, 144);
                    }
                }
                SDL_Rect circle = new SDL_Rect {
                    x = 150, y = 150, w = 50, h = 70,
                };

                SDL_SetRenderDrawColor(Renderer, 255, 255, 0, 255);
                FillEllipsoid(circle);

            }
            else if (GameState is TimeSetPauseState)
            {
            }
            else if (GameState is GotchiPetViewState)
            {
            }
            else if (GameState is GotchiPetEvolveState)
            {
            }
            else if (GameState is GotchiGameState)
            {
            }
        }

        public void OnKeyDown(SDL_Keysym keysym)
        {
            Console.WriteLine($"Key down: {keysym.scancode}");
            Keyboard[(int)keysym.scancode] = 1;

            if (keysym.scancode == SDL_Scancode.SDL_SCANCODE_ESCAPE)
            {
                QuitGame(0);
            }
            else
            {
                if (GameState is GameStartState)
                {

                    if (keysym.scancode == SDL_Scancode.SDL_SCANCODE_BACKSPACE)
                    {
                        if (current_input.Length == 0) return;

                        if (current_input.Length == 4)
                        {
                            current_input = current_input.DropLastChar();
                        }
                        current_input = current_input.DropLastChar();

                        UI?.SetOrUpdateTextVar(TextVarNameType.TimeStart, current_input, FontNameType.RainyHearts, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });
                    }

                    if (current_input.Length > 4) return;

                    if (char.IsAsciiDigit((char)keysym.sym))
                    {
                        if (current_input.Length == 2)
                        {
                            current_input += ':';
                        }

                        current_input += (char)keysym.sym;

                        UI?.SetOrUpdateTextVar(TextVarNameType.TimeStart, current_input, FontNameType.RainyHearts, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });
                    }

                    Console.WriteLine(current_input);
                }
                else if (GameState is TimeSetPauseState)
                {
                }
                else if (GameState is GotchiPetViewState)
                {
                }
                else if (GameState is GotchiPetEvolveState)
                {
                }
                else if (GameState is GotchiGameState)
                {
                }
            }

        }

        public void OnKeyUp(SDL_Keysym keysym)
        {
            //Console.WriteLine($"Key up: {keysym.scancode}");
            Keyboard[(int)keysym.scancode] = 0;
        }

        public void OnMouseDown(SDL_MouseButtonEvent mouseButtonEvent)
        {
            Console.WriteLine($"Mouse click: {mouseButtonEvent.button} at {mouseButtonEvent.x}, {mouseButtonEvent.y}");
            if (mouseButtonEvent.button <= 3)
            {
                Mouse.Buttons[mouseButtonEvent.button] = 1;
            }

        }

        public void OnMouseUp(SDL_MouseButtonEvent mouseButtonEvent)
        {
            //Console.WriteLine($"Mouse released: {mouseButtonEvent.button} at {mouseButtonEvent.x}, {mouseButtonEvent.y}");
            if (mouseButtonEvent.button <= 3)
            {
                Mouse.Buttons[mouseButtonEvent.button] = 0;
            }
        }
        public void OnMouseMove(SDL_MouseMotionEvent mouseMotionEvent)
        {
            //Console.WriteLine($"Mouse moving at {mouseMotionEvent.x}, {mouseMotionEvent.y}");
            Mouse.Position.x = mouseMotionEvent.x;
            Mouse.Position.y = mouseMotionEvent.y;
        }
    }
}
