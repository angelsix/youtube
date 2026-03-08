using Avalonia.Data.Converters;
using Avalonia.Media;
using BatchProcess3.ViewModels.Pages;
using System;
using System.Globalization;

namespace BatchProcess3.ValueConverters;

public class JobStatusToProgressBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not JobStatus status)
            return Brushes.Gray;

        return status switch
        {
            JobStatus.Running => new LinearGradientBrush
            {
                StartPoint = new(0, 0.5, Avalonia.RelativeUnit.Relative),
                EndPoint = new(1, 0.5, Avalonia.RelativeUnit.Relative),
                GradientStops = [new(Color.Parse("#06b6d4"), 0), new(Color.Parse("#6366f1"), 1)]
            },
            JobStatus.Success => new LinearGradientBrush
            {
                StartPoint = new(0, 0.5, Avalonia.RelativeUnit.Relative),
                EndPoint = new(1, 0.5, Avalonia.RelativeUnit.Relative),
                GradientStops = [new(Color.Parse("#22c55e"), 0), new(Color.Parse("#16a34a"), 1)]
            },
            JobStatus.Failed => new LinearGradientBrush
            {
                StartPoint = new(0, 0.5, Avalonia.RelativeUnit.Relative),
                EndPoint = new(1, 0.5, Avalonia.RelativeUnit.Relative),
                GradientStops = [new(Color.Parse("#ef4444"), 0), new(Color.Parse("#dc2626"), 1)]
            },
            JobStatus.Paused => new LinearGradientBrush
            {
                StartPoint = new(0, 0.5, Avalonia.RelativeUnit.Relative),
                EndPoint = new(1, 0.5, Avalonia.RelativeUnit.Relative),
                GradientStops = [new(Color.Parse("#f59e0b"), 0), new(Color.Parse("#d97706"), 1)]
            },
            _ => Brushes.Gray
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class JobStatusToBadgeBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not JobStatus status)
            return Brushes.Transparent;

        return status switch
        {
            JobStatus.Running => new SolidColorBrush(Color.Parse("#2606b6d4")),
            JobStatus.Success => new SolidColorBrush(Color.Parse("#1F22c55e")),
            JobStatus.Failed => new SolidColorBrush(Color.Parse("#1Fef4444")),
            JobStatus.Paused => new SolidColorBrush(Color.Parse("#1Ff59e0b")),
            _ => Brushes.Transparent
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class JobStatusToBadgeForegroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not JobStatus status)
            return Brushes.White;

        return status switch
        {
            JobStatus.Running => new SolidColorBrush(Color.Parse("#06b6d4")),
            JobStatus.Success => new SolidColorBrush(Color.Parse("#4ade80")),
            JobStatus.Failed => new SolidColorBrush(Color.Parse("#f87171")),
            JobStatus.Paused => new SolidColorBrush(Color.Parse("#fbbf24")),
            _ => Brushes.White
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class JobStatusToCardTopBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var param = parameter as string ?? "";

        return param switch
        {
            "purple" => new LinearGradientBrush
            {
                StartPoint = new(0, 0.5, Avalonia.RelativeUnit.Relative),
                EndPoint = new(1, 0.5, Avalonia.RelativeUnit.Relative),
                GradientStops = [new(Color.Parse("#c026d3"), 0), new(Color.Parse("#e879f9"), 1)]
            },
            "teal" => new LinearGradientBrush
            {
                StartPoint = new(0, 0.5, Avalonia.RelativeUnit.Relative),
                EndPoint = new(1, 0.5, Avalonia.RelativeUnit.Relative),
                GradientStops = [new(Color.Parse("#06b6d4"), 0), new(Color.Parse("#0ea5e9"), 1)]
            },
            "green" => new LinearGradientBrush
            {
                StartPoint = new(0, 0.5, Avalonia.RelativeUnit.Relative),
                EndPoint = new(1, 0.5, Avalonia.RelativeUnit.Relative),
                GradientStops = [new(Color.Parse("#22c55e"), 0), new(Color.Parse("#16a34a"), 1)]
            },
            "red" => new LinearGradientBrush
            {
                StartPoint = new(0, 0.5, Avalonia.RelativeUnit.Relative),
                EndPoint = new(1, 0.5, Avalonia.RelativeUnit.Relative),
                GradientStops = [new(Color.Parse("#ef4444"), 0), new(Color.Parse("#dc2626"), 1)]
            },
            _ => Brushes.Transparent
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
