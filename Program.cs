using static SDL2.SDL;

using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace GotchiTaMm
{

    /**
     * @TODO: 
     * 1. Make a SaveState class, serialize it to binary and encrypt to a file (do I need b64?)
     * 2. Make button
     */

    internal class Program
    {
        // GAME LOOP

        static bool Continue = true;

        // SETUP

        static IntPtr Window;
        static IntPtr Renderer;

        // RENDER

        // Structure
        static SDL_Rect my_rectangle = new SDL_Rect { x = 50, y = 50, w = 200, h = 60 };

        // Structure & Pointer to it
        static SDL_Rect my_rectangle2 = new SDL_Rect { x = 250, y = 250, w = 20, h = 60 };
        static IntPtr rectangle_ptr = IntPtr.Zero;

        // INPUT

        static int[] Keyboard = new int[255];

        public delegate void KeysymDelegate(SDL_Keysym keysym);
        public delegate void MouseButtonEventDelegate(SDL_MouseButtonEvent mouseButtonEvent);

        public static event KeysymDelegate KeyDownEvent;
        public static event KeysymDelegate KeyUpEvent;
        public static event MouseButtonEventDelegate MouseDownEvent;
        public static event MouseButtonEventDelegate MouseUpEvent;

        // ENCRYPTION

        static byte[] secret = {
                0x0, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1,
                0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1
        };


        // SAVING

        [Serializable]
        public class SaveState
        {
            public DateTime LastTime { get; set; }
        }

        public static SaveState Save;

        // Actual program starts here...


        static void Main(string[] args)
        {
            Setup();

            Task<SaveState> LoadData = LoadGame();
            Save = LoadData.Result;

            if (Save.LastTime == DateTime.MinValue)
            {
                Console.WriteLine("This is the first time the program has been run.");
            }
            else
            {
                Console.WriteLine($"The program was last shutdown at: {Save.LastTime}");
            }

            int size = Marshal.SizeOf(my_rectangle2);
            rectangle_ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(my_rectangle2, rectangle_ptr, false);

            // Subscribe the respective methods to the events
            // Methods have to respect the delegate definition
            KeyDownEvent += OnKeyDown;
            KeyUpEvent += OnKeyUp;
            MouseDownEvent += OnMouseDown;
            MouseUpEvent += OnMouseUp;

            while (Continue)
            {
                Input();
                Render();
            }

            QuitGame();
        }

        static void Setup()
        {
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine(
                    $"There was an issue starting SDL:\n{SDL_GetError()}!");
            }

            Window = SDL_CreateWindow("GotchiTaMm!!!",
                SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED,
                640, 480, SDL_WindowFlags.SDL_WINDOW_SHOWN);

            if (Window == IntPtr.Zero)
            {
                Console.WriteLine(
                    $"There was an issue creating the window:\n{SDL_GetError()}");
            }

            Renderer = SDL_CreateRenderer(Window, -1,
                SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            if (Renderer == IntPtr.Zero)
            {
                Console.WriteLine(
                    $"There was an issue creating the renderer:\n{SDL_GetError()}");
            }
        }

        static void Render()
        {
            SDL_SetRenderDrawColor(Renderer, 155, 155, 155, 255);
            SDL_RenderClear(Renderer);

            SDL_SetRenderDrawColor(Renderer, 125, 205, 235, 255);
            SDL_RenderDrawLine(Renderer, 5, 5, 300, 310);


            SDL_SetRenderDrawColor(Renderer, 255, 0, 0, 255);
            // Draw with structure ref
            SDL_RenderFillRect(Renderer, ref my_rectangle);

            SDL_SetRenderDrawColor(Renderer, 0, 255, 0, 255);
            // Draw with pointer to structure
            SDL_RenderFillRect(Renderer, rectangle_ptr);

            SDL_RenderPresent(Renderer);
        }

        static void Input()
        {
            while (SDL_PollEvent(out SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        QuitGame();
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        MouseDownEvent?.Invoke(e.button);
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                        MouseUpEvent?.Invoke(e.button);
                        break;
                    case SDL_EventType.SDL_KEYDOWN:
                        KeyDownEvent?.Invoke(e.key.keysym);
                        Keyboard[(int)e.key.keysym.scancode] = 1;
                        break;
                    case SDL_EventType.SDL_KEYUP:
                        KeyUpEvent?.Invoke(e.key.keysym);
                        Keyboard[(int)e.key.keysym.scancode] = 0;
                        break;
                    default:
                        // error...
                        break;
                }
            }
        }

        public static void OnKeyDown(SDL_Keysym keysym)
        {
            Console.WriteLine($"Key down: {keysym.scancode}");

            if (keysym.scancode == SDL_Scancode.SDL_SCANCODE_ESCAPE)
            {
                QuitGame();
            }
        }

        public static void OnKeyUp(SDL_Keysym keysym)
        {
            Console.WriteLine($"Key up: {keysym.scancode}");
        }

        public static void OnMouseDown(SDL_MouseButtonEvent mouseButtonEvent)
        {
            Console.WriteLine($"Mouse click: {mouseButtonEvent.button} at {mouseButtonEvent.x}, {mouseButtonEvent.y}");
        }

        public static void OnMouseUp(SDL_MouseButtonEvent mouseButtonEvent)
        {
            Console.WriteLine($"Mouse released: {mouseButtonEvent.button} at {mouseButtonEvent.x}, {mouseButtonEvent.y}");
        }

        static void SaveGame(DateTime saveTime)
        {
            try
            {
                Save.LastTime = saveTime;

                using FileStream fileStream = new("MEM", FileMode.OpenOrCreate);
                using Aes aes = Aes.Create();
                aes.Key = secret;

                byte[] iv = aes.IV;
                fileStream.Write(iv, 0, iv.Length);

                using CryptoStream cryptoStream = new(
                    fileStream,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write);
                using StreamWriter encryptWriter = new(cryptoStream);

                string jsonString = JsonSerializer.Serialize(Save);
                byte[] jsonData = Encoding.UTF8.GetBytes(jsonString);
                cryptoStream.Write(jsonData, 0, jsonData.Length);

                Console.WriteLine("The file was encrypted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The encryption failed. {ex}");
            }
        }

        static async Task<SaveState> LoadGame()
        {
            SaveState save = new SaveState {
                LastTime = DateTime.MinValue,
            };

            try
            {

                if (File.Exists("MEM"))
                {
                    using FileStream fileStream = new("MEM", FileMode.Open);
                    using Aes aes = Aes.Create();

                    byte[] iv = new byte[aes.IV.Length];
                    int numBytesToRead = aes.IV.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        int n = fileStream.Read(iv, numBytesRead, numBytesToRead);
                        if (n == 0) break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }

                    using CryptoStream cryptoStream = new(fileStream, aes.CreateDecryptor(secret, iv), CryptoStreamMode.Read);

                    using MemoryStream memoryStream = new();
                    await cryptoStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    string jsonString = Encoding.UTF8.GetString(memoryStream.ToArray());
                    if (jsonString != "{}")
                    {
                        save = JsonSerializer.Deserialize<SaveState>(jsonString);
                    }
                    else
                    {
                        Console.WriteLine("DEBUG");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The decryption failed. {ex}");
            }

            return save;
        }

        static void QuitGame()
        {
            // Release unsafe pointer
            Marshal.FreeHGlobal(rectangle_ptr);

            SaveGame(DateTime.Now);
            SDL_Quit();

            Console.WriteLine("Program exited successfully!");
            Environment.Exit(0);
        }
    }
}