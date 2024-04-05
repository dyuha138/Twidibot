using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
//using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Data.SqlClient;

namespace Twidibot {
	public partial class BackWin : Window {
		public event EventHandler<Twident_Msg> Ev_InitStatus;
		public event EventHandler<Twident_Status> Ev_GlobalStatus;
		public event EventHandler<Twident_Null> Ev_Shutdown;
		public Twident_Status GlobalStatus = null;
		public Twident_Status TWHStatus = null;
		public Twident_Status VKPLStatus = null;

		private LoadWin LoadWin = null;
		public TechFuncs TechFuncs = null;
		public DB db = null;
		public Twitch Twitch = null;
		public VKPL VKPL = null;
		public ChatFuncs ChatFuncs = null;
		public MainWin MainWin = null;
		
		//public WebBrowserWin WebBrowserWin = null;
		public bool Work = false;
		public bool HideMode = false;
		public bool WinFirstLoad = true;

		//public List<DB.ChatHistory_tclass> ChatHistoryListTmp = null;
		public ObservableCollection<DB.ChatHistory_tclass> ChatHistoryListTmp = null;

		public string AppVersion = "0.3.0b4";
		public bool ChatHistoryListLock = false;


		public BackWin() {
			InitializeComponent();
			GlobalStatus = new Twident_Status(3, "Загрузка", null, null);
			LoadWin = new LoadWin(this);
			LoadWin.Show();
			Task.Factory.StartNew(() => this.Init());
		}



