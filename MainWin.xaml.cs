using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Reflection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Twidibot.Pages;

namespace Twidibot
{

	public partial class MainWin : Window {

		public Pages.Status PageStatus = null;
		public Pages.AppSettings PageAppSet = null;
		public Pages.Commands PageCommands = null;
		public Pages.DefCom PageDefCom = null;
		public Pages.FuncCom PageFuncCom = null;
		public Pages.SpamMsg PageSpamMsg = null;
		public Pages.ChatHstr PageChatHstr = null;
		public AboutWin AboutWin = null;
		private BackWin TechF = null;
		


		public MainWin(BackWin backWin) {
			InitializeComponent();
			this.TechF = backWin;
			this.Init();
		}


		// -- Функция инициализации всякого на форме и страницах --
		public void Init() {
			this.PageStatus = new Pages.Status(TechF);
			this.PageAppSet = new Pages.AppSettings(TechF, this);
			this.PageCommands = new Pages.Commands(TechF, this);
			this.PageDefCom = new Pages.DefCom(TechF);
			this.PageFuncCom = new Pages.FuncCom(TechF);
			this.PageSpamMsg = new Pages.SpamMsg(TechF);
			this.PageChatHstr = new Pages.ChatHstr(TechF);
			this.AboutWin = new AboutWin(TechF);

			// -- Занесение команд в списки --
			// -- Обычные команды
			PageDefCom.TB.Clear();
			for (uint i = 0; i < TechF.db.DefCommandsList.Count; i++) {
				PageDefCom.TB.Add(TechF.db.DefCommandsList.ElementAt<DB.DefCommand_tclass>(Convert.ToInt32(i)));
			}

			// -- Встроенные функции
			PageFuncCom.TB.Clear();
			for (uint i = 0; i < TechF.db.FuncCommandsList.Count; i++) {
				DB.FuncCommand_tclass FuncComl = TechF.db.FuncCommandsList.ElementAt<DB.FuncCommand_tclass>(Convert.ToInt32(i));
				if (!FuncComl.Secured) { PageFuncCom.TB.Add(FuncComl); }
			}

			// -- Спам сообщений
			PageSpamMsg.TB.Clear();
			for (uint i = 0; i < TechF.db.SpamMessagesList.Count; i++) {
				PageSpamMsg.TB.Add(TechF.db.SpamMessagesList.ElementAt<DB.SpamMessage_tclass>(Convert.ToInt32(i)));
			}


			// -- Скрытие элементов на страницах команд --
			// -- Обычные команды
			PageDefCom.eName.Visibility = Visibility.Hidden;
			PageDefCom.eRes.Visibility = Visibility.Hidden;
			PageDefCom.eCD.Visibility = Visibility.Hidden;
			PageDefCom.bSave.Visibility = Visibility.Hidden;
			PageDefCom.bCan.Visibility = Visibility.Hidden;
			PageDefCom.bAdd.Visibility = Visibility.Visible;
			PageDefCom.lerr.Visibility = Visibility.Hidden;
			PageDefCom.lName_A.Visibility = Visibility.Hidden;
			PageDefCom.cbAlias.Visibility = Visibility.Hidden;

			// -- Встроенные команды
			PageFuncCom.eCom.Visibility = Visibility.Hidden;
			PageFuncCom.eCD.Visibility = Visibility.Hidden;
			PageFuncCom.bSave.Visibility = Visibility.Hidden;
			PageFuncCom.bCan.Visibility = Visibility.Hidden;
			PageFuncCom.lerr.Visibility = Visibility.Hidden;

			// -- Спам сообщений
			PageSpamMsg.eMsg.Visibility = Visibility.Hidden;
			PageSpamMsg.eCD.Visibility = Visibility.Hidden;
			PageSpamMsg.bSave.Visibility = Visibility.Hidden;
			PageSpamMsg.bCan.Visibility = Visibility.Hidden;
			PageSpamMsg.bAdd.Visibility = Visibility.Visible;
			PageSpamMsg.lerr.Visibility = Visibility.Hidden;
		}



