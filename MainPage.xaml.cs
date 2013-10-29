using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BikeWay03.Resources;
using BikeWay03.ViewModels;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Shapes;
using System.Windows.Media;
using BikeWay03.Util;
using BikeWay03.DB;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace BikeWay03
{


    public partial class MainPage : PhoneApplicationPage
    {

        #region variable definitions

        MapLayer _pushpinsStationsLayer;
        MapLayer _pushpinsNetworksLayer;
        Geolocator geolocatorWP8;
        public GeoCoordinate myLocation;
        public int lastZoomLevel;
        public GeoCoordinate mapCenter;
        LocationRectangle locationRectangle;     
        public static Map Map;
        public StatusChangedEventArgs positionStatus;
        public TextBlock statusTextBlock;
        private string _status;
        private bool fullyloaded;
        private double zoomLevelDetail = 17;
        public ProgressBar LoadingBar;
        private bool _isLoadingSomething;

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (!string.Equals(_status,value))
                {
                    _status = value;
                    statusTextBlock.Text = value;
                }
                
            }
        }

        public bool IsLoadingSomething
        {
            get
            {
                return _isLoadingSomething;
            }
            set
            {
                if (_isLoadingSomething != value)
                {
                    _isLoadingSomething = value;
                    if (value == true)
                    {
                        LoadingBar.Visibility = Visibility.Visible;

                    }
                    else
                    {
                        LoadingBar.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        #endregion


        // Constructor
        public MainPage()
        {
            InitializeComponent();

            fullyloaded = false;

            App.MainPage = this;
            // Set the data context of the LongListSelector control to the sample data
            DataContext = App.MainViewModel;
            statusTextBlock = this.StatusTextBlock;
            LoadingBar = this.progressBar;

            Database.initializeDatabase();

            this.Loaded += this.OnPageLoaded;
            Map = this.MapControl;

            //Settings.loadAllSettings();
                        


            
            // Sample code to localize the ApplicationBarupda
            //BuildLocalizedApplicationBar();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Settings.loadAllSettings();
            App.MainPage = this;

            if (Settings.FirstTimeLaunch == true)
            {
                App.MainViewModel.LoadNetworks();
                FirstTimeLaunch();
                return;
            }

            handleNavigationParameters(NavigationContext);

            if (App.MainViewModel.isNetworkLoaded == false)
            {
                App.MainViewModel.LoadData();
            }

            if (App.MainViewModel.isStationsLoaded == false)
            {
                
            }

            if (!App.MainViewModel.IsDataLoaded)
            {
                App.MainPage = this;
                //App.MainViewModel.LoadDataFromAPI();

                
                //UpdatePushpinsStationsOnMap(App.MainViewModel.StationList, locationRectangle);



                UpdatePushpinsStationsOnMap(App.MainViewModel.StationList);


                
                //App.MainPage.UpdatePushpinsOnTheMap(App.StationListViewModel.StationList);
            }

            if (App.isLaunching)
            {

                this.UserLocationMarker = (UserLocationMarker)this.FindName("UserLocationMarker");

                if (Settings.UserLocation == true)
                {
                    GetContinuousLocationUsingWP8Api();
                }

                if (Settings.LastUserLocation != null)
                {
                    Map.SetView(Settings.LastUserLocation, zoomLevelDetail);
                }
            }

        }

        #region extra

        public void handleNavigationParameters(NavigationContext NavigationContext)
        {
            string navigatedFrom;
            if (NavigationContext.QueryString.TryGetValue("navigatedFrom", out navigatedFrom))
            {
                Debug.WriteLine("I came from " + navigatedFrom);
                if (string.Equals(navigatedFrom, "PivotPage"))
                {
   
                        string lat, lng;
                        NavigationContext.QueryString.TryGetValue("latitude", out lat);
                        NavigationContext.QueryString.TryGetValue("longitude", out lng);
                        double latitude = Convert.ToDouble(lat);
                        double longitude = Convert.ToDouble(lng);
                        GeoCoordinate stationCoordinate = new GeoCoordinate(latitude, longitude);
                        Debug.WriteLine("Geocoordinate of station = " + stationCoordinate.ToString());
                        Map.SetView(stationCoordinate, zoomLevelDetail);

                }
            }
        }

        public static LocationRectangle Bounds(Map map)
        {
            
            GeoCoordinate Point1 = map.ConvertViewportPointToGeoCoordinate(new Point(0,0));
            GeoCoordinate Point2 = map.ConvertViewportPointToGeoCoordinate(new Point(map.ActualHeight - 10, map.ActualWidth - 10));
            Point Point3 = map.ConvertGeoCoordinateToViewportPoint(map.Center);
            //Point point = map.Center;

            if ((Point1 == null) || (Point2 == null))
            {
                Debug.WriteLine("WARNING - MAP conversion of Viewport to GeoCoordinate point failed - falling back to backup value.");
                return new LocationRectangle(map.Center, 0.01, 0.01); // sane value
            }

            return new LocationRectangle(Point1, Point2);

            
        }


        public LocationRectangle getLocationRectangle()
        { 
            //LocationRectangle locationRectangle = null;
            if (MapControl.ZoomLevel < 3)
                return null;

            GeoCoordinate topLeft = Map.ConvertViewportPointToGeoCoordinate(new Point(0, 0));
            GeoCoordinate bottomRight =
            Map.ConvertViewportPointToGeoCoordinate(new Point(Map.ActualWidth, Map.ActualHeight));
            
            LocationRectangle lRectangle = LocationRectangle.CreateBoundingRectangle(new[] { topLeft, bottomRight });

            return lRectangle;
            
            //var topLeft = MapControl.ConvertViewportPointToGeoCoordinate(new Point(5.0, 5.0));
            //var bottonRight = MapControl.ConvertViewportPointToGeoCoordinate(new Point(MapControl.ActualWidth - 10, MapControl.ActualHeight - 10));

            //locationRectangle = new LocationRectangle(topLeft, bottonRight);

            //return locationRectangle;
        }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarButtonText;
            ApplicationBar.Buttons.Add(appBarButton);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        #endregion

        #region FirstTimeLaunch

        public void FirstTimeLaunch()
        {
            MessageBoxResult result =
                MessageBox.Show("Can this app use your location to find the right network and show nearby stations?",
                "Location", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                Status = "Trying to find you location...";
                GetContinuousLocationUsingWP8Api();
                Settings.UserLocation = true;

            }
            else
            {
                Settings.UserLocation = false;
                App.MainViewModel.LoadNetworks();
                Status = "";
                IsLoadingSomething = false;                
                Map.ZoomLevel = 4;
            }

        }

        public void FlyToMyLocationFirstTime(GeoCoordinate coordinate)
        {
            Debug.WriteLine("Flying to my location for the first time");
            this.MapControl.SetView(coordinate, 16);

        }
        
        #endregion

        #region WP8 Location API

        private async void GetSingleLocationUsingWP8Api()
        {
            // new instance
            geolocatorWP8 = new Geolocator();

            // GPS or Cellular
            geolocatorWP8.DesiredAccuracy = PositionAccuracy.High;
            // attach status changed handler 
            // it must be attached, otherwise you'll get an exception
            geolocatorWP8.StatusChanged += geolocatorWP8_StatusChanged;

            // time of use of cached location
            TimeSpan maxAge = TimeSpan.FromMinutes(5);
            // timeout of aquiring location
            TimeSpan timeout = TimeSpan.FromSeconds(30);

            var ourLocation = await geolocatorWP8.GetGeopositionAsync(maxAge, timeout);

            /* 
             * When you launched an app and trying to get single location, in 99% you will never get a result. 
             * It happens due to bug or something in the API
             * Have a look on the workaround methods:
             *  - GeolocatorWarming1 ( I use this one in my apps )
             *  - GeolocatorWarming2
             */
        }

        private void GetContinuousLocationUsingWP8Api()
        {
            // new instance
            geolocatorWP8 = new Geolocator();

            // GPS or Cellular
            geolocatorWP8.DesiredAccuracy = PositionAccuracy.Default;

            /* ONLY ONE OF THESE TWO INTERVAL PROPERTIES MUST BE SET */

            // set interval in meters when raise the PositionChanged event
            //geolocatorWP8.MovementThreshold = 200; // 15 METERS
            // or set this interval in milliseconds 
            geolocatorWP8.ReportInterval = 120000; // 2 minute

            // attach status changed handler 
            // it must be attached, otherwise you'll get an exception
            geolocatorWP8.StatusChanged += geolocatorWP8_StatusChanged;

            // start continuous tracking attached an eventhandler
            // tracking starts automatically once the handler attached
            geolocatorWP8.PositionChanged += geolocatorWP8_PositionChanged;

            // to stop tracking detach this handler
            //geolocatorWP8.PositionChanged -= geolocatorWP8_PositionChanged;
        }

        void geolocatorWP8_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            // basically it's all the same as in WP7
            this.positionStatus = args;

            switch (args.Status)
            {
                case PositionStatus.Disabled:
                    Debug.WriteLine("position disabled");
                    MessageBox.Show("Location", "Location Disabled. Please enable location services in you phone", MessageBoxButton.OKCancel);
                    break;
                case PositionStatus.Initializing:
                    Debug.WriteLine("initializing geolocator");
                    break;
                case PositionStatus.NoData:
                    Debug.WriteLine("oooh shit no data");
                    break;
                case PositionStatus.NotInitialized:
                    Debug.WriteLine("not initialized (yet, hopefully)");
                    break;
                case PositionStatus.Ready:
                    Debug.WriteLine("are you reaaaaaady? yeeees");
                    break;
                default:
                    break;
            }
        }

        void geolocatorWP8_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            /* note, this API returns Geocoordinate object which needs to be converted to GeoCoordinate object to show on the map */

            var ourLocation = args.Position.Coordinate;
            //Debug.WriteLine("my location = " + ourLocation.ToString());
            


            //if (!App.RunningInBackground)
            //{
                // do something with user location, update map or something

                // use Dispatcher if you update UI as this code executes in another Thread
                Dispatcher.BeginInvoke(() =>
                {
                    Debug.WriteLine("my location = " + ourLocation.ToGeoCoordinate().ToString());
                    this.UserLocationMarker = (UserLocationMarker)this.FindName("UserLocationMarker");
                    //this.UserLocationMarker.GeoCoordinate = ourLocation.ToGeoCoordinate();
                    

                    if (this.UserLocationMarker != null)
                    {
                        this.UserLocationMarker.GeoCoordinate = ourLocation.ToGeoCoordinate();
                        this.UserLocationMarker.Visibility = Visibility.Visible;
                        this.myLocation = ourLocation.ToGeoCoordinate();

                        if (Settings.FirstTimeLaunch == true)
                        {
                            Debug.WriteLine("Fly to my locaaation");
                            FlyToMyLocationFirstTime(ourLocation.ToGeoCoordinate());
                            Status = "Trying to find a bike network near you...";
                            //Database.tryGetNetwork(ourLocation.ToGeoCoordinate());
                            
                        }
                    }
                    // your UI related code goes here ...

                });
            //}
            //else
            //{
            //    // if app in background => do nothing, or show toast notification
            //    ShellToast toast = new ShellToast();
            //    toast.Content = "You are at " + ourLocation.Latitude + " " + ourLocation.Longitude;
            //    toast.Show();

            //    /* I'm not sure you can update a tile here, but you can try */
            //}
        }

        #endregion

        #region Pushpin drawing

        

        public void UpdatePushpinsStationsOnMap(ObservableCollection<StationModel> stationList)
        {
            if (stationList.Count == 0)
            {
                return;
            }

            

            Status = "Drawing stations on the map...";
            Debug.WriteLine("UpdatingPushpins");
            /* 
             * we need to have a local MapLayer variable in this class to add and remove it from the map
             * otherwise you'll have a lot of layers. Here it's a "_pushpinsLayer"
             */

            // remove previously added layer from the map
            if (_pushpinsStationsLayer != null)
                MapControl.Layers.Remove(_pushpinsStationsLayer);

            if (_pushpinsNetworksLayer != null)
                MapControl.Layers.Remove(_pushpinsNetworksLayer);

            _pushpinsNetworksLayer = null;
            // refresh layer
            _pushpinsStationsLayer = new MapLayer();

            //DrawSationPushpin(stationList[1], _pushpinsLayer);

            foreach (var station in stationList)
            {
                DrawSationPushpin(station, _pushpinsStationsLayer);
                //Debug.WriteLine(station.ID);
            }

            // after we created all pins and put them on a layer, we add this layer to the map
            MapControl.Layers.Add(_pushpinsStationsLayer);

            Status = "Last Update Time: " + new TimeSpan().ToString();
            _isLoadingSomething = false;
            
            fullyloaded = true;
        }

        /*
        public void UpdatePushpinsStationsOnMap(ObservableCollection<StationModel> stationList, LocationRectangle locationRectangle)
        {
            double deltaLatitude = locationRectangle.North - locationRectangle.South;
            double deltaLongitude = locationRectangle.East - locationRectangle.West;

            double south_delta = locationRectangle.South + deltaLatitude;

           

            Debug.WriteLine("Location Rectangle latitude south = " + locationRectangle.South);
            var reducedList = stationList.ToList().FindAll(x => 
                (x.GeoCoordinate.Latitude > (locationRectangle.South - deltaLatitude/2)) &&
                (x.GeoCoordinate.Latitude < (locationRectangle.North + deltaLatitude/2)) &&
                (x.GeoCoordinate.Longitude < (locationRectangle.East + deltaLongitude/2)) &&
                (x.GeoCoordinate.Longitude > (locationRectangle.West - deltaLongitude/2))
                ).ToList();
            
            Debug.WriteLine("temp1 count = " + reducedList.Count);

            _pushpinsStationsLayer = new MapLayer();

            foreach (var station in reducedList)
            {
                DrawSationPushpin(station, _pushpinsStationsLayer);
            }
            /* 
             * we need to have a local MapLayer variable in this class to add and remove it from the map
             * otherwise you'll have a lot of layers. Here it's a "_pushpinsLayer"
             */

            // remove previously added layer from the map
            /*
            if (_pushpinsStationsLayer != null)
                MapControl.Layers.Remove(_pushpinsStationsLayer);

            if (_pushpinsNetworksLayer != null)
                MapControl.Layers.Remove(_pushpinsNetworksLayer);

            _pushpinsNetworksLayer = null;
            // refresh layer
            _pushpinsStationsLayer = new MapLayer();

            //DrawSationPushpin(stationList[1], _pushpinsLayer);

            foreach (var station in stationList)
            {
                DrawSationPushpin(station, _pushpinsStationsLayer);
                //Debug.WriteLine(station.ID);
            }

            // after we created all pins and put them on a layer, we add this layer to the map
            MapControl.Layers.Add(_pushpinsStationsLayer);
             
        }
    */

        public void UpdatePushpinsNetworksOnMap(ObservableCollection<NetworkModel> networkList)
        {

            Debug.WriteLine("UpdatingPushpinsNetwork...............");

            // remove previously added layer from the map
            if (_pushpinsStationsLayer != null)
                MapControl.Layers.Remove(_pushpinsStationsLayer);

            if (_pushpinsNetworksLayer != null)
                MapControl.Layers.Remove(_pushpinsNetworksLayer);

            _pushpinsStationsLayer = null;


            // refresh layer
            _pushpinsNetworksLayer = new MapLayer();

            //DrawSationPushpin(stationList[1], _pushpinsLayer);

            foreach (NetworkModel network in networkList)
            {
                DrawNetworkPushpin(network, _pushpinsNetworksLayer);
            }

            // after we created all pins and put them on a layer, we add this layer to the map
            MapControl.Layers.Add(_pushpinsNetworksLayer);
        }

        private void DrawNetworkPushpin(NetworkModel network, MapLayer _pushpinsLayer)
        {
            int radius = 20;

            GeoCoordinate coordinate = network.GeoCoordinate;


            //Pushpin.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            //_pushpinsNetworksLayer.Add(pushpin);

            SolidColorBrush gray = new SolidColorBrush(Colors.Gray);

            Ellipse gray_circle = new Ellipse();
            gray_circle.Width = radius;
            gray_circle.Height = radius;
            gray_circle.Fill = gray;

            MapOverlay overlay_rect = new MapOverlay();
            overlay_rect.Content = gray_circle;
            overlay_rect.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            overlay_rect.PositionOrigin = new Point(0.5, 0.5);
            _pushpinsNetworksLayer.Add(overlay_rect);

        }


        private void DrawSationPushpin(StationModel station, MapLayer _pushpinsLayer)
        {
            
            int radius = 25;
            double small_radius = radius * 0.85;
            double total = station.bikes + station.free;
            double bikes = station.bikes;
            double percentage = station.percentage;


            //percentage = 0.5;
            //if the arc is more than 180degress, it is a large arc
            bool isLargeArc = false;
            if (percentage >= 0.5)
                isLargeArc = true;

            //get the angle in radians
            double angle_bikes = percentage * 2 * Math.PI;            
            double angle_racks = (2 * Math.PI) - angle_bikes;

            GeoCoordinate coordinate = station.GeoCoordinate;

            //******************************Bikes****************//
            //create a path geometry
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(0,0);            
            //pathGeometry.FillRule = new FillRule(new SolidColorBrush(Colors.Green));
            

            //create a line segment
            LineSegment line = new LineSegment();
            line.Point = new Point(0, -radius);
            figure.Segments.Add(line);
            
            //creae an arc segment      
            double endPoint_x = radius * Math.Sin(angle_bikes);
            double endPoint_y = radius * Math.Cos(angle_bikes);
            ArcSegment arc = new ArcSegment();
            arc.Size = new Size(radius, radius);
            arc.RotationAngle = 0;
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.IsLargeArc = isLargeArc;
            arc.Point = new Point(endPoint_x, -endPoint_y);
            figure.Segments.Add(arc);

            

            pathGeometry.Figures.Add(figure);
            Path path = new Path();
            path.Data = pathGeometry;
            path.Fill = new SolidColorBrush(Colors.Green);
            path.Stroke = new SolidColorBrush(Colors.Green);
            
            //tag the pushpin with the station id;
            path.Tag = App.MainViewModel.StationList.IndexOf(station);
            path.Tap += path_Tap;

            SolidColorBrush gray = new SolidColorBrush(Colors.Gray);
            gray.Opacity = 0.5;            

            Ellipse gray_circle = new Ellipse();
            gray_circle.Width = small_radius *2 ;
            gray_circle.Height = small_radius *2 ;
            gray_circle.Fill = gray;
            //tag the pushpin with the station id;
            gray_circle.Tag = App.MainViewModel.StationList.IndexOf(station);
            gray_circle.Tap += path_Tap;



            EllipseGeometry circle = new EllipseGeometry();
            circle.RadiusX = small_radius * 2;
            circle.RadiusY = small_radius * 2;
            circle.Center = new Point(small_radius, small_radius);
  
           

            //***********************Empty racks****************//
            //create a path geometry
            PathGeometry pathGeometry_r = new PathGeometry();
            PathFigure figure_r = new PathFigure();
            figure_r.StartPoint = new Point(0, 0);

            //create a line segment
            LineSegment line_r = new LineSegment();
            line_r.Point = new Point(0, -small_radius);
            figure_r.Segments.Add(line_r);

            //creae an arc segment      
            double r_endPoint_x = small_radius * Math.Sin(angle_bikes);
            double r_endPoint_y = small_radius * Math.Cos(angle_bikes);
            ArcSegment arc_r = new ArcSegment();
            arc_r.Size = new Size(small_radius, small_radius);
            arc_r.RotationAngle = 0;
            arc_r.SweepDirection = SweepDirection.Counterclockwise;
            arc_r.IsLargeArc = !isLargeArc;
            arc_r.Point = new Point(r_endPoint_x, -r_endPoint_y);
            figure_r.Segments.Add(arc_r);

            pathGeometry_r.Figures.Add(figure_r);
            Path path_r = new Path();
            path_r.Data = pathGeometry_r;
            gray = new SolidColorBrush(Colors.Gray);
            gray.Opacity = 0.5;
            path_r.Fill = gray;
            path_r.Stroke = gray;

            //DrawingBrush opacityBrush = new DrawingBrush();
            
            //path.OpacityMask = path_r;


            //tag the pushpin with the station id;
            path_r.Tag = App.MainViewModel.StationList.IndexOf(station);
            path_r.Tap += path_Tap;
            

            Rectangle rect = new Rectangle();
            rect.Width = radius * 2;
            rect.Height = radius * 2;
            rect.Fill = new SolidColorBrush(Colors.Black);
            //tag the pushpin with the station id;
            rect.Tag = App.MainViewModel.StationList.IndexOf(station);
            rect.Tap += path_Tap;

            //***********************************************************
            // Create a MapOverlay and add marker
       
            MapOverlay overlay_r = new MapOverlay();
            overlay_r.Content = gray_circle;
            overlay_r.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            overlay_r.PositionOrigin = new Point(0.5, 0.5);
            _pushpinsLayer.Add(overlay_r);

            //greeen thing
            MapOverlay overlay = new MapOverlay();
            overlay.Content = path;
            overlay.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            overlay.PositionOrigin = new Point(0, 0);
            _pushpinsLayer.Add(overlay);


       }

        #endregion

        #region Geolocator Stuff

        private async Task ShowUserLocation()
        {
            Geolocator geolocator;
            Geoposition geoposition;

            this.UserLocationMarker = (UserLocationMarker)this.FindName("UserLocationMarker");

            geolocator = new Geolocator();

            geoposition = await geolocator.GetGeopositionAsync();

            this.UserLocationMarker.GeoCoordinate = geoposition.Coordinate.ToGeoCoordinate();
            this.UserLocationMarker.Visibility = System.Windows.Visibility.Visible;
        }

        #endregion

        #region Event Handlers

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            //this.Dispatcher.BeginInvoke(new Action(this.MapFlightToSeattle));

            //this.StoresMapItemsControl.ItemsSource = this.mainViewModel.StoreList;

            Debug.WriteLine("Page Loaded");

            if (Settings.FirstTimeLaunch == true)
            {
                return;
            }

            if (App.isLaunching)
            {

                GeoCoordinate lastCoordinate = Settings.LastUserLocation;

                if (lastCoordinate != null)
                {
                    MapControl.SetView(lastCoordinate, 17);
                    myLocation = lastCoordinate;
                }
            }
            LocationRectangle locationRectangle = getLocationRectangle();
            UpdatePushpinsStationsOnMap(App.MainViewModel.StationList);
            App.isLaunching = false;
        }

        private void RefreshClick(object sender, EventArgs e)
        {

        }

        private void TimerClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/TimerPage.xaml?",UriKind.Relative));
        }

        private void path_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            Shape path = sender as Shape;


            //MapControl.Layers.Remove(_pushpinsLayer); 

            if (path != null)
            {
                string id = "10";
                id = path.Tag.ToString();
                //
                Debug.WriteLine("id of station sent to pivot= " + id);
                NavigationService.Navigate(new Uri("/PivotPage.xaml?"
                            + "stationID=" + id + "&navigatedFrom=" + PivotEnum.pushPinTap.ToString(), UriKind.Relative));
            }


        }
        private void appbar_favorite(object sender, EventArgs e)
        {
            //MapControl.Layers.Remove(_pushpinsLayer);
            NavigationService.Navigate(new Uri("/PivotPage.xaml?"
                            + "navigatedFrom=" + PivotEnum.AppBar_Favorites, UriKind.Relative));

        }

        private void appbar_nearby(object sender, EventArgs e)
        {

            if (Settings.UserLocation == false)
            {
                var result = MessageBox.Show("You need to activate your location to find nearby stations ", "Location", MessageBoxButton.OK);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            string location = "";
            string locationStatus = "&location=";
            if (this.myLocation != null && Settings.UserLocation == true)
            {
                locationStatus += "true";
                location = "&locationLatitude=" + myLocation.Latitude + "&locationLongitude=" + myLocation.Longitude;
            }
            else
            {
                locationStatus += "false";
            }
            //MapControl.Layers.Remove(_pushpinsLayer);
            NavigationService.Navigate(new Uri("/PivotPage.xaml?"
                            + "navigatedFrom=" + PivotEnum.AppBar_Nearby + locationStatus + location, UriKind.Relative));

        }

        private void appbar_routes(object sender, EventArgs e)
        {
            //MapControl.Layers.Remove(_pushpinsLayer);
            NavigationService.Navigate(new Uri("/PivotPage.xaml?"
                            + "navigatedFrom=" + PivotEnum.AppBar_Routes, UriKind.Relative));

        }


        private void MapControl_ZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
        {
            
            int zoomLevel = (int)this.MapControl.ZoomLevel;

                     

            //turning zoom level into int so we don't have that many updates
            if ((this.lastZoomLevel != zoomLevel) && zoomLevel > 3)
            {
                //Debug.WriteLine(zoomLevel);
                Debug.WriteLine("Zoom Level Changed = " + zoomLevel + " lastZoomLevel = " + lastZoomLevel);
                LocationRectangle locationRectangle = getLocationRectangle();
                if ((zoomLevel <= lastZoomLevel) && zoomLevel == 13)
                {
                    Debug.WriteLine("Trying to get network pushpins");
                    if (_pushpinsNetworksLayer == null)
                        UpdatePushpinsNetworksOnMap(App.MainViewModel.NetworkList);

                }
                if ((zoomLevel >= lastZoomLevel) && zoomLevel > 13)
                {
                    Debug.WriteLine("Update Stationss pushpins");
                    UpdatePushpinsStationsOnMap(App.MainViewModel.StationList);
                }

                if ((zoomLevel >= lastZoomLevel) && zoomLevel == 13)
                {
                    Debug.WriteLine("try to get a new network");
                    LocationRectangle networkLocationRectangle = getLocationRectangle();
                    //Debug.WriteLine("delta_latitude = " + networkLocationRectangle.HeightInDegrees);
                   // Debug.WriteLine("delta_longitude = " + networkLocationRectangle.WidthInDegrees);
                    Database.tryGetNetwork(networkLocationRectangle);
                }

                this.lastZoomLevel = zoomLevel;
            }


        }

        private void MapControl_CenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            if (locationRectangle == null)
                return;


            double delta_latitude = locationRectangle.HeightInDegrees/2;
            double delta_longitude = locationRectangle.WidthInDegrees/2;

            double diference_latitude = MapControl.Center.Latitude - mapCenter.Latitude;
            double diference_longitude = MapControl.Center.Longitude - mapCenter.Longitude;

            if (Math.Abs(delta_latitude - diference_latitude) >= 0)
            {
                this.mapCenter = MapControl.Center;
                LocationRectangle lRectangle = getLocationRectangle();
                this.locationRectangle = lRectangle;
            }

            if (Math.Abs(delta_longitude - diference_longitude) >= 0)
            {
                this.mapCenter = MapControl.Center;
                LocationRectangle lRectangle = getLocationRectangle();
                this.locationRectangle = lRectangle;
            }


        }

        #endregion


    }
}