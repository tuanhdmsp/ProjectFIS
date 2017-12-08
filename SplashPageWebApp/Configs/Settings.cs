using System.Configuration;
using Microsoft.Ajax.Utilities;

namespace SplashPageWebApp.Configs
{
    public class Settings
    {
        public static SettingsSection _Config = ConfigurationManager.GetSection("WebAppSettingsGroup/WebAppSettings") as SettingsSection;
        public static ConfigurationSection _EntityConfig = ConfigurationManager.GetSection("entityFramework") as ConfigurationSection;


        public static string GetValueOf(string name)
        {
            foreach (SettingElement setting in _Config.SettingsCollection)
            {
                if (setting.Name.Equals(name))
                {
                    return setting.Value;
                }
            }
            return null;
        }
    }

    public class SettingElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }

            set { this["name"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = true, DefaultValue = "")]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }

    [ConfigurationCollection(typeof(SettingElement))]
    public class SettingsCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("foo")]
        public string foo {
            get { return (string) this["foo"]; }
            set { this["foo"] = value; } }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SettingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SettingElement)element).Name;
        }
    }

    public class SettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("name")]
        public string Name {
            get { return (string) this["name"]; }
            set { this["name"] = value; } }

        [ConfigurationProperty("Settings", IsDefaultCollection = true)]
        public SettingsCollection SettingsCollection
        {
            get { return (SettingsCollection)this["Settings"]; }
            set { this["Settings"] = value; }
        }
    }
}