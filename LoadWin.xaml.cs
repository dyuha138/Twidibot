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
using System.Windows.Threading;

namespace Twidibot
{
	public partial class LoadWin : Window
	{
		public event EventHandler<CEvent_Msg> Ev_Msg;

		BackWin TechF = null;

		public LoadWin(BackWin backWin) {
			InitializeComponent();

			TechF = backWin;
			TechF.Ev_InitStatus += StatusUp;
			//Task.Factory.StartNew(() => this.LoadAnimation());
		}

		private void StatusUp(object sender, CEvent_Msg e) {
			this.Dispatcher.Invoke(() => { this.Label.Content = e.Message; });
		}

		private void LoadAnimation() {
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {

			}));
		}
	}
}
