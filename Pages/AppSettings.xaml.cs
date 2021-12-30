using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Twidibot.Pages
{

	public partial class AppSettings : Page {
		private BackWin TechF = null;
		private MainWin MainWin = null;
		//public bool openmain = false;
		private bool settingchangeadd = false;
		private bool settingchangework = false;

		private string logintmp = null;
		private string passtmp = null;
		private string channeltmp = null;
		private string chathistorydeltmp = null;
		private string spammsgtmp = null;
		private string spammsgetmp = null;

		private string loginprev = null;
		private string passprev = null;
		private string channelprev = null;
		private string chathistorydelprev = null;
		private string spammsgprev = null;

		public AppSettings(BackWin backWin, MainWin mainWin) {
			this.TechF = backWin;
			this.MainWin = mainWin;
			InitializeComponent();
			//TechF.Chat.useDefBot = false;
			rbChatBot_Def.IsEnabled = false;

			if (TechF.Chat.useDefBot == false) {
				eChatLogin.IsEnabled = true;
				eChatPass.IsEnabled = true;
				rbChatBot_Cust.IsChecked = true;
			}
			if (TechF.Chat.useDefBot == true) {
				eChatLogin.IsEnabled = false;
				eChatPass.IsEnabled = false;
				rbChatBot_Def.IsChecked = true;
				eChatLogin.Text = TechF.TechFuncs.GetSettingParam("Login");
				eChatPass.Password = TechF.TechFuncs.GetSettingParam("Pass");
			}
		}


		// -- Типа защита от спама запросов на сохранение данных в бд --
		private void SettingChange() {
			settingchangework = true;
			this.Dispatcher.Invoke(() => { this.ltst.Content = ""; });
			while (settingchangeadd) {
				settingchangeadd = false;
				for (uint i = 0; i < 10; i++) {
					Thread.Sleep(100);
					if (settingchangeadd) { break; }
				}
			}

			settingchangework = false;
			Task.Factory.StartNew(() => {
				if (channeltmp != null) { // -- Канал
					TechF.db.SettingsT.UpdateSetting("Channel", channeltmp.ToLower());
					channeltmp = null;
					TechF.TechFuncs.LogDH("Изменён параметр \"Channel\"");
				}
				if (logintmp != null) { // -- Логин
					TechF.db.SettingsT.UpdateSetting("Login", logintmp.ToLower());
					logintmp = null;
					TechF.TechFuncs.LogDH("Изменён параметр \"login\"");
				} 
				if (passtmp != null) { // -- OAuth
					TechF.db.SettingsT.UpdateSetting("Pass", passtmp);
					passtmp = null;
					TechF.TechFuncs.LogDH("Изменён параметр \"\"");
				} 
				if (chathistorydeltmp != null) { // -- История чата
					TechF.db.SettingsT.UpdateSetting("ChatHistoryDel", chathistorydeltmp);
					chathistorydeltmp = null;
					TechF.TechFuncs.LogDH("Изменён параметр \"ChatHistoryDel\"");
				}
				if (spammsgtmp != null) { // -- Спам сообщениями
					TechF.db.SettingsT.UpdateSetting("SpamMsg", spammsgtmp);
					spammsgtmp = null;
					TechF.TechFuncs.LogDH("Изменён параметр \"SpamMsg\"");
				}
				if (spammsgetmp != null) {
					TechF.db.SettingsT.UpdateSetting("SpamMsgWork", spammsgetmp);
					spammsgetmp = null;
					TechF.TechFuncs.LogDH("Изменён параметр \"SpamMsgWork\"");
				}
				TechF.db.SettingsT.UpdateListfromTable();

				this.Dispatcher.Invoke(() => { this.ltst.Content = "Изменения сохранены"; });
				Thread.Sleep(3000);
				this.Dispatcher.Invoke(() => { this.ltst.Content = ""; });
			});
		}


		// -- Открытие ссылки на страницу получения OAuth кода
		private void OAuthLink_Click(object sender, RoutedEventArgs e) {
			Process.Start("https://twitchapps.com/tmi/");
		}


		// -- Открытие окна "О программе"
		private void bAbout_Click(object sender, RoutedEventArgs e) {
			MainWin.AboutWin.Show();
		}



		// -- Изменение аккаунта бота (стандартный/свой) --
		private void rbChatBot_TypeChanged(object sender, RoutedEventArgs e) {
			if (rbChatBot_Cust.IsChecked == true) {

				eChatLogin.IsEnabled = true;
				eChatPass.IsEnabled = true;
				TechF.Chat.useDefBot = false;
			}
			if (rbChatBot_Def.IsChecked == true) {
				eChatLogin.IsEnabled = false;
				eChatPass.IsEnabled = false;
				eChatLogin.Text = "";
				eChatPass.Password = "";
				Task.Factory.StartNew(() => {
					TechF.Chat.useDefBot = true;
				});
			}
		}



		// -- Cистема по аналогии с DefCom страницей, только там объединение функций по типу, а тут по принадлежности к полю --
		// -- Поле канала
		private void eChatChannel_PTI(object sender, TextCompositionEventArgs e) {
			if (e.Text.Contains(" ") || Char.IsWhiteSpace(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eChatChannel_PKD(object sender, KeyEventArgs e) {
			channelprev = eChatChannel.Text;

			if (e.Key == Key.Space) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains(" ") || str.Contains("\t") || str.Contains("\n")) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eChatChannel_Changed(object sender, TextChangedEventArgs e) {
			if (eChatChannel.Text.Contains(" ") || eChatChannel.Text.Contains("\t") || eChatChannel.Text.Contains("\n")) {
				eChatChannel.Text = channelprev;
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eChatChannel_PKU(object sender, KeyEventArgs e) {
			if (eChatChannel.Text != channelprev) {
				channelprev = eChatChannel.Text;
				if (eChatChannel.Text != "") {
					channeltmp = eChatChannel.Text;
				} else {
					channeltmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}



		// -- Поле логина  --
		private void eChatLogin_PTI(object sender, TextCompositionEventArgs e) {
			if (e.Text.Contains(" ") || Char.IsWhiteSpace(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eChatLogin_PKD(object sender, KeyEventArgs e) {
			loginprev = eChatLogin.Text;

			if (e.Key == Key.Space) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains(" ") || str.Contains("\t") || str.Contains("\n")) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eChatLogin_Changed(object sender, TextChangedEventArgs e) {
			if (eChatLogin.Text.Contains(" ") || eChatLogin.Text.Contains("\t") || eChatLogin.Text.Contains("\n")) {
				eChatLogin.Text = loginprev;
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eChatLogin_PKU(object sender, KeyEventArgs e) {
			if (eChatLogin.Text != loginprev) {
				loginprev = eChatLogin.Text;
				if (eChatLogin.Text != "") {
					logintmp = eChatLogin.Text;
				} else {
					logintmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}

		// -- Второе поле оатч кода --
		private void eChatPass_PTI(object sender, TextCompositionEventArgs e) {
			if (e.Text.Contains(" ") || Char.IsWhiteSpace(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eChatPass_PKD(object sender, KeyEventArgs e) {
			passprev = eChatPass.Password;

			if (e.Key == Key.Space) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains(" ") || str.Contains("\t") || str.Contains("\n")) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eChatPass_PKU(object sender, KeyEventArgs e) {
			if (eChatPass.Password != passprev) {
				passprev = eChatPass.Password;
				if (eChatPass.Password != "") {
					passtmp = eChatPass.Password;
				} else {
					passtmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}

		// -- Поле оатч кода --
		private void eChatPass2_PTI(object sender, TextCompositionEventArgs e) {
			if (e.Text.Contains(" ") || Char.IsWhiteSpace(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
				eChatPass.Password = eChatPass2.Text;
			}
		}
		private void eChatPass2_PKD(object sender, KeyEventArgs e) {
			eChatPass.Password = eChatPass2.Text;
			passprev = eChatPass.Password;

			if (e.Key == Key.Space) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains(" ") || str.Contains("\t") || str.Contains("\n")) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eChatPass2_PKU(object sender, KeyEventArgs e) {
			eChatPass.Password = eChatPass2.Text;
			if (eChatPass.Password != passprev) {
				passprev = eChatPass.Password;
				if (eChatPass.Password != "") {
					passtmp = eChatPass.Password;
				} else {
					passtmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
			eChatPass.Password = eChatPass2.Text;
		}

		// -- Скрытие/показ оатч кода
		private void bChatPassH_Click(object sender, RoutedEventArgs e) {
			if (eChatPass.Visibility == Visibility.Visible) {
				eChatPass2.Text = eChatPass.Password;
				eChatPass.Visibility = Visibility.Collapsed;
				eChatPass2.Visibility = Visibility.Visible;
				cChatPassH.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#FF292929");
			} else {
				if (eChatPass.Visibility == Visibility.Collapsed) {
					eChatPass2.Visibility = Visibility.Collapsed;
					eChatPass.Visibility = Visibility.Visible;
					cChatPassH.Background = null;
				} else { }
			}
		}



		// -- Изменение периода удаления истории сообщений
		private void rbChatHistory_Changed(object sender, RoutedEventArgs e) {
			string daydel = eChatHistory_CustDel.Text;
			if (rbChatHistory_Del1d.IsChecked == true) {
				chathistorydeltmp = "1d";
				eChatHistory_CustDel.Text = "";
				settingchangeadd = true;
				eChatHistory_CustDel.IsEnabled = false;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
			if (rbChatHistory_Del3d.IsChecked == true) {
				chathistorydeltmp = "3d";
				eChatHistory_CustDel.Text = "";
				settingchangeadd = true;
				eChatHistory_CustDel.IsEnabled = false;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
			if (rbChatHistory_Del1w.IsChecked == true) {
				chathistorydeltmp = "1w";
				eChatHistory_CustDel.Text = "";
				settingchangeadd = true;
				eChatHistory_CustDel.IsEnabled = false;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
			if (rbChatHistory_Del0.IsChecked == true) {
				chathistorydeltmp = "0";
				eChatHistory_CustDel.Text = "";
				settingchangeadd = true;
				eChatHistory_CustDel.IsEnabled = false;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
			if (rbChatHistory_DelCust.IsChecked == true) {
				eChatHistory_CustDel.IsEnabled = true;
			}
			settingchangeadd = true;
		}

		// -- Поле своего варианта периода удаления истории чата
		private void eChatHistory_CustDel_PTI(object sender, TextCompositionEventArgs e) {
			if (!Char.IsDigit(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eChatHistory_CustDel_PKD(object sender, KeyEventArgs e) {
			chathistorydelprev = eChatHistory_CustDel.Text;

			if (e.Key == Key.Space) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (!Char.IsDigit(str, 0)) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eChatHistory_CustDel_Changed(object sender, TextChangedEventArgs e) {
			string str = null;
			for (ushort i = 0; i < eChatHistory_CustDel.Text.Length; i++) {
				str = eChatHistory_CustDel.Text.ElementAt<char>(i).ToString();
				if (!Char.IsDigit(str, 0)) {
					eChatHistory_CustDel.Text = chathistorydelprev;
					e.Handled = true;
					break;
				} else {
					e.Handled = false;
				}
			}
			if (eChatHistory_CustDel.Text != "") {
				lChatHistory_Custday.Content = TechF.TechFuncs.WordEndNumber(Convert.ToInt32(eChatHistory_CustDel.Text), "D");
			}
		}
		private void eChatHistory_CustDel_PKU(object sender, KeyEventArgs e) {
			if (eChatHistory_CustDel.Text != chathistorydelprev) {
				chathistorydelprev = eChatHistory_CustDel.Text;
				if (eChatHistory_CustDel.Text != "") {
					chathistorydeltmp = eChatHistory_CustDel.Text;
				} else {
					chathistorydeltmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}




		// -- Изменение типа спама сообщений --
		private void rbSpamMessages_Changed(object sender, RoutedEventArgs e) {
			/*if (rbSpamMessages_0.IsChecked == true) {
				spammsgtypetmp = "0";
				eSpamMessages_Cust.Text = "";
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}*/
			if (rbSpamMessages_1.IsChecked == true) {
				spammsgtmp = "1";
				eSpamMessages_Cust.Text = "";
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}

		// -- Включение/отключение спама сообщений
		private void cbSpamMessages_E_Click(object sender, RoutedEventArgs e) {
			if (cbSpamMessages_E.IsChecked == true) {
				spammsgetmp = "true";
			}
			if (cbSpamMessages_E.IsChecked == false) {
				spammsgetmp = "false";
			}
			settingchangeadd = true;
			if (!settingchangework) {
				Task.Factory.StartNew(() => { SettingChange(); });
			}
		}

		// -- Поле своего варианта периода удаления истории чата
		private void eSpamMessages_Cust_PTI(object sender, TextCompositionEventArgs e) {
			if (!Char.IsDigit(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eSpamMessages_Cust_PKD(object sender, KeyEventArgs e) {
			spammsgprev = eSpamMessages_Cust.Text;

			if (e.Key == Key.Space) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (!Char.IsDigit(str, 0)) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eSpamMessages_Cust_Changed(object sender, TextChangedEventArgs e) {
			string str = null;
			for (ushort i = 0; i < eSpamMessages_Cust.Text.Length; i++) {
				str = eSpamMessages_Cust.Text.ElementAt<char>(i).ToString();
				if (!Char.IsDigit(str, 0)) {
					eSpamMessages_Cust.Text = spammsgprev;
					e.Handled = true;
					break;
				} else {
					e.Handled = false;
				}
			}
			if (eChatHistory_CustDel.Text != "") {
				lChatHistory_Custday.Content = TechF.TechFuncs.WordEndNumber(Convert.ToInt32(eChatHistory_CustDel.Text), "D");
			}
		}
		private void eSpamMessages_Cust_PKU(object sender, KeyEventArgs e) {
			if (eSpamMessages_Cust.Text != spammsgprev) {
				spammsgprev = eSpamMessages_Cust.Text;
				if (eSpamMessages_Cust.Text != "") {
					spammsgtmp = eSpamMessages_Cust.Text;
				} else {
					spammsgtmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}

		private void bWebBrowser_Click(object sender, RoutedEventArgs e) {
			TechF.WebBrowserWin.Show();
			//this.Dispatcher.Invoke(() => { TechF.WebBrowserWin.Web.Source = new Uri(TechF.TechFuncs.GetSourcetoLogin()); });
		}
	}
}
