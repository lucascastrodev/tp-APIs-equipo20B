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
    public partial class FrmCategorias : Form
    {
        private CategoriaNegocio categoriaNegocio;
        public FrmCategorias()
        {
            InitializeComponent();
            categoriaNegocio = new CategoriaNegocio();
        }
        private void frmCategoria_Load(object sender, EventArgs e)
        {
            CargarCategorias();
        }

        private void CargarCategorias()
        {
            try
            {
                List<Categoria> lista = categoriaNegocio.listar();
                dgvCategorias.DataSource = lista;
                dgvCategorias.Columns["IdCategoria"].Visible = false;

                dgvCategorias.Columns["Descripcion"].HeaderText = "Descripción";
                dgvCategorias.Columns["Descripcion"].Width = 200;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar categorías: " + ex.Message);
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
                Categoria nueva = new Categoria();
                nueva.Descripcion = descripcion;

                categoriaNegocio.agregar(nueva);
                MessageBox.Show("Categoría agregada correctamente");

                txtDescripcion.Clear();
                CargarCategorias(); // Recargar la lista
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
            }
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una categoría para eliminar");
                return;
            }

            try
            {
                int idCategoria = (int)dgvCategorias.CurrentRow.Cells["IdCategoria"].Value;
                string descripcion = dgvCategorias.CurrentRow.Cells["Descripcion"].Value.ToString();

                DialogResult respuesta = MessageBox.Show(
                    $"¿Está seguro de eliminar la categoría: {descripcion}?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (respuesta == DialogResult.Yes)
                {
                    categoriaNegocio.eliminar(idCategoria);
                    MessageBox.Show("Categoría eliminada correctamente");
                    CargarCategorias(); // Recargar la lista
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
