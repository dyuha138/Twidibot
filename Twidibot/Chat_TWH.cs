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

namespace Twidibot {

	public partial class Twitch {
		

		public class ChatC {
			public event EventHandler<Twident_ChatMsg> Ev_ChatMsg;
			//public event EventHandler<Twident_Msg> Ev_Notice;
			//public event EventHandler<Twident_Msg> Ev_MsgSented;
			public event EventHandler<Twident_ChatMsg> Ev_BotMsg;
			public event EventHandler<Twident_Status> Ev_GlobalStatus;
			public event EventHandler<Twident_Status> Ev_Status;
			public event EventHandler<Twident_Bool> Ev_ConnectStatus;

			public ChatFuncs Funcs = null;
			private BackWin TechF = null;
			delegate void FuncCDt();
			Type FuncT = null;

			private ObservableCollection<string> MsgListtoSent = null;

			// -- Блок переменных для функции парсинга сообщений
			private bool msgcheck = false;
			private long secdif = 0;
			private string ErrSocket = null;
			//private bool comdtcheck = false;
			private DB.DefCommand_tclass DefCl = null;
			private DB.FuncCommand_tclass FuncCl = null;


			// -- Данные аккаунта, который будет работать как бот --
			public string Channel { get; private set; } // -- В чей чат будем заходить
			public string Login { get; private set; } // -- Логин аккаунта пользователя
			private string Pass { get; set; } // -- Пароль (OAuth код с https://twitchapps.com/tmi/) аккаунта пользователя

			public bool Work { get; private set; }
			public bool Starting { get; private set; }
			public bool ErrPass = false;
			public bool AuthBot = false;
			public int MsgFreq = 1050;
			private WebSocket Socket = null;

			private bool pbotparam_Brodcaster = false;
			private bool pbotparam_Mod = false;
			private bool pbotparam_Sub = false;
			private bool pbotparam_Follow = true;
			private string pbotparam_Color = null;
			private bool pbotparam_Edit = false;
			private bool pbotparam_VIP = false;
			private bool pchannelparam_Emote = false;
			private bool pchannelparam_Sub = false;
			private int pchannelparam_Follow = -1;
			private bool pchannelparam_r9k = false;
			private int pchannelparam_Slow = 0;

			public bool botparam_Brodcaster {
				get { return this.pbotparam_Brodcaster; }
				set {
					if (value) {
						MsgFreq = 50;
					} else {
						if (this.pchannelparam_Slow > 0) { this.MsgFreq = this.pchannelparam_Slow * 1000 + 50; } else { this.MsgFreq = 1050; }
					}
					this.pbotparam_Brodcaster = value;
				}
			}
			public bool botparam_Mod {
				get { return this.pbotparam_Mod; }
				set	{
					if (value) {
						MsgFreq = 50;
					} else {
						if (this.pchannelparam_Slow > 0) { this.MsgFreq = this.pchannelparam_Slow * 1000 + 50; } else { this.MsgFreq = 1050; }
					}
					this.pbotparam_Mod = value;
				}
			}
			public bool botparam_Sub {
				get { return this.pbotparam_Sub; }
				set { this.pbotparam_Sub = value; }
			}
			public bool botparam_Follow {
				get { return this.pbotparam_Follow; }
				set { this.pbotparam_Follow = value; }
			}
			public string botparam_Color {
				get { return this.pbotparam_Color; }
				set { this.pbotparam_Color = value; }
			}
			public bool botparam_Edit {
				get { return this.pbotparam_Edit; }
				set { this.pbotparam_Edit = value; }
			}
			public bool botparam_VIP {
				get { return this.pbotparam_VIP; }
				set	{
					if (value) {
						MsgFreq = 50;
					} else {
						if (this.pchannelparam_Slow > 0) { this.MsgFreq = this.pchannelparam_Slow * 1000 + 50; } else { this.MsgFreq = 1050; }
					}
					this.pbotparam_VIP = value;
				}
			}
			public bool channelparam_Emote {
				get { return this.pchannelparam_Emote; }
				set { this.pchannelparam_Emote = value; }
			}
			public bool channelparam_Sub {
				get { return this.pchannelparam_Sub; }
				set { this.pchannelparam_Sub = value; }
			}
			public int channelparam_Follow {
				get { return this.pchannelparam_Follow; }
				set { this.pchannelparam_Follow = value; }
			}
			public bool channelparam_r9k {
				get { return this.pchannelparam_r9k; }
				set { this.pchannelparam_r9k = value; }
			}
			public int channelparam_Slow {
				get { return this.pchannelparam_Slow; }
				set { this.pchannelparam_Slow = value; }
			}



