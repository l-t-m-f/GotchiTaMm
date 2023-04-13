using static GotchiTaMm.Main_App;
using static SDL2.SDL;

namespace GotchiTaMm;

internal class Subsystem_Input
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

        internal static readonly Lazy<Subsystem_Input> lazyInstance = new Lazy<Subsystem_Input>(() => new Subsystem_Input());
        private Subsystem_Input()
            {
                this.mouse = new Mouse();
                this.keyboard = new Keyboard();
            }

        public static Subsystem_Input Instance {
                get {
                        return lazyInstance.Value;
                    }
            }

        //
        
        public void OnKeyDown(SDL_Keysym keysym)
            {
                Console.WriteLine($"Key down: {keysym.scancode}");
                this.keyboard.state[(int)keysym.scancode] = 1;

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
                                        this.TrimAppIn(true, TextVarNameType.TimeStart, true);
                                    }

                                if (char.IsAsciiDigit((char)keysym.sym))
                                    {
                                        this.PushIntoAppIn(keysym, true, TextVarNameType.TimeStart, true);
                                    }

                                Console.WriteLine(this.appIn);
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
                this.keyboard.state[(int)keysym.scancode] = 0;
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
                if (this.appIn.Length == 0) return;

                if (isTime)
                    {
                        if (Instance.appIn.Length == 4)
                            {
                                this.appIn = this.appIn.DropLastChar();
                            }
                    }

                this.appIn = this.appIn.DropLastChar();

                if (!render) return;

                Subsystem_UI.Instance.SetOrUpdateTextVar(renderTarget, this.appIn, FontNameType.RainyHearts, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });
            }

        internal void PushIntoAppIn(SDL_Keysym keysym, bool render = false, TextVarNameType renderTarget = 0, bool isTime = false)
            {
                if (this.appIn.Length >= this.appInLimit) return;

                if (isTime)
                    {
                        if (Instance.appIn.Length == 2)
                            {
                                Instance.appIn += ':';
                            }
                    }

                this.appIn += (char)keysym.sym;

                if (!render) return;

                Subsystem_UI.Instance.SetOrUpdateTextVar(renderTarget, this.appIn, FontNameType.RainyHearts, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });

            }


        public void SelectButtonPressed()
            {
                Console.WriteLine("Select!");

                if (Game.lazyInstance is null) return;

                if (Game.Instance.state is GameStartState)
                    {
                        Subsystem_UI.Instance.TryInputTime();
                    }
                else if (Game.Instance.state is GotchiPetViewState)
                    {
                        Subsystem_UI.Instance.pictoSelection.SelectNext();
                    }

            }
        public void ExecuteButtonPressed()
            {
                Console.WriteLine(value: "Execute!");

                int currentPicto = 
                    Subsystem_UI.Instance.pictoSelection.cursorIndex;

                switch (currentPicto)
                    {                

                        case 1:
                            // BATHROOM
                            Game.Instance.pet.Clean();
                            break;

                        case 2:
                            // FEED
                            Game.Instance.pet.Feed(Gotchi_Pet.MealType.MEAL);
                            break;

                        case 3:
                            // GAME (PLAY)
                            Game.Instance.pet.PlayWith();
                            break;

                        case 4:
                            // LIGHTS
                            Game.Instance.scene.ToggleLight();
                            break;

                        case 5:
                            // MEDICINE
                            Game.Instance.pet.GiveMeds();
                            break;

                        case 6:
                            // STATUS
                            Subsystem_UI.Instance.Meter(Game.Instance.pet);
                            break;

                        case 7:
                            // DISCIPLINE
                            Game.Instance.pet.Discipline();
                            break;

                        default:
                            break;
                    }
            }
        public void CancelButtonPressed()
            {
                Console.WriteLine("Cancel!");
                if (Game.Instance.state is GotchiPetViewState)
                    {
                        Subsystem_UI.Instance.pictoSelection.ClearSelect();
                    }
            }
    }