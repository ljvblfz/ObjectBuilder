using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Practices.ObjectBuilder;

namespace ObjectBuilder.Samples
{
    public partial class ContainerXmlConfig : IBuilderConfigurator<BuilderStage>
    {
        // Fields

        private IBuilder<BuilderStage> builder;
        private ContainerXmlConfigElement config;

        // Lifetime

        public ContainerXmlConfig(bool enableReflection)
        {
            config = new ContainerXmlConfigElement();
            config.EnableReflection = enableReflection;
        }

        public ContainerXmlConfig(string xml)
        {
            config = ParseXmlConfiguration(xml);
        }

        // Properties

        public bool IncludeReflectionStrategies
        {
            get { return true; }
        }

        // Methods

        public void ApplyConfiguration(IBuilder<BuilderStage> builder)
        {
            this.builder = builder;

            ProcessStrategies();

            if (config.Mappings != null)
                foreach (MappingElement mapping in config.Mappings)
                    ProcessMapping(mapping);

            if (config.BuildRules != null)
            {
                foreach (BuildRuleElement buildRule in config.BuildRules)
                {
                    Type buildType = Type.GetType(buildRule.Type);
                    Debug.Assert(buildType != null);

                    ProcessConstructor(buildRule, buildType);
                    ProcessProperties(buildRule, buildType);
                    ProcessMethods(buildRule, buildType);
                    ProcessSingleton(buildRule, buildType);
                }
            }
        }

        private MethodPolicy GetMethodPolicy(Type typeToBuild, string idToBuild)
        {
            MethodPolicy policy = builder.Policies.Get<IMethodPolicy>(typeToBuild, idToBuild) as MethodPolicy;

            if (policy == null)
            {
                policy = new MethodPolicy();
                builder.Policies.Set<IMethodPolicy>(policy, typeToBuild, idToBuild);
            }

            return policy;
        }

        private static IParameter GetParameterFromConfigParam(object param)
        {
            if (param is ValueElement)
                return ValueElementToParameter(param as ValueElement);
            else if (param is RefElement)
                return RefElementToParameter(param as RefElement);

            Debug.Assert(false);
            return null;
        }

        private static ContainerXmlConfigElement ParseXmlConfiguration(string xml)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ContainerXmlConfigElement));
            StringReader stringReader = new StringReader(xml);
            XmlSchema schema =
                XmlSchema.Read(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(
                        "ObjectBuilder.Samples.Configuration.ConfigXmlConfigElement.xsd"), null);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add(schema);
            XmlReader reader = XmlReader.Create(stringReader, settings);
            ContainerXmlConfigElement configData = (ContainerXmlConfigElement)ser.Deserialize(reader);
            return configData;
        }

        private void ProcessConstructor(BuildRuleElement buildRule, Type buildType)
        {
            if (buildRule.Constructor == null)
                return;

            ConstructorPolicy policy = new ConstructorPolicy();

            foreach (object param in buildRule.Constructor.Items)
                policy.AddParameter(GetParameterFromConfigParam(param));

            builder.Policies.Set<ICreationPolicy>(policy, buildType, null);
        }

        private void ProcessMapping(MappingElement mapping)
        {
            Type fromType = Type.GetType(mapping.FromType);
            Type toType = Type.GetType(mapping.ToType);

            Debug.Assert(fromType != null);
            Debug.Assert(toType != null);

            builder.Policies.Set<ITypeMappingPolicy>(new TypeMappingPolicy(toType, null), fromType, null);
        }

        private void ProcessMethods(BuildRuleElement buildRule, Type buildType)
        {
            if (buildRule.Method == null)
                return;

            MethodPolicy policy = GetMethodPolicy(buildType, null);

            foreach (MethodElement method in buildRule.Method)
            {
                List<IParameter> parameters = new List<IParameter>();

                foreach (object param in method.Items)
                    parameters.Add(GetParameterFromConfigParam(param));

                policy.Methods.Add(method.Name, new MethodCallInfo(method.Name, parameters));
            }
        }

        private void ProcessProperties(BuildRuleElement buildRule, Type buildType)
        {
            if (buildRule.Property == null)
                return;

            PropertySetterPolicy policy = new PropertySetterPolicy();

            foreach (PropertyElement prop in buildRule.Property)
                policy.Properties.Add(prop.Name,
                                      new PropertySetterInfo(prop.Name, GetParameterFromConfigParam(prop.Item)));

            builder.Policies.Set<IPropertySetterPolicy>(policy, buildType, null);
        }

        private void ProcessSingleton(BuildRuleElement buildRule, Type buildType)
        {
            if (buildRule.Mode == ModeElement.Singleton)
                builder.Policies.Set<ISingletonPolicy>(new SingletonPolicy(true), buildType, null);
        }

        private void ProcessStrategies()
        {
            builder.Strategies.AddNew<TypeMappingStrategy>(BuilderStage.PreCreation);
            builder.Strategies.AddNew<SingletonStrategy>(BuilderStage.PreCreation);

            if (config.EnableReflection)
            {
                builder.Strategies.AddNew<ConstructorReflectionStrategy>(BuilderStage.PreCreation);
                builder.Strategies.AddNew<PropertyReflectionStrategy>(BuilderStage.PreCreation);
                builder.Strategies.AddNew<MethodReflectionStrategy>(BuilderStage.PreCreation);
            }

            builder.Strategies.AddNew<CreationStrategy>(BuilderStage.Creation);
            builder.Strategies.AddNew<PropertySetterStrategy>(BuilderStage.Initialization);
            builder.Strategies.AddNew<MethodExecutionStrategy>(BuilderStage.Initialization);
            builder.Strategies.AddNew<BuilderAwareStrategy>(BuilderStage.PostInitialization);

            builder.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
        }

        private static IParameter RefElementToParameter(RefElement refParam)
        {
            Type paramType = Type.GetType(refParam.Type);
            Debug.Assert(paramType != null);
            return new CreationParameter(paramType, null);
        }

        private static IParameter ValueElementToParameter(ValueElement valueParam)
        {
            Type paramType = Type.GetType(valueParam.Type);
            Debug.Assert(paramType != null);
            return new ValueParameter(paramType, Convert.ChangeType(valueParam.Value, paramType));
        }
    }
}