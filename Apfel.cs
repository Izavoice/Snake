using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
namespace WPF_Snake_Demo
{
    class Apfel
    {
        //Felder
        //Color
        Color farbe;

        //Form
        readonly Ellipse kreis;

        //größe
        readonly int groesse;

        //Kollision
        readonly Rectangle rechteckKollision;

        //Apfelmitte pos
        private Point ApfelMittePosition;

        //Konstruktor
        public Apfel(Color farbe, int groesse)
        {
            this.farbe = farbe;
            this.groesse = groesse;
            kreis = new Ellipse
            {
                Name = "Apfel"
            };
            //kollision inztanz
            rechteckKollision = new Rectangle
            {
                Name = "Kollision"
            };

        }

        //Apfel Anzeigen
        public void Anzeigen(Canvas meinCanvas, int balkenbreite)
        {
            //Zuffals generator
            Random zufall = new Random();

            //minimum
            int min = balkenbreite;

            //maximum
            int maxX = (int)meinCanvas.ActualWidth - balkenbreite - groesse;
            int maxY = (int)meinCanvas.ActualHeight - balkenbreite - groesse;

            //Zuffalls zahlen genarieren und zwischen speichern
            int resultX = zufall.Next(min, maxX);
            int resultY = zufall.Next(min, maxY);

            //koodinaten die aktuelle bereite und höhe geben, die balken breite abziehen und die zuffalls zahl abziehen
            ApfelMittePosition.X = meinCanvas.ActualWidth - balkenbreite - resultX;
            ApfelMittePosition.Y = meinCanvas.ActualHeight - balkenbreite - resultY;

            //positonieren
            Canvas.SetLeft(kreis, resultX);
            Canvas.SetTop(kreis, resultY);


            //größe setzten
            kreis.Width = groesse;
            kreis.Height = groesse;

            //farbe
            SolidColorBrush fuellung = new SolidColorBrush(farbe);
            kreis.Fill = fuellung;

            //Dummy kollision
            rechteckKollision.Width = kreis.Width + (groesse - 1);
            rechteckKollision.Height = kreis.Height + (groesse - 1);

            //farbe
            fuellung = new SolidColorBrush(Colors.Aqua)
            {
                Opacity = 0
            };
            rechteckKollision.Fill = fuellung;

            Canvas.SetLeft(rechteckKollision, Canvas.GetLeft(kreis) - ((groesse - 1) / 2));
            Canvas.SetTop(rechteckKollision, Canvas.GetTop(kreis) - ((groesse - 1) / 2));

            //Hinzufügen
            meinCanvas.Children.Add(rechteckKollision);
            meinCanvas.Children.Add(kreis);
        }

        //Apfelmitte zurückgeben
        public Point ApfelMitte()
        {
            return ApfelMittePosition;
        }

        //methode zum entfernen
        public void Entfernen(Canvas meinCanvas)
        {
            meinCanvas.Children.Remove(kreis);
            meinCanvas.Children.Remove(rechteckKollision);
        }
    }
}
