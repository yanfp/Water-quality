using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
//using DevExpress.Skins;


namespace water_quality
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.SkinProject1).Assembly);
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new login2());
            Application.Run(new mainform());
        }
    }
}
