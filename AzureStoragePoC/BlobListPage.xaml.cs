using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Acr.UserDialogs;
using Microsoft.WindowsAzure.Storage.Blob;
using Xamarin.Forms;

namespace AzureStoragePoC
{
    public partial class BlobListPage : ContentPage
    {
        public BlobListPage(IEnumerable<CloudBlockBlob> blobs)
        {
            InitializeComponent();
            ListView.ItemsSource = blobs;
        }

        async void ImageClicked(object sender, SelectedItemChangedEventArgs e)
        {
			var blob = (CloudBlockBlob)e.SelectedItem;
            try
            {
				using (var imageStream = new MemoryStream())
				{
					await blob.DownloadToStreamAsync(imageStream);
					if (imageStream.Length > 0)
					{
                        await Navigation.PushAsync(new ImagePage(blob.Name, 
                                                                 imageStream.ToArray()));
					}
				}
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to download image {blob.Name}: {ex}");
                UserDialogs.Instance.Toast("Image download failed!",
                                           new TimeSpan(0, 0, 2));
            }
        }
    }
}
