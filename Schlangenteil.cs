using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
namespace WPF_Snake_Demo
{
    class Schlangenteil
    {
        //die Felder
        protected Point Position;

        //Die alte Postion
        protected Point altePosition;

        //Die Farbe
        protected Color farbe;

        //die form
        protected Rectangle quadrat;

        //die größe
        protected int groesse;

        //Die Methoden

        //Der konstruktor
        public Schlangenteil(Point position, Color farbe)
        {
            this.Position.X = position.X;
            this.Position.Y = position.Y;


            //alte positionnen
            altePosition.X = this.Position.X;
            altePosition.Y = this.Position.Y;

            //farbe setzten
            this.farbe = farbe;

            //die größe wird fest gesetzten
            groesse = 20;

            //ein neues rechteck erzeugen
            quadrat = new Rectangle
            {
                Name = "Schlange"
            };
        }

        //eine leere Methode zum bewegen
        public virtual void Bewegen(int richtung)
        {
        }

        //die postion setzten
        public void SetztePosition(Point neuePosition)
        {
            //alte Position speichern
            altePosition = Position;
            Position = neuePosition;
        }

        //das Teil anzeigen
        public void Zeichnen(Canvas meinCanvas)
        {
            //quadrat löschen
            meinCanvas.Children.Remove(quadrat);
            //positionieren
            Canvas.SetLeft(quadrat, Position.X);
            Canvas.SetTop(quadrat, Position.Y);

            //größe setzten
            quadrat.Width = groesse;
            quadrat.Height = groesse;

            //farbe und rahmen setzten
            SolidColorBrush fuellung = new SolidColorBrush(farbe);
            quadrat.Fill = fuellung;
            SolidColorBrush rahmen = new SolidColorBrush(Colors.White);
            quadrat.Stroke = rahmen;

            //hinzufügen
            meinCanvas.Children.Add(quadrat);
        }

        //Position Liefern
        public Point LiefereAltePosition()
        {
            return altePosition;
        }

        //größe liefern
        public int LiefereGroesse()
        {
            return groesse;
        }

        //liefere aktuelle position
        public virtual Point LieferePosition()
        {
            return Position;
        }
    }
}
