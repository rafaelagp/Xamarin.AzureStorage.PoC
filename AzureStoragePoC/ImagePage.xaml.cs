using System.IO;
using Xamarin.Forms;

namespace AzureStoragePoC
{
    public partial class ImagePage : ContentPage
    {
        public ImagePage(string title, byte[] imageStream)
        {
            InitializeComponent();
			Title = title;
            Image.Source = ImageSource.FromStream(() => new MemoryStream(imageStream));
        }
    }
}
