using static GotchiTaMm.Main_App;
using static SDL2.SDL;

namespace GotchiTaMm;

internal class Mouse
    {
        internal SDL_Point Position;
        internal readonly int[] Buttons = new int[4];
    }

internal class Keyboard
    {
        internal readonly int[] State = new int[255];
    }
internal class Subsystem_Input
    {
        // Inner classes


        //

        internal readonly Mouse Mouse;
        private readonly Keyboard _keyboard;
        internal string App_In = "";
        private const int _APP_IN_LIMIT = 5;

        //Singleton

        private static readonly Lazy<Subsystem_Input> _Lazy_Instance = new(() => new Subsystem_Input());
        private Subsystem_Input()
            {
                this.Mouse = new Mouse();
                this._keyboard = new Keyboard();
            }

        public static Subsystem_Input Instance => _Lazy_Instance.Value;

        //
        
        public void OnKeyDown(SDL_Keysym keysym)
            {
                Console.WriteLine($"Key down: {keysym.scancode}");
                this._keyboard.State[(int)keysym.scancode] = 1;

                if (keysym.scancode == SDL_Scancode.SDL_SCANCODE_ESCAPE)
                    {
                        QuitGame(0);
                    }
                else
                    {
                        switch (Game.Instance.State)
                            {
                                case Game_State_Start:
                                    {
                                        if (keysym.scancode == SDL_Scancode.SDL_SCANCODE_BACKSPACE)
                                            {
                                                this.Trim_App_In(true, Text_Var_Name_Type.TIME_START, true);
                                            }

                                        if (char.IsAsciiDigit((char)keysym.sym))
                                            {
                                                this.Push_Into_App_In(keysym, true, Text_Var_Name_Type.TIME_START, true);
                                            }

                                        Console.WriteLine(this.App_In);
                                        break;
                                    }
                                case Game_State_Time_Set:
                                    break;
                                case Game_State_Pet_View:
                                    break;
                                case Game_State_Pet_Evolve:
                                    break;
                                case Game_State_Play_Time:
                                    break;
                            }
                    }

            }

        public void On_Key_Up(SDL_Keysym keysym)
            {
                //Console.WriteLine($"Key up: {keysym.scancode}");
                this._keyboard.State[(int)keysym.scancode] = 0;
            }

        public void On_Mouse_Down(SDL_MouseButtonEvent mouse_button_event)
            {
                Console.WriteLine($"Mouse click: {mouse_button_event.button} at {mouse_button_event.x}, {mouse_button_event.y}");
                if (mouse_button_event.button <= 3)
                    {
                        Instance.Mouse.Buttons[mouse_button_event.button] = 1;
                    }

            }

        public void On_Mouse_Up(SDL_MouseButtonEvent mouse_button_event)
            {
                //Console.WriteLine($"Mouse released: {mouseButtonEvent.button} at {mouseButtonEvent.x}, {mouseButtonEvent.y}");
                if (mouse_button_event.button <= 3)
                    {
                        Instance.Mouse.Buttons[mouse_button_event.button] = 0;
                    }
            }
        public void On_Mouse_Move(SDL_MouseMotionEvent mouse_motion_event)
            {
                //Console.WriteLine($"Mouse moving at {mouseMotionEvent.x}, {mouseMotionEvent.y}");
                Instance.Mouse.Position.x = mouse_motion_event.x;
                Instance.Mouse.Position.y = mouse_motion_event.y;
            }

        private void Trim_App_In(bool render = false, Text_Var_Name_Type render_target = 0, bool is_time = false)
            {
                if (this.App_In.Length == 0)
                    {
                        return;
                    }

                if (is_time)
                    {
                        if (Instance.App_In.Length == 4)
                            {
                                this.App_In = this.App_In.Drop_Last_Char();
                            }
                    }

                this.App_In = this.App_In.Drop_Last_Char();

                if (!render)
                    {
                        return;
                    }

                Subsystem_UI.Instance.Set_Or_Update_Text_Var(render_target, this.App_In, Font_Name_Type.RAINY_HEARTS, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });
            }

        private void Push_Into_App_In(SDL_Keysym keysym, bool render = false, Text_Var_Name_Type render_target = 0, bool is_time = false)
            {
                if (this.App_In.Length >= _APP_IN_LIMIT)
                    {
                        return;
                    }

                if (is_time)
                    {
                        if (Instance.App_In.Length == 2)
                            {
                                Instance.App_In += ':';
                            }
                    }

                this.App_In += (char)keysym.sym;

                if (!render)
                    {
                        return;
                    }

                Subsystem_UI.Instance.Set_Or_Update_Text_Var(render_target, this.App_In, Font_Name_Type.RAINY_HEARTS, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });

            }


        public void Select_Button_Pressed()
            {
                Console.WriteLine("Select!");

                switch (Game.Instance.State)
                    {
                        case Game_State_Start:
                            Subsystem_UI.Instance.Try_Input_Time();
                            break;
                        case Game_State_Pet_View:
                            Subsystem_UI.Instance.Picto_Selection.Select_Next();
                            break;
                    }
            }
        public void Execute_Button_Pressed()
            {
                Console.WriteLine(value: "Execute!");

                int current_picto = 
                    Subsystem_UI.Instance.Picto_Selection.Cursor_Index;

                switch (current_picto)
                    {                

                        case 1:
                            // BATHROOM
                            Game.Instance.Pet.Clean();
                            break;

                        case 2:
                            // FEED
                            Game.Instance.Pet.Feed(Gotchi_Pet.Meal_Type.MEAL);
                            break;

                        case 3:
                            // GAME (PLAY)
                            Game.Instance.Pet.Play_With();
                            break;

                        case 4:
                            // LIGHTS
                            Game.Instance.Scene.Toggle_Light();
                            break;

                        case 5:
                            // MEDICINE
                            Game.Instance.Pet.Give_Meds();
                            break;

                        case 6:
                            // STATUS
                            Subsystem_UI.Instance.Meter(Game.Instance.Pet);
                            break;

                        case 7:
                            // DISCIPLINE
                            Game.Instance.Pet.Discipline();
                            break;

                        default:
                            break;
                    }
            }
        public void Cancel_Button_Pressed()
            {
                Console.WriteLine("Cancel!");
                if (Game.Instance.State is Game_State_Pet_View)
                    {
                        Subsystem_UI.Instance.Picto_Selection.Clear_Select();
                    }
            }
    }