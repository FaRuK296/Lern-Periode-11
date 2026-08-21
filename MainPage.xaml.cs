namespace ProteinTracker
{
    public partial class MainPage : ContentPage
    {

        double proteinGoal = 0;
        double totalProtein = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnSaveGoalClicked(object? sender, EventArgs e)
        {
            proteinGoal = double.Parse(ProteinGoalEntry.Text);

            GoalLabel.Text = "Dein Tagesziel: " + proteinGoal + " g Protein";
        }

        private void OnAddFoodClicked(object? sender, EventArgs e)
        {
            double protein = double.Parse(ProteinAmountEntry.Text);

            totalProtein = totalProtein + protein;

            TotalProteinLabel.Text =
                "Aktuell: " + totalProtein + " / " + proteinGoal + " g Protein";

            Label foodLabel = new Label();

            foodLabel.Text =
                FoodNameEntry.Text + " - " + ProteinAmountEntry.Text + " g Protein";

            FoodList.Children.Add(foodLabel);
        }
    }
}

