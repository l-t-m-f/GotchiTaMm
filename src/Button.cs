using static GotchiTaMm.Main_App;
using static SDL2.SDL;

namespace GotchiTaMm;

internal class Button
    {
        private Art_Style_Type _art_style;
        private string _artwork_call_name;
        internal Button_State_Type Button_State;
        private SDL_Point _offset;
        private SDL_Rect _collider;

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
                        IntPtr ptr_to_art;
                       
                        if (this.TestMouseOverlap())
                            {
                
                                this._collider =
                                    Subsystem_Imaging.Instance.Sprite_Atlas
                                        .Get_Atlas_Image_Rect($"{this._artwork_call_name}_hovered");
                                this._collider.w = (int)(this._collider.w * SCREEN_RATIO);
                                this._collider.h = (int)(this._collider.h * SCREEN_RATIO);
                                this._collider.x = (int)(this._offset.x * SCREEN_RATIO)
                                    - this._collider.w / 2;
                                this._collider.y = (int)(this._offset.y * SCREEN_RATIO)
                                    - this._collider.h / 2;
                                ptr_to_art = Subsystem_Imaging.Instance.Sprite_Atlas
                                    .Get_Atlas_Image($"{this._artwork_call_name}_hovered");
                            }
                        else
                            {
                
                                this._collider =
                                    Subsystem_Imaging.Instance.Sprite_Atlas
                                        .Get_Atlas_Image_Rect($"{this._artwork_call_name}_silent");
                                this._collider.w = (int)(this._collider.w * SCREEN_RATIO);
                                this._collider.h = (int)(this._collider.h * SCREEN_RATIO);
                                this._collider.x = (int)(this._offset.x * SCREEN_RATIO)
                                    - this._collider.w / 2;
                                this._collider.y = (int)(this._offset.y * SCREEN_RATIO)
                                    - this._collider.h / 2;
                                ptr_to_art = Subsystem_Imaging.Instance.Sprite_Atlas
                                    .Get_Atlas_Image($"{this._artwork_call_name}_silent");
                            }
                        
                        
                        SDL_RenderCopy(Renderer, ptr_to_art,
                            IntPtr.Zero, ref this._collider);
                    }
            }

        public bool TestMouseOverlap()
            {
                return Subsystem_Input.Instance.Mouse.Position.x >
                       this._collider.x
                       && Subsystem_Input.Instance.Mouse.Position.x <
                       this._collider.x + this._collider.w
                       && Subsystem_Input.Instance.Mouse.Position.y >
                       this._collider.y
                       && Subsystem_Input.Instance.Mouse.Position.y <
                       this._collider.y + this._collider.h;
            }

        internal void Activate()
            {
                this.ButtonEvent?.Invoke();
            }
    }