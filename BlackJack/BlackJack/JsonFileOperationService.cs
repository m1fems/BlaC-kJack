using System;
using System.IO;
using System.Text.Json;

namespace BlackJack
{
    internal class JsonFileOperationService<T>
    {
        private string FilePath { get; set; }
        private Func<T> GetDefaultValue { get; set; } = () => default(T);
        private T JsonObject { get; set; }

        public JsonFileOperationService(string filePath)
        {
            FilePath = filePath;
        }

        public JsonFileOperationService(string filePath, Func<T> getDefaultValue) : this(filePath)
        {
            GetDefaultValue = getDefaultValue;
        }

        public void UpdateObject(T obj)
        {
            string jsonString = JsonSerializer.Serialize<T>(obj);
            File.WriteAllText(FilePath, jsonString);

            JsonObject = obj;
        }

        public T GetObject()
        {
            if (JsonObject == null)
            {
                InitializeJsonObject();
            }

            return JsonObject;
        }

        private void InitializeJsonObject()
        {
            try
            {
                string jsonString = File.ReadAllText(FilePath);
                T obj = JsonSerializer.Deserialize<T>(jsonString);

                JsonObject = obj;
            }
            catch (Exception ex)
            {
                JsonObject = GetDefaultValue();
            }
        }
    }
}