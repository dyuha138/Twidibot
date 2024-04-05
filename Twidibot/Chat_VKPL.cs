using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;

namespace Twidibot {
	public partial class VKPL {

		public class ChatC {
			public event EventHandler<Twident_ChatMsg> Ev_ChatMsg;
			//public event EventHandler<Twident_Msg> Ev_Notice;
			//public event EventHandler<Twident_Msg> Ev_MsgSented;
			public event EventHandler<Twident_ChatMsg> Ev_BotMsg;
			public event EventHandler<Twident_Status> Ev_GlobalStatus;
			public event EventHandler<Twident_Status> Ev_Status;
			public event EventHandler<Twident_Bool> Ev_ConnectStatus;

			//public ChatFuncs Funcs = null;
			private BackWin TechF = null;
			delegate void FuncCDt();
			Type FuncT = null;

			private ObservableCollection<string> MsgListtoSent = null;

			// -- Блок переменных для функции парсинга сообщений
			private bool msgcheck = false;
			private long secdif = 0;
			private string unick = null;
			private string udispnick = null;
			private string msgid = null;
			private string msgtype = null;
			private int uid = 0;
			private string ucolor = null;
			private string uemotes = null;
			private bool uisOwner = false;
			private bool uisMod = false;
			private bool uisVip = false;
			private bool uisSub = false;
			private bool isTurbo = false;
			private bool isPrime = false;
			private string ubadgeinfo = null;
			private string ubadges = null;
			private bool comdtcheck = false;
			private DB.DefCommand_tclass DefCl = null;
			private DB.FuncCommand_tclass FuncCl = null;


			// -- Данные аккаунта, который будет работать как бот --
			public string Channel { get; private set; } // -- В чей чат будем заходить
			public string Login { get; private set; } // -- Логин аккаунта пользователя
			private string Pass { get; set; } // -- Пароль (OAuth код с https://twitchapps.com/tmi/) аккаунта пользователя

			public bool Work = false;
			public bool ErrPass = false;
			public bool Moderator = false;
			public bool AuthBot = false;
			public int MsgFreq = 1050;
			//private WebSocket Socket = null;
			private EdgeOptions BRWoptions = null; // -- Опции запуска
			private EdgeDriver BRWdriver = null; // -- Сам браузер
			private WebDriverWait BRWwait = null;
			private EdgeDriverService BRWdriverservice = null;


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

