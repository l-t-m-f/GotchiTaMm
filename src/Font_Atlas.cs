using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_ttf;

namespace GotchiTaMm;

internal class Font_Atlas
    {
        private const int _FONT_ATLAS_SIZE = 1024;

        private readonly Dictionary<
            Font_Name_Type, Dictionary<int, IntPtr>> _sheets = new();

        private readonly
            Dictionary<Font_Name_Type, Dictionary<int, SDL_Rect[]>?>
            _glyphs = new();

        /// <summary>
        /// Make the atlas sheets for all size factors of a given font.
        /// </summary>
        /// <param name="font_name"></param>
        internal void Make_Sheets_For(Font_Name_Type font_name)
            {
                const int GLYPH_COUNT = '~' - ' ' + 1;
                
                // Mapping of sheets and glyphs per size factor
                var sheet_mapping = new Dictionary<int, IntPtr>();
                var glyph_mapping = new Dictionary<int, SDL_Rect[]>();

                for (int size = 0; size < 8; size++)
                    {
                        IntPtr new_sheet_surface =
                            SDL_CreateRGBSurfaceWithFormat(
                                0, _FONT_ATLAS_SIZE, _FONT_ATLAS_SIZE, 32,
                                SDL_PIXELFORMAT_ARGB8888);
                        SDL_Rect dest = default;
                        try
                            {
                                IntPtr font_resource =
                                    Subsystem_UI.Instance.Fonts_Dictionary
                                        .GetValueOrDefault(font_name)![size];
                                var temp_surface_struct =
                                    Marshal.PtrToStructure<SDL_Surface>(
                                        new_sheet_surface);
                                SDL_SetColorKey(new_sheet_surface, 1,
                                    SDL_MapRGBA
                                        (temp_surface_struct.format, 0, 0, 0, 0));
                                dest.x = 0;
                                dest.y = 0;

                                SDL_Rect[] next_glyph_array =
                                    new SDL_Rect[GLYPH_COUNT];

                                for (char c = ' '; c <= '~'; c++)
                                    {
                                        IntPtr intermediate_surface =
                                            TTF_RenderGlyph_Shaded
                                            (font_resource, c, new
                                                    SDL_Color
                                                        {
                                                            r = 255,
                                                            g = 255,
                                                            b = 255,
                                                            a = 255
                                                        },
                                                new
                                                    SDL_Color
                                                        {
                                                            r = 0, g = 0,
                                                            b = 0,
                                                            a = 0
                                                        });
                                        TTF_SizeText(font_resource,
                                            c.ToString(),
                                            out int w, out int h);
                                        dest.w = w;
                                        dest.h = h;

                                        if (dest.x + dest.w >=
                                            _FONT_ATLAS_SIZE)
                                            {
                                                dest.x = 0;
                                                dest.y += dest.h + 1;

                                                if (dest.y + dest.h >=
                                                 _FONT_ATLAS_SIZE)
                                                    {
                                                        SDL_LogError(
                                                            SDL_LOG_CATEGORY_APPLICATION,
                                                            "Font atlas is too small to fit all " +
                                                            "characters!");
                                                    }
                                            }

                                        SDL_BlitSurface(
                                            intermediate_surface,
                                            IntPtr.Zero, new_sheet_surface,
                                            ref dest);

                                        next_glyph_array[c - ' '] = dest;

                                        SDL_FreeSurface(
                                            intermediate_surface);

                                        dest.x += dest.w;
                                    }

                                glyph_mapping.Add(size, next_glyph_array);
                                sheet_mapping.Add(size,
                                    SDL_CreateTextureFromSurface(
                                        Main_App.Renderer,
                                        new_sheet_surface));
                            }
                        finally

                            {
                                SDL_FreeSurface(new_sheet_surface);
                            }
                    }

                this._glyphs.Add(font_name, glyph_mapping);
                this._sheets.Add(font_name, sheet_mapping);
            }

        internal void Draw_With_Sheet(string text, int font_size, int x,
            int y,
            SDL_Color color, Font_Name_Type font_name, int kerning = 0)
            {
                SDL_Rect last_glyph;
                SDL_Rect dest;

                SDL_SetTextureColorMod(this._sheets[font_name][font_size],
                    color.r,
                    color
                        .g, color.b);

                foreach (char c in text)
                    {
                        this._glyphs.TryGetValue(font_name,
                            out Dictionary<int, SDL_Rect[]>? glyphs_map);

                        if (glyphs_map == null)
                            {
                                SDL_LogError(SDL_LOG_CATEGORY_APPLICATION,
                                    "Font atlas is missing glyphs for " +
                                    "font name: " + font_name);
                                break;
                            }

                        SDL_Rect[] glyphs = glyphs_map[font_size];

                        last_glyph = glyphs[c - 32];

                        dest.x = x;
                        dest.y = y;
                        dest.w = last_glyph.w;
                        dest.h = last_glyph.h;

                        SDL_RenderCopy(Main_App.Renderer,
                            this._sheets[font_name][font_size],
                            ref last_glyph,
                            ref dest);

                        x += last_glyph.w + kerning;
                    }
            }
    }