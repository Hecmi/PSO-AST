using PSO.ArbolExpresiones;
using System;
using System.Collections.Generic;

namespace PSO.Clases
{
    class PSO
    {
        //Variables definidas para la configuración del algoritmo
        int NUMERO_PARTICULAS = 30;
        double W = 0.5;
        double C1 = 1.5;
        double C2 = 1.5;
        int ITERACIONES = 100;

        double[] MEJOR_POSICION_GLOBAL;
        double gBest;

        Random random = new Random();
        double UMBRAL;
        double DOM_MIN;
        double DOM_MAX;

        //Variables relacionadas con la función objetivo
        AST ARBOL_EXPRESIONES;
        int DIMENSION;

        //Otras variables para definir la aceptación de una raíz
        List<double[]> RAICES_ENCONTRADAS;
        List<double> EVALUACION_RAICES_ENCONTRADAS;

        public PSO(int NUMERO_PARTICULAS, int ITERACIONES, double W, double C1, double C2, double umbral, double dom_min, double dom_max, string funcion_objetivo)
        {
            //Inicializar las variables con las definidas por el usuario
            this.NUMERO_PARTICULAS = NUMERO_PARTICULAS;
            this.ITERACIONES = ITERACIONES;

            this.W = W;
            this.C1 = C1;
            this.C2 = C2;

            //Crear la expresión dinámica usando el AST
            ARBOL_EXPRESIONES = new AST(funcion_objetivo);
            DIMENSION = ARBOL_EXPRESIONES.INCOGNITAS.Count;

            this.UMBRAL = umbral;
            this.DOM_MIN = dom_min;
            this.DOM_MAX = dom_max;

            this.RAICES_ENCONTRADAS = new List<double[]>();
            this.EVALUACION_RAICES_ENCONTRADAS = new List<double>();
        }

        private double evaluar_funcion_objetivo(double[] x)
        {
            //Colocar las incógnitas basado en el número de decisiones
            Dictionary<string, double> incognitas_evaluar = new Dictionary<string, double>();
            for (int i = 0; i < DIMENSION; i++)
            {
                incognitas_evaluar[ARBOL_EXPRESIONES.INCOGNITAS[i]] = x[i];
            }

            return ARBOL_EXPRESIONES.evaluar(incognitas_evaluar);
        }

        private bool es_raiz(double valor)
        {
            return valor < UMBRAL;
        }

        private bool es_raiz_encontrada(double[] solucion, double ajuste)
        {
            for (int i = 0; i < RAICES_ENCONTRADAS.Count; i++)
            {
                double[] raiz = RAICES_ENCONTRADAS[i];
                double distancia = 0;

                //Calcular la distancia euclidiana para verificar
                //si no es una raíz repetida
                for (int j = 0; j < DIMENSION; j++)
                {
                    distancia += Math.Pow(solucion[j] - raiz[j], 2);
                }

                //Sí la distancia euclidiana es menor que el umbral
                //significa que es una solución cercana a la ya encontrada
                if (Math.Sqrt(distancia) < UMBRAL)
                {
                    //Sin embargo, se verifica si el ajuste de la nueva solución
                    //es mejor a la ya determinada
                    //if (ajuste < EVALUACION_RAICES_ENCONTRADAS[i])
                    //{
                    //    RAICES_ENCONTRADAS[i] = solucion;
                    //}

                    return true;
                }

            }
            return false;
        }
        private double ajustar_factor_inercia(int iteracion, int max_iteraciones, double w0, double w1)
        {
            double k = 0.1;
            double w_a = w1 + (w0 - w1) * Math.Exp(-k * iteracion);
            return w_a;
        }

