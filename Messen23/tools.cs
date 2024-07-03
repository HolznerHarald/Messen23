using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Messen23
{
    internal class tools
    {
        public static int KalAktiv = 1;
        public static int MessAktiv = 2;
        public static void HSetPixel(Bitmap himgBm, Point PP, Color color, int PunktKreuzGroesse)
        {
            if (0 <= PP.X && PP.X < himgBm.Width && 0 <= PP.Y && PP.Y < himgBm.Height)
            {
                for (int ii = 1; ii <= PunktKreuzGroesse; ii++)
                {
                    if ((PP.X + ii < himgBm.Width) && (PP.Y + ii < himgBm.Height))
                    {
                        himgBm.SetPixel(PP.X + ii, PP.Y + ii, color);
                    }
                    if ((PP.X - ii >= 0) && (PP.Y - ii >= 0))
                    {
                        himgBm.SetPixel(PP.X - ii, PP.Y - ii, color);
                    }
                    if ((PP.X + ii < himgBm.Width) && (PP.Y - ii >= 0))
                    {
                        himgBm.SetPixel(PP.X + ii, PP.Y - ii, color);
                    }
                    if ((PP.X - ii >= 0) && (PP.Y + ii < himgBm.Height))
                    {
                        himgBm.SetPixel(PP.X - ii, PP.Y + ii, color);
                    }
                }
            }
        }
        public static BitmapImage FileLaden(string fname)
        {
            BitmapImage bitimg;
            bitimg = new BitmapImage();
            Uri source = new Uri(fname);
            bitimg.BeginInit();
            bitimg.UriSource = source;
            bitimg.CacheOption = BitmapCacheOption.OnLoad;
            bitimg.EndInit();

            return bitimg;
        }

        public static double Abstand(Point p1, Point p2)
        {
            double abstand = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            return abstand;
        }
        public static double dAbstand(System.Windows.Point p1, System.Windows.Point p2)
        {
            double abstand = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            return abstand;
        }

        internal static System.Windows.Point PMinus(System.Windows.Point s1P1, System.Windows.Point s1P2)
        {
            System.Windows.Point hP = new System.Windows.Point();
            hP.X = s1P1.X- s1P2.X;
            hP.Y = s1P1.Y- s1P2.Y;
            return hP;
        }
        internal static System.Windows.Point PAdd(System.Windows.Point messpunkt, System.Windows.Point pH)
        {
            System.Windows.Point hP = new System.Windows.Point();
            hP.X = messpunkt.X + pH.X;
            hP.Y = messpunkt.Y + pH.Y;
            return hP;
        }
    }
}