			// -- Конструкторы --
			public ChatC(BackWin backWin) {
				this.TechF = backWin;
				this.Funcs = backWin.ChatFuncs;
				FuncT = backWin.ChatFuncs.GetType();
				this.MsgListtoSent = new ObservableCollection<string>();
				//DBLoad();
				NewSocket();
			}

			public ChatC(BackWin backWin, string Channel) {
				this.TechF = backWin;
				FuncT = backWin.ChatFuncs.GetType();
				this.MsgListtoSent = new ObservableCollection<string>();
				TechF.db.SettingsT.UpdateSetting("Channel", Channel);
				TechF.db.SettingsT.UpdateSetting("ManualMode", "1");
				//DBLoad();
				NewSocket();
			}
			public ChatC(BackWin backWin, string Login, string Pass) {
				this.TechF = backWin;
				FuncT = backWin.ChatFuncs.GetType();
				this.MsgListtoSent = new ObservableCollection<string>();
				TechF.db.SettingsT.UpdateSetting("Login", Login);
				TechF.db.SettingsT.UpdateSetting("Pass", Pass);
				TechF.db.SettingsT.UpdateSetting("ManualMode", "1");
				//DBLoad();
				NewSocket();
			}
			public ChatC(BackWin backWin, string Channel, string Login, string Pass) {
				this.TechF = backWin;
				FuncT = backWin.ChatFuncs.GetType();
				this.MsgListtoSent = new ObservableCollection<string>();
				TechF.db.SettingsT.UpdateSetting("Channel", Channel);
				TechF.db.SettingsT.UpdateSetting("Login", Login);
				TechF.db.SettingsT.UpdateSetting("Pass", Pass);
				TechF.db.SettingsT.UpdateSetting("ManualMode", "1");
				//DBLoad();
				NewSocket();
			}


			private void tst(object sender, DataReceivedEventArgs e) {
				//byte[] bm = e.Data;
			}


			// -- Установка канала, логина и пароля из базы данных --
			public void DataUpdate() {
				if (TechF.Twitch.BotChannelisOne) {
					this.Channel = TechF.db.SettingsTWHList.Find(x => x.id == 2).Param;
				} else {
					this.Channel = TechF.db.SettingsTWHList.Find(x => x.id == 1).Param;
				}
				this.Login = TechF.db.SettingsTWHList.Find(x => x.id == 2).Param;
				this.Pass = TechF.db.SettingsTWHList.Find(x => x.id == 3).Param;
			}


			private void NewSocket() {
				if (TechF.TechFuncs.GetSettingTWHParam("Secure_Connect") == "1") {
					this.Socket = new WebSocket("wss://irc-ws.chat.twitch.tv:443");
				} else {
					this.Socket = new WebSocket("ws://irc-ws.chat.twitch.tv:80");
				}
				this.Socket.MessageReceived += MsgParsing;
				this.Socket.Error += ErrWS;
				this.Socket.EnableAutoSendPing = false;
				//this.Socket.DataReceived += tst;
			}

