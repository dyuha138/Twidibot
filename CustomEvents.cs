using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Twidibot
{
	// -- Эвент, который просто перенаправляет сообщение --
	public class CEvent_Msg : EventArgs {
		public readonly string Message;
		public CEvent_Msg(string msg) { Message = msg; }
	}


	// -- Эвент для отправки сообщений из чата --
	public class CEvent_ChatMsg : EventArgs {
		public readonly string Msgid;
		public readonly string Nick;
		public readonly int Userid;
		public readonly string Msg;
		public readonly string Date;
		public readonly string Time;
		public readonly string Color;
		public readonly bool isOwner;
		public readonly bool isMod;
		public readonly bool isVIP;
		public readonly bool isSub;
		public readonly string BadgeInfo;
		public readonly string Badges;
		public CEvent_ChatMsg(string Msgid, string Nick, int Userid, string Msg, string Date, string Time, string Color, bool isOwner = false, bool isMod = false, bool isVIP = false, bool isSub = false, string BadgeInfo = null, string Badges = null) {
			this.Msgid = Msgid;
			this.Nick = Nick;
			this.Userid = Userid;
			this.Msg = Msg;
			this.Date = Date;
			this.Time = Time;
			this.Color = Color;
			this.isOwner = isOwner;
			this.isMod = isMod;
			this.isVIP = isVIP;
			this.isSub = isSub;
			this.BadgeInfo = BadgeInfo;
			this.Badges = Badges;
		}
	}


	// -- Эвент для статуса подключения, принимает вебсокетстейт, а выдаёт русское слово --
	public class CEvent_ChatStatus : EventArgs {
		public readonly string Msg;
		public CEvent_ChatStatus(WebSocket4Net.WebSocketState socketState) {
			if (socketState == WebSocket4Net.WebSocketState.Connecting) {
				Msg = "Подключение...";
			} else {
				if (socketState == WebSocket4Net.WebSocketState.Open) {
					Msg = "Подключено";
				} else {
					if (socketState == WebSocket4Net.WebSocketState.Closing) {
						Msg = "Остановка...";
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
	}


	// -- Эвент, который ничего не передаёт --
	public class CEvent_Null : EventArgs {
		public CEvent_Null() { }
	}


}
