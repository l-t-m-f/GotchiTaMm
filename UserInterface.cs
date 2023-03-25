using System.Text.RegularExpressions;
using static GotchiTaMm.Program;
using static GotchiTaMm.Util;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace GotchiTaMm
{
    internal class UserInterface
    {
        const int MAX_FONT_SIZE_FACTOR = 12;

        string clockRegexFormat = @"^([01]\d|2[0-3]):([0-5]\d)$";
        internal PictoSelection pictoSelection;
        private Regex clockRegex;
        internal bool canDrawPictos = false;
        internal SDL_Rect headerRect = new SDL_Rect { x = 0, y = 0, w = WINDOW_W, h = 10 };
        internal SDL_Rect footerRect = new SDL_Rect { x = 0, y = WINDOW_H - 50, w = WINDOW_W, h = 50 };
        internal Dictionary<ButtonNameType, Button> buttonsDictio = new Dictionary<ButtonNameType, Button>();
        internal Dictionary<string, PackedImage> imagesDictio = new Dictionary<string, PackedImage>();
        internal Dictionary<FontNameType, IntPtr[]> fontsDictio = new Dictionary<FontNameType, IntPtr[]>();
        internal Dictionary<int, IntPtr> textsDictio = new Dictionary<int, IntPtr>();
        internal Dictionary<int, IntPtr> textImagesDictio = new Dictionary<int, IntPtr>();
        internal Dictionary<TextVarNameType, IntPtr> textVarsDictio = new Dictionary<TextVarNameType, IntPtr>();

        //Singleton
        internal static readonly Lazy<UserInterface> lazyInstance = new Lazy<UserInterface>(() => new UserInterface());
        private UserInterface()
        {
            pictoSelection = new PictoSelection();
            clockRegex = new Regex(clockRegexFormat);
            InitButtonsSubroutine();
            InitFontsSubroutine();
            InitTextImagesSubroutine();
            InitImagesSubroutine();
        }

        public static UserInterface Instance {
            get {
                return lazyInstance.Value;
            }
        }

        //

        internal class PictoSelection
        {
            internal IntPtr image;
            internal int cursorIndex = -1;
            internal SDL_Rect selectionPosAndSize;

            internal void SelectNext()
            {
                if (cursorIndex == 7)
                {
                    cursorIndex = 0;
                }
                else
                    cursorIndex++;

                selectionPosAndSize = Instance.imagesDictio.GetValueOrDefault(((PictoNameType)cursorIndex).ToString()).Rectangle;
                selectionPosAndSize.x -= 5;
                selectionPosAndSize.y -= 5;
                selectionPosAndSize.w += 5;
                selectionPosAndSize.h += 5;

            }

            internal void ClearSelect()
            {
                cursorIndex = -1;
            }
        }


        private void InitButtonsSubroutine()
        {
            SDL_Color[] button_color_theme = new SDL_Color[] {
                    new SDL_Color { r = 0, g = 255, b = 0, a = 255 },
                    new SDL_Color { r = 255, g = 0, b = 0, a = 255 },
                };

            buttonsDictio.Add(ButtonNameType.Select, new Button(new SDL_Rect {
                x = ((WINDOW_W / 5) * 1) - 20,
                y = WINDOW_H - 70,
                w = 40,
                h = 40
            },
                button_color_theme, InputSystem.Instance.SelectButtonPressed));

            buttonsDictio.Add(ButtonNameType.Execute, new Button(new SDL_Rect {
                x = (WINDOW_W / 2) - 20,
                y = WINDOW_H - 70,
                w = 40,
                h = 40
            },
                button_color_theme, InputSystem.Instance.ExecuteButtonPressed));

            buttonsDictio.Add(ButtonNameType.Cancel, new Button(new SDL_Rect {
                x = ((WINDOW_W / 5) * 4) - 20,
                y = WINDOW_H - 70,
                w = 40,
                h = 40
            },
                button_color_theme, InputSystem.Instance.CancelButtonPressed));
        }


        private IntPtr CreateTextTexturePointer(string text, FontNameType font_name, int font_size_factor = 4, SDL_Color color = new SDL_Color())
        {
            IntPtr fontToUse = fontsDictio.GetValueOrDefault(font_name)[font_size_factor];

            IntPtr renderedTextSurface = TTF_RenderUTF8_Blended(fontToUse, text, color);
            if (renderedTextSurface == IntPtr.Zero)
            {
                Console.WriteLine("There was a problem creating textvar pointer");
            }

            IntPtr textTexture = SDL_CreateTextureFromSurface(Renderer, renderedTextSurface);
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
                fontsDictio?.Add(((FontNameType)i), new IntPtr[MAX_FONT_SIZE_FACTOR]);

                for (int j = 0 ; j < fontsDictio?.GetValueOrDefault(((FontNameType)i))?.GetLength(0) ; j++)
                {
                    IntPtr lastFont = TTF_OpenFont($"fonts/{(FontNameType)i}.ttf", (int)Math.Pow(2, j));
                    if (lastFont == IntPtr.Zero)
                    {
                        Console.WriteLine("There was a problem loading the font");
                    }

                    IntPtr[]? fontsArray = fontsDictio?.GetValueOrDefault(((FontNameType)i));
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
                IntPtr fontToUse = fontsDictio.GetValueOrDefault(fontName)[fontSizeFac];

                IntPtr testText = TTF_RenderUTF8_Blended(fontToUse, GameStringPool.Data[i].Text, new SDL_Color { r = 55, g = 0, b = 0, a = 255 });
                if (testText == IntPtr.Zero)
                {
                    Console.WriteLine("There was a problem creating text pointer");
                }

                textsDictio.Add(i, testText);

                IntPtr testTextImage = SDL_CreateTextureFromSurface(Program.Renderer, textsDictio.GetValueOrDefault(i));
                if (testTextImage == IntPtr.Zero)
                {
                    Console.WriteLine("There was a problem creating text image pointer");
                }
                textImagesDictio.Add(i, testTextImage);
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

                imagesDictio.Add(lastPictoName, new PackedImage(imagePtr, imageRect));
            }

            IntPtr selectorPtr = IMG_LoadTexture(Renderer, $"gfx/Selector.png");
            if (selectorPtr == IntPtr.Zero)
            {
                Console.WriteLine("There was a problem creating image pointer");
            }

            pictoSelection.image = selectorPtr;

        }

        public void Draw()
        {
            SDL_SetRenderDrawColor(Renderer, 0, 0, 0, 255);
            SDL_RenderFillRect(Renderer, ref headerRect);
            SDL_RenderFillRect(Renderer, ref footerRect);

            foreach (Button b in buttonsDictio.Values)
            {
                b.Draw();
            }

            if (canDrawPictos == false) return;

            foreach (PackedImage i in imagesDictio.Values)
            {
                BlitRect(Renderer, i.Pointer, i.Rectangle);
            }

            if (pictoSelection.cursorIndex < 0) return;

            BlitRect(Renderer, pictoSelection.image, pictoSelection.selectionPosAndSize);
        }

        // TEXTVARS

        // Verifies if a new textVar exists and if so, update it to the new text and optional styling.
        internal void UpdateTextVar(TextVarNameType text_var_label_to_update, string new_text, FontNameType new_font_name, int new_font_size_factor = 4, SDL_Color new_color = new SDL_Color())
        {
            if (textVarsDictio.ContainsKey(text_var_label_to_update) == false)
            {
                SDL_LogError(SDL_LOG_CATEGORY_APPLICATION, "You requested to modify a TextVar which does not exist.");
                return;
            }

            textVarsDictio.Remove(text_var_label_to_update);
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
            textVarsDictio.Add(text_var_label, texture);
        }
        internal void SetOrUpdateTextVar(TextVarNameType text_var_label_to_update, string new_text, FontNameType new_font_name, int new_font_size_factor = 4, SDL_Color new_color = new SDL_Color())
        {
            textVarsDictio.Remove(text_var_label_to_update);
            SetTextVar(text_var_label_to_update, new_text, new_font_name, new_font_size_factor, new_color);
        }

        internal void TryInputTime()
        {
            if (clockRegex.IsMatch(InputSystem.Instance.appIn) == true)
            {
                canDrawPictos = true;
                Game.Instance.state = new GotchiPetViewState();
                Game.Instance.clock = new Clock(
                    int.Parse(InputSystem.Instance.appIn.Substring(0, 2)), 
                    int.Parse(InputSystem.Instance.appIn.Substring(3, 2)), 
                    0);
            }
            else
            {
                InputSystem.Instance.appIn = "";
                SetOrUpdateTextVar(TextVarNameType.TimeStart, InputSystem.Instance.appIn, FontNameType.RainyHearts, 6, new SDL_Color { r = 55, g = 125, b = 125, a = 255 });
            }
        }

    }
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
        ClockTime,

    }

    internal enum ButtonNameType
    {
        Select = 0,
        Execute,
        Cancel,
    }

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

}
