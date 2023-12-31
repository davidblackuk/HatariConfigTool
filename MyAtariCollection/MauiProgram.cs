﻿global using System;
global using System.Text;
global using Microsoft.Extensions.Logging;

global using TinyMvvm;
global using System.Windows.Input;
global using Maui.BindableProperty.Generator.Core;

global using MyAtariCollection.Extensions;
global using MyAtariCollection.Services;
global using MyAtariCollection.Models;
global using MyAtariCollection.Views;
global using MyAtariCollection.ViewModels;

global using CommunityToolkit.Maui;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;

global using Mopups.Interfaces;


using CommunityToolkit.Maui.Storage;
using Mopups.Hosting;
using Mopups.Services;
using MyAtariCollection.Services.ConfigFileSections;
using MyAtariCollection.Services.Filesystem;

namespace MyAtariCollection;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIconsOutlined-Regular.otf", "MaterialIcons");
            }) 
            .ConfigureMopups()
            .UseTinyMvvm()
            .UseMauiCommunityToolkit();

        builder.Services.AddTransient<MainView>();
        builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<NewSystemPopup>();
        builder.Services.AddTransient<NewSystemPopupViewModel>();

        builder.Services.AddTransient<PreferencesPopup>();
        builder.Services.AddTransient<PreferencesPopupViewModel>();
        
        builder.Services.AddTransient<FujiFilePickerPopup>();
        builder.Services.AddTransient<FujiFilePickerPopupViewModel>();

        builder.Services.AddTransient<IFolderPicker>(provider => FolderPicker.Default);
        builder.Services.AddTransient<IFilePicker>(provider => FilePicker.Default);

        builder.Services.AddSingleton<IPopupNavigation>(MopupService.Instance);


        builder.Services.AddTransient<ILogConfigFileSection, LogConfigFileSection>();
        builder.Services.AddTransient<IMemoryConfigFileSection, MemoryConfigFileSection>();
        builder.Services.AddTransient<ISystemConfigFileSection, SystemConfigFileSection>();
        builder.Services.AddTransient<IRomConfigFileSection, RomConfigFileSection>();
        builder.Services.AddTransient<IHardDiskConfigFileSection, HardDiskConfigFileSection>();
        builder.Services.AddTransient<IFloppyConfigFileSection, FloppyConfigFileSection>();
        builder.Services.AddTransient<IAcsiConfigFileSection, AcsiConfigFileSection>();
        builder.Services.AddTransient<IScsiConfigFileSection, ScsiConfigFileSection>();
        builder.Services.AddTransient<IIdeConfigFileSection, IdeConfigFileSection>();
        builder.Services.AddTransient<IScreenConfigFileSection, ScreenConfigFileSection>();
        builder.Services.AddTransient<ISoundConfigFileSection, SoundConfigFileSection>();
        builder.Services.AddTransient<IConfigFileService, ConfigFileService>();
        
        // Services
        builder.Services.AddSingleton<IMachineTemplateService, MachineTemplateService>();
        builder.Services.AddSingleton<ISystemsService, SystemsService>();
        builder.Services.AddSingleton<IPreferencesService, PreferencesService>();
        builder.Services.AddSingleton<IFujiFilePickerService, FujiFilePickerService>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        var built =  builder.Build();

        var preferencesService = built.Services.GetService<IPreferencesService>();
        preferencesService.Load();
        
        return built;
    }
}