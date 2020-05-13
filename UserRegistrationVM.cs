using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp7_8
{
    class UserRegistrationVM:INotifyPropertyChanged
    {
        private RegistrationForm _registrationForm;
        private UserRegistration _workUser;
        public UserRegistration WorkUser
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
        private RelayCommand _cancel;
        public RelayCommand Cancel
        {
            get
            {
                return _cancel ??
                    (_cancel = new RelayCommand(obj =>
                      {
                          Close();
                      }));
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
                        WorkUser.Password = _registrationForm.Pass.Password;
                     }));
            }
        }
        private void Close()
        {
            _registrationForm.Close();
        }
        public UserRegistrationVM(RegistrationForm registrationForm)
        {
            _registrationForm = registrationForm;
            WorkUser = new UserRegistration();
            WorkUser.Execute += Close;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
