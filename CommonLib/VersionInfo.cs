using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    [Serializable]
    public sealed partial class VersionInfo
    {
        //private const int StageDebug = 0;
        private const int StageAlpha1 = 1;
        private const int StageBeta1 = 100;
        private const int StageRC1 = 200;
        private const int StageRelease = 420;

        /// <summary>
        /// Старшая версия проекта
        /// </summary>
        private const string BuildVersionMajor = "1";

        /// <summary>
        /// Младшая версия проекта
        /// </summary>
        private const string BuildVersionMinor = "6";

        /// <summary>
        /// Стадия проекта
        /// </summary>
        /// <remarks>
        /// Значения должны выбираться в соответствии 
        /// с константами StageAlpha1, StageBeta1, StageRC1 и StageRelease.
        /// Например StageRC1 + 1, даёт стадию RC2.
        /// <br />
        /// Так как мы не использум циклы разработки и тестирование, всегда равно 0.
        /// </remarks>
        private const string BuildVersionStage = "0";

#if !VC_REVISION
        /// <summary>
        /// Номер ревизии выставляемый вручную.
        /// Что бы получать номер ревизии из системы контроля версий, 
        /// необходимо выставить флаг компиляции VC_REVISION
        /// </summary>
        private const string BuildVersionRevision = "42"; 
#endif

        /// <summary>
        /// Версия проекта, в таком виде передаётся сборке
        /// </summary>
        public const string BuildVersionString =
            BuildVersionMajor + "." +
            BuildVersionMinor + "." +
            BuildVersionStage + "." +
            BuildVersionRevision;


        public static VersionInfo BuildVersion { get; private set; }

        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Stage { get; private set; }
        public int Revision { get; private set; }

        static VersionInfo()
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            BuildVersion = new VersionInfo(
                version.Major,
                version.Minor,
                version.Build,
                version.Revision);
        }

        private VersionInfo(int major, int minor, int stage, int revision)
        {
            this.Major = major;
            this.Minor = minor;
            this.Stage = stage;
            this.Revision = revision;
        }

        public String VersionString
        {
            get
            {
                String versionString= String.Format("{0}.{1}.{2}.{3}", Major, Minor, Stage, Revision);

                return versionString + stageSuffix(Stage);
            }
        }

        private string stageSuffix(int stageNum)
        {
            /*if (stageNum == StageDebug)
            {
                return " DEBUG";
            }
            else*/
            if (stageNum > StageRelease)
            {
                return String.Format(" Release {0}", stageNum - StageRelease);
            }
            else if (stageNum == StageRelease)
            {
                return String.Empty;
            }
            else if (stageNum >= StageRC1)
            {
                return String.Format(" RC {0}", stageNum - StageRC1 + 1);
            }
            else if (stageNum >= StageBeta1)
            {
                return String.Format(" Beta {0}", stageNum - StageBeta1 + 1);
            }
            else if (stageNum >= StageAlpha1)
            {
                return String.Format(" Alpha {0}", stageNum - StageAlpha1 + 1);
            }

            return String.Empty;
        }
    }
}
