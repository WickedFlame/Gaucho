using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gaucho.Dashboard
{
    public interface IDashboardDispatcher
    {
        Task Dispatch(DashboardContext context);
    }
}
