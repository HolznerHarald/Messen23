// mittlereLinienbreiteX nicht berechnet
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace Messen23
{
    public class Himg
    {
        // GV Globale Variable
        public Bitmap Img1;
        public Lines HorLines, VertLines;

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
        public void Findlines()   // Hauptprozedur 
        {
            HorLines = new Lines(Img1, MW);
            HorLines.Findlines();

            Img1.RotateFlip(RotateFlipType.Rotate90FlipNone);//!!H
            VertLines = new Lines(Img1, MW);
            VertLines.Findlines();
            Img1.RotateFlip(RotateFlipType.Rotate270FlipNone);//!!H
            VertLines.DarkYKoordFlip();

            MW.teste.TestAusgaben(this);
        }

        ///For Test XXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXx 
        private void Graph_Start()
        {
            // Graf graf1;
            //graf1 = new Graf(Img1, BrightnessListe, 1, Color.Red,0,100);
            //graf1 = new Graf(Img1, hBrightnessListe, 1, Color.Blue,0,100);                
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
