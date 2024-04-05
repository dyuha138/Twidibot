using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Timers;
using WebSocket4Net;
using System.Windows;
using System.Globalization;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Interop;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Security.AccessControl;
using RestSharp;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
//using SeleniumUndetectedChromeDriver;

namespace Twidibot {
	public partial class VKPL {
		private BackWin TechF = null;
		public ChatCBRW Chat = null;
		//public APIC API = null;
		public APIBRWC APIBRW = null;
		public bool Active = false;
		public bool ManualMode = false;
		public bool BotChannelisOne = false;


		public VKPL(BackWin backWin) {
			TechF = backWin;
			Chat = new ChatCBRW(backWin);
			//API = new APIC(backWin);
			APIBRW = new APIBRWC(backWin);
			UpdateActive();
		}


		public void UpdateActive() {
			if (TechF.TechFuncs.GetSettingParam("VKPLActive") == "1") { this.Active = true; } else { this.Active = false; }
		}


		public class APIBRWC {
			private BackWin TechF = null;

			public event EventHandler<Twident_Status> Ev_GlobalStatus;

			public EdgeOptions BRWoptions = null; // -- Опции запуска
			public EdgeDriver BRWdriver = null; // -- Сам браузер
			private WebDriverWait BRWwait = null;
			public EdgeDriverService BRWdriverservice = null;
			//public var BRWoptions = UndetectedChromeDriver.Create(driverExecutablePath: await new ChromeDriverInstaller().Auto());
			//public EdgeDriver BRWdriver = null;
			//public EdgeDriverService BRWdriverservice = null;
			public bool Work = false;

			public APIBRWC(BackWin backWin) {
				TechF = backWin;
				InitBRW(false);
			}


			// -- Инициализация параметров браузера
			public void InitBRW(bool isShow) {
				this.BRWoptions = new EdgeOptions();
				if (!isShow) { this.BRWoptions.AddArgument("--headless");
				} else { this.BRWoptions.AddArgument("--start-maximized"); }
				this.BRWoptions.AddArgument("--mute-audio");
				this.BRWoptions.AddArgument("--browser-test");
				this.BRWoptions.AddArgument("--bwsi");
				this.BRWoptions.AddArgument("--deny-permission-prompts");
				this.BRWoptions.AddArgument("--disable-background-media-suspend");
				this.BRWoptions.AddArgument("--disable-background-timer-throttling");
				this.BRWoptions.AddArgument("--disable-backgrounding-occluded-windows");
				this.BRWoptions.AddArgument("--disable-breakpad");
				this.BRWoptions.AddArgument("--disable-crash-reporter");
				//this.BRWoptions.AddArgument("");
				//this.BRWoptions.AddArgument("");
				//this.BRWoptions.AddArgument("");
				//this.BRWoptions.AddArgument("");


				this.BRWdriverservice = EdgeDriverService.CreateDefaultService();
				this.BRWdriverservice.HideCommandPromptWindow = true;
				this.BRWoptions.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/113.0.1774.57");
				this.BRWoptions.BrowserVersion = "113.0.1774.57";
				//this.BRWoptions.AcceptInsecureCertificates = true;
			}


			// -- Запуск браузера --
			public bool StartBRW() {
				try {
					this.BRWdriver = new EdgeDriver(this.BRWdriverservice, this.BRWoptions);
				} catch (Exception ex) {
					Ev_GlobalStatus?.Invoke(this, new Twident_Status(3, "Ошибка сервиса VKPL", "Ошибка открытия браузера EDGE: " + ex.Message + ".\nВнутрення ошибка - " + ex.InnerException.Message, true));
					TechF.TechFuncs.LogDH("Ошибка открытия браузера EDGE: " + ex.Message + "; внутренняя ошибка - " + ex.InnerException.Message);
					this.BRWdriver.Quit();
					return false;
				}
				this.Work = true;
				Task.Factory.StartNew(() => MonitorBRW());
				return true;
			}

			// -- Остановка браузера --
			public void CloseBRW() {
				this.Work = false;
				this.BRWdriver.Quit();
			}


			// -- Отслеживание закрвтия браузера --
			private void MonitorBRW() {
				while (this.Work) {
					try {
						string str = this.BRWdriver.CurrentWindowHandle;
					} catch (Exception) {
						this.Work = false;
						bool VKPLwork = TechF.VKPL.Chat.Work;
						Ev_GlobalStatus?.Invoke(this, new Twident_Status(2, "Ошибка сервиса VKPL", "Браузер закрылся внешним способом. Идёт перезапуск...", true));
						if (VKPLwork) { TechF.VKPL.Chat.Close(); }
						this.StartBRW();
						if (VKPLwork) { TechF.VKPL.Chat.Connect(); }
						Ev_GlobalStatus?.Invoke(this, new Twident_Status(0, "VKPL перезапущен", null, false));
					}
					Thread.Sleep(3000);
				}
			}


			// -- Запуск браузера с показом для входа пользователя --
			public bool OpenBrowserforAuthorization() {
				InitBRW(true);
				if (StartBRW()) {
					this.BRWdriver.Navigate().GoToUrl("https://vkplay.live/");
					return true;
				} else { return false; }
			}


			// -- Получение кука с авторизацией для фоновой авторизации --
			public bool GetAuthCookie() {
				OpenQA.Selenium.Cookie authcookie = null;
				try { authcookie = BRWdriver.Manage().Cookies.GetCookieNamed("auth");
				} catch (Exception ex) { return false; }			
				if (authcookie != null) {
					TechF.db.SettingsVKPLT.UpdateSetting("Bot_AuthCode", authcookie.Value + ";" + authcookie.Expiry.ToString());
					return true;
				} else { return false; }
			}


			// -- Получение ника вошедшего аккаунта --
			public bool GetLoginNick() {
				IWebElement iwel = null;
				try { iwel = BRWdriver.FindElement(By.ClassName("ProfileMenu_name_YFbJZ"));
				} catch (Exception ex) { return false; }
				TechF.db.SettingsVKPLT.UpdateSetting("Login", iwel.Text.ToLower());
				TechF.db.SettingsVKPLT.UpdateSetting("LoginDisp", iwel.Text);
				return true;
			}


			// -- Остановка стрима для удаление бессмысленной нагрузки --
			public void StartStopStream() {
				IWebElement iwel = null;
				try {
					iwel = this.BRWdriver.FindElement(By.ClassName("Player_player_J3sxc"));
				} catch (Exception ex) { return; }
				new Actions(this.BRWdriver).MoveToElement(iwel).Pause(TimeSpan.FromMilliseconds(200)).Click(iwel).Perform(); // -- Наведение на плеер и нажатие на него
			}


