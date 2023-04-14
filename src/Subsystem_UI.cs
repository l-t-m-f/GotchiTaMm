using System.Globalization;
using System.Text.RegularExpressions;
using SDL2;
using static GotchiTaMm.Main_App;
using static GotchiTaMm.Util;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace GotchiTaMm;

internal class Subsystem_Ui
    {
        const int _MAX_FONT_SIZE_FACTOR = 12;

        string _clock_regex_format = @"^([01]\d|2[0-3]):([0-5]\d)$";
        internal Picto_Selection PictoSelection;
        private Regex _clock_regex;

        private bool _can_draw_pictos = false;

        private SDL_Rect _header_rect = new() { x = 0, y = 0, w = WINDOW_W, h = 10 };

        private SDL_Rect _footer_rect = new() { x = 0, y = WINDOW_H - 50, w = WINDOW_W, h = 50 };

        internal readonly Dictionary<Button_Name_Type, Button> Buttons_Dictionary = new();

        private readonly Dictionary<Font_Name_Type, IntPtr[]> _fonts_dictionary = new();

        private readonly Dictionary<int, IntPtr> _texts_dictionary = new();

        internal readonly Dictionary<int, IntPtr> Text_Images_Dictionary = new();

        internal readonly Dictionary<Text_Var_Name_Type, IntPtr> Text_Vars_Dictionary =
            new();

        //Singleton
        private static readonly Lazy<Subsystem_Ui> _Lazy_Instance =
            new(() => new Subsystem_Ui());

        private Subsystem_Ui()
            {
                this.PictoSelection = new Picto_Selection();
                this._clock_regex = new Regex(this._clock_regex_format);
                this.InitButtonsSubroutine();
                this.Init_Fonts_Subroutine();
                this.InitTextImagesSubroutine();
                this.Init_Images_Subroutine();
            }

        public static Subsystem_Ui Instance => _Lazy_Instance.Value;

        //

        internal class Picto_Selection
            {
                internal IntPtr Image;
                internal int Cursor_Index = 0;
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


        private void InitButtonsSubroutine()
            {
                SDL_Color[] button_color_theme = new SDL_Color[]
                    {
                        new SDL_Color { r = 0, g = 255, b = 0, a = 255 },
                        new SDL_Color { r = 255, g = 0, b = 0, a = 255 },
                    };

                this.Buttons_Dictionary.Add(Button_Name_Type.SELECT, new Button(
                    new SDL_Rect
                        {
                            x = ((WINDOW_W / 5) * 1) - 20,
                            y = WINDOW_H - 70,
                            w = 40,
                            h = 40
                        },
                    button_color_theme,
                    Subsystem_Input.Instance.Select_Button_Pressed));

                this.Buttons_Dictionary.Add(Button_Name_Type.EXECUTE, new Button(
                    new SDL_Rect
                        {
                            x = (WINDOW_W / 2) - 20,
                            y = WINDOW_H - 70,
                            w = 40,
                            h = 40
                        },
                    button_color_theme,
                    Subsystem_Input.Instance.Execute_Button_Pressed));

                this.Buttons_Dictionary.Add(Button_Name_Type.CANCEL, new Button(
                    new SDL_Rect
                        {
                            x = ((WINDOW_W / 5) * 4) - 20,
                            y = WINDOW_H - 70,
                            w = 40,
                            h = 40
                        },
                    button_color_theme,
                    Subsystem_Input.Instance.Cancel_Button_Pressed));
            }


        private IntPtr Create_Text_Texture_Pointer(string text,
            Font_Name_Type font_name, int font_size_factor = 4,
            SDL_Color color = new SDL_Color())
            {
                IntPtr font_to_use =
                    this._fonts_dictionary.GetValueOrDefault(font_name)![
                        font_size_factor];

                IntPtr rendered_text_surface =
                    TTF_RenderUTF8_Blended(font_to_use, text, color);
                if (rendered_text_surface == IntPtr.Zero)
                    {
                        Console.WriteLine(
                            "There was a problem creating textvar pointer");
                    }

                IntPtr text_texture =
                    SDL_CreateTextureFromSurface(Renderer,
                        rendered_text_surface);
                if (text_texture == IntPtr.Zero)
                    {
                        Console.WriteLine(
                            "There was a problem creating text image pointer");
                    }

                return text_texture;
            }

        private void Init_Fonts_Subroutine()
            {
                int font_count = Enum.GetValues(typeof(Font_Name_Type)).Length;
                for (int i = 0; i < font_count; i++)
                    {
                        this._fonts_dictionary?.Add(((Font_Name_Type)i),
                            new IntPtr[_MAX_FONT_SIZE_FACTOR]);

                        for (int j = 0;
                             j < this._fonts_dictionary
                                 ?.GetValueOrDefault(((Font_Name_Type)i))
                                 ?.GetLength(0);
                             j++)
                            {
                                IntPtr last_font =
                                    TTF_OpenFont($"fonts/{(Font_Name_Type)i}.ttf",
                                        (int)Math.Pow(2, j));
                                if (last_font == IntPtr.Zero)
                                    {
                                        Console.WriteLine(
                                            "There was a problem loading the font");
                                    }

                                IntPtr[]? fonts_array =
                                    this._fonts_dictionary.GetValueOrDefault(
                                        (Font_Name_Type)i);
                                if (fonts_array != null)
                                    {
                                        fonts_array[j] = last_font;
                                    }
                            }
                    }
            }

        private void InitTextImagesSubroutine()
            {
                int string_pool_len = Game_String_Pool.Data.Count;

                for (int i = 0; i < string_pool_len; i++)
                    {
                        Font_Name_Type font_name = Game_String_Pool.Data[i].Font;
                        int font_size_fac = Game_String_Pool.Data[i].SizeFactor;
                        IntPtr font_to_use =
                            this._fonts_dictionary.GetValueOrDefault(font_name)![
                                font_size_fac];

                        IntPtr test_text = TTF_RenderUTF8_Blended(font_to_use,
                            Game_String_Pool.Data[i].Text,
                            new SDL_Color { r = 55, g = 0, b = 0, a = 255 });
                        if (test_text == IntPtr.Zero)
                            {
                                Console.WriteLine(
                                    "There was a problem creating text pointer");
                            }

                        this._texts_dictionary.Add(i, test_text);

                        IntPtr test_text_image =
                            SDL_CreateTextureFromSurface(Renderer,
                                this._texts_dictionary.GetValueOrDefault(i));
                        if (test_text_image == IntPtr.Zero)
                            {
                                Console.WriteLine(
                                    "There was a problem creating text image pointer");
                            }

                        this.Text_Images_Dictionary.Add(i, test_text_image);
                    }
            }


        private void Init_Images_Subroutine()
            {
                int picto_count = Enum.GetValues(typeof(Picto_Name_Type)).Length;
                for (int i = 0; i < picto_count; i++)
                    {
                        string last_picto_name = ((Picto_Name_Type)i).ToString();
                        IntPtr image_ptr = IMG_LoadTexture(Renderer,
                            $"gfx/{last_picto_name}.png");
                        if (image_ptr == IntPtr.Zero)
                            {
                                Console.WriteLine(
                                    "There was a problem creating image pointer");
                            }

                        SDL_Rect image_rect = i <= 3
                            ? new SDL_Rect
                                    { x = 80 + 100 * i, y = 40, w = 40, h = 30 }
                            : new SDL_Rect
                                {
                                    x = 80 + 100 * (i - 4), y = 200, w = 40,
                                    h = 30
                                };
                        //
                        // this.ImagesDictio.Add(last_picto_name,
                        //     new PackedImage(image_ptr, image_rect));
                    }

                IntPtr selector_ptr =
                    IMG_LoadTexture(Renderer, $"gfx/Selector.png");
                if (selector_ptr == IntPtr.Zero)
                    {
                        Console.WriteLine(
                            "There was a problem creating image pointer");
                    }

                this.PictoSelection.Image = selector_ptr;
            }

        public void Draw()
            {
                SDL_SetRenderDrawColor(Renderer, 0, 0, 0, 255);
                SDL_RenderFillRect(Renderer, ref this._header_rect);
                SDL_RenderFillRect(Renderer, ref this._footer_rect);

                foreach (Button b in this.Buttons_Dictionary.Values)
                    {
                        b.Draw();
                    }

                if (this._can_draw_pictos == false) return;

                // foreach (PackedImage i in this.ImagesDictio.Values)
                //     {
                //         BlitRect(Renderer, i.Pointer, i.Rectangle);
                //     }

                if (this.PictoSelection.Cursor_Index < 0) return;

                BlitRect(Renderer, this.PictoSelection.Image,
                    this.PictoSelection.Selection_Pos_And_Size);
            }

        // TEXTVARS

        // Verifies if a new textVar exists and if so, update it to the new text and optional styling.
        internal void Update_Text_Var(Text_Var_Name_Type text_var_label_to_update,
            string new_text, Font_Name_Type new_font_name,
            int new_font_size_factor = 4, SDL_Color new_color = new SDL_Color())
            {
                if (this.Text_Vars_Dictionary.ContainsKey(text_var_label_to_update) ==
                    false)
                    {
                        SDL_LogError(SDL_LOG_CATEGORY_APPLICATION,
                            "You requested to modify a TextVar which does not exist.");
                        return;
                    }

                this.Text_Vars_Dictionary.Remove(text_var_label_to_update);
                this.Set_Text_Var(text_var_label_to_update, new_text,
                    new_font_name, new_font_size_factor, new_color);
            }

        private void Set_Text_Var(Text_Var_Name_Type text_var_label, string text,
            Font_Name_Type font_name, int font_size_factor = 4,
            SDL_Color color = new SDL_Color())
            {
                IntPtr texture = this.Create_Text_Texture_Pointer(text,
                    font_name, font_size_factor, color);

                if (texture == IntPtr.Zero)
                    {
                        Console.WriteLine("Update to text var failed.");
                        return;
                    }

                this.Text_Vars_Dictionary.Add(text_var_label, texture);
            }

        internal void Set_Or_Update_Text_Var(
            Text_Var_Name_Type text_var_label_to_update, string new_text,
            Font_Name_Type new_font_name, int new_font_size_factor = 4,
            SDL_Color new_color = new SDL_Color())
            {
                this.Text_Vars_Dictionary.Remove(text_var_label_to_update);
                this.Set_Text_Var(text_var_label_to_update, new_text,
                    new_font_name, new_font_size_factor, new_color);
            }

        internal void Try_Input_Time()
            {
                if (this._clock_regex.IsMatch(Subsystem_Input.Instance.App_In))
                    {
                        this._can_draw_pictos = true;

                        TimeSpan actual_offset = DateTime.ParseExact(
                                                    Subsystem_Input.Instance
                                                        .App_In,
                                                    "HH:mm",
                                                    CultureInfo
                                                        .InvariantCulture) -
                                                DateTime.Now;

                        Game.Instance.State = new Game_State_Pet_View();
                        Game.Instance.Clock = new Clock(actual_offset);
                    }
                else
                    {
                        Subsystem_Input.Instance.App_In = "";
                        this.Set_Or_Update_Text_Var(Text_Var_Name_Type.TIME_START,
                            Subsystem_Input.Instance.App_In,
                            Font_Name_Type.RAINY_HEARTS, 6,
                            new SDL_Color
                                    { r = 55, g = 125, b = 125, a = 255 });
                    }
            }


        internal void Meter(Gotchi_Pet pet)
            {
            }
    }

internal enum Button_State_Type
    {
        UNSELECTED = 0,
        SELECTED,
        ACTIVATED,
    }

internal enum Picto_Name_Type
    {
        ATTENTION = 0,
        BATHROOM,
        FOOD,
        GAME,
        LIGHTS,
        MEDICINE,
        STATUS,
        TRAINING,
    }

internal enum Font_Name_Type
    {
        BLUE_SCREEN = 0,
        RAINY_HEARTS,
    }

internal enum Text_Var_Name_Type
    {
        TIME_START = 0,
        CLOCK_TIME,
    }

internal enum Button_Name_Type
    {
        SELECT = 0,
        EXECUTE,
        CANCEL,
    }
