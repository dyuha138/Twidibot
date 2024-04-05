//using ControlzEx.Standard;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Twidibot.Pages {
	public partial class AppSettings : Page	{
		public event EventHandler<Twident_Status> Ev_SaveStatus;
		public event EventHandler<Twident_Status> Ev_GlobalStatus;
		public event EventHandler<Twident_Bool> Ev_ScrollChanged;

		private BackWin TechF = null;
		//private MainWin MainWin = null;
		//public bool openmain = false;
		private bool settingchangeadd = false;
		private bool settingchangework = false;

		private string twhchanneltmp = null;
		private string twhlogintmp = null;
		private string twhpasstmp = null;
		private string vkplchanneltmp = null;
		private string vkpllogintmp = null;
		private string vkplpasstmp = null;

		private string chathistorydeltmp = null;
		private string spammsgtmp = null;
		private string spammsgetmp = null;

		private string twhchannelprev = null;
		private string twhloginprev = null;
		private string twhpassprev = null;
		private string vkplchannelprev = null;
		private string vkplloginprev = null;
		private string vkplpassprev = null;
		private string chathistorydelprev = null;
		private string spammsgprev = null;

		//private bool lsn = false;
		private bool PermStatus = false;
		private bool isLoad = true;
		private bool isVKPLBRWLogin = false;

		public AppSettings(BackWin backWin) {
			InitializeComponent();
			this.TechF = backWin;
			TechF.Twitch.API.Ev_Status += SaveStatus_Change;
			this.Ev_SaveStatus += SaveStatus_Change;
			TechF.Twitch.API.Ev_LoginBotD += TWHLoginBotSet;
			TechF.Twitch.API.Ev_LoginChD += TWHLoginChannelSet;
			TechF.Twitch.API.Ev_Status += SaveStatus_Change;
			isLoad = false;
		}




		// -- Типа защита от спама запросов на сохранение данных в бд --
		private void SettingChange() {
			settingchangework = true;
			if (Ev_SaveStatus != null) { Ev_SaveStatus(this, new Twident_Status(0, "", null, null)); }
			while (settingchangeadd) {
				settingchangeadd = false;
				for (uint i = 0; i < 10; i++) {
					Thread.Sleep(100);
					if (settingchangeadd) { break; }
				}
			}

			if (twhchanneltmp != null || twhlogintmp != null || twhpasstmp != null || vkplchanneltmp != null || vkpllogintmp != null || vkplpasstmp != null || chathistorydeltmp != null || spammsgtmp != null || spammsgetmp != null) {
				Ev_SaveStatus?.Invoke(this, new Twident_Status(2, "Сохранение изменений...", null, false));
				// -- Twitch
				if (twhchanneltmp != null) { // -- Канал
					if (TechF.Twitch.API.Req_UpdateBotChannelInfoLite(true, twhchanneltmp)) {
						TechF.db.SettingsTWHT.UpdateSetting("Channel", twhchanneltmp.ToLower());
						twhchanneltmp = null;
						Task.Factory.StartNew(() => TechF.Twitch.API.Req_BotFollowtoChannel());
					}	
				}
				if (twhlogintmp != null) { // -- Логин
					if (TechF.Twitch.API.Req_UpdateBotChannelInfoLite(false, twhlogintmp)) {
						TechF.db.SettingsTWHT.UpdateSetting("Login", twhlogintmp.ToLower());
						twhlogintmp = null;
						Task.Factory.StartNew(() => TechF.Twitch.API.Req_BotFollowtoChannel());
					}
				}
				if (twhpasstmp != null) { // -- OAuth
					if (!twhpasstmp.StartsWith("oauth:")) { twhpasstmp = "oauth:" + twhpasstmp; }
					TechF.db.SettingsTWHT.UpdateSetting("Pass", twhpasstmp);
					twhpasstmp = null;
				}

				// -- VKPL
				if (vkplchanneltmp != null) { // -- Канал
					TechF.db.SettingsVKPLT.UpdateSetting("Channel", vkplchanneltmp.ToLower());
					vkplchanneltmp = null;
					//TechF.VKPL.API.Req_UpdateBotChannelInfo();
					//Task.Factory.StartNew(() => TechF.VKPL.API.Req_BotFollowtoChannel());
				}
				if (vkpllogintmp != null) { // -- Логин
					TechF.db.SettingsVKPLT.UpdateSetting("Login", vkpllogintmp.ToLower());
					vkpllogintmp = null;
					//TechF.VKPL.API.Req_UpdateBotChannelInfo();
					//Task.Factory.StartNew(() => TechF.VKPL.API.Req_BotFollowtoChannel());
				}
				if (twhpasstmp != null) { // -- Пароль
					//if (!passtmp.StartsWith("oauth:")) { vkplpasstmp = "oauth:" + vkplpasstmp; }
					TechF.db.SettingsVKPLT.UpdateSetting("Pass", vkplpasstmp);
					vkplpasstmp = null;
				}

				// -- Общие настройки
				if (chathistorydeltmp != null) { // -- История чата
					TechF.db.SettingsT.UpdateSetting("ChatHistoryDel", chathistorydeltmp);
					chathistorydeltmp = null;
				}
				if (spammsgtmp != null) { // -- Спам сообщениями
					TechF.db.SettingsT.UpdateSetting("SpamMsgTime", spammsgtmp);
					spammsgtmp = null;
				}

				settingchangework = false;
				Ev_SaveStatus?.Invoke(this, new Twident_Status(1, "Изменения сохранены", null, null));
				Thread.Sleep(3000);
				this.Dispatcher.Invoke(() => { if (lStatus.Text.ToString() == "Изменения сохранены") {
					Ev_GlobalStatus?.Invoke(this, new Twident_Status(0, "Пока без ошибок", null, false));
					Ev_SaveStatus?.Invoke(this, new Twident_Status(0, "", null, false));
				}});
			}
		}



		// -- Открытие окна "О программе"
		private void bAbout_Click(object sender, RoutedEventArgs e) {
			TechF.MainWin.AboutWin = new AboutWin(TechF);
			TechF.MainWin.AboutWin.Show();
		}


		// -- Изменение статуса авторизации --
		private void SaveStatus_Change(object sender, Twident_Status e) {
			//lsn = true;
			if (!PermStatus || (e.Permanent == false && e.Message != "")) {

				this.Dispatcher.Invoke(() => {
					switch (e.StatusCode) {
						case 0:
							lStatus.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA");
						break;
						case 1:
							lStatus.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#33AA66");
						break;
						case 2:
							lStatus.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#FDE100");
						break;
						case 3:
							lStatus.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DE5A4A");
						break;
					}

					this.lStatus.Text = e.Message;
					if (e.Permanent == true) { this.PermStatus = true; } else { PermStatus = false; }
				});
			}

			/*if (!lsw) { // -- Старая система очистки статуса спустя время неактивности
				Task.Factory.StartNew(() => {
					lsw = true;
					while (lsn) {
						lsn = false;
						for (uint i = 0; i < 30; i++) {
							Thread.Sleep(100);
							if (lsn) { break; }
						}
					}
					if (!lsp) { this.Dispatcher.Invoke(() => this.lStatus.Text = ""); }
					lsw = false;
				});
			}*/
		}


		// -- Активация/деактивация платформ --
		private void cbTWH_Active_Toggled(object sender, RoutedEventArgs e) {
			if (cbTWH_Active.IsOn == true) {
				Task.Factory.StartNew(() => { TechF.db.SettingsT.UpdateSetting("TwitchActive", "1"); TechF.Twitch.UpdateActive(); });
			} else {
				Task.Factory.StartNew(() => { TechF.db.SettingsT.UpdateSetting("TwitchActive", "0"); TechF.Twitch.UpdateActive(); });
			}
		}
		private void cbVKPL_Active_Toggled(object sender, RoutedEventArgs e) {
			if (cbVKPL_Active.IsOn == true) {
				Task.Factory.StartNew(() => { TechF.db.SettingsT.UpdateSetting("VKPLActive", "1"); TechF.Twitch.UpdateActive(); });
			} else {
				Task.Factory.StartNew(() => { TechF.db.SettingsT.UpdateSetting("VKPLActive", "0"); TechF.Twitch.UpdateActive(); });
			}
		}




		// -- Блок функций для настройки подключения к Twitch --
		// -- Открытие ссылки на страницу получения OAuth кода --
		private void OAuthTWHLink_Click(object sender, RoutedEventArgs e) {
			TechF.Twitch.API.GetOAuthCode();
		}

		// -- Установка значений по ивенту --
		private void TWHLoginBotSet(object sender, Twident_Null e) {
			string strl = TechF.TechFuncs.GetSettingTWHParam("LoginDisp");
			this.Dispatcher.Invoke(() => { this.eTWH_Login.Text = strl; });
		}
		private void TWHLoginChannelSet(object sender, Twident_Null e) {
			string strl = TechF.TechFuncs.GetSettingTWHParam("ChannelDisp");
			this.Dispatcher.Invoke(() => { this.eTWH_Channel.Text = strl; });
		}


		// -- Кнопка входа в аккаунт --
		private void bTWH_Login_Click(object sender, RoutedEventArgs e) {
			if (bTWH_Login.Content.ToString() == "Войти как бот" || (bTWH_Login.Content.ToString() == "Войти" && cbTWH_BCh.IsOn == true)) {
				TechF.Twitch.API.AuthorizationviaBrowser(false);
			}
			if (bTWH_Login.Content.ToString() == "Войти как канал") {
				TechF.Twitch.API.AuthorizationviaBrowser(true);
			}
		}

		
		// -- Изменение режима бота - бот и канал один и тот же акк или нет --
		private void cbTWH_BCh_Toggled(object sender, RoutedEventArgs e) {
			if (!isLoad) {
				if (cbTWH_BCh.IsOn == true) {
					Task.Factory.StartNew(() => {
						TechF.db.SettingsTWHT.UpdateSetting("BotChannel_isOne", "1");
						TechF.Twitch.BotChannelisOne = true;
					});
				} else {
					Task.Factory.StartNew(() => {
						TechF.db.SettingsTWHT.UpdateSetting("BotChannel_isOne", "0");
						TechF.Twitch.BotChannelisOne = false;
					});
				}
				bTWH_Login_Change();
			}
		}


		// -- Изменение кнопки входа --
		public void bTWH_Login_Change() {
			if (cbTWH_MM.IsOn == true) {
				bTWH_Login.Content = "Войти";
				bTWH_Login.IsEnabled = false;
			} else {
				string ba = TechF.TechFuncs.GetSettingParam("Bot_AuthCode");
				string aa = TechF.TechFuncs.GetSettingParam("Channel_AuthCode");

				if (cbTWH_BCh.IsOn == true) {
					if (ba == "true") {
						bTWH_Login.Content = "Вход выполнен";
						//bTWH_Login.IsEnabled = false;
					} else {
						bTWH_Login.Content = "Войти";
						bTWH_Login.IsEnabled = true;
					}
				} else {
					if (ba == "true" && aa == "true") {
						bTWH_Login.Content = "Вход выполнен";
						//bTWH_Login.IsEnabled = false;
					} else {
						if (ba == "true") {
							bTWH_Login.Content = "Войти как канал";
							bTWH_Login.IsEnabled = true;
						} else {
							bTWH_Login.Content = "Войти как бот";
							bTWH_Login.IsEnabled = true;
						}
					}
				}
			}
		}


		// -- Изменение режима авторизации --
		private void cbTWH_MM_Toggled(object sender, RoutedEventArgs e) {
			if (!isLoad) {
				if (cbTWH_MM.IsOn == true) {
					Task.Factory.StartNew(() => TechF.db.SettingsTWHT.UpdateSetting("ManualMode", "1"));
					eTWH_Channel.IsEnabled = true;
					eTWH_Login.IsEnabled = true;
					eTWH_Pass.IsEnabled = true;
					eTWH_Pass2.IsEnabled = true;
					hlOAuthTWH.Visibility = Visibility.Visible;
				} else {
					Task.Factory.StartNew(() => TechF.db.SettingsTWHT.UpdateSetting("ManualMode", "0"));
					eTWH_Channel.IsEnabled = false;
					eTWH_Login.IsEnabled = false;
					eTWH_Pass.IsEnabled = false;
					eTWH_Pass2.IsEnabled = false;
					hlOAuthTWH.Visibility = Visibility.Hidden;
				}
				bTWH_Login_Change();
			}
		}


		// -- Скрытие/показ оатч кода
		private void bTWH_PassH_Click(object sender, RoutedEventArgs e) {
			if (eTWH_Pass.Visibility == Visibility.Visible) {
				eTWH_Pass2.Text = eTWH_Pass.Password;
				eTWH_Pass2.Visibility = Visibility.Visible;
				eTWH_Pass.Visibility = Visibility.Collapsed;
				bTWH_PassH.Content = Resources["eyew2"];
			} else {
				eTWH_Pass.Visibility = Visibility.Visible;
				eTWH_Pass2.Visibility = Visibility.Collapsed;
				bTWH_PassH.Content = Resources["eyew1"];
			}
		}

		// -- Смена надписи в кнопке --
		private void bTWH_Login_MouseEnter(object sender, MouseEventArgs e) {
			if (bTWH_Login.Content == "Вход выполнен") {
				bTWH_Login.Content = "Войти снова";
			}
		}
		private void bTWH_Login_MouseLeave(object sender, MouseEventArgs e) {
			if (bTWH_Login.Content == "Войти снова") {
				bTWH_Login.Content = "Вход выполнен";
			}
		}


		// -- Блок функций для настройки подключения к VKPL --
		// -- Открытие ссылки на страницу получения пароля --
		private void OAuthVKPLLink_Click(object sender, RoutedEventArgs e) {
			//TechF.VKPL.API.GetOAuthCode();
			bool da = TechF.VKPL.APIBRW.GetLoginNick();
			string str = TechF.VKPL.APIBRW.BRWoptions.BrowserName;
			str = TechF.VKPL.APIBRW.BRWoptions.BrowserVersion;
		}

		// -- Установка значений по ивенту --
		private void VKPLLoginBotSet(object sender, Twident_Null e) {
			string strl = TechF.TechFuncs.GetSettingVKPLParam("LoginDisp");
			this.Dispatcher.Invoke(() => { this.eVKPL_Login.Text = strl; });
		}
		private void VKPLLoginChannelSet(object sender, Twident_Null e) {
			string strl = TechF.TechFuncs.GetSettingVKPLParam("ChannelDisp");
			this.Dispatcher.Invoke(() => { this.eVKPL_Channel.Text = strl; });
		}


		// -- Кнопка входа в аккаунт --
		private void bVKPL_Login_Click(object sender, RoutedEventArgs e) {
			if (this.isVKPLBRWLogin) {
				this.isVKPLBRWLogin = false;
				Ev_SaveStatus?.Invoke(this, new Twident_Status(2, "Сохранение...", null, null));
				Task.Factory.StartNew(() => {
					if (!TechF.VKPL.APIBRW.GetAuthCookie()) {
						Ev_SaveStatus?.Invoke(this, new Twident_Status(2, "Войдите в аккаунт", null, false));
						return;
					}
					if (!TechF.VKPL.APIBRW.GetLoginNick()) {
						Ev_SaveStatus?.Invoke(this, new Twident_Status(3, "Ошибка браузера", null, true));
						return;
					}
					TechF.VKPL.APIBRW.CloseBRW();
					this.Dispatcher.Invoke(() => {
						this.bVKPL_Login.IsEnabled = true;
						this.bVKPL_Login.Content = "Войти повторно";
						this.eVKPL_Login.Text = TechF.TechFuncs.GetSettingVKPLParam("LoginDisp");
					});
					Ev_SaveStatus?.Invoke(this, new Twident_Status(1, "Вход выполнен", null, false));
				});
			} else {
				this.bVKPL_Login.IsEnabled = false;
				this.isVKPLBRWLogin = true;
				Ev_SaveStatus?.Invoke(this, new Twident_Status(0, "Открытие браузера...", null, null));
				Task.Factory.StartNew(() => {
					TechF.VKPL.APIBRW.OpenBrowserforAuthorization();
					this.Dispatcher.Invoke(() => {
						this.bVKPL_Login.IsEnabled = true;
						this.bVKPL_Login.Content = "Сохранить вход";
					});
					Ev_SaveStatus?.Invoke(this, new Twident_Status(2, "Для сохранения авторизации повторно нажмите кнопку входа", null, true));
				});
			}
			//TechF.VKPL.APIBRW.Login();
		}


		// -- Изменение режима бота - бот и канал один и тот же акк или нет --
		private void cbVKPL_BCh_Toggled(object sender, RoutedEventArgs e) {
			if (!isLoad) {
				if (cbVKPL_BCh.IsOn == true) {
					Task.Factory.StartNew(() => {
						TechF.db.SettingsVKPLT.UpdateSetting("BotChannel_isOne", "1");
						TechF.VKPL.BotChannelisOne = true;
					});

				} else {
					Task.Factory.StartNew(() => {
						TechF.db.SettingsVKPLT.UpdateSetting("BotChannel_isOne", "0");
						TechF.VKPL.BotChannelisOne = false;
					});
				}
				//bVKPL_Login_Change();
			}
		}


		// -- Изменение кнопки входа --
		public void bVKPL_Login_Change() {
			if (cbVKPL_MM.IsOn == true) {
				bVKPL_Login.Content = "Войти";
				bVKPL_Login.IsEnabled = false;
			} else {
				string ba = TechF.TechFuncs.GetSettingVKPLParam("Bot_AuthCode");
				string aa = TechF.TechFuncs.GetSettingVKPLParam("Channel_AuthCode");
				aa = "true";

				if (cbVKPL_BCh.IsOn == true) {
					if (ba == "true") {
						bVKPL_Login.Content = "Вход выполнен";
						//bVKPL_Login.IsEnabled = false;
					} else {
						bVKPL_Login.Content = "Войти";
						bVKPL_Login.IsEnabled = true;
					}
				} else {
					if (ba == "true" && aa == "true") {
						bVKPL_Login.Content = "Вход выполнен";
						//bVKPL_Login.IsEnabled = false;
					} else {
						if (ba == "true") {
							bVKPL_Login.Content = "Войти как канал";
							bVKPL_Login.IsEnabled = true;
						} else {
							bVKPL_Login.Content = "Войти как бот";
							bVKPL_Login.IsEnabled = true;
						}
					}
				}
			}
		}


		// -- Изменение режима авторизации --
		private void cbVKPL_MM_Toggled(object sender, RoutedEventArgs e) {
			if (!isLoad) {
				if (cbVKPL_MM.IsOn == true) {
					Task.Factory.StartNew(() => TechF.db.SettingsVKPLT.UpdateSetting("ManualMode", "1"));
					eVKPL_Channel.IsEnabled = true;
					eVKPL_Login.IsEnabled = true;
					eVKPL_Pass.IsEnabled = true;
					eVKPL_Pass2.IsEnabled = true;
					hlOAuthVKPL.Visibility = Visibility.Visible;
				} else {
					Task.Factory.StartNew(() => TechF.db.SettingsVKPLT.UpdateSetting("ManualMode", "0"));
					eVKPL_Channel.IsEnabled = false;
					eVKPL_Login.IsEnabled = false;
					eVKPL_Pass.IsEnabled = false;
					eVKPL_Pass2.IsEnabled = false;
					hlOAuthVKPL.Visibility = Visibility.Hidden;
				}
				//bVKPL_Login_Change();
			}
		}


		// -- Изменения режима работы при рутони
		private void cbVKPL_RC_Toggled(object sender, RoutedEventArgs e) {
			if (!isLoad) {
				if (cbVKPL_RC.IsOn == true) {
					Task.Factory.StartNew(() => TechF.db.SettingsVKPLT.UpdateSetting("RutonyMode", "1"));
				} else {
					Task.Factory.StartNew(() => TechF.db.SettingsVKPLT.UpdateSetting("RutonyMode", "0"));
				}
				//bVKPL_Login_Change();
			}
		}


		// -- Скрытие/показ пароля
		private void bVKPL_PassH_Click(object sender, RoutedEventArgs e) {
			if (eVKPL_Pass.Visibility == Visibility.Visible) {
				eVKPL_Pass2.Text = eVKPL_Pass.Password;
				eVKPL_Pass2.Visibility = Visibility.Visible;
				eVKPL_Pass.Visibility = Visibility.Collapsed;
				bVKPL_PassH.Content = Resources["eyew2"];
			} else {
				eVKPL_Pass.Visibility = Visibility.Visible;
				eVKPL_Pass2.Visibility = Visibility.Collapsed;
				bVKPL_PassH.Content = Resources["eyew1"];
			}
		}

		// -- Смена надписи в кнопке --
		private void bVKPL_Login_MouseEnter(object sender, MouseEventArgs e) {
			if (bVKPL_Login.Content == "Вход выполнен") {
				bVKPL_Login.Content = "Войти снова";
			}
		}
		private void bVKPL_Login_MouseLeave(object sender, MouseEventArgs e) {
			if (bVKPL_Login.Content == "Войти снова") {
				bVKPL_Login.Content = "Вход выполнен";
			}
		}



		// -- Общие настройки --
		// -- Переключение режима работы спама сообщениями
		private void cbSpamMessages_Toggled(object sender, RoutedEventArgs e) {
			if (cbSpamMessages.IsOn == true) {
				Task.Factory.StartNew(() => TechF.db.SettingsT.UpdateSetting("SpamMsgActive", "1"));
			} else {
				Task.Factory.StartNew(() => TechF.db.SettingsT.UpdateSetting("SpamMsgActive", "0"));
			}
		}


		// -- Изменение типа спама сообщений --
		private void rbSpamMessages_Changed(object sender, RoutedEventArgs e) {
			if (!isLoad) {
				if (rbSpamMessages_0.IsChecked == true) {
					//spammsgttmp = "0";
					eSpamMessages_Cust.Text = TechF.TechFuncs.GetSettingParam("SpamMsgTime");
					eSpamMessages_Cust.IsEnabled = true;
					/*settingchangeadd = true;
					if (!settingchangework) {
						Task.Factory.StartNew(() => { SettingChange(); });
					}*/
				}
				if (rbSpamMessages_1.IsChecked == true) {
					spammsgtmp = "0";
					eSpamMessages_Cust.Text = "0";
					eSpamMessages_Cust.IsEnabled = false;
					settingchangeadd = true;
					if (!settingchangework) {
						Task.Factory.StartNew(() => { SettingChange(); });
					}
				}
			}
		}



		// -- Переключение режима работы истории чата
		private void cbChatHistory_Toggled(object sender, RoutedEventArgs e) {
			if (!isLoad) {
				if (cbSpamMessages.IsOn == true) {
					Task.Factory.StartNew(() => TechF.db.SettingsT.UpdateSetting("SpamMsgActive", "1"));
				} else {
					Task.Factory.StartNew(() => TechF.db.SettingsT.UpdateSetting("SpamMsgActive", "0"));
				}
			}
		}


		// -- Изменение периода удаления истории сообщений
		private void rbChatHistory_Changed(object sender, RoutedEventArgs e) {
			if (!isLoad) {
				string daydel = eChatHistory_CustDel.Text;
				if (rbChatHistory_Del1.IsChecked == true) {
					chathistorydeltmp = "3";
					eChatHistory_CustDel.Text = "";
					settingchangeadd = true;
					eChatHistory_CustDel.IsEnabled = false;
					if (!settingchangework) {
						Task.Factory.StartNew(() => { SettingChange(); });
					}
				}
				if (rbChatHistory_Del2.IsChecked == true) {
					chathistorydeltmp = "7";
					eChatHistory_CustDel.Text = "";
					settingchangeadd = true;
					eChatHistory_CustDel.IsEnabled = false;
					if (!settingchangework) {
						Task.Factory.StartNew(() => { SettingChange(); });
					}
				}
				if (rbChatHistory_Del3.IsChecked == true) {
					chathistorydeltmp = "30";
					eChatHistory_CustDel.Text = "";
					settingchangeadd = true;
					eChatHistory_CustDel.IsEnabled = false;
					if (!settingchangework) {
						Task.Factory.StartNew(() => { SettingChange(); });
					}
				}
				if (rbChatHistory_Del0.IsChecked == true) {
					chathistorydeltmp = "0";
					eChatHistory_CustDel.Text = "";
					settingchangeadd = true;
					eChatHistory_CustDel.IsEnabled = false;
					if (!settingchangework) {
						Task.Factory.StartNew(() => { SettingChange(); });
					}
				}
				if (rbChatHistory_DelCust.IsChecked == true) {
					eChatHistory_CustDel.IsEnabled = true;
				}
				settingchangeadd = true;
			}
		}






		// --- Cистема ограничений ---
		// -- Общий блок для некоторых PTI-ограничений --
		private void e_PTI(object sender, TextCompositionEventArgs e) { // -- Поле канала
			if (e.Text.Contains(" ") || Char.IsWhiteSpace(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}


		// -- Twitch --
		// -- Поле канала
		private void eTWH_Channel_PKD(object sender, KeyEventArgs e) {
			twhchannelprev = eTWH_Channel.Text.ToString();

			if (e.Key == Key.Space || e.Key == Key.Decimal || e.Key == Key.Divide || e.Key == Key.Multiply || e.Key == Key.Subtract || e.Key == Key.Oem1 || e.Key == Key.Oem2 || e.Key == Key.Oem3 || e.Key == Key.Oem4 || e.Key == Key.Oem5 || e.Key == Key.Oem6 || e.Key == Key.Oem7 || e.Key == Key.Oem8 || e.Key == Key.Oem102 || e.Key == Key.OemBackslash || e.Key == Key.OemOpenBrackets || e.Key == Key.OemCloseBrackets || e.Key == Key.OemComma || e.Key == Key.OemPlus || e.Key == Key.OemMinus || e.Key == Key.OemPeriod || e.Key == Key.OemPipe || e.Key == Key.OemQuestion || e.Key == Key.OemQuotes || e.Key == Key.OemSemicolon || e.Key == Key.OemTilde) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains(" ") || str.Contains("\t") || str.Contains("\n")) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eTWH_Channel_Changed(object sender, TextChangedEventArgs e) {
			if (eTWH_Channel.Text.ToString().Contains(" ") || eTWH_Channel.Text.ToString().Contains("\t") || eTWH_Channel.Text.ToString().Contains("\n")) {
				eTWH_Channel.Text = twhchannelprev;
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eTWH_Channel_PKU(object sender, KeyEventArgs e) { // -- Запуск сохранения
			if (eTWH_Channel.Text.ToString() != twhchannelprev) {
				twhchannelprev = eTWH_Channel.Text.ToString();
				if (eTWH_Channel.Text.ToString() != "") {
					twhchanneltmp = eTWH_Channel.Text.ToString();
				} else {
					twhchanneltmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}



		// -- Поле логина  --
		private void eTWH_Login_PKD(object sender, KeyEventArgs e) {
			twhloginprev = eTWH_Login.Text;

			if (e.Key == Key.Space || e.Key == Key.Decimal || e.Key == Key.Divide || e.Key == Key.Multiply || e.Key == Key.Subtract || e.Key == Key.Oem1 || e.Key == Key.Oem2 || e.Key == Key.Oem3 || e.Key == Key.Oem4 || e.Key == Key.Oem5 || e.Key == Key.Oem6 || e.Key == Key.Oem7 || e.Key == Key.Oem8 || e.Key == Key.Oem102 || e.Key == Key.OemBackslash || e.Key == Key.OemOpenBrackets || e.Key == Key.OemCloseBrackets || e.Key == Key.OemComma || e.Key == Key.OemPlus || e.Key == Key.OemMinus || e.Key == Key.OemPeriod || e.Key == Key.OemPipe || e.Key == Key.OemQuestion || e.Key == Key.OemQuotes || e.Key == Key.OemSemicolon || e.Key == Key.OemTilde) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains(" ") || str.Contains("\t") || str.Contains("\n") || str.Contains("~") || str.Contains("!") || str.Contains("@") || str.Contains("#") || str.Contains("$") || str.Contains("%") || str.Contains("^") || str.Contains("&") || str.Contains("*") || str.Contains("(") || str.Contains(")") || str.Contains("-") || str.Contains("_") || str.Contains("=") || str.Contains("+") || str.Contains("[") || str.Contains("{") || str.Contains("]") || str.Contains("}") || str.Contains(";") || str.Contains(":") || str.Contains("'") || str.Contains("\"") || str.Contains(",") || str.Contains("<") || str.Contains(".") || str.Contains(">") || str.Contains("/") || str.Contains("?") || str.Contains("\\") || str.Contains("|")) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eTWH_Login_Changed(object sender, TextChangedEventArgs e) {
			if (eTWH_Login.Text.Contains(" ") || eTWH_Login.Text.Contains("\t") || eTWH_Login.Text.Contains("\n")) {
				eTWH_Login.Text = twhloginprev;
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eTWH_Login_PKU(object sender, KeyEventArgs e) { // -- Запуск сохранения
			if (eTWH_Login.Text != twhloginprev) {
				twhloginprev = eTWH_Login.Text;
				if (eTWH_Login.Text != "") {
					twhlogintmp = eTWH_Login.Text;
				} else {
					twhlogintmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}

		// -- Скрывающее поле оатч кода --

		private void eTWH_Pass_PKD(object sender, KeyEventArgs e) {
			twhpassprev = eTWH_Pass.Password;
			TechF.TechFuncs.LogDH(e.Key.ToString());
			if (e.Key == Key.Space || e.Key == Key.Decimal || e.Key == Key.Divide || e.Key == Key.Multiply || e.Key == Key.Subtract || e.Key == Key.Oem2 || e.Key == Key.Oem3 || e.Key == Key.Oem4 || e.Key == Key.Oem5 || e.Key == Key.Oem6 || e.Key == Key.Oem7 || e.Key == Key.Oem8 || e.Key == Key.Oem102 || e.Key == Key.OemBackslash || e.Key == Key.OemOpenBrackets || e.Key == Key.OemCloseBrackets || e.Key == Key.OemComma || e.Key == Key.OemPlus || e.Key == Key.OemMinus || e.Key == Key.OemPeriod || e.Key == Key.OemPipe || e.Key == Key.OemQuestion || e.Key == Key.OemQuotes || e.Key == Key.OemSemicolon || e.Key == Key.OemTilde) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains(" ") || str.Contains("\t") || str.Contains("\n") || str.Contains("~") || str.Contains("!") || str.Contains("@") || str.Contains("#") || str.Contains("$") || str.Contains("%") || str.Contains("^") || str.Contains("&") || str.Contains("*") || str.Contains("(") || str.Contains(")") || str.Contains("-") || str.Contains("_") || str.Contains("=") || str.Contains("+") || str.Contains("[") || str.Contains("{") || str.Contains("]") || str.Contains("}") || str.Contains(";") || str.Contains("'") || str.Contains("\"") || str.Contains(",") || str.Contains("<") || str.Contains(".") || str.Contains(">") || str.Contains("/") || str.Contains("?") || str.Contains("\\") || str.Contains("|")) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eTWH_Pass_PKU(object sender, KeyEventArgs e) {
			if (eTWH_Pass.Password != twhpassprev) {
				twhpassprev = eTWH_Pass.Password;
				if (eTWH_Pass.Password != "") {
					twhpasstmp = eTWH_Pass.Password;
				} else {
					twhpasstmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}

		// -- Раскрывающее поле оатч кода --
		private void eTWH_Pass2_PTI(object sender, TextCompositionEventArgs e) {
			if (e.Text.Contains(" ") || Char.IsWhiteSpace(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
				eTWH_Pass.Password = eTWH_Pass2.Text;
			}
		}
		private void eTWH_Pass2_PKD(object sender, KeyEventArgs e) {
			eTWH_Pass.Password = eTWH_Pass2.Text;
			twhpassprev = eTWH_Pass.Password;

			if (e.Key == Key.Space || e.Key == Key.Decimal || e.Key == Key.Divide || e.Key == Key.Multiply || e.Key == Key.Subtract || e.Key == Key.Oem1 || e.Key == Key.Oem2 || e.Key == Key.Oem3 || e.Key == Key.Oem4 || e.Key == Key.Oem5 || e.Key == Key.Oem6 || e.Key == Key.Oem7 || e.Key == Key.Oem8 || e.Key == Key.Oem102 || e.Key == Key.OemBackslash || e.Key == Key.OemOpenBrackets || e.Key == Key.OemCloseBrackets || e.Key == Key.OemComma || e.Key == Key.OemPlus || e.Key == Key.OemMinus || e.Key == Key.OemPeriod || e.Key == Key.OemPipe || e.Key == Key.OemQuestion || e.Key == Key.OemQuotes || e.Key == Key.OemSemicolon || e.Key == Key.OemTilde) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains(" ") || str.Contains("\t") || str.Contains("\n") || str.Contains("~") || str.Contains("!") || str.Contains("@") || str.Contains("#") || str.Contains("$") || str.Contains("%") || str.Contains("^") || str.Contains("&") || str.Contains("*") || str.Contains("(") || str.Contains(")") || str.Contains("-") || str.Contains("_") || str.Contains("=") || str.Contains("+") || str.Contains("[") || str.Contains("{") || str.Contains("]") || str.Contains("}") || str.Contains(";") || str.Contains(":") || str.Contains("'") || str.Contains("\"") || str.Contains(",") || str.Contains("<") || str.Contains(".") || str.Contains(">") || str.Contains("/") || str.Contains("?") || str.Contains("\\") || str.Contains("|")) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eTWH_Pass2_PKU(object sender, KeyEventArgs e) {
			eTWH_Pass.Password = eTWH_Pass2.Text;
			if (eTWH_Pass.Password != twhpassprev) {
				twhpassprev = eTWH_Pass.Password;
				if (eTWH_Pass.Password != "") {
					twhpasstmp = eTWH_Pass.Password;
				} else {
					twhpasstmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
			eTWH_Pass.Password = eTWH_Pass2.Text;
		}



		// -- VKPL --
		// -- Поле канала
		private void eVKPL_Channel_PKD(object sender, KeyEventArgs e) {
			vkplchannelprev = eVKPL_Channel.Text.ToString();

			if (e.Key == Key.Space || e.Key == Key.Decimal || e.Key == Key.Divide || e.Key == Key.Multiply || e.Key == Key.Subtract || e.Key == Key.Oem1 || e.Key == Key.Oem2 || e.Key == Key.Oem3 || e.Key == Key.Oem4 || e.Key == Key.Oem5 || e.Key == Key.Oem6 || e.Key == Key.Oem7 || e.Key == Key.Oem8 || e.Key == Key.Oem102 || e.Key == Key.OemBackslash || e.Key == Key.OemOpenBrackets || e.Key == Key.OemCloseBrackets || e.Key == Key.OemComma || e.Key == Key.OemPlus || e.Key == Key.OemMinus || e.Key == Key.OemPeriod || e.Key == Key.OemPipe || e.Key == Key.OemQuestion || e.Key == Key.OemQuotes || e.Key == Key.OemSemicolon || e.Key == Key.OemTilde) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains(" ") || str.Contains("\t") || str.Contains("\n")) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eVKPL_Channel_Changed(object sender, TextChangedEventArgs e) {
			if (eVKPL_Channel.Text.ToString().Contains(" ") || eVKPL_Channel.Text.ToString().Contains("\t") || eVKPL_Channel.Text.ToString().Contains("\n")) {
				eVKPL_Channel.Text = vkplchannelprev;
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eVKPL_Channel_PKU(object sender, KeyEventArgs e) { // -- Запуск сохранения
			if (eVKPL_Channel.Text.ToString() != vkplchannelprev) {
				vkplchannelprev = eVKPL_Channel.Text.ToString();
				if (eVKPL_Channel.Text.ToString() != "") {
					vkplchanneltmp = eVKPL_Channel.Text.ToString();
				} else {
					vkplchanneltmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}

		// -- Поле логина  --
		private void eVKPL_Login_PKD(object sender, KeyEventArgs e) {
			vkplloginprev = eVKPL_Login.Text;

			if (e.Key == Key.Space || e.Key == Key.Decimal || e.Key == Key.Divide || e.Key == Key.Multiply || e.Key == Key.Subtract || e.Key == Key.Oem1 || e.Key == Key.Oem2 || e.Key == Key.Oem3 || e.Key == Key.Oem4 || e.Key == Key.Oem5 || e.Key == Key.Oem6 || e.Key == Key.Oem7 || e.Key == Key.Oem8 || e.Key == Key.Oem102 || e.Key == Key.OemBackslash || e.Key == Key.OemOpenBrackets || e.Key == Key.OemCloseBrackets || e.Key == Key.OemComma || e.Key == Key.OemPlus || e.Key == Key.OemMinus || e.Key == Key.OemPeriod || e.Key == Key.OemPipe || e.Key == Key.OemQuestion || e.Key == Key.OemQuotes || e.Key == Key.OemSemicolon || e.Key == Key.OemTilde) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains(" ") || str.Contains("\t") || str.Contains("\n") || str.Contains("~") || str.Contains("!") || str.Contains("@") || str.Contains("#") || str.Contains("$") || str.Contains("%") || str.Contains("^") || str.Contains("&") || str.Contains("*") || str.Contains("(") || str.Contains(")") || str.Contains("-") || str.Contains("_") || str.Contains("=") || str.Contains("+") || str.Contains("[") || str.Contains("{") || str.Contains("]") || str.Contains("}") || str.Contains(";") || str.Contains(":") || str.Contains("'") || str.Contains("\"") || str.Contains(",") || str.Contains("<") || str.Contains(".") || str.Contains(">") || str.Contains("/") || str.Contains("?") || str.Contains("\\") || str.Contains("|")) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eVKPL_Login_Changed(object sender, TextChangedEventArgs e) {
			if (eVKPL_Login.Text.Contains(" ") || eVKPL_Login.Text.Contains("\t") || eVKPL_Login.Text.Contains("\n")) {
				eVKPL_Login.Text = vkplloginprev;
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eVKPL_Login_PKU(object sender, KeyEventArgs e) { // -- Запуск сохранения
			if (eVKPL_Login.Text != vkplloginprev) {
				vkplloginprev = eVKPL_Login.Text;
				if (eVKPL_Login.Text != "") {
					vkpllogintmp = eVKPL_Login.Text;
				} else {
					vkpllogintmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}

		// -- Скрывающее поле оатч кода --
		private void eVKPL_Pass_PKD(object sender, KeyEventArgs e){
			vkplpassprev = eVKPL_Pass.Password;
			TechF.TechFuncs.LogDH(e.Key.ToString());
			if (e.Key == Key.Space || e.Key == Key.Decimal || e.Key == Key.Divide || e.Key == Key.Multiply || e.Key == Key.Subtract || e.Key == Key.Oem2 || e.Key == Key.Oem3 || e.Key == Key.Oem4 || e.Key == Key.Oem5 || e.Key == Key.Oem6 || e.Key == Key.Oem7 || e.Key == Key.Oem8 || e.Key == Key.Oem102 || e.Key == Key.OemBackslash || e.Key == Key.OemOpenBrackets || e.Key == Key.OemCloseBrackets || e.Key == Key.OemComma || e.Key == Key.OemPlus || e.Key == Key.OemMinus || e.Key == Key.OemPeriod || e.Key == Key.OemPipe || e.Key == Key.OemQuestion || e.Key == Key.OemQuotes || e.Key == Key.OemSemicolon || e.Key == Key.OemTilde) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains(" ") || str.Contains("\t") || str.Contains("\n") || str.Contains("~") || str.Contains("!") || str.Contains("@") || str.Contains("#") || str.Contains("$") || str.Contains("%") || str.Contains("^") || str.Contains("&") || str.Contains("*") || str.Contains("(") || str.Contains(")") || str.Contains("-") || str.Contains("_") || str.Contains("=") || str.Contains("+") || str.Contains("[") || str.Contains("{") || str.Contains("]") || str.Contains("}") || str.Contains(";") || str.Contains("'") || str.Contains("\"") || str.Contains(",") || str.Contains("<") || str.Contains(".") || str.Contains(">") || str.Contains("/") || str.Contains("?") || str.Contains("\\") || str.Contains("|")) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eVKPL_Pass_PKU(object sender, KeyEventArgs e) {
			if (eVKPL_Pass.Password != vkplpassprev) {
				vkplpassprev = eVKPL_Pass.Password;
				if (eVKPL_Pass.Password != "") {
					vkplpasstmp = eVKPL_Pass.Password;
				} else {
					vkplpasstmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}

		// -- Раскрывающее поле оатч кода --
		private void eVKPL_Pass2_PTI(object sender, TextCompositionEventArgs e) {
			if (e.Text.Contains(" ") || Char.IsWhiteSpace(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
				eVKPL_Pass.Password = eVKPL_Pass2.Text;
			}
		}
		private void eVKPL_Pass2_PKD(object sender, KeyEventArgs e) {
			eVKPL_Pass.Password = eVKPL_Pass2.Text;
			vkplpassprev = eVKPL_Pass.Password;

			if (e.Key == Key.Space || e.Key == Key.Decimal || e.Key == Key.Divide || e.Key == Key.Multiply || e.Key == Key.Subtract || e.Key == Key.Oem1 || e.Key == Key.Oem2 || e.Key == Key.Oem3 || e.Key == Key.Oem4 || e.Key == Key.Oem5 || e.Key == Key.Oem6 || e.Key == Key.Oem7 || e.Key == Key.Oem8 || e.Key == Key.Oem102 || e.Key == Key.OemBackslash || e.Key == Key.OemOpenBrackets || e.Key == Key.OemCloseBrackets || e.Key == Key.OemComma || e.Key == Key.OemPlus || e.Key == Key.OemMinus || e.Key == Key.OemPeriod || e.Key == Key.OemPipe || e.Key == Key.OemQuestion || e.Key == Key.OemQuotes || e.Key == Key.OemSemicolon || e.Key == Key.OemTilde) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains(" ") || str.Contains("\t") || str.Contains("\n") || str.Contains("~") || str.Contains("!") || str.Contains("@") || str.Contains("#") || str.Contains("$") || str.Contains("%") || str.Contains("^") || str.Contains("&") || str.Contains("*") || str.Contains("(") || str.Contains(")") || str.Contains("-") || str.Contains("_") || str.Contains("=") || str.Contains("+") || str.Contains("[") || str.Contains("{") || str.Contains("]") || str.Contains("}") || str.Contains(";") || str.Contains(":") || str.Contains("'") || str.Contains("\"") || str.Contains(",") || str.Contains("<") || str.Contains(".") || str.Contains(">") || str.Contains("/") || str.Contains("?") || str.Contains("\\") || str.Contains("|")) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eVKPL_Pass2_PKU(object sender, KeyEventArgs e) {
			eVKPL_Pass.Password = eVKPL_Pass2.Text;
			if (eVKPL_Pass.Password != vkplpassprev) {
				vkplpassprev = eVKPL_Pass.Password;
				if (eVKPL_Pass.Password != "") {
					vkplpasstmp = eVKPL_Pass.Password;
				} else {
					vkplpasstmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
			eVKPL_Pass.Password = eVKPL_Pass2.Text;
		}



		// -- Общие --
		// -- Поле своего варианта периода удаления истории чата
		private void eSpamMessages_Cust_PTI(object sender, TextCompositionEventArgs e) {
			if (!Char.IsDigit(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eSpamMessages_Cust_PKD(object sender, KeyEventArgs e) {
			spammsgprev = eSpamMessages_Cust.Text;

			if (e.Key == Key.Space) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (!Char.IsDigit(str, 0)) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eSpamMessages_Cust_Changed(object sender, TextChangedEventArgs e) {
			string str = null;
			for (ushort i = 0; i < eSpamMessages_Cust.Text.Length; i++) {
				str = eSpamMessages_Cust.Text.ElementAt<char>(i).ToString();
				if (!Char.IsDigit(str, 0)) {
					eSpamMessages_Cust.Text = spammsgprev;
					e.Handled = true;
					break;
				} else {
					e.Handled = false;
				}
			}
			if (eChatHistory_CustDel.Text != "") {
				lChatHistory_Custday.Content = TechF.TechFuncs.WordEndNumber(Convert.ToInt32(eChatHistory_CustDel.Text), "D");
			}
		}
		private void eSpamMessages_Cust_PKU(object sender, KeyEventArgs e) {
			if (eSpamMessages_Cust.Text != spammsgprev) {
				spammsgprev = eSpamMessages_Cust.Text;
				if (eSpamMessages_Cust.Text != "") {
					spammsgtmp = eSpamMessages_Cust.Text;
				} else {
					spammsgtmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}

		// -- Поле своего варианта периода удаления истории чата
		private void eChatHistory_CustDel_PTI(object sender, TextCompositionEventArgs e) {
			if (!Char.IsDigit(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}
		private void eChatHistory_CustDel_PKD(object sender, KeyEventArgs e) {
			chathistorydelprev = eChatHistory_CustDel.Text;

			if (e.Key == Key.Space) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (!Char.IsDigit(str, 0)) {
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eChatHistory_CustDel_Changed(object sender, TextChangedEventArgs e) {
			string str = null;
			for (ushort i = 0; i < eChatHistory_CustDel.Text.Length; i++) {
				str = eChatHistory_CustDel.Text.ElementAt<char>(i).ToString();
				if (!Char.IsDigit(str, 0)) {
					eChatHistory_CustDel.Text = chathistorydelprev;
					e.Handled = true;
					break;
				} else {
					e.Handled = false;
				}
			}
			if (eChatHistory_CustDel.Text != "") {
				lChatHistory_Custday.Content = TechF.TechFuncs.WordEndNumber(Convert.ToInt32(eChatHistory_CustDel.Text), "D");
			}
		}
		private void eChatHistory_CustDel_PKU(object sender, KeyEventArgs e) {
			if (eChatHistory_CustDel.Text != chathistorydelprev) {
				chathistorydelprev = eChatHistory_CustDel.Text;
				if (eChatHistory_CustDel.Text != "") {
					chathistorydeltmp = eChatHistory_CustDel.Text;
				} else {
					chathistorydeltmp = null;
				}
				settingchangeadd = true;
				if (!settingchangework) {
					Task.Factory.StartNew(() => { SettingChange(); });
				}
			}
		}



		// -- Прочее
		private void bAbout_RightClick(object sender, MouseButtonEventArgs e) {
			cb_HideMode.Visibility = Visibility.Visible;
		}

		private void cb_HideMode_Changed(object sender, RoutedEventArgs e) {
			if (cb_HideMode.IsChecked == true) {
				TechF.HideMode = true;
			} else {
				TechF.HideMode = false;
			}
		}


		private void bSetTWH_LabelSet(object sender, MouseButtonEventArgs e) {
			this.lPlatform.Content = "Стрим. сервисы - Twitch";
		}

		private void bSetVKPL_LabelSet(object sender, MouseButtonEventArgs e) {
			this.lPlatform.Content = "Стрим. сервисы - VK Play LIVE";
		}

	}
}
