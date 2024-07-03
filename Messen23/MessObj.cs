using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Messen23
{
    internal class MessObj
    {
        internal int _statusMess = 0;
        internal List<Point> MarkPs;
        internal Point LetzterGelöschterP= new Point(-1000, -1000);
        internal System.Windows.Point lastMouseStart;
        internal Bitmap origBitmap;
        internal Bitmap mBitmap;
        internal BitmapImage mbitimg;
        internal string filename;
        internal int Width;
        internal int Height;        
        private MainWindow MW;
        //************** Konstante
        internal int ungeladen = 0;
        internal int geladen = 1;
        internal int MarkierungBegonnen = 2;
        internal int MarkierungFertig = 3;
        internal int statusMess
        {
            get => _statusMess;
            set
            {
                _statusMess = value;
                if (_statusMess == ungeladen)
                {
                    MW.MarkBeginn.IsEnabled = false;
                    MW.MarkSave.IsEnabled = false;
                }
                else if (_statusMess == geladen)
                {
                    MW.MarkBeginn.IsEnabled = true;
                    MW.MarkSave.IsEnabled = false;
                }
                else if (_statusMess == MarkierungBegonnen)
                {
                    MW.MarkBeginn.IsEnabled = false;
                    MW.MarkSave.IsEnabled = true;
                }
                else if (_statusMess == MarkierungFertig)
                {
                    MW.MarkBeginn.IsEnabled = true;
                    MW.MarkSave.IsEnabled = false;
                }
            }
        }
        public MessObj(MainWindow mW)
        {
            MW = mW;
            MarkPs = new List<Point>();
        }

        internal ImageSource Openfoto()
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                filename = fileDialog.FileName;
                Registry.SetValue(TestenReg.keyName, "filenamemessobj", filename);

                mbitimg = tools.FileLaden(filename);

                mBitmap = new Bitmap(filename);
                origBitmap = new Bitmap(filename);

                statusMess = geladen;
                MW.Testliste.Items.Add("Object Open:" + filename);
            }
            return mbitimg;
        }

        internal void Mousedown()
        {
            System.Windows.Point lMouseStart;
            System.Windows.Point lGGMouseStart;
            lMouseStart = Mouse.GetPosition(MW.imgName1);
            Line line1;
            Line line2;

            double currImgX = lMouseStart.X * mBitmap.Width / MW.imgName1.ActualWidth;
            double currImgY = lMouseStart.Y * mBitmap.Height / MW.imgName1.ActualHeight;
            System.Drawing.Point hp = new System.Drawing.Point((int)Math.Round(currImgX), (int)Math.Round(currImgY));
            tools.HSetPixel(mBitmap, hp, System.Drawing.Color.Red, 10);
            MarkPs.Add(hp);

            line1 = new Line();
            line2 = new Line();
            lGGMouseStart = Mouse.GetPosition(MW.GG); // weil Children nur für Grid
            line1.Stroke = System.Windows.Media.Brushes.Blue;
            line1.X1 = lGGMouseStart.X - 5;
            line1.Y1 = lGGMouseStart.Y - 5;
            line1.X2 = lGGMouseStart.X + 5;
            line1.Y2 = lGGMouseStart.Y + 5;
            line2.Stroke = System.Windows.Media.Brushes.Blue;
            line2.X1 = lGGMouseStart.X + 5;
            line2.Y1 = lGGMouseStart.Y - 5;
            line2.X2 = lGGMouseStart.X - 5;
            line2.Y2 = lGGMouseStart.Y + 5;

            MW.GG.Children.Add(line1);
            MW.GG.Children.Add(line2);
        }

        internal void letztesLoeschen()
        {
            LetzterGelöschterP = MarkPs[MarkPs.Count - 1];           
            MarkPs.RemoveAt(MarkPs.Count - 1);
            NeuZeichnen();
        }

        private void NeuZeichnen()
        {
            mBitmap = (Bitmap)origBitmap.Clone();
            for (int ii = 0; ii < MarkPs.Count; ii++)
            {
                tools.HSetPixel(mBitmap, MarkPs[ii], System.Drawing.Color.Orange, 10);                
            }
            
        }

        internal void NaheLösch()
        {
            double currImgX = lastMouseStart.X * mBitmap.Width / MW.imgName1.ActualWidth;
            double currImgY = lastMouseStart.Y * mBitmap.Height / MW.imgName1.ActualHeight;
            System.Drawing.Point hp = new System.Drawing.Point((int)Math.Round(currImgX), (int)Math.Round(currImgY));

            int MinInd=0;
            double minAbst= tools.Abstand(hp, MarkPs[0]);
            for (int ii = 1;ii < MarkPs.Count; ii++)
            {
                double habst=tools.Abstand(hp, MarkPs[ii]);
                if (habst < minAbst) 
                {
                    minAbst = habst;
                    MinInd = ii;
                }
            }
            LetzterGelöschterP = MarkPs[MinInd];
            MarkPs.RemoveAt(MinInd);
            NeuZeichnen();
        }

        internal void GelöschtDazu()
        {
            if (LetzterGelöschterP.X != -1000)
            {
                MarkPs.Add(LetzterGelöschterP);
                LetzterGelöschterP = new Point(-1000, -1000);
                NeuZeichnen();
            }
        }
    }
}
