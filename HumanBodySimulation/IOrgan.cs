using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanBodySimulation
{
    internal interface IOrgan
    {
        void init(Dictionary<string, string> parameters);
        void update(int n, Dictionary<string, string> parameters);
    }
}
