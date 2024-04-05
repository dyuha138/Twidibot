using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Timers;
using System.IO;
using Simple.Data.Extensions;
using System.Diagnostics;
using System.Windows.Shapes;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Documents;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace Twidibot
{
	public class TechFuncs {
		public string LogPath = null;
		private BackWin TechF = null;
		private string ProgramPath = null;
		private List<string> MsgListtoLog = null;
		private bool LogWork = true;
		//private event EventHandler<Twident_Msg> Ev_LogDH;

		private HttpListener MiniServer = null;
		//private TcpClient MiniClient = null;
		private HttpListenerContext MiniServer_Context = null;
		private HttpListenerRequest MiniServer_Request = null;
		private HttpListenerResponse MiniServer_Response = null;


		public TechFuncs(BackWin backWin) {
			this.TechF = backWin;
			this.MsgListtoLog = new List<string>();
			//this.Ev_LogDH += LogDH_WriteBuf;
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
			Date = Date.Replace('.', '-');
			Time = Time.Replace(':', '-');
			var process = Process.GetCurrentProcess();
			ProgramPath = process.MainModule.FileName;
			ProgramPath = ProgramPath.Substring(0, ProgramPath.LastIndexOf("\\"));

			if (!Directory.Exists(ProgramPath + "\\Logs")) { // -- Создание папки, если её не существует
				Directory.CreateDirectory(ProgramPath + "\\Logs");
			}

			LogPath = ProgramPath + "\\Logs\\twidilog_" + Date + "_" + Time + ".txt";
		}



		// -- Инициализация всего местного, кроме лога
		public void Init() {
			MiniServer = new HttpListener();
			//MiniClient = new TcpClient();
			MiniServer.Prefixes.Add("https://twidi.localhost:8138/");
			MiniServer.Prefixes.Add("https://localhost:8138/");
			MiniServer.Prefixes.Add("https://twidi.localhost/");
			//ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
			//Task.Factory.StartNew(() => MiniServer_Start());
			LogDH_StartStop();
		}


		public void LogDH_StartStop() {
			if (TechF.TechFuncs.GetSettingParam("Logging") == "1") {
				LogWork = true;
				Task.Factory.StartNew(() => LogDH_AwaitWrite());
			} else {
				LogWork = false;
			}
		}



		// -- Логгирование --
		// -- Буфер и сразу запись (раньше функция записи была отдельно)
		private void LogDH_AwaitWrite() {
			while (TechF.Work && LogWork) {
				if (MsgListtoLog.Count > 0) {
					byte[] array = null;
					string Msg = MsgListtoLog.ElementAt(0);
					MsgListtoLog.RemoveAt(0);
					FileStream fs = File.Open(LogPath, FileMode.Append); // -- Открываем в режиме записи в конец
					if (Msg.EndsWith("\n")) {
						array = System.Text.Encoding.UTF8.GetBytes(Msg);
					} else {
						array = System.Text.Encoding.UTF8.GetBytes(Msg + "\r\n");
					}
					fs.Write(array, 0, array.Length);
					fs.Flush();
					fs.Close();
				}
				Thread.Sleep(100);
			}
		}

		// -- Принимающая функция
		public void LogDH(string Message) {
			//Ev_LogDH(this, new Twident_Msg(Message));
			MsgListtoLog.Add("(" + DateTime.Now.ToString() + ") " + Message);
		}

		public void LogDH_WriteBuf(object sender, Twident_Msg e) {
			//Task.Factory.StartNew(() => MsgListtoLog.Add("(" + DateTime.Now.ToString() + ") " + e.Message));
			MsgListtoLog.Add("(" + DateTime.Now.ToString() + ") " + e.Message);
		}




		// -- Функция получения параметра настройки, чтобы не делать на месте это каждый раз и не занимать память лишней переменной --
		public string GetSettingParam(string SettingName) {
			bool val = false;
			bool sec = false;
			DB.Setting_tclass Setl = TechF.db.SettingsList.Find(x => x.SetName == SettingName);

			if (Setl == null) { return null; } else {
				sec = Setl.Secured;
				if (Setl.Param != null && Setl.Param != "") {
					val = true;
				} else { val = false; }
			}

			if (sec) {
				if (val) {
					return "true";
				} else { return "false"; }
			} else {
				if (val) {
					return Setl.Param;
				} else { return null; }
			}
		}
		public string GetSettingTWHParam(string SettingName) {
			bool val = false;
			bool sec = false;
			DB.Setting_tclass Setl = TechF.db.SettingsTWHList.Find(x => x.SetName == SettingName);

			if (Setl == null) { return null; } else {
				sec = Setl.Secured;
				if (Setl.Param != null && Setl.Param != "") {
					val = true;
				} else { val = false; }
			}

			if (sec) {
				if (val) {
					return "true";
				} else { return "false"; }
			} else {
				if (val) {
					return Setl.Param;
				} else { return null; }
			}
		}
		public string GetSettingVKPLParam(string SettingName) {
			bool val = false;
			bool sec = false;
			DB.Setting_tclass Setl = TechF.db.SettingsVKPLList.Find(x => x.SetName == SettingName);

			if (Setl == null) { return null; } else {
				sec = Setl.Secured;
				if (Setl.Param != null && Setl.Param != "") {
					val = true;
				} else { val = false; }
			}

			if (sec) {
				if (val) {
					return "true";
				} else { return "false"; }
			} else {
				if (val) {
					return Setl.Param;
				} else { return null; }
			}
		}



		// -- Получение команды для встроенной функции --
		public string GetFuncCommand(int Funcid) {
			DB.FuncCommand_tclass Funcl = TechF.db.FuncCommandsList.Find(x => x.id == Funcid);
			if (Funcl != null && Funcl.Command != null && Funcl.Command != "") {
				return Funcl.Command;
			} else {
				return null;
			}
		}



		// -- Функция установки окончания у слова в соответствии с заданым числом --
		// <param name="EndType">Вариант окончания слова в соответствии с падежами от 1 до 4 включительно (не действует на цифры от 5 до 9 и 0)</param>
		// s: секунды, секунде, секунду, секундой, секунде<br></br>
		// m: минуты, минуте, минуту, минутой, минуте<br></br>
		// h: часа, часу, час, часом, часе<br></br>
		// D: дня, дню, день, днём, дне<br></br>
		// M: месяца, месяцу, месяц, месяцем, месяце<br></br>
		// Y: года, году, год, годом, годе<br></br><br></br>
		// 2,3,4 (0 и 1 варианты окончания объединены)<br></br>
		/// <summary>
		/// Определяет по последней цифре передаваемого числа подходящую форму слова
		/// </summary>
		/// <param name="Number">Число типа short, ushort, int, uint, long, ulong</param>
		/// <param name="TimeType">Тип времени. s - секунда; m - минута; h - час; D - день; M - месяц; Y - год</param>
		/// <param name="EndType">Вариант окончания слова в соответствии с падежами (только для оканчивающей цифры &quot;1&quot;)</param>
		/// <param name="IgnorenotOne">Выдача окончания как при 0 на любой цифре, кроме единицы</param>
		/// <returns>Целое слово с соответствующим падежным окончанием, обозначенное цифрой от 0 до 5 в EndType. При передаче не числа вернёт null
		/// <br></br><br></br>
		/// Варианты ответа<br></br>
		/// 1<br></br>
		/// s: секунда, секунды, секунде, секунду, секундой, секунде<br></br>
		/// m: минута, минуты, минуте, минуту, минутой, минуте<br></br>
		/// h: час, часа, часу, час, часом, часе<br></br>
		/// D: день, дня, дню. день, днём, дне<br></br>
		/// M: месяц, месяца, месяцу, месяц, месяцем, месяце<br></br>
		/// Y: год, года, году, год, годом, годе<br></br><br></br>
		/// 
		/// 2,3,4<br></br>
		/// секунды, минуты, часа, дня, месяца, года<br></br><br></br>
		/// 
		/// 5,6,7,8,9,0<br></br>
		/// секунд, минут, часов, дней, месяцев, лет
		/// </returns>
		public string WordEndNumber(object Number, string TimeType, int EndType = 0, bool IgnorenotOne = false) {
			string strout = null;
			try { strout = Convert.ToString(Number); } catch (Exception) { return null; }
			short tim = 0;

			if (strout.StartsWith("-")) { strout = strout.Substring(1); }

			if (Convert.ToUInt64(strout) > 19) {
				strout = strout.Substring(strout.Length - 1);
				tim = Convert.ToInt16(strout);
			} else { tim = Convert.ToInt16(Number); }

			if (IgnorenotOne && tim > 1) { tim = 0; }

			if (tim == 1) {
				switch (TimeType) {
					case "s":
					switch (EndType) {
						case 0: strout = "секунда"; break;
						case 1: strout = "секунды"; break;
						case 2: strout = "секунде"; break;
						case 3: strout = "секунду"; break;
						case 4: strout = "секундой"; break;
						case 5: strout = "секунде"; break;
						//default: strout = "секунда"; break;
					}
					break;

					case "m":
					switch (EndType) {
						case 0: strout = "минута"; break;
						case 1: strout = "минуты"; break;
						case 2: strout = "минуте"; break;
						case 3: strout = "минуту"; break;
						case 4: strout = "минутой"; break;
						case 5: strout = "минуте"; break;
						//default: strout = "минута"; break;
					}
					break;

					case "h":
					switch (EndType) {
						case 0: strout = "час"; break;
						case 1: strout = "часа"; break;
						case 2: strout = "часу"; break;
						case 3: strout = "час"; break;
						case 4: strout = "часом"; break;
						case 5: strout = "часе"; break;
						//default: strout = "час"; break;
					}
					break;

					case "D":
					switch (EndType) {
						case 0: strout = "день"; break;
						case 1: strout = "дня"; break;
						case 2: strout = "дню"; break;
						case 3: strout = "день"; break;
						case 4: strout = "днём"; break;
						case 5: strout = "дне"; break;
						//default: strout = "день"; break;
					}
					break;

					case "M":
					switch (EndType) {
						case 0: strout = "месяц"; break;
						case 1: strout = "месяца"; break;
						case 2: strout = "месяцу"; break;
						case 3: strout = "месяц"; break;
						case 4: strout = "месяцем"; break;
						case 5: strout = "месяце"; break;
						//default: strout = "месяц"; break;
					}
					break;

					case "Y":
					switch (EndType) {
						case 0: strout = "год"; break;
						case 1: strout = "года"; break;
						case 2: strout = "году"; break;
						case 3: strout = "год"; break;
						case 4: strout = "годом"; break;
						case 5: strout = "годе"; break;
						//default: strout = "год"; break;
					}
					break;
				}

			} else {
				if (tim > 1 && tim < 5) {
					/*switch (TimeType) {
						case "s":
						switch (EndType) {
							case 0:case 1: strout = "секунды"; break;
							case 2: strout = "секунде"; break;
							case 3: strout = "секунду"; break;
							case 4: strout = "секундой"; break;
							case 5: strout = "секунде"; break;
							//default: strout = "секунды"; break;
						} break;

						case "m":
						switch (EndType) {
							case 0:case 1: strout = "минуты"; break;
							case 2: strout = "минуте"; break;
							case 3: strout = "минуту"; break;
							case 4: strout = "минутой"; break;
							case 5: strout = "минуте"; break;
							//default: strout = "минуты"; break;
						} break;

						case "h":
						switch (EndType) {
							case 0:case 1: strout = "часа"; break;
							case 2: strout = "часу"; break;
							case 3: strout = "час"; break;
							case 4: strout = "часом"; break;
							case 5: strout = "часе"; break;
							//default: strout = "часа"; break;
						} break;

						case "D":
						switch (EndType) {
							case 0:case 1: strout = "дня"; break;
							case 2: strout = "дню"; break;
							case 3: strout = "день"; break;
							case 4: strout = "днём"; break;
							case 5: strout = "дне"; break;
							//default: strout = "дня"; break;
						} break;

						case "M":
						switch (EndType) {
							case 0:case 1: strout = "месяца"; break;
							case 2: strout = "месяцу"; break;
							case 3: strout = "месяц"; break;
							case 4: strout = "месяцем"; break;
							case 5: strout = "месяце"; break;
							//default: strout = "месяца"; break;
						} break;

						case "Y":
						switch (EndType) {
							case 0:case 1: strout = "года"; break;
							case 2: strout = "году"; break;
							case 3: strout = "год"; break;
							case 4: strout = "годом"; break;
							case 5: strout = "годе"; break;
							//default: strout = "год"; break;
						} break;
					}*/

					if (TimeType == "s") { strout = "секунды"; }
					if (TimeType == "m") { strout = "минуты"; }
					if (TimeType == "h") { strout = "часа"; }
					if (TimeType == "D") { strout = "дня"; }
					if (TimeType == "M") { strout = "месяца"; }
					if (TimeType == "Y") { strout = "года"; }

				} else {

					/*switch (TimeType) {
						case "s":
						switch (EndType) {
							case 0: strout = "секунд"; break;
							case 1: strout = "секунды"; break;
							case 2: strout = "секунде"; break;
							case 3: strout = "секунду"; break;
							case 4: strout = "секундой"; break;
							case 5: strout = "секунде"; break;
							//default: strout = "секунд"; break;
						} break;

						case "m":
						switch (EndType) {
							case 0: strout = "минут"; break
							case 1: strout = "минуты"; break;
							case 2: strout = "минуте"; break;
							case 3: strout = "минуту"; break;
							case 4: strout = "минутой"; break;
							case 5: strout = "минуте"; break;
							//default: strout = "минут"; break;
						} break;

						case "h":
						switch (EndType) {
							case 0: strout = "часов"; break
							case 1: strout = "часа"; break;
							case 2: strout = "часу"; break;
							case 3: strout = "час"; break;
							case 4: strout = "часом"; break;
							case 5: strout = "часе"; break;
							//default: strout = "часов"; break;
						} break;

						case "D":
						switch (EndType) {
							case 0: strout = "дней"; break
							case 1: strout = "дня"; break;
							case 2: strout = "дню"; break;
							case 3: strout = "день"; break;
							case 4: strout = "днём"; break;
							case 5: strout = "дне"; break;
							//default: strout = "дней"; break;
						} break;

						case "M":
						switch (EndType) {
							case 0: strout = "месяцев"; break
							case 1: strout = "месяца"; break;
							case 2: strout = "месяцу"; break;
							case 3: strout = "месяц"; break;
							case 4: strout = "месяцем"; break;
							case 5: strout = "месяце"; break;
							//default: strout = "месяцев"; break;
						} break;

						case "Y":
						switch (EndType) {
							case 0: strout = "лет"; break
							case 1: strout = "года"; break;
							case 2: strout = "году"; break;
							case 3: strout = "год"; break;
							case 4: strout = "годом"; break;
							case 5: strout = "годе"; break;
							//default: strout = "лет"; break;
						} break;
					}*/

					if (TimeType == "s") { strout = "секунд"; }
					if (TimeType == "m") { strout = "минут"; }
					if (TimeType == "h") { strout = "часов"; }
					if (TimeType == "D") { strout = "дней"; }
					if (TimeType == "M") { strout = "месяцев"; }
					if (TimeType == "Y") { strout = "лет"; }
				}
			}

			return strout;
		}



		// -- Поиск среди ответов на запросы
		public string SearchinJson(string strorg, string str, bool isInt = false) {
			string strtmp, strout;

			if (strorg != null && str != null && strorg.Contains(str)) {

				strtmp = strorg.Substring(strorg.IndexOf("\"" + str + "\"") + str.Length + 2);
				if (isInt) {
					strtmp = strtmp.Substring(strtmp.IndexOf(":") + 1);
					strout = strtmp.Substring(0, strtmp.IndexOf(","));
				} else {
					strtmp = strtmp.Substring(strtmp.IndexOf("\"") + 1);
					strout = strtmp.Substring(0, strtmp.IndexOf("\""));
				}
			} else {
				strout = null;
			}
			//if (strout == "") { strout = null; }
			return strout;
		}


		public string SearchinText(string strOrg, string strSearch, string SearchStart, string SearchEnd, string ValueStart, string ValueEnd) {
			string strtmp, strout;

			if (strOrg != null && strSearch != null && strOrg.Contains(strSearch) && strOrg.Contains(ValueStart) && strOrg.Contains(ValueEnd) && strSearch.Contains(SearchStart) && strSearch.Contains(SearchEnd)) {

				strtmp = strOrg.Substring(strOrg.IndexOf(SearchStart + strSearch + SearchEnd) + SearchStart.Length + strSearch.Length + SearchEnd.Length);
				strtmp = strtmp.Substring(strtmp.IndexOf(ValueStart) + ValueStart.Length);
				strout = strtmp.Substring(0, strtmp.IndexOf(ValueEnd));

			} else {
				strout = null;
			}
			//if (strout == "") { strout = null; }
			return strout;
		}



		// -- Проверка пользователя на наличие в списке випов для доступа к закрытым функциям бота --
		public bool VIPUserValidate(int Userid) {
			int VIPUserid = TechF.db.VIPUsersList.FindIndex(x => x.Userid == Userid);
			if (VIPUserid != (-1)) {
				return true;
			} else {
				return false;
			}
		}
		public string VIPUserValidate(string Userid) {
			DB.VIPUsers_tclass VIPUser = TechF.db.VIPUsersList.Find(x => x.Userid == Convert.ToInt32(Userid));
			if (VIPUser != null) {
				return VIPUser.Nick;
			} else {
				return null;
			}
		}



		// -- "Умная" конвертация DateTime в строку --
		public string DateTimeString(DateTime dtl) {
			StringBuilder strout = new StringBuilder();
			bool addcheck = false;

			if (dtl.Year - 1 > 0) {
				strout.Append((dtl.Year - 1) + " " + TechF.TechFuncs.WordEndNumber(dtl.Year - 1, "Y"));
				addcheck = true;
			}
			if (dtl.Month - 1 > 0) {
				if (addcheck) {
					strout.Append(", " + (dtl.Month - 1) + " " + TechF.TechFuncs.WordEndNumber(dtl.Month - 1, "M"));
				} else {
					strout.Append((dtl.Month - 1) + " " + TechF.TechFuncs.WordEndNumber(dtl.Month - 1, "M"));
					addcheck = true;
				}
			}
			if (dtl.Day - 1 > 0) {
				if (addcheck) {
					strout.Append(", " + (dtl.Day - 1) + " " + TechF.TechFuncs.WordEndNumber(dtl.Day - 1, "D"));
				} else {
					strout.Append((dtl.Day - 1) + " " + TechF.TechFuncs.WordEndNumber(dtl.Day - 1, "D"));
					addcheck = true;
				}
			}
			if (dtl.Hour > 0) {
				if (addcheck) {
					strout.Append(", " + (dtl.Hour) + " " + TechF.TechFuncs.WordEndNumber(dtl.Hour, "h"));
				} else {
					strout.Append(dtl.Hour + " " + TechF.TechFuncs.WordEndNumber(dtl.Hour, "h"));
					addcheck = true;
				}
			}
			if (dtl.Minute > 0) {
				if (addcheck) {
					strout.Append(", " + dtl.Minute + " " + TechF.TechFuncs.WordEndNumber(dtl.Minute, "m"));
				} else {
					strout.Append(dtl.Minute + " " + TechF.TechFuncs.WordEndNumber(dtl.Minute, "m"));
					addcheck = true;
				}
			}
			if (dtl.Second > 0) {
				if (addcheck) {
					strout.Append(", " + dtl.Second + " " + TechF.TechFuncs.WordEndNumber(dtl.Second, "s"));
				} else {
					strout.Append(dtl.Second + " " + TechF.TechFuncs.WordEndNumber(dtl.Second, "s"));
				}
			}
			return strout.ToString();
		}



		// -- Конвертация UserInfoFull в Lite --
		public UserInfoLite UserInfoFullDownCast(UserInfoFull uifl, int ServiceId) {
			UserInfoLite uill = new UserInfoLite();
			uill.id = uifl.id;
			switch (ServiceId) {
				case 0:
					uill.Name = uifl.Name;
				break;
				case 1:
					uill.Name = uifl.TWH.DisplayName;
				break;
				case 2:
					uill.Name = uifl.Name;
					//uill.Name = uifl.VKPL.yes
				break;
			}
			uill.Color = uifl.Color;
			uill.Badges = uifl.Badges;

			return uill;
		}



		// -- Перенапрявляющая фукция отправки сообщения в чат
		public void UniversalSendMsg(int Service, string[] Users, string Message) {
			switch (Service) {
				case 0:
					TechF.Twitch.Chat.SendMsg(Users, Message);
					TechF.VKPL.Chat.SendMsg(Users, Message);
				break;

				case 1:
				TechF.Twitch.Chat.SendMsg(Users, Message);
				break;

				case 2:
				TechF.VKPL.Chat.SendMsg(Users, Message);
				break;

				default:
				break;
			}
		}

		public void UniversalSendCom(int Service, string Command) {
			switch (Service) {
				case 0:
					TechF.Twitch.Chat.SendCom(Command);
					TechF.VKPL.Chat.SendCom(Command);
				break;

				case 1:
					TechF.Twitch.Chat.SendCom(Command);
				break;

				case 2:
					TechF.VKPL.Chat.SendCom(Command);
				break;

				default:
				break;
			}
		}



		public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
			Thread.Sleep(15);
			return true;
		}

		// -- Мини-сервер, в основном для логинов --
		private void MiniServer_Start() {
			MiniServer.Start();
			string rawurl = null;
			string rawurltmp = null;
			string reqservice = null;
			string reqtype = null;
			string reqparams = null;
			//string errstr = null;
			//string errdescstr = null;
			string[] errm = null;
			string responseStr = null;


			while (true) {
				MiniServer_Context = MiniServer.GetContext();
				MiniServer_Request = MiniServer_Context.Request;
				MiniServer_Response = MiniServer_Context.Response;

				TechF.TechFuncs.LogDH("Local Authentication Server - Получен запрос: " + MiniServer_Request.Url.ToString());

				// -- Разбор прилетевших параметров
				responseStr = "";
				//errstr = null;
				//errdescstr = null;
				errm = null;
				rawurl = MiniServer_Request.RawUrl;
				rawurl = rawurl.Substring(1);
				if (rawurl.EndsWith("bg.jpg") || rawurl.EndsWith("favicon.ico")) {
					Stream outputl = MiniServer_Response.OutputStream;
					outputl.Write(new byte[] { }, 0, 0);
					outputl.Close();
					TechF.TechFuncs.LogDH("Local Authentication Server - Запрос проигнорирован");
					continue;
				}
				/*if (rawurl.EndsWith("favicon.ico")) {
					FileStream fsl = new FileStream("media/html/favicon.ico", FileMode.Open);
					byte[] array = new byte[fsl.Length];
					fsl.Read(array, 0, array.Length);
					responseStr = System.Text.Encoding.UTF8.GetString(array);
					fsl.Close();
					byte[] bufferl = System.Text.Encoding.UTF8.GetBytes(responseStr);
					MiniServer_Response.ContentLength64 = bufferl.Length;
					Stream outputl = MiniServer_Response.OutputStream;
					outputl.Write(bufferl, 0, bufferl.Length);
					outputl.Close();
					responseStr = "";
					continue;
				}*/
				reqservice = rawurl.Substring(0, rawurl.IndexOf("/"));
				rawurltmp = rawurl.Substring(rawurl.IndexOf("/") + 1);
				reqtype = rawurltmp.Substring(0, rawurltmp.IndexOf("/"));
				reqparams = rawurltmp.Substring(rawurltmp.IndexOf("/") + 1);


				switch (reqservice) {

					case "twitch":
						switch (reqtype) {
							case "auth": case "auth2":
								errm = TechF.Twitch.API.FLogin(reqparams);
							break;

							default:
								TechF.TechFuncs.LogDH("Local Authentication Server - Ошибка: неизвестный тип аунтентификации (Twitch)");
								errm = new string[1];
								errm[0] = "Ошибка: неизвестный тип аунтентификации";
							break;
						}
					break;


					case "vkpl":

					break;


					default:
						TechF.TechFuncs.LogDH("Local Authentication Server - Ошибка: неизвестный запрос");
						errm = new string[1];
						errm[0] = "Ошибка: неизвестный тип ответа";
					break;
				}


				// -- Отправка страницы пользователю. Если есть ошибка - то редачим страничку
				if (errm != null) {
					FileStream fs = new FileStream("media/html/err.html", FileMode.Open);
					byte[] array = new byte[fs.Length];
					fs.Read(array, 0, array.Length);
					string txtstr = System.Text.Encoding.UTF8.GetString(array);
					for (int i = 0; i < errm.Length; i++) {
						responseStr += "\t\t\t\t<span id=\"" + i + "\" class=\"tdef2\">" + errm[i] + "</span>\r\n\t\t\t\t<br>\r\n";
					}
					responseStr = txtstr.Replace("\t\t\t\t<span id=\"0\" class=\"tdef2\"></span>\r\n\t\t\t\t<br>\r\n", responseStr);
					fs.Close();
					Task.Factory.StartNew(() => {
						StringBuilder strlog = new StringBuilder();
						for (int j = 0; j < errm.Length; j++) { strlog.Append(errm[j]); }
						TechF.TechFuncs.LogDH("Local Authentication Server - Ошибка обработки запроса: " + strlog);
					});
				} else {
					FileStream fs = null;
					if (reqtype == "auth2") {
						fs = new FileStream("media/html/ok2.html", FileMode.Open);
					} else {
						fs = new FileStream("media/html/ok.html", FileMode.Open);
					}
					byte[] array = new byte[fs.Length];
					fs.Read(array, 0, array.Length);
					responseStr = System.Text.Encoding.UTF8.GetString(array);
					fs.Close();
					TechF.TechFuncs.LogDH("Local Server - Запрос обработан успешно");
				}

				//string responseString = "<HTML><head><meta charset=\"utf-8\"><title>Twidibot - авторизация</title></head><BODY>да</BODY></HTML>";
				byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);
				MiniServer_Response.ContentLength64 = buffer.Length;
				System.IO.Stream output = MiniServer_Response.OutputStream;
				output.Write(buffer, 0, buffer.Length);
				output.Close();
			}
		}

		private void MiniServer_Stop() {
			MiniServer.Stop();
		}

	}
}