			public bool botparam_Mod {
				get { return this.pbotparam_Mod; }
				set
				{
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
				set { this.pbotparam_VIP = value; }
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
				FuncT = backWin.ChatFuncs.GetType();
				this.MsgListtoSent = new ObservableCollection<string>();
				//DBLoadSet();
				//NewSocket();
				InitBRW();
			}

			public ChatC(BackWin backWin, string Channel) {
				this.TechF = backWin;
				FuncT = backWin.ChatFuncs.GetType();
				this.MsgListtoSent = new ObservableCollection<string>();
				TechF.db.SettingsT.UpdateSetting("Channel", Channel);
				TechF.db.SettingsT.UpdateSetting("SimpleMode", "1");
				//DBLoadSet();
				//NewSocket();
				InitBRW();
			}
			public ChatC(BackWin backWin, string Login, string Pass) {
				this.TechF = backWin;
				FuncT = backWin.ChatFuncs.GetType();
				this.MsgListtoSent = new ObservableCollection<string>();
				TechF.db.SettingsT.UpdateSetting("Login", Login);
				TechF.db.SettingsT.UpdateSetting("Pass", Pass);
				TechF.db.SettingsT.UpdateSetting("SimpleMode", "1");
				//DBLoadSet();
				//NewSocket();
				InitBRW();
			}
			public ChatC(BackWin backWin, string Channel, string Login, string Pass) {
				this.TechF = backWin;
				FuncT = backWin.ChatFuncs.GetType();
				this.MsgListtoSent = new ObservableCollection<string>();
				TechF.db.SettingsT.UpdateSetting("Channel", Channel);
				TechF.db.SettingsT.UpdateSetting("Login", Login);
				TechF.db.SettingsT.UpdateSetting("Pass", Pass);
				TechF.db.SettingsT.UpdateSetting("SimpleMode", "1");
				//DBLoadSet();
				//NewSocket();
				InitBRW();
			}


			private void tst(object sender, DataReceivedEventArgs e) {
				
			}


			// -- Установка канала, логина и пароля из базы данных или сохранение их в базу --
			public void DataUpdate() {
				if (TechF.VKPL.BotChannelisOne) {
					this.Channel = TechF.db.SettingsVKPLList.Find(x => x.id == 2).Param;
				} else {
					this.Channel = TechF.db.SettingsVKPLList.Find(x => x.id == 1).Param;
				}
				this.Login = TechF.db.SettingsVKPLList.Find(x => x.id == 2).Param;
				this.Pass = TechF.db.SettingsVKPLList.Find(x => x.id == 3).Param;
			}


			/*private void NewSocket() {
				this.Socket = new WebSocket("");
				this.Socket.MessageReceived += MsgParsing;
				this.Socket.Error += ErrWS;
				this.Socket.EnableAutoSendPing = false;
				this.DataReceived += tst;
			}*/

			private void InitBRW() {
				this.BRWoptions = new EdgeOptions();
				this.BRWoptions.AddArgument("--headless");
				this.BRWoptions.AddArgument("--mute-audio");
				this.BRWdriverservice = EdgeDriverService.CreateDefaultService();
				this.BRWdriverservice.HideCommandPromptWindow = true;
			}




			// -- Функция подключения к чату --
			public bool Connect(bool isReconnect = false) {
				if (TechF.VKPL.Active) {
					this.DataUpdate();
					if (!isReconnect) {
						//TechF.TechFuncs.LogDH("(VKPL) Запуск браузера...");
						if (Ev_Status != null) { Ev_Status(this, new Twident_Status(0, "Подключение к серверу...", null, false)); }
					}

					Thread.Sleep(300);

					if (!ErrPass) {
					

						if (!Work) { Task.Factory.StartNew(() => StatusCheck()); }
						if (!Work) { Task.Factory.StartNew(() => SendAwait()); }
						if (!Work) { Task.Factory.StartNew(() => TechF.ChatFuncs.Init()); } // -- Запуск фоновых функций
						this.Work = true;
						if (Ev_ConnectStatus != null) { Ev_ConnectStatus(this, new Twident_Bool(true)); }
						if (Ev_Status != null) { Ev_Status(this, new Twident_Status(3, "Пример ошибки", null, null)); }
						return true;
					} else {
						if (Ev_ConnectStatus != null) { Ev_ConnectStatus(this, new Twident_Bool(false)); }
						return false;
					}
				} else { return false; }
			}

			private void ErrWS(object sender, SuperSocket.ClientEngine.ErrorEventArgs e) {
				if (Ev_Status != null) { Ev_Status(this, new Twident_Status(3, "Ошибка подключения", e.Exception.Message, true)); }
			}


			private void StatusCheck() {
				Thread.Sleep(1000);
				while (Work) {

					Thread.Sleep(1000);
				}
			}



			// -- Обработка поступающих сообщений --
			private void MsgParsing(object sender, MessageReceivedEventArgs e) {
				string msgorg = e.Message, msgtmp = null, msg = null, msgcom = null;
				string[] msgparams = null;
				string[] msgpartmp = null;

				TechF.TechFuncs.LogDH("(VKPL) Получено сообщение: " + msgorg);

				try {
					long udtn = DateTimeOffset.Now.ToUnixTimeSeconds();


					// -- Обнаружение приветственного сообщения --
					/*if (msgorg.IndexOf("") > 0) {
						msgcheck = true;
					}*/
					


					// -- Разбираем сообщение в чате на отдельные компоненты --
					if (msgorg.IndexOf("") > 0) { // -- У нас сообщение в чате
						msg = UserInfo(msgorg); // -- Определяем данные пользователя

						//if (Ev_ChatMsg != null) { Ev_ChatMsg(this, new Twident_ChatMsg(2, msgid, unick, udispnick, uid, msg, udtn, ucolor, uisOwner, false, uisMod, uisVip, uisSub, ubadgeinfo, ubadges)); } // -- Отправляем полученное сообщение через ивент


						// -- Обработка команд в чате --
						if (msg.IndexOf(" ") > 0) {
							msgcom = msg.Substring(0, msg.IndexOf(" "));
						} else { msgcom = msg; }
						msgcom = msgcom.Replace(",", "");

						//msgparams[] = msg.Substring(msg.IndexOf(" "));
						if (msgcom != msg) {
							msgparams = msg.Substring(msg.IndexOf(" ") + 1).Split(' ');
						}


						// -- Поиск команды в списках --
						DefCl = TechF.db.DefCommandsList.Find(x => x.Command == msgcom || x.Command.ToLower() == msgcom.ToLower() || x.Command.ToLowerInvariant() == msgcom.ToLowerInvariant());
						FuncCl = TechF.db.FuncCommandsList.Find(x => x.Command == msgcom || x.Command.ToLower() == msgcom.ToLower() || x.Command.ToLowerInvariant() == msgcom.ToLowerInvariant());

						// -- Обычная команда --
						//if (DefCl != null && DefCl.Enabled && ((DefCl.Secured && TechF.TechFuncs.VIPUserValidate(uid)) || !DefCl.Secured)) {
						if (DefCl != null && DefCl.Enabled) {

							secdif = udtn - DefCl.LastUsed; // -- Вычисляем разницу между текущей датой и датой последнего использования

							if (secdif < DefCl.CoolDown) {
								//this.SendMsg("@" + udispnick + ", кулдаун (ещё " + (DefCl.CoolDown - secdif) + " " + TechF.TechFuncs.WordEndNumber(DefCl.CoolDown - secdif, "s") + ")");
							} else {
								this.SendMsg(DefCl.Result);
								TechF.db.DefCommandsT.UpdateLastUsed(DefCl.id, udtn); // -- Обновление даты использования команды
							}
						}


						// -- Функциональная команда --
						if (FuncCl != null && FuncCl.Enabled && ((FuncCl.Secured && TechF.TechFuncs.VIPUserValidate(uid)) || !FuncCl.Secured)) {

							secdif = udtn - FuncCl.LastUsed; // -- Вычисляем разницу между текущей датой и датой последнего использования

							if (secdif < FuncCl.CoolDown) {
								//this.SendMsg("@" + udispnick + ", кулдаун (ещё " + (FuncCl.CoolDown - secdif) + " " + TechF.TechFuncs.WordEndNumber(FuncCl.CoolDown - secdif, "s") + ")");
							} else {
								if (msgparams == null) { // -- Если параметров в чате нет, то берём стандартные
									msgparams = FuncCl.Params.Split(';');
								} else {
									if (msgparams.Length < FuncCl.Params.Split(';').Length) { // -- Загружаем недостающие параметры загрузкой всех стандартных параметров и заменой указаными
										msgpartmp = msgparams;
										msgparams = null;
										msgparams = FuncCl.Params.Split(';');
										for (ushort i = 0; i < msgpartmp.Length; i++) {
											msgparams[i] = msgpartmp[i];
										}
									}
								}
								msgpartmp = msgparams;
								msgparams = null;
								msgparams = new string[msgpartmp.Length + 2];
								for (ushort i = 0; i < msgpartmp.Length; i++) {
									msgparams[i] = msgpartmp[i];
								}
								msgparams[msgparams.Length - 2] = Convert.ToString(uisOwner) + ";" + Convert.ToString(uisMod) + ";" + Convert.ToString(uisVip) + ";" + Convert.ToString(uisSub);
								msgparams[msgparams.Length - 1] = msgid + ";" + uid + ";" + unick + ";" + udispnick + ";" + ucolor;

								MethodInfo mi = FuncT.GetMethod(FuncCl.FuncName); // -- Поиск кода функции
								Task.Factory.StartNew(() => {
									try {
										mi.Invoke(TechF.ChatFuncs, msgparams); // -- Выполнение функции
									} catch (TargetParameterCountException) {
										SendMsg("@" + udispnick + ", Извините, но вы написали слишком много параметров, я пока не умею обрезать лишнее");
										TechF.TechFuncs.LogDH("Ошибка выполнения функциональной команды: Пользователь указал параметров больше, чем их может быть");
									} catch (Exception el) {
										SendMsg("@" + udispnick + ", Ой, что-то пошло не так, я не могу выполнить команду, извините");
										TechF.TechFuncs.LogDH("Ошибка выполнения функциональной команды: " + el.Message + " | " + el.InnerException);
									}
								});
								TechF.db.FuncCommandsT.UpdateLastUsed(FuncCl.id, udtn); // -- Обновление даты использования команды
							}
						}

						/*if (DefCl == null && FuncCl == null) {
							if (msgcom.StartsWith("!")) {
								this.SendMsg("@" + nick + ", команда \"" + msgcom + "\" не найдена");
							}
						}*/

						DefCl = null;
						FuncCl = null;
						return;
					}


	


					
					// -- Получение и локальная установка параметров чата --
					if (!msgcheck && msgorg.Contains("ROOMSTATE")) {
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
					if (msgorg.Contains("USERSTATE")) {
						//msgcheck = true;
						msgtmp = msgorg.Substring(msgorg.LastIndexOf("mod=") + 4);
						this.botparam_Mod = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

						msgtmp = msgorg.Substring(msgorg.LastIndexOf("subscriber=") + 11);
						this.botparam_Sub = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

					}




				} catch (Exception ex) {
					TechF.TechFuncs.LogDH("(VKPL) Ошибка парсинга сообщения: " + ex.Message);
				}
			}



			private string UserInfo(string msgorg, int msgtypel = 0, bool msgReturn = true) {
				string msgtmp = null;
				string msg = null;
				// - Ник
				msgtmp = msgorg.Substring(msgorg.IndexOf("!") + 1);
				unick = msgtmp.Substring(0, msgtmp.IndexOf("@"));

				msgtmp = msgorg.Substring(msgorg.IndexOf(";display-name=") + 14);
				udispnick = msgtmp.Substring(0, msgtmp.IndexOf(";"));
				if (udispnick == null || udispnick == "") { udispnick = unick; }

				// - Айди юзера
				msgtmp = msgorg.Substring(msgorg.LastIndexOf(";user-id=") + 9);
				uid = Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";")));

