using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Reflection;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration
{
    internal class XPathInformationConfiguration : DialogPage, IConfiguration
    {
        public XPathInformationConfiguration()
        {
            AlwaysDisplayedAttributesSetting = new BindingList<XPathSetting>();
            PreferredAttributeCandidatesSetting = new BindingList<XPathSetting>();
        }

        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();
            LoadCollectionsFromStorage();
            AlwaysDisplayedAttributesSetting.RaiseListChangedEvents = true;
            AlwaysDisplayedAttributesSetting.ListChanged += SaveAlwaysDisplayedAttributesSettingToStorage;
            PreferredAttributeCandidatesSetting.RaiseListChangedEvents = true;
            PreferredAttributeCandidatesSetting.ListChanged += SavePreferredAttributeCandidatesSettingToStorage;
        }

        private void LoadCollectionsFromStorage()
        {
            Package package = (Package)this.GetService(typeof(Package));
            if (package == null)
                return;
            using (RegistryKey userRegistryRoot = package.UserRegistryRoot)
            {
                string settingsRegistryPath = this.SettingsRegistryPath;
                object automationObject = this.AutomationObject;
                RegistryKey registryKey = userRegistryRoot.OpenSubKey(settingsRegistryPath, false);
                if (registryKey == null)
                    return;
                using (registryKey)
                {
                    string[] valueNames = registryKey.GetValueNames();
                    foreach (string name in valueNames)
                    {
                        PropertyInfo property = automationObject.GetType().GetProperty(name);
                        if (property == null)
                            return;
                        if (property.GetCustomAttribute(typeof(TypeConverterAttribute)) == null)
                            return;
                        string convertedValue = registryKey.GetValue(name).ToString();
                        SerializableConverter<BindingList<XPathSetting>> converter = new SerializableConverter<BindingList<XPathSetting>>();
                        property.SetValue(automationObject, converter.ConvertFrom(convertedValue));
                    }
                }
            }
        }

        private void SaveAlwaysDisplayedAttributesSettingToStorage(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType != ListChangedType.Reset)
                SaveCollectionToStorage("AlwaysDisplayedAttributesSetting", (BindingList<XPathSetting>)sender);
        }

        private void SavePreferredAttributeCandidatesSettingToStorage(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType != ListChangedType.Reset)
                SaveCollectionToStorage("PreferredAttributeCandidatesSetting", (BindingList<XPathSetting>)sender);
        }

        private void SaveCollectionToStorage(string propertyName, BindingList<XPathSetting> settings)
        {
            Package package = (Package)this.GetService(typeof(Package));
            if (package == null)
                return;
            using (RegistryKey userRegistryRoot = package.UserRegistryRoot)
            {
                string settingsRegistryPath = this.SettingsRegistryPath;
                RegistryKey registryKey = userRegistryRoot.OpenSubKey(settingsRegistryPath, true) ?? userRegistryRoot.CreateSubKey(settingsRegistryPath);
                using (registryKey)
                {
                    SerializableConverter<BindingList<XPathSetting>> converter = new SerializableConverter<BindingList<XPathSetting>>();
                    object convertedValue = converter.ConvertTo(settings, typeof(string));
                    registryKey.SetValue(propertyName, convertedValue);
                }
            }
        }

        [Browsable(false)]
        public static IConfiguration Current
        {
            get;
            set;
        }

        [Category("Generic XPath")]
        [DisplayName("Always displayed attributes")]
        [Description("Specify attributes which should always be displayed in the XPath.\nE.g. adding the entry \"{http://reasoncodeexample.com/}person/@name\" will display the \"name\"-attribute on all \"person\"-elements in the XML-namespace \"http://reasoncodeexample.com/\".")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(SerializableConverter<BindingList<XPathSetting>>))]
        public BindingList<XPathSetting> AlwaysDisplayedAttributesSetting
        {
            get;
            private set;
        }

        [Browsable(false)]
        public IList<XPathSetting> AlwaysDisplayedAttributes
        {
            get { return AlwaysDisplayedAttributesSetting; }
        }

        [Category("Distinct XPath")]
        [DisplayName("Preferred attribute candidates")]
        [Description("Specify which attributes to use when attempting to determine a \"distinct XPath\" to a node.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(SerializableConverter<BindingList<XPathSetting>>))]
        public BindingList<XPathSetting> PreferredAttributeCandidatesSetting
        {
            get;
            private set;
        }

        [Browsable(false)]
        public IList<XPathSetting> PreferredAttributeCandidates
        {
            get { return PreferredAttributeCandidatesSetting; }
        }
    }
}