using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Reflection;
using Microsoft.VisualStudio.Shell;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration
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
                var settingsRegistryPath = SettingsRegistryPath;
                var automationObject = AutomationObject;
                var registryKey = userRegistryRoot.OpenSubKey(settingsRegistryPath, false);
                if(registryKey == null)
                {
                    return;
                }
                using(registryKey)
                {
                    var valueNames = registryKey.GetValueNames();
                    foreach(var name in valueNames)
                    {
                        var property = automationObject.GetType().GetProperty(name);
                        if(property == null)
                        {
                            return;
                        }
                        if(property.GetCustomAttribute(typeof(TypeConverterAttribute)) == null)
                        {
                            return;
                        }
                        var convertedValue = registryKey.GetValue(name).ToString();
                        var converter = new SerializableConverter<BindingList<XPathSetting>>();
                        property.SetValue(automationObject, converter.ConvertFrom(convertedValue));
                    }
                }
            }
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
}