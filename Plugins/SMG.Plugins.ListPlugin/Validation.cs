using DevExpress.XtraEditors.DXErrorProvider;
using SMG.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMG.Plugins.ListPlugin
{
    public class Validation
    {
        public static bool ValidationRequiredControl(Control control,DXErrorProvider dxErrorProvider1)
        {
            bool result = true;
			try
			{
				if(control != null)
				{
                    if (string.IsNullOrEmpty(control.Text))
                    {
                        dxErrorProvider1.SetError(control, "Trường dữ liệu bắt buộc", DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                        result = false;
                    }
                    else { dxErrorProvider1.SetError(control, "", DevExpress.XtraEditors.DXErrorProvider.ErrorType.None); }
                }
			}
			catch (Exception ex)
			{

                LogSystem.Error(ex);
			}
            return result;
        }
    }
}
