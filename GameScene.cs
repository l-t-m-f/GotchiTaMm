using static GotchiTaMm.Program;
using static GotchiTaMm.Util;

using static SDL2.SDL;

namespace GotchiTaMm
{
    internal class GameScene
    {

        internal void Draw()
        {
            if (Game.Instance.state is GameStartState)
            {
                Blit(Renderer, UserInterface.Instance.textImagesDictio.GetValueOrDefault(1), 260, 120);

                if (UserInterface.Instance.textVarsDictio.Count > 0)
                {
                    Blit(Renderer, UserInterface.Instance.textVarsDictio.GetValueOrDefault(TextVarNameType.TimeStart), 275, 144);
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

                    string clockString = $"{Game.Instance.clock.GetGameTimeHours():D2}:{Game.Instance.clock.GetGameTimeMinutes():D2}";

                    UserInterface.Instance.SetOrUpdateTextVar(TextVarNameType.ClockTime, clockString, FontNameType.RainyHearts, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });

                    Blit(Renderer, UserInterface.Instance.textVarsDictio.GetValueOrDefault(TextVarNameType.ClockTime), 15, 110);
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


    }
}
