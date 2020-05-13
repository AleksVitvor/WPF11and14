using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp7_8
{
    public class User : INotifyPropertyChanged
    {
        public delegate void UserHasRows();
        public event UserHasRows Execute;
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
        private RelayCommand _enter;
        public RelayCommand Enter
        {
            get
            {
                return _enter ??
                    (_enter = new RelayCommand(obj =>
                      {
                          SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
                          try
                          {
                              connection.Open();
                          }
                          catch(Exception ex)
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
                              string select = $"select * from Users where Login='{Login}' and Password='{Password}'";
                              SqlCommand command = new SqlCommand(select, connection);
                              //SqlParameter LoginParameter = new SqlParameter("@Login", Login);
                              //SqlParameter PassParameter = new SqlParameter("@Pass", Password);
                              //command.Parameters.Add(LoginParameter);
                              //command.Parameters.Add(PassParameter);
                              using(SqlDataReader reader=command.ExecuteReader())
                              {
                                  if(reader.HasRows)
                                  {
                                      Execute.Invoke();
                                  }
                                  else
                                  {
                                      MessageBox.Show("Пользователь с такими данными не найден, проверьте данные или пройдите регистрацию");
                                  }
                              }
                              connection.Close();
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
