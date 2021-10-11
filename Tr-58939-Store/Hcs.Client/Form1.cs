using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;

using Hcs.Configuration;
using Hcs.Store;

namespace Hcs.Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            label1.Visible = false;
            label2.Visible = false;
            EntityDataSourceConfiguration conf = getDataSourceConfiguration("config.json");
            EntityDataStoreNew store = new EntityDataStoreNew(conf);
            TransactionInfo info = TransactionInfo.Create(SysOperationCode.AccountImport);

            Guid guid = await store.CreateTransactionAsync(info);
            //Task<Guid> result = store.CreateTransactionAsync(info);
            //var sss = store.CreateTransactionAsync1(info);
            //var sss = store.CreateTransactionAsync2(info);
            { }
            this.label1.Text = DateTime.Now.ToString();
            this.label2.Text = guid.ToString();
            label1.Visible = true;
            label2.Visible = true;
        }

        #region Configuration
        private EntityDataSourceConfiguration getDataSourceConfiguration(string config_file)
        {
            IConfiguration configuration = getConfiguration(config_file);

            //EntityDataSourceConfiguration conf1 = configuration.GetSection("EntityDataSourceConfiguration").Get<EntityDataSourceConfiguration>();
            EntityDataSourceConfiguration conf = new EntityDataSourceConfiguration();
            configuration.Bind("EntityDataSourceConfiguration", conf);
            return conf;
        }

        private IConfiguration getConfiguration(string config_file)
        {
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string conf_dir = base_dir.Substring(0, base_dir.IndexOf("Hcs.Client")) + "Hcs.Client\\";
            { }
            var builder = new ConfigurationBuilder()
                //.SetBasePath(conf_dir).AddJsonFile(config_file)
                .AddJsonFile(conf_dir + config_file)
                ;
            IConfiguration configuration = builder.Build();
            return configuration;
        }
        #endregion
    }
}


