using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    internal class XPathInformationDialogPage : DialogPage, IConfiguration
    {
        public XPathInformationDialogPage()
        {
            AlwaysDisplayedAttributesSetting = new BindingList<XPathSetting>();
            PreferredAttributeCandidatesSetting = new BindingList<XPathSetting>();
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
        public IList<XPathSetting> AlwaysDisplayedAttributes
        {
            get
            {
                return AlwaysDisplayedAttributesSetting;
            }
        }

        [Browsable(false)]
        public IList<XPathSetting> PreferredAttributeCandidates
        {
            get
            {
                return PreferredAttributeCandidatesSetting;
            }
        }

        [Browsable(false)]
        public string StatusbarXPathPreference
        {
            get;
            set;
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
            var package = (Package)GetService(typeof(Package));
            if(package == null)
            {
                return;
            }
            using(var userRegistryRoot = package.UserRegistryRoot)
            {
                LoadCollectionsFromRegistry(userRegistryRoot);
            }
        }

        private void LoadCollectionsFromRegistry(RegistryKey userRegistryRoot)
        {
            var settingsRegistryPath = SettingsRegistryPath;
            var automationObject = AutomationObject;
            var registryKey = userRegistryRoot.OpenSubKey(settingsRegistryPath, false);
            if(registryKey == null)
            {
                LoadDefaultSettings();
                return;
            }
            using(registryKey)
            {
                var propertyNames = registryKey.GetValueNames();
                foreach(var propertyName in propertyNames)
                {
                    SetPropertyValue(automationObject, propertyName, registryKey);
                }
            }
        }

        private void LoadDefaultSettings()
        {
            if(PreferredAttributeCandidates.Any())
            {
                return;
            }
            PreferredAttributeCandidates.Add(new XPathSetting {AttributeName = "id"});
            PreferredAttributeCandidates.Add(new XPathSetting {AttributeName = "name"});
            PreferredAttributeCandidates.Add(new XPathSetting {AttributeName = "type"});
        }

        private void SetPropertyValue(object automationObject, string propertyName, RegistryKey registryKey)
        {
            var property = automationObject.GetType().GetProperty(propertyName);
            if(property == null)
            {
                return;
            }
            if(property.GetCustomAttribute(typeof(TypeConverterAttribute)) == null)
            {
                return;
            }
            var storedValue = registryKey.GetValue(propertyName).ToString();
            var converter = new SerializableConverter<BindingList<XPathSetting>>();
            property.SetValue(automationObject, converter.ConvertFrom(storedValue));
        }

        private void SaveAlwaysDisplayedAttributesSettingToStorage(object sender, ListChangedEventArgs e)
        {
            SaveCollectionToStorage("AlwaysDisplayedAttributesSetting", (BindingList<XPathSetting>)sender);
        }

        private void SavePreferredAttributeCandidatesSettingToStorage(object sender, ListChangedEventArgs e)
        {
            SaveCollectionToStorage("PreferredAttributeCandidatesSetting", (BindingList<XPathSetting>)sender);
        }

        private void SaveCollectionToStorage(string propertyName, BindingList<XPathSetting> settings)
        {
            var package = (Package)GetService(typeof(Package));
            if(package == null)
            {
                return;
            }
            using(var userRegistryRoot = package.UserRegistryRoot)
            {
                SaveCollectionToRegistry(userRegistryRoot, propertyName, settings);
            }
        }

        private void SaveCollectionToRegistry(RegistryKey userRegistryRoot, string propertyName, BindingList<XPathSetting> settings)
        {
            var settingsRegistryPath = SettingsRegistryPath;
            var registryKey = userRegistryRoot.OpenSubKey(settingsRegistryPath, true) ?? userRegistryRoot.CreateSubKey(settingsRegistryPath);
            using(registryKey)
            {
                var converter = new SerializableConverter<BindingList<XPathSetting>>();
                var convertedValue = converter.ConvertTo(settings, typeof(string));
                registryKey.SetValue(propertyName, convertedValue);
            }
        }
    }
}