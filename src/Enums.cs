using System.Drawing;

namespace GotchiTaMm;

internal enum Button_State_Type
    {
        UNSELECTED = 0,
        SELECTED,
        ACTIVATED,
    }

internal enum Picto_Name_Type
    {
        ATTENTION = 0,
        BATHROOM,
        FOOD,
        GAME,
        LIGHTS,
        MEDICINE,
        STATUS,
        TRAINING,
    }

internal enum Font_Name_Type
    {
        BLUE_SCREEN = 0,
        RAINY_HEARTS,
    }

internal static class Font_Name_Type_Extensions
    {
        public static string? To_Friendly_String(this Font_Name_Type font_name)
            {
                return font_name switch
                    {
                        Font_Name_Type.BLUE_SCREEN => "BlueScreen",
                        Font_Name_Type.RAINY_HEARTS => "RainyHearts",
                        _ => null
                    };
            }
    }

internal enum Text_Var_Name_Type
    {
        TIME_START = 0,
        CLOCK_TIME,
    }

internal enum Button_Name_Type
    {
        SELECT = 0,
        EXECUTE,
        CANCEL,
    }