		private void bMenu_Status_Click(object sender, RoutedEventArgs e) {
			this.Title = "Twidibot - Статус";
			PageStatus.lChat.Items.Clear();
			this.FrameV.Margin = new Thickness(10, 0, 10, 38);

			// -- Установка имени бота
			if (TechF.Chat.useDefBot) {
				PageStatus.lBotLogin.Text = "Twidibot";
			} else {
				if (TechF.TechFuncs.GetSettingParam("Login") == null) {
					PageStatus.lBotLogin.Text = "Не назначено";
				} else {
					PageStatus.lBotLogin.Text = TechF.TechFuncs.GetSettingParam("Login"); // -- Вывод канала
				}
			}

			// -- Блокировка кнопки запуска бота, если не указан канал, и установка этого канала
			if (TechF.TechFuncs.GetSettingParam("Channel") == null) {
				PageStatus.lChannel.Text = "Не назначено";
			} else {
				PageStatus.lChannel.Text = TechF.TechFuncs.GetSettingParam("Channel"); // -- Вывод канала
			}

			// -- Установка статуса подключения
			if (TechF.Chat.Worked) {
				PageStatus.lStatus.Content = "Подключено";
				PageStatus.bChatStart.IsEnabled = false;
				PageStatus.bChatStop.IsEnabled = true;
			} else {
				if (PageStatus.lStatus.Content.ToString() != "Отключено") {
					PageStatus.lStatus.Content = "Не подключено";
				}
				PageStatus.bChatStart.IsEnabled = true;
				PageStatus.bChatStop.IsEnabled = false;
			}

			// -- Настройка прочего говна отдельно
			if (!TechF.Chat.Worked) {
				PageStatus.bChatStart.IsEnabled = true;
			} else {
				PageStatus.bChatStart.IsEnabled = false;
			}
			if (TechF.TechFuncs.GetSettingParam("Channel") == null) {
				PageStatus.bChatStart.IsEnabled = false;
				PageStatus.bChatStart.ToolTip = "Запуск невозможен: отсутствует имя канала";
			}
			if (TechF.TechFuncs.GetSettingParam("Login") == null && !TechF.Chat.useDefBot) {
				PageStatus.bChatStart.IsEnabled = false;
				PageStatus.bChatStart.ToolTip = "Запуск невозможен: отсутствует аккаунт бота";
			}
			if (TechF.TechFuncs.GetSettingParam("Channel") == null && TechF.TechFuncs.GetSettingParam("Login") == null) {
				PageStatus.bChatStart.IsEnabled = false;
				PageStatus.bChatStart.ToolTip = "Запуск невозможен: отсутствует имя канала и аккаунт бота";
			}
			if (TechF.TechFuncs.GetSettingParam("Channel") != null && TechF.TechFuncs.GetSettingParam("Login") != null) {
				PageStatus.bChatStart.IsEnabled = true;
				PageStatus.bChatStart.ToolTip = null;
			}

			this.Dispatcher.Invoke(() => { this.FrameV.Content = PageStatus; });


			Task.Factory.StartNew(() => {
				TechF.ChatHistoryListLock = true;

				for (int i = 0; i < TechF.ChatHistoryListTmp.Count; i++) {
					//DB.ChatHistory_tclass chltl = chlt.ElementAt<DB.ChatHistory_tclass>(i);
					DB.ChatHistory_tclass chltl = TechF.ChatHistoryListTmp.ElementAt(i);
					this.Dispatcher.Invoke(() => {
						PageStatus.lChat.Items.Add(new ListWrapC() { Text = "(" + chltl.Time + ") " + chltl.Nick + ": " + chltl.Msg });
						//PageStatus.lChat.ScrollIntoView(PageStatus.lChat.Items[PageStatus.lChat.Items.Count - 1]);
					});
					//Thread.Sleep(5);
				}
				if (PageStatus.lChat.Items.Count > 0) {
					this.Dispatcher.Invoke(() => { PageStatus.lChat.ScrollIntoView(PageStatus.lChat.Items[PageStatus.lChat.Items.Count - 1]); });
				}
				TechF.ChatHistoryListLock = false;
				GC.Collect();
			});
			this.bMenuCom.IsEnabled = true;
			this.bMenuStatus.IsEnabled = false;
			this.bMenuAppSet.IsEnabled = true;
		}



