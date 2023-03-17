using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace GotchiTaMm
{

    internal enum ButtonStateType
    {
        Unselected = 0,
        Selected,
        Activated,
    }

    internal enum PictoNameType
    {
        Attention = 0,
        Bathroom,
        Food,
        Game,
        Lights,
        Medicine,
        Status,
        Training,
    }

    internal enum FontNameType
    {
        BlueScreen = 0,
    }

    internal class UserInterface
    {

        internal struct PackedImage
        {
            internal IntPtr Pointer;
            internal SDL_Rect Rectangle;

            public PackedImage(IntPtr pointer, SDL_Rect rect)
            {
                Pointer = pointer;
                Rectangle = rect;
            }
        }

        const int MAX_FONT_SIZE_FACTOR = 12;

        internal SDL_Rect Header = new SDL_Rect { x = 0, y = 0, w = Program.WINDOW_W, h = 10 };
        internal SDL_Rect Footer = new SDL_Rect { x = 0, y = Program.WINDOW_H - 50, w = Program.WINDOW_W, h = 50 };
        internal static UserInterface? Instance { get; private set; }
        internal List<Button> Buttons = new List<Button>();

        internal Dictionary<string, PackedImage> Images = new Dictionary<string, PackedImage>();

        internal Dictionary<string, IntPtr[]> Fonts = new Dictionary<string, IntPtr[]>();
        internal Dictionary<int, IntPtr> Texts = new Dictionary<int, IntPtr>();
        internal Dictionary<int, IntPtr> TextImages = new Dictionary<int, IntPtr>();

        internal static class GameStringPool
        {

            internal class StringPoolEntry
            {
                internal string Text;
                internal FontNameType Font;
                internal int SizeFactor;

                public StringPoolEntry(string text, FontNameType font, int sizeFactor)
                {
                    Text = text;
                    Font = font;
                    SizeFactor = sizeFactor;
                }
            }

            internal static List<StringPoolEntry> Data = new List<StringPoolEntry>()
            {
                new("Test", FontNameType.BlueScreen, 4),
            };
        }


        private UserInterface()
        {
            InitButtonsSubroutine();
            InitFontsSubroutine();
            InitTextImagesSubroutine();
            InitImagesSubroutine();
        }

        private void InitButtonsSubroutine()
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
        }

        private void InitFontsSubroutine()
        {
            int fontCount = Enum.GetValues(typeof(FontNameType)).Length;
            for (int i = 0 ; i < fontCount ; i++)
            {
                Fonts?.Add(((FontNameType)i).ToString(), new IntPtr[MAX_FONT_SIZE_FACTOR]);

                for (int j = 0 ; j < Fonts?.GetValueOrDefault(((FontNameType)i).ToString())?.GetLength(0) ; j++)
                {
                    IntPtr lastFont = TTF_OpenFont($"{(FontNameType)i}.ttf", (int)Math.Pow(2, j));
                    if (lastFont == IntPtr.Zero)
                    {
                        Console.WriteLine("There was a problem loading the font");
                    }

                    IntPtr[]? fontsArray = Fonts?.GetValueOrDefault(((FontNameType)i).ToString());
                    if (fontsArray != null) fontsArray[j] = lastFont;
                }
            }
        }

        private void InitTextImagesSubroutine()
        {
            int stringPoolLen = GameStringPool.Data.Count;

            for (int i = 0 ; i < stringPoolLen ; i++)
            {
                string fontName = GameStringPool.Data[i].Font.ToString();
                int fontSizeFac = GameStringPool.Data[i].SizeFactor;
                IntPtr fontToUse = Fonts.GetValueOrDefault(fontName)[fontSizeFac];

                IntPtr testText = TTF_RenderUTF8_Blended(fontToUse, GameStringPool.Data[i].Text, new SDL_Color { r = 55, g = 0, b = 0, a = 255 });
                if (testText == IntPtr.Zero)
                {
                    Console.WriteLine("There was a problem creating text pointer");
                }

                Texts.Add(i, testText);

                IntPtr testTextImage = SDL_CreateTextureFromSurface(Program.Renderer, Texts.GetValueOrDefault(i));
                if (testTextImage == IntPtr.Zero)
                {
                    Console.WriteLine("There was a problem creating text image pointer");
                }
                TextImages.Add(i, testTextImage);
            }
        }

        private void InitImagesSubroutine()
        {
            int pictoCount = Enum.GetValues(typeof(PictoNameType)).Length;
            for (int i = 0 ; i < pictoCount ; i++)
            {
                string lastPictoName = ((PictoNameType)i).ToString();
                IntPtr imagePtr = IMG_LoadTexture(Program.Renderer, $"gfx/{lastPictoName}.png");
                if (imagePtr == IntPtr.Zero)
                {
                    Console.WriteLine("There was a problem creating image pointer");
                }

                SDL_Rect imageRect;

                if (i <= 3)
                {
                    imageRect = new SDL_Rect { x = 80 + (100 * i), y = 40, w = 40, h = 30 };
                }
                else
                {
                    imageRect = new SDL_Rect { x = 80 + (100 * (i-4)), y = 200, w = 40, h = 30 };
                }

                Images.Add(lastPictoName, new PackedImage(imagePtr, imageRect));
            }
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

            foreach (PackedImage i in Images.Values)
            {
                Program.BlitRect(i.Pointer, i.Rectangle);
            }
        }
    }

    internal class Button
    {
        internal SDL_Rect Rectangle;
        internal SDL_Color[] Color = new SDL_Color[2];
        internal ButtonStateType ButtonState;

        public Button(SDL_Rect rectangle, SDL_Color[] color)
        {
            Rectangle = rectangle;
            Color = color;
            ButtonState = ButtonStateType.Unselected;
        }

        public void Draw()
        {
            if (TestMouseOverlap() == true)
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
