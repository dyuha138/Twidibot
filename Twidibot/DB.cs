using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Xml.Linq;
using System.Data.SqlTypes;
using System.Runtime.Remoting;
using System.Windows.Interop;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Text.RegularExpressions;
using static Twidibot.DB;

namespace Twidibot {
	public sealed partial class DB {
		private SQLiteConnection DBCon = null;
		//private SQLiteCommand DBQ = null;
		//private SQLiteDataReader dbR = null;

		public string DBName = null;
		private TechFuncs TechFuncs = null;
		//private ObservableCollection<string> SQLList = new ObservableCollection<string>();


		// -- Копии таблиц --
		public readonly List<Setting_tclass> SettingsList = new List<Setting_tclass>();
		public readonly List<Setting_tclass> SettingsTWHList = new List<Setting_tclass>();
		public readonly List<Setting_tclass> SettingsVKPLList = new List<Setting_tclass>();
		public readonly List<DefCommand_tclass> DefCommandsList = new List<DefCommand_tclass>();
		public readonly List<FuncCommand_tclass> FuncCommandsList = new List<FuncCommand_tclass>();
		public readonly List<SpamMessage_tclass> SpamMessagesList = new List<SpamMessage_tclass>();
		//public readonly List<ChatHistory_tclass> ChatHistoryList = new List<ChatHistory_tclass>();
		public readonly List<Funcs_Timer_tclass> TimersList = new List<Funcs_Timer_tclass>();
		public readonly List<Funcs_Stopwatch_tclass> StopwatchesList = new List<Funcs_Stopwatch_tclass>();
		public readonly List<Funcs_Counter_tclass> CountersList = new List<Funcs_Counter_tclass>();
		public readonly List<VIPUsers_tclass> VIPUsersList = new List<VIPUsers_tclass>();

		// -- Экземляры классов для работы с таблицами --
		public SettingsTC SettingsT = null;
		public SettingsTWHTC SettingsTWHT = null;
		public SettingsVKPLTC SettingsVKPLT = null;
		public DefCommandsTC DefCommandsT = null;
		public FuncCommandsTC FuncCommandsT = null;
		public SpamMessagesTC SpamMessagesT = null;
		public ChatHistoryTC ChatHistoryT = null;
		public Funcs_TimersTC Funcs_TimersT = null;
		public Funcs_StopwatchesTC Funcs_StopwatchesT = null;
		public Funcs_CountersTC Funcs_CountersT = null;
		public VIPUsersTC VIPUsersT = null;


		// -- Инициализация --
		public DB(TechFuncs techFuncs) {
			this.TechFuncs = techFuncs;
			this.DBName = "twididb.db";
			//this.DBName = "";
			DBCon = new SQLiteConnection("Data Source=" + this.DBName);
			//DBCon = new SqliteConnection("Data Source=" + this.DBName);
			//DBQ = new SQLiteCommand(DBCon);
			SettingsT = new SettingsTC(this);
			SettingsTWHT = new SettingsTWHTC(this);
			SettingsVKPLT = new SettingsVKPLTC(this);
			DefCommandsT = new DefCommandsTC(this);
			FuncCommandsT = new FuncCommandsTC(this);
			SpamMessagesT = new SpamMessagesTC(this);
			ChatHistoryT = new ChatHistoryTC(this);
			Funcs_TimersT = new Funcs_TimersTC(this);
			Funcs_StopwatchesT = new Funcs_StopwatchesTC(this);
			Funcs_CountersT = new Funcs_CountersTC(this);
			VIPUsersT = new VIPUsersTC(this);
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
			Funcs_TimersT = new Funcs_TimersTC(this);
			Funcs_StopwatchesT = new Funcs_StopwatchesTC(this);
			Funcs_CountersT = new Funcs_CountersTC(this);
			VIPUsersT = new VIPUsersTC(this);
		}*/

		public void Open() {
			DBCon.Open();
		}
		public void Close() {
			DBCon.Close();
		}



		// -- Обновление всех списков  --
		public void UpdateAllLists(bool LoadSettings = true) {
			if (LoadSettings) {
				this.SettingsT.UpdateListfromTable();
				this.SettingsTWHT.UpdateListfromTable();
				this.SettingsVKPLT.UpdateListfromTable();
			}
			this.DefCommandsT.UpdateListfromTable();
			this.FuncCommandsT.UpdateListfromTable();
			this.SpamMessagesT.UpdateListfromTable();
			this.Funcs_TimersT.UpdateListfromTable();
			this.Funcs_StopwatchesT.UpdateListfromTable();
			this.Funcs_CountersT.UpdateListfromTable();
			this.VIPUsersT.UpdateListfromTable();
		}


		// -- Главная функция для выполнения запросов --
		public SQLResultTable SQLQuery(string SQL, bool isRead) {
			SQLiteCommand DBQ = new SQLiteCommand(DBCon);
			SQLiteDataReader R = null;
			SQLResultTable ResTbl = null;
			DBQ.CommandText = SQL;
			R = DBQ.ExecuteReader();

			if (R.HasRows) {
				ResTbl = new SQLResultTable(R);
				R.Close();
				DBQ.CommandText = null;
				if (isRead) { ResTbl.NextRow(); }
				return ResTbl;
			} else {
				R.Close();
				return null;
			}
		}

		public int SQLQuery(string SQL) {
			SQLiteCommand DBQ = new SQLiteCommand(DBCon);
			DBQ.CommandText = SQL;
			int i = DBQ.ExecuteNonQuery();
			DBQ.CommandText = null;
			return i;
		}


		public class SQLResultTable {

			public int RowRead = -1;
			public int Rows = 0;
			public int Colums = 0;
			private List<TRow> Table = new List<TRow>();

			public class TRow {
				public List<string> Row = new List<string>();
			}

