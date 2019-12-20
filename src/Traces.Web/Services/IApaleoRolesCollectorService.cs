using System.Collections.Generic;
using System.Threading.Tasks;

namespace Traces.Web.Services
{
    public interface IApaleoRolesCollectorService
    {
        Task<IReadOnlyList<string>> GetRolesAsync();
    }
}