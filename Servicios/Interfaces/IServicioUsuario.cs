using AlkilaApp.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlkilaApp.Servicios.Interfaces
{
   public interface IServicioUsuario
    {
         Task<bool> AnyadirOActualizarUsuario(Usuario usaurio);
        //    Task<bool> DeleteEmployee(string key);
        //    Task<List<EmployeeModel>> GetAllEmployee();

        
        Task RegistroUsuariosAsync(Usuario usaurio);

        Task ValidarUsuariosAsync(string correo, string contrasenya);


    }
}
