//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.312
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace CodePlex.DependencyInjection.Properties
{
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [DebuggerNonUserCode()]
    [CompilerGenerated()]
    class Resources
    {
        static CultureInfo resourceCulture;
        static ResourceManager resourceMan;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {}

        /// <summary>
        ///   Looks up a localized string similar to Object builder has no strategies..
        /// </summary>
        internal static string BuilderHasNoStrategies
        {
            get { return ResourceManager.GetString("BuilderHasNoStrategies", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to [{0}] {1} / {2} :: {3}.
        /// </summary>
        internal static string BuilderStrategyTraceBuildUp
        {
            get { return ResourceManager.GetString("BuilderStrategyTraceBuildUp", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to [{0}] {1} :: {2}.
        /// </summary>
        internal static string BuilderStrategyTraceTearDown
        {
            get { return ResourceManager.GetString("BuilderStrategyTraceTearDown", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to [BuilderBase] Finished BuildUp on {0} with ID {1}.
        /// </summary>
        internal static string BuildUpFinished
        {
            get { return ResourceManager.GetString("BuildUpFinished", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to [BuilderBase] Starting BuildUp on {0} with ID {1}.
        /// </summary>
        internal static string BuildUpStarting
        {
            get { return ResourceManager.GetString("BuildUpStarting", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Calling constructor({0}).
        /// </summary>
        internal static string CallingConstructor
        {
            get { return ResourceManager.GetString("CallingConstructor", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Calling method {0}({1}).
        /// </summary>
        internal static string CallingMethod
        {
            get { return ResourceManager.GetString("CallingMethod", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Calling IBuilderAware.OnBuiltUp().
        /// </summary>
        internal static string CallingOnBuiltUp
        {
            get { return ResourceManager.GetString("CallingOnBuiltUp", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Calling IBuilderAware.OnTearingDown().
        /// </summary>
        internal static string CallingOnTearingDown
        {
            get { return ResourceManager.GetString("CallingOnTearingDown", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Setting property {0}({1}).
        /// </summary>
        internal static string CallingProperty
        {
            get { return ResourceManager.GetString("CallingProperty", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Can not create an instance of type &apos;{0}&apos;..
        /// </summary>
        internal static string CannotCreateInstanceOfType
        {
            get { return ResourceManager.GetString("CannotCreateInstanceOfType", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to In class &apos;{0}&apos;, cannot inject value on property &apos;{1}&apos; because it is read-only..
        /// </summary>
        internal static string CannotInjectReadOnlyProperty
        {
            get { return ResourceManager.GetString("CannotInjectReadOnlyProperty", resourceCulture); }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get { return resourceCulture; }
            set { resourceCulture = value; }
        }

        /// <summary>
        ///   Looks up a localized string similar to Could not locate dependency &quot;{0}&quot;..
        /// </summary>
        internal static string DependencyMissing
        {
            get { return ResourceManager.GetString("DependencyMissing", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Type must be a subclass of System.Attribute..
        /// </summary>
        internal static string ExceptionAttributeNoSubclassOfAttribute
        {
            get { return ResourceManager.GetString("ExceptionAttributeNoSubclassOfAttribute", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not interceptable..
        /// </summary>
        internal static string InterceptionNotSupported
        {
            get { return ResourceManager.GetString("InterceptionNotSupported", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Too many dependency injection attributes defined on {0}.{1}..
        /// </summary>
        internal static string InvalidAttributeCombination
        {
            get { return ResourceManager.GetString("InvalidAttributeCombination", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Invalid enumeration value..
        /// </summary>
        internal static string InvalidEnumerationValue
        {
            get { return ResourceManager.GetString("InvalidEnumerationValue", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to The value of the argument {0} provided for the enumeration {1} is invalid..
        /// </summary>
        internal static string InvalidEnumValue
        {
            get { return ResourceManager.GetString("InvalidEnumValue", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to An item with the given key is already present in the dictionary..
        /// </summary>
        internal static string KeyAlreadyPresentInDictionary
        {
            get { return ResourceManager.GetString("KeyAlreadyPresentInDictionary", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Missing policy to build {0} named &apos;{1}&apos;..
        /// </summary>
        internal static string MissingPolicyNamed
        {
            get { return ResourceManager.GetString("MissingPolicyNamed", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Missing policy to build unnamed {0}..
        /// </summary>
        internal static string MissingPolicyUnnamed
        {
            get { return ResourceManager.GetString("MissingPolicyUnnamed", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Could not find an appropriately matching constructor..
        /// </summary>
        internal static string NoAppropriateConstructor
        {
            get { return ResourceManager.GetString("NoAppropriateConstructor", resourceCulture); }
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (ReferenceEquals(resourceMan, null))
                {
                    ResourceManager temp = new ResourceManager("CodePlex.DependencyInjection.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Registered as singleton.
        /// </summary>
        internal static string SingletonRegistered
        {
            get { return ResourceManager.GetString("SingletonRegistered", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Returning existing singleton.
        /// </summary>
        internal static string SingletonReturned
        {
            get { return ResourceManager.GetString("SingletonReturned", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to The provided String argument {0} must not be empty..
        /// </summary>
        internal static string StringCannotBeEmpty
        {
            get { return ResourceManager.GetString("StringCannotBeEmpty", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to [BuilderBase] Finished TearDown on {0}.
        /// </summary>
        internal static string TearDownFinished
        {
            get { return ResourceManager.GetString("TearDownFinished", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to [BuilderBase] Starting TearDown on {0}.
        /// </summary>
        internal static string TearDownStarting
        {
            get { return ResourceManager.GetString("TearDownStarting", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Injection Policies applied to object..
        /// </summary>
        internal static string TracePoliciesApplied
        {
            get { return ResourceManager.GetString("TracePoliciesApplied", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to Mapped to {0} / {1}.
        /// </summary>
        internal static string TypeMapped
        {
            get { return ResourceManager.GetString("TypeMapped", resourceCulture); }
        }

        /// <summary>
        ///   Looks up a localized string similar to While resolving dependencies for {2}, the provided type {1} is not compatible with {0}..
        /// </summary>
        internal static string TypeNotCompatible
        {
            get { return ResourceManager.GetString("TypeNotCompatible", resourceCulture); }
        }
    }
}