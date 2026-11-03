using System.Collections.ObjectModel;
using System.Text.Json;

namespace QuestLog;

public partial class MainPage : ContentPage
{
    public ObservableCollection<Quest> Quests { get; } = new();

    public Quest? SelectedQuest { get; set; }

    string path = Path.Combine(FileSystem.AppDataDirectory, "quests.json");

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
        Load();
        UpdateCount();
    }

    async void OnAddClicked(object sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim() ?? "";
        string giver = GiverEntry.Text?.Trim() ?? "";

        if (title == "")
        {
            await DisplayAlertAsync("Missing title", "Please enter a quest title.", "OK");
            return;
        }

        if (!int.TryParse(RewardEntry.Text, out int reward) || reward < 0)
        {
            await DisplayAlertAsync("Invalid reward",
                "Reward must be a whole number, 0 or higher.", "OK");
            return;
        }

        Quests.Add(new Quest { Title = title, Giver = giver, Reward = reward });

        TitleEntry.Text = "";
        GiverEntry.Text = "";
        RewardEntry.Text = "";

        UpdateCount();
        Save();
    }

    async void OnRemoveClicked(object sender, EventArgs e)
    {
        if (SelectedQuest == null)
        {
            await DisplayAlertAsync("Nothing selected", "Tap a quest in the list first.", "OK");
            return;
        }

        Quests.Remove(SelectedQuest);
        SelectedQuest = null;

        UpdateCount();
        Save();
    }

    void UpdateCount()
    {
        CountLabel.Text = $"{Quests.Count} quests";
    }

    void Save()
    {
        string json = JsonSerializer.Serialize(Quests);
        File.WriteAllText(path, json);
    }

    void Load()
    {
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        List<Quest>? saved = JsonSerializer.Deserialize<List<Quest>>(json);
        if (saved == null) return;

        foreach (Quest q in saved)
            Quests.Add(q);
    }
}