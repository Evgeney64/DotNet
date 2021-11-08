using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;

using Server.Core;


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
			if (vmBase.UsersJson != null)
			{ }
			div += "<script>" +
				"" +
				"	$(document).ready(function () {" +
				"		Site_Accordion(\"@Model.UsersJson\");" +
				//"		Site_Accordion(\"@Model.UsersJson\");" +
				"		Site_Accordion1();" +
				//"		Ext.create('Ext.Panel', {" +
    //            "			width: 500, height: 360, padding: 10, layout: 'border'," +
    //            "			items: [" +
    //            "				{xtype: 'panel', title: 'Центральная панель', html: 'Центральная панель', region: 'center', margin: '5 5 5 5' }," +
    //            "				{xtype: 'panel', title: 'Верхняя панель', html: 'Верхняя панель', region: 'north', height: 80}" +
    //            "			]," +
    //            "			renderTo: Ext.getBody()" +
    //            "			});" +
                "	});" +
				"</script>"
				;
			string str = vmBase.UsersJson.Replace("\"", "&quot;");
			div = "<script>$(document).ready(function () {Site_Accordion(\"" + str + "\");});</script>";
			object view = html.Raw(div);
			return view;
		}

	}
}