using System.Collections.Generic;
using System.Threading.Tasks;
using Traces.Web.Models;

namespace Traces.Web.Services
{
    public interface ITracesCollector
    {
        Task<ResultModel<IReadOnlyList<TraceItemModel>>> GetTracesAsync();
    }
}