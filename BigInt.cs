using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevLab
{
    public enum Sign
    {
        Plus = 1,
        Minus = -1
    }

    class BigInt
    {
        public Sign Sign { get; set; }
        public List<int> number;
        public static BigInt one => new BigInt("1");
        public static BigInt zero => new BigInt("0");
        

        public BigInt(char _sign, List<int> _number)//конструктор, деалет BigInt из знака и списка чисел
        {
            Sign = _sign == '+'? Sign.Plus: Sign.Minus;
            number = _number;
            this.DelZeros();
        }

        public BigInt()
        {
            Sign = Sign.Plus;
            number = new List<int>() { 0 };
        }

        public BigInt(string str)//конструктор, деалет BigInt из строки
        {
            number = new List<int>();
            if (str[0] == '+')
            {
                Sign = Sign.Plus;
                str = str.Remove(0, 1);
            }
            else if (str[0] == '-')
            {
                Sign = Sign.Minus;
                str = str.Remove(0, 1);
            }
            else if (char.IsDigit(str[0]))
            {
                Sign = Sign.Plus;
            }
            else
                throw new ArgumentException();
            for (int i = 0; i < str.Length; i++)
                number.Add(int.Parse(str[i].ToString()));
            this.DelZeros();
        }

        public BigInt(int num)
        {
            number = new List<int>();
            Sign = num > 0 ? Sign.Plus : Sign.Minus;  //конструктор, делает BigInt из числа
            while(num > 0)
            {
                number.Add(num % 10);
                num /= 10;
            }
            number.Reverse();
        }

        public BigInt(Sign sign, List<int> _number)
        {
            Sign = sign;
            number = _number;
        }

        public override string ToString()//преобразование BigInt к строке
        {
            var str = "";
            foreach (var e in number)
                str += e.ToString();
            var s = Sign == Sign.Plus ? "" : "-";
            return s + str;
        }

        private void DelZeros()//удаляет лишние нули у числа
        {
            if (this.number.Count > 1)
            {
                while (this.number[0] == 0 && this.number.Count > 1)
                    this.number.RemoveAt(0);
            }
        }

        private static void EqualizePlaces(BigInt a, BigInt b)//делает одинаковым количество разрядов у a и b
        {
            if (a.number.Count > b.number.Count)
                b.number.InsertRange(0, new int[a.number.Count - b.number.Count]);
            else
                a.number.InsertRange(0, new int[b.number.Count - a.number.Count]);
        }

        private static BigInt Addition(BigInt a, BigInt b)//сложение a+b
        {
            a.DelZeros(); b.DelZeros();
            var res = new BigInt();
            var p = 0;
            EqualizePlaces(a, b);
            EqualizePlaces(a, res);
            for (var i = a.number.Count - 1; i >= 0; i--)
            {
                var r = a.number[i] + b.number[i] + p;
                p = r / 10;
                res.number[i] = r % 10;
            }
            if (p == 1)
                res.number.Insert(0, 1);
            return res;
        }

        private static BigInt Subtraction(BigInt a, BigInt b)//a - b из большего вычитает меньшее
        {
            if (a == b) return zero;
            if (b.Sign == Sign.Minus) return Addition(a, b);
            a.DelZeros();
            b.DelZeros();
            var res = new BigInt();
            EqualizePlaces(a, b);
            EqualizePlaces(a, res);         
            var a1 = new List<int>(a.number);
            var b1 = new List<int>(b.number);
            for (var i = a1.Count - 1; i >= 0; i--)
            {
                var r = a1[i] - b1[i];
                if (r < 0)
                {
                    a1[i - 1]--;
                    r = 10 + r;
                }
                res.number[i] = r;
            }
            res.DelZeros();
            return res;
        }

        private static BigInt Multiplication(BigInt a, BigInt b)//умножение a * b
        {
            a.DelZeros(); b.DelZeros();
            var result = new BigInt();
            result.Sign = a.Sign == b.Sign ? Sign.Plus : Sign.Minus;
            var a1 = new BigInt(a.Sign, new List<int>(a.number));
            var b1 = new BigInt(b.Sign, new List<int>(b.number));
            //result.EqualizeTheDigits(a.number.Count + b.number.Count);
            var newList = new List<int>(a1.number);
            newList.AddRange(b1.number);
            EqualizePlaces(result, new BigInt(Sign.Plus, newList));
            a1.number.Reverse(); b1.number.Reverse();
            for (var i = 0; i < a1.number.Count; ++i)
                for (int j = 0, carry = 0; j < b1.number.Count || carry > 0; ++j)
                {
                    var cur = result.number[i + j] + a1.number[i] * (j < b1.number.Count ? b1.number[j] : 0) + carry;
                    result.number[i + j] = cur % 10;
                    carry = cur / 10;
                }
            while (result.number.Count > 1 && result.number.Last() == 0)
                result.number.RemoveAt(result.number.Count - 1);
            
            
            result.number.Reverse();
            result.DelZeros();
            return result;
        }

        private static BigInt Div(BigInt a, BigInt b)//деление нацело a / b
        {
            a.DelZeros();
            b.DelZeros();
            var num1 = new BigInt('+', new List<int>(a.number));
            var num2 = new BigInt('+', new List<int>(b.number));
            if (num1 == num2) return one;
            else if (num1 < num2) return zero;
            else
            {
                var listOfDividers = new List<BigInt>();
                while (num1 > num2)
                {
                    var divider = new BigInt(num2.Sign, new List<int>(num2.number));
                    var dividerWithZeros = new BigInt(num2.Sign, new List<int>(num2.number));
                    var newDividend = zero;
                    var newNumeric = 0;
                    var dif = num1.number.Count - num2.number.Count;//1
                    dividerWithZeros *= BigInt.Pow(new BigInt("10"), new BigInt(dif.ToString()));
                    if (dividerWithZeros > num1)
                        newNumeric = dif - 1;//2
                    else
                        newNumeric = dif;
                    var workedPlace = new BigInt('+' + (Math.Pow(10, newNumeric)).ToString());//3

                    for (var i = 1; num2 * workedPlace * new BigInt(i.ToString()) < num1; i++)
                    {
                        divider = workedPlace * new BigInt(i.ToString());
                        newDividend = divider;
                    }
                    if (num1 > newDividend * num2)
                    {
                        if (num1 - newDividend * num2 < num2)
                        {
                            listOfDividers.Add(divider);
                            break;
                        }
                    }
                    else break;
                    num1 -= newDividend * num2;
                    listOfDividers.Add(divider);
                }                
                var result = zero;
                foreach (var e in listOfDividers)
                    result += e;
                result.Sign = a.Sign == b.Sign ? Sign.Plus : Sign.Minus;
                return result;
            }
        }

        private static BigInt Mod(BigInt a, BigInt b)//остаток от деления на b
        {
            a.DelZeros();
            b.DelZeros();
            var num1 = new BigInt('+', new List<int>(a.number));
            var num2 = new BigInt('+', new List<int>(b.number));

            if (num1 < num2) return num1;
            else if (num1 == num2) return zero;
            else
            {

                while (num1 > num2)
                {
                    var dividerWithZeros = new BigInt(num2.Sign, new List<int>(num2.number));

                    BigInt newDividend;
                    var dif = new BigInt((num1.number.Count - num2.number.Count).ToString());//1
                    dividerWithZeros *= Pow(new BigInt(10), dif);
                    if (dividerWithZeros > num1)
                        newDividend = dif - one;//2
                    else
                        newDividend = dif;
                    var workedPlace = Pow(new BigInt(10), newDividend);//3
                    var newDifrRes = num2 * workedPlace;
                    for (var i = 1; num2 * workedPlace * new BigInt(i) <= num1; ++i)
                    {
                        var divider = workedPlace * new BigInt(i);
                        newDifrRes = divider;
                    }//4
                    if (num1 >= newDifrRes * num2)
                    {
                        if (num1 - newDifrRes * num2 < num2)
                        {

                            num1 -= newDifrRes * num2;
                            break;
                        }
                    }
                    else break;
                    num1 -= newDifrRes * num2;
                }
                num1.DelZeros();
                return num1;
            }
        }

        public static BigInt Pow(BigInt a, BigInt b)//а в степени b
        {
            var res = new BigInt(a.Sign, new List<int>(a.number));
            var i = new BigInt(b.Sign, new List<int>(b.number));
            if (i == zero) return one;
            for (; i > one; i -= one)
                res *= a;
            return res;
        }

        public static BigInt operator +(BigInt a, BigInt b)  // a + V
        {
            if (a.Sign == Sign.Plus && b.Sign == Sign.Plus)
                return Addition(a, b);
            else if (a.Sign == Sign.Plus && b.Sign == Sign.Minus)
                return a - -b;
            else if (b.Sign == Sign.Plus && a.Sign == Sign.Minus)
                return b - -a;
            else
            {
                var sum = Addition(a, b);
                sum.Sign = Sign.Minus;
                return sum;
            }
        }

        public static BigInt operator -(BigInt a, BigInt b)  // a - b
        {
            if (a >= b)
            {
                return Subtraction(a, b);
            }
            else
            {
                return -Subtraction(b, a);
            }
        }
        public static BigInt operator *(BigInt a, BigInt b)  // a * b
        {
            return Multiplication(a, b);
        }
        public static BigInt operator /(BigInt a, BigInt b)  // a / b
        {
            return Div(a, b);
        }
        public static BigInt operator %(BigInt a, BigInt b)  // a % b
        {
            return Mod(a, b);
        }
        public static BigInt operator -(BigInt a)   // унарный минус  -a
        {
            return a.Sign == Sign.Plus ? new BigInt('-', a.number) : new BigInt('+', a.number);
        }

        public static int Compare(BigInt a, BigInt b)//сравнивает 2 числа
        {
            a.DelZeros();
            b.DelZeros();
            if (a.Sign > b.Sign) return 1;
            else if (a.Sign < b.Sign) return -1;
            else
            {
                if (a.number.Count > b.number.Count) return a.Sign == Sign.Plus ? 1 : -1;
                else if (a.number.Count < b.number.Count) return a.Sign == Sign.Plus ? -1 : 1;
                else
                {
                    for (var i = 0; i < a.number.Count; i++)
                    {
                        if (a.number[i] > b.number[i])
                            return a.Sign == Sign.Plus? 1 : -1;
                        else if (a.number[i] < b.number[i])
                            return a.Sign == Sign.Plus ? -1 : 1;
                    }
                    return 0;
                }
            }
               
        }

        public static bool operator == (BigInt a, BigInt b) => Compare(a, b) == 0;
        public static bool operator != (BigInt a, BigInt b) => Compare(a, b) != 0;
        public static bool operator < (BigInt a, BigInt b) => Compare(a, b) < 0;
        public static bool operator > (BigInt a, BigInt b) => Compare(a, b) > 0;
        public static bool operator <= (BigInt a, BigInt b) => Compare(a, b) <= 0;
        public static bool operator >= (BigInt a, BigInt b) => Compare(a, b) >= 0;

        public static BigInt AModBInverse(BigInt a, BigInt b)//находит обратное для a по модулю b
        {
            if (a + one == b) return a;
            var x = new BigInt();
            var y = new BigInt();
            ExtendedEuclid(a, b, out x, out y);
            return (x % b + b) % b;
        }

        private static BigInt ExtendedEuclid(BigInt a, BigInt b, out BigInt x, out BigInt y)//расширенный алгоритм евклида, используется для поиска обратного по модулю
        {
            if (a == new BigInt())
            {
                x = new BigInt();
                y = new BigInt(1);
                return b;
            }
            var nX = new BigInt();
            var nY = new BigInt();
            var res = ExtendedEuclid(b % a, a, out nX, out nY);
            x = nY - (b / a) * nX;
            y = nX;
            return res;
        }

        public bool IsSimple()//проверяет является ли число a простым
        {
            if (this < new BigInt("2"))
                return false;
            if (this == new BigInt("2"))
                return true;
            for (var i = new BigInt("2"); i < this; i += new BigInt("1"))
            {
                if (this % i == zero)
                    return false;
            }
            return true;
        }
    }
}