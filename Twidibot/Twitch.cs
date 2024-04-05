using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Timers;
using WebSocket4Net;
using System.Windows;
using System.Globalization;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Interop;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Ink;
using RestSharp;

namespace Twidibot {
	public partial class Twitch {
		private BackWin TechF = null;
		public ChatC Chat = null;
		public APIC API = null;
		public bool Active = false;
		public bool ManualMode = false;
		public bool BotChannelisOne = false;
		/*public bool BotChannelisOne {
			get { return botChannelisOne; }
			set	{
				botChannelisOne = value;
				if (value) {
					Twitch.API.Req_UpdateBotChannelInfo_login(false);
				} else {
					db.SettingsTWHT.UpdateSetting("Channel", null);
					db.SettingsTWHT.UpdateSetting("ChannelId", null);
					db.SettingsTWHT.UpdateSetting("ChannelDisp", null);
				}
			}
		}*/


		public Twitch(BackWin backWin) {
			TechF = backWin;
			Chat = new ChatC(backWin);
			API = new APIC(backWin);
			UpdateActive();
		}


		public void UpdateActive() {
			if (TechF.TechFuncs.GetSettingParam("TwitchActive") == "1") { this.Active = true; } else { this.Active = false; }
		}


		public class APIC {
			private BackWin TechF = null;

			// -- Данные авторизации --
			private string Client_Id = "frnj4piu30ipfdkdfwz8i8zr1p990m";
			private string Client_Secret = "";

			public bool useDefToken = false;
			public bool tokens_work = false;

			public event EventHandler<Twident_Status> Ev_Status;
			public event EventHandler<Twident_Status> Ev_GlobalStatus;
			public event EventHandler<Twident_Null> Ev_LoginBotD;
			public event EventHandler<Twident_Null> Ev_LoginChD;


			// -- Стандартные аунтентификационные данные
			/*private string def_authCode = null;
			private string def_AuthCode {
				get { return def_authCode; }
				set	{ def_authCode = value;
					TechF.db.SettingsTWHT.UpdateSetting("Def_AuthCode", value);
				}
			}*/

			private string def_access_Token = null;
			private string def_Access_Token {
				get { return def_access_Token; }
				set {
					def_access_Token = value;
					TechF.db.SettingsTWHT.UpdateSetting("Def_Access_Token", value);
				}
			}

			private string def_refresh_Token = null;
			private string def_Refresh_Token {
				get { return def_refresh_Token; }
				set {
					def_refresh_Token = value;
					TechF.db.SettingsTWHT.UpdateSetting("Def_Refresh_Token", value);
				}
			}

			private int def_tokenTime = 0;
			private int def_TokenTime {
				get { return def_tokenTime; }
				set {
					def_tokenTime = value;
					TechF.db.SettingsTWHT.UpdateSetting("Def_TokenTime", value.ToString());
				}
			}



			// -- Аутентификационные данные бота
			private string bot_authCode = null;
			private string bot_AuthCode {
				get { return bot_authCode; }
				set	{
					bot_authCode = value;
					TechF.db.SettingsTWHT.UpdateSetting("Bot_AuthCode", value);
				}
			}

			private string bot_access_Token = null;
			private string bot_Access_Token {
				get { return bot_access_Token; }
				set	{
					bot_access_Token = value;
					TechF.db.SettingsTWHT.UpdateSetting("Bot_Access_Token", value);
				}
			}

			private string bot_refresh_Token = null;
			private string bot_Refresh_Token {
				get { return bot_refresh_Token; }
				set	{
					bot_refresh_Token = value;
					TechF.db.SettingsTWHT.UpdateSetting("Bot_Refresh_Token", value);
				}
			}

			private int bot_tokenTime = 0;
			private int bot_TokenTime {
				get { return bot_tokenTime; }
				set	{
					bot_tokenTime = value;
					TechF.db.SettingsTWHT.UpdateSetting("Bot_TokenTime", value.ToString());
				}
			}


			private string ch_authCode = null;
			private string ch_AuthCode {
				get { return ch_authCode; }
				set	{
					ch_authCode = value;
					TechF.db.SettingsTWHT.UpdateSetting("Channel_AuthCode", value);
				}
			}

			private string ch_access_Token = null;
			private string ch_Access_Token {
				get { return ch_access_Token; }
				set	{
					ch_access_Token = value;
					TechF.db.SettingsTWHT.UpdateSetting("Channel_Access_Token", value);
				}
			}

			private string ch_refresh_Token = null;
			private string ch_Refresh_Token {
				get { return ch_refresh_Token; }
				set	{
					ch_refresh_Token = value;
					TechF.db.SettingsTWHT.UpdateSetting("Channel_Refresh_Token", value);
				}
			}

			private int ch_tokenTime = 0;
			private int ch_TokenTime {
				get { return ch_tokenTime; }
				set	{
					ch_tokenTime = value;
					TechF.db.SettingsTWHT.UpdateSetting("Channel_TokenTime", value.ToString());
				}
			}


			private List<string> ScopesforReq = null;
			private List<string> ScopesforChat = null;
			RestClient ReqClient = null;


