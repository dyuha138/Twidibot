using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using static Twidibot.DB;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
//using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using System.Windows.Forms;
//using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Diagnostics;
using System.Xml.Linq;
using System.Data.SqlTypes;
using System.Runtime.Remoting.Messaging;
using System.Windows.Interop;

namespace Twidibot1
{
	public sealed class DB
	{
		private SQLiteConnection DBCon = null;
		private SQLiteCommand DBQ = null;
		//private SQLiteDataReader dbR = null;

		public string DBName = null;
		private Twidibot.TechFuncs TechFuncs = null;
		//private ObservableCollection<string> SQLList = new ObservableCollection<string>();


		// -- Копии таблиц --
		public readonly List<Setting_tclass> SettingsList = new List<Setting_tclass>();
		public readonly List<DefCommand_tclass> DefCommandsList = new List<DefCommand_tclass>();
		public readonly List<FuncCommand_tclass> FuncCommandsList = new List<FuncCommand_tclass>();
		public readonly List<SpamMessage_tclass> SpamMessagesList = new List<SpamMessage_tclass>();
		//public readonly List<ChatHistory_tclass> ChatHistoryList = new List<ChatHistory_tclass>();
		//public readonly List<ChangeLog_tclass> ChangeLogList = new List<ChangeLog_tclass>();
		public readonly List<Funcs_Timer_tclass> TimersList = new List<Funcs_Timer_tclass>();
		public readonly List<Funcs_Stopwatch_tclass> StopwatchesList = new List<Funcs_Stopwatch_tclass>();
		public readonly List<Funcs_Counter_tclass> CountersList = new List<Funcs_Counter_tclass>();

		// -- Экземляры классов для работы с таблицами --
		public SettingsTC SettingsT = null;
		public DefCommandsTC DefCommandsT = null;
		public FuncCommandsTC FuncCommandsT = null;
		public SpamMessagesTC SpamMessagesT = null;
		public ChatHistoryTC ChatHistoryT = null;
		//public ChangeLogTC ChangeLogT = null;
		public Funcs_TimersTC Funcs_TimersT = null;
		public Funcs_StopwatchesTC Funcs_StopwatchesT = null;
		public Funcs_CountersTC Funcs_CountersT = null;


		// -- Инициализация --
		public DB(Twidibot.TechFuncs techFuncs) {
			this.TechFuncs = techFuncs;
			this.DBName = "twididb.db";
			DBCon = new SQLiteConnection("Data Source=" + this.DBName);
			//DBCon = new SqliteConnection("Data Source=" + this.DBName);
			DBQ = new SQLiteCommand(DBCon);
			SettingsT = new SettingsTC(this);
			DefCommandsT = new DefCommandsTC(this);
			FuncCommandsT = new FuncCommandsTC(this);
			SpamMessagesT = new SpamMessagesTC(this);
			ChatHistoryT = new ChatHistoryTC(this);
			//ChangeLogT = new ChangeLogTC(this);
			Funcs_TimersT = new Funcs_TimersTC(this);
			Funcs_StopwatchesT = new Funcs_StopwatchesTC(this);
			Funcs_CountersT = new Funcs_CountersTC(this);
		}



		/*public DB(TechFuncs techFuncs, string DBName) {
			this.TechFuncs = techFuncs;
			this.DBName = DBName;
			DBCon = new SQLiteConnection("Data Source=" + this.DBName);
			DBQ = new SQLiteCommand(DBCon);
			SettingsT = new SettingsTC();
			DefCommandsT = new DefCommandsTC(this);
			FuncCommandsT = new FuncCommandsTC(this);
			SpamMessagesT = new SpamMessagesTC(this);
			ChatHistoryT = new ChatHistoryTC(this);
			//ChangeLogT = new ChangeLogTC(this);
			Funcs_TimersT = new Funcs_TimersTC(this);
			Funcs_StopwatchesT = new Funcs_StopwatchesTC(this);
			Funcs_CountersT = new Funcs_CountersTC(this);
		}*/

		public void Open() {
			DBCon.Open();
		}
		public void Close() {
			DBCon.Close();
		}



		// -- Обновление всех списков  --
		public void AllListsUpdate() {
			this.SettingsT.UpdateListfromTable();
			this.DefCommandsT.UpdateListfromTable();
			this.FuncCommandsT.UpdateListfromTable();
			this.SpamMessagesT.UpdateListfromTable();
			this.Funcs_TimersT.UpdateListfromTable();
			this.Funcs_StopwatchesT.UpdateListfromTable();
			this.Funcs_CountersT.UpdateListfromTable();
		}


		// -- Главная функция для выполнения запросов --
		public SQLiteDataReader SQLQuery(string SQL, bool isRead) {
			SQLiteDataReader R = null;
			DBQ.CommandText = SQL;
			R = DBQ.ExecuteReader();
			//R.Close();
			//DBQ.CommandText = "";

			if (R.HasRows) {
				if (isRead) { R.Read(); }
				return R;
			} else {
				R.Close();
				return null;
			}
		}

		public int SQLQuery(string SQL) {
			SQLiteDataReader R = null;
			DBQ.CommandText = SQL;
			int i = DBQ.ExecuteNonQuery();
			//DBQ.CommandText = "";
			return i;
		}



		// -- Класс для работы с таблицей Settings --
		public sealed class SettingsTC {
			private DB db = null;
			public SettingsTC(DB db) { this.db = db; }

			public bool UpdateSetting(string SettingName, string Parameter) {
				int i = db.SettingsList.FindIndex(x => x.SetName == SettingName);
				Setting_tclass Setl = db.SettingsList.ElementAt(i);
				if (Setl != null) {
					if (Parameter == null) {
						db.SQLQuery("UPDATE Settings SET Param = null WHERE SetName='" + SettingName + "';");
					} else {
						db.SQLQuery("UPDATE Settings SET Param = '" + Parameter + "' WHERE SetName='" + SettingName + "';");
					}

					db.SettingsList.RemoveAt(i);
					Setl.Param = Parameter;
					db.SettingsList.Insert(i, Setl);
					db.TechFuncs.LogDH("Изменён параметр \"" + SettingName + "\"");
					return true;
				} else {
					return false;
				}
			}

			public string GetParam(string SettingName) {
				SQLiteDataReader R = null;
				string sout = null;
				R = db.SQLQuery("SELECT Param FROM Settings WHERE SetName = '" + SettingName + "';", false);
				if (R != null) {
					try { sout = R.GetString(0); } catch (Exception) { }
				}
				if (!R.IsClosed) { R.Close(); }
				return sout;
			}

