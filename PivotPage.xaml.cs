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
using BikeWay03.DB;

namespace BikeWay03
{
    public partial class PivotPage : PhoneApplicationPage
    {

        //PivotPageViewModel viewModel;


        public PivotPage()
        {

            InitializeComponent();

            //this.viewModel = new PivotPageViewModel();


            this.DataContext = App.PivotPageViewModel;
            Database.getFavorites();

            try
            {
                //Database.SaveStations(App.StationListViewModel.StationList);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
        }
        private void PivotPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string pivotResource = "";
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    if (App.PivotPageViewModel.Station != null)
                    {
                        if (App.PivotPageViewModel.Station.IsFavorite == true)
                        {
                            pivotResource = "StationIsFavorite";
                        }
                        else
                        {
                            pivotResource = "StationNotFavorite";
                        }
                    }
                    
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

                            App.PivotPageViewModel.Station = App.MainViewModel.StationList[id_];
                            DataContext = App.PivotPageViewModel;
                            //this.viewModel = new PivotPageViewModel(App.StationListViewModel.StationList[id_]);
                            //DataContext = viewModel;
                            //this.viewModel.Station = App.StationListViewModel.StationList[id_];
                        }
                    }
                }

                if (String.Equals(navigatedFrom, PivotEnum.AppBar_Nearby.ToString()))
                {

                    //this.viewModel = new PivotPageViewModel(App.StationListViewModel.StationList[50]);
                    //DataContext = viewModel;
                    this.Pivot.SelectedIndex = 1;
                    Pivot.Items.RemoveAt(0);
                }

                if (String.Equals(navigatedFrom, PivotEnum.AppBar_Favorites.ToString()))
                {
                   
                    //this.viewModel = new PivotPageViewModel(App.StationListViewModel.StationList[50]);
                    //DataContext = viewModel;
                    this.Pivot.SelectedIndex = 2;
                    Pivot.Items.RemoveAt(0);
                }

                if (String.Equals(navigatedFrom, PivotEnum.AppBar_Routes.ToString()))
                {

                    //this.viewModel = new PivotPageViewModel(App.StationListViewModel.StationList[50]);
                    //DataContext = viewModel;
                    this.Pivot.SelectedIndex = 3;
                    Pivot.Items.RemoveAt(0);
                }
            }

            updateBars();
        }

        private void favoriteThisStation(object sender, EventArgs e)
        {
            StationModel station = App.PivotPageViewModel.Station;

            if (station != null)
            {
                if (station.IsFavorite == true)
                {
                    station.IsFavorite = false;
                }
                else
                {
                    station.IsFavorite = true; 
                }
                //   
                Database.UpdateFavoriteStation(station);
            }

            this.Pivot.SelectedIndex = 2;
            
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