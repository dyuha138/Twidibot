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
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.Xml.Linq;

namespace TwidiTools
{
	public partial class DB : Window
	{
		private SQLiteConnection SQLCon = null;
		private string DBPath = null;
		private bool dbCon = false;
		private int dbPathnum = 1;

		public DB() {
			InitializeComponent();

			var process = Process.GetCurrentProcess();
			DBPath = process.MainModule.FileName;
			DBPath = DBPath.Substring(0, DBPath.LastIndexOf("\\"));
			DBPath = "F:\\Загрузки\\temp\\twidibot\\twididb.db";
			this.edbPath.Text = DBPath;
		}


		private SQLiteDataReader SQLQuery(string SQL, bool DataReceived = false) {
			SQLiteCommand Q = new SQLiteCommand(SQLCon);
			SQLiteDataReader R = null;
			Q.CommandText = SQL;

			if (DataReceived) {
				R = Q.ExecuteReader();
				R.Read();
				return R;
			} else {
				Q.ExecuteNonQuery();
				return null;
			}
		}



		private void bdbCon_Click(object sender, RoutedEventArgs e) {
			if (!dbCon) {
				DBPath = edbPath.Text;
				bdbCon.IsEnabled = false;
				bdbPathChange.IsEnabled = false;
				edbPath.IsEnabled = false;
				lStatus.Text = "Подключение...";
				lStatus.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA");
				Task.Factory.StartNew(() => {
					try {
						SQLCon = new SQLiteConnection("Data Source=" + this.DBPath);
						SQLCon.Open();
					} catch (Exception ex) {
						this.Dispatcher.Invoke(() => {
							bdbCon.IsEnabled = true;
							bdbPathChange.IsEnabled = true;
							edbPath.IsEnabled = true;
							lStatus.Text = "Ошибка подключения к базе: " + ex.Message + " | " + ex.InnerException;
							lStatus.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#FFF33535");
						});
						return;
					}
					dbCon = true;
					this.Dispatcher.Invoke(() => {
						bdbCon.IsEnabled = true;
						bdbCon.Content = "Отключиться";
						lStatus.Text = "Подключено";
						lStatus.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#FF1DEE08");
						bComsAdd.IsEnabled = true;
						bTSCAdd.IsEnabled = true;
					});
				});
			} else {
				lStatus.Text = "Отключение...";
				lStatus.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA");
				Task.Factory.StartNew(() => {
					SQLCon.Close();
					SQLCon = null;
					this.Dispatcher.Invoke(() => {
						bdbCon.IsEnabled = true;
						bdbPathChange.IsEnabled = true;
						edbPath.IsEnabled = true;
						bdbCon.Content = "Подключиться";
						lStatus.Text = "Не подключено";
						lStatus.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#FFF33535");
						bComsAdd.IsEnabled = false;
						bTSCAdd.IsEnabled = false;
					});
					dbCon = false;
				});
			}
		}


