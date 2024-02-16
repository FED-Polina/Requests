using OOO_Requests.Classes;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
using System.Windows.Shapes;

namespace OOO_Requests.View
{
    /// <summary>
    /// Interaction logic for Request.xaml
    /// </summary>
    public partial class Request : Window
    {
        private Model.Request workRequest;
        public Request() //создание
        {
            InitializeComponent();

            gbInformationWork.Visibility = Visibility.Hidden;
            spMaster.Visibility = Visibility.Hidden;

            tbDateRequest.Text = DateTime.Now.ToString();
            butMain.Content = "Зарегистрировать";
            butMain.Click += butMain_ClickAdd;
            cbTypeFault.ItemsSource = Helper.DB.TypeOfRequest.ToList();
            cbTypeFault.SelectedIndex = 0;
            cbClient.ItemsSource = Helper.DB.User.Where(u => u.UserRole == 1).ToList();
            cbClient.SelectedIndex = 0;
            
        }

        public Request(Model.Request request, RequestViewType type)
        {
            InitializeComponent();

            workRequest = request;

            cbTypeFault.ItemsSource = Helper.DB.TypeOfRequest.ToList();

            tbNumberRequest.Text = request.RequestId.ToString();
            tbDateRequest.Text = DateTime.Now.ToString();
            tbEquipment.Text = request.RequestMachine.ToString();
            cbTypeFault.SelectedIndex = request.TypeOfRequest.TypeId - 1;
            tbDescription.Text = request.RequestDescription;
            cbClient.ItemsSource = new List<Model.User>() { request.User };
            cbClient.SelectedIndex = 0;
            cbStatus.ItemsSource = new List<Model.Status>() { request.Status };
            cbStatus.SelectedIndex = 0;
            cbMaster.ItemsSource = Helper.DB.User.Where(u => u.UserRole == 3).ToList();

            if (type is RequestViewType.Work) //назначение мастера
            {
                gbInformationRequest.IsEnabled = false;
                gbInformationWork.Visibility = Visibility.Hidden;
                tbDateRequest.Text = DateTime.Now.ToString();

                cbMaster.SelectedIndex = 0;

                butMain.Content = "В обработку";
                butMain.Click += butMain_ClickWork;
            }
            else if (type is RequestViewType.Edit) //редактирование 
            {
                tbEquipment.IsEnabled = false;
                cbTypeFault.IsEnabled = false;
                cbStatus.IsEnabled = false;
                cbClient.IsEnabled = false;
                gbInformationWork.Visibility = Visibility.Hidden;

                cbMaster.SelectedItem = Helper.DB.Repair.ToList().Find(rep => rep.RepairId == request.RequestId).User; /*кому заявка уже назначена подставляем в комбобокс*/

                butMain.Content = "Обновить данные о заявке";
                butMain.Click += butMain_ClickEdit;
            }
            else // отчет мастера
            {
                gbInformationRequest.IsEnabled = false;
                spMaster.IsEnabled = false;

                cbMaster.SelectedItem = Helper.DB.Repair.ToList().Find(rep => rep.RepairId == request.RequestId).User;
                cbStep.ItemsSource = Helper.DB.Step.ToList();
                cbStep.SelectedIndex = 0;

                butMain.Content = "Фиксировать отчет";
                butMain.Click += butMain_ClickReport;
            }
        }

        private void buttonNavigation_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void butMain_ClickAdd(object sender, RoutedEventArgs e) /*добавление заявки*/
        {
            Model.Request newRequest = new Model.Request();

            newRequest.RequestDateAdd = DateTime.Now;
            newRequest.RequestMachine = tbEquipment.Text;
            newRequest.RequestType = (cbTypeFault.SelectedItem as Model.TypeOfRequest).TypeId;
            newRequest.RequestDescription = tbDescription.Text;
            newRequest.RequestClient = (cbClient.SelectedItem as Model.User).UserId;
            newRequest.RequestStatus = 1;

            Helper.DB.Request.Add(newRequest);

            //сохранение данных в бд
            try
            {
                Helper.DB.SaveChanges();

                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
                throw;
            }
        }

        private void butMain_ClickWork(object sender, RoutedEventArgs e) /*назначение мастера*/
        {
            Model.Repair newRepair = new Model.Repair();
            newRepair.RepairId = workRequest.RequestId;
            newRepair.RepairStep = 1;
            newRepair.RepairMaster = (cbMaster.SelectedItem as Model.User).UserId;
            newRepair.RepairTime = 0;
            newRepair.RepairComment = null;

            Helper.DB.Repair.Add(newRepair);

            try
            {
                Helper.DB.SaveChanges();

                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
                throw;
            }
        }

        private void butMain_ClickEdit(object sender, RoutedEventArgs e)
        {
            workRequest.RequestDescription = tbDescription.Text;
            
            Helper.DB.Request.AddOrUpdate(workRequest);

            var repair = Helper.DB.Repair.Where(rep => rep.RepairId == workRequest.RequestId).First();

            repair.RepairMaster = (cbMaster.SelectedItem as Model.User).UserId;

            Helper.DB.Repair.AddOrUpdate(repair);

            try
            {
                Helper.DB.SaveChanges();

                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
                throw;
            }
        }

        private void butMain_ClickReport(object sender, RoutedEventArgs e)
        {
            Model.Repair repair = Helper.DB.Repair.Where(rep => rep.RepairId == workRequest.RequestId).First();

            var hours = 0;
            var minutes = 0;

            bool parseHours = int.TryParse(tbTimeHours.Text, out hours); 
            bool parseMinutes = int.TryParse(tbTimeMin.Text, out minutes);

            if (parseHours && parseMinutes)
            {
                repair.RepairTime = hours * 60 + minutes;
                repair.RepairComment = tbComment.Text;

                Helper.DB.Repair.AddOrUpdate(repair);

                try
                {
                    Helper.DB.SaveChanges();

                    Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Error");
                    throw;
                }
            }
            else
            {
                if (parseHours)
                {
                    MessageBox.Show("Заполните поле часов");
                }

                if (parseMinutes)
                {
                    MessageBox.Show("Заполните поле минут");
                }
            }
        }
    }

    public enum RequestViewType
    {
        Work,
        Edit,
        Report
    }
}
