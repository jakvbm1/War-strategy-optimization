﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using War_strategy_optimization;

namespace War_strategy_optimization
{
    internal class WSO : IOptimizationAlgorithm
    {
        public int n_iterations;
        public int n_population;
        public int n_dimensions;
        public int n_of_calls = 0;
        public int current_iteration;
        public double p_param;

        double king_result;
        double[] king_arguments;

        double commander_result;
        double[] commander_arguments;

        public double[] results;
        public double[] ranks;
        public double[] weights;
        public double[][] arguments;

        private double[] upper_limit;
        private double[] lower_limit;
        private double[] temp_arguments;
        private double temp_result;

        public delegate double tested_function(params double[] arg);
        private tested_function f;

        private string file_name = "WSO_in_work.txt";

        public double[] XBest => king_arguments ;

        public double FBest => king_result;

        public int NumberOfEvaluationFitnessFunction => n_of_calls;

        public WSO(int n_iterations, int n_population, int n_dimensions, int n_of_calls, tested_function f, double p_param = 0.5)
        {
            this.n_iterations = n_iterations;
            this.n_population = n_population;
            this.n_dimensions = n_dimensions;
            this.n_of_calls = n_of_calls;
            this.current_iteration = 0;
            this.p_param = p_param;
            this.f = f;

            this.upper_limit = new double[n_dimensions];
            this.lower_limit = new double[n_dimensions];
            this.king_arguments = new double[n_dimensions];
            this.commander_arguments = new double[n_dimensions];
            this.results = new double[n_population];
            this.temp_arguments = new double[n_dimensions];
            this.arguments = new double[n_population][];
            this.ranks = new double[n_population];
            this.weights = new double[n_population];

            for (int i = 0; i < n_population; i++)
            {
                this.arguments[i] = new double[n_dimensions];
                ranks[i] = 1;
                weights[i] = 1;
            }
        }

        bool should_it_be_replaced(int i)
        {
            if (temp_result > results[i])
            {
                ranks[i]++;
                double inside_pow = 1 - (ranks[i] / n_iterations);
            weights[i] = weights[i] * Math.Pow(inside_pow, 1);//w artykule jest do potegi alfa ale nie udalo mi sie jeszcze znalezc co to alfa oznacza
                return true;
            }
            else { return false; }
        }

        void replacing(int i)
        {
            for (int j = 0; j < n_dimensions; j++)
            {
                arguments[i][j] = temp_arguments[j];
            }
            results[i] = temp_result;

        }

        void creating_random_soldier()
        {
            for (int i = 0; i < n_dimensions; i++)
            {
                Random rand = new Random();
                temp_arguments[i] = lower_limit[i] + rand.NextDouble() * (upper_limit[i] - lower_limit[i]);
            }
            temp_result = f(temp_arguments);
        }

        void creating_initial_population()
        {
            for (int i = 0; i < n_population; i++)
            {
                creating_random_soldier();
                replacing(i);
            }
            current_iteration++;
        }

        void finding_king_and_commander()
        {
            int kings_number = 0;
            int commanders_number = 0;
            for (int i = 0; i <n_population; i++)
            {
                if (results[i] < results[kings_number])
                {
                    commanders_number = kings_number;
                    kings_number = i;
                }
            }

            for (int i=0; i < n_dimensions; i++)
            {
                king_arguments[i] = results[kings_number];
                commander_arguments[i] = results[commanders_number];
            }

            king_result = results[kings_number];
            commander_result = results[commanders_number];
        }

        void attack_exploitation(int transformed_num, double p)
        {
            Random rand = new Random();
            for (int i=0; i<n_dimensions;i++)
            {
                temp_arguments[i] = arguments[transformed_num][i] + 2 * p * (commander_arguments[i] - king_arguments[i])
                + rand.NextDouble() * (weights[transformed_num] * king_arguments[i] - arguments[transformed_num][i]);
            }
            if(should_it_be_replaced(transformed_num))
            {
                replacing(transformed_num);
            }
        }

        void defense_exploration(int transformed_num, double p)
        {
            double[] random_soldier = new double[n_dimensions];
            Random rand = new Random();
            for (int i=0; i<n_dimensions; i++)
            {
                random_soldier[i] = lower_limit[i] + rand.NextDouble() * (upper_limit[i] - lower_limit[i]);
            }

            for (int i=0; i<n_dimensions; i++)
            {
                temp_arguments[i] = arguments[transformed_num][i]  + 2*p*(king_arguments[i] - random_soldier[i])
                    + rand.NextDouble() * weights[transformed_num] * (commander_arguments[i] - arguments[transformed_num][i]);
            }

            if (should_it_be_replaced(transformed_num))
            {
                replacing(transformed_num);
            }
        }

