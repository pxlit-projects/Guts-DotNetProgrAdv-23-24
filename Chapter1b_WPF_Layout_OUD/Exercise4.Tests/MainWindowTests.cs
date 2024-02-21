using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Guts.Client.WPF.TestTools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Exercise4.Tests
{
    [ExerciseTestFixture("progAdvNet", "H01b", "Exercise04", @"Exercise4\MainWindow.xaml")]
    [Apartment(ApartmentState.STA)]
    public class MainWindowTests
    {
        private TestWindow<MainWindow> _window;
        private Grid _grid;
        private IList<Label> _labels;
        private IList<GridSplitter> _gridSplitters;


        [OneTimeSetUp]
        public void Setup()
        {
            _window = new TestWindow<MainWindow>();
            _grid = _window.GetUIElements<Grid>().FirstOrDefault();
            _labels = _window.GetUIElements<Label>().ToList();
            _gridSplitters = _window.GetUIElements<GridSplitter>().ToList();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _window.Dispose();
        }

        [MonitoredTest("Should not have changed the codebehind file")]
        public void _01_ShouldNotHaveChangedTheCodebehindFile()
        {
            var codeBehindFilePath = @"Exercise4\MainWindow.xaml.cs";
            var fileHash = Solution.Current.GetFileHash(codeBehindFilePath);
            Assert.That(fileHash, Is.EqualTo("0C-A5-78-B5-C3-D5-D9-21-F6-7C-70-1E-24-F2-89-21"),
                $"The file '{codeBehindFilePath}' has changed. " +
                "Undo your changes on the file to make this test pass. " +
                "This exercise can be completed by purely working with XAML.");
        }


        [MonitoredTest("Grid should have 5 columns")]
        public void _02_GridShouldHaveFiveColumns()
        {
            Assert.That(_grid.ColumnDefinitions.Count, Is.EqualTo(5), "There have to be 5 columns (3 labels and 2 grid splitters).");
        }

        [MonitoredTest("Should have 3 labels with the correct color")]
        public void _03_ShouldHaveThreeLabelsWithTheCorrectColor()
        {
            Assert.That(_labels.Count, Is.EqualTo(3), () => "The grid must contain 3 labels");
            Assert.That(_labels.All(l => l.Parent == _grid), Is.True, "All labels must be direct children of the grid");
            Assert.That(_labels[0].Background == Brushes.Black, Is.True, "The first label should be black");
            Assert.That(_labels[1].Background == Brushes.Yellow, Is.True, "The second label should be yellow");
            Assert.That(_labels[2].Background == Brushes.Red, Is.True, "The third label should be red");
        }

        [MonitoredTest("Labels should be in the correct grid column")]
        public void _04_ShouldHaveLabelsInTheCorrectGridColumn()
        {
            Assert.That(_labels.Count, Is.EqualTo(3), () => "The grid must contain 3 labels");
            Assert.That(_labels[0].GetValue(Grid.ColumnProperty), Is.EqualTo(0), () => "The first label should be in the first column");
            Assert.That(_labels[1].GetValue(Grid.ColumnProperty), Is.EqualTo(2), () => "The second label should be in the third column");
            Assert.That(_labels[2].GetValue(Grid.ColumnProperty), Is.EqualTo(4), () => "The third label should be in the last column");
        }

        [MonitoredTest("There have to be 2 GridSplitters on the left and the right of the middle column")]
        public void _05_ShouldHaveTwoGridSplittersLeftAndRightOfTheMiddleColumn()
        {
            Assert.That(_gridSplitters.Count, Is.EqualTo(2), () => "There should be two GridSplitters.");

            Assert.That(_gridSplitters,
                Has.One.Matches((GridSplitter splitter) => (int)splitter.GetValue(Grid.ColumnProperty) == 1),
                () => "There should be one GridSplitter at the left of the middle column");

            Assert.That(_gridSplitters,
                Has.One.Matches((GridSplitter splitter) => (int)splitter.GetValue(Grid.ColumnProperty) == 3),
                () => "There should be one GridSplitter at the right of the middle column");
        }

        [MonitoredTest("Should have horizontal and vertical alignment set correctly for the splitters")]
        public void _06_ShouldHaveHorizontalAndVerticalAlignmentSetCorrectlyForTheSplitters()
        {
            Assert.That(_gridSplitters.Count, Is.EqualTo(2), () => "There should be two GridSplitters.");

            Assert.That(_gridSplitters,
                Has.All.Matches((GridSplitter splitter) => splitter.HorizontalAlignment == HorizontalAlignment.Center),
                "Not all splitters are horizontally centered");

            Assert.That(_gridSplitters,
                Has.All.Matches((GridSplitter splitter) => splitter.VerticalAlignment == VerticalAlignment.Stretch),
                "Not all splitters are vertically stretched");
        }

        [MonitoredTest("Should have the window as the scope for size sharing")]
        public void _07_ShouldHaveTheWindowAsTheScopeForSizeSharing()
        {
            Assert.That(_window.Window.GetValue(Grid.IsSharedSizeScopeProperty), Is.True,
                "The property 'IsSharedSizeScope' of 'Grid' must be true for 'Window'. " +
                "See https://docs.microsoft.com/dotnet/api/system.windows.controls.grid.issharedsizescope");
        }

        [MonitoredTest("The first and the last column have to share the same size group")]
        public void _08_ShouldHaveTheFirstAndTheLastColumnInTheSameSizeGroup()
        {
            _02_GridShouldHaveFiveColumns();

            Assert.That(_grid.ColumnDefinitions.First().SharedSizeGroup, Is.Not.Null.Or.Empty,
                "There first column should have a 'SharedSizeGroup' defined. " +
                "See https://docs.microsoft.com/dotnet/desktop/wpf/controls/how-to-share-sizing-properties-between-grids?view=netframeworkdesktop-4.8");
            Assert.That(_grid.ColumnDefinitions.Last().SharedSizeGroup, Is.Not.Null.Or.Empty,
                "There last column should have a 'SharedSizeGroup' defined. " +
                "See https://docs.microsoft.com/dotnet/desktop/wpf/controls/how-to-share-sizing-properties-between-grids?view=netframeworkdesktop-4.8");
            Assert.That(_grid.ColumnDefinitions.First().SharedSizeGroup,
                Is.EqualTo(_grid.ColumnDefinitions.Last().SharedSizeGroup),
                "The first and the last column have to share the same Size Group.");
        }

        [MonitoredTest("Should have auto width colums except for the middle column")]
        public void _09_ShouldHaveAutomWidthColumnsExceptForTheMiddleColumn()
        {
            _02_GridShouldHaveFiveColumns();

            var columns = new List<ColumnDefinition>(_grid.ColumnDefinitions);
            if (columns.Count > 2)
            {
                columns.RemoveAt(2);
            }

            Assert.That(columns,
                Has.All.Matches((ColumnDefinition column) => HasAutoColumnWidth(column)),
                () => "All (except the middle) 'ColumnDefinition' elements in the XAML should have auto Width. " +
                      "E.g. 'Width=\"auto\"'");
        }

        private bool HasAutoColumnWidth(ColumnDefinition columnDefinition)
        {
            return columnDefinition.Width == GridLength.Auto;
        }
    }
}
