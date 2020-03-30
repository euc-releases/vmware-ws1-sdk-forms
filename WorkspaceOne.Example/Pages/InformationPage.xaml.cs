using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WorkspaceOne.Forms.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkspaceOne.Example.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InformationPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        public InformationPage()
        {
            InitializeComponent();

            var wso = DependencyService.Get<IWorkspaceOne>().SharedInstance;
            var not = wso != null ? "not" : "";

            Debug.WriteLine($"[{this.GetType()}] wso is {not} null");

            Items = new ObservableCollection<string>
            {
                $"SDK Version: {wso?.SdkVersion ?? string.Empty}",
                $"Server URL: {wso?.DeviceServicesUrl ?? string.Empty}",
                //$"Group ID: {wso?.Account?.ActivationCode ?? string.Empty}",
                //$"Username: {wso?.Account?.Username ?? string.Empty}"
            };

            MyListView.ItemsSource = Items;
        }
    }
}
