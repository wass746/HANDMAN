using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
 

namespace HANDMAN
{
    public partial class MainWindow : Window
    {
        private class WordEntry
        {
            public string Word { get; set; }
            public string Hint { get; set; }

            public WordEntry(string word, string hint)
            {
                Word = word;
                Hint = hint;
            }
        }

        private readonly Random random = new Random();

        private readonly List<WordEntry> words = new List<WordEntry>()
        {
            new WordEntry(
                "MOTOCROSS",
                "Une discipline pratiquée sur un circuit en terre."),

            new WordEntry(
                "YAMAHA",
                "Le constructeur japonais des motos YZ."),

            new WordEntry(
                "CASQUE",
                "Une protection indispensable pour la tête."),

            new WordEntry(
                "PISTON",
                "Une pièce qui se déplace dans le moteur."),

            new WordEntry(
                "CIRCUIT",
                "Le terrain sur lequel roule le pilote."),

            new WordEntry(
                "GUIDON",
                "Il permet de diriger la moto."),

            new WordEntry(
                "PILOTE",
                "La personne qui conduit la moto."),

            new WordEntry(
                "MOTEUR",
                "Il produit la puissance de la moto."),

            new WordEntry(
                "VITESSE",
                "Le pilote la change avec le sélecteur."),

            new WordEntry(
                "SUSPENSION",
                "Elle absorbe les bosses et les sauts."),

            new WordEntry(
                "DEPART",
                "Le moment où la grille tombe."),

            new WordEntry(
                "PNEU",
                "Il assure l'adhérence sur la terre."),

            new WordEntry(
                "ENDURO",
                "Une discipline de moto tout-terrain."),

            new WordEntry(
                "FREIN",
                "Il permet de ralentir la moto.")
        };

        private readonly HashSet<char> guessedLetters =
            new HashSet<char>();

        private string secretWord = "";

        private int errors = 0;

        private const int MaxErrors = 6;

        private bool gameFinished = false;

        public MainWindow()
        {
            InitializeComponent();

            CreateKeyboard();
            StartNewGame();
        }

        private void CreateKeyboard()
        {
            KeyboardPanel.Children.Clear();

            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                Button button = new Button();

                button.Content = letter.ToString();
                button.Tag = letter;
                button.Style =
                    (Style)FindResource("LetterButtonStyle");

                button.Click += LetterButton_Click;

                KeyboardPanel.Children.Add(button);
            }
        }

        private void StartNewGame()
        {
            int randomIndex = random.Next(words.Count);

            WordEntry selectedEntry = words[randomIndex];

            secretWord = selectedEntry.Word.ToUpper();

            HintText.Text = selectedEntry.Hint;

            guessedLetters.Clear();

            errors = 0;
            gameFinished = false;

            MessageText.Text = "";
            MessageText.Foreground =
                new SolidColorBrush(Color.FromRgb(80, 164, 255));

            foreach (object element in KeyboardPanel.Children)
            {
                Button button = element as Button;

                if (button != null)
                {
                    button.IsEnabled = true;
                    button.Opacity = 1;
                }
            }

            UpdateWordDisplay();
            UpdateErrorDisplay();
            DrawHangman();
        }

        private void LetterButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (gameFinished)
            {
                return;
            }

            Button button = sender as Button;

            if (button == null)
            {
                return;
            }

            char selectedLetter = (char)button.Tag;

            button.IsEnabled = false;
            button.Opacity = 0.4;

            guessedLetters.Add(selectedLetter);

            if (secretWord.IndexOf(selectedLetter) == -1)
            {
                errors++;
            }

            UpdateWordDisplay();
            UpdateErrorDisplay();
            DrawHangman();
            CheckGameResult();
        }

        private void UpdateWordDisplay()
        {
            string displayedWord = "";

            foreach (char letter in secretWord)
            {
                if (guessedLetters.Contains(letter))
                {
                    displayedWord += letter + " ";
                }
                else
                {
                    displayedWord += "_ ";
                }
            }

            WordText.Text = displayedWord.Trim();
        }

        private void UpdateErrorDisplay()
        {
            ErrorText.Text =
                "Erreurs : " + errors + " / " + MaxErrors;
        }

        private void CheckGameResult()
        {
            bool playerWon = true;

            foreach (char letter in secretWord)
            {
                if (!guessedLetters.Contains(letter))
                {
                    playerWon = false;
                    break;
                }
            }

            if (playerWon)
            {
                gameFinished = true;

                WordText.Text =
                    string.Join(" ", secretWord.ToCharArray());

                MessageText.Text =
                    "VICTOIRE ! Tu montes sur le podium.";

                MessageText.Foreground = Brushes.LimeGreen;

                DisableKeyboard();
            }
            else if (errors >= MaxErrors)
            {
                gameFinished = true;

                WordText.Text =
                    string.Join(" ", secretWord.ToCharArray());

                MessageText.Text =
                    "PERDU ! Le mot était " + secretWord + ".";

                MessageText.Foreground = Brushes.OrangeRed;

                DisableKeyboard();
            }
        }

        private void DisableKeyboard()
        {
            foreach (object element in KeyboardPanel.Children)
            {
                Button button = element as Button;

                if (button != null)
                {
                    button.IsEnabled = false;
                }
            }
        }

        private void RestartButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void DrawHangman()
        {
            HangmanCanvas.Children.Clear();

            Brush blueBrush =
                new SolidColorBrush(Color.FromRgb(80, 164, 255));

            Brush whiteBrush = Brushes.White;

            // Support
            AddLine(25, 260, 210, 260, blueBrush, 5);
            AddLine(60, 260, 60, 25, blueBrush, 5);
            AddLine(60, 25, 165, 25, blueBrush, 5);
            AddLine(165, 25, 165, 55, blueBrush, 4);

            // Première erreur : tête
            if (errors >= 1)
            {
                Ellipse head = new Ellipse();

                head.Width = 42;
                head.Height = 42;
                head.Stroke = whiteBrush;
                head.StrokeThickness = 4;

                Canvas.SetLeft(head, 144);
                Canvas.SetTop(head, 55);

                HangmanCanvas.Children.Add(head);
            }

            // Deuxième erreur : corps
            if (errors >= 2)
            {
                AddLine(
                    165, 97,
                    165, 170,
                    whiteBrush, 4);
            }

            // Troisième erreur : bras gauche
            if (errors >= 3)
            {
                AddLine(
                    165, 115,
                    125, 145,
                    whiteBrush, 4);
            }

            // Quatrième erreur : bras droit
            if (errors >= 4)
            {
                AddLine(
                    165, 115,
                    205, 145,
                    whiteBrush, 4);
            }

            // Cinquième erreur : jambe gauche
            if (errors >= 5)
            {
                AddLine(
                    165, 170,
                    130, 220,
                    whiteBrush, 4);
            }

            // Sixième erreur : jambe droite
            if (errors >= 6)
            {
                AddLine(
                    165, 170,
                    200, 220,
                    whiteBrush, 4);
            }
        }

        private void AddLine(
            double x1,
            double y1,
            double x2,
            double y2,
            Brush color,
            double thickness)
        {
            Line line = new Line();

            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;

            line.Stroke = color;
            line.StrokeThickness = thickness;

            line.StrokeStartLineCap = PenLineCap.Round;
            line.StrokeEndLineCap = PenLineCap.Round;

            HangmanCanvas.Children.Add(line);
        }
    }
}