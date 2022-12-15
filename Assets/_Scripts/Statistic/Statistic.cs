using CSharpExpansion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Statistic : MonoBehaviour
{
    List<double> list;

    // Start is called before the first frame update
    void Start()
    {
        GaussianNumList gd0_5 = new GaussianNumList(mu: 0.5, sigma: 1.0);
        gd0_5.Truncated(min: 0.0, max: 1.0);
        print($"gd0_5 | GetPValue(0.5): {gd0_5.GetPValue(0.5)}");
        print($"gd0_5 | GetValueR(0.05): {gd0_5.GetValueR(0.05)}");

        GaussianNumList gd0_8 = new GaussianNumList(mu: 0.8, sigma: 1.0);
        gd0_8.Truncated(min: 0.0, max: 1.0);
        print($"gd0_8 | GetPValue(0.5): {gd0_8.GetPValue(0.5)}");
        print($"gd0_8 | GetValueR(0.05): {gd0_8.GetValueR(0.05)}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void uniformDistribution()
    {
        NumList<double> list = new NumList<double>();
        System.Random random = new System.Random();
        int i, n_total = 10000;

        for (i = 0; i < n_total; i++)
        {
            list.Add(random.NextDouble());
        }

        for (double d = 0.0; d <= 0.8; d += 0.2)
        {
            int number = list.RangeNumber(d, d + 0.2);
            print($"P({d} < X < {d + 0.2}) = {(double)number / list.Length()}");
        }
    }

    void normalDistribution()
    {
        double mu = 0.0, sigma = 1.0;
        GaussianDistribution gd = new GaussianDistribution(mu: mu, sigma: sigma);
        NumList<double> gaussian_list = new NumList<double>();
        int n_total = 1000000;
        double value, alpha = 1.96, k = mu + alpha * sigma, k_prime = mu - alpha * sigma;

        for (int i = 0; i < n_total; i++)
        {
            value = gd.Next();
            gaussian_list.Add(value);
        }

        print($"P({k_prime} < X < {k}) = " +
            $"{gaussian_list.RangeNumber(min: k_prime, max: k) * 1.0 / n_total}");
    }

    public (int, int) findBoundary(double value, int lower_bound, int higher_bound)
    {
        print($"findBoundary(value: {value}, lower_bound: {lower_bound}, higher_bound: {higher_bound})");

        if (higher_bound - lower_bound <= 1)
        {
            return (lower_bound, higher_bound);
        }
        else
        {
            int center = (int)Math.Floor((lower_bound + higher_bound) / 2.0);
            print($"center: {center}, value: {list[center]}");

            if (value < list[center])
            {
                print("Lower");
                return findBoundary(value, lower_bound, center);
            }
            else
            {
                print("Higher or Equal");
                return findBoundary(value, center, higher_bound);
            }
        }
    }
}