using OOO_Requests.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace OOO_Requests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text;
            string password = passwordBoxPassword.Password;

            StringBuilder sb = new StringBuilder();
            if (String.IsNullOrEmpty(login))
            {
                sb.AppendLine("Вы забыли ввести логин");
            }
            if (String.IsNullOrEmpty(password))
            {
                sb.AppendLine("Вы забыли ввести пароль");
            }
            
            //поиск в бд пользователя с таким логином и паролем
            if (sb.Length == 0)
            {
                List<Model.User> users = new List<Model.User>();
                //получить все данные их таблицы
                users = Helper.DB.User.Where(u => u.UserLogin == login && u.UserPassword == password).ToList();
                Helper.User = users.FirstOrDefault();
                if (Helper.User != null)
                {
                    sb.AppendLine("Здравствуйте " + Helper.User.UserFullName);
                    sb.AppendLine("Вы вошли с ролью " + Helper.User.Role.RoleName);
                    MessageBox.Show(sb.ToString());
                    goListRequests();
                }
                else
                {
                    sb.AppendLine("Такой пользователь не зарегистрирован");
                }

            }
            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString());
             }

            
        }

        private void buttonNavigation_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void goListRequests() //открытие окна
        {
            View.ListRequests listRequests = new View.ListRequests();
            this.Hide();
            listRequests.ShowDialog();
            this.Show();
        }
    }
}
