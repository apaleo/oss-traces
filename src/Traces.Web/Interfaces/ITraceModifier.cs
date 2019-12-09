using System.Threading.Tasks;

namespace Traces.Web.Interfaces
{
    internal interface ITraceModifier
    {
        Task<bool> CreateOrEditTraceAsync();
    }
}