using System;
using System.Diagnostics;
using System.Linq;
using Acr.UserDialogs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace AzureStoragePoC
{
    public partial class AzureStoragePoCPage : ContentPage
    {
        const string STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=https;" +
                                                 "AccountName=ACCOUNT_NAME_HERE;" +
                                                 "AccountKey=ACCOUNT_KEY_HERE;" +
                                                 "EndpointSuffix=core.windows.net";
        readonly static CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(STORAGE_CONNECTION_STRING);
        readonly static CloudBlobClient _storageServiceCli = _storageAccount.CreateCloudBlobClient();
        readonly static CloudBlobContainer _storageImagesContainer = _storageServiceCli.GetContainerReference("images");

        public AzureStoragePoCPage()
        {
            InitializeComponent();
        }

        async void DownloadClicked(object sender, EventArgs e)
        {
            using (UserDialogs.Instance.Loading("Loading..."))
            {
                if (await _storageImagesContainer.ExistsAsync())
                {
                    try
                    {
						//This overload allows control of the page size. You can return all remaining results by passing null for the maxResults parameter,
						//or by calling a different overload.
						var segment = await _storageImagesContainer.ListBlobsSegmentedAsync("", 
						                                                                    true, 
						                                                                    BlobListingDetails.All, 
						                                                                    null, null, null, null);
                        var blobs = segment.Results.Select(x => (CloudBlockBlob)x);
                        if (!blobs.Any())
                        {
                            UserDialogs.Instance.Alert("No images found",
                                                       "There were no images uploaded yet.",
                                                       "OK");
                            return;
                        }
                        await Navigation.PushAsync(new BlobListPage(blobs));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Azure Storage blob listing failed for container {_storageImagesContainer.Name}: {ex}");
                        UserDialogs.Instance.Toast("Loading failed!",
                                                   new TimeSpan(0, 0, 2));
                        return;
                    }
                }
            }
        }

        async void UploadClicked(object sender, EventArgs e)
        {
            if (CrossMedia.IsSupported && CrossMedia.Current.IsPickPhotoSupported)
			{
                using (var mediaFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    MaxWidthHeight = 1080,
                    PhotoSize = PhotoSize.MaxWidthHeight
                }))
                {
                    if (mediaFile != null)
                    {
                        try
                        {
                            using (UserDialogs.Instance.Loading("Uploading..."))
							{
                                Debug.WriteLine($"Selected Image Path = {mediaFile.Path}");
                                await _storageImagesContainer.CreateIfNotExistsAsync();
                                var fileName = System.IO.Path.GetFileName(mediaFile.Path);
                                var blob = _storageImagesContainer.GetBlockBlobReference(fileName);
								await blob.UploadFromStreamAsync(mediaFile.GetStream());
                            }
                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine($"Azure Storage upload failed for file {mediaFile.Path}: {ex}");
                            UserDialogs.Instance.Toast("Upload failed!", 
                                                       new TimeSpan(0, 0, 2));
                            return;
                        }
                        UserDialogs.Instance.Toast("Upload successful!",
                                                       new TimeSpan(0, 0, 2));
                    }
                }
			}
        }
    }
}