			// -- Изменение разрешения стрима --
			public void ResolutionSet(bool SkipPlayer) {
				IWebElement iwel = null;
				ReadOnlyCollection<IWebElement> iwell = null;
				ShadowRoot vkplayer = null;
				if (!SkipPlayer) {
					try { // -- Поиск внешнего плеера
						iwel = this.BRWdriver.FindElement(By.ClassName("Player_player_J3sxc"));
					} catch (Exception ex) { return; }
					new Actions(this.BRWdriver).MoveToElement(iwel).Pause(TimeSpan.FromMilliseconds(200)).Perform();
				}

				try { // -- Плеер
					iwel = this.BRWdriver.FindElement(By.TagName("vk-video-player"));
				} catch (Exception ex) { return; }
				vkplayer = (ShadowRoot)this.BRWdriver.ExecuteScript("return arguments[0].shadowRoot", iwel); 

				try { // -- Кнопка настроек
					iwel = vkplayer.FindElement(By.ClassName("v-1qf7s4g"));
				} catch (Exception ex) { return; }
				new Actions(this.BRWdriver).MoveToElement(iwel).Pause(TimeSpan.FromMilliseconds(200)).Click(iwel).Perform();
				try { // - Контейнер кнопок меню настроек
					iwel = vkplayer.FindElement(By.ClassName("v-17mht85"));
				} catch (Exception ex) { return; }
				try { // - Кнопка разрешений
					iwel = iwel.FindElement(By.ClassName("menu-desktop"));
				} catch (Exception ex) { return; }
				new Actions(this.BRWdriver).MoveToElement(iwel).Pause(TimeSpan.FromMilliseconds(200)).Click(iwel).Perform();
				
				try { // -- Кнопки разрешений, и выбор минимального разрешения
					iwell = vkplayer.FindElements(By.ClassName("item-quality"));
				} catch (Exception ex) { return; }
				for (int i = iwell.Count - 5; i >= 0; i--) {
					string str = iwell[i].GetAttribute("data-value");
					if (str.Contains("p")) {
						iwel = iwell[i];
						break;
					}
				}
				new Actions(this.BRWdriver).Click(iwel).Perform(); // -- Клик по кнопке минимального разрешения

			}


			// --- Получение сообщения по айдишнику (со смещением с конца) --
			/*public IWebElement GetChatMessage(int Offset = 0) {
				IWebElement iwel = null;
				try {
					iwel = this.BRWdriver.FindElements(By.ClassName("ChatMessage_message_r1jzC")).ElementAt(GetMessagesCount() - Offset - 1);
				} catch (Exception ex) { return null; }
				return iwel;
			}*/

			// -- Получение количества сообщений --
			public int GetMessagesCount() {
				ReadOnlyCollection<IWebElement> iwell = null;
				try {
					iwell = this.BRWdriver.FindElements(By.ClassName("ChatMessage_message_r1jzC"));
				} catch (Exception ex) { return 0; }
				return iwell.Count;
			}

			// -- Получение всех сообщений --
			/*public ReadOnlyCollection<IWebElement> GetAllMessages() {
				ReadOnlyCollection<IWebElement> iwell = null;
				try {
					iwell = this.BRWdriver.FindElements(By.ClassName("ChatMessage_message_r1jzC"));
				} catch (Exception ex) { return null; }
				return iwell;
			}*/

			// -- Высчитывание следующего сообщения после указаного айдишника --
			/*public IWebElement GetNextChatMessage(string idElement) {
				IWebElement iwel = null;
				try {
					iwel = this.BRWdriver.FindElements(By.ClassName("ChatMessage_message_r1jzC")).ElementAt(GetMessagesCount() );
				} catch (Exception ex) { return null; }
				return iwel;
			}*/

			// -- Получения html-кода грида с сообщениями --
			public List<string> GetChatHTML() {
				List<string> listout = new List<string>();
				string chatstr = null;
				string[] chatmass = null;
				string strtmp = null;
				int welcome = 0;
				try {
					chatstr = this.BRWdriver.FindElement(By.ClassName("ReactVirtualized__Grid__innerScrollContainer")).GetAttribute("innerHTML");
				} catch (Exception ex) { return null; }

				chatmass = chatstr.Split(new string[] { "<div class=\"\" style=" }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < chatmass.Length; i++) {
					if (!chatmass[i].Contains("ChatBoxBase_welcome_Cee0j") && !chatmass[i].Contains("ChatBoxBase_infoMessage_P0tOg")) {
						strtmp = chatmass[i].Substring(chatmass[i].IndexOf("<div class=\"ChatMessage_publishTime_EzWt_"));
						listout.Add(strtmp.Substring(0, strtmp.IndexOf("<div class=\"ChatMessage_tooltip_rMCUC")));
					}
				}
				return listout;
			}

			// -- Определение уведомления об неотправке сообщения --
			public bool GetWarningUnMessage() {
				try {
					IWebElement iwel = this.BRWdriver.FindElement(By.ClassName("IdenticalMessageWarning_root_kcCV_"));
				} catch (Exception ex) { return false; }
				return true;
			}

			
			// -- Попытка предотвращения засыппания браузера (функцию переименовать, добавить фокус на поле ввода и нажатие на кнопку лайва) --
			public void StartStopStreamBackground() {
				IWebElement iwel = null;
				ShadowRoot vkplayer = null;
				while (TechF.VKPL.Chat.StreamWork) {
					Thread.Sleep(300000);
					try { // -- Плеер
						iwel = this.BRWdriver.FindElement(By.TagName("vk-video-player"));
					} catch (Exception ex) { return; }
					vkplayer = (ShadowRoot)this.BRWdriver.ExecuteScript("return arguments[0].shadowRoot", iwel);
					try { // -- Кнопка лайва
						iwel = vkplayer.FindElements(By.ClassName("v-1fkqq1h"))[1];
					} catch (Exception ex) { return; }
					new Actions(this.BRWdriver).MoveToElement(iwel).Pause(TimeSpan.FromMilliseconds(200)).Click(iwel).Perform();
					//this.StartStopStream();
					Thread.Sleep(3000);
					this.StartStopStream();
				}
			}

		}



		//public class APIC {
		//	private BackWin TechF = null;

		//	// -- Данные авторизации --
		//	private string Client_Id = "frnj4piu30ipfdkdfwz8i8zr1p990m";
		//	private string Client_Secret = "odrmykw8nj2bwetdvbbzk4ecol9i1d";

		//	public bool useDefToken = false;
		//	public bool tokens_work = false;

		//	public event EventHandler<Twident_Status> Ev_GlobalStatus;
		//	public event EventHandler<Twident_Null> Ev_LoginBotD;
		//	public event EventHandler<Twident_Null> Ev_LoginChD;


		//	// -- Стандартные аунтентификационные данные
		//	/*private string def_authCode = null;
		//	private string def_AuthCode {
		//		get { return def_authCode; }
		//		set	{ def_authCode = value;
		//			TechF.db.SettingsT.UpdateSetting("Def_AuthCode", value);
		//		}
		//	}*/

		//	private string def_access_Token = null;
		//	private string def_Access_Token {
		//		get { return def_access_Token; }
		//		set
		//		{
		//			def_access_Token = value;
		//			TechF.db.SettingsT.UpdateSetting("Def_Access_Token", value);
		//		}
		//	}

		//	private string def_refresh_Token = null;
		//	private string def_Refresh_Token {
		//		get { return def_refresh_Token; }
		//		set
		//		{
		//			def_refresh_Token = value;
		//			TechF.db.SettingsT.UpdateSetting("Def_Refresh_Token", value);
		//		}
		//	}

