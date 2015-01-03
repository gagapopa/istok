using System;
namespace COTES.Calc
{
    /// <summary>
    /// Библиотека для расчёта функций IF97
    /// </summary>
    public class IF97
    {
        /// <summary>
        /// Тип гамма-функции
        /// </summary>
        enum GammaType
        {
            None, Pi, PiPi, Tau, TauTau, PiTau,
            Zero, ZeroPi, ZeroPiPi, ZeroTau, ZeroTauTau,
            Phi, PhiDelta, PhiDeltaDelta, PhiTau, PhiTauTau, PhiDeltaTau
        }

        double _t_gamma(GammaType t, int area, double[] I, double[] J, double[] n,
            double pi, double tau)
        {
            double ret = 0;
            double tau_A = 0;
            double pi_M = 1, pi_A = 0;
            int count,
                countI = I == null ? 0 : I.Length,
                countJ = J == null ? 0 : J.Length,
                countn = n == null ? 0 : n.Length;

            count = Math.Max(Math.Max(countI, countJ), countn);

            switch (area)
            {
                case 0:
                    tau_A = -1.222;
                    pi_M = -1;
                    pi_A = 7.1;
                    break;
                case 1:
                    tau_A = -0.5;
                    break;
                default:
                    break;
            }

            Func<int, double, double, double, double> a_n = null;
            switch (t)
            {
                case GammaType.None:
                    //case GammaType.R:
                    a_n = (i, _I, _J, _n) => _n * Math.Pow(pi_M * pi + pi_A, _I) * Math.Pow(tau + tau_A, _J);
                    break;
                case GammaType.Pi:
                    //case GammaType.RPi:
                    a_n = (i, _I, _J, _n) => pi_M * _n * _I * Math.Pow(pi_M * pi + pi_A, _I - 1) * Math.Pow(tau + tau_A, _J);
                    break;
                //case GammaType.RPiPi:
                case GammaType.PiPi:
                    a_n = (i, _I, _J, _n) => _n * _I * (_I - 1) * Math.Pow(pi_M * pi + pi_A, _I - 2) * Math.Pow(tau + tau_A, _J);
                    break;
                //case GammaType.RTau:
                case GammaType.Tau:
                    a_n = (i, _I, _J, _n) => _n * Math.Pow(pi_M * pi + pi_A, _I) * _J * Math.Pow(tau + tau_A, _J - 1);
                    break;
                //case GammaType.RTauTau:
                case GammaType.TauTau:
                    a_n = (i, _I, _J, _n) => _n * Math.Pow(pi_M * pi + pi_A, _I) * _J * (_J - 1) * Math.Pow(tau + tau_A, _J - 2);
                    break;
                //case GammaType.RPiTau:
                case GammaType.PiTau:
                    a_n = (i, _I, _J, _n) => pi_M * _n * _I * Math.Pow(pi_M * pi + pi_A, _I - 1) * _J * Math.Pow(tau + tau_A, _J - 1);
                    break;

                case GammaType.Zero:
                    ret = Math.Log(pi);
                    a_n = (i, _I, _J, _n) => _n * Math.Pow(tau, _J);
                    break;
                case GammaType.ZeroPi:
                    ret = 1 / pi;
                    break;
                case GammaType.ZeroPiPi:
                    ret = -1 / pi / pi;
                    break;
                case GammaType.ZeroTau:
                    a_n = (i, _I, _J, _n) => _n * _J * Math.Pow(tau, _J - 1);
                    break;
                case GammaType.ZeroTauTau:
                    a_n = (i, _I, _J, _n) => _n * _J * (_J - 1) * Math.Pow(tau, _J - 2);
                    break;
                default:
                    break;
            }

            double n_i, I_i, J_i;
            if (a_n != null)
                for (int i = 0; i < count; i++)
                {
                    I_i = countI > i ? I[i] : 0;
                    J_i = countJ > i ? J[i] : 0;
                    n_i = countn > i ? n[i] : 0;


                    double a = a_n(i, I_i, J_i, n_i);
                    ret += a;
                }
            return ret;
        }

        double _gamma(GammaType t, int area, double pi, double tau)
        {
            return _t_gamma(t, area, _Im[area], _Jm[area], _nm[area], pi, tau);
        }

