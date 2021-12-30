using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Twidibot.DB;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using Twidibot.Pages;
using System.Windows.Forms;
//using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Diagnostics;
using System.Xml.Linq;
using System.Data.SqlTypes;

namespace Twidibot
{
	public class DB
	{
		private SQLiteConnection SQLCon = null;
		//private SQLiteCommand Query = null;
		//private SQLiteDataReader QRes = null;
		public string DBName { get; set; }

		private ObservableCollection<string> SQLList = new ObservableCollection<string>();

		// -- Таблицы --
		public List<Setting_tclass> SettingsList = new List<Setting_tclass>();
		public List<DefCommand_tclass> DefCommandsList = new List<DefCommand_tclass>();
		public List<FuncCommand_tclass> FuncCommandsList = new List<FuncCommand_tclass>();
		public List<SpamMessage_tclass> SpamMessagesList = new List<SpamMessage_tclass>();
		//public List<ChatHistory_tclass> ChatHistoryList = new List<ChatHistory_tclass>();
		//public List<ChangeLog_tclass> ChangeLogList = new List<ChangeLog_tclass>();
		public List<Funcs_Timer_tclass> TimersList = new List<Funcs_Timer_tclass>();
		public List<Funcs_Stopwatch_tclass> StopwatchesList = new List<Funcs_Stopwatch_tclass>();
		public List<Funcs_Counter_tclass> CountersList = new List<Funcs_Counter_tclass>();

		// -- Поля для работы непосредственно с таблицами --
		public SettingsTC SettingsT = null;
		public DefCommandsTC DefCommandsT = null;
		public FuncCommandsTC FuncCommandsT = null;
		public SpamMessagesTC SpamMessagesT = null;
		public ChatHistoryTC ChatHistoryT = null;
		//public ChangeLogTC ChangeLogT = null;
		public Funcs_TimersTC Funcs_TimersT = null;
		public Funcs_StopwatchesTC Funcs_StopwatchesT = null;
		public Funcs_CountersTC Funcs_CountersT = null;


		// -- Конструкторы --
		public DB() {
			this.DBName = "twidibot.db";
			System.Data.ConnectionState state = new System.Data.ConnectionState();
			SQLCon = new SQLiteConnection("Data Source=" + this.DBName);
			state = SQLCon.State;
			//SQLCon = new SqliteConnection("Data Source=" + this.DBName);
			//Query = new SQLiteCommand(SQLCon);
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



		public DB(string DBName) {
			this.DBName = DBName;
			SQLCon = new SQLiteConnection("Data Source=" + this.DBName);
			//Query = new SQLiteCommand(SQLCon);
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

		public void Open() {
			SQLCon.Open();
		}
		public void Close() {
			SQLCon.Close();
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


		

		// -- Класс для работы с таблицей Settings --
		public class SettingsTC {
			private DB db = null;
			public SettingsTC(DB db) {
				this.db = db;
			}

			public void UpdateSetting(string SettingName, string Parameter) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				DB.Setting_tclass Setl = null;
				sql = "UPDATE Settings SET Param = '" + Parameter + "' WHERE SetName='" + SettingName + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.SettingsList.FindIndex(x => x.SetName == SettingName);
				Setl = db.SettingsList.Find(x => x.SetName == SettingName);
				db.SettingsList.RemoveAt(i);
				Setl.Param = Parameter;
				db.SettingsList.Insert(i, Setl);
			}

			public string GetParam(string SettingName) {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				string sout = null;
				Q.CommandText = "SELECT Param FROM Settings WHERE SetName = '" + SettingName + "';";
				R = Q.ExecuteReader();
				if (R.HasRows) {
					R.Read();
					try { sout = R.GetString(0); } catch (Exception) { }
				}
				return sout;
			}

			public void UpdateListfromTable() {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				Q.CommandText = "SELECT * FROM Settings;";
				R = Q.ExecuteReader();
				if (R.HasRows) {
					db.SettingsList.Clear();
					string p1 = null;
					while (R.Read()) {
						p1 = null;
						try { p1 = R.GetString(2); } catch (Exception) { }
						db.SettingsList.Add(new Setting_tclass(R.GetInt32(0), R.GetString(1), p1));
					}
				}
			}
		}



		// -- Класс для работы с таблицей DefCommands --
		public class DefCommandsTC {
			private DB db = null;
			public DefCommandsTC(DB db) {
				this.db = db;
			}

			public int Add(DefCommand_tclass DefComl) {
				StringBuilder sql = new StringBuilder("INSERT INTO DefCommands (Command, Result", 130);
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				if (DefComl.CoolDown > 0) { sql.Append(", CoolDown"); }
				sql.Append(", Enabled");
				if (DefComl.LastUsed != null && DefComl.LastUsed != "") { sql.Append(", LastUsed"); }

				sql.Append(") VALUES ('" + DefComl.Command + "', '" + DefComl.Result + "'");

				if (DefComl.CoolDown > 0) { sql.Append(", '" + DefComl.CoolDown + "'"); }
				sql.Append(", '" + Convert.ToInt32(DefComl.Enabled) + "'");
				if (DefComl.LastUsed != null && DefComl.LastUsed != "") { sql.Append(", " + DefComl.LastUsed + "'"); }
				sql.Append(");  SELECT last_insert_rowid();");

				Q.CommandText = sql.ToString();
				R = Q.ExecuteReader();
				R.Read();

				db.DefCommandsList.Add(new DefCommand_tclass(R.GetInt32(0), DefComl.Command, DefComl.Result, DefComl.CoolDown, DefComl.Enabled, DefComl.LastUsed));

				return R.GetInt32(0);
			}

			public DefCommand_tclass Find(string Command) {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				DefCommand_tclass sout = new DefCommand_tclass();
				Q.CommandText = "SELECT * FROM DefCommands WHERE Command = '" + Command + ";";
				R = Q.ExecuteReader();
				if (R.HasRows) {
					R.Read();
					sout.id = R.GetInt32(0);
					sout.Command = R.GetString(1);
					sout.Result = R.GetString(2);
					try { sout.CoolDown = R.GetInt32(3); } catch (Exception) { }
					sout.Enabled = Convert.ToBoolean(R.GetInt32(4));
					try { sout.LastUsed = R.GetString(5); } catch (Exception) { }
				}
				return sout;
			}

			public bool Remove(string Command) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				DB.DefCommand_tclass DefComl = db.DefCommandsList.Find(x => x.Command == Command);
				if (DefComl != null) {
					db.DefCommandsList.Remove(DefComl);
					sql = "DELETE FROM DefCommands WHERE Command = '" + Command + "';";
					Q.CommandText = sql;
					Q.ExecuteNonQuery();
					return true;
				} else {
					return false;
				}
			}
			public bool Remove(int id) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				DB.DefCommand_tclass DefComl = db.DefCommandsList.Find(x => x.id == id);
				if (DefComl != null) {
					db.DefCommandsList.Remove(DefComl);
					sql = "DELETE FROM DefCommands WHERE id = '" + id + "';";
					Q.CommandText = sql;
					Q.ExecuteNonQuery();
					return true;
				} else {
					return false;
				}
			}

			public void RowUpdate(DefCommand_tclass DefComl) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				sql = "UPDATE DefCommands SET Command = '" + DefComl.Command + "', Result = '" + DefComl.Result + "', CoolDown = '" + DefComl.CoolDown + "' WHERE id = '" + DefComl.id + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.DefCommandsList.FindIndex(x => x.id == DefComl.id);
				db.DefCommandsList.RemoveAt(i);
				db.DefCommandsList.Insert(i, DefComl);
			}

