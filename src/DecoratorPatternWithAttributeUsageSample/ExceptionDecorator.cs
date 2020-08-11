using System;

namespace DecoratorPatternWithAttributeUsageSample
{
    public class ExceptionDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : new()
    {
        private ICommandHandler<TCommand> _decoratee;
        public ExceptionDecorator(ICommandHandler<TCommand> decoratee)
        {
            _decoratee = decoratee;
        }
        public void Handle(TCommand command)
        {
            try
            {
                _decoratee.Handle(command);
                Console.WriteLine("ExceptionDecorator'dan selamlar...");
            }
            catch (Exception ex)
            {
                // exception handling (logging...)
                throw;
            }
        }
    }
}
