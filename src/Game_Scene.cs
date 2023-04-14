using static GotchiTaMm.Main_App;
using static GotchiTaMm.Util;

using static SDL2.SDL;

namespace GotchiTaMm;

internal class Game_Scene
    {
        internal void Draw()
            {
                SDL_RenderCopy(Renderer, Subsystem_Imaging.Instance
                    .Sprite_Atlas.Get_Atlas_Image("Background"), IntPtr.Zero, IntPtr.Zero);
                switch (Game.Instance.State)
                    {
                        case Game_State_Start:
                            {
                                Blit(Renderer, Subsystem_UI.Instance.Text_Images_Dictionary.GetValueOrDefault(1), 20, 50);

                                if (Subsystem_UI.Instance.Text_Vars_Dictionary.Count > 0)
                                    {
                                        Blit(Renderer, Subsystem_UI.Instance.Text_Vars_Dictionary.GetValueOrDefault(Text_Var_Name_Type.TIME_START), 50, 50);
                                    }

                                SDL_Rect egg = Subsystem_Imaging.Instance
                                    .Sprite_Atlas
                                    .Get_Atlas_Image_Rect("Egg_f1");
                                egg.x = 100;
                                egg.y = 100;
                                egg.w = (int)(egg.w * SCREEN_RATIO);
                                egg.h = (int)(egg.h * SCREEN_RATIO);
                                SDL_RenderCopy(Renderer, Subsystem_Imaging.Instance
                                    .Sprite_Atlas.Get_Atlas_Image("Egg_f1"), 
                                    IntPtr.Zero, 
                                    ref egg);
                                break;
                            }
                        case Game_State_Time_Set:
                            break;
                        case Game_State_Pet_View:
                            {
                                // Render the clock text to the screen?
                                if (Game.Instance.Clock is not null)
                                    {

                                        Subsystem_UI.Instance.Set_Or_Update_Text_Var(Text_Var_Name_Type.CLOCK_TIME,
                                            Game.Instance.Clock.Get_Game_Time(),
                                            Font_Name_Type.RAINY_HEARTS, 6,
                                            new SDL_Color
                                                {
                                                    r = 55, g = 125, b = 125,
                                                    a = 255
                                                });

                                        Blit(Renderer,
                                            Subsystem_UI.Instance
                                                .Text_Vars_Dictionary
                                                .GetValueOrDefault(Text_Var_Name_Type.CLOCK_TIME), 15, 110);
                                    }
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
                Game.Instance.State.Lights_Out = Game.Instance.State.Lights_Out != true;
            }
    }