        double _gamma_0(GammaType t, int area, double pi, double tau)
        {
            return _t_gamma(t, area, null, _J0m[area], _n0m[area], pi, tau);
        }

        /// <summary>
        /// Получить номер области
        /// </summary>
        /// <param name="p">Давление</param>
        /// <param name="T">Температура</param>
        /// <returns></returns>
        int _get_area(double p, double T)
        {
            if (273.15 <= T && T <= 623.15 && p_s(T) <= p && p <= 100)
                return 0;
            if ((273.15 <= T && T <= 623.15 && 0 <= p && p <= p_s(T))
                || (623.15 <= T && T <= 863.15 && 0 <= p && p <= _ap(T))
                || (863.15 <= T && T <= 1073.15 && 0 <= p && p <= 100))
                return 1;
            if (623.15 <= T && T <= _aT(p) && _ap(T) <= p && p <= 100)
                return 2;
            if (1073.15 <= T && T <= 2273.15 && 0 < p && p < 10)
                return 4;

            throw new ArgumentException(String.Format("Аргументы (p = {0}, T = {1}) не в области определения", p, T));
        }

        double[] _n_sm = new double[] { 0.11670521452767e4, -0.72421316703206e6, -0.17073846940092e2, 0.12020824702470e5, -0.32325550322333e7, 0.14915108613530e2, -0.48232657361591e4, 0.40511340542057e6, -0.23855557567849, 0.65017534844798e3 };
        const double _p_s = 1;
        const double _T_s = 1;

        public double p_s(double T)
        {
            if (T < 273.15 || T > 647.096)
                throw new ArgumentException(String.Format("Аргумент (T = {0}) не в области определения", T));
            double A, B, C, nu;

            nu = T / _T_s + _n_sm[8] / (T / _T_s - _n_sm[9]);

            A = nu * nu + _n_sm[0] * nu + _n_sm[1];
            B = _n_sm[2] * nu * nu + _n_sm[3] * nu + _n_sm[4];
            C = _n_sm[5] * nu * nu + _n_sm[6] * nu + _n_sm[7];

            return _p_s * Math.Pow(2 * C / (-B + Math.Sqrt(B * B - 4 * A * C)), 4);
        }

        public double T_s(double p)
        {
            if (p < 0.611213e-3 || p > 22.064)
                throw new ArgumentException(String.Format("Аргумент (p = {0}) не в области определения", p));

            double D, E, F, G, beta;

            beta = Math.Pow(p / _p_s, 1.0 / 4);

            E = beta * beta + _n_sm[2] * beta + _n_sm[5];
            F = _n_sm[0] * beta * beta + _n_sm[3] * beta + _n_sm[6];
            G = _n_sm[1] * beta * beta + _n_sm[4] * beta + _n_sm[7];

            D = 2 * G / (-F - Math.Sqrt(F * F - 4 * E * G));

            return (_n_sm[9] + D - Math.Sqrt(Math.Pow(_n_sm[9] + D, 2) - 4 * (_n_sm[8] + _n_sm[9] * D))) / 2 * _T_s;
        }

        double[] _an = new double[] { 0.34805185628969e3, -0.11671859879975e1, 0.10192980039326e-2, 0.57254459862746e3, 0.13918839778870e2 };
        const double _T_a = 1;
        const double _p_a = 1;

        double _ap(double T)
        {
            double Theta = T / _T_a;
            return (_an[0] + _an[1] * Theta + _an[2] * Theta * Theta) * _p_a;
        }

        double _aT(double p)
        {
            double pi = p / _p_a;
            return (_an[3] + Math.Sqrt((pi - _an[4]) / _an[2])) * _T_a;
        }

