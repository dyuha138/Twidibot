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
//using ControlzEx.Standard;
using Twidibot.Pages;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using ControlzEx.Standard;
using System.Text.RegularExpressions;
//using System.Windows.Forms;

namespace Twidibot
{
	public class Funcs
	{
		//public event EventHandler<CEvent_Msg> Ev_SendMsg;

		public bool SpamMessagesWork = false;
		private BackWin TechF = null;

		public Funcs(BackWin backWin) {
			//CommandsUpdate();
			TechF = backWin;
		}


		// -- Отдельная функция инициализации и запуска фоновых функций --
		public void Init() {
			//Task.Factory.StartNew(() => TimersBackCheck());
			SpamMessagesWork = Convert.ToBoolean(TechF.TechFuncs.GetSettingParam("SpamMsgWork"));
			if (SpamMessagesWork) { Task.Factory.StartNew(() => SpamMessages()); }
		}


		// -- Функция спама сообщениями --
		public void SpamMessages() {
			DB.SpamMessage_tclass Spaml = null;
			int i = 0;
			if (TechF.TechFuncs.GetSettingParam("SpamMsg") == "0") { // -- Второй тип спама - по собственному таймеру
				string DT = null;
				while (SpamMessagesWork && TechF.Chat.Worked) {
					Spaml = TechF.db.SpamMessagesList.ElementAt<DB.SpamMessage_tclass>(i);
					DateTime dtl = DateTime.Now;
					if (Spaml.Enabled) {
						if (Spaml.LastUsed != null && Spaml.LastUsed != "") {
							DateTime dtcomlul = DateTime.ParseExact(Spaml.LastUsed, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

							TimeSpan res = dtl - dtcomlul; // -- Вычисление разницы
							double secdif = res.TotalSeconds; // -- Перевод разницы в секунды
							if (secdif >= Spaml.CoolDown * 60) {
								TechF.Chat.SendMsg(Spaml.Message);
								Spaml.LastUsed = DT;
								TechF.db.SpamMessagesT.UpdateLastUsed(Spaml.id, DT);
								TechF.db.SpamMessagesList.RemoveAt(i);
								TechF.db.SpamMessagesList.Insert(i, Spaml);
							}
						} else {
							TechF.Chat.SendMsg(Spaml.Message);
							Spaml.LastUsed = DT;
							TechF.db.SpamMessagesT.UpdateLastUsed(Spaml.id, DT);
							TechF.db.SpamMessagesList.RemoveAt(i);
							TechF.db.SpamMessagesList.Insert(i, Spaml);
						}
					}
					i++;
					if (i >= TechF.db.SpamMessagesList.Count) { i = 0; }
					Thread.Sleep(1300);
				}
			} else { // -- Нормальный тип спама - по общему таймеру, по порядку
				int whorg = Convert.ToInt32(TechF.TechFuncs.GetSettingParam("SpamMsg")) * 60000;
				int wh = whorg;
				while (true) {
					Spaml = TechF.db.SpamMessagesList.ElementAt<DB.SpamMessage_tclass>(i);
					if (Spaml.Enabled) {
						TechF.Chat.SendMsg(Spaml.Message);
						wh = whorg;
					} else { wh = 0; }
					i++;
					if (i >= TechF.db.SpamMessagesList.Count) { i = 0; }
					Thread.Sleep(wh);
				}
			}
		}





		// -- Функция для установки режима чата --
		public void ChatMode(string Mode, string Param1, string Param2, bool[] Permissions, string SenderNick) {
			if (Permissions[0] || Permissions[1]) {
				string comname = TechF.TechFuncs.GetFuncCommand(6);
				int sleep = 0;

				if (!TechF.Chat.botparam_Mod) {
					TechF.Chat.SendMsg("@" + SenderNick + ", у бота отсутствуют права модератора, команда недоступна");
					return;
				}

				if (Mode == "NULL") {
					TechF.Chat.SendMsg("@" + SenderNick + ", не указан выставляемый режим");
				} else {
					if (Mode == "help") {
						TechF.Chat.SendMsg("@" + SenderNick + ", Параметры команды " + comname + ": \"emote\" - только смайлы; \"sub\" - только подписчики; \"follow\" [время в минутах] - только фоловеры; \"slow\" [время в секундах] - медленный режим. Также можно указать последним параметром время автоматического отключения режима");
					} else {
						try {
							Convert.ToInt32(Param1);
						} catch (FormatException e) {
							TechF.Chat.SendMsg("@" + SenderNick + ", ошибка установки режима: неправильно указан формат времени для фолоу-мода или медленного режима");
							return;
						}
						try {
							sleep = Convert.ToInt32(Param2);
						} catch (FormatException e) {
							TechF.Chat.SendMsg("@" + SenderNick + ", ошибка установки режима: неправильно указан формат времени выключения режима чата");
							return;
						}

						switch (Mode) {

							case "emote":
							if (TechF.Chat.channelparam_Emote) {
								TechF.Chat.SendCom("emoteonly");
								TechF.Chat.SendCom("me Режим чата \"только смайлы\" включён");
								if (Param1 != "NULL") { // -- Если указано ещё время, то через это время автоматом отключится режим						
									Task.Factory.StartNew(() => {
										Thread.Sleep(sleep);
										TechF.Chat.SendCom("emoteonlyoff");
										TechF.Chat.SendCom("me Режим чата \"только смайлы\" отключён");
									});
								}
							} else {
								TechF.Chat.SendCom("emoteonlyoff");
								TechF.Chat.SendCom("me Режим чата \"только смайлы\" отключён");
							}
							break;

							case "sub":
							if (TechF.Chat.channelparam_Sub) {
								TechF.Chat.SendCom("subscribers");
								TechF.Chat.SendCom("me Режим чата \"Только подписчики\" включён");
								if (Param1 != "NULL") {
									Task.Factory.StartNew(() => {
										Thread.Sleep(sleep);
										TechF.Chat.SendCom("subscribersoff");
										TechF.Chat.SendCom("me Режим чата \"Только подписчики\" отключён");
									});
								}
							} else {
								TechF.Chat.SendCom("subscribersoff");
								TechF.Chat.SendCom("me Режим чата \"Только подписчики\" отключён");
							}
							break;

							case "follow":
							if (Param1 != "NULL") {
								TechF.Chat.SendCom("followers " + Param1);
								TechF.Chat.SendCom("me Режим чата \"Только фоловеры\" (" + Param1 + " мин) включён");
								if (Param2 != "NULL") {
									Task.Factory.StartNew(() => {
										Thread.Sleep(sleep);
										TechF.Chat.SendCom("followersoff");
										TechF.Chat.SendCom("me Режим чата \"Только фоловеры\" отключён"); ;
									});
								}
							} else {
								TechF.Chat.SendCom("followersoff");
								TechF.Chat.SendCom("me Режим чата \"Только фоловеры\" отключён"); ;
							}
							break;

							case "slow":
							if (Param1 != "NULL") {
								TechF.Chat.SendCom("slow " + Param1);
								TechF.Chat.SendCom("me Режим чата \"Медленный режим\" (" + Param1 + " мин) включён");
								if (Param2 != "NULL") {
									Task.Factory.StartNew(() => {
										Thread.Sleep(sleep);
										TechF.Chat.SendCom("slowoff");
										TechF.Chat.SendCom("me Режим чата \"Медленный режим\" отключён");
									});
								}
							} else {
								TechF.Chat.SendCom("slowoff");
								TechF.Chat.SendCom("me Режим чата \"Медленный режим\" отключён");
							}
							break;
						}
					}
				}
			} else {
				// -- Ответ в режиме расширенных ответов
				//if (TechF.)
				//TechF.Chat.SendCom("@" + SenderNick + ", у вас отсутствуют права модератора, команда невыполнена");
			}
		}






		// -- Пирамидка --
		public void Pyramid(string smile, string maxcount, string SenderNick) {
			int MaxCount = int.Parse(maxcount);
			int i = 0;
			string str = "";
			while (i < MaxCount * 2 - 1) {
				if (i < MaxCount) {
					str += smile + " ";
				} else {
					int i2 = 0;
					int count2 = (MaxCount * 2) - i - 1;
					str = "";
					while (i2 < count2) {
						str += smile + " ";
						i2++;
					}
				}
				i++;
				TechF.Chat.SendMsg(str);
				//Thread.Sleep(1100);
			}
		}

		// -- Реверсивная пирамидка --
		public void RevPyramid(string smile, string maxcount, string SenderNick) {
			int MaxCount = int.Parse(maxcount);
			int i = 0;
			string str = "";
			while (i < MaxCount * 2 - 1) {
				if (i < MaxCount) {
					int i2 = 0;
					int count2 = MaxCount - i;
					str = "";
					while (i2 < count2) {
						str += smile + " ";
						i2++;
					}
				} else {
					str += smile + " ";
				}
				i++;
				TechF.Chat.SendMsg(str);
				//Thread.Sleep(1100);
			}
		}





		// -- Таймер (запуск, проверка, остановка, сброс) --
		public void Timers(string Name, string Param, string SenderNick) {
			DB.Funcs_Timer_tclass Timerl = new DB.Funcs_Timer_tclass();
			bool sqlcheck = false;
			string comname = TechF.TechFuncs.GetFuncCommand(3);

			if (Name == null || Name == "NULL") {
				TechF.Chat.SendMsg("@" + SenderNick + ", Вы не ввели название таймера");
			} else {
				if (Name == "help") {
					TechF.Chat.SendMsg("@" + SenderNick + ", Параметры команды \"" + comname + "\": Проверить - \"[имя таймера]\"; Добавить - \"[имя таймера] [время с припиской \"h\" для часов, \"m\" для минут, или \"s\" для секунд (по-умолчанию минуты), и не более 922337203684 секунд]\"; Пауза - \"[имя таймера] p\"; Возобновить - \"[имя таймера] s\"; Сброс - \"[имя таймера] r\"; Удалить - \"[имя таймера] d\"");
				} else {
					switch (Param) {

						case "d":
							sqlcheck = TechF.db.Funcs_TimersT.Remove(Name);
							if (sqlcheck) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Таймер \"" + Name + "\" удалён");
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Таймер \"" + Name + "\" не найден");
							}
						break;

						case "p":
							sqlcheck = TechF.db.Funcs_TimersT.PauseUpdate(Name, true);
							if (sqlcheck) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Таймер \"" + Name + "\" остановлен");
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Таймер \"" + Name + "\" и так остановлен, или не найден");
							}
						break;

