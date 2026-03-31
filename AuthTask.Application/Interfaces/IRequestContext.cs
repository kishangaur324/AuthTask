using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTask.Application.Interfaces
{
    public interface IRequestContext
    {
        string? RequestId { get; }
    }
}
