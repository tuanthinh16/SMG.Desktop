using SMG.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Plugins.ListPlugin
{
    public class IListPlugin : IPlugin
    {
        public void Load(List<object> listArgs)
        {
			try
			{
                frmListPlugin frm = new frmListPlugin();
                frm.ShowDialog();
            }
			catch (Exception ex)
			{
				LogSystem.Error(ex);
            }
        }
    }
}
