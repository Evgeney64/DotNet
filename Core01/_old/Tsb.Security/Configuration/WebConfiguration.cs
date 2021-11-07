using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;

namespace Tsb.Security.Web.Configuration
{
    public class ConfigSectionSP : ConfigurationSection
    {
        #region
        public ConfigSectionSP()
        {
        }

        [ConfigurationProperty("DomainName", DefaultValue = "TSB", IsRequired = true)]
        public string NtDomainName
        {
            get { return (string)this["DomainName"]; }
            set { this["DomainName"] = value; }
        }
        [ConfigurationProperty("Login", DefaultValue = "billing", IsRequired = true)]
        public string NtLogin
        {
            get { return (string)this["Login"]; }
            set { this["Login"] = value; }
        }
        [ConfigurationProperty("Password", DefaultValue = "LkbyysqGfhjkm123", IsRequired = true)]
        public string NtPassword
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }
        [ConfigurationProperty("BillingDocPage", DefaultValue = "http://vm172:81/TechnosbWEBTestPage.html", IsRequired = true)]
        public string BillingDocPage
        {
            get { return (string)this["BillingDocPage"]; }
            set { this["BillingDocPage"] = value; }
        }


        [ConfigurationProperty("SiteUrl", DefaultValue = "http://vm172/lerning/", IsRequired = true)]
        public string SiteUrl
        {
            get { return (string)this["SiteUrl"]; }
            set { this["SiteUrl"] = value; }
        }

        [ConfigurationProperty("DocLibrary", DefaultValue = "Document", IsRequired = true)]
        public string DocLibrary
        {
            get { return (string)this["DocLibrary"]; }
            set { this["DocLibrary"] = value; }
        }