						case "s":
							sqlcheck = TechF.db.Funcs_TimersT.PauseUpdate(Name, false);
							if (sqlcheck) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Таймер \"" + Name + "\" возобновлён");
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Таймер \"" + Name + "\" и так работает, или не найден");
							}
						break;

						case "r":
							sqlcheck = TechF.db.Funcs_TimersT.Restart(Name);
							if (sqlcheck) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Таймер \"" + Name + "\" обнулён");
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Таймер \"" + Name + "\" не найден");
							}
						break;

						case "NULL":
							Timerl = TechF.db.TimersList.Find(x => x.Name == Name);

							if (Timerl == null) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Таймер с именем \"" + Name + "\" не найден");
							} else {
								// -- Высчитываем общее время пауз
								TimeSpan DTPaused = new TimeSpan();
								if (Timerl.DTPauseResume != null) {
									string[] dtprm = Timerl.DTPauseResume.Split(';');

									for (uint ip = 0; ip < dtprm.Length; ip++) {
										TimeSpan dtl_s = TimeSpan.ParseExact(dtprm[ip].Substring(0, Timerl.DTPauseResume.IndexOf(',')), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
										TimeSpan dtl_r = TimeSpan.ParseExact(dtprm[ip].Substring(Timerl.DTPauseResume.IndexOf(',') + 1), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
										DTPaused += dtl_r - dtl_s;
									}
								}
							// -- Вычисление, сколько осталось: время запуска - (время сейчас - время таймера) + общее время пауз
							

								TimeSpan tsres = DateTime.ParseExact(Timerl.DTStart, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture) - (DateTime.Now - TimeSpan.FromSeconds(Convert.ToDouble(Timerl.Time))) + DTPaused;
								StringBuilder strout = new StringBuilder("@" + SenderNick + ", До окончания таймера \"" + Name + "\" осталось: ", 200);
								DateTime dtres = new DateTime();
								bool adddcheck = false;
								bool addtcheck = false;
								dtres = dtres + tsres;

								if (dtres.Year - 1 > 0) {
									strout.Append((dtres.Year - 1) + TechF.TechFuncs.WordEndNumber(dtres.Year, "Y"));
									adddcheck = true;
								}
								if (dtres.Month - 1 > 0) {
									if (adddcheck) {
										strout.Append(" " + (dtres.Month - 1) + TechF.TechFuncs.WordEndNumber(dtres.Month, "M"));
									} else {
										strout.Append((dtres.Month - 1) + TechF.TechFuncs.WordEndNumber(dtres.Month, "M"));
										adddcheck = true;
									}
								}
								if (dtres.Day - 1 > 0) {
									if (adddcheck) {
										strout.Append(" " + (dtres.Day - 1) + TechF.TechFuncs.WordEndNumber(dtres.Day, "D"));
									} else {
										strout.Append((dtres.Day - 1) + TechF.TechFuncs.WordEndNumber(dtres.Day, "D"));
										adddcheck = true;
									}
								}
								if (dtres.Hour > 0) {
									if (adddcheck) { strout.Append(" "); }
									strout.Append(dtres.Hour + TechF.TechFuncs.WordEndNumber(dtres.Hour, "h"));
									addtcheck = true;
								}
								if (dtres.Minute > 0) {
									if (addtcheck) {
										strout.Append(" " + dtres.Minute + TechF.TechFuncs.WordEndNumber(dtres.Minute, "m"));
									} else {
										strout.Append(dtres.Minute + TechF.TechFuncs.WordEndNumber(dtres.Minute, "m"));
										addtcheck = true;
									}
								}
								if (dtres.Second > 0) {
									if (addtcheck) {
										strout.Append(" " + dtres.Second + TechF.TechFuncs.WordEndNumber(dtres.Second, "s"));
									} else {
										strout.Append(dtres.Second + TechF.TechFuncs.WordEndNumber(dtres.Second, "s"));
									}
								}
								TechF.Chat.SendMsg(strout.ToString());
							}
						break;

						default:
							ulong Time = 0;
							string Type = null, Timestr = null;
							if (Param.StartsWith("-") || Param == "0") {
								TechF.Chat.SendMsg("@" + SenderNick + ", вы указали отрицательное или нулевое значение, таймер НЕ запущен");
								break;
							}
							if (Param.EndsWith("h") || Param.EndsWith("m") || Param.EndsWith("s")) {
								Type = Param.Substring(Param.Length - 1);
								Timestr = Param.Substring(0, Param.Length - 1);
							} else { Timestr = Param; }
							try {
								Time = Convert.ToUInt64(Timestr);
							} catch(FormatException e) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Введены НЕ цифры, таймер НЕ запущен. Чтобы получить помощь, наберите  \"" + comname + " help\"");
								break;
							} catch(OverflowException) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Указано слишком большое число, таймер НЕ запущен");
								break;
							}
							if (Type == null) { Type = "m"; }

							if (Type == "h") {
								Time *= 3600;
							} else {
								if (Type == "m") {
									Time *= 60;
								} else {
									if (Type == "s") {

									} else {
										TechF.Chat.SendMsg("@" + SenderNick + ", Не удалось понять, что вы хотели сделать с таймерами. Чтобы получить помощь, наберите  \"" + comname + " help\"");
										break;
									}
								}
							}
							if (Time > TimeSpan.MaxValue.TotalSeconds - 1) {
								TechF.Chat.SendMsg("@" + SenderNick + ", вы указали слишком большое число (больше 922337203684 секунд), таймер НЕ запущен");
								break;
							}
							Timerl = TechF.db.TimersList.Find(x => x.Name == Name);
							if (Timerl == null) {
								DateTime DTdt = DateTime.Now;
								string DT = DTdt.ToString();
								if (DTdt.Hour < 10) {
									Regex Regexstr = new Regex(@"\s");
									DT = Regexstr.Replace(DT, " 0");
								}
								TechF.db.Funcs_TimersT.Add(new DB.Funcs_Timer_tclass(Name, Time, SenderNick, DT));
								if (Type == "s") { TechF.Chat.SendMsg("@" + SenderNick + ", Таймер \"" + Name + "\" на " + Time + " " + TechF.TechFuncs.WordEndNumber(Time, "s") + " запущен!"); }
								if (Type == "m") { TechF.Chat.SendMsg("@" + SenderNick + ", Таймер \"" + Name + "\" на " + Time / 60 + " " + TechF.TechFuncs.WordEndNumber(Time / 60, "m") + " запущен!"); }
								if (Type == "h") { TechF.Chat.SendMsg("@" + SenderNick + ", Таймер \"" + Name + "\" на " + Time / 3600 + " " + TechF.TechFuncs.WordEndNumber(Time / 3600, "h") + " запущен!"); }
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Ошибка добавления таймера \"" + Name + "\" - такой таймер уже существует");
							}
						break;	
					}
				}
			}
		}



		// -- Фоновая проверка таймеров --
		private void TimersBackCheck() {
			DB.Funcs_Timer_tclass Timerl = new DB.Funcs_Timer_tclass();

			while (TechF.Chat.Worked) {
				if (TechF.db.TimersList.Count > 0) {
					for (uint i = 0; i < TechF.db.TimersList.Count; i++) {
						Timerl = TechF.db.TimersList.ElementAt<DB.Funcs_Timer_tclass>(Convert.ToInt32(i));
						if (!Timerl.Paused) {
							// -- Высчитываем общее время пауз
							TimeSpan DTPaused = new TimeSpan();
							if (Timerl.DTPauseResume != null) {
								string[] dtprm = Timerl.DTPauseResume.Split(';');
								
								for (uint ip = 0; ip < dtprm.Length; ip++) {
									TimeSpan dtl_s = TimeSpan.ParseExact(dtprm[ip].Substring(0, Timerl.DTPauseResume.IndexOf(',')), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
									TimeSpan dtl_r = TimeSpan.ParseExact(dtprm[ip].Substring(Timerl.DTPauseResume.IndexOf(',')+1), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
									DTPaused += dtl_r - dtl_s;
								}
							}
							// -- Вычисление, сколько осталось: время запуска - (время сейчас - время таймера) + общее время пауз
							TimeSpan res = DateTime.ParseExact(Timerl.DTStart, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture) - (DateTime.Now - TimeSpan.FromSeconds(Convert.ToDouble(Timerl.Time))) + DTPaused;
							if (res.TotalSeconds < 1) {
								TechF.Chat.SendMsg("@" + Timerl.Nick + ", таймер \"" + Timerl.Name + "\" закончился!");
								TechF.db.Funcs_TimersT.Remove(Timerl.Name);
								//TechF.db.TimersList.RemoveAt(Convert.ToInt32(i));
							}
						}
					}
				} else {
					Thread.Sleep(3000);
				}
				Thread.Sleep(200);
			}
		}




		// -- Секундомер (запуск, проверка, остановка, сброс) --
		public void Stopwatches(string Name, string Param, string SenderNick) {
			DB.Funcs_Stopwatch_tclass Stopwatchl = new DB.Funcs_Stopwatch_tclass();
			bool sqlcheck = false;
			string comname = TechF.TechFuncs.GetFuncCommand(4);

			if (Name == null || Name == "NULL") {
				TechF.Chat.SendMsg("@" + SenderNick + ", Вы не ввели название секундомера");
			} else {
				if (Name == "help") {
					TechF.Chat.SendMsg("@" + SenderNick + ", Параметры команды \"" + comname + "\": Проверить - \"[имя секундомера]\"; Добавить - \"[имя секундомера] a\"; Пауза - \"[имя секундомера] p\"; Возобновление - \"[имя секундомера] s\"; Сброс - \"[имя секундомера] r\"; Удаление - \"[имя секундомера] d\"");
				} else {
					switch (Param) {

						case "d":
							sqlcheck = TechF.db.Funcs_StopwatchesT.Remove(Name);
							if (sqlcheck) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Секундомер \"" + Name + "\" удалён");
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Секундомер \"" + Name + "\" не найден");
							}
						break;

						case "p":
							sqlcheck = TechF.db.Funcs_StopwatchesT.PauseUpdate(Name, true);
							if (sqlcheck) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Секундомер \"" + Name + "\" остановлен");
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Секундомер \"" + Name + "\" и так остановлен, или не найден");
							}
						break;

						case "s":
							sqlcheck = TechF.db.Funcs_StopwatchesT.PauseUpdate(Name, false);
							if (sqlcheck) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Секундомер \"" + Name + "\" возобновлён");
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Секундомер \"" + Name + "\" и так работает, или не найден");
							}
						break;

						case "r":
							sqlcheck = TechF.db.Funcs_StopwatchesT.Restart(Name);
							if (sqlcheck) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Секундомер \"" + Name + "\" обнулён");
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Секундомер \"" + Name + "\" не найден");
							}
						break;

						case "NULL":
							Stopwatchl = TechF.db.StopwatchesList.Find(x => x.Name == Name);

							if (Stopwatchl == null) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Секундомер с именем \"" + Name + "\" не найден");
							} else {
								// -- Высчитываем общее время пауз
								TimeSpan DTPaused = new TimeSpan();
								if (Stopwatchl.DTPauseResume != null) {
									string[] dtprm = Stopwatchl.DTPauseResume.Split(';');

									for (uint ip = 0; ip < dtprm.Length; ip++) {
										DateTime dtl_s = DateTime.ParseExact(dtprm[ip].Substring(0, Stopwatchl.DTPauseResume.IndexOf(',')), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
										DateTime dtl_r = DateTime.ParseExact(dtprm[ip].Substring(Stopwatchl.DTPauseResume.IndexOf(',') + 1), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
										DTPaused += dtl_r - dtl_s;
									}
								}
								// -- Вычисление, сколько прошло: (время запуска + время сейчас) - общее время пауз
								TimeSpan tsres = (DateTime.ParseExact(Stopwatchl.DTStart, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture) - DateTime.Now) - DTPaused;
								StringBuilder strout = new StringBuilder("@" + SenderNick + ", Значение секундомера \"" + Name + "\": ", 200);
								DateTime dtres = new DateTime();
								bool adddcheck = false;
								bool addtcheck = false;
								dtres = dtres + tsres.Negate();

								if (dtres.Year - 1 > 0) {
									strout.Append((dtres.Year - 1) + " " + TechF.TechFuncs.WordEndNumber(dtres.Year, "Y"));
									adddcheck = true;
								}
								if (dtres.Month - 1 > 0) {
									if (adddcheck) {
										strout.Append(", " + (dtres.Month - 1) + " " + TechF.TechFuncs.WordEndNumber(dtres.Month, "M"));
									} else {
										strout.Append((dtres.Month - 1) + " " + TechF.TechFuncs.WordEndNumber(dtres.Month, "M"));
										adddcheck = true;
									}
								}
								if (dtres.Day - 1 > 0) {
									if (adddcheck) {
										strout.Append(", " + (dtres.Day - 1) + " " + TechF.TechFuncs.WordEndNumber(dtres.Day, "D"));
									} else {
										strout.Append((dtres.Day - 1) + " " + TechF.TechFuncs.WordEndNumber(dtres.Day, "D"));
										adddcheck = true;
									}
								}
								if (dtres.Hour > 0) {
									if (adddcheck) { strout.Append(", "); }
									strout.Append(dtres.Hour + " " + TechF.TechFuncs.WordEndNumber(dtres.Hour, "h"));
									addtcheck = true;
								}
								if (dtres.Minute > 0) {
									if (addtcheck) {
										strout.Append(", " + dtres.Minute + " " + TechF.TechFuncs.WordEndNumber(dtres.Minute, "m"));
									} else {
										strout.Append(dtres.Minute + " " + TechF.TechFuncs.WordEndNumber(dtres.Minute, "m"));
										addtcheck = true;
									}
								}
								if (dtres.Second > 0) {
									if (addtcheck) {
										strout.Append(", " + dtres.Second + " " + TechF.TechFuncs.WordEndNumber(dtres.Second, "s"));
									} else {
										strout.Append(dtres.Second + " " + TechF.TechFuncs.WordEndNumber(dtres.Second, "s"));
									}
								}
								TechF.Chat.SendMsg(strout.ToString());
							}
						break;

						case "a":
							Stopwatchl = TechF.db.StopwatchesList.Find(x => x.Name == Name);
							if (Stopwatchl == null) {
								DateTime DTdt = DateTime.Now;
								string DT = DTdt.ToString();
								if (DTdt.Hour < 10) {
									Regex Regexstr = new Regex(@"\s");
									DT = Regexstr.Replace(DT, " 0");
								}
								TechF.db.Funcs_StopwatchesT.Add(new DB.Funcs_Stopwatch_tclass(Name, SenderNick, DT));
								TechF.Chat.SendMsg("@" + SenderNick + ", Секундомер \"" + Name + "\" запущен");
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Ошибка добавления секундомера \"" + Name + "\" - такой секундомер уже существует");
							}
						break;

						default:
							TechF.Chat.SendMsg("@" + SenderNick + ", Не удалось понять, что вы хотели сделать с секундомерами. Чтобы получить помощь, введите \"" + comname + " help\"");
						break;
					}
				}
			}
		}



		// -- Счётчик (проверка, добавление, сброс) --
		public void Counter(string Name, string Param, string SenderNick) {
			DB.Funcs_Counter_tclass Counterl = new DB.Funcs_Counter_tclass();
			bool sqlcheck = false;
			string comname = TechF.TechFuncs.GetFuncCommand(5);

			if (Name == null || Name == "NULL") {
				TechF.Chat.SendMsg("@" + SenderNick + ", Вы не ввели название счётчика");
			} else {
				if (Name == "help") {
					TechF.Chat.SendMsg("@" + SenderNick + ", Параметры команды \"" + comname + "\": Проверить - \"[имя счётчика]\"; Добавить - \"[имя счётчика] a\"; Установить значение - \"[имя счётчика] [значение]\"; Прибавить 1 - \"[имя счётчика] +\"; Отнять 1 - \"[имя счётчика] -\"; Сброс - \"[имя счётчика] r\"; Удаление - \"[имя счётчика d\"");
				} else {
					switch (Param) {
						case "d":
							sqlcheck = TechF.db.Funcs_CountersT.Remove(Name);
							if (sqlcheck) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Счётчик \"" + Name + "\" удалён");
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Счётчик \"" + Name + "\" не найден");
							}
						break;

						case "r":
							sqlcheck = TechF.db.Funcs_CountersT.Restart(Name);
							if (sqlcheck) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Счётчик \"" + Name + "\" обнулён");
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Счётчик \"" + Name + "\" не найден");
							}
						break;


						case "NULL":
							Counterl = TechF.db.CountersList.Find(x => x.Name == Name);
							if (Counterl != null) {
								TechF.Chat.SendMsg("@" + SenderNick + ", Значение счётчика \"" + Name + "\": " + Counterl.Value);
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Счётчик \"" + Name + "\" не найден");
							}
						break;

						case "+":
							sqlcheck = TechF.db.Funcs_CountersT.Plus(Name);
							if (sqlcheck) {
								Counterl = TechF.db.CountersList.Find(x => x.Name == Name);
								TechF.Chat.SendMsg("@" + SenderNick + ", Счётчик \"" + Name + "\" увеличен на 1, текущее значение - " + Counterl.Value);
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Счётчик \"" + Name + "\" не найден");
							}
						break;

						case "-":
							sqlcheck = TechF.db.Funcs_CountersT.Minus(Name);
							if (sqlcheck) {
								Counterl = TechF.db.CountersList.Find(x => x.Name == Name);
								TechF.Chat.SendMsg("@" + SenderNick + ", Счётчик \"" + Name + "\" уменьшен на 1, текущее значение - " + Counterl.Value);
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Счётчик \"" + Name + "\" не найден");
							}
						break;

						case "a":
						Counterl = TechF.db.CountersList.Find(x => x.Name == Name);
						if (Counterl == null) {
							TechF.db.Funcs_CountersT.Add(new DB.Funcs_Counter_tclass(Name));
							TechF.Chat.SendMsg("@" + SenderNick + ", Счётчик \"" + Name + "\" создан");
						} else {
							TechF.Chat.SendMsg("@" + SenderNick + ", Ошибка добавления счётчика \"" + Name + "\" - такой счётчик уже существует");
						}
						break;

						default:
							int val = 0;
							try {
								val = Convert.ToInt32(Param);
							} catch(FormatException e) {
							TechF.Chat.SendMsg("@" + SenderNick + ", Не удалось понять, что вы хотели сделать со счётчиками. Чтобы получить помощь, введите \"" + comname + " help\"");
								break;
							}
							sqlcheck = TechF.db.Funcs_CountersT.Update(Name, val);
							if (sqlcheck) {
								Counterl = TechF.db.CountersList.Find(x => x.Name == Name);
								TechF.Chat.SendMsg("@" + SenderNick + ", Счётчик \"" + Name + "\" обновлён, текущее значение - " + Counterl.Value);
							} else {
								TechF.Chat.SendMsg("@" + SenderNick + ", Счётчик \"" + Name + "\" не найден");
							}
							
						break;
					}
				}
			}
		}




	}
}
