using System;
namespace COTES.Calc
{
    /// <summary>
    /// Класс для подключения функции IF97 в расчёт в ИСТОК
    /// </summary>
    public static class CalculusETO
    {
        static IF97 lib = new IF97();

        /// <summary>
        /// перевод из кгс/см^2 в МПа
        /// </summary>
        static double kgs2MPa = 10.1972;
        /// <summary>
        /// перевод °C в °K
        /// </summary>
        static double C2K = 273.15;
        /// <summary>
        /// перевод из ккал в кДж
        /// </summary>
        static double kkal2kJ = 0.238846;

        public static double IF97_p_s(double T)
        {
            return lib.p_s(T + C2K) * kgs2MPa;
        }

        public static double IF97_T_s(double p)
        {
            return lib.T_s(p / kgs2MPa) - C2K;
        }

        [System.ComponentModel.Description("Давление")]
        public static double IF97_p(double ro, double T)
        {
            return lib.p(ro, T + C2K)* kgs2MPa;
        }

        [System.ComponentModel.Description("Удельный объем")]
        public static double IF97_nu(double p, double T)
        {
            return lib.nu(p / kgs2MPa, T + C2K);
        }

        [System.ComponentModel.Description("Удельная внутренняя энергия")]
        public static double IF97_u(double p, double T)
        {
            return lib.u(p / kgs2MPa, T + C2K) * kkal2kJ;
        }

        [System.ComponentModel.Description("Удельная энтропия")]
        public static double IF97_s(double p, double T)
        {
            return lib.s(p / kgs2MPa, T + C2K) * kkal2kJ;
        }

        [System.ComponentModel.Description("Удельная энтальпия")]
        public static double IF97_h(double p, double T)
        {
            return lib.h(p / kgs2MPa, T + C2K) * kkal2kJ;
        }

        [System.ComponentModel.Description("Удельная изобарная теплоемкость")]
        public static double IF97_c_p(double p, double T)
        {
            return lib.c_p(p / kgs2MPa, T + C2K) * kkal2kJ;
        }

        [System.ComponentModel.Description("Удельная изохорная теплоемкость")]
        public static double IF97_c_nu(double p, double T)
        {
            return lib.c_nu(p / kgs2MPa, T + C2K) * kkal2kJ;
        }

        [System.ComponentModel.Description("Скорость звука")]
        public static double IF97_w(double p, double T)
        {
            return lib.w(p / kgs2MPa, T + C2K);
        }
    }
}
