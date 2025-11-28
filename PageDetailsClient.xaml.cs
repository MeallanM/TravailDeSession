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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TravailDeSession;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PageDetailsClient : Page
{
    public PageDetailsClient()
    {
        InitializeComponent();
    }



    private async void btnSupprimer_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Confirmation",
                Content = "Voulez-vous vraiment supprimer ce client?",
                PrimaryButtonText = "Oui",
                CloseButtonText = "Non",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (btn.Tag is Client prod)
                    SingletonGeneralUse.getInstance().SupprimerClient(prod);
            }
        }
    }

    private void BtnModifier_Click(object sender, RoutedEventArgs e)
    {

    }
}
