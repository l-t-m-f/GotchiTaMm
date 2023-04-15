using static GotchiTaMm.Main_App;
using static SDL2.SDL;

namespace GotchiTaMm;

internal class Button
    {
        private Art_Style_Type _art_style;
        private string _artwork_call_name;
        internal Button_State_Type Button_State;
        private SDL_Point _offset;

        // EVENTS FOR BUTTONS
        public event VoidDelegate? ButtonEvent;

        public Button(Art_Style_Type art_style, 
        string artwork_call_name, VoidDelegate button_event, SDL_Point offset)
            {
                this._art_style = art_style;
                this._artwork_call_name = artwork_call_name;
                this._offset = offset;
                this.Button_State = Button_State_Type.UNSELECTED;
                this.ButtonEvent = button_event;
            }

        public void Draw()
            {
                if (this._art_style == Art_Style_Type.SINGLE_SPRITE)
                    {
                        IntPtr ptr_to_art =
                            Subsystem_Imaging.Instance.Sprite_Atlas
                                .Get_Atlas_Image(this._artwork_call_name);
                        SDL_Rect art_dest =
                            Subsystem_Imaging.Instance.Sprite_Atlas
                                .Get_Atlas_Image_Rect(
                                    this._artwork_call_name);
                        art_dest.w = (int)(art_dest.w * SCREEN_RATIO);
                        art_dest.h = (int)(art_dest.h * SCREEN_RATIO);
                        art_dest.x = (int)(this._offset.x * SCREEN_RATIO)
                                     - art_dest.w / 2;
                        art_dest.y = (int)(this._offset.y * SCREEN_RATIO)
                                     - art_dest.h / 2;
                        SDL_RenderCopy(Renderer, ptr_to_art,
                            IntPtr.Zero, ref art_dest);
                    }
                // if (this.TestMouseOverlap())
                //     {
                //         SDL_SetRenderDrawColor(Renderer, 255,
                //             Subsystem_Input.Instance.Mouse.Buttons[1] == 1
                //                 ? (byte)0
                //                 : (byte)255, this._color[0].b,
                //             this._color[0].a);
                //     }
                // else
                //     {
                //         SDL_SetRenderDrawColor(Renderer, this._color[0].r,
                //             this._color[0].g, this._color[0].b,
                //             this._color[0].a);
                //     }
                //
                // SDL_RenderFillRect(Renderer, ref this._rectangle);
                //
                // SDL_SetRenderDrawColor(Renderer, this._color[1].r,
                //     this._color[1].g, this._color[1].b, this._color[1].a);
                // SDL_RenderDrawRect(Renderer, ref this._rectangle);
            }

        // public bool TestMouseOverlap()
        //     {
        //         return Subsystem_Input.Instance.Mouse.Position.x >
        //                this._rectangle.x
        //                && Subsystem_Input.Instance.Mouse.Position.x <
        //                this._rectangle.x + this._rectangle.w
        //                && Subsystem_Input.Instance.Mouse.Position.y >
        //                this._rectangle.y
        //                && Subsystem_Input.Instance.Mouse.Position.y <
        //                this._rectangle.y + this._rectangle.h;
        //     }

        internal void Activate()
            {
                this.ButtonEvent?.Invoke();
            }
    }