			public void UpdateListfromTable() {
				SQLiteDataReader R = db.SQLQuery("SELECT * FROM Settings;", false);
				if (R != null) {
					db.SettingsList.Clear();
					string p1 = null;
					while (R.Read()) {
						p1 = null;
						try { p1 = R.GetString(2); } catch (Exception) { }
						db.SettingsList.Add(new Setting_tclass(R.GetInt32(0), R.GetString(1), p1, Convert.ToBoolean(R.GetInt32(3))));
					}
					if (!R.IsClosed) { R.Close(); }
				}
			}
			public void UpdateListfromTable2() {
				SQLiteCommand Q = new SQLiteCommand(db.DBCon);
				Q.CommandText = "SELECT * FROM Settings;";
				SQLiteDataReader R = Q.ExecuteReader();
				if (R != null) {
					db.SettingsList.Clear();
					string p1 = null;
					while (R.Read()) {
						p1 = null;
						try { p1 = R.GetString(2); } catch (Exception) { }
						db.SettingsList.Add(new Setting_tclass(R.GetInt32(0), R.GetString(1), p1, Convert.ToBoolean(R.GetInt32(3))));
					}
					if (!R.IsClosed) { R.Close(); }
				}
			}
		}



		// -- Класс для работы с таблицей DefCommands --
		public sealed class DefCommandsTC {
			private DB db = null;
			public DefCommandsTC(DB db) { this.db = db; }

			public int Add(DefCommand_tclass DefComl) {
				StringBuilder sql = new StringBuilder("INSERT INTO DefCommands (Command, Result", 130);
				SQLiteDataReader R = null;
				if (DefComl.CoolDown > 0) { sql.Append(", CoolDown"); }
				sql.Append(", Enabled");
				if (DefComl.LastUsed != null && DefComl.LastUsed != "") { sql.Append(", LastUsed"); }

				sql.Append(") VALUES ('" + DefComl.Command + "', '" + DefComl.Result + "'");

				if (DefComl.CoolDown > 0) { sql.Append(", '" + DefComl.CoolDown + "'"); }
				sql.Append(", '" + Convert.ToInt32(DefComl.Enabled) + "'");
				if (DefComl.LastUsed != null && DefComl.LastUsed != "") { sql.Append(", " + DefComl.LastUsed + "'"); }
				sql.Append(");  SELECT last_insert_rowid();");

				R = db.SQLQuery(sql.ToString(), true);

				db.DefCommandsList.Add(new DefCommand_tclass(R.GetInt32(0), DefComl.Command, DefComl.Result, DefComl.CoolDown, DefComl.Enabled, DefComl.LastUsed));
				db.TechFuncs.LogDH("Добавлена команда \"" + DefComl.Command + "\"");
				int i = R.GetInt32(0);
				if (!R.IsClosed) { R.Close(); }
				return i;
			}

			public DefCommand_tclass Find(string Command) {
				SQLiteDataReader R = null;
				DefCommand_tclass DefComl = new DefCommand_tclass();
				R = db.SQLQuery("SELECT * FROM DefCommands WHERE Command = '" + Command + ";", true);
				if (R != null) {
					DefComl.id = R.GetInt32(0);
					DefComl.Command = R.GetString(1);
					DefComl.Result = R.GetString(2);
					try { DefComl.CoolDown = R.GetInt32(3); } catch (Exception) { }
					DefComl.Enabled = Convert.ToBoolean(R.GetInt32(4));
					try { DefComl.LastUsed = R.GetString(5); } catch (Exception) { }
					if (!R.IsClosed) { R.Close(); }
				}
				return DefComl;
			}

			public bool Remove(int id) {
				int val = db.SQLQuery("DELETE FROM DefCommands WHERE id = '" + id + "';");
				if (val > 0) {
					int i = db.DefCommandsList.FindIndex(x => x.id == id);
					string str = db.DefCommandsList.ElementAt(i).Command;
					db.DefCommandsList.RemoveAt(i);
					db.TechFuncs.LogDH("Удалена команда \"" + str + "\"");
					return true;
				} else {
					return false;
				}
			}

			public bool RowUpdate(DefCommand_tclass DefComl) {
				int i = db.DefCommandsList.FindIndex(x => x.id == DefComl.id);
				DefCommand_tclass DefComll = db.DefCommandsList.ElementAt(i);
				if (DefComll != null) {
					db.SQLQuery("UPDATE DefCommands SET Command = '" + DefComl.Command + "', Result = '" + DefComl.Result + "', CoolDown = '" + DefComl.CoolDown + "' WHERE id = '" + DefComl.id + "';");

					db.DefCommandsList.RemoveAt(i);
					db.DefCommandsList.Insert(i, DefComl);
					if (DefComl.Command != DefComll.Command) {
						db.TechFuncs.LogDH("Изменена команда \"" + DefComll.Command + "\" (\"" + DefComl.Command + "\")");
					} else {
						db.TechFuncs.LogDH("Изменена команда \"" + DefComll.Command + "\"");
					}
					return true;
				} else {
					return false;
				}
			}

			public bool EnabledUpdate(int id, bool Enabled) {
				int i = db.DefCommandsList.FindIndex(x => x.id == id);
				DefCommand_tclass DefComl = db.DefCommandsList.ElementAt(i);
				if (DefComl != null) {
					db.SQLQuery("UPDATE DefCommands SET Enabled = '" + Convert.ToInt32(Enabled) + "' WHERE id = '" + id + "';");

					db.DefCommandsList.RemoveAt(i);
					DefComl.Enabled = Enabled;
					db.DefCommandsList.Insert(i, DefComl);
					db.TechFuncs.LogDH("Изменена активность команды \"" + DefComl.Command + "\"");
					return true;
				} else {
					return false;
				}
			}

			public bool UpdateLastUsed(int id, string DateTime) {
				int i = db.DefCommandsList.FindIndex(x => x.id == id);
				DefCommand_tclass DefComl = db.DefCommandsList.ElementAt(i);
				if (DefComl != null) {
					db.SQLQuery("UPDATE DefCommands SET LastUsed='" + DateTime + "' WHERE id='" + id + "';");

					db.DefCommandsList.RemoveAt(i);
					DefComl.LastUsed = DateTime;
					db.DefCommandsList.Insert(i, DefComl);
					return true;
				} else {
					return false;
				}
			}

			public void UpdateListfromTable() {
				SQLiteDataReader R = db.SQLQuery("SELECT * FROM DefCommands;", false);
				if (R != null) {
					db.DefCommandsList.Clear();
					int p1 = 0;
					string p2 = null;
					while (R.Read()) {
						p1 = 0; p2 = null;
						try { p1 = R.GetInt32(3); } catch (Exception) { }
						try { p2 = R.GetString(5); } catch (Exception) { }
						db.DefCommandsList.Add(new DefCommand_tclass(R.GetInt32(0), R.GetString(1), R.GetString(2), p1, Convert.ToBoolean(R.GetInt32(4)), p2));
					}
					if (!R.IsClosed) { R.Close(); }
				}
			}
		}



		// -- Класс для работы с таблицей FuncCommands --
		public sealed class FuncCommandsTC {
			private DB db = null;
			public FuncCommandsTC(DB db) { this.db = db; }

