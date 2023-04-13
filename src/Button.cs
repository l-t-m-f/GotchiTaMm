using static GotchiTaMm.Main_App;
using static SDL2.SDL;

namespace GotchiTaMm;

internal class Button
    {

        internal SDL_Rect Rectangle;
        internal SDL_Color[] Color = new SDL_Color[2];
        internal ButtonStateType ButtonState;

        // EVENTS FOR BUTTONS
        public event VoidDelegate? ButtonEvent;

        public Button(SDL_Rect rectangle, SDL_Color[] color, VoidDelegate button_event)
            {
                this.Rectangle = rectangle;
                this.Color = color;
                this.ButtonState = ButtonStateType.Unselected;
                this.ButtonEvent = button_event;
            }

        public void Draw()
            {
                if (this.TestMouseOverlap() == true)
                    {
                        if (Subsystem_Input.Instance.mouse.buttons[1] == 1)
                            {
                                SDL_SetRenderDrawColor(Renderer, 255, 0, this.Color[0].b, this.Color[0].a);
                            }
                        else
                            {
                                SDL_SetRenderDrawColor(Renderer, 255, 255, this.Color[0].b, this.Color[0].a);
                            }
                    }
                else
                    {

                        SDL_SetRenderDrawColor(Renderer, this.Color[0].r, this.Color[0].g, this.Color[0].b, this.Color[0].a);
                    }

                SDL_RenderFillRect(Renderer, ref this.Rectangle);

                SDL_SetRenderDrawColor(Renderer, this.Color[1].r, this.Color[1].g, this.Color[1].b, this.Color[1].a);
                SDL_RenderDrawRect(Renderer, ref this.Rectangle);
            }

        public bool TestMouseOverlap()
            {
                if (Subsystem_Input.Instance.mouse.position.x > this.Rectangle.x
                    && Subsystem_Input.Instance.mouse.position.x < this.Rectangle.x + this.Rectangle.w
                    && Subsystem_Input.Instance.mouse.position.y > this.Rectangle.y
                    && Subsystem_Input.Instance.mouse.position.y < this.Rectangle.y + this.Rectangle.h)
                    {
                        return true;
                    }
                return false;
            }

        internal void Activate()
            {
                this.ButtonEvent?.Invoke();
            }
    }