			public APIC(BackWin backWin) {
				this.TechF = backWin;
				this.ScopesforReq = new List<string>();
				this.ScopesforChat = new List<string>();
				ReqClient = new RestClient("https://api.twitch.tv/helix");

				ReqClient.AddDefaultHeader("Client-Id", Client_Id);
				ReqClient.AddDefaultHeader("User-Agent", "Twidibot/" + TechF.AppVersion);

				if (TechF.TechFuncs.GetSettingTWHParam("Bot_Access_Token") == "false") { useDefToken = true; }

				//def_authCode = TechF.db.SettingsTWHT.GetParam("Def_AuthCode");
				//def_access_Token = TechF.db.SettingsTWHT.GetParam("Def_Access_Token");
				//def_refresh_Token = TechF.db.SettingsTWHT.GetParam("Def_Refresh_Token");
				//def_tokenTime = Convert.ToInt32(TechF.db.SettingsTWHT.GetParam("Def_TokenTime"));

				//bot_authCode = TechF.db.SettingsTWHT.GetParam("Bot_AuthCode");
				//bot_access_Token = TechF.db.SettingsTWHT.GetParam("Bot_Access_Token");
				//bot_refresh_Token = TechF.db.SettingsTWHT.GetParam("Bot_Refresh_Token");
				//bot_tokenTime = Convert.ToInt32(TechF.db.SettingsTWHT.GetParam("Bot_TokenTime"));

				//ch_authCode = TechF.db.SettingsTWHT.GetParam("Channel_AuthCode");
				//ch_access_Token = TechF.db.SettingsTWHT.GetParam("Channel_Access_Token");
				//ch_refresh_Token = TechF.db.SettingsTWHT.GetParam("Channel_Refresh_Token");
				//ch_tokenTime = Convert.ToInt32(TechF.db.SettingsTWHT.GetParam("Channel_TokenTime"));

				//def_authCode = TechF.db.SettingsTWHList.Find(x => x.id == 10).Param;
				def_access_Token = TechF.db.SettingsTWHList.Find(x => x.id == 11).Param;
				def_refresh_Token = TechF.db.SettingsTWHList.Find(x => x.id == 12).Param;
				def_tokenTime = Convert.ToInt32(TechF.db.SettingsTWHList.Find(x => x.id == 13).Param);

				//bot_authCode = TechF.db.SettingsTWHList.Find(x => x.id == 14).Param;
				bot_access_Token = TechF.db.SettingsTWHList.Find(x => x.id == 15).Param;
				bot_refresh_Token = TechF.db.SettingsTWHList.Find(x => x.id == 16).Param;
				bot_tokenTime = Convert.ToInt32(TechF.db.SettingsTWHList.Find(x => x.id == 17).Param);

				//ch_authCode = TechF.db.SettingsTWHList.Find(x => x.id == 18).Param;
				ch_access_Token = TechF.db.SettingsTWHList.Find(x => x.id == 19).Param;
				ch_refresh_Token = TechF.db.SettingsTWHList.Find(x => x.id == 20).Param;
				ch_tokenTime = Convert.ToInt32(TechF.db.SettingsTWHList.Find(x => x.id == 21).Param);
			}


			private void ScopesFill() {
				ScopesforReq.Add("bits:read");
				ScopesforReq.Add("channel:manage:broadcast");
				ScopesforReq.Add("channel:manage:redemptions");
				ScopesforReq.Add("channel:read:editors");
				ScopesforReq.Add("channel:read:hype_train");
				ScopesforReq.Add("channel:read:polls");
				ScopesforReq.Add("channel:read:redemptions");
				ScopesforReq.Add("channel:read:subscriptions");
				ScopesforReq.Add("moderation:read");
				ScopesforReq.Add("moderator:manage:banned_users");
				//ScopesforReq.Add("moderator:read:blocked_terms");
				//ScopesforReq.Add("moderator:manage:blocked_terms");
				//ScopesforReq.Add("moderator:manage:automod");
				ScopesforReq.Add("moderator:read:automod_settings");
				//ScopesforReq.Add("moderator:manage:automod_settings");
				ScopesforReq.Add("moderator:read:chat_settings");
				ScopesforReq.Add("moderator:manage:chat_settings");
				//ScopesforReq.Add("user:edit");
				//ScopesforReq.Add("user:edit:follows");
				//ScopesforReq.Add("user:read:broadcast");
				ScopesforReq.Add("user:read:follows");
				//ScopesforReq.Add("user:read:subscriptions");

				/*
				ScopesforChat.Add("channel_check_subscription");
				ScopesforChat.Add("channel_commercial");
				ScopesforChat.Add("channel_editor");
				//ScopesforChat.Add("channel_feed_edit");
				//ScopesforChat.Add("channel_feed_read");
				ScopesforChat.Add("channel_read");
				//ScopesforChat.Add("channel_stream");
				ScopesforChat.Add("channel_subscriptions");
				ScopesforChat.Add("chat_login");
				//ScopesforChat.Add("collections_edit:");
				//ScopesforChat.Add("communities_edit");
				//ScopesforChat.Add("communities_moderate");
				ScopesforChat.Add("user_read");
				ScopesforChat.Add("user_blocks_edit");
				ScopesforChat.Add("user_blocks_read");
				//ScopesforChat.Add("user_follows_edit");
				ScopesforChat.Add("user_subscriptions");
				//ScopesforChat.Add("viewing_activity_read");
				*/

				ScopesforChat.Add("channel:moderate");
				ScopesforChat.Add("chat:edit");
				ScopesforChat.Add("chat:read");
				ScopesforChat.Add("whispers:read");
				ScopesforChat.Add("whispers:edit");
			}

			private void ScopesClear() {
				ScopesforReq.Clear();
				ScopesforChat.Clear();
			}



