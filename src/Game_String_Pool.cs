namespace GotchiTaMm;

internal static class Game_String_Pool
    {

        internal class StringPoolEntry
            {
                internal string Text;
                internal FontNameType Font;
                internal int SizeFactor;

                public StringPoolEntry(string text, FontNameType font, int sizeFactor)
                    {
                        this.Text = text;
                        this.Font = font;
                        this.SizeFactor = sizeFactor;
                    }
            }

        internal static List<StringPoolEntry> Data = new List<StringPoolEntry>()
            {
                new("Test", FontNameType.BlueScreen, 4),
                new("Enter Time: ", FontNameType.RainyHearts, 5),
            };
    }