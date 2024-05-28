using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;

namespace WeatherCollector_TimelapseCreator.Views;

public sealed partial class HomeLandingPage : Page
{
    public string AppInfo { get; set; }
    public HomeLandingPage()
    {
        this.InitializeComponent();
        AppInfo = $"{App.Current.AppName} v{App.Current.AppVersion}";
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        //allLandingPage.GetData(App.Current.JsonNavigationViewService.DataSource);
        //allLandingPage.OrderBy(i => i.Title);
    }

    private void allLandingPage_OnItemClick(object sender, RoutedEventArgs e)
    {
        var args = (ItemClickEventArgs)e;
        var item = (DataItem)args.ClickedItem;

        //App.Current.JsonNavigationViewService.NavigateTo(item.UniqueId + item.Parameter?.ToString(), item);
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            ServerURL.Text = Globals.Config.ServerLocation;
            Username.Text = Globals.Config.Username;
            Password.Password = Globals.Config.Password;
        } catch { }
    }

    private void Grid_Holding(object sender, Microsoft.UI.Xaml.Input.HoldingRoutedEventArgs e)
    {

    }

    private async void Login_Click(object sender, RoutedEventArgs e)
    {
        if(string.IsNullOrEmpty(ServerURL.Text) || string.IsNullOrEmpty(Password.Password)) { return; }
        ServerURL.IsEnabled = false;
        Username.IsEnabled = false;
        Password.IsEnabled = false;
        Remember.IsEnabled = false;
        Cancel.IsEnabled = false;
        Login.IsEnabled = false;
        Loader.Visibility = Visibility.Visible;

        // Authentication
        Globals.Config.ServerLocation = ServerURL.Text;
        Core.Auth auth = await Core.Auth.Authenticate(Username.Text, Password.Password);
        if(auth.Token == null)
        {
            ServerURL.IsEnabled = true;
            Username.IsEnabled = true;
            Password.IsEnabled = true;
            Remember.IsEnabled = true;
            Cancel.IsEnabled = true;
            Login.IsEnabled = true;
            Loader.Visibility = Visibility.Collapsed;

            return;
        } else
        {
            // Authenticated
            if(Remember.IsChecked == true)
            {
                // Save creds
                Globals.Config.Username = Username.Text;
                Globals.Config.Password = Password.Password;
                Globals.Config.AuthToken = auth.Token;
                Globals.Config.SaveConfig();
            }
        }

        LoadingPage.Visibility = Visibility.Visible;
        LoginPage.Visibility = Visibility.Collapsed;

        // Fetching server base information & seeing how long an image takes to download
        Debug.WriteLine("Auth token: " + Globals.Config.AuthToken);
        try
        {
            Globals.ServerInfo.DataRequest = await Core.Data.RequestFull(Globals.Config.AuthToken);
            DateTime dt = DateTime.Now;
            Globals.ServerInfo.DataRequestCurrentDay = await Core.Data.RequestDay(Globals.Config.AuthToken, dt.Year.ToString(), dt.Month.ToString(), dt.Day.ToString());
        } catch (Exception ex) { }
        Stopwatch sw = new Stopwatch();
        sw.Restart();
        Globals.ServerInfo.ServerImageTest = await Core.Data.RequestLatestImage(Globals.Config.AuthToken);
        sw.Stop();
        Globals.ServerInfo.ServerImageResponseTime = sw.Elapsed;
        Globals.Preview = await Core.Data.RequestLatestImageBitmap(Globals.Config.AuthToken);
        Globals.BasePreview = Globals.Preview;

        // Initialise the cache
        Core.Cache.CacheManager.Initialise();

        // Redirecting to the main page
        App.Current.JsonNavigationViewService.NavigateTo("WeatherCollector_TimelapseCreator.Views.MainSelectionPage");
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        App.Current.Exit();
    }
}

