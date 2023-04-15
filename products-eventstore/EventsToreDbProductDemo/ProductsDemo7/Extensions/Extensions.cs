using DevExpress.Mvvm.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProductsDemo7.Extensions
{
    public static class Extensions
    {
        public static Task onTaskScheduler(Action action ,Action<Exception> errorAction, TaskScheduler scheduler)
        {
                return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, scheduler)
                           .ContinueWith(task => errorAction, TaskContinuationOptions.OnlyOnFaulted);
        }

        public static void fromDictionary<T,U>(this BindingList<U> lst, IDictionary<T,U> dict)
        {
            lst.Clear();
            dict.Values.ForEach(v=>lst.Add(v));
        }

        public static T CloneJson<T>(this T source)
        {
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
    }
}