        double[][] _Im = new double[][]{
            new double[]{0,0,0,0,0,0,0,0,1,1,1,1,1,1,2,2,2,2,2,3,3,3,4,4,4,5,8,8,21,23,29,30,31,32},
            new double[]{1,1,1,1,1,2,2,2,2,2,3,3,3,3,3,4,4,4,5,6,6,6,7,7,7,8,8,9,10,10,10,16,16,18,20,20,20,21,22,23,24,24,24},
            new double[]{0,0,0,0,0,0,0,0,1,1,1,1,2,2,2,2,2,2,3,3,3,3,3,4,4,4,4,5,5,5,6,6,6,7,8,9,9,10,10,11},
            null,
            new double[]{1,1,1,2,3}
        };
        double[][] _Jm = new double[][]{
            new double[]{-2,-1,0,1,2,3,4,5,-9,-7,-1,0,1,3,-3,0,1,3,17,-4,0,6,-5,-2,10,-8,-11,-6,-29,-31,-38,-39,-40,-41},
            new double[]{0,1,2,3,6,1,2,4,7,36,0,1,3,6,35,1,2,3,7,3,16,35,0,11,25,8,36,13,4,10,14,29,50,57,20,35,48,21,53,39,26,40,58},
            new double[]{0,0,1,2,7,10,12,23,2,6,15,17,0,2,6,7,22,26,0,2,4,16,26,0,2,4,26,1,3,26,0,2,26,2,26,2,26,0,1,26},
            null,
            new double[]{0,1,3,9,3}
        };

        double[][] _J0m = new double[][]{
            null,
            new double[]{0,1,-5,-4,-3,-2,-1,2,3},
            null,
            null,
            new double[]{0,1,-3,-2,-1,2}
        };

        double[][] _nm = new double[][]{
            new double[]{0.14632971213167,-0.84548187169114,-0.37563603672040e1,0.33855169168385e1,-0.95791963387872,0.15772038513228,-0.16616417199501e-1,0.81214629983568e-3,0.28319080123804e-3,-0.60706301565874e-3,-0.18990068218419e-1,-0.32529748770505e-1,-0.21841717175414e-1,-0.52838357969930e-4,-0.47184321073267e-3,-0.30001780793026e-3,0.47661393906987e-4,-0.44141845330846e-5,-0.72694996297594e-15,-0.31679644845054e-4,-0.28270797985312e-5,-0.85205128120103e-9,-0.22425281908000e-5,-0.65171222895601e-6,-0.14341729937924e-12,-0.40516996860117e-6,-0.12734301741641e-8,-0.17424871230634e-9,-0.68762131295531e-18,0.14478307828521e-19,0.26335781662795e-22,-0.11947622640071e-22,0.18228094581404e-23,-0.93537087292458e-25},
            new double[]{-0.17731742473213e-2,-0.17834862292358e-1,-0.45996013696365e-1,-0.57581259083432e-1,-0.50325278727930e-1,0.33032641670203e-4,-0.18948987516315e-3,-0.39392777243355e-2,-0.43797295650573e-1,-0.26674547914087e-4,0.20481737692309e-7,0.43870667284435e-6,-0.32277677238570e-4,-0.15033924542148e-2,-0.40668253562649e-1,-0.78847309559367e-9,0.12790717852285e-7,0.48225372718507e-6,0.22922076337661e-5,-0.16714766451061e-10,-0.21171472321355e-2,-0.23895741934104e2,-0.59059564324270e-17,-0.12621808899101e-5,-0.38946842435739e-1,0.11256211360459e-10,-0.82311340897998e1,0.19809712802088e-7,0.10406965210174e-18,-0.10234747095929e-12,-0.10018179379511e-8,-0.80882908646985e-10,0.10693031879409,-0.33662250574171,0.89185845355421e-24,0.30629316876232e-12,-0.42002467698208e-5,-0.59056029685639e-25,0.37826947613457e-5,-0.12768608934681e-14,0.73087610595061e-28,0.55414715350778e-16,-0.94369707241210e-6},
            new double[]{0.10658070028513e1,-0.15732845290239e2,0.20944396974307e2,-0.76867707878716e1,0.26185947787954e1,-0.28080781148620e1,0.12053369696517e1,-0.84566812812502e-2,-0.12654315477714e1,-0.11524407806681e1,0.88521043984318,-0.64207765181607,0.38493460186671,-0.85214708824206,0.48972281541877e1,-0.30502617256965e1,0.39420536879154e-1,0.12558408424308,-0.27999329698710,0.13899799569460e1,-0.20189915023570e1,-0.82147637173963e-2,-0.47596035734923,0.43984074473500e-1,-0.44476435428739,0.90572070719733,0.70522450087967,0.10770512626332,-0.32913623258954,-0.50871062041158,-0.22175400873096e-1,0.94260751665092e-1,0.16436278447961,-0.13503372241348e-1,-0.14834345352472e-1,0.57922953628084e-3,0.32308904703711e-2,0.80964802996215e-4,-0.16557679795037e-3,-0.44923899061815e-4},
            null,
            new double[]{-0.12563183589592e-3,0.21774678714571e-2,-0.45942820899910e-2,-0.39724828359569e-5,0.12919228289784e-6}
        };