			/*public int Add(FuncCommand_tclass FuncComl) {
				StringBuilder sql = new StringBuilder("INSERT INTO FuncCommands (Command, FuncName", 150);
				SQLiteDataReader R = null;

				if (FuncComl.Desc != null && FuncComl.Desc != "") { sql.Append(", Desc"); }
				if (FuncComl.Params != null && FuncComl.Params != "") { sql.Append(", Params"); }
				if (FuncComl.CoolDown > 0) { sql.Append(", CoolDown"); }
				sql.Append(", Secured, Enabled");
				if (FuncComl.LastUsed != null && FuncComl.LastUsed != "") { sql.Append(", LastUsed"); }

				sql.Append(") VALUES ('" + FuncComl.Command + "', '" + FuncComl.FuncName + "'");

				if (FuncComl.Desc != null && FuncComl.Desc != "") { sql.Append(", '" + FuncComl.Desc + "'"); }
				if (FuncComl.Params != null && FuncComl.Params != "") { sql.Append(", '" + FuncComl.Params + "'"); }
				if (FuncComl.CoolDown > 0) { sql.Append(", '" + FuncComl.CoolDown + "'"); }
				sql.Append(", '" + Convert.ToInt32(FuncComl.Secured) + "', '" + Convert.ToInt32(FuncComl.Enabled) + "'");
				if (FuncComl.LastUsed != null && FuncComl.LastUsed != "") { sql.Append(", " + FuncComl.LastUsed + "'"); }
				sql.Append(");  SELECT last_insert_rowid();");

				R = db.SQLQuery(sql.ToString(), true);

				db.FuncCommandsList.Add(new FuncCommand_tclass(R.GetInt32(0), FuncComl.Command, FuncComl.FuncName, FuncComl.Desc, FuncComl.Params, FuncComl.CoolDown, FuncComl.Secured, FuncComl.Enabled, FuncComl.LastUsed));
				int i = R.GetInt32(0);
				if (!R.IsClosed) { R.Close(); }
				return i;
			}*/

			public FuncCommand_tclass Find(string Command) {
				SQLiteDataReader R = null;
				FuncCommand_tclass FuncComl = new FuncCommand_tclass();
				R = db.SQLQuery("SELECT * FROM FuncCommands WHERE Command = '" + Command + ";", true);
				if (R != null) {
					FuncComl.id = R.GetInt32(0);
					FuncComl.Command = R.GetString(1);
					FuncComl.FuncName = R.GetString(2);
					try { FuncComl.Desc = R.GetString(3); } catch (Exception) { }
					try { FuncComl.Params = R.GetString(4); } catch (Exception) { }
					try { FuncComl.CoolDown = R.GetInt32(5); } catch (Exception) { }
					FuncComl.Secured = Convert.ToBoolean(R.GetInt32(6));
					FuncComl.Enabled = Convert.ToBoolean(R.GetInt32(7));
					try { FuncComl.LastUsed = R.GetString(8); } catch (Exception) { }
					if (!R.IsClosed) { R.Close(); }
				}
				return FuncComl;
			}

			public bool UpdateLastUsed(int id, string DateTime) {
				int i = db.FuncCommandsList.FindIndex(x => x.id == id);
				FuncCommand_tclass FuncComl = db.FuncCommandsList.ElementAt(i);
				if (FuncComl != null) {
					db.SQLQuery("UPDATE FuncCommands SET LastUsed='" + DateTime + "' WHERE id='" + id + "';");

					db.FuncCommandsList.RemoveAt(i);
					FuncComl.LastUsed = DateTime;
					db.FuncCommandsList.Insert(i, FuncComl);
					return true;
				} else {
					return false;
				}
			}

			public bool RowUpdate(FuncCommand_tclass FuncComl) {
				int i = db.FuncCommandsList.FindIndex(x => x.id == FuncComl.id);
				FuncCommand_tclass FuncComll = db.FuncCommandsList.ElementAt(i);
				if (FuncComll != null) {
					db.SQLQuery("UPDATE FuncCommands SET Command = '" + FuncComl.Command + "', CoolDown = '" + FuncComl.CoolDown + "' WHERE id = '" + FuncComl.id + "';");

					db.FuncCommandsList.RemoveAt(i);
					db.FuncCommandsList.Insert(i, FuncComl);
					if (FuncComl.Command != FuncComll.Command) {
						db.TechFuncs.LogDH("Изменена встроенная команда \"" + FuncComll.Command + "\" (\"" + FuncComl.Command + "\")");
					} else {
						db.TechFuncs.LogDH("Изменена встроенная команда \"" + FuncComll.Command + "\" (кулдаун)");
					}
					return true;
				} else {
					return false;
				}
			}

			public bool EnabledUpdate(int id, bool Enabled) {
				int i = db.FuncCommandsList.FindIndex(x => x.id == id);
				FuncCommand_tclass FuncComl = db.FuncCommandsList.ElementAt(i);
				if (FuncComl != null) {
					db.SQLQuery("UPDATE FuncCommands SET Enabled = '" + Convert.ToInt32(Enabled) + "' WHERE id = '" + id + "';");

					db.FuncCommandsList.RemoveAt(i);
					FuncComl.Enabled = Enabled;
					db.FuncCommandsList.Insert(i, FuncComl);
					db.TechFuncs.LogDH("Изменена активность встроенной команды \"" + FuncComl.Command + "\"");
					return true;
				} else {
					return false;
				}
			}

			public void UpdateListfromTable() {
				SQLiteDataReader R = db.SQLQuery("SELECT * FROM FuncCommands;", false);
				if (R != null) {
					db.FuncCommandsList.Clear();
					string p1 = null, p2 = null, p4 = null;
					int p3 = 0;
					while (R.Read()) {
						p1 = null; p2 = null; p3 = 0; p4 = null;
						try { p1 = R.GetString(3); } catch (Exception) { }
						try { p2 = R.GetString(4); } catch (Exception) { }
						try { p3 = R.GetInt32(5); } catch (Exception) { }
						try { p4 = R.GetString(8); } catch (Exception) { }
						db.FuncCommandsList.Add(new FuncCommand_tclass(R.GetInt32(0), R.GetString(1), R.GetString(2), p1, p2, p3, Convert.ToBoolean(R.GetInt32(6)), Convert.ToBoolean(R.GetInt32(7)), p4));
					}
					if (!R.IsClosed) { R.Close(); }
				}
			}
		}



		// -- Класс для работы с таблицей SpamMessages --
		public sealed class SpamMessagesTC {
			private DB db = null;
			public SpamMessagesTC(DB db) { this.db = db; }

