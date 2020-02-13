using System.Collections.Generic;
using System.Threading.Tasks;

namespace Traces.Web.Services.Apaleo
{
    public interface IApaleoRolesCollectorService
    {
        Task<IReadOnlyList<string>> GetRolesAsync();
    }
}