			public SQLResultTable(SQLiteDataReader R) {
				if (R != null && !R.IsClosed && R.HasRows) {
					Colums = R.FieldCount;
					string strl = null;
					while (R.Read()) {
						TRow Rowl = new TRow();
						for (int i = 0; i < Colums; i++) {
							strl = R.GetValue(i).ToString();
							if (strl == "" || strl == null) { strl = null; }
							Rowl.Row.Add(strl);
						}
						Table.Add(Rowl);
						Rows++;
					}
				}
			}

			public bool NextRow() { if (RowRead < Rows-1) { RowRead++; return true; } else { return false; } }
			public bool PrevRow() { if (RowRead > 0) { RowRead--; return true; } else { return false; } }
			public void SetRow(int Row) { this.RowRead = Row; }
			public bool HasRows() { if (Rows > 0) { return true; } else { return false; } }

			public string GetStr(int Column) {
				return Table.ElementAt(this.RowRead).Row.ElementAt(Column);
			}
			public int GetInt(int Column) {
				try { return Convert.ToInt32(Table.ElementAt(this.RowRead).Row.ElementAt(Column));
				} catch (Exception) { return 0; }
			}
			public long GetLong(int Column) {
				try {
					return Convert.ToInt64(Table.ElementAt(this.RowRead).Row.ElementAt(Column));
				} catch (Exception) { return 0; }
			}
			public short GetShort(int Column) {
				try {
					return Convert.ToInt16(Table.ElementAt(this.RowRead).Row.ElementAt(Column));
				} catch (Exception) { return 0; }
			}
			public bool GetBool(int Column) {
				//try { return Convert.ToBoolean(Convert.ToInt32(Table.ElementAt(this.RowRead).Row.ElementAt(Column)));
				try { return Convert.ToBoolean(Table.ElementAt(this.RowRead).Row.ElementAt(Column));
					} catch (Exception) { return false; }
			}
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
					db.TechFuncs.LogDH("Изменён параметр приложения \"" + SettingName + "\"");
					return true;
				} else {
					return false;
				}
			}

			public string GetParam(string SettingName) {
				SQLResultTable R = db.SQLQuery("SELECT Param FROM Settings WHERE SetName = '" + SettingName + "';", true);
				string sout = null; 
				if (R != null) {
					try { sout = R.GetStr(0); } catch (Exception) { }
				}
				return sout;
			}