		//	private int def_tokenTime = 0;
		//	private int def_TokenTime {
		//		get { return def_tokenTime; }
		//		set
		//		{
		//			def_tokenTime = value;
		//			TechF.db.SettingsT.UpdateSetting("Def_TokenTime", value.ToString());
		//		}
		//	}



		//	// -- Аутентификационные данные бота
		//	private string bot_authCode = null;
		//	private string bot_AuthCode {
		//		get { return bot_authCode; }
		//		set
		//		{
		//			bot_authCode = value;
		//			TechF.db.SettingsT.UpdateSetting("Bot_AuthCode", value);
		//		}
		//	}

		//	private string bot_access_Token = null;
		//	private string bot_Access_Token {
		//		get { return bot_access_Token; }
		//		set
		//		{
		//			bot_access_Token = value;
		//			TechF.db.SettingsT.UpdateSetting("Bot_Access_Token", value);
		//		}
		//	}

		//	private string bot_refresh_Token = null;
		//	private string bot_Refresh_Token {
		//		get { return bot_refresh_Token; }
		//		set
		//		{
		//			bot_refresh_Token = value;
		//			TechF.db.SettingsT.UpdateSetting("Bot_Refresh_Token", value);
		//		}
		//	}

		//	private int bot_tokenTime = 0;
		//	private int bot_TokenTime {
		//		get { return bot_tokenTime; }
		//		set
		//		{
		//			bot_tokenTime = value;
		//			TechF.db.SettingsT.UpdateSetting("Bot_TokenTime", value.ToString());
		//		}
		//	}


		//	private string ch_authCode = null;
		//	private string ch_AuthCode {
		//		get { return ch_authCode; }
		//		set
		//		{
		//			ch_authCode = value;
		//			TechF.db.SettingsT.UpdateSetting("Channel_AuthCode", value);
		//		}
		//	}

		//	private string ch_access_Token = null;
		//	private string ch_Access_Token {
		//		get { return ch_access_Token; }
		//		set
		//		{
		//			ch_access_Token = value;
		//			TechF.db.SettingsT.UpdateSetting("Channel_Access_Token", value);
		//		}
		//	}

		//	private string ch_refresh_Token = null;
		//	private string ch_Refresh_Token {
		//		get { return ch_refresh_Token; }
		//		set
		//		{
		//			ch_refresh_Token = value;
		//			TechF.db.SettingsT.UpdateSetting("Channel_Refresh_Token", value);
		//		}
		//	}

		//	private int ch_tokenTime = 0;
		//	private int ch_TokenTime {
		//		get { return ch_tokenTime; }
		//		set
		//		{
		//			ch_tokenTime = value;
		//			TechF.db.SettingsT.UpdateSetting("Channel_TokenTime", value.ToString());
		//		}
		//	}


		//	private List<string> ScopesforReq = null;
		//	private List<string> ScopesforChat = null;
		//	RestClient ReqClient = null;


		//	public APIC(BackWin backWin) {
		//		this.TechF = backWin;
		//		this.ScopesforReq = new List<string>();
		//		this.ScopesforChat = new List<string>();
		//		ReqClient = new RestClient("https://api.twitch.tv/helix");

		//		ReqClient.AddDefaultHeader("Client-Id", Client_Id);
		//		ReqClient.AddDefaultHeader("User-Agent", "Twidibot/" + TechF.AppVersion);

		//		if (TechF.TechFuncs.GetSettingParam("Bot_Access_Token") == "false") { useDefToken = true; }

		//		//def_authCode = TechF.db.SettingsT.GetParam("Def_AuthCode");
		//		//def_access_Token = TechF.db.SettingsT.GetParam("Def_Access_Token");
		//		//def_refresh_Token = TechF.db.SettingsT.GetParam("Def_Refresh_Token");
		//		//def_tokenTime = Convert.ToInt32(TechF.db.SettingsT.GetParam("Def_TokenTime"));

		//		//bot_authCode = TechF.db.SettingsT.GetParam("Bot_AuthCode");
		//		//bot_access_Token = TechF.db.SettingsT.GetParam("Bot_Access_Token");
		//		//bot_refresh_Token = TechF.db.SettingsT.GetParam("Bot_Refresh_Token");
		//		//bot_tokenTime = Convert.ToInt32(TechF.db.SettingsT.GetParam("Bot_TokenTime"));

		//		//ch_authCode = TechF.db.SettingsT.GetParam("Channel_AuthCode");
		//		//ch_access_Token = TechF.db.SettingsT.GetParam("Channel_Access_Token");
		//		//ch_refresh_Token = TechF.db.SettingsT.GetParam("Channel_Refresh_Token");
		//		//ch_tokenTime = Convert.ToInt32(TechF.db.SettingsT.GetParam("Channel_TokenTime"));

		//		//def_authCode = TechF.db.SettingsList.Find(x => x.id == 10).Param;
		//		def_access_Token = TechF.db.SettingsList.Find(x => x.id == 11).Param;
		//		def_refresh_Token = TechF.db.SettingsList.Find(x => x.id == 12).Param;
		//		def_tokenTime = Convert.ToInt32(TechF.db.SettingsList.Find(x => x.id == 13).Param);

		//		//bot_authCode = TechF.db.SettingsList.Find(x => x.id == 14).Param;
		//		bot_access_Token = TechF.db.SettingsList.Find(x => x.id == 15).Param;
		//		bot_refresh_Token = TechF.db.SettingsList.Find(x => x.id == 16).Param;
		//		bot_tokenTime = Convert.ToInt32(TechF.db.SettingsList.Find(x => x.id == 17).Param);

		//		//ch_authCode = TechF.db.SettingsList.Find(x => x.id == 18).Param;
		//		ch_access_Token = TechF.db.SettingsList.Find(x => x.id == 19).Param;
		//		ch_refresh_Token = TechF.db.SettingsList.Find(x => x.id == 20).Param;
		//		ch_tokenTime = Convert.ToInt32(TechF.db.SettingsList.Find(x => x.id == 21).Param);
		//	}


		//	private void ScopesFill() {
		//		ScopesforReq.Add("bits:read");
		//		ScopesforReq.Add("channel:manage:broadcast");
		//		ScopesforReq.Add("channel:manage:redemptions");
		//		ScopesforReq.Add("channel:read:editors");
		//		ScopesforReq.Add("channel:read:hype_train");
		//		ScopesforReq.Add("channel:read:polls");
		//		ScopesforReq.Add("channel:read:redemptions");
		//		ScopesforReq.Add("channel:read:subscriptions");
		//		ScopesforReq.Add("moderation:read");
		//		ScopesforReq.Add("moderator:manage:banned_users");
		//		//ScopesforReq.Add("moderator:read:blocked_terms");
		//		//ScopesforReq.Add("moderator:manage:blocked_terms");
		//		//ScopesforReq.Add("moderator:manage:automod");
		//		ScopesforReq.Add("moderator:read:automod_settings");
		//		//ScopesforReq.Add("moderator:manage:automod_settings");
		//		ScopesforReq.Add("moderator:read:chat_settings");
		//		ScopesforReq.Add("moderator:manage:chat_settings");
		//		//ScopesforReq.Add("user:edit");
		//		//ScopesforReq.Add("user:edit:follows");
		//		//ScopesforReq.Add("user:read:broadcast");
		//		ScopesforReq.Add("user:read:follows");
		//		//ScopesforReq.Add("user:read:subscriptions");

