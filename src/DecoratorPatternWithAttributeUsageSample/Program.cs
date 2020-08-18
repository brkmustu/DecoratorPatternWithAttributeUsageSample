using Autofac;

namespace DecoratorPatternWithAttributeUsageSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.AddHandlers();

            using (var container = containerBuilder.Build())
            {
                var fooCommand = container.Resolve<ICommandHandler<FooCommand>>();
                fooCommand.Handle(new FooCommand { Name = "Test Foo", Code = 111 });
            }
            
            Console.Read();
        }
    }
}
