using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;

using Server.Core.ViewModel;

namespace HtmlHelpersApp.App_Code
{
	public static class ListHelper
	{
		public static HtmlString CreateList(this IHtmlHelper html, string[] items)
		{
			string result = "<ul>";
			foreach (string item in items)
			{
				result = $"{result}<li>{item}</li>";
			}
			result = $"{result}</ul>";
			return new HtmlString(result);
		}

		public static object Customize(this IHtmlHelper html, VmBase vmBase)
		{
			string div = "";
			div += ""
				//+ "<html lang='en'>"
				//+ "<body>"
				+ "<h3>Улицы</h3>"
				//+ "<script src='~/lib/jquery/dist/jquery.min.js'></script>"
				+ "<script>"
					+ "$(document).ready(function () {"
						+ "Site_Accordion('@Model.UsersJson');"
					+ "});"
				+ "</script>"
				//+ "</body>"
				//+ "</html>"
				+ ""
				;
			//vmBase.HtmlString = @"<html lang='en'/>";
			object view = html.Raw(div);
			return view;
		}

	}
}