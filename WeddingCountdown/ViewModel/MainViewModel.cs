using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using WeddingCountdown.Extend;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.IO;
using Windows.UI.Xaml.Controls;
using System.Runtime.CompilerServices;

namespace WeddingCountdown.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public const string ClockPropertyName = "Clock";

        private string _clock = "Starting...";
        private bool _runClock;

        private RelayCommand _uploadPicture;
        private string _image = "assets/back1.jpg";

        private DateTimeOffset? weddingDate;


        public DateTimeOffset? WeddingDate
        {
            get
            {
                return this.weddingDate;
            }
            set
            {
                this.weddingDate = value;
            }
        }


        public string Image
        {
            get
            {
                    return _image;
            }
            set
            {
                this._image = value;
                RaisePropertyChanged("Image");
            }
        }


        public string Clock
        {
            get
            {
                return _clock;
            }
            set
            {
                Set(ClockPropertyName, ref _clock, value);
            }
        }

        private async void PickImage()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                string imageName = GetStoredImagePath();

                if (imageName.Contains("assets/") == false)
                {
                    if (await ApplicationData.Current.RoamingFolder.FileExistsAsync(imageName))
                    {
                        StorageFile x = await ApplicationData.Current.RoamingFolder.GetFileAsync(imageName);

                        await x.DeleteAsync();
                    }
                }
               
                imageName = Guid.NewGuid().ToString() + ".jpg";

                var newFile = await file.CopyAsync(ApplicationData.Current.RoamingFolder, imageName);

                SaveStoredImagePath(imageName);

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Image = newFile.Path;
                });

            }
            else
            {
            }
        }


        private string GetStoredImagePath()
        {
            var roamingSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (roamingSettings.Values.ContainsKey("myimage"))
            {
                return roamingSettings.Values["myimage"].ToString();
            }
            else
            {
                return "assets/back1.jpg";
            }
        }

        private DateTimeOffset? GetWeddingDate()
        {
            DateTimeOffset dt = DateTimeOffset.Now;

            var roamingSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (roamingSettings.Values.ContainsKey("mydate"))
            {
                return (DateTimeOffset)roamingSettings.Values["mydate"];
            }
            else
            {
            }

            return dt;
        }

        private void SetWeddingDate(DateTimeOffset dt)
        {
            var roamingSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (roamingSettings.Values.ContainsKey("mydate"))
            {
                roamingSettings.Values["mydate"] = dt;
            }
            else
            {
                roamingSettings.Values.Add("mydate", dt);
            }

        }


        private void SaveStoredImagePath(string path)
        {
            var roamingSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (roamingSettings.Values.ContainsKey("myimage"))
            {
                roamingSettings.Values["myimage"] = path;
            }
            else
            {
                roamingSettings.Values.Add("myimage", path);
            }
        }

        public RelayCommand UploadPictureCommand
        {
            get
            {
                return _uploadPicture
                    ?? (_uploadPicture = new RelayCommand(
                    () =>
                    {
                        PickImage();
                    }));

            }
        }


        public MainViewModel()
        {
            Initialize();
        }

        public void RunClock()
        {
            _runClock = true;

            Task.Run(async () =>
            {
                while (_runClock)
                {
                    try
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            TimeSpan ts = weddingDate.Value - DateTime.Now;

                            Clock = ts.ToReadableString();

                            SetWeddingDate(weddingDate.Value);

                        });

                        await Task.Delay(1000);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            });
        }

        public void StopClock()
        {
            _runClock = false;
        }

        private async Task Initialize()
        {
            try
            {
                WeddingDate = GetWeddingDate();

                string imageName = GetStoredImagePath();
                if (await ApplicationData.Current.RoamingFolder.FileExistsAsync(imageName))
                {
                    StorageFile x = await ApplicationData.Current.RoamingFolder.GetFileAsync(imageName);

                    Image = x.Path;
                }
                    

            }
            catch (Exception ex)
            {
                // Report error here
            }
        }
    }
}