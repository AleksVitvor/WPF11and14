using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp7_8
{
    class UserVM : INotifyPropertyChanged
    {
        private EnterWindow _enterWindow;
        private User _workUser;
        public User WorkUser
        {
            get
            {
                return _workUser;
            }
            set
            {
                _workUser = value;
                OnPropertyChanged("WorkUser");
            }
        }
        private RelayCommand _passChanged;
        public RelayCommand PassChanged
        {
            get
            {
                return _passChanged ??
                    (_passChanged = new RelayCommand(obj =>
                      {
                          WorkUser.Password = _enterWindow.Password.Password;
                      }));
            }
        }
        private RelayCommand _registration;
        public RelayCommand Registaration
        {
            get
            {
                return _registration ??
                    (_registration = new RelayCommand(obj =>
                      {
                          RegistrationForm registrationForm = new RegistrationForm();
                          registrationForm.ShowDialog();
                      }));
            }
        }
        private void HideAndShow()
        {
            _enterWindow.Hide();
            _enterWindow.Password.Clear();
            MainWindow mainWindow = new MainWindow(WorkUser, _enterWindow);
            mainWindow.Show();
        }
        public UserVM(EnterWindow enterWindow)
        {
            _enterWindow = enterWindow;
            WorkUser = new User();
            WorkUser.Execute += HideAndShow;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}