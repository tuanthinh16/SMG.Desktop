using SMG.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Plugins.Login
{
    internal class ILogin : IPlugin
    {
        public void Load(List<object> listArgs)
        {
            try
            {
                frmLogin frm = new frmLogin();
                frm.ShowDialog();
                
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
