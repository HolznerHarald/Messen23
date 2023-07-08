using System.Collections.Generic;
using System.Windows.Media;
using System;
using System.Drawing;
using Color = System.Drawing.Color;
using System.Linq;
using Pen = System.Drawing.Pen;

namespace Messen23
{
    class hGraf
    {
        public Bitmap Img1;
        //Darstellungsart : 0 nur Punkt , 1 Verbundene Linien
        public hGraf(Bitmap imgagepict, List<double> BrightnessListe, int Darstellungsart, Color col, int StartPunktX, int StartPunktY)
        {
            init(imgagepict, BrightnessListe, Darstellungsart, col, StartPunktX, StartPunktY);
        }

        public void init(Bitmap imgagepict, List<double> BrightnessListe, int Darstellungsart, Color col, int StartPunktX, int StartPunktY)
        {
            double hd;
            double dMin = BrightnessListe.Min();
            double dMax = BrightnessListe.Max();
            double dGroessteDiff = dMax - dMin;
            Img1 = imgagepict;

            if (Darstellungsart == 0)
                for (int ii = 0; ii < BrightnessListe.Count; ii++)
                {
                    hd = (BrightnessListe[ii] - dMin) / dGroessteDiff;
                    Img1.SetPixel(ii, Img1.Height - 1 - (int)Math.Round(hd * 100), col);
                }
            else if (Darstellungsart == 1)
                for (int ii = 0; ii < BrightnessListe.Count - 1; ii++)
                {
                    hd = (BrightnessListe[ii] - dMin) / dGroessteDiff;
                    Point P1 = new Point(ii + StartPunktX, Img1.Height - 1 - StartPunktY - (int)Math.Round(hd * 100));
                    hd = (BrightnessListe[ii + 1] - dMin) / dGroessteDiff;
                    Point P2 = new Point(ii + 1 + StartPunktX, Img1.Height - 1 - StartPunktY - (int)Math.Round(hd * 100));

                    Graphics g = Graphics.FromImage(Img1);
                    Pen opaquePen = new Pen(col, 1);
                    g.DrawLine(opaquePen, P1, P2);
                }
        }

    }
}
