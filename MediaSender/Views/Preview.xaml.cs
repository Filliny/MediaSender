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
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Preview : Window
	{
		public Preview(Uri source)
		{
			InitializeComponent();
			PreviewFrame.Source  = source;
			PreviewFrame.Stretch = Stretch.Uniform;
		}

		private void PreviewFrame_OnMouseDown(object sender, MouseButtonEventArgs e) {
			
			this.Close();
		}


		
    }
}
