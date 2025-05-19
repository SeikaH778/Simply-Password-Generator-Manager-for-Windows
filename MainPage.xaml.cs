
using TestApp2.Models;
namespace TestApp2
{
    public partial class MainPage : ContentPage
    {
        
        public MainPage()
        {
            InitializeComponent();
            DbInitial();
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            passwordLenghtLabel.Text = Convert.ToInt32(e.NewValue).ToString();
        }
        private string GeneratePassword(int length)
        {
            const string chars = "qwertyuiopasdfghjklzxcvbnm123456789!#-+*";
            Random rand = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        private void GeneratePasswordButtonClick(object sender, EventArgs e)
        {
            ReadyPass.Text = GeneratePassword(Convert.ToInt32(passwordLenghtLabel.Text));

        }

        private void DbInitial()
        {
            string appDataPath = FileSystem.AppDataDirectory;
            string folderPath = Path.Combine(appDataPath, "pass");
            string filePath = Path.Combine(folderPath, "passwords.db");
            Directory.CreateDirectory(folderPath);

            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(appDataPath + @"\pass");
                using (var db = new Db_context())
                {
                    db.Database.EnsureCreated();
                }
            }
            else
            {
                using (var db = new Db_context())
                {
                    var noteList = db.Notes.ToList();
                    foreach (var note in noteList)
                    {
                        SavedPasswordsPicker.Items.Add(note.Reason);
                    }
                   
                }
            }

        }


        private void SavePasswordButtonClick(object sender, EventArgs e)
        {
            using (var db = new Db_context())
            {
                if (IsPasswordExists() == false)
                {
                    db.Add(new Note { Password = $"{ReadyPass.Text}", Reason = $"{passReason.Text}" });
                    db.SaveChanges();
                    SavedPasswordsPicker.Items.Add(passReason.Text);
                }
                else
                {
                    DisplayAlert("","Пароль уже сохранен","OK");
                }
            }
        }
        private bool IsPasswordExists()
        {
            using (var db = new Db_context())
            {  
               var noteList = db.Notes
               .Where(n => n.Password.Contains(ReadyPass.Text) || n.Reason.Contains(passReason.Text))
               .OrderByDescending(n => n.Reason)
                .ToList();
                if (noteList.Count>0)
                {
                    return true;
                }
                else { return false; }
                    
            }
        }

        private void SavedPasswordsPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (var db = new Db_context())
            {
                try
                {
                    var noteList = db.Notes
                   .Where(n => n.Reason.Contains(SavedPasswordsPicker.SelectedItem.ToString()))
                   .OrderByDescending(n => n.Reason)
                    .ToList();
                    DisplayAlert("", $"Ваш пароль: {noteList[0].Password} \n от {noteList[0].Reason}", "Ok");
                }
                catch (Exception ex) { }
            }
            
        }

        private void ClearDBButtonClick(object sender, EventArgs e)
        {
            var items = SavedPasswordsPicker.Items.ToList();

            foreach (var item in items)
            {
                SavedPasswordsPicker.Items.Remove(item);
            }
            using (var db = new Db_context())
            {
                
                db.Notes.RemoveRange(db.Notes);
                DisplayAlert("", $"БД очищена", "Ok");
                db.SaveChanges();
                
            }
        }
    }
}
