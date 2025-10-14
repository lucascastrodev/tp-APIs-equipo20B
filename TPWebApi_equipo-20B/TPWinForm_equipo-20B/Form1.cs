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
    public partial class frmPrincipal : Form
    {
        private List<Articulo> listaarticulo;
        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            listaarticulo = negocio.listar();
            dgvArticulos.DataSource = listaarticulo;


        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAgregar altaProducto = new frmAgregar();
            altaProducto.ShowDialog();

            ArticuloNegocio negocio = new ArticuloNegocio();
            dgvArticulos.DataSource = null;              // limpiar la grilla
            dgvArticulos.DataSource = negocio.listar();  // actualizar con los nuevos datos
        }

        private void ocultarcolumna()
        {
            dgvArticulos.Columns["idArticulo"].Visible = false;
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo seleccionado;

            try
            {
                DialogResult respuesta = MessageBox.Show("¿Estas seguro que querés eliminar? No podrás volver atrás", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                    negocio.eliminar(seleccionado.idArticulo);
                    dgvArticulos.DataSource = negocio.listar();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {

            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
            frmAgregar altaProducto = new frmAgregar(seleccionado);
            altaProducto.ShowDialog();

            ArticuloNegocio negocio = new ArticuloNegocio();
            dgvArticulos.DataSource = null;              // limpiar la grilla
            dgvArticulos.DataSource = negocio.listar();  // actualizar con los nuevos datos

        }


        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
            FrmDetalle detalleproducto = new FrmDetalle(seleccionado);
            detalleproducto.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Articulo> listafiltrada;

            string filtro = txbBuscar.Text;

            if (filtro != "")
            {
                listafiltrada = listaarticulo.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Codigo.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            { listafiltrada = listaarticulo; }
            dgvArticulos.DataSource = null;
            dgvArticulos.DataSource = listafiltrada;
            ocultarcolumna();
        }


        private void txbBuscar_TextChanged(object sender, EventArgs e)
        {
            List<Articulo> listafiltrada;

            string filtro = txbBuscar.Text;

            if (filtro != "")
            {
                listafiltrada = listaarticulo.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Codigo.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            { listafiltrada = listaarticulo; }
            dgvArticulos.DataSource = null;
            dgvArticulos.DataSource = listafiltrada;
            ocultarcolumna();
        }

        private void btnGestionarImagenes_Click_1(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null && dgvArticulos.CurrentRow.Cells["idArticulo"].Value != null)
            {
                int idArticulo = (int)dgvArticulos.CurrentRow.Cells["idArticulo"].Value;
                string nombreArticulo = dgvArticulos.CurrentRow.Cells["nombre"].Value.ToString();

                frmGestionImagenes formImagenes = new frmGestionImagenes(idArticulo, nombreArticulo);
                formImagenes.ShowDialog();
            }
            else
            {
                MessageBox.Show("Seleccione un artículo primero");
            }

        }
        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null && dgvArticulos.CurrentRow.Cells["idArticulo"].Value != null)
            {
                int idArticulo = (int)dgvArticulos.CurrentRow.Cells["idArticulo"].Value;

                ArticuloNegocio articuloNegocio = new ArticuloNegocio();
                string urlImagen = articuloNegocio.ObtenerPrimeraImagenUrl(idArticulo);

                CargarImagenArticulo(urlImagen);
            }
            else
            {
                CargarImagenGenerica();
            }
        }
        private void CargarImagenArticulo(string urlImagen)
        {
            try
            {
                pbxarticulo.Load(urlImagen);
            }
            catch (Exception)
            {
                CargarImagenGenerica();
            }
        }

        private void CargarImagenGenerica()
        {
            try
            {
                pbxarticulo.Load("https://cdn4.iconfinder.com/data/icons/ui-beast-4/32/Ui-12-512.png");
            }
            catch
            {
                pbxarticulo.Image = null;
            }
        }

        private void btnGestionCategorias_Click(object sender, EventArgs e)
        {
            FrmCategorias Form3 = new FrmCategorias();
            Form3.ShowDialog();
        }

        private void btnGestionarMarcas_Click(object sender, EventArgs e)
        {
            FrmMarca formMarcas = new FrmMarca();
            formMarcas.ShowDialog();
        }
    }
}