			public void EnabledUpdate(string Command, bool Enabled) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				DefCommand_tclass DefComl = null;
				sql = "UPDATE DefCommands SET Enabled = '" + Convert.ToInt32(Enabled) + "' WHERE Command = '" + Command + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.DefCommandsList.FindIndex(x => x.Command == Command);
				DefComl = db.DefCommandsList.Find(x => x.Command == Command);
				db.DefCommandsList.RemoveAt(i);
				db.DefCommandsList.Insert(i, DefComl);
			}
			public void EnabledUpdate(int id, bool Enabled) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				DefCommand_tclass DefComl = null;
				sql = "UPDATE DefCommands SET Enabled = '" + Convert.ToInt32(Enabled) + "' WHERE id = '" + id + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.DefCommandsList.FindIndex(x => x.id == id);
				DefComl = db.DefCommandsList.Find(x => x.id == id);
				db.DefCommandsList.RemoveAt(i);
				DefComl.Enabled = Enabled;
				db.DefCommandsList.Insert(i, DefComl);
			}

			public void UpdateLastUsed(string Command, string Data) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				DefCommand_tclass DefComl = null;
				sql = "UPDATE DefCommands SET LastUsed='" + Data + "' WHERE Command='" + Command + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.DefCommandsList.FindIndex(x => x.Command == Command);
				DefComl = db.DefCommandsList.Find(x => x.Command == Command);
				db.DefCommandsList.RemoveAt(i);
				DefComl.LastUsed = Data;
				db.DefCommandsList.Insert(i, DefComl);
			}
			public void UpdateLastUsed(int id, string Data) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				DefCommand_tclass DefComl = null;
				sql = "UPDATE DefCommands SET LastUsed='" + Data + "' WHERE id='" + id + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.DefCommandsList.FindIndex(x => x.id == id);
				DefComl = db.DefCommandsList.Find(x => x.id == id);
				db.DefCommandsList.RemoveAt(i);
				DefComl.LastUsed = Data;
				db.DefCommandsList.Insert(i, DefComl);
			}

			public void UpdateListfromTable() {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				Q.CommandText = "SELECT * FROM DefCommands;";
				R = Q.ExecuteReader();
				if (R.HasRows) {
					db.DefCommandsList.Clear();
					int p1 = 0;
					string p2 = null;
					while (R.Read()) {
						p1 = 0; p2 = null;
						try { p1 = R.GetInt32(3); } catch (Exception) { }
						try { p2 = R.GetString(5); } catch (Exception) { }
						db.DefCommandsList.Add(new DefCommand_tclass(R.GetInt32(0), R.GetString(1), R.GetString(2), p1, Convert.ToBoolean(R.GetInt32(4)), p2));
					}
				}
			}
		}



		// -- Класс для работы с таблицей FuncCommands --
		public class FuncCommandsTC {
			private DB db = null;
			public FuncCommandsTC(DB db) {
				this.db = db;
			}

			public int Add(FuncCommand_tclass FuncComl) {
				StringBuilder sql = new StringBuilder("INSERT INTO FuncCommands (Command, FuncName", 150);
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
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

				Q.CommandText = sql.ToString();
				R = Q.ExecuteReader();
				R.Read();

				db.FuncCommandsList.Add(new FuncCommand_tclass(R.GetInt32(0), FuncComl.Command, FuncComl.FuncName, FuncComl.Desc, FuncComl.Params, FuncComl.CoolDown, FuncComl.Secured, FuncComl.Enabled, FuncComl.LastUsed));

				return R.GetInt32(0);
			}

			public FuncCommand_tclass Find(string Command) {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				FuncCommand_tclass sout = new FuncCommand_tclass();
				Q.CommandText = "SELECT * FROM FuncCommands WHERE Command = '" + Command + ";";
				R = Q.ExecuteReader();
				if (R.HasRows) {
					R.Read();
					sout.id = R.GetInt32(0);
					sout.Command = R.GetString(1);
					sout.FuncName = R.GetString(2);
					try { sout.Desc = R.GetString(3); } catch (Exception) { }
					try { sout.Params = R.GetString(4); } catch (Exception) { }
					try { sout.CoolDown = R.GetInt32(5); } catch (Exception) { }
					sout.Secured = Convert.ToBoolean(R.GetInt32(6));
					sout.Enabled = Convert.ToBoolean(R.GetInt32(7));
					try { sout.LastUsed = R.GetString(8); } catch (Exception) { }
				}
				return sout;
			}

			public void UpdateLastUsed(string Command, string Data) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				FuncCommand_tclass FuncComl = null;
				sql = "UPDATE FuncCommands SET LastUsed='" + Data + "' WHERE Command='" + Command + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.FuncCommandsList.FindIndex(x => x.Command == Command);
				FuncComl = db.FuncCommandsList.Find(x => x.Command == Command);
				db.FuncCommandsList.RemoveAt(i);
				FuncComl.LastUsed = Data;
				db.FuncCommandsList.Insert(i, FuncComl);
			}
			public void UpdateLastUsed(int id, string Data) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				FuncCommand_tclass FuncComl = null;
				sql = "UPDATE FuncCommands SET LastUsed='" + Data + "' WHERE id='" + id + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.FuncCommandsList.FindIndex(x => x.id == id);
				FuncComl = db.FuncCommandsList.Find(x => x.id == id);
				db.FuncCommandsList.RemoveAt(i);
				FuncComl.LastUsed = Data;
				db.FuncCommandsList.Insert(i, FuncComl);
			}

			public void RowUpdate(FuncCommand_tclass FuncComl) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				sql = "UPDATE FuncCommands SET Command = '" + FuncComl.Command + "', CoolDown = '" + FuncComl.CoolDown + "' WHERE id = '" + FuncComl.id + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.FuncCommandsList.FindIndex(x => x.id == FuncComl.id);
				db.FuncCommandsList.RemoveAt(i);
				db.FuncCommandsList.Insert(i, FuncComl);
			}

			public void EnabledUpdate(string Command, bool Enabled) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				FuncCommand_tclass FuncComl = null;
				sql = "UPDATE FuncCommands SET Enabled = '" + Convert.ToInt32(Enabled) + "' WHERE Command = '" + Command + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.FuncCommandsList.FindIndex(x => x.Command == Command);
				FuncComl = db.FuncCommandsList.Find(x => x.Command == Command);
				db.FuncCommandsList.RemoveAt(i);
				FuncComl.Enabled = Enabled;
				db.FuncCommandsList.Insert(i, FuncComl);
			}
			public void EnabledUpdate(int id, bool Enabled) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				FuncCommand_tclass FuncComl = null;
				sql = "UPDATE FuncCommands SET Enabled = '" + Convert.ToInt32(Enabled) + "' WHERE id = '" + id + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.FuncCommandsList.FindIndex(x => x.id == id);
				FuncComl = db.FuncCommandsList.Find(x => x.id == id);
				db.FuncCommandsList.RemoveAt(i);
				FuncComl.Enabled = Enabled;
				db.FuncCommandsList.Insert(i, FuncComl);
			}

			public void UpdateListfromTable() {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				Q.CommandText = "SELECT * FROM FuncCommands;";
				R = Q.ExecuteReader();
				if (R.HasRows) {
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
				}
			}
		}



		// -- Класс для работы с таблицей SpamMessages --
		public class SpamMessagesTC {
			private DB db = null;
			public SpamMessagesTC(DB db) {
				this.db = db;
			}

			public int Add(SpamMessage_tclass SpamMsgl) {			
				StringBuilder sql = new StringBuilder("INSERT INTO SpamMessages (Message", 120);
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				if (SpamMsgl.CoolDown > 0) { sql.Append(", CoolDown"); }
				sql.Append(", Enabled");
				if (SpamMsgl.LastUsed != null && SpamMsgl.LastUsed != "") { sql.Append(", LastUsed"); }

				sql.Append(") VALUES ('" + SpamMsgl.Message + "'");

				if (SpamMsgl.CoolDown > 0) { sql.Append(", '" + SpamMsgl.CoolDown + "'"); }
				sql.Append(", '" + Convert.ToInt32(SpamMsgl.Enabled) + "'");
				if (SpamMsgl.LastUsed != null && SpamMsgl.LastUsed != "") { sql.Append(", " + SpamMsgl.LastUsed + "'"); }
				sql.Append(");  SELECT last_insert_rowid();");

				Q.CommandText = sql.ToString();
				R = Q.ExecuteReader();
				R.Read();

				db.SpamMessagesList.Add(new SpamMessage_tclass(R.GetInt32(0), SpamMsgl.Message, SpamMsgl.CoolDown, SpamMsgl.Enabled, SpamMsgl.LastUsed));

				return R.GetInt32(0);
			}

			public SpamMessage_tclass Find(string Command) {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				SpamMessage_tclass sout = new SpamMessage_tclass();
				Q.CommandText = "SELECT * FROM SpamMessages WHERE Command = '" + Command + ";";
				R = Q.ExecuteReader();
				if (R.HasRows) {
					R.Read();
					sout.id = R.GetInt32(0);
					sout.Message = R.GetString(1);
					sout.CoolDown = R.GetInt32(2);
					sout.Enabled = Convert.ToBoolean(R.GetInt32(3));
				}
				return sout;
			}

			public void Remove(string Message) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				sql = "DELETE FROM SpamMessages WHERE Message = '" + Message + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				db.SpamMessagesList.Remove(db.SpamMessagesList.Find(x => x.Message == Message));
			}
			public void Remove(int id) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				sql = "DELETE FROM SpamMessages WHERE id = '" + id + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				db.SpamMessagesList.Remove(db.SpamMessagesList.Find(x => x.id == id));
			}

			public void UpdateLastUsed(string Message, string Data) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				SpamMessage_tclass SpamMsgl = null;
				sql = "UPDATE SpamMessages SET LastUsed='" + Data + "' WHERE Message='" + Message + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.SpamMessagesList.FindIndex(x => x.Message== Message);
				SpamMsgl = db.SpamMessagesList.Find(x => x.Message == Message);
				db.SpamMessagesList.RemoveAt(i);
				SpamMsgl.LastUsed = Data;
				db.SpamMessagesList.Insert(i, SpamMsgl);
			}
			public void UpdateLastUsed(int id, string Data) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				SpamMessage_tclass SpamMsgl = null;
				sql = "UPDATE SpamMessages SET LastUsed='" + Data + "' WHERE id='" + id + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.SpamMessagesList.FindIndex(x => x.id == id);
				SpamMsgl = db.SpamMessagesList.Find(x => x.id == id);
				db.SpamMessagesList.RemoveAt(i);
				SpamMsgl.LastUsed = Data;
				db.SpamMessagesList.Insert(i, SpamMsgl);
			}

			public void RowUpdate(SpamMessage_tclass SpamMsgl) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				sql = "UPDATE SpamMessages SET Message = '" + SpamMsgl.Message + "', CoolDown = '" + SpamMsgl.CoolDown + "' WHERE id = '" + SpamMsgl.id + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.SpamMessagesList.FindIndex(x => x.id == SpamMsgl.id);
				db.SpamMessagesList.RemoveAt(i);
				db.SpamMessagesList.Insert(i, SpamMsgl);
			}

			public void EnabledUpdate(string Message, bool Enabled) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				SpamMessage_tclass SpamMsgl = null;
				sql = "UPDATE SpamMessages SET Enabled = '" + Convert.ToInt32(Enabled) + "' WHERE Message= '" + Message + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.SpamMessagesList.FindIndex(x => x.Message == Message);
				SpamMsgl = db.SpamMessagesList.Find(x => x.Message == Message);
				db.SpamMessagesList.RemoveAt(i);
				SpamMsgl.Enabled = Enabled;
				db.SpamMessagesList.Insert(i, SpamMsgl);
			}
			public void EnabledUpdate(int id, bool Enabled) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				int i = 0;
				SpamMessage_tclass SpamMsgl = null;
				sql = "UPDATE SpamMessages SET Enabled = '" + Convert.ToInt32(Enabled) + "' WHERE id = '" + id + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				i = db.SpamMessagesList.FindIndex(x => x.id == id);
				SpamMsgl = db.SpamMessagesList.Find(x => x.id == id);
				db.SpamMessagesList.RemoveAt(i);
				SpamMsgl.Enabled = Enabled;
				db.SpamMessagesList.Insert(i, SpamMsgl);
			}

			public void UpdateListfromTable() {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				Q.CommandText = "SELECT * FROM SpamMessages;";
				R = Q.ExecuteReader();
				if (R.HasRows) {
					db.SpamMessagesList.Clear();
					int p1 = 0;
					string p2 = null;
					while (R.Read()) {
						try { p1 = R.GetInt32(2); } catch (Exception) { }
						try { p2 = R.GetString(4); } catch (Exception) { }
						db.SpamMessagesList.Add(new SpamMessage_tclass(R.GetInt32(0), R.GetString(1), p1, Convert.ToBoolean(R.GetInt32(3)), p2));
					}
				}
			}
		}



		// -- Класс для работы с таблицей ChatHistory --
		public class ChatHistoryTC {
			private DB db = null;
			private List<string> sqladdmsg = new List<string>();
			private bool addmsg = false;
			public ChatHistoryTC(DB db) {
				this.db = db;
				Task.Factory.StartNew(() => this.Transact());
			}

			public void Add(ChatHistory_tclass ChatHstrl) {
				StringBuilder sql = new StringBuilder("INSERT INTO ChatHistory (Userid", 120);
				//SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				//SQLiteDataReader R = null;
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

				//Q.CommandText = sql;
				//R = Q.ExecuteReader();
				//R.Read();

				//db.ChatHistoryList.Add(new ChatHistory_tclass(R.GetInt32(0), ChatHstrl.Userid, ChatHstrl.Msgid, ChatHstrl.Nick, ChatHstrl.Msg, ChatHstrl.Date, ChatHstrl.Time, ChatHstrl.isOwner, ChatHstrl.isDel, ChatHistory.isMod, ChatHistory.isVIP, ChatHistory.isSub, ChatHstrl.BadgeInfo, ChatHstrl.Badges));

				sqladdmsg.Add(sql.ToString());
			}

			public bool MsgDeleted(string Msgid) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				//int i = 0;
				ChatHistory_tclass ChatHstrl = null;
				int updated = 0;
				sql = "UPDATE ChatHistory SET isDel = '1' WHERE Msgid = '" + Msgid + "';";
				Q.CommandText = sql;
				updated = Q.ExecuteNonQuery();
				if (updated == 1) {
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
			public bool MsgDeleted(int id) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				//int i = 0;
				ChatHistory_tclass ChatHstrl = null;
				int updated = 0;
				sql = "UPDATE ChatHistory SET isDel = '1' WHERE id = '" + id + "';";
				Q.CommandText = sql;
				updated = Q.ExecuteNonQuery();
				if (updated == 1) {
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

			public void Remove(string Message) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				sql = "DELETE FROM ChatHistory WHERE Message = '" + Message + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				//db.ChatHistoryList.Remove(db.ChatHistoryList.Find(x => x.Message == Message));
			}
			public void Remove(int id) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				sql = "DELETE FROM ChatHistory WHERE id = '" + id + "';";
				Q.CommandText = sql;
				Q.ExecuteNonQuery();

				//db.ChatHistoryList.Remove(db.ChatHistoryList.Find(x => x.id == id));
			}

			private void UpdateListfromTable() {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				Q.CommandText = "SELECT * FROM ChatHistory;";
				R = Q.ExecuteReader();
				if (R.HasRows) {
					db.SpamMessagesList.Clear();
					while (R.Read()) {
						//db.ChatHistoryList.Add(new ChatHistory_tclass(R.GetInt32(0), R.GetString(1), R.GetString(2), R.GetString(3), R.GetString(4), Convert.ToBoolean(R.GetInt32(5)), Convert.ToBoolean(R.GetInt32(6)), Convert.ToBoolean(R.GetInt32(7)), Convert.ToBoolean(R.GetInt32(8))));
					}
				}
			}

			private void Transact() {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				while (true) {
					Thread.Sleep(3000);
					if (sqladdmsg.Count > 0) {
						Q.CommandText = "";
						for (ushort i = 0; i < sqladdmsg.Count; i++) {
							Q.CommandText += sqladdmsg.ElementAt<string>(i);
						}
						sqladdmsg.Clear();
						Q.ExecuteNonQuery();
					}
				}
			}
		}



		// -- Класс для работы с таблицей Funcs_Timers --
		public class Funcs_TimersTC {
			private DB db = null;
			public Funcs_TimersTC(DB db) {
				this.db = db;
			}

			public void Add(Funcs_Timer_tclass Timerl) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				sql = "INSERT INTO Funcs_Timers (Name, Time, Nick, DTStart, Paused) VALUES ('" + Timerl.Name + "', '" + Timerl.Time + "', '" + Timerl.Nick + "', '" + Timerl.DTStart + "', '" + Convert.ToInt32(Timerl.Paused) + "'); SELECT last_insert_rowid();";
				Q.CommandText = sql;
				R = Q.ExecuteReader();
				R.Read();

				db.TimersList.Add(new Funcs_Timer_tclass(R.GetInt32(0), Timerl.Name, Timerl.Time, Timerl.Nick, Timerl.DTStart, Timerl.DTPauseResume, Timerl.Paused));
			}

			public bool Remove(string Name) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				DB.Funcs_Timer_tclass Timerl = db.TimersList.Find(x => x.Name == Name);
				if (Timerl != null) {
					db.TimersList.Remove(Timerl);
					sql = "DELETE FROM Funcs_Timers WHERE Name = '" + Name + "';";
					Q.CommandText = sql;
					Q.ExecuteNonQuery();
					return true;
				} else {
					return false;
				}
			}
			public bool Remove(int id) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				DB.Funcs_Timer_tclass Timerl = db.TimersList.Find(x => x.id == id);
				if (Timerl != null) {
					db.TimersList.Remove(Timerl);
					sql = "DELETE FROM Funcs_Timers WHERE id = '" + id + "';";
					Q.CommandText = sql;
					Q.ExecuteNonQuery();
					return true;
				} else {
					return false;
				}
			}

			public bool PauseUpdate(string Name, bool Paused) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				int i = 0;
				Funcs_Timer_tclass Timerl = null;
				string DTstr = DateTime.Now.ToString();
				string strres = null;

				// -- Т.к. да, делаем логику обработки дат начала и конца паузы тут, хоть это немного и не надёжно --
				// -- Получение строки, которая уже есть в базе, потому что именно добавлять к строке в запросе нельзя (хоть я и не гуглил, в транзакт-sql наверно можно) 
				sql = "SELECT DTPauseResume, Paused FROM Funcs_Timers WHERE Name = '" + Name + "';";
				Q.CommandText = sql;
				R = Q.ExecuteReader();
				R.Read();
				try { strres = R.GetString(0); } catch (Exception) { }
				if (Convert.ToBoolean(R.GetInt32(1)) == !Paused) {
					R.Close();

					// -- Ну и добавляем текущую дату к строке в соответствии с заданным режимом
					if (Paused) {
						sql = "UPDATE Funcs_Timers SET DTPauseResume = '" + strres + DTstr + ",', Paused = '1' WHERE Name = '" + Name + "';";
					} else {
						if (!Paused) {
							sql = "UPDATE Funcs_Timers SET DTPauseResume = '" + strres + DTstr + ";', Paused = '0' WHERE Name = '" + Name + "';";
						} else { }
					}

					Q.CommandText = sql;
					Q.ExecuteNonQuery();

					i = db.TimersList.FindIndex(x => x.Name == Name);
					Timerl = db.TimersList.Find(x => x.Name == Name);
					Timerl.Paused = Paused;
					Timerl.DTPauseResume = strres + DTstr;
					db.TimersList.RemoveAt(i);
					db.TimersList.Insert(i, Timerl);
					return true;
				} else {
					return false;
				}
			}
			/*public bool PauseUpdate(int id, bool Paused) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				int i = 0;
				Funcs_Timer_tclass Timerl = null;
				string DTstr = DateTime.Now.ToString();
				string strres = null;

				sql = "SELECT DTPauseResume, Paused FROM Funcs_Timers WHERE id = '" + id + "';";
				Q.CommandText = sql;
				R = Q.ExecuteReader();
				R.Read();
				try { strres = R.GetString(0); } catch (Exception) { }
				if (Convert.ToBoolean(R.GetInt32(1)) == !Paused) {
					R.Close();

					if (Paused) {
						sql = "UPDATE Funcs_Timers SET DTPauseResume = '" + strres + DTstr + ",', Paused = '1' WHERE id = '" + id + "';";
					} else {
						if (!Paused) {
							sql = "UPDATE Funcs_Timers SET DTPauseResume = '" + strres + DTstr + ";', Paused = '0' WHERE id = '" + id + "';";
						} else { }
					}
					Q.CommandText = sql;
					Q.ExecuteNonQuery();

					i = db.TimersList.FindIndex(x => x.id == id);
					Timerl = db.TimersList.Find(x => x.id == id);
					Timerl.Paused = Paused;
					Timerl.DTPauseResume = strres + DTstr;
					db.TimersList.RemoveAt(i);
					db.TimersList.Insert(i, Timerl);
					return true;
				} else {
					return false;
				}
			}*/

			public bool Restart(string Name) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				string DTPR = null;
				string DT = null;
				int i = 0;
				DB.Funcs_Timer_tclass Timerl = db.TimersList.Find(x => x.Name == Name);
				if (Timerl != null) {
					if (Timerl.DTPauseResume != null && Timerl.DTPauseResume != "") {
						DTPR = Timerl.DTPauseResume.Substring(Timerl.DTPauseResume.LastIndexOf(';') + 1);
					} else {
						DTPR = "";
					}
					DT = DateTime.Now.ToString();

					if (DTPR.Length > 2) {
						DTPR = DTPR.Substring(1, DTPR.IndexOf(','));
						sql = "UPDATE Funcs_Timers SET DTPauseResume = '" + DTPR + "', DTStart = '" + DT + "' WHERE id = '" + Timerl.id + "';";
						Timerl.DTPauseResume = DTPR;
					} else {
						sql = "UPDATE Funcs_Timers SET DTStart = '" + DT + "' WHERE id = '" + Timerl.id + "';";
					}

					Q.CommandText = sql;
					Q.ExecuteNonQuery();

					i = db.TimersList.FindIndex(x => x.id == Timerl.id);
					db.TimersList.RemoveAt(i);
					Timerl.DTStart = DT;
					db.TimersList.Insert(i, Timerl);
					return true;
				} else {
					return false;
				}
			}

			public void UpdateListfromTable() {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				Q.CommandText = "SELECT * FROM Funcs_Timers;";
				R = Q.ExecuteReader();
				if (R.HasRows) {
					db.TimersList.Clear();
					while (R.Read()) {
						db.TimersList.Add(new Funcs_Timer_tclass(R.GetInt32(0), R.GetString(1), Convert.ToUInt64(R.GetString(2)), R.GetString(3), R.GetString(4), R.GetString(5), Convert.ToBoolean(R.GetInt32(6))));
					}
				}
			}
		}



		// -- Класс для работы с таблицей Funcs_Stopwatches --
		public class Funcs_StopwatchesTC {
			private DB db = null;
			public Funcs_StopwatchesTC(DB db) {
				this.db = db;
			}

			public void Add(Funcs_Stopwatch_tclass Stopwatchl) {
				StringBuilder sql = new StringBuilder("INSERT INTO Funcs_Stopwatches (Name", 130);
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				if (Stopwatchl.Nick != null & Stopwatchl.Nick != "") { sql.Append(", Nick"); }
				sql.Append(", DTStart, Paused) VALUES ('" + Stopwatchl.Name + "'");

				if (Stopwatchl.Nick != null & Stopwatchl.Nick != "") { sql.Append(", '" + Stopwatchl.Nick + "'"); }
				sql.Append(", '" + Stopwatchl.DTStart + "', '" + Convert.ToInt32(Stopwatchl.Paused) + "'); SELECT last_insert_rowid();");

				Q.CommandText = sql.ToString();
				R = Q.ExecuteReader();
				R.Read();

				db.StopwatchesList.Add(new Funcs_Stopwatch_tclass(R.GetInt32(0), Stopwatchl.Name, Stopwatchl.Nick, Stopwatchl.DTStart, Stopwatchl.DTPauseResume, Stopwatchl.Paused));
			}

			public bool Remove(string Name) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				DB.Funcs_Stopwatch_tclass Stopwatchl = db.StopwatchesList.Find(x => x.Name == Name);
				if (Stopwatchl != null) {
					db.StopwatchesList.Remove(Stopwatchl);
					sql = "DELETE FROM Funcs_Stopwatches WHERE Name = '" + Name + "';";
					Q.CommandText = sql;
					Q.ExecuteNonQuery();
					return true;
				} else {
					return false;
				}
			}
			public bool Remove(int id) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				DB.Funcs_Stopwatch_tclass Stopwatchl = db.StopwatchesList.Find(x => x.id == id);
				if (Stopwatchl != null) {
					db.StopwatchesList.Remove(Stopwatchl);
					sql = "DELETE FROM Funcs_Stopwatches WHERE id = '" + id + "';";
					Q.CommandText = sql;
					Q.ExecuteNonQuery();
					return true;
				} else {
					return false;
				}
			}

			public bool PauseUpdate(string Name, bool Paused) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				int i = 0;
				Funcs_Stopwatch_tclass Stopwatchl = db.StopwatchesList.Find(x => x.Name == Name);
				string DTstr = DateTime.Now.ToString();
				string dtprorg = Stopwatchl.DTPauseResume;

				// -- Т.к. да, делаем логику обработки дат начала и конца паузы тут, хоть это немного и не надёжно, но с другой стороны... --
				if (Stopwatchl != null && Stopwatchl.Paused == !Paused) {
					// -- Добавляем текущую дату к строке в соответствии с заданным режимом
					if (Paused) {
						sql = "UPDATE Funcs_Stopwatches SET DTPauseResume = '" + dtprorg + DTstr + ",', Paused = '1' WHERE Name = '" + Name + "';";
					} else {
						if (!Paused) {
							sql = "UPDATE Funcs_Stopwatches SET DTPauseResume = '" + dtprorg + DTstr + ";', Paused = '0' WHERE Name = '" + Name + "';";
						} else { }
					}

					Q.CommandText = sql;
					Q.ExecuteNonQuery();

					i = db.StopwatchesList.FindIndex(x => x.Name == Name);
					db.StopwatchesList.RemoveAt(i);
					Stopwatchl.Paused = Paused;
					Stopwatchl.DTPauseResume = dtprorg + DTstr;
					db.StopwatchesList.Insert(i, Stopwatchl);
					return true;
				} else {
					return false;
				}
			}
			/*public bool PauseUpdate(int id, bool Paused) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				int i = 0;
				Funcs_Stopwatch_tclass Stopwatchl = db.StopwatchesList.Find(x => x.id == id);
				string DTstr = DateTime.Now.ToString();
				string dtprorg = Stopwatchl.DTPauseResume;

				if (Stopwatchl != null && Stopwatchl.Paused == !Paused) {
					if (Paused) {
						sql = "UPDATE Funcs_Stopwatches SET DTPauseResume = '" + dtprorg + DTstr + ",', Paused = '1' WHERE id = '" + id + "';";
					} else {
						if (!Paused) {
							sql = "UPDATE Funcs_Stopwatches SET DTPauseResume = '" + dtprorg + DTstr + ";', Paused = '0' WHERE id = '" + id + "';";
						} else { }
					}
					Q.CommandText = sql;
					Q.ExecuteNonQuery();

					i = db.StopwatchesList.FindIndex(x => x.id == id);
					db.StopwatchesList.RemoveAt(i);
					Stopwatchl.Paused = Paused;
					Stopwatchl.DTPauseResume = dtprorg + DTstr;
					db.StopwatchesList.Insert(i, Stopwatchl);
					return true;
				} else {
					return false;
				}
			}*/

			public bool Restart(string Name) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				string DTPR = null;
				string DT = null;
				int i = 0;
				DB.Funcs_Stopwatch_tclass Stopwatchl = db.StopwatchesList.Find(x => x.Name == Name);
				if (Stopwatchl != null) {
					if (Stopwatchl.DTPauseResume != null && Stopwatchl.DTPauseResume != "") {
						DTPR = Stopwatchl.DTPauseResume.Substring(Stopwatchl.DTPauseResume.LastIndexOf(';') + 1);
					} else {
						DTPR = "";
					}
					DT = DateTime.Now.ToString();

					if (DTPR.Length > 2) {
						DTPR = DTPR.Substring(1, DTPR.IndexOf(','));
						sql = "UPDATE Funcs_Stopwatches SET DTPauseResume = '" + DTPR + "', DTStart = '" + DT + "' WHERE id = '" + Stopwatchl.id + "';";
						Stopwatchl.DTPauseResume = DTPR;
					} else {
						sql = "UPDATE Funcs_Stopwatches SET DTStart = '" + DT + "' WHERE id = '" + Stopwatchl.id + "';";
					}

					Q.CommandText = sql;
					Q.ExecuteNonQuery();

					i = db.StopwatchesList.FindIndex(x => x.id == Stopwatchl.id);
					db.StopwatchesList.RemoveAt(i);
					Stopwatchl.DTStart = DT;
					db.StopwatchesList.Insert(i, Stopwatchl);
					return true;
				} else {
					return false;
				}
			}

			public void UpdateListfromTable() {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				Q.CommandText = "SELECT * FROM Funcs_Stopwatches;";
				R = Q.ExecuteReader();
				if (R.HasRows) {
					db.StopwatchesList.Clear();
					string p1 = null;
					while (R.Read()) {
						p1 = null;
						try { p1 = R.GetString(4); } catch (Exception) { }
						db.StopwatchesList.Add(new Funcs_Stopwatch_tclass(R.GetInt32(0), R.GetString(1), R.GetString(2), R.GetString(3), p1, Convert.ToBoolean(R.GetInt32(5))));
					}
				}
			}
		}



		// -- Класс для работы с таблицей Funcs_Counters --
		public class Funcs_CountersTC {
			private DB db = null;
			public Funcs_CountersTC(DB db) {
				this.db = db;
			}

			public void Add(Funcs_Counter_tclass Counterl) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				sql = "INSERT INTO Funcs_Counters(Name, Value) VALUES ('" + Counterl.Name + "', '" + Counterl.Value + "'); SELECT last_insert_rowid();";
				Q.CommandText = sql;
				R = Q.ExecuteReader();
				R.Read();

				db.CountersList.Add(new Funcs_Counter_tclass(R.GetInt32(0), Counterl.Name, Counterl.Value));
			}

			public bool Remove(string Name) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				DB.Funcs_Counter_tclass Counterl = db.CountersList.Find(x => x.Name == Name);
				if (Counterl != null) {
					db.CountersList.Remove(Counterl);
					sql = "DELETE FROM Funcs_Counters WHERE Name = '" + Name + "';";
					Q.CommandText = sql;
					Q.ExecuteNonQuery();
					return true;
				} else {
					return false;
				}
			}
			public bool Remove(int id) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				DB.Funcs_Counter_tclass Counterl = db.CountersList.Find(x => x.id == id);
				if (Counterl != null) {
					db.CountersList.Remove(Counterl);
					sql = "DELETE FROM Funcs_Counters WHERE id = '" + id + "';";
					Q.CommandText = sql;
					Q.ExecuteNonQuery();
					return true;
				} else {
					return false;
				}
			}

			public bool Update(string Name, int Value) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				string DTPR = null;
				string DT = null;
				int i = 0;
				DB.Funcs_Counter_tclass Counterl = db.CountersList.Find(x => x.Name == Name);
				if (Counterl != null) {
					sql = "UPDATE Funcs_Counters SET Value = '" + Value + "' WHERE id = '" + Counterl.id + "';";
					Q.CommandText = sql;
					Q.ExecuteNonQuery();

					i = db.CountersList.FindIndex(x => x.id == Counterl.id);
					db.CountersList.RemoveAt(i);
					Counterl.Value = Value;
					db.CountersList.Insert(i, Counterl);
					return true;
				} else {
					return false;
				}
			}

			public bool Plus(string Name) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				string DTPR = null;
				string DT = null;
				int i = 0;
				int val = 0;
				DB.Funcs_Counter_tclass Counterl = db.CountersList.Find(x => x.Name == Name);
				if (Counterl != null) {
					Counterl.Value += 1;
					sql = "UPDATE Funcs_Counters SET Value = '" + Counterl.Value + "' WHERE id = '" + Counterl.id + "';";
					Q.CommandText = sql;
					Q.ExecuteNonQuery();

					i = db.CountersList.FindIndex(x => x.id == Counterl.id);
					db.CountersList.RemoveAt(i);
					db.CountersList.Insert(i, Counterl);
					return true;
				} else {
					return false;
				}
			}

			public bool Minus(string Name) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				string DTPR = null;
				string DT = null;
				int i = 0;
				int val = 0;
				DB.Funcs_Counter_tclass Counterl = db.CountersList.Find(x => x.Name == Name);
				if (Counterl != null) {
					Counterl.Value -= 1;
					sql = "UPDATE Funcs_Counters SET Value = '" + Counterl.Value + "' WHERE id = '" + Counterl.id + "';";
					Q.CommandText = sql;
					Q.ExecuteNonQuery();

					i = db.CountersList.FindIndex(x => x.id == Counterl.id);
					db.CountersList.RemoveAt(i);
					db.CountersList.Insert(i, Counterl);
					return true;
				} else {
					return false;
				}
			}

			public bool Restart(string Name) {
				string sql = null;
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				string DTPR = null;
				string DT = null;
				int i = 0;
				DB.Funcs_Counter_tclass Counterl = db.CountersList.Find(x => x.Name == Name);
				if (Counterl != null) {
					sql = "UPDATE Funcs_Counters SET Value = '0' WHERE id = '" + Counterl.id + "';";
					Q.CommandText = sql;
					Q.ExecuteNonQuery();

					i = db.CountersList.FindIndex(x => x.id == Counterl.id);
					db.CountersList.RemoveAt(i);
					Counterl.Value = 0;
					db.CountersList.Insert(i, Counterl);
					return true;
				} else {
					return false;
				}
			}

			public void UpdateListfromTable() {
				SQLiteCommand Q = new SQLiteCommand(db.SQLCon);
				SQLiteDataReader R = null;
				Q.CommandText = "SELECT * FROM Funcs_Counters;";
				R = Q.ExecuteReader();
				if (R.HasRows) {
					db.CountersList.Clear();
					while (R.Read()) {
						db.CountersList.Add(new Funcs_Counter_tclass(R.GetInt32(0), R.GetString(1), R.GetInt32(2)));
					}
				}
			}
		}





		/*// -- Функция переодической работы с базой данных --
		private void dbUpdate() {
			Query.CommandText = "";
			for (ushort i = 0; i < SQLList.Count; i++) {
				Query.CommandText += SQLList.ElementAt<string>(i);
			}
			Query.ExecuteNonQuery();
			Query.CommandText = "";
		}
		*/


		// -- Небольшая автоматизация запросов --
		public SQLiteDataReader SQLQuery(string SQL, bool DataReceived = true) {
			SQLiteCommand Q = new SQLiteCommand(SQLCon);
			SQLiteDataReader R = null;
			Q.CommandText = SQL;

			if (DataReceived) {
				R = Q.ExecuteReader();
			} else {
				Q.ExecuteNonQuery();
			}
			if (R.HasRows) {
				R.Read();
				return R;
			} else {
				return null;
			}
		}




		// -- Классы таблиц --
		// -- Settings
		public class Setting_tclass {
			public int id { get; set; }
			public string SetName { get; set; }
			public string Param { get; set; }

			public Setting_tclass() { }
			public Setting_tclass(string Setting) {
				this.SetName = Setting;
			}
			public Setting_tclass(string Setting, string Parameter) {
				this.SetName = Setting;
				this.Param = Parameter;
			}
			public Setting_tclass(int id, string Setting, string Parameter) {
				this.id = id;
				this.SetName = Setting;
				this.Param = Parameter;
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
			public bool Paused { get; set; }

			public Funcs_Timer_tclass() { }
			public Funcs_Timer_tclass(string Name, ulong Time, string Nick, string DTStart, string DTPauseResume = null, bool Paused = false) {
				this.Name = Name;
				this.Time = Time;
				this.Nick = Nick;
				this.DTStart = DTStart;
				this.DTPauseResume = DTPauseResume;
				this.Paused = Paused;
			}

			public Funcs_Timer_tclass(int id, string Name, ulong Time, string Nick, string DTStart, string DTPauseResume = null, bool Paused = false) {
				this.id = id;
				this.Name = Name;
				this.Time = Time;
				this.Nick = Nick;
				this.DTStart = DTStart;
				this.DTPauseResume = DTPauseResume;
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
