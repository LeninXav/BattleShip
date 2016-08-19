using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ProyectoAplicaciones
{
    // Clase barco la cual me permitirá manipular los barcos 
    class Barco
    {
        // Creo una varialbe posición del tipo punto la cual me permitirá
        // Obtener la posición en X y Y del barco
        Point posicion;

        // Refactorizamos la variable Posición
        public Point Posicion
        {
            get { return posicion; }
            set { posicion = value; }
        }

        // Definimos a puntosVida como entero lo cual me servirá para ver 
        // Si el barco sigue a flote o se hundió
        int puntosVida;

        // Refactorizamos los puntosVida
        public int PuntosVida
        {
            get { return puntosVida; }
            set { puntosVida = value; }
        }
        
        // Defino al ancho del barco como entero
        int ancho;

        // Refactorizamos el ancho del barco
        public int Ancho
        {
            get { return ancho; }
            set { ancho = value; }
        }

        // Defino al largo del barco como entero
        int largo;
        // Refactorizamos el largo del barco
        public int Largo
        {
            get { return largo; }
            set { largo = value; }
        }

        // Creo un arreglo de puntos este me servirá para la división del barco 
        // dependiendo del tamaño en picturebox más pequeños
        Point[] puntos;

        // Refactorizamos el arreglo de puntos
        public Point[] Puntos
        {
            get { return puntos; }
            set { puntos = value; }
        }
        List<Point> puntosGolpe;

        public List<Point> PuntosGolpe
        {
            get { return puntosGolpe; }
            set { puntosGolpe = value; }
        }

        // Constructor de la clase Barco
        public Barco(int vida)
        {
            // Pasamos el valor de los puntos de vida a la variable vida
            puntosVida = vida;
            puntosGolpe = new List<Point>();
        }

        // Función permite defininir el arreglo Puntos
        public void Actualizar()
        {
            // Creo el arreglo con los puntos de vida que me defina el barco
            puntos = new Point[puntosVida];
            
            // Lazo for para la división del barco (picturebox) en puntos
            for (int i = 0; i < puntosVida; i++)
            {
                // Si i es igual a 0
                if (i == 0)
                {
                    // Definimos el primer punto como la posición
                    puntos[0] = posicion;
                    
                }
                // Caso contrario si el ancho es diferente de 30
                else if (ancho != 30)
                {
                    // Creo un punto y le cargo al arreglo
                    puntos[i] = new Point(posicion.X + (i * 30), posicion.Y);
                   
                }
                // Caso contrario si el largo es diferente de 30
                else if (largo != 30)
                {
                    // Creo un punto y le cargo al arreglo
                    puntos[i] = new Point(posicion.X, posicion.Y + (i * 30));
                    
                }
            }
        }
    }
}
