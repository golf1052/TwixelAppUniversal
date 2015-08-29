using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwixelAppUniversal
{
    public class ItemLoader
    {
        bool currentlyLoading;
        bool endOfList = false;

        public ItemLoader()
        {
            currentlyLoading = false;
        }

        public async Task<T> LoadItems<T>(Func<Task<T>> func)
        {
            Task<T> task = func.Invoke();
            T result = await task;
            return result;
        }
    }
}
