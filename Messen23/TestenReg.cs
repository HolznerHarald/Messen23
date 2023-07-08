using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Messen23
{
    class TestenReg
    {
        private TestAnsicht TestAn;

        internal int glAnsichtSingle = 0;
        internal int glTestAnsicht1, glTestAnsicht2;
        internal int glTTKreuz, glTTLinien;
        internal const string subkey = "Messen23";
        internal const string keyName = userRoot + "\\" + subkey;

        private MainWindow MW;
        private const string userRoot = "HKEY_CURRENT_USER";
        private int PunktKreuzGroesse = 15;

        //XXXXXX Konstruktor
        internal TestenReg(MainWindow mW)
        {
            MW = mW;
            Registry.SetValue(keyName, "", 9998);
            glTestAnsicht1 = (int)Registry.GetValue(keyName, "TestAnsicht1", 1);
            glTestAnsicht2 = (int)Registry.GetValue(keyName, "TestAnsicht2", 1);
            glTTKreuz = (int)Registry.GetValue(keyName, "TestAnsichtChKreuz", 1);
            glTTLinien = (int)Registry.GetValue(keyName, "TestAnsichtChLine", 0);
            glAnsichtSingle = (int)Registry.GetValue(keyName, "TestAnsichtsingle", 0);


            String s2 = (String)Registry.GetValue(keyName, "filename", "def");
            if (s2 != "def" && File.Exists(s2))
                MW.glAktFileName = s2;
            else
                MW.glAktFileName = @"C:\C#\C# messen  ALT\C# messen\k1000.jpg";

            //glAktFileName2 = @"C:\C#\C# messen  ALT\C# messen\test500.jpg";
        }

        internal void closing()
        {
            Registry.SetValue(keyName, "TestAnsicht1", glTestAnsicht1);
            Registry.SetValue(keyName, "TestAnsicht2", glTestAnsicht2);

            Registry.SetValue(keyName, "TestAnsichtChKreuz", glTTKreuz);
            Registry.SetValue(keyName, "TestAnsichtChLine", glTTLinien);
            Registry.SetValue(keyName, "TestAnsichtsingle", glAnsichtSingle);

            if (TestAn != null)
                TestAn.Close();
        }

        internal void AnsichtClick()
        {
            TestAn = new TestAnsicht(this);
            TestAn.Init();

            bool? Res = TestAn.ShowDialog();
            if (Res == false)
                return;
            if (TestAn.single.IsChecked == true)
                glTTLinien = 0;
            else
                glAnsichtSingle = 1;
            if (TestAn.CH12.IsChecked == true)
                glTTLinien = 0;
            else
                glTTLinien = 1;
            if (TestAn.CH13.IsChecked == true)
                glTTKreuz = 0;
            else
                glTTKreuz = 1;

            if (TestAn.single.IsChecked == true)
                glAnsichtSingle = 0;
            else
                glAnsichtSingle = 1;
            if (TestAn.RB11.IsChecked == true)
                glTestAnsicht1 = 1;
            else if (TestAn.RB12.IsChecked == true)
                glTestAnsicht1 = 2;
            else
                glTestAnsicht1 = 3;

            if (TestAn.RB21.IsChecked == true)
                glTestAnsicht2 = 1;
            else if (TestAn.RB22.IsChecked == true)
                glTestAnsicht2 = 2;
            else if (TestAn.RB23.IsChecked == true)
                glTestAnsicht2 = 3;
            else
                glTestAnsicht2 = 4;
        }

        internal void TestAusgaben(Himg himg)
        {
            //Kreuzerln
            if ((glAnsichtSingle == 0 && glTestAnsicht1 == 3) ||
                (glAnsichtSingle == 1 && glTTKreuz == 1))
            {
                if (glTestAnsicht2 == 2 || glTestAnsicht2 == 4)
                {//Kreuzerln horiz
                    ZeichneKreuze(himg.HorLines.Dark, Color.Red, himg);
                    HZeichneLinie(himg, 0, himg.HorLines.Startline + 1, himg.Img1.Width / 4, himg.HorLines.Startline + 1, Color.Red);  //!!TEST                   
                    //HZeichneLinie(himg, 0, himg.StartlineHorizontal + 1, himg.Img1.Width / 4, himg.StartlineHorizontal, Color.Red);  //!!TEST                   
                }
                if (glTestAnsicht2 == 3 || glTestAnsicht2 == 4)
                {//Kreuzerln vert 
                    ZeichneKreuze(himg.VertLines.Dark, Color.DarkBlue, himg);
                    HZeichneLinie(himg, himg.VertLines.Startline + 1, 0, himg.VertLines.Startline + 1, himg.Img1.Height, Color.Red);  //!!TEST                    
                }
            }
            //Linien
            if ((glAnsichtSingle == 0 && glTestAnsicht1 == 2) ||
                (glAnsichtSingle == 1 && glTTLinien == 1))
            {
                if (glTestAnsicht2 == 2 || glTestAnsicht2 == 4)
                {//Linien horiz
                    ZeichneLines(himg.HorLines.Dark, Color.Red, himg);

                }
                if (glTestAnsicht2 == 3 || glTestAnsicht2 == 4)
                {//Linien vert   
                    ZeichneLines(himg.VertLines.Dark, Color.DarkBlue, himg);
                }
            }
        }

        private void ZeichneLines(List<Lines.sDarkX> darkH, Color col, Himg himg)
        {
            //ZeichneLinie(himg, darkH[0].VertDown[2 - 1], darkH[0].VertDown[2], col);
            // return;

            for (int ii = 0; ii < darkH.Count; ii++)
            {  // for test Linien grüne Linien
                for (int jj = 1; jj < darkH[ii].Glatt.Count; jj++)
                    HZeichneLinie(himg, darkH[ii].Glatt[jj - 1], darkH[ii].Glatt[jj], col);
                /*

                                for (int jj = 1; jj < darkH[ii].VertDown.Count; jj++)
                                    HZeichneLinie(himg,darkH[ii].VertDown[jj - 1], darkH[ii].VertDown[jj], col);

                                for (int jj = 1; jj < darkH[ii].VertUp.Count; jj++)
                                    HZeichneLinie(himg,darkH[ii].VertUp[jj], darkH[ii].VertUp[jj-1], col);
                */
            }
        }

        private void ZeichneKreuze(List<Lines.sDarkX> darkH, Color col, Himg himg)
        {
            for (int ii = 0; ii < darkH.Count; ii++)
            {

                for (int jj = 0; jj < darkH[ii].VertDown.Count; jj++)
                    HSetPixel(himg, darkH[ii].VertDown[jj], col);
                for (int jj = 0; jj < darkH[ii].VertUp.Count; jj++)
                    HSetPixel(himg, darkH[ii].VertUp[jj], col);
            }

        }
        private void HSetPixel(Himg himg, Point PP, Color color)
        {
            himg.Img1.SetPixel(PP.X, PP.Y, color);

            for (int ii = 1; ii <= PunktKreuzGroesse; ii++)
            {
                if ((PP.X + ii < himg.Img1.Width) && (PP.Y + ii < himg.Img1.Height))
                {
                    himg.Img1.SetPixel(PP.X + ii, PP.Y + ii, color);
                }
                if ((PP.X - ii >= 0) && (PP.Y - ii >= 0))
                {
                    himg.Img1.SetPixel(PP.X - ii, PP.Y - ii, color);
                }
                if ((PP.X + ii < himg.Img1.Width) && (PP.Y - ii >= 0))
                {
                    himg.Img1.SetPixel(PP.X + ii, PP.Y - ii, color);
                }
                if ((PP.X - ii >= 0) && (PP.Y + ii < himg.Img1.Height))
                {
                    himg.Img1.SetPixel(PP.X - ii, PP.Y + ii, color);
                }
            }
        }
        ///XXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXxXXXXXXXXXx
        private void HZeichneLinie(Himg himg, Point point1, Point point2, Color col)
        {
            HZeichneLinie(himg, point1.X, point1.Y, point2.X, point2.Y, col);
        }
        private void HZeichneLinie(Himg himg, int Startx, int Starty, int Endx, int Endy, Color col)
        {
            // for (int ii = 1; ii < 20; ii++)  // !! For Test
            //     for (int jj = 1; jj < 20; jj++)
            //         himg.Img1.SetPixel(Startx + ii, Starty + jj, col);

            if (Endy - Starty > Endx - Startx)
                for (int ii = 0; ii < Endy - Starty; ii++)
                    himg.Img1.SetPixel(Startx + ii * (Endx - Startx) / (Endy - Starty), Starty + ii, col);
            else
                for (int ii = 0; ii < Endx - Startx; ii++)
                    himg.Img1.SetPixel(Startx + ii, Starty + ii * (Endy - Starty) / (Endx - Startx), col);
        }
    }
}
