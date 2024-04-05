using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
using System.Xml.Linq;

namespace Twidibot.Pages
{
	public partial class SpamMsg : Page
	{

		public class TableElement {
			private Pages.SpamMsg PageSpamMsg = null;
			private int RowCount = 0;
			private BackWin TechF = null;
			public List<int> idList = new List<int>();

			public TableElement(Pages.SpamMsg pageSpamMsg, BackWin backWin) {
				this.PageSpamMsg = pageSpamMsg;
				this.TechF = backWin;
			}

			public void Add(DB.SpamMessage_tclass SpamMsgl) {
				TextBlock tb1 = new TextBlock();
				TextBlock tb2 = new TextBlock();
				TextBlock tb3 = new TextBlock();
				CheckBox cbl = new CheckBox();
				Image img1 = new Image();
				Image img2 = new Image();
				RowCount++;


				if (RowCount > 1) {
					RowDefinition rd1 = new RowDefinition();
					RowDefinition rd2 = new RowDefinition();
					Canvas c1 = new Canvas();
					Canvas ce = null;

					// -- Настройка горизонтального закрашивания
					c1.Name = "c_" + RowCount;
					c1.SetValue(Grid.ColumnSpanProperty, 15);
					c1.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#E5801C");
					c1.SetValue(Grid.RowProperty, PageSpamMsg.GridL.RowDefinitions.Count - 1);

					// -- Настройка роудефинейшен
					rd1.Height = new GridLength(2);
					rd2.Height = new GridLength(30);

					// -- Изменение местоположения конечной канвы 
					ce = (Canvas)LogicalTreeHelper.FindLogicalNode(PageSpamMsg.GridL, "ce");
					PageSpamMsg.GridL.Children.Remove(ce);
					ce.SetValue(Grid.RowProperty, PageSpamMsg.GridL.RowDefinitions.Count + 1);


					PageSpamMsg.GridL.RowDefinitions.Insert(PageSpamMsg.GridL.RowDefinitions.Count - 1, rd1);
					PageSpamMsg.GridL.RowDefinitions.Insert(PageSpamMsg.GridL.RowDefinitions.Count - 1, rd2);
					PageSpamMsg.GridL.Children.Add(c1);
					PageSpamMsg.GridL.Children.Add(ce);
				}


				// -- Лейбл для номера команды
				tb1.Name = "lid_" + RowCount;
				tb1.SetValue(Grid.RowProperty, PageSpamMsg.GridL.RowDefinitions.Count - 2); // -- Строка
				tb1.FontSize = 12; // -- Размер шрифта
				tb1.HorizontalAlignment = HorizontalAlignment.Center; // -- Выравнивание по горизонтали
				tb1.VerticalAlignment = VerticalAlignment.Center; // -- Выравнивание по вертикали
				tb1.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA"); // -- Цвет
				tb1.Text = RowCount.ToString(); // -- Содержимое
				tb1.SetValue(Grid.ColumnProperty, 1); // -- Столбец

				// -- Лейбл для сообщения
				tb2.Name = "lmsg_" + RowCount;
				tb2.SetValue(Grid.RowProperty, PageSpamMsg.GridL.RowDefinitions.Count - 2);
				tb2.FontSize = 12;
				tb2.VerticalAlignment = VerticalAlignment.Center;
				tb2.TextWrapping = TextWrapping.Wrap;
				tb2.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA"); // -- Цвет
				tb2.Text = SpamMsgl.Message;
				tb2.SetValue(Grid.ColumnProperty, 3);

				// -- Лейбл для кулдауна
				tb3.Name = "lcd_" + RowCount;
				tb3.SetValue(Grid.RowProperty, PageSpamMsg.GridL.RowDefinitions.Count - 2);
				tb3.FontSize = 12;
				tb3.HorizontalAlignment = HorizontalAlignment.Center;
				tb3.VerticalAlignment = VerticalAlignment.Center;
				tb3.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA"); // -- Цвет
				tb3.Text = SpamMsgl.CoolDown.ToString();
				tb3.SetValue(Grid.ColumnProperty, 5);

				// -- Настройка чекбокса --
				cbl.Name = "cb_" + RowCount;
				cbl.SetValue(Grid.RowProperty, PageSpamMsg.GridL.RowDefinitions.Count - 2);
				cbl.SetValue(Grid.ColumnProperty, 7);
				//cbl.Margin = new Thickness(23, 0, 23, 0);
				cbl.HorizontalAlignment = HorizontalAlignment.Center;
				cbl.VerticalAlignment = VerticalAlignment.Center;
				cbl.IsChecked = SpamMsgl.Enabled;
				cbl.PreviewMouseLeftButtonUp += PageSpamMsg.cbStatusEdit;
				cbl.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#E5801C");

				// -- Настройка "кнопки" изменения
				img1.Name = "be_" + RowCount;
				img1.Width = 25;
				img1.Height = 25;
				img1.PreviewMouseLeftButtonUp += PageSpamMsg.bEdit;
				img1.SetValue(Grid.RowProperty, PageSpamMsg.GridL.RowDefinitions.Count - 2);
				img1.SetValue(Grid.ColumnProperty, 9);
				img1.Source = new BitmapImage(new Uri("\\media/img/icons/bedit.ico", UriKind.Relative));

				// -- Настройка "кнопки" удаления
				img2.Name = "bd_" + RowCount;
				img2.Width = 20;
				img2.Height = 20;
				img2.PreviewMouseLeftButtonUp += PageSpamMsg.bDel;
				img2.SetValue(Grid.RowProperty, PageSpamMsg.GridL.RowDefinitions.Count - 2);
				img2.SetValue(Grid.ColumnProperty, 11);
				img2.Source = new BitmapImage(new Uri("\\media/img/icons/bdel.ico", UriKind.Relative));


				PageSpamMsg.GridL.Children.Add(tb1);
				PageSpamMsg.GridL.Children.Add(tb2);
				PageSpamMsg.GridL.Children.Add(tb3);
				PageSpamMsg.GridL.Children.Add(cbl);
				PageSpamMsg.GridL.Children.Add(img1);
				PageSpamMsg.GridL.Children.Add(img2);
				idList.Add(SpamMsgl.id);

				if (RowCount > 8) { PageSpamMsg.GridL.Height += 31; }
			}


