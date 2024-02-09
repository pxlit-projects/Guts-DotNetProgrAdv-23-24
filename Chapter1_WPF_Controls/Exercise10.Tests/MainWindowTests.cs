using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Guts.Client.WPF.TestTools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Exercise10.Tests
{
    [ExerciseTestFixture("progAdvNet", "H01", "Exercise10", @"Exercise10\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private Button _rainbowButton;
        private Button _smallFontButton;
        private Button _disabledButton;
        private Style _rainbowStyle;

        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();

            var allButtons = _window.GetUIElements<Button>().ToList();
            if (allButtons.All(button => button.Parent is Grid && button.VerticalAlignment == VerticalAlignment.Top))
            {
                allButtons = allButtons.OrderBy(button => button.Margin.Top).ToList();
            }

            if (allButtons.Count >= 1)
            {
                _rainbowButton = allButtons.ElementAt(0);
            }
            if (allButtons.Count >= 2)
            {
                _smallFontButton = allButtons.ElementAt(1);
            }
            if (allButtons.Count >= 3)
            {
                _disabledButton = allButtons.ElementAt(2);
            }

            _rainbowStyle = _window.Window.Resources.Values.OfType<Style>().FirstOrDefault();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Dispatcher.CurrentDispatcher.InvokeShutdown();
            _window.Dispose();
        }
        
        [MonitoredTest("Should not have changed the codebehind file")]
        public void _1_ShouldNotHaveChangedTheCodebehindFile()
        {
            var codeBehindFilePath = @"Exercise10\MainWindow.xaml.cs";

            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("65-ED-88-84-AC-80-3B-2E-BC-40-FF-21-96-C7-17-E7"), () =>
                $"The file '{codeBehindFilePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }
        
        [MonitoredTest("Should have 3 buttons")]
        public void _2_ShouldHaveThreeButtons()
        {
            AssertHasButtons();
        }

        [MonitoredTest("Should have a style resource that targets buttons")]
        public void _3_ShouldHaveAStyleResourceThatTargetsButtons()
        {
            AssertHasStyle();
        }

        [MonitoredTest("Should have a style resource that sets the foreground to a linear gradient brush")]
        public void _4_ShouldHaveAStyleResourceThatSetsTheForegroundToALinearGradientBrush()
        {
            AssertHasStyle();
            GetAndAssertForegroundSetter();
        }

        [MonitoredTest("Should have a style resource that sets the font weight and size")]
        public void _5_ShouldHaveAStyleResourceThatSetsTheFontWeightAndSize()
        {
            AssertHasStyle();

            var fontWeightSetter = _rainbowStyle.Setters.OfType<Setter>()
                .FirstOrDefault(s => s.Property.Name.ToLower() == "fontweight");
            Assert.That(fontWeightSetter, Is.Not.Null,
                "No 'Setter' that targets the 'FontWeight' property could be found.");
            Assert.That(fontWeightSetter.Value.ToString(), Is.EqualTo("Bold").IgnoreCase,
                "The 'Value' of the font weight 'Setter' should be 'Bold'.");

            var fontSizeSetter = _rainbowStyle.Setters.OfType<Setter>()
                .FirstOrDefault(s => s.Property.Name.ToLower() == "fontsize");
            Assert.That(fontSizeSetter, Is.Not.Null,
                "No 'Setter' that targets the 'FontSize' property could be found.");
            int.TryParse(fontSizeSetter.Value.ToString(), out int size);
            Assert.That(size, Is.GreaterThanOrEqualTo(20), "The 'Value' of the font size 'Setter' is not big enough.");
        }

        [MonitoredTest("Should have the linear gradient brush of the button style correctly defined")]
        public void _6_ShouldHaveTheLinearGradientBrushCorrectlyDefined()
        {
            AssertHasStyle();
            Setter foregroundSetter = GetAndAssertForegroundSetter();
            var brush = (LinearGradientBrush)foregroundSetter.Value;

            Assert.That(brush.StartPoint.Y, Is.LessThan(brush.EndPoint.Y),
                "Since the gradient should flow from the top to the button, the Y coordinate of the 'StartPoint' should be less than the Y coordinate of the 'EndPoint'.");
            Assert.That(brush.StartPoint.X, Is.EqualTo(brush.EndPoint.X),
                "Since the gradient should flow from the top to the button, the X coordinate of the 'StartPoint' and 'EndPoint' should be the same.");

            Assert.That(brush.GradientStops, Has.Count.EqualTo(7), "The gradient brush should have 7 gradient stops. One for each color of the rainbow.");

            Assert.That(brush.GradientStops.First().Offset, Is.Zero, "The 'Offset' of the first 'GradientStop' must be zero.");
            var usedColors = new List<Color>();
            double previousOffset = -0.15;
            foreach (GradientStop stop in brush.GradientStops)
            {
                Assert.That(usedColors, Does.Not.Contain(stop.Color),
                    $"The color {stop.Color} is used twice. Each 'GradientStop' should have a unique color.");
                usedColors.Add(stop.Color);
                Assert.That(stop.Offset, Is.GreaterThanOrEqualTo(0.0),
                    "The 'Offset' of each 'GradientStop' must be greater than or equal to zero.");
                Assert.That(stop.Offset, Is.LessThanOrEqualTo(1.0),
                    "The 'Offset' of each 'GradientStop' must be less than or equal to one.");
                Assert.That(stop.Offset, Is.EqualTo(previousOffset + 0.15).Within(0.001),
                    "Each 'GradientStop' should be 15% further than the previous 'GradientStop'.");
                previousOffset = stop.Offset;
            }
        }

        [MonitoredTest("Should use the defined style for the 3 buttons")]
        public void _7_ShouldUseTheDefinedStyleForTheButtons()
        {
            AssertHasButtons();
            AssertHasStyle();
            Assert.That(_rainbowButton.Style, Is.EqualTo(_rainbowStyle),
                () => "The 'Style' property of the top button is not set correctly.");
            Assert.That(_smallFontButton.Style, Is.EqualTo(_rainbowStyle),
                () => "The 'Style' property of the middle button is not set correctly.");
            Assert.That(_disabledButton.Style, Is.EqualTo(_rainbowStyle),
                () => "The 'Style' property of the bottom button is not set correctly.");
        }

        [MonitoredTest("Should have a button with a smaller fontsize in the middle")]
        public void _8_ShouldHaveAButtonWithASmallerFontsizeInTheMiddle()
        {
            AssertHasButtons();
            Assert.That(_smallFontButton.FontSize, Is.LessThan(20),
                "The 'FontSize' of the middle button should be smaller than 20.");
        }

        [MonitoredTest("Should have a disabled button at the bottom")]
        public void _9_ShouldHaveADisabledButtonAtTheBottom()
        {
            AssertHasButtons();
            Assert.That(_disabledButton.IsEnabled, Is.False);
        }

        private void AssertHasButtons()
        {
            Assert.That(_rainbowButton, Is.Not.Null, "The button on the top could not be found.");
            Assert.That(_smallFontButton, Is.Not.Null, "The button in the middle could not be found.");
            Assert.That(_disabledButton, Is.Not.Null, "The button at the bottom could not be found.");
        }

        private void AssertHasStyle()
        {
            Assert.That(_rainbowStyle, Is.Not.Null,
                () => "The 'Resources' collection of the window should contain an instance of 'Style'.");
            Assert.That(_rainbowStyle.TargetType.Name, Is.EqualTo("Button"),
                () => "A 'Style' instance was found but it does not target buttons ('TargetType').");
        }

        private Setter GetAndAssertForegroundSetter()
        {
            var foregroundSetter = _rainbowStyle.Setters.OfType<Setter>()
                .FirstOrDefault(s => s.Property.Name.ToLower() == "foreground");
            Assert.That(foregroundSetter, Is.Not.Null,
                () => "No 'Setter' that targets the 'Foreground' property could be found.");
            Assert.That(foregroundSetter.Value, Is.TypeOf<LinearGradientBrush>(),
                () => "The 'Value' of the 'Setter' should be an instance of 'LinearGradientBrush'.");
            return foregroundSetter;
        }
    }
}