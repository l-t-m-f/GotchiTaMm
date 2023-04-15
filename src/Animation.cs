using System.Text.RegularExpressions;
using static SDL2.SDL;
using static SDL2.SDL_image;

namespace GotchiTaMm;

class Animation
    {
        internal IntPtr Texture;
        internal int Frame_Count;
        internal SDL_Rect Rectangle;

        internal uint
            Frame_Duration; // Duration of each frame in milliseconds

        internal uint Timer; // Timer to control the animation

        public void Draw(int x, int y)
            {
                // Calculate the elapsed time since the last frame
                uint elapsed_time = SDL_GetTicks() - this.Timer;

                // Check if it's time to switch to the next frame
                if (elapsed_time >= this.Frame_Duration)
                    {
                        int spacing = 2;
                        this.Rectangle.x += this.Rectangle.w + spacing;
                        if (this.Rectangle.x >=
                            (this.Rectangle.w + spacing) * this.Frame_Count)
                            {
                                this.Rectangle.x = 0;
                            }

                        // Update the timer
                        this.Timer = SDL_GetTicks();
                    }

                // Render the current frame
                SDL_Rect destination = new SDL_Rect
                    {
                        x = x, y = y, 
                        w = (int)(this.Rectangle.w * Main_App.SCREEN_RATIO),
                        h = (int)(this.Rectangle.h * Main_App.SCREEN_RATIO)
                    };
                SDL_RenderCopy(Main_App.Renderer, this.Texture,
                    ref this.Rectangle, ref
                    destination);
            }
    }

class Animation_Loader
    {
        internal Dictionary<string, Animation> Animations = new();

        internal void Make_Animations()
            {
                Dictionary<string, List<string>> animations =
                    this.Get_Animations_Names("images/anims");

                foreach (KeyValuePair<string, List<string>> animation in
                         animations)
                    {
                        SDL_QueryTexture(
                            IMG_LoadTexture(Main_App.Renderer, animation
                                .Value
                                .ElementAt(0)), out uint _, out int _,
                            out int width,
                            out int height);


                        var next_animation_surface
                            = SDL_CreateRGBSurfaceWithFormat(
                                0, width * (2 + animation.Value.Count),
                                height, 32,
                                SDL_PIXELFORMAT_ARGB8888);

                        SDL_Rect next_dest = new SDL_Rect
                            {
                                x = 0, y = 0, w = width, h = height
                            };

                        foreach (string a in animation.Value)
                            {
                                var next_anim_frame =
                                    IMG_Load(a);
                                SDL_BlitSurface(next_anim_frame,
                                    IntPtr.Zero,
                                    next_animation_surface, ref next_dest);
                                next_dest.x += next_dest.w + 2;
                            }

                        const uint
                            FRAME_DURATION =
                                250; // Set this to the desired duration of each frame in milliseconds
                        this.Animations[animation.Key] = new Animation
                            {
                                Texture =
                                    SDL_CreateTextureFromSurface(
                                        Main_App.Renderer,
                                        next_animation_surface),
                                Frame_Count = animation.Value.Count,
                                Rectangle = new SDL_Rect
                                    {
                                        x = 0, y = 0, w = width, h = height
                                    },
                                Frame_Duration = FRAME_DURATION,
                                Timer = 0
                            };
                    }
            }

        private Dictionary<string, List<string>> Get_Animations_Names(
            string directory_path)
            {
                Dictionary<string, List<string>> animations =
                    new Dictionary<string, List<string>>();

                // Check if the specified directory exists
                if (Directory.Exists(directory_path))
                    {
                        // Get all the files in the directory
                        string[] file_paths =
                            Directory.GetFiles(directory_path);

                        // Iterate through the files
                        foreach (string file_path in file_paths)
                            {
                                // Extract the file name without extension
                                string file_name_without_extension =
                                    Path.GetFileNameWithoutExtension(
                                        file_path);

                                // Match the NAME_FRAMECOUNT pattern
                                Match match = Regex.Match(
                                    file_name_without_extension,
                                    @"^(.+)_(\d+)$");

                                if (match.Success)
                                    {
                                        string name = match.Groups[1].Value;
                                        int frame_count =
                                            int.Parse(match.Groups[2]
                                                .Value);

                                        // Add the file path to the appropriate animation group
                                        if (!animations.ContainsKey(name))
                                            {
                                                animations[name] =
                                                    new List<string>();
                                            }

                                        animations[name].Add(file_path);
                                    }
                            }

                        // Sort the animation frames by frame count
                        foreach (var animation in animations)
                            {
                                animation.Value.Sort((path1, path2) =>
                                    {
                                        string file_name1 =
                                            Path
                                                .GetFileNameWithoutExtension(
                                                    path1);
                                        string file_name2 =
                                            Path
                                                .GetFileNameWithoutExtension(
                                                    path2);

                                        int frame_count1 =
                                            int.Parse(Regex
                                                .Match(file_name1,
                                                    @"_(\d+)$").Groups[1]
                                                .Value);
                                        int frame_count2 =
                                            int.Parse(Regex
                                                .Match(file_name2,
                                                    @"_(\d+)$").Groups[1]
                                                .Value);

                                        return frame_count1.CompareTo(
                                            frame_count2);
                                    });
                            }
                    }
                else
                    {
                        Console.WriteLine(
                            "The specified directory does not exist.");
                    }

                return animations;
            }
    }