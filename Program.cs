using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScreensaverTests
{
    internal class Program
    {

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        public static Rectangle? PreviewRect;
        static void Main(string[] args)
        {

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

            foreach (string str in args)
            {

                Console.WriteLine(str);

            }

            if (args.Length > 0)
            {

                string arg1 = args[0].ToLower();
                if (arg1 == "/c") // config
                {

                    Console.WriteLine("not supported");

                }
                if (arg1 == "/p") // preview
                {

                    if (args.Length > 1)
                    {

                        string arg2 = args[1];

                        IntPtr windowPointer = new IntPtr(long.Parse(arg2));
                        
                        Screensaver.PreviewHandle = windowPointer;
                        Screensaver.IsInPreview = true;

                    }

                    Screensaver.InitAndRun();

                }
                if (arg1 == "/s") // fullscreen
                {

                    Screensaver.InitAndRun();

                }

            } else
            {

                Screensaver.InitAndRun();

            }

        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {

            Screensaver.Destroy();

        }

    }
}
