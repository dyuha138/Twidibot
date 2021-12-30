﻿using System;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Twidibot.Pages
{
	public partial class FuncCom : Page
	{

		public class TableElement {
			private Pages.FuncCom PageFuncCom = null;
			private int RowCount = 0;
			private BackWin TechF = null;
			public List<int> idList = new List<int>();

			public TableElement(Pages.FuncCom pageFuncCom, BackWin backWin) {
				this.PageFuncCom = pageFuncCom;
				this.TechF = backWin;
			}

			public void Add(DB.FuncCommand_tclass FuncComl) {
				TextBlock tb1 = new TextBlock();
				TextBlock tb2 = new TextBlock();
				TextBlock tb3 = new TextBlock();
				TextBlock tb4 = new TextBlock();
				CheckBox cbl = new CheckBox();
				Image imgl = new Image();
				RowCount++;


				if (RowCount > 1) {
					RowDefinition rd1 = new RowDefinition();
					RowDefinition rd2 = new RowDefinition();
					Canvas c1 = new Canvas();
					Canvas ce = null;

					// -- Настройка горизонтального закрашивания
					c1.Name = "c_" + RowCount;
					c1.SetValue(Grid.ColumnSpanProperty, 15);
					c1.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#FFE37E16");
					c1.SetValue(Grid.RowProperty, PageFuncCom.GridL.RowDefinitions.Count - 1);

					// -- Настройка роудефинейшен
					rd1.Height = new GridLength(2);
					rd2.Height = new GridLength(30);

					// -- Изменение местоположения конечной канвы 
					ce = (Canvas)LogicalTreeHelper.FindLogicalNode(PageFuncCom.GridL, "ce");
					PageFuncCom.GridL.Children.Remove(ce);
					ce.SetValue(Grid.RowProperty, PageFuncCom.GridL.RowDefinitions.Count + 1);


					PageFuncCom.GridL.RowDefinitions.Insert(PageFuncCom.GridL.RowDefinitions.Count - 1, rd1);
					PageFuncCom.GridL.RowDefinitions.Insert(PageFuncCom.GridL.RowDefinitions.Count - 1, rd2);
					PageFuncCom.GridL.Children.Add(c1);
					PageFuncCom.GridL.Children.Add(ce);
				}


				// -- Лейбл для номера команды
				tb1.Name = "lid_" + RowCount;
				tb1.SetValue(Grid.RowProperty, PageFuncCom.GridL.RowDefinitions.Count - 2); // -- Строка
				tb1.FontSize = 12; // -- Размер шрифта
				tb1.HorizontalAlignment = HorizontalAlignment.Center; // -- Выравнивание по горизонтали
				tb1.VerticalAlignment = VerticalAlignment.Center; // -- Выравнивание по вертикали
				tb1.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDE8E8E8"); // -- Цвет
				tb1.Text = RowCount.ToString(); // -- Содержимое
				tb1.SetValue(Grid.ColumnProperty, 1); // -- Столбец

				// -- Лейбл для команды
				tb2.Name = "lc_" + RowCount;
				tb2.SetValue(Grid.RowProperty, PageFuncCom.GridL.RowDefinitions.Count - 2);
				tb2.FontSize = 12;
				tb2.VerticalAlignment = VerticalAlignment.Center;
				tb2.TextWrapping = TextWrapping.Wrap;
				tb2.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDE8E8E8");
				tb2.Text = FuncComl.Command;
				tb2.SetValue(Grid.ColumnProperty, 3);

				// -- Лейбл для описания
				tb3.Name = "ld_" + RowCount;
				tb3.SetValue(Grid.RowProperty, PageFuncCom.GridL.RowDefinitions.Count - 2);
				tb3.FontSize = 12;
				tb3.VerticalAlignment = VerticalAlignment.Center;
				tb3.TextWrapping = TextWrapping.Wrap;
				tb3.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDE8E8E8");
				tb3.Text = FuncComl.Desc;
				tb3.SetValue(Grid.ColumnProperty, 5);

				// -- Лейбл для кулдауна
				tb4.Name = "lcd_" + RowCount;
				tb4.SetValue(Grid.RowProperty, PageFuncCom.GridL.RowDefinitions.Count - 2);
				tb4.FontSize = 12;
				tb4.HorizontalAlignment = HorizontalAlignment.Center;
				tb4.VerticalAlignment = VerticalAlignment.Center;
				tb4.Foreground = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#DDE8E8E8");
				tb4.Text = FuncComl.CoolDown.ToString();
				tb4.SetValue(Grid.ColumnProperty, 7);

				// -- Настройка чекбокса --
				cbl.Name = "cb_" + RowCount;
				cbl.SetValue(Grid.RowProperty, PageFuncCom.GridL.RowDefinitions.Count - 2);
				cbl.SetValue(Grid.ColumnProperty, 9);
				//cbl.Margin = new Thickness(23, 0, 23, 0);
				cbl.HorizontalAlignment = HorizontalAlignment.Center;
				cbl.VerticalAlignment = VerticalAlignment.Center;
				cbl.IsChecked = FuncComl.Enabled;
				cbl.PreviewMouseLeftButtonUp += PageFuncCom.cbStatusEdit;
				cbl.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#FFE37E16");

				// -- Настройка "кнопки" изменения
				imgl.Name = "be_" + RowCount;
				imgl.Width = 25;
				imgl.Height = 25;
				imgl.PreviewMouseLeftButtonUp += PageFuncCom.bEdit;
				imgl.SetValue(Grid.RowProperty, PageFuncCom.GridL.RowDefinitions.Count - 2);
				imgl.SetValue(Grid.ColumnProperty, 11);
				imgl.Source = new BitmapImage(new Uri("\\media/img/bedit.ico", UriKind.Relative));


				PageFuncCom.GridL.Children.Add(tb1);
				PageFuncCom.GridL.Children.Add(tb2);
				PageFuncCom.GridL.Children.Add(tb3);
				PageFuncCom.GridL.Children.Add(tb4);
				PageFuncCom.GridL.Children.Add(cbl);
				PageFuncCom.GridL.Children.Add(imgl);
				idList.Add(FuncComl.id);

				if (RowCount > 8) { PageFuncCom.GridL.Height += 31; }
			}


			public void Clear() {
				for (uint i = 5; i < PageFuncCom.GridL.RowDefinitions.Count - 2; i++) {
					PageFuncCom.GridL.RowDefinitions.RemoveAt(Convert.ToInt32(i));
				}
				RowCount = 0;
			}


			public void Remove(int row) {
				TextBlock tbl = new TextBlock();
				CheckBox cbl = new CheckBox();
				Image imgl = new Image();
				Canvas cl = new Canvas();
				//Canvas ce = new Canvas();
				int rowdel = 0;
				int RowCounttmp = RowCount;

				// -- Удаление выбранного элемента вместе со всеми последующими
				for (uint i = 0; i < RowCounttmp; i++) {
					tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageFuncCom.GridL, "lid_" + (row + i).ToString());
					if (tbl == null) { break; }
					PageFuncCom.GridL.Children.Remove(tbl);
					tbl = null;

					tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageFuncCom.GridL, "lc_" + (row + i).ToString());
					PageFuncCom.GridL.Children.Remove(tbl);
					tbl = null;

					tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageFuncCom.GridL, "ld_" + (row + i).ToString());
					PageFuncCom.GridL.Children.Remove(tbl);
					tbl = null;

					tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageFuncCom.GridL, "lcd_" + (row + i).ToString());
					PageFuncCom.GridL.Children.Remove(tbl);
					tbl = null;

					cbl = (CheckBox)LogicalTreeHelper.FindLogicalNode(PageFuncCom.GridL, "cb_" + (row + i).ToString());
					PageFuncCom.GridL.Children.Remove(cbl);
					cbl = null;

					imgl = (Image)LogicalTreeHelper.FindLogicalNode(PageFuncCom.GridL, "be_" + (row + i).ToString());
					PageFuncCom.GridL.Children.Remove(imgl);
					imgl = null;

					cl = (Canvas)LogicalTreeHelper.FindLogicalNode(PageFuncCom.GridL, "c_" + (row + i).ToString());
					PageFuncCom.GridL.Children.Remove(cl);
					cl = null;

					if (i == 0) { rowdel = 2 + ((row * 2) - 2); }
					RowCount--;

					if (RowCount > 0) {
						PageFuncCom.GridL.RowDefinitions.RemoveAt(rowdel);
						PageFuncCom.GridL.RowDefinitions.RemoveAt(rowdel);
					}
					if (RowCount > 8) { PageFuncCom.GridL.Height -= 31; }
				}

				GC.Collect();
				idList.RemoveAt(row - 1);

				// -- Восстановление всех строк, идущих после удаляемой
				for (uint i = Convert.ToUInt32(RowCount); i < TechF.db.FuncCommandsList.Count; i++) {
					this.Add(TechF.db.FuncCommandsList.ElementAt<DB.FuncCommand_tclass>(Convert.ToInt32(i)));
				}

				/*// -- Изменение местоположения конечной канвы 
				ce = (Canvas)LogicalTreeHelper.FindLogicalNode(PageFuncCom.GridL, "ce");
				PageFuncCom.GridL.Children.Remove(ce);
				ce.SetValue(Grid.RowProperty, PageFuncCom.GridL.RowDefinitions.Count - 1);
				PageFuncCom.GridL.Children.Add(ce);*/
			}


			public void Edit(DB.FuncCommand_tclass FuncComl, int row) {
				TextBlock tbl = new TextBlock();

				tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageFuncCom.GridL, "lc_" + row);
				PageFuncCom.GridL.Children.Remove(tbl);
				tbl.Text = FuncComl.Command;
				PageFuncCom.GridL.Children.Add(tbl);

				tbl = (TextBlock)LogicalTreeHelper.FindLogicalNode(PageFuncCom.GridL, "lcd_" + row);
				PageFuncCom.GridL.Children.Remove(tbl);
				tbl.Text = FuncComl.CoolDown.ToString();
				PageFuncCom.GridL.Children.Add(tbl);
			}
		}





		BackWin TechF = null;
		public TableElement TB = null;
		private string eComPrev = "";
		private string eCDPrev = "";
		private int RowEdit_Row = 0;
		private int RowEdit_id = 0;

		public FuncCom(BackWin backWin) {
			InitializeComponent();
			this.TechF = backWin;
			this.TB = new TableElement(this, backWin);
		}



		// -- Изменение команды
		private void bSave_Click(object sender, RoutedEventArgs e) {
			DB.FuncCommand_tclass FuncComl = new DB.FuncCommand_tclass();
			DB.DefCommand_tclass DefComl = new DB.DefCommand_tclass();
			this.eCom.IsEnabled = false;
			this.eCD.IsEnabled = false;
			this.bSave.IsEnabled = false;
			this.bCan.IsEnabled = false;
			bool aerr = false;

			string com = eCom.Text;
			string cd = eCD.Text;


			Task.Factory.StartNew(() => {
				FuncComl = TechF.db.FuncCommandsList.Find(x => x.Command == com);
				DefComl = TechF.db.DefCommandsList.Find(x => x.Command == com);

				if (com == "" || com == null) {
					aerr = true;
					lerr.Content = "Не введена новая команда для функции!";
				}

				if (cd == "" || cd == null) {
					cd = "0";
				}

				if (!aerr && (FuncComl != null || DefComl != null)) {
					aerr = true;
					lerr.Content = "Такая команда уже существует!";
				}

				if (!aerr) {
					FuncComl.Command = com;
					FuncComl.CoolDown = Convert.ToInt32(cd);
					FuncComl.id = RowEdit_id;

					TechF.db.FuncCommandsT.RowUpdate(FuncComl);
					this.Dispatcher.Invoke(() => TB.Edit(FuncComl, RowEdit_Row));
					RowEdit_Row = 0;
					RowEdit_id = 0;

					this.Dispatcher.Invoke(() => { this.eCom.Visibility = Visibility.Hidden; });
					this.Dispatcher.Invoke(() => { this.eCD.Visibility = Visibility.Hidden; });
					this.Dispatcher.Invoke(() => { this.bSave.Visibility = Visibility.Hidden; });
					this.Dispatcher.Invoke(() => { this.bCan.Visibility = Visibility.Hidden; });
					this.Dispatcher.Invoke(() => { this.lerr.Visibility = Visibility.Hidden; });
					this.Dispatcher.Invoke(() => { this.eCom.Text = ""; });
					this.Dispatcher.Invoke(() => { this.eCD.Text = ""; });
				}

				this.Dispatcher.Invoke(() => { this.eCom.IsEnabled = true; });
				this.Dispatcher.Invoke(() => { this.eCD.IsEnabled = true; });
				this.Dispatcher.Invoke(() => { this.bSave.IsEnabled = true; });
				this.Dispatcher.Invoke(() => { this.bCan.IsEnabled = true; });
			});
		}


		// -- Отмена занесения новой команды
		private void bCan_Click(object sender, RoutedEventArgs e) {
			this.eCom.Visibility = Visibility.Hidden;
			this.eCD.Visibility = Visibility.Hidden;
			this.bSave.Visibility = Visibility.Hidden;
			this.bCan.Visibility = Visibility.Hidden;
			this.lerr.Visibility = Visibility.Hidden;
			this.eCom.Text = "";
			this.eCD.Text = "";
			RowEdit_Row = 0;
			RowEdit_id = 0;
		}




		// -- Ограничения на вводимые знаки --
		// -- Для обычного ввода
		private void eCom_PTI(object sender, TextCompositionEventArgs e) {

		}

		private void eCD_PTI(object sender, TextCompositionEventArgs e) {
			if (!Char.IsDigit(e.Text, 0)) {
				e.Handled = true;
			} else {
				e.Handled = false;
			}
		}

		// -- Для вставки через ctrl+v
		private void eCom_PKD(object sender, KeyEventArgs e) {
			this.eComPrev = eCom.Text;
		}
		private void eCom_Changed(object sender, TextChangedEventArgs e) {

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
				str = eCD.Text.ElementAt<char>(i).ToString();
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
			int rowl = Convert.ToInt32(cbl.Name.Substring(cbl.Name.Length - 1));

			Task.Factory.StartNew(() => {
				int idl = TB.idList.ElementAt<int>(rowl - 1);

				if (!cb) {
					TechF.db.FuncCommandsT.EnabledUpdate(idl, true);
				} else {
					TechF.db.FuncCommandsT.EnabledUpdate(idl, false);
				}
			});
		}


		// -- Почти перенаправляющая функция изменения записи
		public void bEdit(object sender, MouseButtonEventArgs e) {
			Image imgl = sender as Image;
			DB.FuncCommand_tclass FuncComl = null;
			int rowl = Convert.ToInt32(imgl.Name.Substring(imgl.Name.Length - 1));
			this.RowEdit_Row = rowl;
			this.RowEdit_id = TB.idList.ElementAt<int>(rowl - 1);

			FuncComl = TechF.db.FuncCommandsList.Find(x => x.id == TB.idList.ElementAt<int>(rowl - 1));

			this.eCom.Text = FuncComl.Command;
			this.eCD.Text = FuncComl.CoolDown.ToString();

			this.eCom.Visibility = Visibility.Visible;
			this.eCD.Visibility = Visibility.Visible;
			this.bSave.Visibility = Visibility.Visible;
			this.bCan.Visibility = Visibility.Visible;
			this.lerr.Visibility = Visibility.Visible;
		}
	}
}
