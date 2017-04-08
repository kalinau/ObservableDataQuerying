using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ObservableData.Tests.Core;
using ObservableData.Structures;

namespace ObservableData.Tests.Visual
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StringBuilder _stringBuilder = new StringBuilder();
        private readonly ObservableCollection<TestEntity> _source = new ObservableCollection<TestEntity>();
        private int _index;
        private TestEntity _selected;

        public MainWindow()
        {
            InitializeComponent();
            ListView.ItemsSource = _source;
            var s = new ObservableList<int>();
        }

        private static bool TryGetInt(TextBox textBox, out int value)
        {
            if (int.TryParse(textBox.Text, out value))
            {
                return true;
            }
            MessageBox.Show("invalid");
            return false;
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            if (TryGetInt(AddValue, out var value))
            {
                _source.Add(new TestEntity(value, _index++));
            }
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            if (TryGetInt(SelectedValue, out var value))
            {
                _selected?.ChangeValue(value);
            }
            _selected = null;
            this.SelectedForm.Visibility = Visibility.Collapsed;
        }

        private void OnSelected(object sender, RoutedEventArgs e)
        {
            _selected = this.ListView?.SelectedItem as TestEntity;
            if (_selected == null) return;
            this.SelectedValue.Text = _selected.Value.ToString();
            this.SelectedIndex.Text = _selected.Index.ToString();
            this.SelectedForm.Visibility = Visibility.Visible;
        }
    }
}
