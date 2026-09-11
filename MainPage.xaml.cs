namespace ProteinTracker
{
    public partial class MainPage : ContentPage
    {
        double proteinGoal = 0;
        double totalProtein = 0;

        List<FoodItem> foods = new List<FoodItem>();

        bool goalAlertShown = false;

        public MainPage()
        {
            InitializeComponent();

            LoadData();
            UpdateDisplay();
        }

        private void OnSaveGoalClicked(object? sender, EventArgs e)
        {
            if (double.TryParse(ProteinGoalEntry.Text, out double goal))
            {
                proteinGoal = goal;

                goalAlertShown = false;

                SaveData();
                UpdateDisplay();
            }
        }

        private void OnEditGoalClicked(object? sender, EventArgs e)
        {
            ProteinGoalEntry.Text = proteinGoal.ToString();
        }

        private void OnAddFoodClicked(object? sender, EventArgs e)
        {
            if (double.TryParse(ProteinAmountEntry.Text, out double protein))
            {
                FoodItem newFood = new FoodItem
                {
                    Name = FoodNameEntry.Text ?? "",
                    Protein = protein
                };

                foods.Add(newFood);

                totalProtein = totalProtein + protein;

                FoodNameEntry.Text = "";
                ProteinAmountEntry.Text = "";

                SaveData();
                UpdateDisplay();
            }
        }

        private void DeleteFood(FoodItem food)
        {
            foods.Remove(food);

            totalProtein = totalProtein - food.Protein;

            goalAlertShown = false;

            SaveData();
            UpdateDisplay();
        }

        private void EditFood(FoodItem food)
        {
            FoodNameEntry.Text = food.Name;
            ProteinAmountEntry.Text = food.Protein.ToString();

            foods.Remove(food);

            totalProtein = totalProtein - food.Protein;

            SaveData();
            UpdateDisplay();
        }

        private async void OnResetDayClicked(object? sender, EventArgs e)
        {
            bool reset = await DisplayAlert(
                "Tag zurücksetzen",
                "Möchtest du wirklich alle heutigen Einträge löschen?",
                "Ja",
                "Nein");

            if (reset)
            {
                foods.Clear();

                totalProtein = 0;

                goalAlertShown = false;

                SaveData();
                UpdateDisplay();
            }
        }

        private async void UpdateDisplay()
        {
            GoalLabel.Text =
                "Dein Tagesziel: " + proteinGoal + " g Protein";

            TotalProteinLabel.Text =
                "Aktuell: " + totalProtein + " / " + proteinGoal + " g Protein";

            
            if (foods.Count == 1)
            {
                FoodCountLabel.Text = "1 Lebensmittel eingetragen";
            }
            else
            {
                FoodCountLabel.Text =
                    foods.Count + " Lebensmittel eingetragen";
            }

            if (proteinGoal > 0)
            {
                double progress = totalProtein / proteinGoal;

                double percent = progress * 100;

                double progressForBar = progress;

                if (progressForBar > 1)
                {
                    progressForBar = 1;
                }

                ProteinProgressBar.Progress = progressForBar;

                double remaining = proteinGoal - totalProtein;

                if (remaining > 0)
                {
                    RemainingProteinLabel.Text =
                        Math.Round(percent)
                        + "% erreicht - noch "
                        + remaining
                        + " g bis zum Tagesziel";

                    goalAlertShown = false;
                }

                else if (remaining == 0)
                {
                    RemainingProteinLabel.Text =
                        "100% erreicht - Tagesziel genau erreicht!";

                    if (!goalAlertShown)
                    {
                        goalAlertShown = true;

                        await DisplayAlert(
                            "Tagesziel erreicht",
                            "Du hast dein Protein-Tagesziel erreicht!",
                            "OK");
                    }
                }

                else
                {
                    double overGoal = totalProtein - proteinGoal;

                    RemainingProteinLabel.Text =
                        Math.Round(percent)
                        + "% erreicht - "
                        + overGoal
                        + " g über dem Tagesziel";

                    if (!goalAlertShown)
                    {
                        goalAlertShown = true;

                        await DisplayAlert(
                            "Tagesziel erreicht",
                            "Du hast dein Protein-Tagesziel erreicht!",
                            "OK");
                    }
                }
            }
            else
            {
                ProteinProgressBar.Progress = 0;

                RemainingProteinLabel.Text =
                    "Noch kein Tagesziel festgelegt";
            }

            FoodList.Children.Clear();

            foreach (FoodItem food in foods)
            {
                HorizontalStackLayout foodRow =
                    new HorizontalStackLayout
                    {
                        Spacing = 15
                    };

                Label foodLabel =
                    new Label
                    {
                        Text =
                            food.Name
                            + " - "
                            + food.Protein
                            + " g Protein",

                        VerticalOptions =
                            LayoutOptions.Center,

                        HorizontalOptions =
                            LayoutOptions.StartAndExpand
                    };

                Button editButton =
                    new Button
                    {
                        Text = "Bearbeiten"
                    };

                editButton.Clicked += (sender, e) =>
                {
                    EditFood(food);
                };

                Button deleteButton =
                    new Button
                    {
                        Text = "Löschen"
                    };

                deleteButton.Clicked += (sender, e) =>
                {
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
            Preferences.Set(
                "ProteinGoal",
                proteinGoal);

            string foodText = "";

            foreach (FoodItem food in foods)
            {
                foodText =
                    foodText
                    + food.Name
                    + ";"
                    + food.Protein
                    + "|";
            }

            Preferences.Set(
                "Foods",
                foodText);
        }

        private void LoadData()
        {
            proteinGoal =
                Preferences.Get(
                    "ProteinGoal",
                    0.0);

            string foodText =
                Preferences.Get(
                    "Foods",
                    "");

            if (foodText != "")
            {
                string[] foodEntries =
                    foodText.Split('|');

                foreach (string entry in foodEntries)
                {
                    if (entry != "")
                    {
                        string[] values =
                            entry.Split(';');

                        FoodItem food =
                            new FoodItem
                            {
                                Name = values[0],
                                Protein =
                                    double.Parse(values[1])
                            };

                        foods.Add(food);

                        totalProtein =
                            totalProtein
                            + food.Protein;
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

