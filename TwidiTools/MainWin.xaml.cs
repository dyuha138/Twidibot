using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TwidiTools
{
	public partial class MainWin : Window
	{
		LogAnalyzer LogAnalyzerWin = new LogAnalyzer();
		DB DBWin = new DB();

		public MainWin() {
			InitializeComponent();
			LogAnalyzerWin.Closed += LogAnalyzerWin_Close;
			DBWin.Closed += DBWin_Close;
		}

		private void LogAnalyzerWin_Close(object sender, EventArgs e) {
			LogAnalyzerWin = null;
		}
		private void DBWin_Close(object sender, EventArgs e) {
			DBWin = null;
		}

		private void bLog_Click(object sender, RoutedEventArgs e) {
			if (LogAnalyzerWin == null) {
				LogAnalyzerWin = new LogAnalyzer();
				LogAnalyzerWin.Closed += LogAnalyzerWin_Close;
			}
			LogAnalyzerWin.Show();
		}

		private void bdb_Click(object sender, RoutedEventArgs e) {
			if (DBWin == null) {
				DBWin = new DB();
				DBWin.Closed += DBWin_Close;
			}
			DBWin.Show();
		}
	}
}
