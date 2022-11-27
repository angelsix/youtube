using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaLoudnessMeter.ViewModels;
using System;

namespace AvaloniaLoudnessMeter.Views
{
    public partial class MainView : UserControl
    {
        #region Private Members

        private Control mChannelConfigPopup;
        private Control mChannelConfigButton;
        private Control mMainGrid;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <exception cref="Exception">Throws if named controls cannot be found</exception>
        public MainView()
        {
            InitializeComponent();

            // Gather the named controls
            mChannelConfigButton = this.FindControl<Control>("ChannelConfigurationButton") ?? throw new Exception("Cannot find Channel Configuration Button by name");
            mChannelConfigPopup = this.FindControl<Control>("ChannelConfigurationPopup") ?? throw new Exception("Cannot find Channel Configuration Popup by name");
            mMainGrid = this.FindControl<Control>("MainGrid") ?? throw new Exception("Cannot find Main Grid by name");
        }
        
        #endregion

        protected override async void OnLoaded()
        {
            await ((MainViewModel)DataContext).LoadSettingsCommand.ExecuteAsync(null);

            base.OnLoaded();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

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
            => ((MainViewModel)DataContext).ChannelConfigurationButtonPressedCommand.Execute(null);
    }
}