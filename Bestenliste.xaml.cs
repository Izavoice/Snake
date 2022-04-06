using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Snake_Demo
{
    /// <summary>
    /// Interaktionslogik für Bestenliste.xaml
    /// </summary>
    public partial class Bestenliste : Window
    {
        public Bestenliste(List<string> eintraege)
        {
            InitializeComponent();

            int zaehlerZeile = 1;
            int zaehlerSpalte = 0;

            //einträge verarbeiten
            foreach (string zeichenkette in eintraege)
            {
                //label erstellen
                Label meinLabel = new Label
                {
                    Content = zeichenkette
                };

                //im Grid Positionieren
                Grid.SetRow(meinLabel, zaehlerZeile);
                Grid.SetColumn(meinLabel, zaehlerSpalte);
                meinGrid.Children.Add(meinLabel);

                //Spalte erhöhen
                zaehlerSpalte++;

                //wen 2 spalten gefüllt sind
                if (zaehlerSpalte == 2)
                {
                    zaehlerSpalte = 0;
                    zaehlerZeile++;
                    meinGrid.RowDefinitions.Add(new RowDefinition());
                }
            }
        }
    }
}
