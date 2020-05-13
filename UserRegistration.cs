using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp7_8
{
    class UserRegistration:INotifyPropertyChanged
    {
        public delegate void End();
        public event End Execute;
        private string _login;
        public string Login
        {
            get
            {
                return _login;
            }
            set
            {
                _login = value;
                OnPropertyChanged("Login");
            }
        }
        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (value.Length > 7)
                {
                    MD5 mD5 = MD5.Create();
                    _password = Convert.ToBase64String(mD5.ComputeHash(Convert.FromBase64String(value)));
                }
                OnPropertyChanged("Password");
            }
        }
        private string _surname;
        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                _surname = value;
                OnPropertyChanged("Surname");
            }
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        private RelayCommand _register;
        public RelayCommand Register
        {
            get
            {
                return _register ??
                    (_register = new RelayCommand(obj =>
                      {
                          if (Name != null && Surname != null && Login != null && Password != null)
                          {
                            SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
                              try
                              {
                                  connection.Open();
                              }
                              catch (Exception ex)
                              {
                                  connection = new SqlConnection(Properties.Settings.Default.ReservedConnection);
                                  connection.Open();
                                  SqlCommand command = connection.CreateCommand();
                                  using (FileStream fstream = File.OpenRead("Execute.sql"))
                                  {
                                      // преобразуем строку в байты
                                      byte[] array = new byte[fstream.Length];
                                      // считываем данные
                                      fstream.Read(array, 0, array.Length);
                                      // декодируем байты в строку
                                      string textFromFile = System.Text.Encoding.Default.GetString(array);
                                      command.CommandText = textFromFile;
                                      command.ExecuteNonQuery();
                                  }
                              }
                              finally
                              {
                                  string insert = $"insert into Users values('{Login}', '{Password}', '{Surname}','{Name}')";
                                  SqlCommand command = new SqlCommand(insert, connection);
                                  command.ExecuteNonQuery();
                                  Execute.Invoke();
                                  connection.Close();
                              }
                            }
                          else
                          {
                              MessageBox.Show("Проверьте введённые данные(пароль должен быть не короче 8-ми символов)");
                          }
                      }));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
