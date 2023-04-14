using static GotchiTaMm.Main_App;
using static SDL2.SDL;

namespace GotchiTaMm;

internal class Button
    {
        private SDL_Rect _rectangle;
        private readonly SDL_Color[] _color;
        internal Button_State_Type Button_State;

        // EVENTS FOR BUTTONS
        public event VoidDelegate? ButtonEvent;

        public Button(SDL_Rect rectangle, SDL_Color[] color,
            VoidDelegate button_event)
            {
                this._rectangle = rectangle;
                this._color = color;
                this.Button_State = Button_State_Type.UNSELECTED;
                this.ButtonEvent = button_event;
            }

        public void Draw()
            {
                if (this.TestMouseOverlap())
                    {
                        SDL_SetRenderDrawColor(Renderer, 255,
                            Subsystem_Input.Instance.Mouse.Buttons[1] == 1
                                ? (byte)0
                                : (byte)255, this._color[0].b,
                            this._color[0].a);
                    }
                else
                    {
                        SDL_SetRenderDrawColor(Renderer, this._color[0].r,
                            this._color[0].g, this._color[0].b,
                            this._color[0].a);
                    }

                SDL_RenderFillRect(Renderer, ref this._rectangle);

                SDL_SetRenderDrawColor(Renderer, this._color[1].r,
                    this._color[1].g, this._color[1].b, this._color[1].a);
                SDL_RenderDrawRect(Renderer, ref this._rectangle);
            }

        public bool TestMouseOverlap()
            {
                return Subsystem_Input.Instance.Mouse.Position.x >
                       this._rectangle.x
                       && Subsystem_Input.Instance.Mouse.Position.x <
                       this._rectangle.x + this._rectangle.w
                       && Subsystem_Input.Instance.Mouse.Position.y >
                       this._rectangle.y
                       && Subsystem_Input.Instance.Mouse.Position.y <
                       this._rectangle.y + this._rectangle.h;
            }

        internal void Activate()
            {
                this.ButtonEvent?.Invoke();
            }
    }