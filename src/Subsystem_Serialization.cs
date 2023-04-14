using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static GotchiTaMm.Main_App;

namespace GotchiTaMm;

internal class Subsystem_Serialization
    {

        // ENCRYPTION

        private static readonly byte[] _Secret = {
                0x0, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1,
                0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1
            };

        internal Save_State? SavedGame;

        //Singleton

        private static readonly Lazy<Subsystem_Serialization> _Lazy_Instance = new Lazy<Subsystem_Serialization>(() => new Subsystem_Serialization());
        private Subsystem_Serialization()
            {
            }

        public static Subsystem_Serialization Instance => _Lazy_Instance.Value;

        //
        internal void Save_Game(DateTime save_time)
            {
                try
                    {
                        if (this.SavedGame == null)
                            {
                                Console.WriteLine("Error! Attempting to save, but SaveState is corrupt.");
                                QuitGame(-1);
                                return;
                            }

                        this.SavedGame.Last_Time = save_time;

                        using FileStream file_stream = new("MEM", FileMode.OpenOrCreate);
                        using Aes aes = Aes.Create();
                        aes.Key = _Secret;

                        byte[] iv = aes.IV;
                        file_stream.Write(iv, 0, iv.Length);

                        using CryptoStream crypto_stream = new(
                            file_stream,
                            aes.CreateEncryptor(),
                            CryptoStreamMode.Write);
                        using StreamWriter encrypt_writer = new(crypto_stream);

                        string json_string = JsonSerializer.Serialize(this.SavedGame);
                        byte[] json_data = Encoding.UTF8.GetBytes(json_string);
                        crypto_stream.Write(json_data, 0, json_data.Length);

                        Console.WriteLine("The file was encrypted.");
                    }
                catch (Exception ex)
                    {
                        Console.WriteLine($"The encryption failed. {ex}");
                    }
            }

        internal async Task<Save_State> Load_Game()
            {
                Save_State? save = new Save_State {
                        Last_Time = DateTime.MinValue,
                    };

                try
                    {

                        if (File.Exists("MEM"))
                            {
                                await using FileStream file_stream = new("MEM", FileMode.Open);
                                using Aes aes = Aes.Create();

                                byte[] iv = new byte[aes.IV.Length];
                                int num_bytes_to_read = aes.IV.Length;
                                int num_bytes_read = 0;
                                while (num_bytes_to_read > 0)
                                    {
                                        int n = file_stream.Read(iv, num_bytes_read, num_bytes_to_read);
                                        if (n == 0)
                                            {
                                                break;
                                            }
                                        num_bytes_read += n;
                                        num_bytes_to_read -= n;
                                    }

                                await using CryptoStream crypto_stream = 
                                    new(file_stream, aes.CreateDecryptor(_Secret, iv), CryptoStreamMode.Read);

                                using MemoryStream memory_stream = new();
                                await crypto_stream.CopyToAsync(memory_stream);
                                memory_stream.Position = 0;

                                string json_string = Encoding.UTF8.GetString(memory_stream.ToArray());
                                if (json_string != "{}")
                                    {
                                        save = JsonSerializer.Deserialize<Save_State>(json_string);
                                    }
                            }
                    }
                catch (Exception ex)
                    {
                        Console.WriteLine($"The decryption failed. {ex}");
                    }

                if (save != null)
                    {
                        return save;
                    }

                Console.WriteLine("Save is corrupt, therefore, must reset save.");
                save = new Save_State {
                        Last_Time = DateTime.MinValue
                    };

                return save;
            }
    }