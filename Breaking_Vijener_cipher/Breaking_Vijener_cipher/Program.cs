/*Разобьём текст x1, x2, ..., xn на столбцы размера t.
Если t кратно длине ключа, то каждые два элемента текста, отстоящие друг от друга на a*t позиций,
a ∈ N, зашифрованы одним и тем же алфавитом. А это означает, что каждая строка в выписанной выше таблице получена из открытого текста перестановкой.
Если же t не кратно длине ключа, то строки являются полиалфавитным шифром.
Индекс совпадений для перестановки открытого текста и для полиалфавитного шифра заметно отличается.
Таким образом, перебирая различные значения t и вычисляя для каждого из них индекс совпадений, мы можем выделить те t, которые кратны длине ключа.
Предположим, мы определили длину ключа t.
Вновь выпишем текст в столбцы размера t.
Рассмотрим две строки этой таблицы. Сдвинем алфавит одной из строк на {\displaystyle s}s символов и вычислим взаимный индекс совпадений полученных строк.
Т.к. каждая из этих двух строк получена сдвигом алфавита открытого текста, то максимум взаимного индекса совпадений будет наблюдаться при нулевом конечном относительном сдвиге.
Поэтому применяется следующий алгоритм: вычисляется взаимный индекс совпадений для различных s, ищется значение s, при котором взаимный индекс совпадений максимален.
Тогда начальный относительный сдвиг строк будет равен  m-s (m — размер алфавита).
Вычисляются относительные сдвиги между всеми парами строк. Т.к. сдвиги строк таблицы соответствуют сдвигам букв ключа, то остаётся перебрать m возможных ключей и выбрать из них наиболее правдоподобный.
Зашифрованный текст находится в chiper.txt, а расшифрованный текст и ключ находятся в plane.txt.*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Breaking_Vijener_cipher
{
    public static class Odds //Значение индекса совпадений, изменяя его значение можно подобрать длинну ключа, в среднем для русского языка 0.0553
    {
        public const double Koef = 0.058;
    }
    class Alphavit 
    {
        public List<char> alpha = new List<char> { 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф',
            'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я', ' ', ',', '.' };
    }
    class Program
    {
        static void Main(string[] args)
        {
            string cipherFileName = @"../../cipher.txt";
            string planeFileName = @"../../plane.txt";
            StreamReader reader = new StreamReader(cipherFileName, encoding: Encoding.Default);
            StreamWriter writer = new StreamWriter(planeFileName);
            string cipherText = reader.ReadToEnd();
            List<char> key = new List<char>();
            string plane = IndexOfCoincidence(cipherText, ref key);
            writer.WriteLine(plane);
            Console.WriteLine(plane);
            Console.Write("key = '");
            foreach (char c in key)
            { 
                writer.Write(c);
                Console.Write(c);
            }
            Console.WriteLine("'");
            reader.Close();
            writer.Close();
        }
        public static string IndexOfCoincidence(string text,ref List<char> key)//Метод индекс совпадений, делит текст на 1,2,3,... части и подсчитывает для каждой части индекс совпадений в методе GetIndexOfCoincidence
        {
            Alphavit a = new Alphavit();
            int N = a.alpha.Count;
            string newText = "";
            for (int i = 1; i < 25; i++)
            {
                var strings = new string[i];
                for (int j = 0; j < text.Length; j++)
                {
                    strings[j % i] += text[j];
                }
                double abs = 0;
                foreach (string s in strings)
                {
                    abs += GetIndexOfCoincidence(s);
                }
                abs /= strings.Length;
                if (abs > Odds.Koef)//если находится достоверный вариант получаем длинну ключа
                {
                    int max = 0;
                    Console.WriteLine("key length = " + i.ToString());
                    key.Add('a');
                    for (int j = 1; j < strings.Length; j++)
                        strings[j] = MutalIndexOfCoincidence(strings[0], strings[j], ref key);//передаем каждую поделенную строку в метод MutalIndexOfCoincidence
                    for (int j = 0; j < text.Length; j++)
                    {
                        newText += strings[j % i][j / i];
                    }
                    for (int j = 0; j < newText.Length; j++)
                    {
                        if (newText[j] == ' ')
                            max++;
                    }
                    int kMax = 0;
                    for (int k = 1; k < N; k++)
                    {
                        string textCesar = "";
                        int count_ = 0;
                        for (int j = 0; j < newText.Length; j++)
                        {
                            textCesar += a.alpha[(a.alpha.IndexOf(newText[j]) + k) % N];
                            if (textCesar[j] == ' ')
                                count_++;
                        }
                        if (count_ > max)
                        {
                            max = count_;
                            newText = textCesar;
                            kMax = k;
                        }
                    }
                    key[0] = a.alpha[a.alpha.Count - kMax * 2];
                    for (int j = 1; j < key.Count; j++)
                    {
                        key[j] = a.alpha[(a.alpha.Count + a.alpha.IndexOf(key[j]) - kMax * 2) % a.alpha.Count];
                    }
                    return newText;
                }
            }
            return "";
        }
        public static double GetIndexOfCoincidence(string text)//подсчитывает индекс совпадений 
        {
            double result = 0;
            Alphavit a = new Alphavit();
            int N = a.alpha.Count;
            Dictionary<char, int> dict = new Dictionary<char, int>();
            for (int i = 0; i < N; i++)
                dict.Add(a.alpha[i], 0);
            foreach (char c in text)
                dict[c]++;
            for (int i = 0; i < N; i++)
                result += (double)(dict[a.alpha[i]] * (dict[a.alpha[i]] - 1)) / (text.Length * (text.Length - 1));
            return result;
        }
        public static string MutalIndexOfCoincidence(string str1,string str2, ref List<char> key)//метод взаимного индекса совпадений
        {
            double result = 0;
            double max = 0;
            int maxI = 0;
            Dictionary<char, double> dict1 = new Dictionary<char, double>();
            Dictionary<char, double> dict2 = new Dictionary<char, double>();
            Alphavit a = new Alphavit();
            
            int N = a.alpha.Count;
            for (int i = 0; i < N; i++)
            {
                dict1.Add(a.alpha[i], 0);
                dict2.Add(a.alpha[i], 0);
            }
            foreach (char c in str1)
            {
                dict1[c]++;
            }
            foreach (char c in str2)
            {
                dict2[c]++;
            }
            for (int i = 0; i < N; i++)
            {
                dict1[a.alpha[i]] /= str1.Length;
                dict2[a.alpha[i]] /= str2.Length;
                result += (dict1[a.alpha[i]] * dict2[a.alpha[i]]);
                max = result;
            }
            for (int i = 1; i < N; i++)
            {
                result = 0;
                for (int j = 0; j < N; j++)
                {
                    int k = (j + i) % N;
                    result += dict1[a.alpha[j]] * dict2[a.alpha[k]];
                }
                if (result > max)
                {
                    max = result;
                    maxI = i;
                }
            }
            key.Add(a.alpha[maxI]);
            string newStr2 = "";
            for (int i = 0; i < str2.Length; i++)
            {
                int k = 0;
                foreach (char c in dict2.Keys)
                {
                    if (c == str2[i])
                        break;
                    k++;
                }
                newStr2 += dict2.ElementAt(Math.Abs((k + N - maxI)) % N).Key;
            }
            return newStr2;
        }
    }
}
