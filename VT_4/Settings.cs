using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using static VT_4.MainPage;

namespace VT_4
{
    public class Settings
    {
        public ApplicationDataContainer LocalSettings;

        private MainPage mainPage;

        public Settings(MainPage mainPage)
        {
            this.mainPage = mainPage;
            LocalSettings = ApplicationData.Current.LocalSettings;
        }

        public void Save(String parameter, byte value)
        {
            LocalSettings.Values[parameter] = value;
        }
    }
}
