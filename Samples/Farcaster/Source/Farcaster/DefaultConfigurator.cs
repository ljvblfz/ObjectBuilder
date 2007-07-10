using System;
using Microsoft.Practices.ObjectBuilder;

namespace Farcaster
{
	/// <summary>
	/// Default configurator class for a <see cref="BuilderContainer"/>.
	/// </summary>
	/// <remarks>
	/// Default strategies added by this configurator are:
	/// <list type="bullet">
	/// <item>
	/// <term><see cref="SingletonStrategy"/></term>
	/// <description>Ensures singleton behavior of objects.</description>
	/// </item>
	/// <item>
	/// <term><see cref="CreationStrategy"/></term>
	/// <description>Creates instances of objects that ensures singleton behavior of objects.</description>
	/// </item>
	/// </list>
	/// 
	/// </remarks>
	public class DefaultConfigurator : IBuilderConfigurator<BuilderStage>
	{
		#region IBuilderConfigurator<BuilderStage> Members

		/// <summary>
		/// Applies the default strategies to the builder.
		/// </summary>
		/// <param name="builder">The builder to apply configuration to.</param>
		public void ApplyConfiguration(IBuilder<BuilderStage> builder)
		{
			AddPreCreationStrategies(builder);
			AddCreationStrategies(builder);
			AddInitializationStrategies(builder);
			AddPostInitializationStrategies(builder);
			SetPolicyDefaults(builder);
		}

		#endregion

		/// <summary>
		/// Adds strategies to the <see cref="BuilderStage.PreCreation"/> stage.
		/// By default, adds the <see cref="SingletonStrategy"/> strategy to the <paramref name="builder"/>.
		/// </summary>
		/// <param name="builder">The builder to add strategies to.</param>
		protected virtual void AddPreCreationStrategies(IBuilder<BuilderStage> builder)
		{
			builder.Strategies.AddNew<TypeMappingStrategy>(BuilderStage.PreCreation);
			builder.Strategies.AddNew<SingletonStrategy>(BuilderStage.PreCreation);
		}

		/// <summary>
		/// Adds strategies to the <see cref="BuilderStage.Creation"/> stage.
		/// By default, adds the <see cref="CreationStrategy"/> strategy to the <paramref name="builder"/>.
		/// </summary>
		/// <param name="builder">The builder to add strategies to.</param>
		protected virtual void AddCreationStrategies(IBuilder<BuilderStage> builder)
		{
			builder.Strategies.AddNew<CreationStrategy>(BuilderStage.Creation);
		}

		/// <summary>
		/// Adds strategies to the <see cref="BuilderStage.PreCreation"/> stage.
		/// By default, adds the <see cref="SingletonDisposeEventStrategy"/> and 
		/// <see cref="ComponentSiteStrategy"/> strategies to the <paramref name="builder"/>.
		/// </summary>
		/// <param name="builder">The builder to add strategies to.</param>
		protected virtual void AddInitializationStrategies(IBuilder<BuilderStage> builder)
		{
			builder.Strategies.AddNew<SingletonDisposeEventStrategy>(BuilderStage.Initialization);
			builder.Strategies.AddNew<ComponentSiteStrategy>(BuilderStage.Initialization);
		}

		/// <summary>
		/// Adds strategies to the <see cref="BuilderStage.PreCreation"/> stage.
		/// By default does not add any strategies.
		/// </summary>
		/// <param name="builder">The builder to add strategies to.</param>
		protected virtual void AddPostInitializationStrategies(IBuilder<BuilderStage> builder)
		{
		}

		/// <summary>
		/// Sets default policies. Base implementation sets the <see cref="DefaultCreationPolicy"/> 
		/// as the default for the <see cref="ICreationPolicy"/>.
		/// </summary>
		/// <param name="builder">The builder to set policy defaults.</param>
		/// <seealso cref="PolicyList.SetDefault"/>.
		protected virtual void SetPolicyDefaults(IBuilder<BuilderStage> builder)
		{
			builder.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
		}
	}
}
