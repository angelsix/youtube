using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace AvaloniaLoudnessMeter;

public class AnimatedPopup : ContentControl
{
    #region Private Members

    /// <summary>
    /// Store the controls desired size
    /// </summary>
    private Size mDesiredSize;

    /// <summary>
    /// A flag for when we are animating
    /// </summary>
    private bool mAnimating;

    /// <summary>
    /// The animation UI timer
    /// </summary>
    private DispatcherTimer mAnimationTimer;

    /// <summary>
    /// The current position in the animation
    /// </summary>
    private int mAnimationCurrentTick;
    
    #endregion

    #region Constructor
    
    /// <summary>
    /// Default constructor
    /// </summary>
    public AnimatedPopup()
    {
        // Get a 60 FPS timespan
        var framerate = TimeSpan.FromSeconds(1 / 60.0);

        // Make a new dispatch timer
        mAnimationTimer = new DispatcherTimer
        {
            // Set the timer to run 60 times a second
            Interval = framerate 
        };
        
        // Fix for 3 seconds
        var animationTime = TimeSpan.FromSeconds(1);

        // Calculate total ticks that make up the animation time
        var totalTicks = (int)(animationTime.TotalSeconds / framerate.TotalSeconds);

        // Keep track of current tick
        mAnimationCurrentTick = 0;
        
        // Callback on every tick
        mAnimationTimer.Tick += (sender, e) =>
        {
            // Increment the tick
            mAnimationCurrentTick++;
            
            // Set animating flag
            mAnimating = true;

            // If we have reached the total ticks...
            if (mAnimationCurrentTick > totalTicks)
            {
                // Stop this animation timer
                mAnimationTimer.Stop();
                
                // Clear animating flag
                mAnimating = false;
                
                // Break out of code
                return;
            }

            // Get percentage of the way through the current animation
            var percentageAnimated = (float)mAnimationCurrentTick / totalTicks;
            
            // Make an animation easing
            var easing = new QuadraticEaseIn();
            
            // Calculate final width and height
            var finalWidth = mDesiredSize.Width * easing.Ease(percentageAnimated);
            var finalHeight = mDesiredSize.Height * easing.Ease(percentageAnimated);

            // Do our animation
            Width = finalWidth;
            Height = finalHeight;
            
            Console.WriteLine($"Current tick: {mAnimationCurrentTick}");
        };
    }
    
    #endregion

    public override void Render(DrawingContext context)
    {
        // If we are not animating...
        if (!mAnimating)
        {
            // Set desired size (which includes margin, so remove that from our calculation)
            mDesiredSize = DesiredSize - Margin;
            
            // Reset animation position
            mAnimationCurrentTick = 0;
            
            // Star timer
            mAnimationTimer.Start();

            Console.WriteLine($"Desired size: {mDesiredSize}");
        }

        base.Render(context);
    }
}