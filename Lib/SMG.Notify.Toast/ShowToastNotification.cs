using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Alerter;

namespace SMG.Notify.Toast
{
    public class ToastNotifier
    {
        private static AlertControl alertControl;

        static ToastNotifier()
        {
            alertControl = new AlertControl
            {
                AutoFormDelay = 2000, // Close after 2 seconds
                
            };
            alertControl.AppearanceCaption.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            alertControl.AppearanceCaption.ForeColor = Color.Green;
            alertControl.AppearanceText.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            alertControl.AppearanceText.ForeColor = Color.Green;
            alertControl.FormLoad += AlertControl_AlertFormLoad;
        }

        public static void ShowInformation(string message)
        {
            ShowAlert("Information", message ?? "Operation Successful!");
        }

        public static void ShowSuccess(string message)
        {
            ShowAlert("Success", message ?? "Success!");
        }

        public static void ShowError(string message)
        {
            ShowAlert("Error", message ?? "An error occurred!");
        }

        private static void ShowAlert(string caption, string text)
        {
            AlertInfo info = new AlertInfo(caption, text);
            alertControl.Show(null, info);
        }

        private static void AlertControl_AlertFormLoad(object sender, AlertFormLoadEventArgs e)
        {
            // Center the alert on the screen
            e.AlertForm.Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - e.AlertForm.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - e.AlertForm.Height) / 2
            );

            // Set the text color to green
            
        }
    }
}
