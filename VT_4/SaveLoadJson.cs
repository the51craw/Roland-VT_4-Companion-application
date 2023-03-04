using JsonToSource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace VT_4
{
    public sealed partial class MainPage : Page
    {
        private async Task ReadJsonFile()
        {
            try
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.FileTypeFilter.Add(".json");
                StorageFile file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    String content = await FileIO.ReadTextAsync(file);
                    VT4 = JsonConvert.DeserializeObject<VT4>(content);
                    UpdateGui();
                }
            }
            catch (Exception e) { }
        }

        private async Task WriteJsonFile()
        {
            try
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.FileTypeChoices.Add("Json", new List<string>() { ".json" });
                StorageFile saveSettings = await savePicker.PickSaveFileAsync();
                if (saveSettings != null)
                {
                    String json = JsonHelper.FormatJson(JsonConvert.SerializeObject(VT4));
                    CachedFileManager.DeferUpdates(saveSettings);
                    await FileIO.WriteTextAsync(saveSettings, json);
                }
            }
            catch (Exception e) { }
        }
    }
}
