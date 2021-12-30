using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
//using System.Net.Sockets.Socket;
using System.Net.Http;
using Tiny.RestClient;
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
//using WebSocket4Net;
//using System.Runtime.Remoting.Contexts;

namespace Twidibot
{
	public class TechFuncs
	{
		public event EventHandler<CEvent_Msg> Ev_SendMsg;

		public string LogPath { get; set; }
		private BackWin TechF = null;
		private string ProgramPath = null;
		private List<string> MsgListtoLog = null;



		public TechFuncs(BackWin backWin) {
			this.TechF = backWin;
			Task.Factory.StartNew(() => LogDHAwait());
			this.MsgListtoLog = new List<string>();
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

			//LogPath = "F:\\twichalog_" + Date + "_" + Time + ".txt";
			//LogPath = "F:\\twicha_log.txt";
			LogPath = ProgramPath + "\\Logs\\twidilog_" + Date + "_" + Time + ".txt";
			//FileInfo fileinf = new FileInfo(LogPath);
			//fileinf.Create();
			//fileinf.Refresh();
			//fileinf = null;
			//File.Create(LogPath);
			//GC.Collect();
		}



		// -- Инициализация всего местного, кроме лога
		public void Init() {
			
		}


		// -- Логгирование --
		// -- Буфер
		private void LogDHAwait() {
			while (TechF.Work) {
				if (MsgListtoLog.Count > 0) {
					LogDHWrite(MsgListtoLog.ElementAt<string>(0));
					MsgListtoLog.RemoveAt(0);
				}
				Thread.Sleep(50);
			}
		}

		// -- Принимающая функция
		public void LogDH(string Message) {
			MsgListtoLog.Add(Message);
		}

		// -- Непосредственно запись
		private void LogDHWrite(string Msg) {
			FileStream fs = File.Open(LogPath, FileMode.Append); // -- Открываем в режиме записи в конец

			string DT = DateTime.Now.ToString();
			string Date = DT.Substring(0, 10);
			string Time = DT.Substring(11);
			byte[] array = null;

			if (Msg.EndsWith("\n")) {
				array = System.Text.Encoding.UTF8.GetBytes("(" + Date + " " + Time + ") " + Msg);
			} else {
				array = System.Text.Encoding.UTF8.GetBytes("(" + Date + " " + Time + ") " + Msg + "\n");
			}
			fs.Write(array, 0, array.Length);
			fs.Flush();
			fs.Close();
		}



		// -- Функция получения параметра настройки, чтобы не делать на месте это каждый раз и не занимать память лишней переменной --
		public string GetSettingParam(string SettingName) {
			DB.Setting_tclass Setl = TechF.db.SettingsList.Find(x => x.SetName == SettingName);
			if (Setl != null && Setl.Param != null && Setl.Param != "") {
				return Setl.Param;
			} else {
				return null;
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
		public string WordEndNumber(object Number, string TimeType, int EndType = 2) {
			string strout = null;
			try { strout = Convert.ToString(Number); } catch (Exception) { return "false"; }
			uint tim = 0;

			if (strout.StartsWith("-")) { strout = strout.Substring(1); }

			if (Convert.ToUInt64(Number) > 19) {
				strout = strout.Substring(strout.Length - 2);
				tim = Convert.ToUInt32(strout);
			} else { tim = Convert.ToUInt32(Number); }
			if (tim == 1) {
				if (TimeType == "s") { if (EndType == 1) { strout = "секунду"; } else { strout = "секунда"; } }
				if (TimeType == "m") { if (EndType == 1) { strout = "минуту"; } else { strout = "секунда"; } }
				if (TimeType == "h") { strout = "час"; }
				if (TimeType == "D") { strout = "день"; }
				if (TimeType == "M") { strout = "месяц"; }
				if (TimeType == "Y") { strout = "год"; }
			} else {
				if (tim > 1 && tim < 5) {
					if (TimeType == "s") { strout = "секунды"; }
					if (TimeType == "m") { strout = "минуты"; }
					if (TimeType == "h") { strout = "часа"; }
					if (TimeType == "D") { strout = "дня"; }
					if (TimeType == "M") { strout = "месяца"; }
					if (TimeType == "Y") { strout = "года"; }
				} else {
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




		public string SearchOfHttp(string txt_org, string str, string CharS = null, string CharE = null, bool isOther = false) {
			string txt, strout;

			txt = txt_org.Substring(txt_org.IndexOf("\"" + str + "\"") + str.Length + 2);

			if (isOther) {
				txt = txt.Substring(txt.IndexOf(CharS) + 1);
				strout = txt.Substring(0, txt.IndexOf(CharE));
			} else {
				txt = txt.Substring(txt.IndexOf("\"") + 1);
				strout = txt.Substring(0, txt.IndexOf("\""));
			}
			
			return strout;
		}
	}
}
