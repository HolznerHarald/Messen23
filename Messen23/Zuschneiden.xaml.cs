using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Messen23
{
    /// <summary>
    /// Interaktionslogik für Zuschneiden.xaml
    /// </summary>
    public partial class Zuschneiden : Window
    {
        MainWindow WM;
        public Zuschneiden()
        {
            InitializeComponent();
            Txt.Text = "1.Zeile\n2.Zeile";
            ButtonSave.IsEnabled = false;
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            WM = Application.Current.MainWindow as MainWindow;
        }

        public void RechteckFertig()
        {
            Txt.Text = "jetzt Speichern möglich";
            ButtonSave.IsEnabled = true;
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            WM.RemoveChildren();
            WM.Zusch = null;
            Close();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            WM.RechteckSpeichern();
            WM.Zusch = null;
            Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WM.RemoveChildren();
            WM.Zusch = null;
        }
    }
}
