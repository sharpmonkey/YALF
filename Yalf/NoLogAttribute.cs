using System;

namespace Yalf
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	public class NoLogAttribute : Attribute
	{
        public NoLogAttribute() { }
	}
}
