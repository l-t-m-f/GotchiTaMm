using static SDL2.SDL;
using static SDL2.SDL_image;
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
        const int MAX_FONT_SIZE_FACTOR = 12;

        internal SDL_Rect Header = new SDL_Rect { x = 0, y = 0, w = Program.WINDOW_W, h = 10 };
        internal SDL_Rect Footer = new SDL_Rect { x = 0, y = Program.WINDOW_H - 50, w = Program.WINDOW_W, h = 50 };
        internal static UserInterface? Instance { get; private set; }
        internal List<Button> Buttons = new List<Button>();
        internal Dictionary<string, IntPtr[]> Fonts = new Dictionary<string, IntPtr[]>();
        internal Dictionary<string, IntPtr> Texts = new Dictionary<string, IntPtr>();
        internal Dictionary<string, IntPtr> Images = new Dictionary<string, IntPtr>();
        internal Dictionary<string, IntPtr> TextImages = new Dictionary<string, IntPtr>();

        private UserInterface()
        {
            SDL_Color[] button_color_theme = new SDL_Color[] {
                    new SDL_Color { r = 0, g = 255, b = 0, a = 255 },
                    new SDL_Color { r = 255, g = 0, b = 0, a = 255 },
                };
            Buttons.Add(new Button(new SDL_Rect {
                x = ((Program.WINDOW_W / 5) * 1) - 20,
                y = Program.WINDOW_H - 70,
                w = 40,
                h = 40
            },
                button_color_theme));

            Buttons.Add(new Button(new SDL_Rect {
                x = (Program.WINDOW_W / 2) - 20,
                y = Program.WINDOW_H - 70,
                w = 40,
                h = 40
            },
                button_color_theme));

            Buttons.Add(new Button(new SDL_Rect {
                x = ((Program.WINDOW_W / 5) * 4) - 20,
                y = Program.WINDOW_H - 70,
                w = 40,
                h = 40
            },
                button_color_theme));

            Fonts.Add("BlueScreen", new IntPtr[MAX_FONT_SIZE_FACTOR]);

            for(int i = 0 ; i < Fonts?.GetValueOrDefault("BlueScreen")?.GetLength(0) ; i++)
            {
                IntPtr lastFont = TTF_OpenFont("blue_screen.ttf", (int)Math.Pow(2, i));
                if (lastFont == IntPtr.Zero)
                {
                    Console.WriteLine("There was a problem loading the font");
                }

                IntPtr[]? fontsArray = Fonts?.GetValueOrDefault("BlueScreen");
                if(fontsArray != null)
                    fontsArray[i] = lastFont;
            }

            IntPtr testText = TTF_RenderUTF8_Blended(Fonts.GetValueOrDefault("BlueScreen")[4], "Test", new SDL_Color { r = 55, g = 0, b = 0, a = 255 });
            if (testText == IntPtr.Zero)
            {
                Console.WriteLine("There was a creating text pointer");
            }
            Texts.Add("Test", testText);

            IntPtr testTextImage = SDL_CreateTextureFromSurface(Program.Renderer, Texts.GetValueOrDefault("Test"));
            if (testTextImage == IntPtr.Zero)
            {
                Console.WriteLine("There was a creating text image pointer");
            }
            TextImages.Add("Test", testTextImage);

            Images.Add("Attention", IMG_LoadTexture(Program.Renderer, "gfx/Attention.png"));
            Images.Add("Bathroom", IMG_LoadTexture(Program.Renderer, "gfx/Bathroom.png"));
            Images.Add("Food", IMG_LoadTexture(Program.Renderer, "gfx/Food.png"));
            Images.Add("Game", IMG_LoadTexture(Program.Renderer, "gfx/Game.png"));
            Images.Add("Lights", IMG_LoadTexture(Program.Renderer, "gfx/Lights.png"));
            Images.Add("Medicine", IMG_LoadTexture(Program.Renderer, "gfx/Medicine.png"));
            Images.Add("Status", IMG_LoadTexture(Program.Renderer, "gfx/Status.png"));
            Images.Add("Training", IMG_LoadTexture(Program.Renderer, "gfx/Training.png"));
        }

        public static UserInterface GetUI()
        {
            if (Instance == null)
            {
                Instance = new UserInterface();
            }
            return Instance;
        }

        public void Draw()
        {
            SDL_SetRenderDrawColor(Program.Renderer, 0, 0, 0, 255);
            SDL_RenderFillRect(Program.Renderer, ref Header);
            SDL_RenderFillRect(Program.Renderer, ref Footer);

            foreach (Button b in Buttons)
            {
                b.Draw();
            }

            int x = 0;
            int y = 0;

            foreach(IntPtr i in Images.Values)
            {
                Program.Blit(i, 50 * x, 25 * y);
                x++; y++;
            }
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
            if(TestMouseOverlap() == true)
            {
                if (Program.Mouse.Buttons[1] == 1)
                {
                    SDL_SetRenderDrawColor(Program.Renderer, 255, 0, Color[0].b, Color[0].a);
                }
                else
                {
                    SDL_SetRenderDrawColor(Program.Renderer, 255, 255, Color[0].b, Color[0].a);
                }
            }
            else
            {

                SDL_SetRenderDrawColor(Program.Renderer, Color[0].r, Color[0].g, Color[0].b, Color[0].a);
            }

            SDL_RenderFillRect(Program.Renderer, ref Rectangle);

            SDL_SetRenderDrawColor(Program.Renderer, Color[1].r, Color[1].g, Color[1].b, Color[1].a);
            SDL_RenderDrawRect(Program.Renderer, ref Rectangle);
        }

        public bool TestMouseOverlap()
        {
            if (Program.Mouse.Position.x > Rectangle.x
                && Program.Mouse.Position.x < Rectangle.x + Rectangle.w
                && Program.Mouse.Position.y > Rectangle.y
                && Program.Mouse.Position.y < Rectangle.y + Rectangle.h)
            {
                return true;
            }
            return false;
        }
    }
}
