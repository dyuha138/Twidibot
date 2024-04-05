using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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

namespace Twidibot.Pages {
	public partial class DefCom : Page {

		public class TableElement {
			private Pages.DefCom PageDefCom = null;
			private int RowCount = 0;
			private BackWin TechF = null;
			public List<int> idList = new List<int>();

			public TableElement(Pages.DefCom pageDefCom, BackWin backWin) {
				this.PageDefCom = pageDefCom;
				this.TechF = backWin;
			}

			public void Add(DB.DefCommand_tclass DefComl) {
				TextBlock tb1 = new TextBlock();
				TextBlock tb2 = new TextBlock();
				TextBlock tb3 = new TextBlock();
				TextBlock tb4 = new TextBlock();
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
					c1.SetValue(Grid.RowProperty, PageDefCom.GridL.RowDefinitions.Count - 1);

					// -- Настройка роудефинейшен
					rd1.Height = new GridLength(2);
					rd2.Height = new GridLength(30);

					// -- Изменение местоположения конечной канвы 
					ce = (Canvas)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "ce");
					PageDefCom.GridL.Children.Remove(ce);
					ce.SetValue(Grid.RowProperty, PageDefCom.GridL.RowDefinitions.Count + 1);


					PageDefCom.GridL.RowDefinitions.Insert(PageDefCom.GridL.RowDefinitions.Count - 1, rd1);
					PageDefCom.GridL.RowDefinitions.Insert(PageDefCom.GridL.RowDefinitions.Count - 1, rd2);
					PageDefCom.GridL.Children.Add(c1);
					PageDefCom.GridL.Children.Add(ce);
				}


				// -- Лейбл для номера команды
				tb1.Name = "lid_" + RowCount;
				tb1.SetValue(Grid.RowProperty, PageDefCom.GridL.RowDefinitions.Count - 2); // -- Строка
				tb1.FontSize = 12; // -- Размер шрифта
				tb1.HorizontalAlignment = HorizontalAlignment.Center; // -- Выравнивание по горизонтали
				tb1.VerticalAlignment = VerticalAlignment.Center; // -- Выравнивание по вертикали
				tb1.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA"); // -- Цвет
				tb1.Text = RowCount.ToString(); // -- Содержимое
				tb1.SetValue(Grid.ColumnProperty, 1); // -- Столбец

				// -- Лейбл для команды
				tb2.Name = "ln_" + RowCount;
				tb2.SetValue(Grid.RowProperty, PageDefCom.GridL.RowDefinitions.Count - 2);
				tb2.FontSize = 12;
				tb2.VerticalAlignment = VerticalAlignment.Center;
				tb2.TextWrapping = TextWrapping.Wrap;
				tb2.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA");
				tb2.Text = DefComl.Command;
				tb2.SetValue(Grid.ColumnProperty, 3);

				// -- Лейбл для ответа
				tb3.Name = "lr_" + RowCount;
				tb3.SetValue(Grid.RowProperty, PageDefCom.GridL.RowDefinitions.Count - 2);
				tb3.FontSize = 12;
				tb3.VerticalAlignment = VerticalAlignment.Center;
				tb3.TextWrapping = TextWrapping.Wrap;
				tb3.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA");
				tb3.Text = DefComl.Result;
				tb3.SetValue(Grid.ColumnProperty, 5);

				// -- Лейбл для кулдауна
				tb4.Name = "lcd_" + RowCount;
				tb4.SetValue(Grid.RowProperty, PageDefCom.GridL.RowDefinitions.Count - 2);
				tb4.FontSize = 12;
				tb4.HorizontalAlignment = HorizontalAlignment.Center;
				tb4.VerticalAlignment = VerticalAlignment.Center;
				tb4.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDDDDA");
				tb4.Text = DefComl.CoolDown.ToString();
				tb4.SetValue(Grid.ColumnProperty, 7);