			// -- Функция подключения к чату --
			public bool Connect(bool isReconnect = false) {
				if (TechF.Twitch.Active) {
					this.Starting = true;
					this.DataUpdate();
					ErrPass = false;
					ErrSocket = null;
					if (!isReconnect) {
						this.NewSocket();
						TechF.TechFuncs.LogDH("Подключение бота к чату Twitch...");
						Ev_Status?.Invoke(this, new Twident_Status(0, "Подключение к серверу", null, false));
						Ev_GlobalStatus?.Invoke(this, new Twident_Status(0, null, null, false));
					}

					if ((Socket.State == WebSocketState.Closed || Socket.State == WebSocketState.None) && Socket.State != WebSocketState.Open) {
						try {
							Socket.Open(); // - Подключение к серверу
						} catch (Exception ex) { // - Сообщение об ошибке
							Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка подключения к серверу", null, true));
							Ev_GlobalStatus?.Invoke(this, new Twident_Status(2, "Ошибка работы Twitch", ex.Message, true));
							Ev_ConnectStatus?.Invoke(this, new Twident_Bool(false));
							this.Starting = false;
							return false;
						}

						while (true) { // -- Блокировка выполнения дальнейшего кода, пока не будет установлено соединение
							int i = 0;
							if (Socket.State == WebSocketState.Open) { break; }
							if (Socket.State == WebSocketState.Closed) {
								Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка подключения к серверу", null, true));
								if (ErrSocket != null) {
									Ev_GlobalStatus?.Invoke(this, new Twident_Status(2, "Ошибка работы Twitch", ErrSocket, true));
								} else { Ev_GlobalStatus?.Invoke(this, new Twident_Status(2, "Ошибка работы Twitch", "Неизвестая ошибка, повторите попытку подключения", true)); }
								
								Ev_ConnectStatus?.Invoke(this, new Twident_Bool(false));
								this.Starting = false;
								return false;
							}
							if (i > 200) {
								Socket.Close();
								Ev_Status?.Invoke(this, new Twident_Status(2, "Превышено время подключения", null, true));
								Ev_ConnectStatus?.Invoke(this, new Twident_Bool(false));
								this.Starting = false;
								return false;
							}
							Thread.Sleep(50);
							i++;
						}

						// -- Вход в аккаунт
						Ev_Status?.Invoke(this, new Twident_Status(0, "Авторизация", null, null));
						Send("PASS " + Pass);
						Send("NICK " + Login);
						Thread.Sleep(2000);

						if (!ErrPass) {
							// -- Запрос разрешений
							Send("CAP REQ :twitch.tv/membership");
							Send("CAP REQ :twitch.tv/tags");
							Send("CAP REQ :twitch.tv/commands");

							// -- И подключение к чату
							Ev_Status?.Invoke(this, new Twident_Status(0, "Вход в чат канала", null, null));
							Send("JOIN #" + Channel);
							//Send("JOIN #dyuha138");
							Thread.Sleep(500);

							// -- И отправка уведомления о том, что бот работает
							//SendMsg("Я включился!");

							// -- Ну и даём знать всем остальным, что можно работать --

							if (!Work) { Task.Factory.StartNew(() => StatusCheck()); }
							if (!Work) { Task.Factory.StartNew(() => SendAwait()); }
							if (!Work) { Task.Factory.StartNew(() => Funcs.Init()); } // -- Запуск фоновых функций
							this.Work = true;
							Ev_ConnectStatus?.Invoke(this, new Twident_Bool(true));
							Ev_Status?.Invoke(this, new Twident_Status(1, "Подключено", null, null));
							this.Starting = false;
							return true;
						} else {
							Ev_ConnectStatus?.Invoke(this, new Twident_Bool(false));
							this.Starting = false;
							return false;
						}
					} else {
						Ev_ConnectStatus?.Invoke(this, new Twident_Bool(false));
						this.Starting = false;
						return false;
					}
				} else { this.Starting = false; return false; }
			}

			private void ErrWS(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
				ErrSocket = e.Exception.Message + " | " + e.Exception.InnerException.Message;
			}


			private void StatusCheck() {
				int crec = 0;
				int TimeToReconnect = 0;
				WebSocketState oldstate = WebSocketState.None;
				Thread.Sleep(1000);
				while (Work) {
					oldstate = Socket.State;

					if (Socket.State != oldstate) { oldstate = Socket.State; }

					if (Socket.State == WebSocketState.Closed && Work) { // -- Переподключение
						if (TimeToReconnect == 0) {
							TimeToReconnect = 1;
							Ev_Status?.Invoke(this, new Twident_Status(2, "Попытка переподключения к Twitch...", null, false));
							TechF.TechFuncs.LogDH("Разрыв соединения, переподключение...");
							crec++;
						} else {
							TimeToReconnect *= 2;
							crec++;
							TechF.TechFuncs.LogDH("Попытка переподключения №" + crec + "...");

							for (ushort i = 0; i < TimeToReconnect; i++) {
								Ev_Status?.Invoke(this, new Twident_Status(2, "Разрыв соединения", "Повторое подключение через " + (TimeToReconnect - i).ToString() + " " + TechF.TechFuncs.WordEndNumber(TimeToReconnect, "s") + "...", null));
								Thread.Sleep(1000);
							}
						}
						//Thread.Sleep(TimeToReconnect * 1000);
						this.Connect(true);
						TimeToReconnect = 0;
						crec = 0;
						if (Socket.State == WebSocketState.Open) { Ev_Status?.Invoke(this, new Twident_Status(1, "Подключено", null, null)); }
					}
					Thread.Sleep(100);
				}
				//if (Ev_Status != null) { Ev_Status(this, new Twident_ChatStatus(Socket.State)); }
			}



