using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
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
using System.Windows.Threading;

namespace Twidibot.Pages {
	public class ListWrapC {
		public string Text { get; set; }
		public ImageSource ServiceImage { get; set; }
		public ListWrapC (string Text, ImageSource ServiceImage) {
			this.Text = Text;
			this.ServiceImage = ServiceImage;
		}
	}

	public partial class Status : Page {
		private BackWin TechF = null;
		public bool ToolTipShow = false;
		private bool PermStatusGlobal = false;
		private bool PermStatusTWH = false;
		private bool PermStatusVKPL = false;
		public event EventHandler<Twident_Status> Ev_GlobalStatus;

		public Status(BackWin backWin) {
			TechF = backWin;
			InitializeComponent();

			this.lChat.Items.Clear();
		}

		// -- Подписки на события статусов
		public void InitStatus() {
			this.Ev_GlobalStatus += GlobalStatus_Set;
			TechF.Twitch.Chat.Ev_ChatMsg += lChat_Add;
			TechF.Twitch.Chat.Ev_BotMsg += lChat_Add;
			TechF.VKPL.Chat.Ev_ChatMsg += lChat_Add;
			TechF.VKPL.Chat.Ev_BotMsg += lChat_Add;
			TechF.Ev_GlobalStatus += GlobalStatus_Set;
			//TechF.MainWin.Ev_GlobalStatus += StatusGlobal_Set;
			TechF.MainWin.PageAppSet.Ev_GlobalStatus += GlobalStatus_Set;

			TechF.Twitch.Chat.Ev_Status += TWHStatus_Set;
			TechF.Twitch.Chat.Ev_GlobalStatus += GlobalStatus_Set;
			TechF.VKPL.Chat.Ev_Status += VKPLStatus_Set;
			TechF.VKPL.Chat.Ev_GlobalStatus += GlobalStatus_Set;

			TechF.Twitch.API.Ev_Status += TWHStatus_Set;
			TechF.Twitch.API.Ev_GlobalStatus += GlobalStatus_Set;
			TechF.Twitch.Chat.Ev_ConnectStatus += OnOff_Set;
			TechF.VKPL.Chat.Ev_ConnectStatus += OnOff_Set;
			TechF.VKPL.Chat.Ev_tst += tst;
		}

		public void tst(object sender, Twident_Status e) {
			this.Dispatcher.Invoke(() => {
				switch (e.StatusCode) {
					case 0:
						this.l1.Content = e.Message;
					break;
					case 1:
						this.l2.Content = e.Message;
					break;
					case 2:
						this.l3.Content = e.Message;
					break;
					case 3:
						this.l4.Content = e.Message;
					break;
				}
			});
		}


		/*// -- Вывод ошибки чата --
		private void Error_Set(object sender, Twident_Msg e) {
			this.Dispatcher.Invoke(() => this.lError.Text = e.Message);
		}*/

		// -- Вывод глобального статуса --
		public void GlobalStatus_Set(object sender, Twident_Status e) {
			if (!PermStatusGlobal || e.Permanent == false && e.Message != null) {

				this.Dispatcher.Invoke(() => {
					switch (e.StatusCode) {
						case 0:
						this.lStatusGlobal.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA");
						break;
						case 1:
						this.lStatusGlobal.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#33AA66");
						break;
						case 2:
						this.lStatusGlobal.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#FDE100");
						break;
						case 3:
						this.lStatusGlobal.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DE5A4A");
						break;
					}

					this.lStatusGlobal.Content = e.Message;
					if (e.Permanent == true) { this.PermStatusGlobal = true; } else { this.PermStatusGlobal = false; }
					this.lError.Text = e.Description;
				});
			}

			if (e.Message == null) {
				this.Dispatcher.Invoke(() => {
					this.lStatusGlobal.Content = "Пока без ошибок";
					this.lStatusGlobal.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA");
					this.lError.Text = null;
					this.PermStatusGlobal = false;
				});
			}
		}

		// -- Вывод статуса подключения к Twitch --
		public void TWHStatus_Set(object sender, Twident_Status e) {
			if (!PermStatusTWH || e.Permanent == false) {

				this.Dispatcher.Invoke(() => {
					switch (e.StatusCode) {
						case 0:
							this.lStatusTWH.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA");
						break;
						case 1:
							this.lStatusTWH.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#33AA66");
						break;
						case 2:
							this.lStatusTWH.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#FDE100");
						break;
						case 3:
							this.lStatusTWH.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DE5A4A");
						break;
					}

					this.lStatusTWH.Content = e.Message;
					if (e.Permanent == true) { this.PermStatusTWH = true; } else { PermStatusTWH = false; }
					//if (e.Description != null) { this.lError.Text = e.Description; }
				});
			}
		}

