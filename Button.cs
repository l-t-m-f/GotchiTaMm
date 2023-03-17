using static SDL2.SDL;
using static SDL2.SDL_ttf;

namespace GotchiTaMm
{
    internal enum ButtonState
    {
        Undefined = -1,
        Alone = 0,
        Hovered = 1,
        Pressed = 2,
        Activated = 3,
    }

    internal class UserInterface
    {
        internal static UserInterface? Instance { get; private set; }
        internal Button[] buttons = new Button[3];
        internal IntPtr[] fonts = new IntPtr[3];
        internal IntPtr[] texts = new IntPtr[3];

        private UserInterface()
        {
            buttons[0] = new Button(
                new SDL_Rect { x = 100, y = 300, w = 50, h = 50 },
                new SDL_Color[] {
                    new SDL_Color { r = 0, g = 255, b = 0, a = 255 },
                    new SDL_Color { r = 255, g = 0, b = 0, a = 255 },
                });

            buttons[1] = new Button(
                new SDL_Rect { x = 200, y = 300, w = 50, h = 50 },
                new SDL_Color[] {
                    new SDL_Color { r = 0, g = 255, b = 0, a = 255 },
                    new SDL_Color { r = 255, g = 0, b = 0, a = 255 },
                });

            buttons[2] = new Button(
                new SDL_Rect { x = 300, y = 300, w = 50, h = 50 },
                new SDL_Color[] {
                    new SDL_Color { r = 0, g = 255, b = 0, a = 255 },
                    new SDL_Color { r = 255, g = 0, b = 0, a = 255 },
                });

            fonts[0] = TTF_OpenFont("blue_screen.ttf", 36);
            if (fonts[0] == IntPtr.Zero)
            {
                Console.WriteLine("There was a problem loading the font");
            }

            texts[0] = TTF_RenderUTF8_Blended(fonts[0], "Test", new SDL_Color { r = 55, g = 0, b = 0, a = 255 });
            if (texts[0] == IntPtr.Zero)
            {
                Console.WriteLine("There was a creating surface of text");
            }
        }

        public static UserInterface GetUI()
        {
            if(Instance == null)
            {
                Instance = new UserInterface();
            }
            return Instance;
        }
    }

    internal class Button
    {
        internal SDL_Rect Rectangle;
        internal SDL_Color[] Color = new SDL_Color[2];
        internal ButtonState ButtonState;

        public Button(SDL_Rect rectangle, SDL_Color[] color)
        {
            Rectangle = rectangle;
            Color = color;
            ButtonState = ButtonState.Alone;
        }

        public void Draw()
        {
            SDL_SetRenderDrawColor(Program.Renderer, Color[0].r, Color[0].g, Color[0].b, Color[0].a);
            SDL_RenderFillRect(Program.Renderer, ref Rectangle);

            SDL_SetRenderDrawColor(Program.Renderer, Color[1].r, Color[1].g, Color[1].b, Color[1].a);
            SDL_RenderDrawRect(Program.Renderer, ref Rectangle);
        }
    }
}
