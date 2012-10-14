using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maacro.Infrastructure
{
    public interface IInputListener : IDisposable
    {
        void Start();
        void Stop();
    }
}
