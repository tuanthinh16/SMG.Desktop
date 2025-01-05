using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Plugins
{
    public interface IReport
    {
        bool GetData(string jsonFilter);
        bool ProcessData();
        bool ExportData(SMG.Models.Report report,string outputFile);
    }
}
