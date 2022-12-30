using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

    ParserManager manager = new ParserManager();

    public ICommand StartParserCommand { get; }

    public MainWindowViewModel()
    {
        StartParserCommand = new LambdaCommand(async e => await StartParser(), e => CurrentPage == 1);
    }

    private async Task StartParser()
    {
        await manager.Parse(new Progress<Tuple<int, int>>(v =>
        {
            CurrentPage = v.Item1;
            TotalPages = v.Item2;
        }));

        CurrentPage = 1;
    }
}
