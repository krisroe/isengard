﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IsengardClient {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.5.0.0")]
    internal sealed partial class IsengardSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static IsengardSettings defaultInstance = ((IsengardSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new IsengardSettings())));
        
        public static IsengardSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DefaultWeapon {
            get {
                return ((string)(this["DefaultWeapon"]));
            }
            set {
                this["DefaultWeapon"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DefaultRealm {
            get {
                return ((string)(this["DefaultRealm"]));
            }
            set {
                this["DefaultRealm"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Good")]
        public string PreferredAlignment {
            get {
                return ((string)(this["PreferredAlignment"]));
            }
            set {
                this["PreferredAlignment"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string UserName {
            get {
                return ((string)(this["UserName"]));
            }
            set {
                this["UserName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int DefaultAutoHazyThreshold {
            get {
                return ((int)(this["DefaultAutoHazyThreshold"]));
            }
            set {
                this["DefaultAutoHazyThreshold"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool VerboseMode {
            get {
                return ((bool)(this["VerboseMode"]));
            }
            set {
                this["VerboseMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool QueryMonsterStatus {
            get {
                return ((bool)(this["QueryMonsterStatus"]));
            }
            set {
                this["QueryMonsterStatus"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Green")]
        public global::System.Drawing.Color FullColor {
            get {
                return ((global::System.Drawing.Color)(this["FullColor"]));
            }
            set {
                this["FullColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Red")]
        public global::System.Drawing.Color EmptyColor {
            get {
                return ((global::System.Drawing.Color)(this["EmptyColor"]));
            }
            set {
                this["EmptyColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int DefaultAutoSpellLevelMin {
            get {
                return ((int)(this["DefaultAutoSpellLevelMin"]));
            }
            set {
                this["DefaultAutoSpellLevelMin"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int DefaultAutoSpellLevelMax {
            get {
                return ((int)(this["DefaultAutoSpellLevelMax"]));
            }
            set {
                this["DefaultAutoSpellLevelMax"] = value;
            }
        }
    }
}
