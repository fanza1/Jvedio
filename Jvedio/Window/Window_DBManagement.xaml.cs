﻿using Jvedio.Plot.Bar;
using Jvedio.ViewModel;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static Jvedio.FileProcess;

namespace Jvedio
{
    /// <summary>
    /// Window_DBManagement.xaml 的交互逻辑
    /// </summary>
    public partial class Window_DBManagement : Jvedio_BaseWindow
    {


        public CancellationTokenSource cts;
        public CancellationToken ct;

        public VieModel_DBManagement vieModel_DBManagement;

        public Window_DBManagement()
        {
            InitializeComponent();

            vieModel_DBManagement = new VieModel_DBManagement();
            vieModel_DBManagement.ListDatabase();
            vieModel_DBManagement.Statistic();
            this.DataContext = vieModel_DBManagement;
            vieModel_DBManagement.CurrentDataBase = Path.GetFileNameWithoutExtension(Properties.Settings.Default.DataBasePath);


            //显示数据
            IDBarView.Datas = vieModel_DBManagement.LoadID();
            IDBarView.Title = Jvedio.Language.Resources.ID;

        }

        public void LoadDataBase(object sender, MouseButtonEventArgs e)
        {

            string name = "";
            Border border = sender as Border;
            Grid grid = border.Parent as Grid;
            Grid grid1 = grid.Parent as Grid;
            TextBlock textBlock = grid1.Children[1] as TextBlock;
            name = textBlock.Text.ToLower();

            Main main = App.Current.Windows[0] as Main;
            main.DatabaseComboBox.SelectedItem = name;

        }

        public void RefreshMain()
        {
            //刷新主界面
            Main main = App.Current.Windows[0] as Main;
            main.vieModel.LoadDataBaseList();
            if (main.vieModel.DataBases.Count > 1)
            {
                main.DatabaseComboBox.Visibility = Visibility.Visible;
                main.DatabaseComboBox.SelectedItem = Path.GetFileNameWithoutExtension(Properties.Settings.Default.DataBasePath);


            }
        }