		// -- Вывод статуса подключения к VKPL --
		public void VKPLStatus_Set(object sender, Twident_Status e) {
			if (!PermStatusVKPL || e.Permanent == false) {

				this.Dispatcher.Invoke(() => {
					switch (e.StatusCode) {
						case 0:
							this.lStatusVKPL.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA");
						break;
						case 1:
							this.lStatusVKPL.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#33AA66");
						break;
						case 2:
							this.lStatusVKPL.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#FDE100");
						break;
						case 3:
							this.lStatusVKPL.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DE5A4A");
						break;
					}

					this.lStatusVKPL.Content = e.Message;
					if (e.Permanent == true) { this.PermStatusVKPL = true; } else { this.PermStatusVKPL = false; }
					//if (e.Description != null) { this.lError.Text = e.Description; }
				});
			}
		}


		// -- Изменение доступности общих кнопок запуска/остановки --
		public void OnOff_Auto() {
			if (TechF.Twitch.Active && !TechF.VKPL.Active) {
				if (TechF.Twitch.Chat.Work) {
					bStart.IsEnabled = false;
					bStop.IsEnabled = true;
				} else {
					bStart.IsEnabled = true;
					bStop.IsEnabled = false;
				}
			} else {
				if (!TechF.Twitch.Active && TechF.VKPL.Active) {
					if (TechF.VKPL.Chat.Work) {
						bStart.IsEnabled = false;
						bStop.IsEnabled = true;
					} else {
						bStart.IsEnabled = true;
						bStop.IsEnabled = false;
					}
				} else {
					if (TechF.Twitch.Active && TechF.VKPL.Active) {
						if (TechF.Twitch.Chat.Work && TechF.VKPL.Chat.Work) {
							bStart.IsEnabled = false;
							bStop.IsEnabled = true;
						} else {
							if (!TechF.Twitch.Chat.Work && !TechF.VKPL.Chat.Work) {
								bStart.IsEnabled = true;
								bStop.IsEnabled = false;
							} else {
								bStart.IsEnabled = true;
								bStop.IsEnabled = true;
							}
						}
					} else {
						bStart.IsEnabled = false;
						bStop.IsEnabled = false;
					}
				}
			}
			// -- Мне лень как-то вставлять в систему выше настройку индивидуальных кнопок, поэтому настроим отдельно
			if (TechF.Twitch.Active && TechF.Twitch.Chat.Work) {
				this.bStartStop_TWH_icon.Source = new BitmapImage(new Uri("\\media/img/icons/stop.ico", UriKind.Relative));
			} else { this.bStartStop_TWH_icon.Source = new BitmapImage(new Uri("\\media/img/icons/play.ico", UriKind.Relative)); }

			if (TechF.VKPL.Active && TechF.VKPL.Chat.Work) {
				this.bStartStop_VKPL_icon.Source = new BitmapImage(new Uri("\\media/img/icons/stop.ico", UriKind.Relative));
			} else { this.bStartStop_VKPL_icon.Source = new BitmapImage(new Uri("\\media/img/icons/play.ico", UriKind.Relative)); }
		}
		private void OnOff_Set(object sender, Twident_Bool e) {
			this.Dispatcher.Invoke(() => { OnOff_Auto(); });
		}
		private void OnOff_Set(bool OnOff) {
			this.Dispatcher.Invoke(() => {
				if (OnOff) {
					bStart.IsEnabled = false;
					bStop.IsEnabled = true;
				} else {
					bStart.IsEnabled = true;
					bStop.IsEnabled = false;
				}
			});
		}



		// -- Приём сообщения --
		private void lChat_Add(object sender, Twident_ChatMsg e) {
			if (!TechF.ChatHistoryListLock) {
				string dtl = DateTimeOffset.FromUnixTimeSeconds(e.UnixTime).LocalDateTime.ToString();

				this.Dispatcher.Invoke(() => {
					switch (e.ServiceType) {
						case 1:
							lChat.Items.Add(new ListWrapC("(" + dtl.Substring(dtl.IndexOf(" ") + 1) + ") " + e.Nick + ": " + e.Msg, new BitmapImage(new Uri("\\media/img/icons/twh.ico", UriKind.Relative))));
						break;
						case 2:
							lChat.Items.Add(new ListWrapC("(" + dtl.Substring(dtl.IndexOf(" ") + 1) + ") " + e.Nick + ": " + e.Msg, new BitmapImage(new Uri("\\media/img/icons/vkpl.ico", UriKind.Relative))));
						break;
						default:
							lChat.Items.Add(new ListWrapC("(" + dtl.Substring(dtl.IndexOf(" ") + 1) + ") " + e.Nick + ": " + e.Msg, null));
						break;
					}
					lChat.ScrollIntoView(lChat.Items[lChat.Items.Count - 1]); // -- Пролистывание чата в самый низ
				}); 
			}
		}