		//		/*
		//		ScopesforChat.Add("channel_check_subscription");
		//		ScopesforChat.Add("channel_commercial");
		//		ScopesforChat.Add("channel_editor");
		//		//ScopesforChat.Add("channel_feed_edit");
		//		//ScopesforChat.Add("channel_feed_read");
		//		ScopesforChat.Add("channel_read");
		//		//ScopesforChat.Add("channel_stream");
		//		ScopesforChat.Add("channel_subscriptions");
		//		ScopesforChat.Add("chat_login");
		//		//ScopesforChat.Add("collections_edit:");
		//		//ScopesforChat.Add("communities_edit");
		//		//ScopesforChat.Add("communities_moderate");
		//		ScopesforChat.Add("user_read");
		//		ScopesforChat.Add("user_blocks_edit");
		//		ScopesforChat.Add("user_blocks_read");
		//		//ScopesforChat.Add("user_follows_edit");
		//		ScopesforChat.Add("user_subscriptions");
		//		//ScopesforChat.Add("viewing_activity_read");
		//		*/

		//		ScopesforChat.Add("channel:moderate");
		//		ScopesforChat.Add("chat:edit");
		//		ScopesforChat.Add("chat:read");
		//		ScopesforChat.Add("whispers:read");
		//		ScopesforChat.Add("whispers:edit");
		//	}

		//	private void ScopesClear() {
		//		ScopesforReq.Clear();
		//		ScopesforChat.Clear();
		//	}



		//	// -- Проверка токена доступа --
		//	public bool ValidateToken(string atoken = null, bool isChannel = false) {
		//		string strres = null;
		//		string strerr = null;
		//		bool reqsuccess = false;
		//		string reqstatus = null;
		//		string Access_Tokenl = null;

		//		if (atoken != null) { Access_Tokenl = atoken; } else {
		//			if (useDefToken) { Access_Tokenl = def_Access_Token; } else {
		//				if (isChannel) { Access_Tokenl = ch_Access_Token; } else { Access_Tokenl = bot_Access_Token; }
		//			}
		//		}

		//		RestClient ReqClientl = new RestClient("https://id.twitch.tv");

		//		RestRequest req = new RestRequest("oauth2/validate", Method.Get);

		//		req.AddHeader("Authorization", "Bearer " + Access_Tokenl);
		//		req.AddHeader("User-Agent", "Twidibot/" + TechF.AppVersion);


		//		// -- Непосредственно выполнение запроса
		//		try {
		//			Task<RestResponse> reqresstream = ReqClientl.GetAsync(req);

		//			RestResponse resstream = reqresstream.Result;

		//			strres = resstream.Content;
		//			reqsuccess = resstream.IsSuccessful;
		//			//reqstatus = resstream.ResponseStatus.ToString();
		//			if (!reqsuccess) { reqstatus = TechF.TechFuncs.SearchinJson(strres, "error"); }

		//		} catch (Exception ex) {
		//			TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка проверки Access Token: " + ex.Message + " | " + ex.InnerException.Message);
		//			return false;
		//		}


		//		// -- Обработка ошибок
		//		if (!reqsuccess) {
		//			strerr = TechF.TechFuncs.SearchinJson(strres, "message");

		//			if (strerr == "invalid access token") {
		//				TechF.TechFuncs.LogDH("Twitch API - Запрос проверки Access Token: Токен недействителен");
		//			} else {
		//				TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка проверки Access Token: " + reqstatus + " | " + strerr);
		//			}
		//			return false;
		//		} else {
		//			if (useDefToken) {
		//				def_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", "}"));
		//			} else {
		//				if (isChannel) {
		//					ch_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", "}"));
		//				} else {
		//					bot_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", "}"));
		//				}
		//			}
		//			TechF.TechFuncs.LogDH("Twitch API - Запрос проверки Access Token: Успешно");
		//			return true;
		//		}
		//	}

		//	public void ValidateandRefreshAllTokens() {
		//		if (useDefToken) {
		//			if (!ValidateToken()) {
		//				RefreshToken();
		//			}
		//		} else {
		//			if (bot_Access_Token != null) {
		//				if (!ValidateToken(null, false)) {
		//					RefreshToken(false);
		//				}
		//			}
		//			if (ch_Access_Token != null) {
		//				if (!ValidateToken(null, true)) {
		//					RefreshToken(true);
		//				}
		//			}
		//		}
		//	}



		//	// -- Получение обычного аксесс токена --
		//	/*private string GetToken() {
		//		string strout = "";
		//		var client = new TinyRestClient(new HttpClient(), "https://id.twitch.tv");
		//		try {
		//			StringBuilder sb = new StringBuilder();
		//			byte[] buf = new byte[8192];

		//			StringBuilder scopes = new StringBuilder();
		//			for (int i = 0; i < ScopesforReq.Count; i++) {
		//				if (i > 0) { scopes.Append("%20"); }
		//				scopes.Append(ScopesforReq.ElementAt<string>(i));
		//			}

		//			var response = client.PostRequest("oauth2/token").
		//				AddQueryParameter("client_id", Client_Id).
		//				AddQueryParameter("client_secret", Client_Secret).
		//				AddQueryParameter("grant_type", "client_credentials").
		//				AddQueryParameter("scopes", scopes.ToString()).
		//				AddQueryParameter("User-Agent", "Twidibot/" + TechF.AppVersion).
		//				ExecuteAsStreamAsync();

		//			Stream resStream = response.Result;
		//			int count = 0;
		//			do {
		//				count = resStream.Read(buf, 0, buf.Length);
		//				if (count != 0) { sb.Append(Encoding.UTF8.GetString(buf, 0, count)); }
		//			} while (count > 0);

		//			strout = sb.ToString();
		//			strout = TechF.TechFuncs.SearchinJson(strout, "access_token");

		//			Access_Token = strout;

		//		} catch (Exception ex) {
		//			strout = ex.Message;
		//		}
		//		TechF.TechFuncs.LogDH("Сгенерирован Twitch-запрос получения обычного Access Token");
		//		return strout;
		//	}*/



		//	// -- Получение нормального аксесс токена, после авторизации --
		//	public bool GetTokenAuth(bool isChannel = false) {
		//		string strres = null;
		//		string strerr = null;
		//		bool reqsuccess = false;
		//		string reqstatus = null;
		//		string AuthCodel = null;
		//		string Redirectl = null;
		//		string acl = null;

		//		if (isChannel) { AuthCodel = ch_AuthCode; } else { AuthCodel = bot_AuthCode; }
		//		if (isChannel) { Redirectl = "channel"; } else { Redirectl = "bot"; }

		//		RestClient ReqClientl = new RestClient("https://id.twitch.tv");

		//		RestRequest req = new RestRequest("oauth2/token?client_id=" + Client_Id + "&client_secret=" + Client_Secret + "&code=" + AuthCodel + "&grant_type=authorization_code&redirect_uri=https://twidi.localhost:8138/twitch/auth/" + Redirectl, Method.Post);

