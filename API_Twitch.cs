using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Tiny.RestClient;

namespace Twidibot
{
	public class API_Twitch
	{
		BackWin TechF = null;

		// -- Данные авторизации --
		private string Client_Id = "frnj4piu30ipfdkdfwz8i8zr1p990m";
		private string Client_Secret = "9i0fh5yjf3sbk0db23dv6kr4gaj42x";

		private string access_Token = null;
		private string Access_Token {
			get { return access_Token; }
			set {
				if (ValidToken(value)) {
					access_Token = value;
				} else {
					access_Token = "null";
				}
			}
		}
		private string refresh_Token = null;
		private string Refresh_Token {
			get { return refresh_Token; }
			set { refresh_Token = value; }
		}

		private string authCode = null;
		private string AuthCode {
			get { return authCode; }
			set { authCode = value; }
		}

		private HttpListener AuthServer = null;
		private TcpClient AuthClient = null;
		private HttpListenerContext AuthServer_Context = null;
		private HttpListenerRequest AuthServer_Request = null;
		private HttpListenerResponse AuthServer_Response = null;

		private int TokenTime = 0;
		private List<string> ScopesforReq = null;


		public API_Twitch(BackWin backWin) {
			this.TechF = backWin;

			this.ScopesforReq = new List<string>();
			AuthServer = new HttpListener();
			AuthClient = new TcpClient();
			AuthServer.Prefixes.Add("https://localhost:8138/auth/");
			ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

			Task.Factory.StartNew(() => AuthServer_Start());
			AuthCode = TechF.TechFuncs.GetSettingParam("AuthCode");
			//Access_Token = "";

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
			ScopesforReq.Add("moderator:read:blocked_terms");
			ScopesforReq.Add("moderator:manage:blocked_terms");
			//ScopesforReq.Add("moderator:manage:automod");
			ScopesforReq.Add("moderator:read:automod_settings");
			//ScopesforReq.Add("moderator:manage:automod_settings");
			ScopesforReq.Add("moderator:read:chat_settings");
			ScopesforReq.Add("moderator:manage:chat_settings");
			ScopesforReq.Add("user:edit");
			//ScopesforReq.Add("user:edit:follows");
			ScopesforReq.Add("user:read:broadcast");
			ScopesforReq.Add("user:read:follows");
			ScopesforReq.Add("user:read:subscriptions");
		}





			public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
			return true;
		}

