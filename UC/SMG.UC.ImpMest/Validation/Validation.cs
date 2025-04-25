using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMG.UC.ImpMest.Validation
{
    public class Validation
    {
        public static bool ValiadtionRequiredControrl(Control control, DXErrorProvider dXErrorProvider) 
        {
            if(control is DevExpress.XtraEditors.TextEdit)
            {
                DevExpress.XtraEditors.TextEdit textEdit = control as DevExpress.XtraEditors.TextEdit;
                if (string.IsNullOrEmpty(textEdit.Text))
                {
                    dXErrorProvider.SetError(textEdit, "This field is required",ErrorType.Warning);
                    return false;
                }
                else
                {
                    dXErrorProvider.SetError(textEdit, "",ErrorType.None);
                    return true;
                }
            }
            return false;
        }
    }
}
