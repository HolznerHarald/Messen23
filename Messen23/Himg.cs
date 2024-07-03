// mittlereLinienbreiteX nicht berechnet
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using static Messen23.Lines;
using System.Threading.Tasks;
using System.Windows;



namespace Messen23
{
    public class Himg
    {
        // GV Globale Variable
        public Bitmap Img1;
        public Lines HorLines, VertLines;
        internal List<S1Kl>[] S1xy;    //Schnittpunkte

        private MainWindow MW;

        private bool testen = true;
        private hGraf graf1;
        //  Konstruktor XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        public Himg(Bitmap imgagepict, MainWindow mW)
        {
            MW = mW;
            Img1 = imgagepict;
        }

        ///Hauptprozedur XXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxHauptprozedur
        public bool Findlines()   // Hauptprozedur 
        {
            HorLines = new Lines(Img1, MW);
            HorLines.Findlines();

            Img1.RotateFlip(RotateFlipType.Rotate90FlipNone);//!!H
            VertLines = new Lines(Img1, MW);
            VertLines.Findlines();
            Img1.RotateFlip(RotateFlipType.Rotate270FlipNone);//!!H
            VertLines.DarkYKoordFlip();

            FindS1X();
            //MW.teste.TestAusgaben(this);
            MW.teste.ZeichneSchnittpunkte(this);
            return true;
        }

