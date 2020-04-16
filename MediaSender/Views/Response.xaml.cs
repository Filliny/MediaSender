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

namespace MediaSender
{
    /// <summary>
    /// Interaction logic for Response.xaml
    /// </summary>
    public partial class Response : Window
    {
        public string RCode { get; set; }
        public Response()
        {
            InitializeComponent();
            RCode = "Code here";
        }

        private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
 
                ((TextBox)sender).SelectAll();

        }

        private void Code_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && OkButtn.IsEnabled && CodeTextBox.Text != "")
            {

                this.Close();
            }
        }

        private void OkButtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (OkButtn.IsEnabled && CodeTextBox.Text != "")
            {

                this.Close();
            }
        }
    }
}
