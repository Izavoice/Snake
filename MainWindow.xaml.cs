using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace WPF_Snake_Demo
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Felder
        //Die Schlange
        readonly List<Schlangenteil> schlange;

        //Score Klasse feld
        readonly Score spielpunkte;

        //Timer
        readonly DispatcherTimer timerSchlange;
        readonly DispatcherTimer timerSpielzeit;

        readonly DispatcherTimer timerBoost;

        //Speichern der Geschwindichkeit
        int boostwert;
        //Für die 100 punkte
        int boostPunkte;
        //Ob der booster an oder aus ist
        bool boosterAnAus;

        //Apfel klasse
        Apfel meinApfel;

        //Zustand des spiels
        bool spielUnterbrochen;
        bool spielGestartet;

        bool levelzwei;

        //schlangen Geschwindigkeit
        int geschwindigkeit;

        //command
        static readonly RoutedCommand pause = new RoutedCommand();

        //für die Punkte
        int punkte;
        int punkteMehr;

        //Für die zeit
        private int zeit;

        //Für die richtung
        private int richtung;

        //für die breite der spielfeldbegrenzung
        readonly int balkenbreite;

        //zugiff Eigenschaft
        public static RoutedCommand Pause
        {
            get
            {
                return pause;
            }
        }

        static readonly RoutedCommand spielNeu = new RoutedCommand();
        public static RoutedCommand SpielNeu
        {
            get
            {
                return spielNeu;
            }
        }

        //Main Konstruktor
        public MainWindow()
        {
            System.Threading.Thread.Sleep(3000);
            InitializeComponent();
            //Liste
            schlange = new List<Schlangenteil>();

            //eine Bestenliste erzeugen
            spielpunkte = new Score();

            levelzwei = false;

            geschwindigkeit = 1000;
            spielGestartet = false;
            balkenbreite = 25;
            punkteMehr = 10;

            //Timer für das bewegen der schlange
            //die Instanz erzeugen
            timerSchlange = new DispatcherTimer
            {
                //das Intervall setzen
                Interval = TimeSpan.FromMilliseconds(geschwindigkeit)
            };
            //die Methode für das Ereignis zuweisen
            timerSchlange.Tick += new EventHandler(Timer_SchlangeBewegen);

            //der Timer für die Spielzeit
            //die Instanz erzeugen
            timerSpielzeit = new DispatcherTimer
            {
                //das Intervall setzen
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            //die Methode für das Ereignis zuweisen
            timerSpielzeit.Tick += new EventHandler(Timer_Spielzeit);

            boosterAnAus = false;
            timerBoost = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            timerBoost.Tick += new EventHandler(Timer_TimerBoost);

            //Spiel anhalten 
            spielUnterbrochen = true;
        }

        //start daten
        void Start()
        {
            if (levelzwei == false)
            {
                spielpunkte.LoeschePunkte();
                punkte = 0;
                zeit = 0;

                Booster_Bar.Value = 0;

                geschwindigkeit = 1000;
                timerSchlange.Interval = TimeSpan.FromMilliseconds(geschwindigkeit);
            }

            richtung = 0;
            zeitAnzeige.Content = zeit;
            punktAnzeige.Content = punkte;


            spielGestartet = true;

            //Schwierigkeit elemente deaktivieren
            Menu_Item_Schwierigkeit.IsEnabled = false;


            spielfeld.Children.Clear();
            schlange.Clear();
            //und die Grenzen neu zeichnen
            ZeichneSpielfeld();
            //den Schlangenkopf erzeugen und positionieren
            Schlangenkopf meineSchlangeKopf = new Schlangenkopf(new Point(spielfeld.ActualWidth / 2, spielfeld.ActualHeight / 2), Colors.Red);
            //in die Liste setzen
            schlange.Add(meineSchlangeKopf);

            meinApfel = new Apfel(Colors.Green, 20);
            meinApfel.Anzeigen(spielfeld, balkenbreite);
        }

        //Hilfsmethode zum Zeichen der Bregrenzung
        void ZeichneRechteck(Point position, double laenge, double breite)
        {
            //balken instanz
            Rectangle balken = new Rectangle();
            Canvas.SetLeft(balken, position.X);
            Canvas.SetTop(balken, position.Y);

            balken.Name = "Grenze";
            balken.Width = laenge;
            balken.Height = breite;
            SolidColorBrush fuellung = new SolidColorBrush(Colors.Red);
            balken.Fill = fuellung;
            //und hinzufügen
            spielfeld.Children.Add(balken);
        }

        void ZeichneSpielfeld()
        {
            //der Balken oben
            ZeichneRechteck(new Point(0, 0), spielfeld.ActualWidth, balkenbreite);
            //der Balken rechts
            ZeichneRechteck(new Point(spielfeld.ActualWidth - balkenbreite, 0), balkenbreite, spielfeld.ActualHeight);
            //der Balken unten
            ZeichneRechteck(new Point(0, spielfeld.ActualHeight - balkenbreite), spielfeld.ActualWidth, balkenbreite);
            //und der Balken links
            ZeichneRechteck(new Point(0, 0), balkenbreite, spielfeld.ActualHeight);
        }

        //zeichne neues spielfeld
        void ZeichneLevelZwei()
        {
            ZeichneRechteck(new Point(((spielfeld.ActualWidth - balkenbreite) / 2) - (balkenbreite * 2), (spielfeld.ActualHeight - balkenbreite) / 2), balkenbreite, balkenbreite);
            ZeichneRechteck(new Point(((spielfeld.ActualWidth - balkenbreite) / 2) + (balkenbreite * 2), (spielfeld.ActualHeight - balkenbreite) / 2), balkenbreite, balkenbreite);
        }

       //ist der apfel auf der grenze gespawnt?
        private void KollisionLevelZwei()
        {
            HitTestResult apfelKollision = VisualTreeHelper.HitTest(spielfeld, meinApfel.ApfelMitte());
            if (apfelKollision != null)
            {
                string name = ((Shape)apfelKollision.VisualHit).Name;

                if (name == "Grenze")
                {
                    //alten apfel löschen
                    meinApfel.Entfernen(spielfeld);
                    //und einen neuen erzeugen
                    meinApfel = new Apfel(Colors.Green, 20);
                    meinApfel.Anzeigen(spielfeld, balkenbreite);
                }
            }
        }

        private void Timer_Spielzeit(object sender, EventArgs e)
        {
  
            //aufrufen der grenze
            KollisionLevelZwei();
            zeit++;
            zeitAnzeige.Content = zeit;
        }

        private void Timer_SchlangeBewegen(object sender, EventArgs e)
        {
            //den Kopf in die angegebene Richtung bewegen
            schlange[0].Bewegen(richtung);
            //und zeichnen
            schlange[0].Zeichnen(spielfeld);
            //die Teile in einer Schleife bewegen
            for (int index = 1; index < schlange.Count; index++)
            {
                schlange[index].SetztePosition(schlange[index - 1].LiefereAltePosition());
                schlange[index].Zeichnen(spielfeld);
            }
            KollisionPruefen();
        }

        private void Timer_TimerBoost(object sender, EventArgs e)
        {
            //Progress Bar reduzieren
            Booster_Bar.Value--;

            //wen Die progress Bar 0 ereicht hat
            if (Booster_Bar.Value == 0)
            {
                timerBoost.Stop();
                //Die allte geschwindichkeit wieder hinzufügen
                geschwindigkeit += boostwert;
                timerSchlange.Interval = TimeSpan.FromMilliseconds(geschwindigkeit);
                boostwert = 0;
                boosterAnAus = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ZeichneSpielfeld();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            //oben
            if (e.Key == Key.Up)
            {
                richtung = 0;
            }
            //unten
            if (e.Key == Key.Down)
            {
                richtung = 2;
            }
            if (e.Key == Key.Left)
            {
                richtung = 3;
            }
            if (e.Key == Key.Right)
            {
                richtung = 1;
            }
            if (e.Key == Key.Space)
            {
                //Wen Boost time vorhanden ist und die geschwindigkeit nicht schnneler alls 100 Milisekunden ist
                if (boosterAnAus == false && geschwindigkeit > 100 && Booster_Bar.Value != 0)
                {
                    //schlange schnneler machen
                    geschwindigkeit /= 2;
                    //zwischenspeicher
                    boostwert = geschwindigkeit;
                    //an der schklange übergeben
                    timerSchlange.Interval = TimeSpan.FromMilliseconds(geschwindigkeit);
                    boosterAnAus = true;
                    timerBoost.Start();
                }
                else
                {
                    //Wen der Boost vor dem zeit limit beendet wurde
                    //Die allte geschwindichkeit wieder hinzufügen
                    if (Booster_Bar.Value > 0 && boosterAnAus == true)
                    {
                        timerBoost.Stop();
                        geschwindigkeit += boostwert;
                        timerSchlange.Interval = TimeSpan.FromMilliseconds(geschwindigkeit);
                        boostwert = 0;
                        boosterAnAus = false;
                    }
                }
            }
        }

        private void KollisionPruefen()
        {

            //treffer abfrage
            HitTestResult treffer = VisualTreeHelper.HitTest(spielfeld, schlange[0].LieferePosition());
            if (treffer != null)
            {
                string name = ((Shape)treffer.VisualHit).Name;

                //kollision mit der schlange oder rand
                if (name == "Grenze" || name == "Schlange")
                {
                    //Abfrage für neues spiel oder beenden
                    SpielEnde();
                }
                if (name == "Apfel" || name == "Kollision")
                {
                    //Punkte erhöhen
                    punkte = spielpunkte.VeraenderePunkte(punkteMehr);
                    punktAnzeige.Content = punkte;
                    boostPunkte += punkteMehr;
                    //Wen die punkte 100 ereichen + 1 sekunde
                    if (boostPunkte == 100)
                    {
                        //eine sekunde zur ProgressBar Hinzufügen
                        Booster_Bar.Value++;
                        //dern Punkte counter wieder auf 0 setzten
                        boostPunkte = 0;
                    }

                    if (punkte % 50 == 0 && geschwindigkeit > 100)
                    {
                        geschwindigkeit -= 100;
                        timerSchlange.Interval = TimeSpan.FromMilliseconds(geschwindigkeit);
                    }

                    //Schlangenteil hinzufügen
                    Schlangenteil sTeil = new Schlangenteil(new Point(schlange[schlange.Count - 1].LiefereAltePosition().X, schlange[schlange.Count - 1].LiefereAltePosition().Y + schlange[schlange.Count - 1].LiefereGroesse()), Colors.Black);
                    schlange.Add(sTeil);

                    //alten apfel löschen
                    meinApfel.Entfernen(spielfeld);
                    //und einen neuen erzeugen
                    meinApfel = new Apfel(Colors.Green, 20);
                    meinApfel.Anzeigen(spielfeld, balkenbreite);

                    if (punkte == 300)
                    {
                        LevelZweiDialog();
                    }
                }
            }
        }

        void SpielPause()
        {
            //Spiel Am laufen
            if (spielUnterbrochen == false)
            {
                //timer anhalten
                timerSchlange.Stop();
                timerSpielzeit.Stop();
                timerBoost.Stop();

                //Menü item makieren
                Menu_Item_SpielPause.IsChecked = true;

                //titel ändern
                Title = "Snake - Das Spiel ist angehalten!";
                spielUnterbrochen = true;
            }
            else
            {
                //timer wieder starten
                timerSchlange.Start();
                timerSpielzeit.Start();

                timerBoost.Start();

                //Menü item makierung aufheben
                Menu_Item_SpielPause.IsChecked = false;

                //Titel ändern
                Title = "Snake";
                spielUnterbrochen = false;
            }
        }

        void LevelZweiDialog()
        {
            //dialog
            MessageBox.Show("Level Zwei ereicht, zum neuen spielfeld wechseln", "Level Zwei", MessageBoxButton.OK, MessageBoxImage.Information);
            levelzwei = true;
            Start();
            ZeichneLevelZwei();
        }

        bool NeuesSpiel()
        {
            bool ergebnis = false;

            //Dialog erzeugen
            MessageBoxResult abfrage = MessageBox.Show("Wollen Sie ein neues Spiel starten?", "Neues Spiel", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (abfrage == MessageBoxResult.Yes)
            {
                levelzwei = false;
                //Spiel starten
                Start();
                ergebnis = true;
            }
            return ergebnis;
        }

        //Methode Wen das Spiel zu ende ist
        void SpielEnde()
        {
            //Spiel anhalten
            SpielPause();

            //Meldung
            MessageBox.Show("Schade.", "Spielende", MessageBoxButton.OK, MessageBoxImage.Information);

            //reicht es für einen Eintrag in der Bestenliste?
            if (spielpunkte.NeuerEintrag(this) == true)
            {
                spielpunkte.ListeAusgeben(this);
            }

            //abfrage für ein neues spiel
            if (NeuesSpiel() == true)
            {
                //Fortsetzten
                SpielPause();
            }
            else
            {
                Close();
            }
        }

        //Methode zum Beenden
        private void Menu_Item_Beenden_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Methode für die Einstellungen
        void SetzeEinstellungen(int breite, int hoehe, int punkteNeu)
        {
            Width = breite;
            Height = hoehe;

            //Punkte setzten
            punkteMehr = punkteNeu;

            //fenster Position
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;

            //Elemente im Spielfeld löschen
            spielfeld.Children.Clear();

            //Spielfeld neu zeichnen
            ZeichneSpielfeld();
        }

        //Menu items
        private void Menu_Item_Einfach_Click(object sender, RoutedEventArgs e)
        {
            Menu_Item_Mittel.IsChecked = false;
            Menu_Item_Schwer.IsChecked = false;
            SetzeEinstellungen(1000, 820, 1);
        }

        private void Menu_Item_Mittel_Click(object sender, RoutedEventArgs e)
        {
            Menu_Item_Einfach.IsChecked = false;
            Menu_Item_Schwer.IsChecked = false;
            SetzeEinstellungen(800, 620, 10);
        }

        private void Menu_Item_Schwer_Click(object sender, RoutedEventArgs e)
        {
            Menu_Item_Einfach.IsChecked = false;
            Menu_Item_Mittel.IsChecked = false;
            SetzeEinstellungen(400, 320, 25);
        }

        private void Menu_Item_Bestenliste_Click(object sender, RoutedEventArgs e)
        {
            bool weiter = false;
            //läuft gerade ein Spiel?
            if (spielUnterbrochen == false)
            {
                SpielPause();
                weiter = true;
            }
            //die Liste anzeigen
            spielpunkte.ListeAusgeben(this);
            //das Spiel wieder fortsetzen

            if (weiter == true)
                SpielPause();
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (spielfeld != null && spielGestartet == true)
            {
                e.CanExecute = true;
            }
        }
        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SpielPause();
        }


       //kann ein neues spiel erstellet werden
        private void SpielNeu_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (spielfeld != null)
            {
                e.CanExecute = true;
            }
        }
        //neues spiel
        private void SpielNeu_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (spielUnterbrochen == false)
            {
                //Pausieren
                SpielPause();
                //Dialog zeigen
                NeuesSpiel();
                //Spiel weiter wieder starten
                SpielPause();
            }
            //wen kein spiel läuft
            else
            {
                if (NeuesSpiel() == true)
                {
                    SpielPause();
                }
            }
        }
    }
}
