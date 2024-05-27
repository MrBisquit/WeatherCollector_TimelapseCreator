namespace WeatherCollector_TimelapseCreator;

public partial class App : Application
{
    public static Window CurrentWindow = Window.Current;
    public IThemeService ThemeService { get; set; }
    public IJsonNavigationViewService JsonNavigationViewService { get; set; }
    //public IPageServiceEx PageService { get; set; }
    //public INavigationViewServiceEx NavigationViewService { get; set; }
    //public INavigationServiceEx navigationServiceEx { get; set; }
    public new static App Current => (App)Application.Current;
    public string AppVersion { get; set; } = AssemblyInfoHelper.GetAssemblyVersion();
    public string AppName { get; set; } = "WeatherCollector_TimelapseCreator";
    public App()
    {
        this.InitializeComponent();
        JsonNavigationViewService = new JsonNavigationViewService();
        JsonNavigationViewService.ConfigDefaultPage(typeof(HomeLandingPage));
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        CurrentWindow = new Window();

        CurrentWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        CurrentWindow.AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;

        if (CurrentWindow.Content is not Frame rootFrame)
        {
            CurrentWindow.Content = rootFrame = new Frame();
        }

        ThemeService = new ThemeService();
        ThemeService.Initialize(CurrentWindow);
        ThemeService.ConfigBackdrop();
        ThemeService.ConfigElementTheme();

        rootFrame.Navigate(typeof(MainPage));

        CurrentWindow.Title = CurrentWindow.AppWindow.Title = $"{AppName} v{AppVersion}";
        CurrentWindow.AppWindow.SetIcon("Assets/icon.ico");

        CurrentWindow.Activate();
        ContextMenuItem menu = new ContextMenuItem
        {
            Title = "Open WeatherCollector_TimelapseCreator Here",
            AcceptDirectory = true,
            Exe = "WeatherCollector_TimelapseCreator.exe",
            Param = "{path}"
        };
        await ContextMenuService.Ins.SaveAsync(menu);
    }
}

