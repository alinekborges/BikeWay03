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
using System.Device.Location;
using System.Collections.ObjectModel;
using Microsoft.Phone.Scheduler;

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

        #region Managing pivots

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
                case 2:
                    pivotResource = "FavoriteAppBar";
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
                            

                            Debug.WriteLine("bikes: " + App.PivotPageViewModel.Station.bikes);
                            Debug.WriteLine("free: " + App.PivotPageViewModel.Station.free);

                            var nearbyList = OrderNearbyStations(App.PivotPageViewModel.Station.GeoCoordinate, App.MainViewModel.StationList);
                            App.PivotPageViewModel.nearbyList = nearbyList;

                            DataContext = App.PivotPageViewModel;
                            //Database.getNearbyStations(App.PivotPageViewModel.Station.GeoCoordinate);
                            //this.viewModel = new PivotPageViewModel(App.StationListViewModel.StationList[id_]);
                            //DataContext = viewModel;
                            //this.viewModel.Station = App.StationListViewModel.StationList[id_];
                        }
                    }
                }

                if (String.Equals(navigatedFrom, PivotEnum.AppBar_Nearby.ToString()))
                {
                    this.Pivot.SelectedIndex = 0;
                    Pivot.Items.RemoveAt(0);

                    string locationStatus;
                    if (NavigationContext.QueryString.TryGetValue("location", out locationStatus))
                    {
                        if (string.Equals(locationStatus, "true"))
                        {
                            string latitude = "";
                            string longitude = "";

                            

                            if (NavigationContext.QueryString.TryGetValue("locationLatitude", out latitude))
                            {
                                
                            }
                            if (NavigationContext.QueryString.TryGetValue("locationLongitude", out longitude))
                            {

                            }

                            if (String.IsNullOrEmpty(latitude) == false && String.IsNullOrEmpty(longitude) == false)
                            {

                            }
                            


                        }
                    }
                    //this.viewModel = new PivotPageViewModel(App.StationListViewModel.StationList[50]);
                    //DataContext = viewModel;
                    
                }

                if (String.Equals(navigatedFrom, PivotEnum.AppBar_Favorites.ToString()))
                {
                   
                    //this.viewModel = new PivotPageViewModel(App.StationListViewModel.StationList[50]);
                    //DataContext = viewModel;
                    this.Pivot.SelectedIndex = 1;
                    Pivot.Items.RemoveAt(0);
                }

                if (String.Equals(navigatedFrom, PivotEnum.AppBar_Routes.ToString()))
                {

                    //this.viewModel = new PivotPageViewModel(App.StationListViewModel.StationList[50]);
                    //DataContext = viewModel;
                    this.Pivot.SelectedIndex = 2;
                    Pivot.Items.RemoveAt(0);
                }
            }

            updateBars();
        }

        #endregion

        public static ObservableCollection<StationModel> OrderNearbyStations(GeoCoordinate coordinate, ObservableCollection<StationModel> stationList)
        {
            //List<StationModel> tempList = new List<StationModel>();

            foreach (StationModel station in stationList)
            {
                //StationModel stationModel = StationModel.getStationModel(station);
                double distance = coordinate.GetDistanceTo(station.GeoCoordinate);
                station.distance = distance;
                //tempList.Add(stationModel);
            }

            var nearbyList = stationList.OrderBy(x => x.distance).ToList();

            ObservableCollection<StationModel> list = new ObservableCollection<StationModel>();

            foreach (StationModel station in nearbyList)
            {
                list.Add(station);
            }

            return list;
        }

        #region Live Tiles
        private void btnIconicTile_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image image = (sender) as Image;
            Debug.WriteLine(image.Tag);

            IconicTileData oIcontile = new IconicTileData();
            

            StationModel station = null;
            int id = Convert.ToInt32(image.Tag.ToString());

            foreach (StationModel s in App.MainViewModel.StationList)
            {
                if (id == s.ID)
                {
                    station = s;
                    break;
                }
            }

            if (station == null)
                return;

            oIcontile.Title = station.Name;
            Uri IconUri = getIconImageUri(station);
            Uri SmallIconUri = getSmallIconImageUri(station);

            oIcontile.IconImage = IconUri;
            oIcontile.SmallIconImage = SmallIconUri;
                       
           
            string tileID = station.ID.ToString();
            string tileName = "BikeWayTile";

            // find the tile object for the application tile that using "Iconic" contains string in it.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(tileID));

            if (TileToFind != null && TileToFind.NavigationUri.ToString().Contains(tileID))
            {
                TileToFind.Delete();
                ShellTile.Create(new Uri("/MainPage.xaml?id="+tileID+"&tileName="+tileName, UriKind.Relative), oIcontile, true);
            }
            else
            {
                ShellTile.Create(new Uri("/MainPage.xaml?id="+tileID+"&tileName="+tileName, UriKind.Relative), oIcontile, true);
            }
            SetUpLiveTileAgent();
        }

        private Uri getIconImageUri(StationModel station)
        {
            int roundedPercentage = station.RoundedPercentage;

            string percentage = roundedPercentage.ToString();

            string size = "_202x202";
            string type = ".png";

            string path = "LiveTilesTemplates/";

            string full_path = path + percentage + size + type;

            return new Uri(full_path, UriKind.RelativeOrAbsolute);
        }

        private Uri getSmallIconImageUri(StationModel station)
        {

            int roundedPercentage = station.RoundedPercentage;
            string percentage = roundedPercentage.ToString();

            string size = "_101x101";
            string type = ".png";

            string path = "LiveTilesTemplates/";

            string full_path = path + percentage + size + type;

            return new Uri(full_path, UriKind.RelativeOrAbsolute);
        }

        void SetUpLiveTileAgent()
        {

            Debug.WriteLine("setting up live tile agent");
            //start background agent 
            PeriodicTask periodicTask = new PeriodicTask("TileUpdate");
            periodicTask.Description = "Updates the live tiles.";
            periodicTask.ExpirationTime = System.DateTime.Now.AddDays(5);//TODO: don't expire, but download new data in the background

            // If the agent is already registered with the system,
            if (ScheduledActionService.Find(periodicTask.Name) != null)
                ScheduledActionService.Remove(periodicTask.Name);
            //not supported in current version
            //periodicTask.BeginTime = DateTime.Now.AddSeconds(10);
            //only can be called when application is running in foreground
            try
            {
                ScheduledActionService.Add(periodicTask);
            }
            catch { }
            if (Debugger.IsAttached)
                try
                {
                    ScheduledActionService.LaunchForTest(periodicTask.Name, TimeSpan.FromSeconds(12));//launch it after 12 seconds - unless you want to wait 30minutes ;)
                    Debug.WriteLine("Background Agent launched..");
                }
                catch { }
        }

        #endregion


        #region menu Performances
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


        #endregion



        public void updateBars()
        {
            
            //top nearby station
            double width = this.red_1.RenderSize.Width;
            Debug.WriteLine(width.ToString());
            //this.green_1.Width = width * this.viewModel.nearbyList[0].percentage;


        }

        private void Image_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }



    }
}