		// -- Авто-запуск бота --
		private void bStart_Click(object sender, RoutedEventArgs e) {
			bStart.IsEnabled = false;
			//bool twh = false;
			//bool vkpl = false;
			if (TechF.Twitch.Active && !TechF.Twitch.Chat.Work && !TechF.Twitch.Chat.Starting) {
				this.bStartStop_TWH_icon.Visibility = Visibility.Collapsed;
				this.bStartStop_TWH.IsEnabled = false;
				this.aStartStop_TWH_load.Visibility = Visibility.Visible;
			}
			if (TechF.VKPL.Active && !TechF.VKPL.Chat.Work && !TechF.VKPL.Chat.Starting) {
				this.bStartStop_VKPL_icon.Visibility = Visibility.Collapsed;
				this.bStartStop_VKPL.IsEnabled = false;
				this.aStartStop_VKPL_load.Visibility = Visibility.Visible;
			}
			Task.Factory.StartNew(() => {
				//if (Ev_GlobalStatus != null) { Ev_GlobalStatus(this, neд Twident_Status(0, "Подключение к чатам", null, false)); }

				if (TechF.Twitch.Active && !TechF.Twitch.Chat.Work && !TechF.Twitch.Chat.Starting) {
					Task.Factory.StartNew(() => {
						TechF.Twitch.Chat.Connect();
						this.Dispatcher.Invoke(() => {
							this.bStartStop_TWH_icon.Source = new BitmapImage(new Uri("\\media/img/icons/stop.ico", UriKind.Relative));
							this.bStartStop_TWH_icon.Visibility = Visibility.Visible;
							this.bStartStop_TWH.IsEnabled = true;
							this.aStartStop_TWH_load.Visibility = Visibility.Hidden;
						});
					});
				}
				if (TechF.VKPL.Active && !TechF.VKPL.Chat.Work && !TechF.VKPL.Chat.Starting) {
					Task.Factory.StartNew(() => {
						TechF.VKPL.Chat.Connect();
						this.Dispatcher.Invoke(() => {
							this.bStartStop_VKPL_icon.Source = new BitmapImage(new Uri("\\media/img/icons/stop.ico", UriKind.Relative));
							this.bStartStop_VKPL_icon.Visibility = Visibility.Visible;
							this.bStartStop_VKPL.IsEnabled = true;
							this.aStartStop_VKPL_load.Visibility= Visibility.Hidden;
						});
					});
				}
			});
		}

		// -- Авто-остановка бота --
		private void bStop_Click(object sender, RoutedEventArgs e) {
			bStop.IsEnabled = false;
			if (TechF.Twitch.Active && !TechF.Twitch.Chat.Work) {
				this.bStartStop_TWH_icon.Visibility = Visibility.Collapsed;
				this.bStartStop_TWH.IsEnabled = false;
				this.aStartStop_TWH_load.Visibility = Visibility.Visible;
			}
			if (TechF.VKPL.Active) {
				this.bStartStop_VKPL_icon.Visibility = Visibility.Collapsed;
				this.bStartStop_VKPL.IsEnabled = false;
				this.aStartStop_VKPL_load.Visibility = Visibility.Visible;
			}
			if (TechF.Twitch.Active && !TechF.Twitch.Chat.Work) {
				Task.Factory.StartNew(() => {
					TechF.Twitch.Chat.Close();
					this.Dispatcher.Invoke(() => {
						this.bStartStop_TWH_icon.Source = new BitmapImage(new Uri("\\media/img/icons/play.ico", UriKind.Relative));
						this.bStartStop_TWH_icon.Visibility = Visibility.Visible;
						this.bStartStop_TWH.IsEnabled = true;
						this.aStartStop_TWH_load.Visibility = Visibility.Hidden;
					});
				});
			}
			if (TechF.VKPL.Active && !TechF.VKPL.Chat.Work) {
				Task.Factory.StartNew(() => {
					TechF.VKPL.Chat.Close();
					this.Dispatcher.Invoke(() => {
						this.bStartStop_VKPL_icon.Source = new BitmapImage(new Uri("\\media/img/icons/play.ico", UriKind.Relative));
						this.bStartStop_VKPL_icon.Visibility = Visibility.Visible;
						this.bStartStop_VKPL.IsEnabled = true;
						this.aStartStop_VKPL_load.Visibility = Visibility.Hidden;
					});
				});
			}
		}


