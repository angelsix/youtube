using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using AvaloniaLoudnessMeter.DataModels;
using AvaloniaLoudnessMeter.Services;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaLoudnessMeter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    #region Private Members

    /// <summary>
    /// The audio capture service
    /// </summary>
    private IAudioCaptureService mAudioCaptureService;

    #endregion

    #region Public Properties

    [ObservableProperty] private string _boldTitle = "AVALONIA";

    [ObservableProperty] private string _regularTitle = "LOUDNESS METER";

    [ObservableProperty] private string _shortTermLoudness = "0 LUFS";

    [ObservableProperty] private string _integratedLoudness = "0 LUFS";

    [ObservableProperty] private string _loudnessRange = "0 LU";

    [ObservableProperty] private string _realtimeDynamics = "0 LU";

    [ObservableProperty] private string _averageDynamics = "0 LU";

    [ObservableProperty] private string _momentaryMaxLoudness = "0 LUFS";

    [ObservableProperty] private string _shortTermMaxLoudness = "0 LUFS";

    [ObservableProperty] private string _truePeakMax = "0 dB";

    [ObservableProperty] private bool _channelConfigurationListIsOpen;

    [ObservableProperty] private double _volumePercentPosition;

    [ObservableProperty] private double _volumeContainerSize; 

    [ObservableProperty]
    private ObservableGroupedCollection<string, ChannelConfigurationItem> _channelConfigurations = default!;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ChannelConfigurationButtonText))]
    private ChannelConfigurationItem? _selectedChannelConfiguration;

    public string ChannelConfigurationButtonText => SelectedChannelConfiguration?.ShortText ?? "Select Channel";

    #endregion

    #region Public Commands

    [RelayCommand]
    private void ChannelConfigurationButtonPressed() => ChannelConfigurationListIsOpen ^= true;

    [RelayCommand]
    private void ChannelConfigurationItemPressed(ChannelConfigurationItem item)
    {
        // Update the selected item
        SelectedChannelConfiguration = item;

        // Close the menu
        ChannelConfigurationListIsOpen = false;
    }

    /// <summary>
    /// Do initial loading of data and settings up services
    /// </summary>
    [RelayCommand]
    private async Task LoadAsync()
    {
        // Get the channel configuration data
        var channelConfigurations = await mAudioCaptureService.GetChannelConfigurationsAsync();

        // Create a grouping from the flat data
        ChannelConfigurations =
            new ObservableGroupedCollection<string, ChannelConfigurationItem>(
                channelConfigurations.GroupBy(item => item.Group));
        
        StartCapture(1);
    }
    
    #endregion

    #region Constructor

    /// <summary>
    ///     Default constructor
    /// </summary>
    /// <param name="audioInterfaceService">The audio interface service</param>
    public MainViewModel(IAudioCaptureService audioInterfaceService)
    {
        mAudioCaptureService = audioInterfaceService;
        
        Initialize();
    }

    /// <summary>
    ///     Design-time constructor
    /// </summary>
    public MainViewModel()
    {
        mAudioCaptureService = new BassAudioCaptureService();
        
        Initialize();
    }
    
    #endregion

    private void Initialize()
    {
        // Temp code to move volume position
        
        var tick = 0;
        var input = 0.0;

        var tempTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1 / 60.0)
        };

        tempTimer.Tick += (s, e) =>
        {
            tick++;

            // Slow down ticks
            input = tick / 20f;
            
            // Scale value
            var scale = _volumeContainerSize / 2f;

            VolumePercentPosition = (Math.Sin(input) + 1) * scale;
        };
        
        tempTimer.Start();
    }
    
    /// <summary>
    /// Starts capturing audio from the specified device
    /// </summary>
    /// <param name="deviceId">The device ID</param>
    private void StartCapture(int deviceId)
    {
        mAudioCaptureService = new BassAudioCaptureService(deviceId);
            
        // Listen out for chunks of information
        mAudioCaptureService.AudioChunkAvailable += audioChuckData =>
        {
            ShortTermLoudness = $"{audioChuckData.ShortTermLUFS:0.0} LUFS";
            IntegratedLoudness  = $"{audioChuckData.IntegratedLUFS:0.0} LUFS";
            LoudnessRange = $"{audioChuckData.LoudnessRange:0.0} LU";
            RealtimeDynamics = $"{audioChuckData.RealtimeDynamics:0.0} LU";
            AverageDynamics = $"{audioChuckData.AverageRealtimeDynamics:0.0} LU";
            MomentaryMaxLoudness = $"{audioChuckData.MomentaryMaxLUFS:0.0} LUFS";
            ShortTermMaxLoudness = $"{audioChuckData.ShortTermMaxLUFS:0.0} LUFS";
            TruePeakMax = $"{audioChuckData.TruePeakMax:0.0} dB";
        };
        
        // Start capturing
        mAudioCaptureService.Start();
    }
}