			public int Add(SpamMessage_tclass SpamMsgl) {
				StringBuilder sql = new StringBuilder("INSERT INTO SpamMessages (Message", 120);
				SQLiteDataReader R = null;
				if (SpamMsgl.CoolDown > 0) { sql.Append(", CoolDown"); }
				sql.Append(", Enabled");
				if (SpamMsgl.LastUsed != null && SpamMsgl.LastUsed != "") { sql.Append(", LastUsed"); }

				sql.Append(") VALUES ('" + SpamMsgl.Message + "'");

				if (SpamMsgl.CoolDown > 0) { sql.Append(", '" + SpamMsgl.CoolDown + "'"); }
				sql.Append(", '" + Convert.ToInt32(SpamMsgl.Enabled) + "'");
				if (SpamMsgl.LastUsed != null && SpamMsgl.LastUsed != "") { sql.Append(", " + SpamMsgl.LastUsed + "'"); }
				sql.Append(");  SELECT last_insert_rowid();");

				R = db.SQLQuery(sql.ToString(), true);

				db.SpamMessagesList.Add(new SpamMessage_tclass(R.GetInt32(0), SpamMsgl.Message, SpamMsgl.CoolDown, SpamMsgl.Enabled, SpamMsgl.LastUsed));
				db.TechFuncs.LogDH("Добавлено спам-сообщение \"" + SpamMsgl.Message + "\"");
				int i = R.GetInt32(0);
				if (!R.IsClosed) { R.Close(); }
				return i;
			}

			public SpamMessage_tclass Find(string Message) {
				SQLiteDataReader R = null;
				SpamMessage_tclass SpamMsgl = new SpamMessage_tclass();
				R = db.SQLQuery("SELECT * FROM SpamMessages WHERE Message = '" + Message + ";", true);
				if (R != null) {
					SpamMsgl.id = R.GetInt32(0);
					SpamMsgl.Message = R.GetString(1);
					SpamMsgl.CoolDown = R.GetInt32(2);
					SpamMsgl.Enabled = Convert.ToBoolean(R.GetInt32(3));
					if (!R.IsClosed) { R.Close(); }
				}
				return SpamMsgl;
			}

			public bool Remove(int id) {
				int val = db.SQLQuery("DELETE FROM SpamMessages WHERE id = '" + id + "';");
				if (val > 0) {
					int i = db.SpamMessagesList.FindIndex(x => x.id == id);
					SpamMessage_tclass SpamMsgl = db.SpamMessagesList.ElementAt(i);
					db.SpamMessagesList.RemoveAt(i);
					db.TechFuncs.LogDH("Удалено спам-сообщение \"" + SpamMsgl.Message + "\"");
					return true;
				} else {
					return false;
				}
			}

			public bool UpdateLastUsed(int id, string DateTime) {
				int i = db.SpamMessagesList.FindIndex(x => x.id == id);
				SpamMessage_tclass SpamMsgl = db.SpamMessagesList.ElementAt(i);
				if (SpamMsgl != null) {
					db.SQLQuery("UPDATE SpamMessages SET LastUsed='" + DateTime + "' WHERE id='" + id + "';");

					db.SpamMessagesList.RemoveAt(i);
					SpamMsgl.LastUsed = DateTime;
					db.SpamMessagesList.Insert(i, SpamMsgl);
					return true;
				} else {
					return false;
				}
			}

			public bool RowUpdate(SpamMessage_tclass SpamMsgl) {
				int i = db.SpamMessagesList.FindIndex(x => x.id == SpamMsgl.id);
				SpamMessage_tclass SpamMsgll = db.SpamMessagesList.ElementAt(i);
				if (SpamMsgll != null) {
					db.SQLQuery("UPDATE SpamMessages SET Message = '" + SpamMsgl.Message + "', CoolDown = '" + SpamMsgl.CoolDown + "' WHERE id = '" + SpamMsgl.id + "';");

					db.SpamMessagesList.RemoveAt(i);
					db.SpamMessagesList.Insert(i, SpamMsgl);
					if (SpamMsgl.Message != SpamMsgll.Message) {
						db.TechFuncs.LogDH("Изменено спам-сообщение \"" + SpamMsgll.Message + "\" (\"" + SpamMsgl.Message + "\")");
					} else {
						db.TechFuncs.LogDH("Изменено спам-сообщение \"" + SpamMsgll.Message + "\" (кулдаун)");
					}
					return true;
				} else {
					return false;
				}
			}

			public bool EnabledUpdate(int id, bool Enabled) {
				int i = db.SpamMessagesList.FindIndex(x => x.id == id);
				SpamMessage_tclass SpamMsgl = db.SpamMessagesList.ElementAt(i);
				if (SpamMsgl != null) {
					db.SQLQuery("UPDATE SpamMessages SET Enabled = '" + Convert.ToInt32(Enabled) + "' WHERE id = '" + id + "';");

					db.SpamMessagesList.RemoveAt(i);
					SpamMsgl.Enabled = Enabled;
					db.SpamMessagesList.Insert(i, SpamMsgl);
					db.TechFuncs.LogDH("Изменена активность спам-сообщения \"" + SpamMsgl.Message + "\"");
					return true;
				} else {
					return false;
				}
			}

			public void UpdateListfromTable() {
				SQLiteDataReader R = db.SQLQuery("SELECT * FROM SpamMessages;", false);
				if (R != null) {
					db.SpamMessagesList.Clear();
					int p1 = 0;
					string p2 = null;
					while (R.Read()) {
						try { p1 = R.GetInt32(2); } catch (Exception) { }
						try { p2 = R.GetString(4); } catch (Exception) { }
						db.SpamMessagesList.Add(new SpamMessage_tclass(R.GetInt32(0), R.GetString(1), p1, Convert.ToBoolean(R.GetInt32(3)), p2));
					}
					if (!R.IsClosed) { R.Close(); }
				}
			}
		}



		// -- Класс для работы с таблицей ChatHistory --
		public sealed class ChatHistoryTC {
			private DB db = null;
			private List<string> sqladdmsg = new List<string>();
			private bool addmsg = false;
			public ChatHistoryTC(DB db) {
				this.db = db;
				//Task.Factory.StartNew(() => this.Transact());
			}

			public void Add(ChatHistory_tclass ChatHstrl) {
				StringBuilder sql = new StringBuilder("INSERT INTO ChatHistory (Userid", 120);
				if (ChatHstrl.Msgid != null || ChatHstrl.Msgid != "") { sql.Append(", Msgid"); }
				sql.Append(", Nick, Msg, Date, Time, Color, isOwner, isDel, isMod, isVIP, isSub");
				if (ChatHstrl.BadgeInfo != null || ChatHstrl.BadgeInfo != "") { sql.Append(", BadgeInfo"); }
				if (ChatHstrl.Badges != null || ChatHstrl.Badges != "") { sql.Append(", Badges"); }

				sql.Append(") VALUES ('" + ChatHstrl.Userid + "'");

				if (ChatHstrl.Msgid != null || ChatHstrl.Msgid != "") { sql.Append(", '" + ChatHstrl.Msgid + "'"); }
				sql.Append(", '" + ChatHstrl.Nick + "', '" + ChatHstrl.Msg + "', '" + ChatHstrl.Date + "', '" + ChatHstrl.Time + "', '" + ChatHstrl.Color + "', '" + Convert.ToInt32(ChatHstrl.isOwner) + "', '" + Convert.ToInt32(ChatHstrl.isMod) + "', '" + Convert.ToInt32(ChatHstrl.isVIP) + "', '" + Convert.ToInt32(ChatHstrl.isSub) + "'");
				if (ChatHstrl.BadgeInfo != null || ChatHstrl.BadgeInfo != "") { sql.Append(", '" + ChatHstrl.BadgeInfo + "'"); }
				if (ChatHstrl.Badges != null || ChatHstrl.Badges != "") { sql.Append(", '" + ChatHstrl.Badges + "'"); }
				//sql.Append(");  SELECT last_insert_rowid();");
				sql.Append(");");


				//db.ChatHistoryList.Add(new ChatHistory_tclass(R.GetInt32(0), ChatHstrl.Userid, ChatHstrl.Msgid, ChatHstrl.Nick, ChatHstrl.Msg, ChatHstrl.Date, ChatHstrl.Time, ChatHstrl.isOwner, ChatHstrl.isDel, ChatHistory.isMod, ChatHistory.isVIP, ChatHistory.isSub, ChatHstrl.BadgeInfo, ChatHstrl.Badges));

				sqladdmsg.Add(sql.ToString());
			}

