using static GotchiTaMm.Main_App;
using static GotchiTaMm.Util;

using static SDL2.SDL;

namespace GotchiTaMm;

internal class Game_Scene
    {
        private bool _lights_out = false;

        internal void Draw()
            {
                switch (Game.Instance.State)
                    {
                        case Game_State_Start:
                            {
                                Blit(Renderer, Subsystem_Ui.Instance.Text_Images_Dictionary.GetValueOrDefault(1), 260, 120);

                                if (Subsystem_Ui.Instance.Text_Vars_Dictionary.Count > 0)
                                    {
                                        Blit(Renderer, Subsystem_Ui.Instance.Text_Vars_Dictionary.GetValueOrDefault(Text_Var_Name_Type.TIME_START), 275, 144);
                                    }

                                SDL_Rect circle = new SDL_Rect {
                                        x = 150,
                                        y = 150,
                                        w = 50,
                                        h = 70,
                                    };
                                SDL_SetRenderDrawColor(Renderer, 255, 255, 0, 255);
                                Fill_Ellipsoid(Renderer, circle);
                                break;
                            }
                        case Game_State_Time_Set:
                            break;
                        case Game_State_Pet_View:
                            {
                                // Render the clock text to the screen?
                                if (Game.Instance.Clock is not null)
                                    {

                                        Subsystem_Ui.Instance.Set_Or_Update_Text_Var(Text_Var_Name_Type.CLOCK_TIME, Game.Instance.Clock.GetGameTime(), Font_Name_Type.RAINY_HEARTS, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });

                                        Blit(Renderer, Subsystem_Ui.Instance.Text_Vars_Dictionary.GetValueOrDefault(Text_Var_Name_Type.CLOCK_TIME), 15, 110);
                                    }
                                SDL_Rect circle = new SDL_Rect {
                                        x = 195,
                                        y = 132,
                                        w = 30,
                                        h = 42,
                                    };

                                SDL_SetRenderDrawColor(Renderer, 255, 255, 0, 255);
                                Fill_Ellipsoid(Renderer, circle);
                                break;
                            }
                        case Game_State_Pet_Evolve:
                            break;
                        case Game_State_Play_Time:
                            break;
                    }
            }


        internal void Toggle_Light()
            {
                this._lights_out = this._lights_out != true;
            }
    }