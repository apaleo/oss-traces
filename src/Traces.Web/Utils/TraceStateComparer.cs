using System.Collections.Generic;
using Traces.Common.Enums;
using Traces.Web.Models;

namespace Traces.Web.Utils
{
    public class TraceStateComparer : IComparer<TraceItemModel>
    {
        public int Compare(TraceItemModel x, TraceItemModel y)
        {
            if (x.State.Equals(y.State))
            {
                return 0;
            }

            if (x.State == TraceState.Active)
            {
                return -1;
            }

            return 1;
        }
    }
}
