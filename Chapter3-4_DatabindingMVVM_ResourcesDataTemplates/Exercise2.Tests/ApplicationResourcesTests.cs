using Exercise2.Converters;
using System.Windows.Controls;
using System.Windows.Media;

namespace Exercise2.Tests;

[Apartment(ApartmentState.STA)]
[ExerciseTestFixture("progAdvNet", "H03-04", "Exercise02",
    @"Exercise2\App.xaml;Exercise2\Resources\Brushes.xaml;Exercise2\Resources\Converters.xaml;")]
public class ApplicationResourcesTests
{
    private ApplicationTester _applicationTester = null!;

    [OneTimeSetUp]
    public void BeforeAllTests()
    {
        _applicationTester = new ApplicationTester();
        _applicationTester.LoadApplication();
    }

    [MonitoredTest]
    public void _01_ShouldHaveADarkRadialBrush()
    {
        RadialGradientBrush darkRadialGradientBrush = _applicationTester.TryGetApplicationResource<RadialGradientBrush>("DarkRadialBrush");
        Assert.That(darkRadialGradientBrush.GradientStops.Count, Is.EqualTo(2), "The DarkRadialBrush should have 2 colors");
        Assert.That(darkRadialGradientBrush.GradientStops[1].Offset, Is.GreaterThan(0.5d), "At (at least) 50% of the center the brush should have the second color");
        Assert.That(darkRadialGradientBrush.GradientStops[0].Color != darkRadialGradientBrush.GradientStops[1].Color, Is.True, "The brush should use 2 different colors");
    }

    [MonitoredTest]
    public void _02_ShouldHaveADarkSolidBrush()
    {
        SolidColorBrush darkSolidBrush = _applicationTester.TryGetApplicationResource<SolidColorBrush>("DarkSolidBrush");
        Assert.That(darkSolidBrush.Color.R < 150 && darkSolidBrush.Color.G < 150 && darkSolidBrush.Color.B < 150,
            Is.True, "The color of the DarkSolidBrush should be darker");
    }

    [MonitoredTest]
    public void _03_ShouldHaveAFontBrush()
    {
        SolidColorBrush fontBrush = _applicationTester.TryGetApplicationResource<SolidColorBrush>("FontBrush");
        Assert.That(fontBrush.Color.R > 150 && fontBrush.Color.G > 150 && fontBrush.Color.B > 150,
            Is.True, "The color of the FontBrush should be lighter");
    }

    [MonitoredTest]
    public void _04_ShouldHaveARatingStarsConverter()
    {
        _applicationTester.TryGetApplicationResource<RatingStarsConverter>("RatingStarsConverter");
    }

    [MonitoredTest]
    public void _05_ShouldHaveABooleanToVisibilityConverter()
    {
        _applicationTester.TryGetApplicationResource<BooleanToVisibilityConverter>("BooleanToVisibilityConverter");
    }
}