		// --- Функция инициализации ---
		private void Init() {
			Work = true;
			TechFuncs = new TechFuncs(this);
			//Stopwatch sw = new Stopwatch();

			Ev_InitStatus(this, new Twident_Msg("Инициализация базы данных"));
			TechFuncs.LogDH("Инициализация базы данных");

			db = new DB(this.TechFuncs);
			db.Open();

		

			ChatHistoryListTmp = new ObservableCollection<DB.ChatHistory_tclass>();

			
			Ev_InitStatus(this, new Twident_Msg("Загрузка настроек"));
			TechFuncs.LogDH("Загрузка настроек");
			db.SettingsT.UpdateListfromTable();
			db.SettingsTWHT.UpdateListfromTable();
			db.SettingsVKPLT.UpdateListfromTable();

			Ev_InitStatus(this, new Twident_Msg("Применение глобальных настроек"));
			//TechFuncs.LogDH("Применение глобальных настроек...");


			//AppVersion = db.SettingsList.Find(x => x.id == 1).Param;
			//Twitch.SimpleMode = Convert.ToBoolean(Convert.ToInt32(db.SettingsTWHList.Find(x => x.id == 9).Param));
			//Twitch.botChannelisOne = Convert.ToBoolean(Convert.ToInt32(db.SettingsTWHList.Find(x => x.id == 8).Param));
			//VKPL.SimpleMode = Convert.ToBoolean(Convert.ToInt32(db.SettingsVKPLList.Find(x => x.id == 9).Param));
			//VKPL.botChannelisOne = Convert.ToBoolean(Convert.ToInt32(db.SettingsVKPLList.Find(x => x.id == 8).Param));

			/*// -- Для начала проверим, есть ли наша папочка в реестре
			RegistryKey RegL = Registry.CurrentUser;
			RegistryKey Reg = RegL.OpenSubKey("SOFTWARE\\Twidibot");

			if (Reg == null) { // -- Папочки нет, создаём её, и понимаем, что это первый запуск бота
				Reg = RegL.CreateSubKey("SOFTWARE\\Twidibot");
				Reg.SetValue("Version", AppVersion);
				Reg.SetValue("Language", "RU");
				//Reg.SetValue("", "");
				// -- Тут будет открываться страничка с предложением воспользоваться мастером настройки

				Reg.Flush();
			} else { // -- Папка есть, поэтому идём проверять настроечки
				if (Reg.GetValue("Version").ToString() != AppVersion) {
					// -- Тут будет показываться окошко с ченджлогом
				}
				// -- Тут будет смена языка функцией
				//FuncChangeLanguage(Reg.GetValue("Language"));
			}
			*/


			// -- Загружаем базу в отдельные листы, чтобы работало всё быстрее
			Ev_InitStatus(this, new Twident_Msg("Инициализия функций"));
			TechFuncs.LogDH("Инициализация внутренних функций");
			TechFuncs.Init();
			TechFuncs.LogDH("Инициализация пользовательских функций");
			ChatFuncs = new ChatFuncs(this);
			Ev_InitStatus(this, new Twident_Msg("Загрузка базы данных"));
			TechFuncs.LogDH("Загрузка базы данных");
			db.UpdateAllLists(false);

			Ev_InitStatus(this, new Twident_Msg("Инициализация модуля Twitch"));
			TechFuncs.LogDH("Инициализация модуля Twitch");
			Twitch = new Twitch(this);
			//Twitch.Chat = new Chat(this, "dyuha138");
			//Twitch.Chat = new Chat(this, "gruz");
			Twitch.Chat.Ev_ChatMsg += ChatMsgSave;
			Twitch.Chat.Ev_BotMsg += ChatMsgSave;
			TechFuncs.LogDH("Инициализация доступа к Twitch API");
			//Twitch.API.FLogin("bot?code=&scope=bits%3Aread+channel%3Amanage%3Abroadcast+channel%3Amanage%3Aredemptions+channel%3Aread%3Aeditors+channel%3Aread%3Ahype_train+channel%3Aread%3Apolls+channel%3Aread%3Aredemptions+channel%3Aread%3Asubscriptions+moderation%3Aread+moderator%3Amanage%3Abanned_users+moderator%3Aread%3Aautomod_settings+moderator%3Aread%3Achat_settings+moderator%3Amanage%3Achat_settings+user%3Aread%3Afollows");
			//Twitch.API.ValidateandRefreshAllTokens();

			Ev_InitStatus(this, new Twident_Msg("Инициализация модуля VK Play LIVE"));
			TechFuncs.LogDH("Инициализация модуля VK Play LIVE");
			VKPL = new VKPL(this);
			VKPL.Chat.Ev_ChatMsg += ChatMsgSave;
			VKPL.Chat.Ev_BotMsg += ChatMsgSave;

			Ev_InitStatus(this, new Twident_Msg("Применение мелких настроек"));
			Twitch.Chat.DataUpdate();
			VKPL.Chat.DataUpdate();		
			Ev_GlobalStatus += GlobalStatusSave;
			Twitch.Chat.Ev_Status += TWHStatusSave;
			Twitch.Chat.Ev_GlobalStatus += GlobalStatusSave;
			VKPL.Chat.Ev_Status += VKPLStatusSave;
			VKPL.Chat.Ev_GlobalStatus += GlobalStatusSave;
			Twitch.API.Ev_Status += TWHStatusSave;
			Twitch.API.Ev_GlobalStatus += GlobalStatusSave;

			Ev_InitStatus(this, new Twident_Msg("Инициализация и загрузка интерфейса"));
			TechFuncs.LogDH("Инициализация и загрузка интерфейса");
			

			this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
				MainWin = new MainWin(this);
				//WebBrowserWin = new WebBrowserWin(this);
				MainWin.PageAppSet.Ev_ScrollChanged += MainFrameV_ScrollChanged;
				MainWin.PageStatus.InitStatus();
				LoadWin.Close();
				LoadWin = null;
				MainWin.Show();
			}));


			GC.Collect();
			TechFuncs.LogDH("Основная загрузка завершена");
			Ev_GlobalStatus?.Invoke(this, new Twident_Status(0, "Пока без ошибок", null, false));
			if (!Twitch.ManualMode) {
				TechFuncs.LogDH("Выполнение настроечных запросов для Twitch...");
				Task.Factory.StartNew(() => {
					//Twitch.API.GetTokenAuth(); // -- Для ручного изменения токена авторизации
					//Twitch.API.Req_UpdateBotChannelInfo(); // -- Только при установке канала/бота через конструктор чата
					//Chat.botparam_Follow = Twitch.API.Req_BotFollowtoChannel();
					//Twitch.API.GetOAuthCode();
				});
			}
			//string oauthurl = Twitch.API.GetOAuthCode(true); // -- Т.к. пока нет выбора браузера, в каком запустить, будем узнавать ссылку на получения oauth через точку остановки



			// -- Место для тестов --
			{
				//Stopwatch sw = new Stopwatch();


				//VKPL.Chat.ChatMsgParse("<div class=\"ChatMessage_publishTime_EzWt_\">18:03</div><div class=\"ChatMessage_message_r1jzC\"><div class=\"ChatMessage_author_aK1y8\"><span class=\"ChatMessage_name_RE2MY\" data-test-id=\"ChatMessage:author\" style=\"color: rgb(163, 108, 89);\">dyuha138:</span></div><span class=\"ChatMessage_text_Eklor\" data-test-id=\"ChatMessage:message\"><span class=\"BlockRenderer_markup_Wtipg BlockRenderer_noIndent_yWqyP\"><span class=\"mention ContentRenderer_mention_eMmgA\" data-mention-id=\"15816367\" data-display-name=\"Twidibot\" contenteditable=\"false\">Twidibot</span>, опять&nbsp;<img alt=\"MeHappy\" src=\"https://images.vkplay.live/smile/4890a9c7-fd45-44af-9e45-d46a5d1187b3/size/small?change_time=1670447567\" srcset=\"https://images.vkplay.live/smile/4890a9c7-fd45-44af-9e45-d46a5d1187b3/size/medium?change_time=1670447567 2x, https://images.vkplay.live/smile/4890a9c7-fd45-44af-9e45-d46a5d1187b3/size/large?change_time=1670447567 3x\" style=\"vertical-align: middle; width: 28px; height: 28px; cursor: pointer;\" data-id=\"4890a9c7-fd45-44af-9e45-d46a5d1187b3\" data-type=\"smile\">&nbsp;тестим<img alt=\"bbansheeKek\" src=\"https://images.vkplay.live/smile/dccc6814-f9b0-4d09-abef-17ccb11d4c61/size/small?change_time=1669196223\" srcset=\"https://images.vkplay.live/smile/dccc6814-f9b0-4d09-abef-17ccb11d4c61/size/medium?change_time=1669196223 2x, https://images.vkplay.live/smile/dccc6814-f9b0-4d09-abef-17ccb11d4c61/size/large?change_time=1669196223 3x\" style=\"vertical-align: middle; width: 28px; height: 28px; cursor: pointer;\" data-id=\"dccc6814-f9b0-4d09-abef-17ccb11d4c61\" data-type=\"smile\"></span>");
				//Twitch.API.ApiRequest("users?login=");


				//for (int i = 28; i > 8; i--) {
				//DB.Setting_tclass setl = db.SettingsList.ElementAt(i-3);
				//db.SQLQuery("UPDATE Settings SET SetName = '" + i + "', Param = '', Secured = '0' WHERE id='" + (i-2).ToString() + "';");
				//db.SQLQuery("UPDATE Settings SET SetName = '" + setl.SetName + "', Param = '" + setl.Param + "', Secured = '" + Convert.ToInt32(setl.Secured).ToString() + "' WHERE id='" + i.ToString() + "';");
				//}
				//db.SQLQuery("UPDATE Settings SET SetName = 'ChannelId', Param = null, Secured = '0' WHERE id='7';");
				//db.SQLQuery("UPDATE Settings SET SetName = 'LoginId', Param = null, Secured = '0' WHERE id='8';");


				//List<string> listsl = new List<string>();
				//List<DB.Funcs_Timer_tclass> listtl = new List<DB.Funcs_Timer_tclass>();
				//Random rand = new Random();
				//TechFuncs.LogDH("Счётчик выполнения кода - начали");


				/*sw.Start();

				sw.Stop();
				TechFuncs.LogDH("Счётчик выполнения кода -: " + sw.Elapsed + " | " + sw.ElapsedMilliseconds + " | " + sw.ElapsedTicks);
				sw.Reset();*/


				//Twitch.API.ApiRequest("users?login=twidibot", false, "GET", null, null, null, new string[,] { { "Ratelimit" }, { "Remaining" } });

				//db.DefCommandsT.Add(new DB.DefCommand_tclass("tst", "tstotvet", 3, false, true));


				//MessageBox.Show("", "");


				// -- Перемещение записей вниз
				//DB.SQLResultTable restbl = db.SQLQuery("SELECT id, \"Замена c_id\", \"Аддон к_id\" FROM Моды ORDER BY id;", false);
				//restbl.SetRow(restbl.Rows - 1);
				//int i = 194;
				//while (restbl.PrevRow()) {
				//	if (restbl.RowRead == 196 - 1) {
				//		int da = 0;
				//	}
				//	if (restbl.RowRead > i - 1) { db.SQLQuery($"UPDATE Моды SET id = id + 1 WHERE id = {restbl.GetInt(0)};"); }
				//	if (restbl.GetInt(1) > i) { db.SQLQuery($"UPDATE Моды SET \"Замена c_id\"  = \"Замена c_id\" + 1 WHERE id = {restbl.GetInt(0) + 1};"); }
				//	if (restbl.GetInt(2) > i) { db.SQLQuery($"UPDATE Моды SET \"Аддон к_id\" = \"Аддон к_id\" + 1 WHERE id = {restbl.GetInt(0) + 1};"); }
				//}
			}
		}



		private void TaskbarIcon_LeftClick(object sender, RoutedEventArgs e) {
			if (!MainWin.IsVisible) {
				MainWin = new MainWin(this);
				MainWin.PageStatus.InitStatus();
				MainWin.Show();
				//Ev_GlobalStatus?.Invoke(this, this.GlobalStatus);
				if (this.GlobalStatus != null) { MainWin.PageStatus.GlobalStatus_Set(null, this.GlobalStatus); }
				if (this.TWHStatus != null) { MainWin.PageStatus.TWHStatus_Set(null, this.TWHStatus); }
				if (this.VKPLStatus != null) { MainWin.PageStatus.VKPLStatus_Set(null, this.VKPLStatus); }
			} else {
				if (MainWin.WindowState == WindowState.Normal) {
					MainWin.Activate();
				} else {
					if (MainWin.WindowState == WindowState.Minimized) {
						MainWin.WindowState = WindowState.Normal;
						MainWin.Show();
						MainWin.Activate();
					}
				}
			}
		}

		private void TaskbarIcon_Menu_AppHide_Click(object sender, RoutedEventArgs e) {
			MainWin.Hide();
		}

		private void TaskbarIcon_Menu_AppStop_Click(object sender, RoutedEventArgs e) {
			Work = false;
			Ev_Shutdown?.Invoke(null, new Twident_Null());
			if (Twitch.Chat.Work) { Twitch.Chat.Close(); }
			if (VKPL.Chat.Work) { VKPL.Chat.Close(); }
			MainWin.Close();
			//MainWin = null;
			db.Close();
			TechFuncs.LogDH("Работа завершена");
			Thread.Sleep(500);
			Application.Current.Shutdown();
		}

		private void TaskbarIcon_Menu_Con_Click(object sender, RoutedEventArgs e) {
			if (!Twitch.Chat.Work) { Twitch.Chat.Connect();
			} else { Twitch.Chat.Close(); }

			if (!VKPL.Chat.Work) { VKPL.Chat.Connect();
			} else { VKPL.Chat.Close(); }
		}


		private void TaskbarIcon_Opened(object sender, RoutedEventArgs e) {
			/*if (Chat.Work) {
				tbTaskbarIcon_Menu_Status.Text = "Статус: Работает \u221a";
				bTaskbarIcon_Menu_Con.Header = "Отключиться";
			} else {
				tbTaskbarIcon_Menu_Status.Text = "Статус: Не подключён";
				bTaskbarIcon_Menu_Con.Header = "Подключиться";
			}
			if (TechFuncs.GetSettingParam("Channel") == null) {
				bTaskbarIcon_Menu_Con.IsEnabled = false;
			} else {
				bTaskbarIcon_Menu_Con.IsEnabled = true;
			}*/
			tbTaskbarIcon_Menu_Status.Text = this.GlobalStatus.Message;
		}

		private void ChatMsgSave(object sender, Twident_ChatMsg e) {
			ChatHistoryListTmp.Add(new DB.ChatHistory_tclass(e.ServiceType, e.Userid, e.Msgid, e.Nick, e.DispNick, e.Msg, e.UnixTime, e.Color, e.isOwner, false, e.isMod, e.isVIP, e.isSub, e.BadgeInfo, e.Badges));
			while (ChatHistoryListTmp.Count > 100) {
				ChatHistoryListTmp.RemoveAt(0);
			}
			//db.ChatHistoryT.Add(new DB.ChatHistory_tclass(e.Userid, e.Msgid, e.Nick, e.Msg, e.Date, e.Time, e.Color, e.isOwner, false, e.isMod, e.isVIP, e.isSub, e.BadgeInfo, e.Badges));
		}

		
		private void MainFrameV_ScrollChanged(object sender, Twident_Bool e) {
			Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => {
				if (e.Handle) {
					//MainWin.FrameV.Margin = new Thickness(10, 0, 0, 40);
				} else {
					//MainWin.FrameV.Margin = new Thickness(10, 0, 10, 40);
				}
			}));
			
		}


		public void GlobalStatusSave(object sender, Twident_Status e) {
			this.GlobalStatus = e;
		}

		public void TWHStatusSave(object sender, Twident_Status e) {
			this.TWHStatus = e;
		}

		public void VKPLStatusSave(object sender, Twident_Status e) {
			this.VKPLStatus = e;
		}

	}
}
