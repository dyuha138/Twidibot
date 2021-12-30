using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Interop;

namespace LogAnalyzer
{
	public partial class MainWin : Window
	{
		public string LogsPath = null;
		public string[] LogsList = null;

		public MainWin() {
			InitializeComponent();
			
			var process = Process.GetCurrentProcess();
			LogsPath = process.MainModule.FileName;
			LogsPath = LogsPath.Substring(0, LogsPath.LastIndexOf("\\"));
			LogsPath += "\\Logs";
			LogsPath = "F:\\Творчество (в кавычках)\\VS\\Twidibot\\bin\\x64\\Debug\\Logs";
			this.eLogsPath.Text = LogsPath;
		}


		// -- Небольшая автоматизация regex-поисков --
		public string FRegexRep(string strorg, string rgxstr, string strrep = "") {
			Regex Regexstr = new Regex(rgxstr);
			return Regexstr.Replace(strorg, strrep);
		}



		// -- Обновление списка
		private void bListUpdate_Click(object sender, RoutedEventArgs e) {
			this.bListUpdate.IsEnabled = false;
			this.lOut.Text = "";
			LogsPath = this.eLogsPath.Text;
			Task.Factory.StartNew(() => {
				try {
					LogsList = Directory.GetFiles(LogsPath);
				} catch (Exception ex) {
					this.lOut.Text = ex.Message;
					return;
				}

				for (int i = 0; i < LogsList.Length; i++) {
					LogsList[i] = LogsList[i].Substring(LogsList[i].LastIndexOf("\\") + 1);
				}
				for (int i = 0; i < LogsList.Length; i++) {
					LogsList[i] = LogsList[i].Substring(0, LogsList[i].LastIndexOf("."));
				}

				this.Dispatcher.Invoke(() => this.lbLogsList.Items.Clear()); 
				for (int i = 0; i < LogsList.Length; i++) {
					this.Dispatcher.Invoke(() => this.lbLogsList.Items.Add(LogsList[i]));
				}

				this.Dispatcher.Invoke(() => this.bListUpdate.IsEnabled = true);
			});
		}




