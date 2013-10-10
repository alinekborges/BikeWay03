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
using BikeWay03.ViewModels;
using BikeWay03.Util;
using BikeWay03.DB;

namespace BikeWay03
{
    public partial class MainPage : PhoneApplicationPage
    {
        MapLayer _pushpinsLayer;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            App.MainPage = this;
            // Set the data context of the LongListSelector control to the sample data
            DataContext = App.MainViewModel;

            Database.initializeDatabase();

            Settings.loadAllSettings();

            Debug.WriteLine("is Network loaded = " + Settings.IsNetworkSavedToDatabase.ToString());
           
            // Sample code to localize the ApplicationBarupda
            //BuildLocalizedApplicationBar();
        }


        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {


            if (!App.MainViewModel.IsDataLoaded)
            {
                App.MainPage = this;
                App.MainViewModel.LoadDataFromAPI();
                UpdatePushpinsOnTheMap(App.MainViewModel.StationList);
                //App.MainPage.UpdatePushpinsOnTheMap(App.StationListViewModel.StationList);
            }
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

        public void UpdatePushpinsOnTheMap(ObservableCollection<StationModel> stationList) 
        {

            Debug.WriteLine("UpdatingPushpins");
            /* 
             * we need to have a local MapLayer variable in this class to add and remove it from the map
             * otherwise you'll have a lot of layers. Here it's a "_pushpinsLayer"
             */

            // remove previously added layer from the map
            if (_pushpinsLayer != null)
                MapControl.Layers.Remove(_pushpinsLayer);

            // refresh layer
            _pushpinsLayer = new MapLayer();

            //DrawSationPushpin(stationList[1], _pushpinsLayer);

            foreach (var station in stationList)
            {
                DrawSationPushpin(station, _pushpinsLayer);
                //Debug.WriteLine(station.ID);
            }

            // after we created all pins and put them on a layer, we add this layer to the map
            MapControl.Layers.Add(_pushpinsLayer);
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
                Debug.WriteLine("id of station sent to pivot= " +id);
                NavigationService.Navigate(new Uri("/PivotPage.xaml?" 
                            + "stationID="+ id + "&navigatedFrom=" + PivotEnum.pushPinTap.ToString(), UriKind.Relative));
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
            //MapControl.Layers.Remove(_pushpinsLayer);
            NavigationService.Navigate(new Uri("/PivotPage.xaml?"
                            + "navigatedFrom=" + PivotEnum.AppBar_Nearby, UriKind.Relative));

        }

        private void appbar_routes(object sender, EventArgs e)
        {
            //MapControl.Layers.Remove(_pushpinsLayer);
            NavigationService.Navigate(new Uri("/PivotPage.xaml?"
                            + "navigatedFrom=" + PivotEnum.AppBar_Routes, UriKind.Relative));

        } 

        private void DrawSationPushpin(StationModel station, MapLayer _pushpinsLayer)
        {

            
            int radius = 25;
            double small_radius = radius * 0.85;
            double total = station.bikes + station.free;
            double bikes = station.bikes;
            double percentage = 0.85;
            if (total != 0)
            {
                //calculate the porcentage of bikes in the station
                percentage = bikes / total;
                station.percentage = percentage;
                //Debug.WriteLine(percentage);
            }

            //percentage = 0.5;
            //if the arc is more than 180degress, it is a large arc
            bool isLargeArc = false;
            if (percentage >= 0.5)
                isLargeArc = true;

            //get the angle in radians
            double angle_bikes = percentage * 2 * Math.PI;            
            double angle_racks = (2 * Math.PI) - angle_bikes;

            //Debug.WriteLine(angle_bikes);
            //Debug.WriteLine(angle_racks);

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

            //GeometryGroup pie = new GeometryGroup();
//pie.Children.Add(circle);
            //pie.Children.Add(pathGeometry);
            


            //***********************************************************
            // Create a MapOverlay and add marker

            //MapOverlay overlay_rect = new MapOverlay();
            //overlay_rect.Content = rect;
            //overlay_rect.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            //overlay_rect.PositionOrigin = new Point(0.5, 0.5);
            //_pushpinsLayer.Add(overlay_rect);

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

        private void RefreshClick(object sender, EventArgs e)
        {

        }

        private void TimerClick(object sender, EventArgs e)
        {

        }




        private void MapControl_ZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
        {
            Debug.WriteLine(this.MapControl.ZoomLevel);
        }


    }
}