		//		req.AddHeader("User-Agent", "Twidibot/" + TechF.AppVersion);


		//		// -- Непосредственно выполнение запроса
		//		try {
		//			Task<RestResponse> reqresstream = ReqClientl.PostAsync(req);

		//			RestResponse resstream = reqresstream.Result;

		//			strres = resstream.Content;
		//			reqsuccess = resstream.IsSuccessful;
		//			//reqstatus = resstream.ResponseStatus.ToString();
		//			if (!reqsuccess) { reqstatus = TechF.TechFuncs.SearchinJson(strres, "error"); }

		//		} catch (Exception ex) {
		//			TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка получения Access Token: " + ex.Message + " | " + ex.InnerException.Message);
		//			return false;
		//		}


		//		// -- Обработка ошибок
		//		if (!reqsuccess) {
		//			strerr = TechF.TechFuncs.SearchinJson(strres, "message");
		//			TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка получения Access Token: " + reqstatus + " | " + strerr);
		//			return false;
		//		}

		//		acl = TechF.TechFuncs.SearchinJson(strres, "access_token");
		//		if (ValidateToken(acl)) {
		//			bot_access_Token = acl;
		//			if (isChannel) {
		//				ch_Access_Token = acl;
		//				ch_Refresh_Token = TechF.TechFuncs.SearchinJson(strres, "refresh_token");
		//				//ch_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", ","));
		//			} else {
		//				bot_Access_Token = acl;
		//				bot_Refresh_Token = TechF.TechFuncs.SearchinJson(strres, "refresh_token");
		//				//bot_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", ","));
		//			}
		//			TechF.TechFuncs.LogDH("Twitch API - Запрос получения Access Token: Успешно");
		//			return true;
		//		} else {
		//			if (isChannel) {
		//				ch_Access_Token = null;
		//				ch_Refresh_Token = null;
		//				//ch_TokenTime = null;
		//			} else {
		//				bot_Access_Token = null;
		//				bot_Refresh_Token = null;
		//				//bot_TokenTime = null;
		//			}
		//			TechF.TechFuncs.LogDH("Twitch API - Ошибка проверки Access Token при его получении: Токен недействителен");
		//			return false;
		//		}
		//	}



		//	// -- Обновление аксесс токена --
		//	public bool RefreshToken(bool isChannel = false) {
		//		string strres = null;
		//		string strerr = null;
		//		bool reqsuccess = false;
		//		string reqstatus = null;
		//		string Refresh_Tokenl = null;
		//		string atl = null;

		//		if (useDefToken) { Refresh_Tokenl = def_Refresh_Token; } else {
		//			if (isChannel) { Refresh_Tokenl = ch_Refresh_Token; } else { Refresh_Tokenl = bot_Refresh_Token; }
		//		}

		//		RestClient ReqClientl = new RestClient("https://id.twitch.tv");

		//		RestRequest req = new RestRequest("oauth2/token?client_id=" + Client_Id + "&client_secret=" + Client_Secret + "&grant_type=refresh_token" + "&refresh_token=" + Refresh_Tokenl, Method.Post);

		//		req.AddHeader("User-Agent", "Twidibot/" + TechF.AppVersion);


		//		// -- Непосредственно выполнение запроса
		//		try {
		//			Task<RestResponse> reqresstream = ReqClientl.PostAsync(req);

		//			RestResponse resstream = reqresstream.Result;

		//			strres = resstream.Content;
		//			reqsuccess = resstream.IsSuccessful;
		//			//reqstatus = resstream.ResponseStatus.ToString();
		//			if (!reqsuccess) { reqstatus = TechF.TechFuncs.SearchinJson(strres, "error"); }

		//		} catch (Exception ex) {
		//			TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка обновления Access Token: " + ex.Message + " | " + ex.InnerException.Message);
		//			return false;
		//		}


		//		// -- Обработка ошибок
		//		if (!reqsuccess) {
		//			strerr = TechF.TechFuncs.SearchinJson(strres, "message");

		//			switch (strerr) {
		//				case "Invalid refresh token":
		//				tokens_work = false;
		//				TechF.TechFuncs.LogDH("Twitch API - Ошибка обновления Access Token: неправильный токен обновления. Требуется новая авторизация");
		//				if (isChannel) {
		//					if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(3, "Ошибка доступа к Twitch API", "Требуется повторный вход в аккаунт канала", true)); }
		//				} else {
		//					if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(3, "Ошибка доступа к Twitch API", "Требуется повторный вход в аккаунт бота", true)); }
		//				}
		//				break;
		//				default:
		//				TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка обновления Access Token: " + reqstatus + " | " + strerr);
		//				break;
		//			}
		//			return false;
		//		}

		//		atl = TechF.TechFuncs.SearchinJson(strres, "access_token");
		//		if (ValidateToken(atl)) {
		//			if (useDefToken) {
		//				def_Access_Token = atl;
		//				def_Refresh_Token = TechF.TechFuncs.SearchinJson(strres, "refresh_token");
		//				//def_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", ","));
		//			} else {
		//				if (isChannel) {
		//					ch_Access_Token = atl;
		//					ch_Refresh_Token = TechF.TechFuncs.SearchinJson(strres, "refresh_token");
		//					//ch_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", ","));
		//				} else {
		//					bot_Access_Token = atl;
		//					bot_Refresh_Token = TechF.TechFuncs.SearchinJson(strres, "refresh_token");
		//					//bot_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", ","));
		//				}
		//			}
		//			TechF.TechFuncs.LogDH("Twitch API - Запрос получения Access Token: Успешно");
		//			return true;
		//		} else {
		//			if (isChannel) {
		//				ch_Access_Token = null;
		//				ch_Refresh_Token = null;
		//				//ch_TokenTime = null;
		//			} else {
		//				bot_Access_Token = null;
		//				bot_Refresh_Token = null;
		//				//bot_TokenTime = null;
		//			}
		//			return false;
		//		}
		//	}



		//	// -- Удаление аксесс токена --
		//	public bool RemoveToken(bool isChannel = false) {
		//		string strres = null;
		//		string strerr = null;
		//		bool reqsuccess = false;
		//		string reqstatus = null;
		//		string Access_Tokenl = null;

		//		if (useDefToken) { Access_Tokenl = def_Access_Token; } else {
		//			if (isChannel) { Access_Tokenl = ch_Access_Token; } else { Access_Tokenl = bot_Access_Token; }
		//		}

		//		RestClient ReqClientl = new RestClient("https://id.twitch.tv");

		//		RestRequest req = new RestRequest("oauth2/revoke?client_id=" + Client_Id + "&token=" + Access_Tokenl, Method.Post);

		//		req.AddHeader("User-Agent", "Twidibot/" + TechF.AppVersion);


		//		// -- Непосредственно выполнение запроса
		//		try {
		//			Task<RestResponse> reqresstream = ReqClientl.PostAsync(req);

		//			RestResponse resstream = reqresstream.Result;

		//			strres = resstream.Content;
		//			reqsuccess = resstream.IsSuccessful;
		//			//reqstatus = resstream.ResponseStatus.ToString();
		//			if (!reqsuccess) { reqstatus = TechF.TechFuncs.SearchinJson(strres, "error"); }

