using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Messen23
{
    internal class Kalibr
    {
        internal int _statusKalib = 0;
        internal Bitmap kBitmap;
        internal BitmapImage kbitimg;
        internal string filename;
        internal int Wiidth;
        internal int Height;
        internal  List<S1Kl>[] S1x;
        private MainWindow MW;
        //************** Konstante
        internal int ungeladen= 0;
        internal int geladen= 1;
        internal int bearbeitet= 2;
        internal int geöffnet= 3;        
        internal int StatusKalib
        {
            get => _statusKalib;
            set
            {
                _statusKalib = value;
                if (_statusKalib == ungeladen)
                {
                    MW.KalE.IsEnabled = false;
                    MW.KalS.IsEnabled = false;
                    MW.KalZ.IsEnabled = false;
                }
                else if (_statusKalib == geladen)
                {
                    MW.KalE.IsEnabled = true;
                    MW.KalS.IsEnabled = false;
                    MW.KalZ.IsEnabled = false;
                }
                else if (_statusKalib == bearbeitet)
                {
                    MW.KalE.IsEnabled = false;
                    MW.KalS.IsEnabled = true;
                    MW.KalZ.IsEnabled = true;
                }
                else if (_statusKalib == geöffnet)
                {
                    MW.KalE.IsEnabled = false;
                    MW.KalS.IsEnabled = false;
                    MW.KalZ.IsEnabled = true;
                }
            }
        }
        public Kalibr(MainWindow mW)
        {
            MW = mW;
        }

        public Kalibr(MainWindow mW, List<S1Kl>[] s1xy, int width, int height)
        {
            MW = mW;
            S1x= s1xy;
            Wiidth = width;
            Height = height;
        }
        internal bool Laden()
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {       
                String FileName = fileDialog.FileName;
                try
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(FileName, FileMode.Open)))
                    {
                        Wiidth = reader.ReadInt32();
                        Height = reader.ReadInt32();
                        int AnzLines = reader.ReadInt32();
                        S1x = new List<S1Kl>[AnzLines];
                        List<S1Kl> aktS1List = new List<S1Kl>();
                        int Zae = 0;
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            double d1 = reader.ReadDouble();
                            if (d1 < -1000000)
                            {
                                S1x[Zae] = aktS1List;
                                aktS1List=new List<S1Kl>();
                                Zae++;
                            }
                            else
                            {
                                double d2 = reader.ReadDouble();
                                int i1 = reader.ReadInt32();
                                int i2 = reader.ReadInt32();
                                aktS1List.Add(new S1Kl(new System.Windows.Point(d1, d2),i1,i2));
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    MW.Testliste.Items.Add(ee.Message);
                    return false;
                }
                return true;
            }
            else
                return false; 
        }

        internal void Save()
        {
            double endeReihe = 1000001;
            using (var strm = new MemoryStream())
            using (var bw = new BinaryWriter(strm))
            {
                bw.Write(Wiidth);
                bw.Write(Height);
                bw.Write(S1x.Length);
                foreach (List<S1Kl> Li in S1x)
                {
                    for (int ii = 0; ii < Li.Count; ii++)
                    {
                        bw.Write(Li[ii].S1P.X);
                        bw.Write(Li[ii].S1P.Y);
                        bw.Write(Li[ii].horizLine);
                        bw.Write(Li[ii].vertLine);
                    }                 
                    bw.Write(-endeReihe);
                }
                bw.Flush();
                File.WriteAllBytes("C:\\C#\\myfile.bytes", strm.ToArray());
            }
        }

        internal Bitmap Zeigen()
        {
            System.Windows.Point P1;
            System.Windows.Point P2;
            Point PI1;
            Point PI2;

            Bitmap hmb = new Bitmap(Wiidth, Height);

            Graphics g = Graphics.FromImage(hmb);
            Pen opaquePen = new Pen(Color.Red, 1);
            for (int ii = 0; ii < S1x.Length; ii++)
            {
                for (int jj = 0; jj < S1x.ElementAt(ii).Count - 1; jj++)
                {
                    P1 = S1x.ElementAt(ii).ElementAt(jj).S1P;
                    P2 = S1x.ElementAt(ii).ElementAt(jj + 1).S1P;
                    PI1 = new Point((int)Math.Round(P1.X), (int)Math.Round(P1.Y));
                    PI2 = new Point((int)Math.Round(P2.X), (int)Math.Round(P2.Y));
                    g.DrawLine(opaquePen, PI1, PI2);
                }
            }
            for (int ii = 0; ii < S1x.Length - 1; ii++)
            {
                for (int jj = 0; jj < S1x.ElementAt(ii).Count; jj++)
                {
                    for (int jj1 = 0; jj1 < S1x.ElementAt(ii + 1).Count; jj1++)
                    {
                        if (S1x.ElementAt(ii + 1).ElementAt(jj1).vertLine ==
                            S1x.ElementAt(ii).ElementAt(jj).vertLine)
                        {
                            P1 = S1x.ElementAt(ii+1).ElementAt(jj1).S1P;
                            P2 = S1x.ElementAt(ii).ElementAt(jj).S1P;
                            PI1 = new Point((int)Math.Round(P1.X), (int)Math.Round(P1.Y));
                            PI2 = new Point((int)Math.Round(P2.X), (int)Math.Round(P2.Y));
                            g.DrawLine(opaquePen, PI1, PI2);
                        }
                    }                    
                }
            }
            return hmb;  
        }

        internal BitmapImage Openfoto()
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                filename = fileDialog.FileName;
                Registry.SetValue(TestenReg.keyName, "filenameobj", filename);

                kbitimg = tools.FileLaden(filename);

                kBitmap = new Bitmap(filename);         

                StatusKalib = geladen;
                MW.Testliste.Items.Add("Open:" + filename);
            }
            return kbitimg;
        }        
    }
}