				// - Айди сообщения
				msgtmp = msgorg.Substring(msgorg.LastIndexOf(";id=") + 4);
				msgid = msgtmp.Substring(0, msgtmp.IndexOf(";"));

				// - Какое-то инфо о значках
				msgtmp = msgorg.Substring(msgorg.LastIndexOf("@badge-info=") + 12);
				ubadgeinfo = msgtmp.Substring(0, msgtmp.IndexOf(";"));

				// - Сами значки
				msgtmp = msgorg.Substring(msgorg.LastIndexOf(";badges=") + 8);
				ubadges = msgtmp.Substring(0, msgtmp.IndexOf(";"));

				// - Цвет ника
				msgtmp = msgorg.Substring(msgorg.LastIndexOf(";color=") + 7);
				ucolor = msgtmp.Substring(0, msgtmp.IndexOf(";"));

				// - Смайлики
				msgtmp = msgorg.Substring(msgorg.LastIndexOf(";emotes=") + 8);
				uemotes = msgtmp.Substring(0, msgtmp.IndexOf(";"));

				// - Флаги
				//msgtmp = msgorg.Substring(msgorg.LastIndexOf(";flags=") + 7);
				//flags = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

				// -- Права пользователя
				// - Владелец канала, через значки
				uisOwner = ubadges.Contains("broadcaster");

