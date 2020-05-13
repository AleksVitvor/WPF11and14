using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfApp7_8;

namespace Vitvor._7_8WPF
{
    [Serializable]
    public class Task : INotifyPropertyChanged
    {
        private int _taskId;
        public int TaskId
        {
            get
            {
                return _taskId;
            }
            private set
            {
                _taskId = value;
            }
        }
        private string _fullDescription;
        public string FullDescription
        {
            get
            {
                return _fullDescription;
            }
            set
            {
                _fullDescription = value;
                SqlConnection connection = new SqlConnection(WpfApp7_8.Properties.Settings.Default.ConnectionString);
                try
                {
                    connection.Open();
                }
                finally
                {
                    string update = $"update TODOList set [FullDescription]='{_fullDescription}' where id={TaskId}";
                    SqlCommand command = new SqlCommand(update, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                OnPropertyChanged("FullDescription");
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
                SqlConnection connection = new SqlConnection(WpfApp7_8.Properties.Settings.Default.ConnectionString);
                try
                {
                    connection.Open();
                }
                finally
                {
                    string update = $"update TODOList set [ShortDescription]='{_name}' where id={TaskId}";
                    SqlCommand command = new SqlCommand(update, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                OnPropertyChanged("Name");
            }
        }
        private string _priority;
        public string Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
                SqlConnection connection = new SqlConnection(WpfApp7_8.Properties.Settings.Default.ConnectionString);
                try
                {
                    connection.Open();
                }
                finally
                {
                    string update = $"update TODOList set [Priority]='{_priority}' where id={TaskId}";
                    SqlCommand command = new SqlCommand(update, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                OnPropertyChanged("Priority");
            }
        }
        private string _periodicity;
        public string Periodicity
        {
            get
            {
                return _periodicity;
            }
            set
            {
                _periodicity = value;
                SqlConnection connection = new SqlConnection(WpfApp7_8.Properties.Settings.Default.ConnectionString);
                try
                {
                    connection.Open();
                }
                finally
                {
                    string update = $"update TODOList set [Periodicity]='{_periodicity}' where id={TaskId}";
                    SqlCommand command = new SqlCommand(update, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                OnPropertyChanged("Periodicity");
            }
        }
        private DateTime _duration;
        public DateTime Duration
        {
            get
            {
                return _duration.Date;
            }
            set
            {
                if (value > _startDate)
                {
                    _duration = value;
                    SqlConnection connection = new SqlConnection(WpfApp7_8.Properties.Settings.Default.ConnectionString);
                    try
                    {
                        connection.Open();
                    }
                    finally
                    {
                        string update = $"update TODOList set [Finish]='{_duration}' where id={TaskId}";
                        SqlCommand command = new SqlCommand(update, connection);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    OnPropertyChanged("Duration");
                }
                else
                {
                    MessageBox.Show("Нет возможности установить такую дату окончания");
                }
            }
        }
        private string _category;
        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
                SqlConnection connection = new SqlConnection(WpfApp7_8.Properties.Settings.Default.ConnectionString);
                try
                {
                    connection.Open();
                }
                finally
                {
                    string update = $"update TODOList set [Category]='{_category}' where id={TaskId}";
                    SqlCommand command = new SqlCommand(update, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                OnPropertyChanged("Category");
            }
        }
        private string _state;
        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                SqlConnection connection = new SqlConnection(WpfApp7_8.Properties.Settings.Default.ConnectionString);
                try
                {
                    connection.Open();
                }
                finally
                {
                    string update = $"update TODOList set State='{_state}' where id={TaskId}";
                    SqlCommand command = new SqlCommand(update, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                OnPropertyChanged("State");
            }
        }
        private DateTime _startDate;
        public DateTime Date
        {
            get
            {
                return _startDate.Date;
            }
            set
            {
                _startDate = value;
                SqlConnection connection = new SqlConnection(WpfApp7_8.Properties.Settings.Default.ConnectionString);
                try
                {
                    connection.Open();
                }
                finally
                {
                    string update = $"update TODOList set [Start]='{_startDate}' where id={TaskId}";
                    SqlCommand command = new SqlCommand(update, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                OnPropertyChanged("Date");
            }
        }
        private BitmapImage _image;
        public BitmapImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                OnPropertyChanged("Image");
            }
        }
        public void AddNewtask(User user)
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
                    connection.Close();
                }
            }
            finally
            {
                string select = $"select id from Users where Login='{user.Login}' and Password='{user.Password}'";
                SqlCommand command = new SqlCommand(select, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    string insert = $"insert into TODOList values({reader.GetInt32(0)}, '{Name}', '{FullDescription}', '{Date}', '{Duration}', '{Priority}', '{Category}', '{State}', '{Image}', '{Periodicity}')";
                    reader.Close();
                    SqlCommand insertTask = new SqlCommand(insert, connection);
                    insertTask.ExecuteNonQuery();
                    string searchid = $"select id from TODOList where [Start]='{_startDate}' and State='{_state}' and [Category]='{_category}' and " +
                        $"[Finish]='{_duration}' and [Periodicity]='{_periodicity}' and [Priority]='{_priority}' and [FullDescription]='{_fullDescription}' and [ShortDescription]='{_name}'";
                    SqlCommand sqlCommand = new SqlCommand(searchid, connection);
                    using(SqlDataReader dataReader=sqlCommand.ExecuteReader())
                    {
                        dataReader.Read();
                        TaskId = dataReader.GetInt32(0);
                    }
                }
                connection.Close();
            }
        }
        public void DeleteTask()
        {
            SqlConnection connection = new SqlConnection(WpfApp7_8.Properties.Settings.Default.ConnectionString);
            try
            {
                connection.Open();
            }
            finally
            {
                string delete = $"delete from TODOList where id={TaskId}";
                SqlCommand command = new SqlCommand(delete, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public Task()
        {
            Date = DateTime.Today;
            Duration = DateTime.Today.AddDays(1);
            SqlConnection connection = new SqlConnection(WpfApp7_8.Properties.Settings.Default.ConnectionString);
            try
            {
                connection.Open();
            }
            finally
            {
                string search = $"select id from TODOList";
                SqlCommand command = new SqlCommand(search, connection);
                using(SqlDataReader reader=command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        TaskId = reader.GetInt32(0);
                    }
                    TaskId++;
                }
                connection.Close();
            }
        }
        public Task(int id)
        {
            _taskId = id;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}