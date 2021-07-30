using System;
using System.Collections;
using System.Collections.Generic;

namespace CSharpExpansion
{
    public class NumList<T> : IEnumerable
    {
        protected List<T> numbers;

        public NumList(List<T> numbers = null)
        {
            if (numbers == null)
            {
                this.numbers = new List<T>();
            }
            else
            {
                this.numbers = numbers;
            }
        }

        public NumList(NumList<T> list)
        {
            numbers = new List<T>();

            foreach(T number in list)
            {
                numbers.Add(number);
            }
        }

        public NumList(T[] numbers)
        {
            this.numbers = new List<T>(numbers);
        }

        public NumList(int size)
        {
            numbers = new List<T>();

            for (int i = 0; i < size; i++)
            {
                numbers.Add(default(T));
            }
        }

        public T this[int index]
        {
            set { numbers[index] = value; }
            get { return numbers[index]; }
        }

        // 參考：https://stackoverflow.com/questions/8122611/c-sharp-adding-two-generic-values
        public static NumList<T> operator +(NumList<T> list1, NumList<T> list2)
        {
            if (list1.Length() != list2.Length())
            {
                throw new Exception("NumList 相加的兩物件，個數需相同");
            }

            int i, len = list1.Length();
            dynamic v1, v2, val;
            NumList<T> list = new NumList<T>();

            for (i = 0; i < len; i++)
            {
                v1 = list1[i];
                v2 = list2[i];
                val = v1 + v2;
                list.Add(val);
            }

            return list;
        }

        public static NumList<T> operator -(NumList<T> list1, NumList<T> list2)
        {
            if (list1.Length() != list2.Length())
            {
                throw new Exception("NumList 相減的兩物件，個數需相同");
            }

            int i, len = list1.Length();
            dynamic v1, v2, val;
            NumList<T> list = new NumList<T>();

            for (i = 0; i < len; i++)
            {
                v1 = list1[i];
                v2 = list2[i];
                val = v1 - v2;
                list.Add(val);
            }

            return list;
        }

        public void Add(T val)
        {
            numbers.Add(val);
        }

        public void Remove(T value)
        {
            numbers.Remove(value);
        }

        public void RemoveAt(int index)
        {
            numbers.RemoveAt(index);
        }

        public int Length()
        {
            return numbers.Count;
        }

        public void Clear()
        {
            numbers = new List<T>();
        }

        public T GetMaxValue()
        {
            dynamic max_value = numbers[0];

            foreach (T number in numbers)
            {
                if (number > max_value)
                {
                    max_value = number;
                }
            }

            return max_value;
        }

        public (int, T) GetMaxValueIndex()
        {
            int index = 0, len = numbers.Count;
            dynamic max_value = numbers[0];

            for (int i = 1; i < len; i++)
            {
                if (numbers[i] > max_value)
                {
                    max_value = numbers[i];
                    index = i;
                }
            }

            return (index, max_value);
        }

        public T GetMinValue()
        {
            dynamic min_value = numbers[0];

            foreach (T number in numbers)
            {
                if (number < min_value)
                {
                    min_value = number;
                }
            }

            return min_value;
        }

        public (int, T) GetMinValueIndex()
        {
            int index = 0, len = numbers.Count;
            dynamic min_value = numbers[0];

            for (int i = 1; i < len; i++)
            {
                if (numbers[i] < min_value)
                {
                    min_value = numbers[i];
                    index = i;
                }
            }

            return (index, min_value);
        }

        public void RangeFilter_(T min, T max)
        {
            NumList<T> list = RangeFilter(min, max);
            Clear();

            int i, len = list.Length();

            for (i = 0; i < len; i++)
            {
                Add(list[i]);
            }
        }

        public NumList<T> RangeFilter(T min, T max)
        {
            List<T> list = new List<T>();

            foreach (dynamic number in numbers)
            {
                if ((min <= number) && (number <= max))
                {
                    list.Add(number);
                }
            }

            return new NumList<T>(list);
        }

        public int RangeNumber(T min, T max)
        {
            int num = 0;

            foreach (dynamic number in numbers)
            {
                if ((min <= number) && (number <= max))
                {
                    num++;
                }
            }

            return num;
        }

        public void Sort()
        {
            numbers.Sort();
        }

        public T Sum()
        {
            dynamic _sum = default(T);

            foreach (T num in numbers)
            {
                _sum += num;
            }

            return _sum;
        }

        public T Mean(int digit = 4)
        {
            if (Length() == 0)
            {
                return default(T);
            }

            return (T)Math.Round((dynamic)Sum() / Length(), digit);
        }

        public double GeometricMean(int digit = 4)
        {
            double geometric = 1.0;

            foreach (dynamic num in numbers)
            {
                geometric *= num;
            }

            return Math.Round(Math.Pow(geometric, 1.0 / Length()), digit);
        }

        public List<double> Pow(dynamic exp)
        {
            List<double> list = new List<double>();

            foreach (dynamic number in numbers)
            {
                list.Add((double)Math.Pow((double)number, (double)exp));
            }

            return list;
        }

        public double Variance(int digit = 4)
        {
            // 平方期望值
            List<double> square = Pow(exp: 2.0);
            NumList<double> square_list = new NumList<double>(square);
            double square_mean = square_list.Mean();

            // 期望值平方
            double mean = (dynamic)Mean();
            double mean_square = Math.Pow(mean, 2.0);
            double variance = square_mean - (dynamic)mean_square;

            return Math.Round(variance, digit);
        }

        public double Std(int digit = 4)
        {
            double variance = Variance();
            double std = Math.Pow(variance, 0.5);

            return Math.Round(std, digit);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)numbers).GetEnumerator();
        }

        //private enumerator class
        private class NumEnumerator : IEnumerator
        {
            public NumList<T> list;
            private int it = -1;

            //constructor
            public NumEnumerator(NumList<T> list)
            {
                this.list = list;
            }

            private IEnumerator getEnumerator()
            {
                return (IEnumerator)this;
            }

            //IEnumerator
            public bool MoveNext()
            {
                it++;

                return (it < list.Length());
            }

            //IEnumerator
            public void Reset()
            {
                it = -1;
            }

            //IEnumerator
            public object Current
            {
                get
                {
                    try
                    {
                        return list[it];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }  //end nested class
    }
}