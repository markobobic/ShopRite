using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Core.Extensions
{
    public class TasksExtension
    {
        public static Task DelayedTask(Task task, TimeSpan delay, CancellationToken token)
        {
            return Task.WhenAll(task, Task.Delay(delay, token));
        }
    }
}
