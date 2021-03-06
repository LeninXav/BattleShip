﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Configuration;
using System.IO;
using System.Net;

namespace ProyectoAplicaciones
{
    public partial class Cliente : Form
    {
        public Cliente()
        {
            InitializeComponent();
        }

        // Creo un arreglo de barcos definijendo el nuemro de barcos en 5
        private Barco[] barcos = new Barco[5];
        private List<PictureBox> objetos;
        private TcpClient cliente;
        private delegate void Pinto(int x, int y, bool estado);
        private delegate void Desbloquea();
        private delegate void Bloqueo();
        private delegate void Vida();
        private delegate void Nuevo();
        bool turno;
        NetworkStream datos;

        private void Cliente_Load(object sender, EventArgs e)
        {
            /*Thread hiloCliente = new Thread(new ThreadStart(Comunicacion));
            hiloCliente.Start();*/
            // Esta función nos permite dibujar la grilla en el picturebox
            // Radar
            Dibujar(picRadar);
            // Esta función nos permite dibujar la grilla en el picturebox
            // Posición de los barcos
            Dibujar(picPosicion);
            // Lazo for para crear y colocar los barcos en el arreglo y para tomar
            // en cuenta los puntos de vida
            for (int i = 0; i < barcos.Length; i++)
            {
                // Creo los barcos con los puntos de vida
                barcos[i] = new Barco(i + 1);
            }

            turno = true;
            objetos = new List<PictureBox>();
        }

        // Función que me va a permitir dibujar la grilla en un picturebox
        private void Dibujar(PictureBox fondo)
        {
            // Cargo el picture box en la variable f del tipo picturebox
            PictureBox f = fondo;
            // Instancio una variable tipo bitmap que me va a servir para graficar
            Bitmap b;
            // Inicializo la variable b con las medidas del picturebox f
            b = new Bitmap(f.Width, f.Height);
            // Le cargo la propiedad image del picturebox f el bitmap b para esto uso casting
            f.Image = (Image)b;
            // Creo un objeto Graphics al cual para inicializar le pasamos el bitmap
            Graphics g = Graphics.FromImage(b);

            // Lazo for que me permitirá dibujar líneas tanto en las coordenadas X como en Y
            for (int i = 1; i < 10; i++)
            {
                // Esta función me permite dibujar las líneas desde la posición 0 hasta el otro extremo
                // del picture box
                g.DrawLine(Pens.Black, new Point((i * 30), 0), new Point((i * 30), 300));
                g.DrawLine(Pens.Black, new Point(0, (i * 30)), new Point(300, (i * 30)));
            }
            // Dibujo un rectángulo para el contorno del picturebox
            g.DrawRectangle(Pens.Black, 0, 0, f.Width - 1, f.Height - 1);
            // Libro los recursos utilizados para el Graphics
            g.Dispose();
        }

