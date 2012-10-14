using ReactiveUI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;

namespace Maacro.ViewModel
{
    public static class ReactiveCommandExtensions
    {
        public static ReactiveCommand WithSubscription(this ReactiveCommand source, Func<object, bool> whereClause, Action<object> subscriptionFunc)
        {
            source.Where(whereClause).Subscribe(subscriptionFunc);

            return source;
        }

        public static ReactiveCommand WithSubscription(this ReactiveCommand source, Action<object> subscriptionFunc)
        {
            source.Subscribe(subscriptionFunc);

            return source;
        }
    }
}