				// - Модер
				msgtmp = msgorg.Substring(msgorg.LastIndexOf(";mod=") + 5);
				uisMod = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

				// - Вип через значки
				uisVip = ubadges.Contains("vip");

				// - Саб
				msgtmp = msgorg.Substring(msgorg.LastIndexOf(";subscriber=") + 12);
				uisSub = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

				// - Турбо
				//msgtmp = msgorg.Substring(msgorg.LastIndexOf(";turbo=") + 7);
				//isTurbo = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

				// - Прайм
				//msgtmp = msgorg.Substring(msgorg.LastIndexOf(";prime=") + 7);
				//isPrime = Convert.ToBoolean(Convert.ToInt32(msgtmp.Substring(0, msgtmp.IndexOf(";"))));

				// -- Параметры, которые есть только у других сообщений
				if (msgtypel == 1 || msgtypel == 2) {
					// - Тип сообщения
					msgtmp = msgorg.Substring(msgorg.IndexOf(";msg-id=") + 8);
					msgtype = msgtmp.Substring(0, msgtmp.IndexOf(";"));
				}

				// -- Выделяем сообщение
				//msgtmp = msgtmp.Substring(msgtmp.IndexOf(" :") + 1);
				msgtmp = msgorg.Substring(msgorg.IndexOf("#" + this.Channel + " :") + this.Channel.Length + 3);
				//msgtmp = msgtmp.Substring(msgtmp.IndexOf(" :") + 2);
				msg = msgtmp.Trim();