        private void picRadar_Click(object sender, EventArgs e)
        {
            if (turno == true)
            {
                // Obtenemos la posición en enteros del 1 al 10 en la coordenada X
                int x = picRadar.PointToClient(Cursor.Position).X / 30;
                // Obtenemos la posición en enteros del 1 al 10 en la coordenada Y
                int y = picRadar.PointToClient(Cursor.Position).Y / 30;
                // Función que me permitirá pintar el cuadradito donde sea el ataque
                Enviar("punto", x, y);
                turno = false;
            }
            else
                MessageBox.Show("No es tu turno", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        // Función Pintar Que me servirá para pintar los cuadrados donde se lleve a cabo el ataque
        private void Pintar(int x, int y, bool estado)
        {
            // Creo un nuevo picturebox llamado pic para crear los cuadrados que me determinarán el
            // ataque
            PictureBox pic = new PictureBox();
            // Creo un punto donde estará la posición del picturebox 
            Point punto = new Point(picRadar.Location.X + (x * 30), picRadar.Location.Y + (y * 30));
            // Pongo el punto con la propiedad location en el picturebox
            pic.Location = punto;
            
            // Definimos el ancho y largo de nuestro picturebox pequeño que será de 30*30 (pixeles)
            pic.Width = 30;
            pic.Height = 30;
            // Agregamos el picturebox a controls para que se pueda observar el picturebox pequeño (creado)
            // En el picture box grande
            this.Controls.Add(pic);
            //Agregamos a la lista de los picturebox creados, lo que nos servira para jugar de nuevo
            objetos.Add(pic);
            // Elijo el borde del tipo FixedSingle
            pic.BorderStyle = BorderStyle.FixedSingle;
            if (estado == true)
            {
                // Pinto de color rojo
                pic.BackColor = Color.GreenYellow;
                // Para que sea visible el pictrue box
                pic.Visible = true;
            }
            else
            {
                // Pinto de color rojo
                pic.BackColor = Color.Red;
                // Para que sea visible el pictrue box
                pic.Visible = true;
            }
            // Traemos hacia adelante el picture box para que no le solape el picturebox grande
            pic.BringToFront();
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            // Creo un randómico valor que me servirá para las ubicaciones aleatorias
            Random valor = new Random();
            // Llamamos a la función ubicación Ubicación que me servirá para colocar los barcos
            // Aleatoriamente los pictureBox de los barcos
            Ubicacion(valor);
            btnJugar.Enabled = true;
        }

        // Función ubicación que me servirá para la ubicación de los picturebox de los barcos
        private void Ubicacion(Random valor)
        {
            // Creo un entero posición y lo inicializo en  para definir la posición del barco dentro del 
            // arreglo anteriormente creado
            int posicion = 0;
            // Lazo for each para los barocs
            foreach (Barco obj in barcos)
            {
                // Lazo infinito para verificar que no se sobrelapen los barcos
                while (true)
                {
                    // Generamos un randómico en X que va de 0 a 10
                    int x = valor.Next(0, 10);
                    // Generamos un randómico en Y que va de 0 a 10
                    int y = valor.Next(0, 10);

                    // Con esta condición definimo que no se salga el tamaño en ancho del picturebox
                    if ((x + obj.PuntosVida) < 10)
                    {
                        // Condición para que no se sobrelapen los picturebox de los barcos
                        if (Verificar(posicion, x, y, obj.PuntosVida, 1))
                        {
                            // Creo un punto para la posición del picturebox dentro del formularios para 
                            // que se ubique correctaente el barco
                            Point punto = new Point((x * 30) + picPosicion.Location.X, (y * 30) +
                                picPosicion.Location.Y);
                            // Agrego en el barco la posición
                            obj.Posicion = punto;
                            // Definimos el ancho del barco y lo asociamos con los puntos de vida
                            obj.Ancho = obj.PuntosVida * 30;
                            // Definimos el largo del barco
                            obj.Largo = 30;
                            // LLamamos a la función actualizar de la clase Barco
                            obj.Actualizar();
                            // Llamamos a la función poner barco 
                            PonerBarco(x, y, obj.PuntosVida, 1, obj.PuntosVida);
                            // Rompo el lazo infinito con cualquier condición que cumpla
                            break;
                        }
                    }
                    // Caso contrario si no es en lo horizontal ahora condición para la colocación 
                    // vertical
                    else if ((y + obj.PuntosVida) < 10)
                    {
                        // // Condición para que no se sobrelapen los picturebox de los barcos
                        if (Verificar(posicion, x, y, 1, obj.PuntosVida))
                        {
                            // Creo un punto para la posición del picturebox dentro del formularios para 
                            // que se ubique correctaente el barco
                            Point punto = new Point((x * 30) + picPosicion.Location.X, (y * 30) + picPosicion.Location.Y);
                            // Agrego en el barco la posición
                            obj.Posicion = punto;
                            // Defino el ancho del barco
                            obj.Ancho = 30;
                            // Definimos el largo del barco con los puntos de vida
                            obj.Largo = obj.PuntosVida * 30;
                            // LLamamos a la función actualizar de la clase Barco
                            obj.Actualizar();
                            // Llamamos a la función poner barco 
                            PonerBarco(x, y, 1, obj.PuntosVida, obj.PuntosVida);
                            // Rompo el lazo infinito con cualquier condición que cumpla
                            break;
                        }
                    }
                }
                // Aumentamos la posición en 1 para verificar que no se sobrelapen los barcos
                posicion++;
            }
        }

        // Función PonerBarco para colocar el barco dentro del picturebox
        private void PonerBarco(int x, int y, int ancho, int largo, int tipo)
        {
            // Creamos el punto de posición del barco
            Point punto = new Point(picPosicion.Location.X + (x * 30), picPosicion.Location.Y + (y * 30));

            // Con el swtich definimos que tipo de barco vamos a colocar en el picturebox
            switch (tipo)
            {
                // Caso 1 Definen los datos (propiedades) del picture box esto es para todos los casos
                case 1:
                    picBarco1.Location = punto;
                    picBarco1.Width = ancho * 30;
                    picBarco1.Height = largo * 30;
                    picBarco1.BorderStyle = BorderStyle.FixedSingle;
                    picBarco1.Visible = true;
                    picBarco1.BackColor = Color.Blue;
                    break;
                case 2:
                    picBarco2.Location = punto;
                    picBarco2.Width = ancho * 30;
                    picBarco2.Height = largo * 30;
                    picBarco2.BorderStyle = BorderStyle.FixedSingle;
                    picBarco2.Visible = true;
                    picBarco2.BackColor = Color.Purple;
                    break;
                case 3:
                    picBarco3.Location = punto;
                    picBarco3.Width = ancho * 30;
                    picBarco3.Height = largo * 30;
                    picBarco3.BorderStyle = BorderStyle.FixedSingle;
                    picBarco3.Visible = true;
                    picBarco3.BackColor = Color.Green;
                    break;
                case 4:
                    picBarco4.Location = punto;
                    picBarco4.Width = ancho * 30;
                    picBarco4.Height = largo * 30;
                    picBarco4.BorderStyle = BorderStyle.FixedSingle;
                    picBarco4.Visible = true;
                    picBarco4.BackColor = Color.Yellow;
                    break;
                case 5:
                    picBarco5.Location = punto;
                    picBarco5.Width = ancho * 30;
                    picBarco5.Height = largo * 30;
                    picBarco5.BorderStyle = BorderStyle.FixedSingle;
                    picBarco5.Visible = true;
                    picBarco5.BackColor = Color.Brown;
                    break;
            }
        }

        // Función que nos permitirá veriicar que no se sobrelapen los barcos
        private bool Verificar(int posicion, int x, int y, int ancho, int largo)
        {
            // Si la posición del barco dentro del arreglo es 0 se colocará en el picturebox
            if (posicion == 0)
                return true;
            // Caso contrario 
            else
            {
                // Creo un lazo for para ir obteniendo la propiedad puntos de los barcos ya posicionados
                for (int i = 0; i < posicion; i++)
                {
                    // Creo un arreglo de puntos con la propiedad Puntos de los barcos
                    Point[] valor = barcos[i].Puntos;
                    // Verificamos si el ancho es diferente de 1 (el barco estará horizontalmente)
                    if (ancho != 1)
                    {
                        // Hacemos un for each del arreglo de puntos obtenido
                        foreach (Point punto in valor)
                        {
                            // Otro lazo for para hacer la comparación del ancho del barco y los puntos
                            // del barco anteriormente posicionado
                            for (int j = 0; j < ancho; j++)
                            {
                                // Hacemos la verificación si encontramos una coincidencia del punto retornará
                                // la función un false ya que se sobrelapan
                                if (punto == new Point(picPosicion.Location.X + (j * 30) + (x * 30),
                                    picPosicion.Location.Y + (y * 30)))
                                    return false;
                            }
                        }
                    }
                    // Verificamos si el largo es diferente de 1 (el barco estará verticalmente)
                    else if (largo != 1)
                    {
                        // Hacemos un for each del arreglo de puntos obtenido
                        foreach (Point punto in valor)
                        {
                            // Otro lazo for para hacer la comparación del largo del barco y los puntos
                            // del barco anteriormente posicionado
                            for (int j = 0; j < largo; j++)
                            {
                                // Hacemos la verificación si encontramos una coincidencia del punto retornará
                                // la función un false ya que se sobrelapan
                                if (punto == new Point(picPosicion.Location.X + (x * 30), picPosicion.Location.Y + (y * 30) + (j * 30)))
                                    return false;
                            }
                        }
                    }
                }
                // Retorna verdadero si no hay ninguna coincidencia
                return true;
            }
        }
        //Esta funcion nos permite recibir informacion desde el cliente
        public void Recibir()
        {
            //Obtenemos el numero de puerto del archivo de configuracion
            string puerto = ConfigurationManager.AppSettings["puerto"].ToString();
            //Conectamos el cliente
            cliente = new TcpClient(txtDirecionIP.Text, Convert.ToInt32(puerto));
            //Comprobamos que este conectado el usuario
            if (cliente.Connected)
            {
                MessageBox.Show("Cliente conectado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //Invocamos a la funcion bloquear
                Invoke(new Bloqueo(Bloquear));
            }
            string mensaje1 = "";
            //Este lazo se ejecuta mientras no llegue un mensaje de Terminar
            do
            {
                //creamos un flujo
                datos = cliente.GetStream();
                //Leemos los datos enviados de acuerdo al orden con lo que enviamos los datos
                BinaryReader datosRx = new BinaryReader(datos);
                mensaje1 = datosRx.ReadString();
                int x = datosRx.ReadInt32();
                int y = datosRx.ReadInt32();
                //Observams que tipo de mensaje recibimos
                switch (mensaje1)
                {
                    case "True":
                        //pintamos en radar de acuerdo a la respuesta del servidor
                        Invoke(new Pinto(Pintar), new object[] { x, y, true });
                        break;
                    case "False":
                        //pintamos en radar de acuerdo a la respuesta del servidor
                        Invoke(new Pinto(Pintar), new object[] { x, y, false });
                        break;
                    case "punto":
                        //Enviamos si nos llego un golpe o no
                        Enviar(Disparo(x, y).ToString(), x, y);
                        //verificamos la vida
                        Invoke(new Vida(VerificarVida));
                        //Cambiamos el estado de la variable turno
                        Invoke(new Desbloquea(Desbloquear));
                        break;
                    case "Ganar":
                        //Informamos que se gano la partida
                        MessageBox.Show("Ganaste", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        //Preguntamos si quiere seguir jugando
                        DialogResult resultado = MessageBox.Show("Quieres Jugar un Nuevo Juego", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (resultado == DialogResult.Yes)
                            //Si selecciona si Invoacamos una funcion para juego nuevo
                            Invoke(new Nuevo(NuevoJuego));
                        else
                        {
                            //Enviamos terminar al servidor
                            Enviar("Terminar", 0, 0);
                            MessageBox.Show("La aplicacion se cerrara", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;
                    default:
                        break;
                }
            } while (mensaje1 != "Terminar");
            //Cerramos el flujo
            datos.Close();
            //Cerramos el cliente
            cliente.Close();
            //Cerramos el formulario
            this.Close();
            
        }
        // Este evento permite concetar con el servidor
        private void btnConectar_Click(object sender, EventArgs e)
        {
            //habilitamos el boton generar
            btnGenerar.Enabled = true;
            //creamos un hilo, y pasamos la funcion recibir
            Thread hilo = new Thread(new ThreadStart(Recibir));
            //Iniciamos el hilo
            hilo.Start();
        }
        //Esta funcion permite comprobar si el disparo golpeo un barco
        public bool Disparo(int x, int y)
        {
            //creamos un tipo punto
            Point p = new Point(picPosicion.Location.X + (x * 30), picPosicion.Location.Y + (y * 30));
            //hacemos un foreach para buscar entre los barcos
            foreach (Barco barco in barcos)
            {
                //Cargamos la propiedad Puntos, para hacer la comprobacion
                Point[] valor = barco.Puntos;
                //hacemos un foreach dentro del arreglo anteriormente obtenido
                foreach (Point p2 in valor)
                {
                    //Comprobamos si los puntos son iguales
                    if (p2 == p)
                    {
                        //Esto comprobamos que no sea en el mismo punto el golpe
                        int i = 0;
                        foreach (Point punto in barco.PuntosGolpe)
                        {
                            if (punto == p)
                            {
                                i++;
                            }
                        }
                        //Si no se ha dado anteriormente un golpe se disminuye los puntos de vida
                        if (i == 0)
                        {
                            MessageBox.Show("Te Golpearon", "Peligro", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            barco.PuntosVida--;
                            barco.PuntosGolpe.Add(p);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        //Esta funcion nos permite enviar mensajes al servidor
        public void Enviar(string mensaje, int x, int y)
        {
            //Creamos un binaryWriter para enviar datos
            BinaryWriter datos = new BinaryWriter(new BufferedStream(cliente.GetStream()));
            //Enviamos los datos de acuerdo al siguiente orden
            datos.Write(mensaje);
            datos.Write(x);
            datos.Write(y);
            datos.Flush();
        }
        //Este evento se genera cuando damos click en Jugar
        private void btnJugar_Click(object sender, EventArgs e)
        {
            //Informamos que el juego va ha comenzar
            MessageBox.Show("Listo Para Comenzar La Batalla", "Pregunta", MessageBoxButtons.OK, MessageBoxIcon.Question);
            //Desabilitamos los botones
            btnGenerar.Enabled = false;
            btnJugar.Enabled = false;
            //Enviamos un mensaje al servidor con enviar
            Enviar("Jugar", 0, 0);
        }
        //Cambiamos el valor para desbloquear
        public void Desbloquear()
        {
            //cambiamos el valor de turno a true
            turno = true;
        }
        //Esta funcion nos permite bloquear los controles
        public void Bloquear()
        {
            btnConectar.Enabled = false;
        }
        //Esta funcion nos permite verificar los puntos de vida
        public void VerificarVida()
        {
            //vemos cuantos barcos tienen puntos de vida
            int i = 0;
            foreach (Barco obj in barcos)
                if (obj.PuntosVida != 0)
                    i++;
            //Si ya ninguno tiene puntos de vida
            if (i == 0)
            {
                //Enviamos ganar al servidor
                Enviar("Ganar", 0, 0);
                //Ponemos en un messagebox que perdimos
                MessageBox.Show("Perdiste", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                //cambiamos el turno a false
                turno = false;
                //Preguntamos si quiere seguir jugando
                DialogResult resultado = MessageBox.Show("Quieres Jugar un Nuevo Juego", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resultado == DialogResult.Yes)
                    //Si selecciona si llamamos a la funcion para juego nuevo
                    NuevoJuego();
                else
                {
                    //Enviamos terminar al servidor
                    Enviar("Terminar", 0, 0);
                    MessageBox.Show("La aplicacion se cerrara", "Informe", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void Cliente_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
        //esta funcion es cuando se desea jugar un juego nuevo
        public void NuevoJuego()
        {
            //Removemos los picturebox que se crearon
            foreach (PictureBox obj in objetos)
                this.Controls.Remove(obj);
            //creamos de nuevo los barcos
            for (int i = 0; i < barcos.Length; i++)
            {
                // Creo los barcos con los puntos de vida
                barcos[i] = new Barco(i + 1);
            }
            //Habilitamos el boton generar
            btnGenerar.Enabled = true;
            //Cambiamos el valor dd turno a true
            turno = true;
            //Enviamos el mensaje nuevo
            Enviar("Nuevo", 0, 0);
        }

        private void Cliente_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
        //Este evento se genera cuando damos click en el boton cerrar
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            //Comprobamos si esta conectado
            if (cliente != null)
            {
                //Enviamos un mensaje su estamos conectados
                Enviar("Terminar", 0, 0);
            }
            else
                //Cerramos el form
                this.Close();
        }
    }
}
