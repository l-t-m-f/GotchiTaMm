#undef DEBUG

using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_image;

namespace GotchiTaMm;

/// <summary>
/// Class to load and store all images in the game.
/// </summary>
public class Subsystem_Imaging
    {
        internal const string DIRECTORY_PATH = "images";
        private const string _SEARCH_PATTERN = "*.png";
        internal Sprite_Atlas Sprite_Atlas { get; set; }

        //Lazy Singleton implementation
        private static readonly Lazy<Subsystem_Imaging> _Lazy_Instance =
            new(() => new Subsystem_Imaging());

        private Subsystem_Imaging()
            {
                this.Sprite_Atlas = new Sprite_Atlas();
            }

        public static Subsystem_Imaging Instance => _Lazy_Instance.Value;

        /// <summary>
        /// Create a Sprite_Atlas object from the images in the images directory.
        /// </summary>
        internal void Make_Atlas()
            {
                string[] file_names =
                    Directory.GetFiles(DIRECTORY_PATH, _SEARCH_PATTERN);
                Dictionary<IntPtr, Atlas_Entry> surface_to_entry_mapping =
                    new Dictionary<IntPtr, Atlas_Entry>();

                foreach (string file_name in file_names)
                    {
#if DEBUG
        Console.WriteLine(file_path);
#endif
                        int last_i = this.Sprite_Atlas.Entries.Count;
                        IntPtr surface = IMG_Load(file_name);
                        this.Sprite_Atlas.Surface_Data[last_i] = surface;
                        Atlas_Entry entry = new Atlas_Entry(file_name);
                        this.Sprite_Atlas.Entries.Add(entry);
                        surface_to_entry_mapping[surface] = entry;
                    }

                Array.Resize(ref this.Sprite_Atlas.Surface_Data,
                    this.Sprite_Atlas.Entries.Count);
                Array.Sort(this.Sprite_Atlas.Surface_Data, this.Sprite_Atlas);

                // Sort the Entries list based on the sorted Surface_Data array
                this.Sprite_Atlas.Entries = this.Sprite_Atlas.Surface_Data.Select(surface => surface_to_entry_mapping[surface]).ToList();

                for (var i = 0; i < this.Sprite_Atlas.Surface_Data.Length; i++)
                    {
                        SDL_QueryTexture(
                            SDL_CreateTextureFromSurface(Main_App.Renderer,
                                this.Sprite_Atlas.Surface_Data[i]),
                            out uint _, out int _, out int w, out int h);

                        var rotated = false;
                        Atlas_Node? found_node = Sprite_Atlas.find_node(
                            this.Sprite_Atlas.First, w,
                            h);

                        if (found_node == null)
                            {
                                rotated = true;
                                found_node =
                                    Sprite_Atlas.find_node(
                                        this.Sprite_Atlas.First,
                                        h, w);
                            }

                        if (found_node != null)
                            {
#if DEBUG
                                Console.WriteLine($"Node found for image #{i}");
#endif
                                if (rotated)
                                    {
                                        found_node.Height = w;
                                        found_node.Width = h;
                                    }

                                var dest = new SDL_Rect
                                    {
                                        x = found_node.X,
                                        y = found_node.Y,
                                        w = w,
                                        h = h
                                    };

                                this.Sprite_Atlas.Entries[i] =
                                    new Atlas_Entry(
                                        this.Sprite_Atlas.Entries[i]
                                            .Filename,
                                        dest, rotated);

                                if (rotated == false)
                                    {
                                        SDL_BlitSurface(
                                            this.Sprite_Atlas.Surface_Data[i],
                                            IntPtr.Zero,
                                            this.Sprite_Atlas.Master_Surface,
                                            ref dest);
                                    }
                                else
                                    {
                                        IntPtr result =
                                            Rotate_Surface(this.Sprite_Atlas
                                                .Surface_Data[i]);
                                        if (result != IntPtr.Zero)
                                            {
                                                SDL_BlitSurface(result,
                                                    IntPtr.Zero,
                                                    this.Sprite_Atlas
                                                        .Master_Surface,
                                                    ref dest);
                                            }
                                    }
                            }

                        SDL_FreeSurface(this.Sprite_Atlas.Surface_Data[i]);
                    }
            }

        /// <summary>
        /// Rotates a surface 90 degrees clockwise using its pointer.
        /// </summary>
        /// <param name="source_surface_ptr"></param>
        /// <returns></returns>
        private static IntPtr Rotate_Surface(IntPtr source_surface_ptr)
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
                                uint pixel = Get_Pixel(source_surface_ptr, x,
                                    y);

#if DEBUG
                                (byte r, byte g, byte b, byte a) =
                                    get_pixel_color_values(source_surface_ptr,
                                        x, y);
                                Console.WriteLine(
                                    $"Source Pixel ({x}, {y}): R: {r} G: {g} B: {b} A: {a}");
#endif
                                int dest_x = source_surface.h - y - 1;
                                Set_Pixel(dest_surface_ptr, dest_x, x, pixel);

#if DEBUG
                                (byte dest_r, byte dest_g, byte dest_b,
                                        byte dest_a)
                                    =
                                    Get_Pixel_Color_Values(dest_surface_ptr, dest_x, x);
                                Console.WriteLine(
                                    $"Dest Pixel ({dest_x}, {x}): R: {dest_r} G: {dest_g} B: {dest_b} A: {dest_a}");
#endif
                            }
                    }

                SDL_UnlockSurface(source_surface_ptr);
                SDL_UnlockSurface(dest_surface_ptr);

                return dest_surface_ptr;
            }

        private static uint Get_Pixel(IntPtr surface_ptr, int x, int y)
            {
                var surface =
                    Marshal.PtrToStructure<SDL_Surface>(surface_ptr);
                const int BYTES_PER_PIXEL = 4; //ARGB8888
                IntPtr pixel = surface.pixels + y * surface.pitch +
                               x * BYTES_PER_PIXEL;
                return (uint)Marshal.ReadInt32(pixel);
            }

        private static void Set_Pixel(IntPtr surface_ptr, int x, int y,
            uint new_pixel)
            {
                var surface =
                    Marshal.PtrToStructure<SDL_Surface>(surface_ptr);
                const int BYTES_PER_PIXEL = 4; //ARGB8888
                IntPtr pixel = surface.pixels + y * surface.pitch +
                               x * BYTES_PER_PIXEL;

                Marshal.WriteInt32(pixel, (int)new_pixel);
            }

        private static (byte r, byte g, byte b, byte a) Get_Pixel_Color_Values(
            IntPtr surface_ptr, int x, int y)
            {
                uint pixel = Get_Pixel(surface_ptr, x, y);
                var r = (byte)(pixel & 0xFF);
                var g = (byte)((pixel >> 8) & 0xFF);
                var b = (byte)((pixel >> 16) & 0xFF);
                var a = (byte)((pixel >> 24) & 0xFF);
                return (r, g, b, a);
            }
    }