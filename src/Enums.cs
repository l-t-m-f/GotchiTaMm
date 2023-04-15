using System.ComponentModel;

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

internal enum Art_Style_Type
    {
        SINGLE_SPRITE,
        ANIMATION
    }

internal static class Font_Name_Type_Extensions
    {
        public static string? To_Friendly_String(
            this Font_Name_Type font_name)
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
    
    
internal enum LifeStageType
    {
        EGG = 0,
        BABY,
        CHILD,
        TEENAGER,
        ADULT,
        SPECIAL_ADULT,
        SENIOR,
        DEATH,
    }

internal enum Gotchi_Pet_Form_Type
    {
        [Description("Baby")] BABYTCHI = 0,
        [Description("Child")] MARUTCHI,
        [Description("Teenager")] KUCHITAMATCHI,
        TAMATCHI,
        [Description("Adult")] TARAKOTCHI,
        NYOROTCHI,
        KUCHIPATCHI,
        MASKUTCHI,
        GINJIROTCHI,
        MAMETCHI,
        [Description("SpecialAdult")] BILL,
    }