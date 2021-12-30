using System;
using System.Collections.Generic;
using System.Linq;
//using System.Content;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Twichabot
{

	public partial class Form1 : Window
	{

		//TechFuncs TechFuncs = new TechFuncs();
		BackWin TechF = null;

		public Form1(BackWin backWin) {
			InitializeComponent();

			TechF = backWin;

			this.Head_List.Items.Clear();
			this.ChatMes_List.Items.Clear();

			TechF.Chat.Funcs.Ev_Pyramid += Pyr;

			TechF.Chat.Ev_Error += ErrorOut;
			TechF.Chat.Ev_ChatMsg += ChatMes_List_Add;
			//TechF.Chat.Ev_Status += ChatStatus_Set;
			//TechF.Chat.Ev_Status2 += ChatStatus2_Set;
		}

		private void button3_Click(object sender, RoutedEventArgs e) {
			TechF.Chat.Funcs.Ev_Valid += Label4W;
			string str = this.Edit_AcToken.Text;
			Task.Factory.StartNew(() => TechF.Chat.Funcs.ValidToken(str));
		}
		private void Label4W(object sender, CEvent_Msg e) {
			this.Dispatcher.Invoke(() => { this.label4.Content = e.Message; });
		}


		private void button4_Click(object sender, RoutedEventArgs e) {
			TechF.Chat.Funcs.Ev_RTT += Label4W;
			TechF.Chat.Funcs.Access_Token = this.Edit_AcToken.Text;
			string[] mass = new string[0];
			int i = 0;
			string str = this.Edit_Url.Text;

			foreach (string s in Head_List.Items) {
				Array.Resize(ref mass, mass.Length + 1);
				mass[i] = s;
				i++;
			}

			if ((bool)this.RB_K.IsChecked) {
				Task.Factory.StartNew(() => TechF.Chat.Funcs.RTT("api", str, mass, true));
			} else { Task.Factory.StartNew(() => TechF.Chat.Funcs.RTT("api", str, mass)); }
		}


		private void button6_Click(object sender, RoutedEventArgs e) {
			this.Head_List.Items.Clear();
		}

		private void button5_Click(object sender, RoutedEventArgs e) {
			this.Head_List.Items.Add(this.Edit_Head.Text);
		}



		private void button7_Click(object sender, RoutedEventArgs e) {
			if (Edit_Login.Text != "") {
				TechF.Chat.Login = Edit_Login.Text;
			} else {
				TechF.Chat.Login = null;
			}

			if (Edit_Pass.Text != "") {
				TechF.Chat.Pass = Edit_Pass.Text;
			} else {
				TechF.Chat.Pass = null;
			}

			Task.Factory.StartNew(() => TechF.Chat.Connect());
		}

		private void button9_Click(object sender, RoutedEventArgs e) {
			TechF.Chat.Close();
		}


		private void ChatMes_List_Add(object sender, CEvent_ChatMsg e) {
			//this.Dispatcher.Invoke(() => { ChatMes_List.Items.Add(e.Message); });
			this.Dispatcher.Invoke(() => { ChatMes_List.Items.Add("(" + e.Date + " " + e.Time + ") " + e.Nick + ": " + e.Msg); }); 
		}

		private void button8_Click(object sender, RoutedEventArgs e) {
			string str = this.Edit_Msg.Text;
			//Task.Factory.StartNew(() => TechF.Chat.Send(str));
		}


		private void ErrorOut(object sender, CEvent_Msg e) {
			this.Dispatcher.Invoke(() => { this.label11.Text = e.Message; });
		}

		private void ChatStatus_Set(object sender, CEvent_Msg e) {
			this.Dispatcher.Invoke(() => { this.chatstatus.Content = e.Message; });
		}
		private void ChatStatus2_Set(object sender, CEvent_Msg e) {
			this.Dispatcher.Invoke(() => { this.chatstatus2.Text = e.Message; });
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			Task.Factory.StartNew(() => TechF.Chat.Funcs.Pyramid("Kappa", "4"));
		}

		private void Pyr(object sender, CEvent_Msg e) {
			this.Dispatcher.Invoke(() => { Head_List.Items.Add(e.Message); });
		}
	}
}
