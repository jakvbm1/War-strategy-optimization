using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace War_strategy_optimization
{
    internal class WSO
    {
        public int n_iterations;
        public int n_population;
        public int n_dimensions;
        public int n_of_calls;
        public int current_iteration;
        public double p_param;

        double king_result;
        double[] king_arguments;

        double commander_result;
        double[] commander_arguments;

        public double[] results;
        public double[][] arguments;

        private double[] upper_limit;
        private double[] lower_limit;
        private double[] temp_arguments;

        public delegate double tested_function(params double[] arg);
        private tested_function f;

        public WSO(int n_iterations, int n_population, int n_dimensions, int n_of_calls, int current_iteration, tested_function f, double p_param=0.5)
        {
            this.n_iterations = n_iterations;
            this.n_population = n_population;
            this.n_dimensions = n_dimensions;
            this.n_of_calls = n_of_calls;
            this.current_iteration = current_iteration;
            this.p_param = p_param;
            this.f = f;

            this.upper_limit = new double[n_dimensions];
            this.lower_limit = new double[n_dimensions];
            this.king_arguments = new double[n_dimensions];
            this.commander_arguments = new double[n_dimensions];
            this.results = new double[n_population];
            this.temp_arguments = new double[n_dimensions];
            this.arguments = new double[n_population][];

            for (int i = 0; i < n_population; i++)
            {
                this.arguments[i] = new double[n_dimensions];
            }
        }
    }
}
