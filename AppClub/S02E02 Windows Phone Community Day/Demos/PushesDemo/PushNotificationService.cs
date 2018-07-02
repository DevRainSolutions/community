using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Info;
using Microsoft.Phone.Notification;

namespace PushesDemo
{
    public class PushNotificationsService
    {
        HttpNotificationChannel CurrentChannel { get; set; }

        private bool _started = false;

        public void Start()
        {
           if (_started) return;

            _started = true;

            AcquirePushChannel();

        }


        void AcquirePushChannel()
        {
            //if (!NetworkHelper.IsNetworkAvailable()) return;

            CurrentChannel = HttpNotificationChannel.Find("Test");

            if (CurrentChannel == null)
            {
                CurrentChannel = new HttpNotificationChannel("Test");
                CurrentChannel.ChannelUriUpdated += CurrentChannel_ChannelUriUpdated;
                CurrentChannel.ErrorOccurred += CurrentChannel_ErrorOccurred;
                //CurrentChannel.HttpNotificationReceived += CurrentChannelOnHttpNotificationReceived;
                CurrentChannel.Open();

            }
            else
            {
                RegisterForNotifications();
            }


        }




 
        void CurrentChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            Debug.WriteLine("1111_CurrentChannel_ErrorOccurred");
            Debug.WriteLine(e.Message);

            


        }

        void CurrentChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            CurrentChannel.ChannelUriUpdated -= CurrentChannel_ChannelUriUpdated;
            CurrentChannel.ErrorOccurred -= CurrentChannel_ErrorOccurred;
            RegisterForNotifications();
        }

        async Task RegisterForNotifications()
        {
            if (!CurrentChannel.IsShellToastBound)
            {
                CurrentChannel.BindToShellToast();
            }

            CurrentChannel.ErrorOccurred -= CurrentChannel_ErrorOccurred;
            CurrentChannel.ShellToastNotificationReceived -= channel_ShellToastNotificationReceived;
            CurrentChannel.ErrorOccurred += CurrentChannel_ErrorOccurred;
            CurrentChannel.ShellToastNotificationReceived += channel_ShellToastNotificationReceived;

            if (CurrentChannel.ChannelUri != null)
            {
                var id = GetDeviceUniqueID();

                var channels = await App.MobileService.GetTable<Channel>().Where(x => x.DeviceId == id).ToListAsync();
                if (channels.Any())
                {
                    var c = channels.First();
                    c.UpdateDate = DateTime.Today;
                    c.Uri = CurrentChannel.ChannelUri.ToString();

                    await App.MobileService.GetTable<Channel>().UpdateAsync(c);
                }
                else
                {
                    var c = new Channel();
                    c.UpdateDate = DateTime.Today;
                    c.Uri = CurrentChannel.ChannelUri.ToString();
                    c.DeviceId = id;

                    await App.MobileService.GetTable<Channel>().InsertAsync(c);
                }


            }
            else
            {
                _result.SetResult(false);
            }

        }



        private TaskCompletionSource<bool> _result;
        public async Task<bool> RegisterPushNotifications()
        {
            _result = new TaskCompletionSource<bool>();

            AcquirePushChannel();

            return await _result.Task;
        }

        public async Task UnRegisterForNotifications()
        {
            var id = GetDeviceUniqueID();

            var channels = await App.MobileService.GetTable<Channel>().Where(x => x.DeviceId == id).ToListAsync();
            if (channels.Any())
            {
                await App.MobileService.GetTable<Channel>().DeleteAsync(channels.First());
            }

            if (CurrentChannel != null)
            {
                CurrentChannel.ErrorOccurred -= CurrentChannel_ErrorOccurred;
                CurrentChannel.ShellToastNotificationReceived -= channel_ShellToastNotificationReceived;
            }


        }

        static async void channel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            Debug.WriteLine("1111_channel_ShellToastNotificationReceived");




        }



        public static string GetDeviceUniqueID()
        {
            try
            {
                byte[] result = null;
                object uniqueId;
                if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uniqueId))
                    result = (byte[])uniqueId;

                return Convert.ToBase64String(result);
            }
            catch
            {
                return string.Empty;
            }
        }

    }
}
