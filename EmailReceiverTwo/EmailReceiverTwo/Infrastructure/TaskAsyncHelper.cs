using System.Threading.Tasks;

namespace EmailReceiverTwo.Infrastructure
{
    internal static class TaskAsyncHelper
    {
        public static Task Empty
        {
            get
            {
                return FromResult<object>(null);
                ;
            }
        }

        public static Task<T> FromResult<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }

    }
}
