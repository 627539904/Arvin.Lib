using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace Arvin.Extensions
{
    public static class ThreadExtension
    {
        public static async Task MutiThread<T>(this List<T> list, Func<T, Task> func, int maxConcurrentTasks = 10, bool isDebug = false)
        {
            await MultiThreadImpl(list, (item, _) => func(item), maxConcurrentTasks, isDebug);
        }
        public static async Task MutiThread<T>(this List<T> list, Func<T,int, Task> func, int maxConcurrentTasks = 10, bool isDebug = false)
        {
            await MultiThreadImpl(list, func, maxConcurrentTasks, isDebug);
        }

        private static async Task MultiThreadImpl<T>(List<T> list, Func<T, int, Task> funcWithIndex, int maxConcurrentTasks, bool isDebug)
        {
            maxConcurrentTasks = 10; // 实际上，这里可以移除这行代码，因为已经在方法签名中默认设置了
            SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrentTasks);
            List<Task> tasks = new List<Task>();
            int i = 0;

            foreach (var item in list)
            {
                if (isDebug)
                {
                    // 注意：这里我们假设如果isDebug为真，则不使用并发处理
                    if (funcWithIndex != null && funcWithIndex.GetMethodInfo().GetParameters().Length == 2)
                    {
                        await funcWithIndex(item, i);
                    }
                    else
                    {
                        throw new ArgumentException("isDebug is true but provided function does not match expected signature with index.");
                    }
                    i++;
                    continue;
                }

                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        await funcWithIndex(item, i++);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
