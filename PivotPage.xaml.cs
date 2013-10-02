using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BikeWay03.ViewModels;
using System.Diagnostics;

namespace BikeWay03
{
    public partial class PivotPage : PhoneApplicationPage
    {

        PivotPageViewModel viewModel;


        public PivotPage()
        {

            InitializeComponent();

            this.viewModel = new PivotPageViewModel();

            this.DataContext = viewModel;
            
            
                
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string id = "";
            if (NavigationContext.QueryString.TryGetValue("stationID", out id))
            {
                Debug.WriteLine(id);
                int id_ = Convert.ToInt32(id) - 1;
                this.viewModel.Station = App.StationListViewModel.StationList[id_];
            }
        }
    }
}