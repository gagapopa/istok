using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.ASC.Report
{
    [TestFixture]
    public class ReportParametersContainerTests
    {
        //[Test]
        //public void SetProperties_AddNew()
        //{
        //    ReportParametersContainer container = new ReportParametersContainer();
        //    ReportParameter[] startParameters = new ReportParameter[]{
        //        new ReportParameter("parameter1", "Parameter 1", "", "", typeof(int), 1),
        //        new ReportParameter("parameter2", "Parameter 2", "", "", typeof(int), 2)
        //    };

        //    container.SetProperties(startParameters);

        //    List<ReportParameter> parameters = new List<ReportParameter>(startParameters);
        //    parameters.Add(new ReportParameter("parameter3", "Parameter 3", "", "", typeof(int), 3));
        //    parameters.Add(new ReportParameter("parameter4", "Parameter 4", "", "", typeof(int), 4));

        //    container.SetProperties(parameters);

        //    Assert.AreEqual(4, container.Parameters.Count);
        //    Assert.AreEqual("parameter1", container.Parameters[0].Name);
        //    Assert.AreEqual("parameter2", container.Parameters[1].Name);
        //    Assert.AreEqual("parameter3", container.Parameters[2].Name);
        //    Assert.AreEqual("parameter4", container.Parameters[3].Name);
        //}

        //[Test]
        //public void SetProperties_RemoveNotInList()
        //{
        //    ReportParametersContainer container = new ReportParametersContainer();
        //    ReportParameter[] startParameters = new ReportParameter[]{
        //        new ReportParameter("parameter1", "Parameter 1", "", "", typeof(int), 1),
        //        new ReportParameter("parameter2", "Parameter 2", "", "", typeof(int), 2),
        //        new ReportParameter("parameter3", "Parameter 3", "", "", typeof(int), 3),
        //        new ReportParameter("parameter4", "Parameter 4", "", "", typeof(int), 4)
        //    };

        //    container.SetProperties(startParameters);

        //    List<ReportParameter> parameters = new List<ReportParameter>(startParameters);
        //    parameters.RemoveAt(2);

        //    container.SetProperties(parameters);

        //    Assert.AreEqual(3, container.Parameters.Count);
        //    Assert.AreEqual("parameter1", container.Parameters[0].Name);
        //    Assert.AreEqual("parameter2", container.Parameters[1].Name);
        //    Assert.AreEqual("parameter4", container.Parameters[2].Name);
        //}

        //[Test]
        //public void SetProperties_ParameterValuesUnchanged()
        //{
        //    ReportParametersContainer container = new ReportParametersContainer();
        //    ReportParameter[] startParameters = new ReportParameter[]{
        //        new ReportParameter("parameter1", "Parameter 1", "", "", typeof(int), 1),
        //        new ReportParameter("parameter2", "Parameter 2", "", "", typeof(int), 2),
        //        new ReportParameter("parameter3", "Parameter 3", "", "", typeof(int), 3),
        //        new ReportParameter("parameter4", "Parameter 4", "", "", typeof(int), 4)
        //    };

        //    container.SetProperties(startParameters);

        //    container.Parameters[1].SetValue(134);
        //    container.Parameters[2].SetValue(546);

        //    startParameters[1] = new ReportParameter("parameter2", "Parameter 2", "", "", typeof(int), 789);
        //    startParameters[2] = new ReportParameter("parameter3", "Parameter 3", "", "", typeof(int), 986);

        //    container.SetProperties(startParameters);

        //    Assert.AreEqual(4, container.Parameters.Count);
        //    Assert.AreEqual("parameter1", container.Parameters[0].Name);
        //    Assert.AreEqual("parameter2", container.Parameters[1].Name);
        //    Assert.AreEqual(134, container.Parameters[1].GetValue());
        //    Assert.AreEqual("parameter3", container.Parameters[2].Name);
        //    Assert.AreEqual(546, container.Parameters[2].GetValue());
        //    Assert.AreEqual("parameter4", container.Parameters[3].Name);
        //}
    }
}
