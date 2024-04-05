using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WebSocket4Net;
using System.Globalization;

namespace Twidibot {
	public partial class VKPL {

		public class ChatCBRW {
			public event EventHandler<Twident_ChatMsg> Ev_ChatMsg;
			//public event EventHandler<Twident_Msg> Ev_MsgSented;
			public event EventHandler<Twident_ChatMsg> Ev_BotMsg;
			public event EventHandler<Twident_Status> Ev_GlobalStatus;
			public event EventHandler<Twident_Status> Ev_Status;
			public event EventHandler<Twident_Bool> Ev_ConnectStatus;
			public event EventHandler<Twident_Status> Ev_tst;

			public ChatFuncs Funcs = null;
			private BackWin TechF = null;
			delegate void FuncCDt();
			Type FuncT = null;

			private ObservableCollection<VKPLsendmsg> MsgListtoSent = null;

			// -- Блок переменных для функции парсинга сообщений
			//private bool comdtcheck = false;
			private DB.DefCommand_tclass DefCl = null;
			private DB.FuncCommand_tclass FuncCl = null;
			private bool RutonyMode = false;


			// -- Данные аккаунта, который будет работать как бот --
			public string Channel { get; private set; } // -- В чей чат будем заходить
			public string Login { get; private set; } // -- Логин аккаунта пользователя
			public bool Work { get; private set; }
			public bool MonitorWork { get; private set; }
			public bool StreamWork { get; private set; }
			public bool Starting { get; private set; }
			public bool AuthBot { get; private set; }
			public int MsgFreq = 1050;