			public bool MsgDeleted(string Msgid) {
				int val = db.SQLQuery("UPDATE ChatHistory SET isDel = '1' WHERE Msgid = '" + Msgid + "';");
				if (val > 0) {
					return true;
				} else {
					return false;
				}

				//i = db.ChatHistoryList.FindIndex(x => x.Msgid == Msgid);
				//ChatHstrl = db.ChatHistoryList.Find(x => x.Msgsid == Msgid);
				//db.ChatHistoryList.RemoveAt(i);
				//ChatHstrl = Enabled;
				//db.ChatHistoryList.Insert(i, ChatHstrl);
			}
			public bool MsgDeleted(int id) {
				int val = db.SQLQuery("UPDATE ChatHistory SET isDel = '1' WHERE id = '" + id + "';");
				if (val > 0) {
					return true;
				} else {
					return false;
				}

				//i = db.ChatHistoryList.FindIndex(x => x.id == id);
				//ChatHstrl = db.ChatHistoryList.Find(x => x.id == id);
				//db.ChatHistoryList.RemoveAt(i);
				//ChatHstrl = Enabled;
				//db.ChatHistoryList.Insert(i, ChatHstrl);
			}

			/*public void Remove(string Message) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.DBCon);
				sql = "DELETE FROM ChatHistory WHERE Message = '" + Message + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				//db.ChatHistoryList.Remove(db.ChatHistoryList.Find(x => x.Message == Message));
			}*/
			public bool Remove(int id) {
				int val = db.SQLQuery("DELETE FROM ChatHistory WHERE id = '" + id + "';");
				if (val > 0) {
					return true;
				} else {
					return false;
				}

				//db.ChatHistoryList.Remove(db.ChatHistoryList.Find(x => x.id == id));
			}

			private void UpdateListfromTable() {
				SQLiteDataReader R = db.SQLQuery("SELECT * FROM ChatHistory;", false);
				if (R != null) {
					//db.ChatHistoryList.Clear();
					while (R.Read()) {
						//db.ChatHistoryList.Add(new ChatHistory_tclass(R.GetInt32(0), R.GetString(1), R.GetString(2), R.GetString(3), R.GetString(4), Convert.ToBoolean(R.GetInt32(5)), Convert.ToBoolean(R.GetInt32(6)), Convert.ToBoolean(R.GetInt32(7)), Convert.ToBoolean(R.GetInt32(8))));
					}
				}
			}

			private void Transact() {
				string sqlstr = "";
				while (true) {
					Thread.Sleep(3000);
					if (sqladdmsg.Count > 0) {
						for (ushort i = 0; i < sqladdmsg.Count; i++) {
							sqlstr += sqladdmsg.ElementAt(i);
						}
						sqladdmsg.Clear();
						db.SQLQuery(sqlstr);
						sqlstr = "";
					}
				}
			}
		}



		// -- Класс для работы с таблицей Funcs_Timers --
		public sealed class Funcs_TimersTC {
			private DB db = null;
			public Funcs_TimersTC(DB db) { this.db = db; }

			public int Add(Funcs_Timer_tclass Timerl) {
				SQLiteDataReader R = db.SQLQuery("INSERT INTO Funcs_Timers (Name, Time, Nick, DTStart, ChannelNotif, Paused) VALUES ('" + Timerl.Name + "', '" + Timerl.Time + "', '" + Timerl.Nick + "', '" + Timerl.DTStart + "', '" + Convert.ToInt32(Timerl.ChannelNotif) + "', '" + Convert.ToInt32(Timerl.Paused) + "'); SELECT last_insert_rowid();", true);

				db.TimersList.Add(new Funcs_Timer_tclass(R.GetInt32(0), Timerl.Name, Timerl.Time, Timerl.Nick, Timerl.DTStart, Timerl.DTPauseResume, Timerl.Paused));
				db.TechFuncs.LogDH("Добавлен таймер \"" + Timerl.Name + "\"");
				int i = R.GetInt32(0);
				if (!R.IsClosed) { R.Close(); }
				return i;
			}

			public bool Remove(string Name) {
				int val = db.SQLQuery("DELETE FROM Funcs_Timers WHERE Name = '" + Name + "';");
				if (val > 0) {
					db.TimersList.RemoveAt(db.TimersList.FindIndex(x => x.Name == Name));
					db.TechFuncs.LogDH("Удалён таймер \"" + Name + "\"");
					return true;
				} else {
					return false;
				}
			}

			public bool PauseUpdate(string Name, bool Paused) {
				int i = db.TimersList.FindIndex(x => x.Name == Name);
				Funcs_Timer_tclass Timerl = db.TimersList.ElementAt(i);
				string DTstr = DateTime.Now.ToString();
				string dtprorg = Timerl.DTPauseResume;

				if (Timerl != null) {
					// -- Т.к. да, делаем логику обработки дат начала и конца паузы тут, хоть это немного и не надёжно --
					if (Timerl.Paused != Paused) {
						if (Paused) {
							db.SQLQuery("UPDATE Funcs_Timers SET DTPauseResume = '" + dtprorg + DTstr + ",', Paused = '1' WHERE Name = '" + Name + "';");
						} else {
							db.SQLQuery("UPDATE Funcs_Timers SET DTPauseResume = '" + dtprorg + DTstr + ";', Paused = '0' WHERE Name = '" + Name + "';");
						}

						db.TimersList.RemoveAt(i);
						Timerl.Paused = Paused;
						Timerl.DTPauseResume = dtprorg + DTstr;
						db.TimersList.Insert(i, Timerl);
						db.TechFuncs.LogDH("Изменена активность таймера \"" + Timerl.Name + "\"");
						return true;
					} else { return false; }
				} else { return false; }
			}

