using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Messen23
{
    public class Lines
    {
        // GV Globale Variable
        public List<sDarkX> Dark = new List<sDarkX>();        
        public int Startline;

        private List<int> DarkMax = new List<int>();
        private Bitmap Img1;
        private MainWindow MW;
        private double mittlereLinienAbstand;
        private double mittlereLinienbreiteX = 1;


        // GD Globale Declarations
        //  horizontale

        private int AnzLines;
        private int GlaettungsBereich = 20;
        private int GlaettungsBereich2 = 10;
        private double Glaettungsfaktor = 0.9;
        private int SuchWinkel = 20;
        const int StartBereichsTeil = 100;
        const int EndBereichsTeil = 10;
        const int RahmenbereichsTeil = 4;
        const double VergleichsVerhältnis = 1.2;
        const double Schwelle = 0.3;
        const int DurschBereichsteil = 10;
        /// <summary>        
        /// </summary>
        /// <param name="imgagepict"></param>
        //  Konstruktor XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        public Lines(Bitmap imgagepict, MainWindow mW)
        {
            MW = mW;
            Img1 = imgagepict;
            AnzLines = Img1.Width / 10;
        }
        private struct MüB
        {
            public int Bereich; public int Min;
            public MüB(int hBereich, int hMin)
            {
                Bereich = hBereich;
                Min = hMin;
            }
        }
        private struct AüB
        {
            public int Bereich; public int Anstieg;
            public AüB(int hBereich, int hAnstieg)
            {
                Bereich = hBereich;
                Anstieg = hAnstieg;
            }
        }
        public struct sDarkX
        {
            public Point St;
            public int Linienbreite;
            public List<Point> VertDown;
            public List<Point> VertUp;
            public List<Point> Glatt;
            public sDarkX(Point Start, int LinBr)
            {
                //Form1.ActiveForm();
                St = Start;
                Linienbreite = LinBr;
                VertDown = new List<Point>();
                VertUp = new List<Point>();
                Glatt = new List<Point>();
            }
        }

        ///Hauptprozedur XXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxHauptprozedur
        ///              

        public void Findlines()   // Hauptprozedur 
        {            
            // findet die horizontale Linie,StartlineHorizontal, entlang dann nach vertikalen Linien gesucht wird;            
            Startline = findstartlineHV(); //METH1
            // entlang Startlinie Durchschnitte über Bereich, damit Punkte  über SChwelle finden-> glLinienPunkte
            List<int> LinesPunkte = FindePunkteAufLinie(Startline);
            // Punkte ohne Abstände zu Linien vereinen, Linienabstände berechnen, Ausreißer ??
            Dark = FindLinienBeginn(Startline, LinesPunkte);
            // berechnet die mittlere Linienbreite            
            mittlereLinienbreiteX = berechneMitllereLinienbreite(Dark);

            do
                mittlereLinienAbstand = CheckLinienAbstände(Dark, mittlereLinienbreiteX);
            while (mittlereLinienAbstand < 0);

            //zu allen Punkten DarkX entlang der Startlinie wird eine vertikale Linie gesucht
            for (int ii = 0; ii < Dark.Count; ii++)
                GoAlongLine(ii, Dark);
            Glätten2(Dark);           
        }
        public void DarkYKoordFlip()
        {
            List<sDarkX> hDarkY = new List<sDarkX>();
            for (int ii = 0; ii < Dark.Count; ii++)
            {
                hDarkY.Add(new sDarkX(new Point(Dark[ii].St.Y, Img1.Height - 1 - Dark[ii].St.X), Dark[ii].Linienbreite));

                for (int jj = 0; jj < Dark[ii].VertDown.Count; jj++)
                    hDarkY[ii].VertDown.Add(new Point(Dark[ii].VertDown[jj].Y, Img1.Height - 1 - Dark[ii].VertDown[jj].X));
                for (int jj = 0; jj < Dark[ii].VertUp.Count; jj++)
                    hDarkY[ii].VertUp.Add(new Point(Dark[ii].VertUp[jj].Y, Img1.Height - 1 - Dark[ii].VertUp[jj].X));
            }
            Dark = hDarkY;
        }


        private double berechneMitllereLinienbreite(List<sDarkX> Hdark)
        {
            double Hsum = 0;
            for (int ii = 0; ii < Hdark.Count; ii++)
                Hsum = Hsum + Hdark[ii].Linienbreite;
            return Hsum / Hdark.Count;
        }

        private void Glätten2(List<sDarkX> dark)
        {
            double SummeWichtung = 0;

            for (int jj = -GlaettungsBereich2; jj <= GlaettungsBereich2; jj++)
            {
                double hd2 = (double)Math.Abs(jj);
                SummeWichtung = SummeWichtung + (GlaettungsBereich2 - hd2) / GlaettungsBereich2;
            }

            for (int ii = 0; ii < dark.Count; ii++)
                GlättenHilfsProz(dark[ii], SummeWichtung);
        }

        private void GlättenHilfsProz(sDarkX hDark, double SummeWichtung)
        {
            for (int ii = hDark.VertDown.Count - 1; ii >= 0; ii--)
                hDark.Glatt.Add(hDark.VertDown[ii]);
            for (int ii = 0; ii < hDark.VertUp.Count; ii++)
                hDark.Glatt.Add(hDark.VertUp[ii]);

            List<double> GlattListe = new List<double>();
            for (int ii = GlaettungsBereich2; ii < hDark.Glatt.Count - GlaettungsBereich2; ii++)
            {
                double hd = 0;
                // Algorithmus 1 zur Glättung
                for (int jj = -GlaettungsBereich2; jj <= GlaettungsBereich2; jj++)
                {
                    double hd2 = (double)Math.Abs(jj);
                    hd = hd + hDark.Glatt[ii + jj].X * (GlaettungsBereich2 - hd2) / GlaettungsBereich2;
                }
                hd = hd / SummeWichtung;
                GlattListe.Add(hd);
            }
            for (int ii = 0; ii < GlaettungsBereich2; ii++)
            {
                hDark.Glatt.RemoveAt(0);
                hDark.Glatt.RemoveAt(hDark.Glatt.Count - 1);
            }
            for (int ii = 0; ii < GlattListe.Count; ii++)
                hDark.Glatt[ii] = new Point((int)Math.Round(GlattListe[ii]), hDark.Glatt[ii].Y);
        }
        ///Meth1 XXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXx
        private int findstartlineHV()
        {
            List<double> SummeColBListe = new List<double>();

            for (int ii = 0; ii < 2 * AnzLines + 2 * GlaettungsBereich + 1; ii++)  // dazu werden um die horizontale Linie rauf und runter jeweils AnzLines gecheckt 
            {
                int HLineNr = Img1.Height / 2 - AnzLines - GlaettungsBereich + ii;
                double HHd = SummeColB(HLineNr);
                SummeColBListe.Add(HHd);
            }

            List<double> GlattSumCol = Glätten(SummeColBListe);
            int HMaxZae = HMax(GlattSumCol);
            int LineNr = Img1.Height / 2 + HMaxZae - (GlattSumCol.Count - 1) / 2;
            return LineNr;            
        }
        ///Ebene3 XXXXXXXXXxXXXXXXXXXx
        private double SummeColB(int welcheLinie)
        {
            double SummeB = 0;
            Point StartP, RichtungsVektor;
            StartP = new Point(0, welcheLinie);
            RichtungsVektor = new Point(1, 0);

            for (int ii = 0; ii < Img1.Width; ii++)
            {
                Color col = Img1.GetPixel(StartP.X + ii * RichtungsVektor.X, StartP.Y + ii * RichtungsVektor.Y);
                SummeB = SummeB + col.GetBrightness(); //!!Test
                SummeB = SummeB + col.B;
            }
            return SummeB;
        }    
        ///Ebene3 XXXXXXXXXxXXXXXXXXXx
        private List<double> Glätten(List<double> HListe)
        {
            List<double> GlattListe = new List<double>();
            for (int ii = GlaettungsBereich; ii < HListe.Count - GlaettungsBereich; ii++)
            {
                double hd = 0;
                // Algorithmus 1 zur Glättung
                for (int jj = -GlaettungsBereich; jj <= GlaettungsBereich; jj++)
                {
                    double hd2 = (double)Math.Abs(jj);
                    hd = hd + HListe[ii + jj] * Math.Pow(Glaettungsfaktor, hd2);
                }
                GlattListe.Add(hd);
            }
            return GlattListe;
        }  
  
        ///Meth2 XXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXx
        private List<int> FindePunkteAufLinie(int Starty)
        {
            List<int> LinienPunkte=new List<int>();
            int Bereich = Img1.Width / DurschBereichsteil;
            List<int> ColBList = new List<int>();
            List<double> Durchschnitte = new List<double>();
            List<double> DurchschnitteMinusColB = new List<double>();
            Color col;
            for (int ii = 0; ii < Img1.Width; ii++)
            {
                col = Img1.GetPixel(ii, Starty);
                ColBList.Add(col.B);
            }

            for (int ii = 0; ii < Img1.Width; ii++)
            {
                Durchschnitte.Add(BerechneDurchschnitt(ii, Bereich, ColBList));
                DurchschnitteMinusColB.Add(Durchschnitte[ii] - ColBList[ii]);
            }
            double ZeilenMin = DurchschnitteMinusColB[0];
            double ZeilenMax = DurchschnitteMinusColB[0];
            for (int ii = 1; ii < DurchschnitteMinusColB.Count; ii++)
            {
                if (DurchschnitteMinusColB[ii] < ZeilenMin)
                    ZeilenMin = DurchschnitteMinusColB[ii];
                if (DurchschnitteMinusColB[ii] > ZeilenMax)
                    ZeilenMax = DurchschnitteMinusColB[ii];
            }
            for (int ii = 0; ii < Img1.Width; ii++)
            {
                double dSchwelle = Schwelle * (ZeilenMax);
                if (DurchschnitteMinusColB[ii] > dSchwelle)
                    LinienPunkte.Add(ii);
            }
            return LinienPunkte;
        }

        ///Meth3 XXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXx
        private List<sDarkX> FindLinienBeginn(int Starty, List<int> linesPunkte)
        {
            List<sDarkX> hDark = new List<sDarkX>();
            List<int> LinienAbstände = new List<int>();
            int PunkteAufLinie = 0;
            for (int ii = 0; ii < linesPunkte.Count; ii++)
            {
                if (ii > 1 && linesPunkte[ii] - linesPunkte[ii - 1] > 1)
                {
                    LinienAbstände.Add(linesPunkte[ii] - linesPunkte[ii - 1]);
                    hDark.Add(new sDarkX(new Point(linesPunkte[ii - 1] - PunkteAufLinie / 2, Starty), PunkteAufLinie));
                    PunkteAufLinie = 1;
                }
                else
                    PunkteAufLinie++;
            }
            hDark.Add(new sDarkX(new Point(linesPunkte[^1] - PunkteAufLinie / 2, Starty), PunkteAufLinie));
            return hDark;
        }

        private int CheckLinienAbstände(List<sDarkX> PDark, double mittlLinienbreite)
        {
            List<int> Abstände = new List<int>();

            for (int ii = 0; ii < PDark.Count - 1; ii++)
            {
                Abstände.Add(PDark[ii + 1].St.X - PDark[ii].St.X);
            }

            int Zae;
            int MaxZae = 0;
            int AbstandVonMaxZae = -99;
            for (int ii = 0; ii < Abstände.Count - 1; ii++)
            {
                Zae = 0;
                for (int jj = 0; jj < Abstände.Count - 1; jj++)
                    if (ii != jj && Math.Abs(Abstände[ii] - Abstände[jj]) <= mittlereLinienbreiteX)
                    {
                        Zae++;
                    }
                if (Zae > MaxZae)
                {
                    MaxZae = Zae;
                    AbstandVonMaxZae = Abstände[ii];
                }
            }

            //List<int> HAbstände = Abstände.OrderByDescending(x => x).ToList();
            if ((double)Abstände.Max() / (double)AbstandVonMaxZae > 1.1)
            {
                MessageBox.Show("Linienabstände unklar!!!");
                MW.Testliste.Items.Add("Linienabstände unklar!!!\n" +
                    "mindestens ein Abstand zu groß:" + Abstände[0].ToString());
            }
            else if ((double)Abstände.Min() / (double)AbstandVonMaxZae < 0.9)
            {
                //MessageBox.Show("Linienabstände unklar!!!");
                MW.Testliste.Items.Add("Linienabstände unklar!!!\n" +
                    "mindestens ein Abstand zu klein:" + Abstände[Abstände.Count - 1].ToString());

                for (int ii = 0; ii < PDark.Count - 1; ii++)
                {
                    if (PDark[ii].Linienbreite < 0.5 * mittlLinienbreite)
                    {
                        if (ii < Abstände.Count &&
                            (double)Abstände[ii] / (double)AbstandVonMaxZae < 0.9)
                        {
                            PDark.RemoveAt(ii);
                            MW.Testliste.Items.Add("ein Pdark entfernt");
                            return -1;
                        }
                        else if (ii > 0 &&
                            (double)Abstände[ii - 1] / (double)AbstandVonMaxZae < 0.9)
                        {
                            PDark.RemoveAt(ii);
                            MW.Testliste.Items.Add("ein Pdark entfernt");
                            return -1;
                        }
                    }
                }
            }
            return AbstandVonMaxZae;
            //Entferne unpassende Abstände
        }

        private double BerechneDurchschnitt(int von, int bereich, List<int> colBList)
        {
            double Summe = 0;
            von = von - bereich / 2;
            if (von < 0)
                von = 0;
            if (bereich > Img1.Width - von)
                bereich = Img1.Width - von;

            for (int ii = von; ii < von + bereich; ii++)
                Summe = Summe + colBList[ii];
            Summe = Summe / bereich;

            //double  Summe= colBList.Skip(von).Take(bereich).Sum() / bereich;  // gehte eh aber gerundet     
            return Summe;
        }

        private int CaLinienAbstandsav(int Starty)
        {
            List<MüB> FürBereicheMinDiff = new List<MüB>();
            List<AüB> Anstiege = new List<AüB>();
            int KleinsterBereich = Img1.Width / StartBereichsTeil;
            int GrößterBereich = Img1.Width / EndBereichsTeil;
            int Rahmen = Img1.Width / RahmenbereichsTeil;
            for (int jj = KleinsterBereich; jj <= GrößterBereich; jj++)
                FürBereicheMinDiff.Add(new MüB(jj, MinDiffMaxMin(jj, Rahmen, Starty, true)));

            int kk = 0;
            while (kk < FürBereicheMinDiff.Count)
            {
                double dh = FürBereicheMinDiff[kk].Bereich * VergleichsVerhältnis;
                int hind = Convert.ToInt32(Math.Round(dh));
                if (hind > GrößterBereich)
                {
                    kk++;
                    continue;
                }
                Anstiege.Add(new AüB(kk, FürBereicheMinDiff[hind].Bereich - FürBereicheMinDiff[kk].Bereich));
                kk++;
            }

            AüB MaxAnstieg = Anstiege.OrderByDescending(x => x.Anstieg).First(); //!!!
            return MaxAnstieg.Bereich;
        }

        private int MinDiffMaxMin(int Bereich, int RahmenUmMitte, int Starty, bool ReturnMinOrIndex)
        {
            List<int> DiffMinMax = new List<int>();
            int Startx = Img1.Width / 2 - RahmenUmMitte;
            for (int ii = 0; ii < 2 * RahmenUmMitte - Bereich; ii++)
            {
                List<int> HList = new List<int>();
                for (int jj = 0; jj < Bereich; jj++)
                {
                    Color col = Img1.GetPixel(Startx + ii + jj, Starty);
                    HList.Add(col.B);
                }
                HList.Sort();
                DiffMinMax.Add(HList[Bereich - 1] - HList[0]);
            }
            int MinIndex = 0;
            int Hmin = DiffMinMax[0];
            for (int ii = 1; ii < DiffMinMax.Count; ii++)
                if (DiffMinMax[ii] < Hmin)
                {
                    Hmin = DiffMinMax[ii];
                    MinIndex = ii;
                }

            if (ReturnMinOrIndex)
                return Hmin;
            else
                return MinIndex + Startx;
        }
        ///Meth4 XXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXx
        private void GoAlongLine(int ii, List<sDarkX> PDark)
        {
            Point SuchPunkt = PSuch();
            bool weiter = true;
            Point StartP;
            Point ZielP;

            StartP = PDark[ii].St;
            while (weiter)  //down
            {
                if (StartP.Y + SuchPunkt.Y >= Img1.Height)
                {
                    weiter = false;
                    continue;
                }
                //speichert in Liste DarkZ jeweils in Winkelbereich die Summe von Brightness entlang Geraden im Bereich SuchPunkt:Bleibt, variiert von - bis + SuchPunkt.x  (im DarXX den Index)                         
                ZielP = findAVertikalOne(StartP, SuchPunkt);
                PDark[ii].VertDown.Add(ZielP);
                StartP = ZielP;
            }
            StartP = PDark[ii].St;
            SuchPunkt.Y = -SuchPunkt.Y;
            weiter = true;
            while (weiter)  //up
            {
                if (StartP.Y + SuchPunkt.Y <= 0)
                {
                    weiter = false;
                    continue;
                }
                //speichert in Liste DarkZ jeweils in Winkelbereich die Summe von Brightness entlang Geraden im Bereich SuchPunkt:Bleibt, variiert von - bis + SuchPunkt.x  (im DarXX den Index)                         
                ZielP = findAVertikalOne(StartP, SuchPunkt);
                PDark[ii].VertUp.Add(ZielP);
                StartP = ZielP;
            }
        }
        ///Ebene3 XXXXXXXXXxXXXXXXXXXx
        private Point PSuch()
        {
            Point StartP = new Point(0, 0);
            double fWinkelPunkte = 20 * mittlereLinienbreiteX;
            fWinkelPunkte = (float)Math.Tan(Math.PI * SuchWinkel / 180) * fWinkelPunkte;

            StartP.Y = (int)Math.Round(20 * mittlereLinienbreiteX);
            StartP.X = (int)Math.Round(fWinkelPunkte);

            return StartP;
        }

        ///Ebene3 XXXXXXXXXxXXXXXXXXXx
        private Point findAVertikalOne(Point StartP, Point SuchPunkt)
        {
            List<double> DarkZ = new List<double>();
            int von, bis, neuvon, neubis;
            double hd;
            Point neuSPunkt = new Point();
            Point ZielP;

            neuSPunkt.Y = SuchPunkt.Y;
            von = -SuchPunkt.X;
            bis = SuchPunkt.X;
            neuvon = von;
            neubis = bis;
            for (int ii = von; ii <= bis; ii++)
            {
                if (StartP.X + ii < 0)
                    neuvon = ii + 1;
            }
            for (int ii = bis; ii >= von; ii--)
            {
                if (StartP.X + ii >= Img1.Width)
                    neubis = ii - 1;
            }
            double Min1 = 10000000000;
            int MinZae = -1;
            for (int ii = neuvon; ii <= neubis; ii++)
            {
                neuSPunkt.X = ii;
                hd = GeradeInRichtung(StartP, neuSPunkt);
                DarkZ.Add(hd);
                if (hd < Min1)
                {
                    Min1 = hd;
                    MinZae = ii;
                }
            }
            ZielP = new Point(StartP.X + MinZae, StartP.Y + SuchPunkt.Y);
            return ZielP;
        }
        ///Ebene4 XXXXXXXXXxXXXXXXXXXx
        private double GeradeInRichtung(Point StartP, Point RichtungP)
        {
            Point MessPunkt = new Point();
            double darkZ = 0;
            int von = 0;
            int bis = RichtungP.Y;
            if (RichtungP.Y < 0)
            {
                von = RichtungP.Y;
                bis = 0;
            }


            for (int ii = von; ii <= bis; ii++)
            {
                MessPunkt.Y = ii + StartP.Y;
                if (ii == 0)
                {
                    MessPunkt.X = StartP.X;
                }
                else
                {
                    int xx = (int)Math.Round((float)ii * RichtungP.X / RichtungP.Y);
                    MessPunkt.X = StartP.X + xx;
                }
                Color col = Img1.GetPixel(MessPunkt.X, MessPunkt.Y);

                //darkZ = darkZ + col.GetBrightness(); HHT
                darkZ = darkZ + col.B;
            }
            return darkZ;
        }
        //Hilfs-Methoden XXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXx  

        ///XXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXx
        // returned den Index mit dem Maximalwert
        private int HMax(List<double> Hliste)
        {
            double Max1 = 0;
            int MaxZae = -1;
            for (int ii = 0; ii < Hliste.Count; ii++)
            {
                if ((Hliste[ii] > Max1))
                {
                    Max1 = Hliste[ii];
                    MaxZae = ii;
                }
            }
            return MaxZae;
        }       
    }
}