        private void FindS1X()   //SChnittpunkte finden
        {
            S1xy = new List<S1Kl>[HorLines.Dark.Count];
            for (int ii = 0; ii< HorLines.Dark.Count; ii++)    
                S1xy[ii] = new List<S1Kl>();
            erstelleAllePS();  // down und up -Listen vereinen
            for (int kk = 0; kk < HorLines.Dark.Count; kk++)
            {
                int AnzPs = HorLines.Dark[kk].AllePs.Count;
                for (int jj = AnzPs - 2; jj >=0; jj--)
                { //  für alle Punktepaare, jj und jj+1 die Schnittpunkte bestimmen mit VerLines.Dark
                    BestimmeVertikale(HorLines.Dark[kk].AllePs[jj+1], HorLines.Dark[kk].AllePs[jj], S1xy[kk],kk);                    
                }
            }
        }
        private void BestimmeVertikale(System.Drawing.Point Hpoint1, System.Drawing.Point Hpoint2, List<S1Kl> spoints, int kk1)
        {       
            double MX = (Hpoint1.X + Hpoint2.X)/2;  //Mittelwert von X
            for (int kk = 0; kk < VertLines.Dark.Count; kk++)
            {
                int IndexNext1 = 0; //Einfach die nächsten 2 Indicis bestimmen
                int IndexNext2 = 1;
                if( Math.Abs(VertLines.Dark[kk].AllePs[IndexNext1].X - MX) >
                    Math.Abs(VertLines.Dark[kk].AllePs[IndexNext2].X - MX))
                {
                    int hi = IndexNext1;
                    IndexNext1 = IndexNext2;
                    IndexNext2= hi;
                }

                for (int ii = 2; ii < VertLines.Dark[kk].AllePs.Count; ii++)
                {
                    if (Math.Abs(VertLines.Dark[kk].AllePs[ii].X - MX) <
                        Math.Abs(VertLines.Dark[kk].AllePs[IndexNext1].X - MX))
                    {
                        IndexNext2 = IndexNext1;
                        IndexNext1 = ii;
                    }    
                }
                double MaxAbst = 20 * HorLines.mittlereLinienbreiteX;
                if (Math.Abs(VertLines.Dark[kk].AllePs[IndexNext1].X - MX) > MaxAbst &&
                     Math.Abs(VertLines.Dark[kk].AllePs[IndexNext2].X - MX) > MaxAbst)
                    continue;
                double MVertY = (VertLines.Dark[kk].AllePs[IndexNext1].Y + VertLines.Dark[kk].AllePs[IndexNext2].Y) / 2;
  /*              if (MVertY > (2 * Hpoint2.Y - Hpoint1.Y))
                    continue;
                if (MVertY < (2 * Hpoint1.Y - Hpoint2.Y))
                    continue;*/
                S1Kl hp = Schneide(Hpoint1, Hpoint2, VertLines.Dark[kk].AllePs[IndexNext1], VertLines.Dark[kk].AllePs[IndexNext2]);
                if (hp.S1P.X != -1)
                {
                    hp.horizLine = kk1;
                    hp.vertLine = kk;
                    spoints.Add(hp);
                }
            }                   
        }
        private S1Kl Schneide(System.Drawing.Point hpoint1, System.Drawing.Point hpoint2, System.Drawing.Point point1, System.Drawing.Point point2)
        {
            S1Kl S1x = new S1Kl(new System.Windows.Point(-1, -1), 0, 0);

            System.Windows.Point S1 = new System.Windows.Point(-1,-1);
            bool ParallelY = false;
            double kk1=0, dd1=0, kk2, dd2;
            if (hpoint1.X == hpoint2.X)
            {
                ParallelY = true;
            }
            else
            {
                kk1 = (hpoint2.Y - hpoint1.Y) / (hpoint2.X - hpoint1.X);
                dd1 = hpoint1.Y - kk1 * hpoint1.X;
            }
            kk2 = (point2.Y - point1.Y) / (point2.X - point1.X);
            dd2 = point1.Y - kk2 * point1.X;

            if (ParallelY)
            {
                S1.X = hpoint1.X;
                S1.Y = kk2 * S1.X + dd2;
            }
            else
            {
                S1.X = (dd1 - dd2) / (kk2 - kk1);
                S1.Y = kk1 * S1.X + dd1;
            }

            double LageS1;
            LageS1 = (S1.Y - hpoint1.Y) / (hpoint2.Y - hpoint1.Y);

            if (LageS1 <= 1 && LageS1 >= 0)
            {
                S1x.S1P = S1;
                return S1x;
            }
            return S1x; 
        }
        private void erstelleAllePS()
        {
            for (int kk = 0; kk < HorLines.Dark.Count; kk++)
            {
                for (int ii = HorLines.Dark[kk].VertUp.Count - 1; ii >= 0; ii--)
                    HorLines.Dark[kk].AllePs.Add(HorLines.Dark[kk].VertUp[ii]);
                HorLines.Dark[kk].AllePs.Add(HorLines.Dark[kk].St);
                for (int ii = 0; ii < HorLines.Dark[kk].VertDown.Count; ii++)
                    HorLines.Dark[kk].AllePs.Add(HorLines.Dark[kk].VertDown[ii]);                
            }
            for (int kk = 0; kk < VertLines.Dark.Count; kk++)
            {
                for (int ii = VertLines.Dark[kk].VertUp.Count - 1; ii >= 0; ii--)
                    VertLines.Dark[kk].AllePs.Add(VertLines.Dark[kk].VertUp[ii]);
                VertLines.Dark[kk].AllePs.Add(VertLines.Dark[kk].St);
                for (int ii = 0; ii < VertLines.Dark[kk].VertDown.Count; ii++)
                    VertLines.Dark[kk].AllePs.Add(VertLines.Dark[kk].VertDown[ii]);              
            }
        }

        ///For Test XXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXx 
        public void Graph_Start()
        {
            hGraf graf1;
            int StartY = Img1.Width / 2;
            List<double> BrightnessListe= new List<double>();
            for (int ii = 0; ii < Img1.Width; ii++)
            {
                Color col = Img1.GetPixel(ii, StartY);
                BrightnessListe.Add(col.R);
            }
            graf1 = new hGraf(Img1, BrightnessListe, 1, Color.Red,0,100);
            //graf1 = new hGraf(Img1, hBrightnessListe, 1, Color.Blue,0,100);                
        }
        ///XXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXx
        private void Graph_Start2()
        {
            /*for Test */
            /*
            MW.teste.HSetPixel(this,new Point(80, StartlineHorizontal), Color.Green);
            MW.teste.HSetPixel(this, new Point(120, StartlineHorizontal), Color.Green);
            MW.teste.HSetPixel(this, new Point(50, StartlineHorizontal), Color.Green);
            MW.teste.HSetPixel(this, new Point(150, StartlineHorizontal), Color.Green);
            */
            //hGraf graf1 = new hGraf(Img1,SummeColBListe, 1, Color.Red,0,0);
        }
    }
}
