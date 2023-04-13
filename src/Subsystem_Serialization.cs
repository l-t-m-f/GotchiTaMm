using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static GotchiTaMm.Main_App;

namespace GotchiTaMm;

internal class Subsystem_Serialization
    {

        // ENCRYPTION

        static byte[] secret = {
                0x0, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1,
                0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1
            };

        internal Save_State? SavedGame;

        //Singleton

        internal static readonly Lazy<Subsystem_Serialization> lazyInstance = new Lazy<Subsystem_Serialization>(() => new Subsystem_Serialization());
        private Subsystem_Serialization()
            {
            }

        public static Subsystem_Serialization Instance {
                get {
                        return lazyInstance.Value;
                    }
            }

        //
        internal void SaveGame(DateTime saveTime)
            {
                try
                    {
                        if (this.SavedGame == null)
                            {
                                Console.WriteLine("Error! Attempting to save, but SaveState is corrupt.");
                                QuitGame(-1);
                                return;
                            }

                        this.SavedGame.LastTime = saveTime;

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

                        string jsonString = JsonSerializer.Serialize(this.SavedGame);
                        byte[] jsonData = Encoding.UTF8.GetBytes(jsonString);
                        cryptoStream.Write(jsonData, 0, jsonData.Length);

                        Console.WriteLine("The file was encrypted.");
                    }
                catch (Exception ex)
                    {
                        Console.WriteLine($"The encryption failed. {ex}");
                    }
            }

        internal async Task<Save_State> LoadGame()
            {
                Save_State? save = new Save_State {
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
                                        save = JsonSerializer.Deserialize<Save_State>(jsonString);
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
                        save = new Save_State {
                                LastTime = DateTime.MinValue,
                            };
                    }

                return save;
            }
    }