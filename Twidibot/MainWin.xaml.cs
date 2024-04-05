using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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

namespace Twidibot {
	public partial class MainWin : MetroWindow {
		public Pages.Status PageStatus = null;
		public Pages.AppSettings PageAppSet = null;
		public Pages.Commands PageCommands = null;
		public Pages.DefCom PageDefCom = null;
		public Pages.FuncCom PageFuncCom = null;
		public Pages.SpamMsg PageSpamMsg = null;
		public Pages.ChatHstr PageChatHstr = null;
		public AboutWin AboutWin = null;
		private BackWin TechF = null;
		public event EventHandler<Twident_Status> Ev_GlobalStatus;
		


		public MainWin(BackWin backWin) {
			InitializeComponent();
			this.TechF = backWin;
			string w = TechF.TechFuncs.GetSettingParam("MainWin_Width");
			string h = TechF.TechFuncs.GetSettingParam("MainWin_Height");
			if (w != null) { this.Width = Convert.ToDouble(w); }
			if (h != null) { this.Height = Convert.ToDouble(h); }
			this.Init();
		}


		// -- Функция инициализации всякого на форме и страницах --
		public void Init() {
			this.PageStatus = new Pages.Status(TechF);
			//this.AboutWin = new AboutWin(TechF);
			this.PageAppSet = new Pages.AppSettings(TechF);
			this.PageDefCom = new Pages.DefCom(TechF);
			this.PageFuncCom = new Pages.FuncCom(TechF);
			this.PageSpamMsg = new Pages.SpamMsg(TechF);
			this.PageCommands = new Pages.Commands(TechF);
			//this.PageChatHstr = new Pages.ChatHstr(TechF);

			typeof(System.Windows.Controls.Primitives.ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this.bMenu_Status, new object[0]);



			// -- Занесение команд в списки --
			// -- Обычные команды
			PageDefCom.TB.Clear();
			for (int i = 0; i < TechF.db.DefCommandsList.Count; i++) {
				PageDefCom.TB.Add(TechF.db.DefCommandsList.ElementAt(i));
			}

			// -- Встроенные функции
			PageFuncCom.TB.Clear();
			for (int i = 0; i < TechF.db.FuncCommandsList.Count; i++) {
				DB.FuncCommand_tclass FuncComl = TechF.db.FuncCommandsList.ElementAt(i);
				if (!FuncComl.Secured) { PageFuncCom.TB.Add(FuncComl); }
			}

			// -- Спам сообщений
			PageSpamMsg.TB.Clear();
			for (int i = 0; i < TechF.db.SpamMessagesList.Count; i++) {
				PageSpamMsg.TB.Add(TechF.db.SpamMessagesList.ElementAt(i));
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



			// -- Настройка страницы с настройками --



			// -- Прочее
			if (TechF.WinFirstLoad) {
				TechF.WinFirstLoad = false;
				if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(0, "Ожидание пользователя", null, false)); }
			}

			//PageStatus.InitStatus(); // - Инициализация подписок на статусы
		}




