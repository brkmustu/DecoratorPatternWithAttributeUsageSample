using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPatternWithAttributeUsageSample
{
    public class FooCommand : ICommand
    {
        public string Name { get; set; }
        public int Code { get; set; }
    }
}
