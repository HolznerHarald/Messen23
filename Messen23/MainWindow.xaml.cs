// <Canvas x:Name="CC" MouseDown="imgName1_MouseDown" >
// F12? alt shift
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;

namespace Messen23
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal int AktivObj = 0;
        private Himg glimg1;       
        internal Kalibr kal;
        internal MessObj messO;
        internal Auswertung Ausw;
        private readonly MainWindow MW;
        internal BitmapImage glimage1;
        //private Bitmap glBitmap;

        internal Zuschneiden Zusch;
        internal TestenReg teste;
       //  internal string glAktFileName;

        private bool glZuschMaus = false;
        private System.Windows.Point CurrentPos;
        private Line glline1;
        private Line glline2;
        private Line glline3;
        private Line glline4;
        private int glZoomFaktor;

        private string glAktFileName2;
        private int Imagemode = 1;
        private System.Windows.Point glMouseStart;
        private System.Windows.Point glGGMouseStart;
        private System.Windows.Point glMouseStartImg;
  
        public MainWindow()
        {          
            InitializeComponent();
            KalE.IsEnabled = false;
            KalS.IsEnabled = false;            
            KalZ.IsEnabled = false;
            MarkBeginn.IsEnabled = false;
            MarkSave.IsEnabled = false;
            MW = System.Windows.Application.Current.MainWindow as MainWindow;
            teste = new TestenReg(MW);
            //Testliste.Items.Add(glAktFileName);
            //  FileLaden(glAktFileName);

            Imagemode = 1;
            Scroll1.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            Scroll1.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }
        private void KalOpenFoto_Click(object sender, RoutedEventArgs e)
        {
            AktivObj = tools.KalAktiv;
            kal = new Kalibr(MW);
            imgName1.Source = kal.Openfoto();
        }
        private void Bearbeiten_Click(object sender, RoutedEventArgs e)
        {
            AktivObj = tools.KalAktiv;
            using (var memory = new MemoryStream())
            {

                glimg1 = new Himg(kal.kBitmap, MW);
                if (glimg1.Findlines())
                {
                    //kal = new Kalibr(MW, glimg1.S1xy, kal.kBitmap.Width, kal.kBitmap.Height);
                    kal.Height = kal.kBitmap.Height;
                    kal.Wiidth = kal.kBitmap.Width;
                    kal.S1x = glimg1.S1xy;
                    kal.StatusKalib = kal.bearbeitet;
                }
                else
                    kal.StatusKalib = kal.geladen;
                
                MemoryLaden(kal.kBitmap);
            }
        }
        private void KalS_Click(object sender, RoutedEventArgs e)
        {
            AktivObj = tools.KalAktiv;
            kal.Save();
        }
        private void KalO_Click(object sender, RoutedEventArgs e)
        {
            AktivObj = tools.KalAktiv;
            Bitmap lBitmap;
            kal = new Kalibr(MW);
            if (kal.Laden())
            {
                kal.StatusKalib = kal.geöffnet;
                lBitmap = kal.Zeigen();            
                MemoryLaden(lBitmap);                
            }
            else
                kal.StatusKalib = kal.ungeladen;
        }
        private void KalZ_Click(object sender, RoutedEventArgs e)
        {
            AktivObj = tools.KalAktiv;
            Bitmap lBitmap;
            if (kal != null)
            {
                lBitmap = kal.Zeigen();
                MemoryLaden(lBitmap);                
            }
        }
        private void ObjektOpen_Click(object sender, RoutedEventArgs e)
        {
            AktivObj = tools.MessAktiv;
            messO = new MessObj(MW);
            imgName1.Source = messO.Openfoto();
        }
        private void ObjektMBeginn_Click(object sender, RoutedEventArgs e)
        {
            AktivObj = tools.MessAktiv;
            messO.statusMess = messO.MarkierungBegonnen;
        }
        private void ObjektMSave_Click(object sender, RoutedEventArgs e)
        {
            AktivObj = tools.MessAktiv;
            if(messO.MarkPs.Count > 0) 
            {
                messO.statusMess = messO.MarkierungFertig;
            }
            else 
            {
                System.Windows.MessageBox.Show("Mindestens eine Markierung notwendig");
            }
        }


        private void ImageAnsicht_Click(object sender, RoutedEventArgs e)
        {
            //!!H  Zuerst imgName ohne Scrollview für Option 1 , dann .sabled 
            ImageOption ImOp = new ImageOption();
            if (Imagemode == 1)
                ImOp.RB1.IsChecked = true;
            else if (Imagemode == 2)
                ImOp.RB2.IsChecked = true;
            else if (Imagemode == 3)
                ImOp.RB3.IsChecked = true;

            bool? Res = ImOp.ShowDialog();
            if (Res == false)
                return;
            if (ImOp.RB1.IsChecked == true)
            {
                Imagemode = 1;
                Scroll1.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                Scroll1.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
            else if (ImOp.RB2.IsChecked == true)
            {
                Imagemode = 2;
                Scroll1.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                Scroll1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            else if (ImOp.RB3.IsChecked == true)
            {
                Imagemode = 3;
                Scroll1.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                Scroll1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        }
        private void TestAnsicht_Click(object sender, RoutedEventArgs e)
        {
            teste.AnsichtClick();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            teste.closing();
            if (Zusch != null)
                Zusch.Close();
        }

        //XXXXXXXXXXXXXXXX Subroutines
        private void MemoryLaden(Bitmap bm)
        {
            using (var memory = new MemoryStream())
            {
                bm.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                //var bitmapImage = new BitmapImage();
                glimage1 = new BitmapImage();
                glimage1.BeginInit();
                glimage1.StreamSource = memory;
                glimage1.CacheOption = BitmapCacheOption.OnLoad; // this is key to destory the image with a new image coming in.
                glimage1.EndInit();
                imgName1.Source = glimage1;
            }
        }
        internal void RemoveChildren()
        {
            GG.Children.Remove(glline1);
            GG.Children.Remove(glline2);
            GG.Children.Remove(glline3);
            GG.Children.Remove(glline4);
        }
        internal void RechteckSpeichern()
        {
            /*
            double currImgX = CurrentPos.X - (glMouseStart.X - glMouseStartImg.X);
            currImgX = currImgX * glBitmap.Width / imgName1.ActualWidth;
            double currImgY = CurrentPos.Y - (glMouseStart.Y - glMouseStartImg.Y);
            currImgY = currImgY * glBitmap.Height / imgName1.ActualHeight;
            double BereichW = (CurrentPos.X - glMouseStart.X);
            double BereichH = (CurrentPos.Y - glMouseStart.Y);
            int AdaptW = (int)(BereichW * glBitmap.Width / imgName1.ActualWidth);
            int AdaptH = (int)(BereichH * glBitmap.Height / imgName1.ActualHeight);
            int AdaptImgStartX = (int)(glMouseStartImg.X * glBitmap.Width / imgName1.ActualWidth);
            int AdaptImgStartY = (int)(glMouseStartImg.Y * glBitmap.Height / imgName1.ActualHeight);
            MemoryStream memory = CropAndResizeImage(glBitmap, AdaptW, AdaptH, AdaptImgStartX, AdaptImgStartY, (int)currImgX, (int)currImgY, ImageFormat.Jpeg);

            glBitmap.Dispose();  //!!H unbedingt notwendig , oder auch Loadoptions?
            File.Delete(glAktFileName);
            memory.Position = 0;
            glBitmap = new Bitmap(memory);
            glBitmap.Save(glAktFileName);
            MemoryLaden(memory);
            */
        }      
        private void Zuschneiden_Click(object sender, RoutedEventArgs e)
        {
            Zusch = new Zuschneiden();
            Zusch.Show();
        }
    
        // Beginn Maus
       /* private void GG_PreviewMouseDown(object sender, MouseButtonEventArgs e)  //!!H   
        {
            // Prev liefert anderen Wert (26 mehr !!!) als  glMouseStart = Mouse.GetPosition(GG); 
            glMouseStart = e.GetPosition(this);
            Testliste.Items.Add("GGPrevMousexy" + glMouseStart.X + " " + glMouseStart.Y);
        }*/
        private void imgName1_MouseDownLeft(object sender, MouseButtonEventArgs e)
        {
            if (AktivObj == tools.MessAktiv && messO.statusMess == messO.MarkierungBegonnen  )               
            {
                messO.Mousedown();
                MemoryLaden(messO.mBitmap);                
            }
            else if (Zusch != null)    
            {
                glZuschMaus = true;
                glMouseStart = Mouse.GetPosition(GG); // weil Children nur für Grid
                glMouseStartImg = Mouse.GetPosition(imgName1);
                Testliste.Items.Add("GetpositionGG:" + glMouseStart.X + " " + glMouseStart.Y);
                Testliste.Items.Add("GetpositionimgName1:" + glMouseStartImg.X + " " + glMouseStartImg.Y);
            }

        }
        private void File_Language_Click(object sender, MouseButtonEventArgs e)
        {
        }
        private void imgName1_MouseDownRight(object sender, MouseButtonEventArgs e)
        {
            
            if (AktivObj == tools.MessAktiv && messO.statusMess == messO.MarkierungBegonnen)
            {                
                messO.lastMouseStart = Mouse.GetPosition(MW.imgName1);

                ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
                cm.Items.Clear();
                MenuItem item1 = new MenuItem();
                item1.Header = "Letzten Messpunkt entfernen";                
                item1.Click += new RoutedEventHandler(ContLösch_Click);
                cm.Items.Add(item1);
                MenuItem item2 = new MenuItem();
                item2.Header = "Nähesten Messpunkt entfernen";
                item2.IsEnabled = true;
                item2.Click += new RoutedEventHandler(NaheLösch_Click);
                cm.Items.Add(item2);
                MenuItem item3 = new MenuItem();
                item3.Header = "Letzten gelöschten Punkt wieder dazu";
                item3.IsEnabled = true;
                item3.Click += new RoutedEventHandler(Gelöschtdazu_Click);
                cm.Items.Add(item3);
                if (messO.MarkPs.Count > 0)
                {
                    item1.IsEnabled = true;
                    item2.IsEnabled = true;
                }
                else
                {
                    item1.IsEnabled = false;
                    item2.IsEnabled = false;
                }
                if (messO.LetzterGelöschterP.X==-1000)
                    item3.IsEnabled = false;
                else
                    item3.IsEnabled = true;

                cm.IsOpen = true;                
            }           
        }
        private void ImgName1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Zusch == null)
                return;
            if (glZuschMaus)
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    glZuschMaus = false;
                    Zusch.RechteckFertig();
                }
                else
                {
                    // weil Children nur für Grid
                    CurrentPos = Mouse.GetPosition(GG);
                    //Testliste.Items.Add("GetpositionGG:" + glMouseStart.X + " " + glMouseStart.Y);

                    if (glline1 != null)
                    {
                        RemoveChildren();
                    }
                    glline1 = new Line();
                    glline2 = new Line();
                    glline3 = new Line();
                    glline4 = new Line();
                    glline1.Stroke = System.Windows.Media.Brushes.Red;
                    glline1.X1 = glMouseStart.X;
                    glline1.Y1 = glMouseStart.Y;
                    glline1.X2 = CurrentPos.X;
                    glline1.Y2 = glMouseStart.Y;
                    glline2.Stroke = System.Windows.SystemColors.WindowFrameBrush;
                    glline2.X1 = CurrentPos.X;
                    glline2.Y1 = glMouseStart.Y;
                    glline2.X2 = CurrentPos.X;
                    glline2.Y2 = CurrentPos.Y;
                    glline3.Stroke = System.Windows.SystemColors.WindowFrameBrush;
                    glline3.X1 = CurrentPos.X;
                    glline3.Y1 = CurrentPos.Y;
                    glline3.X2 = glMouseStart.X;
                    glline3.Y2 = CurrentPos.Y;
                    glline4.Stroke = System.Windows.SystemColors.WindowFrameBrush;
                    glline4.X1 = glMouseStart.X;
                    glline4.Y1 = CurrentPos.Y;
                    glline4.X2 = glMouseStart.X;
                    glline4.Y2 = glMouseStart.Y;

                    GG.Children.Add(glline1);
                    GG.Children.Add(glline2);
                    GG.Children.Add(glline3);
                    GG.Children.Add(glline4);
                }
            }
        }
        
        //XXXXXXXXXXXXXXXX  Test Menu
        private void MenuItemT1_Click(object sender, RoutedEventArgs e)
        {
            Testliste.Items.Add("datei - Width: " + glimage1.PixelWidth.ToString());
            Testliste.Items.Add("datei - Heigth: " + glimage1.PixelHeight.ToString());
            Testliste.Items.Add("GGWH:" + GG.ActualWidth.ToString() + " " + GG.ActualHeight.ToString());
            Testliste.Items.Add("ImWH:" + imgName.ActualWidth.ToString() + " " + imgName.ActualHeight.ToString());
            Testliste.Items.Add("Im1WH:" + imgName1.ActualWidth.ToString() + " " + imgName1.ActualHeight.ToString());
        }
        private void MenuItemT2_Click(object sender, RoutedEventArgs e)
        {
            Scroll1.Height = 300;
            Scroll1.Width = 300;
        }
        private void MenuItemT3_Click(object sender, RoutedEventArgs e)
        {
            Registry.CurrentUser.DeleteSubKey(TestenReg.subkey); //!! Test            
        }
        private void MenuItemT4_Click(object sender, RoutedEventArgs e)
        {
        /*   System.Drawing.Image DrawImg = glBitmap;

            DrawImg = ResizeImage(DrawImg, 500);
            DrawImg.Save(glAktFileName2);*/
        }
        private static System.Drawing.Image ResizeImage(System.Drawing.Image Dimage, int newWidth)
        {
            int width = newWidth;
            int height = newWidth;
            double factor = Dimage.Width / (double)Dimage.Height;
            if (factor > 1)
                height = (int)(height / factor); // Querformat
            else
                width = (int)(width * factor); // Hochformat

            return Dimage.GetThumbnailImage(width, height, null, IntPtr.Zero);//!!H
        }
        private void MenuItemT5_Click(object sender, RoutedEventArgs e)
        {
            // in diesem Fall ganze Bitmap auschnneiden und auf 1/3 großes kopieren
            //MemoryStream memory = CropAndResizeImage(glBitmap, glBitmap.Width/3, glBitmap.Height/3, 0, 0, glBitmap.Width, glBitmap.Height, ImageFormat.Jpeg);

            // in diesem Fall 1/10 von Bitmap auschnneiden und auf 1/10 großes kopieren
            //MemoryStream memory = CropAndResizeImage(glBitmap, glBitmap.Width / 10, glBitmap.Height / 10, 0, 0, glBitmap.Width/10, glBitmap.Height/10, ImageFormat.Jpeg);

            // in diesem Fall 1/10 von Bitmap auschnneiden und auf 1/2 großes kopieren
        //    MemoryStream memory = CropAndResizeImage(glBitmap, glBitmap.Width / 2, glBitmap.Height / 2, 0, 0, glBitmap.Width / 10, glBitmap.Height / 10, ImageFormat.Jpeg);

          //  MemoryLaden(memory);
            Testliste.Items.Add("glimage1 - Width: " + glimage1.PixelWidth.ToString());
            Testliste.Items.Add("glimage1 - Heigth: " + glimage1.PixelHeight.ToString());
        }
        private MemoryStream CropAndResizeImage(Bitmap bmp, int targetWidth, int targetHeight, int x1, int y1, int x2, int y2, ImageFormat imageFormat)
        {
            var bmp1 = new Bitmap(targetWidth, targetHeight);
            Graphics g = Graphics.FromImage(bmp1);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            int width = x2 - x1;
            int height = y2 - y1;

            g.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, targetWidth, targetHeight), x1, y1, width, height, GraphicsUnit.Pixel);

            var memStream = new MemoryStream();
            bmp1.Save(memStream, imageFormat);
            return memStream;
        }
        private void MenuItemT6_Click(object sender, RoutedEventArgs e)
        {
  /*     Graphics GG = Graphics.FromImage(glBitmap);
            System.Drawing.Pen blackPen = new System.Drawing.Pen(System.Drawing.Color.Black, 30);
            GG.DrawLine(blackPen, 0, 0, 1000, 1000);
            glAktFileName = @"C:\C#\Messen Foto\k1.jpg";
            glBitmap.Save(glAktFileName);
            FileLaden(glAktFileName);*/
        }
        private void MenuItemT7_Click(object sender, RoutedEventArgs e)
        {
        /*    Bitmap myBitmap = new Bitmap(glAktFileName);
            Bitmap myBitmap2 = new Bitmap(100, 100);

            //System.Drawing.Rectangle expansionRectangle = new System.Drawing.Rectangle(135, 10,
            //   myBitmap.Width, myBitmap.Height);
            // System.Drawing.Rectangle compressionRectangle = new System.Drawing.Rectangle(0, 0,
            //    myBitmap.Width / 10, myBitmap.Height / 10);
            System.Drawing.Rectangle compressionRectangle = new System.Drawing.Rectangle(0, 0,
                100, 100);

            Graphics myGraphics = Graphics.FromImage(myBitmap2);
            myGraphics.DrawImage(myBitmap, 0, 0);
            //myGraphics.DrawImage(myBitmap, expansionRectangle);
            myGraphics.DrawImage(myBitmap, compressionRectangle);

            if (File.Exists(glAktFileName2))
                File.Delete(glAktFileName2);
            myBitmap2.Save(glAktFileName2);*/
        }
        private void MenuItemT8_Click(object sender, RoutedEventArgs e)
        {

            // We may have already set the LayoutTransform to a ScaleTransform.
            // If not, do so now.

            var scaler = Doc.LayoutTransform as ScaleTransform;

            if (scaler == null)
            {
                scaler = new ScaleTransform(1.0, 1.0);
                Doc.LayoutTransform = scaler;
            }

            // We'll need a DoubleAnimation object to drive
            // the ScaleX and ScaleY properties.

            DoubleAnimation animator = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(600)),
            };

            // Toggle the scale between 1.0 and 1.5.

            if (scaler.ScaleX == 1.0)
            {
                animator.To = 1.5;
            }
            else
            {
                animator.To = 1.0;
            }

            scaler.BeginAnimation(ScaleTransform.ScaleXProperty, animator);
            scaler.BeginAnimation(ScaleTransform.ScaleYProperty, animator);
        }
        private void MenuItemT21_Click(object sender, RoutedEventArgs e)
        {   //!!!H

     /*       // Datei muss exisiteren für diesen Test
            string fname = @"C:\C#\C# messen  ALT\C# messen\Temp1.jpg";
            FileLaden(fname);
            /*string fname = @"C:\C#\C# messen  ALT\C# messen\Temp1.jpg";
            var bm1 = new BitmapImage();
            Uri source = new Uri(fname);
            bm1.BeginInit();
            bm1.UriSource = source;
            bm1.CacheOption = BitmapCacheOption.OnLoad;
            bm1.EndInit();
            imgName1.Source = bm1;
            glBitmap = new Bitmap(fname);
            glBitmap.Dispose();  //!!H unbedingt notwendig , oder auch Loadoptions?
            glAktFileName = fname;
            File.Delete(fname);*/
        }
        private void MenuItemT22_Click(object sender, RoutedEventArgs e)
        {  
        }
        private void MenuItemT23_Click(object sender, RoutedEventArgs e)
        {
            Scroll1.Visibility = Visibility.Visible;
        }
        private void MenuItemT24_Click(object sender, RoutedEventArgs e)
        {
            Scroll1.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            Scroll1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }
        private void MenuItemT25_Click(object sender, RoutedEventArgs e)
        {           
            glimg1 = new Himg(kal.kBitmap, MW);
            glimg1.Graph_Start();                               
            MemoryLaden(kal.kBitmap);
         
            glimg1.Graph_Start();
        }
        private void MenuItemT26_Click(object sender, RoutedEventArgs e)
        {
            /*
            Testliste.Items.Add("lgBitmap:" + glBitmap.Width + " " + glBitmap.Height);
            glBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);//!!H
            Testliste.Items.Add("lgBitmap:" + glBitmap.Width + " " + glBitmap.Height);
 */
        }

        private void ContLösch_Click(object sender, RoutedEventArgs e)
        {
            messO.letztesLoeschen();
            MemoryLaden(messO.mBitmap);
        }
        private void NaheLösch_Click(object sender, RoutedEventArgs e)
        {
            messO.NaheLösch();
            MemoryLaden(messO.mBitmap);
        }
        private void Gelöschtdazu_Click(object sender, RoutedEventArgs e)
        {
            messO.GelöschtDazu();
            MemoryLaden(messO.mBitmap);
        }

        private void Auswerten_Click(object sender, RoutedEventArgs e)
        {
            if (messO.statusMess == messO.MarkierungFertig &&
               kal.StatusKalib >= kal.bearbeitet)
            {
                Ausw = new Auswertung(kal,messO,MW);
                Ausw.Starten();
            }
            else
                MessageBox.Show("Error1:Status von Kalbrierung,Markierung passt nicht!!!");

        }
    }
}
