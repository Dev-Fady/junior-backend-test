using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace junior_backend_test.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base()
        {
        }
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
