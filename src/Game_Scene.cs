using static GotchiTaMm.Main_App;
using static GotchiTaMm.Util;

using static SDL2.SDL;

namespace GotchiTaMm;

internal class Game_Scene
    {
        internal bool LightsOut = false;

        internal void Draw()
            {
                if (Game.Instance.state is GameStartState)
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

                    }
                else if (Game.Instance.state is TimeSetPauseState)
                    {
                    }
                else if (Game.Instance.state is GotchiPetViewState)
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
                    }
                else if (Game.Instance.state is GotchiPetEvolveState)
                    {
                    }
                else if (Game.Instance.state is GotchiGameState)
                    {
                    }
            }


        internal void ToggleLight()
            {
                if (this.LightsOut == true)
                    {
                        this.LightsOut = false;
                    }
                else
                    {
                        this.LightsOut = true;
                    }
            }
    }