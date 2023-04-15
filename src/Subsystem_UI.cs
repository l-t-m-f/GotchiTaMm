using System.Globalization;
using System.Text.RegularExpressions;
using static GotchiTaMm.Main_App;
using static GotchiTaMm.Util;
using static SDL2.SDL;
using static SDL2.SDL_ttf;

namespace GotchiTaMm;

// ReSharper disable once InconsistentNaming
internal class Subsystem_UI
    {
        private const int _MAX_FONT_SIZE_FACTOR = 8;

        private const string _CLOCK_REGEX_FORMAT =
            @"^([01]\d|2[0-3]):([0-5]\d)$";

        internal Picto_Selection Picto_Selection { get; init; }
        private Regex Clock_Regex { get; init; }
        private bool Can_Draw_Pictos { get; set; }

        internal readonly Dictionary<Button_Name_Type, Button>
            Buttons_Dictionary = new();

        internal readonly Dictionary<Font_Name_Type, IntPtr[]>
            Fonts_Dictionary =
                new();

        private readonly Dictionary<int, IntPtr> _texts_dictionary = new();

        internal readonly Dictionary<int, IntPtr>
            Text_Images_Dictionary = new();

        internal readonly Dictionary<Text_Var_Name_Type, IntPtr>
            Text_Vars_Dictionary =
                new();
        //Singleton
        private static readonly Lazy<Subsystem_UI> _Lazy_Instance =
            new(() => new Subsystem_UI());

        private Subsystem_UI()
            {
                this.Picto_Selection = new Picto_Selection();
                this.Clock_Regex = new Regex(_CLOCK_REGEX_FORMAT);
            }

        public static Subsystem_UI Instance => _Lazy_Instance.Value;
        
        internal void Init()
            {
                this.Init_Buttons_Subroutine();
                this.Init_Fonts_Subroutine();
                this.Init_Text_Images_Subroutine();
            }

        private void Init_Buttons_Subroutine()
            {
                this.Buttons_Dictionary.Add(Button_Name_Type.SELECT, new
                    Button(Art_Style_Type.SINGLE_SPRITE, "Button_B_silent",
                        Subsystem_Input.Instance.Select_Button_Pressed,
                        new SDL_Point
                            {
                                x = WINDOW_W / 5 * 1, 
                                y = 80
                            }));

                this.Buttons_Dictionary.Add(Button_Name_Type.EXECUTE, new
                    Button(Art_Style_Type.SINGLE_SPRITE, "Button_Y_silent",
                        Subsystem_Input.Instance.Select_Button_Pressed,
                        new SDL_Point
                            {
                                x = WINDOW_W / 2, 
                                y = 80
                            }));

                this.Buttons_Dictionary.Add(Button_Name_Type.CANCEL, new
                    Button(Art_Style_Type.SINGLE_SPRITE, "Button_R_silent",
                        Subsystem_Input.Instance.Select_Button_Pressed,
                        new SDL_Point
                            {
                                x = WINDOW_W / 5 * 4,
                                y = 80
                            }));
            }


        private void Init_Fonts_Subroutine()
            {
                /*
                 * Loops for each enum value in Font_Name_Type
                 * Each value should match a font file in the fonts folder
                 * Enum name is ALL_CAPPED, while font file name is defined in
                 * the Enums.cs file as part of Font_Name_Type's extensions
                 */
                foreach (Font_Name_Type font_name_type in Enum.GetValues(
                             typeof(Font_Name_Type)))
                    {
                        /*
                         * Array of pointers to fonts of different sizes
                         * min size if 2^2, max size is 2^10
                         */
                        var next_font_array = new IntPtr[_MAX_FONT_SIZE_FACTOR];

                        /*
                         * Load fonts in the array one by one
                         */
                        for (int j = 0; j < next_font_array.Length; j++)
                            {
                                IntPtr last_font = TTF_OpenFont(
                                    $"fonts/{font_name_type.To_Friendly_String()}.ttf",
                                    (int)Math.Pow(2, j));
                                if (last_font == IntPtr.Zero)
                                    {
                                        SDL_LogError(
                                            SDL_LOG_CATEGORY_APPLICATION,
                                            $"There was a problem loading the font!\n {SDL_GetError()}");
                                    }

                                next_font_array[j] = last_font;
                            }
                        
                        /*
                         * Add the array to the dictionary
                         */
                        this.Fonts_Dictionary.Add(font_name_type,
                            next_font_array);
                    }
            }


        private void Init_Text_Images_Subroutine()
            {
                int string_pool_len = Game_String_Pool.Data.Count;

                for (int i = 0; i < string_pool_len; i++)
                    {
                        Font_Name_Type font_name =
                            Game_String_Pool.Data[i].Font;
                        int font_size_fac = Game_String_Pool.Data[i].Size_Factor;
                        IntPtr font_to_use =
                            this.Fonts_Dictionary.GetValueOrDefault(font_name)![
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

        public void Draw()
            {
                
                foreach (Button b in this.Buttons_Dictionary.Values)
                    {
                        b.Draw();
                    }

                // if (this.Can_Draw_Pictos == false)
                //     {
                //         return;
                //     }
                //
                // if (this.Picto_Selection.Cursor_Index < 0)
                //     {
                //     }

                switch (Game.Instance.State)
                    {
                        case Game_State_Start:
                            { this.DrawPictos();
                                break;
                            }
                        case Game_State_Pet_View:
                            {
                                this.DrawPictos();
                                break;
                            }
                    }

                SDL_Rect mouse_rect = Subsystem_Imaging.Instance
                                .Sprite_Atlas
                                .Get_Atlas_Image_Rect
                                    ("Cursor");
                            mouse_rect.x = Subsystem_Input.Instance.Mouse
                                .Position.x;
                            mouse_rect.y = Subsystem_Input.Instance.Mouse
                                .Position.y;
                            mouse_rect.w =
                                (int)(mouse_rect.w * SCREEN_RATIO);
                            mouse_rect.h =
                                (int)(mouse_rect.h * SCREEN_RATIO);
                            SDL_RenderCopy(Renderer, Subsystem_Imaging
                                    .Instance.Sprite_Atlas
                                    .Get_Atlas_Image("Cursor"),
                                IntPtr.Zero,
                                ref mouse_rect);

                        // BlitRect(Renderer, this.Picto_Selection.Image,
                //     this.Picto_Selection.Selection_Pos_And_Size);
            }

        private void DrawPictos()
            {
                for (int row = 0; row < 2; row++)
                    {
                        for (int col = 0; col < 4; col++)
                            {
                                SDL_Rect picto_bg_dest =
                                    Subsystem_Imaging.Instance
                                        .Sprite_Atlas.Get_Atlas_Image_Rect
                                            ("ButtonBg");
                                
                                
                                picto_bg_dest.w = (int)(picto_bg_dest.w * SCREEN_RATIO);
                                picto_bg_dest.h = (int)(picto_bg_dest.h * SCREEN_RATIO);
                                picto_bg_dest.x =
                                    (int)(WINDOW_W/5 * col * 
                                          SCREEN_RATIO) + (int)
                                          (picto_bg_dest.w
                                        * 1.25);
                                picto_bg_dest.y = row == 0
                                    ? (int)(2 *
                                            SCREEN_RATIO)
                                    : (int)(40 *
                                            SCREEN_RATIO);
                                
                                SDL_Rect picto_dest =
                                    Subsystem_Imaging.Instance
                                        .Sprite_Atlas.Get_Atlas_Image_Rect
                                            ($"Picto{1+(row * 4 + col)}");
                                
                                picto_dest.w = (int)(picto_dest.w * SCREEN_RATIO);
                                picto_dest.h = (int)(picto_dest.h * SCREEN_RATIO);
                                picto_dest.x =
                                    (int)(WINDOW_W/5 * col * 
                                          SCREEN_RATIO) + picto_dest.w / 3;
                                picto_dest.y = row == 0
                                    ? (int)(2 *
                                            SCREEN_RATIO)
                                    : (int)(40 *
                                            SCREEN_RATIO);
                                SDL_RenderCopy(Renderer,
                                    Subsystem_Imaging.Instance.Sprite_Atlas
                                        .Get_Atlas_Image(
                                            "ButtonBg")
                                    , IntPtr.Zero, ref picto_bg_dest);
                                SDL_RenderCopy(Renderer,
                                    Subsystem_Imaging.Instance.Sprite_Atlas
                                        .Get_Atlas_Image(
                                            $"Picto{1+(row * 4 + col)}")
                                    , IntPtr.Zero, ref picto_dest);
                            }
                    }
            }

        internal void Update_Text_Var(
            Text_Var_Name_Type text_var_label_to_update,
            string new_text, Font_Name_Type new_font_name,
            int new_font_size_factor = 4, SDL_Color new_color = new SDL_Color())
            {
                if (this.Text_Vars_Dictionary.ContainsKey(
                        text_var_label_to_update) ==
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

        private void Set_Text_Var(Text_Var_Name_Type text_var_label,
            string text,
            Font_Name_Type font_name, int font_size_factor = 4,
            SDL_Color color = new SDL_Color())
            {
                IntPtr texture = Subsystem_Imaging.Instance.Create_Text_Texture_Pointer(text,
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
                if (this.Clock_Regex.IsMatch(Subsystem_Input.Instance.App_In))
                    {
                        this.Can_Draw_Pictos = true;

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
                        this.Set_Or_Update_Text_Var(
                            Text_Var_Name_Type.TIME_START,
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
