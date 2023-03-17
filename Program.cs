﻿using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Drawing;

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

        internal const int WINDOW_W = 480;
        internal const int WINDOW_H = 320;

        static bool Continue = true;

        // SETUP

        static internal IntPtr Window;
        static internal IntPtr Renderer;

        // RENDER

        // Structure
        static SDL_Rect my_rectangle = new SDL_Rect { x = 50, y = 50, w = 200, h = 60 };

        // Structure & Pointer to it
        static SDL_Rect my_rectangle2 = new SDL_Rect { x = 250, y = 250, w = 20, h = 60 };
        static IntPtr rectangle_ptr = IntPtr.Zero;

        static SDL_Rect my_circle = new SDL_Rect { x = WINDOW_W / 2, y = WINDOW_H / 2, w = 100, h = 80 };

        // INPUT

        internal static int[] Keyboard = new int[255];

        public static class Mouse
        {
            internal static SDL_Point Position = new SDL_Point();
            internal static int[] Buttons = new int[4];
        }

        public delegate void KeysymDelegate(SDL_Keysym keysym);
        public delegate void MouseButtonEventDelegate(SDL_MouseButtonEvent mouseButtonEvent);
        public delegate void MouseMotionEventDelegate(SDL_MouseMotionEvent mouseMotionEvent);

        public static event KeysymDelegate? KeyDownEvent;
        public static event KeysymDelegate? KeyUpEvent;
        public static event MouseButtonEventDelegate? MouseDownEvent;
        public static event MouseButtonEventDelegate? MouseUpEvent;
        public static event MouseMotionEventDelegate? MouseMotionEvent;

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

        public static SaveState? Save;

        // OTHER THREADS

        static Thread? counter;

        // GUI

        static UserInterface? UI;


        // Actual program starts here...


        static void Main(string[] args)
        {

            Setup();

            UI = UserInterface.GetUI();

            counter = new Thread(() => CounterThread());

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

            counter.Start();

            int size = Marshal.SizeOf(my_rectangle2);
            rectangle_ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(my_rectangle2, rectangle_ptr, false);

            // Subscribe the respective methods to the events
            // Methods have to respect the delegate definition
            KeyDownEvent += OnKeyDown;
            KeyUpEvent += OnKeyUp;
            MouseDownEvent += OnMouseDown;
            MouseUpEvent += OnMouseUp;
            MouseMotionEvent += OnMouseMove;

            while (Continue)
            {
                Input();
                Render();
            }

            QuitGame(0);
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
                WINDOW_W, WINDOW_H, SDL_WindowFlags.SDL_WINDOW_SHOWN);

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

            if (TTF_Init() < 0)
            {
                Console.WriteLine(
                    $"There was an issue starting SDL_ttf:\n{SDL_GetError()}!");
            }

            IMG_InitFlags img_flags = IMG_InitFlags.IMG_INIT_PNG;
            if (IMG_Init(img_flags) != (int)img_flags)
            {
                Console.WriteLine(
                    $"There was an issue starting SDL_image:\n{SDL_GetError()}!");
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

            UI.Draw();

            IntPtr texture_of = SDL_CreateTextureFromSurface(Renderer, UI.Texts[0]);

            Blit(texture_of, 0, 220);

            SDL_SetRenderDrawColor(Renderer, 0, 255, 255, 255);

            FillEllipsoid(my_circle);

            SDL_RenderPresent(Renderer);
        }

        static void Input()
        {
            while (SDL_PollEvent(out SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        QuitGame(0);
                        break;
                    case SDL_EventType.SDL_MOUSEMOTION:
                        MouseMotionEvent?.Invoke(e.motion);
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        MouseDownEvent?.Invoke(e.button);
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                        MouseUpEvent?.Invoke(e.button);
                        break;
                    case SDL_EventType.SDL_KEYDOWN:
                        KeyDownEvent?.Invoke(e.key.keysym);
                        break;
                    case SDL_EventType.SDL_KEYUP:
                        KeyUpEvent?.Invoke(e.key.keysym);
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
            Keyboard[(int)keysym.scancode] = 1;

            if (keysym.scancode == SDL_Scancode.SDL_SCANCODE_ESCAPE)
            {
                QuitGame(0);
            }
        }

        public static void OnKeyUp(SDL_Keysym keysym)
        {
            Console.WriteLine($"Key up: {keysym.scancode}");
            Keyboard[(int)keysym.scancode] = 0;
        }

        public static void OnMouseDown(SDL_MouseButtonEvent mouseButtonEvent)
        {
            Console.WriteLine($"Mouse click: {mouseButtonEvent.button} at {mouseButtonEvent.x}, {mouseButtonEvent.y}");
            if (mouseButtonEvent.button <= 3)
            {
                Mouse.Buttons[mouseButtonEvent.button] = 1;
            }
        }

        public static void OnMouseUp(SDL_MouseButtonEvent mouseButtonEvent)
        {
            Console.WriteLine($"Mouse released: {mouseButtonEvent.button} at {mouseButtonEvent.x}, {mouseButtonEvent.y}");
            if (mouseButtonEvent.button <= 3)
            {
                Mouse.Buttons[mouseButtonEvent.button] = 0;
            }
        }
        public static void OnMouseMove(SDL_MouseMotionEvent mouseMotionEvent)
        {
            Console.WriteLine($"Mouse moving at {mouseMotionEvent.x}, {mouseMotionEvent.y}");
            Mouse.Position.x = mouseMotionEvent.x;
            Mouse.Position.y = mouseMotionEvent.y;
        }

        public static void CounterThread()
        {
            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine("A second has passed.");
            }
        }

        static void SaveGame(DateTime saveTime)
        {
            try
            {
                if (Save == null)
                {
                    Console.WriteLine("Error! Attempting to save, but SaveState is corrupt.");
                    QuitGame(-1);
                    return;
                }
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
            SaveState? save = new SaveState {
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The decryption failed. {ex}");
            }

            if (save == null)
            {

                Console.WriteLine("Save is corrupt, therefore, must reset save.");
                save = new SaveState {
                    LastTime = DateTime.MinValue,
                };
            }

            return save;
        }

        public static void DrawEllipsoid(SDL_Rect circle)
        {
            double pih = Math.PI / 2;
            const int prec = 150; // precision value; value of 1 will draw a diamond, 27 makes pretty smooth circles.
            double theta = 0; // angle that will be increased each loop

            int x = (int)(circle.w * Math.Cos(theta));//start point
            int y = (int)(circle.h * Math.Sin(theta));//start point
            int x1 = x;
            int y1 = y;

            double step = pih / prec; // amount to add to theta each time (degrees)
            for (theta = step ; theta <= pih ; theta += step)//step through only a 90 arc (1 quadrant)
            {
                //get new point location
                x1 = (int)(circle.w * Math.Cos(theta) + 0.5); //new point (+.5 is a quick rounding method)
                y1 = (int)(circle.h * Math.Sin(theta) + 0.5); //new point (+.5 is a quick rounding method)

                //draw line from previous point to new point, ONLY if point incremented
                if ((x != x1) || (y != y1))//only draw if coordinate changed
                {
                    SDL_RenderDrawLine(Renderer, circle.x + x, circle.y - y, circle.x + x1, circle.y - y1);//quadrant TR
                    SDL_RenderDrawLine(Renderer, circle.x - x, circle.y - y, circle.x - x1, circle.y - y1);//quadrant TL
                    SDL_RenderDrawLine(Renderer, circle.x - x, circle.y + y, circle.x - x1, circle.y + y1);//quadrant BL
                    SDL_RenderDrawLine(Renderer, circle.x + x, circle.y + y, circle.x + x1, circle.y + y1);//quadrant BR
                }
                //save previous points
                x = x1;//save new previous point
                y = y1;//save new previous point
            }
            //arc did not finish because of rounding, so finish the arc 
            if (x != 0)
            {
                x = 0;
                SDL_RenderDrawLine(Renderer, circle.x + x, circle.y - y, circle.x + x1, circle.y - y1);//quadrant TR
                SDL_RenderDrawLine(Renderer, circle.x - x, circle.y - y, circle.x - x1, circle.y - y1);//quadrant TL
                SDL_RenderDrawLine(Renderer, circle.x - x, circle.y + y, circle.x - x1, circle.y + y1);//quadrant BL
                SDL_RenderDrawLine(Renderer, circle.x + x, circle.y + y, circle.x + x1, circle.y + y1);//quadrant BR
            }
        }
        public static void FillEllipsoid(SDL_Rect circle)
        {
            double pih = Math.PI / 2;
            const int prec = 150; // precision value; value of 1 will draw a diamond, 27 makes pretty smooth circles.
            double theta = 0; // angle that will be increased each loop

            int x = (int)(circle.w * Math.Cos(theta));//start point
            int y = (int)(circle.h * Math.Sin(theta));//start point
            int x1 = x;
            int y1 = y;

            SDL_RenderDrawLine(Renderer, circle.x - x1, circle.y - y1, circle.x + x1, circle.y - y1);
            SDL_RenderDrawLine(Renderer, circle.x - x1, circle.y + y1, circle.x + x1, circle.y + y1);

            double step = pih / prec; // amount to add to theta each time (degrees)
            for (; theta <= pih + step; theta += step)//step through only a 90 arc (1 quadrant)
            {
                //get new point location
                x1 = (int)(circle.w * Math.Cos(theta) + 0.5); //new point (+.5 is a quick rounding method)
                y1 = (int)(circle.h * Math.Sin(theta) + 0.5); //new point (+.5 is a quick rounding method)

                //draw line from previous point to new point, ONLY if point incremented
                if ((x != x1) || (y != y1))//only draw if coordinate changed
                {
                    SDL_RenderDrawLine(Renderer, circle.x - x1, circle.y - y1, circle.x + x1, circle.y - y1);
                    SDL_RenderDrawLine(Renderer, circle.x - x1, circle.y + y1, circle.x + x1, circle.y + y1);
                }       
                //save previous points
                x = x1;//save new previous point
                y = y1;//save new previous point
            }
            //arc did not finish because of rounding, so finish the arc
            if (x != 0)
            {
                x = 0;
                SDL_RenderDrawLine(Renderer, circle.x - x1, circle.y - y1, circle.x + x1, circle.y - y1);
                SDL_RenderDrawLine(Renderer, circle.x - x1, circle.y + y1, circle.x + x1, circle.y + y1);
            }
        }


        internal static void Blit(IntPtr texture, int x, int y)
        {
            SDL_Rect destination;
            destination.x = x;
            destination.y = y;
            SDL_QueryTexture(texture, out uint format, out int access, out destination.w, out destination.h);
            SDL_RenderCopy(Renderer, texture, IntPtr.Zero, ref destination);
        }

        static void QuitGame(sbyte ProgramCode)
        {
            // Release unsafe pointer
            Marshal.FreeHGlobal(rectangle_ptr);

            SaveGame(DateTime.Now);
            TTF_Quit();
            SDL_Quit();

            Console.WriteLine("Program exited successfully!");
            Environment.Exit(ProgramCode);
        }
    }
}