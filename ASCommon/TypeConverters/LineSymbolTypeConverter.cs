using System;
using System.ComponentModel;

namespace COTES.ISTOK.ASC.TypeConverters
{
    // Тип маркера на графике
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

    public class LineSymbolTypeConverter : EnumConverter
    {
        public LineSymbolTypeConverter()
            : base(typeof(LineSymbolType))
        {
            //
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.Equals(typeof(string)))
            {
                switch ((LineSymbolType)value)
                {
                    case LineSymbolType.Circle: return "Круг";
                    case LineSymbolType.Diamond: return "Ромб";
                    case LineSymbolType.HDash: return "Горизонтальная линия";
                    case LineSymbolType.Plus: return "Плюс";
                    case LineSymbolType.Square: return "Квадрат";
                    case LineSymbolType.Star: return "Звезда";
                    case LineSymbolType.Triangle: return "Треугольник";
                    case LineSymbolType.TriangleDown: return "Нижний треугольник";
                    case LineSymbolType.VDash: return "Вертикальная линия";
                    case LineSymbolType.XCross: return "Х-крест";
                    case LineSymbolType.None: return "Пусто";
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                switch (value.ToString())
                {
                    case "Круг": return LineSymbolType.Circle;
                    case "Ромб": return LineSymbolType.Diamond;
                    case "Горизонтальная линия": return LineSymbolType.HDash;
                    case "Плюс": return LineSymbolType.Plus;
                    case "Квадрат": return LineSymbolType.Square;
                    case "Звезда": return LineSymbolType.Star;
                    case "Треугольник": return LineSymbolType.Triangle;
                    case "Нижний треугольник": return LineSymbolType.TriangleDown;
                    case "Вертикальная линия": return LineSymbolType.VDash;
                    case "Х-крест": return LineSymbolType.XCross;
                    case "Пусто": return LineSymbolType.None;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    // Стиль линии на графике
    public enum LineStyle
    {
        Сплошная = 0,
        Пунктир = 1,
        Точка = 2,
        Точка_пунктир = 3,
        Пунктир_точка_точка = 4,
    }

}
