using System;
using System.Collections.Generic;
using PSO.ArbolExpresiones;

namespace PSO
{
    class Program
    {    
        public static void Main()
        {
            //string ecuacion = "-(-(-x+1)*(x-8,5)*(x-3))";
            //AST ast = new AST(ecuacion);

            //ast.mostrar_arbol();

            //Dictionary<string, double> valores_incognitas = new Dictionary<string, double>
            //{
            //    { "x", 2 },
            //    { "y", 100 },
            //};

            //for (int i = 0; i < ast.INCOGNITAS.Count; i++)
            //{
            //    Console.WriteLine(ast.INCOGNITAS[i]);
            //}

            //double resultado = ast.evaluar(valores_incognitas);
            //Console.WriteLine(resultado);

            string ECUACION = "(10-x)^2+100*(y-x^2)^2";
            ECUACION = "x^2-100*x+16";

            int NUMERO_PARTICULAS = 30000;
            int NUMERO_ITERACIONES = 100;
            double FACTOR_INERCIA = 0.5;
            double FACTOR_COGNITIVO = 1.5;
            double FACTOR_SOCIAL = 1.5;

            Clases.PSO pso = new Clases.PSO(
                NUMERO_PARTICULAS,
                NUMERO_ITERACIONES,
                FACTOR_INERCIA,
                FACTOR_COGNITIVO,
                FACTOR_SOCIAL,
                0.2,
                -1,
                300,
                ECUACION
            );

            pso.ejecutar();
            pso.get_mejor_solucion();

            //Clases.PSO_MIN pso_min = new Clases.PSO_MIN(
            //    NUMERO_PARTICULAS,
            //    NUMERO_ITERACIONES,
            //    FACTOR_INERCIA,
            //    FACTOR_COGNITIVO,
            //    FACTOR_SOCIAL,              
            //    ECUACION
            //);

            //pso_min.ejecutar();
            //pso_min.get_mejor_solucion();
        }
    }
}
