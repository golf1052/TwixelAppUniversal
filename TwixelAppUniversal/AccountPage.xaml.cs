using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TwixelAppUniversal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountPage : Page
    {
        ObservableCollection<AccountListViewBinding> accounts;
         
        public AccountPage()
        {
            this.InitializeComponent();
            accounts = new ObservableCollection<AccountListViewBinding>();
            AccountListViewBinding voyboy = new AccountListViewBinding();
            voyboy.Image = "http://static-cdn.jtvnw.net/jtv_user_pictures/voyboy-profile_image-68389dfbd20eaece-300x300.jpeg";
            voyboy.Name = "Voyboy";
            AccountListViewBinding golf1052 = new AccountListViewBinding();
            golf1052.Image = "http://static-cdn.jtvnw.net/jtv_user_pictures/golf1052-profile_image-45a81d49cc8f742a-300x300.png";
            golf1052.Name = "golf1052";
            accounts.Add(voyboy);
            accounts.Add(golf1052);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HelperMethods.HideSplitView();
            if (AppConstants.users.Count == 0)
            {

            }
            else
            {
                proceedTextBlock.Text = "Choose an account";
            }
            base.OnNavigatedTo(e);
        }

        private void accountsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (accountsListView.SelectedIndex == -1)
            {
                proceedButton.Content = "Login";
            }
            else
            {
                proceedButton.Content = "Select";
            }
        }

        private void accountsListView_Loaded(object sender, RoutedEventArgs e)
        {
            accountsListView.ItemsSource = accounts;
        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            accountsListView.SelectedIndex = -1;
        }
    }
}
