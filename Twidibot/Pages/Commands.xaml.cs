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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Twidibot.Pages
{
	public partial class Commands : Page
	{
		private BackWin TechF = null;

		public Commands(BackWin backWin) {
			InitializeComponent();
			TechF = backWin;
		}

		private void bMenu_Spam_Click(object sender, RoutedEventArgs e) {
			this.Dispatcher.Invoke(() => { this.FrameV.Content = TechF.MainWin.PageSpamMsg; });
			TechF.MainWin.Title = "Twidibot - Настройка переодических сообщений";
			this.bMenu_Spam.IsEnabled = false;
			this.bMenu_Def.IsEnabled = true;
			this.bMenu_Func.IsEnabled = true;
		}

		private void bMenu_Def_Click(object sender, RoutedEventArgs e) {
			this.Dispatcher.Invoke(() => { this.FrameV.Content = TechF.MainWin.PageDefCom; });
			TechF.MainWin.Title = "Twidibot - Настройка обычных команд";
			this.bMenu_Spam.IsEnabled = true;
			this.bMenu_Def.IsEnabled = false;
			this.bMenu_Func.IsEnabled = true;
		}

		private void bMenu_Func_Click(object sender, RoutedEventArgs e) {
			this.Dispatcher.Invoke(() => { this.FrameV.Content = TechF.MainWin.PageFuncCom; });
			TechF.MainWin.Title = "Twidibot - Настройка встроенных команд";
			this.bMenu_Spam.IsEnabled = true;
			this.bMenu_Def.IsEnabled = true;
			this.bMenu_Func.IsEnabled = false;
		}
	}
}