		//		} catch (Exception ex) {
		//			TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка удаления Access Token: " + ex.Message + " | " + ex.InnerException.Message);
		//			return false;
		//		}


		//		// -- Обработка ошибок
		//		if (!reqsuccess) {
		//			strerr = TechF.TechFuncs.SearchinJson(strres, "message");
		//			TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка удаления Access Token: " + reqstatus + " | " + strerr);
		//			return false;
		//		}

		//		if (isChannel) {
		//			ch_Access_Token = null;
		//			TechF.db.SettingsT.UpdateSetting("Channel_Access_Token", null);
		//		} else {
		//			bot_Access_Token = null;
		//			TechF.db.SettingsT.UpdateSetting("Bot_Access_Token", null);
		//		}
		//		TechF.TechFuncs.LogDH("Twitch API - Запрос удаления Access Token: Успешно");

		//		return true;
		//	}




		//	// -- Крафт ссылки для авторизации через браузер и открытие браузера пользователя с этой ссылкой
		//	public void AuthorizationviaBrowser(bool isChannel = false, bool AutoClose = false) {
		//		StringBuilder req = new StringBuilder("https://id.twitch.tv/oauth2/authorize", 200);

		//		// -- Крафт запроса
		//		if (isChannel) {
		//			req.Append("?client_id=" + Client_Id + "&redirect_uri=https://twidi.localhost:8138/twitch/auth/channel&response_type=code&scope=");
		//		} else {
		//			req.Append("?client_id=" + Client_Id + "&redirect_uri=https://twidi.localhost:8138/twitch/auth/bot&response_type=code&scope=");
		//		}

		//		// -- Сбор разрешений
		//		ScopesFill();
		//		for (int i = 0; i < ScopesforReq.Count; i++) {
		//			if (i > 0) { req.Append("%20"); }
		//			req.Append(ScopesforReq.ElementAt(i));
		//		}
		//		ScopesClear();
		//		// -- Генерация шифрованной строки с даннами, что планируем получать
		//		// тута да, функция генерации state
		//		//req.Append("&state=");

		//		TechF.TechFuncs.LogDH("Twitch API - Авторизация через браузер...");
		//		Process.Start(req.ToString());
		//	}


		//	// -- Крафт ссылки для получения OAuth-кода
		//	public string GetOAuthCode(bool isReturn = false) {
		//		StringBuilder req = new StringBuilder("https://id.twitch.tv/oauth2/authorize?client_id=" + Client_Id + "&redirect_uri=https://twitchapps.com/tmi/&response_type=token&scope=", 250);


		//		// -- Сбор разрешений
		//		ScopesFill();
		//		for (int i = 0; i < ScopesforChat.Count; i++) {
		//			if (i > 0) { req.Append("%20"); }
		//			req.Append(ScopesforChat.ElementAt(i));
		//		}
		//		ScopesClear();
		//		// -- Генерация шифрованной строки с даннами, что планируем получать
		//		// тута да, функция генерации state
		//		//req.Append("&state=");

		//		if (isReturn) {
		//			return req.ToString();
		//		} else {
		//			TechF.TechFuncs.LogDH("Twitch API - Получение OAuth через браузер...");
		//			Process.Start(req.ToString());
		//			return null;
		//		}
		//	}




		//	// -- Основная функция для запросов --
		//	public string ApiRequest(string RequstUrl, bool isChannel = false, string ReqType = "GET", string Json = null, string[,] Parameters = null, string[] Return_Params = null, string[,] Headers = null) {
		//		string strout = null;
		//		int Parameters_c = 0;
		//		int Headers_c = 0;
		//		int Return_Params_c = 0;
		//		string strerr = null;
		//		bool reqsuccess = false;
		//		string reqstatus = null;
		//		string Access_Tokenl = null;
		//		RestRequest req = null;

		//		// -- Некоторые настройки
		//		if (RequstUrl.StartsWith("/")) { RequstUrl = RequstUrl.Substring(1); }

		//		if (useDefToken) { Access_Tokenl = def_Access_Token; } else {
		//			if (isChannel) { Access_Tokenl = ch_Access_Token; } else { Access_Tokenl = bot_Access_Token; }
		//		}

		//		if (Parameters != null) {
		//			Parameters_c = Parameters.Length;
		//		}
		//		if (Headers != null) {
		//			Headers_c = Headers.GetLength(1);
		//		}
		//		if (Return_Params != null) {
		//			Return_Params_c = Return_Params.Length;
		//		}



		//		// -- Инициализация запроса
		//		switch (ReqType) {
		//			case "GET":
		//			req = new RestRequest(RequstUrl, Method.Get);
		//			break;
		//			case "POST":
		//			req = new RestRequest(RequstUrl, Method.Post);
		//			break;
		//			case "PATCH":
		//			req = new RestRequest(RequstUrl, Method.Patch);
		//			//req.AddHeader("Content-Type", "application/json");
		//			break;
		//			case "PUT":
		//			req = new RestRequest(RequstUrl, Method.Put);
		//			break;

		//			default:
		//			return "Неправильно указан метод запроса";
		//			//break;
		//		}

		//		// -- Добавление аксес токена, т.к. он может быть уникальным чуть ли не для каждого запроса
		//		req.AddHeader("Authorization", "Bearer " + Access_Tokenl);
		//		//req.

		//		// -- Добавление кастомных заголовков в запрос
		//		for (ushort i = 0; i < Headers_c; i++) {
		//			req.AddHeader(Headers[0, i], Headers[1, i]);
		//		}

		//		// -- Добавление параметров, если вдруг они указаны через массив, а не через реквестюрл
		//		for (ushort i = 0; i < Parameters_c; i++) {
		//			req.AddParameter(Parameters[0, i], Parameters[1, i]);
		//		}

		//		// -- Крафтим запрос в стиле CURL для вывода в лог
		//		StringBuilder strlog = new StringBuilder(ReqType + " " + RequstUrl + " -H 'Authorization: Bearer [скрыто]' -H 'Client-Id: " + Client_Id + "'");
		//		if (ReqType == "PATCH") {
		//			strlog.Append(" -H 'Content-Type: application/json'");
		//		}

		//		strlog.Append(" -H 'User-Agent: Twidibot/" + TechF.AppVersion + "'");

		//		for (ushort i = 0; i < Headers_c; i++) { // -- Добавление заголовков
		//			strlog.Append(" -H '" + Headers[0, i] + ": " + Headers[1, i] + "'");
		//		}

		//		TechF.TechFuncs.LogDH("Twitch API - Запрос: " + strlog);


		//		// -- Непосредственно выполнение запроса
		//		try {
		//			Task<RestResponse> reqresstream = null;

		//			switch (ReqType) {
		//				case "GET":
		//				reqresstream = ReqClient.ExecuteGetAsync(req);
		//				break;
		//				case "POST":
		//				reqresstream = ReqClient.PostAsync(req);
		//				break;
		//				case "PATCH":
		//				reqresstream = ReqClient.PatchAsync(req);
		//				break;
		//				case "PUT":
		//				reqresstream = ReqClient.PutAsync(req);
		//				break;
		//			}

