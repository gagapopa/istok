using System;
using System.Collections.Generic;
using System.Threading;
using COTES.ISTOK;
using System.Xml;
using System.IO;
using COTES.ISTOK.ASC;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace COTES.ISTOK.Assignment
{
    interface IExportService
    {
        void OwnerToString(int owner, out String ownerClass, out String ownerValue);

        int OwnerFromString(String ownerClass, String ownerValue);

        String GetParameterCode(OperationState state, int parameterID);

        UnitNode NewInstance(OperationState state, int unitTypeId);

        IEnumerable<Package> GetValues(OperationState state, ParameterNode parameterNode, DateTime startTime, DateTime endTime);
    }
}
