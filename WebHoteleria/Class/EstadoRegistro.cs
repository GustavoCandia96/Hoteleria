using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoteleria.Class
{
    public class EstadoRegistro
    {


        #region Propiedades

        public string Id { get; set; }
        public string Descripcion { get; set; }

        #endregion

        #region Metodos

        /*
         * METODO QUE DEVUELVE UN LISTADO DE ESTADO DE REGISTROS
         * SE UTILIZAN EN TODOS LOS MODULOS PARA HABILITAR O INHABILITAR EL REGISTRO
         */
        public List<EstadoRegistro> ObtenerListadoEstadosRegistros()
        {
            List<EstadoRegistro> retorno = new List<EstadoRegistro>
            {
                new EstadoRegistro{Id = "A", Descripcion = "Activo"},
                new EstadoRegistro{Id = "I", Descripcion = "Inactivo"},
            };
            return retorno;
        }

        #endregion

    }
}