		private void bMenu_Status_Click(object sender, RoutedEventArgs e) {
			this.Title = "Twidibot - Статус";
			PageStatus.lChat.Items.Clear();
			//this.FrameV.Margin = new Thickness(10, 0, 10, 40);

			// -- Корректировка кнопок запуска/остановки
			PageStatus.OnOff_Auto();
			


			// -- Настройка запретов на запуск
			if (TechF.TechFuncs.GetSettingParam("TwitchActive") == "1") {
				if (!TechF.Twitch.Chat.Work) { PageStatus.lStatusTWH.Content = "Не подключено"; }
				if (TechF.TechFuncs.GetSettingTWHParam("Channel") == null) {
					PageStatus.bStartStop_TWH.IsEnabled = false;
					PageStatus.bStartStop_TWH.ToolTip = "Запуск невозможен: отсутствует имя канала";
				}
				if (TechF.TechFuncs.GetSettingTWHParam("Login") == null) {
					PageStatus.bStartStop_TWH.IsEnabled = false;
					PageStatus.bStartStop_TWH.ToolTip = "Запуск невозможен: отсутствует аккаунт бота";
				}
				if (TechF.TechFuncs.GetSettingTWHParam("Channel") == null && TechF.TechFuncs.GetSettingParam("Login") == null) {
					PageStatus.bStartStop_TWH.IsEnabled = false;
					PageStatus.bStartStop_TWH.ToolTip = "Запуск невозможен: отсутствует имя канала и аккаунт бота";
				}
				if (TechF.TechFuncs.GetSettingTWHParam("Channel") != null && TechF.TechFuncs.GetSettingParam("Login") == null) {
					PageStatus.bStartStop_TWH.IsEnabled = true;
					PageStatus.bStart.ToolTip = null;
				}
			} else {
				PageStatus.lStatusTWH.Content = "Не активно";
				PageStatus.bStartStop_TWH.IsEnabled = false;
				PageStatus.bStartStop_TWH.ToolTip = "Сервис отключён в настройках";
			}

			if (TechF.TechFuncs.GetSettingParam("VKPLActive") == "1") {
				if (!TechF.VKPL.Chat.Work) { PageStatus.lStatusVKPL.Content = "Не подключено"; }
				if (TechF.TechFuncs.GetSettingVKPLParam("Channel") == null) {
					PageStatus.bStartStop_VKPL.IsEnabled = false;
					PageStatus.bStartStop_VKPL.ToolTip = "Запуск невозможен: отсутствует имя канала";
				} else {
					PageStatus.bStartStop_VKPL.IsEnabled = true;
					PageStatus.bStartStop_VKPL.ToolTip = null;
				}
				/*if (TechF.TechFuncs.GetSettingTWHParam("Login") == null) {
					PageStatus.bStartStop_TWH.IsEnabled = false;
					PageStatus.bStartStop_TWH.ToolTip = "Запуск невозможен: отсутствует аккаунт бота (Twitch)";
				}
				if (TechF.TechFuncs.GetSettingTWHParam("Channel") == null && TechF.TechFuncs.GetSettingParam("Login") == null) {
					PageStatus.bStartStop_TWH.IsEnabled = false;
					PageStatus.bStartStop_TWH.ToolTip = "Запуск невозможен: отсутствует имя канала и аккаунт бота (Twitch)";
				}
				if (TechF.TechFuncs.GetSettingTWHParam("Channel") != null && TechF.TechFuncs.GetSettingParam("Login") == null) {
					PageStatus.bStartStop_TWH.IsEnabled = true;
					PageStatus.bStart.ToolTip = null;
				}*/
			} else {
				PageStatus.lStatusVKPL.Content = "Не активно";
				PageStatus.bStartStop_VKPL.IsEnabled = false;
				PageStatus.bStartStop_VKPL.ToolTip = "Сервис отключён в настройках";
			}


			this.Dispatcher.Invoke(() => { this.FrameV.Content = PageStatus; });


			// -- Заполнение чата --
			Task.Factory.StartNew(() => {
				TechF.ChatHistoryListLock = true;
				string dtl = null;

				for (int i = 0; i < TechF.ChatHistoryListTmp.Count; i++) {
					//DB.ChatHistory_tclass chltl = chlt.ElementAt<DB.ChatHistory_tclass>(i);
					DB.ChatHistory_tclass chltl = TechF.ChatHistoryListTmp.ElementAt(i);
					dtl = DateTimeOffset.FromUnixTimeSeconds(chltl.UnixTime).LocalDateTime.ToString();
					this.Dispatcher.Invoke(() => {
						switch (chltl.ServiceType) {
							case 1:
								PageStatus.lChat.Items.Add(new ListWrapC("(" + dtl.Substring(dtl.IndexOf(" ") + 1) + ") " + chltl.Nick + ": " + chltl.Msg, new BitmapImage(new Uri("\\media/img/icons/twh.ico", UriKind.Relative))));
							break;
							case 2:
								PageStatus.lChat.Items.Add(new ListWrapC("(" + dtl.Substring(dtl.IndexOf(" ") + 1) + ") " + chltl.Nick + ": " + chltl.Msg, new BitmapImage(new Uri("\\media/img/icons/vkpl.ico", UriKind.Relative))));
							break;
							default:
								PageStatus.lChat.Items.Add(new ListWrapC("(" + dtl.Substring(dtl.IndexOf(" ") + 1) + ") " + chltl.Nick + ": " + chltl.Msg, null));
							break;
						}
					});
				}
				if (PageStatus.lChat.Items.Count > 0) {
					this.Dispatcher.Invoke(() => { PageStatus.lChat.ScrollIntoView(PageStatus.lChat.Items[PageStatus.lChat.Items.Count - 1]); });
				}
				TechF.ChatHistoryListLock = false;
				GC.Collect();
			});
			this.bMenu_Com.IsEnabled = true;
			this.bMenu_Status.IsEnabled = false;
			this.bMenu_AppSet.IsEnabled = true;
		}


		



