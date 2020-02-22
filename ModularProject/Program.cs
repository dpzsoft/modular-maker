using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModularProject {
    static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 f;

            if (args.Length > 0) {
                string path = args[0].Replace("\"", "");
                //MessageBox.Show(path);
                f = new Form1(path);
            } else {
                f = new Form1();
            }

            Application.Run(f);

            //Application.Run(new Form1());
        }
    }
}
