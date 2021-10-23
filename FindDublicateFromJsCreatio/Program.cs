using ClientDependency.Core.CompositeFiles;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace FindDublicateFromJsCreatio
{
    class Program
    {
        static void Main(string[] args)
        {
            string textFromFile = "ContactProfileSchema.js";
            //string textFromFile = "attributeJson.json";

            //attributeJson.json

            // чтение из файла
            using (FileStream fstream = File.OpenRead(textFromFile))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                textFromFile = System.Text.Encoding.Default.GetString(array);
                //Console.WriteLine($"Текст из файла: {textFromFile}");
            }

            //JObject json = JObject.Parse(textFromFile);


            FindPartInJs.FindAttributes(textFromFile);
        }
    }
}
