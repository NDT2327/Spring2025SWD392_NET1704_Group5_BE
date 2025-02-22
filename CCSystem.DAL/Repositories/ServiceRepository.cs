using CCSystem.DAL.DBContext;
using CCSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.DAL.Repositories
{
    public class ServiceRepository
    {
        private readonly SP25_SWD392_CozyCareContext _context;

        public ServiceRepository(SP25_SWD392_CozyCareContext context)
        {
            _context = context;
        }

        public async Task CreateServiceAsync(Service service)
        {
            try
            {
                await this._context.Services.AddAsync(service);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
