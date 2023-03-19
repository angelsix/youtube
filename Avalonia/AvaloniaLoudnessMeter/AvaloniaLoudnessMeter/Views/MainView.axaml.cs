using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaLoudnessMeter.Services;
using AvaloniaLoudnessMeter.ViewModels;
using NWaves.Signals;
using NWaves.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace AvaloniaLoudnessMeter.Views
{
    public partial class MainView : UserControl
    {
        #region Private Members

        private MainViewModel mViewModel => (MainViewModel)DataContext;

        private AudioCaptureService mCaptureDevice;

        private Queue<double> mLufs = new Queue<double>();

        private int mCaptureFrequecy = 44100;

        private Control mChannelConfigPopup;
        private Control mChannelConfigButton;
        private Control mMainGrid;
        private Control mVolumeContainer;
        
        /// <summary>
        /// The timeout timer to detect when auto-sizing has finished firing
        /// </summary>
        private Timer mSizingTimer;

        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <exception cref="Exception">Throws if named controls cannot be found</exception>
        public MainView()
        {
            InitializeComponent();

            mSizingTimer = new Timer((t) =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    // Update the desired size
                    UpdateSizes();
                });
            });

            // Gather the named controls
            mChannelConfigButton = this.FindControl<Control>("ChannelConfigurationButton") ?? throw new Exception("Cannot find Channel Configuration Button by name");
            mChannelConfigPopup = this.FindControl<Control>("ChannelConfigurationPopup") ?? throw new Exception("Cannot find Channel Configuration Popup by name");
            mMainGrid = this.FindControl<Control>("MainGrid") ?? throw new Exception("Cannot find Main Grid by name");
            mVolumeContainer = this.FindControl<Control>("VolumeContainer") ?? throw new Exception("Cannot find Volume Container by name");
        }
        
        #endregion

        private void UpdateSizes()
        {
            mViewModel.VolumeContainerSize = mVolumeContainer.Bounds.Height;
        }

        protected override async void OnLoaded()
        {
            await mViewModel.LoadSettingsCommand.ExecuteAsync(null);

            StartCapture(1);
            
            base.OnLoaded();
        }

        private void StartCapture(int deviceId)
        {
            mCaptureDevice = new AudioCaptureService(deviceId, mCaptureFrequecy);
            
            mCaptureDevice.DataAvailable += (buffer, length) =>
            {
                CalculateValues(buffer);
            };
        
            mCaptureDevice.Start();
        }

        private void CalculateValues(byte[] buffer)
        {
            //Console.WriteLine(BitConverter.ToString(buffer));

            // Get total PCM16 samples in this buffer (16 bits per sample)
            var sampleCount = buffer.Length / 2;

            // Create our Discrete Signal ready to be filled with information
            var signal = new DiscreteSignal(mCaptureFrequecy, sampleCount);

            // Loop all bytes and extract the 16 bits, into signal floats
            using var reader = new BinaryReader(new MemoryStream(buffer));

            for (var i = 0; i < sampleCount; i++)
                signal[i] = reader.ReadInt16() / 32768f;
            
            // Calculate the LUFS
            var lufs = Scale.ToDecibel(signal.Rms() * 1.2);
            mLufs.Enqueue(lufs);
            
            // Keep list to 10 samples
            if (mLufs.Count > 10)
                mLufs.Dequeue();

            var averageLufs = mLufs.Average();

            Dispatcher.UIThread.InvokeAsync(() => mViewModel.ShortTermLoudness = $"{averageLufs:0.0} LUFS");
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            mSizingTimer.Change(100, int.MaxValue);

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Get relative position of button, in relation to main grid
                var position = mChannelConfigButton.TranslatePoint(new Point(), mMainGrid) ??
                               throw new Exception("Cannot get TranslatePoint from Configuration Button");

                // Set margin of popup so it appears bottom left of button
                mChannelConfigPopup.Margin = new Thickness(
                    position.X,
                    0,
                    0,
                    mMainGrid.Bounds.Height - position.Y - mChannelConfigButton.Bounds.Height);
            });
        }

        private void InputElement_OnPointerPressed(object sender, PointerPressedEventArgs e)
            => mViewModel.ChannelConfigurationButtonPressedCommand.Execute(null);
    }
}