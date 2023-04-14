namespace GotchiTaMm;

internal static class Game_String_Pool
    {

        internal class StringPoolEntry
            {
                internal readonly string Text;
                internal readonly Font_Name_Type Font;
                internal readonly int SizeFactor;

                public StringPoolEntry(string text, Font_Name_Type font, int size_factor)
                    {
                        this.Text = text;
                        this.Font = font;
                        this.SizeFactor = size_factor;
                    }
            }

        internal static readonly List<StringPoolEntry> Data = new()
            {
                new StringPoolEntry("Test", Font_Name_Type.BLUE_SCREEN, 4),
                new StringPoolEntry("Enter Time: ", Font_Name_Type.RAINY_HEARTS, 5),
            };
    }