			public bool Restart(string Name) {
				string DTPR = null;
				string DT = null;
				int i = db.TimersList.FindIndex(x => x.Name == Name);
				Funcs_Timer_tclass Timerl = db.TimersList.ElementAt(i);
				if (Timerl != null) {
					if (Timerl.DTPauseResume != null && Timerl.DTPauseResume != "") {
						DTPR = Timerl.DTPauseResume.Substring(Timerl.DTPauseResume.LastIndexOf(';') + 1);
					} else {
						DTPR = "";
					}
					DT = DateTime.Now.ToString();

					if (DTPR.Length > 2) {
						DTPR = DTPR.Substring(1, DTPR.IndexOf(','));
						db.SQLQuery("UPDATE Funcs_Timers SET DTPauseResume = '" + DTPR + "', DTStart = '" + DT + "' WHERE id = '" + Timerl.id + "';");
						Timerl.DTPauseResume = DTPR;
					} else {
						db.SQLQuery("UPDATE Funcs_Timers SET DTStart = '" + DT + "' WHERE id = '" + Timerl.id + "';");
					}

					db.TimersList.RemoveAt(i);
					Timerl.DTStart = DT;
					db.TimersList.Insert(i, Timerl);
					db.TechFuncs.LogDH("Сброшен таймер \"" + Timerl.Name + "\"");
					return true;
				} else {
					return false;
				}
			}

			public bool UpdateTime(string Name, ulong Time) {
				int i = db.TimersList.FindIndex(x => x.Name == Name);
				Funcs_Timer_tclass Timerl = db.TimersList.ElementAt(i);
				int val = db.SQLQuery("UPDATE Funcs_Timers SET Time = '" + Time.ToString() + "' WHERE Name = '" + Name + "';");
				if (val > 0) {
					db.TimersList.RemoveAt(i);
					Timerl.Time = Time;
					db.TimersList.Insert(i, Timerl);
					db.TechFuncs.LogDH("Обновлён таймер \"" + Name + "\"");
					return true;
				} else {
					return false;
				}
			}

			public void UpdateListfromTable() {
				SQLiteDataReader R = db.SQLQuery("SELECT * FROM Funcs_Timers;", false);
				if (R != null) {
					db.TimersList.Clear();
					while (R.Read()) {
						db.TimersList.Add(new Funcs_Timer_tclass(R.GetInt32(0), R.GetString(1), Convert.ToUInt64(R.GetString(2)), R.GetString(3), R.GetString(4), R.GetString(5), Convert.ToBoolean(R.GetInt32(6)), Convert.ToBoolean(R.GetInt32(7))));
					}
					if (!R.IsClosed) { R.Close(); }
				}
			}
		}



		// -- Класс для работы с таблицей Funcs_Stopwatches --
		public sealed class Funcs_StopwatchesTC {
			private DB db = null;
			public Funcs_StopwatchesTC(DB db) {	this.db = db; }

			public int Add(Funcs_Stopwatch_tclass Stopwatchl) {
				StringBuilder sql = new StringBuilder("INSERT INTO Funcs_Stopwatches (Name", 130);
				SQLiteDataReader R = null;
				if (Stopwatchl.Nick != null & Stopwatchl.Nick != "") { sql.Append(", Nick"); }
				sql.Append(", DTStart, Paused) VALUES ('" + Stopwatchl.Name + "'");

				if (Stopwatchl.Nick != null & Stopwatchl.Nick != "") { sql.Append(", '" + Stopwatchl.Nick + "'"); }
				sql.Append(", '" + Stopwatchl.DTStart + "', '" + Convert.ToInt32(Stopwatchl.Paused) + "'); SELECT last_insert_rowid();");

				R = db.SQLQuery(sql.ToString(), true);

				db.StopwatchesList.Add(new Funcs_Stopwatch_tclass(R.GetInt32(0), Stopwatchl.Name, Stopwatchl.Nick, Stopwatchl.DTStart, Stopwatchl.DTPauseResume, Stopwatchl.Paused));
				db.TechFuncs.LogDH("Добавлен секундомер \"" + Stopwatchl.Name + "\"");
				int i = R.GetInt32(0);
				if (!R.IsClosed) { R.Close(); }
				return i;
			}

			public bool Remove(string Name) {
				int val = db.SQLQuery("DELETE FROM Funcs_Stopwatches WHERE Name = '" + Name + "';");
				if (val > 0) {
					db.StopwatchesList.RemoveAt(db.StopwatchesList.FindIndex(x => x.Name == Name));
					db.TechFuncs.LogDH("Удалён секундомер \"" + Name + "\"");
					return true;
				} else {
					return false;
				}
			}

			public bool PauseUpdate(string Name, bool Paused) {
				int i = db.StopwatchesList.FindIndex(x => x.Name == Name);
				Funcs_Stopwatch_tclass Stopwatchl = db.StopwatchesList.Find(x => x.Name == Name);
				string DTstr = DateTime.Now.ToString();
				string dtprorg = Stopwatchl.DTPauseResume;

				// -- Т.к. да, делаем логику обработки дат начала и конца паузы тут, хоть это немного и не надёжно, но с другой стороны... --
				if (Stopwatchl != null && Stopwatchl.Paused == !Paused) {
					if (Paused) {
						db.SQLQuery("UPDATE Funcs_Stopwatches SET DTPauseResume = '" + dtprorg + DTstr + ",', Paused = '1' WHERE Name = '" + Name + "';");
					} else {
						db.SQLQuery("UPDATE Funcs_Stopwatches SET DTPauseResume = '" + dtprorg + DTstr + ";', Paused = '0' WHERE Name = '" + Name + "';");
					}

					db.StopwatchesList.RemoveAt(i);
					Stopwatchl.Paused = Paused;
					Stopwatchl.DTPauseResume = dtprorg + DTstr;
					db.StopwatchesList.Insert(i, Stopwatchl);
					db.TechFuncs.LogDH("Изменена активность секундомера \"" + Stopwatchl.Name + "\"");
					return true;
				} else {
					return false;
				}
			}

			public bool Restart(string Name) {
				string DTPR = null;
				string DT = null;
				int i = db.StopwatchesList.FindIndex(x => x.Name == Name);
				Funcs_Stopwatch_tclass Stopwatchl = db.StopwatchesList.ElementAt(i);
				if (Stopwatchl != null) {
					if (Stopwatchl.DTPauseResume != null && Stopwatchl.DTPauseResume != "") {
						DTPR = Stopwatchl.DTPauseResume.Substring(Stopwatchl.DTPauseResume.LastIndexOf(';') + 1);
					} else {
						DTPR = "";
					}
					DT = DateTime.Now.ToString();

					if (DTPR.Length > 2) {
						DTPR = DTPR.Substring(1, DTPR.IndexOf(','));
						db.SQLQuery("UPDATE Funcs_Stopwatches SET DTPauseResume = '" + DTPR + "', DTStart = '" + DT + "' WHERE id = '" + Stopwatchl.id + "';");
						Stopwatchl.DTPauseResume = DTPR;
					} else {
						db.SQLQuery("UPDATE Funcs_Stopwatches SET DTStart = '" + DT + "' WHERE id = '" + Stopwatchl.id + "';");
					}

					db.StopwatchesList.RemoveAt(i);
					Stopwatchl.DTStart = DT;
					db.StopwatchesList.Insert(i, Stopwatchl);
					db.TechFuncs.LogDH("Сброшен секундомер \"" + Stopwatchl.Name + "\"");
					return true;
				} else {
					return false;
				}
			}