        double[][] _n0m = new double[][]{
            null,
            new double[]{-0.96927686500217e1,0.10086655968018e2,-0.56087911283020e-2,0.71452738081455e-1,-0.40710498223928,0.14240819171444e1,-0.43839511319450e1,-0.28408632460772,0.21268463753307e-1},
            null,
            null,
            new double[]{-0.13179983674201e2,0.68540841634434e1,-0.24805148933466e-1,0.36901534980333,-0.31161318213925e1,-0.32961626538917}
        };

        const double _R = 0.461526;
        const double _T_0 = 1386;
        const double _p_0 = 16.53;
        const double _T_1 = 540;
        const double _p_1 = 1;
        const double _T_4 = 1000;
        const double _p_4 = 1;
        const double _T_c = 647.096;
        const double _p_c = 22.064;
        const double _ro_c = 322;

        /// <summary>
        /// Давление
        /// </summary>
        /// <param name="ro"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public double p(double ro, double T)
        {
            double delta = ro / _ro_c, tau = T / _T_c;
            int area = 2;// get_area(p, T);


            switch (area)
            {
                case 2:
                    return delta * _gamma(GammaType.PhiDelta, area, delta, tau) * ro * _R * T;

                default:
                    break;
            }
            return double.NaN;
        }

        /// <summary>
        /// Удельный объем
        /// </summary>
        /// <param name="p">Давление, МПа</param>
        /// <param name="T">Температура, К</param>
        /// <returns>Удельный объем, м^3/кг</returns>
        public double nu(double p, double T)
        {
            double pi = 0, tau = 0;
            int area = _get_area(p, T);

            switch (area)
            {
                case 0:
                    pi = p / _p_0;
                    tau = _T_0 / T;
                    return _R * T * pi * _gamma(GammaType.Pi, area, pi, tau) / p / 1000;
                case 1:
                    pi = p / _p_1;
                    tau = _T_1 / T;
                    return pi * (_gamma_0(GammaType.ZeroPi, area, pi, tau) + _gamma(GammaType.Pi, area, pi, tau)) * _R * T / p / 1000;
                case 4:
                    pi = p / _p_4;
                    tau = _T_4 / T;
                    return pi * (_gamma_0(GammaType.ZeroPi, area, pi, tau) + _gamma(GammaType.Pi, area, pi, tau)) * _R * T / p / 1000;
                default:
                    break;
            } return double.NaN;
        }

        /// <summary>
        /// Удельная внутренняя энергия
        /// </summary>
        /// <param name="p">Давление, МПа</param>
        /// <param name="T">Температура, К</param>
        /// <returns>Удельная внутренняя энергия, кДж/кг</returns>
        public double u(double p, double T)
        {
            double pi = 0, tau = 0;
            int area = _get_area(p, T);

            switch (area)
            {
                case 0:
                    pi = p / _p_0;
                    tau = _T_0 / T;
                    return (tau * _gamma(GammaType.Tau, area, pi, tau) - pi * _gamma(GammaType.Pi, area, pi, tau)) * _R * T;
                case 1:
                    pi = p / _p_1;
                    tau = _T_1 / T;
                    return (tau * (_gamma_0(GammaType.ZeroTau, area, pi, tau) + _gamma(GammaType.Tau, area, pi, tau)) -
                        pi * (_gamma_0(GammaType.ZeroPi, area, pi, tau) + _gamma(GammaType.Pi, area, pi, tau))) * _R * T;
                case 4:
                    pi = p / _p_4;
                    tau = _T_4 / T;
                    return (tau * (_gamma_0(GammaType.ZeroTau, area, pi, tau) + _gamma(GammaType.Tau, area, pi, tau)) -
                        pi * (_gamma_0(GammaType.ZeroPi, area, pi, tau) + _gamma(GammaType.Pi, area, pi, tau))) * _R * T;
                default:
                    break;
            } return double.NaN;
        }

