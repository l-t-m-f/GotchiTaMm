namespace GotchiTaMm;

internal static class Game_String_Pool
    {
        internal class String_Pool_Entry
            {
                internal readonly string Text;
                internal readonly Font_Name_Type Font;
                internal readonly int Size_Factor;

                public String_Pool_Entry(string text, Font_Name_Type font,
                    int size_factor)
                    {
                        this.Text = text;
                        this.Font = font;
                        this.Size_Factor = size_factor;
                    }
            }

        internal static readonly List<String_Pool_Entry> Data = new()
            {
                new String_Pool_Entry("Test", Font_Name_Type.BLUE_SCREEN, 4),
                new String_Pool_Entry("Enter Time: ",
                    Font_Name_Type.RAINY_HEARTS, 5),
            };
    }