			public void Clear() {
				if (PageSpamMsg.GridL.RowDefinitions.Count > 5) {
					int rowl = PageSpamMsg.GridL.RowDefinitions.Count - 1;
					this.Remove(1, false, false); // -- Удаление элементов

					for (int i = 3; i < rowl; i++) {
						PageSpamMsg.GridL.RowDefinitions.RemoveAt(3); // -- Удаление самой строки
					}

					RowDefinition rdl = new RowDefinition(); // -- Восстановление строки, которая разделяет названия столбцов от самих записей
					rdl.Height = GridLength.Auto;
					PageSpamMsg.GridL.RowDefinitions.Insert(PageSpamMsg.GridL.RowDefinitions.Count - 1, rdl);
				}
			}


			public void Remove(int row, bool RemoveRowDef = true, bool RestoreNext = true) {
				TextBlock tbl = new TextBlock();
				CheckBox cbl = new CheckBox();
				Image imgl = new Image();
				Canvas cl = new Canvas();
				Canvas ce = new Canvas();
				int rowdel = 0;
				int RowCounttmp = RowCount;

				// -- Удаление выбранного элемента вместе со всеми последующими
				for (int i = row; i < RowCounttmp; i++) {
					tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageSpamMsg.GridL, "lid_" + i.ToString());
					if (tbl == null) { break; }
					PageSpamMsg.GridL.Children.Remove(tbl);
					tbl = null;

					tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageSpamMsg.GridL, "lmsg_" + i.ToString());
					PageSpamMsg.GridL.Children.Remove(tbl);
					tbl = null;

					tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageSpamMsg.GridL, "lcd_" + i.ToString());
					PageSpamMsg.GridL.Children.Remove(tbl);
					tbl = null;

					cbl = (CheckBox)LogicalTreeHelper.FindLogicalNode(PageSpamMsg.GridL, "cb_" + i.ToString());
					PageSpamMsg.GridL.Children.Remove(cbl);
					cbl = null;

					imgl = (Image)LogicalTreeHelper.FindLogicalNode(PageSpamMsg.GridL, "be_" + i.ToString());
					PageSpamMsg.GridL.Children.Remove(imgl);
					imgl = null;

					imgl = (Image)LogicalTreeHelper.FindLogicalNode(PageSpamMsg.GridL, "bd_" + i.ToString());
					PageSpamMsg.GridL.Children.Remove(imgl);
					imgl = null;

					cl = (Canvas)LogicalTreeHelper.FindLogicalNode(PageSpamMsg.GridL, "c_" + i.ToString());
					PageSpamMsg.GridL.Children.Remove(cl);
					cl = null;

					if (i == 0) { rowdel = 2 + ((row * 2) - 2); }
					RowCount--;

					if (RowCount > 0 && RemoveRowDef) {
						PageSpamMsg.GridL.RowDefinitions.RemoveAt(rowdel);
						PageSpamMsg.GridL.RowDefinitions.RemoveAt(rowdel);
					}
					if (RowCount > 8) { PageSpamMsg.GridL.Height -= 31; }
				}

