using System;
using System.Collections.Generic;
using System.Text;

namespace Twidibot {

	public partial class DB {
		// -- Классы, повторяющие таблицы --
		// -- Settings (включает как и общие настройки, так и для каждой из платформ)
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
			public bool isAlias { get; set; }
			public bool Enabled { get; set; }
			public long LastUsed { get; set; }

			public DefCommand_tclass() { }
			public DefCommand_tclass(string Command, string Result, int CoolDown = 0, bool isAlias = false, bool Enabled = true, long LastUsed = 0) {
				this.Command = Command;
				this.Result = Result;
				this.CoolDown = CoolDown;
				this.isAlias = isAlias;
				this.Enabled = Enabled;
				this.LastUsed = LastUsed;
			}
			public DefCommand_tclass(int id, string Command, string Result, int CoolDown = 0, bool isAlias = false, bool Enabled = true, long LastUsed = 0) {
				this.id = id;
				this.Command = Command;
				this.Result = Result;
				this.CoolDown = CoolDown;
				this.isAlias = isAlias;
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
			public long LastUsed { get; set; }

			public FuncCommand_tclass() { }
			public FuncCommand_tclass(string Command, string FuncName, string Desc = null, string Params = null, int CoolDown = 0, bool Enabled = true, bool Secured = false, long LastUsed = 0) {
				this.Command = Command;
				this.FuncName = FuncName;
				this.Desc = Desc;
				this.Params = Params;
				this.CoolDown = CoolDown;
				this.Enabled = Enabled;
				this.Secured = Secured;
				this.LastUsed = LastUsed;
			}
			public FuncCommand_tclass(int id, string Command, string FuncName, string Desc = null, string Params = null, int CoolDown = 0, bool Enabled = true, bool Secured = false, long LastUsed = 0) {
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
			public long LastUsed { get; set; }

			public SpamMessage_tclass() { }
			public SpamMessage_tclass(string Message, int CoolDown = 0, bool Enabled = true, long LastUsed = 0) {
				this.Message = Message;
				this.CoolDown = CoolDown;
				this.Enabled = Enabled;
				this.LastUsed = LastUsed;
			}
			public SpamMessage_tclass(int id, string Message, int CoolDown = 0, bool Enabled = true, long LastUsed = 0) {
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
			public int ServiceType { get; set; }
			public int Userid { get; set; }
			public string Msgid { get; set; }
			public string Nick { get; set; }
			public string DispNick { get; set; }
			public string Msg { get; set; }
			public long UnixTime { get; set; }
			public string Color { get; set; }
			public bool isOwner { get; set; }
			public bool isDel { get; set; }
			public bool isMod { get; set; }
			public bool isVIP { get; set; }
			public bool isSub { get; set; }
			public string BadgeInfo { get; set; }
			public string Badges { get; set; }


			public ChatHistory_tclass() { }
			public ChatHistory_tclass(int ServiceType, int Userid, string Msgid, string Nick, string DispNick, string Msg, long UnixTime, string Color, bool Owner = false, bool Deleted = false, bool Moderator = false, bool VIP = false, bool Subscriber = false, string BadgeInfo = null, string Badges = null) {
				this.Userid = Userid;
				this.ServiceType = ServiceType;
				this.Msgid = Msgid;
				this.Nick = Nick;
				this.DispNick = DispNick;
				this.Msg = Msg;
				this.UnixTime = UnixTime;
				this.Color = Color;
				this.isOwner = Owner;
				this.isDel = Deleted;
				this.isMod = Moderator;
				this.isVIP = VIP;
				this.isSub = Subscriber;
				this.BadgeInfo = BadgeInfo;
				this.Badges = Badges;
			}
			public ChatHistory_tclass(int id, int ServiceType, int Userid, string Msgid, string Nick, string DispNick, string Msg, long UnixTime, string Color, bool Owner = false, bool Deleted = false, bool Moderator = false, bool VIP = false, bool Subscriber = false, string BadgeInfo = null, string Badges = null) {
				this.id = id;
				this.ServiceType = ServiceType;
				this.Userid = Userid;
				this.Msgid = Msgid;
				this.Nick = Nick;
				this.DispNick = DispNick;
				this.Msg = Msg;
				this.UnixTime = UnixTime;
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


		// -- Funcs_Timers
		public class Funcs_Timer_tclass {
			public int id { get; set; }
			public string Name { get; set; }
			public long Time { get; set; }
			public string Nick { get; set; }
			public long DTStart { get; set; }
			public string DTPauseResume { get; set; }
			public short Notiflvl { get; set; }
			public bool Paused { get; set; }
			public short Service { get; set; }

			public Funcs_Timer_tclass() { }
			public Funcs_Timer_tclass(string Name, long Time, string Nick, long DTStart, string DTPauseResume = null, short Notiflvl = 1, bool Paused = false, short Service = 0) {
				this.Name = Name;
				this.Time = Time;
				this.Nick = Nick;
				this.DTStart = DTStart;
				this.DTPauseResume = DTPauseResume;
				this.Notiflvl = Notiflvl;
				this.Paused = Paused;
				this.Service = Service;
			}
			public Funcs_Timer_tclass(int id, string Name, long Time, string Nick, long DTStart, string DTPauseResume = null, short Notiflvl = 1, bool Paused = false, short Service = 0) {
				this.id = id;
				this.Name = Name;
				this.Time = Time;
				this.Nick = Nick;
				this.DTStart = DTStart;
				this.DTPauseResume = DTPauseResume;
				this.Notiflvl = Notiflvl;
				this.Paused = Paused;
				this.Service = Service;
			}
		}


		// -- Funcs_Stopwatches
		public class Funcs_Stopwatch_tclass {
			public int id { get; set; }
			public string Name { get; set; }
			public string Nick { get; set; }
			public long DTStart { get; set; }
			public string DTPauseResume { get; set; }
			public bool Paused { get; set; }

			public Funcs_Stopwatch_tclass() { }
			public Funcs_Stopwatch_tclass(string Name, string Nick, long DTStart, string DTPauseResume = null, bool Paused = false) {
				this.Name = Name;
				this.Nick = Nick;
				this.DTStart = DTStart;
				this.DTPauseResume = DTPauseResume;
				this.Paused = Paused;
			}
			public Funcs_Stopwatch_tclass(int id, string Name, string Nick, long DTStart, string DTPauseResume = null, bool Paused = false) {
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


		// -- VIPUsers
		public class VIPUsers_tclass {
			public int id { get; set; }
			public string Nick { get; set; }
			public int Userid { get; set; }

			public VIPUsers_tclass() { }
			public VIPUsers_tclass(string Nick, int Userid) {
				this.Nick = Nick;
				this.Userid = Userid;
			}
			public VIPUsers_tclass(int id, string Nick, int Userid) {
				this.id = id;
				this.Nick = Nick;
				this.Userid = Userid;
			}
		}
	}





	public class UserInfoFull {
		public string id { get; set; }
		public string Name { get; set; }
		public string Color { get; set; }
		public string Badges { get; set; }
		public PermissionsInfo Permissions { get; set; }
		public TWHInfo TWH { get; set; }
		public VKPLInfo VKPL { get; set; }


		public UserInfoFull() {
			this.TWH = new TWHInfo();
			this.VKPL = new VKPLInfo();
			this.Permissions = new PermissionsInfo();
		}
		private UserInfoFull(bool noperm) {
			this.TWH = new TWHInfo();
			this.VKPL = new VKPLInfo();
		}
		public UserInfoFull(string Name, string Color, string Badges) : this() {
			this.Name = Name;
			this.Color = Color;
			this.Badges = Badges;
		}
		public UserInfoFull(string Name, string Color, string Badges, bool isBrodcaster = false, bool isModerator = false, bool isVIP = false, bool isSubscriber = false, bool isFolower = false) : this(true) {
			this.Name = Name;
			this.Color = Color;
			this.Badges = Badges;
			this.Permissions = new PermissionsInfo(isBrodcaster, isModerator, isVIP, isSubscriber, isFolower);
		}
		public UserInfoFull(string Name, string Color, string Badges, string TWH_DisplayName, string TWH_BadgesInfo, string TWH_ClientNonce, bool isBrodcaster = false, bool isModerator = false, bool isVIP = false, bool isSubscriber = false, bool isFolower = false) {
			this.Name = Name;
			this.Color = Color;
			this.Badges = Badges;
			this.TWH = new TWHInfo(TWH_DisplayName, TWH_BadgesInfo, TWH_ClientNonce);
			this.Permissions = new PermissionsInfo(isBrodcaster, isModerator, isVIP, isSubscriber, isFolower);
		}
		public UserInfoFull(string Name, string Color, string Badges, string VKPL_yes, bool isBrodcaster = false, bool isModerator = false, bool isVIP = false, bool isSubscriber = false, bool isFolower = false) {
			this.Name = Name;
			this.Color = Color;
			this.Badges = Badges;
			this.VKPL = new VKPLInfo(VKPL_yes);
			this.Permissions = new PermissionsInfo(isBrodcaster, isModerator, isVIP, isSubscriber, isFolower);
		}

		public class TWHInfo {
			public string DisplayName { get; set; }
			public string BadgesInfo { get; set; }
			public string ClientNonce { get; set; }

			public TWHInfo() { }
			public TWHInfo(string DisplayName, string BadgesInfo, string ClientNonce) {
				this.DisplayName = DisplayName;
				this.BadgesInfo = BadgesInfo;
				this.ClientNonce = ClientNonce;
			}
		}

		public class VKPLInfo {
			public string CopyCount { get; set; }
			public VKPLInfo() { }
			public VKPLInfo(string CopyCount) {
				this.CopyCount = CopyCount;
			}
		}
	}



	public class UserInfoLite {
		public string id { get; set; }
		public string Name { get; set; }
		public string Color { get; set; }
		public string Badges { get; set; }

		public UserInfoLite() { }
		public UserInfoLite(string id, string Name, string Color, string Badges) {
			this.id = id;
			this.Name = Name;
			this.Color = Color;
			this.Badges = Badges;
		}
	}



	public class PermissionsInfo {
		private bool pisBrod = false;
		private bool pisMod = false;
		private bool pisVIP = false;
		public bool isBrodcaster {
			get { return this.pisBrod; }
			set {
				if (value) {
					this.pisMod = false;
					this.pisBrod = true;
				} else {
					this.pisBrod = false;
				}
			}
		}
		public bool isModerator {
			get { return this.pisMod; }
			set {
				if (value && (!this.pisBrod || !this.pisMod)) {
					this.pisMod = true;
				} else {
					this.pisMod = false;
				}
			}
		}
		public bool isVIP {
			get { return this.pisVIP; }
			set {
				if (value && (!this.pisBrod || !this.pisMod)) {
					this.pisVIP = true;
				} else {
					this.pisVIP = false;
				}
			}
		}
		public bool isSubscriber { get; set; }
		public bool isFolower { get; set; }

		public PermissionsInfo() { }
		public PermissionsInfo(bool isBrodcaster, bool isModerator, bool isVIP, bool isSubscriber, bool isFolower) {
			this.isBrodcaster = isBrodcaster;
			this.isModerator = isModerator;
			this.isVIP = isVIP;
			this.isSubscriber = isSubscriber;
			this.isFolower = isFolower;
		}

	}



	public class ChatMessageFull {
		public string id { get; set; }
		public UserInfoFull UserInfo { get; set; }
		//public PermissionsInfo Permissions { get; set; }
		public string Smiles { get; set; }
		public string Flags { get; set; }
		public string Message { get; set; }
		public string Type { get; set; }

		public ChatMessageFull() { }
		public ChatMessageFull(string id, UserInfoFull UserInfo, string Smiles, string Flags, string Message, string Type) {
			this.id = id;
			this.UserInfo = UserInfo;
			//this.UserPermissions = UserPermissions;
			this.Smiles = Smiles;
			this.Flags = Flags;
			this.Message = Message;
			this.Type = Type;
		}
	}

	public class ChatMessageLite {
		public string id { get; set; }
		public UserInfoLite UserInfo { get; set; }
		//public PermissionsInfo UserPermissions { get; set; }
		public string Smiles { get; set; }
		public string Flags { get; set; }
		public string Message { get; set; }
		public string Type { get; set; }

		public ChatMessageLite() { }
		public ChatMessageLite(string id, UserInfoLite UserInfo, string Smiles, string Flags, string Message, string Type) {
			this.id = id;
			this.UserInfo = UserInfo;
			//this.UserPermissions = UserPermissions;
			this.Smiles = Smiles;
			this.Flags = Flags;
			this.Message = Message;
			this.Type = Type;
		}
	}


	public class ServiceInfo {
		public short id { get; set; }
		public PermissionsInfo BotPermissions { get; set; }

		public ServiceInfo() { }
		public ServiceInfo(short id, PermissionsInfo BotPermissions) {
			this.id = id;
			this.BotPermissions = BotPermissions;
		}
	}



	// -- Класс для отправляемых сообщений (в частности, чтобы делать полноценные обращения в браузерной версии VKPL)
	public class VKPLsendmsg {
		public string[] users { get; set; }
		public string msg { get; set; }

		public VKPLsendmsg() { }
		public VKPLsendmsg(string[] Users, string Message) {
			this.users = Users;
			this.msg = Message;
		}
	}

}


