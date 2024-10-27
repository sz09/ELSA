namespace ELSA.Utilities
{
    public static class ThrottlingUtils
    {
        public static async Task ConcurrentExecForEachAsync<T>(this IEnumerable<T> inputList, Func<T, Task> taskExecFunc, int maxConcurrent = 10)
        {
            var throttler = new SemaphoreSlim(initialCount: maxConcurrent);
            var allTasks = new List<Task>();
            foreach (var n in inputList)
            {

                await throttler.WaitAsync();
                var executingTask = Task.Run(async () =>
                {
                    try
                    {
                        await taskExecFunc(n);
                    }
                    finally
                    {
                        throttler.Release();
                    }
                });
                allTasks.Add(executingTask);

            }
            await Task.WhenAll(allTasks);
        }


        public static async Task<IEnumerable<R>> ConcurrentExecForEachAsync<T, R>(this IEnumerable<T> inputList, Func<T, Task<R>> taskExecFunc, int maxConcurrent = 10)
        {
            var throttler = new SemaphoreSlim(initialCount: maxConcurrent);
            var allTasks = new List<Task<R>>();
            foreach (var n in inputList)
            {

                await throttler.WaitAsync();
                var executingTask = Task.Run(async () =>
                {
                    try
                    {
                        var result = await taskExecFunc(n);
                        return result;
                    }
                    finally
                    {
                        throttler.Release();
                    }
                });
                allTasks.Add(executingTask);

            }
            var resultList = await Task.WhenAll(allTasks);
            return resultList;
        }
    }
}