				// -- Настройка чекбокса --
				cbl.Name = "cb_" + RowCount;
				cbl.SetValue(Grid.RowProperty, PageDefCom.GridL.RowDefinitions.Count - 2);
				cbl.SetValue(Grid.ColumnProperty, 9);
				//cbl.Margin = new Thickness(23, 0, 23, 0);
				cbl.HorizontalAlignment = HorizontalAlignment.Center;
				cbl.VerticalAlignment = VerticalAlignment.Center;
				cbl.IsChecked = DefComl.Enabled;
				cbl.PreviewMouseLeftButtonUp += PageDefCom.cbStatusEdit;
				cbl.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#E5801C");

				// -- Настройка "кнопки" изменения
				img1.Name = "be_" + RowCount;
				img1.Width = 25;
				img1.Height = 25;
				img1.PreviewMouseLeftButtonUp += PageDefCom.bEdit;
				img1.SetValue(Grid.RowProperty, PageDefCom.GridL.RowDefinitions.Count - 2);
				img1.SetValue(Grid.ColumnProperty, 11);
				img1.Source = new BitmapImage(new Uri("\\media/img/icons/bedit.ico", UriKind.Relative));

				// -- Настройка "кнопки" удаления
				img2.Name = "bd_" + RowCount;
				img2.Width = 20;
				img2.Height = 20;
				img2.PreviewMouseLeftButtonUp += PageDefCom.bDel;
				img2.SetValue(Grid.RowProperty, PageDefCom.GridL.RowDefinitions.Count - 2);
				img2.SetValue(Grid.ColumnProperty, 13);
				img2.Source = new BitmapImage(new Uri("\\media/img/icons/bdel.ico", UriKind.Relative));


				PageDefCom.GridL.Children.Add(tb1);
				PageDefCom.GridL.Children.Add(tb2);
				PageDefCom.GridL.Children.Add(tb3);
				PageDefCom.GridL.Children.Add(tb4);
				PageDefCom.GridL.Children.Add(cbl);
				PageDefCom.GridL.Children.Add(img1);
				PageDefCom.GridL.Children.Add(img2);
				idList.Add(DefComl.id);

				if (RowCount > 8) { PageDefCom.GridL.Height += 31; }
			}


			public void Clear() {
				if (PageDefCom.GridL.RowDefinitions.Count > 5) {
					int rowl = PageDefCom.GridL.RowDefinitions.Count - 1;
					this.Remove(1, false, false); // -- Удаление элементов

					for (int i = 3; i < rowl; i++) {
						PageDefCom.GridL.RowDefinitions.RemoveAt(3); // -- Удаление самой строки
					}

					RowDefinition rdl = new RowDefinition(); // -- Восстановление строки, которая разделяет названия столбцов от самих записей
					rdl.Height = GridLength.Auto;
					PageDefCom.GridL.RowDefinitions.Insert(PageDefCom.GridL.RowDefinitions.Count - 1, rdl);
				}
			}


			public void Remove(int row, bool RemoveRowDef = true, bool RestoreNext = true) {
				TextBlock tbl = new TextBlock();
				CheckBox cbl = new CheckBox();
				Image imgl = new Image();
				Canvas cl = new Canvas();
				Canvas ce = new Canvas();
				int rowdel = 0;
				int RowCounttmp = RowCount + 1;

				// -- Удаление выбранного элемента вместе со всеми последующими
				for (int i = row; i < RowCounttmp; i++) {
					tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "lid_" + i.ToString());
					if (tbl == null) { break; }
					PageDefCom.GridL.Children.Remove(tbl);
					tbl = null;

					tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "ln_" + i.ToString());
					PageDefCom.GridL.Children.Remove(tbl);
					tbl = null;

					tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "lr_" + i.ToString());
					PageDefCom.GridL.Children.Remove(tbl);
					tbl = null;

					tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "lcd_" + i.ToString());
					PageDefCom.GridL.Children.Remove(tbl);
					tbl = null;

					cbl = (CheckBox)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "cb_" + i.ToString());
					PageDefCom.GridL.Children.Remove(cbl);
					cbl = null;

					imgl = (Image)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "be_" + i.ToString());
					PageDefCom.GridL.Children.Remove(imgl);
					imgl = null;

					imgl = (Image)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "bd_" + i.ToString());
					PageDefCom.GridL.Children.Remove(imgl);
					imgl = null;

					cl = (Canvas)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "c_" + i.ToString());
					PageDefCom.GridL.Children.Remove(cl);
					cl = null;

					if (i == 0) { rowdel = 2 + ((row * 2) - 2); }
					RowCount--;

					if (RowCount > 0 && RemoveRowDef) {
						PageDefCom.GridL.RowDefinitions.RemoveAt(rowdel);
						PageDefCom.GridL.RowDefinitions.RemoveAt(rowdel);
					}

					if (RowCount > 8) { PageDefCom.GridL.Height -= 31; }
				}

				
				idList.RemoveAt(row - 1);
				if (RestoreNext) { // -- Восстановление всех строк, идущих после удаляемой
					for (int i = RowCount; i < TechF.db.DefCommandsList.Count; i++) {
						this.Add(TechF.db.DefCommandsList.ElementAt(i));
					}
				}

				/*// -- Изменение местоположения конечной канвы 
				ce = (Canvas)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "ce");
				PageDefCom.GridL.Children.Remove(ce);
				ce.SetValue(Grid.RowProperty, PageDefCom.GridL.RowDefinitions.Count - 1);
				PageDefCom.GridL.Children.Add(ce);*/
				Task.Factory.StartNew(() => GC.Collect());
			}


			public void Edit(DB.DefCommand_tclass DefComl, int row) {
				TextBlock tbl = new TextBlock();

				tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "ln_" + row);
				PageDefCom.GridL.Children.Remove(tbl);
				tbl.Text = DefComl.Command;
				PageDefCom.GridL.Children.Add(tbl);

				tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "lr_" + row);
				PageDefCom.GridL.Children.Remove(tbl);
				tbl.Text = DefComl.Result;
				PageDefCom.GridL.Children.Add(tbl);

				tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageDefCom.GridL, "lcd_" + row);
				PageDefCom.GridL.Children.Remove(tbl);
				tbl.Text = DefComl.CoolDown.ToString();
				PageDefCom.GridL.Children.Add(tbl);
			}
		}





		BackWin TechF = null;
		public TableElement TB = null;
		private string eNamePrev = "";
		private string eResPrev = "";
		private string eCDPrev = "";
		private bool RowEdit_Check = false;
		private int RowEdit_Row = 0;
		private int RowEdit_id = 0;

		public DefCom(BackWin backWin) {
			InitializeComponent();
			this.TechF = backWin;
			this.TB = new TableElement(this, backWin);
		}



		// -- Показ полей для добавления новой команды
		private void bAdd_Click(object sender, RoutedEventArgs e) {
			this.eName.Visibility = Visibility.Visible;
			this.eRes.Visibility = Visibility.Visible;
			this.eCD.Visibility = Visibility.Visible;
			this.bSave.Visibility = Visibility.Visible;
			this.bCan.Visibility = Visibility.Visible;
			this.bAdd.Visibility = Visibility.Hidden;
			this.lerr.Visibility = Visibility.Visible;
			this.lName_A.Visibility = Visibility.Visible;
			this.cbAlias.Visibility = Visibility.Visible;
		}


		// -- Сохранение/изменение команды
		private void bSave_Click(object sender, RoutedEventArgs e) {
			DB.DefCommand_tclass DefComl = null;
			DB.FuncCommand_tclass FuncComl = null;
			this.eName.IsEnabled = false;
			this.eRes.IsEnabled = false;
			this.eCD.IsEnabled = false;
			this.bSave.IsEnabled = false;
			this.bCan.IsEnabled = false;
			this.bAdd.IsEnabled = false;
			//this.cbComName_Alias.IsEnabled = false;
			bool aerr = false;

			string name = eName.Text;
			string res = eRes.Text;
			string cd = eCD.Text;
			bool cb = cbAlias.IsChecked.GetValueOrDefault(false);


			Task.Factory.StartNew(() => {
				if (name == "" || name == null) {
					aerr = true;
					this.Dispatcher.Invoke(() => lerr.Content = "Не введена команда");
				}

				if (!aerr && (res == "" || res == null)) {
					aerr = true;
					this.Dispatcher.Invoke(() => lerr.Content = "Не введён ответ на команду");
				}

				if ((name == "" || name == null) && (res == "" || res == null)) {
					aerr = true;
					this.Dispatcher.Invoke(() => lerr.Content = "Не введены ни команда, ни ответ на команду");
				}

				if (cd == "" || cd == null) {
					cd = "0";
				}

				if (!aerr) { // -- Если нет ошибок в первой стадии проверок, то идём дальше
					if (RowEdit_Check) { // -- Изменение команды
						if (cb) {
							DefComl = TechF.db.DefCommandsList.Find(x => x.Command == name);
							FuncComl = TechF.db.FuncCommandsList.Find(x => x.Command == name);
						} else {
							DefComl = TechF.db.DefCommandsList.Find(x => x.Command == "!" + name);
							FuncComl = TechF.db.FuncCommandsList.Find(x => x.Command == "!" + name);
						}

						if ((DefComl != null && DefComl.id != RowEdit_id ) || (FuncComl != null && FuncComl.id != RowEdit_id)) {
							this.Dispatcher.Invoke(() => lerr.Content = "Такая команда уже существует");
						} else {
							if (DefComl == null) { DefComl = new DB.DefCommand_tclass(); }
							DefComl.id = RowEdit_id;
							if (cb) { DefComl.Command = name; } else { DefComl.Command = "!" + name; }
							DefComl.Result = res;
							DefComl.CoolDown = Convert.ToInt32(cd);
							DefComl.isAlias = cb;
							TechF.db.DefCommandsT.RowUpdate(DefComl);
							this.Dispatcher.Invoke(() => TB.Edit(DefComl, RowEdit_Row));
							RowEdit_Check = false;
							RowEdit_Row = 0;
							RowEdit_id = 0;
						}

					} else { // -- Добавление команды
						if (cb) {
							DefComl = TechF.db.DefCommandsList.Find(x => x.Command == name);
							FuncComl = TechF.db.FuncCommandsList.Find(x => x.Command == name);
						} else {
							DefComl = TechF.db.DefCommandsList.Find(x => x.Command == "!" + name);
							FuncComl = TechF.db.FuncCommandsList.Find(x => x.Command == "!" + name);
						}

						if (FuncComl != null || DefComl != null) {
							this.Dispatcher.Invoke(() => lerr.Content = "Такая команда уже существует");
						} else {
							DefComl = new DB.DefCommand_tclass();
							if (cb) { DefComl.Command = name; } else { DefComl.Command = "!" + name; }
							DefComl.Result = res;
							DefComl.CoolDown = Convert.ToInt32(cd);
							DefComl.isAlias = cb;
							DefComl.Enabled = true;
							DefComl.id = TechF.db.DefCommandsT.Add(DefComl); // -- Добавление новой команды в базу
							this.Dispatcher.Invoke(() => TB.Add(DefComl));
						}
					}
					this.Dispatcher.Invoke(() => {
						this.eName.Visibility = Visibility.Hidden;
						this.eRes.Visibility = Visibility.Hidden;
						this.eCD.Visibility = Visibility.Hidden;
						this.bSave.Visibility = Visibility.Hidden;
						this.bCan.Visibility = Visibility.Hidden;
						this.bAdd.Visibility = Visibility.Visible;
						this.lerr.Visibility = Visibility.Hidden;
						this.lName_A.Visibility = Visibility.Hidden;
						this.cbAlias.Visibility = Visibility.Hidden;
						this.eName.Text = "";
						this.eRes.Text = "";
						this.eCD.Text = "";
						this.lerr.Content = "";
					});
				}

				this.Dispatcher.Invoke(() => {
					this.eName.IsEnabled = true;
					this.eRes.IsEnabled = true;
					this.eCD.IsEnabled = true;
					this.bSave.IsEnabled = true;
					this.bCan.IsEnabled = true;
					this.bAdd.IsEnabled = true;
				});
				//this.Dispatcher.Invoke(() => this.cbComName_Alias.IsEnabled = true);
			});
		}


		// -- Отмена занесения новой команды
		private void bCan_Click(object sender, RoutedEventArgs e) {
			this.eName.Visibility = Visibility.Hidden;
			this.eRes.Visibility = Visibility.Hidden;
			this.eCD.Visibility = Visibility.Hidden;
			this.bSave.Visibility = Visibility.Hidden;
			this.bCan.Visibility = Visibility.Hidden;
			this.bAdd.Visibility = Visibility.Visible;
			this.lerr.Visibility = Visibility.Hidden;
			this.lName_A.Visibility = Visibility.Hidden;
			this.cbAlias.Visibility = Visibility.Hidden;
			this.eName.Text = "";
			this.eRes.Text = "";
			this.eCD.Text = "";
			this.lerr.Content = "";
			RowEdit_Check = false;
			RowEdit_Row = 0;
			RowEdit_id = 0;
		}




		// -- Ограничения на вводимые знаки --
		// -- Для обычного ввода
		private void eName_PTI(object sender, TextCompositionEventArgs e) {
			if (e.Text.Contains("!") || e.Text.Contains(" ") || Char.IsWhiteSpace(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}

		private void eRes_PTI(object sender, TextCompositionEventArgs e) {

		}

		private void eCD_PTI(object sender, TextCompositionEventArgs e) {
			if (!Char.IsDigit(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}

		// -- Для вставки через ctrl+v
		private void eName_PKD(object sender, KeyEventArgs e) {
			this.eNamePrev = eName.Text;

			if (e.Key == Key.Space) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}

			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				string str = Clipboard.GetText();
				if (str.Contains("!") || str.Contains(" ") || str.Contains("\t") || str.Contains("\n")) {
					//eName.Text = this.eNamePrev;
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			}
		}
		private void eName_Changed(object sender, TextChangedEventArgs e) {
			if (eName.Text.Contains("!") || eName.Text.Contains(" ") || eName.Text.Contains("\t") || eName.Text.Contains("\n")) {
				eName.Text = this.eNamePrev;
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}

		private void eRes_PKD(object sender, KeyEventArgs e) {
			this.eResPrev = eRes.Text;
		}
		private void eRes_Changed(object sender, TextChangedEventArgs e) {

		}

		private void eCD_PKD(object sender, KeyEventArgs e) {
			this.eCDPrev = eCD.Text;

			if (e.Key == Key.Space) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
			if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) {
				//string str = Clipboard.GetText();
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
		private void cbStatusEdit(object sender, MouseButtonEventArgs e) {
			CheckBox cbl = sender as CheckBox;
			bool cb = cbl.IsChecked.GetValueOrDefault(false);
			int rowl = Convert.ToInt32(cbl.Name.Substring(cbl.Name.LastIndexOf("_") + 1));
			cb = !cb;

			Task.Factory.StartNew(() => {
				int idl = TB.idList.ElementAt(rowl - 1);
				TechF.db.DefCommandsT.EnabledUpdate(idl, cb);
			});
		}


		// -- Почти перенаправляющая функция изменения записи
		public void bEdit(object sender, MouseButtonEventArgs e) {
			//this.bAdd.RaiseEvent(new RoutedEventArgs(this.bAdd.ClickEvent));
			Image imgl = sender as Image;
			DB.DefCommand_tclass DefComl = null;
			int rowl = Convert.ToInt32(imgl.Name.Substring(imgl.Name.LastIndexOf("_") + 1));
			this.RowEdit_Check = true;
			this.RowEdit_Row = rowl;
			this.RowEdit_id = TB.idList.ElementAt(rowl - 1);

			DefComl = TechF.db.DefCommandsList.Find(x => x.id == RowEdit_id);

			if (DefComl.isAlias) {
				this.eName.Text = DefComl.Command;
				this.cbAlias.IsChecked = true;
			} else {
				this.eName.Text = DefComl.Command.Substring(1);
				this.cbAlias.IsChecked = false;
			}

			this.eRes.Text = DefComl.Result;
			this.eCD.Text = DefComl.CoolDown.ToString();

			typeof(System.Windows.Controls.Primitives.ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this.bAdd, new object[0]);
		}


		// -- Почти перенаправляющая функция удаления записи
		public void bDel(object sender, MouseButtonEventArgs e) {
			Image imgl = sender as Image;
			int rowl = Convert.ToInt32(imgl.Name.Substring(imgl.Name.LastIndexOf("_") + 1));
			
			TechF.db.DefCommandsT.Remove(TB.idList.ElementAt(rowl - 1));
			TB.Remove(rowl);
		}

	}
}