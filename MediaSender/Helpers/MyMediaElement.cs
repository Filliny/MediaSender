using System;
using System.Windows.Controls;

namespace MediaSender.Helpers
{
	public class MyMediaElement : Image
	{

		public bool IsSelected { get; set; }
		public Uri SourceUri { get; set; }
	}
}