        double median(int i) // funckja do znajdowania mediany argumentow dla konkretnego wymiaru
        {
            double[] copied_arguments = new double[n_population];
            for (int j=0; j<n_population; j++)
            {
                copied_arguments[j] = arguments[j][i];
            }

            for (int j=0; j<n_population; j++)
            {
                for (int k=0; k<n_population-j; k++)
                {
                    if (copied_arguments[k] > copied_arguments[k+1])
                    {
                        double temporary = copied_arguments[k];
                        copied_arguments[k] = copied_arguments[k+1];
                        copied_arguments[k + 1] = temporary;
                    }
                }
            }

            if (n_population % 2 ==1)
            {
                return copied_arguments[(n_population - 1) / 2 + 1];
            }

            else 
            {
                return (copied_arguments[(n_population / 2)] + copied_arguments[(n_population / 2 + 1)]) / 2;
            }
        }

        void relocating_the_weak()
        {
            Random rand = new Random();
            int the_weakest = 0;
            for (int i=0; i<n_population; i++)
            {
                if (results[the_weakest] < results[i])
                {
                    the_weakest = i;
                }
            }

            for (int i=0; i<n_dimensions; i++)
            {
                arguments[the_weakest][i] = -(1-rand.NextDouble()) * (arguments[the_weakest][i] - median(i) + king_arguments[i]);
            }

            results[the_weakest] = f(arguments[the_weakest]);

        }
        public void SaveToFileStateOfAlghoritm()
        {
            StreamWriter sw = File.CreateText(file_name);
            sw.WriteLine(n_of_calls);
            sw.WriteLine(current_iteration);

            sw.Write(king_result + ", ");
            for (int i = 0; i < n_dimensions; i++)
            {
                sw.Write(king_arguments[i] + ", ");
            }
            sw.Write('\n');

            sw.Write(commander_result);
            for (int i = 0; i < n_dimensions; i++)
            {
                sw.Write(commander_arguments[i]);
            }
            sw.Write('\n');

            for (int i = 0; i < n_population; i++)
            {
                sw.Write(results[i] + ", ");
                for (int j = 0; j < n_dimensions; j++)
                {
                    sw.Write(arguments[i][j] + ", ");
                }
                sw.Write('\n');
            }

            for (int i = 0; i < n_population; i++)
            {
                sw.Write(ranks[i]);
            }
            sw.Write('\n');
            for (int i = 0; i < n_population; i++)
            {
                sw.Write(weights[i]);
            }
            sw.Close();
        }

        public void LoadFromFileStateOfAlghoritm()
        {
            if (File.Exists(file_name))
            {
                StreamReader sr = new StreamReader(file_name);
                string line = "";

                line = sr.ReadLine();
                n_of_calls = Convert.ToInt32(line);

                line = sr.ReadLine();
                current_iteration = Convert.ToInt32(line);

                line = sr.ReadLine();
                string[] numbers = line.Split(", ");

                king_result = Convert.ToDouble(numbers[0]);
                for (int i = 0; i < n_dimensions; i++)
                {
                    king_arguments[i] = Convert.ToDouble(numbers[1 + i]);
                }

                line = sr.ReadLine();
                numbers = line.Split(", ");

                commander_result = Convert.ToDouble(numbers[0]);
                for (int i = 0; i < n_dimensions; i++)
                {
                    commander_arguments[i] = Convert.ToDouble(numbers[1 + i]);
                }
                for (int i = 0; i < n_population; i++)
                {
                    line = sr.ReadLine();
                    numbers = line.Split(", ");

                    results[i] = Convert.ToDouble(numbers[0]);
                    for (int j = 0; j < n_dimensions; j++)
                    {
                        arguments[i][j] = Convert.ToDouble(numbers[1 + j]);
                    }

                }
                line = sr.ReadLine();
                numbers = line.Split(", ");
                for (int i = 0; i < n_population; i++)
                {
                    ranks[i] = Convert.ToDouble(numbers[i]);
                }

                line = sr.ReadLine();
                numbers = line.Split(", ");
                for (int i = 0; i < n_population; i++)
                {
                    weights[i] = Convert.ToDouble(numbers[i]);
                }
                sr.Close();
            }

        }

        public void SaveResult()
        {
            throw new NotImplementedException();
        }

        public double Solve()
        {
            LoadFromFileStateOfAlghoritm();
            if (current_iteration == 0)
            {
                creating_initial_population();
            }
            finding_king_and_commander();
            Random rand = new Random();

            for (int ci = current_iteration; ci > n_iterations; ci++)
            {
                for (int i = 0; i < n_population; i++)
                {
                    double p = rand.NextDouble();
                    if (p < p_param)
                    {
                        defense_exploration(i, p);
                    }

                    else
                    {
                        attack_exploitation(i, p);
                    }
                }
                finding_king_and_commander();
                relocating_the_weak();
                SaveToFileStateOfAlghoritm();
            }
            return king_result;
        }
    }
}
