using dominio;
using negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPWinForm_equipo_20B
{
    public partial class FrmMarca : Form
    {
        private MarcaNegocio marcaNegocio;
        public FrmMarca()
        {
            InitializeComponent();
            marcaNegocio = new MarcaNegocio(); 
        }

        private void FrmMarcas_Load(object sender, EventArgs e)
        {
            CargarMarcas();
        }

        private void CargarMarcas()
        {
            try
            {
                List<Marca> lista = marcaNegocio.listar(); 
                dgvMarcas.DataSource = lista;
                dgvMarcas.Columns["IdMarca"].Visible = false;

                dgvMarcas.Columns["Descripcion"].HeaderText = "Descripción";
                dgvMarcas.Columns["Descripcion"].Width = 200;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar marcas: " + ex.Message);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string descripcion = txtDescripcion.Text.Trim();

            if (string.IsNullOrEmpty(descripcion))
            {
                MessageBox.Show("Ingrese una descripción");
                return;
            }

            try
            {
                Marca nueva = new Marca();
                nueva.Descripcion = descripcion;

                marcaNegocio.agregar(nueva);
                MessageBox.Show("Marca agregada correctamente");

                txtDescripcion.Clear();
                CargarMarcas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvMarcas.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una marca para eliminar");
                return;
            }

            try
            {
                int idMarca = (int)dgvMarcas.CurrentRow.Cells["IdMarca"].Value;
                string descripcion = dgvMarcas.CurrentRow.Cells["Descripcion"].Value.ToString();

                DialogResult respuesta = MessageBox.Show(
                    $"¿Está seguro de eliminar la marca: {descripcion}?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (respuesta == DialogResult.Yes)
                {
                    marcaNegocio.eliminar(idMarca);
                    MessageBox.Show("Marca eliminada correctamente");
                    CargarMarcas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
