using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;

namespace ReasonCodeExample.XPathTools.Configuration
{
    internal class XPathToolsDialogPage : DialogPage, IConfiguration
    {
        public XPathToolsDialogPage()
        {
            AlwaysDisplayedAttributesSetting = new BindingList<XPathSetting>();
            PreferredAttributeCandidatesSetting = new BindingList<XPathSetting>();
        }

        [Category("Statusbar")]
        [DisplayName("Statusbar XPath format")]
        [Description("Select the XPath format used in the statusbar.")]
        public XPathFormat? StatusbarXPathFormatSetting
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
        public XPathFormat? StatusbarXPathFormat
        {
            get
            {
                return StatusbarXPathFormatSetting;
            }
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
            if (package == null)
            {
                return;
            }
            using (var userRegistryRoot = package.UserRegistryRoot)
            {
                LoadCollectionsFromRegistry(userRegistryRoot);
            }
        }

        private void LoadCollectionsFromRegistry(RegistryKey userRegistryRoot)
        {
            var settingsRegistryPath = SettingsRegistryPath;
            var automationObject = AutomationObject;
            var registryKey = userRegistryRoot.OpenSubKey(settingsRegistryPath, false);
            if (registryKey == null)
            {
                LoadDefaultSettings();
                return;
            }
            using (registryKey)
            {
                var propertyNames = registryKey.GetValueNames();
                foreach (var propertyName in propertyNames)
                {
                    SetPropertyValue(automationObject, propertyName, registryKey);
                }
            }
        }

        private void LoadDefaultSettings()
        {
            if (PreferredAttributeCandidates.Any())
            {
                return;
            }
            PreferredAttributeCandidates.Add(new XPathSetting { AttributeName = "id" });
            PreferredAttributeCandidates.Add(new XPathSetting { AttributeName = "name" });
            PreferredAttributeCandidates.Add(new XPathSetting { AttributeName = "type" });
        }

        private void SetPropertyValue(object automationObject, string propertyName, RegistryKey registryKey)
        {
            var property = automationObject.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return;
            }

            try
            {
                var storedValue = registryKey.GetValue(propertyName).ToString();
                var converter = GetTypeConverter(propertyName);
                var convertedValue = converter.ConvertFrom(storedValue);
                property.SetValue(automationObject, convertedValue);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        private TypeConverter GetTypeConverter(string propertyName)
        {
            var property = GetType().GetProperty(propertyName);
            if (property == null)
            {
                throw new InvalidOperationException($"Property '{propertyName}' not found on type '{GetType()}.'");
            }
            var typeConverterAttribute = property.GetCustomAttribute<TypeConverterAttribute>();
            if (typeConverterAttribute == null)
            {
                return TypeDescriptor.GetConverter(property.PropertyType);
            }
            var converterType = Type.GetType(typeConverterAttribute.ConverterTypeName);
            return (TypeConverter)Activator.CreateInstance(converterType);
        }

        private void SaveAlwaysDisplayedAttributesSettingToStorage(object sender, ListChangedEventArgs e)
        {
            SaveValueToStorage(nameof(AlwaysDisplayedAttributesSetting), sender);
        }

        private void SavePreferredAttributeCandidatesSettingToStorage(object sender, ListChangedEventArgs e)
        {
            SaveValueToStorage(nameof(PreferredAttributeCandidatesSetting), sender);
        }

        private void SaveValueToStorage(string propertyName, object propertyValue)
        {
            var package = (Package)GetService(typeof(Package));
            if (package == null)
            {
                return;
            }
            using (var userRegistryRoot = package.UserRegistryRoot)
            {
                SaveValueToRegistry(userRegistryRoot, propertyName, propertyValue);
            }
        }

        private void SaveValueToRegistry(RegistryKey userRegistryRoot, string propertyName, object propertyValue)
        {
            var settingsRegistryPath = SettingsRegistryPath;
            var registryKey = userRegistryRoot.OpenSubKey(settingsRegistryPath, true) ?? userRegistryRoot.CreateSubKey(settingsRegistryPath);
            using (registryKey)
            {
                try
                {
                    var converter = GetTypeConverter(propertyName);
                    var convertedValue = converter.ConvertTo(propertyValue, typeof(string));
                    registryKey.SetValue(propertyName, convertedValue);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }
        }
    }
}
