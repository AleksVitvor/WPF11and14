using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml.Serialization;
using WpfApp7_8;
using Vitvor._7_8WPF; 
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Converters;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;

namespace Vitvor._7_8WPF
{
    public class TaskVM : INotifyPropertyChanged
    {
        private MainWindow _mainWindow;
        private Form form;
        private User _user;
        public ObservableCollection<Task> TODO { get; set; } = new ObservableCollection<Task>();
        public ObservableCollection<Task> TODOSearch { get; set; } = new ObservableCollection<Task>();
        private Task _todo;
        
        public Task SelectedTask
        {
            get
            {
                return _todo;
            }
            set
            {
                _todo = value;
                OnPropertyChanged("SelectedTask");
            }
        }
        private RelayCommand _add;
        public RelayCommand Add
        {
            get
            {
                return _add ??
                    (_add = new RelayCommand(obj =>
                      {
                          Form form = new Form(this);
                          this.form = form;
                          SelectedTask = new Task();
                          form.calendarStart.SelectedDate = SelectedTask.Date;
                          form.calendarEnd.SelectedDate = SelectedTask.Duration;
                          form.ShowDialog();
                      }));
            }
        }
        private RelayCommand _addNew;
        public RelayCommand AddNew
        {
            get
            {
                return _addNew ??
                    (_addNew = new RelayCommand(obj =>
                      {
                          Task task = obj as Task;
                          if (task != null)
                          {
                              task.Category = form.category.SelectedItem.ToString().Remove(0,38);
                              task.Priority = form.priority.SelectedItem.ToString().Remove(0,38);
                              task.Date = (DateTime)form.calendarStart.SelectedDate;
                              task.Duration = (DateTime)form.calendarEnd.SelectedDate;
                              task.AddNewtask(_user);
                              TODO.Add(task);
                              form.Close();

                          }
                      }));
            }
        }
        private RelayCommand _delete;
        public RelayCommand Delete
        {
            get
            {
                return _delete ??
                    (_delete = new RelayCommand(obj =>
                     {
                         Task task = obj as Task;
                         if (task != null)
                         {
                             task.DeleteTask();
                             TODO.Remove(task);
                         }
                     }));
            }
        }
        private RelayCommand _search;
        public RelayCommand Search
        {
            get
            {
                return _search ??
                    (_search = new RelayCommand(obj =>
                      {
                          string part = obj as string;
                          if (part != null)
                          {
                              if (TODOSearch.Count < TODO.Count)
                              {
                                  foreach (var i in TODO)
                                  {
                                      TODOSearch.Add(i);
                                  }
                              }
                              if (part.Equals("Category"))
                              {

                                  if (_mainWindow.Category.SelectedIndex != -1)
                                  {
                                      TODO.Clear();
                                      var search = from i in TODOSearch where _mainWindow.Category.SelectedItem.ToString().Contains(i.Category) select i;
                                      foreach (var i in search)
                                      {
                                          TODO.Add(i);
                                      }
                                  }
                                  else
                                  {
                                      TODOSearch.Clear();
                                  }
                              }
                              else if (part.Equals("Priority"))
                              {
                                  if (_mainWindow.Priority.SelectedIndex != -1)
                                  {
                                      TODO.Clear();
                                      var search = from i in TODOSearch where _mainWindow.Priority.SelectedItem.ToString().Contains(i.Priority) select i;
                                      foreach (var i in search)
                                      {
                                          TODO.Add(i);
                                      }
                                  }
                                  else
                                  {
                                      TODOSearch.Clear();
                                  }
                              }
                              else if (part.Equals("Date"))
                              {
                                  TODO.Clear();
                                  var search = from i in TODOSearch where i.Date == _mainWindow.Date.SelectedDate select i;
                                  foreach (var i in search)
                                  {
                                      TODO.Add(i);
                                  }
                              }
                          }
                      }));
            }
        }
        private RelayCommand _reset;
        public RelayCommand Reset
        {
            get
            {
                return _reset ??
                    (_reset = new RelayCommand(obj =>
                      {
                          TODO.Clear();
                          foreach (var i in TODOSearch)
                          {
                              TODO.Add(i);
                          }
                          TODOSearch.Clear();
                          _mainWindow.Date.SelectedDate = DateTime.Today;
                          _mainWindow.Priority.SelectedIndex = -1;
                          _mainWindow.Category.SelectedIndex = -1;
                      }));
            }
        }
        private RelayCommand _up;
        public RelayCommand Up
        {
            get
            {
                return _up ??
                    (_up = new RelayCommand(obj =>
                      {
                          if (_mainWindow.table.SelectedIndex != -1)
                          {
                              if (_mainWindow.table.SelectedIndex != 0)
                                  _mainWindow.table.SelectedIndex += -1;
                              else
                                  _mainWindow.table.SelectedIndex = _mainWindow.table.Items.Count - 1;
                          }
                      }));
            }
        }
        private RelayCommand _down;
        public RelayCommand Down
        {
            get
            {
                return _down ??
                    (_down = new RelayCommand(obj =>
                      {
                          if (_mainWindow.table.SelectedIndex==-1)
                          {
                              _mainWindow.table.SelectedIndex = 0;
                          }
                          else
                          {
                              if (_mainWindow.table.SelectedIndex != _mainWindow.table.Items.Count - 1)
                                  _mainWindow.table.SelectedIndex += 1;
                              else
                                  _mainWindow.table.SelectedIndex = 0;
                          }
                      }));
            }
        }
        private RelayCommand _changeLanguage;
        public RelayCommand ChangeLanguage
        {
            get
            {
                return _changeLanguage ??
                    (_changeLanguage = new RelayCommand(obj =>
                      {
                        string style1 = obj as string;
                        if (style1!=null)
                          {
                              string style = "Resourses/lang." + style1;
                              // определяем путь к файлу ресурсов
                              var uri = new Uri(style + ".xaml", UriKind.Relative);
                              // загружаем словарь ресурсов
                              ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                              ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                                            where d.Source != null && d.Source.OriginalString.Contains("Resourses/lang.")
                                                            select d).FirstOrDefault();
                              // очищаем коллекцию ресурсов приложения
                              if (oldDict != null)
                              {
                                  int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                                  Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                                  Application.Current.Resources.MergedDictionaries.Insert(ind, resourceDict);
                              }
                              else
                              {
                                  Application.Current.Resources.MergedDictionaries.Add(resourceDict);
                              }
                          }
                      }));
            }

        }
        private RelayCommand _changeTheme;
        public RelayCommand ChangeTheme
        {
            get
            {
                return _changeTheme ??
                    (_changeTheme = new RelayCommand(obj =>
                    {
                        string style1 = "";
                        if (_mainWindow.ChangeTheme.Background.ToString().Contains("4B0082"))
                        {
                            style1 = "Dark";
                        }
                        else
                            style1 = "Light";
                        if (style1 != null)
                        {
                            string style = "Resourses/" + style1+"Theme";
                            // определяем путь к файлу ресурсов
                            var uri = new Uri(style + ".xaml", UriKind.Relative);
                            // загружаем словарь ресурсов
                            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                            ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                                          where d.Source != null && d.Source.OriginalString.Contains("Theme.")
                                                          select d).FirstOrDefault();
                            // очищаем коллекцию ресурсов приложения
                            if (oldDict != null)
                            {
                                int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                                Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                                Application.Current.Resources.MergedDictionaries.Insert(ind, resourceDict);
                            }
                            else
                            {
                                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
                            }
                        }
                    }));
            }
        }
        private RelayCommand _addPicture;
        public RelayCommand AddPicture
        {
            get
            {
                return _addPicture ??
                    (_addPicture = new RelayCommand(obj =>
                      {
                          OpenFileDialog ofdPicture = new OpenFileDialog();
                          ofdPicture.Filter =
                              "Image files|*.bmp;*.jpg;*.gif;*.png;*.tif|All files|*.*";
                          ofdPicture.FilterIndex = 1;

                          if (ofdPicture.ShowDialog() == true)
                              SelectedTask.Image = new BitmapImage(new Uri(ofdPicture.FileName));
                      }));
            }
        }
        public void LoadTasks()
        {
            SqlConnection connection = new SqlConnection(WpfApp7_8.Properties.Settings.Default.ConnectionString);
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                connection = new SqlConnection(WpfApp7_8.Properties.Settings.Default.ReservedConnection);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                using (FileStream fstream = File.OpenRead("Execute.sql"))
                {
                    // преобразуем строку в байты
                    byte[] array = new byte[fstream.Length];
                    // считываем данные
                    fstream.Read(array, 0, array.Length);
                    // декодируем байты в строку
                    string textFromFile = Encoding.Default.GetString(array);
                    command.CommandText = textFromFile;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                string select = $"select id from Users where Login='{_user.Login}' and Password='{_user.Password}'";
                SqlCommand command = new SqlCommand(select, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    string selectTasks = $"select * from TODOList where Userid='{reader.GetInt32(0)}'";
                    reader.Close();
                    SqlCommand selectTask = new SqlCommand(selectTasks, connection);
                    using(SqlDataReader dataReader=selectTask.ExecuteReader())
                    {
                        while(dataReader.Read())
                        {
                            Task task = new Task(dataReader.GetInt32(0));
                            task.Name = dataReader.GetString(2);
                            task.FullDescription = dataReader.GetString(3);
                            task.Date = dataReader.GetDateTime(4);
                            task.Duration = dataReader.GetDateTime(5);
                            task.Priority = dataReader.GetString(6);
                            task.Category = dataReader.GetString(7);
                            task.State = dataReader.GetString(8);
                            task.Image = new BitmapImage(new Uri(dataReader.GetString(9)));
                            task.Periodicity = dataReader.GetString(10);
                            TODO.Add(task);
                        }
                    }
                }
                connection.Close();
            }
        }
        public TaskVM(MainWindow mainWindow, User user)
        {
            _user = user;
            _mainWindow = mainWindow;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
