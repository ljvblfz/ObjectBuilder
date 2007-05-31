using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using CodePlex.DependencyInjection.Properties;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class CreationStrategy : BuilderStrategy
    {
        // Methods

        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            if (existing != null)
                BuildUpExistingObject(context, typeToBuild, existing, idToBuild);
            else
                existing = BuildUpNewObject(context, typeToBuild, existing, idToBuild);

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }

        static void BuildUpExistingObject(IBuilderContext context,
                                          Type typeToBuild,
                                          object existing,
                                          string idToBuild)
        {
            RegisterObject(context, typeToBuild, existing, idToBuild);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        static object BuildUpNewObject(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            ICreationPolicy policy = context.Policies.Get<ICreationPolicy>(typeToBuild, idToBuild);

            if (policy == null)
            {
                if (idToBuild == null)
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                              Resources.MissingPolicyUnnamed, typeToBuild));
                else
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                              Resources.MissingPolicyNamed, typeToBuild, idToBuild));
            }

            try
            {
                existing = FormatterServices.GetSafeUninitializedObject(typeToBuild);
            }
            catch (MemberAccessException exception)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.CannotCreateInstanceOfType, typeToBuild), exception);
            }

            RegisterObject(context, typeToBuild, existing, idToBuild);
            InitializeObject(context, existing, idToBuild, policy);
            return existing;
        }

        static void InitializeObject(IBuilderContext context,
                                     object existing,
                                     string id,
                                     ICreationPolicy policy)
        {
            Type type = existing.GetType();
            ConstructorInfo constructor = policy.SelectConstructor(context, type, id);

            if (constructor == null)
            {
                if (type.IsValueType)
                    return;

                throw new ArgumentException(Resources.NoAppropriateConstructor);
            }

            object[] parms = policy.GetParameters(context, type, id, constructor);

            MethodBase method = constructor;
            Guard.ValidateMethodParameters(method, parms, existing.GetType());

            method.Invoke(existing, parms);
        }

        static void RegisterObject(IBuilderContext context,
                                   Type typeToBuild,
                                   object existing,
                                   string idToBuild)
        {
            if (context.Locator == null)
                return;

            if (context.Lifetime == null)
                return;

            ISingletonPolicy singletonPolicy = context.Policies.Get<ISingletonPolicy>(typeToBuild, idToBuild);

            if (singletonPolicy != null && singletonPolicy.IsSingleton)
            {
                lock (context.Locator)
                    context.Locator.Add(new DependencyResolutionLocatorKey(typeToBuild, idToBuild), existing);

                lock (context.Lifetime)
                    context.Lifetime.Add(existing);
            }
        }
    }
}