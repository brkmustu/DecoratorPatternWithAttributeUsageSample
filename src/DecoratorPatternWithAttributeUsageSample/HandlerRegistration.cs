using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DecoratorPatternWithAttributeUsageSample
{
    public static class HandlerRegistration
    {
        public static void AddHandlers(this ContainerBuilder containerBuilder)
        {
            //Get all the Types With
            List<Type> handlerTypes = typeof(ICommand).Assembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(y => IsHandlerInterface(y)))
                .Where(x => x.Name.EndsWith("Handler"))
                .ToList();

            foreach (Type handlerType in handlerTypes)
            {
                //Try to register each handler one by one
                AddHandler(containerBuilder, handlerType, handlerType.GetMethod("Handle").GetParameters().FirstOrDefault().ParameterType);
            }
        }

        private static void AddHandler(ContainerBuilder containerBuilder, Type handlerType, Type parameterType)
        {
            //Get all the custom attributes the Given Handler is decorated with.
            object[] attributes = handlerType.GetCustomAttributes(false);
            //Convert those Attributes to the Decorator classes
            //using our one to one Attribute to Decorator Mapping
            List<Type> decorators = attributes
                .Select(x => ToDecorator(x))
                .Reverse()
                .ToList();
            //Get the Type for the Handler Class that should be registered for Dependency Injection
            Type interfaceType = handlerType.GetInterfaces().Single(y => IsHandlerInterface(y));

            containerBuilder.RegisterType(handlerType).As(interfaceType);

            foreach (var decorator in decorators)
            {
                containerBuilder.RegisterDecorator(decorator.MakeGenericType(parameterType), interfaceType);
            }
        }

        private static bool IsHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(ICommandHandler<>) || typeDefinition == typeof(IQueryHandler<,>);
        }

        private static Type ToDecorator(object attribute)
        {
            Type type = attribute.GetType();

            if (type == typeof(ExceptionDecoratorAttribute))
                return typeof(ExceptionDecorator<>);

            // other attributes go here

            throw new ArgumentException(attribute.ToString());
        }

        private static Func<IServiceProvider, object> BuildFactory(List<Type> decorators, Type interfaceType)
        {
            //Get the constructor for each of the decorators
            List<ConstructorInfo> ctors = decorators
                .Select(x =>
                {
                    Type type = x.IsGenericType ? x.MakeGenericType(interfaceType.GenericTypeArguments) : x;
                    return type.GetConstructors().Single();
                })
                .ToList();

            //Factory that will returned as result
            //This factory will take DI Container or ServiceProvider as parameter.
            Func<IServiceProvider, object> func = provider =>
            {
                object current = null;
                //Iterate through the Constructor for Each Decorator
                foreach (ConstructorInfo ctor in ctors)
                {
                    //Get all the parameters for a given Decorator Constructor
                    List<ParameterInfo> parameterInfos = ctor.GetParameters().ToList();
                    //Fetch the Required parameters from the ServiceProvider or DI container
                    object[] parameters = GetParameters(parameterInfos, current, provider);
                    //Invoke the Constructor
                    current = ctor.Invoke(parameters);
                }

                return current;
            };

            return func;
        }

        //Get Parameters from Dependency Injection Container
        private static object[] GetParameters(List<ParameterInfo> parameterInfos, object current, IServiceProvider provider)
        {
            var result = new object[parameterInfos.Count];

            for (int i = 0; i < parameterInfos.Count; i++)
            {
                //Get the Object from DI Container for each ParameterInfo
                result[i] = GetParameter(parameterInfos[i], current, provider);
            }

            return result;
        }

        //Get Object or Parameter Value from DI Container
        private static object GetParameter(ParameterInfo parameterInfo, object current, IServiceProvider provider)
        {
            Type parameterType = parameterInfo.ParameterType;

            if (IsHandlerInterface(parameterType))
                return current;
            //Get the Parameter object from DI Container(ServicesProvider)
            object service = provider.GetService(parameterType);
            if (service != null)
                return service;

            throw new ArgumentException($"Type {parameterType} not found");
        }
    }
}