		private void bMenu_AppSet_Click(object sender, RoutedEventArgs e) {
			this.Title = "Twidibot - Настройки";
			DB.Setting_tclass Setl = new DB.Setting_tclass();
			this.FrameV.Margin = new Thickness(10, 0, 0, 38);

			// -- Аккаунт бота
			if (TechF.Chat.Worked) {
				PageAppSet.eChatChannel.IsEnabled = false;
				PageAppSet.eChatLogin.IsEnabled = false;
				PageAppSet.eChatPass.IsEnabled = false;
				//PageAppSet.rbChatBot_Cust.IsEnabled = false;
				//PageAppSet.rbChatBot_Def.IsEnabled = false;
				PageAppSet.eChatChannel.ToolTip = "Бот запущен, изменение невозможно";
				PageAppSet.eChatLogin.ToolTip = "Бот запущен, изменение невозможно";
				PageAppSet.eChatPass.ToolTip = "Бот запущен, изменение невозможно";
				//PageAppSet.rbChatBot_Cust.ToolTip = "Бот запущен, изменение невозможно";
				//PageAppSet.rbChatBot_Def.ToolTip = "Бот запущен, изменение невозможно";
			} else {
				if (TechF.Chat.useDefBot) {
					PageAppSet.eChatLogin.IsEnabled = false;
					PageAppSet.eChatPass.IsEnabled = false;
					PageAppSet.rbChatBot_Def.IsEnabled = true;
					PageAppSet.eChatLogin.Text = null;
					PageAppSet.eChatPass.Password = null;
					PageAppSet.rbChatBot_Def.IsChecked = true;
				} else {
					PageAppSet.eChatLogin.IsEnabled = true;
					PageAppSet.eChatPass.IsEnabled = true;
					PageAppSet.rbChatBot_Cust.IsEnabled = true;
					PageAppSet.eChatLogin.Text = TechF.TechFuncs.GetSettingParam("Login");
					PageAppSet.eChatPass.Password = TechF.TechFuncs.GetSettingParam("Pass");
				}
				PageAppSet.eChatChannel.ToolTip = null;
				PageAppSet.eChatLogin.ToolTip = null;
				PageAppSet.eChatPass.ToolTip = null;
				//PageAppSet.rbChatBot_Cust.ToolTip = null;
				//PageAppSet.rbChatBot_Def.ToolTip = null;
			}
			PageAppSet.eChatChannel.Text = TechF.TechFuncs.GetSettingParam("Channel");

			// -- История чата
			Setl = TechF.db.SettingsList.Find(x => x.id == 7);
			switch (Setl.Param) {
				case "0":
					PageAppSet.rbChatHistory_Del0.IsChecked = true;
					PageAppSet.eChatHistory_CustDel.IsEnabled = false;
				break;
				case "1d":
					PageAppSet.rbChatHistory_Del1d.IsChecked = true;
					PageAppSet.eChatHistory_CustDel.IsEnabled = false;
				break;
				case "3d":
					PageAppSet.rbChatHistory_Del3d.IsChecked = true;
					PageAppSet.eChatHistory_CustDel.IsEnabled = false;
				break;
				case "1w":
					PageAppSet.rbChatHistory_Del1w.IsChecked = true;
					PageAppSet.eChatHistory_CustDel.IsEnabled = false;
				break;
				default:
					PageAppSet.rbChatHistory_DelCust.IsChecked = true;
					PageAppSet.eChatHistory_CustDel.Text = Setl.Param;
					PageAppSet.eChatHistory_CustDel.IsEnabled = true;
				break;
			}

			// -- Переодические сообщения
			if (TechF.TechFuncs.GetSettingParam("SpamMsg") == "0") {
				PageAppSet.rbSpamMessages_1.IsChecked = true;
				PageAppSet.eSpamMessages_Cust.Text = "";
			} else {
				PageAppSet.rbSpamMessages_0.IsChecked = true;
				PageAppSet.eSpamMessages_Cust.Text = TechF.TechFuncs.GetSettingParam("SpamMsg");
			}
			if (TechF.TechFuncs.GetSettingParam("SpamMsgWork") == "true") {
				PageAppSet.cbSpamMessages_E.IsChecked = true;
			} else {
				PageAppSet.cbSpamMessages_E.IsChecked = false;
			}

			this.Dispatcher.Invoke(() => { this.FrameV.Content = PageAppSet; });
			this.bMenuCom.IsEnabled = true;
			this.bMenuStatus.IsEnabled = true;
			this.bMenuAppSet.IsEnabled = false;
		}



		private void bMenu_Com_Click(object sender, RoutedEventArgs e) {
			this.FrameV.Margin = new Thickness(10, 0, 10, 38);
			typeof(System.Windows.Controls.Primitives.ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(PageCommands.bMenuDef, new object[0]);
			this.Dispatcher.Invoke(() => { this.FrameV.Content = PageCommands; });
			//this.Title = "Twidibot - Настройка команд";
			this.bMenuCom.IsEnabled = false;
			this.bMenuStatus.IsEnabled = true;
			this.bMenuAppSet.IsEnabled = true;
		}
		


		private void bMenu_ChatHstr_Click(object sender, RoutedEventArgs e) {


			this.Dispatcher.Invoke(() => { this.FrameV.Content = PageChatHstr; });
		}
	}
}
