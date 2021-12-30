using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
//using System.Net.Sockets;
//using System.Net.WebSockets;
using System.Timers;
using WebSocket4Net;
using System.Windows;
using System.Globalization;
using System.Reflection;
using System.Collections.ObjectModel;
//using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Twidibot
{
	delegate void FuncCDt();
	public class Chat
	{
		public event EventHandler<CEvent_Msg> Ev_Error;
		public event EventHandler<CEvent_ChatMsg> Ev_ChatMsg;
		public event EventHandler<CEvent_ChatStatus> Ev_Status;
		//public event EventHandler<CEvent_Msg> Ev_Notice;
		//public event EventHandler<CEvent_Msg> Ev_MsgSented;
		public event EventHandler<CEvent_ChatMsg> Ev_BotMsg;

		public Funcs Funcs = null;
		private BackWin TechF = null;

		private ObservableCollection<string> MsgListtoSent = null;

		//DateTime DT = new DateTime();


		// -- Данные аккаунта, который будет работать как бот --
		public string Channel { get; private set; } // -- В чей чат будем заходить
		public string Login { get; private set; } // -- Логин аккаунта пользователя
		public string Pass { get; private set; } // -- Пароль (OAuth код с https://twitchapps.com/tmi/) аккаунта пользователя

		private const string DefLogin = "twidibot";
		private const string DefPass = "oauth:vvtvywz3m0dds6fi1kq5ub28fvdx4s";

		public bool Worked = false;
		public bool Moderator = false;
		public bool AuthBot = false;
		public bool useDefBot = true;
		public int MsgFreq = 1050;
		private WebSocket Socket = new WebSocket("wss://irc-ws.chat.twitch.tv");

		private bool pbotparam_Mod = false;
		private bool pbotparam_Sub = false;
		private bool pbotparam_Follow = true;
		private string pbotparam_Color = null;
		private bool pbotparam_Edit = false;
		private bool pchannelparam_Emote = false;
		private bool pchannelparam_Sub = false;
		private int pchannelparam_Follow = -1;
		private bool pchannelparam_r9k = false;
		private int pchannelparam_Slow = 0;

		public bool botparam_Mod {
			get { return pbotparam_Mod; }
			set {
				if (value) {
					MsgFreq = 50;
				} else {
					if (pchannelparam_Slow > 0) { MsgFreq = pchannelparam_Slow * 1000 + 50; } else { MsgFreq = 1050; } }
				pbotparam_Mod = value;
			}
		}
		public bool botparam_Sub {
			get { return pbotparam_Sub; }
			set { pbotparam_Mod = value; }
		}
		public bool botparam_Follow {
			get { return pbotparam_Follow; }
			set { pbotparam_Follow = value; }
		}
		public string botparam_Color {
			get { return pbotparam_Color; }
			set { pbotparam_Color = value; }
		}
		public bool botparam_Edit {
			get { return pbotparam_Edit; }
			set { pbotparam_Edit = value; }
		}

		public bool channelparam_Emote {
			get { return pchannelparam_Emote; }
			set { pchannelparam_Emote = value; }
		}
		public bool channelparam_Sub {
			get { return pchannelparam_Sub; }
			set { pchannelparam_Sub = value; }
		}
		public int channelparam_Follow {
			get { return pchannelparam_Follow; }
			set { pchannelparam_Follow = value; }
		}
		public bool channelparam_r9k {
			get { return pchannelparam_r9k; }
			set { pchannelparam_r9k = value; }
		}
		public int channelparam_Slow {
			get { return pchannelparam_Slow; }
			set { pchannelparam_Slow = value; }
		}



		// -- Конструкторы --
		public Chat(BackWin backWin) {
			this.TechF = backWin;
			this.Funcs = new Funcs(backWin);
			this.MsgListtoSent = new ObservableCollection<string>();
			DBLoadSet();
			this.useDefBot = true;
		}

		public Chat(BackWin backWin, string Channel) {
			this.TechF = backWin;
			this.Funcs = new Funcs(backWin);
			this.MsgListtoSent = new ObservableCollection<string>();
			TechF.db.SettingsT.UpdateSetting("Channel", Channel);
			DBLoadSet();
			this.useDefBot = true;
		}
		public Chat(BackWin backWin, string Login, string Pass) {
			this.TechF = backWin;
			this.Funcs = new Funcs(backWin);
			this.MsgListtoSent = new ObservableCollection<string>();
			TechF.db.SettingsT.UpdateSetting("Login", Login);
			TechF.db.SettingsT.UpdateSetting("Pass", Pass);
			DBLoadSet();
			this.useDefBot = false;
		}
		public Chat(BackWin backWin, string Channel, string Login, string Pass) {
			this.TechF = backWin;
			this.Funcs = new Funcs(backWin);
			this.MsgListtoSent = new ObservableCollection<string>();
			TechF.db.SettingsT.UpdateSetting("Channel", Channel);
			TechF.db.SettingsT.UpdateSetting("Login", Login);
			TechF.db.SettingsT.UpdateSetting("Pass", Pass);
			DBLoadSet();
			this.useDefBot = false;
		}

		// -- Установка канала, логина и пароля из базы данных или сохранение их в базу --
		private void DBLoadSet() {
			DB.Setting_tclass Setl = TechF.db.SettingsList.Find(x => x.id == 2);
			this.Channel = Setl.Param;
			Setl = TechF.db.SettingsList.Find(x => x.id == 3);
			this.Login = Setl.Param;
			Setl = TechF.db.SettingsList.Find(x => x.id == 4);
			this.Pass = Setl.Param;
			if (this.Login != null && this.Pass != null ) { useDefBot = false; } else { useDefBot = true; }
		}




		// -- Функция подключения к чату --
		public void Connect() {
			this.DBLoadSet();
			TechF.TechFuncs.LogDH("Подключение к чату...");

			if (Socket.State != WebSocketState.Open) {
				Socket = new WebSocket("wss://irc-ws.chat.twitch.tv");
				Socket.MessageReceived += MsgParsing;
				Socket.Error += ErrWS;

				// -- Подключение к серверу --
				if (Socket.State != WebSocketState.Open) {
					try {
						Socket.Open();
					} catch (Exception ex) { // -- Сообщение об ошибке
						if (Ev_Error != null) { Ev_Error(this, new CEvent_Msg(ex.Message)); }
					}
				}

				while (true) { // -- Блокировка выполнения дальнейшего кода, пока не будет установлено соединение --
					if (Socket.State == WebSocketState.Open) {
						if (Ev_Status != null) { Ev_Status(this, new CEvent_ChatStatus(Socket.State)); }
						break;
					}
					if (Ev_Status != null) { Ev_Status(this, new CEvent_ChatStatus(Socket.State)); }
					Thread.Sleep(50);
				}
							

				// -- Вход в аккаунт --
				if (!useDefBot) {
					Send("PASS " + Pass);
					Send("NICK " + Login);
				} else {
					Send("PASS " + DefPass);
					Send("NICK " + DefLogin);
				}
				Thread.Sleep(2000);

				// -- Запрос разрешений
				Send("CAP REQ :twitch.tv/membership");
				Send("CAP REQ :twitch.tv/tags");
				Send("CAP REQ :twitch.tv/commands");

				// --И подключение к чату--
				Send("JOIN #" + Channel);
				//Send("JOIN #dyuha138");
				Thread.Sleep(500);

				// -- И отправка уведомления о том, что бот работает --
				//SendMsg("Я включился!");

				// -- Ну и даём знать всем остальным, что можно работать --
				if (!Worked) { Task.Factory.StartNew(() => Status()); }
				if (!Worked) { Task.Factory.StartNew(() => SendAwait()); }
				if (!Worked) { Task.Factory.StartNew(() => Funcs.Init()); } // -- Запуск фоновых функций
				Worked = true;
			}
		}

		private void ErrWS(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
			if (Ev_Error != null) { Ev_Error(this, new CEvent_Msg(e.Exception.Message)); }
		}

		private async void Status() {
			int crec = 0;
			int TimeToReconnect = 0;
			WebSocketState oldstate = WebSocketState.None;
			Thread.Sleep(500);
			while (Worked) {
				oldstate = Socket.State;

				if (Socket.State != oldstate) {
					if (Ev_Status != null) { Ev_Status(this, new CEvent_ChatStatus(Socket.State)); }
					oldstate = Socket.State;
				}

				if (Socket.State == WebSocketState.Closed) { // -- Переподключение
					if (TimeToReconnect == 0) {
						TimeToReconnect = 1;
						if (Ev_Error != null) { Ev_Error(this, new CEvent_Msg("Разрыв соединения. Повторое подключение...")); }
						TechF.TechFuncs.LogDH("Разрыв соединения, переподключение...");
						crec++;
					} else {
						TimeToReconnect *= 2;
						crec++;
						TechF.TechFuncs.LogDH("Попытка переподключения №" + crec + "...");

						for (ushort i = 0; i < TimeToReconnect; i++) {
							if (Ev_Error != null) { Ev_Error(this, new CEvent_Msg("Разрыв соединения. Повторое подключение через " + (TimeToReconnect-i).ToString() + " " + TechF.TechFuncs.WordEndNumber(TimeToReconnect, "s") + "...")); }
							Thread.Sleep(1000);
						}
					}
					//Thread.Sleep(TimeToReconnect * 1000);
					await Task.Factory.StartNew(() => this.Connect());
					TimeToReconnect = 0;
					crec = 0;
					if (Socket.State == WebSocketState.Open) { if (Ev_Error != null) { Ev_Error(this, new CEvent_Msg("")); } }
				}
				Thread.Sleep(100);
			}
			//if (Ev_Status != null) { Ev_Status(this, new CEvent_ChatStatus(Socket.State)); }
		}



		// -- Обработка поступающих сообщений --
		private void MsgParsing(object sender, MessageReceivedEventArgs e) {
			try {
				string msgorg = e.Message;
				string msgtmp = null;
				string msg = null, msgcom = null;
				string[] msgparams = null;
				bool msgcheck = false;
				string nick = null;
				string dispnick = null;
				string msgid = null;
				int userid = 0;
				string ucolor = null;
				string uemotes = null;
				bool isOwner = false;
				bool isMod = false;
				bool isVip = false;
				bool isSub = false;
				//bool isTurbo = false;
				//bool isPrime = false;
				string badgeinfo = null;
				string badges = null;
				int DefClir = 0, FuncClir = 0;
				DB.DefCommand_tclass DefCl = null;
				DB.FuncCommand_tclass FuncCl = null;
				//FuncCDt FuncCD = null;

				TechF.TechFuncs.LogDH("Принято на сокет: " + msgorg);

				Type t = Funcs.GetType();

				DateTime DTdt = DateTime.Now;
				string DT = DTdt.ToString();
				if (DTdt.Hour < 10) {
					Regex Regexstr = new Regex(@"\s");
					DT = Regexstr.Replace(DT, " 0");
				}
				string[] dtm = DT.Split(' ');
				string Date = dtm[0];
				string Time = dtm[1];
				//DTdt = null;
				dtm = null;


				/*// -- Обнаружение приветственного сообщения --
				if ((msgorg.IndexOf(":tmi.twitch.tv 001 " + Login + " :Welcome, GLHF!\r\n" +
					":tmi.twitch.tv 002 " + Login + " :Your host is tmi.twitch.tv\r\n" +
					":tmi.twitch.tv 003 " + Login + " :This server is rather new\r\n") >= 0) || (
					msgorg.IndexOf(":tmi.twitch.tv 001 " + DefLogin + " :Welcome, GLHF!\r\n" +
					":tmi.twitch.tv 002 " + DefLogin + " :Your host is tmi.twitch.tv\r\n" +
					":tmi.twitch.tv 003 " + DefLogin + " :This server is rather new\r\n") >= 0))
					{
					msgcheck = true;
					//if (Ev_ChatMsg != null) { Ev_ChatMsg(this, new CEvent_Msg("Подключение к твичу: \u221a")); }
				}*/

				// -- Пинг понг обработчик
				if (msgorg.IndexOf("PING :tmi.twitch.tv") >= 0) {
					msgcheck = true;
					Send("PONG :tmi.twitch.tv");
				}



				// -- Разбираем сообщение в чате на отдельные компоненты --
				if (!msgcheck && msgorg.IndexOf("tmi.twitch.tv PRIVMSG") > 0) { // -- У нас сообщение в чате
					msgcheck = true;

					// -- Определяем...
					// -- Ник
					msgtmp = msgorg.Substring(msgorg.IndexOf("!") + 1);
					nick = msgtmp.Substring(0, msgtmp.IndexOf("@"));

					msgtmp = msgorg.Substring(msgorg.IndexOf(";display-name=") + 14);
					dispnick = msgtmp.Substring(0, msgtmp.IndexOf(";"));
					if (dispnick == null || dispnick == "") { dispnick = nick; }

					// -- Айди юзера
					msgtmp = msgorg.Substring(msgorg.LastIndexOf("user-id=") + 8);
					userid = Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";")));

					// -- Айди сообщения
					msgtmp = msgorg.Substring(msgorg.LastIndexOf(";id=") + 4);
					msgid = msgtmp.Substring(0, msgtmp.IndexOf(";"));

					// -- Какое-то инфо о значках
					msgtmp = msgorg.Substring(msgorg.LastIndexOf("badge-info=") + 11);
					badgeinfo = msgtmp.Substring(0, msgtmp.IndexOf(";"));

					// -- Сами значки
					msgtmp = msgorg.Substring(msgorg.LastIndexOf("badges=") + 7);
					badges = msgtmp.Substring(0, msgtmp.IndexOf(";"));

					// -- Модера
					msgtmp = msgorg.Substring(msgorg.LastIndexOf(";mod=") + 5);
					isMod = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

					// -- Випа через значки, потому что отдельного параметра нет
					isVip = badges.Contains("vip");

					// -- Саба
					msgtmp = msgorg.Substring(msgorg.LastIndexOf(";subscriber=") + 12);
					isSub = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

					// -- Турбо
					//msgtmp = msgorg.Substring(msgorg.LastIndexOf(";turbo=") + 7);
					//isTurbo = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

					// -- Смайлики
					msgtmp = msgorg.Substring(msgorg.LastIndexOf(";emotes=") + 8);
					uemotes = msgtmp.Substring(0, msgtmp.IndexOf(";"));

					// -- Флаги
					//msgtmp = msgorg.Substring(msgorg.LastIndexOf(";flags=") + 7);
					//flags = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

					// -- Владельца канала, через значки
					isOwner = badges.Contains("broadcaster");

					// -- Цвет ника
					msgtmp = msgorg.Substring(msgorg.LastIndexOf("color=") + 6);
					ucolor = msgtmp.Substring(0, msgtmp.IndexOf(";"));


					// -- Выделяем сообщение
					//msgtmp = msgtmp.Substring(msgtmp.IndexOf(" :") + 1);
					msgtmp = msgtmp.Substring(msgtmp.IndexOf("#" + Channel + " :") + Channel.Length + 1);
					msgtmp = msgtmp.Substring(msgtmp.IndexOf(" :") + 2);
					msg = msgtmp.Trim();

					if (Ev_ChatMsg != null) { Ev_ChatMsg(this, new CEvent_ChatMsg(msgid, dispnick, userid, msg, Date, Time, ucolor, isOwner, isMod, isVip, isSub, badgeinfo, badges)); } // -- Отправляем полученное сообщение через ивент

					// -- Обработка команд в чате --
					if (msg.IndexOf(" ") > 0) {
						msgcom = msg.Substring(0, msg.IndexOf(" "));
					} else { msgcom = msg; }
					msgcom = msgcom.Replace(",", "");

					//msgparams[] = msg.Substring(msg.IndexOf(" "));
					if (msgcom != msg) {
						msgparams = msg.Substring(msg.IndexOf(" ")+1).Split(' ');
					}


					// -- Временная врезка специально для груза
					if (msgorg.Contains("custom-reward-id=e4f75bd0-0e28-4975-9263-e6548f53b85f")) {
						string msgtmp2 = null;
						if (msg.StartsWith("@")) {
							msgtmp2 = msg.Substring(1);
						} else { msgtmp2 = msg; }
						if (botparam_Mod) {
							SendCom("timeout " + msgtmp2 + " 60 Выстрел");
						} else {
							SendMsg("Нет прав модера, выстрел не сделан");
						}
					}


					// -- Поиск команды в списках --
					DefCl = TechF.db.DefCommandsList.Find(x => x.Command == msgcom);
					FuncCl = TechF.db.FuncCommandsList.Find(x => x.Command == msgcom);

					// --Обычная команда --
					if (DefCl != null) {
						DefClir = TechF.db.DefCommandsList.FindIndex(x => x.id == DefCl.id);
						if (DefCl.LastUsed != null && DefCl.LastUsed != "") { // -- Если нет в базе даты последнего использования, то отправляем ответ
							// -- Работа с датой последнего использования
							DateTime dtcomlul = DateTime.ParseExact(DefCl.LastUsed, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
							DateTime dtl = DateTime.ParseExact(Date + " " + Time, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

							TimeSpan res = dtl - dtcomlul; // -- Вычисление разницы
							double secdif = res.TotalSeconds; // -- Перевод разницы в секунды
							if (secdif < DefCl.CoolDown) {
								//this.SendMsg("@" + nick + ", кулдаун");
							} else {
								this.SendMsg(DefCl.Result);
							}
						} else {
							this.SendMsg(DefCl.Result); // -- Отправка в чат ответа
						}
					}

					// -- Функциональная команда --
					if (FuncCl != null) {
						FuncClir = TechF.db.FuncCommandsList.FindIndex(x => x.id == FuncCl.id);
						if (FuncCl.LastUsed != null && FuncCl.LastUsed != "") { // -- Если нет в базе даты последнего использования, то отправляем ответ
							// -- Работа с датой последнего использования
							DateTime dtcomlul = DateTime.ParseExact(FuncCl.LastUsed, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
							DateTime dtl = DateTime.ParseExact(Date + " " + Time, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

							TimeSpan res = dtl - dtcomlul; // -- Вычисление разницы
							double secdif = res.TotalSeconds; // -- Перевод разницы в секунды
							if (secdif < FuncCl.CoolDown) {
								//this.SendMsg("@" + nick + ", кулдаун");
							} else {
								if (msgparams == null) { // -- Если параметров в чате нет, то берём стандартные
									msgparams = FuncCl.Params.Split(';');
								} else {
									if (msgparams.Length < FuncCl.Params.Split(';').Length) { // -- Загружаем недостающие параметры загрузкой всех стандартных параметров и заменой указаными
										string[] msgpartmp = msgparams;
										msgparams = null;
										msgparams = FuncCl.Params.Split(';');
										for (ushort i = 0; i < msgpartmp.Length; i++) {
											msgparams[i] = msgpartmp[i];
										}
									}
								}
								if (msgparams == null) {
									SendMsg("@" + nick + ", неуказаны параметры функции, команда невыполнена");
								} else {
									string[] msgpartmp = msgparams;
									msgparams = null;
									msgparams = new string[msgpartmp.Length + 1];
									for (ushort i = 0; i < msgpartmp.Length; i++) {
										msgparams[i] = msgpartmp[i];
									}
									msgparams[msgparams.Length - 1] = nick;
									if (FuncCl.Enabled) {
										MethodInfo mi = t.GetMethod(FuncCl.FuncName);
										mi.Invoke(Funcs, msgparams);
									}
								}
							}
						} else {
							if (msgparams == null) { // -- Если параметров в чате нет, то берём стандартные
								if (FuncCl.Params != null) { msgparams = FuncCl.Params.Split(';'); }
							} else {
								if (msgparams.Length < FuncCl.Params.Split(';').Length) { // -- Загружаем недостающие параметры загрузкой всех стандартных параметров и заменой указаными
									string[] msgpartmp = msgparams;
									msgparams = null;
									msgparams = FuncCl.Params.Split(';');
									for (ushort i=0; i < msgpartmp.Length; i++) {
										msgparams[i] = msgpartmp[i];
									}
								}
							}
							if (msgparams == null && FuncCl.Params != null && FuncCl.Params != "") {
								SendMsg("@" + nick + ", неуказаны параметры функции, команда невыполнена");
							} else {
								string[] msgpartmp = msgparams;
								msgparams = null;
								msgparams = new string[msgpartmp.Length + 1];
								for (ushort i = 0; i < msgpartmp.Length; i++) {
									msgparams[i] = msgpartmp[i];
								}
								msgparams[msgparams.Length - 1] = nick;
								if (FuncCl.Enabled) {
									MethodInfo mi = t.GetMethod(FuncCl.FuncName);
									mi.Invoke(Funcs, msgparams);
								}
							}
						}
					}

					if (DefCl == null && FuncCl == null) {
						if (msgcom.StartsWith("!")) {
							//this.SendMsg("@" + nick + ", команда \"" + msgcom + "\" не найдена");
						}
					}

					// -- Cохранение даты использования команды
					if (DefCl != null) {
						TechF.db.DefCommandsT.UpdateLastUsed(DefCl.id, DT);
						TechF.db.DefCommandsList.Insert(DefClir, DefCl);
					}
					if (FuncCl != null) {
						TechF.db.FuncCommandsT.UpdateLastUsed(FuncCl.id, DT);
						TechF.db.FuncCommandsList.Insert(FuncClir, FuncCl);
					}

					DefCl = null;
					FuncCl = null;
				}




				// -- Уведомления о сабках и рейдах
				if (!msgcheck && msgorg.Contains(":tmi.twitch.tv USERNOTICE")) {
					msgcheck = true;


				}



				// -- Уведомление об очищении всего чата, или же об бане пользователя
				if (!msgcheck && msgorg.Contains(":tmi.twitch.tv CLEARCHAT")) {
					msgcheck = true;

				}



				// -- Уведомление об удалении сообщения (типа бана)
				if (!msgcheck && msgorg.Contains(":tmi.twitch.tv CLEARMSG")) {
					msgcheck = true;

				}



				// -- Обычные NOTICE сообщения --
				if (!msgcheck && msgorg.Contains(":tmi.twitch.tv NOTICE")) { 
					msgcheck = true;
					//if (Ev_Notice != null) { Ev_Notice(this, new CEvent_Msg(msgtmp.Substring(0, msgtmp.Length - 1))); }

					msgtmp = msgorg.Substring(msgorg.IndexOf("msg-id=") + 7); 
					msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(" "));

					// -- Отправляем сообщение слишком быстро, повышаем задержку
					if (msgtmp == "msg_ratelimit") { this.MsgFreq = 1050; }
					// -- Бот незафоловлен на канал, и стоит фолоу-мод (определять, на сколько стоит фолоу-мод сложновато, поэтому ставим просто 0 надеясь, что это не шибко повлиет на работу)
					if (msgtmp == "msg_followersonly" || msgtmp == "msg_followersonly_zero") { this.channelparam_Follow = 0; }
				}



				// -- Мультисообщение о заходе в чат или выходе из чата пользователя
				if (!msgcheck && (msgorg.Contains("tmi.twitch.tv JOIN #") || msgorg.Contains("tmi.twitch.tv PART #"))) {
					//msgcheck = true;
				
				}



				// -- Получение и локальная установка параметров чата --
				if (!msgcheck && msgorg.Contains("tmi.twitch.tv ROOMSTATE #")) {
				//if (msgorg.Contains("tmi.twitch.tv ROOMSTATE #")) {
					//msgcheck = true;
					if (msgorg.Contains("emote-only=")) {
						msgtmp = msgorg.Substring(msgorg.LastIndexOf("emote-only=") + 11);
						msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(" "));
						if (msgtmp.Length > 5) { msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(";")); }
						channelparam_Emote = Convert.ToBoolean(Convert.ToInt32(msgtmp));
					}
					if (msgorg.Contains("followers-only=")) {
						msgtmp = msgorg.Substring(msgorg.LastIndexOf("followers-only=") + 15);
						msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(" "));
						if (msgtmp.Length > 5) { msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(";")); }
						channelparam_Follow = Convert.ToInt32(msgtmp);
					}
					if (msgorg.Contains("r9k=")) {
						msgtmp = msgorg.Substring(msgorg.LastIndexOf("r9k=") + 4);
						msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(" "));
						if (msgtmp.Length > 5) { msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(";")); }
						channelparam_r9k = Convert.ToBoolean(Convert.ToInt32(msgtmp));
					}
					if (msgorg.Contains("subs-only=")) {
						msgtmp = msgorg.Substring(msgorg.LastIndexOf("subs-only=") + 10);
						msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(" "));
						if (msgtmp.Length > 5) { msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(";")); }
						channelparam_Sub = Convert.ToBoolean(Convert.ToInt32(msgtmp));
					}
					if (msgorg.Contains("slow=")) {
						msgtmp = msgorg.Substring(msgorg.LastIndexOf("slow=") + 5);
						msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(" "));
						if (msgtmp.Length > 5) { msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(";")); }
						channelparam_Slow = Convert.ToInt32(msgtmp);
					}
				}


				// -- Получение параметров бота --
				if (msgorg.Contains("tmi.twitch.tv USERSTATE #")) {
					//msgcheck = true;
					msgtmp = msgorg.Substring(msgorg.LastIndexOf("mod=") + 4);
					botparam_Mod = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

					msgtmp = msgorg.Substring(msgorg.LastIndexOf("subscriber=") + 11);
					botparam_Sub = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

				}



				// -- Обработка спец сообщений с кодами --
				// -- 353
				/*if (!msgcheck && msgorg.Contains("tmi.twitch.tv 353")) {
					if () {
						msgtmp = msgorg.Substring(0, msgorg.IndexOf("#");
						msg = msgtmp.Substring(0, msgtmp.IndexOf(" :") + 1);
					}
					msgtmp = msgorg.Substring(0, msgorg.IndexOf("#" + Channel + " :") + 1);
					msg = msgtmp.Substring(0, msgtmp.Length);
				}
			
				// -- 366
				if (!msgcheck && msgorg.Contains("tmi.twitch.tv 366")) {

				}*/

			} catch (Exception ex) {
				TechF.TechFuncs.LogDH("Ошибка парсинга сообщения. Сообщение: " + ex.Message);
			}

		}




		// -- Буфер отправляемых сообщений --
		private void SendAwait() {
			Thread.Sleep(500);
			while (Worked) {
				if (MsgListtoSent.Count > 0 && Socket.State == WebSocketState.Open) {
					if ((!channelparam_Emote || botparam_Mod) && (!channelparam_Sub || botparam_Sub) && (channelparam_Follow == (-1) || botparam_Follow)) {
						Send(MsgListtoSent.ElementAt<string>(0));
					}
					MsgListtoSent.RemoveAt(0);
					Thread.Sleep(this.MsgFreq);
				}
				Thread.Sleep(50);
			}
			MsgListtoSent.Clear();
		}


		// -- Отправка сообщения серверу --
		private void Send(string Msg)  {
			try {
				Socket.Send(Msg + "\r\n");
			} catch (Exception ex) {
				if (Ev_Error != null) { Ev_Error(this, new CEvent_Msg(ex.Message)); }
			}
			string DT = DateTime.Now.ToString();
			string Date = DT.Substring(0, 10);
			string Time = DT.Substring(11);
			
			TechF.TechFuncs.LogDH("Отправлено на сокет: " + Msg);
			if (Msg.Contains("PRIVMSG")) {
				Msg = Msg.Substring(Msg.IndexOf(":") + 1);
				if (!useDefBot) {
					if (Ev_BotMsg != null) { Ev_BotMsg(this, new CEvent_ChatMsg(null, Login, 0, Msg, Date, Time, null, false, botparam_Mod, false, botparam_Sub)); }
				} else { if (Ev_BotMsg != null) { Ev_BotMsg(this, new CEvent_ChatMsg(null, DefLogin, 0, Msg, Date, Time, "#8A2BE2", false, botparam_Mod, false, botparam_Sub)); } }
			}
		}


		// -- Отправка команд в чат
		public void SendCom(string Command, string[] Params = null) {
			SendMsg("/" + Command);
		}

		// -- Отправка обычного сообщения в чат
		public void SendMsg(string msg) {
			//Send("PRIVMSG " + "#" + channel + " :" + msg);
			MsgListtoSent.Add("PRIVMSG " + "#" + Channel + " :" + msg);
		}
		public void SendMsg(object sender, CEvent_Msg e) {
			//Send("PRIVMSG " + "#" + channel + " :" + e.Message);
			MsgListtoSent.Add("PRIVMSG " + "#" + Channel + " :" + e.Message);
		}



		// -- Закрытие соединения --
		public void Close() {
			TechF.TechFuncs.LogDH("Отключение от чата...");
			Worked = false;
			Send("PART #" + Channel);
			//Send("PART #dyuha138");
			Socket.Close();
			Thread.Sleep(500);
			if (Ev_Status != null) { Ev_Status(this, new CEvent_ChatStatus(WebSocketState.Closed)); }
			TechF.TechFuncs.LogDH("Выполнено отключение от чата");
			//Socket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
			//Socket.Dispose();
		}
	}
}