        /// <summary>
        /// Удельная энтропия
        /// </summary>
        /// <param name="p">Давление, МПа</param>
        /// <param name="T">Температура, К</param>
        /// <returns>Удельная энтропия, кДж/(кг*К)</returns>
        public double s(double p, double T)
        {
            double pi = 0, tau = 0;
            int area = _get_area(p, T);

            switch (area)
            {
                case 0:
                    pi = p / _p_0;
                    tau = _T_0 / T;
                    return (tau * _gamma(GammaType.Tau, area, pi, tau) - _gamma(GammaType.None, area, pi, tau)) * _R;
                case 1:
                    pi = p / _p_1;
                    tau = _T_1 / T;
                    return (tau * (_gamma_0(GammaType.ZeroTau, area, pi, tau) + _gamma(GammaType.Tau, area, pi, tau)) -
                        (_gamma_0(GammaType.Zero, area, pi, tau) + _gamma(GammaType.None, area, pi, tau))) * _R;
                case 4:
                    pi = p / _p_4;
                    tau = _T_4 / T;
                    return (tau * (_gamma_0(GammaType.ZeroTau, area, pi, tau) + _gamma(GammaType.Tau, area, pi, tau)) -
                        (_gamma_0(GammaType.Zero, area, pi, tau) + _gamma(GammaType.None, area, pi, tau))) * _R;
                default:
                    break;
            }
            return double.NaN;
        }

        /// <summary>
        /// Удельная энтальпия
        /// </summary>
        /// <param name="p">Давление, МПа</param>
        /// <param name="T">Температура, К</param>
        /// <returns>Удельная энтальпия, кДж/кг</returns>
        public double h(double p, double T)
        {
            double pi = 0, tau = 0;
            int area = _get_area(p, T);

            switch (area)
            {
                case 0:
                    pi = p / _p_0;
                    tau = _T_0 / T;
                    return tau * _gamma(GammaType.Tau, area, pi, tau) * _R * T;
                case 1:
                    pi = p / _p_1;
                    tau = _T_1 / T;
                    return tau * (_gamma_0(GammaType.ZeroTau, area, pi, tau) + _gamma(GammaType.Tau, area, pi, tau)) * _R * T;
                case 4:
                    pi = p / _p_4;
                    tau = _T_4 / T;
                    return tau * (_gamma_0(GammaType.ZeroTau, area, pi, tau) + _gamma(GammaType.Tau, area, pi, tau)) * _R * T;
                default:
                    break;
            }
            return double.NaN;
        }

        /// <summary>
        /// Удельная изобарная теплоемкость
        /// </summary>
        /// <param name="p">Давление, МПа</param>
        /// <param name="T">Температура, К</param>
        /// <returns>Удельная изобарная теплоемкость, кДж/(кг*К)</returns>
        public double c_p(double p, double T)
        {
            double pi = 0, tau = 0;
            int area = _get_area(p, T);

            switch (area)
            {
                case 0:
                    pi = p / _p_0;
                    tau = _T_0 / T;
                    return -tau * tau * _gamma(GammaType.TauTau, area, pi, tau) * _R;
                case 1:
                    pi = p / _p_1;
                    tau = _T_1 / T;
                    return -tau * tau * (_gamma_0(GammaType.ZeroTauTau, area, pi, tau) + _gamma(GammaType.TauTau, area, pi, tau)) * _R;
                case 4:
                    pi = p / _p_4;
                    tau = _T_4 / T;
                    return -tau * tau * (_gamma_0(GammaType.ZeroTauTau, area, pi, tau) + _gamma(GammaType.TauTau, area, pi, tau)) * _R;
                default:
                    break;
            }
            return double.NaN;
        }

