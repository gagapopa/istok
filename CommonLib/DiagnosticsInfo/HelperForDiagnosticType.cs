/*
 * Создано в SharpDevelop.
 * Пользователь: Господин
 * Дата: 17.02.2015
 * Время: 18:12
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;

namespace COTES.ISTOK.DiagnosticsInfo
{
	/// <summary>
	/// Description of HelperForDiagnosticType.
	/// </summary>
	static class HelperForDiagnosticType
	{
		public static  IEnumerable<Type> GetFiagTypes(ICustomAttributeProvider provider){			
			var type = 	Assembly.Load("COTES.ISTOK.Server.Core").GetType("COTES.ISTOK.Assignment.Gdiag.GlobalDiagnostics");
			yield return type;
		}
	}
}
