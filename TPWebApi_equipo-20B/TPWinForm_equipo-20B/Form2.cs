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
   public partial class frmGestionImagenes : Form
    {
        private int idArticulo;
        private string nombreArticulo;
        private ImagenNegocio imagenNegocio;
        private List<Imagen> listaImagenes;

        public frmGestionImagenes(int idArticulo, string nombreArticulo)
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.None;
            this.idArticulo = idArticulo;
            this.nombreArticulo = nombreArticulo;
            this.imagenNegocio = new ImagenNegocio();
            this.Text = "Gestión de Imágenes - " + nombreArticulo;
        }

        private void frmGestionImagenes_Load(object sender, EventArgs e)
        {
            CargarImagenes();
            pbxImagen.SizeMode = PictureBoxSizeMode.Zoom;

            // ← Versión simple
            if (listaImagenes != null && listaImagenes.Count > 0)
            {
                CargarImagenEnPictureBox(listaImagenes[0].UrlImagen);
            }
            else
            {
                MostrarImagenGenerica();
            }
        }

        private void CargarImagenes()
        {
            try
            {
                listaImagenes = imagenNegocio.listarPorArticulo(idArticulo);
                dgvImagenes.DataSource = listaImagenes;

                // Configurar columnas
                dgvImagenes.Columns["IdImagen"].Visible = false;
                dgvImagenes.Columns["IdArticulo"].Visible = false;
                dgvImagenes.Columns["UrlImagen"].HeaderText = "URL de la Imagen";
                dgvImagenes.Columns["UrlImagen"].Width = 300;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar imágenes: " + ex.Message);
            }
        }

        private void dgvImagenes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvImagenes.CurrentRow != null && dgvImagenes.CurrentRow.Cells["UrlImagen"].Value != null)
            {
                string urlImagen = dgvImagenes.CurrentRow.Cells["UrlImagen"].Value.ToString();
                CargarImagenEnPictureBox(urlImagen);
            }
            else
            {
                MostrarImagenGenerica();
            }
        }

        private void CargarImagenEnPictureBox(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    pbxImagen.Load(url);
                }
                else
                {
                    MostrarImagenGenerica();
                }
            }
            catch (Exception)
            {
                MostrarImagenGenerica();
            }
        }

        private void MostrarImagenGenerica()
        {
            try
            {
             
                string urlGenerica = "https://cdn4.iconfinder.com/data/icons/ui-beast-4/32/Ui-12-512.png";
                pbxImagen.Load(urlGenerica);
            }
            catch (Exception)
            {
               
                pbxImagen.Image = null;
            }
        }



        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string url = txtUrlImagen.Text.Trim();

            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Ingrese una URL válida");
                return;
            }

            try
            {
                Imagen nuevaImagen = new Imagen
                {
                    IdArticulo = idArticulo,
                    UrlImagen = url
                };

                imagenNegocio.agregar(nuevaImagen);
                MessageBox.Show("Imagen agregada correctamente");
                txtUrlImagen.Clear();
                CargarImagenes(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar imagen: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvImagenes.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una imagen para eliminar");
                return;
            }

            try
            {
                int idImagen = (int)dgvImagenes.CurrentRow.Cells["IdImagen"].Value;

                DialogResult respuesta = MessageBox.Show(
                    "¿Está seguro de eliminar esta imagen?",
                    "Eliminar Imagen",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (respuesta == DialogResult.Yes)
                {
                    imagenNegocio.eliminar(idImagen);
                    MessageBox.Show("Imagen eliminada correctamente");
                    CargarImagenes(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar imagen: " + ex.Message);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
