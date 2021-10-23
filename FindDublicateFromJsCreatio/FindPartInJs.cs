using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FindDublicateFromJsCreatio
{
    public static class FindPartInJs
    {
        public static List<Attribute> FindAttributes(string fullJs)
        {
            List<Attribute> attributes = new List<Attribute>();

            var enumMainAttributes = new string[]
                { "mixins", "attributes", "modules", "details", "rules",
                "businessRules", "messages", "dataModels", "diff", "methods" };

            fullJs = DeleteNeddlessSymbols(fullJs);

            foreach (var attribute in enumMainAttributes)
            {
                var result = new List<string>();

                var cutjs = CutNeedPart(fullJs, attribute);
                if (cutjs == string.Empty)
                {
                    continue;
                }

                if (attribute == "methods")
                {
                    // В блок методов записываются лишние ковычки
                    // { methods: { getOpenDefaultValues: "function (typeColumnValue", operation) { var defa
                }
                else
                {
                    JObject jObject = JObject.Parse(cutjs);
                    JToken list = jObject[attribute];

                    if (attribute == "diff")
                    {
                        foreach (var e in list)
                        {
                            result.Add(e.Value<object>("name").ToString());
                            Console.WriteLine(e.Value<object>("name").ToString());
                        }
                    }
                    else
                    {
                        foreach (var e in list)
                        {
                            result.Add(e.Path.Replace(attribute + ".", string.Empty));
                            Console.WriteLine(e.Path.Replace(attribute + ".", string.Empty));
                        }
                    }
                }

                attributes.Add(new Attribute()
                {
                    Name = attribute,
                    Childs = result
                });
                Console.WriteLine();
            }

            return attributes;
        }


        private static string CutNeedPart(string fullJs, string attributeName)
        {
            Regex regex = new Regex(attributeName + ": [{|\\[]");

            // ToDo обработка отсутствия атрибута
            int index = regex.Match(fullJs).Index;
            if (index <= 0)
            {
                return string.Empty;
            }

            var cutJs = fullJs.Substring(index);

            var chars = cutJs.ToCharArray();
            var indexEnd = 0;
            var balanceBrace = 0;
            var canEndCalculate = false;

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '{' || chars[i] == '[')
                {
                    balanceBrace++;
                    canEndCalculate = true;
                }
                else if (chars[i] == '}' || chars[i] == ']')
                {
                    balanceBrace--;
                }

                if (canEndCalculate && balanceBrace == 0)
                {
                    indexEnd = i;
                    break;
                }
            }

            cutJs = cutJs.Substring(0, indexEnd + 1);

            // Добавляем ковычки значениям после которых идет символ '}'.
            regex = new Regex(":\\s([^\"{\\[][^},]*)[^\"}]\\s*}");
            cutJs = regex.Replace(cutJs, ": \"$1\" }");

            // Добавляем ковычки значениям после которых идет символ ','.
            regex = new Regex(":\\s([^\"{\\[][^},]*[^\"}])\\s*,");
            cutJs = regex.Replace(cutJs, ": \"$1\",");

            cutJs = "{ " + cutJs + " }";

            return cutJs;
        }

        private static string DeleteNeddlessSymbols(string text)
        {
            // Удаляем табуляцию.
            Regex regex = new Regex(@"\t");
            text = regex.Replace(text, string.Empty);

            // Удаляем однострочные комментарии.
            regex = new Regex(@"\s*//[^\n]*\n");
            text = regex.Replace(text, "");

            // Меняем пробелы на одинарные
            regex = new Regex(@"\s\s+");
            text = regex.Replace(text, " ");

            // Удаляем многострочные комментарии.
            regex = new Regex(@"\s*/\*\*[^/]*\*/");
            text = regex.Replace(text, " ");

            // Меняем пробелы на одинарные
            regex = new Regex(@"\s\s+");
            text = regex.Replace(text, " ");

            return text;
        }
    }
}
