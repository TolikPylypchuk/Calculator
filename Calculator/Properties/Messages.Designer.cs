﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Calculator.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Calculator.Properties.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Division by zero equals infinity..
        /// </summary>
        internal static string DivisionByZero {
            get {
                return ResourceManager.GetString("DivisionByZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid expression..
        /// </summary>
        internal static string InvalidExpression {
            get {
                return ResourceManager.GetString("InvalidExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A logarithm of a negative number is not a real number..
        /// </summary>
        internal static string LogarithmOfNegativeNumber {
            get {
                return ResourceManager.GetString("LogarithmOfNegativeNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A root of a negative number is not a real number..
        /// </summary>
        internal static string RootOfNegativeNumber {
            get {
                return ResourceManager.GetString("RootOfNegativeNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 0/0 is undefined..
        /// </summary>
        internal static string ZeroByZero {
            get {
                return ResourceManager.GetString("ZeroByZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 0^0 is undefined..
        /// </summary>
        internal static string ZeroToThePowerOfZero {
            get {
                return ResourceManager.GetString("ZeroToThePowerOfZero", resourceCulture);
            }
        }
    }
}
