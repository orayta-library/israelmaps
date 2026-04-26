using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;

namespace IsraelMapsApp;

public sealed partial class MainWindow : Window
{
    private bool isOfflineMode = false;

    public MainWindow()
    {
        InitializeComponent();
        InitializeWebView();
    }

    private async void InitializeWebView()
    {
        try
        {
            LoadingRing.IsActive = true;
            await MapWebView.EnsureCoreWebView2Async();
            
            MapWebView.CoreWebView2.Settings.IsScriptEnabled = true;
            MapWebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            
            LoadingRing.IsActive = false;
        }
        catch (Exception ex)
        {
            LoadingRing.IsActive = false;
            var dialog = new ContentDialog
            {
                Title = "שגיאה",
                Content = $"לא ניתן לטעון את המפה: {ex.Message}",
                CloseButtonText = "אישור",
                XamlRoot = Content.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }

    private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (!string.IsNullOrWhiteSpace(args.QueryText))
        {
            var script = $"searchLocation('{args.QueryText.Replace("'", "\\'")}');";
            await MapWebView.ExecuteScriptAsync(script);
        }
    }

    private async void OfflineToggle_Click(object sender, RoutedEventArgs e)
    {
        isOfflineMode = OfflineToggle.IsChecked ?? false;
        var script = $"setOfflineMode({isOfflineMode.ToString().ToLower()});";
        await MapWebView.ExecuteScriptAsync(script);
    }
}
