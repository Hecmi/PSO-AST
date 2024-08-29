using PSO.ArbolExpresiones;
using System;
using System.Collections.Generic;

namespace PSO.Clases
{
    class PSO_MIN
    {
        //Variables definidas para la configuración del algoritmo
        private int NUMERO_PARTICULAS = 30;
        private double W = 0.5;
        private double C1 = 1.5;
        private double C2 = 1.5;
        private int ITERACIONES = 100;
        double[] MEJOR_POSICION_GLOBAL;

        private Random random = new Random();

        //Variables relacionadas con la función objetivo
        AST ARBOL_EXPRESIONES;

        public PSO_MIN(int NUMERO_PARTICULAS, int ITERACIONES, double W, double C1, double C2, string funcion_objetivo)
        {
            //Inicializar las variables con las definidas por el usuario
            this.NUMERO_PARTICULAS = NUMERO_PARTICULAS;
            this.ITERACIONES = ITERACIONES;

            this.W = W;
            this.C1 = C1;
            this.C2 = C2;

            //Crear la expresión dinámica usando el AST
            ARBOL_EXPRESIONES = new AST(funcion_objetivo);
        }

        private double evaluar_funcion_objetivo(double[] x)
        {
            //Colocar las incógnitas basado en el número de decisiones
            Dictionary<string, double> incognitas_evaluar = new Dictionary<string, double>();
            for (int i = 0; i < ARBOL_EXPRESIONES.INCOGNITAS.Count; i++)
            {
                incognitas_evaluar[ARBOL_EXPRESIONES.INCOGNITAS[i]] = x[i];
            }

            return ARBOL_EXPRESIONES.evaluar(incognitas_evaluar);
        }

        public void ejecutar()
        {
            //Número de dimensiones que es equivalente a la cantidad de 
            //incógnitas de la función objetivo
            int dimensiones = ARBOL_EXPRESIONES.INCOGNITAS.Count;

            //Inicializar las variables correspondientes
            Particula[] particulas = new Particula[NUMERO_PARTICULAS];
            MEJOR_POSICION_GLOBAL = new double[dimensiones];
            double gBest = double.MaxValue;

            //Inicializar las partículas
            for (int i = 0; i < NUMERO_PARTICULAS; i++)
            {
                particulas[i] = new Particula(dimensiones);
                for (int d = 0; d < dimensiones; d++)
                {
                    //Iniciar la partícula i en una posición aleatoria
                    //en este caso entre -5 y 5
                    particulas[i].Posicion[d] = random.NextDouble() * 10 - 5;
                    particulas[i].Velocidad[d] = random.NextDouble() * 2 - 1;

                    particulas[i].MejorPosicion[d] = particulas[i].Posicion[d];
                }

                double ajuste_actual = evaluar_funcion_objetivo(particulas[i].Posicion);
                particulas[i].PBest = ajuste_actual;

                if (ajuste_actual < gBest)
                {
                    gBest = ajuste_actual;
                    Array.Copy(particulas[i].MejorPosicion, MEJOR_POSICION_GLOBAL, dimensiones);
                }
            }

            //Ejecutar el algoritmo PSO
            for (int iter = 0; iter < ITERACIONES; iter++)
            {
                for (int i = 0; i < particulas.Length; i++)
                {
                    Particula particula = particulas[i];

                    //Actualizar la velocidad y posición de cada partícula
                    for (int d = 0; d < dimensiones; d++)
                    {
                        double r1 = random.NextDouble();
                        double r2 = random.NextDouble();

                        particula.Velocidad[d] = W * particula.Velocidad[d]
                                            + C1 * r1 * (particula.MejorPosicion[d] - particula.Posicion[d])
                                            + C2 * r2 * (MEJOR_POSICION_GLOBAL[d] - particula.Posicion[d]);

                        particula.Posicion[d] += particula.Velocidad[d];
                    }

                    //Evaluar la nueva posición de cada partícula
                    double ajuste_actual = evaluar_funcion_objetivo(particula.Posicion);
                    if (ajuste_actual < particula.PBest)
                    {
                        particula.PBest = ajuste_actual;
                        Array.Copy(particula.Posicion, particula.MejorPosicion, dimensiones);

                        //Sí el valor es menor, entonces ajustar el mejor global (gBest)
                        if (ajuste_actual < gBest)
                        {
                            gBest = ajuste_actual;
                            Array.Copy(particula.Posicion, MEJOR_POSICION_GLOBAL, dimensiones);
                        }
                    }
                }
            }
        }

        public Dictionary<string, double> get_mejor_solucion()
        {
            Dictionary<string, double> mejor_solucion = new Dictionary<string, double>();
            for (int i = 0; i < ARBOL_EXPRESIONES.INCOGNITAS.Count; i++)
            {
                mejor_solucion[ARBOL_EXPRESIONES.INCOGNITAS[i]] = MEJOR_POSICION_GLOBAL[i];
            }

            Console.WriteLine($"{string.Join(", ", mejor_solucion)}, f = {evaluar_funcion_objetivo(MEJOR_POSICION_GLOBAL)}");
            return mejor_solucion;
        }
    }
}