using LevLab;
using System;
using System.Collections.Generic;
using System.Xml;

namespace LevLab
{
    class RSA
    {
        char[] characters = new char[] { '#', 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И',
                                                'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С',
                                                'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ь', 'Ы', 'Ъ',
                                                'Э', 'Ю', 'Я', ' ', '1', '2', '3', '4', '5', '6', '7',
                                                '8', '9', '0', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
                                                'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S',
                                                'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        
        public string MakeRSA(BigInt p, BigInt q, string str)
        {
            var inputStr = str.ToUpper();
            if (p.IsSimple() && q.IsSimple())
            {
                var n = p * q;
                var m = ((p - BigInt.one) * (q - BigInt.one));
                var e = FindE(m);
                var d = BigInt.AModBInverse(e, m);
                var encodeRes = Encode(inputStr, e, n);
                var decodeRes = Decode(encodeRes, d, n);
                Console.WriteLine(decodeRes);
                return decodeRes;
            }
            else
                Console.WriteLine("p или q не простые числа");
            return "";
        }

        private string Decode(List<string> encodeRes, BigInt d, BigInt n)
        {
            var res = "";
            foreach (var e in encodeRes)
            {
                var b = new BigInt(e);
                b = BigInt.Pow(b,d);
                b = b % n;
                var index = Convert.ToInt32(b.ToString());
                res += characters[index].ToString();
            }
            return res;
        }

        private List<string> Encode(string s, BigInt e, BigInt n)
        {
            var res = new List<string>();
            for (var i = 0; i < s.Length; i++)
            {
                var index = Array.IndexOf(characters, s[i]);

                var r = new BigInt(index);
                r = BigInt.Pow(r, e);

                r = r % n;
                res.Add(r.ToString());
            }
            return res;
        }

        private BigInt FindE(BigInt m)//находим коэф e
        {
            var e = m - BigInt.one;
            for (var i = new BigInt(2); i <= m; i += BigInt.one)
                if ((m % i == BigInt.zero) && (e % i == BigInt.zero))
                {
                    e -= BigInt.one;
                    i = BigInt.one;
                }
            return e;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var a = new BigInt();
        if (a.Sign != Sign.Plus || a.number.Equals(new List<int>()))
            Console.WriteLine("Ошибка конструктора 1");
        var b = new BigInt("+123");
        if (b.Sign != Sign.Plus || b.number.Equals(new List<int>() { 1, 2, 3 }))
            Console.WriteLine("Ошибка конструктора 2");
        var c = new BigInt("123");
        if (c.Sign != Sign.Plus || a.number.Equals(new List<int>() { 1, 2, 3 }))
            Console.WriteLine("Ошибка конструктора 3");
        var d = new BigInt("-123");
        if (d.Sign != Sign.Minus || d.number.Equals(new List<int>() { 1, 2, 3 }))
            Console.WriteLine("Ошибка конструктора 4");
        var e = new BigInt('-', new List<int>() { 1, 2, 3 });
        if (e.Sign != Sign.Minus || e.number.Equals(new List<int>() { 1, 2, 3 }))
            Console.WriteLine("Ошибка конструктора 5");

        var f = new BigInt("54321");
        var g = new BigInt("12345");
        var h = new BigInt("-123");
        var i = new BigInt("100");
        var j = new BigInt("-54321");
        var k = BigInt.zero;
        // var i = f + g;//66666
        //var j = f + h;//12222

        //Сложение тест
        var s1 = f + g;
        if (s1.ToString() != "66666")
            Console.WriteLine("Ошибка сложения 1");
        var s2 = f + h;
        if (s2.ToString() != "54198")
            Console.WriteLine("Ошибка сложения 2");
        var s3 = f + i;
        if (s3.ToString() != "54421")
            Console.WriteLine("Ошибка сложения 3");
        var s4 = j + i;
        if (s4.ToString() != "-54221")
            Console.WriteLine("Ошибка сложения 4");
        var s5 = g + j;
        if (s5.ToString() != "-41976")
            Console.WriteLine("Ошибка сложения 4");

        //Вычитание тест
        var sub1 = f - g;
        if (sub1.ToString() != "41976")
            Console.WriteLine("Ошибка вычитания 1");
        var sub2 = f - h;
        if (sub2.ToString() != "54444")
            Console.WriteLine("Ошибка вычитания 2");
        var sub3 = f - j;
        if (sub3.ToString() != "108642")
            Console.WriteLine("Ошибка вычитания 3");
        var sub4 = j - h;
        if (sub4.ToString() != "-54444")
            Console.WriteLine("Ошибка вычитания 4");
        var sub5 = g - j;
        if (sub5.ToString() != "66666")
            Console.WriteLine("Ошибка вычитания 5");

        //Умножение тест
        var m1 = f * g;
        if (m1.ToString() != "670592745")
            Console.WriteLine("Ошибка умножения 1");
        var m2 = f * h;
        if (m2.ToString() != "-6681483")
            Console.WriteLine("Ошибка умножения 2");
        var m3 = h * i;
        if (m3.ToString() != "-12300")
            Console.WriteLine("Ошибка умножения 3");
        var m4 = g * k;
        if (m4.ToString() != "0")
            Console.WriteLine("Ошибка умножения 4");
        var m5 = g * j;
        if (m5.ToString() != "-670592745")
            Console.WriteLine("Ошибка умножения 5");

        //Деление тест
        var d1 = f / g;
        if (d1.ToString() != "4")
            Console.WriteLine("Ошибка деления 1");
        var d2 = g / i;
        if (d2.ToString() != "123")
            Console.WriteLine("Ошибка деления 2");
        var d3 = j / i;
        if (d3.ToString() != "-543")
            Console.WriteLine("Ошибка деления 3");
        var d4 = j % i;
        if (d4.ToString() != "21")
            Console.WriteLine("Ошибка деления 4");
        var d5 = f % g;
        if (d5.ToString() != "4941")
            Console.WriteLine("Ошибка деления 5");
        var d6 = g % h;
        if (d6.ToString() != "45")
            Console.WriteLine("Ошибка деления 6");
        var d7 = h % g;
        if (d7.ToString() != "123")
            Console.WriteLine("Ошибка деления 7");
        var d8 = i / g;
        if (d8.ToString() != "0")
            Console.WriteLine("Ошибка деления 8");
        var d9 = new BigInt(143) % new BigInt(72);
        if (d9.ToString() != "71")
            Console.WriteLine("Ошибка деления 9");

        //Проверка на простое число тест
        var simple1 = new BigInt(3);
        if (!simple1.IsSimple())
            Console.WriteLine("Ошибка простые числа 1");
        var simple2 = new BigInt(37);
        if (!simple2.IsSimple())
            Console.WriteLine("Ошибка простые числа 2");
        var simple3 = new BigInt(103);
        if (!simple3.IsSimple())
            Console.WriteLine("Ошибка простые числа 3");
        var simple4 = new BigInt(179);
        if (!simple4.IsSimple())
            Console.WriteLine("Ошибка простые числа 4");
        var simple5 = new BigInt(28);
        if (simple5.IsSimple())
            Console.WriteLine("Ошибка простые числа 5");

        //RSA тест
        var rsa1 = new RSA();
        var rs1 = rsa1.MakeRSA(new BigInt(13), new BigInt(7), "test");
        if (rs1 != "TEST")
            Console.WriteLine("Ошибка RSA 1");
        var rsa2 = new RSA();
        var rs2 = rsa1.MakeRSA(new BigInt(7), new BigInt(17), "lab rabota");
        if (rs2 != "LAB RABOTA")
            Console.WriteLine("Ошибка RSA 2");
        //-----------------------------------------------------------

        var p = "";
        var q = "";
        var str = "";
        
        var xDoc = new XmlDocument();
        xDoc.Load("lab1.xml");
        var xRoot = xDoc.DocumentElement;
        foreach (XmlNode xNode in xRoot)
        {

            if (xNode.Name == "q")
                q = xNode.InnerText;
            if (xNode.Name == "p")
                p = xNode.InnerText;
            if (xNode.Name == "text")
                str = xNode.InnerText;

        }
        var rsa = new RSA();
        rsa.MakeRSA(new BigInt(p), new BigInt(q), str);
        Console.ReadKey();
    }
}

