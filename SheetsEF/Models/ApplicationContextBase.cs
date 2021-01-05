using GoogleLib;
using Microsoft.Extensions.Caching.Memory;
using Models;
using SheetsEF.Interfaces;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SheetsEF.Models
{
    public class ApplicationContextBase : ISheetsContext, IData
    {
        private readonly MemoryCache _cache;

        public ApplicationContextBase(ApplicationContextOptions options)
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public T GetData<T>(string key)
        {
            _cache.TryGetValue(key, out T data);
            return data;
        }

        public async Task<bool> GetDataFromSheetsAsync(string key)
        {
            GoogleSheets googleSheets = new GoogleSheets();
            ServiceContext serviceContext = new ServiceContext();
            await serviceContext.InitContextAsync(googleSheets);

            _cache.Set(key, 
                serviceContext,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            return true;
        }

        public bool BackupDataToSheets()
        {
            return true;
        }

        public virtual async Task UpdatingSheetsAsync()
        {
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                await GetDataFromSheetsAsync(prop.Name);
            }
        }
    }
}