			public void UpdateListfromTable() {
				SQLiteDataReader R = db.SQLQuery("SELECT * FROM Funcs_Stopwatches;", false);
				if (R != null) {
					db.StopwatchesList.Clear();
					string p1 = null;
					while (R.Read()) {
						p1 = null;
						try { p1 = R.GetString(4); } catch (Exception) { }
						db.StopwatchesList.Add(new Funcs_Stopwatch_tclass(R.GetInt32(0), R.GetString(1), R.GetString(2), R.GetString(3), p1, Convert.ToBoolean(R.GetInt32(5))));
					}
					if (!R.IsClosed) { R.Close(); }
				}
			}
		}



		// -- Класс для работы с таблицей Funcs_Counters --
		public sealed class Funcs_CountersTC {
			private DB db = null;
			public Funcs_CountersTC(DB db) { this.db = db; }

			public int Add(Funcs_Counter_tclass Counterl) {
				SQLiteDataReader R = null;
				R = db.SQLQuery("INSERT INTO Funcs_Counters(Name, Value) VALUES ('" + Counterl.Name + "', '" + Counterl.Value + "'); SELECT last_insert_rowid();", true);
				
				db.CountersList.Add(new Funcs_Counter_tclass(R.GetInt32(0), Counterl.Name, Counterl.Value));
				db.TechFuncs.LogDH("Добавлен счётчик \"" + Counterl.Name + "\"");
				int i = R.GetInt32(0);
				if (!R.IsClosed) { R.Close(); }
				return i;
			}

			public bool Remove(string Name) {
				int val = db.SQLQuery("DELETE FROM Funcs_Counters WHERE Name = '" + Name + "';");
				if (val > 0) {
					db.CountersList.RemoveAt(db.CountersList.FindIndex(x => x.Name == Name));
					db.TechFuncs.LogDH("Удалён счётчик \"" + Name + "\"");
					return true;
				} else {
					return false;
				}
			}

			public bool Update(string Name, int Value) {
				int i = db.CountersList.FindIndex(x => x.Name == Name);
				Funcs_Counter_tclass Counterl = db.CountersList.ElementAt(i);
				if (Counterl != null) {
					db.SQLQuery("UPDATE Funcs_Counters SET Value = '" + Value + "' WHERE id = '" + Counterl.id + "';");

					db.CountersList.RemoveAt(i);
					Counterl.Value = Value;
					db.CountersList.Insert(i, Counterl);
					return true;
				} else {
					return false;
				}
			}

			public bool Plus(string Name) {
				int i = db.CountersList.FindIndex(x => x.Name == Name);
				Funcs_Counter_tclass Counterl = db.CountersList.ElementAt(i);
				if (Counterl != null) {
					Counterl.Value += 1;
					db.SQLQuery("UPDATE Funcs_Counters SET Value = '" + Counterl.Value + "' WHERE id = '" + Counterl.id + "';");

					db.CountersList.RemoveAt(i);
					db.CountersList.Insert(i, Counterl);
					return true;
				} else {
					return false;
				}
			}

			public bool Minus(string Name) {
				int i = db.CountersList.FindIndex(x => x.Name == Name);
				Funcs_Counter_tclass Counterl = db.CountersList.ElementAt(i);
				if (Counterl != null) {
					Counterl.Value -= 1;
					db.SQLQuery("UPDATE Funcs_Counters SET Value = '" + Counterl.Value + "' WHERE id = '" + Counterl.id + "';");

					db.CountersList.RemoveAt(i);
					db.CountersList.Insert(i, Counterl);
					return true;
				} else {
					return false;
				}
			}

			public bool Restart(string Name) {
				int i = db.CountersList.FindIndex(x => x.Name == Name);
				Funcs_Counter_tclass Counterl = db.CountersList.ElementAt(i);
				if (Counterl != null) {
					db.SQLQuery("UPDATE Funcs_Counters SET Value = '0' WHERE id = '" + Counterl.id + "';");

					db.CountersList.RemoveAt(i);
					Counterl.Value = 0;
					db.CountersList.Insert(i, Counterl);
					return true;
				} else {
					return false;
				}
			}

			public void UpdateListfromTable() {
				SQLiteDataReader R = db.SQLQuery("SELECT * FROM Funcs_Counters;", false);
				if (R != null) {
					db.CountersList.Clear();
					while (R.Read()) {
						db.CountersList.Add(new Funcs_Counter_tclass(R.GetInt32(0), R.GetString(1), R.GetInt32(2)));
					}
					if (!R.IsClosed) { R.Close(); }
				}
			}
		}





		// -- Классы таблиц --
		// -- Settings
		public class Setting_tclass {
			public int id { get; set; }
			public string SetName { get; set; }
			public string Param { get; set; }
			public bool Secured { get; set; }

			public Setting_tclass() { }
			public Setting_tclass(string Setting, string Parameter = null, bool Secured = false) {
				this.SetName = Setting;
				this.Param = Parameter;
				this.Secured = Secured;
			}
			public Setting_tclass(int id, string Setting, string Parameter = null, bool Secured = false) {
				this.id = id;
				this.SetName = Setting;
				this.Param = Parameter;
				this.Secured = Secured;
			}
		}


		// -- DefCommand
		public class DefCommand_tclass {
			public int id { get; set; }
			public string Command { get; set; }
			public string Result { get; set; }
			public int CoolDown { get; set; }
			public bool Enabled { get; set; }
			public string LastUsed { get; set; }

			public DefCommand_tclass() { }
			public DefCommand_tclass(string Command, string Result, int CoolDown = 0, bool Enabled = true, string LastUsed = null) {
				this.Command = Command;
				this.Result = Result;
				this.CoolDown = CoolDown;
				this.Enabled = Enabled;
				this.LastUsed = LastUsed;
			}
			public DefCommand_tclass(int id, string Command, string Result, int CoolDown = 0, bool Enabled = true, string LastUsed = null) {
				this.id = id;
				this.Command = Command;
				this.Result = Result;
				this.CoolDown = CoolDown;
				this.Enabled = Enabled;
				this.LastUsed = LastUsed;
			}
		}


		// -- FuncCommand
		public class FuncCommand_tclass {
			public int id { get; set; }
			public string Command { get; set; }
			public string FuncName { get; set; }
			public string Desc { get; set; }
			public string Params { get; set; }
			public int CoolDown { get; set; }
			public bool Enabled { get; set; }
			public bool Secured { get; set; }
			public string LastUsed { get; set; }

