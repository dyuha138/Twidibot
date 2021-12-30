using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Threading;

namespace Twidibot.Pages {
	public class ListWrapC {
		public String Text { get; set; }
	}
	public partial class Status : Page {
		BackWin TechF = null;
		public bool ToolTipShow = false;

		public Status(BackWin backWin) {
			TechF = backWin;
			InitializeComponent();

			this.lChat.Items.Clear();

			TechF.Chat.Ev_Error += Error_Set;
			TechF.Chat.Ev_ChatMsg += lChat_Add;
			TechF.Chat.Ev_BotMsg += lChat_Add;
			TechF.Chat.Ev_Status += Status_Set;
		}

			// -- Вывод ошибки чата --
		private void Error_Set(object sender, CEvent_Msg e) {
			this.Dispatcher.Invoke(() => { this.lError.Text = e.Message; });
		}

		// -- Статус подключения --
		private void Status_Set(object sender, CEvent_ChatStatus e) {
			this.Dispatcher.Invoke(() => { this.lStatus.Content = e.Msg; });
			if (e.Msg == "Отключено") {
				this.Dispatcher.Invoke(() => { bChatStart.IsEnabled = true; });
				this.Dispatcher.Invoke(() => { bChatStop.IsEnabled = false; });
			}
			if (e.Msg == "Подключено") {
				this.Dispatcher.Invoke(() => { bChatStart.IsEnabled = false; });
				this.Dispatcher.Invoke(() => { bChatStop.IsEnabled = true; });
			}
		}
		private void Status_Set(object sender, CEvent_Msg e) {
			this.Dispatcher.Invoke(() => { this.lStatus.Content = e.Message; });
		}

		// -- Приём сообщения --
		private void lChat_Add(object sender, CEvent_ChatMsg e) {
			if (!TechF.ChatHistoryListLock) {
				this.Dispatcher.Invoke(() => { lChat.Items.Add(new ListWrapC() { Text = "(" + e.Time + ") " + e.Nick + ": " + e.Msg }); });
				this.Dispatcher.Invoke(() => { lChat.ScrollIntoView(lChat.Items[lChat.Items.Count - 1]); }); // -- Лаконичная строчка, которая пролистывает чат в самый низ
			}
		}

		// -- Запуск бота в чат --
		private void bChatStart_Click(object sender, RoutedEventArgs e) {
			bChatStart.IsEnabled = false;
			Task.Factory.StartNew(() => TechF.Chat.Connect());
		}

		// -- Остановка бота --
		private void bChatStop_Click(object sender, RoutedEventArgs e) {
			bChatStop.IsEnabled = false;
			Task.Factory.StartNew(() => TechF.Chat.Close());
		}

		// -- Отправка сообщения от лица бота в чат --
		private void bChatMsgSend(object sender, RoutedEventArgs e) {
			string str = this.eChatMsgSend.Text;
			Task.Factory.StartNew(() => {
				TechF.Chat.SendMsg(str);
			});
			this.eChatMsgSend.Text = "";
		}
		private void bChatMsgSend(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				string str = this.eChatMsgSend.Text;
				Task.Factory.StartNew(() => {
					TechF.Chat.SendMsg(str);
				});
				this.eChatMsgSend.Text = "";
			}
		}

		private void btstr_Click(object sender, RoutedEventArgs e) {
			//TechF.TechFuncs.LogDH("Ответ на запрос: " + TechF.TechFuncs.RTT("users/follows?to_id=107731051&first=100", null));
			TechF.API_Twitch.ApiRequest("users/follows?from_id=107731051");
		}

		private void btstl_Click(object sender, RoutedEventArgs e) {
			TechF.API_Twitch.AuthorizationviaBrowser();
			//TechF.TechFuncs.LogDH("Ответ на запрос: " + TechF.TechFuncs.GetToken());
		}

		private void btstt_Click(object sender, RoutedEventArgs e) {
			TechF.API_Twitch.GetTokenAuth();
		}
	}
}