		private void bMenu_AppSet_Click(object sender, RoutedEventArgs e) {
			this.Title = "Twidibot - Настройки";
			//DB.Setting_tclass Setl = new DB.Setting_tclass();
			//this.FrameV.Margin = new Thickness(10, 0, 0, 40);

			if (TechF.HideMode) {
				PageAppSet.cb_HideMode.IsChecked = true;
			} else {
				PageAppSet.cb_HideMode.IsChecked = false;
			}

			if (TechF.TechFuncs.GetSettingParam("TwitchActive") == "1") {
				PageAppSet.cbTWH_Active.IsOn = true;
			} else {
				PageAppSet.cbTWH_Active.IsOn = false;
			}
			if (TechF.TechFuncs.GetSettingParam("VKPLActive") == "1") {
				PageAppSet.cbVKPL_Active.IsOn = true;
			} else {
				PageAppSet.cbVKPL_Active.IsOn = false;
			}

			// -- Twitch --
			PageAppSet.eTWH_Channel.Text = TechF.TechFuncs.GetSettingTWHParam("ChannelDisp");
			PageAppSet.eTWH_Login.Text = TechF.TechFuncs.GetSettingTWHParam("LoginDisp");
			PageAppSet.eTWH_Pass.Password = TechF.db.SettingsTWHList.Find(x => x.id == 3).Param;
			PageAppSet.eTWH_Pass2.Visibility = Visibility.Collapsed;

			if (TechF.Twitch.Chat.Work) {
				PageAppSet.cbTWH_Active.IsEnabled = false;
				PageAppSet.cbTWH_BCh.IsEnabled = false;
				PageAppSet.cbTWH_MM.IsEnabled = false;
				PageAppSet.eTWH_Channel.IsEnabled = false;
				PageAppSet.eTWH_Login.IsEnabled = false;
				PageAppSet.eTWH_Pass.IsEnabled = false;
				PageAppSet.eTWH_Pass2.IsEnabled = false;
				PageAppSet.bTWH_Login.IsEnabled = false;
				PageAppSet.cbTWH_Active.ToolTip = "Бот запущен, изменение невозможно";
				PageAppSet.cbTWH_BCh.ToolTip = "Бот запущен, изменение невозможно";
				PageAppSet.cbTWH_MM.ToolTip = "Бот запущен, изменение невозможно";
				PageAppSet.eTWH_Channel.ToolTip = "Бот запущен, изменение невозможно";
				PageAppSet.eTWH_Login.ToolTip = "Бот запущен, изменение невозможно";
				PageAppSet.eTWH_Pass.ToolTip = "Бот запущен, изменение невозможно";
				PageAppSet.eTWH_Pass2.ToolTip = "Бот запущен, изменение невозможно";
				PageAppSet.bTWH_Login.ToolTip = "Бот запущен, вход невозможен";
			} else {
				PageAppSet.cbTWH_Active.IsEnabled = true;
				PageAppSet.cbTWH_BCh.IsEnabled = true;
				PageAppSet.cbTWH_MM.IsEnabled = true;
				PageAppSet.eTWH_Channel.IsEnabled = true;
				PageAppSet.eTWH_Login.IsEnabled = true;
				PageAppSet.eTWH_Pass.IsEnabled = true;
				PageAppSet.eTWH_Pass2.IsEnabled = true;
				PageAppSet.bTWH_Login.IsEnabled = true;
				PageAppSet.cbTWH_Active.ToolTip = null;
				PageAppSet.cbTWH_BCh.ToolTip = null;
				PageAppSet.cbTWH_MM.ToolTip = null;
				PageAppSet.eTWH_Channel.ToolTip = null;
				PageAppSet.eTWH_Login.ToolTip = null;
				PageAppSet.eTWH_Pass.ToolTip = null;
				PageAppSet.eTWH_Pass2.ToolTip = null;
				PageAppSet.bTWH_Login.ToolTip = null;
			}

			if (TechF.TechFuncs.GetSettingTWHParam("BotChannel_isOne") == "1") {
				PageAppSet.cbTWH_BCh.IsOn = true;
			} else {
				PageAppSet.cbTWH_BCh.IsOn = false;
			}
			if (TechF.TechFuncs.GetSettingTWHParam("ManualMode") == "1") {
				PageAppSet.eTWH_Channel.IsEnabled = true;
				PageAppSet.eTWH_Login.IsEnabled = true;
				PageAppSet.eTWH_Pass.IsEnabled = true;
				PageAppSet.eTWH_Pass2.IsEnabled = true;
				PageAppSet.cbTWH_MM.IsOn = true;
			} else {
				PageAppSet.eTWH_Channel.IsEnabled = false;
				PageAppSet.eTWH_Login.IsEnabled = false;
				PageAppSet.eTWH_Pass.IsEnabled = false;
				PageAppSet.eTWH_Pass2.IsEnabled = false;
				PageAppSet.cbTWH_MM.IsOn = false;
			}
			PageAppSet.bTWH_Login_Change();


			// -- VKPL --
			PageAppSet.eVKPL_Channel.Text = TechF.TechFuncs.GetSettingVKPLParam("ChannelDisp");
			PageAppSet.eVKPL_Login.Text = TechF.TechFuncs.GetSettingVKPLParam("LoginDisp");
			PageAppSet.eVKPL_Pass.Password = TechF.db.SettingsVKPLList.Find(x => x.id == 3).Param;
			PageAppSet.eVKPL_Pass2.Visibility = Visibility.Collapsed;

			if (TechF.VKPL.Chat.Work) {
				PageAppSet.cbVKPL_Active.IsEnabled = false;
				PageAppSet.cbVKPL_BCh.IsEnabled = false;
				//PageAppSet.cbVKPL_MM.IsEnabled = false;
				PageAppSet.cbVKPL_RC.IsEnabled = false;
				PageAppSet.eVKPL_Channel.IsEnabled = false;
				//PageAppSet.eVKPL_Login.IsEnabled = false;
				//PageAppSet.eVKPL_Pass.IsEnabled = false;
				//PageAppSet.eVKPL_Pass2.IsEnabled = false;
				PageAppSet.bVKPL_Login.IsEnabled = false;
				PageAppSet.cbVKPL_Active.ToolTip = "Бот запущен, вход невозможен";
				PageAppSet.cbVKPL_BCh.ToolTip = "Бот запущен, изменение невозможно";
				//PageAppSet.cbVKPL_MM.ToolTip = "Бот запущен, изменение невозможно";
				PageAppSet.cbVKPL_RC.ToolTip = "Бот запущен, изменение невозможно";
				PageAppSet.eVKPL_Channel.ToolTip = "Бот запущен, изменение невозможно";
				//PageAppSet.eVKPL_Login.ToolTip = "Бот запущен, изменение невозможно";
				//PageAppSet.eVKPL_Pass.ToolTip = "Бот запущен, изменение невозможно";
				//PageAppSet.eVKPL_Pass2.ToolTip = "Бот запущен, изменение невозможно";
				PageAppSet.bVKPL_Login.ToolTip = "Бот запущен, вход невозможен";
			} else {
				PageAppSet.cbVKPL_Active.IsEnabled = true;
				PageAppSet.cbVKPL_BCh.IsEnabled = true;
				//PageAppSet.cbVKPL_MM.IsEnabled = true;
				PageAppSet.cbVKPL_RC.IsEnabled = true;
				PageAppSet.eVKPL_Channel.IsEnabled = true;
				//PageAppSet.eVKPL_Login.IsEnabled = true;
				//PageAppSet.eVKPL_Pass.IsEnabled = true;
				//PageAppSet.eVKPL_Pass2.IsEnabled = true;
				PageAppSet.bVKPL_Login.IsEnabled = true;
				PageAppSet.cbVKPL_Active.ToolTip = null;
				PageAppSet.cbVKPL_BCh.ToolTip = null;
				//PageAppSet.cbVKPL_MM.ToolTip = null;
				PageAppSet.cbVKPL_RC.ToolTip = null;
				PageAppSet.eVKPL_Channel.ToolTip = null;
				//PageAppSet.eVKPL_Login.ToolTip = null;
				//PageAppSet.eVKPL_Pass.ToolTip = null;
				//PageAppSet.eVKPL_Pass2.ToolTip = null;
				PageAppSet.bVKPL_Login.ToolTip = null;
			}

			if (TechF.TechFuncs.GetSettingVKPLParam("BotChannel_isOne") == "1") {
				PageAppSet.cbVKPL_BCh.IsOn = true;
			} else {
				PageAppSet.cbVKPL_BCh.IsOn = false;
			}
			if (TechF.TechFuncs.GetSettingVKPLParam("ManualMode") == "1") {
				PageAppSet.cbVKPL_MM.IsOn = true;
				PageAppSet.eVKPL_Channel.IsEnabled = true;
				PageAppSet.eVKPL_Login.IsEnabled = true;
				PageAppSet.eVKPL_Pass.IsEnabled = true;
				PageAppSet.eVKPL_Pass2.IsEnabled = true;
			} else {
				PageAppSet.cbVKPL_MM.IsOn = false;
				//PageAppSet.eVKPL_Channel.IsEnabled = false;
				PageAppSet.eVKPL_Login.IsEnabled = false;
				PageAppSet.eVKPL_Pass.IsEnabled = false;
				PageAppSet.eVKPL_Pass2.IsEnabled = false;
			}
			PageAppSet.bVKPL_Login_Change();


			// -- История чата
			if (TechF.TechFuncs.GetSettingParam("ChatHistoryActive") == "1") {
				PageAppSet.cbChatHistory.IsOn = true;
			} else {
				PageAppSet.cbChatHistory.IsOn = false;
			}
			switch (TechF.TechFuncs.GetSettingParam("ChatHistoryDelTime")) {
				case "0":
					PageAppSet.rbChatHistory_Del0.IsChecked = true;
					PageAppSet.eChatHistory_CustDel.IsEnabled = false;
				break;
				case "3":
					PageAppSet.rbChatHistory_Del1.IsChecked = true;
					PageAppSet.eChatHistory_CustDel.IsEnabled = false;
				break;
				case "7":
					PageAppSet.rbChatHistory_Del2.IsChecked = true;
					PageAppSet.eChatHistory_CustDel.IsEnabled = false;
				break;
				case "30":
					PageAppSet.rbChatHistory_Del3.IsChecked = true;
					PageAppSet.eChatHistory_CustDel.IsEnabled = false;
				break;
				default:
					PageAppSet.rbChatHistory_DelCust.IsChecked = true;
					PageAppSet.eChatHistory_CustDel.Text = TechF.TechFuncs.GetSettingParam("ChatHistoryDelTime");
					PageAppSet.eChatHistory_CustDel.IsEnabled = true;
				break;
			}

			// -- Переодические сообщения
			if (TechF.TechFuncs.GetSettingParam("SpamMsgActive") == "1") {
				PageAppSet.cbSpamMessages.IsOn = true;
			} else {
				PageAppSet.cbSpamMessages.IsOn = false;
			}
			if (TechF.TechFuncs.GetSettingParam("SpamMsgTime") == "0") {
				PageAppSet.rbSpamMessages_1.IsChecked = true;
				PageAppSet.eSpamMessages_Cust.IsEnabled = false;
				PageAppSet.eSpamMessages_Cust.Text = "";
			} else {
				PageAppSet.rbSpamMessages_0.IsChecked = true;
				PageAppSet.eSpamMessages_Cust.IsEnabled = true;
				PageAppSet.eSpamMessages_Cust.Text = TechF.TechFuncs.GetSettingParam("SpamMsgTime");
			}

			this.Dispatcher.Invoke(() => this.FrameV.Content = PageAppSet);
			this.bMenu_Com.IsEnabled = true;
			this.bMenu_Status.IsEnabled = true;
			this.bMenu_AppSet.IsEnabled = false;
		}



		private void bMenu_Com_Click(object sender, RoutedEventArgs e) {
			//this.FrameV.Margin = new Thickness(10, 0, 10, 40);
			typeof(System.Windows.Controls.Primitives.ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(PageCommands.bMenu_Def, new object[0]);
			this.Dispatcher.Invoke(() => this.FrameV.Content = PageCommands);
			//this.Title = "Twidibot - Настройка команд";
			this.bMenu_Com.IsEnabled = false;
			this.bMenu_Status.IsEnabled = true;
			this.bMenu_AppSet.IsEnabled = true;
		}
		



		private void bMenu_Other_Click(object sender, RoutedEventArgs e) {
			this.Dispatcher.Invoke(() => this.FrameV.Content = PageChatHstr);
		}


		private void bMenu_Alert_Click(object sender, RoutedEventArgs e) {
			//this.Dispatcher.Invoke(() => );
		}

		private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			double w = this.Width;
			double h = this.Height;
			Task.Factory.StartNew(() => {
				TechF.db.SettingsT.UpdateSetting("MainWin_Width", w.ToString());
				TechF.db.SettingsT.UpdateSetting("MainWin_Height", h.ToString());
			});
			
		}
	}
}
