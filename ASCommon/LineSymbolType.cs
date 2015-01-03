using System.ComponentModel;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Тип маркера на графике
    /// </summary>
    [TypeConverter(typeof(LineSymbolTypeConverter))]
    public enum LineSymbolType
    {
        Square,
        Diamond,
        Triangle,
        Circle,
        XCross,
        Plus,
        Star,
        TriangleDown,
        HDash,
        VDash,
        //UserDefined,
        //Default,
        None = VDash + 3
        //Пусто = 0,
        //Ромб,
        //Треугольник,
        //Квадрат,
        //Антитреугольник
    }

    /// <summary>
    /// Стиль линии на графике
    /// </summary>
    public enum LineStyle
    {
        Сплошная = 0,
        Пунктир = 1,
        Точка = 2,
        Точка_пунктир = 3,
        Пунктир_точка_точка = 4,
    }

}
