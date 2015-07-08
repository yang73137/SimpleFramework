using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MethodInvokeAttribute : ProxyAttribute
    {
        public void AAA()
        {
            Thread.CurrentContext.co
        }
    }
}