				return msg;
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
					
				} catch (Exception ex) {
					if (Ev_Status != null) { Ev_Status(this, new Twident_Status(3, "Ошибка отправки сообщения", "Подробности неизвестны, но возможно соединение было сброшено уже после успешного подключения", true)); }
				}
				long udtn = DateTimeOffset.Now.ToUnixTimeSeconds();

				TechF.TechFuncs.LogDH("Отправлено на сокет: " + Msg);
				if (Msg.Contains("PRIVMSG")) {
					Msg = Msg.Substring(Msg.IndexOf(":") + 1);
					//if (Ev_BotMsg != null) { Ev_BotMsg(this, new Twident_ChatMsg(2, null, Login, Login, 0, Msg, udtn, null, false, false, botparam_Mod, botparam_VIP, botparam_Sub)); }
				}
			}


			// -- Отправка команд в чат
			public void SendCom(string Command, string[] Params = null) {
				SendMsg("/" + Command);
			}

			// -- Отправка обычного сообщения в чат
			public void SendMsg(string msg) {
				if (!TechF.HideMode) {
					MsgListtoSent.Add("PRIVMSG " + "#" + Channel + " :" + msg);
				}
			}
			public void SendMsg(object sender, Twident_Msg e) {
				if (!TechF.HideMode) {
					MsgListtoSent.Add("PRIVMSG " + "#" + Channel + " :" + e.Message);
				}
			}



			// -- Закрытие соединения --
			public void Close() {
				TechF.TechFuncs.LogDH("(VKPL) Закрытие браузера EDGE...");
				Work = false;
				if (Ev_Status != null) { Ev_Status(this, new Twident_Status(2, "Отключение...", null, false)); }
				Send("PART #" + Channel);
				
				
				TechF.TechFuncs.LogDH("(VKPL) EDGE закрыт");
				if (Ev_Status != null) { Ev_Status(this, new Twident_Status(0, "Отключено", null, null)); }
				if (Ev_ConnectStatus != null) { Ev_ConnectStatus(this, new Twident_Bool(false)); }
			}
		}


	}
}
