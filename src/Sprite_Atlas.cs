using static SDL2.SDL;

namespace GotchiTaMm;

internal class Sprite_Atlas : IComparer<IntPtr>
    {
        internal const int ATLAS_SIZE = 700;
        private const int _PADDING = 4;
        public List<Atlas_Entry> Entries { get; set; }
        internal IntPtr[] Surface_Data = new IntPtr[50]; // surfaces
        public readonly Atlas_Node First;
        private readonly LinkedList<Atlas_Node> _nodes = new();
        internal readonly IntPtr Master_Surface;

        public Sprite_Atlas()
            {
                this.Entries = new List<Atlas_Entry>();
                var root_node = new Atlas_Node(0, 0, ATLAS_SIZE, ATLAS_SIZE);
                this._nodes.AddLast(root_node);
                this.Master_Surface = SDL_CreateRGBSurfaceWithFormat(0,
                    ATLAS_SIZE,
                    ATLAS_SIZE,
                    32,
                    SDL_PIXELFORMAT_ARGB8888); // used to be RGBA8888
                this.First = root_node;
            }

        // IComparer Interface
        /// <summary>
        /// This implementation of IComparer is used to sort the images files according
        /// to their height.
        /// </summary>
        /// <param name="surface_a"></param>
        /// <param name="surface_b"></param>
        /// <returns></returns>
        public int Compare(IntPtr surface_a, IntPtr surface_b)
            {
                if (SDL_QueryTexture(
                        SDL_CreateTextureFromSurface(Main_App.Renderer,
                            surface_a),
                        out uint _,
                        out int _, out int height_x, out int _) < 0)
                    {
                        SDL_LogError(SDL_LOG_CATEGORY_APPLICATION,
                            $"There was an issue querying the texture \n{SDL_GetError()}");
                        return 0;
                    }

                if (SDL_QueryTexture(
                        SDL_CreateTextureFromSurface(Main_App.Renderer,
                            surface_b),
                        out uint _,
                        out int _, out int height_y, out int _) >= 0)
                    {
                        return height_y.CompareTo(height_x);
                    }

                SDL_LogError(SDL_LOG_CATEGORY_APPLICATION,
                    $"There was an issue querying the texture \n{SDL_GetError()}");
                return 0;

                // Descending order
            }

        // Atlas Tree Methods

        /// <summary>
        /// Recover a SDL_Texture pointer based on the filename. For this to
        /// work, the image must have been added to the atlas first. If the
        /// image is within the DIRECTORY_PATH, then the filename should be
        /// valid. The filename allows us to recover an AtlasEntry which has
        /// the SDL_Rect information required to extract the image from the
        /// master surface.
        /// </summary>
        /// <param name="short_filename"></param>
        /// <returns></returns>
        public IntPtr Get_Atlas_Image(string short_filename)
            {
                var full_filename =
                    $"{Subsystem_Imaging.DIRECTORY_PATH}\\{short_filename}.png";
                Atlas_Entry? entry = null;
                foreach (var t in this.Entries)
                    {
                        if (t.Filename != full_filename) continue;
                        entry = t;
                        break;
                    }

                if (entry is null)
                    {
                        return 0;
                    }

                SDL_Rect extraction_rectangle = entry.Rectangle;


                // Create a new surface to hold the extracted image
                IntPtr extracted_surface = SDL_CreateRGBSurfaceWithFormat(0,
                    extraction_rectangle.w, extraction_rectangle.h, 32,
                    SDL_PIXELFORMAT_ARGB8888); // used to be RGBA8888
                if (extracted_surface == IntPtr.Zero)
                    {
                        SDL_LogError(SDL_LOG_CATEGORY_APPLICATION,
                            $"There was an issue creating the extracted surface:" +
                            $"\n{SDL_GetError()}");
                        return IntPtr.Zero;
                    }
                
                
                
                
                // Extract the image from the MasterSurface using the extraction rectangle
                if (SDL_BlitSurface(this.Master_Surface,
                        ref extraction_rectangle,
                        extracted_surface, IntPtr.Zero) != 0)
                    {
                        SDL_LogError(SDL_LOG_CATEGORY_APPLICATION,
                            $"There was an issue extracting the image:" +
                            $"\n{SDL_GetError()}");
                        SDL_FreeSurface(extracted_surface);
                        return IntPtr.Zero;
                    }
                
                
                if (entry.Rotated)
                    {
                        IntPtr rotated_extr_surface = Subsystem_Imaging
                            .Rotate_Surface(extracted_surface);

                        // int temp_w = extraction_rectangle.w;
                        // extraction_rectangle.w = extraction_rectangle.h;
                        // extraction_rectangle.h = temp_w;
                        extracted_surface = rotated_extr_surface;
                    }

                IntPtr final_texture =
                    SDL_CreateTextureFromSurface(Main_App.Renderer,
                        extracted_surface);

                SDL_FreeSurface(extracted_surface);

                return final_texture;
            }

        internal SDL_Rect Get_Atlas_Image_Rect(string short_filename)
            {
                var full_filename =
                    $"{Subsystem_Imaging.DIRECTORY_PATH}\\{short_filename}.png";
                Atlas_Entry? entry =
                    this.Entries.FirstOrDefault(
                        t => t.Filename == full_filename);
                return entry?.Rectangle ?? default;
            }

        /// <summary>
        /// This function attempts to find a suitable node in the atlas tree.
        /// If a suitable node is found, it is marked as used and the children
        /// nodes are created. If no suitable node is found, then the function
        /// returns null.
        /// </summary>
        /// <param name="parent_node"></param>
        /// <param name="required_w">Width dimension required for the node.</param>
        /// <param name="required_h">Height dimension required for the node.</param>
        /// <returns></returns>
        public static Atlas_Node? find_node(Atlas_Node parent_node,
            int required_w, int required_h)
            {
                if (parent_node.Used)
                    {
                        if (parent_node.Children == null)
                            {
#if DEBUG
                                Console.WriteLine(
                                    $"Node ({parent_node.X}, {parent_node.Y}, " +
                                    $"{parent_node.Width}, {parent_node.Height}) is used but has no children.");
#endif
                                return null;
                            }

                        Atlas_Node? node = find_node(parent_node.Children[0],
                                               required_w, required_h) ??
                                           find_node(parent_node.Children[1],
                                               required_w, required_h);
                        return node;
                    }

                if (required_w <= parent_node.Width &&
                    required_h <= parent_node.Height)
                    {
#if DEBUG
                        Console.WriteLine(
                            $"Found a suitable node at ({parent_node.X}, {parent_node.Y}, " +
                            $"{parent_node.Width}, {parent_node.Height}) for dimensions ({required_w}, {required_h})");
#endif
                        split_node(parent_node, required_w, required_h);
                        return parent_node;
                    }

#if DEBUG
                Console.WriteLine($"Node ({parent_node.X}, {parent_node.Y}, " +
                                  $"{parent_node.Width}, {parent_node.Height}) is too small for dimensions ({required_w}, {required_h})");
#endif
                return null;
            }

        /// <summary>
        /// Split a node into two children nodes. The node is marked as used.
        /// </summary>
        /// <param name="parent_node"></param>
        /// <param name="used_w"></param>
        /// <param name="used_h"></param>
        private static void split_node(Atlas_Node parent_node, int used_w,
            int used_h)
            {
                parent_node.Used = true;

                int width_padding = parent_node.Width - used_w >= _PADDING
                    ? _PADDING
                    : 0;
                int height_padding = parent_node.Height - used_h >= _PADDING
                    ? _PADDING
                    : 0;

                if (parent_node.Width - used_w - width_padding < 0 ||
                    parent_node.Height - used_h - height_padding < 0)
                    {
                        SDL_LogError(SDL_LOG_CATEGORY_APPLICATION,
                            $"Invalid split at Node ({parent_node.X}, {parent_node.Y}, " +
                            $"{parent_node.Width}, {parent_node.Height}) with dimensions ({used_w}, {used_h})");
                        return;
                    }

                parent_node.Children = new Atlas_Node[2];
                parent_node.Children[0] =
                    new Atlas_Node(parent_node.X + used_w + width_padding,
                        parent_node.Y,
                        parent_node.Width - used_w - width_padding, used_h);
                parent_node.Children[1] =
                    new Atlas_Node(parent_node.X,
                        parent_node.Y + used_h + height_padding,
                        parent_node.Width,
                        parent_node.Height - used_h - height_padding);
            }

        public int Get_Index_Of_Surface(IntPtr surface)
            {
                int index = Array.IndexOf(this.Surface_Data, surface);
                return index;
            }
    }

/**
 * A node in the atlas tree.
 * As the tree is created, more nodes are made by splitting the last node
 */
public class Atlas_Node
    {
        public bool Used { get; set; }
        public int X { get; }
        public int Y { get; }
        public int Width { get; set; }

        public int Height { get; set; }

        // Children can be null or have valid elements
        public Atlas_Node[]? Children { get; set; }

        public Atlas_Node(int x, int y, int width, int height)
            {
                this.X = x;
                this.Y = y;
                this.Width = width;
                this.Height = height;
                this.Children = null;
            }
    }

/**
 * Represents a finalized image in the atlas by filename, drawing rectangle, and a rotation flag.
 * These can be used to blit from the image with ease.
 */
internal class Atlas_Entry
    {
        public string Filename { get; }
        public SDL_Rect Rectangle { get; set; }
        public bool Rotated { get; set; }

        public Atlas_Entry(string filename, SDL_Rect
                rectangle = default,
            bool rotated = false)
            {
                this.Filename = filename;
                this.Rectangle = rectangle;
                this.Rotated = rotated;
            }
    }