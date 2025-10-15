using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;

namespace negocio
{
    public class ArticuloNegocio
    {
        public List<Articulo> listar()
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            ImagenNegocio imagenNegocio = new ImagenNegocio();

            try
            {
                datos.setearConsulta("SELECT Id, Codigo, Nombre, Descripcion, IdCategoria, IdMarca, Precio FROM Articulos");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.idArticulo = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Categoria = new Categoria();
                    aux.Categoria.IdCategoria = (int)datos.Lector["IdCategoria"];
                    aux.Marca = new Marca();
                    aux.Marca.IdMarca = (int)datos.Lector["IdMarca"];
                    aux.Precio = (decimal)datos.Lector["Precio"];

                    aux.Imagenes = imagenNegocio.listarPorArticulo(aux.idArticulo);

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public void agregar(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(
                    "INSERT INTO ARTICULOS (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, Precio) " +
                    "VALUES (@Codigo, @Nombre, @Descripcion, @IdMarca, @IdCategoria, @Precio)"
                );

                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@IdMarca", nuevo.Marca.IdMarca);
                datos.setearParametro("@IdCategoria", nuevo.Categoria.IdCategoria);
                datos.setearParametro("@Precio", nuevo.Precio);

                datos.ejecutarAccion(); // ← Solo ejecutar acción, no necesitas el ID
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public void eliminar(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("delete from ARTICULOS where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public void modificar(Articulo modificar)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("update ARTICULOS set Codigo = @Codigo, Nombre = @Nombre, Descripcion = @Descripcion , IdMarca = @IdMarca, IdCategoria = @Idcat, Precio = @Precio Where Id = @id");
                datos.setearParametro("@Codigo", modificar.Codigo);
                datos.setearParametro("@Nombre", modificar.Nombre);
                datos.setearParametro("@Descripcion", modificar.Descripcion);
                datos.setearParametro("@IdMarca", modificar.Marca.IdMarca);
                datos.setearParametro("@Idcat", modificar.Categoria.IdCategoria);
                datos.setearParametro("@Precio", modificar.Precio);
                datos.setearParametro("@Id", modificar.idArticulo);

                datos.ejecutarAccion();

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }


        public string ObtenerPrimeraImagenUrl(int idArticulo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT TOP 1 ImagenUrl FROM IMAGENES WHERE IdArticulo = @id ORDER BY Id");
                datos.setearParametro("@id", idArticulo);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        return (string)datos.Lector["ImagenUrl"];
                }

                return "https://cdn4.iconfinder.com/data/icons/ui-beast-4/32/Ui-12-512.png"; // Imagen genérica
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }
    }
}
