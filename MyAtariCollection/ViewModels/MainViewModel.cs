using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Storage;
using MyAtariCollection.Services.Filesystem;


namespace MyAtariCollection.ViewModels;

public partial class MainViewModel : TinyViewModel
{
    private readonly ISystemsService systemService;
    private readonly IConfigFileService configFileService;
    private readonly IPopupNavigation popupNavigation;
    private readonly IServiceProvider serviceProvider;
    private readonly IPreferencesService preferencesService;
    private readonly IFujiFilePickerService fujiFilePicker;

    public MainViewModel(ISystemsService systemService, IConfigFileService configFileService, 
        IPopupNavigation popupNavigation, 
        IServiceProvider serviceProvider, IPreferencesService preferencesService,
        IFujiFilePickerService fujiFilePicker)
    {
        this.systemService = systemService;
        this.configFileService = configFileService;
        this.popupNavigation = popupNavigation;
        this.serviceProvider = serviceProvider;
        this.preferencesService = preferencesService;
        this.fujiFilePicker = fujiFilePicker;
    }


    [ObservableProperty] 
    private ObservableCollection<AtariConfiguration> systems = new();

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(HasSelectedConfig))]
    private AtariConfiguration selectedConfiguration;
    
    [ObservableProperty]
    private SectionVisibility sectionVisibility = new SectionVisibility(); 

    public bool HasSelectedConfig => SelectedConfiguration != null;
    

    #region ---- RELAY COMMANDS ----
    
        
    [RelayCommand()]
    private async void BrowseRoms()
    {
        var file = await fujiFilePicker.PickFile("ROM Image",(filename) => SelectedConfiguration.RomImage = filename, preferencesService.Preferences.RomFolder);
    }
    
    [RelayCommand]
    private void ClearRom()
    {
        SelectedConfiguration.RomImage = String.Empty;
    }
    [RelayCommand()]

    private async void BrowseCartridges()
    {
         await fujiFilePicker.PickFile("Cartridge Image",(filename) => SelectedConfiguration.CartridgeImage = filename, preferencesService.Preferences.CartridgeFolder);
    }
    
    [RelayCommand]
    private void ClearCartridge()
    {
        SelectedConfiguration.CartridgeImage = String.Empty;
    }
    
    #region disk image  operations
    
    [RelayCommand()]
    private async void BrowseAcsiDiskImage(int diskId)
    {
        await fujiFilePicker.PickFile("ASCI Disk Image",
            (filename) => SelectedConfiguration.AcsiImagePaths.SetImagePath(diskId, filename), preferencesService.Preferences.HardDiskFolder);
    }
    
    [RelayCommand()]
    private void ClearAcsiDiskImage(int diskId) => SelectedConfiguration.AcsiImagePaths.ClearImagePath(diskId);


    [RelayCommand()]
    private async void BrowseScsiDiskImage(int diskId)
    {
        await fujiFilePicker.PickFile("SCSI Disk Image",
            (filename) => SelectedConfiguration.ScsiImagePaths.SetImagePath(diskId, filename), preferencesService.Preferences.HardDiskFolder);
    }
    
    [RelayCommand()]
    private void ClearScsiDiskImage(int diskId) => SelectedConfiguration.ScsiImagePaths.ClearImagePath(diskId);

    [RelayCommand()]
    private async void BrowseIdeDiskImage(int diskId)
    {
        await fujiFilePicker.PickFile("IDE Disk Image",
            (filename) => SelectedConfiguration.IdeOptions.SetImagePath(diskId, filename), preferencesService.Preferences.HardDiskFolder);
    }
    
    [RelayCommand()]
    private void ClearIdeDiskImage(int diskId) => SelectedConfiguration.IdeOptions.ClearImagePath(diskId);

    [RelayCommand()]
    private async void BrowseFloppyDiskImage(int diskId)
    {
        
        await fujiFilePicker.PickFile("Floppy Disk Image",
            (filename) => SelectedConfiguration.FloppyOptions.SetImagePath(diskId, filename), preferencesService.Preferences.FloppyDiskFolder);

    }

    [RelayCommand()]
    private void ClearFloppyDiskImage(int diskId) => SelectedConfiguration.FloppyOptions.ClearImagePath(diskId);

    
    
    [RelayCommand()]
    private async void BrowseGemdosFolder()
    {
        await fujiFilePicker.PickFolder("GEMDOS Folder",
            (filename) => SelectedConfiguration.GdosDriveOptions.GemdosFolder = filename, preferencesService.Preferences.GemDosFolder);
    }
    
    [RelayCommand()]
    private void ClearGemdosFolder(int diskId) => SelectedConfiguration.GdosDriveOptions.GemdosFolder = string.Empty;



    #endregion

    [RelayCommand]
    private async void CreateNewSystem()
    {
        var popup = serviceProvider.GetService<NewSystemPopup>();
        await popupNavigation.PushAsync(popup);

        popup.Disappearing += (sender, args) =>
        {
            if (popup.ViewModel.Confirmed)
            {
                var system = popup.ViewModel.GetConfiguration();
                Systems.Add(system);
                SelectedConfiguration = system;
            }
        };      
    }

    [RelayCommand]
    private async void EditPreferences()
    {
        var popup = serviceProvider.GetService<PreferencesPopup>();
        await popupNavigation.PushAsync(popup);

        popup.Disappearing += async (sender, args) =>
        {
             if (popup.ViewModel.Confirmed)
             {
                await preferencesService.Save();
             }
        };      
    }
 

    [RelayCommand()]
    private async void Run()
    {
       
        var configFileContent = configFileService.Generate(SelectedConfiguration);
        Console.WriteLine($"\n{configFileContent}");

        await File.WriteAllTextAsync("/Users/davidblack/Library/Application Support/Hatari/hatari.cfg", configFileContent);
        await Launcher.Default.OpenAsync("file:///Applications/Hatari.app?--args%20--machine%20falcon");
     
    }

    #endregion
}