using EstateAgency.Models;
using EstateAgency.Services;
using EstateAgency.States;
using RealEstateAgency.Enums;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace EstateAgency
{
    public partial class AddEditPropertyWindow : Window
    {
        private readonly PropertyService _propertyService;
        private readonly Property? _property;

        private bool IsEdit => _property is not null;

        public AddEditPropertyWindow(PropertyService propertyService, Property? property)
        {
            InitializeComponent();

            _propertyService = propertyService;
            _property = property;

            Title = IsEdit ? "Редактирование объекта" : "Новый объект недвижимости";
            Loaded += OnWindowLoaded;
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadRealtorsAsync();
                LoadStatuses();
                FillFields();
            }
            catch (Exception exception)
            {
                ExceptionHandler.Handle(exception, "Подготовка формы объекта");
            }
        }

        private async Task LoadRealtorsAsync()
        {
            List<Realtor> realtors = await _propertyService.GetRealtorsAsync();

            RealtorCombo.ItemsSource = realtors;
            RealtorCombo.DisplayMemberPath = "FullName";
            RealtorCombo.SelectedValuePath = "Id";

            if (_property is not null)
                RealtorCombo.SelectedValue = _property.RealtorId;
            else if (realtors.Count > 0)
                RealtorCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// Заполняет выпадающий список статусов
        /// </summary>
        private void LoadStatuses()
        {
            StatusCombo.Items.Clear();

            if (_property is null)
            {
                AddStatusItem(PropertyStatus.Free);
                StatusCombo.SelectedIndex = 0;
                StatusCombo.IsEnabled = false;
                return;
            }

            var available = new List<PropertyStatus> { _property.Status };
            available.AddRange(_property.AvailableTransitions);

            foreach (PropertyStatus status in available.Distinct())
                AddStatusItem(status);

            StatusCombo.SelectedIndex = 0;
        }

        private void AddStatusItem(PropertyStatus status)
        {
            StatusCombo.Items.Add(new ComboBoxItem
            {
                Content = PropertyStates.Resolve(status).Title,
                Tag = status
            });
        }

        private void FillFields()
        {
            if (_property is null)
            {
                TypeCombo.SelectedIndex = 0;
                return;
            }

            CityBox.Text = _property.Address.City;
            StreetBox.Text = _property.Address.Street;
            HouseBox.Text = _property.Address.House;
            ApartmentBox.Text = _property.Address.Apartment;

            TypeCombo.SelectedIndex = _property.Type switch
            {
                PropertyType.House => 1,
                PropertyType.Commercial => 2,
                _ => 0
            };

            AreaBox.Text = _property.Area.ToString("N1");
            RoomsBox.Text = _property.Rooms.ToString();
            FloorBox.Text = _property.Floor.ToString();
            PriceBox.Text = _property.Price.ToString("N0");
        }

        private async void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (!TryBuildProperty(out Property? draft, out string error))
            {
                MessageBox.Show(error, "Проверьте данные",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveButton.IsEnabled = false;

            try
            {
                if (IsEdit)
                {
                    draft!.Id = _property!.Id;
                    await _propertyService.UpdatePropertyAsync(draft);
                }
                else
                {
                    await _propertyService.AddPropertyAsync(draft!);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception exception)
                when (exception is ArgumentException or InvalidOperationException or KeyNotFoundException)
            {
                MessageBox.Show(exception.Message, "Не удалось сохранить",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception exception)
            {
                ExceptionHandler.Handle(exception, "Сохранение объекта");
            }
            finally
            {
                SaveButton.IsEnabled = true;
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Собирает объект из полей формы и проверяет ввод
        /// </summary>
        private bool TryBuildProperty(out Property? property, out string error)
        {
            property = null;
            error = string.Empty;

            string city = CityBox.Text.Trim();
            string street = StreetBox.Text.Trim();
            string house = HouseBox.Text.Trim();
            string apartment = ApartmentBox.Text.Trim();

            if (city.Length == 0 || street.Length == 0 || house.Length == 0)
            {
                error = "Заполните город, улицу и дом.";
                return false;
            }

            if (RealtorCombo.SelectedItem is not Realtor realtor)
            {
                error = "Выберите риелтора.";
                return false;
            }

            if (!TryParseDouble(AreaBox.Text, out double area) || area <= 0)
            {
                error = "Площадь должна быть числом больше нуля.";
                return false;
            }

            if (!int.TryParse(RoomsBox.Text.Trim(), out int rooms) || rooms <= 0)
            {
                error = "Количество комнат должно быть целым числом больше нуля.";
                return false;
            }

            if (!int.TryParse(FloorBox.Text.Trim(), out int floor) || floor < 0)
            {
                error = "Этаж должен быть целым неотрицательным числом.";
                return false;
            }

            if (!TryParseDecimal(PriceBox.Text, out decimal price) || price <= 0)
            {
                error = "Цена должна быть числом больше нуля.";
                return false;
            }

            PropertyType type = TypeCombo.SelectedIndex switch
            {
                1 => PropertyType.House,
                2 => PropertyType.Commercial,
                _ => PropertyType.Apartment
            };

            PropertyStatus status = _property is null
                ? PropertyStatus.Free
                : GetSelectedStatus();

            property = new Property
            {
                Address = new Address
                {
                    City = city,
                    Street = street,
                    House = house,
                    Apartment = apartment
                },
                Type = type,
                Area = area,
                Rooms = rooms,
                Floor = floor,
                Price = price,
                Status = status,
                Realtor = realtor,
                RealtorId = realtor.Id
            };

            return true;
        }

        private PropertyStatus GetSelectedStatus() =>
            (StatusCombo.SelectedItem as ComboBoxItem)?.Tag is PropertyStatus status
                ? status
                : _property!.Status;

        private static bool TryParseDouble(string text, out double value)
        {
            string normalized = text.Trim().Replace(" ", string.Empty).Replace(',', '.');
            return double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryParseDecimal(string text, out decimal value)
        {
            string normalized = text.Trim().Replace(" ", string.Empty).Replace(',', '.');
            return decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }
    }
}
