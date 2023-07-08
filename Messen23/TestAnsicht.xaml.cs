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
    /// Interaktionslogik für TestAnsicht.xaml
    /// </summary>
    public partial class TestAnsicht : Window
    {
        private TestenReg Teste;
        internal MainWindow MW = Application.Current.MainWindow as MainWindow;
        internal TestAnsicht(TestenReg testen)
        {
            InitializeComponent();
            Teste = testen;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void single_Checked(object sender, RoutedEventArgs e)
        {
            Teste.glAnsichtSingle = 0;
            this.Init();
        }
        private void multi_Checked(object sender, RoutedEventArgs e)
        {
            Teste.glAnsichtSingle = 1;
            this.Init();
        }

        internal void Init()
        {
            if (Teste.glAnsichtSingle == 0)
            {
                single.IsChecked = true;
                multicheck.Visibility = Visibility.Hidden;
                singlecheck.Visibility = Visibility.Visible;
            }
            else
            {
                multi.IsChecked = true;
                multicheck.Visibility = Visibility.Visible;
                singlecheck.Visibility = Visibility.Hidden;
            }
            if (Teste.glTestAnsicht1 == 1)
                RB11.IsChecked = true;
            else if (Teste.glTestAnsicht1 == 2)
                RB12.IsChecked = true;
            else
                RB13.IsChecked = true;

            if (Teste.glTestAnsicht2 == 1)
                RB21.IsChecked = true;
            else if (Teste.glTestAnsicht2 == 2)
                RB22.IsChecked = true;
            else if (Teste.glTestAnsicht2 == 3)
                RB23.IsChecked = true;
            else
                RB24.IsChecked = true;

            CH12.IsChecked = false;
            CH13.IsChecked = false;
            if (Teste.glTTKreuz == 1)
                CH13.IsChecked = true;
            if (Teste.glTTLinien == 0)
                CH12.IsChecked = true;
        }
    }
}
