using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace COTES.ISTOK
{
    public static class Consts
    {
        #region PropertyTable
        public const string PropertyTableName = "Name";
        public const string PropertyTableDisplayName = "DisplayName";
        public const string PropertyTableDescription = "Description";
        public const string PropertyTableCategory = "Category";
        public const string PropertyTableValue = "Value";
        #endregion

        #region ParamListTable
        public const string ParamListTableName = "Name";
        public const string ParamListTableCode = "Code";
        #endregion

        #region ChannelProperties
        public const string PropertyUseRemoting = "UseRemoting";
        public const string PropertyRemotingHost = "RemotingHost";
        public const string PropertyRemotingPort = "RemotingPort";
        public const string PropertySeparator = "Separator";
        public const string PropertyNamesFile = "NamesFile";
        public const string PropertyNamesStruct = "NamesStruct";
        #endregion


        #region Parameters
        public const string ParameterID = "ID";
        public const string ParameterName = "Name";
        public const string ParameterValue = "Value";
        public const string ParameterCode = "code";
        public const string ParameterUnit = "unit";
        public const string ParameterTime = "Time";
        public const string ParameterQuality = "Quality";
        public const string ParameterChannel = "Channel";
        public const string ParameterMinValue = "MinValue";
        public const string ParameterMaxValue = "MaxValue";
        public const string ParameterMinWarningLimit = "MinWarningLimit";
        public const string ParameterMaxWarningLimit = "MaxWarningLimit";
        #endregion

        #region Tunnel
        public const string remoteServerURI = "RemoteServer.rem";
        #endregion
    }
}
