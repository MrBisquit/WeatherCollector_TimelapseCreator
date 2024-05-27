using System.Diagnostics;

namespace WeatherCollector_TimelapseCreator.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        Globals.MainPage = this;
        Globals.Config = new Core.Config(); // Init config
        Globals.ServerInfo = new Core.ServerInfo(); // Init server info
        Globals.Info = new Core.Info(); // Init info
        appTitleBar.Window = App.CurrentWindow;
        App.Current.JsonNavigationViewService.Initialize(NavView, NavFrame);
        App.Current.JsonNavigationViewService.ConfigJson("Assets/NavViewMenu/AppData.json");

        //NavigateFrame(new HomeLandingPage());

        //App.Current.PageService = new PageServiceEx();
        //App.Current.PageService.SetDefaultPage(typeof(HomeLandingPage));
        //App.Current.navigationServiceEx = new NavigationServiceEx(App.Current.PageService);
        //App.Current.NavigationViewService = new NavigationViewServiceEx(App.Current.navigationServiceEx, App.Current.PageService);
        //App.Current.NavigationViewService.Initialize(NavView);

        // Now attempt to load the config
        Debug.WriteLine("Attempting to load config");
        Globals.Config.Prepare();
        try
        {
            Globals.Config.LoadConfig();
        } catch (Exception ex)
        {
            // No file present
            Globals.Config.SaveConfig();
            Debug.WriteLine(ex.ToString());
            Debug.WriteLine(Globals.AppDataBase);
        }
        Debug.WriteLine(Globals.AppDataBase);
    }

    public void NavigateFrame(Page page)
    {
        NavFrame.Navigate(typeof(MainPage), page);
    }

    private void appTitleBar_BackButtonClick(object sender, RoutedEventArgs e)
    {
        /*if (NavFrame.CanGoBack)
        {
            NavFrame.GoBack();
        }*/
    }

    private void appTitleBar_PaneButtonClick(object sender, RoutedEventArgs e)
    {
        //NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void NavFrame_Navigated(object sender, NavigationEventArgs e)
    {
        //appTitleBar.IsBackButtonVisible = NavFrame.CanGoBack;
    }

    private void ThemeButton_Click(object sender, RoutedEventArgs e)
    {
        var element = App.CurrentWindow.Content as FrameworkElement;

        if (element.ActualTheme == ElementTheme.Light)
        {
            element.RequestedTheme = ElementTheme.Dark;
        }
        else if (element.ActualTheme == ElementTheme.Dark)
        {
            element.RequestedTheme = ElementTheme.Light;
        }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        //NavigateFrame(new HomeLandingPage());
        //App.Current.NavigationViewService.Initialize(NavView);
    }
}

