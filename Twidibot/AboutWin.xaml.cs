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
using System.Windows.Threading;

namespace Twidibot
{
	public partial class AboutWin : Window
	{
		private BackWin TechF = null;
		private int Easter_clicks = 0;
		private bool Easter_check = false;

		public AboutWin(BackWin backWin) {
			InitializeComponent();
			this.TechF = backWin;
			this.lAppVersion.Content = "Версия: " + TechF.TechFuncs.GetSettingParam("Version");
		}

		private void Easter_Click(object sender, RoutedEventArgs e) {
			if (Easter_check) {
				if (Easter_clicks > 4 || Easter_clicks == 0) {
					Easter_clicks = 1;
					imgLogo.Source = new BitmapImage(new Uri("\\media/img/logodef2.png", UriKind.Relative));
				} else {
					if (Easter_clicks == 1) {
						Easter_clicks = 0;
						imgLogo.Source = new BitmapImage(new Uri("\\media/img/logoz.png", UriKind.Relative));
					} else {
						imgLogo.Source = new BitmapImage(new Uri("\\media/img/logodef2.png", UriKind.Relative));
					}
				}
			} else {
				Easter_clicks++;
				if (Easter_clicks > 4) {
					Easter_check = true;
					imgLogo.Source = new BitmapImage(new Uri("\\media/img/logoz.png", UriKind.Relative));
					MessageBox.Show("Вы нашли пасхалку! Это изначальная версия логотипа, которая была выполнена самим ZerograviTea. А стандартное лого - это моя адаптация под флэт-дизайн", "Пасхал_очка");
				}
			}
		}

		private void VisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if (e.NewValue as bool? == false) {
				TechF.MainWin.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => {
					TechF.MainWin.AboutWin = null;
				}));
			}
		}
	}
}
