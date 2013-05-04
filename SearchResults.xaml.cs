using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ModernAlpha
{
    public static class Globals
    {
        public static string searchAssumptionsInput { get; set; }
    }
    
    public sealed partial class SearchResults : ModernAlpha.Common.LayoutAwarePage
    {
        private string searchText;
        private string searchAssumptionsDesc;
        private string appID = "INSERT-YOUR-ID-HERE";

        private List<string> listTitle = new List<string>();
        private List<string> listPlain = new List<string>();
        private List<string> listImage = new List<string>();
        private List<string> listAssumptions = new List<string>();

        private string[] listTitleArray;
        private string[] listPlainArray;
        private string[] listImageArray;
        private string[] listAssumptionsArray;
        
        private DataTransferManager dataTransferManager;
        private HttpClient httpClient;
        private HttpClientHandler handler;

        public class FeedItem
        {
            public string TitleTitle { get; set; }
            public string TitleImage { get; set; }
            public int ColSpan { get; set; }
            public int RowSpan { get; set; }
        }

        public SearchResults()
        {
            this.InitializeComponent();

            handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false;

            httpClient = new HttpClient(handler);
            // Limit the max buffer size for the response so we don't get overwhelmed
            httpClient.MaxResponseContentBufferSize = 256000;
            // Add a user-agent header
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.dataTransferManager = DataTransferManager.GetForCurrentView();
            this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.ShareTextHandler);
            
            searchText = e.Parameter as string;
            textBoxSearch.Text = searchText;

            GetResults();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.ShareTextHandler);
        }

        private void textBoxSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string searchText = textBoxSearch.Text;

                this.Frame.Navigate(typeof(SearchResults), searchText);
            }
        }

        private void assumptionsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Globals.searchAssumptionsInput = listAssumptionsArray[assumptionsBox.SelectedIndex];

                this.Frame.Navigate(typeof(SearchResults), searchText);
            }
            catch { }
        }

        private async void GetResults()
        {
            if (NetworkInformation.GetInternetConnectionProfile() != null)
            {
                HttpResponseMessage response = await httpClient.GetAsync("http://api.wolframalpha.com/v2/query?appid="+ appID + "&input=" + searchText + "&format=image,plaintext&assumption=" + Globals.searchAssumptionsInput);

                string xmlString = await response.Content.ReadAsStringAsync();

                XDocument xdoc = XDocument.Parse(xmlString);

                var queryresult = xdoc.Element("queryresult").Elements("pod");
                var queryresultAssumptions = xdoc.Element("queryresult").Elements("assumptions").Elements("assumption").Elements("value");

                string returnValueTitle = string.Empty;

                string returnValuePlain = string.Empty;

                string returnValueImage = string.Empty;
                string returnValueImageWidth = string.Empty;
                string returnValueImageHeight = string.Empty;

                int returnValueImageWidthInt = 0;
                int returnValueImageWidthIntLargest = 0;
                int returnValueImageHeightInt = 0;

                foreach (var singleImageSize in queryresult)
                {
                    returnValueImageWidth = string.Empty;
                    returnValueImageWidth += singleImageSize.Element("subpod").Element("img").Attribute("width");
                    returnValueImageWidth = returnValueImageWidth.Replace("width=", "");
                    returnValueImageWidth = returnValueImageWidth.Replace("\"", "");

                    returnValueImageWidthInt = int.Parse(returnValueImageWidth);

                    if (returnValueImageWidthIntLargest < int.Parse(returnValueImageWidth))
                        returnValueImageWidthIntLargest = int.Parse(returnValueImageWidth);
                }

                if (returnValueImageWidthIntLargest < 400)
                    returnValueImageWidthIntLargest = 400;

                foreach (var singleImage in queryresult)
                {
                    returnValueTitle = string.Empty;
                    returnValueTitle += singleImage.Attribute("title");
                    returnValueTitle = returnValueTitle.Replace("title=", "");
                    returnValueTitle = returnValueTitle.Replace("\"", "");

                    returnValuePlain = string.Empty;
                    returnValuePlain += singleImage.Element("subpod").Element("plaintext");
                    returnValuePlain = returnValuePlain.Replace("<plaintext>", "");
                    returnValuePlain = returnValuePlain.Replace("</plaintext>", "");

                    returnValueImage = string.Empty;
                    returnValueImage += singleImage.Element("subpod").Element("img").Attribute("src");
                    returnValueImage = returnValueImage.Replace("src=", "");
                    returnValueImage = returnValueImage.Replace("\"", "");
                    returnValueImage = returnValueImage.Replace("amp;", "");

                    returnValueImageHeight = string.Empty;
                    returnValueImageHeight += singleImage.Element("subpod").Element("img").Attribute("height");
                    returnValueImageHeight = returnValueImageHeight.Replace("height=", "");
                    returnValueImageHeight = returnValueImageHeight.Replace("\"", "");

                    returnValueImageHeightInt = int.Parse(returnValueImageHeight);

                    FeedItem feedItem = new FeedItem()
                    {
                        TitleTitle = returnValueTitle,
                        TitleImage = returnValueImage,
                        ColSpan = returnValueImageWidthIntLargest + 30,
                        RowSpan = returnValueImageHeightInt + 70
                    };

                    resultsPanel.Items.Add(feedItem);

                    listTitle.Add(returnValueTitle);
                    listPlain.Add(returnValuePlain);
                    listImage.Add(returnValueImage);
                }

                foreach (var assumption in queryresultAssumptions)
                {
                    searchAssumptionsDesc = string.Empty;
                    searchAssumptionsDesc += assumption.Attribute("desc");
                    searchAssumptionsDesc = searchAssumptionsDesc.Replace("desc=", "");
                    searchAssumptionsDesc = searchAssumptionsDesc.Replace("\"", "");

                    Globals.searchAssumptionsInput = string.Empty;
                    Globals.searchAssumptionsInput += assumption.Attribute("input");
                    Globals.searchAssumptionsInput = Globals.searchAssumptionsInput.Replace("input=", "");
                    Globals.searchAssumptionsInput = Globals.searchAssumptionsInput.Replace("\"", "");

                    assumptionsBox.Items.Add("as " + searchAssumptionsDesc);

                    listAssumptions.Add(Globals.searchAssumptionsInput);
                }

                try
                {
                    assumptionsBox.SelectedIndex = 0;
                    assumptionsBox.Visibility = Visibility.Visible;

                    assumptionsColumn.Width = new GridLength(2, GridUnitType.Star);
                    assumptionsColumn.MinWidth = 200;
                }
                catch { }

                progressBar.Visibility = Visibility.Collapsed;

                listTitleArray = listTitle.ToArray();
                listPlainArray = listPlain.ToArray();
                listImageArray = listImage.ToArray();
                listAssumptionsArray = listAssumptions.ToArray();

                if (resultsPanel.Items.Count == 0)
                {
                    textBlockError.Text = "No results for your search term were found.";
                    textBlockError.Visibility = Visibility.Visible;
                }
            }
            else
            {
                progressBar.Visibility = Visibility.Collapsed;
                
                textBlockError.Text = "Your PC isn't connected to the Internet. To use Einstein, connect to the Internet and then try searching again.";
                textBlockError.Visibility = Visibility.Visible;
            }
        }

        private void resultsPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (resultsPanel.SelectedIndex != -1)
            {
                if (listPlainArray[resultsPanel.SelectedIndex] != "")
                    copyText.Visibility = Visibility.Visible;

                copyImage.Visibility = Visibility.Visible;
                saveImage.Visibility = Visibility.Visible;

                appBar.IsOpen = true;
            }
        }

        private void appBar_Opened(object sender, object e)
        {
            if (resultsPanel.SelectedIndex != -1)
            {
                if (listPlainArray[resultsPanel.SelectedIndex] != "")
                    copyText.Visibility = Visibility.Visible;

                copyImage.Visibility = Visibility.Visible;
                saveImage.Visibility = Visibility.Visible;
            }
            else
            {
                viewOnline.Visibility = Visibility.Visible;
            }
        }

        private void appBar_Closed(object sender, object e)
        {
            resultsPanel.SelectedIndex = -1;

            copyText.Visibility = Visibility.Collapsed;
            copyImage.Visibility = Visibility.Collapsed;
            saveImage.Visibility = Visibility.Collapsed;

            if (ApplicationView.Value == ApplicationViewState.Snapped)
                viewOnline.Visibility = Visibility.Collapsed;
            else
                viewOnline.Visibility = Visibility.Visible;
        }

        private void copyText_Click(object sender, RoutedEventArgs e)
        {
            DataPackage copyText = new DataPackage();
            copyText.SetText(listPlainArray[resultsPanel.SelectedIndex]);

            Clipboard.SetContent(copyText);

            appBar.IsOpen = false;
        }

        private void copyImage_Click(object sender, RoutedEventArgs e)
        {
            DataPackage copyImage = new DataPackage();
            copyImage.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(listImageArray[resultsPanel.SelectedIndex])));

            Clipboard.SetContent(copyImage);

            appBar.IsOpen = false;
        }

        private async void saveImage_Click(object sender, RoutedEventArgs e)
        {
            DataPackage saveImage = new DataPackage();
            saveImage.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(listImageArray[resultsPanel.SelectedIndex])));

            StorageFile file = await KnownFolders.PicturesLibrary.CreateFileAsync(searchText + " - " + listTitleArray[resultsPanel.SelectedIndex] + ".jpeg", CreationCollisionOption.GenerateUniqueName);

            BackgroundDownloader downloader = new BackgroundDownloader();

            DownloadOperation d = downloader.CreateDownload(new Uri(listImageArray[resultsPanel.SelectedIndex]), file);

            await d.StartAsync();

            appBar.IsOpen = false;
        }

        private async void viewOnline_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(("http://www.wolframalpha.com/input/?i=" + searchText).Replace(" ", "+")));

            appBar.IsOpen = false;
        }

        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Description = "Share this search result";
            request.Data.SetUri(new Uri(("http://www.wolframalpha.com/input/?i=" + searchText).Replace(" ", "+")));
            request.Data.SetText(searchText);

            if (resultsPanel.SelectedIndex != -1)
            {
                request.Data.Properties.Title = searchText + ": " + (listTitleArray[resultsPanel.SelectedIndex]) + " - Einstein";
                request.Data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(listImageArray[resultsPanel.SelectedIndex]));
                request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(listImageArray[resultsPanel.SelectedIndex])));
            }
            else
            {
                request.Data.Properties.Title = searchText + " - Einstein";
            }
        }
    }
}
