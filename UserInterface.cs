using System.Text.RegularExpressions;
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
        RainyHearts,
    }

    internal enum TextVarNameType
    {
        TimeStart = 0,

    }

    internal enum ButtonNameType
    {
        Select = 0,
        Execute,
        Cancel,
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

        string H24ClockRegex = @"^([01]\d|2[0-3]):([0-5]\d)$";
        Regex ClockRegex;

        const int MAX_FONT_SIZE_FACTOR = 12;

        internal bool DrawPictos = false;

        internal SDL_Rect Header = new SDL_Rect { x = 0, y = 0, w = Program.WINDOW_W, h = 10 };
        internal SDL_Rect Footer = new SDL_Rect { x = 0, y = Program.WINDOW_H - 50, w = Program.WINDOW_W, h = 50 };
        internal static UserInterface? Instance { get; private set; }
        internal Dictionary<ButtonNameType, Button> Buttons = new Dictionary<ButtonNameType, Button>();

        internal Dictionary<string, PackedImage> Images = new Dictionary<string, PackedImage>();

        internal Dictionary<FontNameType, IntPtr[]> Fonts = new Dictionary<FontNameType, IntPtr[]>();

        internal Dictionary<int, IntPtr> Texts = new Dictionary<int, IntPtr>();
        internal Dictionary<int, IntPtr> TextImages = new Dictionary<int, IntPtr>();

        // Dictionary of "TextVar", which are just text SDL_texture coming from user input.
        internal Dictionary<TextVarNameType, IntPtr> TextVars = new Dictionary<TextVarNameType, IntPtr>();

        private UserInterface()
        {
            ClockRegex = new Regex(H24ClockRegex);
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

            Buttons.Add(ButtonNameType.Select, new Button(new SDL_Rect {
                x = ((Program.WINDOW_W / 5) * 1) - 20,
                y = Program.WINDOW_H - 70,
                w = 40,
                h = 40
            },
                button_color_theme, Select));

            Buttons.Add(ButtonNameType.Execute, new Button(new SDL_Rect {
                x = (Program.WINDOW_W / 2) - 20,
                y = Program.WINDOW_H - 70,
                w = 40,
                h = 40
            },
                button_color_theme, Execute));

            Buttons.Add(ButtonNameType.Cancel, new Button(new SDL_Rect {
                x = ((Program.WINDOW_W / 5) * 4) - 20,
                y = Program.WINDOW_H - 70,
                w = 40,
                h = 40
            },
                button_color_theme, Cancel));
        }


        private IntPtr CreateTextTexturePointer(string text, FontNameType font_name, int font_size_factor = 4, SDL_Color color = new SDL_Color())
        {
            IntPtr fontToUse = Fonts.GetValueOrDefault(font_name)[font_size_factor];

            IntPtr renderedTextSurface = TTF_RenderUTF8_Blended(fontToUse, text, color);
            if (renderedTextSurface == IntPtr.Zero)
            {
                Console.WriteLine("There was a problem creating textvar pointer");
            }

            IntPtr textTexture = SDL_CreateTextureFromSurface(Program.Renderer, renderedTextSurface);
            if (textTexture == IntPtr.Zero)
            {
                Console.WriteLine("There was a problem creating text image pointer");
            }

            return textTexture;
        }

        private void InitFontsSubroutine()
        {
            int fontCount = Enum.GetValues(typeof(FontNameType)).Length;
            for (int i = 0 ; i < fontCount ; i++)
            {
                Fonts?.Add(((FontNameType)i), new IntPtr[MAX_FONT_SIZE_FACTOR]);

                for (int j = 0 ; j < Fonts?.GetValueOrDefault(((FontNameType)i))?.GetLength(0) ; j++)
                {
                    IntPtr lastFont = TTF_OpenFont($"{(FontNameType)i}.ttf", (int)Math.Pow(2, j));
                    if (lastFont == IntPtr.Zero)
                    {
                        Console.WriteLine("There was a problem loading the font");
                    }

                    IntPtr[]? fontsArray = Fonts?.GetValueOrDefault(((FontNameType)i));
                    if (fontsArray != null) fontsArray[j] = lastFont;
                }
            }
        }

        private void InitTextImagesSubroutine()
        {
            int stringPoolLen = GameStringPool.Data.Count;

            for (int i = 0 ; i < stringPoolLen ; i++)
            {
                FontNameType fontName = GameStringPool.Data[i].Font;
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
                    imageRect = new SDL_Rect { x = 80 + (100 * (i - 4)), y = 200, w = 40, h = 30 };
                }

                Images.Add(lastPictoName, new PackedImage(imagePtr, imageRect));
            }
        }

        public static UserInterface Get()
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

            foreach (Button b in Buttons.Values)
            {
                b.Draw();
            }

            if (DrawPictos == false) return;

            foreach (PackedImage i in Images.Values)
            {
                Program.BlitRect(i.Pointer, i.Rectangle);
            }
        }

        // TEXTVARS

        // Verifies if a new textVar exists and if so, update it to the new text and optional styling.
        internal void UpdateTextVar(TextVarNameType text_var_label_to_update, string new_text, FontNameType new_font_name, int new_font_size_factor = 4, SDL_Color new_color = new SDL_Color())
        {
            if (TextVars.ContainsKey(text_var_label_to_update) == false)
            {
                SDL_LogError(SDL_LOG_CATEGORY_APPLICATION, "You requested to modify a TextVar which does not exist.");
                return;
            }

            TextVars.Remove(text_var_label_to_update);
            SetTextVar(text_var_label_to_update, new_text, new_font_name, new_font_size_factor, new_color);
        }

        internal void SetTextVar(TextVarNameType text_var_label, string text, FontNameType font_name, int font_size_factor = 4, SDL_Color color = new SDL_Color())
        {

            IntPtr texture = CreateTextTexturePointer(text, font_name, font_size_factor, color);

            if (texture == IntPtr.Zero)
            {
                Console.WriteLine("Update to text var failed.");
                return;
            }
            TextVars.Add(text_var_label, texture);
        }
        internal void SetOrUpdateTextVar(TextVarNameType text_var_label_to_update, string new_text, FontNameType new_font_name, int new_font_size_factor = 4, SDL_Color new_color = new SDL_Color())
        {
            TextVars.Remove(text_var_label_to_update);
            SetTextVar(text_var_label_to_update, new_text, new_font_name, new_font_size_factor, new_color);
        }

        private void Select()
        {
            Console.WriteLine("Select!");

            if (Game.Instance is null) return;

            if(Game.Instance.GameState is GameStartState)
            {
                if(ClockRegex.IsMatch(Game.Instance.current_input) == true)
                {
                    DrawPictos = true;
                    Game.Instance.GameState = new GotchiPetViewState();
                }
                else
                {
                    Game.Instance.current_input = "";
                    SetOrUpdateTextVar(TextVarNameType.TimeStart, Game.Instance.current_input, FontNameType.RainyHearts, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });
                }
            }

        }
        private void Execute()
        {
            Console.WriteLine(value: "Execute!");
        }
        private void Cancel()
        {
            Console.WriteLine("Cancel!");
        }
    }    
}
