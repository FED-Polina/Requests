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
using System.Windows.Shapes;

namespace OOO_Requests.View
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        private List<Statistic> statistics = new List<Statistic>();
        public Statistics()
        {
            InitializeComponent();
        }

        private void buttonNavigation_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var requests = Helper.DB.Request.ToList();
            tbCountDoneRequest.Text = requests.Count(rep => rep.RequestStatus == 3).ToString(); 
            var repairs = Helper.DB.Repair.ToList().FindAll(rep => rep.RepairStep == 2); /*заявки в обработке*/
            tbEverageTime.Text = (repairs.Sum(it => it.RepairTime) / repairs.Count).ToString();

            foreach (var breakType in Helper.DB.TypeOfRequest.ToList()) /*перебор всех типов ошибок*/
            {
                Statistic newStatistic = new Statistic();
                newStatistic.BreakType = breakType.TypeName;
                newStatistic.BreakCount = requests.Count(req => req.TypeOfRequest == breakType);

                statistics.Add(newStatistic); /*добавление в лист*/
            }

            dgStatistics.ItemsSource = statistics; /*заполнение данными грида*/
        }
    }
}