        public void ejecutar()
        {            
            //Inicializar las variables correspondientes
            Particula[] particulas = new Particula[NUMERO_PARTICULAS];
            MEJOR_POSICION_GLOBAL = new double[DIMENSION];
            gBest = double.MaxValue;

            //Inicializar las partículas
            for (int i = 0; i < NUMERO_PARTICULAS; i++)
            {
                particulas[i] = new Particula(DIMENSION);
                for (int d = 0; d < DIMENSION; d++)
                {
                    //Iniciar la partícula i en una posición aleatoria
                    //en este caso entre -5 y 5
                    //particulas[i].Posicion[d] = random.NextDouble() * 10 - 5;

                    //Asignar una posición aleatoria basado en el dominio mínimo y máximo
                    particulas[i].Posicion[d] = DOM_MIN + random.NextDouble() * (DOM_MAX - DOM_MIN);
                    //Console.WriteLine(i + ": " + String.Join(',', particulas[i].Posicion));

                    //Asignar aleatoriamente una velocidad a cada elemento de la partícula
                    particulas[i].Velocidad[d] = random.NextDouble() * 2 - 1;
                    particulas[i].MejorPosicion[d] = particulas[i].Posicion[d];
                }

                double ajuste_actual = Math.Abs(evaluar_funcion_objetivo(particulas[i].Posicion));
                particulas[i].PBest = ajuste_actual;

                if (ajuste_actual < gBest)
                {
                    gBest = ajuste_actual;
                    Array.Copy(particulas[i].MejorPosicion, MEJOR_POSICION_GLOBAL, DIMENSION);
                }
            }

            //Ejecutar el algoritmo PSO
            for (int iter = 0; iter < ITERACIONES; iter++)
            {
                //W = ajustar_factor_inercia(iter, ITERACIONES, 0.9, 0.4);
                for (int i = 0; i < NUMERO_PARTICULAS; i++)
                {
                    Particula particula = particulas[i];

                    //Actualizar la velocidad y posición de cada partícula
                    for (int d = 0; d < DIMENSION; d++)
                    {
                        double r1 = random.NextDouble();
                        double r2 = random.NextDouble();

                        particula.Velocidad[d] = W * particula.Velocidad[d]
                                            + C1 * r1 * (particula.MejorPosicion[d] - particula.Posicion[d])
                                            + C2 * r2 * (MEJOR_POSICION_GLOBAL[d] - particula.Posicion[d]);

                        particula.Posicion[d] += particula.Velocidad[d];
                    }

                    //Evaluar la nueva posición de cada partícula
                    double ajuste_actual = Math.Abs(evaluar_funcion_objetivo(particula.Posicion));

                    if (ajuste_actual < particula.PBest)
                    {
                        particula.PBest = ajuste_actual;
                        Array.Copy(particula.Posicion, particula.MejorPosicion, DIMENSION);
                    }

                    //Sí el valor es menor, entonces ajustar el mejor global (gBest)
                    if (ajuste_actual < gBest)
                    {
                        gBest = ajuste_actual;
                        if (es_raiz(ajuste_actual) && !es_raiz_encontrada(particula.Posicion, ajuste_actual))
                        {
                            //Console.WriteLine($"{string.Join(", ", particula.Posicion)}: {evaluar_funcion_objetivo(particula.Posicion)} < {UMBRAL}");
                            RAICES_ENCONTRADAS.Add((double[])particula.Posicion.Clone());
                            EVALUACION_RAICES_ENCONTRADAS.Add(ajuste_actual);
                        }                
                    }                 
                }
                //Console.WriteLine($"Iteración {iter + 1}: Mejor posición = ({string.Join(", ", MEJOR_POSICION_GLOBAL)}), gBest = {gBest}");
            }
        }

        public Dictionary<string, double> get_mejor_solucion()
        {
            for (int i = 0; i < RAICES_ENCONTRADAS.Count; i++)
            {
                Dictionary<string, double> mejor_solucion = new Dictionary<string, double>();
                double[] raiz = RAICES_ENCONTRADAS[i];

                for (int j = 0; j < DIMENSION; j++)
                {
                    mejor_solucion[ARBOL_EXPRESIONES.INCOGNITAS[j]] = raiz[j];
                }

                Console.WriteLine($"{string.Join(", ", mejor_solucion)}, f = {evaluar_funcion_objetivo(raiz)}");                
            }

            return new Dictionary<string, double>();
        }

    }
}