        /// <summary>
        /// Удельная изохорная теплоемкость
        /// </summary>
        /// <param name="p">Давление, МПа</param>
        /// <param name="T">Температура, К</param>
        /// <returns>Удельная изохорная теплоемкость, кДж/(кг*К)</returns>
        public double c_nu(double p, double T)
        {
            double pi = 0, tau = 0;
            int area = _get_area(p, T);

            switch (area)
            {
                case 0:
                    pi = p / _p_0;
                    tau = _T_0 / T;
                    return (-tau * tau * _gamma(GammaType.TauTau, area, pi, tau) +
                        Math.Pow(_gamma(GammaType.Pi, area, pi, tau) - tau * _gamma(GammaType.PiTau, area, pi, tau), 2) /
                        _gamma(GammaType.PiPi, area, pi, tau)) * _R;
                case 1:
                    pi = p / _p_1;
                    tau = _T_1 / T;
                    return (-tau * tau * (_gamma_0(GammaType.ZeroTauTau, area, pi, tau) + _gamma(GammaType.TauTau, area, pi, tau)) -
                        Math.Pow(1 + pi * _gamma(GammaType.Pi, area, pi, tau) - tau * pi * _gamma(GammaType.PiTau, area, pi, tau), 2) /
                        (1 - pi * pi * _gamma(GammaType.PiPi, area, pi, tau))) * _R;
                case 4:
                    pi = p / _p_4;
                    tau = _T_4 / T;
                    return (-tau * tau * (_gamma_0(GammaType.ZeroTauTau, area, pi, tau) + _gamma(GammaType.TauTau, area, pi, tau)) -
                        Math.Pow(1 + pi * _gamma(GammaType.Pi, area, pi, tau) - tau * pi * _gamma(GammaType.PiTau, area, pi, tau), 2) /
                        (1 - pi * pi * _gamma(GammaType.PiPi, area, pi, tau))) * _R;
                default:
                    break;
            }
            return double.NaN;
        }

        /// <summary>
        /// Скорость звука
        /// </summary>
        /// <param name="p">Давление, МПа</param>
        /// <param name="T">Температура, К</param>
        /// <returns>Скорость звука, м/с</returns>
        public double w(double p, double T)
        {
            int area = _get_area(p, T);
            double pi = 0, tau = 0;

            switch (area)
            {
                case 0:
                    pi = p / _p_0;
                    tau = _T_0 / T;
                    double gamma_pi = _gamma(GammaType.Pi, area, pi, tau);
                    double w_2 = gamma_pi * gamma_pi * _R * T /
                        (Math.Pow(gamma_pi - tau * _gamma(GammaType.PiTau, area, pi, tau), 2) / tau / tau /
                        _gamma(GammaType.TauTau, area, pi, tau) - _gamma(GammaType.PiPi, area, pi, tau)) * 1000;
                    return Math.Sqrt(w_2);
                case 1:
                    pi = p / _p_1;
                    tau = _T_1 / T;
                    gamma_pi = _gamma(GammaType.Pi, area, pi, tau);
                    w_2 = (1 + 2 * pi * gamma_pi + pi * pi * gamma_pi * gamma_pi) /
                       ((1 - pi * pi * _gamma(GammaType.PiPi, area, pi, tau)) + Math.Pow(1 + pi * gamma_pi - tau * pi * _gamma(GammaType.PiTau, area, pi, tau), 2) /
                       (tau * tau * (_gamma_0(GammaType.ZeroTauTau, area, pi, tau) + _gamma(GammaType.TauTau, area, pi, tau)))) * _R * T * 1000;
                    return Math.Sqrt(w_2);
                case 4:
                    pi = p / _p_4;
                    tau = _T_4 / T;
                    gamma_pi = _gamma(GammaType.Pi, area, pi, tau);
                    w_2 = (1 + 2 * pi * gamma_pi + pi * pi * gamma_pi * gamma_pi) /
                        ((1 - pi * pi * _gamma(GammaType.PiPi, area, pi, tau)) + Math.Pow(1 + pi * gamma_pi - tau * pi * _gamma(GammaType.PiTau, area, pi, tau), 2) /
                        (tau * tau * (_gamma_0(GammaType.ZeroTauTau, area, pi, tau) + _gamma(GammaType.TauTau, area, pi, tau)))) * _R * T * 1000;
                    return Math.Sqrt(w_2);
                default:
                    break;
            }
            return double.NaN;
        }
    }
}