			// -- Обработка поступающих сообщений --
			private void MsgParsing(object sender, MessageReceivedEventArgs e) {
				string msgorg = e.Message, msgtmp = null, msgcom = null;

				TechF.TechFuncs.LogDH("(Twitch) Принято на сокет: " + msgorg);

				try {
					long udtn = DateTimeOffset.Now.ToUnixTimeSeconds();


					// -- Обнаружение приветственного сообщения --
					if (msgorg.IndexOf(":tmi.twitch.tv 001") > 0) {
						msgcheck = true;
						//if (Ev_ChatMsg != null) { Ev_ChatMsg(this, new Twident_Msg("Подключение к твичу: \u221a")); }

					}

					// -- Пинг-понг обработчик
					if (msgorg.StartsWith("PING :tmi.twitch.tv")) {
						Send("PONG :tmi.twitch.tv");
						return;
					}




					// -- Разбираем сообщение в чате на отдельные компоненты --
					if (msgorg.IndexOf("tmi.twitch.tv PRIVMSG") > 0) { // -- У нас сообщение в чате
						ChatMessageFull cmf = ChatMsgParse(msgorg); // -- Парсим сообщение

						//Ev_ChatMsg?.Invoke(this, new Twident_ChatMsg(1, msgid, unick, udispnick, uid, msg, udtn, ucolor, uisOwner, false, uisMod, uisVip, uisSub, ubadgeinfo, ubadges));
						Ev_ChatMsg?.Invoke(this, new Twident_ChatMsg(1, cmf.id, cmf.UserInfo.Name, cmf.UserInfo.TWH.DisplayName, Convert.ToInt32(cmf.UserInfo.id), cmf.Message, udtn, cmf.UserInfo.Color, cmf.UserInfo.Permissions.isBrodcaster, cmf.UserInfo.Permissions.isModerator, cmf.UserInfo.Permissions.isVIP, cmf.UserInfo.Permissions.isSubscriber, cmf.UserInfo.TWH.BadgesInfo, cmf.UserInfo.Badges));

						// -- Обработка команды --
						if (cmf.Message.IndexOf(" ") > 0) {
							msgcom = cmf.Message.Substring(0, cmf.Message.IndexOf(" "));
						} else { msgcom = cmf.Message; }
						msgcom = msgcom.Replace(",", "");
					


						/*// -- Временная врезка специально для груза
						if (this.Channel == "gruz") {
							if (msgorg.Contains("custom-reward-id=e4f75bd0-0e28-4975-9263-e6548f53b85f")) {
								string msgtmp2 = null;
								string[] msgpar = msg.Split(' ');
								if (msgpar[0].StartsWith("@")) {
									msgtmp2 = msgpar[0].Substring(1);
								} else { msgtmp2 = msgpar[0]; }
								if (this.botparam_Mod) {
									Funcs.Ban(msgtmp2, 60, "Выстрел (" + udispnick + ")");
								} else {
									SendMsg("Нет прав модера, выстрел не сделан");
								}
							}
						}
						*/


						// -- Поиск команды в базе --
						DefCl = TechF.db.DefCommandsList.Find(x => x.Command == msgcom || x.Command.ToLower() == msgcom.ToLower() || x.Command.ToLowerInvariant() == msgcom.ToLowerInvariant());
						FuncCl = TechF.db.FuncCommandsList.Find(x => x.Command == msgcom || x.Command.ToLower() == msgcom.ToLower() || x.Command.ToLowerInvariant() == msgcom.ToLowerInvariant());

						// -- Обычная команда --
						//if (DefCl != null && DefCl.Enabled && ((DefCl.Secured && TechF.TechFuncs.VIPUserValidate(uid)) || !DefCl.Secured)) {
						if (DefCl != null && DefCl.Enabled) {

							secdif = udtn - DefCl.LastUsed; // -- Вычисляем разницу между текущей датой и датой последнего использования

							if (secdif < DefCl.CoolDown) {
								//this.SendMsg("@" + udispnick + ", кулдаун (ещё " + (DefCl.CoolDown - secdif) + " " + TechF.TechFuncs.WordEndNumber(DefCl.CoolDown - secdif, "s") + ")");
							} else {
								if (DefCl.Result.StartsWith("/")) {
									this.SendCom(DefCl.Result.Substring(1));
								} else { this.SendMsg(null, DefCl.Result); }
								TechF.db.DefCommandsT.UpdateLastUsed(DefCl.id, udtn); // -- Обновление даты использования команды
							}
						}


						// -- Функциональная (встроенная) команда --
						if (FuncCl != null && FuncCl.Enabled && ((FuncCl.Secured && TechF.TechFuncs.VIPUserValidate(Convert.ToInt32(cmf.UserInfo.id))) || !FuncCl.Secured)) {

							secdif = udtn - FuncCl.LastUsed; // -- Вычисляем разницу между текущей датой и датой последнего использования

							if (secdif < FuncCl.CoolDown) {
								//this.SendMsg("@" + udispnick + ", кулдаун (ещё " + (FuncCl.CoolDown - secdif) + " " + TechF.TechFuncs.WordEndNumber(FuncCl.CoolDown - secdif, "s") + ")");

							} else {
								// -- Подготовка переменных
								ServiceInfo ServiceInfo = new ServiceInfo(1, new PermissionsInfo(this.botparam_Brodcaster, this.botparam_Mod, this.botparam_VIP, this.botparam_Sub, this.botparam_Follow));
								List<string> msgparams = new List<string>();
								object[] funcparams = new object[4];
								string[] orgparams = FuncCl.Params.Split(';');

								// -- Парсинг параметров
								if (msgcom != cmf.Message) {
									msgparams = cmf.Message.Substring(cmf.Message.IndexOf(" ") + 1).Split(' ').ToList();
								}

								// -- Дополнение недостающими параметрами
								if (msgparams.Count < orgparams.Length) {
									for (int i = msgparams.Count; i < orgparams.Length; i++) {
										msgparams.Add(orgparams[i]);
									}
								}

								// -- Занесение собранных данных в один массив
								funcparams[0] = ServiceInfo;
								funcparams[1] = TechF.TechFuncs.UserInfoFullDownCast(cmf.UserInfo, 1);
								funcparams[2] = cmf.UserInfo.Permissions;
								funcparams[3] = msgparams;

								MethodInfo mi = FuncT.GetMethod(FuncCl.FuncName); // -- Поиск кода функции
								Task.Factory.StartNew(() => {
									try {
										mi.Invoke(TechF.ChatFuncs, funcparams); // -- Выполнение функции
									} catch (TargetParameterCountException) {
										SendMsg(new string[] { cmf.UserInfo.TWH.DisplayName }, "Извините, но вы написали слишком много параметров, я пока не умею обрезать лишнее");
										TechF.TechFuncs.LogDH("Ошибка выполнения функциональной команды: Пользователь указал параметров больше, чем их может быть");
									} catch (Exception el) {
										SendMsg(new string[] { cmf.UserInfo.TWH.DisplayName }, "Ой, что-то пошло не так, я не могу выполнить команду, извините");
										TechF.TechFuncs.LogDH("Ошибка выполнения функциональной команды: " + el.Message + " | " + el.InnerException);
									}
								});
								TechF.db.FuncCommandsT.UpdateLastUsed(FuncCl.id, udtn); // -- Обновление даты использования команды
							}
						} else {
							//this.SendMsg("@" + cmf.UserInfo.TWH.DisplayName + ", Извините, но у вас нет доступа к этой команде");
						}


						/*if (DefCl == null && FuncCl == null) {
							if (msgcom.StartsWith("!")) {
								this.SendMsg("@" + cmf.UserInfo.TWH.DisplayName + ", команда \"" + msgcom + "\" не найдена");
							}
						}*/


						DefCl = null;
						FuncCl = null;
						return;
					}




					// -- Уведомления о сабках, рейдах, и объявления
					if (msgorg.Contains(":tmi.twitch.tv USERNOTICE")) {
						//msgcheck = true;
						ChatMessageFull cmf = ChatMsgParse(msgorg);


						// -- Сообщение типа "Объявление", которое конечно нужно отображать
						switch (cmf.Type) {

							case "announcement":
								Ev_ChatMsg?.Invoke(this, new Twident_ChatMsg(1, cmf.id, cmf.UserInfo.Name, cmf.UserInfo.TWH.DisplayName, Convert.ToInt32(cmf.UserInfo.id), cmf.Message, udtn, cmf.UserInfo.Color, cmf.UserInfo.Permissions.isBrodcaster, cmf.UserInfo.Permissions.isModerator, cmf.UserInfo.Permissions.isVIP, cmf.UserInfo.Permissions.isSubscriber, cmf.UserInfo.TWH.BadgesInfo, cmf.UserInfo.Badges));
							break;
						}
						return;
					}



					// -- Уведомление об очищении всего чата, или же об бане пользователя
					if (!msgcheck && msgorg.Contains(":tmi.twitch.tv CLEARCHAT")) {
						msgcheck = true;

					}



					// -- Уведомление об удалении сообщения (типа бана)
					if (!msgcheck && msgorg.Contains(":tmi.twitch.tv CLEARMSG")) {
						msgcheck = true;

					}



					// -- NOTICE сообщения --
					if (msgorg.Contains(":tmi.twitch.tv NOTICE")) {
						//msgcheck = true;
						//if (Ev_Notice != null) { Ev_Notice(this, new Twident_Msg(msgtmp.Substring(0, msgtmp.Length - 1))); }
						msgtmp = msgorg.Substring(msgorg.IndexOf(" :") + 2);
						msgtmp = msgtmp.Substring(0, msgtmp.Length - 2);

						switch (msgtmp) {
							case "Login authentication failed": // -- Неправильный oauth токен
							ErrPass = true;
							//Thread.Sleep(100);
							Ev_Status?.Invoke(this, new Twident_Status(3, "Неправильный OAuth-токен", null, true));
							Ev_GlobalStatus?.Invoke(this, new Twident_Status(3, "Неправильный OAuth-токен для Twitch", "Зайдите в настройки приложения и получите новый OAuth-токен", true));
							TechF.TechFuncs.LogDH("(Twitch) Пользователь указал неправильный OAuth-токен, отмена входа");
							break;

							default:
							break;
						}


						// это надо посмотреть на сообщения от твича и засунуть в свич
						/*// -- Отправляем сообщение слишком быстро, повышаем задержку
						if (msgtmp == "msg_ratelimit") { this.MsgFreq = 1050; }
						// -- Бот незафоловлен на канал, и стоит фолоу-мод (определять, на сколько стоит фолоу-мод лень, поэтому ставим просто 0 надеясь, что это не шибко повлиет на работу)
						if (msgtmp == "msg_followersonly" || msgtmp == "msg_followersonly_zero") { this.channelparam_Follow = 0; }
						*/
						return;
					}



					// -- Мультисообщение о заходе в чат или выходе из чата пользователя
					if (!msgcheck && (msgorg.Contains("tmi.twitch.tv JOIN") || msgorg.Contains("tmi.twitch.tv PART"))) {
						//msgcheck = true;

					}



					// -- Получение и локальная установка параметров чата --
					if (!msgcheck && msgorg.Contains("tmi.twitch.tv ROOMSTATE")) {
						//if (msgorg.Contains("tmi.twitch.tv ROOMSTATE #")) {
						//msgcheck = true;
						if (msgorg.Contains("emote-only=")) {
							msgtmp = msgorg.Substring(msgorg.LastIndexOf("emote-only=") + 11);
							msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(" "));
							if (msgtmp.Length > 5) { msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(";")); }
							this.channelparam_Emote = Convert.ToBoolean(Convert.ToInt32(msgtmp));
						}
						if (msgorg.Contains("followers-only=")) {
							msgtmp = msgorg.Substring(msgorg.LastIndexOf("followers-only=") + 15);
							msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(" "));
							if (msgtmp.Length > 5) { msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(";")); }
							this.channelparam_Follow = Convert.ToInt32(msgtmp);
						}
						if (msgorg.Contains("r9k=")) {
							msgtmp = msgorg.Substring(msgorg.LastIndexOf("r9k=") + 4);
							msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(" "));
							if (msgtmp.Length > 5) { msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(";")); }
							this.channelparam_r9k = Convert.ToBoolean(Convert.ToInt32(msgtmp));
						}
						if (msgorg.Contains("subs-only=")) {
							msgtmp = msgorg.Substring(msgorg.LastIndexOf("subs-only=") + 10);
							msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(" "));
							if (msgtmp.Length > 5) { msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(";")); }
							this.channelparam_Sub = Convert.ToBoolean(Convert.ToInt32(msgtmp));
						}
						if (msgorg.Contains("slow=")) {
							msgtmp = msgorg.Substring(msgorg.LastIndexOf("slow=") + 5);
							msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(" "));
							if (msgtmp.Length > 5) { msgtmp = msgtmp.Substring(0, msgtmp.IndexOf(";")); }
							this.channelparam_Slow = Convert.ToInt32(msgtmp);
						}
					}


					// -- Получение параметров бота --
					if (msgorg.Contains("tmi.twitch.tv USERSTATE")) {
						//msgcheck = true;
						msgtmp = msgorg.Substring(msgorg.LastIndexOf("mod=") + 4);
						this.botparam_Mod = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

						msgtmp = msgorg.Substring(msgorg.LastIndexOf("subscriber=") + 11);
						this.botparam_Sub = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

					}



					// -- Обработка спец сообщений с кодами --
					// -- 353
					/*if (msgorg.Contains("tmi.twitch.tv 353")) {
						if () {
							msgtmp = msgorg.Substring(0, msgorg.IndexOf("#");
							msg = msgtmp.Substring(0, msgtmp.IndexOf(" :") + 1);
						}
						msgtmp = msgorg.Substring(0, msgorg.IndexOf("#" + Channel + " :") + 1);
						msg = msgtmp.Substring(0, msgtmp.Length);
						return;
					}

					// -- 366
					if (msgorg.Contains("tmi.twitch.tv 366")) {

						return;
					}*/

				} catch (Exception ex) {
					TechF.TechFuncs.LogDH("(Twitch) Ошибка парсинга сообщения: " + ex.Message);
				}
			}


			private ChatMessageFull ChatMsgParse(string msgorg) {
				ChatMessageFull cmfl = new ChatMessageFull();
				string msgtmp = null;
				int itmp = 0;
				cmfl.UserInfo = UserInfo(msgorg);
				cmfl.UserInfo.Permissions = UserPermissions(msgorg, cmfl.UserInfo.Badges);

				// - Айди сообщения
				msgtmp = msgorg.Substring(msgorg.LastIndexOf(";id=") + 4);
				cmfl.id = msgtmp.Substring(0, msgtmp.IndexOf(";"));

				// - Смайлики
				msgtmp = msgorg.Substring(msgorg.LastIndexOf(";emotes=") + 8);
				cmfl.Smiles = msgtmp.Substring(0, msgtmp.IndexOf(";"));

				// - Флаги
				//msgtmp = msgorg.Substring(msgorg.LastIndexOf(";flags=") + 7);
				//cmfl.Flags = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

				// -- Параметры, которые есть не у всех сообщений
				// - Тип сообщения
				itmp = msgorg.IndexOf(";msg-id=");
				if (itmp > 0) {
					msgtmp = msgorg.Substring(itmp + 8);
					cmfl.Type = msgtmp.Substring(0, msgtmp.IndexOf(";"));
				}

				// -- Выделяем сообщение
				//msgtmp = msgtmp.Substring(msgtmp.IndexOf(" :") + 1);
				//msgtmp = msgorg.Substring(itmp + this.Channel.Length + 26);
				msgtmp = msgorg.Substring(msgorg.IndexOf("PRIVMSG #" + this.Channel + " :") + this.Channel.Length + 11);
				cmfl.Message = msgtmp.Trim();

				return cmfl;
			}


			private UserInfoFull UserInfo(string msgorg) {
				UserInfoFull userinfo = new UserInfoFull();
				string msgtmp = null;

				// - Айди юзера
				msgtmp = msgorg.Substring(msgorg.IndexOf(";user-id=") + 9);
				userinfo.id = msgtmp.Substring(0, msgtmp.IndexOf(";"));

				// - Ник
				msgtmp = msgorg.Substring(msgorg.IndexOf("!") + 1);
				userinfo.Name = msgtmp.Substring(0, msgtmp.IndexOf("@"));

				msgtmp = msgorg.Substring(msgorg.IndexOf(";display-name=") + 14);
				userinfo.TWH.DisplayName = msgtmp.Substring(0, msgtmp.IndexOf(";"));
				if (userinfo.TWH.DisplayName == null || userinfo.TWH.DisplayName == "") { userinfo.TWH.DisplayName = userinfo.Name; }

				// - Какое-то инфо о значках
				//msgtmp = msgorg.Substring(msgorg.IndexOf("@badge-info=") + 12);
				//userinfo.TWH.BadgesInfo = msgtmp.Substring(0, msgtmp.IndexOf(";"));

				// - Сами значки
				msgtmp = msgorg.Substring(msgorg.IndexOf(";badges=") + 8);
				userinfo.Badges = msgtmp.Substring(0, msgtmp.IndexOf(";"));

				// - Цвет ника
				msgtmp = msgorg.Substring(msgorg.IndexOf(";color=") + 7);
				userinfo.Color = msgtmp.Substring(0, msgtmp.IndexOf(";"));

				// - ClientNonce
				//msgtmp = msgorg.Substring(msgorg.IndexOf(";client-nonce=") + 14);
				//userinfo.TWH.ClientNonce = msgtmp.Substring(0, msgtmp.IndexOf(";"));

				return userinfo;
			}

			private PermissionsInfo UserPermissions(string msgorg, string badges = null) {
				PermissionsInfo userperm = new PermissionsInfo();
				string msgtmp = null;

				if (badges == null) {
					msgtmp = msgorg.Substring(msgorg.IndexOf(";badges=") + 8);
					badges = msgtmp.Substring(0, msgtmp.IndexOf(";"));
				}

				// - Владелец канала
				userperm.isBrodcaster = badges.Contains("broadcaster");

				// - Модер
				msgtmp = msgorg.Substring(msgorg.IndexOf(";mod=") + 5);
				userperm.isModerator = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

				// - Вип
				userperm.isVIP = badges.Contains("vip");

				// - Саб
				msgtmp = msgorg.Substring(msgorg.IndexOf(";subscriber=") + 12);
				userperm.isSubscriber = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

				// - Турбо
				//msgtmp = msgorg.Substring(msgorg.IndexOf(";turbo=") + 7);
				//userperm[4] = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

				// - Прайм
				//msgtmp = msgorg.Substring(msgorg.IndexOf(";prime=") + 7);
				//userperm[5] = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

				return userperm;
			}





			// -- Буфер отправляемых сообщений --
			private void SendAwait() {
				Thread.Sleep(1000);
				while (Work) {
					if (MsgListtoSent.Count > 0) {
						if ((!channelparam_Emote || botparam_Mod) && (!channelparam_Sub || botparam_Sub || botparam_Mod) && (channelparam_Follow == (-1) || botparam_Follow || botparam_Mod)) {
							Send(MsgListtoSent.ElementAt(0));
						}
						MsgListtoSent.RemoveAt(0);
						Thread.Sleep(this.MsgFreq);
					}
					Thread.Sleep(10);
				}
				MsgListtoSent.Clear();
			}


			// -- Отправка сообщения серверу --
			private void Send(string Msg) {
				Msg = Msg.Replace("\\n", "\n");
				Msg = Msg.Replace("\\r", "\n");
				try {
					Socket.Send(Msg + "\r\n");
				} catch (Exception ex) {
					Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка отправки сообщения", "Подробности неизвестны, но возможно соединение было сброшено уже после успешного подключения", true));
				}
				long udtn = DateTimeOffset.Now.ToUnixTimeSeconds();

				TechF.TechFuncs.LogDH("(Twitch) Отправлено на сокет: " + Msg);
				if (Msg.Contains("PRIVMSG")) {
					Msg = Msg.Substring(Msg.IndexOf(":") + 1);
					Ev_BotMsg?.Invoke(this, new Twident_ChatMsg(1, null, Login, Login, 0, Msg, udtn, null, botparam_Brodcaster, botparam_Mod, botparam_VIP, botparam_Sub));
				}
			}


			// -- Отправка команд в чат
			public void SendCom(string Command, string[] Params = null) {
				if (!TechF.HideMode && this.Work) {
					MsgListtoSent.Add("/" + Command);
				}
			}

			// -- Отправка обычного сообщения в чат
			public void SendMsg(string[] usr, string msg) {
				if (!TechF.HideMode && this.Work) {
					if (usr != null) {
						MsgListtoSent.Add("PRIVMSG " + "#" + Channel + " :" + "@" + string.Join(", @", usr) + ", " + msg);
					} else {
						MsgListtoSent.Add("PRIVMSG " + "#" + Channel + " :" + msg);
					}
				}
			}
			public void SendMsg(object sender, Twident_Msg e) {
				if (!TechF.HideMode && this.Work) {
					MsgListtoSent.Add("PRIVMSG " + "#" + Channel + " :" + e.Message);
				}
			}



			// -- Закрытие соединения --
			public void Close() {
				if (this.Work) {
					Ev_Status?.Invoke(this, new Twident_Status(2, "Отключение", null, false));
					TechF.TechFuncs.LogDH("(Twitch) Отключение от чата...");
					Work = false;
					Send("PART #" + Channel);
					Socket.Close();
					while (Socket.State == WebSocketState.Closing) { Thread.Sleep(100); }
					TechF.TechFuncs.LogDH("(Twitch) Выполнено отключение от чата");
					Ev_Status?.Invoke(this, new Twident_Status(0, "Отключено", null, null));
					Ev_ConnectStatus?.Invoke(this, new Twident_Bool(false));
				}
			}
		}
	}
}