			public void UpdateListfromTable() {
				db.TechFuncs.LogDH("Считывание из базы данных таблицы Settings...");
				SQLResultTable R = db.SQLQuery("SELECT * FROM Settings;", false);
				if (R != null) {
					db.SettingsList.Clear();
					while (R.NextRow()) {
						db.SettingsList.Add(new Setting_tclass(R.GetInt(0), R.GetStr(1), R.GetStr(2), R.GetBool(3)));
					}
				}
				db.TechFuncs.LogDH("Считывание из базы данных таблицы Settings завершено");
			}
		}


		// -- Класс для работы с таблицей Settings_TW --
		public sealed class SettingsTWHTC {
			private DB db = null;
			public SettingsTWHTC(DB db) { this.db = db; }

			public bool UpdateSetting(string SettingName, string Parameter) {
				int i = db.SettingsTWHList.FindIndex(x => x.SetName == SettingName);
				Setting_tclass Setl = db.SettingsTWHList.ElementAt(i);
				if (Setl != null) {
					if (Parameter == null) {
						db.SQLQuery("UPDATE Settings_TWH SET Param = null WHERE SetName='" + SettingName + "';");
					} else {
						db.SQLQuery("UPDATE Settings_TWH SET Param = '" + Parameter + "' WHERE SetName='" + SettingName + "';");
					}

					db.SettingsTWHList.RemoveAt(i);
					Setl.Param = Parameter;
					db.SettingsTWHList.Insert(i, Setl);
					db.TechFuncs.LogDH("Изменён параметр Twitch \"" + SettingName + "\"");
					return true;
				} else {
					return false;
				}
			}

			public string GetParam(string SettingName) {
				SQLResultTable R = db.SQLQuery("SELECT Param FROM Settings_TWH WHERE SetName = '" + SettingName + "';", true);
				string sout = null;
				if (R != null) {
					try { sout = R.GetStr(0); } catch (Exception) { }
				}
				return sout;
			}

			public void UpdateListfromTable() {
				db.TechFuncs.LogDH("Считывание из базы данных таблицы Settings_TWH...");
				SQLResultTable R = db.SQLQuery("SELECT * FROM Settings_TWH;", false);
				if (R != null) {
					db.SettingsTWHList.Clear();
					while (R.NextRow()) {
						db.SettingsTWHList.Add(new Setting_tclass(R.GetInt(0), R.GetStr(1), R.GetStr(2), R.GetBool(3)));
					}
				}
				db.TechFuncs.LogDH("Считывание из базы данных таблицы Settings_TWH завершено");
			}
		}


		// -- Класс для работы с таблицей Settings_VKPL --
		public sealed class SettingsVKPLTC {
			private DB db = null;
			public SettingsVKPLTC(DB db) { this.db = db; }

			public bool UpdateSetting(string SettingName, string Parameter) {
				int i = db.SettingsVKPLList.FindIndex(x => x.SetName == SettingName);
				Setting_tclass Setl = db.SettingsVKPLList.ElementAt(i);
				if (Setl != null) {
					if (Parameter == null) {
						db.SQLQuery("UPDATE Settings_VKPL SET Param = null WHERE SetName='" + SettingName + "';");
					} else {
						db.SQLQuery("UPDATE Settings_VKPL SET Param = '" + Parameter + "' WHERE SetName='" + SettingName + "';");
					}

					db.SettingsVKPLList.RemoveAt(i);
					Setl.Param = Parameter;
					db.SettingsVKPLList.Insert(i, Setl);
					db.TechFuncs.LogDH("Изменён параметр VK Play LIVE \"" + SettingName + "\"");
					return true;
				} else {
					return false;
				}
			}

			public string GetParam(string SettingName) {
				SQLResultTable R = db.SQLQuery("SELECT Param FROM Settings_VKPL WHERE SetName = '" + SettingName + "';", true);
				string sout = null;
				if (R != null) {
					try { sout = R.GetStr(0); } catch (Exception) { }
				}
				return sout;
			}

			public void UpdateListfromTable() {
				db.TechFuncs.LogDH("Считывание из базы данных таблицы Settings_VKPL...");
				SQLResultTable R = db.SQLQuery("SELECT * FROM Settings_VKPL;", false);
				if (R != null) {
					db.SettingsVKPLList.Clear();
					while (R.NextRow()) {
						db.SettingsVKPLList.Add(new Setting_tclass(R.GetInt(0), R.GetStr(1), R.GetStr(2), R.GetBool(3)));
					}
				}
				db.TechFuncs.LogDH("Считывание из базы данных таблицы Settings_VKPL завершено");
			}
		}



		// -- Класс для работы с таблицей DefCommands --
		public sealed class DefCommandsTC {
			private DB db = null;
			public DefCommandsTC(DB db) { this.db = db; }

			public int Add(DefCommand_tclass DefComl) {
				StringBuilder sql = new StringBuilder("INSERT INTO DefCommands (Command, Result", 130);
				SQLResultTable R = null;
				if (DefComl.CoolDown > 0) { sql.Append(", CoolDown"); }
				sql.Append(", isAlias, Enabled");
				if (DefComl.LastUsed > 0) { sql.Append(", LastUsed"); }

				sql.Append(") VALUES ('" + DefComl.Command + "', '" + DefComl.Result + "'");

				if (DefComl.CoolDown > 0) { sql.Append(", " + DefComl.CoolDown.ToString() + ""); }
				sql.Append(", " + DefComl.isAlias.ToString() + ", " + DefComl.Enabled.ToString());
				if (DefComl.LastUsed > 0) { sql.Append(", '" + DefComl.LastUsed.ToString() + "'"); }
				sql.Append("); SELECT last_insert_rowid();");

				R = db.SQLQuery(sql.ToString(), true);

				db.DefCommandsList.Add(new DefCommand_tclass(R.GetInt(0), DefComl.Command, DefComl.Result, DefComl.CoolDown, DefComl.isAlias, DefComl.Enabled, DefComl.LastUsed));
				db.TechFuncs.LogDH("Добавлена команда \"" + DefComl.Command + "\"");
				return R.GetInt(0);
			}

			public DefCommand_tclass Find(string Command) {
				SQLResultTable R = db.SQLQuery("SELECT * FROM DefCommands WHERE Command = '" + Command + ";", true);
				DefCommand_tclass DefComl = new DefCommand_tclass();
				if (R != null) {
					DefComl.id = R.GetInt(0);
					DefComl.Command = R.GetStr(1);
					DefComl.Result = R.GetStr(2);
					DefComl.CoolDown = R.GetInt(3);
					DefComl.isAlias = R.GetBool(4);
					DefComl.Enabled = R.GetBool(5);
					DefComl.LastUsed = R.GetLong(6);
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
					db.SQLQuery("UPDATE DefCommands SET Command = '" + DefComl.Command + "', Result = '" + DefComl.Result + "', CoolDown = " + DefComl.CoolDown + ", isAlias = " + DefComl.isAlias.ToString() + " WHERE id = '" + DefComl.id + "';");

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
					db.SQLQuery("UPDATE DefCommands SET Enabled = " + Enabled.ToString() + " WHERE id = '" + id + "';");

					db.DefCommandsList.RemoveAt(i);
					DefComl.Enabled = Enabled;
					db.DefCommandsList.Insert(i, DefComl);
					db.TechFuncs.LogDH("Изменена активность команды \"" + DefComl.Command + "\"");
					return true;
				} else {
					return false;
				}
			}

			public bool UpdateLastUsed(int id, long UnixTime) {
				int i = db.DefCommandsList.FindIndex(x => x.id == id);
				DefCommand_tclass DefComl = db.DefCommandsList.ElementAt(i);
				if (DefComl != null) {
					db.SQLQuery("UPDATE DefCommands SET LastUsed = " + UnixTime + " WHERE id='" + id + "';");

					db.DefCommandsList.RemoveAt(i);
					DefComl.LastUsed = UnixTime;
					db.DefCommandsList.Insert(i, DefComl);
					return true;
				} else {
					return false;
				}
			}

			public void UpdateListfromTable() {
				db.TechFuncs.LogDH("Считывание из базы данных таблицы DefCommands...");
				SQLResultTable R = db.SQLQuery("SELECT * FROM DefCommands;", false);
				if (R != null) {
					db.DefCommandsList.Clear();
					while (R.NextRow()) {
						db.DefCommandsList.Add(new DefCommand_tclass(R.GetInt(0), R.GetStr(1), R.GetStr(2), R.GetInt(3), R.GetBool(4), R.GetBool(5), R.GetLong(6)));
					}
				}
				db.TechFuncs.LogDH("Считывание из базы данных таблицы DefCommands завершено");
			}
		}



		// -- Класс для работы с таблицей FuncCommands --
		public sealed class FuncCommandsTC {
			private DB db = null;
			public FuncCommandsTC(DB db) { this.db = db; }

			/*public int Add(FuncCommand_tclass FuncComl) {
				StringBuilder sql = new StringBuilder("INSERT INTO FuncCommands (Command, FuncName", 150);
				SQLResultTable R = null;

				if (FuncComl.Desc != null && FuncComl.Desc != "") { sql.Append(", Desc"); }
				if (FuncComl.Params != null && FuncComl.Params != "") { sql.Append(", Params"); }
				if (FuncComl.CoolDown > 0) { sql.Append(", CoolDown"); }
				sql.Append(", Secured, Enabled");
				if (FuncComl.LastUsed != null && FuncComl.LastUsed != "") { sql.Append(", LastUsed"); }

				sql.Append(") VALUES ('" + FuncComl.Command + "', '" + FuncComl.FuncName + "'");

				if (FuncComl.Desc != null && FuncComl.Desc != "") { sql.Append(", '" + FuncComl.Desc + "'"); }
				if (FuncComl.Params != null && FuncComl.Params != "") { sql.Append(", '" + FuncComl.Params + "'"); }
				if (FuncComl.CoolDown > 0) { sql.Append(", " + FuncComl.CoolDown.ToString() + ""); }
				sql.Append(", " + FuncComl.Secured.ToString() + ", " + FuncComl.Enabled.ToString() + "");
				if (FuncComl.LastUsed != null && FuncComl.LastUsed != "") { sql.Append(", " + FuncComl.LastUsed + ""); }
				sql.Append(");  SELECT last_insert_rowid();");

				R = db.SQLQuery(sql.ToString(), true);

				db.FuncCommandsList.Add(new FuncCommand_tclass(R.GetInt32(0), FuncComl.Command, FuncComl.FuncName, FuncComl.Desc, FuncComl.Params, FuncComl.CoolDown, FuncComl.Secured, FuncComl.Enabled, FuncComl.LastUsed));
				return R.GetInt(0);
			}*/

			public FuncCommand_tclass Find(string Command) {
				SQLResultTable R = db.SQLQuery("SELECT * FROM FuncCommands WHERE Command = '" + Command + ";", true);
				FuncCommand_tclass FuncComl = new FuncCommand_tclass();
				if (R != null) {
					FuncComl.id = R.GetInt(0);
					FuncComl.Command = R.GetStr(1);
					FuncComl.FuncName = R.GetStr(2);
					FuncComl.Desc = R.GetStr(3);
					FuncComl.Params = R.GetStr(4);
					FuncComl.CoolDown = R.GetInt(5);
					FuncComl.Secured = R.GetBool(6);
					FuncComl.Enabled = R.GetBool(7);
					FuncComl.LastUsed = R.GetLong(8);
				}
				return FuncComl;
			}

			public bool UpdateLastUsed(int id, long UnixTime) {
				int i = db.FuncCommandsList.FindIndex(x => x.id == id);
				FuncCommand_tclass FuncComl = db.FuncCommandsList.ElementAt(i);
				if (FuncComl != null) {
					db.SQLQuery("UPDATE FuncCommands SET LastUsed = " + UnixTime + " WHERE id='" + id + "';");

					db.FuncCommandsList.RemoveAt(i);
					FuncComl.LastUsed = UnixTime;
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
					db.SQLQuery("UPDATE FuncCommands SET Command = '" + FuncComl.Command + "', CoolDown = " + FuncComl.CoolDown.ToString() + " WHERE id = '" + FuncComl.id + "';");

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
					db.SQLQuery("UPDATE FuncCommands SET Enabled = " + Enabled.ToString() + " WHERE id = '" + id + "';");

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
				db.TechFuncs.LogDH("Считывание из базы данных таблицы FuncCommands...");
				SQLResultTable R = db.SQLQuery("SELECT * FROM FuncCommands;", false);
				if (R != null) {
					db.FuncCommandsList.Clear();
					while (R.NextRow()) {
						db.FuncCommandsList.Add(new FuncCommand_tclass(R.GetInt(0), R.GetStr(1), R.GetStr(2), R.GetStr(3), R.GetStr(4), R.GetInt(5), R.GetBool(6), R.GetBool(7), R.GetLong(8)));
					}
				}
				db.TechFuncs.LogDH("Считывание из базы данных таблицы FuncCommands завершено");
			}
		}



		// -- Класс для работы с таблицей SpamMessages --
		public sealed class SpamMessagesTC {
			private DB db = null;
			public SpamMessagesTC(DB db) { this.db = db; }

			public int Add(SpamMessage_tclass SpamMsgl) {
				StringBuilder sql = new StringBuilder("INSERT INTO SpamMessages (Message", 120);
				SQLResultTable R = null;
				if (SpamMsgl.CoolDown > 0) { sql.Append(", CoolDown"); }
				sql.Append(", Enabled");
				if (SpamMsgl.LastUsed > 0) { sql.Append(", LastUsed"); }

				sql.Append(") VALUES ('" + SpamMsgl.Message + "'");

				if (SpamMsgl.CoolDown > 0) { sql.Append(", " + SpamMsgl.CoolDown.ToString() + ""); }
				sql.Append(", " + SpamMsgl.Enabled.ToString() + "");
				if (SpamMsgl.LastUsed > 0) { sql.Append(", " + SpamMsgl.LastUsed.ToString() + ""); }
				sql.Append("); SELECT last_insert_rowid();");

				R = db.SQLQuery(sql.ToString(), true);

				db.SpamMessagesList.Add(new SpamMessage_tclass(R.GetInt(0), SpamMsgl.Message, SpamMsgl.CoolDown, SpamMsgl.Enabled, SpamMsgl.LastUsed));
				db.TechFuncs.LogDH("Добавлено спам-сообщение \"" + SpamMsgl.Message + "\"");
				return R.GetInt(0);
			}

			public SpamMessage_tclass Find(string Message) {
				SQLResultTable R = db.SQLQuery("SELECT * FROM SpamMessages WHERE Message = '" + Message + ";", true);
				SpamMessage_tclass SpamMsgl = new SpamMessage_tclass();
				if (R != null) {
					SpamMsgl.id = R.GetInt(0);
					SpamMsgl.Message = R.GetStr(1);
					SpamMsgl.CoolDown = R.GetInt(2);
					SpamMsgl.Enabled = R.GetBool(3);
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

			public bool UpdateLastUsed(int id, long UnixTime) {
				int i = db.SpamMessagesList.FindIndex(x => x.id == id);
				SpamMessage_tclass SpamMsgl = db.SpamMessagesList.ElementAt(i);
				if (SpamMsgl != null) {
					db.SQLQuery("UPDATE SpamMessages SET LastUsed = " + UnixTime + " WHERE id='" + id + "';");

					db.SpamMessagesList.RemoveAt(i);
					SpamMsgl.LastUsed = UnixTime;
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
					db.SQLQuery("UPDATE SpamMessages SET Message = '" + SpamMsgl.Message + "', CoolDown = " + SpamMsgl.CoolDown.ToString() + " WHERE id = '" + SpamMsgl.id + "';");

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
					db.SQLQuery("UPDATE SpamMessages SET Enabled = " + Enabled.ToString() + " WHERE id = '" + id + "';");

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
				db.TechFuncs.LogDH("Считывание из базы данных таблицы SpamMessages...");
				SQLResultTable R = db.SQLQuery("SELECT * FROM SpamMessages;", false);
				if (R != null) {
					db.SpamMessagesList.Clear();
					while (R.NextRow()) {
						db.SpamMessagesList.Add(new SpamMessage_tclass(R.GetInt(0), R.GetStr(1), R.GetInt(2), R.GetBool(3), R.GetLong(4)));
					}
				}
				db.TechFuncs.LogDH("Считывание из базы данных таблицы SpamMessages завершено");
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
				if (ChatHstrl.Msgid != null && ChatHstrl.Msgid != "") { sql.Append(", Msgid"); }
				sql.Append(", Nick, Msg, UnixTime, Color, isOwner, isDel, isMod, isVIP, isSub");
				if (ChatHstrl.BadgeInfo != null && ChatHstrl.BadgeInfo != "") { sql.Append(", BadgeInfo"); }
				if (ChatHstrl.Badges != null && ChatHstrl.Badges != "") { sql.Append(", Badges"); }

				sql.Append(") VALUES ('" + ChatHstrl.Userid + "'");

				if (ChatHstrl.Msgid != null && ChatHstrl.Msgid != "") { sql.Append(", '" + ChatHstrl.Msgid + "'"); }
				sql.Append(", '" + ChatHstrl.Nick + "', '" + ChatHstrl.Msg + "', " + ChatHstrl.UnixTime.ToString() + ", '" + ChatHstrl.Color + "', " + ChatHstrl.isOwner.ToString() + ", " + ChatHstrl.isMod.ToString() + ", " + ChatHstrl.isVIP.ToString() + ", " + ChatHstrl.isSub.ToString());
				if (ChatHstrl.BadgeInfo != null && ChatHstrl.BadgeInfo != "") { sql.Append(", '" + ChatHstrl.BadgeInfo + "'"); }
				if (ChatHstrl.Badges != null && ChatHstrl.Badges != "") { sql.Append(", '" + ChatHstrl.Badges + "'"); }
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
				//db.TechFuncs.LogDH("Считывание из базы данных таблицы ChatHistory...");
				db.TechFuncs.LogDH("Попытка считывания из базы данных таблицы ChatHistory: этот функционал пока недоступен");
				/*SQLResultTable R = db.SQLQuery("SELECT * FROM ChatHistory;", false);
				if (R != null) {
					db.ChatHistoryList.Clear();
					while (R.NextRow()) {
						db.ChatHistoryList.Add(new ChatHistory_tclass(R.GetInt(0), R.GetStr(1), R.GetStr(2), R.GetStr(3), R.GetLong(4), R.GetStr(5), R.GetBool(6), R.GetBool(7), R.GetBool(8), R.GetBool(9), R.GetBool(10), R.GetStr(11), R.GetStr(12));
					}
				}*/
				//db.TechFuncs.LogDH("Считывание из базы данных таблицы ChatHistory завершено");
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
				SQLResultTable R = db.SQLQuery("INSERT INTO Funcs_Timers (Name, Time, Nick, DTStart, Notiflvl, Paused, Service) VALUES ('" + Timerl.Name + "', " + Timerl.Time.ToString() + ", '" + Timerl.Nick + "', " + Timerl.DTStart.ToString() + ", " + Timerl.Notiflvl.ToString() + ", " + Timerl.Paused.ToString() + ", " + Timerl.Service.ToString() + "); SELECT last_insert_rowid();", true);

				db.TimersList.Add(new Funcs_Timer_tclass(R.GetInt(0), Timerl.Name, Timerl.Time, Timerl.Nick, Timerl.DTStart, Timerl.DTPauseResume, Timerl.Notiflvl, Timerl.Paused, Timerl.Service));
				db.TechFuncs.LogDH("Добавлен таймер \"" + Timerl.Name + "\"");
				return R.GetInt(0);
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

			public bool PauseUpdate(string Name, bool Paused, long UnixTime) {
				int i = db.TimersList.FindIndex(x => x.Name == Name);
				Funcs_Timer_tclass Timerl = db.TimersList.Find(x => x.Name == Name);
				string DTPRorg = Timerl.DTPauseResume;

				if (Timerl != null && Timerl.Paused == !Paused) {
					if (Paused) {
						if (DTPRorg != null) {
							db.SQLQuery("UPDATE Funcs_Timers SET DTPauseResume = '" + DTPRorg + ";" + UnixTime.ToString() + "', Paused = 1 WHERE Name = '" + Name + "';");
						} else {
							db.SQLQuery("UPDATE Funcs_Timers SET DTPauseResume = '" + UnixTime.ToString() + "', Paused = 1 WHERE Name = '" + Name + "';");
						}

					} else {
						db.SQLQuery("UPDATE Funcs_Timers SET DTPauseResume = '" + DTPRorg + "," + UnixTime.ToString() + "', Paused = 0 WHERE Name = '" + Name + "';");
					}

					db.TimersList.RemoveAt(i);
					Timerl.Paused = Paused;
					if (Paused) {
						if (DTPRorg != null) {
							Timerl.DTPauseResume = DTPRorg + ";" + UnixTime.ToString();
						} else {
							Timerl.DTPauseResume = UnixTime.ToString();
						}
					} else {
						Timerl.DTPauseResume = DTPRorg + "," + UnixTime.ToString();
					}
					db.TimersList.Insert(i, Timerl);
					db.TechFuncs.LogDH("Изменена активность таймера \"" + Timerl.Name + "\"");
					return true;
				} else {
					return false;
				}
			}
			public bool Restart(string Name, long UnixTime) {
				int i = db.TimersList.FindIndex(x => x.Name == Name);
				Funcs_Timer_tclass Timerl = db.TimersList.ElementAt(i);
				if (Timerl != null) {

					if (Timerl.Paused) {
						db.SQLQuery("UPDATE Funcs_Timers SET DTPauseResume = '" + UnixTime + "', DTStart = " + UnixTime + " WHERE id = '" + Timerl.id + "';");
						Timerl.DTPauseResume = UnixTime.ToString();
					} else {
						db.SQLQuery("UPDATE Funcs_Timers SET DTPauseResume = null, DTStart = " + UnixTime + " WHERE id = '" + Timerl.id + "';");
						Timerl.DTPauseResume = null;
					}

					db.TimersList.RemoveAt(i);
					Timerl.DTStart = UnixTime;
					db.TimersList.Insert(i, Timerl);
					db.TechFuncs.LogDH("Сброшен таймер \"" + Timerl.Name + "\"");
					return true;
				} else {
					return false;
				}
			}

			public bool UpdateTime(string Name, long Seconds) {
				int i = db.TimersList.FindIndex(x => x.Name == Name);
				Funcs_Timer_tclass Timerl = db.TimersList.ElementAt(i);
				int val = db.SQLQuery("UPDATE Funcs_Timers SET Time = " + Seconds + " WHERE Name = '" + Name + "';");
				if (val > 0) {
					db.TimersList.RemoveAt(i);
					Timerl.Time = Seconds;
					db.TimersList.Insert(i, Timerl);
					db.TechFuncs.LogDH("Обновлён таймер \"" + Name + "\"");
					return true;
				} else {
					return false;
				}
			}

			public void UpdateListfromTable() {
				db.TechFuncs.LogDH("Считывание из базы данных таблицы Funcs_Timers...");
				SQLResultTable R = db.SQLQuery("SELECT * FROM Funcs_Timers;", false);
				if (R != null) {
					db.TimersList.Clear();
					while (R.NextRow()) {
						db.TimersList.Add(new Funcs_Timer_tclass(R.GetInt(0), R.GetStr(1), R.GetLong(2), R.GetStr(3), R.GetLong(4), R.GetStr(5), R.GetShort(6), R.GetBool(7), R.GetShort(8)));
					}
				}
				db.TechFuncs.LogDH("Считывание из базы данных таблицы Funcs_Timers завершено");
			}
		}



		// -- Класс для работы с таблицей Funcs_Stopwatches --
		public sealed class Funcs_StopwatchesTC {
			private DB db = null;
			public Funcs_StopwatchesTC(DB db) {	this.db = db; }

			public int Add(Funcs_Stopwatch_tclass Stopwatchl) {
				StringBuilder sql = new StringBuilder("INSERT INTO Funcs_Stopwatches (Name", 130);
				SQLResultTable R = null;
				if (Stopwatchl.Nick != null && Stopwatchl.Nick != "") { sql.Append(", Nick"); }
				sql.Append(", DTStart, Paused) VALUES ('" + Stopwatchl.Name + "'");

				if (Stopwatchl.Nick != null && Stopwatchl.Nick != "") { sql.Append(", '" + Stopwatchl.Nick + "'"); }
				sql.Append(", " + Stopwatchl.DTStart.ToString() + ", " + Stopwatchl.Paused.ToString() + "); SELECT last_insert_rowid();");

				R = db.SQLQuery(sql.ToString(), true);

				db.StopwatchesList.Add(new Funcs_Stopwatch_tclass(R.GetInt(0), Stopwatchl.Name, Stopwatchl.Nick, Stopwatchl.DTStart, Stopwatchl.DTPauseResume, Stopwatchl.Paused));
				db.TechFuncs.LogDH("Добавлен секундомер \"" + Stopwatchl.Name + "\"");
				return R.GetInt(0);
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

			public bool PauseUpdate(string Name, bool Paused, long UnixTime) {
				int i = db.StopwatchesList.FindIndex(x => x.Name == Name);
				Funcs_Stopwatch_tclass Stopwatchl = db.StopwatchesList.Find(x => x.Name == Name);
				string DTPRorg = Stopwatchl.DTPauseResume;

				if (Stopwatchl != null && Stopwatchl.Paused == !Paused) {
					if (Paused) {
						if (DTPRorg != null) {
							db.SQLQuery("UPDATE Funcs_Stopwatches SET DTPauseResume = '" + DTPRorg + ";" + UnixTime.ToString() + "', Paused = 1 WHERE Name = '" + Name + "';");
						} else {
							db.SQLQuery("UPDATE Funcs_Stopwatches SET DTPauseResume = '" + UnixTime.ToString() + "', Paused = 1 WHERE Name = '" + Name + "';");
						}
						
					} else {
						db.SQLQuery("UPDATE Funcs_Stopwatches SET DTPauseResume = '" + DTPRorg + "," + UnixTime.ToString() + "', Paused = 0 WHERE Name = '" + Name + "';");
					}

					db.StopwatchesList.RemoveAt(i);
					Stopwatchl.Paused = Paused;
					if (Paused) {
						if (DTPRorg != null) {
							Stopwatchl.DTPauseResume = DTPRorg + ";" + UnixTime.ToString();
						} else {
							Stopwatchl.DTPauseResume = UnixTime.ToString();
						}
					} else {
						Stopwatchl.DTPauseResume = DTPRorg + "," + UnixTime.ToString();
					}
					db.StopwatchesList.Insert(i, Stopwatchl);
					db.TechFuncs.LogDH("Изменена активность секундомера \"" + Stopwatchl.Name + "\"");
					return true;
				} else {
					return false;
				}
			}

			public bool Restart(string Name, long UnixTime) {
				int i = db.StopwatchesList.FindIndex(x => x.Name == Name);
				Funcs_Stopwatch_tclass Stopwatchl = db.StopwatchesList.ElementAt(i);
				if (Stopwatchl != null) {

					if (Stopwatchl.Paused) {
						db.SQLQuery("UPDATE Funcs_Stopwatches SET DTPauseResume = '" + UnixTime.ToString() + "', DTStart = " + UnixTime.ToString() + " WHERE id = '" + Stopwatchl.id + "';");
						Stopwatchl.DTPauseResume = UnixTime.ToString();
					} else {
						db.SQLQuery("UPDATE Funcs_Stopwatches SET DTPauseResume = null, DTStart = " + UnixTime.ToString() + " WHERE id = '" + Stopwatchl.id + "';");
						Stopwatchl.DTPauseResume = null;
					}
					
					db.StopwatchesList.RemoveAt(i);
					Stopwatchl.DTStart = UnixTime;
					db.StopwatchesList.Insert(i, Stopwatchl);
					db.TechFuncs.LogDH("Сброшен секундомер \"" + Stopwatchl.Name + "\"");
					return true;
				} else {
					return false;
				}
			}

			public bool UpdateTime(string Name, long Seconds, bool isAdd) {
				int i = db.StopwatchesList.FindIndex(x => x.Name == Name);
				Funcs_Stopwatch_tclass Stopwatchl = db.StopwatchesList.Find(x => x.Name == Name);
				string DTPR = null;
				string UnixTimeP = null;
				string TimeUp = null;

				if (Stopwatchl != null) {

					if (Stopwatchl.Paused) {
						if (Stopwatchl.DTPauseResume.Contains(";")) {
							DTPR = Stopwatchl.DTPauseResume.Substring(0, Stopwatchl.DTPauseResume.LastIndexOf(";") + 1);
							UnixTimeP = Stopwatchl.DTPauseResume.Substring(Stopwatchl.DTPauseResume.LastIndexOf(";") + 1);
						} else {
							UnixTimeP = Stopwatchl.DTPauseResume;
						}
					}

					if (isAdd) { TimeUp = "+" + Seconds; } else { TimeUp = "-" + Seconds; }

					if (Stopwatchl.Paused) {
						if (DTPR != null) {
							db.SQLQuery("UPDATE Funcs_Stopwatches SET DTPauseResume = '" + DTPR + TimeUp + ";" + UnixTimeP + "' WHERE Name = '" + Name + "';");
						} else {
							db.SQLQuery("UPDATE Funcs_Stopwatches SET DTPauseResume = '" + TimeUp + ";" + UnixTimeP + "' WHERE Name = '" + Name + "';");
						}
					} else {
						if (Stopwatchl.DTPauseResume != null) {
							db.SQLQuery("UPDATE Funcs_Stopwatches SET DTPauseResume = '" + Stopwatchl.DTPauseResume + ";" + TimeUp + "' WHERE Name = '" + Name + "';");
						} else {
							db.SQLQuery("UPDATE Funcs_Stopwatches SET DTPauseResume = '" + TimeUp + "' WHERE Name = '" + Name + "';");
						}
					}

					if (Stopwatchl.Paused) {
						if (DTPR != null) {
							Stopwatchl.DTPauseResume = DTPR + TimeUp + ";" + UnixTimeP;
						} else {
							Stopwatchl.DTPauseResume = TimeUp + ";" + UnixTimeP;
						}
					} else {
						if (Stopwatchl.DTPauseResume != null) {
							Stopwatchl.DTPauseResume = Stopwatchl.DTPauseResume + ";" + TimeUp;
						} else {
							Stopwatchl.DTPauseResume = TimeUp;
						}
					}
					db.StopwatchesList.RemoveAt(i);
					db.StopwatchesList.Insert(i, Stopwatchl);
					db.TechFuncs.LogDH("Изменено время секундомера \"" + Stopwatchl.Name + "\"");
					return true;
				} else {
					return false;
				}
			}	
			
			public void UpdateListfromTable() {
				db.TechFuncs.LogDH("Считывание из базы данных таблицы Funcs_Stopwatches...");
				SQLResultTable R = db.SQLQuery("SELECT * FROM Funcs_Stopwatches;", false);
				if (R != null) {
					db.StopwatchesList.Clear();
					while (R.NextRow()) {
						db.StopwatchesList.Add(new Funcs_Stopwatch_tclass(R.GetInt(0), R.GetStr(1), R.GetStr(2), R.GetLong(3), R.GetStr(4), R.GetBool(5)));
					}
				}
				db.TechFuncs.LogDH("Считывание из базы данных таблицы Funcs_Stopwatches завершено");
			}
		}



		// -- Класс для работы с таблицей Funcs_Counters --
		public sealed class Funcs_CountersTC {
			private DB db = null;
			public Funcs_CountersTC(DB db) { this.db = db; }

			public int Add(Funcs_Counter_tclass Counterl) {
				SQLResultTable R = null;
				R = db.SQLQuery("INSERT INTO Funcs_Counters (Name, Value) VALUES ('" + Counterl.Name + "', " + Counterl.Value.ToString() + "); SELECT last_insert_rowid();", true);
				
				db.CountersList.Add(new Funcs_Counter_tclass(R.GetInt(0), Counterl.Name, Counterl.Value));
				db.TechFuncs.LogDH("Добавлен счётчик \"" + Counterl.Name + "\"");
				return R.GetInt(0);
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
					db.SQLQuery("UPDATE Funcs_Counters SET Value = " + Value.ToString() + " WHERE id = '" + Counterl.id + "';");

					db.CountersList.RemoveAt(i);
					Counterl.Value = Value;
					db.CountersList.Insert(i, Counterl);
					return true;
				} else {
					return false;
				}
			}

			public bool Plus(string Name, int Value = 1) {
				int i = db.CountersList.FindIndex(x => x.Name == Name);
				Funcs_Counter_tclass Counterl = db.CountersList.ElementAt(i);
				if (Counterl != null) {
					Counterl.Value += Value;
					db.SQLQuery("UPDATE Funcs_Counters SET Value = " + Counterl.Value.ToString() + " WHERE id = '" + Counterl.id + "';");

					db.CountersList.RemoveAt(i);
					db.CountersList.Insert(i, Counterl);
					return true;
				} else {
					return false;
				}
			}

			public bool Minus(string Name, int Value = 1) {
				int i = db.CountersList.FindIndex(x => x.Name == Name);
				Funcs_Counter_tclass Counterl = db.CountersList.ElementAt(i);
				if (Counterl != null) {
					Counterl.Value -= Value;
					db.SQLQuery("UPDATE Funcs_Counters SET Value = " + Counterl.Value.ToString() + " WHERE id = '" + Counterl.id + "';");

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
					db.SQLQuery("UPDATE Funcs_Counters SET Value = 0 WHERE id = '" + Counterl.id + "';");

					db.CountersList.RemoveAt(i);
					Counterl.Value = 0;
					db.CountersList.Insert(i, Counterl);
					return true;
				} else {
					return false;
				}
			}

			public void UpdateListfromTable() {
				db.TechFuncs.LogDH("Считывание из базы данных таблицы Funcs_Counters...");
				SQLResultTable R = db.SQLQuery("SELECT * FROM Funcs_Counters;", false);
				if (R != null) {
					db.CountersList.Clear();
					while (R.NextRow()) {
						db.CountersList.Add(new Funcs_Counter_tclass(R.GetInt(0), R.GetStr(1), R.GetInt(2)));
					}
				}
				db.TechFuncs.LogDH("Считывание из базы данных таблицы Funcs_Counters завершено");
			}
		}



		// -- Класс для работы с таблицей VIPUsers --
		public sealed class VIPUsersTC {
			private DB db = null;
			public VIPUsersTC(DB db) { this.db = db; }

			public int Add(VIPUsers_tclass VIPUsersl) {
				SQLResultTable R = null;
				R = db.SQLQuery("INSERT INTO VIPUsers (Nick, Userid) VALUES ('" + VIPUsersl.Nick + "', '" + VIPUsersl.Userid + "'); SELECT last_insert_rowid();", true);

				db.VIPUsersList.Add(new VIPUsers_tclass(R.GetInt(0), VIPUsersl.Nick, VIPUsersl.Userid));
				db.TechFuncs.LogDH("Добавлен вип-пользователь \"" + VIPUsersl.Nick + "\" (Userid - " + VIPUsersl.Userid + ")");
				return R.GetInt(0);
			}

			public bool Remove(string Nick) {
				int val = db.SQLQuery("DELETE FROM VIPUsers WHERE Nick = '" + Nick + "';");
				if (val > 0) {
					db.VIPUsersList.RemoveAt(db.VIPUsersList.FindIndex(x => x.Nick == Nick));
					db.TechFuncs.LogDH("Удалён вип-пользователь с ником \"" + Nick + "\"");
					return true;
				} else {
					return false;
				}
			}

			/*public bool Remove(string Userid) {
				int val = db.SQLQuery("DELETE FROM VIPUsers WHERE Userid = '" + Userid + "';");
				if (val > 0) {
					db.VIPUsersList.RemoveAt(db.VIPUsersList.FindIndex(x => x.Userid == Userid));
					db.TechFuncs.LogDH("Удалён вип-чатер с Userid \"" + Userid + "\"");
					return true;
				} else {
					return false;
				}
			}*/

			public void Wipe() {
				db.SQLQuery("DELETE FROM VIPUsers;");
				db.VIPUsersList.Clear();
				db.TechFuncs.LogDH("Очищен список вип-пользователей");
			}

			public bool UpdateNicks(List<VIPUsers_tclass> VIPUsersl) {
				int val;
				for (int i = 0; i < VIPUsersl.Count; i++) {
					val = db.SQLQuery("UPDATE VIPUsers SET Nick = '" + VIPUsersl.ElementAt(i).Nick + "' WHERE Userid = '" + VIPUsersl.ElementAt(i).Userid + "';");
					db.TechFuncs.LogDH("Обновлён ник вип-пользователя с Userid \"" + VIPUsersl.ElementAt(i).Userid + "\" на \"" + VIPUsersl.ElementAt(i).Nick + "\"");
					if (val == 0) {
						db.TechFuncs.LogDH("Ошибка обновления ника вип-пользователя с Userid \"" + VIPUsersl.ElementAt(i).Userid + "\" - не найден");
						return false;
					}
				}
				return true;
			}

			public void UpdateListfromTable() {
				db.TechFuncs.LogDH("Считывание из базы данных таблицы VIPUsers...");
				SQLResultTable R = db.SQLQuery("SELECT * FROM VIPUsers;", false);
				if (R != null) {
					db.VIPUsersList.Clear();
					while (R.NextRow()) {
						db.VIPUsersList.Add(new VIPUsers_tclass(R.GetInt(0), R.GetStr(1), R.GetInt(2)));
					}
				}
				db.TechFuncs.LogDH("Считывание из базы данных таблицы VIPUsers завершено");
			}
		}


	}
}