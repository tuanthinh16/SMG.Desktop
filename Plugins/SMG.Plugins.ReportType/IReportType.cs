using SMG.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Plugins.ReportType
{
    public class IReportType : IPlugin
    {
        public void Load(List<object> listArgs)
        {
			try
			{
                frmListReportType frm = new frmListReportType();
                frm.ShowDialog();
            }
			catch (Exception ex)
			{
				LogSystem.Error(ex);
            }
        }
    }
}
