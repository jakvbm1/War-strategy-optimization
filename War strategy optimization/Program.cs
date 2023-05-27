
using System.Runtime.CompilerServices;

namespace War_strategy_optimization
{
    class Program
    {
        public static double Rastrigin_function(params double[] x) // minimum (0,0, ..., 0) 0, df: [-5.12; 5.12]
        {
            int n = x.Length;
            double sum = 0;
            for (int i = 0; i < n; i++)
                sum += x[i] * x[i] - 10 * Math.Cos(2 * Math.PI * x[i]);
            return 10 * n + sum;
        }

        public static double Rosenbrock_function(params double[] x) //minimum (1, 1, ..., 1) 0 df: caly przedzial R
        {
            int dimension = x.Length;
            double sum = 0;
            if (dimension != 1)
            {
                for (int i = 0; i < dimension - 1; i++)
                {
                    sum += 100 * Math.Pow((x[i + 1] - (x[i] * x[i])), 2) + Math.Pow((1 - x[i]), 2);
                }
            }
            return sum;
        }

        public static double Sphere_function(params double[] x)// minimum (0,0, ..., 0) 0, df: caly przedzial R
        {
            int dimension = x.Length;
            double sum = 0;
            for (int i = 0; i < dimension; i++)
            {
                sum += x[i] * x[i];
            }
            return sum;
        }

        public static double Beale_function(params double[] x) //minimum (3, 0.5) 0, df:[-4,5; 4,5] tylko dwa wymiary!
        {
            int dimension = x.Length;
            double sum = 0;
            if (dimension == 2)
            {
                sum = Math.Pow((1.5 - x[0] + (x[0] * x[1])), 2) + Math.Pow((2.25 - x[0] + (x[0] * x[1] * x[1])), 2)
                    + Math.Pow((2.625 - x[0] + (x[0] * x[1] * x[1] * x[1])), 2);

                return sum;
            }
            else
            {
                return 10000000;
            }
        }

        public static double Bukin_function_N6(params double[] x) //minimum (-10, 1) 0 df [-15; -5] x [-3; 3] tylko dwa wymiary!
        {
            int dimension = x.Length;
            double sum = 0;
            if (dimension == 2)
            {
                sum = 100 * Math.Sqrt(Math.Abs(x[1] - (0.01 * x[0] * x[0]))) + 0.01 * Math.Abs(x[0] + 10);
                return sum;
            }
            else
            {
                return 10000000;
            }
        }

        public static double Himmelblaus_function_N6(params double[] x) //df [-5;5], minima: (3, 2), (-2.805118, 3.131312), (-3.779310, -3.283186), (3.584428, -1.848126) 0, tylko dwa wymiary!
        {
            int dimension = x.Length;
            double sum = 0;
            if (dimension == 2)
            {
                sum = Math.Pow((x[0] * x[0] + x[1] - 11), 2) + Math.Pow((x[1] * x[1] + x[0] - 7), 2);
                return sum;
            }
            else
            {
                return 10000000;
            }
        }

        public static double Eggholder_function_N6(params double[] x) //df [-512; 512], minimum: (512, 404.2319) -959.6407 tylko dwa wymiary!
        {
            int dimension = x.Length;
            double sum = 0;
            if (dimension == 2)
            {
                sum = -(x[1] + 47) * Math.Sin(Math.Sqrt(Math.Abs(x[0] / 2 + x[1] + 47))) - x[0] * Math.Sin(Math.Abs(x[0] - (x[1] + 47)));
                return sum;
            }
            else
            {
                return 10000000;
            }
        }

        public static void setting_limits(ref double[] upper_limit, ref double[] lower_limit, int dimension)
        {
            string user_input;
            double limit_d, limit_u;
            Console.WriteLine("Czy granice dla wszystkich wymiarow sa identyczne? [t/n]");
            char ans;
            ans = Convert.ToChar(Console.ReadLine());

            if (ans == 't' || ans == 'T')
            {

                Console.WriteLine("podaj ograniczenie dolne");
                user_input = Console.ReadLine();
                limit_d = Double.Parse(user_input);


                Console.WriteLine("podaj ograniczenie gorne");
                user_input = Console.ReadLine();
                limit_u = Double.Parse(user_input);

                for (int i = 0; i < dimension; i++)
                {
                    upper_limit[i] = limit_u;
                    lower_limit[i] = limit_d;
                }
            }

            else
            {
                for (int i = 0; i < dimension; i++)
                {
                    Console.WriteLine("Wymiar: " + (i + 1));
                    Console.WriteLine("podaj ograniczenie dolne");
                    user_input = Console.ReadLine();
                    limit_d = Double.Parse(user_input);

                    Console.WriteLine("podaj ograniczenie gorne");
                    user_input = Console.ReadLine();
                    limit_u = Double.Parse(user_input);
                    upper_limit[i] = limit_u;
                    lower_limit[i] = limit_d;
                }
            }
        }

        private static void Main(string[] args)
        {
            int[] dimensions = { 2, 5, 10, 30, 50 };
            int[] iterations = { 5, 10, 20, 50 };
            int[] population = { 10, 15, 20, 50, 100 };

            //int j = 0;
            int d = 0;
            //int p = 0


            double[] upper_limit = new double[dimensions[d]];
            double[] lower_limit = new double[dimensions[d]];

            setting_limits(ref upper_limit, ref lower_limit, dimensions[d]);

            WSO test = new WSO(iterations[3], population[2], dimensions[d], Rastrigin_function);
            test.limit_setter(lower_limit, upper_limit);
            test.Solve();

            //for(int a=0; a <5; a++)
            //{
            //    double[] upper_lim = new double[dimensions[a]];
            //    double[] lower_lim = new double[dimensions[a]];

            //    for (int i = 0; i < dimensions[a]; i++)
            //    {
            //        upper_lim[i] = 0;
            //        lower_lim[i] = 0;
            //    }

            //    setting_limits(ref upper_lim, ref lower_lim, dimensions[a]);

            //    for (int b=0; b <4; b++)
            //    {
            //        for (int c=0; c <5; c++)
            //        {
            //            string end_file = "C:\\WSO\\END_RESULT";
            //            string st = "D"+dimensions[a].ToString();
            //            string st2 = "I"+iterations[b].ToString();
            //            string st3 = "P"+population[c].ToString();
            //            StreamWriter sw = File.CreateText(end_file+st+st2+st3+".txt");
            //            for (int e=0; e <10; e++)
            //            {
            //                WSO wso_test = new WSO(iterations[b], population[c], dimensions[a], Rastrigin_function);
            //                wso_test.limit_setter(lower_lim, upper_lim);
            //                double to_save = wso_test.Solve();
            //                sw.WriteLine(to_save);
            //            }
            //            sw.Close();
            //        }
            //    }
            //}


            
            





            
        }
    }
}
