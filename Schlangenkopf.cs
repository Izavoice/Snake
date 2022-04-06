using System.Windows;
using System.Windows.Media;
namespace WPF_Snake_Demo
{
    class Schlangenkopf : Schlangenteil
    {
        //Konstruktor
        public Schlangenkopf(Point position, Color farbe) : base(position, farbe)
        {

        }

        public override void Bewegen(int richtung)
        {
            //alte Position
            altePosition = Position;

            switch (richtung)
            {
                //nach oben
                case 0:
                    {
                        Position.Y -= groesse;
                        break;
                    }
                //nach rechts
                case 1:
                    {
                        Position.X += groesse;
                        break;
                    }
                //nach untern
                case 2:
                    {
                        Position.Y += groesse;
                        break;
                    }
                //nach links
                case 3:
                    {
                        Position.X -= groesse;
                        break;
                    }
            }
        }
        public override Point LieferePosition()
        {
            return new Point(Position.X + (groesse / 2), Position.Y + (groesse / 2));

        }
    }
}
