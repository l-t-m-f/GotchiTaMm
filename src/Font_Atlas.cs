using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_ttf;

namespace GotchiTaMm;

internal class Font_Atlas
    {
        private const int _FONT_ATLAS_SIZE = 256;

        internal Dictionary<Font_Name_Type, IntPtr> Sheets = new();
        internal Dictionary<Font_Name_Type, SDL_Rect[]?> Glyphs = new();

        internal Font_Atlas()
            {
            }

        internal void Make_Sheet_For(Font_Name_Type font_name,
            int font_size = 4)
            {
                IntPtr new_sheet_surface = SDL_CreateRGBSurfaceWithFormat(
                    0, _FONT_ATLAS_SIZE, _FONT_ATLAS_SIZE, 32,
                    SDL_PIXELFORMAT_ARGB8888);
                SDL_Rect dest = default;
                try
                    {
                        IntPtr font_resource =
                            Subsystem_UI.Instance.Fonts_Dictionary
                                .GetValueOrDefault
                                    (font_name)![
                                    font_size];
                        SDL_Surface temp_surface_struct = Marshal
                            .PtrToStructure<SDL_Surface>(new_sheet_surface);
                        SDL_SetColorKey(new_sheet_surface, 1, SDL_MapRGBA
                            (temp_surface_struct.format, 0, 0, 0, 0));
                        dest.x = 0;
                        dest.y = 0;

                        int glyph_count = '~' - ' ' + 1;
                        SDL_Rect[] glyph_array = new SDL_Rect[glyph_count];

                        for (char c = ' '; c <= '~'; c++)
                            {
                                string char_as_str = c.ToString();

                                IntPtr intermediate_surface =
                                    TTF_RenderGlyph_Shaded
                                    (font_resource, c, new
                                        SDL_Color
                                            {
                                                r = 255, g = 255, b = 255,
                                                a = 255
                                            },
                                        new
                                            SDL_Color
                                                {
                                                    r = 0, g = 0, b = 0,
                                                    a = 0
                                                });
                                TTF_GlyphMetrics(font_resource, c, out int minx, out int maxx, out int miny, out int maxy, out int advance);
                                dest.w = advance;
                                dest.h = maxy - miny;

                                if (dest.x + dest.w >= _FONT_ATLAS_SIZE)
                                    {
                                        dest.x = 0;
                                        dest.y += dest.h + 6;

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

                                glyph_array[c - ' '] = dest;

                                SDL_FreeSurface(intermediate_surface);
                                
                                dest.x += dest.w;
                            }

                        this.Glyphs[font_name] = glyph_array;

                        Sheets.Add(font_name, SDL_CreateTextureFromSurface
                            (Main_App.Renderer, new_sheet_surface));
                    }
                finally

                    {
                        SDL_FreeSurface(new_sheet_surface);
                    }
            }

        internal void Draw_With_Sheet(string text, int x, int y,
            SDL_Color color, Font_Name_Type font_name, int kerning = 0)
            {
                SDL_Rect last_glyph;
                SDL_Rect dest;

                SDL_SetTextureColorMod(this.Sheets[font_name], color.r,
                    color
                        .g, color.b);

                foreach (char c in text)
                    {
                        this.Glyphs.TryGetValue(font_name,
                            out SDL_Rect[]? glyphs);
                        if (glyphs == null)
                            {
                                SDL_LogError(SDL_LOG_CATEGORY_APPLICATION,
                                    "Font atlas is missing glyphs for " +
                                    "font name: " + font_name);
                                break;
                            }

                        last_glyph = glyphs[c - 32];

                        dest.x = x;
                        dest.y = y;
                        dest.w = last_glyph.w;
                        dest.h = last_glyph.h;

                        SDL_RenderCopy(Main_App.Renderer,
                            this.Sheets[font_name], ref last_glyph,
                            ref dest);

                        x += last_glyph.w;
                    }
            }
    }