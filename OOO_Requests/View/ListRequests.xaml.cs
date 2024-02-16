using OOO_Requests.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OOO_Requests.View
{
    /// <summary>
    /// Interaction logic for ListRequests.xaml
    /// </summary>
    public partial class ListRequests : Window
    {
        List<Model.Request> requests = new List<Model.Request>();
        public ListRequests()
        {
            InitializeComponent();
        }

        private void buttonNavigation_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            requests = Helper.DB.Request.ToList();

            cbFilterStatus.ItemsSource = Helper.DB.Status.ToList();
            cbFilterStatus.SelectedIndex = 0;
            cbFilterType.ItemsSource = Helper.DB.TypeOfRequest.ToList();
            cbFilterType.SelectedIndex = 0;

            if (Helper.User.Role.RoleId == 1)
            {
                showRequests(requests.FindAll(req => req.User.UserId == Helper.User.UserId));
                butAddRequest.Visibility = Visibility.Hidden;
                butEditRequest.Visibility = Visibility.Hidden;
                butReport.Visibility = Visibility.Hidden;
                butStatistics.Visibility = Visibility.Hidden;
                butWorkingRequest.Visibility = Visibility.Hidden;
                
            }
            else if (Helper.User.Role.RoleId == 2)
            {
                showRequests(requests);
            }
            else
            {
                showRequests(requests.FindAll(req => req.Repair.RepairMaster == Helper.User.UserId));
                butAddRequest.Visibility = Visibility.Hidden;
                butEditRequest.Visibility = Visibility.Hidden;
                butStatistics.Visibility = Visibility.Hidden;
                butWorkingRequest.Visibility = Visibility.Hidden;
            }

            
        }

        private void showRequests(List<Model.Request> newRequests)
        {
            dgRequests.ItemsSource = null;
            dgRequests.ItemsSource = newRequests; /*добавление в грид всех новых заявок*/
        }

        private void cbFilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showRequests(requests.FindAll(req => req.Status.StatusId == (cbFilterStatus.SelectedItem as Model.Status).StatusId));
        }

        private void cbFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showRequests(requests.FindAll(req => req.TypeOfRequest.TypeId == (cbFilterType.SelectedItem as Model.TypeOfRequest).TypeId));
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var id = -1;
            var isId = int.TryParse(tbSearch.Text, out id);

            if (isId) /*если можем распарсить ищем айди*/
            {
                showRequests(requests.FindAll(req => req.RequestId == id));
            }
            else /*иначе ищем по оборудованию*/
            {
                showRequests(requests.FindAll(req => req.RequestMachine.Contains(tbSearch.Text)));
            }
            
        }

        private void butNullFilter_Click(object sender, RoutedEventArgs e)
        {
            showRequests(requests);
        }

        private void butAddRequest_Click(object sender, RoutedEventArgs e)
        {
            View.Request requestView = new Request();
            this.Hide();
            requestView.ShowDialog();
            this.Show();
        }

        private void butWorkingRequest_Click(object sender, RoutedEventArgs e) /*назначение мастера*/
        {
            if (dgRequests.SelectedItem != null)
            {
                var repair = Helper.DB.Repair.ToList().FindAll(rep => rep.RepairId == (dgRequests.SelectedItem as Model.Request).RequestId); /*подсчет количества отчетов к заявке*/
                if (repair.Count == 0)
                {
                    View.Request requestView = new Request(dgRequests.SelectedItem as Model.Request, RequestViewType.Work);
                    this.Hide();
                    requestView.ShowDialog();
                    this.Show();
                }
                else
                {
                    MessageBox.Show("Заявке уже назначен мастер");
                }
                
            }
            else
            {
                MessageBox.Show("Выберите заявку");
            }
            
        }

        private void butEditRequest_Click(object sender, RoutedEventArgs e)
        {
            if (dgRequests.SelectedItem != null)
            {
                View.Request requestView = new Request(dgRequests.SelectedItem as Model.Request, RequestViewType.Edit);
                this.Hide();
                requestView.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Выберите заявку");
            }
        }

        private void butReport_Click(object sender, RoutedEventArgs e) /*отчет мастера*/
        {
            if (dgRequests.SelectedItem != null)
            {
                if ((dgRequests.SelectedItem as Model.Request).Repair.RepairStep == 1) /*если статус в работе*/
                {
                    View.Request requestView = new Request(dgRequests.SelectedItem as Model.Request, RequestViewType.Report);
                    this.Hide();
                    requestView.ShowDialog();
                    this.Show();
                }
                else
                {
                    MessageBox.Show("Отчет по заявке уже сформирован");
                }
                
            }
            else
            {
                MessageBox.Show("Выберите заявку");
            }
        }

        private void butStatistics_Click(object sender, RoutedEventArgs e)
        {
            View.Statistics statistics = new Statistics();
            this.Hide();
            statistics.ShowDialog();
            this.Show();
        }
    }
}
