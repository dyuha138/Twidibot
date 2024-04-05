using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Twidibot
{
	// -- Класс эвента, который просто перенаправляет сообщение --
	public class Twident_Msg : EventArgs {
		public readonly string Message;
		public Twident_Msg(string msg) { Message = msg; }
	}


	// -- Класс эвента для отправки сообщений из чатов --
	public class Twident_ChatMsg : EventArgs {
		public readonly int ServiceType;
		public readonly string Msgid;
		public readonly string Nick;
		public readonly string DispNick;
		public readonly int Userid;
		public readonly string Msg;
		public readonly long UnixTime;
		public readonly string Color;
		public readonly bool isOwner;
		public readonly bool isMod;
		public readonly bool isVIP;
		public readonly bool isSub;
		public readonly string BadgeInfo;
		public readonly string Badges;
		public Twident_ChatMsg(int ServiceType, string Msgid, string Nick, string DispNick, int Userid, string Msg, long UnixTime, string Color, bool isOwner = false, bool isMod = false, bool isVIP = false, bool isSub = false, string BadgeInfo = null, string Badges = null) {
			this.ServiceType = ServiceType;
			this.Msgid = Msgid;
			this.Nick = Nick;
			this.DispNick = DispNick;
			this.Userid = Userid;
			this.Msg = Msg;
			this.UnixTime = UnixTime;
			this.Color = Color;
			this.isOwner = isOwner;
			this.isMod = isMod;
			this.isVIP = isVIP;
			this.isSub = isSub;
			this.BadgeInfo = BadgeInfo;
			this.Badges = Badges;
		}
	}

	// -- Впомогательный класс эвента для удалённого сообщения
	public class Twident_ChatMsgDel : EventArgs {
		public readonly int ServiceType;
		public readonly string Msgid;
		public readonly string Nick;
		public readonly string DispNick;
		public readonly int Userid;
		public readonly string Msg;
		public readonly long UnixTime;

		public Twident_ChatMsgDel(int ServiceType, string Msgid, string Nick, string DispNick, int Userid, string Msg, long UnixTime) {
			this.ServiceType = ServiceType;
			this.Msgid = Msgid;
			this.Nick = Nick;
			this.DispNick = DispNick;
			this.Userid = Userid;
			this.Msg = Msg;
			this.UnixTime = UnixTime;
		}
	}



	// -- Класс эвента для статуса подключения, принимает вебсокетстейт, а выдаёт русское слово (после введения глобального статуса не актуален) --
	/*public class Twident_ChatStatus : EventArgs {
		public readonly string Msg;
		public Twident_ChatStatus(WebSocket4Net.WebSocketState socketState) {
			if (socketState == WebSocket4Net.WebSocketState.Connecting) {
				Msg = "Подключение...";
			} else {
				if (socketState == WebSocket4Net.WebSocketState.Open) {
					Msg = "Подключено";
				} else {
					if (socketState == WebSocket4Net.WebSocketState.Closing) {
						Msg = "Отключение...";
					} else {
						if (socketState == WebSocket4Net.WebSocketState.Closed) {
							Msg = "Отключено";
						} else {
							if (socketState == WebSocket4Net.WebSocketState.None) {
								Msg = "Не подключено";
							} else {
								Msg = "Неизвестная ошибка";
							}
						}
					}
				}
			}
		}
	}*/


	// -- Класс эвента, который ничего не передаёт, чтобы просто что-то вызвать --
	public class Twident_Null : EventArgs {
		public Twident_Null() { }
	}



	// -- Класс эвента статуса работы --
	/// <summary>
	/// Эвент статуса работы
	/// </summary>
	/// <returns>
	/// Принятые в проекте коды сообщения<br></br>
	/// 0 - обычное сообщение (для прогресса, например)<br></br>
	/// 1 - успех (сообщение обычно будет гореть зелёным цветом)<br></br>
	/// 2 - некритичная ошибка (жёлтый)<br></br>
	/// 3 - критическая ошибка (красный)<br></br>
	/// </returns>
	public class Twident_Status : EventArgs {
		public readonly int StatusCode;
		public readonly string Message;
		public readonly string Description;
		public readonly bool? Permanent;
		public Twident_Status(int StatusCode, string Message, string Description, bool? Permanent) {
			this.StatusCode = StatusCode;
			this.Message = Message;
			this.Description = Description;
			this.Permanent = Permanent;
		}
	}



	// -- Класс эвента для передачи була --
	public class Twident_Bool : EventArgs {
		public readonly bool Handle;
		public Twident_Bool(bool Handle) {
			this.Handle = Handle;
		}
	}

}
