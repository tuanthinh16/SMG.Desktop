using SMG.Logging;
using SMG.Models;
using SMG.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Plugins.ReportTemplate
{
    internal class IReportTemplate : IPlugin
    {

        public void Load(List<object> listArgs)
        {
            try
            {
                ModuleData moduleData = null;
                for (int i = 0; i < listArgs.Count; i++)
                {
                    if (listArgs[i] is ModuleData)
                    {
                        moduleData = (ModuleData)listArgs[i];
                    }
                }
                frmListReportTemplate frm = new frmListReportTemplate();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

    }
}
