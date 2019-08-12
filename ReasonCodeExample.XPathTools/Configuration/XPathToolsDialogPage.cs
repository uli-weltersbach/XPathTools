using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

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
        public XPathFormat StatusbarXPathFormatSetting
        {
            get;
            set;
        } = XPathFormat.Generic;

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

        #region Load and initialize settings

        private void LoadCollectionsFromStorage()
        {
            var package = (Package)GetService(typeof(Package));
            if(package == null)
            {
                return;
            }

            using(var userRegistryRoot = package.UserRegistryRoot)
            {
                LoadSettingsFromRegistry(userRegistryRoot);
            }
        }

        private void LoadSettingsFromRegistry(RegistryKey userRegistryRoot)
        {
            var settingsRegistryPath = SettingsRegistryPath;
            var automationObject = AutomationObject;
            var registryKey = userRegistryRoot.OpenSubKey(settingsRegistryPath, true);
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
                    LoadSettingValue(automationObject, propertyName, registryKey);
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

        private void LoadSettingValue(object automationObject, string propertyName, RegistryKey registryKey)
        {
            var property = automationObject.GetType().GetProperty(propertyName);
            if(property == null)
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
                // Delete the value if it can't be deserialized,
                // to prevent old serialization formats from
                // interfering with storing new values.
                registryKey.DeleteValue(propertyName);
                var defaultValue = GetDefaultValue(property);
                property.SetValue(automationObject, defaultValue);
            }
        }

        private TypeConverter GetTypeConverter(string propertyName)
        {
            var property = GetType().GetProperty(propertyName);
            if(property == null)
            {
                throw new InvalidOperationException($"Property '{propertyName}' not found on type '{GetType()}.'");
            }

            var typeConverterAttribute = property.GetCustomAttribute<TypeConverterAttribute>();
            if(typeConverterAttribute == null)
            {
                return TypeDescriptor.GetConverter(property.PropertyType);
            }

            var converterType = Type.GetType(typeConverterAttribute.ConverterTypeName);
            return (TypeConverter)Activator.CreateInstance(converterType);
        }

        private object GetDefaultValue(PropertyInfo property)
        {
            return property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null;
        }

        #endregion

        #region Save settings

        private void SaveAlwaysDisplayedAttributesSettingToStorage(object sender, ListChangedEventArgs e)
        {
            SaveSettingToStorage(nameof(AlwaysDisplayedAttributesSetting), sender);
        }

        private void SavePreferredAttributeCandidatesSettingToStorage(object sender, ListChangedEventArgs e)
        {
            SaveSettingToStorage(nameof(PreferredAttributeCandidatesSetting), sender);
        }

        private void SaveSettingToStorage(string propertyName, object propertyValue)
        {
            var package = (Package)GetService(typeof(Package));
            if(package == null)
            {
                return;
            }

            using(var userRegistryRoot = package.UserRegistryRoot)
            {
                SaveSettingToRegistry(userRegistryRoot, propertyName, propertyValue);
            }
        }

        private void SaveSettingToRegistry(RegistryKey userRegistryRoot, string propertyName, object propertyValue)
        {
            var settingsRegistryPath = SettingsRegistryPath;
            var registryKey = userRegistryRoot.OpenSubKey(settingsRegistryPath, true) ?? userRegistryRoot.CreateSubKey(settingsRegistryPath);
            using(registryKey)
            {
                try
                {
                    var converter = GetTypeConverter(propertyName);
                    var convertedValue = converter.ConvertTo(propertyValue, typeof(string));
                    registryKey.SetValue(propertyName, convertedValue);
                }
                catch(Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }
        }

        #endregion
    }
}
