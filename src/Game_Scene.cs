using static GotchiTaMm.Main_App;
using static GotchiTaMm.Util;

using static SDL2.SDL;

namespace GotchiTaMm;

internal class Game_Scene
    {
        private bool _lights_out = false;

        internal void Draw()
            {
                switch (Game.Instance.state)
                    {
                        case GameStartState:
                            {
                                Blit(Renderer, Subsystem_UI.Instance.textImagesDictio.GetValueOrDefault(1), 260, 120);

                                if (Subsystem_UI.Instance.textVarsDictio.Count > 0)
                                    {
                                        Blit(Renderer, Subsystem_UI.Instance.textVarsDictio.GetValueOrDefault(TextVarNameType.TimeStart), 275, 144);
                                    }

                                SDL_Rect circle = new SDL_Rect {
                                        x = 150,
                                        y = 150,
                                        w = 50,
                                        h = 70,
                                    };
                                SDL_SetRenderDrawColor(Renderer, 255, 255, 0, 255);
                                FillEllipsoid(Renderer, circle);
                                break;
                            }
                        case TimeSetPauseState:
                            break;
                        case GotchiPetViewState:
                            {
                                // Render the clock text to the screen?
                                if (Game.Instance.clock is not null)
                                    {

                                        Subsystem_UI.Instance.SetOrUpdateTextVar(TextVarNameType.ClockTime, Game.Instance.clock.GetGameTime(), FontNameType.RainyHearts, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });

                                        Blit(Renderer, Subsystem_UI.Instance.textVarsDictio.GetValueOrDefault(TextVarNameType.ClockTime), 15, 110);
                                    }
                                SDL_Rect circle = new SDL_Rect {
                                        x = 195,
                                        y = 132,
                                        w = 30,
                                        h = 42,
                                    };

                                SDL_SetRenderDrawColor(Renderer, 255, 255, 0, 255);
                                FillEllipsoid(Renderer, circle);
                                break;
                            }
                        case GotchiPetEvolveState:
                            break;
                        case GotchiGameState:
                            break;
                    }
            }


        internal void Toggle_Light()
            {
                this._lights_out = this._lights_out != true;
            }
    }