				idList.RemoveAt(row - 1);

				if (RestoreNext) { // -- Восстановление всех строк, идущих после удаляемой
					for (uint i = Convert.ToUInt32(RowCount); i < TechF.db.SpamMessagesList.Count; i++) {
						this.Add(TechF.db.SpamMessagesList.ElementAt(Convert.ToInt32(i)));
					}
				}

				/*// -- Изменение местоположения конечной канвы 
				ce = (Canvas)LogicalTreeHelper.FindLogicalNode(PageSpamMsg.GridL, "ce");
				PageSpamMsg.GridL.Children.Remove(ce);
				ce.SetValue(Grid.RowProperty, PageSpamMsg.GridL.RowDefinitions.Count - 1);
				PageSpamMsg.GridL.Children.Add(ce);*/
				Task.Factory.StartNew(() => GC.Collect());
			}


			public void Edit(DB.SpamMessage_tclass SpamMsgl, int row) {
				TextBlock tbl = new TextBlock();

				tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageSpamMsg.GridL, "lmsg_" + row);
				PageSpamMsg.GridL.Children.Remove(tbl);
				tbl.Text = SpamMsgl.Message;
				PageSpamMsg.GridL.Children.Add(tbl);

				tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageSpamMsg.GridL, "lcd_" + row);
				PageSpamMsg.GridL.Children.Remove(tbl);
				tbl.Text = SpamMsgl.CoolDown.ToString();
				PageSpamMsg.GridL.Children.Add(tbl);
			}
		}





		BackWin TechF = null;
		public TableElement TB = null;
		//private string eMsgPrev = "";
		private string eCDPrev = "";
		private bool RowEdit_Check = false;
		private int RowEdit_Row = 0;
		private int RowEdit_id = 0;

		public SpamMsg(BackWin backWin) {
			InitializeComponent();
			this.TechF = backWin;
			this.TB = new TableElement(this, backWin);
		}



		// -- Показ полей для добавления новой команды
		private void bAdd_Click(object sender, RoutedEventArgs e) {
			this.eMsg.Visibility = Visibility.Visible;
			this.eCD.Visibility = Visibility.Visible;
			this.bSave.Visibility = Visibility.Visible;
			this.bCan.Visibility = Visibility.Visible;
			this.bAdd.Visibility = Visibility.Hidden;
			this.lerr.Visibility = Visibility.Visible;
		}


		// -- Сохранение/изменение команды
		private void bSave_Click(object sender, RoutedEventArgs e) {
			DB.SpamMessage_tclass SpamMsgl = null;
			this.eMsg.IsEnabled = false;
			this.eCD.IsEnabled = false;
			this.bSave.IsEnabled = false;
			this.bCan.IsEnabled = false;
			this.bAdd.IsEnabled = false;
			bool aerr = false;

			string msg = eMsg.Text;
			string cd = eCD.Text;

			Task.Factory.StartNew(() => {
				if (msg == "" || msg == null) {
					aerr = true;
					this.Dispatcher.Invoke(() => lerr.Content = "Не введено сообщение");
				}

				if (cd == "" || cd == null) {
					cd = "0";
				}

				if (!aerr) {
					if (RowEdit_Check) {
						SpamMsgl = TechF.db.SpamMessagesList.Find(x => x.Message == msg);

						if (SpamMsgl != null && SpamMsgl.id != RowEdit_id) {
							this.Dispatcher.Invoke(() => lerr.Content = "Такое сообщение уже существует");
						} else {
							SpamMsgl.Message = msg;
							SpamMsgl.CoolDown = Convert.ToInt32(cd);
							TechF.db.SpamMessagesT.RowUpdate(SpamMsgl);
							this.Dispatcher.Invoke(() => TB.Edit(SpamMsgl, RowEdit_Row));
							RowEdit_Check = false;
							RowEdit_Row = 0;
							RowEdit_id = 0;
						}

					} else {
						SpamMsgl = TechF.db.SpamMessagesList.Find(x => x.Message == msg);

						if (SpamMsgl != null) {
							this.Dispatcher.Invoke(() => lerr.Content = "Такое переодическое сообщение уже существует");
						} else {
							SpamMsgl = new DB.SpamMessage_tclass();
							SpamMsgl.id = RowEdit_id;
							SpamMsgl.Message = msg;
							SpamMsgl.CoolDown = Convert.ToInt32(cd);
							SpamMsgl.id = TechF.db.SpamMessagesT.Add(SpamMsgl); // -- Добавление новой команды в базу
							this.Dispatcher.Invoke(() => TB.Add(SpamMsgl));

						}
					}
					this.Dispatcher.Invoke(() => {
						this.eMsg.Visibility = Visibility.Hidden;
						this.eCD.Visibility = Visibility.Hidden;
						this.bSave.Visibility = Visibility.Hidden;
						this.bCan.Visibility = Visibility.Hidden;
						this.bAdd.Visibility = Visibility.Visible;
						this.lerr.Visibility = Visibility.Hidden;
						this.eMsg.Text = "";
						this.eCD.Text = "";
						this.lerr.Content = "";
					});
				}

				this.Dispatcher.Invoke(() => {
					this.eMsg.IsEnabled = true;
					this.eCD.IsEnabled = true;
					this.bSave.IsEnabled = true;
					this.bCan.IsEnabled = true;
					this.bAdd.IsEnabled = true;
				});
			});
		}


		// -- Отмена занесения новой команды
		private void bCan_Click(object sender, RoutedEventArgs e) {
			this.eMsg.Visibility = Visibility.Hidden;
			this.eCD.Visibility = Visibility.Hidden;
			this.bSave.Visibility = Visibility.Hidden;
			this.bCan.Visibility = Visibility.Hidden;
			this.bAdd.Visibility = Visibility.Visible;
			this.lerr.Visibility = Visibility.Hidden;
			this.eMsg.Text = "";
			this.eCD.Text = "";
			RowEdit_Check = false;
			RowEdit_Row = 0;
			RowEdit_id = 0;
		}




		// -- Ограничения на вводимые знаки --
		// -- Для обычного ввода
		private void eMsg_PTI(object sender, TextCompositionEventArgs e) {

		}

		private void eCD_PTI(object sender, TextCompositionEventArgs e) {
			if (!Char.IsDigit(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}

		// -- Для вставки через ctrl+v
		private void eMsg_PKD(object sender, KeyEventArgs e) {
			//this.eMsgPrev = eMsg.Text;
		}
		private void eMsg_Changed(object sender, TextChangedEventArgs e) {

		}

		private void eCD_PKD(object sender, KeyEventArgs e) {
			this.eCDPrev = eCD.Text;

			if (e.Key == Key.Space) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (!Char.IsDigit(eCD.Text, 0)) {
					eCD.Text = this.eCDPrev;
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eCD_Changed(object sender, TextChangedEventArgs e) {
			string str = null;
			for (ushort i = 0; i < eCD.Text.Length; i++) {
				str = eCD.Text.ElementAt(i).ToString();
				if (!Char.IsDigit(eCD.Text, 0)) {
					eCD.Text = this.eCDPrev;
					e.Handled = true;
					break;
				} else {
					e.Handled = false;
				}
			}
		}


		// -- Изменение состояния команды
		public void cbStatusEdit(object sender, MouseButtonEventArgs e) {
			CheckBox cbl = sender as CheckBox;
			bool cb = cbl.IsChecked.GetValueOrDefault(false);
			int rowl = Convert.ToInt32(cbl.Name.Substring(cbl.Name.LastIndexOf("_") + 1));
			cb = !cb;

			Task.Factory.StartNew(() => {
				int idl = TB.idList.ElementAt(rowl - 1);

				if (!cb) {
					TechF.db.SpamMessagesT.EnabledUpdate(idl, true);
				} else {
					TechF.db.SpamMessagesT.EnabledUpdate(idl, false);
				}
			});
		}


		// -- Почти перенаправляющая функция изменения записи
		public void bEdit(object sender, MouseButtonEventArgs e) {
			//this.bAdd.RaiseEvent(new RoutedEventArgs(this.bAdd.ClickEvent));
			Image imgl = sender as Image;
			DB.SpamMessage_tclass SpamMsgl = null;
			int rowl = Convert.ToInt32(imgl.Name.Substring(imgl.Name.LastIndexOf("_") + 1));
			this.RowEdit_Check = true;
			this.RowEdit_Row = rowl;
			this.RowEdit_id = TB.idList.ElementAt<int>(rowl - 1);

			SpamMsgl = TechF.db.SpamMessagesList.Find(x => x.id == TB.idList.ElementAt(rowl - 1));

			this.eMsg.Text = SpamMsgl.Message;
			this.eCD.Text = SpamMsgl.CoolDown.ToString();

			typeof(System.Windows.Controls.Primitives.ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this.bAdd, new object[0]);
		}


		// -- Почти перенаправляющая функция удаления записи
		public void bDel(object sender, MouseButtonEventArgs e) {
			Image imgl = sender as Image;
			int rowl = Convert.ToInt32(imgl.Name.Substring(imgl.Name.LastIndexOf("_") + 1));

			TechF.db.SpamMessagesT.Remove(rowl);
			TB.Remove(rowl);
		}
	}
}