		// -- Мини-сервер, на который будет перенаправляться залогиненый пользователь
		private void AuthServer_Start() {
			AuthServer.Start();
			string rawurl_org = null;
			string rawurl_tmp = null;
			string errstr = null;
			string errdescstr = null;
			string[] errm = null;
			string responseStr = null;

			while (true) {
				AuthServer_Context = AuthServer.GetContext();
				AuthServer_Request = AuthServer_Context.Request;
				AuthServer_Response = AuthServer_Context.Response;

				TechF.TechFuncs.LogDH("Получен запрос на локальный сервер: " + AuthServer_Request.Url.ToString());

				// -- Разбор прилетевших параметров
				responseStr = "";
				errstr = null;
				errdescstr = null;
				errm = null;
				rawurl_org = AuthServer_Request.RawUrl;
				if (rawurl_org.Contains("code=")) {
					rawurl_tmp = rawurl_org.Substring(rawurl_org.IndexOf("code=") + 5);
					//Access_Token= rawurl_tmp.Substring(0, rawurl_tmp.IndexOf("&scope="));
					AuthCode = rawurl_tmp.Substring(0, rawurl_tmp.IndexOf("&scope=")); // -- Сохраняем код авторизации
					TechF.db.SettingsT.UpdateSetting("AuthCode", AuthCode);

					if (!GetTokenAuth()) {
						errm = new string[2];
						errm[0] = "Ошибка проверки на правильность полученного токена авторизации";
						errm[1] = "Можно попробовать ещё раз авторизоваться. Иначе пишите разрабу, или смотрите в лог";
					}
				} else {
					rawurl_tmp = rawurl_org.Substring(rawurl_org.IndexOf("/auth?") + 6);
					errm = rawurl_tmp.Split('&');
				}


				// -- Отправка страницы пользователю. Если есть ошибка - то редачим страничку
				if (errm != null) {
					FileStream fs = new FileStream("media/html/auth_err.html", FileMode.Open);
					byte[] array = new byte[fs.Length];
					fs.Read(array, 0, array.Length);
					string txtstr = System.Text.Encoding.UTF8.GetString(array);
					for (int i = 0; i < errm.Length; i++) {
						responseStr += "\t\t\t\t<span id=\"" + i + "\" class=\"tdef2\">" + errm[i] + "</span>\r\n\t\t\t\t<br>\r\n";
					}
					responseStr = txtstr.Replace("\t\t\t\t<span id=\"0\" class=\"tdef2\"></span>\r\n\t\t\t\t<br>\r\n", responseStr);
					fs.Close();
					Task.Factory.StartNew(() => {
						StringBuilder strlog = new StringBuilder();
						for (int j = 0; j < errm.Length; j++) { strlog.Append(errm[j]); }
						TechF.TechFuncs.LogDH("Ошибка аунтентификации через браузер: " + strlog);
					});
				} else {
					FileStream fs = new FileStream("media/html/auth_ok.html", FileMode.Open);
					byte[] array = new byte[fs.Length];
					fs.Read(array, 0, array.Length);
					responseStr = System.Text.Encoding.UTF8.GetString(array);
					fs.Close();
					TechF.TechFuncs.LogDH("Успешная аунтентификация через браузер");
				}

				//string responseString = "<HTML><head><meta charset=\"utf-8\"><title>Twidibot - авторизация</title></head><BODY>да</BODY></HTML>";
				byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);
				AuthServer_Response.ContentLength64 = buffer.Length;
				System.IO.Stream output = AuthServer_Response.OutputStream;
				output.Write(buffer, 0, buffer.Length);
				output.Close();
				//break;
			}
		}

		private void AuthServer_Stop() {
			AuthServer.Stop();
		}




		// -- Проверка токена доступа --
		public bool ValidToken(string atoken) {
			string strout = null;
			try {
				StringBuilder sb = new StringBuilder();
				byte[] buf = new byte[8192];

				WebRequest request = WebRequest.Create("https://id.twitch.tv/oauth2/validate");
				//request.Headers.Add("Authorization: OAuth " + atoken);
				request.Headers.Add("Authorization: Bearer " + atoken);

				WebResponse response = request.GetResponse();
				Stream resStream = response.GetResponseStream();
				int count = 0;
				do {
					count = resStream.Read(buf, 0, buf.Length);
					if (count != 0) { sb.Append(Encoding.UTF8.GetString(buf, 0, count)); }
				} while (count > 0);

				strout = sb.ToString();
				try {
					TokenTime = Convert.ToInt32(TechF.TechFuncs.SearchOfHttp(strout, "expires_in", ":", "}", true));
				} catch (FormatException) { }

			} catch (WebException ex) {
				TechF.TechFuncs.LogDH("Отправлен запрос проверки Access Token: " + ex.Message);
				return false;
			}
			TechF.TechFuncs.LogDH("Отправлен запрос проверки Access Token: Успешно");
			return true;
		}



		// -- Получение обычного аксесс токена --
		private string GetToken() {
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
				strout = TechF.TechFuncs.SearchOfHttp(strout, "access_token");

				Access_Token = strout;

			} catch (Exception ex) {
				strout = ex.Message;
			}
			TechF.TechFuncs.LogDH("Сгенерирован запрос получения обычного Access Token");
			return strout;
		}



		// -- Получение нормального аксесс токена, после авторизации --
		public bool GetTokenAuth() {
			string httptxt = "";
			var client = new TinyRestClient(new HttpClient(), "https://id.twitch.tv");
			Task<Stream> response = null;
			try {
				StringBuilder sb = new StringBuilder();
				byte[] buf = new byte[8192];

				/*StringBuilder scopes = new StringBuilder();
				for (int i = 0; i < ScopesforReq.Count; i++) {
					if (i > 0) { scopes.Append("%20"); }
					scopes.Append(ScopesforReq.ElementAt<string>(i));
				}*/

				//client.Settings.DefaultHeaders.Add("client_id", Client_Id);
				//client.Settings.DefaultHeaders.Add("client_secret", Client_Secret);
				//client.Settings.DefaultHeaders.Add("code", AuthCode);
				//client.Settings.DefaultHeaders.Add("grant_type", "authorization_code");
				//client.Settings.DefaultHeaders.Add("redirect_uri", "https://localhost:8138/auth");
				//client.Settings.DefaultHeaders.Add("User-Agent", "Twidibot/" + TechF.AppVersion);

				//client.Settings.

				//response = client.PostRequest("oauth2/token").ExecuteAsStreamAsync();
				response = client.PostRequest("oauth2/token?client_id=" + Client_Id + "&client_secret=" + Client_Secret + "&code=" + AuthCode + "&grant_type=authorization_code" + "&redirect_uri=https://localhost:8138/auth" + "&User-Agent=" + "Twidibot/" + TechF.AppVersion).ExecuteAsStreamAsync();

				Stream resStream = response.Result;
				int count = 0;
				do {
					count = resStream.Read(buf, 0, buf.Length);
					if (count != 0) { sb.Append(Encoding.UTF8.GetString(buf, 0, count)); }
				} while (count > 0);

				httptxt = sb.ToString();
				Access_Token = TechF.TechFuncs.SearchOfHttp(httptxt, "access_token");
				Refresh_Token = TechF.TechFuncs.SearchOfHttp(httptxt, "refresh_token");

			} catch (Exception ex) {
				TechF.TechFuncs.LogDH("Ошибка проверки аксес токена после авторизации: " + ex.Message);
				return false;
			}
			if (Access_Token != null || Access_Token != "" || Access_Token != "null") {
				TechF.TechFuncs.LogDH("Запрос получения аунтефикационного Access Token: Успешно");
				return true;
			} else {
				return false;
				TechF.TechFuncs.LogDH("Ошибка проверки аксес токена после авторизации: Токен недействителен");
			}

		}



		// -- Обновление аксесс токена --
		public bool RefreshToken() {
			string httptxt = "";
			var client = new TinyRestClient(new HttpClient(), "https://id.twitch.tv");
			try {
				StringBuilder sb = new StringBuilder();
				byte[] buf = new byte[8192];

				StringBuilder scopes = new StringBuilder();
				for (int i = 0; i < ScopesforReq.Count; i++) {
					if (i > 0) { scopes.Append("%20"); }
					scopes.Append(ScopesforReq.ElementAt<string>(i));
				}

				var response = client.PostRequest("oauth2/token--data-urlencode").
					AddQueryParameter("grant_type", "refresh_token").
					AddQueryParameter("refresh_token", Refresh_Token).
					AddQueryParameter("client_id", Client_Id).
					AddQueryParameter("client_secret", Client_Secret).
					AddQueryParameter("User-Agent", "Twidibot/" + TechF.AppVersion).
					ExecuteAsStreamAsync();

				Stream resStream = response.Result;
				int count = 0;
				do {
					count = resStream.Read(buf, 0, buf.Length);
					if (count != 0) { sb.Append(Encoding.UTF8.GetString(buf, 0, count)); }
				} while (count > 0);

				httptxt = sb.ToString();
				Access_Token = TechF.TechFuncs.SearchOfHttp(httptxt, "access_token");
				Refresh_Token = TechF.TechFuncs.SearchOfHttp(httptxt, "refresh_token");

			} catch (Exception ex) {
				TechF.TechFuncs.LogDH("Ошибка обновления Access Token: " + ex.Message);
				return false;
			}
			if (Access_Token != null || Access_Token != "" || Access_Token != "null") {
				TechF.TechFuncs.LogDH("Запрос обновления Access Token: Успешно");
				return true;
			} else {
				TechF.TechFuncs.LogDH("Ошиюка запроса обновления Access Token: Токен недействителен");
				return false;
			}

		}



		// -- Удаление аксесс токена --
		private bool DeleteToken() {
			string httptxt = "";
			var client = new TinyRestClient(new HttpClient(), "https://id.twitch.tv");
			try {
				StringBuilder sb = new StringBuilder();
				byte[] buf = new byte[8192];

				StringBuilder scopes = new StringBuilder();
				for (int i = 0; i < ScopesforReq.Count; i++) {
					if (i > 0) { scopes.Append("%20"); }
					scopes.Append(ScopesforReq.ElementAt<string>(i));
				}

				var response = client.PostRequest("oauth2/revoke").
					AddQueryParameter("client_id", Client_Id).
					AddQueryParameter("token", Access_Token).
					AddQueryParameter("User-Agent", "Twidibot/" + TechF.AppVersion).
					ExecuteAsStreamAsync();

				Stream resStream = response.Result;
				int count = 0;
				do {
					count = resStream.Read(buf, 0, buf.Length);
					if (count != 0) { sb.Append(Encoding.UTF8.GetString(buf, 0, count)); }
				} while (count > 0);

				httptxt = sb.ToString();

			} catch (Exception ex) {
				TechF.TechFuncs.LogDH("Ошибка удаления Access Token: " + ex.Message);
				return false;
			}
			TechF.TechFuncs.LogDH("Запрос удаления Access Token: Успешно");
			return true;
		}



		// -- Крафт ссылки для авторизации через браузер --
		private string GetURLLogin() {
			string strout = "";
			StringBuilder req = new StringBuilder("https://id.twitch.tv/oauth2/authorize", 200);

			// -- Крафт запроса
			req.Append("?client_id=" + Client_Id + "&redirect_uri=https://localhost:8138/auth&response_type=code&scope=");
			for (int i = 0; i < ScopesforReq.Count; i++) {
				if (i > 0) { req.Append("%20"); }
				req.Append(ScopesforReq.ElementAt<string>(i));
			}
			strout = req.ToString();
			// -- Генерация шифрованной строки с даннами, что планируем получать
			// тута да функция генерации state
			//req.Append("&state=");

			/*try {
				StringBuilder sb = new StringBuilder();
				byte[] buf = new byte[8192];

				WebRequest request = WebRequest.Create(req.ToString());
				WebResponse response = request.GetResponse();
				Stream resStream = response.GetResponseStream();
				int count = 0;
				do {
					count = resStream.Read(buf, 0, buf.Length);
					if (count != 0) { sb.Append(Encoding.UTF8.GetString(buf, 0, count)); }
				} while (count > 0);

				strout = sb.ToString();

			} catch (WebException) {
				strout = "Ошибка!";
			}*/
			TechF.TechFuncs.LogDH("Сгенерирована ссылка авторизации через браузер");
			return strout;
		}

		// -- Открытие браузера пользователя для авторизации
		public void AuthorizationviaBrowser() {
			TechF.TechFuncs.LogDH("Авторизация через браузер...");
			Process.Start(GetURLLogin());
		}




		// -- Основная функция для запросов --
		public string ApiRequest(string addr, string[,] Headers = null, string Type = "GET", string[] Return_Params = null) {
			string strout = null;
			int Headers_c = 0;
			int Return_Params_c = 0;

			if (Headers != null) {
				Headers_c = Headers.Length; // -- Кол-во заголовков
			}
			if (Return_Params != null) {
				Return_Params_c = Return_Params.Length;
			}

			var client = new TinyRestClient(new HttpClient(), "https://api.twitch.tv/helix");
			Task<Stream> response = null;
			try {
				StringBuilder sb = new StringBuilder();
				byte[] buf = new byte[8192];

				if (addr.StartsWith("/")) { addr = addr.Substring(1); }

				//var response = client.GetRequest(addr);
				//ExecuteAsStreamAsync();
				client.Settings.DefaultHeaders.AddBearer(Access_Token);
				//client.Settings.DefaultHeaders.Add("Authorization", "Bearer " + Access_Token);
				//client.Settings.DefaultHeaders.Add("Authorization", "Bearer oauth:7ypvs7zx54nj54o1ol0ijbmsk6xdvb");
				client.Settings.DefaultHeaders.Add("Client-Id", Client_Id);
				client.Settings.DefaultHeaders.Add("User-Agent", "Twidibot/" + TechF.AppVersion);

				for (ushort i = 0; i < Headers_c; i++) { // -- Добавление заголовков в запрос
					client.Settings.DefaultHeaders.Add(Headers[i, 0], Headers[i, 1]);
				}
				switch (Type) {
					case "GET":
						response = client.GetRequest(addr).ExecuteAsStreamAsync();
					break;

					case "POST":
					response = client.PostRequest(addr).ExecuteAsStreamAsync();
					break;

					case "PATCH":
					client.Settings.DefaultHeaders.Add("Content-Type", "application/json");
					response = client.PatchRequest(addr).ExecuteAsStreamAsync();
					break;
				}


				Stream resStream = response.Result;
				int count = 0;
				do {
					count = resStream.Read(buf, 0, buf.Length);
					if (count != 0) { sb.Append(Encoding.UTF8.GetString(buf, 0, count)); }
				} while (count > 0);

				strout = sb.ToString();

			} catch (Exception ex) {
				strout = ex.Message;
			}

			// -- Крафтим строку с выбранными параметрами
			if (Return_Params_c > 0) {

			}


			// -- Крафтим запрос в стиле CURL для вывода в лог
			StringBuilder strlog = new StringBuilder(Type + " " + addr + " -H 'Authorization: Bearer " + Access_Token + "' -H 'Client-Id: " + Client_Id + "'");
			if (Type == "PATCH") {
				strlog.Append(" -H 'Content-Type: application/json'");
			}

			strlog.Append(" -H 'User-Agent: Twidibot/" + TechF.AppVersion + "'");

			for (ushort i = 0; i < Headers_c; i++) { // -- Добавление заголовков
				strlog.Append(" -H '" + Headers[i, 0] + ": " + Headers[i, 1] + "'");
			}

			TechF.TechFuncs.LogDH("Отправлен запрос: " + strlog);
			if (Return_Params_c > 0) {
				TechF.TechFuncs.LogDH("Ответ на запрос (только указанные парамтеры): " + strout);
			} else {
				TechF.TechFuncs.LogDH("Ответ на запрос: " + strout);
			}

			return strout;
		}



		// -- Обновление секрета клиента
		public bool Update_ClientSecret() {

			return false;
		}



		// -- Определение фолоу на канал подключения
		public bool Req_FollowtoChannel() {
			string loginl = TechF.TechFuncs.GetSettingParam("Login");
			string channell = TechF.TechFuncs.GetSettingParam("Channel");
			string res = null;
			string cursor = null;
			if (loginl != null && channell != null) {
				while (true) {
			
					if (cursor != null || cursor != "") {
						res = ApiRequest("follows?from_login=" + loginl + "&first=50");
					} else {
						res = ApiRequest("follows?from_login=" + loginl + "&first=10&pagination=" + cursor);
					}
					
					if (res.Contains("\"to_login\":\"" + channell + "\"")) {
						return true;
					} else {
						cursor = TechF.TechFuncs.SearchOfHttp(res, "pagination", "{", "}");
						cursor = TechF.TechFuncs.SearchOfHttp(cursor, "cursor");
						if (cursor != null || cursor != "") {
							return false;
						} else {
							continue;
						}
					}
				}
			} else {
				return false;
			}
		}




	}
}
