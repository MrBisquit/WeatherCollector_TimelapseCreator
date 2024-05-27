using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WeatherCollector_TimelapseCreator.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPreparingPage : Page
    {
        public MainPreparingPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Initialising the generator...");
            Debug.WriteLine(Windows.Storage.ApplicationData.Current.LocalFolder.Path);

            Core.Generation.Generator generator = new Core.Generation.Generator(Globals.Info);
            Progress<Core.Generation.Generator.Progress> progress = new Progress<Core.Generation.Generator.Progress>();

            progress.ProgressChanged += Progress_ProgressChanged;

            generator.GeneratePaths();
            Task.Factory.StartNew(() =>
            {
                // Multi-threading 🎉

                generator.StartGeneration(progress);
            });
        }

        private void Progress_ProgressChanged(object sender, Core.Generation.Generator.Progress e)
        {
            Step.Text = e.StepText;
            StepPB.Value = e.Step;
            PStep.Text = e.PStepText;
            PStepPB.Value = e.ProgressStep;
            PStepPB.IsIndeterminate = e.Indeterminate;
        }
    }
}
