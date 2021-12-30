using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Data.Entity;
//using System.Data.Common;
//using System.Data.Entity.Core.Metadata.Edm;

namespace Twidibot {

	/*
	public class DB : DbContext {

		public event EventHandler<CEvent_Null> Ev_Save;
		public event EventHandler<CEvent_Msg> Ev_Error;
		private bool UpdateCheck = false;

		public DB() : base("DBCon") { // -- Непосредственно подключения к базе
									  //Task.Factory.StartNew(() => dbUpdate());
		}



		//private void dbUpdate() {
		//	while (true) {
		//		if (UpdateCheck) {
		//			this.SaveChanges();
		//			UpdateCheck = false;
		//			if (Ev_Save != null) { Ev_Save(this, new CEvent_Null()); }
		//		}
		//		Thread.Sleep(100);
		//	}
		//}


		// -- Функция изменения записи --
		public void RecEdit() {

		}



		// -- Поля для работы с таблицами --
		// -- Settings
		private DbSet<Setting> settings;
		public DbSet<Setting> Settings {
			get { return this.settings; }
			set {
				this.settings = value;
				this.UpdateCheck = true;
			}
		}

		// -- DefCommands
		private DbSet<DefCommand> defCommands;
		public DbSet<DefCommand> DefCommands {
			get { return this.defCommands; }
			set {
				this.defCommands = value;
				UpdateCheck = true;
			}
		}

		// -- FuncCommands
		private DbSet<FuncCommand> funcCommands;
		public DbSet<FuncCommand> FuncCommands {
			get { return this.funcCommands; }
			set {
				this.funcCommands = value;
				UpdateCheck = true;
			}
		}

		// -- ChatHistory
		private DbSet<ChatHistory> chatHistorys;
		public DbSet<ChatHistory> ChatHistorys {
			get { return this.chatHistorys; }
			set {
				this.chatHistorys = value;
				UpdateCheck = true;
			}
		}

		// -- ChangeLog
		private DbSet<ChangeLog> changeLogs;
		public DbSet<ChangeLog> ChangeLogs {
			get { return this.changeLogs; }
			set {
				this.changeLogs = value;
				UpdateCheck = true;
			}
		}

		// -- SpamMessage
		private DbSet<SpamMessage> spamMessages;
		public DbSet<SpamMessage> SpamMessages {
			get { return this.spamMessages; }
			set {
				this.spamMessages = value;
				UpdateCheck = true;
			}
		}



		// -- Классы таблиц --
		// -- Settings
		public class Setting {
			public int id { get; set; }

			public string SetName { get; set; }
			public string Param { get; set; }

			public Setting() { }

			public Setting(string Setting) {
				this.SetName = Setting;
			}
			public Setting(string Setting, string Parameter) {
				this.SetName = Setting;
				this.Param = Parameter;
			}
		}

		// -- DefCommand
		public class DefCommand {
			public int id { get; set; }
			public string Command { get; set; }
			public string Result { get; set; }
			public int CoolDown { get; set; }
			public string LastUsed { get; set; }


			public DefCommand() { }

			public DefCommand(string Command, string Result, int CoolDown = 0, string LastUsed = null) {
				this.Command = Command;
				this.Result = Result;
				this.CoolDown = CoolDown;
				this.LastUsed = LastUsed;
			}
		}

		// -- FuncCommand
		public class FuncCommand {
			public int id { get; set; }
			public string Command { get; set; }
			public string FuncName { get; set; }
			public int CoolDown { get; set; }
			public string LastUsed { get; set; }
			public string Params { get; set; }
			public bool Enabled { get; set; }

			public FuncCommand() { }

			public FuncCommand(string Command, string FuncName, string Params = null, int CoolDown = 0, string LastUsed = null, bool Enabled = true) {
				this.Command = Command;
				this.FuncName = FuncName;
				this.CoolDown = CoolDown;
				this.LastUsed = LastUsed;
				this.Params = Params;
				this.Enabled = Enabled;
			}
		}

		// -- ChatHistory
		public class ChatHistory {
			public int id { get; set; }
			public string Nick { get; set; }
			public string Msg { get; set; }
			public string Date { get; set; }
			public string Time { get; set; }
			public bool isDel { get; set; }
			public bool isMod { get; set; }
			public bool isVIP { get; set; }
			public bool isSub { get; set; }

			public ChatHistory() { }

			public ChatHistory(string Nick, string Msg, string Date, string Time, bool Deleted = false, bool Moderator = false, bool VIP = false, bool Subscriber = false) {
				this.Nick = Nick;
				this.Msg = Msg;
				this.Date = Date;
				this.Time = Time;
				this.isDel = Deleted;
				this.isMod = Moderator;
				this.isVIP = VIP;
				this.isSub = Subscriber;
			}
		}

		// -- ChangeLog
		public class ChangeLog {
			public int id { get; set; }
			public string Version { get; set; }
			public string Desc { get; set; }

			public ChangeLog() { }

			public ChangeLog(string Version, string Desc) {
				this.Version = Version;
				this.Desc = Desc;
			}
		}

		// -- SpamMessages
		public class SpamMessage {
			public int id { get; set; }
			public string Message { get; set; }
			public int CoolDown { get; set; }

			public SpamMessage() { }

			public SpamMessage(string Message, int CoolDown) {
				this.Message = Message;
				this.CoolDown = CoolDown;
			}
		}
	}
	*/
}