			// -- Проверка токена доступа --
			public bool ValidateToken(string atoken = null, bool isChannel = false) {
				string strres = null;
				string strerr = null;
				bool reqsuccess = false;
				string reqstatus = null;
				string Access_Tokenl = null;

				if (atoken != null) { Access_Tokenl = atoken; } else {
					if (useDefToken) { Access_Tokenl = def_Access_Token; } else {
						if (isChannel) { Access_Tokenl = ch_Access_Token; } else { Access_Tokenl = bot_Access_Token; }
					}
				}

				RestClient ReqClientl = new RestClient("https://id.twitch.tv");

				RestRequest req = new RestRequest("oauth2/validate", Method.Get);

				req.AddHeader("Authorization", "Bearer " + Access_Tokenl);
				req.AddHeader("User-Agent", "Twidibot/" + TechF.AppVersion);


				// -- Непосредственно выполнение запроса
				try {
					Task<RestResponse> reqresstream = ReqClientl.GetAsync(req);

					RestResponse resstream = reqresstream.Result;

					strres = resstream.Content;
					reqsuccess = resstream.IsSuccessful;
					//reqstatus = resstream.ResponseStatus.ToString();
					if (!reqsuccess) { reqstatus = TechF.TechFuncs.SearchinJson(strres, "error"); }

				} catch (Exception ex) {
					TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка проверки Access Token: " + ex.Message + " | " + ex.InnerException.Message);
					return false;
				}


				// -- Обработка ошибок
				if (!reqsuccess) {
					strerr = TechF.TechFuncs.SearchinJson(strres, "message");

					if (strerr == "invalid access token") {
						TechF.TechFuncs.LogDH("Twitch API - Запрос проверки Access Token: Токен недействителен");
					} else {
						TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка проверки Access Token: " + reqstatus + " | " + strerr);
					}
					return false;
				} else {
					if (useDefToken) {
						def_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinText(strres, "expires_in", "\"", "\"", ":", "}"));
					} else {
						if (isChannel) {
							ch_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinText(strres, "expires_in", "\"", "\"", ":", "}"));
						} else {
							bot_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinText(strres, "expires_in", "\"", "\"", ":", "}"));
						}
					}
					TechF.TechFuncs.LogDH("Twitch API - Запрос проверки Access Token: Успешно");
					return true;
				}
			}

			public void ValidateandRefreshAllTokens() {
				if (useDefToken) {
					if (!ValidateToken()) {
						RefreshToken();
					}
				} else {
					if (bot_Access_Token != null) {
						if (!ValidateToken(null, false)) {
							RefreshToken(false);
						}
					}
					if (ch_Access_Token != null) {
						if (!ValidateToken(null, true)) {
							RefreshToken(true);
						}
					}
				}
			}



			// -- Получение обычного аксесс токена --
			/*private string GetToken() {
				string strout = "";
				var client = new TinyRestClient(new HttpClient(), "https://id.twitch.tv");
				try {
					StringBuilder sb = new StringBuilder();
					byte[] buf = new byte[8192];

					StringBuilder scopes = new StringBuilder();
					for (int i = 0; i < ScopesforReq.Count; i++) {
						if (i > 0) { scopes.Append("%20"); }
						scopes.Append(ScopesforReq.ElementAt<string>(i));
					}

					var response = client.PostRequest("oauth2/token").
						AddQueryParameter("client_id", Client_Id).
						AddQueryParameter("client_secret", Client_Secret).
						AddQueryParameter("grant_type", "client_credentials").
						AddQueryParameter("scopes", scopes.ToString()).
						AddQueryParameter("User-Agent", "Twidibot/" + TechF.AppVersion).
						ExecuteAsStreamAsync();

					Stream resStream = response.Result;
					int count = 0;
					do {
						count = resStream.Read(buf, 0, buf.Length);
						if (count != 0) { sb.Append(Encoding.UTF8.GetString(buf, 0, count)); }
					} while (count > 0);

					strout = sb.ToString();
					strout = TechF.TechFuncs.SearchinJson(strout, "access_token");

					Access_Token = strout;

				} catch (Exception ex) {
					strout = ex.Message;
				}
				TechF.TechFuncs.LogDH("Сгенерирован Twitch-запрос получения обычного Access Token");
				return strout;
			}*/



			// -- Получение нормального аксесс токена, после авторизации --
			public bool GetTokenAuth(bool isChannel = false) {
				string strres = null;
				string strerr = null;
				bool reqsuccess = false;
				string reqstatus = null;
				string AuthCodel = null;
				string Redirectl = null;
				string acl = null;

				if (isChannel) { AuthCodel = ch_AuthCode; } else { AuthCodel = bot_AuthCode; }
				if (isChannel) { Redirectl = "channel"; } else { Redirectl = "bot"; }

				RestClient ReqClientl = new RestClient("https://id.twitch.tv");

				RestRequest req = new RestRequest("oauth2/token?client_id=" + Client_Id + "&client_secret=" + Client_Secret + "&code=" + AuthCodel + "&grant_type=authorization_code&redirect_uri=https://twidi.localhost:8138/twitch/auth/" + Redirectl, Method.Post);

				req.AddHeader("User-Agent", "Twidibot/" + TechF.AppVersion);


				// -- Непосредственно выполнение запроса
				try {
					Task<RestResponse> reqresstream = ReqClientl.PostAsync(req);

					RestResponse resstream = reqresstream.Result;

					strres = resstream.Content;
					reqsuccess = resstream.IsSuccessful;
					//reqstatus = resstream.ResponseStatus.ToString();
					if (!reqsuccess) { reqstatus = TechF.TechFuncs.SearchinJson(strres, "error"); }

				} catch (Exception ex) {
					TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка получения Access Token: " + ex.Message + " | " + ex.InnerException.Message);
					return false;
				}


				// -- Обработка ошибок
				if (!reqsuccess) {
					strerr = TechF.TechFuncs.SearchinJson(strres, "message");
					TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка получения Access Token: " + reqstatus + " | " + strerr);
					return false;
				}

				acl = TechF.TechFuncs.SearchinJson(strres, "access_token");
				if (ValidateToken(acl)) {
					bot_access_Token = acl;
					if (isChannel) {
						ch_Access_Token = acl;
						ch_Refresh_Token = TechF.TechFuncs.SearchinJson(strres, "refresh_token");
						//ch_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", ","));
					} else {
						bot_Access_Token = acl;
						bot_Refresh_Token = TechF.TechFuncs.SearchinJson(strres, "refresh_token");
						//bot_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", ","));
					}
					TechF.TechFuncs.LogDH("Twitch API - Запрос получения Access Token: Успешно");
					return true;
				} else {
					if (isChannel) {
						ch_Access_Token = null;
						ch_Refresh_Token = null;
						//ch_TokenTime = null;
					} else {
						bot_Access_Token = null;
						bot_Refresh_Token = null;
						//bot_TokenTime = null;
					}
					TechF.TechFuncs.LogDH("Twitch API - Ошибка проверки Access Token при его получении: Токен недействителен");
					return false;
				}
			}



			// -- Обновление аксесс токена --
			public bool RefreshToken(bool isChannel = false) {
				string strres = null;
				string strerr = null;
				bool reqsuccess = false;
				string reqstatus = null;
				string Refresh_Tokenl = null;
				string atl = null;

				if (useDefToken) { Refresh_Tokenl = def_Refresh_Token; } else {
					if (isChannel) { Refresh_Tokenl = ch_Refresh_Token; } else { Refresh_Tokenl = bot_Refresh_Token; }
				}

				RestClient ReqClientl = new RestClient("https://id.twitch.tv");

				RestRequest req = new RestRequest("oauth2/token?client_id=" + Client_Id + "&client_secret=" + Client_Secret + "&grant_type=refresh_token" + "&refresh_token=" + Refresh_Tokenl, Method.Post);

				req.AddHeader("User-Agent", "Twidibot/" + TechF.AppVersion);


				// -- Непосредственно выполнение запроса
				try {
					Task<RestResponse> reqresstream = ReqClientl.PostAsync(req);

					RestResponse resstream = reqresstream.Result;

					strres = resstream.Content;
					reqsuccess = resstream.IsSuccessful;
					//reqstatus = resstream.ResponseStatus.ToString();
					if (!reqsuccess) { reqstatus = TechF.TechFuncs.SearchinJson(strres, "error"); }

				} catch (Exception ex) {
					TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка обновления Access Token: " + ex.Message + " | " + ex.InnerException.Message);
					return false;
				}


				// -- Обработка ошибок
				if (!reqsuccess) {
					strerr = TechF.TechFuncs.SearchinJson(strres, "message");

					switch (strerr) {
						case "Invalid refresh token":
						tokens_work = false;
						TechF.TechFuncs.LogDH("Twitch API - Ошибка обновления Access Token: неправильный токен обновления. Требуется новая авторизация");
						if (isChannel) {
							Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка доступа к Twitch API", "Требуется повторный вход в аккаунт канала", true));
						} else {
							Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка доступа к Twitch API", "Требуется повторный вход в аккаунт бота", true));
						}
						break;
						default:
						TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка обновления Access Token: " + reqstatus + " | " + strerr);
						break;
					}
					return false;
				}

				atl = TechF.TechFuncs.SearchinJson(strres, "access_token");
				if (ValidateToken(atl)) {
					if (useDefToken) {
						def_Access_Token = atl;
						def_Refresh_Token = TechF.TechFuncs.SearchinJson(strres, "refresh_token");
						//def_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", ","));
					} else {
						if (isChannel) {
							ch_Access_Token = atl;
							ch_Refresh_Token = TechF.TechFuncs.SearchinJson(strres, "refresh_token");
							//ch_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", ","));
						} else {
							bot_Access_Token = atl;
							bot_Refresh_Token = TechF.TechFuncs.SearchinJson(strres, "refresh_token");
							//bot_TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchinJson(strres, "expires_in", ":", ","));
						}
					}
					TechF.TechFuncs.LogDH("Twitch API - Запрос получения Access Token: Успешно");
					return true;
				} else {
					if (isChannel) {
						ch_Access_Token = null;
						ch_Refresh_Token = null;
						//ch_TokenTime = null;
					} else {
						bot_Access_Token = null;
						bot_Refresh_Token = null;
						//bot_TokenTime = null;
					}
					return false;
				}
			}



			// -- Удаление аксесс токена --
			public bool RemoveToken(bool isChannel = false) {
				string strres = null;
				string strerr = null;
				bool reqsuccess = false;
				string reqstatus = null;
				string Access_Tokenl = null;

				if (useDefToken) { Access_Tokenl = def_Access_Token; } else {
					if (isChannel) { Access_Tokenl = ch_Access_Token; } else { Access_Tokenl = bot_Access_Token; }
				}

				RestClient ReqClientl = new RestClient("https://id.twitch.tv");

				RestRequest req = new RestRequest("oauth2/revoke?client_id=" + Client_Id + "&token=" + Access_Tokenl, Method.Post);

				req.AddHeader("User-Agent", "Twidibot/" + TechF.AppVersion);


				// -- Непосредственно выполнение запроса
				try {
					Task<RestResponse> reqresstream = ReqClientl.PostAsync(req);

					RestResponse resstream = reqresstream.Result;

					strres = resstream.Content;
					reqsuccess = resstream.IsSuccessful;
					//reqstatus = resstream.ResponseStatus.ToString();
					if (!reqsuccess) { reqstatus = TechF.TechFuncs.SearchinJson(strres, "error"); }

				} catch (Exception ex) {
					TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка удаления Access Token: " + ex.Message + " | " + ex.InnerException.Message);
					return false;
				}


				// -- Обработка ошибок
				if (!reqsuccess) {
					strerr = TechF.TechFuncs.SearchinJson(strres, "message");
					TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка удаления Access Token: " + reqstatus + " | " + strerr);
					return false;
				}

				if (isChannel) {
					ch_Access_Token = null;
					TechF.db.SettingsTWHT.UpdateSetting("Channel_Access_Token", null);
				} else {
					bot_Access_Token = null;
					TechF.db.SettingsTWHT.UpdateSetting("Bot_Access_Token", null);
				}
				TechF.TechFuncs.LogDH("Twitch API - Запрос удаления Access Token: Успешно");

				return true;
			}




			// -- Крафт ссылки для авторизации через браузер и открытие браузера пользователя с этой ссылкой
			public void AuthorizationviaBrowser(bool isChannel = false, bool AutoClose = false) {
				StringBuilder req = new StringBuilder("https://id.twitch.tv/oauth2/authorize", 200);

				// -- Крафт запроса
				if (isChannel) {
					req.Append("?client_id=" + Client_Id + "&redirect_uri=https://twidi.localhost:8138/twitch/auth/channel&response_type=code&scope=");
				} else {
					req.Append("?client_id=" + Client_Id + "&redirect_uri=https://twidi.localhost:8138/twitch/auth/bot&response_type=code&scope=");
				}

				// -- Сбор разрешений
				ScopesFill();
				for (int i = 0; i < ScopesforReq.Count; i++) {
					if (i > 0) { req.Append("%20"); }
					req.Append(ScopesforReq.ElementAt(i));
				}
				ScopesClear();
				// -- Генерация шифрованной строки с даннами, что планируем получать
				// тута да, функция генерации state
				//req.Append("&state=");

				TechF.TechFuncs.LogDH("Twitch API - Авторизация через браузер...");
				Process.Start(req.ToString());
			}


			// -- Крафт ссылки для получения OAuth-кода
			public string GetOAuthCode(bool isReturn = false) {
				StringBuilder req = new StringBuilder("https://id.twitch.tv/oauth2/authorize?client_id=" + Client_Id + "&redirect_uri=https://twitchapps.com/tmi/&response_type=token&scope=", 250);


				// -- Сбор разрешений
				ScopesFill();
				for (int i = 0; i < ScopesforChat.Count; i++) {
					if (i > 0) { req.Append("%20"); }
					req.Append(ScopesforChat.ElementAt(i));
				}
				ScopesClear();
				// -- Генерация шифрованной строки с даннами, что планируем получать
				// тута да, функция генерации state
				//req.Append("&state=");

				if (isReturn) {
					return req.ToString();
				} else {
					TechF.TechFuncs.LogDH("Twitch API - Получение OAuth через браузер...");
					Process.Start(req.ToString());
					return null;
				}
			}




			// -- Основная функция для запросов --
			public string ApiRequest(string RequstUrl, bool isChannel = false, string ReqType = "GET", string Json = null, string[,] Parameters = null, string[] Return_Params = null, string[,] Headers = null) {
				string strout = null;
				int Parameters_c = 0;
				int Headers_c = 0;
				int Return_Params_c = 0;
				string strerr = null;
				bool reqsuccess = false;
				string reqstatus = null;
				string Access_Tokenl = null;
				RestRequest req = null;

				// -- Некоторые настройки
				if (RequstUrl.StartsWith("/")) { RequstUrl = RequstUrl.Substring(1); }

				if (useDefToken) { Access_Tokenl = def_Access_Token; } else {
					if (isChannel) { Access_Tokenl = ch_Access_Token; } else { Access_Tokenl = bot_Access_Token; }
				}

				if (Parameters != null) {
					Parameters_c = Parameters.Length;
				}
				if (Headers != null) {
					Headers_c = Headers.GetLength(1);
				}
				if (Return_Params != null) {
					Return_Params_c = Return_Params.Length;
				}



				// -- Инициализация запроса
				switch (ReqType) {
					case "GET":
						req = new RestRequest(RequstUrl, Method.Get);
					break;
					case "POST":
						req = new RestRequest(RequstUrl, Method.Post);
					break;
					case "PATCH":
						req = new RestRequest(RequstUrl, Method.Patch);
						//req.AddHeader("Content-Type", "application/json");
					break;
					case "PUT":
						req = new RestRequest(RequstUrl, Method.Put);
					break;

					default:
					return "Неправильно указан метод запроса";
					//break;
				}

				// -- Добавление аксес токена, т.к. он может быть уникальным чуть ли не для каждого запроса
				req.AddHeader("Authorization", "Bearer " + Access_Tokenl);
				//req.

				// -- Добавление кастомных заголовков в запрос
				for (ushort i = 0; i < Headers_c; i++) {
					req.AddHeader(Headers[0, i], Headers[1, i]);
				}

				// -- Добавление параметров, если вдруг они указаны через массив, а не через реквестюрл
				for (ushort i = 0; i < Parameters_c; i++) {
					req.AddParameter(Parameters[0, i], Parameters[1, i]);
				}

				// -- Крафтим запрос в стиле CURL для вывода в лог
				StringBuilder strlog = new StringBuilder(ReqType + " " + RequstUrl + " -H 'Authorization: Bearer [скрыто]' -H 'Client-Id: " + Client_Id + "'");
				if (ReqType == "PATCH") {
					strlog.Append(" -H 'Content-Type: application/json'");
				}

				strlog.Append(" -H 'User-Agent: Twidibot/" + TechF.AppVersion + "'");

				for (ushort i = 0; i < Headers_c; i++) { // -- Добавление заголовков
					strlog.Append(" -H '" + Headers[0, i] + ": " + Headers[1, i] + "'");
				}

				TechF.TechFuncs.LogDH("Twitch API - Запрос: " + strlog);


				// -- Непосредственно выполнение запроса
				try {
					Task<RestResponse> reqresstream = null;

					switch (ReqType) {
						case "GET":
							reqresstream = ReqClient.ExecuteGetAsync(req);
						break;
						case "POST":
							reqresstream = ReqClient.PostAsync(req);
						break;
						case "PATCH":
							reqresstream = ReqClient.PatchAsync(req);
						break;
						case "PUT":
							reqresstream = ReqClient.PutAsync(req);
						break;
					}

					RestResponse resstream = reqresstream.Result;

					strout = resstream.Content;
					reqsuccess = resstream.IsSuccessful;
					//reqstatus = resstream.ResponseStatus.ToString();
					if (!reqsuccess) { reqstatus = TechF.TechFuncs.SearchinJson(strout, "error"); }

				} catch (Exception ex) {
					TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка запроса: " + ex.Message + " | " + ex.InnerException.Message);
				}


				// -- Обработка ошибок
				if (!reqsuccess) {
					strerr = TechF.TechFuncs.SearchinJson(strout, "message");

					switch (reqstatus) {

						case "Unauthorized":
							switch (strerr) {

								case "Invalid OAuth token":
									if (Access_Tokenl != null && Access_Tokenl != "") {
										TechF.TechFuncs.LogDH("Twitch API - Ошибка запроса: требуется обновление Access Token");
										if (!RefreshToken()) {
											TechF.TechFuncs.LogDH("Twitch API - Продолжение работы запросов невозможно");
											return null;
										}
										strout = ApiRequest(RequstUrl, isChannel, ReqType, Json, Parameters, Return_Params, Headers);
									} else {
										TechF.TechFuncs.LogDH("Twitch API - Ошибка запроса: Отсутствует Access Token");
										if (!GetTokenAuth()) {
											AuthorizationviaBrowser(isChannel);
											strout = "false";
										}
									}
								break;

								case "incorrect user authorization":
									TechF.TechFuncs.LogDH("Twitch API - Ошибка запроса: Неправильная авторизация - скорее всего, она прошла под не тем аккаунтом, от имени которого проходит запрос");
									//AuthorizationviaBrowser(isChannel);
									strout = "false";
								break;

								default:
									TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка запроса: " + reqstatus + " | " + strerr);
								break;
							}
						break;

						//case "Bad Request":

						//break;

						default:
							TechF.TechFuncs.LogDH("Twitch API - Необрабатываемая ошибка запроса: " + reqstatus + " | " + strerr);
						break;
					}
				}


				if (strout != "false") {
					// -- Крафтим строку с выбранными параметрами, 
					if (Return_Params_c > 0) {
						StringBuilder sbl = new StringBuilder(100);
						for (ushort i = 0; i < Return_Params_c; i++) {
							if (i == 0) {
								sbl.Append(TechF.TechFuncs.SearchinJson(strout, Return_Params[i]));
							} else {
								sbl.Append(";" + TechF.TechFuncs.SearchinJson(strout, Return_Params[i]));
							}
						}
						strout = sbl.ToString();
						TechF.TechFuncs.LogDH("Twitch API - Ответ на запрос (только указанные параметры): " + strout);
					} else {
						TechF.TechFuncs.LogDH("Twitch API - Ответ на запрос: " + strout);
					}
				}
				return strout;
			}



			// -- Функция разбора данных после вход в аккаунт через браузер
			public string[] FLogin(string reqParams) {
				string strtmp = reqParams.Substring(reqParams.IndexOf("code=") + 5);
				bool isChannel = false;
				string[] errml = new string[2];
				string strlog = null;
				string strlogerr = null;
				if (reqParams.StartsWith("channel")) { isChannel = true; }
				if (isChannel) {
					strlog = "API Twitch - Авторизация канала: ";
					strlogerr = "API Twitch - Ошибка авторизация канала: ";
				} else {
					strlog = "API Twitch - Авторизация бота: ";
					strlogerr = "API Twitch - Ошибка авторизация бота: ";
				}
				TechF.TechFuncs.LogDH(strlog + "Запуск разбора авторизации канала через браузер...");
				Ev_Status?.Invoke(this, new Twident_Status(0, "Выделение токена авторизации...", null, false));
				if (reqParams.Contains("code=")) {
					if (isChannel) {
						ch_AuthCode = strtmp.Substring(0, strtmp.IndexOf("&scope=")); // -- Сохраняем код авторизации
					} else {
						bot_AuthCode = strtmp.Substring(0, strtmp.IndexOf("&scope="));
					}

					TechF.TechFuncs.LogDH(strlog + "получение токена аунтентификации...");
					Ev_Status?.Invoke(this, new Twident_Status(0, "Получение токена аунтентификации...", null, null));
					if (!GetTokenAuth(isChannel)) {
						errml[0] = "Ошибка проверки на правильность полученного токена авторизации";
						errml[1] = "Можно попробовать ещё раз авторизоваться. Иначе пишите разрабу, или смотрите в лог";
						TechF.TechFuncs.LogDH(strlogerr + "Полученный токен авторизации не подтверждён");
						Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка получения токена аунтентификации", null, true));
						return errml;
					}

					TechF.TechFuncs.LogDH(strlog + "Получение данных аккаунта...");
					Ev_Status?.Invoke(this, new Twident_Status(0, "Получение данных аккаунта...", null, null));
					if (!Req_UpdateBotChannelInfoLite(isChannel)) {
						errml[0] = "Ошибка получения информации об аккаунте";
						errml[1] = "Можно попробовать ещё раз авторизоваться. Иначе пишите разрабу, или смотрите в лог";
						TechF.TechFuncs.LogDH(strlogerr + "Непонятная ошибка получения данных об аккаунте");
						Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка получения данных аккаунта", null, true));
						return errml;
					} else {
						TechF.TechFuncs.LogDH(strlog + "Успешно");
						if (TechF.TechFuncs.GetSettingTWHParam("Bot_Access_Token") == "true") { useDefToken = false; } else { useDefToken = true; }
						if (!isChannel) {
							//GetOAuthCode();
							Ev_LoginBotD?.Invoke(this, new Twident_Null());
							Ev_Status?.Invoke(this, new Twident_Status(0, "Авторизация в процессе...", "Вставте OAuth с открывшегося сайта", null));
						} else {
							Ev_LoginChD?.Invoke(this, new Twident_Null());
							Ev_Status?.Invoke(this, new Twident_Status(1, "Авторизация прошла успешно", null, false));
						}
						return null;
					}
				} else {
					TechF.TechFuncs.LogDH(strlogerr + "Не найден код авторизации");
					Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка авторизации", "Не найден код авторизации. Попробуйте снова, иначе обратитесь к разработчику", true));
					if (isChannel) {
						strtmp = strtmp.Substring(8);
					} else {
						strtmp = strtmp.Substring(3);
					}
					return strtmp.Split('&');
				}
			}



			// -- Обновление секрета приложения
			public bool UpdateClientSecret() {


				//TechF.TechFuncs.LogDH("Twitch API - Запрос обновления секрета приложения: ");
				return false;
			}


			// -- Определение фолоу на канал подключения
			public bool Req_BotFollowtoChannel() {
				if (TechF.TechFuncs.GetSettingTWHParam("BotChannel_isOne") == "0") {
					string loginl = TechF.TechFuncs.GetSettingTWHParam("Login");
					string channell = TechF.TechFuncs.GetSettingTWHParam("Channel");
					string loginid = TechF.TechFuncs.GetSettingTWHParam("LoginId");
					string res = null;
					string cursor = null;
					if (loginl != null && channell != null) {

						//string botid = ApiRequest("users?login=" + loginl, false, "GET", null, null, new string[] { "id" });

						while (true) {

							if (cursor != null || cursor != "") {
								res = ApiRequest("users/follows?from_id=" + loginid + "&first=50");
							} else {
								res = ApiRequest("users/follows?from_id=" + loginid + "&first=10&pagination=" + cursor);
							}

							if (res.Contains("\"to_login\":\"" + channell + "\"")) {
								return true;
							} else {
								cursor = TechF.TechFuncs.SearchinText(res, "pagination", "\"", "\"",  "{", "}");
								cursor = TechF.TechFuncs.SearchinJson(cursor, "cursor");
								if (cursor != null || cursor != "") {
									return false;
								} else {
									continue;
								}
							}
						}
					} else { return false; }
				} else { return true; }
			}



			// -- Получение информации о залогиненом пользователе
			// - Версия для функции
			public bool Req_UpdateBotChannelInfo_login(bool isChannel) {
				string res = null;

				if (isChannel) {
					if (TechF.TechFuncs.GetSettingTWHParam("Channel_AuthCode") == "true") {
						res = ApiRequest("users", true);
						TechF.db.SettingsTWHT.UpdateSetting("Channel", TechF.TechFuncs.SearchinJson(res, "login"));
						TechF.db.SettingsTWHT.UpdateSetting("ChannelId", TechF.TechFuncs.SearchinJson(res, "id"));
						TechF.db.SettingsTWHT.UpdateSetting("ChannelDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
						return true;
					} else {
						Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка обновления информации о канале", null, true));
						return false;
					}
				} else {
					if (TechF.TechFuncs.GetSettingTWHParam("Bot_AuthCode") == "true") {
						useDefToken = false;
						res = ApiRequest("users", false);
						TechF.db.SettingsTWHT.UpdateSetting("Login", TechF.TechFuncs.SearchinJson(res, "login"));
						TechF.db.SettingsTWHT.UpdateSetting("LoginId", TechF.TechFuncs.SearchinJson(res, "id"));
						TechF.db.SettingsTWHT.UpdateSetting("LoginDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
						if (TechF.Twitch.BotChannelisOne) {
							TechF.db.SettingsTWHT.UpdateSetting("Channel", TechF.TechFuncs.SearchinJson(res, "login"));
							TechF.db.SettingsTWHT.UpdateSetting("ChannelId", TechF.TechFuncs.SearchinJson(res, "id"));
							TechF.db.SettingsTWHT.UpdateSetting("ChannelDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
						}
						return true;
					} else {
						Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка обновления информации об аккаунте", null, true));
						return false;
					}
				}
			}


			// - Версия с выбором, что обновлять
			public bool Req_UpdateBotChannelInfoLite(bool isChannel, string user = null) {
				string res = null;
				string id = null;
				int status = 0;

				if (isChannel) {
					if (user == null) { user = TechF.TechFuncs.GetSettingTWHParam("Channel"); }
					res = ApiRequest("users?login=" + user);
					status = Convert.ToInt32(TechF.TechFuncs.SearchinJson(res, "status", true));
					id = TechF.TechFuncs.SearchinJson(res, "id");
					if (status == 200 || id != null) {
						TechF.db.SettingsTWHT.UpdateSetting("ChannelId", id);
						TechF.db.SettingsTWHT.UpdateSetting("ChannelDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
						return true;
					} else {
						switch (status) {
							case 0:	case 400:
								Ev_Status?.Invoke(this, new Twident_Status(3, "Указаного канала не существует", null, true));
							break;

							default:
								Ev_Status?.Invoke(this, new Twident_Status(3, "Неизвестная ошибка обновления информации о канале", null, true));
							break;
						}
						return false;
					}
				} else {
					if (user == null) { user = TechF.TechFuncs.GetSettingTWHParam("Login"); }
					res = ApiRequest("users?login=" + user);
					status = Convert.ToInt32(TechF.TechFuncs.SearchinJson(res, "status", true));
					id = TechF.TechFuncs.SearchinJson(res, "id");
					if (status == 200 || id != null) {
						TechF.db.SettingsTWHT.UpdateSetting("LoginId", id);
						TechF.db.SettingsTWHT.UpdateSetting("LoginDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
						return true;
					} else {
						switch (status) {
							case 0:	case 400:
								Ev_Status?.Invoke(this, new Twident_Status(3, "Указаного аккаунта не существует", null, true));
							break;

							default:
								Ev_Status?.Invoke(this, new Twident_Status(3, "Неизвестная ошибка обновления информации о бота", null, true));
							break;
						}
						return false;
					}
				} 
			}

			// -- Версия для полного обновления (честно говоря, я думаю, что она не особо актуальна)
			public bool Req_UpdateBotChannelInfoFull() {
				string res = null;
				string bl = TechF.TechFuncs.GetSettingTWHParam("Login");
				string cl = TechF.TechFuncs.GetSettingTWHParam("Channel");
				string bi = null;
				string ci = null;
				if (bl == cl) { cl = null; }

				if (bl != null && cl == null) {
					res = ApiRequest("users?login=" + bl);
					bi = TechF.TechFuncs.SearchinJson(res, "id");
					if (bi != null) {
						TechF.db.SettingsTWHT.UpdateSetting("LoginId", bi);
						TechF.db.SettingsTWHT.UpdateSetting("LoginDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
						return true;
					} else {
						Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка обновления информации о боте", null, true));
						return false;
					}
				}

				if (bl == null && cl != null) {
					res = ApiRequest("users?login=" + cl);
					ci = TechF.TechFuncs.SearchinJson(res, "id");
					if (ci != null) {
						TechF.db.SettingsTWHT.UpdateSetting("ChannelId", ci);
						TechF.db.SettingsTWHT.UpdateSetting("ChannelDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
						return true;
					} else {
						Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка обновления информации о канале", null, true));
						return false;
					}
				}

				if (bl != null && cl != null) {
					res = ApiRequest("users?login=" + bl + "&login=" + cl);
					string[] resm = res.Split(new string[1] { "},{" }, StringSplitOptions.None);
					if (resm.Length == 1) {
						if (TechF.TechFuncs.SearchinJson(res, "login") == bl) {
							bi = TechF.TechFuncs.SearchinJson(res, "id");
							if (bi != null) {
								TechF.db.SettingsTWHT.UpdateSetting("LoginId", bi);
								TechF.db.SettingsTWHT.UpdateSetting("LoginDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
								return true;
							} else {
								Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка обновления информации о боте", null, true));
								return false;
							}
						} else {
							if (TechF.TechFuncs.SearchinJson(res, "login") == cl) {
								ci = TechF.TechFuncs.SearchinJson(res, "id");
								if (ci != null) {
									TechF.db.SettingsTWHT.UpdateSetting("ChannelId", ci);
									TechF.db.SettingsTWHT.UpdateSetting("ChannelDisp", TechF.TechFuncs.SearchinJson(res, "display_name"));
									return true;
								} else {
									Ev_Status?.Invoke(this, new Twident_Status(3, "Ошибка обновления информации о канале", null, true));
									return false;
								}
							} else {
								Ev_Status?.Invoke(this, new Twident_Status(3, "", "Несоответствие сохранённых и полученных при обновлении аккаунтов", true));
								return false;
							}
						}
					} else {
						bi = TechF.TechFuncs.SearchinJson(resm[0], "id");
						if (bi != null) {
							TechF.db.SettingsTWHT.UpdateSetting("LoginId", bi);
							TechF.db.SettingsTWHT.UpdateSetting("LoginDisp", TechF.TechFuncs.SearchinJson(resm[0], "display_name"));
						}

						ci = TechF.TechFuncs.SearchinJson(resm[1], "id");
						if (ci != null) {
							TechF.db.SettingsTWHT.UpdateSetting("ChannelId", ci);
							TechF.db.SettingsTWHT.UpdateSetting("ChannelDisp", TechF.TechFuncs.SearchinJson(resm[1], "display_name"));
						}
						return true;
					}
				} else { return false; }
			}



			// -- Получение OAuth кода - пароля для входа в чат --
			public bool Req_UpdateOAuth() {


				return false;
			}

		}

	}
}
