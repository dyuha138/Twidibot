using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//using CefSharp;

namespace Twidibot
{
	public partial class WebBrowserWin : Window
	{
		private BackWin TechF = null;

		public WebBrowserWin(BackWin backWin) {
			InitializeComponent();
			this.TechF = backWin;
		}
	}
}
