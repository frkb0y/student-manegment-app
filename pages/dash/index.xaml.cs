using plz_fix.pages.dash.toolspage;
using Microsoft.Data.SqlClient;
using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;
using plz_fix.Models;
using Microsoft.Maui.Controls; // Use this namespace for MAUI controls




namespace plz_fix.pages.dash;

public partial class index : ContentPage
{
    private string _username;
    public ICommand RefreshCommand { get; }

    public index(string username)
    {
        InitializeComponent();

        _username = username;
        UsernameLabel.Text = $"Hi, {_username}";

        RefreshCommand = new Command(LoadData);

        LoadData();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetNavBarIsVisible(this, false);
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private void OnPresenceTapped(object sender, EventArgs e)
    {
        MessagingCenter.Send(this, "GoToClassesTab");
    }

    private void OnCompletenessTapped(object sender, EventArgs e)
    {
        Console.WriteLine("Completeness card tapped");
    }

    private async void OnCalendarTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new plz_fix.pages.dash.toolspage.PopupImagePage());
    }




    private async void OnPaymentTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new pay());
    }

    private async void OnEvaluationTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new profilepages.note(_username));
    }


    private async void LoadData()
    {
        int total = PaymentData.GetTotalPayments();
        int missing = PaymentData.GetMissingPayments();
        PaymentLabel.Text = $"{missing} / {total}";

        int noteCount = profilepages.note.NoteData.GetNoteCount(_username);
        EvaluationLabel.Text = noteCount.ToString();

        await LoadTaskCompletenessAndTasks(_username);
    }

  

    private async void OnScheduleTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new plz_fix.pages.dash.toolspage.schedual(_username));
    }

    private async Task LoadTaskCompletenessAndTasks(string username)
    {
        string connectionString = "Data Source=DESKTOP-V0T4PRO\\USER19;Initial Catalog=XAF;User ID=sa;Password=123456789;TrustServerCertificate=True;Encrypt=True;";
        int completed = 0;
        int total = 0;
        List<TodoModel> tasks = new();

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // Get user's OID
                string getUserOidQuery = "SELECT OID FROM UserApp WHERE Username = @Username";
                int userOid = -1;

                using (SqlCommand cmd = new SqlCommand(getUserOidQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                        userOid = Convert.ToInt32(result);
                    else
                    {
                        CompletenessLabel.Text = "0 / 0";
                        return;
                    }
                }

                // Get task completion stats
                string query = "SELECT IsCompleted FROM Todo WHERE AssignedTo = @UserOid";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserOid", userOid);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            total++;
                            if (reader["IsCompleted"] != DBNull.Value && Convert.ToBoolean(reader["IsCompleted"]))
                                completed++;
                        }
                    }
                }

                CompletenessLabel.Text = $"{completed} / {total}";

                // Get tasks
                string getTasksQuery = @"
                    SELECT TOP 3 OID, WorkName, WorkDescription, StartDate 
                    FROM Todo 
                    WHERE AssignedTo = @UserOid 
                    ORDER BY StartDate ASC";

                using (SqlCommand cmd = new SqlCommand(getTasksQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserOid", userOid);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tasks.Add(new TodoModel
                            {
                                OID = Convert.ToInt32(reader["OID"]),
                                WorkName = reader["WorkName"].ToString(),
                                WorkDescription = reader["WorkDescription"].ToString(),
                                StartDate = Convert.ToDateTime(reader["StartDate"])
                            });
                        }
                    }
                }
            }

            // Display tasks in UI
            ToDoList.Children.Clear();
            foreach (var task in tasks)
            {
                var frame = new Frame
                {
                    BackgroundColor = Color.FromArgb("#FFFFFF"),
                    CornerRadius = 12,
                    HasShadow = true,
                    Padding = 15,
                    Margin = new Thickness(0, 0, 0, 10),
                    Content = new VerticalStackLayout
                    {
                        Spacing = 4,
                        Children =
                        {
                            new Label { Text = task.WorkName, FontSize = 16, FontAttributes = FontAttributes.Bold }, // Use Microsoft.Maui.Controls.Label
                            new Label { Text = task.WorkDescription, FontSize = 14, TextColor = Colors.Gray }, // Use Microsoft.Maui.Controls.Label
                            new Label { Text = $"Due: {task.StartDate:MMM dd, yyyy}", FontSize = 12, TextColor = Colors.DarkGray } // Use Microsoft.Maui.Controls.Label
                        }
                    }
                };

                ToDoList.Children.Add(frame);
            }

            if (tasks.Count == 0)
                await DisplayAlert("No Tasks", "You have no assigned tasks.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load tasks: {ex.Message}", "OK");
            CompletenessLabel.Text = "0 / 0";
        }
    }
}