		//			RestResponse resstream = reqresstream.Result;

		//			strout = resstream.Content;
		//			reqsuccess = resstream.IsSuccessful;
		//			//reqstatus = resstream.ResponseStatus.ToString();
		//			if (!reqsuccess) { reqstatus = TechF.TechFuncs.SearchinJson(strout, "error"); }

		//		} catch (Exception ex) {
		//			TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка запроса: " + ex.Message + " | " + ex.InnerException.Message);
		//		}


		//		// -- Обработка ошибок
		//		if (!reqsuccess) {
		//			strerr = TechF.TechFuncs.SearchinJson(strout, "message");

		//			switch (reqstatus) {

		//				case "Unauthorized":
		//				switch (strerr) {

		//					case "Invalid OAuth token":
		//					if (Access_Tokenl != null && Access_Tokenl != "") {
		//						TechF.TechFuncs.LogDH("Twitch API - Ошибка запроса: требуется обновление Access Token");
		//						if (!RefreshToken()) {
		//							TechF.TechFuncs.LogDH("Twitch API - Продолжение работы запросов невозможно");
		//							return null;
		//						}
		//						strout = ApiRequest(RequstUrl, isChannel, ReqType, Json, Parameters, Return_Params, Headers);
		//					} else {
		//						TechF.TechFuncs.LogDH("Twitch API - Ошибка запроса: Отсутствует Access Token");
		//						if (!GetTokenAuth()) {
		//							AuthorizationviaBrowser(isChannel);
		//							strout = "false";
		//						}
		//					}
		//					break;

		//					case "incorrect user authorization":
		//					TechF.TechFuncs.LogDH("Twitch API - Ошибка запроса: Неправильная авторизация - скорее всего, она прошла под не тем аккаунтом, от имени которого проходит запрос");
		//					//AuthorizationviaBrowser(isChannel);
		//					strout = "false";
		//					break;

		//					default:
		//					TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка запроса: " + reqstatus + " | " + strerr);
		//					break;
		//				}
		//				break;

		//				//case "Bad Request":

		//				//break;

		//				default:
		//				TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка запроса: " + reqstatus + " | " + strerr);
		//				break;
		//			}
		//		}


		//		if (strout != "false") {
		//			// -- Крафтим строку с выбранными параметрами, 
		//			if (Return_Params_c > 0) {
		//				StringBuilder sbl = new StringBuilder(100);
		//				for (ushort i = 0; i < Return_Params_c; i++) {
		//					if (i == 0) {
		//						sbl.Append(TechF.TechFuncs.SearchinJson(strout, Return_Params[i]));
		//					} else {
		//						sbl.Append(";" + TechF.TechFuncs.SearchinJson(strout, Return_Params[i]));
		//					}
		//				}
		//				strout = sbl.ToString();
		//				TechF.TechFuncs.LogDH("Twitch API - Ответ на запрос (только указанные параметры): " + strout);
		//			} else {
		//				TechF.TechFuncs.LogDH("Twitch API - Ответ на запрос: " + strout);
		//			}
		//		}
		//		return strout;
		//	}



		//	// -- Функция разбора данных после вход в аккаунт через браузер
		//	public string[] FLogin(string reqParams) {
		//		string strtmp = reqParams.Substring(reqParams.IndexOf("code=") + 5);
		//		bool isChannel = false;
		//		string[] errml = new string[2];
		//		string strlog = null;
		//		string strlogerr = null;
		//		if (reqParams.StartsWith("channel")) { isChannel = true; }
		//		if (isChannel) {
		//			strlog = "API Twitch - Авторизация канала: ";
		//			strlogerr = "API Twitch - Ошибка авторизация канала: ";
		//		} else {
		//			strlog = "API Twitch - Авторизация бота: ";
		//			strlogerr = "API Twitch - Ошибка авторизация бота: ";
		//		}
		//		TechF.TechFuncs.LogDH(strlog + "Запуск разбора авторизации канала через браузер...");
		//		if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(0, "Выделение токена авторизации...", null, false)); }

		//		if (reqParams.Contains("code=")) {
		//			if (isChannel) {
		//				ch_AuthCode = strtmp.Substring(0, strtmp.IndexOf("&scope=")); // -- Сохраняем код авторизации
		//			} else {
		//				bot_AuthCode = strtmp.Substring(0, strtmp.IndexOf("&scope="));
		//			}

		//			TechF.TechFuncs.LogDH(strlog + "получение токена аунтентификации...");
		//			if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(0, "Получение токена аунтентификации...", null, null)); }
		//			if (!GetTokenAuth(isChannel)) {
		//				errml[0] = "Ошибка проверки на правильность полученного токена авторизации";
		//				errml[1] = "Можно попробовать ещё раз авторизоваться. Иначе пишите разрабу, или смотрите в лог";
		//				TechF.TechFuncs.LogDH(strlogerr + "Полученный токен авторизации не подтверждён");
		//				if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(3, "Ошибка получения токена аунтентификации", null, true)); }
		//				return errml;
		//			}

		//			TechF.TechFuncs.LogDH(strlog + "Получение данных аккаунта...");
		//			if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(0, "Получение данных аккаунта...", null, null)); }
		//			if (!Req_UpdateBotChannelInfo_login(isChannel)) {
		//				errml[0] = "Ошибка получения информации об аккаунте";
		//				errml[1] = "Можно попробовать ещё раз авторизоваться. Иначе пишите разрабу, или смотрите в лог";
		//				TechF.TechFuncs.LogDH(strlogerr + "Непонятная ошибка получения данных об аккаунте");
		//				if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(3, "Ошибка получения данных аккаунта", null, true)); }
		//				return errml;
		//			} else {
		//				TechF.TechFuncs.LogDH(strlog + "Успешно");
		//				if (TechF.TechFuncs.GetSettingParam("Bot_Access_Token") == "true") { useDefToken = false; } else { useDefToken = true; }
		//				if (!isChannel) {
		//					//GetOAuthCode();
		//					if (Ev_LoginBotD != null) { Ev_LoginBotD(this, new Twident_Null()); }
		//					if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(0, "Авторизация в процессе...", "Вставте OAuth с открывшегося сайта", null)); }
		//				} else {
		//					if (Ev_LoginChD != null) { Ev_LoginChD(this, new Twident_Null()); }
		//					if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(1, "Авторизация прошла успешно", null, false)); }
		//				}
		//				return null;
		//			}
		//		} else {
		//			TechF.TechFuncs.LogDH(strlogerr + "Не найден код авторизации");
		//			if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(3, "Ошибка авторизации", "Не найден код авторизации. Попробуйте снова, иначе обратитесь к разработчику", true)); }
		//			if (isChannel) {
		//				strtmp = strtmp.Substring(8);
		//			} else {
		//				strtmp = strtmp.Substring(3);
		//			}
		//			return strtmp.Split('&');
		//		}
		//	}



		//	// -- Обновление секрета приложения
		//	public bool UpdateClientSecret() {


		//		//TechF.TechFuncs.LogDH("Twitch API - Запрос обновления секрета приложения: ");
		//		return false;
		//	}


