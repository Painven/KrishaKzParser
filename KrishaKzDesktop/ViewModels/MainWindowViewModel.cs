using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static KrishaKzDesktop.KrishaParser;

namespace KrishaKzDesktop.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    int minimumPrice = 120000;
    public int MinimumPrice
    {
        get => minimumPrice;
        set => Set(ref minimumPrice, value);
    }

    int maximumPrice = 180000;
    public int MaximumPrice
    {
        get => maximumPrice;
        set => Set(ref maximumPrice, value);
    }

    int minimumArea = 22;
    public int MinimumArea
    {
        get => minimumArea;
        set => Set(ref minimumArea, value);
    }

    int maximumArea = 35;
    public int MaximumArea
    {
        get => maximumArea;
        set => Set(ref maximumArea, value);
    }

    int livingRoomsCount = 1;
    public int LivingRoomsCount
    {
        get => livingRoomsCount;
        set => Set(ref livingRoomsCount, value);
    }

    int totalPages = 1;
    public int TotalPages
    {
        get => totalPages;
        set => Set(ref totalPages, value);
    }

    int currentPage = 1;
    public int CurrentPage
    {
        get => currentPage;
        set => Set(ref currentPage, value);
    }

    bool isLivingRoomWithFurniture = true;
    public bool IsLivingRoomWithFurniture
    {
        get => isLivingRoomWithFurniture;
        set => Set(ref isLivingRoomWithFurniture, value);
    }

    int selectedItemPage = 1;
    public int SelectedItemPage
    {
        get => selectedItemPage;
        set
        {
            if (Set(ref selectedItemPage, value))
            {
                RaisePropertyChanged(nameof(ParsedFilteredAppartments));
            }
        }
    }

    public int ItemsTotalPages
    {
        get => (int)Math.Ceiling((double)ParsedAppartments.Count / ITEMS_PER_PAGE);
    }

    const int ITEMS_PER_PAGE = 20;

    public ObservableCollection<Appartment> ParsedAppartments { get; set; } = new();
    public IEnumerable<Appartment> ParsedFilteredAppartments
    {
        get => ParsedAppartments.Skip(ITEMS_PER_PAGE * (SelectedItemPage - 1)).Take(ITEMS_PER_PAGE);
    }

    private readonly ParserManager manager;

    public ICommand StartParserCommand { get; }
    public ICommand MovePreviousCommand { get; }
    public ICommand MoveNextCommand { get; }

    public MainWindowViewModel()
    {
        StartParserCommand = new LambdaCommand(async e => await StartParser(), e => CurrentPage == 1);
        MovePreviousCommand = new LambdaCommand(e => SelectedItemPage--, e => SelectedItemPage > 1);
        MoveNextCommand = new LambdaCommand(e => SelectedItemPage++, e => SelectedItemPage < ItemsTotalPages - 1);
        manager = new ParserManager();

        LoadItems();

        ParsedAppartments.CollectionChanged += (o, e) => RaisePropertyChanged(nameof(ItemsTotalPages));
        RaisePropertyChanged(nameof(ItemsTotalPages));
    }

    private async Task StartParser()
    {
        try
        {
            var progress =
                new Progress<Tuple<int, int>>(v =>
                {
                    CurrentPage = v.Item1;
                    TotalPages = v.Item2;
                });

            await manager.Parse(
                minPrice: MinimumPrice, maxPrice: MaximumPrice,
                minArea: MinimumArea, maxArea: MaximumArea,
                withFurniture: IsLivingRoomWithFurniture,
                roomsCount: LivingRoomsCount,
                progress);

            LoadItems();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка\r\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            CurrentPage = 1;
        }

    }

    private void LoadItems()
    {
        foreach (var product in manager.appartments)
        {
            ParsedAppartments.Add(product);
        }
    }
}
