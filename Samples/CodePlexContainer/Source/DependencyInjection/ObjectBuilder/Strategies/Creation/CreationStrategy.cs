using System;
using System.Globalization;
using System.Reflection;
using CodePlex.DependencyInjection.Properties;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class CreationStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            return base.BuildUp(context,
                                typeToBuild,
                                existing ?? BuildUpNewObject(context, typeToBuild, idToBuild),
                                idToBuild);
        }

        static object BuildUpNewObject(IBuilderContext context,
                                       Type typeToBuild,
                                       string idToBuild)
        {
            ICreationPolicy policy = context.Policies.Get<ICreationPolicy>(typeToBuild,
                                                                           idToBuild);

            if (policy == null)
            {
                if (idToBuild == null)
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                              Resources.MissingPolicyUnnamed,
                                                              typeToBuild));
                else
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                              Resources.MissingPolicyNamed,
                                                              typeToBuild,
                                                              idToBuild));
            }

            try
            {
                ConstructorInfo constructor = policy.SelectConstructor(context, typeToBuild, idToBuild);

                if (constructor == null)
                {
                    if (typeToBuild.IsValueType)
                        return Activator.CreateInstance(typeToBuild);

                    throw new ArgumentException(Resources.NoAppropriateConstructor);
                }

                return constructor.Invoke(policy.GetParameters(context, typeToBuild, idToBuild, constructor));
            }
            catch (MemberAccessException ex)
            {
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentCulture,
                                  Resources.CannotCreateInstanceOfType,
                                  typeToBuild),
                    ex);
            }
        }
    }
}