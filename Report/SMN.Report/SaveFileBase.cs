using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMN.Report
{
    public class SaveFileBase
    {
        public SaveFileBase() { }
        public SaveFileBase(string dirName,string outputFile) 
        {
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string outputPath = Path.Combine(baseDirectory, "Data", dirName, outputFile);

            // Kiểm tra và tạo thư mục nếu chưa tồn tại
            string outputDirectory = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
        }
    }
}
