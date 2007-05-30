namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class Builder : BuilderBase<BuilderStage>
    {
        // Lifetime

        public Builder()
            : this(null) {}

        public Builder(IBuilderConfigurator<BuilderStage> configurator)
        {
            Strategies.AddNew<TypeMappingStrategy>(BuilderStage.PreCreation);
            Strategies.AddNew<SingletonStrategy>(BuilderStage.PreCreation);
            Strategies.AddNew<ConstructorReflectionStrategy>(BuilderStage.PreCreation);
            Strategies.AddNew<PropertyReflectionStrategy>(BuilderStage.PreCreation);
            Strategies.AddNew<MethodReflectionStrategy>(BuilderStage.PreCreation);
            Strategies.AddNew<CreationStrategy>(BuilderStage.Creation);
            Strategies.AddNew<PropertySetterStrategy>(BuilderStage.Initialization);
            Strategies.AddNew<MethodExecutionStrategy>(BuilderStage.Initialization);
            Strategies.AddNew<BuilderAwareStrategy>(BuilderStage.PostInitialization);

            Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());

            if (configurator != null)
                configurator.ApplyConfiguration(this);
        }
    }
}