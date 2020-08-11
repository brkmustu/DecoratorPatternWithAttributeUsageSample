
namespace DecoratorPatternWithAttributeUsageSample
{
    [ExceptionDecorator]
    public class FooCommandHandler : ICommandHandler<FooCommand>
    {
        public void Handle(FooCommand command)
        {
            System.Console.WriteLine("Foo name is : " + command.Name);
            System.Console.WriteLine("Foo code is : " + command.Code);
        }
    }
}