			public FuncCommand_tclass() { }
			public FuncCommand_tclass(string Command, string FuncName, string Desc = null, string Params = null, int CoolDown = 0, bool Enabled = true, bool Secured = false, string LastUsed = null) {
				this.Command = Command;
				this.FuncName = FuncName;
				this.Desc = Desc;
				this.Params = Params;
				this.CoolDown = CoolDown;
				this.Enabled = Enabled;
				this.Secured = Secured;
				this.LastUsed = LastUsed;
			}
			public FuncCommand_tclass(int id, string Command, string FuncName, string Desc = null, string Params = null, int CoolDown = 0, bool Enabled = true, bool Secured = false, string LastUsed = null) {
				this.id = id;
				this.Command = Command;
				this.FuncName = FuncName;
				this.Desc = Desc;
				this.Params = Params;
				this.CoolDown = CoolDown;
				this.Enabled = Enabled;
				this.Secured = Secured;
				this.LastUsed = LastUsed;
			}
		}


		// -- SpamMessages
		public class SpamMessage_tclass {
			public int id { get; set; }
			public string Message { get; set; }
			public int CoolDown { get; set; }
			public bool Enabled { get; set; }
			public string LastUsed { get; set; }

			public SpamMessage_tclass() { }
			public SpamMessage_tclass(string Message, int CoolDown = 0, bool Enabled = true, string LastUsed = null) {
				this.Message = Message;
				this.CoolDown = CoolDown;
				this.Enabled = Enabled;
				this.LastUsed = LastUsed;
			}
			public SpamMessage_tclass(int id, string Message, int CoolDown = 0, bool Enabled = true, string LastUsed = null) {
				this.id = id;
				this.Message = Message;
				this.CoolDown = CoolDown;
				this.Enabled = Enabled;
				this.LastUsed = LastUsed;
			}
		}


		// -- ChatHistory
		public class ChatHistory_tclass {
			public int id { get; set; }
			public int Userid { get; set; }
			public string Msgid { get; set; }
			public string Nick { get; set; }
			public string Msg { get; set; }
			public string Date { get; set; }
			public string Time { get; set; }
			public string Color { get; set; }
			public bool isOwner { get; set; }
			public bool isDel { get; set; }
			public bool isMod { get; set; }
			public bool isVIP { get; set; }
			public bool isSub { get; set; }
			public string BadgeInfo { get; set; }
			public string Badges { get; set; }

			public ChatHistory_tclass() { }
			public ChatHistory_tclass(int Userid, string Msgid, string Nick, string Msg, string Date, string Time, string Color, bool Owner = false, bool Deleted = false, bool Moderator = false, bool VIP = false, bool Subscriber = false, string BadgeInfo = null, string Badges = null) {
				this.Userid = Userid;
				this.Msgid = Msgid;
				this.Nick = Nick;
				this.Msg = Msg;
				this.Date = Date;
				this.Time = Time;
				this.Color = Color;
				this.isOwner = Owner;
				this.isDel = Deleted;
				this.isMod = Moderator;
				this.isVIP = VIP;
				this.isSub = Subscriber;
				this.BadgeInfo = BadgeInfo;
				this.Badges = Badges;
			}
			public ChatHistory_tclass(int id, int Userid, string Msgid, string Nick, string Msg, string Date, string Time, string Color, bool Owner = false, bool Deleted = false, bool Moderator = false, bool VIP = false, bool Subscriber = false, string BadgeInfo = null, string Badges = null) {
				this.id = id;
				this.Userid = Userid;
				this.Msgid = Msgid;
				this.Nick = Nick;
				this.Msg = Msg;
				this.Date = Date;
				this.Time = Time;
				this.Color = Color;
				this.isOwner = Owner;
				this.isDel = Deleted;
				this.isMod = Moderator;
				this.isVIP = VIP;
				this.isSub = Subscriber;
				this.BadgeInfo = BadgeInfo;
				this.Badges = Badges;
			}
		}


		// -- ChangeLog
		public class ChangeLog_tclass {
			public int id { get; set; }
			public string Version { get; set; }
			public string Desc { get; set; }

			public ChangeLog_tclass() { }
			public ChangeLog_tclass(string Version, string Desc) {
				this.Version = Version;
				this.Desc = Desc;
			}
			public ChangeLog_tclass(int id, string Version, string Desc) {
				this.id = id;
				this.Version = Version;
				this.Desc = Desc;
			}
		}

		// -- Funcs_Timers
		public class Funcs_Timer_tclass {
			public int id { get; set; }
			public string Name { get; set; }
			public ulong Time { get; set; }
			public string Nick { get; set; }
			public string DTStart { get; set; }
			public string DTPauseResume { get; set; }
			public bool ChannelNotif { get; set; }
			public bool Paused { get; set; }

			public Funcs_Timer_tclass() { }
			public Funcs_Timer_tclass(string Name, ulong Time, string Nick, string DTStart, string DTPauseResume = null, bool ChannelNotif = false, bool Paused = false) {
				this.Name = Name;
				this.Time = Time;
				this.Nick = Nick;
				this.DTStart = DTStart;
				this.DTPauseResume = DTPauseResume;
				this.ChannelNotif = ChannelNotif;
				this.Paused = Paused;
			}

			public Funcs_Timer_tclass(int id, string Name, ulong Time, string Nick, string DTStart, string DTPauseResume = null, bool ChannelNotif = false, bool Paused = false) {
				this.id = id;
				this.Name = Name;
				this.Time = Time;
				this.Nick = Nick;
				this.DTStart = DTStart;
				this.DTPauseResume = DTPauseResume;
				this.ChannelNotif = ChannelNotif;
				this.Paused = Paused;
			}
		}


		// -- Funcs_Stopwatches
		public class Funcs_Stopwatch_tclass {
			public int id { get; set; }
			public string Name { get; set; }
			public string Nick { get; set; }
			public string DTStart { get; set; }
			public string DTPauseResume { get; set; }
			public bool Paused { get; set; }

			public Funcs_Stopwatch_tclass() { }
			public Funcs_Stopwatch_tclass(string Name, string Nick, string DTStart, string DTPauseResume = null, bool Paused = false) {
				this.Name = Name;
				this.Nick = Nick;
				this.DTStart = DTStart;
				this.DTPauseResume = DTPauseResume;
				this.Paused = Paused;
			}

			public Funcs_Stopwatch_tclass(int id, string Name, string Nick, string DTStart, string DTPauseResume = null, bool Paused = false) {
				this.id = id;
				this.Name = Name;
				this.Nick = Nick;
				this.DTStart = DTStart;
				this.DTPauseResume = DTPauseResume;
				this.Paused = Paused;
			}
		}


		// -- Funcs_Counters
		public class Funcs_Counter_tclass {
			public int id { get; set; }
			public string Name { get; set; }
			public int Value { get; set; }

			public Funcs_Counter_tclass() { }
			public Funcs_Counter_tclass(string Name, int Value = 0) {
				this.Name = Name;
				this.Value = Value;
			}

			public Funcs_Counter_tclass(int id, string Name, int Value = 0) {
				this.id = id;
				this.Name = Name;
				this.Value = Value;
			}
		}
	}
}
