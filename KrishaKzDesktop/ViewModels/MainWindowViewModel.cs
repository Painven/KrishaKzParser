using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        set
        {
            if (Set(ref currentPage, value) && ParsedAppartments.Any())
            {
                RaisePropertyChanged(nameof(ParsedFilteredAppartments));
            }
        }
    }

    bool isLivingRoomWithFurniture = true;
    public bool IsLivingRoomWithFurniture
    {
        get => isLivingRoomWithFurniture;
        set => Set(ref isLivingRoomWithFurniture, value);
    }

    const int ITEMS_PER_PAGE = 20;

    string selectedStoredProfile;
    public string SelectedStoredProfile
    {
        get => selectedStoredProfile;
        set
        {
            if (Set(ref selectedStoredProfile, value) && File.Exists(value))
            {
                manager = new ParserManager(value);
                LoadItems();
            }
        }
    }

    public ObservableCollection<string> StoredProfileFiles { get; set; } = new();
    public ObservableCollection<Appartment> ParsedAppartments { get; set; } = new();

    public IEnumerable<Appartment> ParsedFilteredAppartments
    {
        get => ParsedAppartments.Skip(ITEMS_PER_PAGE * (CurrentPage - 1)).Take(ITEMS_PER_PAGE);
    }

    private ParserManager manager;

    public ICommand StartParserCommand { get; }
    public ICommand MovePreviousCommand { get; }
    public ICommand MoveNextCommand { get; }

    public MainWindowViewModel()
    {
        StartParserCommand = new LambdaCommand(async e => await StartParser(), e => CurrentPage == 1);
        MovePreviousCommand = new LambdaCommand(e => CurrentPage--, e => CurrentPage > 1);
        MoveNextCommand = new LambdaCommand(e => CurrentPage++, e => CurrentPage < TotalPages);

        LoadStoredFilesList();
    }

    private async Task StartParser()
    {
        ParsedAppartments.Clear();

        try
        {
            var progress =
                new Progress<Tuple<int, int>>(v =>
                {
                    CurrentPage = v.Item1;
                    TotalPages = v.Item2;
                });

            string fileName = DateTime.Now.Date.ToString("yyyyMMdd") + $"_{minimumPrice / 1000}k-{maximumPrice / 1000}k.json";
            manager = new ParserManager(fileName);

            await manager.Parse(
                minPrice: MinimumPrice, maxPrice: MaximumPrice,
                minArea: MinimumArea, maxArea: MaximumArea,
                withFurniture: IsLivingRoomWithFurniture,
                roomsCount: LivingRoomsCount,
                progress);

            LoadStoredFilesList();
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

    private void LoadStoredFilesList()
    {
        StoredProfileFiles.Clear();

        var source = Directory.GetFiles(Path.GetDirectoryName(this.GetType().Assembly.Location), "*.json")
            .Select(f => new FileInfo(f))
            .Where(fi => Regex.IsMatch(fi.Name, @"^\d{8}_(.*)\.json$"))
            .OrderByDescending(fi => fi.CreationTime)
            .Select(fi => fi.Name)
            .ToArray();

        foreach (var file in source)
        {
            StoredProfileFiles.Add(file);
        }
        SelectedStoredProfile = StoredProfileFiles.FirstOrDefault();
    }

    private void LoadItems()
    {
        ParsedAppartments.Clear();

        foreach (var product in manager.appartments)
        {
            ParsedAppartments.Add(product);
        }

        TotalPages = (int)Math.Ceiling((double)ParsedAppartments.Count / ITEMS_PER_PAGE);
        CurrentPage = 1;
    }
}