			private int MsgCountPrev = 0;

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
				set {
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
				set {
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
			public int channelparam_Slow {
				get { return this.pchannelparam_Slow; }
				set { this.pchannelparam_Slow = value; }
			}



			// -- Конструкторы --
			public ChatCBRW(BackWin backWin) {
				this.TechF = backWin;
				FuncT = backWin.ChatFuncs.GetType();
				this.MsgListtoSent = new ObservableCollection<VKPLsendmsg>();
				//DBLoadSet();
				//NewSocket();
				//Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка запуска браузера", null, true));
				Task.Factory.StartNew(() => tst());
			}

			public ChatCBRW(BackWin backWin, string Channel) {
				this.TechF = backWin;
				FuncT = backWin.ChatFuncs.GetType();
				this.MsgListtoSent = new ObservableCollection<VKPLsendmsg>();
				TechF.db.SettingsT.UpdateSetting("Channel", Channel);
				TechF.db.SettingsT.UpdateSetting("SimpleMode", "1");
				//DBLoadSet();
				//NewSocket();
				//Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка запуска браузера", null, true));
			}


			private void tst() {
				while (true) {
					Ev_tst?.Invoke(null, new Twident_Status(0, this.Work.ToString(), null, null));
					Ev_tst?.Invoke(null, new Twident_Status(1, this.MonitorWork.ToString(), null, null));
					Ev_tst?.Invoke(null, new Twident_Status(2, this.StreamWork.ToString(), null, null));
					//Ev_tst?.Invoke(null, new Twident_Status(3, this.Starting.ToString(), null, null));
					Thread.Sleep(500);
				}
			}


			// -- Установка канала, логина и пароля из базы данных или сохранение их в базу --
			public void DataUpdate() {
				if (TechF.VKPL.BotChannelisOne) {
					this.Channel = TechF.db.SettingsVKPLList.Find(x => x.id == 2).Param;
				} else {
					this.Channel = TechF.db.SettingsVKPLList.Find(x => x.id == 1).Param;
				}
				this.Login = TechF.db.SettingsVKPLList.Find(x => x.id == 2).Param;
				this.RutonyMode = Convert.ToBoolean(Convert.ToInt32(TechF.db.SettingsVKPLList.Find(x => x.id == 22).Param));
			}

					


			// -- Функция подключения к чату --
			public bool Connect(bool isReconnect = false) {
				try {
					if (TechF.VKPL.Active) {
						this.DataUpdate();
						IWebElement elementtmp = null;
						bool Err = false;
						string[] authcookie = null;
						this.Starting = true;

						if (!isReconnect) {
							TechF.TechFuncs.LogDH("(VKPL) Запуск браузера");
							Ev_Status?.Invoke(this, new Twident_Status(0, "Запуск браузера", null, false));
							TechF.VKPL.APIBRW.InitBRW(true);
							TechF.VKPL.APIBRW.StartBRW();
						} else {
							TechF.VKPL.APIBRW.BRWdriver.Navigate().Refresh();
							Thread.Sleep(2000);
						}

						TechF.TechFuncs.LogDH("(VKPL) Открытие канала");
						Ev_Status?.Invoke(this, new Twident_Status(0, "Открытие канала", null, null));
						TechF.VKPL.APIBRW.BRWdriver.Navigate().GoToUrl("https://vkplay.live/" + Channel.ToLower()); // - Открываем вкладку
						Ev_Status?.Invoke(this, new Twident_Status(0, "Ожидание дозагрузки страницы", null, null));
						Thread.Sleep(2000);

						TechF.TechFuncs.LogDH("(VKPL) Авторизация");
						Ev_Status?.Invoke(this, new Twident_Status(0, "Авторизация", null, null));
						// -- Добавление кука для авторизации
						if (TechF.TechFuncs.GetSettingVKPLParam("Bot_AuthCode") == "true") {
							authcookie = TechF.db.SettingsVKPLList.Find(x => x.SetName == "Bot_AuthCode").Param.Split(';');
							try {
								TechF.VKPL.APIBRW.BRWdriver.Manage().Cookies.AddCookie(new Cookie("auth", authcookie[0], ".vkplay.live", "/", DateTime.ParseExact(authcookie[1], "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture), true, false, "None"));
							} catch (Exception ex) {
								Exception exl = ex;
							}
						}

						TechF.VKPL.APIBRW.BRWdriver.Navigate().Refresh();
						Thread.Sleep(2000);


						// -- Проверка на вход в аккаунт
						try {
							elementtmp = TechF.VKPL.APIBRW.BRWdriver.FindElement(By.ClassName("TopMenuRightUnAuthorized_signIn_WJLAc"));
						} catch (Exception) { }
						if (elementtmp != null) {
							Ev_Status?.Invoke(this, new Twident_Status(3, "Не выполнен вход в аккаунт", null, true));
							Ev_GlobalStatus?.Invoke(this, new Twident_Status(2, "Требуется авторизация VKPL", "Перейдите в настройки VKPL и войдите в аккаунт в браузере", true));
							Ev_ConnectStatus?.Invoke(this, new Twident_Bool(false));
							this.Close();
							this.Starting = false;
							return false;
						}

						// -- Проверка на существование канала
						try {
							elementtmp = TechF.VKPL.APIBRW.BRWdriver.FindElement(By.ClassName("Error_title_MyZoO"));
						} catch (Exception) { }
						if (elementtmp != null) {
							Ev_Status?.Invoke(this, new Twident_Status(3, "Канала не существует", null, true));
							Ev_ConnectStatus?.Invoke(this, new Twident_Bool(false));
							TechF.VKPL.APIBRW.CloseBRW();
							this.Starting = false;
							return false;
						} else {
							//try {
							//	TechF.VKPL.APIBRW.BRWdriver.FindElement(By.ClassName("StreamStatus_text_Xoh1I"));
							//	Ev_Status?.Invoke(this, new Twident_Status(2, "Канал оффлайн, ожидание", null, null));
							//	Task.Factory.StartNew(() => ChannelStatusOnCheck());
							//	TechF.TechFuncs.LogDH("(VKPL) Запущено ожидание запуска стрима на канале");
							//	Err = true;
							//} catch (Exception) {
								Ev_Status?.Invoke(this, new Twident_Status(1, "Подключено", null, null));
							//}
						}

						this.Work = true;
						if (!Err) {
							this.StreamWork = true;
							TechF.TechFuncs.LogDH("(VKPL) Подключено успешно");
							Task.Factory.StartNew(() => SendAwait());
							Task.Factory.StartNew(() => MsgMonitor());
							Task.Factory.StartNew(() => TechF.ChatFuncs.Init()); // -- Запуск фоновых функций
							//Task.Factory.StartNew(() => ChannelStatusOffCheck());
							Ev_Status?.Invoke(this, new Twident_Status(1, "Подключено", null, null));
							Task.Factory.StartNew(() => { TechF.VKPL.APIBRW.StartStopStream(); TechF.VKPL.APIBRW.ResolutionSet(true); TechF.VKPL.APIBRW.StartStopStreamBackground(); });
						}

						//this.Work = true;
						Ev_ConnectStatus?.Invoke(this, new Twident_Bool(true));
						this.Starting = false;
						return true;

					}
				} catch (ObjectDisposedException) { }
				this.Starting = false;
				return false;
			}


			// -- Ожидание на запуск стрима --
			private void ChannelStatusOnCheck() {
				Thread.Sleep(1000);
				try { TechF.VKPL.APIBRW.BRWdriver.FindElement(By.ClassName("LastStreamInfo_text_DmK6e")); // - Типа костыль, чтобы событие о состоянии не спамилось
					Ev_Status?.Invoke(this, new Twident_Status(2, "Канал оффлайн, ожидание", null, null));
				} catch (Exception) { }

				while (this.Work) {
					try {
						TechF.VKPL.APIBRW.BRWdriver.FindElement(By.ClassName("LastStreamInfo_text_DmK6e"));
					} catch (Exception) {
						this.StreamWork = true;
						Task.Factory.StartNew(() => MsgMonitor());
						Task.Factory.StartNew(() => SendAwait());
						Task.Factory.StartNew(() => TechF.ChatFuncs.Init());
						Task.Factory.StartNew(() => ChannelStatusOffCheck());
						Ev_Status?.Invoke(this, new Twident_Status(1, "Подключено", null, null));
						TechF.TechFuncs.LogDH("(VKPL) Обнаружен стрим, полноценная работа запущена");
						TechF.VKPL.APIBRW.StartStopStream();
						break;
					}
					Thread.Sleep(3000);
				}
			}

			// -- Отслеживание остановки стрима
			private void ChannelStatusOffCheck() {
				Thread.Sleep(1000);
				while (this.Work) {
					try {
						TechF.VKPL.APIBRW.BRWdriver.FindElement(By.ClassName("LastStreamInfo_text_DmK6e"));
						this.StreamWork = false;
						Task.Factory.StartNew(() => ChannelStatusOnCheck());
						Ev_Status?.Invoke(this, new Twident_Status(2, "Канал оффлайн, ожидание", null, null));
						TechF.TechFuncs.LogDH("(VKPL) Стрим завершён, переход в режим ожидания");
						break;
					} catch (Exception) { }
					Thread.Sleep(3000);
				}
			}
		



			// --- Отслеживание новых сообщений (типа аналог события прихода сообщения через вебсокет) ---
			public void MsgMonitor() {
				this.MonitorWork = true;
				Thread.Sleep(1000);
				//ReadOnlyCollection<IWebElement> msglist = TechF.VKPL.APIBRW.GetAllMessages();
				List<string> msglist = TechF.VKPL.APIBRW.GetChatHTML();
				List<ChatMessageFull> cmflistold = new List<ChatMessageFull>();
				List<ChatMessageFull> cmflistnew = new List<ChatMessageFull>();
				ChatMessageFull cmflold = null;
				ChatMessageFull cmflnew = null;

				// -- Парсим каждое сообщение
				for (int i = 0; i < msglist.Count; i++) { // - Парсим каждое сообщение
					cmflistold.Add(this.ChatMsgParse(msglist.ElementAt(i)));
				}

				// -- Сохраняем последнее сообщение
				if (cmflistold.Count > 0) { cmflold = cmflistold.ElementAt(cmflistold.Count - 1); }

				
					// -- Цикл парсинга новых сообщений
				while (this.Work) {
					msglist = TechF.VKPL.APIBRW.GetChatHTML(); // - Обновляем список сообщений
					cmflistnew.Clear();
					
					if (msglist != null) {
						for (int i = 0; i < msglist.Count; i++) { // - Парсим каждое сообщение
							cmflistnew.Add(this.ChatMsgParse(msglist.ElementAt(i)));
						}
						Ev_tst?.Invoke(null, new Twident_Status(3, "норм", null, null));
					} else {
						Ev_tst?.Invoke(null, new Twident_Status(3, "не норм", null, null));
						continue;
					}
					
					

					// -- Ищем сохранённое сообщение в новом списке
					for (int i = cmflistnew.Count - 1; i >= 0; i--) {
						if (this.ChatMessageFullCompare(cmflold, cmflistnew.ElementAt(i))) {
							try { cmflnew = cmflistnew.ElementAt(i + 1);
							} catch (ArgumentOutOfRangeException) { break; }
							TechF.TechFuncs.LogDH("(VKPL) Необработанное новое HTML-сообщение: " + msglist.ElementAt(msglist.Count - 1));
							if (cmflnew.UserInfo.Name.ToLower() != this.Login.ToLower()) { this.FuncStart(cmflnew); } // - Запускаем обработку следующего сообщения
							cmflold = cmflnew;
							break;
						}
					}
					
					Thread.Sleep(600);
				}

				this.MonitorWork = false;
			}



			// -- Функция сравнения двух чатмесседжей --
			private bool ChatMessageFullCompare(ChatMessageFull CMF1, ChatMessageFull CMF2) {
				//return CMF1.Equals(CMF2);

				if (CMF1.UserInfo.Name == CMF2.UserInfo.Name &&
					CMF1.UserInfo.Badges == CMF2.UserInfo.Badges &&
					CMF1.UserInfo.Color == CMF2.UserInfo.Color &&
					CMF1.Message == CMF2.Message) {
					return true;
				} else { return false; }
			}






			// -- Обработка поступающих сообщений --
			private void FuncStart(ChatMessageFull cmf) {
				string msgorg = null, msgtmp = null, msg = null, msgcom = null;
				long secdif = 0;

				//TechF.TechFuncs.LogDH("(VKPL) Получено сообщение: msg_id=\"" + cmf.id + "\", badges_urlcode=\"" + cmf.UserInfo.Badges + "\", smiles=\"" + cmf.Smiles + "\", flags=\"" + cmf.Flags + "\", color=\"" + cmf.UserInfo.Color + "\", user_id=\"" + cmf.UserInfo.id + " #" + Channel + " " + "@" + cmf.UserInfo.Name + ": " + cmf.Message);

				try {
					long udtn = DateTimeOffset.Now.ToUnixTimeSeconds();


					// -- Разбираем сообщение в чате на отдельные компоненты --
					//if (msgorg.IndexOf("tmi.twitch.tv PRIVMSG") > 0) { // -- У нас сообщение в чате
						//ChatMessageFull cmf = ChatMsgParse(iwe); // -- Парсим сообщение

						Ev_ChatMsg?.Invoke(this, new Twident_ChatMsg(2, cmf.id, cmf.UserInfo.Name, cmf.UserInfo.Name, 0, cmf.Message, udtn, cmf.UserInfo.Color, cmf.UserInfo.Permissions.isBrodcaster, cmf.UserInfo.Permissions.isModerator, cmf.UserInfo.Permissions.isVIP, cmf.UserInfo.Permissions.isSubscriber, cmf.UserInfo.TWH.BadgesInfo, cmf.UserInfo.Badges));

						// -- Обработка команды --
						if (cmf.Message.IndexOf(" ") > 0) {
							msgcom = cmf.Message.Substring(0, cmf.Message.IndexOf(" "));
						} else { msgcom = cmf.Message; }
						msgcom = msgcom.Replace(",", "");


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
								this.SendMsg(null, DefCl.Result);
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
								ServiceInfo ServiceInfo = new ServiceInfo(2, new PermissionsInfo(this.botparam_Brodcaster, this.botparam_Mod, this.botparam_VIP, this.botparam_Sub, this.botparam_Follow));
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
								funcparams[1] = TechF.TechFuncs.UserInfoFullDownCast(cmf.UserInfo, 2);
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
						//return;
					//}


				} catch (Exception ex) {
					TechF.TechFuncs.LogDH("(VKPL) Ошибка парсинга сообщения: " + ex.Message);
				}
			}



			// --- Парсинг сообщения ---
			// -- Общая функция --
			private ChatMessageFull ChatMsgParse(IWebElement msgorg) {
				ChatMessageFull cmfl = new ChatMessageFull();
				string msgtmp = null;
				int itmp = 0;
				cmfl.UserInfo = UserInfo(msgorg);
				cmfl.UserInfo.Permissions = UserPermissions(cmfl.UserInfo.Badges);


				// -- Сообщение
				cmfl.Message = msgorg.Text.Substring(msgorg.Text.IndexOf(":") + 3);
				
				return cmfl;
			}

			
			public ChatMessageFull ChatMsgParse(string msgorg) {
				ChatMessageFull cmfl = new ChatMessageFull();
				string strtmp = null;
				string strtmp2 = null;
				string[] listmsgpart = null;
				string[] listsmile = null;
				int itmp = 0;
				bool reply = false;
				bool smile = false;
				bool msgend = false;

				msgorg = msgorg.Replace("&nbsp;", "");
				cmfl.UserInfo = UserInfo(msgorg);
				cmfl.UserInfo.Permissions = UserPermissions(cmfl.UserInfo.Badges);

				// -- Сообщение --
				// -- Ответ
				if (msgorg.Contains("ContentRenderer_mention_eMmgA")) {
					strtmp = msgorg.Substring(msgorg.IndexOf("ContentRenderer_mention_eMmgA") + 40);
					strtmp = TechF.TechFuncs.SearchinText(strtmp, "data-display-name", "", "", "\"", "\"");
					cmfl.Message = "@" + strtmp + " ";
					reply = true;
				} else {
					cmfl.Message = "";
				}

				if (reply) {
					strtmp = msgorg.Substring(msgorg.IndexOf("contenteditable=") + 16);
					strtmp2 = strtmp.Substring(strtmp.IndexOf("</span>") + 7);
				} else {
					strtmp2 = msgorg.Substring(msgorg.IndexOf("BlockRenderer_noIndent_yWqyP") + 30);
				}

				while (!msgend) {
					if (!strtmp2.Contains("<img alt=")) {
						strtmp = strtmp2.Substring(0, strtmp2.IndexOf("</span>"));
						if (smile) { cmfl.Message += " " + strtmp;
						} else { cmfl.Message += strtmp; }
						msgend = true;
					} else {
						if (!strtmp2.StartsWith("<img alt=")) {
							if (smile) { cmfl.Message += " " + strtmp2.Substring(0, strtmp2.IndexOf("<img alt="));
							} else { cmfl.Message += strtmp2.Substring(0, strtmp2.IndexOf("<img alt=")); }
							
						}						
						
						// -- Смайлики
						strtmp = TechF.TechFuncs.SearchinText(strtmp2, "src", "", "", "\"", "\"");
						strtmp = strtmp.Substring(strtmp.IndexOf("smile/") + 6);
						strtmp = strtmp.Substring(0, strtmp.IndexOf("/size"));
						if (smile) {
							cmfl.Smiles += ";" + strtmp;
						} else { cmfl.Smiles = strtmp; smile = true; }

						strtmp = TechF.TechFuncs.SearchinText(strtmp2, "alt", "", "", "\"", "\""); // -- пока-что оставляем в сообщении код смайла
						cmfl.Message += " " + strtmp;

						strtmp2 = strtmp2.Substring(strtmp2.IndexOf(">") + 1); // - вырезаем обработанную картинку

						if (strtmp2.StartsWith("</span>")) {
							msgend = true;
						}
					}
				}

				return cmfl;
			}


			// -- Основная функция парсинга --
			private UserInfoFull UserInfo(IWebElement msgorg) {
				UserInfoFull userinfo = new UserInfoFull();
				ReadOnlyCollection<IWebElement> iwellist = null;
				string strtmp = null;

				// - Ник
				userinfo.Name = msgorg.Text.Substring(0, msgorg.Text.IndexOf(":"));

				// - Значки
				try {
					iwellist = msgorg.FindElements(By.ClassName("ChatBadge_root_IHNlp"));
					for (int i = 0; i < iwellist.Count; i++) {
						strtmp = iwellist.ElementAt(i).GetAttribute("innerHTML");
						strtmp = TechF.TechFuncs.SearchinText(strtmp, "src", "", "", "\"", "\"");
						strtmp = strtmp.Substring(strtmp.IndexOf("badge/") + 6);
						strtmp = strtmp.Substring(0, strtmp.IndexOf("/size"));
						switch (strtmp) {
							case "7447a8b2-78ef-41a1-b0ce-d4ef23ca0f39": // - Чат-бот
								if (i > 0) {
									userinfo.Badges += ";OrigBot";
								} else { userinfo.Badges = "OrigBot"; }
							break;

							case "1592e2a3-fd5f-45bf-858b-da089ee3459b": // - Модер
								if (i > 0) {
									userinfo.Badges += ";Mod";
								} else { userinfo.Badges = "Mod"; }
							break;

							case "69b9405b-81ae-40b4-abdb-2d47cff10637": // - Стример
							if (i > 0) {
								userinfo.Badges += ";Owner";
							} else { userinfo.Badges = "Owner"; }
							break;

							case "395ecb85-6a75-4217-93b8-686fbd204a8c": // - Галочник
							if (i > 0) {
								userinfo.Badges += ";Verify";
							} else { userinfo.Badges = "Verify"; }
							break;

							default: // - Предполагается, что это саб
								if (i > 0) {
									userinfo.Badges += ";Sub";
								} else { userinfo.Badges = "Sub"; }
							break;
						}
					}
				} catch (Exception ex) { }


				// - Цвет ника
				try {
					strtmp = msgorg.FindElement(By.ClassName("ChatMessage_name_RE2MY")).GetAttribute("style");
					strtmp = strtmp.Substring(strtmp.IndexOf("rgb(") + 4);
					strtmp = strtmp.Substring(0, strtmp.IndexOf(")"));
					userinfo.Color = strtmp.Replace(" ", "");
				} catch (Exception ex) { }

				return userinfo;
			}

			// -- Основная функция парсинга (HTML версия) --
			private UserInfoFull UserInfo(string msgorg) {
				UserInfoFull userinfo = new UserInfoFull();
				string msgtmp = null;
				string[] listbadges = null;
				string strtmp = null;

				// - Цвет ника
				strtmp = msgorg.Substring(msgorg.IndexOf("style=\"color: rgb(") + 19);
				userinfo.Color = strtmp.Substring(0, strtmp.IndexOf(");"));
				userinfo.Color = userinfo.Color.Replace(" ", "");

				// - Ник
				strtmp = strtmp.Substring(strtmp.IndexOf(");") + 4);
				userinfo.Name = strtmp.Substring(0, strtmp.IndexOf(":"));

				// - Значки
				if (msgorg.Contains("ChatBadge_root_IHNlp")) {
					listbadges = msgorg.Split(new string[] { "ChatBadge_root_IHNlp" }, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 1; i < listbadges.Length; i++) {
						strtmp = TechF.TechFuncs.SearchinText(listbadges[i], "src", "", "", "\"", "\"");
						strtmp = strtmp.Substring(strtmp.IndexOf("badge/") + 6);
						strtmp = strtmp.Substring(0, strtmp.IndexOf("/size"));
						switch (strtmp) {
							case "7447a8b2-78ef-41a1-b0ce-d4ef23ca0f39": // - Чат-бот
							if (i > 1) {
								userinfo.Badges += ";OrigBot";
							} else { userinfo.Badges = "OrigBot"; }
							break;

							case "1592e2a3-fd5f-45bf-858b-da089ee3459b": // - Модер
							if (i > 1) {
								userinfo.Badges += ";Mod";
							} else { userinfo.Badges = "Mod"; }
							break;

							case "69b9405b-81ae-40b4-abdb-2d47cff10637": // - Стример
							if (i > 1) {
								userinfo.Badges += ";Owner";
							} else { userinfo.Badges = "Owner"; }
							break;

							default: // - Предполагается, что это саб
							if (i > 1) {
								userinfo.Badges += ";Sub";
							} else { userinfo.Badges = "Sub"; }
							break;
						}
					}
				}

				return userinfo;
			}


			// -- Парсинг разрешений из ранее созданной строки значков --
			private PermissionsInfo UserPermissions(string badges = null) {
				PermissionsInfo userperm = new PermissionsInfo();
				
				if (badges != null && badges != "") {
					if (badges.Contains("Owner")) {
						userperm.isBrodcaster = true;
					}
					if (badges.Contains("Mod")) {
						userperm.isModerator = true;
					}
					if (badges.Contains("Sub")) {
						userperm.isSubscriber = true;
					}
				}
				
				return userperm;
			}





			// -- Буфер отправляемых сообщений --
			private void SendAwait() {
				Thread.Sleep(1000);
				while (this.Work && this.StreamWork) {
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


			// -- Отправка сообщения --
			private void Send(VKPLsendmsg Msg) {
				//Msg = Msg.Replace("\\n", "\n");
				//Msg = Msg.Replace("\\r", "\n");
				try {
					TechF.VKPL.APIBRW.BRWdriver.FindElement(By.ClassName("ChatPublisher_editor_mQThT")).Click();
					if (Msg.users != null) {
						for (int i = 0; i < Msg.users.Length; i++) {
							new Actions(TechF.VKPL.APIBRW.BRWdriver).SendKeys("@" + Msg.users[i]).Pause(TimeSpan.FromMilliseconds(100)).Perform(); // -- Печать ника
							new Actions(TechF.VKPL.APIBRW.BRWdriver).SendKeys(Keys.Tab).Perform(); // -- Превращение ника в обращение
							if (this.RutonyMode) {
								new Actions(TechF.VKPL.APIBRW.BRWdriver).SendKeys("@" + Msg.users[i] + ", ").Perform();
							}
						}
					}

					new Actions(TechF.VKPL.APIBRW.BRWdriver).SendKeys(Msg.msg).Perform(); // -- Печать сообщения
					new Actions(TechF.VKPL.APIBRW.BRWdriver).SendKeys(Keys.Enter).Pause(TimeSpan.FromMilliseconds(500)).Perform(); // -- Отправка через энтер


					if (TechF.VKPL.APIBRW.GetWarningUnMessage()) {
						new Actions(TechF.VKPL.APIBRW.BRWdriver).KeyDown(Keys.Control).SendKeys("a").KeyUp(Keys.Control).SendKeys(Keys.Backspace).Perform();
						if (Msg.users != null) {
							TechF.TechFuncs.LogDH("(VKPL) Отправка в чат через браузер не удалась - такое сообщение уже было отправлено. Сообщение: @" + string.Join(", @", Msg.users) + ", " + Msg.msg);
						} else { TechF.TechFuncs.LogDH("(VKPL)  Отправка в чат через браузер не удалась - такое сообщение уже было отправлено. Сообщение: " + Msg.msg); }
					} else {
						if (Msg.users != null) {
							TechF.TechFuncs.LogDH("(VKPL) Отправлено в чат через браузер: @" + string.Join(", @", Msg.users) + ", " + Msg.msg);
						} else { TechF.TechFuncs.LogDH("(VKPL) Отправлено в чат через браузер: " + Msg.msg); }
						long udtn = DateTimeOffset.Now.ToUnixTimeSeconds();
						//if (Msg.Contains("PRIVMSG")) {
						//Msg = Msg.Substring(Msg.IndexOf(":") + 1);
							Ev_BotMsg?.Invoke(this, new Twident_ChatMsg(2, null, Login, Login, 0, Msg.msg, udtn, null, false, botparam_Mod, botparam_VIP, botparam_Sub, null, null));
						//}
					}


				} catch (Exception ex) {
					Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка отправки сообщения", ex.Message, true));
					TechF.TechFuncs.LogDH("(VKPL) Ошибка отправки сообщения в чат через браузер: " + ex.Message + " | " + ex.InnerException.Message);
				}			
			}


			// -- Отправка команд в чат
			public void SendCom(string Command, string[] Params = null) {
				if (!TechF.HideMode && this.Work) {
					MsgListtoSent.Add(new VKPLsendmsg(null, "/" + Command));
				}
			}

			// -- Отправка обычного сообщения в чат
			public void SendMsg(string[] usr, string msg) {
				if (!TechF.HideMode && this.Work) {
					MsgListtoSent.Add(new VKPLsendmsg(usr, msg));
				}
			}
			public void SendMsg(object sender, string[] usr, Twident_Msg e) {
				if (!TechF.HideMode && this.Work) {
					MsgListtoSent.Add(new VKPLsendmsg(usr, e.Message));
				}
			}


			
			// -- Закрытие соединения --
			public void Close() {
				int mwi = 0;
				TechF.TechFuncs.LogDH("(VKPL) Остановка работы сервиса VKPL");
				Work = false;
				StreamWork = false;
				Ev_Status?.Invoke(this, new Twident_Status(2, "Остановка", null, false));
				while (this.MonitorWork) { Thread.Sleep(500); mwi++; if (mwi > 10) { this.MonitorWork = false; } } // - Костыль внутри вроде бы нормальной защиты, но установки мониторворка в фелс е срабатывает при отключённом стриме, и я не знаю, почему
				TechF.TechFuncs.LogDH("(VKPL) Закрытие браузера EDGE");
				//Send("PART #" + Channel);
				TechF.VKPL.APIBRW.CloseBRW();
				TechF.TechFuncs.LogDH("(VKPL) EDGE закрыт, ожидание завершения фоновых функций...");
				Thread.Sleep(3000);
				Ev_Status?.Invoke(this, new Twident_Status(0, "Отключено", null, null));
				Ev_ConnectStatus?.Invoke(this, new Twident_Bool(false));
			}

		}
	}
}
