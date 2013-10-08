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
using BikeWay03.Util;

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
        private void PivotPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string pivotResource = "";
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    pivotResource = "Station";
                    break;

                case 1:
                    pivotResource = "";
                    break;
                default:
                    break;

            }
            ApplicationBar = (ApplicationBar)Resources[pivotResource];

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            

            string navigatedFrom;
            if (NavigationContext.QueryString.TryGetValue("navigatedFrom", out navigatedFrom))
            {
                
                if (String.Equals(navigatedFrom,PivotEnum.pushPinTap.ToString()))
                {
                    string id = "";
                    if (NavigationContext.QueryString.TryGetValue("stationID", out id))
                    {
                        this.Pivot.SelectedIndex = 0;
                        if (id != null)
                        {
                            Debug.WriteLine(id);
                            int id_ = Convert.ToInt32(id) - 1;
                            this.viewModel = new PivotPageViewModel(App.StationListViewModel.StationList[id_]);
                            DataContext = viewModel;
                            //this.viewModel.Station = App.StationListViewModel.StationList[id_];
                        }
                    }
                }

                if (String.Equals(navigatedFrom, PivotEnum.AppBar_Nearby.ToString()))
                {

                    this.viewModel = new PivotPageViewModel(App.StationListViewModel.StationList[50]);
                    DataContext = viewModel;
                    this.Pivot.SelectedIndex = 1;
                    Pivot.Items.RemoveAt(0);
                }

                if (String.Equals(navigatedFrom, PivotEnum.AppBar_Favorites.ToString()))
                {
                   
                    this.viewModel = new PivotPageViewModel(App.StationListViewModel.StationList[50]);
                    DataContext = viewModel;
                    this.Pivot.SelectedIndex = 2;
                    Pivot.Items.RemoveAt(0);
                }

                if (String.Equals(navigatedFrom, PivotEnum.AppBar_Routes.ToString()))
                {

                    this.viewModel = new PivotPageViewModel(App.StationListViewModel.StationList[50]);
                    DataContext = viewModel;
                    this.Pivot.SelectedIndex = 3;
                    Pivot.Items.RemoveAt(0);
                }
            }

            updateBars();
        }


        public void updateBars()
        {
            
            //top nearby station
            double width = this.red_1.RenderSize.Width;
            Debug.WriteLine(width.ToString());
            //this.green_1.Width = width * this.viewModel.nearbyList[0].percentage;

            

        }
    }
}