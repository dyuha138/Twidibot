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
		private Pages.DefCom PageDefCom;
		private Pages.FuncCom PageFuncCom;
		private Pages.SpamMsg PageSpamMsg;
		private MainWin MainWin;

		public Commands(BackWin backWin, MainWin mainWin) {
			InitializeComponent();
			TechF = backWin;
			this.MainWin = mainWin;
			this.PageDefCom = mainWin.PageDefCom;
			this.PageFuncCom = mainWin.PageFuncCom;
			this.PageSpamMsg = mainWin.PageSpamMsg;
		}

		private void bMenu_Spam_Click(object sender, RoutedEventArgs e) {
			this.Dispatcher.Invoke(() => { this.FrameV.Content = PageSpamMsg; });
			MainWin.Title = "Twidibot - Настройка переодических сообщений";
			this.bMenuSpam.IsEnabled = false;
			this.bMenuDef.IsEnabled = true;
			this.bMenuFunc.IsEnabled = true;
		}

		private void bMenu_Def_Click(object sender, RoutedEventArgs e) {
			this.Dispatcher.Invoke(() => { this.FrameV.Content = PageDefCom; });
			MainWin.Title = "Twidibot - Настройка обычных команд";
			this.bMenuSpam.IsEnabled = true;
			this.bMenuDef.IsEnabled = false;
			this.bMenuFunc.IsEnabled = true;
		}

		private void bMenu_Func_Click(object sender, RoutedEventArgs e) {
			this.Dispatcher.Invoke(() => { this.FrameV.Content = PageFuncCom; });
			MainWin.Title = "Twidibot - Настройка встроенных команд";
			this.bMenuSpam.IsEnabled = true;
			this.bMenuDef.IsEnabled = true;
			this.bMenuFunc.IsEnabled = false;
		}
	}
}
