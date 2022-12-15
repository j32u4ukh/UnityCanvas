using System;
using System.Collections.Generic;

namespace CSharpExpansion
{
    public class GaussianDistribution
    {
        public double Mu
        {
            get { return mu; }
        }
        private double mu;

        public double Sigma
        {
            get { return sigma; }
        }
        private double sigma;

        public double Max
        {
            get { return gaussian_list[number - 1]; }
        }

        public double Min
        {
            get { return gaussian_list[0]; }
        }

        private Random random;
        private NumList<double> gaussian_list;
        private int index = 0;
        private int number;

        public GaussianDistribution(double mu, double sigma, int number = 10000)
        {
            this.mu = mu;
            this.sigma = sigma;

            random = new System.Random();
            Build(number: number);
        }

        public void Build(int number = 10000)
        {
            if (number >= 0)
            {
                this.number = number;

                if (number % 2 == 1)
                {
                    this.number++;
                }

                double sqrt, rand, sin, cos;
                gaussian_list = new NumList<double>();

                for (int i = 0; i < number; i += 2)
                {
                    /* 均勻分配 轉 高斯分配
                     x = np.sqrt(-2.0 * np.log(u)) * np.cos(2.0 * np.pi * v) * sigma + mu
                     y = np.sqrt(-2.0 * np.log(u)) * np.sin(2.0 * np.pi * v) * sigma + mu
                     */
                    sqrt = Math.Sqrt(-2.0 * Math.Log(random.NextDouble())) * sigma;
                    rand = 2.0 * Math.PI * random.NextDouble();
                    sin = Math.Sin(rand);
                    cos = Math.Cos(rand);

                    gaussian_list.Add(mu + sqrt * sin);
                    gaussian_list.Add(mu + sqrt * cos);
                }

                gaussian_list.Sort();
            }
        }

        public double Next()
        {
            index = random.Next(0, number);

            return gaussian_list[index];
        }

        public void Truncated(double min, double max)
        {
            List<double> list = new List<double>();

            foreach (double d in gaussian_list)
            {
                if ((min <= d) && (d <= max))
                {
                    list.Add(d);
                }
            }

            number = list.Count;
            gaussian_list = new NumList<double>(list);
            gaussian_list.Sort();
        }

        double GetValue(double index)
        {
            int floor = (int)Math.Floor(index), ceiling = (int)Math.Ceiling(index);
            double w_floor = ceiling - index, w_ceiling = index - floor;

            return gaussian_list[floor] * w_floor + gaussian_list[ceiling] * w_ceiling;
        }

        public double GetValueL(double p)
        {
            p = Math.Max(Math.Min(1.0, p), 0.0);
            double index = p * number;

            return GetValue(index);
        }

        public double GetValueR(double p)
        {
            return GetValueL(1.0 - p);
        }

        public double GetPValue(double value)
        {
            (int, int) boundary = FindBoundary(value, 0, number);

            return (boundary.Item1 * 1.0) / number;
        }

        public (int, int) FindBoundary(double value, int lower_bound, int higher_bound)
        {
            if (higher_bound - lower_bound <= 1)
            {
                return (lower_bound, higher_bound);
            }
            else
            {
                int center = (int)Math.Floor((lower_bound + higher_bound) / 2.0);

                if (value < gaussian_list[center])
                {
                    return FindBoundary(value, lower_bound, center);
                }
                else
                {
                    return FindBoundary(value, center, higher_bound);
                }
            }
        }
    }
}