        public void EditDataBase(object sender, MouseButtonEventArgs e)
        {
            string name = "";
            Border border = sender as Border;
            Grid grid = border.Parent as Grid;
            Grid grid1 = grid.Parent as Grid;
            TextBlock textBlock = grid1.Children[1] as TextBlock;
            name = textBlock.Text.ToLower();
            vieModel_DBManagement.CurrentDataBase = name;


            //ShowEditGrid();
            var brush = new SolidColorBrush(Colors.Red);
            NameBorder.Background = brush;
            Color TargColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Application.Current.Resources["BackgroundMain"].ToString())).Color;
            var ca = new ColorAnimation(TargColor, TimeSpan.FromSeconds(0.75));
            brush.BeginAnimation(SolidColorBrush.ColorProperty, ca);

        }

        public void ShowEditGrid()
        {
            if (SecondRow.Height == new GridLength(0))
            {

                Task.Run(() =>
                {
                    for (int i = 0; i <= 20; i++)
                    {
                        this.Dispatcher.Invoke((Action)delegate { SecondRow.Height = new GridLength(5 * i); });
                        Task.Delay(1).Wait();
                    }
                });
            }
            else
            {
                Task.Run(() =>
                {
                    for (int i = 20; i >= 0; i--)
                    {
                        this.Dispatcher.Invoke((Action)delegate { SecondRow.Height = new GridLength(5 * i); });
                        Task.Delay(1).Wait();
                    }
                });
            }
        }

        public void DelDataBase(object sender, MouseButtonEventArgs e)
        {
            //删除数据库

            string name = "";
            Border border = sender as Border;
            Grid grid = border.Parent as Grid;
            Grid grid1 = grid.Parent as Grid;
            TextBlock textBlock = grid1.Children[1] as TextBlock;
            name = textBlock.Text.ToLower();

            if (name == "info") return;


            if (new Msgbox(this, $"是否确认删除{name}?").ShowDialog() == true)
            {
                string dirpath = DateTime.Now.ToString("yyyyMMddHHss");
                Directory.CreateDirectory($"BackUp\\{dirpath}");
                if (File.Exists($"DataBase\\{name}.sqlite"))
                {
                    //备份
                    File.Copy($"DataBase\\{name}.sqlite", $"BackUp\\{dirpath}\\{name}.sqlite", true);
                    //删除

                    try
                    {
                        File.Delete($"DataBase\\{name}.sqlite");

                        vieModel_DBManagement.DataBases.Remove(name);
                        RefreshMain();
                    }
                    catch
                    {
                        new Msgbox(this, $"数据库【{name}】正在被使用，请切换后重试！").ShowDialog();
                    }


                }



            }

        }




        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }

        private void CartesianChart_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int num = Properties.Settings.Default.Statictistic_ID_Number;
            num += (int)(e.Delta / 120);

            if (num < 5) num = 5;
            else if (num > 50) num = 50;

            Properties.Settings.Default.Statictistic_ID_Number = num;
            Properties.Settings.Default.Save();
        }

        private void CartesianChart_PreviewMouseWheel1(object sender, MouseWheelEventArgs e)
        {
            int num = Properties.Settings.Default.Statictistic_Genre_Number;
            num += (int)(e.Delta / 120);

            if (num < 5) num = 5;
            else if (num > 50) num = 50;

            Properties.Settings.Default.Statictistic_Genre_Number = num;
            Properties.Settings.Default.Save();
        }

        private void CartesianChart_PreviewMouseWheel2(object sender, MouseWheelEventArgs e)
        {
            int num = Properties.Settings.Default.Statictistic_Tag_Number;
            num += (int)(e.Delta / 120);

            if (num < 5) num = 5;
            else if (num > 50) num = 50;

            Properties.Settings.Default.Statictistic_Tag_Number = num;
            Properties.Settings.Default.Save();
        }


        private void CartesianChart_PreviewMouseWheel3(object sender, MouseWheelEventArgs e)
        {
            int num = Properties.Settings.Default.Statictistic_Actor_Number;
            num += (int)(e.Delta / 120);

            if (num < 5) num = 5;
            else if (num > 50) num = 50;

            Properties.Settings.Default.Statictistic_Actor_Number = num;
            Properties.Settings.Default.Save();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogInput dialogInput = new DialogInput(this, "输入数据库名称");
            if (dialogInput.ShowDialog() == true)
            {
                string name = dialogInput.Text.ToLower();


                if (vieModel_DBManagement.DataBases.Contains(name))
                {
                    new Msgbox(this, "已存在").ShowDialog();
                    return;
                }




                MySqlite db = new MySqlite("DataBase\\" + name);
                db.CreateTable(DataBase.SQLITETABLE_MOVIE);
                db.CreateTable(DataBase.SQLITETABLE_ACTRESS);
                db.CreateTable(DataBase.SQLITETABLE_LIBRARY);
                db.CreateTable(DataBase.SQLITETABLE_JAVDB);


                vieModel_DBManagement.DataBases.Add(name);
                //刷新主界面
                RefreshMain();


            }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            OpenFileDialog1.Title = "选择数据库";
            OpenFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            OpenFileDialog1.Filter = "Sqlite 文件|*.sqlite";
            OpenFileDialog1.Multiselect = true;
            OpenFileDialog1.RestoreDirectory = true;
            if (OpenFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] names = OpenFileDialog1.FileNames;

                foreach (var item in names)
                {
                    string name = Path.GetFileNameWithoutExtension(item).ToLower();
                    if (name == "info") continue;

                    if (!DataBase.IsProPerSqlite(item)) continue;

                    if (File.Exists($"DataBase\\{name}.sqlite"))
                    {
                        if (new Msgbox(this, $"已存在 {name}，是否覆盖？").ShowDialog() == true)
                        {
                            File.Copy(item, $"DataBase\\{name}.sqlite", true);

                            if (!vieModel_DBManagement.DataBases.Contains(name)) vieModel_DBManagement.DataBases.Add(name);

                        }
                    }
                    else
                    {
                        File.Copy(item, $"DataBase\\{name}.sqlite", true);
                        if (!vieModel_DBManagement.DataBases.Contains(name)) vieModel_DBManagement.DataBases.Add(name);

                    }

                }



            }
            RefreshMain();
        }

        private void Jvedio_BaseWindow_ContentRendered(object sender, EventArgs e)
        {


            //设置当前数据库
            for (int i = 0; i < vieModel_DBManagement.DataBases.Count; i++)
            {
                if (vieModel_DBManagement.DataBases[i].ToLower() == Path.GetFileNameWithoutExtension(Properties.Settings.Default.DataBasePath).ToLower())
                {
                    DatabaseComboBox.SelectedIndex = i;
                    break;
                }
            }

            if (vieModel_DBManagement.DataBases.Count == 1) DatabaseComboBox.Visibility = Visibility.Hidden;

        }

        private void DatabaseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            if (e.AddedItems[0].ToString().ToLower() != Path.GetFileNameWithoutExtension(Properties.Settings.Default.DataBasePath).ToLower())
            {
                if (e.AddedItems[0].ToString() == "info")
                    Properties.Settings.Default.DataBasePath = AppDomain.CurrentDomain.BaseDirectory + $"{e.AddedItems[0].ToString()}.sqlite";
                else
                    Properties.Settings.Default.DataBasePath = AppDomain.CurrentDomain.BaseDirectory + $"DataBase\\{e.AddedItems[0].ToString()}.sqlite";
                //切换数据库
                vieModel_DBManagement.Statistic();
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {


            cts = new CancellationTokenSource();
            cts.Token.Register(() => { HandyControl.Controls.Growl.Info("取消当前任务！", "DBManageGrowl"); });
            ct = cts.Token;

            //数据库管理
            var cb = CheckBoxWrapPanel.Children.OfType<CheckBox>().ToList();
            string path = "";
            if (vieModel_DBManagement.CurrentDataBase.ToLower() == "info")
                path = $"{vieModel_DBManagement.CurrentDataBase}";
            else
                path = $"DataBase\\{vieModel_DBManagement.CurrentDataBase}";
            MySqlite db = new MySqlite(path);


            if ((bool)cb[1].IsChecked || (bool)cb[2].IsChecked) WaitingPanel.Visibility = Visibility.Visible;

            if ((bool)cb[0].IsChecked)
            {
                //重置信息
                db.DeleteTable("movie");
                db.CreateTable(DataBase.SQLITETABLE_MOVIE);
                HandyControl.Controls.Growl.Success("成功重置信息", "DBManageGrowl");
            }

            if ((bool)cb[1].IsChecked)
            {
                //删除不存在影片
                long num = 0;
                await Task.Run(() =>
                {

                    var movies = db.SelectMoviesBySql("select * from movie");
                    try
                    {
                        movies.ForEach(movie =>
                            {
                        ct.ThrowIfCancellationRequested();
                        if (!File.Exists(movie.filepath))
                        {
                            db.DeleteByField("movie", "id", movie.id);
                            num++;
                        }

                    });
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {ex.Message}");
                        return false;

                    }
                    return true;
                }, ct);

                HandyControl.Controls.Growl.Success($"成功删除 {num} 个不存在的影片", "DBManageGrowl");
            }

            if ((bool)cb[2].IsChecked)
            {
                var movies = db.SelectMoviesBySql("select * from movie");
                StringCollection ScanPath = ReadScanPathFromConfig(vieModel_DBManagement.CurrentDataBase);

                long num = 0;

                await Task.Run(() =>
                {
                    try
                    {
                        movies.ForEach(movie =>
                        {
                            ct.ThrowIfCancellationRequested();
                            if (!IsPathIn(movie.filepath, ScanPath))
                            {
                                db.DeleteByField("movie", "id", movie.id);
                                num++;
                            }
                        });
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {ex.Message}");
                        return false;
                    }
                    
                    return true;
                }, ct);


                HandyControl.Controls.Growl.Success($"成功删除 {num} 个不位于指定目录的影片", "DBManageGrowl");
            }

            db.Vacuum();
            db.CloseDB();
            cts.Dispose();
            WaitingPanel.Visibility = Visibility.Hidden;

            await Task.Run(() => { Task.Delay(500).Wait(); });

            Main main = null;
            Window window = Jvedio.GetWindow.Get("Main");
            if (window != null) main = (Main)window;
            main?.vieModel.Reset();



        }

        public bool IsPathIn(string path, StringCollection paths)
        {
            foreach (var item in paths)
            {
                if (path.IndexOf(item) >= 0) return true;
            }
            return false;
        }



        private void WaitingPanel_Cancel(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            WaitingPanel.Visibility = Visibility.Hidden;
        }

    }

}
