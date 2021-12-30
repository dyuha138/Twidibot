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
using Microsoft.Win32;

namespace Twidibot
{
	public partial class BackWin : Window
	{

		public event EventHandler<CEvent_Msg> Ev_InitStatus;

		//WindowState prevState;
		private MainWin MainWin = null;
		public TechFuncs TechFuncs = null;
		public Chat Chat = null;
		public DB db = null;
		private LoadWin LoadWin = null;
		public WebBrowserWin WebBrowserWin = null;
		public API_Twitch API_Twitch = null;
		public bool Work = false;

		//public List<DB.ChatHistory_tclass> ChatHistoryListTmp = null;
		public ObservableCollection<DB.ChatHistory_tclass> ChatHistoryListTmp = null;

		public string AppVersion = null;

		public bool ChatHistoryListLock = false;


		public BackWin() {
			InitializeComponent();
			LoadWin = new LoadWin(this);
			LoadWin.Ev_Msg += LoadWin_Ev_Msg;
			LoadWin.Show();
			Task.Factory.StartNew(() => this.Init());
		}

		private void LoadWin_Ev_Msg(object sender, CEvent_Msg e) {

		}



		// --- Функция инициализации ---
		private void Init() {
			Work = true;
			TechFuncs = new TechFuncs(this);

			if (Ev_InitStatus != null) { Ev_InitStatus(this, new CEvent_Msg("Инициализация базы данных...")); }
			TechFuncs.LogDH("Инициализация базы данных...");

			db = new DB();
			db.Open();
			ChatHistoryListTmp = new ObservableCollection<DB.ChatHistory_tclass>();

			if (Ev_InitStatus != null) { Ev_InitStatus(this, new CEvent_Msg("Загрузка настроек...")); }
			TechFuncs.LogDH("Загрузка настроек...");

			db.SettingsT.UpdateListfromTable();

			if (Ev_InitStatus != null) { Ev_InitStatus(this, new CEvent_Msg("Применение настроек...")); }
			TechFuncs.LogDH("Применение настроек...");

			DB.Setting_tclass Setl = db.SettingsList.Find(x => x.id == 1);
			AppVersion = Setl.Param;
			Setl = null;


			// -- Для начала проверим, есть ли наша папочка в реестре
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

				// -- Место для тестовых занесений в бд --
				{
					//db.DefCommandsT.Add(new DB.DefCommand_tclass("синонас", "я @ ты @", 10));

				}

				// -- Загружаем базу в отдельные листы, чтобы работало всё быстрее
				if (Ev_InitStatus != null) { Ev_InitStatus(this, new CEvent_Msg("Загрузка пользовательских команд...")); }
				TechFuncs.LogDH("Загрузка пользовательских команд...");
				db.DefCommandsT.UpdateListfromTable();
				if (Ev_InitStatus != null) { Ev_InitStatus(this, new CEvent_Msg("Загрузка встроеных команд...")); }
				TechFuncs.LogDH("Загрузка встроеных команд...");
				db.FuncCommandsT.UpdateListfromTable();
				if (Ev_InitStatus != null) { Ev_InitStatus(this, new CEvent_Msg("Загрузка переодических сообщений...")); }
				TechFuncs.LogDH("Загрузка переодических сообщений...");
				db.SpamMessagesT.UpdateListfromTable();
				if (Ev_InitStatus != null) { Ev_InitStatus(this, new CEvent_Msg("Загрузка прочего...")); }
				TechFuncs.LogDH("Загрузка таймеров...");
				db.Funcs_TimersT.UpdateListfromTable();
				TechFuncs.LogDH("Загрузка секундомеров...");
				db.Funcs_StopwatchesT.UpdateListfromTable();
				TechFuncs.LogDH("Загрузка счётчиков...");
				db.Funcs_CountersT.UpdateListfromTable();
				TechFuncs.LogDH("Инициализация прочих функций...");
				TechFuncs.Init();
			}



			if (Ev_InitStatus != null) { Ev_InitStatus(this, new CEvent_Msg("Инициализация функций чата...")); }
			TechFuncs.LogDH("Инициализация функций чата...");

			//Chat = new Chat(this, "dyuha138");
			Chat = new Chat(this, "gruz");
			//Chat = new Chat(this);
			Chat.Ev_ChatMsg += ChatMsgSave;
			Chat.Ev_BotMsg += ChatMsgSave;
			//Chat.botparam_Mod = true;

			if (Ev_InitStatus != null) { Ev_InitStatus(this, new CEvent_Msg("Инициализация доступа к различным API...")); }
			TechFuncs.LogDH("Инициализация доступа к Twitch API...");
			API_Twitch = new API_Twitch(this);


			if (Ev_InitStatus != null) { Ev_InitStatus(this, new CEvent_Msg("Инициализация и загрузка интерфейса...")); }
			TechFuncs.LogDH("Инициализация и загрузка интерфейса...");

			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
				MainWin = new MainWin(this);
				WebBrowserWin = new WebBrowserWin(this);
				LoadWin.Close();
				LoadWin = null;
				MainWin.Show();
			}));

			TechFuncs.LogDH("Выполнение настроечных запросов...");
			Chat.botparam_Follow = API_Twitch.Req_FollowtoChannel();
			


			GC.Collect();
			TechFuncs.LogDH("Загрузка завершена");
		}



		private void TaskbarIcon_LeftClick(object sender, RoutedEventArgs e) {
			if (!MainWin.IsVisible) {
				MainWin = new MainWin(this);
				MainWin.Show();
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
			if (Chat.Worked) {
				Chat.Close();
			}
			MainWin.Close();
			//MainWin = null;
			TechFuncs.LogDH("Работа завершена");
			Thread.Sleep(500);
			Application.Current.Shutdown();
		}

		private void TaskbarIcon_Menu_Con_Click(object sender, RoutedEventArgs e) {
			if (!Chat.Worked) {
				Chat.Connect();
			} else {
				if (Chat.Worked) {
					Chat.Close();
				}
			}
		}


		private void TaskbarIcon_Opened(object sender, RoutedEventArgs e) {
			if (Chat.Worked) {
				tbTaskbarIcon_Menu_Status.Text = "Статус: Работает \u221a";
				bTaskbarIcon_Menu_Con.Header = "Отключиться...";
			}
			if (!Chat.Worked) {
				tbTaskbarIcon_Menu_Status.Text = "Статус: Не подключён";
				bTaskbarIcon_Menu_Con.Header = "Подключиться...";
			}
			if (TechFuncs.GetSettingParam("Channel") == null) {
				bTaskbarIcon_Menu_Con.IsEnabled = false;
			} else {
				bTaskbarIcon_Menu_Con.IsEnabled = true;
			}
		}

		private void ChatMsgSave(object sender, CEvent_ChatMsg e) {
			ChatHistoryListTmp.Add(new DB.ChatHistory_tclass(e.Userid, e.Msgid, e.Nick, e.Msg, e.Date, e.Time, e.Color, e.isOwner, false, e.isMod, e.isVIP, e.isSub, e.BadgeInfo, e.Badges));
			while (ChatHistoryListTmp.Count > 100) {
				ChatHistoryListTmp.RemoveAt(0);
			}
			//db.ChatHistoryT.Add(new DB.ChatHistory_tclass(e.Userid, e.Msgid, e.Nick, e.Msg, e.Date, e.Time, e.Color, e.isOwner, false, e.isMod, e.isVIP, e.isSub, e.BadgeInfo, e.Badges));
		}
	}
}
