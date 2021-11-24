using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Filters;

using Home.Controllers;

namespace Client.Mvc.Models
{
    public static class ConfigurateHelper
    {
		private static string root_dir_name = "Client.Mvc";
		public static DataConfiguration GetConfiguration(string config_file, string name)
		{
			IConfiguration configuration = getConfiguration(root_dir_name, root_dir_name, config_file);
			DataConfiguration conf = new DataConfiguration();
			configuration.Bind(name, conf);

			return conf;
		}

		private static IConfiguration getConfiguration(string client_path, string config_path, string config_file)
		{
			//string base_dir = AppDomain.CurrentDomain.BaseDirectory;
			//string conf_dir = base_dir.Substring(0, base_dir.IndexOf(client_path)) + config_path + "\\";
			string conf_dir = IOHelper.GetRootDir(client_path) + "\\";
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