		// -- Очистка лога первого типа --
		private void bDel1_Click(object sender, RoutedEventArgs e) {
			//this.bListUpdate.IsEnabled = false;
			this.bDel1.IsEnabled = false;
			string lstr = this.lbLogsList.SelectedItem.ToString();
			Task.Factory.StartNew(() => {
				Regex Regexstr = new Regex("");
				System.Text.RegularExpressions.Match matchl = null;
				List<string> Logtxtl = new List<string>();
				List<string> Logtxtlout = new List<string>();

				// -- Считываем лог
				this.Dispatcher.Invoke(() => this.lOut.Text = "Считывание файла...");
				FileStream fs = new FileStream(LogsPath + "\\" + lstr + ".txt", FileMode.Open);
				byte[] array = new byte[fs.Length];
				fs.Read(array, 0, array.Length);
				string Logtxt = System.Text.Encoding.UTF8.GetString(array);
				fs.Close();
				fs = null;

				// -- Разделяем на строки и выводим в массив
				string[] Logtxtm = Logtxt.Split(new string[1] { "\r\n" }, StringSplitOptions.None);

				// -- Переносим всё в нормальный массив - List
				for (int i = 0; i < Logtxtm.Length; i++) {
					Logtxtl.Add(Logtxtm[i]);
				}

				// -- Удаление лишней инфы (перенос изменённой в новым массив)
				this.Dispatcher.Invoke(() => this.lOut.Text = "Удаляем лишнее...");
				for (int i = 0; i < Logtxtm.Length; i++) {
					string logstrl = Logtxtl.ElementAt<string>(i);
					if (logstrl.Contains(" PING :tmi.twitch.tv") || logstrl.Contains(" PONG :tmi.twitch.tv")) { continue; }

					logstrl = FRegexRep(logstrl, @"Принято на сокет:", ">");
					logstrl = FRegexRep(logstrl, @"Отправлено на сокет:", ">");

					Regexstr = new Regex(@"^(\(.*? .*?\) >)|^(\(.*? .*?\) <)");
					matchl = Regexstr.Match(logstrl);

					if (matchl.Success) {
						logstrl = FRegexRep(logstrl, @"client-nonce=.*?;");
						logstrl = FRegexRep(logstrl, @"color=.*?;");
						logstrl = FRegexRep(logstrl, @"emotes=.*?;");
						logstrl = FRegexRep(logstrl, @";id=.*?;", ";");
						logstrl = FRegexRep(logstrl, @"mod=.*?;", "");
						logstrl = FRegexRep(logstrl, @"room-id=.*?;", "");
						logstrl = FRegexRep(logstrl, @"subscriber=.*?;", "");
						logstrl = FRegexRep(logstrl, @"tmi-sent-ts=.*?;", "");
						logstrl = FRegexRep(logstrl, @"tmi-sent-ts=.*? ", "");
						logstrl = FRegexRep(logstrl, @"turbo=.*?;", "");
						logstrl = FRegexRep(logstrl, @"user-id=.*?;", "");
						logstrl = FRegexRep(logstrl, @"reply-parent-display-name=.*?;", "");
						logstrl = FRegexRep(logstrl, @"reply-parent-msg-body=.*?;", "");
						logstrl = FRegexRep(logstrl, @"reply-parent-msg-id=.*?;", "this-msg-reply-parent;");
						logstrl = FRegexRep(logstrl, @"reply-parent-user-id=.*?;", "");
						logstrl = FRegexRep(logstrl, @"reply-parent-user-login=.*?;", "");
						logstrl = FRegexRep(logstrl, @" :.*?@", " @");
					}
					
					Logtxtlout.Add(logstrl);
				}
				
				// -- И сохраняем новую версию
				this.Dispatcher.Invoke(() => this.lOut.Text = "Сохраняем новую версию...");
				if (File.Exists(LogsPath + "\\" + lstr + "_cleared1.txt")) {
					File.Delete(LogsPath + "\\" + lstr + "_cleared1.txt");
				}
				fs = File.Open(LogsPath + "\\" + lstr + "_cleared1.txt", FileMode.OpenOrCreate, FileAccess.Write); // -- Открываем в режиме записи в конец
				array = null;
				StringBuilder Logtxtsb = new StringBuilder(Convert.ToInt32(Logtxt.Length/1.5));
				for (int i = 0; i < Logtxtlout.Count; i++) {
					Logtxtsb.Append(Logtxtlout.ElementAt<string>(i) + "\r\n");
				}
				array = System.Text.Encoding.UTF8.GetBytes(Logtxtsb.ToString());
				fs.Write(array, 0, array.Length);
				fs.Flush();
				fs.Close();

				this.Dispatcher.Invoke(() => this.lOut.Text = "Изменённая копия сохранена в той же папке с тем же именем с припиской \"_cleared1\"");
				this.Dispatcher.Invoke(() => this.bDel1.IsEnabled = true);
			});
		}




