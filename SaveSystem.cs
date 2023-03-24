using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using static GotchiTaMm.Program;

namespace GotchiTaMm
{
    internal class SaveSystem
    {

        // ENCRYPTION

        static byte[] secret = {
                0x0, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1,
                0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1, 0x1
        };

        internal SaveState? SavedGame;

        //Singleton

        internal static readonly Lazy<SaveSystem> lazyInstance = new Lazy<SaveSystem>(() => new SaveSystem());
        private SaveSystem()
        {
        }

        public static SaveSystem Instance {
            get {
                return lazyInstance.Value;
            }
        }

        //
        internal void SaveGame(DateTime saveTime)
        {
            try
            {
                if (SavedGame == null)
                {
                    Console.WriteLine("Error! Attempting to save, but SaveState is corrupt.");
                    QuitGame(-1);
                    return;
                }
                SavedGame.LastTime = saveTime;

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

                string jsonString = JsonSerializer.Serialize(SavedGame);
                byte[] jsonData = Encoding.UTF8.GetBytes(jsonString);
                cryptoStream.Write(jsonData, 0, jsonData.Length);

                Console.WriteLine("The file was encrypted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The encryption failed. {ex}");
            }
        }

        internal async Task<SaveState> LoadGame()
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
    }
}