        [ConfigurationProperty("TaskListName", DefaultValue = "Задачи", IsRequired = false)]
        public string TaskListName
        {
            get { return (string)this["TaskListName"]; }
            set { this["TaskListName"] = value; }
        }
        #endregion
    }

    public class ConfigSectionEmail : ConfigurationSection
    {
        #region
        [ConfigurationProperty("DefaultProvider", DefaultValue = "Empty")]
        [StringValidator(MinLength = 1)]
        public string DefaultProvider
        {
            get
            {
                return (string)base["DefaultProvider"];
            }
            set
            {
                base["DefaultProvider"] = value;
            }
        }
        [ConfigurationProperty("Providers")]
        public ProviderSettingsCollection Providers
        {
            get
            {
                return (ProviderSettingsCollection)base["Providers"];
            }
        }
        #endregion
    }

    public class ConfigSectionSMS : ConfigurationSection
    {
        #region
        [ConfigurationProperty("DefaultProvider", DefaultValue = "Empty")]
        [StringValidator(MinLength = 1)]
        public string DefaultProvider
        {
            get
            {
                return (string)base["DefaultProvider"];
            }
            set
            {
                base["DefaultProvider"] = value;
            }
        }
        [ConfigurationProperty("Providers")]
        public ProviderSettingsCollection Providers
        {
            get
            {
                return (ProviderSettingsCollection)base["Providers"];
            }
        }
        #endregion
    }

    public class ConfigSectionOutlook : ConfigurationSection
    {
        #region
        public ConfigSectionOutlook()
        {
        }

        [ConfigurationProperty("IsOn")]
        public bool IsOn
        {
            get { return (bool)this["IsOn"]; }
            set { this["IsOn"] = value; }
        }
        #endregion
    }

    public class ConfigSectionSIP : ConfigurationSection
    {
        #region
        public ConfigSectionSIP()
        {
        }

        [ConfigurationProperty("IsOn")]
        public bool IsOn
        {
            get { return (bool)this["IsOn"]; }
            set { this["IsOn"] = value; }
        }

        [ConfigurationProperty("URL")]
        public string URL
        {
            get { return (string)this["URL"]; }
            set { this["URL"] = value; }
        }

        [ConfigurationProperty("OuterPrefix")]
        public string OuterPrefix
        {
            get { return (string)this["OuterPrefix"]; }
            set { this["OuterPrefix"] = value; }
        }
        #endregion
    }

    public class ConfigSectionSharedDir : ConfigurationSection
    {
        #region
        public ConfigSectionSharedDir()
        {
        }

        [ConfigurationProperty("DirPath")]
        public string DirPath
        {
            get { return (string)this["DirPath"]; }
            set { this["DirPath"] = value; }
        }
        #endregion
    }

    #region Sync
    public class ConfigSectionSync : ConfigurationSection
    {
        [ConfigurationProperty("Connections")]
        public SyncConnectionCollection Connections
        {
            get
            {
                return (SyncConnectionCollection)base["Connections"];
            }
        }
    }

    [ConfigurationCollection(typeof(SyncConnection))]
    public class SyncConnectionCollection : ConfigurationElementCollection
    {
        public new SyncConnection this[string name]
        {
            get
            {
                return (SyncConnection)base.BaseGet(name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SyncConnection();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SyncConnection)element).Name;
        }
        public void Add(SyncConnection syncConnection)
        {
            base.BaseAdd(syncConnection);
        }
        public void Clear()
        {
            base.BaseClear();
        }
        public void Remove(SyncConnection syncConnection)
        {
            if (base.BaseIndexOf(syncConnection) >= 0)
            {
                base.BaseRemove(syncConnection.Name);
            }
        }
        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }

    public class SyncConnection : ConfigurationElement
    {
        [ConfigurationProperty("Name")]
        public string Name
        {
            get
            {
                return (string)base["Name"];
            }
            set
            {
                base["Name"] = value;
            }
        }
        [ConfigurationProperty("FriendlyName")]
        public string FriendlyName
        {
            get
            {
                return (string)base["FriendlyName"];
            }
            set
            {
                base["FriendlyName"] = value;
            }
        }
        [ConfigurationProperty("ConnectionStringName")]
        public string ConnectionStringName
        {
            get
            {
                return (string)base["ConnectionStringName"];
            }
            set
            {
                base["ConnectionStringName"] = value;
            }
        }
    }
    #endregion

    #region Import
    public class ConfigSectionImport : ConfigurationSection
    {
        [ConfigurationProperty("RequestTimeout")]
        public int? RequestTimeout
        {
            get
            {
                return (int?)base["RequestTimeout"];
            }
            set
            {
                base["RequestTimeout"] = value;
            }
        }
        [ConfigurationProperty("Sources")]
        public ImportSourceCollection Sources
        {
            get
            {
                return (ImportSourceCollection)base["Sources"];
            }
        }
    }

    [ConfigurationCollection(typeof(ImportSource))]
    public class ImportSourceCollection : ConfigurationElementCollection
    {
        public new ImportSource this[string name]
        {
            get
            {
                return (ImportSource)base.BaseGet(name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ImportSource();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ImportSource)element).Name;
        }
        public void Add(ImportSource importSource)
        {
            base.BaseAdd(importSource);
        }
        public void Clear()
        {
            base.BaseClear();
        }
        public void Remove(ImportSource importSource)
        {
            if (base.BaseIndexOf(importSource) >= 0)
            {
                base.BaseRemove(importSource.Name);
            }
        }
        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }

    public class ImportSource : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base["Name"];
            }
            set
            {
                base["Name"] = value;
            }
        }
        [ConfigurationProperty("FriendlyName")]
        public string FriendlyName
        {
            get
            {
                return (string)base["FriendlyName"];
            }
            set
            {
                base["FriendlyName"] = value;
            }
        }
        // сзявь с NSI_DATA_SOURCE, надеюсь что временно
        [ConfigurationProperty("Id")]
        public int Id
        {
            get
            {
                return (int)base["Id"];
            }
            set
            {
                base["Id"] = value;
            }
        }
        [ConfigurationProperty("TypeId", IsRequired = true)]
        public int TypeId
        {
            get
            {
                return (int)base["TypeId"];
            }
            set
            {
                base["TypeId"] = value;
            }
        }
        [ConfigurationProperty("URL", IsRequired = true)]
        public string URL
        {
            get
            {
                return (string)base["URL"];
            }
            set
            {
                base["URL"] = value;
            }
        }
    }
    #endregion

    #region Task Scheduler
    public class ConfigSectionTaskScheduler : ConfigurationSection
    {
        [ConfigurationProperty("IsOn", DefaultValue = true)]
        public bool IsOn
        {
            get
            {
                return (bool)this["IsOn"];
            }
            set
            {
                this["IsOn"] = value;
            }
        }        
        [ConfigurationProperty("FolderPath", IsRequired = true, DefaultValue = "\\")]
        public string FolderPath
        {
            get 
            {
                return (string)this["FolderPath"];
            }
            set
            {
                this["FolderPath"] = value;
            }
        }
        [ConfigurationProperty("AsyncMode", DefaultValue = false)]
        public bool AsyncMode
        {
            get
            {
                return (bool)this["AsyncMode"];
            }
            set
            {
                this["AsyncMode"] = value;
            }
        }

        [ConfigurationProperty("Credentials", IsRequired = false)]
        public Credentials Credentials
        {
            get
            {
                return (Credentials)this["Credentials"];
            }
            set
            {
                this["Credentials"] = value;
            }
        }
    }

    public class Credentials : ConfigurationElement
    {
        [ConfigurationProperty("Scheduler")]
        public UserNameCredentials Scheduler
        {
            get
            {
                return (UserNameCredentials)base["Scheduler"];
            }
            set
            {
                base["Scheduler"] = value;
            }
        }
        [ConfigurationProperty("Application")]
        public UserNameCredentials Application
        {
            get
            {
                return (UserNameCredentials)base["Application"];
            }
            set
            {
                base["Application"] = value;
            }
        }
    }

    public class UserNameCredentials : ConfigurationElement
    {
        [ConfigurationProperty("Username", IsRequired = true)]
        public string Username
        {
            get
            {
                return (string)this["Username"];
            }
            set
            {
                this["Username"] = value;
            }
        }
        [ConfigurationProperty("Password", IsRequired = true)]
        public string Password
        {
            get
            {
                return (string)this["Password"];
            }
            set
            {
                this["Password"] = value;
            }
        }
    }
    #endregion

    #region Log
    public class ConfigSectionLog : ConfigurationSection
    {
        [ConfigurationProperty("IsOn", DefaultValue = false)]
        public bool IsOn
        {
            get
            {
                return (bool)this["IsOn"];
            }
            set
            {
                this["IsOn"] = value;
            }
        }
    }
    #endregion

    #region DataLog
    public class ConfigSectionDataLog : ConfigurationSection
    {
        [ConfigurationProperty("IsOn", DefaultValue = false)]
        public bool IsOn
        {
            get
            {
                return (bool)this["IsOn"];
            }
            set
            {
                this["IsOn"] = value;
            }
        }
    }
    #endregion

    #region System
    public class ConfigSectionSystem : ConfigurationSection
    {
        [ConfigurationProperty("AppId")]
        public int AppId
        {
            get
            {
                return (int)this["AppId"];
            }
            set
            {
                this["AppId"] = value;
            }
        }

        [ConfigurationProperty("ShowConnectParamsEditor", DefaultValue = false)]
        public bool ShowConnectParamsEditor
        {
            get
            {
                return (bool)this["ShowConnectParamsEditor"];
            }
            set
            {
                this["ShowConnectParamsEditor"] = value;
            }
        }

        [ConfigurationProperty("Modules")]
        public SystemModule Modules
        {
            get
            {
                return (SystemModule)base["Modules"];
            }
        }

        [ConfigurationProperty("Connections")]
        public SystemConnectionCollection Connections
        {
            get
            {
                return (SystemConnectionCollection)base["Connections"];
            }
        }

        [ConfigurationProperty("Clients")]
        public SystemClientCollection Clients
        {
            get
            {
                return (SystemClientCollection)base["Clients"];
            }
        }

        [ConfigurationProperty("ExternalApi")]
        public SystemExternalApi ExternalApi
        {
            get
            {
                return (SystemExternalApi)base["ExternalApi"];
            }
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public SystemIdentityCollection Identities
        {
            get
            {
                return (SystemIdentityCollection)base[""];
            }
        }
        public SystemIdentity Identity
        {
            get
            {
                return this.Identities[(int)Tsb.Security.Web.Identity.Application.Type];
            }
        }

        [ConfigurationProperty("Help")]
        public SystemHelpCollection Help
        {
            get
            {
                return (SystemHelpCollection)base["Help"];
            }
        }

        [ConfigurationProperty("BillBerry")]
        public SystemBillBerry BillBerry
        {
            get
            {
                return (SystemBillBerry)base["BillBerry"];
            }
        }

        [ConfigurationProperty("Api")]
        public SystemApiCollection Api
        {
            get
            {
                return (SystemApiCollection)base["Api"];
            }
        }

        [ConfigurationProperty("Printers")]
        public SystemPrinterCollection Printers
        {
            get
            {
                return (SystemPrinterCollection)base["Printers"];
            }
        }

        [ConfigurationProperty("DocumentStorageFilePaths")]
        public DocumentStorageFilePathCollection DocumentStorageFilePaths
        {
            get
            {
                return (DocumentStorageFilePathCollection)base["DocumentStorageFilePaths"];
            }
        }

        [ConfigurationProperty("Report")]
        public ConfigElementReport Report
        {
            get
            {
                return (ConfigElementReport)base["Report"];
            }
        }
    }


    public class ConfigElementReport : ConfigurationElement
    {
        [ConfigurationProperty("Connections")]
        public DataSourceConnectionCollection Connections
        {
            get
            {
                return (DataSourceConnectionCollection)base["Connections"];
            }
        }
    }


    #region Connections
    [ConfigurationCollection(typeof(SystemConnection))]
    public class SystemConnectionCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("DefaultConnection", IsRequired = true)]
        public string DefaultConnection
        {
            get
            {
                return (string)base["DefaultConnection"];
            }
            set
            {
                base["DefaultConnection"] = value;
            }
        }
        [ConfigurationProperty("UseDefaultConnection")]
        public bool UseDefaultConnection
        {
            get
            {
                return (bool)base["UseDefaultConnection"];
            }
            set
            {
                base["UseDefaultConnection"] = value;
            }
        }
        [ConfigurationProperty("MasterConnection")]
        public string MasterConnection
        {
            get
            {
                return (string)base["MasterConnection"];
            }
            set
            {
                base["MasterConnection"] = value;
            }
        }
        public bool MasterConnectionSpecified
        {
            get
            {
                return this.ElementInformation.Properties["MasterConnection"].ValueOrigin != System.Configuration.PropertyValueOrigin.Default;
            }
        }

        public new SystemConnection this[string name]
        {
            get
            {
                return (SystemConnection)base.BaseGet(name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SystemConnection();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SystemConnection)element).Name;
        }
        public void Add(SystemConnection systemConnection)
        {
            base.BaseAdd(systemConnection);
        }
        public void Clear()
        {
            base.BaseClear();
        }
        public void Remove(SystemConnection systemConnection)
        {
            if (base.BaseIndexOf(systemConnection) >= 0)
            {
                base.BaseRemove(systemConnection.Name);
            }
        }
        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }

    public class SystemConnection : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base["Name"];
            }
            set
            {
                base["Name"] = value;
            }
        }
        [ConfigurationProperty("FriendlyName")]
        public string FriendlyName
        {
            get
            {
                return (string)base["FriendlyName"];
            }
            set
            {
                base["FriendlyName"] = value;
            }
        }
        [ConfigurationProperty("Modules")]
        [RegexStringValidator(@"(\d+(,)?)?")]
        public string Modules
        {
            get
            {
                return (string)base["Modules"];
            }
            set
            {
                base["Modules"] = value;
            }
        }
        //public string[] ModuleList
        //{
        //    get
        //    {
        //        return this.Modules.Split(',');
        //    }
        //}
        [ConfigurationProperty("DeveloperMode")]
        public bool DeveloperMode
        {
            get
            {
                return (bool)base["DeveloperMode"];
            }
            set
            {
                base["DeveloperMode"] = value;
            }
        }
        public bool DeveloperModeSpecified
        {
            get
            {
                return this.ElementInformation.Properties["DeveloperMode"].ValueOrigin != System.Configuration.PropertyValueOrigin.Default;
            }
        }
        [ConfigurationProperty("ExcludeFromSync")]
        public bool ExcludeFromSync
        {
            get
            {
                return (bool)base["ExcludeFromSync"];
            }
            set
            {
                base["ExcludeFromSync"] = value;
            }
        }
        public bool ExcludeFromSyncSpecified
        {
            get
            {
                return this.ElementInformation.Properties["ExcludeFromSync"].ValueOrigin != System.Configuration.PropertyValueOrigin.Default;
            }
        }
        [ConfigurationProperty("AllowEditImportData")]
        public bool AllowEditImportData
        {
            get
            {
                return (bool)base["AllowEditImportData"];
            }
            set
            {
                base["AllowEditImportData"] = value;
            }
        }
        public bool AllowEditImportDataSpecified
        {
            get
            {
                return this.ElementInformation.Properties["AllowEditImportData"].ValueOrigin != System.Configuration.PropertyValueOrigin.Default;
            }
        }
    }
    #endregion

    #region Clients
    [ConfigurationCollection(typeof(SystemClient))]
    public class SystemClientCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("DefaultClient", IsRequired = true)]
        public int DefaultClient
        {
            get
            {
                return (int)base["DefaultClient"];
            }
            set
            {
                base["DefaultClient"] = value;
            }
        }
        [ConfigurationProperty("UseDefaultClient")]
        public bool UseDefaultClient
        {
            get
            {
                return (bool)base["UseDefaultClient"];
            }
            set
            {
                base["UseDefaultClient"] = value;
            }
        }
        
        public SystemClient this[int id]
        {
            get
            {
                return (SystemClient)base.BaseGet(key: id);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SystemClient();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SystemClient)element).Id;
        }
        public void Add(SystemClient systemClient)
        {
            base.BaseAdd(systemClient);
        }
        public void Clear()
        {
            base.BaseClear();
        }
        public void Remove(SystemClient systemClient)
        {
            if (base.BaseIndexOf(systemClient) >= 0)
            {
                base.BaseRemove(systemClient.Id);
            }
        }
        public void Remove(int id)
        {
            base.BaseRemove(id);
        }
    }

    public class SystemClient : ConfigurationElement
    {
        [ConfigurationProperty("Id", IsRequired = true, IsKey = true)]
        public int Id
        {
            get
            {
                return (int)base["Id"];
            }
            set
            {
                base["Id"] = value;
            }
        }
        [ConfigurationProperty("FriendlyName")]
        public string FriendlyName
        {
            get
            {
                return (string)base["FriendlyName"];
            }
            set
            {
                base["FriendlyName"] = value;
            }
        }
    }
    #endregion

    #region Modules
    public class SystemModule : ConfigurationElement
    {
        [ConfigurationProperty("DefaultModule", IsRequired = true)]
        public int DefaultModule
        {
            get
            {
                return (int)base["DefaultModule"];
            }
            set
            {
                base["DefaultModule"] = value;
            }
        }
        [ConfigurationProperty("UseDefaultModule")]
        public bool UseDefaultModule
        {
            get
            {
                return (bool)base["UseDefaultModule"];
            }
            set
            {
                base["UseDefaultModule"] = value;
            }
        }
    }
    #endregion

    #region Help
    [ConfigurationCollection(typeof(SystemHelp), AddItemName = "Source")]
    public class SystemHelpCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("DefaultSource", IsRequired = true)]
        public int DefaultSource
        {
            get
            {
                return (int)base["DefaultSource"];
            }
            set
            {
                base["DefaultConnection"] = value;
            }
        }

        public new SystemHelp this[int id]
        {
            get
            {
                return (SystemHelp)base.BaseGet(key: id);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SystemHelp();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SystemHelp)element).Id;
        }
        public void Add(SystemHelp item)
        {
            base.BaseAdd(item);
        }
        public void Clear()
        {
            base.BaseClear();
        }
        public void Remove(SystemHelp item)
        {
            if (base.BaseIndexOf(item) >= 0)
            {
                base.BaseRemove(item.Id);
            }
        }
        public void Remove(int id)
        {
            base.BaseRemove(id);
        }
    }
    
    public class SystemHelp : ConfigurationElement
    {
        [ConfigurationProperty("Id", IsRequired = true, IsKey = true)]
        public int Id
        {
            get
            {
                return (int)base["Id"];
            }
            set
            {
                base["Id"] = value;
            }
        }

        [ConfigurationProperty("Uri", DefaultValue = "/")]
        [StringValidator(MinLength = 1)]
        public string Uri
        {
            get
            {
                return (string)base["Uri"];
            }
            set
            {
                base["Uri"] = value;
            }
        }

        [ConfigurationProperty("Version")]
        public int Version
        {
            get
            {
                return (int)base["Version"];
            }
            set
            {
                base["Version"] = value;
            }
        }
        public bool VersionSpecified
        {
            get
            {
                return this.ElementInformation.Properties["Version"].ValueOrigin != PropertyValueOrigin.Default;
            }
        }

        [ConfigurationProperty("TimeOut")]
        public int TimeOut
        {
            get
            {
                return (int)base["TimeOut"];
            }
            set
            {
                base["TimeOut"] = value;
            }
        }

        [ConfigurationProperty("Credentials")]
        public UserNameCredentials Credentials
        {
            get
            {
                return (UserNameCredentials)base["Credentials"];
            }
            set
            {
                base["Credentials"] = value;
            }
        }
    }
    #endregion

    #region BillBerry
    public class SystemBillBerry : ConfigurationElement
    {
        [ConfigurationProperty("Uri", IsRequired = true)]
        public string Uri
        {
            get
            {
                return (string)base["Uri"];
            }
            set
            {
                base["Uri"] = value;
            }
        }

        [ConfigurationProperty("UseApi", DefaultValue = true)]
        public bool UseApi
        {
            get
            {
                return (bool)this["UseApi"];
            }
            set
            {
                this["UseApi"] = value;
            }
        }

        [ConfigurationProperty("Transport", DefaultValue = SystemApiTransport.Http)]
        public SystemApiTransport Transport
        {
            get
            {
                return (SystemApiTransport)this["Transport"];
            }
            set
            {
                this["UseApi"] = Transport;
            }
        }

        [ConfigurationProperty("Stamp")]
        public Guid Stamp
        {
            get
            {
                return (Guid)base["Stamp"];
            }
            set
            {
                base["Stamp"] = value;
            }
        }
        public bool StampSpecified
        {
            get
            {
                return this.ElementInformation.Properties["Stamp"].ValueOrigin != PropertyValueOrigin.Default;
            }
        }

        [ConfigurationProperty("Amqp")]
        public SystemApiTransportAmqp TransportAmqp
        {
            get
            {
                return (SystemApiTransportAmqp)base["Amqp"];
            }
        }
    }

    public class SystemApiTransportAmqp : ConfigurationElement
    {
        [ConfigurationProperty("Queue", IsRequired = true)]
        public string Queue
        {
            get
            {
                return (string)base["Queue"];
            }
            set
            {
                base["Queue"] = value;
            }
        }

        [ConfigurationProperty("Connection")]
        public string Connection
        {
            get
            {
                return (string)base["Connection"];
            }
            set
            {
                base["Connection"] = value;
            }
        }
        public bool ConnectionSpecified
        {
            get
            {
                return this.ElementInformation.Properties["Connection"].ValueOrigin != PropertyValueOrigin.Default;
            }
        }
    }

    public enum SystemApiTransport
    {
        Http = 1,
        Amqp = 2,
        HttpViaAmqp = 3,
    }
    #endregion

    #region ExternalApi
    public class SystemExternalApi : ConfigurationElement
    {
        [ConfigurationProperty("Uri")]
        public string Uri
        {
            get
            {
                return (string)base["Uri"];
            }
            set
            {
                base["Uri"] = value;
            }
        }

        [ConfigurationProperty("Username")]
        public string Username
        {
            get
            {
                return (string)base["Username"];
            }
            set
            {
                base["Username"] = value;
            }
        }

        [ConfigurationProperty("Password")]
        public string Password
        {
            get
            {
                return (string)base["Password"];
            }
            set
            {
                base["Password"] = value;
            }
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public SystemExternalApiOperationCollection Operations
        {
            get
            {
                return (SystemExternalApiOperationCollection)base[""];
            }
        }
    }

    [ConfigurationCollection(typeof(SystemExternalApiOperation))]
    public class SystemExternalApiOperationCollection : ConfigurationElementCollection
    {
        public SystemExternalApiOperation this[string name]
        {
            get
            {
                return (SystemExternalApiOperation)base.BaseGet(key: name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SystemExternalApiOperation();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SystemExternalApiOperation)element).Name;
        }
        public void Add(SystemExternalApiOperation operation)
        {
            base.BaseAdd(operation);
        }
        public void Clear()
        {
            base.BaseClear();
        }
        public void Remove(SystemExternalApiOperation operation)
        {
            if (base.BaseIndexOf(operation) >= 0)
            {
                base.BaseRemove(operation.Name);
            }
        }
        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }

    public class SystemExternalApiOperation : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base["Name"];
            }
            set
            {
                base["Name"] = value;
            }
        }

        [ConfigurationProperty("Uri")]
        public string Uri
        {
            get
            {
                return (string)base["Uri"];
            }
            set
            {
                base["Uri"] = value;
            }
        }
    }
    #endregion

    #region Identity
    [ConfigurationCollection(typeof(SystemIdentity), AddItemName = "Identity")]
    public class SystemIdentityCollection : ConfigurationElementCollection
    {
        public new SystemIdentity this[int type]
        {
            get
            {
                return (SystemIdentity)base.BaseGet(key: type);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SystemIdentity();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SystemIdentity)element).AppType;
        }
        public void Add(SystemIdentity item)
        {
            base.BaseAdd(item);
        }
        public void Clear()
        {
            base.BaseClear();
        }
        public void Remove(SystemIdentity item)
        {
            if (base.BaseIndexOf(item) >= 0)
            {
                base.BaseRemove(item.AppType);
            }
        }
        public void Remove(int type)
        {
            base.BaseRemove(type);
        }
    }

    public class SystemIdentity : ConfigurationElement
    {

        [ConfigurationProperty("AppType",
#if LIK
            DefaultValue = (int)Tsb.Security.Web.Identity.ApplicationType.Personal,
#else
            DefaultValue = (int)Tsb.Security.Web.Identity.ApplicationType.Corporate,
#endif
            IsKey = true)]
        public int AppType
        {
            get
            {
                return (int)base["AppType"];
            }
            set
            {
                base["AppType"] = value;
            }
        }

        [ConfigurationProperty("LoginBy", DefaultValue = SystemLoginBy.Email)]
        public SystemLoginBy LoginBy
        {
            get
            {
                return (SystemLoginBy)base["LoginBy"];
            }
            set
            {
                base["LoginBy"] = value;
            }
        }

        [ConfigurationProperty("UseAsName", DefaultValue = SystemUseAsName.Email)]
        public SystemUseAsName UseAsName
        {
            get
            {
                return (SystemUseAsName)base["UseAsName"];
            }
            set
            {
                base["UseAsName"] = value;
            }
        }

        [ConfigurationProperty("AutoGeneratePassword", DefaultValue = false)]
        public bool AutoGeneratePassword
        {
            get
            {
                return (bool)base["AutoGeneratePassword"];
            }
            set
            {
                base["AutoGeneratePassword"] = value;
            }
        }

        [ConfigurationProperty("KeepRegInfo", DefaultValue = true)]
        public bool KeepRegInfo
        {
            get
            {
                return (bool)base["KeepRegInfo"];
            }
            set
            {
                base["KeepRegInfo"] = value;
            }
        }

        [ConfigurationProperty("AppUri")]
        public string AppUri
        {
            get
            {
                return (string)base["AppUri"];
            }
            set
            {
                base["AppUri"] = value;
            }
        }
        public bool AppUriSpecified
        {
            get
            {
                return this.ElementInformation.Properties["AppUri"].ValueOrigin != PropertyValueOrigin.Default;
            }
        }

        [ConfigurationProperty("Session")]
        public SystemSession Session
        {
            get
            {
                return (SystemSession)base["Session"];
            }
        }
        
        [ConfigurationProperty("UserValidator")]
        public SystemUserValidator UserValidator
        {
            get
            {
                return (SystemUserValidator)base["UserValidator"];
            }
        }

        [ConfigurationProperty("PasswordValidator")]
        public SystemPasswordValidator PasswordValidator
        {
            get
            {
                return (SystemPasswordValidator)base["PasswordValidator"];
            }
        }

        [ConfigurationProperty("PasswordGenerator")]
        public SystemPasswordGenerator PasswordGenerator
        {
            get
            {
                return (SystemPasswordGenerator)base["PasswordGenerator"];
            }
        }

        [ConfigurationProperty("Lockout")]
        public SystemLockout Lockout
        {
            get
            {
                return (SystemLockout)base["Lockout"];
            }
        }

        [ConfigurationProperty("TwoFactor")]
        public SystemTwoFactor TwoFactor
        {
            get
            {
                return (SystemTwoFactor)base["TwoFactor"];
            }
        }

        [ConfigurationProperty("EmailService")]
        public SystemEmailService EmailService
        {
            get
            {
                return (SystemEmailService)base["EmailService"];
            }
        }

        [ConfigurationProperty("SmsService")]
        public SystemSmsService SmsService
        {
            get
            {
                return (SystemSmsService)base["SmsService"];
            }
        }

        [ConfigurationProperty("ThirdPartyAuthentication")]
        public SystemThirdPartyAuthentication ThirdPartyAuthentication
        {
            get
            {
                return (SystemThirdPartyAuthentication)base["ThirdPartyAuthentication"];
            }
        }
    }

    public class SystemUserValidator : ConfigurationElement
    {
        [ConfigurationProperty("AllowOnlyAlphanumericUserNames", DefaultValue = false)]
        public bool AllowOnlyAlphanumericUserNames
        {
            get
            {
                return (bool)base["AllowOnlyAlphanumericUserNames"];
            }
            set
            {
                base["AllowOnlyAlphanumericUserNames"] = value;
            }
        }

        [ConfigurationProperty("RequireEmail", DefaultValue = true)]
        public bool RequireEmail
        {
            get
            {
                return (bool)base["RequireEmail"];
            }
            set
            {
                base["RequireEmail"] = value;
            }
        }

        [ConfigurationProperty("RequireUniqueEmail", DefaultValue = true)]
        public bool RequireUniqueEmail
        {
            get
            {
                return (bool)base["RequireUniqueEmail"];
            }
            set
            {
                base["RequireUniqueEmail"] = value;
            }
        }

        [ConfigurationProperty("RequireConfirmedEmail", DefaultValue = true)]
        public bool RequireConfirmedEmail
        {
            get
            {
                return (bool)base["RequireConfirmedEmail"];
            }
            set
            {
                base["RequireConfirmedEmail"] = value;
            }
        }

        [ConfigurationProperty("RequirePhoneNumber", DefaultValue = false)]
        public bool RequirePhoneNumber
        {
            get
            {
                return (bool)base["RequirePhoneNumber"];
            }
            set
            {
                base["RequirePhoneNumber"] = value;
            }
        }

        [ConfigurationProperty("RequireUniquePhoneNumber", DefaultValue = false)]
        public bool RequireUniquePhoneNumber
        {
            get
            {
                return (bool)base["RequireUniquePhoneNumber"];
            }
            set
            {
                base["RequireUniquePhoneNumber"] = value;
            }
        }

        [ConfigurationProperty("RequireConfirmedPhoneNumber", DefaultValue = false)]
        public bool RequireConfirmedPhoneNumber
        {
            get
            {
                return (bool)base["RequireConfirmedPhoneNumber"];
            }
            set
            {
                base["RequireConfirmedPhoneNumber"] = value;
            }
        }

        [ConfigurationProperty("RequireFullName", DefaultValue = true)]
        public bool RequireFullName
        {
            get
            {
                return (bool)base["RequireFullName"];
            }
            set
            {
                base["RequireFullName"] = value;
            }
        }

        [ConfigurationProperty("RequireCyrillicName", DefaultValue = true)]
        public bool RequireCyrillicName
        {
            get
            {
                return (bool)base["RequireCyrillicName"];
            }
            set
            {
                base["RequireCyrillicName"] = value;
            }
        }
    }

    public class SystemPasswordValidator : ConfigurationElement
    {
        [ConfigurationProperty("RequiredLength", DefaultValue = 8)]
        public int RequiredLength
        {
            get
            {
                return (int)base["RequiredLength"];
            }
            set
            {
                base["RequiredLength"] = value;
            }
        }

        [ConfigurationProperty("RequireNonLetterOrDigit", DefaultValue = false)]
        public bool RequireNonLetterOrDigit
        {
            get
            {
                return (bool)base["RequireNonLetterOrDigit"];
            }
            set
            {
                base["RequireNonLetterOrDigit"] = value;
            }
        }

        [ConfigurationProperty("RequireDigit", DefaultValue = false)]
        public bool RequireDigit
        {
            get
            {
                return (bool)base["RequireDigit"];
            }
            set
            {
                base["RequireDigit"] = value;
            }
        }

        [ConfigurationProperty("RequireLowercase", DefaultValue = false)]
        public bool RequireLowercase
        {
            get
            {
                return (bool)base["RequireLowercase"];
            }
            set
            {
                base["RequireLowercase"] = value;
            }
        }

        [ConfigurationProperty("RequireUppercase", DefaultValue = false)]
        public bool RequireUppercase
        {
            get
            {
                return (bool)base["RequireUppercase"];
            }
            set
            {
                base["RequireUppercase"] = value;
            }
        }
    }

    public class SystemPasswordGenerator : ConfigurationElement
    {
        [ConfigurationProperty("MaxLength", DefaultValue = 8)]
        public int MaxLength
        {
            get
            {
                return (int)base["MaxLength"];
            }
            set
            {
                base["MaxLength"] = value;
            }
        }

        [ConfigurationProperty("WithoutNonLetterOrDigit", DefaultValue = false)]
        public bool WithoutNonLetterOrDigit
        {
            get
            {
                return (bool)base["WithoutNonLetterOrDigit"];
            }
            set
            {
                base["WithoutNonLetterOrDigit"] = value;
            }
        }

        [ConfigurationProperty("WithoutDigit", DefaultValue = false)]
        public bool WithoutDigit
        {
            get
            {
                return (bool)base["WithoutDigit"];
            }
            set
            {
                base["WithoutDigit"] = value;
            }
        }

        [ConfigurationProperty("WithoutLowercase", DefaultValue = false)]
        public bool WithoutLowercase
        {
            get
            {
                return (bool)base["WithoutLowercase"];
            }
            set
            {
                base["WithoutLowercase"] = value;
            }
        }

        [ConfigurationProperty("WithoutUppercase", DefaultValue = false)]
        public bool WithoutUppercase
        {
            get
            {
                return (bool)base["WithoutUppercase"];
            }
            set
            {
                base["WithoutUppercase"] = value;
            }
        }
    }

    public class SystemLockout : ConfigurationElement
    {
        [ConfigurationProperty("Enabled", DefaultValue = true)]
        public bool Enabled
        {
            get
            {
                return (bool)base["Enabled"];
            }
            set
            {
                base["Enabled"] = value;
            }
        }

        [ConfigurationProperty("LockoutInterval", DefaultValue = 300)]
        public int LockoutInterval
        {
            get
            {
                return (int)base["LockoutInterval"];
            }
            set
            {
                base["LockoutInterval"] = value;
            }
        }

        [ConfigurationProperty("MaxFailedAccessAttempts", DefaultValue = 5)]
        public int MaxFailedAccessAttempts
        {
            get
            {
                return (int)base["MaxFailedAccessAttempts"];
            }
            set
            {
                base["MaxFailedAccessAttempts"] = value;
            }
        }
    }
    
    public class SystemTwoFactor : ConfigurationElement
    {
        [ConfigurationProperty("Enabled", DefaultValue = false)]
        public bool Enabled
        {
            get
            {
                return (bool)base["Enabled"];
            }
            set
            {
                base["Enabled"] = value;
            }
        }
    }

    public class SystemSmsService : ConfigurationElement
    {
        [ConfigurationProperty("Provider")]
        public string Provider
        {
            get
            {
                return (string)base["Provider"];
            }
            set
            {
                base["Provider"] = value;
            }
        }
    }

    public class SystemEmailService : ConfigurationElement
    {
        [ConfigurationProperty("Provider")]
        public string Provider
        {
            get
            {
                return (string)base["Provider"];
            }
            set
            {
                base["Provider"] = value;
            }
        }

        [ConfigurationProperty("CallbackUri")]
        public string CallbackUri
        {
            get
            {
                return (string)base["CallbackUri"];
            }
            set
            {
                base["CallbackUri"] = value;
            }
        }
        public bool CallbackUriSpecified
        {
            get
            {
                return this.ElementInformation.Properties["CallbackUri"].ValueOrigin != PropertyValueOrigin.Default;
            }
        }
    }

    public class SystemSession : ConfigurationElement
    {
        [ConfigurationProperty("ExpireInterval", DefaultValue = 1440)]
        public int ExpireInterval
        {
            get
            {
                return (int)base["ExpireInterval"];
            }
            set
            {
                base["ExpireInterval"] = value;
            }
        }

        [ConfigurationProperty("SlidingExpiration", DefaultValue = true)]
        public bool SlidingExpiration
        {
            get
            {
                return (bool)base["SlidingExpiration"];
            }
            set
            {
                base["SlidingExpiration"] = value;
            }
        }

        [ConfigurationProperty("ValidateStampInterval", DefaultValue = 60)]
        public int ValidateStampInterval
        {
            get
            {
                return (int)base["ValidateStampInterval"];
            }
            set
            {
                base["ValidateStampInterval"] = value;
            }
        }
    }

    public class SystemThirdPartyAuthentication : ConfigurationElement
    {
        private IList<SystemThirdPartyAuthenticationItem> items;
        
        public ICollection<SystemThirdPartyAuthenticationItem> Items
        {
            get
            {
                if (items == null)
                {
                    var allItems = new List<SystemThirdPartyAuthenticationItem>
                    {
                        this.Windows,
                        this.Google,
                        this.Facebook,
                        this.VK,
                        this.OK,
                        this.Apple,
                        this.Bitrix24,
                        this.Esia,
                        this.BillBerry,
                    };
                    items = allItems.Where(ss => ss.ElementInformation.IsPresent).ToList().AsReadOnly();
                }
                return items;
            }
        }

        [ConfigurationProperty("Windows")]
        public SystemThirdPartyAuthenticationWindows Windows
        {
            get
            {
                return (SystemThirdPartyAuthenticationWindows)base["Windows"];
            }
        }

        [ConfigurationProperty("Google")]
        public SystemThirdPartyAuthenticationGoogle Google
        {
            get
            {
                return (SystemThirdPartyAuthenticationGoogle)base["Google"];
            }
        }

        [ConfigurationProperty("Facebook")]
        public SystemThirdPartyAuthenticationFacebook Facebook
        {
            get
            {
                return (SystemThirdPartyAuthenticationFacebook)base["Facebook"];
            }
        }

        [ConfigurationProperty("VK")]
        public SystemThirdPartyAuthenticationVK VK
        {
            get
            {
                return (SystemThirdPartyAuthenticationVK)base["VK"];
            }
        }

        [ConfigurationProperty("OK")]
        public SystemThirdPartyAuthenticationOK OK
        {
            get
            {
                return (SystemThirdPartyAuthenticationOK)base["OK"];
            }
        }

        [ConfigurationProperty("Bitrix24")]
        public SystemThirdPartyAuthenticationBitrix24 Bitrix24
        {
            get
            {
                return (SystemThirdPartyAuthenticationBitrix24)base["Bitrix24"];
            }
        }

        [ConfigurationProperty("Esia")]
        public SystemThirdPartyAuthenticationEsia Esia
        {
            get
            {
                return (SystemThirdPartyAuthenticationEsia)base["Esia"];
            }
        }

        [ConfigurationProperty("Apple")]
        public SystemThirdPartyAuthenticationApple Apple
        {
            get
            {
                return (SystemThirdPartyAuthenticationApple)base["Apple"];
            }
        }

        [ConfigurationProperty("BillBerry")]
        public SystemThirdPartyAuthenticationBillBerry BillBerry
        {
            get
            {
                return (SystemThirdPartyAuthenticationBillBerry)base["BillBerry"];
            }
        }
    }

    public class SystemThirdPartyAuthenticationItem : ConfigurationElement
    {
        public bool IsWindows { get; set; }

        [ConfigurationProperty("AuthenticationType", IsRequired = true)]
        public string AuthenticationType
        {
            get
            {
                return (string)base["AuthenticationType"];
            }
            set
            {
                base["AuthenticationType"] = value;
            }
        }

        [ConfigurationProperty("Caption")]
        public string Caption
        {
            get
            {
                return (string)base["Caption"];
            }
            set
            {
                base["Caption"] = value;
            }
        }

        [ConfigurationProperty("AllowedOn", DefaultValue = SystemCommunication.Application)]
        public SystemCommunication AllowedOn
        {
            get
            {
                return (SystemCommunication)base["AllowedOn"];
            }
            set
            {
                base["AllowedOn"] = value;
            }
        }
    }

    public class SystemThirdPartyAuthenticationWindows : SystemThirdPartyAuthenticationItem
    {
        public SystemThirdPartyAuthenticationWindows()
        {
            this.AuthenticationType = "Ntlm";
            this.Caption = "Windows";
            this.IsWindows = true;
        }
    }

    public class SystemThirdPartyAuthenticationGoogle : SystemThirdPartyAuthenticationItem
    {
        [ConfigurationProperty("ClientId", IsRequired = true)]
        public string ClientId
        {
            get
            {
                return (string)base["ClientId"];
            }
            set
            {
                base["ClientId"] = value;
            }
        }

        [ConfigurationProperty("ClientSecret", IsRequired = true)]
        public string ClientSecret
        {
            get
            {
                return (string)base["ClientSecret"];
            }
            set
            {
                base["ClientSecret"] = value;
            }
        }

        public SystemThirdPartyAuthenticationGoogle()
        {
            this.AuthenticationType = "Google";
            this.Caption = "Google";
        }
    }

    public class SystemThirdPartyAuthenticationFacebook : SystemThirdPartyAuthenticationItem
    {
        [ConfigurationProperty("ClientId", IsRequired = true)]
        public string ClientId
        {
            get
            {
                return (string)base["ClientId"];
            }
            set
            {
                base["ClientId"] = value;
            }
        }

        [ConfigurationProperty("ClientSecret", IsRequired = true)]
        public string ClientSecret
        {
            get
            {
                return (string)base["ClientSecret"];
            }
            set
            {
                base["ClientSecret"] = value;
            }
        }

        public SystemThirdPartyAuthenticationFacebook()
        {
            this.AuthenticationType = "Facebook";
            this.Caption = "Facebook";
        }
    }

    public class SystemThirdPartyAuthenticationVK : SystemThirdPartyAuthenticationItem
    {
        [ConfigurationProperty("ClientId", IsRequired = true)]
        public string ClientId
        {
            get
            {
                return (string)base["ClientId"];
            }
            set
            {
                base["ClientId"] = value;
            }
        }

        [ConfigurationProperty("ClientSecret", IsRequired = true)]
        public string ClientSecret
        {
            get
            {
                return (string)base["ClientSecret"];
            }
            set
            {
                base["ClientSecret"] = value;
            }
        }

        [ConfigurationProperty("ApiVersion", DefaultValue = "5.131")]
        public string ApiVersion
        {
            get
            {
                return (string)base["ApiVersion"];
            }
            set
            {
                base["ApiVersion"] = value;
            }
        }

        public SystemThirdPartyAuthenticationVK()
        {
            this.AuthenticationType = "VKontakte";
            this.Caption = "VK";
        }
    }

    public class SystemThirdPartyAuthenticationOK : SystemThirdPartyAuthenticationItem
    {
        [ConfigurationProperty("ClientId", IsRequired = true)]
        public string ClientId
        {
            get
            {
                return (string)base["ClientId"];
            }
            set
            {
                base["ClientId"] = value;
            }
        }

        [ConfigurationProperty("ClientSecret", IsRequired = true)]
        public string ClientSecret
        {
            get
            {
                return (string)base["ClientSecret"];
            }
            set
            {
                base["ClientSecret"] = value;
            }
        }

        [ConfigurationProperty("ClientPublic", IsRequired = true)]
        public string ClientPublic
        {
            get
            {
                return (string)base["ClientPublic"];
            }
            set
            {
                base["ClientPublic"] = value;
            }
        }

        public SystemThirdPartyAuthenticationOK()
        {
            this.AuthenticationType = "OK";
            this.Caption = "OK";
        }
    }

    public class SystemThirdPartyAuthenticationBitrix24 : SystemThirdPartyAuthenticationItem
    {
        [ConfigurationProperty("ClientId", IsRequired = true)]
        public string ClientId
        {
            get
            {
                return (string)base["ClientId"];
            }
            set
            {
                base["ClientId"] = value;
            }
        }

        [ConfigurationProperty("ClientSecret", IsRequired = true)]
        public string ClientSecret
        {
            get
            {
                return (string)base["ClientSecret"];
            }
            set
            {
                base["ClientSecret"] = value;
            }
        }

        public SystemThirdPartyAuthenticationBitrix24()
        {
            this.AuthenticationType = "Bitrix24";
            this.Caption = "Bitrix24";
        }
    }

    public class SystemThirdPartyAuthenticationBillBerry : SystemThirdPartyAuthenticationItem
    {
        [ConfigurationProperty("ClientId", IsRequired = true)]
        public string ClientId
        {
            get
            {
                return (string)base["ClientId"];
            }
            set
            {
                base["ClientId"] = value;
            }
        }

        [ConfigurationProperty("ClientSecret", IsRequired = true)]
        public string ClientSecret
        {
            get
            {
                return (string)base["ClientSecret"];
            }
            set
            {
                base["ClientSecret"] = value;
            }
        }

        [ConfigurationProperty("TokenEndpoint", IsRequired = true)]
        public string TokenEndpoint
        {
            get
            {
                return (string)base["TokenEndpoint"];
            }
            set
            {
                base["TokenEndpoint"] = value;
            }
        }

        [ConfigurationProperty("AuthorizationEndpoint", IsRequired = true)]
        public string AuthorizationEndpoint
        {
            get
            {
                return (string)base["AuthorizationEndpoint"];
            }
            set
            {
                base["AuthorizationEndpoint"] = value;
            }
        }

        [ConfigurationProperty("UserInfoEndpoint")]
        public string UserInfoEndpoint
        {
            get
            {
                return (string)base["UserInfoEndpoint"];
            }
            set
            {
                base["UserInfoEndpoint"] = value;
            }
        }

        public SystemThirdPartyAuthenticationBillBerry()
        {
            this.AuthenticationType = "BillBerry";
            this.Caption = "ЛИК";
        }
    }

    public class SystemThirdPartyAuthenticationEsia : SystemThirdPartyAuthenticationItem
    {
        [ConfigurationProperty("UseTestEnvironment", DefaultValue = false)]
        public bool UseTestEnvironment
        {
            get
            {
                return (bool)base["UseTestEnvironment"];
            }
            set
            {
                base["UseTestEnvironment"] = value;
            }
        }


        [ConfigurationProperty("ClientId", IsRequired = true)]
        public string ClientId
        {
            get
            {
                return (string)base["ClientId"];
            }
            set
            {
                base["ClientId"] = value;
            }
        }

        [ConfigurationProperty("Scope")]
        public string Scope
        {
            get
            {
                return (string)base["Scope"];
            }
            set
            {
                base["Scope"] = value;
            }
        }
        public bool ScopeSpecified
        {
            get
            {
                return this.ElementInformation.Properties["Scope"].ValueOrigin != PropertyValueOrigin.Default;
            }
        }

        [ConfigurationProperty("ClientCertificate", IsRequired = true)]
        public SystemCertificateElement ClientCertificate
        {
            get
            {
                return (SystemCertificateElement)base["ClientCertificate"];
            }
        }

        [ConfigurationProperty("EsiaCertificate")]
        public SystemCertificateElement EsiaCertificate
        {
            get
            {
                return (SystemCertificateElement)base["EsiaCertificate"];
            }
        }

        public SystemThirdPartyAuthenticationEsia()
        {
            this.AuthenticationType = "ESIA";
            this.Caption = "ЕСИА";
        }
    }

    public class SystemThirdPartyAuthenticationApple : SystemThirdPartyAuthenticationItem
    {
        [ConfigurationProperty("ClientId", IsRequired = true)]
        public string ClientId
        {
            get
            {
                return (string)base["ClientId"];
            }
            set
            {
                base["ClientId"] = value;
            }
        }

        [ConfigurationProperty("ClientTeamId", IsRequired = true)]
        public string ClientTeamId
        {
            get
            {
                return (string)base["ClientTeamId"];
            }
            set
            {
                base["ClientTeamId"] = value;
            }
        }

        [ConfigurationProperty("ClientSecretId", IsRequired = true)]
        public string ClientSecretId
        {
            get
            {
                return (string)base["ClientSecretId"];
            }
            set
            {
                base["ClientSecretId"] = value;
            }
        }

        [ConfigurationProperty("ClientSecret", IsRequired = true)]
        public string ClientSecret
        {
            get
            {
                return (string)base["ClientSecret"];
            }
            set
            {
                base["ClientSecret"] = value;
            }
        }

        public SystemThirdPartyAuthenticationApple()
        {
            this.AuthenticationType = "Apple";
            this.Caption = "Apple";
        }
    }

    public class SystemCertificateElement : ConfigurationElement
    {
        [ConfigurationProperty("FindValue", DefaultValue = "")]
        [StringValidator(MinLength = 0)]
        public string FindValue
        {
            get
            {
                return (string)base["FindValue"];
            }
            set
            {
                base["FindValue"] = value;
            }
        }

        [ConfigurationProperty("StoreLocation", DefaultValue = StoreLocation.LocalMachine)]
        public StoreLocation StoreLocation
        {
            get
            {
                return (StoreLocation)base["StoreLocation"];
            }
            set
            {
                base["StoreLocation"] = value;
            }
        }

        [ConfigurationProperty("StoreName", DefaultValue = StoreName.My)]
        public StoreName StoreName
        {
            get
            {
                return (StoreName)base["StoreName"];
            }
            set
            {
                base["StoreName"] = value;
            }
        }

        [ConfigurationProperty("X509FindType", DefaultValue = X509FindType.FindByThumbprint)]
        public X509FindType X509FindType
        {
            get
            {
                return (X509FindType)base["X509FindType"];
            }
            set
            {
                base["X509FindType"] = value;
            }
        }

        public X509Certificate2 GetCertificate()
        {
            X509Certificate2 x509Certificate2 = null;
            if (FindValue == null)
            {
                throw new ArgumentException("FindValue");
            }

            var x509CertificateStore = new X509Store(StoreName, StoreLocation);
            try
            {
                x509CertificateStore.Open(OpenFlags.ReadOnly);
                var x509Certificate2Collection = x509CertificateStore.Certificates.Find(X509FindType, FindValue, false);
                if (x509Certificate2Collection.Count > 0)
                {
                    x509Certificate2 = x509Certificate2Collection[0];
                }
            }
            finally
            {
                x509CertificateStore.Close();
            }
            return x509Certificate2;
        }
    }

    //public class SystemIdentityConnections : ConfigurationElement
    //{
    //    [ConfigurationProperty("AuthConnection")]
    //    public SystemIdentityConnection AuthConnection
    //    {
    //        get
    //        {
    //            return (SystemIdentityConnection)base["AuthConnection"];
    //        }
    //    }

    //    [ConfigurationProperty("MainConnection")]
    //    public SystemIdentityConnection MainConnection
    //    {
    //        get
    //        {
    //            return (SystemIdentityConnection)base["MainConnection"];
    //        }
    //    }
    //}

    //public class SystemIdentityConnection : ConfigurationElement
    //{
    //    [ConfigurationProperty("Name", IsRequired = true)]
    //    public string Name
    //    {
    //        get
    //        {
    //            return (string)base["Name"];
    //        }
    //        set
    //        {
    //            base["Name"] = value;
    //        }
    //    }

    //}

    [Flags]
    public enum SystemLoginBy
    {
        Email = 1,
        PhoneNumber = 2,
    }

    public enum SystemUseAsName
    {
        Email = SystemLoginBy.Email,
        PhoneNumber = SystemLoginBy.PhoneNumber,
    }

    [Flags]
    public enum SystemCommunication
    {
        Application = 1,
        Api = 2,
    }
    #endregion

    #region Api
    [ConfigurationCollection(typeof(SystemApi))]
    public class SystemApiCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("AllowedOnAmqp", DefaultValue = false)]
        public bool AllowedOnAmqp
        {
            get
            {
                return (bool)base["AllowedOnAmqp"];
            }
            set
            {
                base["AllowedOnAmqp"] = value;
            }
        }

        [ConfigurationProperty("Amqp")]
        public SystemApiTransportAmqp TransportAmqp
        {
            get
            {
                return (SystemApiTransportAmqp)base["Amqp"];
            }
        }

        [ConfigurationProperty("Stamp")]
        public Guid Stamp
        {
            get
            {
                return (Guid)base["Stamp"];
            }
            set
            {
                base["Stamp"] = value;
            }
        }
        public bool StampSpecified
        {
            get
            {
                return this.ElementInformation.Properties["Stamp"].ValueOrigin != PropertyValueOrigin.Default;
            }
        }

        public new SystemApi this[string name]
        {
            get
            {
                return (SystemApi)base.BaseGet(name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SystemApi();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SystemApi)element).Name;
        }
        public void Add(SystemApi systemApi)
        {
            base.BaseAdd(systemApi);
        }
        public void Clear()
        {
            base.BaseClear();
        }
        public void Remove(SystemApi systemApi)
        {
            if (base.BaseIndexOf(systemApi) >= 0)
            {
                base.BaseRemove(systemApi.Name);
            }
        }
        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }

    public class SystemApi : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base["Name"];
            }
            set
            {
                base["Name"] = value;
            }
        }
    }
    #endregion

    #region base for Printers, DocumentStorageFilePaths

    public class ConfigurationUnaryElementListed : ConfigurationElement
    {
        [ConfigurationProperty("Id", IsRequired = true, IsKey = true)]
        public int Id
        {
            get
            {
                return (int)base["Id"];
            }
            set
            {
                base["Id"] = value;
            }
        }
        [ConfigurationProperty("FriendlyName", IsRequired = true)]
        public string FriendlyName
        {
            get
            {
                return (string)base["FriendlyName"];
            }
            set
            {
                base["FriendlyName"] = value;
            }
        }
        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)base["Name"];
            }
            set
            {
                base["Name"] = value;
            }
        }
    }

    public class UnaryListedCollection<TyElement> : ConfigurationElementCollection
        where TyElement : ConfigurationUnaryElementListed, new()
    {
        [ConfigurationProperty("Default", IsRequired = true)]
        public int Default
        {
            get
            {
                return (int)base["Default"];
            }
            set
            {
                base["Default"] = value;
            }
        }
        public TyElement this[int id]
        {
            get
            {
                return (TyElement)base.BaseGet(key: id);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new TyElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TyElement)element).Id;
        }
        public void Add(ConfigurationUnaryElementListed value)
        {
            base.BaseAdd(value);
        }
        public void Clear()
        {
            base.BaseClear();
        }
        public void Remove(ConfigurationUnaryElementListed value)
        {
            if (base.BaseIndexOf(value) >= 0)
            {
                base.BaseRemove(value.Id);
            }
        }
        public void Remove(int id)
        {
            base.BaseRemove(id);
        }
    }
    
    #endregion 

    #region Printers

    public class SystemPrinter : ConfigurationUnaryElementListed { }

    [ConfigurationCollection(typeof(SystemPrinter))]
    public class SystemPrinterCollection : UnaryListedCollection<SystemPrinter> { }

    #endregion

    #region DocumentStorageFilePaths

    public class DocumentStorageFilePath : ConfigurationUnaryElementListed { }

    [ConfigurationCollection(typeof(DocumentStorageFilePath))]
    public class DocumentStorageFilePathCollection : UnaryListedCollection<DocumentStorageFilePath> { }

    #endregion

    #endregion

    #region Gos / Privat
    public class ConfigSectionGosApi : ConfigurationSection
    {
        [ConfigurationProperty("Connections")]
        public DataSourceConnectionCollection Connections
        {
            get
            {
                return (DataSourceConnectionCollection)base["Connections"];
            }
        }

        [ConfigurationProperty("Api")]
        public GosApi Api
        {
            get
            {
                return (GosApi)base["Api"];
            }
        }
    }

    public class GosApi : ConfigurationElement
    {
        [ConfigurationProperty("ADOConnection", IsRequired = true)]
        public string ADOConnection
        {
            get
            {
                return (string)this["ADOConnection"];
            }
            set
            {
                this["ADOConnection"] = value;
            }
        }

        [ConfigurationProperty("User", DefaultValue = "")]
        public string User
        {
            get
            {
                return (string)this["User"];
            }
            set
            {
                this["User"] = value;
            }
        }

        [ConfigurationProperty("Password", DefaultValue = "")]
        public string Password
        {
            get
            {
                return (string)this["Password"];
            }
            set
            {
                this["Password"] = value;
            }
        }

        [ConfigurationProperty("PrivatePath", DefaultValue = "")]
        public string PrivatePath
        {
            get
            {
                return (string)this["PrivatePath"];
            }
            set
            {
                this["PrivatePath"] = value;
            }
        }

        [ConfigurationProperty("ResourcePath", DefaultValue = "")]
        public string ResourcePath
        {
            get
            {
                return (string)this["ResourcePath"];
            }
            set
            {
                this["ResourcePath"] = value;
            }
        }
    }

    public class ConfigSectionPrivatImport : ConfigurationSection
    {
        [ConfigurationProperty("Connections")]
        public DataSourceConnectionCollection Connections
        {
            get
            {
                return (DataSourceConnectionCollection)base["Connections"];
            }
        }

        [ConfigurationProperty("Report")]
        public PrivatReport Report
        {
            get
            {
                return (PrivatReport)base["Report"];
            }
        }
    }

    public class PrivatReport : ConfigurationElement
    {
        [ConfigurationProperty("Connection", IsRequired = true)]
        public string Connection
        {
            get
            {
                return (string)this["Connection"];
            }
            set
            {
                this["Connection"] = value;
            }
        }

        [ConfigurationProperty("ADOConnection", IsRequired = true)]
        public string ADOConnection
        {
            get
            {
                return (string)this["ADOConnection"];
            }
            set
            {
                this["ADOConnection"] = value;
            }
        }
    }

    [ConfigurationCollection(typeof(DataSourceConnection))]
    public class DataSourceConnectionCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("DefaultConnection", IsRequired = true)]
        public string DefaultConnection
        {
            get
            {
                return (string)base["DefaultConnection"];
            }
            set
            {
                base["DefaultConnection"] = value;
            }
        }

        public new DataSourceConnection this[string name]
        {
            get
            {
                return (DataSourceConnection)base.BaseGet(name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DataSourceConnection();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DataSourceConnection)element).Name;
        }
        public void Add(DataSourceConnection systemConnection)
        {
            base.BaseAdd(systemConnection);
        }
        public void Clear()
        {
            base.BaseClear();
        }
        public void Remove(DataSourceConnection systemConnection)
        {
            if (base.BaseIndexOf(systemConnection) >= 0)
            {
                base.BaseRemove(systemConnection.Name);
            }
        }
        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }

    public class DataSourceConnection : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base["Name"];
            }
            set
            {
                base["Name"] = value;
            }
        }

        [ConfigurationProperty("FriendlyName")]
        public string FriendlyName
        {
            get
            {
                return (string)base["FriendlyName"];
            }
            set
            {
                base["FriendlyName"] = value;
            }
        }
        
        [ConfigurationProperty("DataSourceId")]
        public byte DataSourceId
        {
            get
            {
                return (byte)base["DataSourceId"];
            }
            set
            {
                base["DataSourceId"] = value;
            }
        }
        public bool DataSourceIdSpecified
        {
            get
            {
                return this.ElementInformation.Properties["DataSourceId"].ValueOrigin != System.Configuration.PropertyValueOrigin.Default;
            }
        }
    }

    
    #endregion

    #region Captcha
    public class ConfigSectionCaptcha : ConfigurationSection
    {
        [ConfigurationProperty("DefaultProvider", DefaultValue = "ReCaptchaV2Provider")]
        [StringValidator(MinLength = 1)]
        public string DefaultProvider
        {
            get
            {
                return (string)base["DefaultProvider"];
            }
            set
            {
                base["DefaultProvider"] = value;
            }
        }
        [ConfigurationProperty("Providers")]
        public ProviderSettingsCollection Providers
        {
            get
            {
                return (ProviderSettingsCollection)base["Providers"];
            }
        }
    }
    #endregion

    #region Rabbit
    public class ConfigSectionRabbit : ConfigurationSection
    {
        [ConfigurationProperty("Connections")]
        public RabbitConnections Connections
        {
            get
            {
                return (RabbitConnections)base["Connections"];
            }
        }

        [ConfigurationProperty("Queues")]
        public RabbitQueues Queues
        {
            get
            {
                return (RabbitQueues)base["Queues"];
            }
        }
    }

    [ConfigurationCollection(typeof(RabbitQueue))]
    public class RabbitConnections : ConfigurationElementCollection
    {
        [ConfigurationProperty("DefaultConnection")]
        public string DefaultConnection
        {
            get
            {
                return (string)base["DefaultConnection"];
            }
            set
            {
                base["DefaultConnection"] = value;
            }
        }
        public bool DefaultConnectionSpecified
        {
            get
            {
                return this.ElementInformation.Properties["DefaultConnection"].ValueOrigin != System.Configuration.PropertyValueOrigin.Default;
            }
        }

        public RabbitConnection this[string name]
        {
            get
            {
                return (RabbitConnection)base.BaseGet(name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RabbitConnection();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RabbitConnection)element).Name;
        }
        public void Add(RabbitConnection rabbiеQueue)
        {
            base.BaseAdd(rabbiеQueue);
        }
        public void Clear()
        {
            base.BaseClear();
        }
        public void Remove(RabbitConnection rabbiеQueue)
        {
            if (base.BaseIndexOf(rabbiеQueue) >= 0)
            {
                base.BaseRemove(rabbiеQueue.Name);
            }
        }
        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }

    public class RabbitConnection : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base["Name"];
            }
            set
            {
                base["Name"] = value;
            }
        }

        [ConfigurationProperty("HostName", IsRequired = true)]
        public string HostName
        {
            get
            {
                return (string)base["HostName"];
            }
            set
            {
                base["HostName"] = value;
            }
        }

        [ConfigurationProperty("UserName", IsRequired = true)]
        public string UserName
        {
            get
            {
                return (string)base["UserName"];
            }
            set
            {
                base["UserName"] = value;
            }
        }

        [ConfigurationProperty("Password", IsRequired = true)]
        public string Password
        {
            get
            {
                return (string)base["Password"];
            }
            set
            {
                base["Password"] = value;
            }
        }

        [ConfigurationProperty("VirtualHost", DefaultValue = "/")]
        [StringValidator(MinLength = 1)]
        public string VirtualHost
        {
            get
            {
                return (string)base["VirtualHost"];
            }
            set
            {
                base["VirtualHost"] = value;
            }
        }
    }

    [ConfigurationCollection(typeof(RabbitQueue))]
    public class RabbitQueues : ConfigurationElementCollection
    {
        public RabbitQueue this[string name]
        {
            get
            {
                return (RabbitQueue)base.BaseGet(name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RabbitQueue();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RabbitQueue)element).Name;
        }
        public void Add(RabbitQueue rabbiеQueue)
        {
            base.BaseAdd(rabbiеQueue);
        }
        public void Clear()
        {
            base.BaseClear();
        }
        public void Remove(RabbitQueue rabbiеQueue)
        {
            if (base.BaseIndexOf(rabbiеQueue) >= 0)
            {
                base.BaseRemove(rabbiеQueue.Name);
            }
        }
        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }

    public class RabbitQueue : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base["Name"];
            }
            set
            {
                base["Name"] = value;
            }
        }

        [ConfigurationProperty("QueueName", IsRequired = true)]
        public string QueueName
        {
            get
            {
                return (string)base["QueueName"];
            }
            set
            {
                base["QueueName"] = value;
            }
        }

        [ConfigurationProperty("Passive", DefaultValue = false)]
        public bool Passive
        {
            get
            {
                return (bool)base["Passive"];
            }
            set
            {
                base["Passive"] = value;
            }
        }
    }
    #endregion

    public static class WebConfigurationHelper
    {
        private static string AuthenticationSectionName = "system.web/authentication";
        
        private static string SharePointSectionName = "CustomSectionGroup/ConfigSectionSP";
        private static string EmailSectionName = "CustomSectionGroup/ConfigSectionEmail";
        private static string SMSSectionName = "CustomSectionGroup/ConfigSectionSMS";
        private static string ImportSectionName = "CustomSectionGroup/ConfigSectionImport";
        private static string OutlookSectionName = "CustomSectionGroup/ConfigSectionOutlook";
        private static string SIPSectionName = "CustomSectionGroup/ConfigSectionSIP";
        private static string SharedDirSectionName = "CustomSectionGroup/ConfigSectionSharedDir";
        private static string SyncSectionName = "CustomSectionGroup/ConfigSectionSync";
        private static string TaskSchedulerSectionName = "CustomSectionGroup/ConfigSectionTaskScheduler";
        private static string LogSectionName = "CustomSectionGroup/ConfigSectionLog";
        private static string SystemSectionName = "CustomSectionGroup/ConfigSectionSystem";
        private static string DataLogSectionName = "CustomSectionGroup/ConfigSectionDataLog";
        private static string GosApiSectionName = "CustomSectionGroup/ConfigSectionGosApi";
        private static string PrivatImportSectionName = "CustomSectionGroup/ConfigSectionPrivatImport";
        private static string CaptchaSectionName = "CustomSectionGroup/ConfigSectionCaptcha";
        private static string RabbitSectionName = "CustomSectionGroup/ConfigSectionRabbit";

        public static AuthenticationMode AuthenticationMode
        {
            get
            {
                AuthenticationSection section = GetAuthenticationSection();
                return section != null ? section.Mode : AuthenticationMode.None;
            }
        }

        public static System.Configuration.Configuration GetConfiguration()
        {
            return WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);

//            WebConfigurationFileMap fileMap = new WebConfigurationFileMap();
//            VirtualDirectoryMapping virtualDirectoryMap = new VirtualDirectoryMapping(AppDomain.CurrentDomain.BaseDirectory, true, "Web.config");
//            fileMap.VirtualDirectories.Add("/", virtualDirectoryMap);
//            return WebConfigurationManager.OpenMappedWebConfiguration(fileMap, "/");
        }

        public static ConnectionStringSettingsCollection GetConnectionStrings()
        {
            return WebConfigurationManager.ConnectionStrings;
        }
        public static ConnectionStringSettingsCollection GetConnectionStrings(System.Configuration.Configuration configuration)
        {
            return configuration.ConnectionStrings.ConnectionStrings;
        }
        public static AuthenticationSection GetAuthenticationSection()
        {
            return (AuthenticationSection)WebConfigurationManager.GetSection(AuthenticationSectionName);
        }

        public static ConfigSectionSP GetConfigSectionSP()
        {
            return WebConfigurationManager.GetWebApplicationSection(SharePointSectionName) as ConfigSectionSP;
        }
        public static ConfigSectionSP GetConfigSectionSP(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(SharePointSectionName) as ConfigSectionSP;
        }
        public static ConfigSectionEmail GetConfigSectionEmail()
        {
            return WebConfigurationManager.GetWebApplicationSection(EmailSectionName) as ConfigSectionEmail;
        }
        public static ConfigSectionEmail GetConfigSectionEmail(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(EmailSectionName) as ConfigSectionEmail;
        }
        public static ConfigSectionSMS GetConfigSectionSMS()
        {
            return WebConfigurationManager.GetWebApplicationSection(SMSSectionName) as ConfigSectionSMS;
        }
        public static ConfigSectionSMS GetConfigSectionSMS(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(SMSSectionName) as ConfigSectionSMS;
        }
        public static ConfigSectionImport GetConfigSectionImport()
        {
            return WebConfigurationManager.GetWebApplicationSection(ImportSectionName) as ConfigSectionImport;
        }
        public static ConfigSectionImport GetConfigSectionImport(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(ImportSectionName) as ConfigSectionImport;
        }
        public static ConfigSectionOutlook GetConfigSectionOutlook()
        {
            return WebConfigurationManager.GetWebApplicationSection(OutlookSectionName) as ConfigSectionOutlook;
        }
        public static ConfigSectionOutlook GetConfigSectionOutlook(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(OutlookSectionName) as ConfigSectionOutlook;
        }
        public static ConfigSectionSIP GetConfigSectionSIP()
        {
            return WebConfigurationManager.GetWebApplicationSection(SIPSectionName) as ConfigSectionSIP;
        }
        public static ConfigSectionSIP GetConfigSectionSIP(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(SIPSectionName) as ConfigSectionSIP;
        }
        public static ConfigSectionSharedDir GetConfigSectionSharedDir()
        {
            return WebConfigurationManager.GetWebApplicationSection(SharedDirSectionName) as ConfigSectionSharedDir;
        }
        public static ConfigSectionSharedDir GetConfigSectionSharedDir(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(SharedDirSectionName) as ConfigSectionSharedDir;
        }
        public static ConfigSectionSync GetConfigSectionSync()
        {
            return WebConfigurationManager.GetWebApplicationSection(SyncSectionName) as ConfigSectionSync;
        }
        public static ConfigSectionSync GetConfigSectionSync(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(SyncSectionName) as ConfigSectionSync;
        }
        public static ConfigSectionTaskScheduler GetConfigSectionTaskScheduler()
        {
            return WebConfigurationManager.GetWebApplicationSection(TaskSchedulerSectionName) as ConfigSectionTaskScheduler;
        }
        public static ConfigSectionTaskScheduler GetConfigSectionTaskScheduler(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(TaskSchedulerSectionName) as ConfigSectionTaskScheduler;
        }
        public static ConfigSectionLog GetConfigSectionLog()
        {
            return WebConfigurationManager.GetWebApplicationSection(LogSectionName) as ConfigSectionLog;
        }
        public static ConfigSectionLog GetConfigSectionLog(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(LogSectionName) as ConfigSectionLog;
        }
        public static ConfigSectionDataLog GetConfigSectionDataLog()
        {
            return WebConfigurationManager.GetWebApplicationSection(DataLogSectionName) as ConfigSectionDataLog;
        }
        public static ConfigSectionDataLog GetConfigSectionDataLog(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(DataLogSectionName) as ConfigSectionDataLog;
        }
        public static ConfigSectionGosApi GetConfigSectionGosApi()
        {
            return WebConfigurationManager.GetWebApplicationSection(GosApiSectionName) as ConfigSectionGosApi;
        }

        public static ConfigSectionPrivatImport GetConfigSectionPrivat()
        {
            return WebConfigurationManager.GetWebApplicationSection(PrivatImportSectionName) as ConfigSectionPrivatImport;
        }

        
        public static ConfigSectionSystem GetConfigSectionSystem()
        {
            return WebConfigurationManager.GetWebApplicationSection(SystemSectionName) as ConfigSectionSystem;
        }
        public static ConfigSectionSystem GetConfigSectionSystem(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(SystemSectionName) as ConfigSectionSystem;
        }
        public static ConfigSectionCaptcha GetConfigSectionCaptcha()
        {
            return WebConfigurationManager.GetWebApplicationSection(CaptchaSectionName) as ConfigSectionCaptcha;
        }
        public static ConfigSectionCaptcha GetConfigSectionCaptcha(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(CaptchaSectionName) as ConfigSectionCaptcha;
        }
        public static ConfigSectionRabbit GetConfigSectionRabbit()
        {
            return WebConfigurationManager.GetWebApplicationSection(RabbitSectionName) as ConfigSectionRabbit;
        }
        public static ConfigSectionRabbit GetConfigSectionRabbit(System.Configuration.Configuration configuration)
        {
            return configuration.GetSection(RabbitSectionName) as ConfigSectionRabbit;
        }
    }
}