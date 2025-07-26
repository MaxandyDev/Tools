using Noteblock.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;       
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;



namespace Noteblock
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<object> Items { get; set; } = new();

        private static readonly string NotesFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Noteblock");

        private static readonly string NotesFilePath = Path.Combine(NotesFolderPath, "notes.json");



        public MainWindow()
        {
            InitializeComponent();
            this.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/SchriftrolleIco.ico"));
            LoadNotesFromFile();
            NotesList.ItemsSource = Items;
            AddAddNoteButton();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }


        private void AddAddNoteButton()
        {
            var existing = Items.OfType<AddNoteButtonMarker>().FirstOrDefault();
            if (existing != null)
                Items.Remove(existing);

            Items.Add(new AddNoteButtonMarker());
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            Items.Insert(Items.Count - 1, new NoteItem { Text = "Neue Notiz", Checked = false });
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is NoteItem note)
            {
                Items.Remove(note);
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            SaveNotesToFile();
            this.Close();
        }

        private void SaveNotesToFile()
        {
            var notesToSave = Items.OfType<NoteItem>().ToList();

            // Ordner erstellen, falls er nicht existiert
            if (!Directory.Exists(NotesFolderPath))
                Directory.CreateDirectory(NotesFolderPath);

            var json = JsonSerializer.Serialize(notesToSave, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(NotesFilePath, json);
        }


        private void LoadNotesFromFile()
        {
            if (!File.Exists(NotesFilePath))
                return;

            var json = File.ReadAllText(NotesFilePath);
            var loadedNotes = JsonSerializer.Deserialize<List<NoteItem>>(json);

            if (loadedNotes != null)
            {
                foreach (var note in loadedNotes)
                    Items.Add(note);
            }
        }


    }
}