		// -- Очистка лога второго типа --
		private void bDel2_Click(object sender, RoutedEventArgs e) {
			//this.bListUpdate.IsEnabled = false;
			this.bDel2.IsEnabled = false;
			string lstr = this.lbLogsList.SelectedItem.ToString();
			Task.Factory.StartNew(() => {
				Regex Regexstr = new Regex("");
				System.Text.RegularExpressions.Match matchl = null;
				List<string> Logtxtl = new List<string>();
				List<string> Logtxtlout = new List<string>();

				// -- Считываем лог
				this.Dispatcher.Invoke(() => this.lOut.Text = "Считывание файла...");
				FileStream fs = new FileStream(LogsPath + "\\" + lstr + ".txt", FileMode.Open);
				byte[] array = new byte[fs.Length];
				fs.Read(array, 0, array.Length);
				string Logtxt = System.Text.Encoding.UTF8.GetString(array);
				fs.Close();
				fs = null;

				// -- Разделяем на строки и выводим в массив
				string[] Logtxtm = Logtxt.Split(new string[1] { "\r\n" }, StringSplitOptions.None);

				// -- Переносим всё в нормальный массив - List
				for (int i = 0; i < Logtxtm.Length; i++) {
					Logtxtl.Add(Logtxtm[i]);
				}

				// -- Удаление лишней инфы (перенос изменённой в новым массив)
				this.Dispatcher.Invoke(() => this.lOut.Text = "Удаляем лишнее...");
				for (int i = 0; i < Logtxtm.Length; i++) {
					string logstrl = Logtxtl.ElementAt<string>(i);
					if (logstrl.Contains(" PING :tmi.twitch.tv") || logstrl.Contains(" PONG :tmi.twitch.tv") || logstrl.Contains(".tmi.twitch.tv JOIN") || logstrl.Contains(".tmi.twitch.tv PART")) { continue; }

					logstrl = FRegexRep(logstrl, @"Принято на сокет:", ">");
					logstrl = FRegexRep(logstrl, @"Отправлено на сокет:", ">");

					Regexstr = new Regex(@"^(\(.*? .*?\) >)|^(\(.*? .*?\) <)");
					matchl = Regexstr.Match(logstrl);

					if (matchl.Success) {
						logstrl = FRegexRep(logstrl, @"client-nonce=.*?;");
						logstrl = FRegexRep(logstrl, @"color=.*?;");
						logstrl = FRegexRep(logstrl, @"emotes=.*?;");
						logstrl = FRegexRep(logstrl, @";id=.*?;", ";");
						logstrl = FRegexRep(logstrl, @"mod=.*?;", "");
						logstrl = FRegexRep(logstrl, @"room-id=.*?;", "");
						logstrl = FRegexRep(logstrl, @"subscriber=.*?;", "");
						logstrl = FRegexRep(logstrl, @"tmi-sent-ts=.*?;", "");
						logstrl = FRegexRep(logstrl, @"tmi-sent-ts=.*? ", "");
						logstrl = FRegexRep(logstrl, @"turbo=.*?;", "");
						logstrl = FRegexRep(logstrl, @"user-id=.*?;", "");
						logstrl = FRegexRep(logstrl, @"reply-parent-display-name=.*?;", "");
						logstrl = FRegexRep(logstrl, @"reply-parent-msg-body=.*?;", "");
						logstrl = FRegexRep(logstrl, @"reply-parent-msg-id=.*?;", "this-msg-reply-parent;");
						logstrl = FRegexRep(logstrl, @"reply-parent-user-id=.*?;", "");
						logstrl = FRegexRep(logstrl, @"reply-parent-user-login=.*?;", "");
						logstrl = FRegexRep(logstrl, @" :.*?@", " @");
					}

					Logtxtlout.Add(logstrl);
				}

				// -- И сохраняем новую версию
				this.Dispatcher.Invoke(() => this.lOut.Text = "Сохраняем новую версию...");
				if (File.Exists(LogsPath + "\\" + lstr + "_cleared2.txt")) {
					File.Delete(LogsPath + "\\" + lstr + "_cleared2.txt");
				}
				fs = File.Open(LogsPath + "\\" + lstr + "_cleared2.txt", FileMode.OpenOrCreate, FileAccess.Write); // -- Открываем в режиме записи в конец
				array = null;
				StringBuilder Logtxtsb = new StringBuilder(Convert.ToInt32(Logtxt.Length / 1.5));
				for (int i = 0; i < Logtxtlout.Count; i++) {
					Logtxtsb.Append(Logtxtlout.ElementAt<string>(i) + "\r\n");
				}
				array = System.Text.Encoding.UTF8.GetBytes(Logtxtsb.ToString());
				fs.Write(array, 0, array.Length);
				fs.Flush();
				fs.Close();

				this.Dispatcher.Invoke(() => this.lOut.Text = "Изменённая копия сохранена в той же папке с тем же именем с припиской \"_cleared2\"");
				this.Dispatcher.Invoke(() => this.bDel2.IsEnabled = true);
			});
		}




