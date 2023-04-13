#undef DEBUG

using static GotchiTaMm.Main_App;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_image;

namespace GotchiTaMm;

internal class Subsystem_Imaging
    {
        internal const string DIRECTORY_PATH = "images";
        private const string _SEARCH_PATTERN = "*.png";
        private Sprite_Atlas Sprite_Atlas { get; set; }

        //Singleton
        private static readonly Lazy<Subsystem_Imaging> _LAZY_INSTANCE = 
            new Lazy<Subsystem_Imaging>(() => new Subsystem_Imaging());
        private Subsystem_Imaging()
            {
                this.Sprite_Atlas = new Sprite_Atlas();
            }

        public static Subsystem_Imaging instance => _LAZY_INSTANCE.Value;

        internal void Make_Atlas()
            {
                // Open the directory
                string[] file_names =
                    Directory.GetFiles(DIRECTORY_PATH, _SEARCH_PATTERN);

                foreach (string file_name in file_names)
                    {
#if DEBUG
                        Console.WriteLine(file_path);
#endif
                        string file_path = Path.GetRelativePath(".", file_name);
                        this.Sprite_Atlas.surface_data[
                                this.Sprite_Atlas.entries.Count] =
                            IMG_Load(file_path);
                        this.Sprite_Atlas.entries.Add(new AtlasEntry(file_path,
                            new SDL_Rect
                                    { x = 0, y = 0, w = 0, h = 0 }, false));
                    }

                Array.Resize(ref this.Sprite_Atlas.surface_data,
                    this.Sprite_Atlas.entries.Count);
                Array.Sort(this.Sprite_Atlas.surface_data, this.Sprite_Atlas);


                for (var i = 0; i < this.Sprite_Atlas.surface_data.Length; i++)
                    {
                        SDL_QueryTexture(
                            SDL_CreateTextureFromSurface(Main_App.Renderer,
                                this.Sprite_Atlas.surface_data[i]), 
                            out uint _, out int _, out int w, out int h);

                        var rotated = false;
                        AtlasNode? found_node = Sprite_Atlas.find_node(
                            this.Sprite_Atlas.first, w,
                            h);

                        if (found_node == null)
                            {
                                rotated = true;
                                found_node =
                                    Sprite_Atlas.find_node(
                                        this.Sprite_Atlas.first,
                                        h, w);
                            }

                        if (found_node != null)
                            {
#if DEBUG
                                Console.WriteLine($"Node found for image #{i}");
#endif
                                if (rotated)
                                    {
                                        found_node.height = w;
                                        found_node.width = h;
                                    }

                                var dest = new SDL_Rect
                                    {
                                        x = found_node.x,
                                        y = found_node.y,
                                        w = found_node.width,
                                        h = found_node.height
                                    };

                                this.Sprite_Atlas.entries[i] =
                                    new AtlasEntry(
                                        this.Sprite_Atlas.entries[i].filename,
                                        dest, rotated);

                                if (rotated == false)
                                    {
                                        SDL_BlitSurface(
                                            this.Sprite_Atlas.surface_data[i],
                                            IntPtr.Zero,
                                            this.Sprite_Atlas.master_surface,
                                            ref dest);
                                    }
                                else
                                    {
                                        IntPtr result =
                                            blit_rotated(this.Sprite_Atlas.surface_data[i]);
                                        if (result != IntPtr.Zero)
                                            {
                                                SDL_BlitSurface(result,
                                                    IntPtr.Zero, this.Sprite_Atlas.master_surface,
                                                    ref dest);
                                            }
                                    }
                            }

                        SDL_FreeSurface(this.Sprite_Atlas.surface_data[i]);
                    }
            }

        private static IntPtr blit_rotated(IntPtr source_surface_ptr)
            {
                var source_surface =
                    Marshal.PtrToStructure<SDL_Surface>(source_surface_ptr);

#if DEBUG
                Console.WriteLine(
                    $"Source surface dimensions: {source_surface.w}x{source_surface.h}");
#endif

                IntPtr dest_surface_ptr = SDL_CreateRGBSurfaceWithFormat(0,
                    source_surface.h, source_surface.w, 32,
                    SDL_PIXELFORMAT_ARGB8888);

                SDL_LockSurface(source_surface_ptr);
                SDL_LockSurface(dest_surface_ptr);

                for (var y = 0; y < source_surface.h; y++)
                    {
                        for (var x = 0; x < source_surface.w; x++)
                            {
                                uint pixel = get_pixel(source_surface_ptr, x,
                                    y);

#if DEBUG
                                (byte r, byte g, byte b, byte a) =
                                    get_pixel_color_values(source_surface_ptr,
                                        x, y);
                                Console.WriteLine(
                                    $"Source Pixel ({x}, {y}): R: {r} G: {g} B: {b} A: {a}");
#endif
                                int dest_x = source_surface.h - y - 1;
                                set_pixel(dest_surface_ptr, dest_x, x, pixel);

#if DEBUG
                                (byte dest_r, byte dest_g, byte dest_b,
                                        byte dest_a)
                                    =
                                    get_pixel_color_values(dest_surface_ptr, dest_x, x);
                                Console.WriteLine(
                                    $"Dest Pixel ({dest_x}, {x}): R: {dest_r} G: {dest_g} B: {dest_b} A: {dest_a}");
#endif
                            }
                    }

                SDL_UnlockSurface(source_surface_ptr);
                SDL_UnlockSurface(dest_surface_ptr);

                return dest_surface_ptr;
            }

        private static uint get_pixel(IntPtr surface_ptr, int x, int y)
            {
                var surface =
                    Marshal.PtrToStructure<SDL_Surface>(surface_ptr);
                const int bytes_per_pixel = 4; //ARGB8888
                IntPtr pixel = surface.pixels + y * surface.pitch +
                               x * bytes_per_pixel;
                return (uint)Marshal.ReadInt32(pixel);
            }

        private static void set_pixel(IntPtr surface_ptr, int x, int y,
            uint new_pixel)
            {
                var surface =
                    Marshal.PtrToStructure<SDL_Surface>(surface_ptr);
                const int bytes_per_pixel = 4; //ARGB8888
                IntPtr pixel = surface.pixels + y * surface.pitch +
                               x * bytes_per_pixel;

                Marshal.WriteInt32(pixel, (int)new_pixel);
            }

        private static (byte r, byte g, byte b, byte a) get_pixel_color_values(
            IntPtr surface_ptr, int x, int y)
            {
                uint pixel = get_pixel(surface_ptr, x, y);
                var r = (byte)(pixel & 0xFF);
                var g = (byte)((pixel >> 8) & 0xFF);
                var b = (byte)((pixel >> 16) & 0xFF);
                var a = (byte)((pixel >> 24) & 0xFF);
                return (r, g, b, a);
            }
    }