		//	// -- Определение фолоу на канал подключения
		//	public bool Req_BotFollowtoChannel() {
		//		if (TechF.TechFuncs.GetSettingParam("BotChannel_isOne") == "0") {
		//			string loginl = TechF.TechFuncs.GetSettingParam("Login");
		//			string channell = TechF.TechFuncs.GetSettingParam("Channel");
		//			string loginid = TechF.TechFuncs.GetSettingParam("LoginId");
		//			string res = null;
		//			string cursor = null;
		//			if (loginl != null && channell != null) {

		//				//string botid = ApiRequest("users?login=" + loginl, false, "GET", null, null, new string[] { "id" });

		//				while (true) {

		//					if (cursor != null || cursor != "") {
		//						res = ApiRequest("users/follows?from_id=" + loginid + "&first=50");
		//					} else {
		//						res = ApiRequest("users/follows?from_id=" + loginid + "&first=10&pagination=" + cursor);
		//					}

		//					if (res.Contains("\"to_login\":\"" + channell + "\"")) {
		//						return true;
		//					} else {
		//						cursor = TechF.TechFuncs.SearchinJson(res, "pagination", "{", "}");
		//						cursor = TechF.TechFuncs.SearchinJson(cursor, "cursor");
		//						if (cursor != null || cursor != "") {
		//							return false;
		//						} else {
		//							continue;
		//						}
		//					}
		//				}
		//			} else { return false; }
		//		} else { return true; }
		//	}



		//	// -- Получение информации о залогиненом пользователе
		//	// - Версия для функции входа в аккаунт
		//	public bool Req_UpdateBotChannelInfo_login(bool isChannel) {
		//		string res = null;

		//		if (isChannel) {
		//			if (TechF.TechFuncs.GetSettingParam("Channel_AuthCode") == "true") {
		//				res = ApiRequest("users", true);
		//				TechF.db.SettingsT.UpdateSetting("Channel", TechF.TechFuncs.SearchinJson(res, "login"));
		//				TechF.db.SettingsT.UpdateSetting("ChannelId", TechF.TechFuncs.SearchinJson(res, "id"));
		//				TechF.db.SettingsT.UpdateSetting("ChannelDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
		//				return true;
		//			} else {
		//				if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(3, "Ошибка обновления информации о канале", null, true)); }
		//				return false;
		//			}
		//		} else {
		//			if (TechF.TechFuncs.GetSettingParam("Bot_AuthCode") == "true") {
		//				useDefToken = false;
		//				res = ApiRequest("users", false);
		//				TechF.db.SettingsT.UpdateSetting("Login", TechF.TechFuncs.SearchinJson(res, "login"));
		//				TechF.db.SettingsT.UpdateSetting("LoginId", TechF.TechFuncs.SearchinJson(res, "id"));
		//				TechF.db.SettingsT.UpdateSetting("LoginDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
		//				if (TechF.BotChannelisOne) {
		//					TechF.db.SettingsT.UpdateSetting("Channel", TechF.TechFuncs.SearchinJson(res, "login"));
		//					TechF.db.SettingsT.UpdateSetting("ChannelId", TechF.TechFuncs.SearchinJson(res, "id"));
		//					TechF.db.SettingsT.UpdateSetting("ChannelDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
		//				}
		//				return true;
		//			} else {
		//				if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(3, "Ошибка обновления информации об аккаунте", null, true)); }
		//				return false;
		//			}
		//		}
		//	}

		//	// - Основная версия
		//	public bool Req_UpdateBotChannelInfo() {
		//		string res = null;
		//		string bl = TechF.TechFuncs.GetSettingParam("Login");
		//		string cl = TechF.TechFuncs.GetSettingParam("Channel");
		//		string bi = null;
		//		string ci = null;
		//		if (bl == cl) { cl = null; }

		//		if (bl != null && cl == null) {
		//			res = ApiRequest("users?login=" + bl);
		//			bi = TechF.TechFuncs.SearchinJson(res, "id");
		//			if (bi != null) {
		//				TechF.db.SettingsT.UpdateSetting("LoginId", bi);
		//				TechF.db.SettingsT.UpdateSetting("LoginDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
		//				return true;
		//			} else {
		//				if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(3, "Ошибка обновления информации о боте", null, true)); }
		//				return false;
		//			}
		//		}

		//		if (bl == null && cl != null) {
		//			res = ApiRequest("users?login=" + cl);
		//			ci = TechF.TechFuncs.SearchinJson(res, "id");
		//			if (ci != null) {
		//				TechF.db.SettingsT.UpdateSetting("ChannelId", ci);
		//				TechF.db.SettingsT.UpdateSetting("ChannelDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
		//				return true;
		//			} else {
		//				if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(3, "Ошибка обновления информации о канале", null, true)); }
		//				return false;
		//			}
		//		}

		//		if (bl != null && cl != null) {
		//			res = ApiRequest("users?login=" + bl + "&login=" + cl);
		//			string[] resm = res.Split(new string[1] { "},{" }, StringSplitOptions.None);
		//			if (resm.Length == 1) {
		//				if (TechF.TechFuncs.SearchinJson(res, "login") == bl) {
		//					bi = TechF.TechFuncs.SearchinJson(res, "id");
		//					if (bi != null) {
		//						TechF.db.SettingsT.UpdateSetting("LoginId", bi);
		//						TechF.db.SettingsT.UpdateSetting("LoginDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
		//						return true;
		//					} else {
		//						if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(3, "Ошибка обновления информации о боте", null, true)); }
		//						return false;
		//					}
		//				} else {
		//					if (TechF.TechFuncs.SearchinJson(res, "login") == cl) {
		//						ci = TechF.TechFuncs.SearchinJson(res, "id");
		//						if (ci != null) {
		//							TechF.db.SettingsT.UpdateSetting("ChannelId", ci);
		//							TechF.db.SettingsT.UpdateSetting("ChannelDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
		//							return true;
		//						} else {
		//							if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(3, "Ошибка обновления информации о канале", null, true)); }
		//							return false;
		//						}
		//					} else {
		//						if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, new Twident_Status(3, "", "Несоответствие сохранённых и полученных при обновлении аккаунтов", true)); }
		//						return false;
		//					}
		//				}
		//			} else {
		//				bi = TechF.TechFuncs.SearchinJson(resm[0], "id");
		//				if (bi != null) {
		//					TechF.db.SettingsT.UpdateSetting("LoginId", bi);
		//					TechF.db.SettingsT.UpdateSetting("LoginDisp", TechF.TechFuncs.SearchinJson(resm[0], "display_name"));
		//				}

		//				ci = TechF.TechFuncs.SearchinJson(resm[1], "id");
		//				if (ci != null) {
		//					TechF.db.SettingsT.UpdateSetting("ChannelId", ci);
		//					TechF.db.SettingsT.UpdateSetting("ChannelDisp", TechF.TechFuncs.SearchinJson(resm[1], "display_name"));
		//				}
		//				return true;
		//			}
		//		} else { return false; }
		//	}



		//	// -- Получение OAuth кода - пароля для входа в чат --
		//	public bool Req_UpdateOAuth() {


		//		return false;
		//	}

		//}

	}
}
