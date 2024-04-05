using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
//using ControlzEx.Standard;
using System.Text.RegularExpressions;
using System.Xml.Linq;
//using System.Web.UI.WebControls.WebParts;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Windows;

namespace Twidibot
{
	public class ChatFuncs {
		//public event EventHandler<Twident_Msg> Ev_SendMsg;

		public bool SpamMessagesWork = false;
		private BackWin TechF = null;
		private string msgerrbeg = "Я ещё не умею исправлять такие ошибки: ";
		private bool isInit = false;

		public ChatFuncs(BackWin backWin) {
			//CommandsUpdate();
			TechF = backWin;
		}


		// -- Отдельная функция инициализации и запуска фоновых функций --
		public void Init() {
			if (!isInit) {
				Task.Factory.StartNew(() => TimersBackCheck());
				SpamMessagesWork = Convert.ToBoolean(Convert.ToInt32(TechF.TechFuncs.GetSettingParam("SpamMsgWork")));
				if (SpamMessagesWork) { Task.Factory.StartNew(() => SpamMessages()); }
				isInit = true;
			}	
		}



		// -- Функция спама сообщениями --
		public void SpamMessages() {
			DB.SpamMessage_tclass Spaml = null;
			int i = 0;
			long udtn = 0;
			if (TechF.TechFuncs.GetSettingParam("SpamMsg") == "0") { // -- Второй тип спама - по собственному таймеру
				while (SpamMessagesWork && (TechF.Twitch.Chat.Work || TechF.VKPL.Chat.Work)) {
					Spaml = TechF.db.SpamMessagesList.ElementAt(i);
					udtn = DateTimeOffset.Now.ToUnixTimeSeconds();
					if (Spaml.Enabled) {
						if (udtn - Spaml.LastUsed >= Spaml.CoolDown * 60) {
							TechF.TechFuncs.UniversalSendMsg(0, null, Spaml.Message);
							TechF.db.SpamMessagesT.UpdateLastUsed(Spaml.id, udtn);
						}
					}
					i++;
					if (i >= TechF.db.SpamMessagesList.Count) { i = 0; }
					Thread.Sleep(1300);
				}
			} else { // -- Нормальный тип спама - по общему таймеру, по порядку
				int wh = Convert.ToInt32(TechF.TechFuncs.GetSettingParam("SpamMsg")) * 60000;
				while (true) {
					Spaml = TechF.db.SpamMessagesList.ElementAt(i);
					if (Spaml.Enabled) {
						TechF.TechFuncs.UniversalSendMsg(0, null, Spaml.Message);
						//TechF.db.SpamMessagesT.UpdateLastUsed(Spaml.id, udtn);
						Thread.Sleep(wh);
					}
					i++;
					if (i >= TechF.db.SpamMessagesList.Count) { i = 0; }
				}
			}
		}




		// -- Базовые команды чата (непредназначены для прямого вызова из чата, только из кода других функций) --
		/// <summary>
		/// Банит пользователя в чате
		/// </summary>
		/// <param name="Nick">Ник пользователя</param>
		/// <param name="Timeout">Время бана в секундах (при значении 0 банит навсегда)</param>
		/// <param name="Reason">Причина бана</param>
		public void BanTWH(string Nick, uint Timeout = 1, string Reason = null) {
			if (Nick != null && Nick != "") {
				if (Timeout == 0) {
					if (Reason != null) {
						TechF.TechFuncs.UniversalSendCom(1, "ban " + Nick + " " + Reason);
					} else {
						TechF.TechFuncs.UniversalSendCom(1, "ban " + Nick);
					}
				} else {
					if (Reason != null) {
						TechF.TechFuncs.UniversalSendCom(1, "timeout " + Nick + " " + Timeout.ToString() + " " + Reason);
					} else {
						TechF.TechFuncs.UniversalSendCom(1, "timeout " + Nick + " " + Timeout.ToString());
					}
				}
			}
		}






