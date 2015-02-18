/*
 * Создано в SharpDevelop.
 * Пользователь: Господин
 * Дата: 13.02.2015
 * Время: 11:15
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using NUnit.Framework;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;
using COTES.ISTOK.DiagnosticsInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace COTES.ISTOK.ClientCore.TestClientCore
{
	[TestFixture]
	public class DiagnisticsTest
	{
		[Test]
		public void GetDiagnosticObject()
		{
			 
			 EndpointAddress address = new EndpointAddress("net.tcp://localhost:4001/GlobalQueryManager");
			 var bind = new NetTcpBinding();
             var factory = new ChannelFactory<IGlobalQueryManager>(bind, address);
             factory.Open();
             IGlobalQueryManager qm = factory.CreateChannel();
             var res = qm.Connect("admin","admin");
             Guid UID = res.UserGuid;
             Diagnostics diagAssert = qm.GetDiagnosticsObject(UID).Result;
             
             Assert.IsNotNull(diagAssert);
		}
	}
}
