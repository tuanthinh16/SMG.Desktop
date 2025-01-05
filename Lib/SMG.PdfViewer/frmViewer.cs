using FlexCel.Render;
using FlexCel.XlsAdapter;
using SMG.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;

namespace SMG.PdfViewer
{
    public partial class frmViewer : Form
    {
        string filePath = string.Empty;
        public frmViewer(string filePath)
        {
            this.filePath = filePath;
            InitializeComponent();
        }
        

        private void frmViewer_Load(object sender, EventArgs e)
        {
            try
            {
                if(!string.IsNullOrEmpty(this.filePath))
                {
                    
                    MemoryStream pdfStream = ConvertExcelToPdf(this.filePath);

                    // Hiển thị PDF từ MemoryStream trên DevExpress PDF Viewer
                    pdfViewer1.LoadDocument(pdfStream);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        public MemoryStream ConvertExcelToPdf(string excelFilePath)
        {
            MemoryStream pdfStream = new MemoryStream();
            try
            {
                XlsFile xls = new XlsFile(excelFilePath);
                

                using (FlexCelPdfExport pdfExport = new FlexCelPdfExport(xls))
                {
                    pdfExport.BeginExport(pdfStream);
                    pdfExport.ExportAllVisibleSheets(false, "Sheet");
                    pdfExport.EndExport();
                }

                pdfStream.Position = 0; 
                
            }
            catch (Exception ex)
            {
                
                LogSystem.Error(ex);
                return null;
            }
            return pdfStream;

        }

    }
}