		private void bComsAdd_Click(object sender, RoutedEventArgs e) {
			bComsAdd.IsEnabled = false;
			bdbCon.IsEnabled = false;
			lOut.Text = "Работаем...";
			Task.Factory.StartNew(() => {
				SQLiteDataReader R = null;
				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='хуест';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('хуест', 'тест', 10, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!бз';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!бз', 'кто чихнул, тому будь здоров(а)', 2, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!бз2';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!бз2', 'кто болеет, тому будь здоров(а)', 2, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!ботдюхи';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!ботдюхи', 'ну я, и чё', 5, 0, 1);"); }
				
				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!ку';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!ку', 'кто приходит, тому KonCha', 2, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!ку2';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!ку2', 'кто здоровается, тому KonCha', 2, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!ку3';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!ку3', 'кто возвращается, тому KonCha', 2, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!сн';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!сн', 'кто уходит, тому KonCha', 2, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!па';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!па', 'кто ест, тому приятного', 2, 0, 1);"); }
				
				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!др';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!др', 'у кого др, того с др', 2, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!спс';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!спс', 'кто что-нибудь желает, тому спасибо', 2, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!пж';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!пж', 'кто говорит спасибо,тому пожалуйста', 2, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!пз';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!пз', 'Данные для входа на сервер зомбоида: айпи - , порт - . Пароль сервера отсутствует, а логин и пароль вы придумываете сами', 10, 0, 0);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!боты+вчат';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!боты+вчат', '+', 5, 0, 1);"); }
				
				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='+';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('+', '+', 5, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='-';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('-', '-', 5, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='±';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('±', '±', 5, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='^';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('^', '^', 5, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='^+';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('^+', '^+', 3, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='+^';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('+^', '+^', 3, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='асу';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('асу', 'осуждаем да', 1, 1, 1);"); }
				
				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='осуждаю';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('осуждаю', 'и я осуждаю да', 1, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='асуждаю';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('асуждаю', 'и я асуждаю да', 1, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='осуждаем';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('осуждаем', 'осуждаем да', 1, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='асуждаем';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('асуждаем', 'асуждаем да', 1, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='одобряю';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('одобряю', 'и я одобряю да', 1, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='одобряем';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('одобряем', 'одобряем да', 1, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!онлифанс';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!онлифанс', 'Секретное убежище: https://clck.ru/epHWR', 1, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='SMOrc';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('SMOrc', 'SMOrc SMOrc SMOrc', 3, 1, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!тг';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!тг', 'Канал: https://t.me/gruzil0 | Чат: https://t.me/GruzClub', 1, 0, 0);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!телега';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!телега', 'Канал: https://t.me/gruzil0 | Чат: https://t.me/GruzClub', 1, 0, 0);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!вклайф';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!вклайф', 'Новая Российская стриминговая платформа: https://vkplay.live/gruz', 1, 0, 0);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!вклайв';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!вклайв', 'Новая Российская стриминговая платформа: https://vkplay.live/gruz', 1, 0, 0);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!фильм';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!фильм', 'Я не знаю, какой сейчас фильм(', 3, 0, 0);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!код';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!код', ' Activison ID стримера: gruz#7847625', 3, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!вкп';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!вкп', 'Приложение VKPlay LIVE  в... Google Play: https://goo.su/UE48t | AppStore: https://goo.su/CHgf12N | RuStore: https://goo.su/lYz4cv | APK: https://goo.su/AVbb', 5, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!джейбит';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!джейбит', 'Лучшая группа об индустрии стриминга: https://vk.com/jaybeat5', 5, 0, 1);"); }

				R = SQLQuery("SELECT id FROM DefCommands WHERE Command='!дуолинго';", true);
				if (!R.HasRows) { SQLQuery("INSERT INTO DefCommands (Command, Result, CoolDown, isAlias, Enabled) VALUES ('!дуолинго', 'Давай учить иностранный язык вместе бесплатно! Duolingo — это весело и эффективно. Вот ссылка с приглашением: https://invite.duolingo.com/BDHTZTB5CWWKTOQBYCSWLSXFTU?v=if', 5, 0, 1);"); }

				this.Dispatcher.Invoke(() => {
					bComsAdd.IsEnabled = true;
					bdbCon.IsEnabled = true;
					lOut.Text = "Готово";
				});
			});
		}

		private void bTSCAdd_Click(object sender, RoutedEventArgs e) {
			bTSCAdd.IsEnabled = false;
			bdbCon.IsEnabled = false;
			lOut.Text = "Работаем...";
			Task.Factory.StartNew(() => {
				SQLiteDataReader R = null;
				//R = SQLQuery("SELECT id FROM Funcs_Timers WHERE Name='';", true);
				//if (!R.HasRows) { SQLQuery("INSERT INTO Funcs_Timers (Name, Time, Nick, DTStart, DTPauseResume, Notiflvl, Paused) VALUES ('', '', '', '', '', '', '');"); }

				//R = SQLQuery("SELECT id FROM Funcs_Stopwatches WHERE Name='';", true);
				//if (!R.HasRows) { SQLQuery("INSERT INTO Funcs_Stopwatches (Name, Nick, DTStart, DTPauseResume, Paused) VALUES ('', '', '', '', '');"); }

				//R = SQLQuery("SELECT id FROM Funcs_Counters WHERE Name='';", true);
				//if (!R.HasRows) { SQLQuery("INSERT INTO Funcs_Counters (Name, Value) VALUES ('', '');"); }


				this.Dispatcher.Invoke(() => {
					bTSCAdd.IsEnabled = true;
					bdbCon.IsEnabled = true;
					lOut.Text = "Готово";
				});
			});
		}

		private void bdbPathChanged(object sender, RoutedEventArgs e) {
			switch (dbPathnum) {
				case 1:
					dbPathnum++;
					DBPath = "F:\\Загрузки\\temp\\twidibot\\twididb.db";
					this.edbPath.Text = DBPath;
				break;

				case 2:
					dbPathnum++;
					DBPath = "F:\\Творчество (в кавычках)\\VS\\Twidibot\\twididb.db";
					this.edbPath.Text = DBPath;
				break;

				case 3:
					//dbPathnum++;
					dbPathnum = 1;
					DBPath = "S:\\twidibot\\twididb.db";
					this.edbPath.Text = DBPath;
				break;

				default:
					dbPathnum = 1;
				break;
			}
		}
	}
}
