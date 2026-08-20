namespace ProteinTracker
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnSaveGoalClicked(object? sender, EventArgs e)
        {
            GoalLabel.Text = "Dein Tagesziel: " + ProteinGoalEntry.Text + " g Protein";
        }
    }
}