		// -- Запуск-остановка бота для Twitch
		private void bStartStop_TWH_Click(object sender, RoutedEventArgs e) {
			this.bStartStop_TWH_icon.Visibility = Visibility.Collapsed;
			this.bStartStop_TWH.IsEnabled = false;
			this.aStartStop_TWH_load.Visibility = Visibility.Visible;
			if (TechF.Twitch.Chat.Work) {
				Task.Factory.StartNew(() => {
					TechF.Twitch.Chat.Close();
					this.Dispatcher.Invoke(() => {
						this.bStartStop_TWH_icon.Source = new BitmapImage(new Uri("\\media/img/icons/play.ico", UriKind.Relative));
						this.bStartStop_TWH_icon.Visibility = Visibility.Visible;
						this.bStartStop_TWH.IsEnabled = true;
						this.aStartStop_TWH_load.Visibility = Visibility.Hidden;
					});
				});

			} else {
				Task.Factory.StartNew(() => {
					if (TechF.Twitch.Chat.Connect()) {
						this.Dispatcher.Invoke(() => {
							this.bStartStop_TWH_icon.Source = new BitmapImage(new Uri("\\media/img/icons/stop.ico", UriKind.Relative));
							this.bStartStop_TWH_icon.Visibility = Visibility.Visible;
							this.bStartStop_TWH.IsEnabled = true;
							this.aStartStop_TWH_load.Visibility = Visibility.Hidden;
						});
					} else {
						this.Dispatcher.Invoke(() => {
							this.bStartStop_TWH_icon.Source = new BitmapImage(new Uri("\\media/img/icons/play.ico", UriKind.Relative));
							this.bStartStop_TWH_icon.Visibility = Visibility.Visible;
							this.bStartStop_TWH.IsEnabled = true;
							this.aStartStop_TWH_load.Visibility = Visibility.Hidden;
						});
					}
				});
			}
		}


		// -- Запуск-остановка бота VKPL --
		private void bStartStop_VKPL_Click(object sender, RoutedEventArgs e) {
			this.bStartStop_VKPL_icon.Visibility = Visibility.Collapsed;
			this.bStartStop_VKPL.IsEnabled = false;
			this.aStartStop_VKPL_load.Visibility = Visibility.Visible;
			if (TechF.VKPL.Chat.Work) {
				Task.Factory.StartNew(() => {
					TechF.VKPL.Chat.Close();
					this.Dispatcher.Invoke(() => {
						this.bStartStop_VKPL_icon.Source = new BitmapImage(new Uri("\\media/img/icons/play.ico", UriKind.Relative));
						this.bStartStop_VKPL_icon.Visibility = Visibility.Visible;
						this.bStartStop_VKPL.IsEnabled = true;
						this.aStartStop_VKPL_load.Visibility = Visibility.Hidden;
					});
				});

			} else {
				Task.Factory.StartNew(() => {
					if (TechF.VKPL.Chat.Connect()) {
						this.Dispatcher.Invoke(() => {
							this.bStartStop_VKPL_icon.Source = new BitmapImage(new Uri("\\media/img/icons/stop.ico", UriKind.Relative));
							this.bStartStop_VKPL_icon.Visibility = Visibility.Visible;
							this.bStartStop_VKPL.IsEnabled = true;
							this.aStartStop_VKPL_load.Visibility = Visibility.Hidden;
						});
					} else {
						this.Dispatcher.Invoke(() => {
							this.bStartStop_VKPL_icon.Source = new BitmapImage(new Uri("\\media/img/icons/play.ico", UriKind.Relative));
							this.bStartStop_VKPL_icon.Visibility = Visibility.Visible;
							this.bStartStop_VKPL.IsEnabled = true;
							this.aStartStop_VKPL_load.Visibility = Visibility.Hidden;
						});
					}
				});
			}
		}



		// -- Отправка сообщения от лица бота в чат (пока что сразу во все чаты) --
		private void bChatMsgSend(object sender, RoutedEventArgs e) {
			string str = this.eChatMsgSend.Text;
			TechF.TechFuncs.UniversalSendMsg(0, null, str);
			this.eChatMsgSend.Text = "";
		}
		private void bChatMsgSend(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				string str = this.eChatMsgSend.Text;
				TechF.TechFuncs.UniversalSendMsg(0, null, str);
				this.eChatMsgSend.Text = "";
			}
		}

		private void btstr_Click(object sender, RoutedEventArgs e) {
			//TechF.TechFuncs.LogDH("Ответ на запрос: " + TechF.TechFuncs.RTT("users/follows?to_id=107731051&first=100", null));
			//TechF.Twitch.API.ApiRequest("users/follows?from_id=107731051");
			//TechF.Twitch.API.ApiRequest("users?login=jaybeat5");
			//TechF.Twitch.API.ApiRequest("channel_points/custom_rewards?broadcaster_id=43182504");
		}

		private void b1(object sender, RoutedEventArgs e) {
			Task.Factory.StartNew(() => TechF.VKPL.APIBRW.ResolutionSet(true));
		}

		private void b2(object sender, RoutedEventArgs e) {
			Task.Factory.StartNew(() => TechF.VKPL.Chat.MsgMonitor());
		}
	}
}
