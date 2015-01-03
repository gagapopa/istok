using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ServiceProcess;

namespace COTES.ISTOK
{
    /// <summary>
    /// Позволяет установить Windows Service'у Description и режим Interactive
    /// </summary>
    public sealed class WindowsServiceHelper
    {
        //Это класс-утилита, поэтому нет смысла создавать его экземпляры
        private WindowsServiceHelper()
        {

        }

        public static void ChangeInteractiveState(string serviceName, bool makeInteractive)
        {
            ChangeService(serviceName, false, makeInteractive, string.Empty);
        }

        public static void ChangeDescription(string serviceName, string description)
        {
            ChangeService(serviceName, true, false, description);
        }

        static void ChangeService(string serviceName, bool setDescription, bool makeInteractive, string description)
        {
            //Открываем service control manager
            IntPtr hSCM = OpenSCManager(IntPtr.Zero, IntPtr.Zero, SC_MANAGER_ALL_ACCESS);
            CheckHandle(hSCM);
            try
            {
                //открываем службу
                IntPtr hService = OpenService(hSCM, serviceName, SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);
                CheckHandle(hService);

                try
                {
                    if (setDescription)
                        ChangeDescriptionInternal(hService, description);
                    else
                        ChangeInteractiveStateInternal(hService, makeInteractive);
                }
                finally
                {
                    //Закрываем Handle'ы службы и scm
                    CloseServiceHandle(hService);
                }
            }
            finally
            {
                //Закрываем Handle SCM
                CloseServiceHandle(hSCM);
            }
        }

        static void ChangeInteractiveStateInternal(IntPtr hService, bool makeInteractive)
        {
            //получаем размер необходимого буфера для получения структуры с конфигурацией службы
            //и затем вторым вызовом QueryServiceConfig уже получаем заполненный буфер из которого далее получаем структуру
            int bytesNeaded;

            if (!QueryServiceConfig(hService, IntPtr.Zero, 0, out bytesNeaded) && Marshal.GetLastWin32Error() != ERROR_INSUFFICIENT_BUFFER)
                ThrowLastError();

            IntPtr buffer = Marshal.AllocHGlobal(bytesNeaded);
            if (!QueryServiceConfig(hService, buffer, bytesNeaded, out bytesNeaded))
                ThrowLastError();

            QUERY_SERVICE_CONFIG serviceConfig = (QUERY_SERVICE_CONFIG)Marshal.PtrToStructure(buffer, typeof(QUERY_SERVICE_CONFIG));
            Marshal.FreeHGlobal(buffer);

            //Если служба запускается не под account'ом LocalSystem,
            //следаем так, чтобы она запускалась под этим account'ом, т. к. это требуется для установки флага SERVICE_INTERACTIVE_PROCESS
            if (serviceConfig.lpServiceStartName != ServiceAccount.LocalSystem.ToString())
                if (!ChangeServiceConfig(hService, SERVICE_NO_CHANGE, SERVICE_NO_CHANGE, SERVICE_NO_CHANGE, null, null, IntPtr.Zero, null, ServiceAccount.LocalSystem.ToString(), string.Empty, null))
                    ThrowLastError();

            //Добавляем/убираем флаг SERVICE_INTERACTIVE_PROCESS
            serviceConfig.dwServiceType = makeInteractive ? serviceConfig.dwServiceType | SERVICE_INTERACTIVE_PROCESS : serviceConfig.dwServiceType & ~SERVICE_INTERACTIVE_PROCESS;

            //А здесь непосредственно изменяем конфигурацию службы
            if (!ChangeServiceConfig(hService, serviceConfig.dwServiceType, SERVICE_NO_CHANGE, SERVICE_NO_CHANGE, null, null, IntPtr.Zero, null, null, null, null))
                ThrowLastError();
        }

        static void ChangeDescriptionInternal(IntPtr hService, string description)
        {
            SERVICE_DESCRIPTION serviceDescription = new SERVICE_DESCRIPTION(description);
            IntPtr pointer = Marshal.AllocHGlobal(Marshal.SizeOf(serviceDescription));
            Marshal.StructureToPtr(serviceDescription, pointer, false);
            try
            {
                if (!ChangeServiceConfig2(hService, SERVICE_CONFIG_DESCRIPTION, pointer))
                    ThrowLastError();
            }
            finally
            {
                Marshal.FreeHGlobal(pointer);
            }
        }

        /// <summary>
        /// Проверяет переданный handle и если он не правильный, генерирует исключение
        /// </summary>
        /// <param name="handle">Handle, который нужно проверить на валидность</param>
        static void CheckHandle(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                ThrowLastError();
        }

        static void ThrowLastError()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        #region Необходимые Windows API функции и константы

        const int SC_MANAGER_ALL_ACCESS = 0x000F003F;
        const int SERVICE_QUERY_CONFIG = 1;
        const int SERVICE_CHANGE_CONFIG = 2;

        const int SERVICE_INTERACTIVE_PROCESS = 0x00000100;
        const int SERVICE_NO_CHANGE = -1;

        const int ERROR_INSUFFICIENT_BUFFER = 122;

        const int SERVICE_CONFIG_DESCRIPTION = 1;

        const string ADVAPI32_DLL = "Advapi32.dll";

        [DllImport(ADVAPI32_DLL, SetLastError = true)]
        static extern IntPtr OpenSCManager(IntPtr lpMachineName, IntPtr lpDatabaseName, int dwDesiredAccess);


        [DllImport(ADVAPI32_DLL, SetLastError = true)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, int dwDesiredAccess);


        [DllImport(ADVAPI32_DLL, SetLastError = true)]
        static extern bool QueryServiceConfig(IntPtr hService, IntPtr lpServiceConfig,
            int cbBufSize, out int pcbBytesNeeded);

        [DllImport(ADVAPI32_DLL, SetLastError = true)]
        static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport(ADVAPI32_DLL, SetLastError = true)]
        static extern bool ChangeServiceConfig(IntPtr hService,
            int dwServiceType,
            int dwStartType,
            int dwErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            IntPtr lpdwTagId,
            string lpDependencies,
            string lpServiceStartName,
            string lpPassword,
            string lpDisplayName
            );

        [DllImport(ADVAPI32_DLL, SetLastError = true)]
        static extern bool ChangeServiceConfig2(
            IntPtr hService,
            int dwInfoLevel,
            IntPtr lpInfo
            );


        [StructLayout(LayoutKind.Sequential)]
        struct QUERY_SERVICE_CONFIG
        {
            public int dwServiceType;
            public int dwStartType;
            public int dwErrorControl;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpBinaryPathName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpLoadOrderGroup;
            public int dwTagId;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpDependencies;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpServiceStartName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpDisplayName;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SERVICE_DESCRIPTION
        {
            public string lpDescription;
            public SERVICE_DESCRIPTION(string description)
            {
                lpDescription = description;
            }
        }

        #endregion
    }
}
