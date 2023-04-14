namespace GotchiTaMm;

public static class String_Extensions
    {
        public static string Drop_Last_Char(this string value)
            {
                if (string.IsNullOrEmpty(value)) return value;
                return value.Length == 1 ? "" : value.Substring(0, value.Length-1);
            }
    }