		// -- Очистка лога третьего типа --
		private void bDel3_Click(object sender, RoutedEventArgs e) {
			//this.bListUpdate.IsEnabled = false;
			this.bDel3.IsEnabled = false;
			string lstr = this.lbLogsList.SelectedItem.ToString();
			Task.Factory.StartNew(() => {
				Regex Regexstr = new Regex("");
				System.Text.RegularExpressions.Match matchl = null;
				List<string> Logtxtl = new List<string>();
				List<string> Logtxtlout = new List<string>();

				// -- Считываем лог
				this.Dispatcher.Invoke(() => this.lOut.Text = "Считывание файла...");
				FileStream fs = new FileStream(LogsPath + "\\" + lstr + ".txt", FileMode.Open);
				byte[] array = new byte[fs.Length];
				fs.Read(array, 0, array.Length);
				string Logtxt = System.Text.Encoding.UTF8.GetString(array);
				fs.Close();
				fs = null;

				// -- Разделяем на строки и выводим в массив
				string[] Logtxtm = Logtxt.Split(new string[1] { "\r\n" }, StringSplitOptions.None);

				// -- Переносим всё в нормальный массив - List
				for (int i = 0; i < Logtxtm.Length; i++) {
					Logtxtl.Add(Logtxtm[i]);
				}

				// -- Удаление лишней инфы (перенос изменённой в новым массив)
				this.Dispatcher.Invoke(() => this.lOut.Text = "Удаляем лишнее...");
				for (int i = 0; i < Logtxtm.Length; i++) {
					string logstrl = Logtxtl.ElementAt<string>(i);
					if (logstrl.Contains(" PING :tmi.twitch.tv") || logstrl.Contains(" PONG :tmi.twitch.tv") || logstrl.Contains(".tmi.twitch.tv JOIN") || logstrl.Contains(".tmi.twitch.tv PART")) { continue; }

					logstrl = FRegexRep(logstrl, @"Принято на сокет:", ">");
					logstrl = FRegexRep(logstrl, @"Отправлено на сокет:", ">");

					Regexstr = new Regex(@"^(\(.*? .*?\) >)|^(\(.*? .*?\) <)");
					matchl = Regexstr.Match(logstrl);

					if (matchl.Success) {
						logstrl = FRegexRep(logstrl, @"badge-info=.*?;");
						logstrl = FRegexRep(logstrl, @"badges=.*?;");
						logstrl = FRegexRep(logstrl, @"client-nonce=.*?;");
						logstrl = FRegexRep(logstrl, @"color=.*?;");
						logstrl = FRegexRep(logstrl, @"emotes=.*?;");
						logstrl = FRegexRep(logstrl, @"first-msg=.*?;");
						logstrl = FRegexRep(logstrl, @"flags=.*?;");
						logstrl = FRegexRep(logstrl, @";id=.*?;", ";");
						logstrl = FRegexRep(logstrl, @"mod=.*?;", "");
						logstrl = FRegexRep(logstrl, @"room-id=.*?;", "");
						logstrl = FRegexRep(logstrl, @"subscriber=.*?;", "");
						logstrl = FRegexRep(logstrl, @"tmi-sent-ts=.*?;", "");
						logstrl = FRegexRep(logstrl, @"tmi-sent-ts=.*? ", "");
						logstrl = FRegexRep(logstrl, @"turbo=.*?;", "");
						logstrl = FRegexRep(logstrl, @"user-id=.*?;", "");
						logstrl = FRegexRep(logstrl, @"reply-parent-display-name=.*?;", "");
						logstrl = FRegexRep(logstrl, @"reply-parent-msg-body=.*?;", "");
						logstrl = FRegexRep(logstrl, @"reply-parent-msg-id=.*?;", "this-msg-reply-parent;");
						logstrl = FRegexRep(logstrl, @"reply-parent-user-id=.*?;", "");
						logstrl = FRegexRep(logstrl, @"reply-parent-user-login=.*?;", "");
						logstrl = FRegexRep(logstrl, @" :.*?@", " @");
						logstrl = FRegexRep(logstrl, @" #.*? :", " :");
					}

					Logtxtlout.Add(logstrl);
				}

				// -- И сохраняем новую версию
				this.Dispatcher.Invoke(() => this.lOut.Text = "Сохраняем новую версию...");
				if (File.Exists(LogsPath + "\\" + lstr + "_cleared3.txt")) {
					File.Delete(LogsPath + "\\" + lstr + "_cleared3.txt");
				}
				fs = File.Open(LogsPath + "\\" + lstr + "_cleared3.txt", FileMode.OpenOrCreate, FileAccess.Write); // -- Открываем в режиме записи в конец
				array = null;
				StringBuilder Logtxtsb = new StringBuilder(Convert.ToInt32(Logtxt.Length / 1.5));
				for (int i = 0; i < Logtxtlout.Count; i++) {
					Logtxtsb.Append(Logtxtlout.ElementAt<string>(i) + "\r\n");
				}
				array = System.Text.Encoding.UTF8.GetBytes(Logtxtsb.ToString());
				fs.Write(array, 0, array.Length);
				fs.Flush();
				fs.Close();

				this.Dispatcher.Invoke(() => this.lOut.Text = "Изменённая копия сохранена в той же папке с тем же именем с припиской \"_cleared3\"");
				this.Dispatcher.Invoke(() => this.bDel2.IsEnabled = true);
			});
		}
	}
}
