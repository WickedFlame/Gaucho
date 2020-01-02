using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Dashboard
{
    public class NonEscapedString
    {
        private readonly string _value;

        public NonEscapedString(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
