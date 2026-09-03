namespace ProteinTracker
{
    public partial class MainPage : ContentPage
    {
        double proteinGoal = 0;
        double totalProtein = 0;

        List<FoodItem> foods = new List<FoodItem>();

        public MainPage()
        {
            InitializeComponent();

            LoadData();
            UpdateDisplay();
        }

        private void OnSaveGoalClicked(object? sender, EventArgs e)
        {
            proteinGoal = double.Parse(ProteinGoalEntry.Text);

            GoalLabel.Text = "Dein Tagesziel: " + proteinGoal + " g Protein";

            SaveData();
            UpdateDisplay();
        }

        private void OnAddFoodClicked(object? sender, EventArgs e)
        {
            double protein = double.Parse(ProteinAmountEntry.Text);

            FoodItem newFood = new FoodItem
            {
                Name = FoodNameEntry.Text,
                Protein = protein
            };

            foods.Add(newFood);

            totalProtein = totalProtein + protein;


            FoodNameEntry.Text = "";
            ProteinAmountEntry.Text = "";

            SaveData();
            UpdateDisplay();
        }

        private void DeleteFood(FoodItem food)
        {
            foods.Remove(food);

            totalProtein = totalProtein - food.Protein;

            SaveData();
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            GoalLabel.Text = "Dein Tagesziel: " + proteinGoal + " g Protein";

            TotalProteinLabel.Text =
                "Aktuell: " + totalProtein + " / " + proteinGoal + " g Protein";

            if (proteinGoal > 0)
            {
                double progress = totalProtein / proteinGoal;

                if (progress > 1)
                {
                    progress = 1;
                }

                ProteinProgressBar.Progress = progress;

                double remaining = proteinGoal - totalProtein;

                if (remaining > 0)
                {
                    RemainingProteinLabel.Text =
                        "Noch " + remaining + " g bis zum Tagesziel";
                }
                else
                {
                    RemainingProteinLabel.Text =
                        "Tagesziel erreicht!";
                }
            }

            FoodList.Children.Clear();

            foreach (FoodItem food in foods)
            {
                HorizontalStackLayout foodRow = new HorizontalStackLayout
                {
                    Spacing = 15
                };

                Label foodLabel = new Label
                {
                    Text = food.Name + " - " + food.Protein + " g Protein",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };

                Button deleteButton = new Button
                {
                    Text = "Löschen"
                };

                Button editButton = new Button
                {
                    Text = "Bearbeiten"
                };


                deleteButton.Clicked += (sender, e) =>
                {
                    DeleteFood(food);
                };

                editButton.Clicked += (sender, e) =>
                {

                    FoodNameEntry.Text = food.Name;
                    ProteinAmountEntry.Text = food.Protein.ToString();
                    DeleteFood(food);
                };

                foodRow.Children.Add(foodLabel);
                foodRow.Children.Add(editButton);
                foodRow.Children.Add(deleteButton);

                FoodList.Children.Add(foodRow);
            }
        }

        private void SaveData()
        {
            Preferences.Set("ProteinGoal", proteinGoal);

            string foodText = "";

            foreach (FoodItem food in foods)
            {
                foodText = foodText
                    + food.Name
                    + ";"
                    + food.Protein
                    + "|";
            }

            Preferences.Set("Foods", foodText);
        }

        private void LoadData()
        {
            proteinGoal = Preferences.Get("ProteinGoal", 0.0);

            string foodText = Preferences.Get("Foods", "");

            if (foodText != "")
            {
                string[] foodEntries = foodText.Split('|');

                foreach (string entry in foodEntries)
                {
                    if (entry != "")
                    {
                        string[] values = entry.Split(';');

                        FoodItem food = new FoodItem
                        {
                            Name = values[0],
                            Protein = double.Parse(values[1])
                        };

                        foods.Add(food);

                        totalProtein = totalProtein + food.Protein;
                    }
                }
            }
        }
    }

    public class FoodItem
    {
        public string Name { get; set; } = "";
        public double Protein { get; set; }
    }
}

