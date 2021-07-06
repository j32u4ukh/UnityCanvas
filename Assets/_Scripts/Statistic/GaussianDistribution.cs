
using System;
using System.Collections.Generic;
using UnityEngine;

public class GaussianDistribution
{
    /* 均勻分配 轉 高斯分配
     x = np.sqrt(-2.0 * np.log(u)) * np.cos(2.0 * np.pi * v) * sigma + mu
     y = np.sqrt(-2.0 * np.log(u)) * np.sin(2.0 * np.pi * v) * sigma + mu
     */
    private double mu;
    private double sigma;
    private bool use_sin = false;
    private System.Random random;
    private List<double> gaussian_list;
    private int index = 0;

    public GaussianDistribution(double mu, double sigma)
    {
        this.mu = mu;
        this.sigma = sigma;
        random = new System.Random();
        gaussian_list = new List<double>();
    }

    public double Next()
    {
        if (index % 2 == 0)
        {
            double sqrt = Math.Sqrt(-2.0 * Math.Log(random.NextDouble())) * sigma;
            double rand = 2.0 * Math.PI * random.NextDouble();
            double sin = Math.Sin(rand);
            double cos = Math.Cos(rand);

            gaussian_list.Add(mu + sqrt * sin);
            gaussian_list.Add(mu + sqrt * cos);
        }

        double value = gaussian_list[index];
        index++;

        return value;
    }
}

public class Trigonometric
{
    public static double Deg2Rad = Math.PI / 180.0;

    public static double Sin(double angle)
    {
        return Math.Sin(angle * Deg2Rad);
    }

    public static double Cos(double angle)
    {
        return Math.Cos(angle * Deg2Rad);
    }
}
