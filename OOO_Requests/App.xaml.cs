using OOO_Requests.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OOO_Requests
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                Helper.DB = new Model.DBRequests(); //связь с бд
                                                         // MessageBox.Show("Связь с бд");
            }
            catch
            {
                MessageBox.Show("Прроблема связи с базой");
                return;
            }
        }
    }
}
