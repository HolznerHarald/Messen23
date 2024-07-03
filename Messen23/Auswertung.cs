using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Messen23
{
    internal class Auswertung
    {
        private Kalibr kal;
        private MessObj messO;
        private MainWindow MW;

        struct Position
        {
            internal int ErrorIndex;
            internal int LinksIndex;
            internal int UntenLinksIndex;
            internal int UntenRechtsIndex;  //kann verschieden sein, S1x.vertkal muss gleich sein
            internal bool linksNearer;
            internal bool untenNearer;
        }

        public Auswertung(Kalibr kal, MessObj messO, MainWindow mW)
        {
            this.kal = kal;
            this.messO = messO;
            MW = mW;
        }

        internal void Starten()
        {   //  Probleme :1. für andere Seite nicht Index sonder gleiche Vertiklae Line verwenden
            // 2: Routine für Vektoraddition
            Position PosImNetz;
            System.Windows.Point Messpunkt;
            for (int ii=0; ii<messO.MarkPs.Count; ii++) 
            {
                PosImNetz = new Position();
                Messpunkt = new System.Windows.Point(messO.MarkPs[ii].X, messO.MarkPs[ii].Y);
                PosImNetz =BestimmePosLR(Messpunkt);            
                if (PosImNetz.ErrorIndex>0)
                {
                    System.Windows.MessageBox.Show("Errorcode:"+PosImNetz.ErrorIndex.ToString());
                    return;
                }
                PosImNetz = BestimmePosUO(Messpunkt, PosImNetz);   //!!!!!
                if (PosImNetz.ErrorIndex > 0)
                {
                    System.Windows.MessageBox.Show("Errorcode:" + PosImNetz.ErrorIndex.ToString());
                    return;
                }
                

                int StartVert0 = kal.S1x[0][0].vertLine;
                int vertZuLO = kal.S1x[PosImNetz.LinksIndex][PosImNetz.UntenLinksIndex].vertLine - StartVert0; ;

                MW.Testliste.Items.Add("Koo. " + ii.ToString() + ":" + PosImNetz.LinksIndex.ToString() +
                    "//" + vertZuLO.ToString());


                //zum Testen :
                //MarkiereUmrand(PosImNetz);
                System.Windows.Point KooImUm=BerechneKooImUmrand(PosImNetz, Messpunkt);
                MW.Testliste.Items.Add("Koo. Umrand" + ii.ToString() + ":" + KooImUm.X.ToString() +
                    "//" + KooImUm.Y.ToString());

            }
            // 1 "Gerade gefunden mit nähesten S1, aber kein Teilstück mit echtem S1"
            // 2 "keine anderseitige Gerade vorhanden"
            // 3 "anderseitige Gerade hat kein paralleles Teilstück"
            // 4,5 "Sicherheit Überprüfen fehlgeschlagen , Punkt nicht zwischen den 2 Geraden"
            // 6 "unterer Teil nicht darunter"
            // 7 "oberer Teil nicht drüber"
        }
        

        private System.Windows.Point BerechneKooImUmrand(Position posImNetz, System.Windows.Point messpunkt)
        {
            System.Windows.Point PH, PUL, PUR, POL, POR, PPllOben, PPllSeite;
            double KoXUnten, KoXOben, KoYRechts,KoYLinks, KoXGewichtet, KoYGewichtet;
            PUL = kal.S1x[posImNetz.LinksIndex][posImNetz.UntenLinksIndex].S1P;
            PUR = kal.S1x[posImNetz.LinksIndex + 1][posImNetz.UntenRechtsIndex].S1P;
            POL = kal.S1x[posImNetz.LinksIndex][posImNetz.UntenLinksIndex + 1].S1P;
            POR = kal.S1x[posImNetz.LinksIndex + 1][posImNetz.UntenRechtsIndex + 1].S1P;
            //Parallele zu nearInd durch messpunkt und POben
            if (posImNetz.linksNearer)
            {
                PH = tools.PMinus(POL, PUL);                    
                PPllOben = tools.PAdd(messpunkt,PH);
            }
            else
            {
                PH = tools.PMinus(POR, PUR);
                PPllOben = tools.PAdd(messpunkt,PH);
            }            
            KoXUnten = SchneideVerhältnis(messpunkt, PPllOben, PUL, PUR);
            KoXOben = SchneideVerhältnis(messpunkt, PPllOben, POL, POR);

            //Parallele zu Horizontalen durch messpunkt und PllSeite
            if (posImNetz.untenNearer)
            {
                PH = tools.PMinus(PUR, PUL);
                PPllSeite = tools.PAdd(messpunkt, PH);
            }
            else
            {
                PH = tools.PMinus(POR, POL);
                PPllSeite = tools.PAdd(messpunkt, PH);
            }
            KoYLinks = SchneideVerhältnis(messpunkt, PPllSeite, PUL, POL);
            KoYRechts = SchneideVerhältnis(messpunkt, PPllSeite, PUR, POR);
            //!!!!! wo näher, das Verhältnis returnen  , noch besser wichten
            KoXGewichtet = KoXUnten * KoYLinks + KoXOben * (1- KoYLinks);
            KoYGewichtet = KoYLinks * KoXUnten + KoYRechts * (1- KoXUnten);
            return new System.Windows.Point(KoXGewichtet, KoYGewichtet);
        }
        //System.Windows.Point messpunkt, System.Windows.Point pPllSeite, System.Windows.Point pUL, System.Windows.Point pOL)
        private double SchneideVerhältnis(System.Windows.Point messpunkt, System.Windows.Point PLoderU, System.Windows.Point P1, System.Windows.Point P2)
        {
            System.Windows.Point S1;
            bool ParallelY1 = false;
            bool ParallelY2 = false;
            double kk1 = 0;
            double dd1 = 0;
            double kk2 = 0;
            double dd2 = 0;
            double Winkel;
            double LageS1;
            //1.Gerade init
            if (messpunkt.X == PLoderU.X)
            {
                ParallelY1 = true;
            }
            else
            {
                kk1 = (PLoderU.Y - messpunkt.Y) / (PLoderU.X - messpunkt.X);
                dd1 = messpunkt.Y - kk1 * messpunkt.X;
                if (PLoderU.X - messpunkt.X >= 0)
                {
                    if (PLoderU.Y - messpunkt.Y >= 0)
                        Winkel = Math.Atan(kk1);  //1.Qu.
                    else
                        Winkel = 2 * Math.PI + Math.Atan(kk1); //4.Qu.
                }
                else
                {
                    if (PLoderU.Y - messpunkt.Y >= 0)
                        Winkel = Math.Atan(kk1) + Math.PI;   //2.Qu.
                    else
                        Winkel = Math.PI + Math.Atan(kk1); //3.Qu.
                }
            }
            //2.te Gerade init
            if (P1.X == P2.X)
            {
                ParallelY2 = true;
            }
            else
            {
                kk2 = (P2.Y - P1.Y) / (P2.X - P1.X);
                dd2 = P1.Y - kk2 * P1.X;
                if (P2.X - P1.X >= 0)
                {
                    if (P2.Y - P1.Y >= 0)
                        Winkel = Math.Atan(kk2);  //1.Qu.
                    else
                        Winkel = 2 * Math.PI + Math.Atan(kk2); //4.Qu.
                }
                else
                {
                    if (P2.Y - P1.Y >= 0)
                        Winkel = Math.Atan(kk2) + Math.PI;   //2.Qu.
                    else
                        Winkel = Math.PI + Math.Atan(kk2); //3.Qu.
                }
            }
            //Schneiden
            if (ParallelY1)
            {
                S1.X = messpunkt.X;
                S1.Y = kk2 * S1.X + dd2;
            }
            else if (ParallelY2) 
            {
                S1.X = P1.X;
                S1.Y = kk1 * S1.X + dd1;
            }
            else
            {
                S1.X = (dd1 - dd2) / (kk2 - kk1);
                S1.Y = kk1 * S1.X + dd1;
            }

            if (Math.Abs(P2.X - P1.X) > Math.Abs(P2.Y - P1.Y))
                LageS1 = (S1.X - P1.X) / (P2.X - P1.X);
            else
                LageS1 = (S1.Y - P1.Y) / (P2.Y - P1.Y);
                        
            return LageS1;
        }

        private Position BestimmePosLR(System.Windows.Point P1)
        {
            Position posN =  new Position();
            int NearInd = -1;
            int nearIndOp;
            System.Windows.Point NormaleS1;
            double NormalenAbst = 100000000000000;
            //Gerade mit nähesten S1
            for (int ii = 0; ii < kal.S1x.Length; ii++)
            {
                NormaleS1 = SchneideNormalePg(P1, kal.S1x[ii][0].S1P, kal.S1x[ii][kal.S1x[ii].Count - 1].S1P);
                double hAbst = tools.dAbstand(NormaleS1, P1);
                if (hAbst < NormalenAbst)
                {
                    NormalenAbst = hAbst;
                    NearInd = ii;
                }
            }
            //Finde Teil-Gerade mit echtem S1
            int VertInd = -1;
            for (int jj = 0; jj < kal.S1x[NearInd].Count - 1; jj++)
            {
                NormaleS1 = SchneideNormalePg(P1, kal.S1x[NearInd][jj].S1P, kal.S1x[NearInd][jj + 1].S1P);
                if (echterS1(NormaleS1, kal.S1x[NearInd][jj].S1P, kal.S1x[NearInd][jj + 1].S1P))
                {
                    VertInd = kal.S1x[NearInd][jj].vertLine;
                    break;
                }
            }
            if (VertInd == -1)
            {
                posN.ErrorIndex = -1;
                return posN;
            }
            if (VertInd >= kal.S1x[NearInd].Count - 1)           
            {
                posN.ErrorIndex = -3;
                return posN;
            }

            // Index andere Seite nearIndOp  
            if (NormaleS1.X > P1.X)
                nearIndOp = NearInd - 1;
            else
                nearIndOp = NearInd + 1;

            if (nearIndOp < 0 || nearIndOp + 1 > kal.S1x.Length - 1)
                ; //Errorcode   !!!!!

            // check Index andere Seite nearIndOp
            int vertIndOp = -1;
            for (int ii = 0; ii < kal.S1x[posN.LinksIndex + 1].Count - 1; ii++)
                if (kal.S1x[NearInd][VertInd].vertLine == kal.S1x[nearIndOp][ii].vertLine)
                    vertIndOp = ii;

            if (vertIndOp == -1)            
            {
                posN.ErrorIndex = -1;
                return posN;
            }   
            if (vertIndOp >= kal.S1x[nearIndOp].Count - 1)
            {
                posN.ErrorIndex = -3;
                return posN;
            }

            if (NearInd < nearIndOp)
            {
                posN.LinksIndex = NearInd;
                posN.linksNearer = true;
                posN.UntenLinksIndex = VertInd;
                posN.UntenRechtsIndex = vertIndOp;
            }
            else
            {
                posN.LinksIndex = nearIndOp;
                posN.linksNearer = false;
                posN.UntenLinksIndex = vertIndOp;
                posN.UntenRechtsIndex = VertInd;
            }
            return posN;
        }

        private System.Windows.Point SchneideNormalePg(System.Windows.Point point, System.Windows.Point P1, System.Windows.Point P2)
        {
            System.Windows.Point NormalenS1;
            bool ParallelY = false;
            double kk = 0;
            double dd = 0;
            double Winkel;

            if (P1.X == P2.X)
            {
                ParallelY = true;
            }
            else
            {
                kk = (P2.Y - P1.Y) / (P2.X - P1.X);
                dd = P1.Y - kk * P1.X;
                if (P2.X - P1.X >= 0)
                {
                    if (P2.Y - P1.Y >= 0)
                        Winkel = Math.Atan(kk);  //1.Qu.
                    else
                        Winkel = 2 * Math.PI + Math.Atan(kk); //4.Qu.
                }
                else
                {
                    if (P2.Y - P1.Y >= 0)
                        Winkel = Math.Atan(kk) + Math.PI;   //2.Qu.
                    else
                        Winkel = Math.PI + Math.Atan(kk); //3.Qu.
                }
            }
            if (ParallelY)
            {
                NormalenS1.X = P1.X;
                NormalenS1.Y = point.Y;
            }
            else if (kk == 0)
            {
                NormalenS1.X = point.X;
                NormalenS1.Y = dd;
            }
            else
            {
                double Normalkk = -1 / kk;
                double Normaldd = point.Y - Normalkk * point.X;
                NormalenS1.X = (dd - Normaldd) / (Normalkk - kk);
                NormalenS1.Y = Normalkk * NormalenS1.X + Normaldd;
            }
            return NormalenS1;
        }

        private Position BestimmePosUO(System.Windows.Point P1, Position posN)
        {
            //zur Sicherheit Überprüfen ob nearIndOp eh auf anderer Seite von Punkt

            /*NormaleS1 = SchneideNormalePg(P1, kal.S1x[HorIndOp][0].S1P, kal.S1x[HorIndOp][kal.S1x[HorIndOp].Count - 1].S1P);
            if (NormaleS1.X <= P1.X && HorIndOp > NearInd)
                return new int[2] { -4, 0 };
            if (NormaleS1.X >= P1.X && HorIndOp < NearInd)
                return new int[2] { -5, 0 };*/
            // ist untere Seite untere Seite ?
            System.Windows.Point NormaleS1;

            NormaleS1 = SchneideNormalePg(P1, kal.S1x[posN.LinksIndex][posN.UntenLinksIndex].S1P,
                                kal.S1x[posN.LinksIndex + 1][posN.UntenRechtsIndex].S1P);
            
          /* Error nicht notwendig , ist halt bisschen drunter
             if (NormaleS1.X > P1.X)
                return new int[2] { -6, 0 }; */
            double AbstUnten = tools.dAbstand(NormaleS1, P1);
            // ist obere Seite obere Seite ?
            NormaleS1 = SchneideNormalePg(P1, kal.S1x[posN.LinksIndex][posN.UntenLinksIndex + 1].S1P,
                                           kal.S1x[posN.LinksIndex + 1][posN.UntenRechtsIndex + 1].S1P);
            /* Error nicht notwendig , 
            if (NormaleS1.X < P1.X)
                return new int[2] { -7, 0 }; */
            double AbstOben = tools.dAbstand(NormaleS1, P1);
            
            if (AbstUnten < AbstOben)
                posN.untenNearer = true;
            else
                posN.untenNearer = false;

            return posN;
        }
        

        private bool echterS1(System.Windows.Point S1, System.Windows.Point P1, System.Windows.Point P2)
        {
            double LageS1;
            if (Math.Abs(P2.X - P1.X) > Math.Abs(P2.Y - P1.Y))
                LageS1 = (S1.X - P1.X) / (P2.X - P1.X);
            else
                LageS1 = (S1.Y - P1.Y) / (P2.Y - P1.Y);

            if (LageS1 <= 1 && LageS1 >= 0)
            {
                return true;
            }
            else
                return false;
        }
    }
}
