using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Plugins
{
    public interface IPlugin
    {

        void Load(List<object> listArgs);
    }
}
