﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parking
{
    public partial class fParking : Form
    {
        clsBD oBaseDatos = new clsBD("..\\..\\..\\bdparking.mdb");
        clsVehiculo oVehiculo = new clsVehiculo();

        DataSet dsMarcas;
        DataSet dsModelos;
        DataSet dsVehiculos;

        bool aparcado = false;


        // -------- para estado botones---------------
        string[] estados = new string[] { "inicio", "entrada", "saida" };
        Control[] controles;
        bool[,] comportamientoControles = new bool[,]
            {
                {true,true,false,false,false,false,false},
                {false,false,false,true,false,true,true},
                {false,false,true,false,true,true,true}
            };


        // -------- para ticket ----------------------
        int posicionVertical = 0;
        double TotalTicket = 1.5;

        Font fuenteG = new Font("Courier New", 12);
        Font fuenteN = new Font("Courier New", 10);
        Font fuenteP = new Font("Courier New", 8);
        Font fuenteMP = new Font("Courier New", 6);
        Font fuenteMParial = new Font("Arial", 6);
        // --------------------------------------------

        public fParking()
        {
            InitializeComponent();
            oBaseDatos.AbrirBD();
            controles = new Control[] { btEntrada, btSaida, gbxMatriculas, gbxDatos, btImprimir, btCancelar, btGardar };
			//Cargamos los dataset, ya que estos dos no cambian
            dsVehiculos = oVehiculo.leerCochesParking(oBaseDatos);
            dsMarcas = oVehiculo.leerMarcas(oBaseDatos);
        }

        private void estadoBotones(string estado)
        {
            int fila = Array.IndexOf(estados, estado);
            for (int i = 0; i < controles.GetLength(0); i++)
            {
                controles[i].Enabled = comportamientoControles[fila, i];
            }

        }

        private void limpiarCampos()
        {
            cbxVehiculosParking.Text = "";
            cbxMarca.Text = "";
            cbxModelo.Text = "";
            txbMatricula.Text = "";
        }

        private void fParking_Load(object sender, EventArgs e)
        {
            estadoBotones("inicio");

			//Cargamos los combos de matriculas y marcas ya que no cambian
            cbxVehiculosParking.Items.Clear();

            for (int i = 0; i < dsVehiculos.Tables[0].Rows.Count; i++)
            {
                cbxVehiculosParking.Items.Add(dsVehiculos.Tables[0].Rows[i][1]);
            }

            cbxMarca.Items.Clear();

            for (int i = 0; i < dsMarcas.Tables[0].Rows.Count; i++)
            {
                cbxMarca.Items.Add(dsMarcas.Tables[0].Rows[i][1]);
            }
        }

        // imprimir ticket
        private void btImprimir_Click(object sender, EventArgs e)
        {
            vistaPrevia.Document = documentoImprimir;
            vistaPrevia.ShowDialog();
        }

        // informe ticket

        private void contidoPaxina(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            string cadenaGuiones = "_".PadRight(50, '_');

            //cabeceira de ticket
            e.Graphics.DrawString("         P A R K I N G", fuenteN, Brushes.Black, 10, 10, new StringFormat());
            e.Graphics.DrawString("          -----------", fuenteN, Brushes.Black, 10, 20, new StringFormat());
            e.Graphics.DrawString("         piringalla s.l.", fuenteN, Brushes.Black, 10, 45, new StringFormat());
            e.Graphics.DrawString("         nif A270000000", fuenteN, Brushes.Black, 10, 60, new StringFormat());
            e.Graphics.DrawString("       Angelo Colocci s/n", fuenteN, Brushes.Black, 10, 75, new StringFormat());
            e.Graphics.DrawString("           27001 LUGO", fuenteN, Brushes.Black, 10, 90, new StringFormat());


            e.Graphics.DrawString(Convert.ToString(DateTime.Now), fuenteP, Brushes.Black, 10, 115, new StringFormat());

            e.Graphics.DrawString(cadenaGuiones, fuenteMP, Brushes.Black, 10, 140, new StringFormat());
            posicionVertical = 160;


            // detalle
            for (int i = 0; i < dsMarcas.Tables[0].Rows.Count; i++)
            {
                if (oVehiculo.idMarca == Convert.ToInt32(dsMarcas.Tables[0].Rows[i][0]))
                {
                    oVehiculo.marca = Convert.ToString(dsMarcas.Tables[0].Rows[i][1]);
                }
            }

            dsModelos = oVehiculo.leerModelos(oBaseDatos);
            for (int i = 0; i < dsModelos.Tables[0].Rows.Count; i++)
            {
                if (oVehiculo.idModelo == Convert.ToInt32(dsModelos.Tables[0].Rows[i][0]))
                {
                    oVehiculo.modelo = Convert.ToString(dsModelos.Tables[0].Rows[i][1]);
                    break;
                }
            }


            e.Graphics.DrawString(oVehiculo.matricula, fuenteN, Brushes.Black, 20, posicionVertical, new StringFormat());
            e.Graphics.DrawString(oVehiculo.marca, fuenteN, Brushes.Black, 20, posicionVertical + 20, new StringFormat());
            e.Graphics.DrawString(oVehiculo.modelo, fuenteN, Brushes.Black, 20, posicionVertical + 40, new StringFormat());

            // pe de ticket

            posicionVertical = 220;

            e.Graphics.DrawString(cadenaGuiones, fuenteMP, Brushes.Black, 10, posicionVertical, new StringFormat());
            posicionVertical += 15;
            e.Graphics.DrawString("total...", fuenteG, Brushes.Black, 90, posicionVertical, new StringFormat());
            e.Graphics.DrawString(string.Format("{0:###,##0.00}", TotalTicket), fuenteG, Brushes.Black, 180, posicionVertical, new StringFormat());
            posicionVertical += 40;
            e.Graphics.DrawString("   gracias pola súa visita", fuenteN, Brushes.Black, 20, posicionVertical, new StringFormat());
        }

        private void btSaida_Click(object sender, EventArgs e)
        {
            estadoBotones("saida");
            aparcado = true;
        }

        private void cbxVehiculosParking_SelectedIndexChanged(object sender, EventArgs e)
        {
            oVehiculo.valorarPropiedades(dsVehiculos, cbxVehiculosParking.SelectedIndex);

            txbMatricula.Text = oVehiculo.matricula;

            for (int i = 0; i < dsMarcas.Tables[0].Rows.Count; i++)
            {
                if(oVehiculo.idMarca == Convert.ToInt32(dsMarcas.Tables[0].Rows[i][0]))
                {
                    cbxMarca.SelectedItem = dsMarcas.Tables[0].Rows[i][1];
                    break;
                }
            }

            dsModelos = oVehiculo.leerModelos(oBaseDatos);

            cbxModelo.Items.Clear();

            for (int i = 0; i < dsModelos.Tables[0].Rows.Count; i++)
            {
                cbxModelo.Items.Add(Convert.ToString(dsModelos.Tables[0].Rows[i][1]));

                if (Convert.ToInt32(dsModelos.Tables[0].Rows[i][0]) == oVehiculo.idModelo)
                {
                    cbxModelo.SelectedItem = dsModelos.Tables[0].Rows[i][1];
                    break;
                }
            }
        }

        private void btCancelar_Click(object sender, EventArgs e)
        {
            limpiarCampos();
            estadoBotones("inicio");
        }

        private void btGardar_Click(object sender, EventArgs e)
        {
            if(aparcado)
            {
                oVehiculo.eliminarVehiculoParking(oBaseDatos);
            }
            else
            {
                for (int i = 0; i < dsMarcas.Tables[0].Rows.Count; i++)
                {
                    if (cbxMarca.SelectedItem == dsMarcas.Tables[0].Rows[i][1])
                    {
                        oVehiculo.idMarca = Convert.ToInt32(dsMarcas.Tables[0].Rows[i][0]);
                        break;
                    }
                }
                Console.Write(oVehiculo.idMarca);
                dsModelos = oVehiculo.leerModelos(oBaseDatos);
                for (int i = 0; i < dsModelos.Tables[0].Rows.Count; i++)
                {
                    if (cbxModelo.SelectedItem.Equals(dsModelos.Tables[0].Rows[i][1]))
                    {
                        oVehiculo.idModelo = Convert.ToInt32(dsModelos.Tables[0].Rows[i][0]);
                        break;
                    }
                }

                oVehiculo.matricula = txbMatricula.Text;

                oVehiculo.gardarVehiculoParking(oBaseDatos);
            }

            limpiarCampos();
        }

        private void btEntrada_Click(object sender, EventArgs e)
        {
            estadoBotones("entrada");
            aparcado = false;
        }

        private void cbxMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dsMarcas.Tables[0].Rows.Count; i++)
            {
                if (cbxMarca.SelectedItem.Equals(dsMarcas.Tables[0].Rows[i][1]))
                {
                    oVehiculo.idMarca = Convert.ToInt32(dsMarcas.Tables[0].Rows[i][0]);
                    break;
                }
            }

            dsModelos = oVehiculo.leerModelos(oBaseDatos);

            cbxModelo.Items.Clear();

            for (int i = 0; i < dsModelos.Tables[0].Rows.Count; i++)
            {
                cbxModelo.Items.Add(Convert.ToString(dsModelos.Tables[0].Rows[i][1]));

                if (Convert.ToInt32(dsModelos.Tables[0].Rows[i][0]) == oVehiculo.idModelo)
                {
                    cbxModelo.SelectedItem = dsModelos.Tables[0].Rows[i][1];
                    break;
                }
            }
        }
    }
}
