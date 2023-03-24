using static GotchiTaMm.Program;
using static SDL2.SDL;

namespace GotchiTaMm
{
    internal class Button
    {

        internal SDL_Rect Rectangle;
        internal SDL_Color[] Color = new SDL_Color[2];
        internal ButtonStateType ButtonState;

        // EVENTS FOR BUTTONS
        public event VoidDelegate? ButtonEvent;

        public Button(SDL_Rect rectangle, SDL_Color[] color, VoidDelegate button_event)
        {
            Rectangle = rectangle;
            Color = color;
            ButtonState = ButtonStateType.Unselected;
            ButtonEvent = button_event;
        }

        public void Draw()
        {
            if (TestMouseOverlap() == true)
            {
                if (InputSystem.Instance.mouse.buttons[1] == 1)
                {
                    SDL_SetRenderDrawColor(Renderer, 255, 0, Color[0].b, Color[0].a);
                }
                else
                {
                    SDL_SetRenderDrawColor(Renderer, 255, 255, Color[0].b, Color[0].a);
                }
            }
            else
            {

                SDL_SetRenderDrawColor(Renderer, Color[0].r, Color[0].g, Color[0].b, Color[0].a);
            }

            SDL_RenderFillRect(Renderer, ref Rectangle);

            SDL_SetRenderDrawColor(Renderer, Color[1].r, Color[1].g, Color[1].b, Color[1].a);
            SDL_RenderDrawRect(Renderer, ref Rectangle);
        }

        public bool TestMouseOverlap()
        {
            if (InputSystem.Instance.mouse.position.x > Rectangle.x
                && InputSystem.Instance.mouse.position.x < Rectangle.x + Rectangle.w
                && InputSystem.Instance.mouse.position.y > Rectangle.y
                && InputSystem.Instance.mouse.position.y < Rectangle.y + Rectangle.h)
            {
                return true;
            }
            return false;
        }

        internal void Activate()
        {
            ButtonEvent?.Invoke();
        }
    }
}
