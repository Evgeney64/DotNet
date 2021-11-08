using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;

using Server.Core.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using Server.Core.Public;
//using Server.Core.AuthModel;
using Microsoft.AspNetCore.Mvc.Filters;

using ru.tsb.mvc;
using Tsb.Security.Web.Models;

namespace Client.Mvc.Models
{
    public static class ConfigurateHelper
    {
		public static DataConfiguration GetConfiguration(string config_file, string name)
		{
			IConfiguration configuration = getConfiguration("Client.Mvc", "Client.Mvc", config_file);
			DataConfiguration conf = new DataConfiguration();
			configuration.Bind(name, conf);

			return conf;
		}

		private static IConfiguration getConfiguration(string client_path, string config_path, string config_file)
		{
			string base_dir = AppDomain.CurrentDomain.BaseDirectory;
			string conf_dir = base_dir.Substring(0, base_dir.IndexOf(client_path)) + config_path + "\\";
			{ }
			var builder = new ConfigurationBuilder()
				//.SetBasePath(conf_dir).AddJsonFile(config_file)
				.AddJsonFile(conf_dir + config_file)
				;
			IConfiguration configuration = builder.Build();
			return configuration;
		}
	}

	public class DataConfiguration
	{
		public string ConnectionString { get; set; }
		public int CommandTimeout { get; set; }
		public bool is_postgres { get; set; }
	}

}
