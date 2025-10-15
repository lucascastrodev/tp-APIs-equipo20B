using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using dominio;
using negocio;
using api_producto.Models;

namespace api_producto.Controllers
{
    [RoutePrefix("api/Articulo")]
    public class ArticuloController : ApiController
    {
        
        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            var negocioArt = new ArticuloNegocio();
            var lista = negocioArt.listar();
            return Ok(lista);
        }

        
        [HttpGet, Route("{id:int}")]
        public IHttpActionResult Get(int id)
        {
            var negocioArt = new ArticuloNegocio();
            var lista = negocioArt.listar();
            var art = lista.Find(x => x.idArticulo == id);
            if (art == null) return NotFound();
            return Ok(art);
        }

        
        [HttpPost, Route("")]
        public IHttpActionResult Post([FromBody] ArticuloCreateDto dto)
        {
            if (dto == null) return BadRequest("El cuerpo de la solicitud está vacío.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var marcaSrv = new MarcaNegocio();
                var catSrv = new CategoriaNegocio();

                var marcas = marcaSrv.listar();
                var cats = catSrv.listar();

                if (!marcas.Any(m => m.IdMarca == dto.IdMarca))
                    return BadRequest("La marca especificada no existe.");

                if (!cats.Any(c => c.IdCategoria == dto.IdCategoria))
                    return BadRequest("La categoría especificada no existe.");

                var nuevo = new Articulo
                {
                    Codigo = dto.Codigo?.Trim(),
                    Nombre = dto.Nombre?.Trim(),
                    Descripcion = dto.Descripcion?.Trim(),
                    Marca = new Marca { IdMarca = dto.IdMarca },
                    Categoria = new Categoria { IdCategoria = dto.IdCategoria },
                    Precio = dto.Precio
                };

                var artSrv = new ArticuloNegocio();
                artSrv.agregar(nuevo);

                return Content(HttpStatusCode.Created, new
                {
                    message = "Artículo creado correctamente.",
                    data = new
                    {
                        nuevo.Codigo,
                        nuevo.Nombre,
                        nuevo.Descripcion,
                        IdMarca = dto.IdMarca,
                        IdCategoria = dto.IdCategoria,
                        nuevo.Precio
                    }
                });
            }
            catch (HttpResponseException) { throw; }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un error al crear el artículo.",
                    detail = ex.Message
                });
            }
        }


        [HttpPost, Route("{id:int}/imagenes")]
        public IHttpActionResult AgregarImagenes(int id, [FromBody] ArticuloAddImagesDto dto)
        {
            if (dto == null || dto.Imagenes == null || dto.Imagenes.Count == 0)
                return BadRequest("Debe enviar al menos una imagen.");

            var artSrv = new ArticuloNegocio();
            var existe = artSrv.listar().Any(a => a.idArticulo == id);
            if (!existe) return NotFound();

            try
            {
                var imgSrv = new ImagenNegocio();

                foreach (var url in dto.Imagenes)
                {
                    imgSrv.agregar(new Imagen
                    {
                        IdArticulo = id,
                        UrlImagen = url
                    });
                }

                return StatusCode(HttpStatusCode.NoContent); // 204
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un error al agregar las imágenes.",
                    detail = ex.Message
                });
            }
        }



        [HttpPut, Route("{id:int}")]
        public void Put(int id, [FromBody] string value) { }

        [HttpDelete, Route("{id:int}")]
        public void Delete(int id) { }
    }
}
