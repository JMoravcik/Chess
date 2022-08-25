using Blazored.LocalStorage;
using Newtonsoft.Json;
using System.Text;

namespace Chess.WebAsm.Client.Services
{
    public class MemoryService
    {
        public MemoryService(ILocalStorageService localStorage)
        {
            LocalStorage = localStorage;
        }

        private ILocalStorageService LocalStorage { get; }

        public async Task<T> Get<T>()
            where T : class
        {
            return await Get<T>(typeof(T).FullName);
        }

        public async Task<T> Get<T>(string name)
            where T : class
        {
            try
            {
                bool contains = await LocalStorage.ContainKeyAsync(name);
                if (!contains) return null;
                var data = await LocalStorage.GetItemAsStringAsync(name);
                if (string.IsNullOrEmpty(data))
                    return null;

                return FromBase64Json<T>(data);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task Set<T>(T obj)
            where T : class
        {
            await Set<T>(typeof(T).FullName, obj);
        }

        public async Task Set<T>(string name, T obj)
            where T : class
        {
            string text = ToBase64Json(obj);
            await LocalStorage.SetItemAsStringAsync(name, text);
        }

        public async Task Remove<T>()
            where T : class
        {
            await Remove(typeof(T).FullName);
        }

        public async Task Remove(params string[] name)
        {
            await LocalStorage.RemoveItemsAsync(name);
        }
        public string ToBase64Json<T>(T obj)
            where T : class
        {
            string json = JsonConvert.SerializeObject(obj);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            string base64Json = Convert.ToBase64String(jsonBytes);
            return base64Json;
        }

        public T FromBase64Json<T>(string base64Json)
            where T : class
        {
            var jsonBytes = Convert.FromBase64String(base64Json);
            var json = Encoding.UTF8.GetString(jsonBytes);
            var obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }
    }
}
