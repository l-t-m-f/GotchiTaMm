using static SDL2.SDL;

namespace GotchiTaMm;


internal class Picto_Selection
    {
        // internal IntPtr Image;
        internal int Cursor_Index;
        internal SDL_Rect Selection_Pos_And_Size;

        internal void Select_Next()
            {
                if (this.Cursor_Index == 7)
                    {
                        this.Cursor_Index = 1;
                    }
                else
                    {
                        this.Cursor_Index++;
                    }
                //
                // this.Selection_Pos_And_Size = Instance.ImagesDictio
                //     .GetValueOrDefault(
                //         ((PictoNameType)this.Cursor_Index).ToString())
                //     .Rectangle;
                this.Selection_Pos_And_Size.x -= 5;
                this.Selection_Pos_And_Size.y -= 5;
                this.Selection_Pos_And_Size.w += 5;
                this.Selection_Pos_And_Size.h += 5;
            }

        internal void Clear_Select()
            {
                this.Cursor_Index = -1;
            }
    }