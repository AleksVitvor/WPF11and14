using System.Windows;
using Vitvor._7_8WPF;

namespace WpfApp7_8
{
    public partial class MainWindow : Window
    {
        public delegate void WindowLoaded();
        public event WindowLoaded Execute;
        private EnterWindow _enterWindow;
        public MainWindow(User user, EnterWindow enterWindow)
        {
            _enterWindow = enterWindow;
            TaskVM vM = new TaskVM(this, user);
            DataContext = vM;
            InitializeComponent();
            Execute += vM.LoadTasks;
            Execute.Invoke();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _enterWindow.Show();
        }
    }
}
