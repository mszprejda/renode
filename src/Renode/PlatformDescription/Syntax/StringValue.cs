//
// Copyright (c) 2010-2017 Antmicro
//
// This file is licensed under the MIT License.
// Full license text is available in 'licenses/MIT.txt'.
//

namespace Antmicro.Renode.PlatformDescription.Syntax
{
    public sealed class StringValue : Value, ISimplestValue
    {
        public StringValue(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("[StringValue: '{0}']", Value);
        }

        public string Value { get; private set; }

        public object ConvertedValue
        {
            get
            {
                return Value;
            }
        }
    }
}
