using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.IServices
{
    public interface IJsService
    {
        Task Alert(string message);
    }
}