		// -- Функция для установки режима чата --
		public void ChatMode(ServiceInfo ServiceInfo, UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {

			if (UserPermissions.isBrodcaster || UserPermissions.isModerator || UserInfo.Name == "Dyuha138") {
				string comname = TechF.TechFuncs.GetFuncCommand(8);
				int sleep = 0;

				switch (ServiceInfo.id) {
					case 1:
						this.ChatModeTWH(UserInfo, UserPermissions, ParamsOrig);
					break;

					case 2:
						//ChatModeVKPL(UserInfo, UserPermissions, Params);
						//TechF.TechFuncs.UniversalSendCom(2, new string[] { UserInfo.Name }, "команда недоступна");
					break;

				}

		
			} else {
				// -- Ответ в режиме расширенных ответов
				//if (TechF.)
				//TechF.TechFuncs.UniversalSendCom(new string[] { UserInfo.Name }, "у вас отсутствуют права модератора, команда невыполнена");
			}
		}

		private void ChatModeTWH(UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {
			string[] Params = ParamsOrig.ToArray();
			ParamsOrig = null;
			bool Timerc = false;
			Params[0] = Params[0].ToLower();
			
			if (UserPermissions.isBrodcaster || UserPermissions.isModerator || UserInfo.Name == "Dyuha138") {
				string comname = TechF.TechFuncs.GetFuncCommand(8);
				int sleep = 0;

				if (!TechF.Twitch.Chat.botparam_Mod) {
					TechF.TechFuncs.UniversalSendMsg(1, new string[] { UserInfo.Name }, "У меня нет прав модератора, я не умею обходить это ограничение");
					return;
				}

				switch (Params[0]) {
					/*case "NULL":
						TechF.TechFuncs.UniversalSendMsg(new string[] { UserInfo.Name }, "Вы не указали режим чата");
						return;
					break;*/

					case "h": case "help":
						TechF.TechFuncs.UniversalSendMsg(1, new string[] { UserInfo.Name }, "Параметры команды " + comname + ": \"emote\" - только смайлы; \"sub\" - только подписчики; \"follow\" [время в минутах] - только фоловеры; \"slow\" [время в секундах] - медленный режим. Также можно указать последним параметром время автоматического отключения режима (в минутах)");
					break;

					case "emote":
						if (!TechF.Twitch.Chat.channelparam_Emote) {
							if (Params[1] != "NULL") { // -- Если указано ещё время, то через это время автоматом отключится режим		
								try {
									sleep = Convert.ToInt32(Params[1]) * 60000;
								} catch (FormatException) {
									TechF.TechFuncs.UniversalSendMsg(1, new string[] { UserInfo.Name }, msgerrbeg + "вы неправильно указали формат времени отключения режима чата, нужны только цифры");
									return;
								}
								Timerc = true;
								Task.Factory.StartNew(() => {
									Thread.Sleep(sleep);
									TechF.TechFuncs.UniversalSendCom(1, "emoteonlyoff");
									TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"только смайлы\" отключён по таймеру");
								});
							}
							TechF.TechFuncs.UniversalSendCom(1, "emoteonly");
							if (Timerc) {
								TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"только смайлы\" включён на " + Params[1] + " " + TechF.TechFuncs.WordEndNumber(Params[1], "m", 1));
							} else {
								TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"только смайлы\" включён");
							}
						} else {
							TechF.TechFuncs.UniversalSendCom(1, "emoteonlyoff");
							TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"только смайлы\" отключён");
						}
					break;

					case "sub":
						if (!TechF.Twitch.Chat.channelparam_Sub) {
							if (Params[1] != "NULL") {
								try {
									sleep = Convert.ToInt32(Params[1]) * 60000;
								} catch (FormatException) {
									TechF.TechFuncs.UniversalSendMsg(1, new string[] { UserInfo.Name }, msgerrbeg + "вы неправильно указали формат времени отключения режима чата");
									return;
								}
								Timerc = true;
								Task.Factory.StartNew(() => {
									Thread.Sleep(sleep);
									TechF.TechFuncs.UniversalSendCom(1, "subscribersoff");
									TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"Только подписчики\" отключён по таймеру");
								});
							}
							TechF.TechFuncs.UniversalSendCom(1, "subscribers");
							if (Timerc) {
								TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"только подписчики\" включён на " + Params[1] + " " + TechF.TechFuncs.WordEndNumber(Params[1], "m", 1));
							} else {
								TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"только подписчики\" включён");
							}
						} else {
							TechF.TechFuncs.UniversalSendCom(1, "subscribersoff");
							TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"Только подписчики\" отключён");
						}
					break;

					case "follow":
						if (TechF.Twitch.Chat.channelparam_Follow == (-1)) {
							if (Params[1] != "NULL") { Params[1] = "0"; }
							try {
								Convert.ToInt32(Params[1]);
							} catch (FormatException) {
								TechF.TechFuncs.UniversalSendMsg(1, new string[] { UserInfo.Name },  msgerrbeg + "вы неправильно указали формат времени допуска к чату после фолоу");
								return;
							}
							if (Params[2] != "NULL") {
								try {
									sleep = Convert.ToInt32(Params.ElementAt(2)) * 60000;
								} catch (FormatException) {
									TechF.TechFuncs.UniversalSendMsg(1, new string[] { UserInfo.Name }, msgerrbeg + "вы неправильно указали формат времени отключения режима чата");
									return;
								}
								Timerc = true;
								Task.Factory.StartNew(() => {
									Thread.Sleep(sleep);
									TechF.TechFuncs.UniversalSendCom(1, "followersoff");
									TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"Только фоловеры\" отключён по таймеру"); ;
								});
							}
							TechF.TechFuncs.UniversalSendCom(1, "followers " + Params[1]);
							if (Timerc) {
								TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"Только фоловеры\" (" + Params[1] + " мин) включён на " + Params[2] + " " + TechF.TechFuncs.WordEndNumber(Params[2], "m", 1));
							} else {
								TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"Только фоловеры\" (" + Params[1] + " мин) включён");
							}
							/*} else {
								TechF.TechFuncs.UniversalSendMsg(new string[] { UserInfo.Name }, msgerrbeg + "вы не указали время допуска к чату после фолоу");
							}*/
						} else {
							TechF.TechFuncs.UniversalSendCom(1, "followersoff");
							TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"Только фоловеры\" отключён"); ;
						}
					break;

					case "slow":
						if (TechF.Twitch.Chat.channelparam_Slow == 0) {
							if (Params[1] != "NULL") { Params[1] = "5"; }
							try {
								Convert.ToInt32(Params[1]);
							} catch (FormatException) {
								TechF.TechFuncs.UniversalSendMsg(1, new string[] { UserInfo.Name }, msgerrbeg + "вы неправильно указали формат времени");
								return;
							}
							if (Params[1] != "NULL") {
								try {
									sleep = Convert.ToInt32(Params[1]) * 60000;
								} catch (FormatException) {
									TechF.TechFuncs.UniversalSendMsg(1, new string[] { UserInfo.Name }, msgerrbeg + "вы неправильно указали формат времени выключения режима чата");
									return;
								}
								Timerc = true;
								Task.Factory.StartNew(() => {
									Thread.Sleep(sleep);
									TechF.TechFuncs.UniversalSendCom(1, "slowoff");
									TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"Медленный режим\" отключён по таймеру");
								});
							}
							TechF.TechFuncs.UniversalSendCom(1, "slow " + Params[1]);
							if (Timerc) {
								TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"Медленный режим\" (" + Params[1] + " мин) включён на " + Params[2] + " " + TechF.TechFuncs.WordEndNumber(Params[2], "m", 1));
							} else {
								TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"Медленный режим\" (" + Params[1] + " мин) включён");
							}
							/*} else {
								TechF.TechFuncs.UniversalSendMsg(new string[] { UserInfo.Name }, "ошибка установки режима чата: не указано время задержки сообщений");
							}*/
						} else {
							TechF.TechFuncs.UniversalSendCom(1, "slowoff");
							TechF.TechFuncs.UniversalSendCom(1, "me Режим чата \"Медленный режим\" отключён");
						}
					break;
				}
			} else {
				// -- Ответ в режиме расширенных ответов
				//if (TechF.)
				//TechF.TechFuncs.UniversalSendCom(new string[] { UserInfo.Name }, "у вас отсутствуют права модератора, команда невыполнена");
			}
		}






		// -- Пирамидка --
		public void Pyramid(ServiceInfo ServiceInfo, UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {
			int MaxCount = Convert.ToInt32(ParamsOrig.ElementAt(1));
			int i = 0;
			string str = "";
			while (i < MaxCount * 2 - 1) {
				if (i < MaxCount) {
					str += ParamsOrig.ElementAt(0) + " ";
				} else {
					int i2 = 0;
					int count2 = (MaxCount * 2) - i - 1;
					str = "";
					while (i2 < count2) {
						str += ParamsOrig.ElementAt(0) + " ";
						i2++;
					}
				}
				i++;
				TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, str);
			}
		}

		// -- Реверсивная пирамидка --
		public void RevPyramid(ServiceInfo ServiceInfo, UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {
			int MaxCount = Convert.ToInt32(ParamsOrig.ElementAt(1));
			int i = 0;
			string str = "";
			while (i < MaxCount * 2 - 1) {
				if (i < MaxCount) {
					int i2 = 0;
					int count2 = MaxCount - i;
					str = "";
					while (i2 < count2) {
						str += ParamsOrig.ElementAt(0) + " ";
						i2++;
					}
				} else {
					str += ParamsOrig.ElementAt(0) + " ";
				}
				i++;
				TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, str);
			}
		}





		// -- Таймер --
		public void Timers(ServiceInfo ServiceInfo, UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {
			string comname = TechF.TechFuncs.GetFuncCommand(3);
			string[] Params = ParamsOrig.ToArray();
			ParamsOrig = null;
			DB.Funcs_Timer_tclass Timerl = null;
			bool sqlcheck = false;
			long udtn = DateTimeOffset.Now.ToUnixTimeSeconds();
			long udtm = 253402300799 - udtn - 10; // Оставшееся unix-время
			Params[0] = Params[0].ToLower();
			if (Params[1] != "NULL") { Params[1] = Params[1].ToLower(); }

			switch (Params[0]) {

				/*case "NULL":
					TechF.TechFuncs.UniversalSendMsg(new string[] { UserInfo.Name }, "Вы не ввели название таймера (параметр help покажет помощь)");
				break;*/

				case "h": case "help":
					TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Параметры команды \"" + comname + "\": Проверить - \"[имя таймера]\"; Создать - \"[имя] [время с припиской \"h\" для часов, \"m\" для минут, или \"s\" для секунд (по-умолчанию минуты), и не более " + udtm + " " + TechF.TechFuncs.WordEndNumber(udtm, "s", 2, true) + "] [режим оповещения: 0 - без обращений; 1 - обращение только к создателю; 2 - только к стримеру; 3 - и к создателю и к стримеру]\"; Пауза - \"[имя] p\"; Возобновить - \"[имя] s\"; Сброс - \"[имя] r\"; Удалить - \"[имя] d\"; Вывести все существующие таймеры - \"all\"");
				break;

				case "all":
					StringBuilder strtmp = new StringBuilder(TechF.db.TimersList.Count * 5);
					for (int i = 0; i < TechF.db.TimersList.Count; i++) {
						Timerl = TechF.db.TimersList.ElementAt(i);
						strtmp.Append("\"" + Timerl.Name + "\"; ");
					}
					if (TechF.db.TimersList.Count > 0) {
						TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Список существующих таймеров: " + strtmp.ToString());
					} else {
						TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но таймеры отсутствуют");
					}
				break;

				default:
					switch (Params[1]) {

						case "NULL": // - Проверка
							Timerl = TechF.db.TimersList.Find(x => x.Name == Params[0]);

							if (Timerl != null) {
								long udtpause = 0;
								// -- Высчитываем общее время пауз
								if (Timerl.DTPauseResume != null) {
									string[] udtprms = Timerl.DTPauseResume.Split(';');
									long udtprp = 0;
									long udtprs = 0;
									udtprms = Timerl.DTPauseResume.Split(';');
									for (int ip = 0; ip < udtprms.Length; ip++) {
										if (udtprms[ip].Contains(",")) {
											udtprp = Convert.ToInt64(udtprms[ip].Substring(0, udtprms[ip].IndexOf(",")));
											udtprs = Convert.ToInt64(udtprms[ip].Substring(udtprms[ip].IndexOf(",") + 1));
										} else {
											udtprp = Convert.ToInt64(udtprms[ip]);
											udtprs = udtn;
										}
										udtpause += udtprs - udtprp;
									}
								}
								// -- Вычисление, сколько осталось: время запуска - (время сейчас - время таймера) + общее время пауз
								long udtres = Timerl.DTStart - (udtn - Timerl.Time) + udtpause;
								DateTime dtres = new DateTime();
								dtres = dtres.AddSeconds(udtres);
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Значение таймера \"" + Params[0] + "\": " + TechF.TechFuncs.DateTimeString(dtres));
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не нашёл таймер \"" + Params[0] + "\"");
							}
						break;

						case "p": case "pause": // Пауза
							sqlcheck = TechF.db.Funcs_TimersT.PauseUpdate(Params[0], true, DateTimeOffset.Now.ToUnixTimeSeconds());
							if (sqlcheck) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Остановил таймер \"" + Params[0] + "\"");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Таймер \"" + Params[0] + "\" и так остановлен, или я его не нашёл");
							}
						break;

						case "s": case "start": // Возобновление
							sqlcheck = TechF.db.Funcs_TimersT.PauseUpdate(Params[0], false, DateTimeOffset.Now.ToUnixTimeSeconds());
							if (sqlcheck) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Возобновил таймер \"" + Params[0] + "\"");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Таймер \"" + Params[0] + "\" и так работает, или я его не нашёл");
							}
						break;

						case "r": case "res": case "reset": // Обнуление
							sqlcheck = TechF.db.Funcs_TimersT.Restart(Params[0], DateTimeOffset.Now.ToUnixTimeSeconds());
							if (sqlcheck) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Обнулил таймер \"" + Params[0] + "\"");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Я не нашёл таймер \"" + Params[0] + "\"");
							}
						break;

						case "d": case "del": case "delete": // Удаление
							sqlcheck = TechF.db.Funcs_TimersT.Remove(Params[0]);
							if (sqlcheck) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Удалил таймер \"" + Params[0] + "\"");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не нашёл таймер \"" + Params[0] + "\"");
							}
						break;


						default:
							long Time = 0;
							string Type = null, Timestr = null, Timestrout = null;

							if (Params[1].StartsWith("+") || Params[1].StartsWith("-")) {
								bool isAdd = false;
								if (Params[1].StartsWith("+")) { isAdd = true; } else { if (Params[1].StartsWith("-")) { isAdd = false; } }

								Params[1] = Params[1].Substring(1);
								Timerl = TechF.db.TimersList.Find(x => x.Name == Params[0]);
								if (Timerl != null) {
									if (Params[1].EndsWith("h") || Params[1].EndsWith("m") || Params[1].EndsWith("s")) {
										Type = Params[1].Substring(Params[1].Length - 1);
										Timestr = Params[1].Substring(0, Params[1].Length - 1);
									} else { Timestr = Params[1]; }
									try {
										Time = Convert.ToInt64(Timestr);
									} catch (FormatException) {
										TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не смог понять, что вы хотите от меня. Чтобы получить помощь, введите \"" + comname + " help\"");
										break;
									} catch (OverflowException) {
										TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "вы указали слишком большие цифры");
										break;
									}
									if (Type == null) { Type = "m"; }

									try {
										if (Type == "h") {
											Timestrout = Time.ToString() + " " + TechF.TechFuncs.WordEndNumber(Time, "h");
											Time *= 3600;
										} else {
											if (Type == "m") {
												Timestrout = Time.ToString() + " " + TechF.TechFuncs.WordEndNumber(Time, "m");
												Time *= 60;
											} else {
												if (Type == "s") {
													Timestrout = Time.ToString() + " " + TechF.TechFuncs.WordEndNumber(Time, "s");

												} else {
													TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "вы неправильно указали формат времени, приписки к цифрам допускаются только такие: s, m, h");
													break;
												}
											}
										}
									} catch (Exception) {
										TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "вы указали слишком большие цифры");
										break;
									}

									if (isAdd) { Time += Timerl.Time; } else { Time = Timerl.Time - Time; }
									if (Time > udtm) {
										TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "вы указали слишком большие цифры");
										//TechF.TechFuncs.UniversalSendMsg(new string[] { UserInfo.Name }, "Указано слишком большое число (" + udtm + " " + TechF.TechFuncs.WordEndNumber(udtm, "s", 2, true) + "), таймер НЕ запущен");
										break;
									} else {
										if (Time < 1) {
											TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "итоговое время оказалось отрицательным или нулевым, укажите цифры по-меньше");
											break;
										} else {
											TechF.db.Funcs_TimersT.UpdateTime(Params[0], Time);
											if (isAdd) {
												TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Увеличил таймер \"" + Params[0] + "\" на " + Timestrout);
											} else {
												TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Уменьшил таймер \"" + Params[0] + "\" на " + Timestrout);
											}
										}
									}
								} else {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не нашёл таймер \"" + Params[0] + "\"");
								}

							} else {
								if (Params[1] == "0") {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но указывать ноль нельзя");
									break;
								}
								if (Params[1].EndsWith("h") || Params[1].EndsWith("m") || Params[1].EndsWith("s")) {
									Type = Params[1].Substring(Params[1].Length - 1);
									Timestr = Params[1].Substring(0, Params[1].Length - 1);
								} else { Timestr = Params[1]; }
								try {
									Time = Convert.ToInt64(Timestr);
								} catch (FormatException) {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не смог понять, что вы хотите от меня. Чтобы получить помощь, введите \"" + comname + " help\"");
									break;
								} catch (OverflowException) {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "вы указали слишком большие цифры");
									break;
								}
								if (Type == null) { Type = "m"; }

								try {
									if (Type == "h") {
										Timestrout = Time.ToString() + " " + TechF.TechFuncs.WordEndNumber(Time, "h");
										Time *= 3600;
									} else {
										if (Type == "m") {
											Timestrout = Time.ToString() + " " + TechF.TechFuncs.WordEndNumber(Time, "m");
											Time *= 60;
										} else {
											if (Type == "s") {
												Timestrout = Time.ToString() + " " + TechF.TechFuncs.WordEndNumber(Time, "s");
												} else {
												TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "вы неправильно указали формат времени, приписки к цифрам допускаются только такие: s, m, h");
												break;
											}
										}
									}
								} catch (Exception) {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "вы указали слишком большие цифры");
									break;
								}

								if (Time > udtm) {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "вы указали слишком большие цифры");
									//TechF.TechFuncs.UniversalSendMsg(new string[] { UserInfo.Name }, "Указано слишком большое число (" + udtm + " " + TechF.TechFuncs.WordEndNumber(udtm, "s", 2, true) + "), таймер НЕ запущен");
									break;
								}
								Timerl = TechF.db.TimersList.Find(x => x.Name == Params[0]);
								if (Timerl == null) {
									try {
										Convert.ToInt16(Params[2]);
									} catch (Exception) {
										//TechF.TechFuncs.UniversalSendMsg(new string[] { UserInfo.Name }, "Ошибка создания таймера \"" + Name + "\" - неверный формат третьего параметра (уровень уведомления об окончании) - принимаются только цифры от 0 до 3 включительно");
										//break;
										Params[2] = "1";
									}
									if (Convert.ToInt16(Params[2]) > 3) { Params[2] = "3"; }
									if (Convert.ToInt16(Params[2]) < 0) { Params[2] = "0"; }
									try {
										Convert.ToInt16(Params[3]);
									} catch {
										Params[3] = ServiceInfo.id.ToString();
									}
									TechF.db.Funcs_TimersT.Add(new DB.Funcs_Timer_tclass(Params[0], Time, UserInfo.Name, udtn, null, Convert.ToInt16(Params[2]), false, Convert.ToInt16(Params[3])));

									if (Type == "s") { TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Таймер \"" + Params[0] + "\" на " + Time + " " + TechF.TechFuncs.WordEndNumber(Time, "s", 3) + " запустил!"); }
									if (Type == "m") { TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Таймер \"" + Params[0] + "\" на " + Time / 60 + " " + TechF.TechFuncs.WordEndNumber(Time / 60, "m", 3) + " запустил!"); }
									if (Type == "h") { TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Таймер \"" + Params[0] + "\" на " + Time / 3600 + " " + TechF.TechFuncs.WordEndNumber(Time / 3600, "h", 3) + " запустил!"); }
								} else {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но таймер \"" + Params[0] + "\" уже существует");
								}
							}
						break;
					}
				break;
			}
		}



		// -- Фоновая проверка таймеров --
		private void TimersBackCheck() {
			DB.Funcs_Timer_tclass Timerl = new DB.Funcs_Timer_tclass();
			long udtn = 0;
			long udtpause = 0;
			long udtprp = 0;
			long udtprs = 0;
			string[] udtprms = null;

			while (TechF.Twitch.Chat.Work || TechF.VKPL.Chat.Work) {
				if (TechF.db.TimersList.Count > 0) {
					for (int i = 0; i < TechF.db.TimersList.Count; i++) {
						Timerl = TechF.db.TimersList.ElementAt(i);
						if (!Timerl.Paused) {
							udtn = DateTimeOffset.Now.ToUnixTimeSeconds();

							// -- Высчитываем общее время пауз
							if (Timerl.DTPauseResume != null) {
								udtprms = Timerl.DTPauseResume.Split(';');
								for (int ip = 0; ip < udtprms.Length; ip++) {
									if (udtprms[ip].Contains(",")) {
										udtprp = Convert.ToInt64(udtprms[ip].Substring(0, udtprms[ip].IndexOf(",")));
										udtprs = Convert.ToInt64(udtprms[ip].Substring(udtprms[ip].IndexOf(",") + 1));
									} else {
										udtprp = Convert.ToInt64(udtprms[ip]);
										udtprs = udtn;
									}
									udtpause += udtprs - udtprp;
								}
							}

							// -- Вычисление, сколько осталось: время запуска - (время сейчас - время таймера) + общее время пауз
							long udtres = Timerl.DTStart - (udtn - Timerl.Time) + udtpause;
							if (udtres < 1) {
								if (Timerl.Notiflvl == 0) { TechF.TechFuncs.UniversalSendMsg(Timerl.Service, null, "Таймер \"" + Timerl.Name + "\" закончился!"); }
								if (Timerl.Notiflvl == 1) { TechF.TechFuncs.UniversalSendMsg(Timerl.Service, new string[] { Timerl.Nick }, "Таймер \"" + Timerl.Name + "\" закончился!"); }
								if (Timerl.Notiflvl == 2) { TechF.TechFuncs.UniversalSendMsg(Timerl.Service, new string[] { TechF.TechFuncs.GetSettingTWHParam("ChannelDisp") }, "Таймер \"" + Timerl.Name + "\" закончился!"); }		
								if (Timerl.Notiflvl == 3) { TechF.TechFuncs.UniversalSendMsg(Timerl.Service, new string[] { Timerl.Nick, TechF.TechFuncs.GetSettingTWHParam("ChannelDisp") }, "Таймер \"" + Timerl.Name + "\" закончился!"); }
								TechF.db.Funcs_TimersT.Remove(Timerl.Name);
							}
							//TechF.db.TimersList.RemoveAt(Convert.ToInt32(i));
						}
					}
				} else {
					Thread.Sleep(3000);
				}
				Thread.Sleep(500);
			}
		}




		// -- Секундомер --
		public void Stopwatches(ServiceInfo ServiceInfo, UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {
			string comname = TechF.TechFuncs.GetFuncCommand(5);
			string[] Params = ParamsOrig.ToArray();
			ParamsOrig = null;
			DB.Funcs_Stopwatch_tclass Stopwatchl = null;
			bool sqlcheck = false;
			Params[0] = Params[0].ToLower();
			if (Params[1] != "NULL") { Params[1] = Params[1].ToLower(); }

			switch (Params[0]) {

				/*case "NULL":
					TechF.TechFuncs.UniversalSendMsg(new string[] { UserInfo.Name }, "Не введенно название секундомера (параметр help покажет помощь)");
				break;*/

				case "h": case "help":
					TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Параметры команды \"" + comname + "\": Проверить - \"[имя секундомера]\"; Создать - \"[имя] a\"; Пауза - \"[имя] p\"; Возобновление - \"[имя] s\"; Сброс - \"[имя] r\"; Удаление - \"[имя] d\"; Вывести все существующие секундомеры - \"all\"");
				break;

				case "all":
					StringBuilder strtmp = new StringBuilder(TechF.db.StopwatchesList.Count * 5);
					for (int i = 0; i < TechF.db.StopwatchesList.Count; i++) {
						Stopwatchl = TechF.db.StopwatchesList.ElementAt(i);
						strtmp.Append("\"" + Stopwatchl.Name + "\"; ");
					}
					if (TechF.db.StopwatchesList.Count > 0) {
						TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Список существующих секундомеров: " + strtmp.ToString());
					} else {
						TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но секундомеры отсутствуют");
					}
				break;

				default:
					switch (Params[1]) {

						case "NULL": // - Проверка
							Stopwatchl = TechF.db.StopwatchesList.Find(x => x.Name == Params[0]);

							if (Stopwatchl != null) {
								long udtn = DateTimeOffset.Now.ToUnixTimeSeconds();
								long udtpause = 0;
								// -- Высчитываем общее время пауз
								if (Stopwatchl.DTPauseResume != null) {
									string[] udtprms = Stopwatchl.DTPauseResume.Split(';');
									long udtprp = 0;
									long udtprs = 0;
									for (int ip = 0; ip < udtprms.Length; ip++) {
										if (udtprms[ip].StartsWith("+")) {
											udtpause -= Convert.ToInt64(udtprms[ip].Substring(1));
										} else {
											if (udtprms[ip].StartsWith("-")) {
												udtpause += Convert.ToInt64(udtprms[ip].Substring(1));
											} else {
												if (udtprms[ip].Contains(",")) {
													udtprp = Convert.ToInt64(udtprms[ip].Substring(0, udtprms[ip].IndexOf(",")));
													udtprs = Convert.ToInt64(udtprms[ip].Substring(udtprms[ip].IndexOf(",") + 1));
												} else {
													udtprp = Convert.ToInt64(udtprms[ip]);
													udtprs = udtn;
												}
												udtpause += udtprs - udtprp;
											}
										}
									}
								}

								// -- Вычисление, сколько прошло: (время сейчас - время запуска) - общее время пауз
								long udtres = (udtn - Stopwatchl.DTStart) - udtpause;
								DateTime dtres = new DateTime();
								dtres = dtres.AddSeconds(udtres);
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Значение секундомера \"" + Params[0] + "\": " + TechF.TechFuncs.DateTimeString(dtres));
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не нашёл секундомер \"" + Params[0] + "\"");
							}
						break;

						case "a": case "add": // Добавление
							Stopwatchl = TechF.db.StopwatchesList.Find(x => x.Name == Params[0]);
							if (Stopwatchl == null) {
								TechF.db.Funcs_StopwatchesT.Add(new DB.Funcs_Stopwatch_tclass(Params[0], UserInfo.Name, DateTimeOffset.Now.ToUnixTimeSeconds()));
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Создал и запустил секундомер \"" + Params[0] + "\"");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но секундомер \"" + Params[0] + "\" уже существует");
							}
						break;

						case "p": case "pause": // Пауза
							sqlcheck = TechF.db.Funcs_StopwatchesT.PauseUpdate(Params[0], true, DateTimeOffset.Now.ToUnixTimeSeconds());
							if (sqlcheck) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Остановил секундомер \"" + Params[0] + "\"");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но секундомер \"" + Params[0] + "\" и так работает, или я его не нашёл");
							}
						break;

						case "s": case "start": // Возобновление
							sqlcheck = TechF.db.Funcs_StopwatchesT.PauseUpdate(Params[0], false, DateTimeOffset.Now.ToUnixTimeSeconds());
							if (sqlcheck) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Возобновил секундомер \"" + Params[0] + "\"");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но секундомер \"" + Params[0] + "\" и так работает, или я его не нашёл");
							}
						break;

						case "r": case "res": case "reset": // Сброс
							sqlcheck = TechF.db.Funcs_StopwatchesT.Restart(Params[0], DateTimeOffset.Now.ToUnixTimeSeconds());
							if (sqlcheck) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Обнулил секундомер \"" + Params[0] + "\"");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не нашёл секундомер \"" + Params[0] + "\"");
							}
						break;

						case "d": case "del": case "delete": // Удаление
							sqlcheck = TechF.db.Funcs_StopwatchesT.Remove(Params[0]);
							if (sqlcheck) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Удалил секундомер \"" + Params[0] + "\"");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не нашёл секундомер \"" + Params[0] + "\"");
							}
						break;


						default:
							long Time = 0;
							string Type = null, Timestr = null, Timestrout = null;

							if (Params[1].StartsWith("+") || Params[1].StartsWith("-")) {
								bool isAdd = false;
								if (Params[1].StartsWith("+")) { isAdd = true; } else { if (Params[1].StartsWith("-")) { isAdd = false; } }

								Params[1] = Params[1].Substring(1);
								Stopwatchl = TechF.db.StopwatchesList.Find(x => x.Name == Params[0]);
								if (Stopwatchl != null) {
									if (Params[1].EndsWith("h") || Params[1].EndsWith("m") || Params[1].EndsWith("s")) {
										Type = Params[1].Substring(Params[1].Length - 1);
										Timestr = Params[1].Substring(0, Params[1].Length - 1);
									} else { Timestr = Params[1]; }
									try {
										Time = Convert.ToInt64(Timestr);
									} catch (FormatException) {
										TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не смог понять, что вы хотите от меня. Чтобы получить помощь, введите \"" + comname + " help\"");
										break;
									} catch (OverflowException) {
										TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "вы указали слишком большие цифры");
										break;
									}
									if (Type == null) { Type = "m"; }

									try {
										if (Type == "h") {
											Timestrout = Time.ToString() + " " + TechF.TechFuncs.WordEndNumber(Time, "h");
											Time *= 3600;
										} else {
											if (Type == "m") {
												Timestrout = Time.ToString() + " " + TechF.TechFuncs.WordEndNumber(Time, "m");
												Time *= 60;
											} else {
												if (Type == "s") {
													Timestrout = Time.ToString() + " " + TechF.TechFuncs.WordEndNumber(Time, "s");

												} else {
													TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "вы неправильно указали формат времени, приписки к цифрам допускаются только такие: s, m, h");
													break;
												}
											}
										}
									} catch (Exception) {
										TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "вы указали слишком большие цифры");
										break;
									}

									if (Time > 253402300799) {
										TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "итоговое время оказалось слишком большим, укажите цифры по-меньше");
										break;
									} else {
										if (Time < 1) {
											TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, msgerrbeg + "итоговое время оказалось отрицательным или нулевым, укажите цифры по-меньше");
											break;
										} else {
											TechF.db.Funcs_StopwatchesT.UpdateTime(Params[0], Time, isAdd);
											if (isAdd) {
												TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Увеличил секундомер \"" + Params[0] + "\" на " + Timestrout);
											} else {
												TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Уменьшил секундомер \"" + Params[0] + "\" на " + Timestrout);
											}
										}
									}
								} else {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не нашёл секундомер \"" + Params[0] + "\"");
								}
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не смог понять, что вы хотите от меня. Чтобы получить помощь, введите \"" + comname + " help\"");
							}
						break;
					}
				break;
			}
		}



		// -- Счётчик --
		public void Counter(ServiceInfo ServiceInfo, UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {
			string comname = TechF.TechFuncs.GetFuncCommand(7);
			string[] Params = ParamsOrig.ToArray();
			ParamsOrig = null;
			DB.Funcs_Counter_tclass Counterl = new DB.Funcs_Counter_tclass();
			bool sqlcheck = false;
			Params[0] = Params[0].ToLower();
			if (Params[1] != "NULL") { Params[1] = Params[1].ToLower(); }

			switch (Params[0]) {

				/*case "NULL":
					TechF.TechFuncs.UniversalSendMsg(new string[] { UserInfo.Name }, "Вы не ввели название счётчика (параметр help покажет помощь)");
				break;*/

				case "h": case "help":
					TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Параметры команды \"" + comname + "\": Проверить - \"[имя счётчика]\"; Создать - \"[имя] a\"; Установить значение - \"[имя] [значение]\"; Прибавить 1 - \"[имя] +\"; Отнять 1 - \"[имя] -\"; Сброс - \"[имя] r\"; Удаление - \"[имя] d\"; Вывести все существующие счётчики - \"all\"");
				break;

				case "all":
					StringBuilder strtmp = new StringBuilder(TechF.db.StopwatchesList.Count * 5);
					for (int i = 0; i < TechF.db.CountersList.Count; i++) {
						Counterl = TechF.db.CountersList.ElementAt(i);
						strtmp.Append("\"" + Counterl.Name + "\" - " + Counterl.Value + "; ");
					}
					if (TechF.db.CountersList.Count > 0) {
						TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Список существующих счётчиков: " + strtmp.ToString());
					} else {
						TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но счётчики отсутствуют");
					}
				break;

				default:
					switch (Params[1]) {

						case "NULL":
							Counterl = TechF.db.CountersList.Find(x => x.Name == Params[0]);
							if (Counterl != null) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Значение счётчика \"" + Params[0] + "\": " + Counterl.Value);
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не нашёл счётчик \"" + Params[0] + "\"");
							}
						break;

						case "a": case "add": // Добавление
							Counterl = TechF.db.CountersList.Find(x => x.Name == Params[0]);
							if (Counterl == null) {
								TechF.db.Funcs_CountersT.Add(new DB.Funcs_Counter_tclass(Params[0]));
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Создал счётчик \"" + Params[0] + "\"");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но счётчик \"" + Params[0] + "\" уже существует");
							}
						break;

						case "+": // Прибавление
							sqlcheck = TechF.db.Funcs_CountersT.Plus(Params[0]);
							if (sqlcheck) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Увеличил счётчик \"" + Params[0] + "\", текущее значение - " + TechF.db.CountersList.Find(x => x.Name == Params[0]).Value);
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не нашёл счётчик \"" + Params[0] + "\"");
							}
						break;

						case "-": // Убавление
							sqlcheck = TechF.db.Funcs_CountersT.Minus(Params[0]);
							if (sqlcheck) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Уменьшил счётчик \"" + Params[0] + "\", текущее значение - " + TechF.db.CountersList.Find(x => x.Name == Params[0]).Value);
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не нашёл счётчик \"" + Params[0] + "\"");
							}
						break;

						case "r": case "res": case "restart": // Обнуление
							sqlcheck = TechF.db.Funcs_CountersT.Restart(Params[0]);
							if (sqlcheck) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Обнулил счётчик \"" + Params[0] + "\"");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, " Извините, но я не нашёл счётчик \"" + Params[0] + "\"");
							}
						break;

						case "d": case "del": case "delete": // Удаление
							sqlcheck = TechF.db.Funcs_CountersT.Remove(Params[0]);
							if (sqlcheck) {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Удалил счётчик \"" + Params[0] + "\"");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, " Извините, но я не нашёл счётчик \"" + Params[0] + "\"");
							}
						break;


						default:
							int val = 0;
							string vals = null;
							bool isPlus = false;
							if (Params[1].StartsWith("+") || Params[1].StartsWith("-")) {
								vals = Params[1].Substring(1);

								try {
									val = Convert.ToInt32(vals);
								} catch (FormatException) {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не смог понять, что вы хотите от меня. Чтобы получить помощь, введите \"" + comname + " help\"");
									break;
								}

								if (Params[1].StartsWith("+")) {
									isPlus = true;
									sqlcheck = TechF.db.Funcs_CountersT.Plus(Params[0], val);
								} else {
									isPlus = false;
									sqlcheck = TechF.db.Funcs_CountersT.Minus(Params[0], val);
								}

								if (sqlcheck) {
									if (isPlus) {
										TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Увеличил счётчик \"" + Params[0] + "\" на " + val + ", текущее значение - " + TechF.db.CountersList.Find(x => x.Name == Params[0]).Value);
									} else {
										TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Уменьшил счётчик \"" + Params[0] + "\" на " + val + ", текущее значение - " + TechF.db.CountersList.Find(x => x.Name == Params[0]).Value);
									}
								} else {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не нашёл счётчик \"" + Params[0] + "\"");
								}

							} else {
								try {
									val = Convert.ToInt32(Params[1]);
								} catch (FormatException) {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не смог понять, что вы хотите от меня. Чтобы получить помощь, введите \"" + comname + " help\"");
									break;
								}

								sqlcheck = TechF.db.Funcs_CountersT.Update(Params[0], val);
								if (sqlcheck) {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Установил значение счётчика \"" + Params[0] + "\" на " + TechF.db.CountersList.Find(x => x.Name == Params[0]).Value);
								} else {
									TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но я не нашёл счётчик \"" + Params[0] + "\"");
								}
							}
						break;
					}
				break;
			}
		}




		// -- Функция, которая крафтит строку на подобие фразы Лебедева "ну умер и умер" --
		public void NuLebedev(ServiceInfo ServiceInfo, UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {
			string comname = TechF.TechFuncs.GetFuncCommand(10);
			string[] Params = ParamsOrig.ToArray();
			ParamsOrig = null;
			int setparc = 0;			
			StringBuilder sb = new StringBuilder(Params.Length * 6);
			string wordsstr = null;

			switch (Params[0]) {

				/*case "NULL":
					TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "отсутствуют параметры (параметр help покажет помощь)");
				break;*/

				case "h": case "help":
					TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Параметры функции " + comname + ": [слова через пробелы] [тип окончания в виде цифры (от 1 до 5)] [любой символ, чтобы вывести вторую часть (после \"и\") только с первым словом]. Для отображения всех окончаний наберите первым параметром одно из этих слов: t, type, types, var, variant, variants, варианты");
				break;

				case "t": case "type": case "types": case "var": case "variant": case "variants": case "варики": case "варианты":
					TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Варианты окончания фукнции " + comname + ": 0 - без окончания; 1 - \"бухтеть\"; 2 - \"бубнеть\"; 3 - \"бубонить\"; 4 - \"пердеть\"; 5 - \"хуёнить\"");
				break;


				default:
					if (Params[Params.Length - 2] == "0" || Params[Params.Length - 2] == "NULL" || Params[Params.Length - 2] == "1" || Params[Params.Length - 2] == "2" || Params[Params.Length - 2] == "3" || Params[Params.Length - 2] == "4" || Params[Params.Length - 2] == "5") {
						setparc = 2;
					} else {
						if (Params[Params.Length - 1] == "0" || Params[Params.Length - 1] == "NULL" || Params[Params.Length - 1] == "1" || Params[Params.Length - 1] == "2" || Params[Params.Length - 1] == "3" || Params[Params.Length - 1] == "4" || Params[Params.Length - 1] == "5") {
							setparc = 1;
						}
					}

					for (int i = 0; i < Params.Length - setparc; i++) {
						if (i == 0) { sb.Append(Params[i]);
						} else { sb.Append(" " + Params[i]); }	
					}
					wordsstr = sb.ToString();
					sb.Clear();
					
					switch (Params[Params.Length - setparc]) {
						case "0": case "NULL":
							if (setparc == 2 && Params[Params.Length - 1] != "NULL") {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, "Ну " + wordsstr + " и " + wordsstr.Substring(0, wordsstr.IndexOf(" ")));
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, "Ну " + wordsstr + " и " + wordsstr);
							}
						break;
						case "1": default:
							if (setparc == 2 && Params[Params.Length - 1] != "NULL") {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, "Ну " + wordsstr + " и " + wordsstr.Substring(0, wordsstr.IndexOf(" ")) + ", чё бухтеть то");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, "Ну " + wordsstr + " и " + wordsstr + ", чё бухтеть то");
							}
						break;
						case "2":
							if (setparc == 2 && Params[Params.Length - 1] != "NULL") {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, "Ну " + wordsstr + " и " + wordsstr.Substring(0, wordsstr.IndexOf(" ")) + ", чё бубнеть то");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, "Ну " + wordsstr + " и " + wordsstr + ", чё бубнеть то");
							}
						break;
						case "3":
							if (setparc == 2 && Params[Params.Length - 1] != "NULL") {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, "Ну " + wordsstr + " и " + wordsstr.Substring(0, wordsstr.IndexOf(" ")) + ", чё бубонить то");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, "Ну " + wordsstr + " и " + wordsstr + ", чё бубонить то");
							}
						break;
						case "4":
							if (setparc == 2 && Params[Params.Length - 1] != "NULL") {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, "Ну " + wordsstr + " и " + wordsstr.Substring(0, wordsstr.IndexOf(" ")) + ", чё пердеть то");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, "Ну " + wordsstr + " и " + wordsstr + ", чё пердеть то");
							}
						break;
						case "5":
							if (setparc == 2 && Params[Params.Length - 1] != "NULL") {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, "Ну " + wordsstr + " и " + wordsstr.Substring(0, wordsstr.IndexOf(" ")) + ", чё хуёнить то");
							} else {
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, "Ну " + wordsstr + " и " + wordsstr + ", чё хуёнить то");
							}
					break;
				}
				break;
			}
		}




		// -- Вывод оставшегося времени до конца unix-времени - 31 декабря 9999 года --
		public void StayUnixTime(ServiceInfo ServiceInfo, UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {
			string comname = TechF.TechFuncs.GetFuncCommand(11);
			string[] Params = ParamsOrig.ToArray();
			ParamsOrig = null;
			Params[0] = Params[0].ToLower();

			if (Params[0] == "h" || Params[0] == "help") {
				//TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Параметры функции " + comname + ": [смещение по UTC (+3, -3)]");
				TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Параметров функции " + comname + " пока что нет");
				return;
			}

			long udtn = DateTimeOffset.Now.ToUnixTimeSeconds();
			long ustaytime = 253402300799 - udtn - 10;

			DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(ustaytime);
			//if (TimeZonel != "NULL") {

			//} else {
			//	DateTime dtres = dto.DateTime
			//}

			StringBuilder strout = new StringBuilder("Оставшееся время до конца unix: ", 200);
			bool addcheck = false;

			strout.Append(ustaytime + " " + TechF.TechFuncs.WordEndNumber(ustaytime, "s") + "; ");

			if (dto.Year - 1 > 0) {
				strout.Append((dto.Year - 1) + " " + TechF.TechFuncs.WordEndNumber(dto.Year - 1, "Y"));
				addcheck = true;
			}
			if (dto.Month - 1 > 0) {
				if (addcheck) {
					strout.Append(", " + (dto.Month - 1) + " " + TechF.TechFuncs.WordEndNumber(dto.Month - 1, "M"));
				} else {
					strout.Append((dto.Month - 1) + " " + TechF.TechFuncs.WordEndNumber(dto.Month - 1, "M"));
					addcheck = true;
				}
			}
			if (dto.Day - 1 > 0) {
				if (addcheck) {
					strout.Append(", " + (dto.Day - 1) + " " + TechF.TechFuncs.WordEndNumber(dto.Day - 1, "D"));
				} else {
					strout.Append((dto.Day - 1) + " " + TechF.TechFuncs.WordEndNumber(dto.Day - 1, "D"));
					addcheck = true;
				}
			}
			if (dto.Hour > 0) {
				if (addcheck) {
					strout.Append(", " + (dto.Hour) + " " + TechF.TechFuncs.WordEndNumber(dto.Hour, "h"));
				} else {
					strout.Append(dto.Hour + " " + TechF.TechFuncs.WordEndNumber(dto.Hour, "h"));
					addcheck = true;
				}
			}
			if (dto.Minute > 0) {
				if (addcheck) {
					strout.Append(", " + dto.Minute + " " + TechF.TechFuncs.WordEndNumber(dto.Minute, "m"));
				} else {
					strout.Append(dto.Minute + " " + TechF.TechFuncs.WordEndNumber(dto.Minute, "m"));
					addcheck = true;
				}
			}
			if (dto.Second > 0) {
				if (addcheck) {
					strout.Append(", " + dto.Second + " " + TechF.TechFuncs.WordEndNumber(dto.Second, "s"));
				} else {
					strout.Append(dto.Second + " " + TechF.TechFuncs.WordEndNumber(dto.Second, "s"));
				}
			}

			strout.Append(". Максимальное значение unix-времени - 253402300799");

			TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, strout.ToString());
		}




		// -- Просто возвращает принятые слова (максимум 50) --
		public void Echo(ServiceInfo ServiceInfo, UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {
			//string comname = TechF.TechFuncs.GetFuncCommand(12);
			string[] Params = ParamsOrig.ToArray();
			ParamsOrig = null;

			if (Params[0] != "NULL") {
				StringBuilder sb = new StringBuilder(Params.Length * 6);

				if ((UserPermissions.isBrodcaster || UserPermissions.isModerator || UserInfo.Name == "Dyuha138") && (Params[0] != "!title" && Params[0] != "!setgame")) {

					for (int i = 0; i < Params.Length; i++) {
						if (i == 0) { sb.Append(Params[i]);
						} else { sb.Append(" " + Params[i]); }
					}

					TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, null, sb.ToString());
				} else {
					TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Я не буду выполнять команды, запрещённые для обычных смертных");
				}
			}
		}




		// -- Простейшая хуйня - самобан --
		public void SelfBan(ServiceInfo ServiceInfo, UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {
			//string comname = TechF.TechFuncs.GetFuncCommand(13);
			string[] Params = ParamsOrig.ToArray();
			ParamsOrig = null;

			uint uintl = 1;

			if (UserPermissions.isBrodcaster) {
				TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Стример, что ты делаешь?");
				return;
			}
			if (UserPermissions.isModerator) {
				TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Вы модер, я не умею вас банить");
				return;
			}

			try {
				uintl = Convert.ToUInt32(Params[0]);
			} catch (Exception) {
				TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Параметр не соответствует числу. Принимаются только секунды в виде арабских цифр");
				return;
			}
			if (uintl == 0) {
				TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Э, куда 0 пишешь, перманентный бан я не дам");
				return;
			}
			switch (ServiceInfo.id) {
				case 1:
					BanTWH(UserInfo.Name, uintl, "Самобан");
				break;

				case 2:
					//BanVKPL(new string[] { UserInfo.Name }, uintl, "Самобан");
					TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "самобан пока не доступен");
				break;
			}
		}




		// -- Получение через парсинг страницы в браузере количества зрителей на VKPlay LIVE --
		public void VKPlayLiveViewers(ServiceInfo ServiceInfo, UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {
			string comname = TechF.TechFuncs.GetFuncCommand(15);
			string[] Params = ParamsOrig.ToArray();
			ParamsOrig = null;
			int viewers = 0;
			string DispChannel = null;

			switch (Params[0]) {
				case "h": case "help":
					TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Параметры функции " + comname + ": [ник стримера из адресной строки на VKPlay LIVE (превращение отображаемого ника в ник из адресной строки пока не поддерживается)]");
				break;

				case "NULL":
					Params[0] = TechF.Twitch.Chat.Channel.ToLower();
				break;
			}

			TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "подождите, выполняю...");

			Task.Factory.StartNew(() => {
				// -- Настраиваем запуск Edge из-под Sodium
				EdgeOptions options = new EdgeOptions(); // -- Опции запуска
				EdgeDriver driver = null; // -- Сам браузер
				EdgeDriverService driverservice = EdgeDriverService.CreateDefaultService();
				IWebElement viewerselement = null; // -- Объекты, представляющие из себя веб-элементы
				IWebElement dispnickelement = null;

				if (!TechF.VKPL.Chat.Work) {
					options.AddArgument("--headless");
					options.AddArgument("--mute-audio");
					driverservice.HideCommandPromptWindow = true;

					// -- Запускаем...
					try {
						driver = new EdgeDriver(driverservice, options);
						//driver.Manage().Window.Minimize();
					} catch (Exception ex) {
						TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, произошла ошибка при открытии. Сообщите об этом разработчику, или загляните в логи");
						TechF.TechFuncs.LogDH("Ошибка открытия браузера EDGE: " + ex.Message + "; внутрення ошибка - " + ex.InnerException.Message);
						driver.Quit();
						return;
					}
				}

				driver.Navigate().GoToUrl("https://vkplay.live/" + Params[0]); // - Открываем вкладку
				Thread.Sleep(2000);

				try {
					viewerselement = driver.FindElement(By.ClassName("StreamViewers_text_h1QBT")); // - Ищем строку со зрителями
				} catch (Exception) {
					try {
						driver.FindElement(By.ClassName("Error_title_MyZoO")); // - Если строка со зрителями не найдена, ищем уведомление о том, что канала не существует
					} catch (Exception) {
						TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Канал оффлайн");
						driver.Quit();
						return;
					}
					TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Канал несуществует");
					driver.Quit();
					return;
				}

				dispnickelement = driver.FindElement(By.ClassName("StreamPanel_author_DA_pH")); // - Ищем строку с отображаемым ником

				viewers = Convert.ToInt32(viewerselement.Text.Substring(0, viewerselement.Text.IndexOf(" "))); // - Вырезаем количество зрителей из счётчика зрителей
				DispChannel = dispnickelement.Text;
				driver.Quit(); // - Закрываем браузер
			
				TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Количество зрителей у \"" + DispChannel + "\" на VKPlay LIVE: " + viewers.ToString());
			});
		}




		// -- Управление списком вип-пользователей, у которых есть доступ к секретным, закрытым функциям бота --
		public void VIPUsersManage(ServiceInfo ServiceInfo, UserInfoLite UserInfo, PermissionsInfo UserPermissions, List<string> ParamsOrig) {
			string comname = TechF.TechFuncs.GetFuncCommand(16);
			string[] Params = ParamsOrig.ToArray();
			ParamsOrig = null;
			string reqres = null;
			string uid = null;
			if (Params[0] != "NULL") { Params[0] = Params[0].ToLower(); }

			if (UserPermissions.isBrodcaster || UserPermissions.isModerator || UserInfo.Name == "Dyuha138") {

				switch (Params[0]) {

					case "help":
						TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Параметры функции \"" + comname + "\": Добавить/удалить - \"[ник]\"; Удалить из списка всех - \"wipe\"; Обновить ники - \"update\"");
					break;

					case "wipe":
						TechF.db.VIPUsersT.Wipe();
						TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Полностью очистил список вип-чатерсов");
					break;

					case "update":
						TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Запустил фоновое обновление информации об вип-чатерсах");
						Task.Factory.StartNew(() => {
							string nickl = null;
							List<DB.VIPUsers_tclass> VIPUsersUpdate = new List<DB.VIPUsers_tclass>();
							for (int i = 0; i < TechF.db.VIPUsersList.Count(); i++) {
								reqres = TechF.Twitch.API.ApiRequest("users?id=" + TechF.db.VIPUsersList.ElementAt(i).Userid);
								nickl = TechF.TechFuncs.SearchinJson(reqres, "login");
								if (nickl != TechF.db.VIPUsersList.ElementAt(i).Nick) {
									VIPUsersUpdate.Add(new DB.VIPUsers_tclass(nickl, TechF.db.VIPUsersList.ElementAt(i).Userid));
								}
							}
							TechF.db.VIPUsersT.UpdateNicks(VIPUsersUpdate);
							TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Завершил фоновое обновление информации об вип-чатерсах");
						});
					break;

					case "list":
						StringBuilder strtmp = new StringBuilder(TechF.db.VIPUsersList.Count * 7);
						DB.VIPUsers_tclass VIPUserl = new DB.VIPUsers_tclass();
						for (int i = 0; i < TechF.db.VIPUsersList.Count; i++) {
							VIPUserl = TechF.db.VIPUsersList.ElementAt(i);
							strtmp.Append("\"" + VIPUserl.Nick + "\"; ");
						}
						if (strtmp.Length > 0) {
							TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Список существующих вип-чатерсов: " + strtmp.ToString());
						} else {
							TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но вип-чатерсы отсутствуют");
						}
					break;


					default:
						reqres = TechF.Twitch.API.ApiRequest("users?login=" + Params[0]);
						uid = TechF.TechFuncs.SearchinJson(reqres, "id");

						if (uid != null) {
							if (TechF.TechFuncs.VIPUserValidate(uid) != null) {
								TechF.db.VIPUsersT.Add(new DB.VIPUsers_tclass(Params[0], Convert.ToInt32(uid)));
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Добавил нового вип-чатерcа, теперь у него есть доступ к моим скрытым функциям");
							} else {
								TechF.db.VIPUsersT.Remove(uid);
								TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Удалил чатерса \"" + Params[0] + "\" из списка вип-чатерсов");
							}
						} else {
							TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но вы видимо неправильно ввели ник, потому что я не нашёл такого чатерса");
						}
					break;

				}	
			} else {
				//TechF.TechFuncs.UniversalSendMsg(ServiceInfo.id, new string[] { UserInfo.Name }, "Извините, но у вас нет доступа к этой команде");
			}
		}
	}
}