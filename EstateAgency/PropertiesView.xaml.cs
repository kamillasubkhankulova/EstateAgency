using EstateAgency.Models;
using EstateAgency.Services;
using RealEstateAgency.Enums;
using System.Windows;
using System.Windows.Controls;

namespace EstateAgency
{
    public partial class PropertiesView : UserControl
    {
        private readonly PropertyService _propertyService;

        /// <summary>
        /// Полный список из БД фильтрую
        /// </summary>
        private List<Property> _allProperties = new();

        public PropertiesView(PropertyService propertyService)
        {
            InitializeComponent();
            _propertyService = propertyService;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                _allProperties = await _propertyService.GetAllPropertiesAsync();
                PropertiesGrid.ItemsSource = _allProperties;
            }
            catch (Exception exception)
            {
                ExceptionHandler.Handle(exception, "Загрузка списка объектов");
            }
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<Property> query = _allProperties;

                string address = AddressFilter.Text.Trim();
                if (!string.IsNullOrWhiteSpace(address))
                {
                    query = query.Where(p => p.Address.ToString()
                        .Contains(address, StringComparison.OrdinalIgnoreCase));
                }

                PropertyType? type = GetSelectedType();
                if (type.HasValue)
                    query = query.Where(p => p.Type == type.Value);

                PropertiesGrid.ItemsSource = query.ToList();
            }
            catch (Exception exception)
            {
                ExceptionHandler.Handle(exception, "Фильтрация объектов");
            }
        }

        private async void OnResetClick(object sender, RoutedEventArgs e)
        {
            AddressFilter.Clear();
            TypeFilter.SelectedIndex = 0;
            await LoadDataAsync();
        }

        private PropertyType? GetSelectedType() => TypeFilter.SelectedIndex switch
        {
            1 => PropertyType.Apartment,
            2 => PropertyType.House,
            3 => PropertyType.Commercial,
            _ => null
        };

        /// <summary>
        /// Добавление объекта
        /// </summary>
        private async void OnAddClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new AddEditPropertyWindow(_propertyService, null)
                {
                    Owner = Window.GetWindow(this)
                };

                if (window.ShowDialog() == true)
                    await LoadDataAsync();
            }
            catch (Exception exception)
            {
                ExceptionHandler.Handle(exception, "Добавление объекта");
            }
        }

        /// <summary>
        /// Редактирование выбранного объекта
        /// </summary>
        private async void OnEditClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PropertiesGrid.SelectedItem is not Property selected)
                {
                    MessageBox.Show("Выберите объект в таблице.", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var window = new AddEditPropertyWindow(_propertyService, selected)
                {
                    Owner = Window.GetWindow(this)
                };

                if (window.ShowDialog() == true)
                    await LoadDataAsync();
            }
            catch (Exception exception)
            {
                ExceptionHandler.Handle(exception, "Редактирование объекта");
            }
        }

        /// <summary>
        /// Удаление выбранного объекта
        /// </summary>
        private async void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PropertiesGrid.SelectedItem is not Property selected)
                {
                    MessageBox.Show("Выберите объект в таблице.", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                MessageBoxResult confirm = MessageBox.Show(
                    $"Удалить объект «{selected.Address}»?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm != MessageBoxResult.Yes)
                    return;

                await _propertyService.DeletePropertyAsync(selected.Id);
                await LoadDataAsync();
            }
            catch (Exception exception)
            {
                ExceptionHandler.Handle(exception, "Удаление объекта");
            }
        }
    }
}