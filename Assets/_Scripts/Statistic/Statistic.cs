using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        double mu = 0.0, sigma = 1.0;
        GaussianDistribution gd = new GaussianDistribution(mu: mu, sigma: sigma);
        DoubleList gaussian_list = new DoubleList();
        int n_total = 1000000;
        double value, alpha = 1.96, k = mu + alpha * sigma, k_prime = mu - alpha * sigma;

        for (int i = 0; i < n_total; i++)
        {
            value = gd.Next();
            gaussian_list.add(value);
        }

        print($"P({k_prime} < X < {k}) = " +
            $"{gaussian_list.rangeNumber(min: k_prime, max: k) * 1.0 / n_total}");

        //print(Trigonometric.Sin(0.5));
        //print(Trigonometric.Cos(0.5));
        //print(Math.Sin(0.5));
        //print(Math.Cos(0.5));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void uniformDistribution()
    {
        DoubleList list = new DoubleList();
        System.Random random = new System.Random();
        int i, n_total = 10000;

        for (i = 0; i < n_total; i++)
        {
            list.add(random.NextDouble());
        }

        for (double d = 0.0; d <= 0.8; d += 0.2)
        {
            int number = list.rangeNumber(d, d + 0.2);
            print($"P({d} < X < {d + 0.2}) = {(double)number / list.length()}");
        }
    }
}

public class DoubleList
{
    List<double> numbers;

    public DoubleList(List<double> numbers = null)
    {
        if(numbers == null)
        {
            clear();
        }
        else
        {
            this.numbers = numbers;
        }        
    }

    public DoubleList(params double[] numbers)
    {
        this.numbers = new List<double>(numbers);
    }

    public double this[int index]
    {
        set { numbers[index] = value; }
        get { return numbers[index]; }
    }

    public static DoubleList operator +(DoubleList list1, DoubleList list2)
    {
        if (list1.length() != list2.length())
        {
            throw new Exception("DoubleList 相加的兩物件，個數需相同");
        }

        int i, len = list1.length();
        double val;
        DoubleList list = new DoubleList();

        for (i = 0; i < len; i++)
        {
            val = list1[i] + list2[i];
            list.add(val);
        }

        return list;
    }

    public static DoubleList operator -(DoubleList list1, DoubleList list2)
    {
        if (list1.length() != list2.length())
        {
            throw new Exception(string.Format("FloatList 相加的兩物件，個數需相同"));
        }

        int i, len = list1.length();
        double val;
        DoubleList list = new DoubleList();

        for (i = 0; i < len; i++)
        {
            val = list1[i] - list2[i];
            list.add(val);
        }

        return list;
    }

    public void add(double val)
    {
        numbers.Add(val);
    }

    public void remove(double value)
    {
        numbers.Remove(value);
    }

    public void removeAt(int index)
    {
        numbers.RemoveAt(index);
    }

    public void clear()
    {
        numbers = new List<double>();
    }

    public int length()
    {
        return numbers.Count;
    }

    public double sum()
    {
        double _sum = 0.0;

        foreach (double num in numbers)
        {
            _sum += num;
        }

        return _sum;
    }

    public double mean(int digit = 4)
    {
        if (length() == 0)
        {
            return 0.0;
        }

        return (float)Math.Round(sum() / length(), digit);
    }

    public double geometricMean(int digit = 4)
    {
        double geometric = 1.0;

        foreach (double num in numbers)
        {
            geometric *= num;
        }

        return (float)Math.Round(Math.Pow(geometric, 1f / length()), digit);
    }

    public void rangeFilter_(double min, double max)
    {
        DoubleList list = rangeFilter(min, max);
        clear();

        int i, len = list.length();

        for(i = 0; i < len; i++)
        {
            add(list[i]);
        }
    }

    public DoubleList rangeFilter(double min, double max)
    {
        List<double> list = new List<double>();

        foreach(double number in numbers)
        {
            if((min <= number) && (number <= max))
            {
                list.Add(number);
            }
        }

        return new DoubleList(list);
    }

    public int rangeNumber(double min, double max)
    {
        int num = 0;

        foreach (double number in numbers)
        {
            if ((min <= number) && (number <= max))
            {
                num++;
            }
        }

        return num;
    }
}
