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
        // GET: api/Articulo
        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            var negocioArt = new ArticuloNegocio();
            var lista = negocioArt.listar();
            return Ok(lista);
        }

        // GET: api/Articulo/{id}
        [HttpGet, Route("{id:int}")]
        public IHttpActionResult Get(int id)
        {
            var negocioArt = new ArticuloNegocio();
            var lista = negocioArt.listar();
            var art = lista.Find(x => x.idArticulo == id);
            if (art == null) return NotFound();
            return Ok(art);
        }

        // POST: api/Articulo
        [HttpPost, Route("")]
        public IHttpActionResult Post([FromBody] ArticuloCreateDto dto)
        {
            if (dto == null) return BadRequest("El cuerpo de la solicitud está vacío.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var marcaSrv = new MarcaNegocio();
                var catSrv = new CategoriaNegocio();
                var artSrv = new ArticuloNegocio();

                
                if (!marcaSrv.listar().Any(m => m.IdMarca == dto.IdMarca))
                    return Content(HttpStatusCode.BadRequest, "La marca especificada no existe.");
                if (!catSrv.listar().Any(c => c.IdCategoria == dto.IdCategoria))
                    return Content(HttpStatusCode.BadRequest, "La categoría especificada no existe.");

                
                var existentes = artSrv.listar();
                if (existentes.Any(a => string.Equals(a.Codigo, dto.Codigo?.Trim(), StringComparison.OrdinalIgnoreCase)))
                    return Content(HttpStatusCode.Conflict, "Ya existe un artículo con ese código.");

                var nuevo = new Articulo
                {
                    Codigo = dto.Codigo?.Trim(),
                    Nombre = dto.Nombre?.Trim(),
                    Descripcion = dto.Descripcion?.Trim(),
                    Marca = new Marca { IdMarca = dto.IdMarca },
                    Categoria = new Categoria { IdCategoria = dto.IdCategoria },
                    Precio = dto.Precio
                };

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
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un error al crear el artículo.",
                    detail = ex.Message
                });
            }
        }

        // POST: api/Articulo/{id}/imagenes
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

               
                bool UrlValida(string u) =>
                    Uri.TryCreate(u?.Trim(), UriKind.Absolute, out var uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

                var candidatas = dto.Imagenes
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .Select(u => u.Trim())
                    .Where(UrlValida)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (candidatas.Count == 0)
                    return BadRequest("Las URLs enviadas no son válidas.");

                
                var yaCargadas = (imgSrv.listarPorArticulo(id) ?? new List<Imagen>())
                    .Select(i => i.UrlImagen?.Trim() ?? string.Empty)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var nuevas = candidatas.Where(u => !yaCargadas.Contains(u)).ToList();
                var duplicadas = candidatas.Except(nuevas, StringComparer.OrdinalIgnoreCase).ToList();

                foreach (var url in nuevas)
                {
                    imgSrv.agregar(new Imagen
                    {
                        IdArticulo = id,
                        UrlImagen = url
                    });
                }

                if (nuevas.Count == 0)
                    return Content(HttpStatusCode.BadRequest, "No se agregaron imágenes porque ya existían o eran inválidas.");

                
                return Ok( new
                {
                    agregadas = nuevas.Count,
                    duplicadas = duplicadas.Count
                });
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

        // PUT: api/Articulo/{id}
        [HttpPut, Route("{id:int}")]
        public IHttpActionResult Put(int id, [FromBody] ArticuloUpdateDto dto)
        {
            if (dto == null) return BadRequest("El cuerpo de la solicitud está vacío.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var artSrv = new ArticuloNegocio();
                var existentes = artSrv.listar();
                var actual = existentes.Find(a => a.idArticulo == id);
                if (actual == null) return NotFound();

                var marcaSrv = new MarcaNegocio();
                var catSrv = new CategoriaNegocio();

                if (!marcaSrv.listar().Any(m => m.IdMarca == dto.IdMarca))
                    return BadRequest("La marca especificada no existe.");

                if (!catSrv.listar().Any(c => c.IdCategoria == dto.IdCategoria))
                    return BadRequest("La categoría especificada no existe.");

                
                if (existentes.Any(a =>
                    a.idArticulo != id &&
                    string.Equals(a.Codigo, dto.Codigo?.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    return Content(HttpStatusCode.Conflict, "Ya existe otro artículo con ese código.");
                }

                actual.Codigo = dto.Codigo?.Trim();
                actual.Nombre = dto.Nombre?.Trim();
                actual.Descripcion = dto.Descripcion?.Trim();
                actual.Marca = new Marca { IdMarca = dto.IdMarca };
                actual.Categoria = new Categoria { IdCategoria = dto.IdCategoria };
                actual.Precio = dto.Precio;

                artSrv.modificar(actual);

                return Ok(new
                {
                    message = "Artículo modificado correctamente.",
                    data = new
                    {
                        actual.idArticulo,
                        actual.Codigo,
                        actual.Nombre,
                        actual.Descripcion,
                        IdMarca = dto.IdMarca,
                        IdCategoria = dto.IdCategoria,
                        actual.Precio
                    }
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    message = "Ocurrió un error al modificar el artículo.",
                    detail = ex.Message
                });
            }
        }

        // DELETE: api/Articulo/{id}
        [HttpDelete, Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var negocio = new ArticuloNegocio();
                var lista = negocio.listar();
                var articulo = lista.Find(a => a.idArticulo == id);

                if (articulo == null)
                    return NotFound();

                negocio.eliminar(id);

                return Ok(new { message = "Artículo eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    message = "Error al eliminar el artículo.",
                    detail = ex.Message
                });
            }
        }
    }
}
