using System;
using System.Collections.Generic;
using System.IO;

namespace WPF_Snake_Demo
{
    //die Klasse für die Liste
    //Sie muss die Schnittstelle IComparable implementieren
    class Liste : IComparable
    {
        //die Felder
        int listePunkte;
        string listeName;

        //die Methoden
        //der Konstruktor
        public Liste()
        {
            //er setzt die Punkte und den Namen auf
            //Standardwerte
            listePunkte = 0;
            listeName = "Nobody";
        }

        //die Vergleichsmethode
        public int CompareTo(object objekt)
        {
            Liste tempListe = (Liste)(objekt);
            if (this.listePunkte < tempListe.listePunkte)
                return 1;
            if (this.listePunkte > tempListe.listePunkte)
                return -1;
            else
                return 0;
        }

        //die Methode zum Setzen von Einträgen
        public void SetzeEintrag(int punkte, string name)
        {
            listePunkte = punkte;
            listeName = name;
        }

        //die Methode zum Liefern der Punkte
        public int GetPunkte()
        {
            return listePunkte;
        }

        //die Methode zum Liefern des Namens
        public string GetName()
        {
            return listeName;
        }
    }

    class Score
    {
        //die Felder
        int punkte;

        //die Anzahl der Einträge in der Liste
        readonly int anzahl = 10;

        //für die Liste
        readonly Liste[] bestenliste;

        //für den Dateinamen
        readonly string dateiname;

        //die Methoden
        //der Konstruktor
        public Score()
        {
            punkte = 0;
            //eine neue Instanz der Liste erstellen
            bestenliste = new Liste[anzahl];
            //die Elemente initialisieren
            for (int i = 0; i < anzahl; i++)
                bestenliste[i] = new Liste();
            //den Dateinamen aus dem Pfad zusammenbauen
            dateiname = AppDomain.CurrentDomain.BaseDirectory + "\\score.dat";
            //wenn es die Datei schon gibt, die Daten lesen
            if (File.Exists(dateiname))
                LesePunkte();
        }

        //zum Verändern der Punkte
        public int VeraenderePunkte(int anzahl)
        {
            punkte += anzahl;
            return punkte;
        }

        //zum Zurücksetzen der Punkte
        public void LoeschePunkte()
        {
            punkte = 0;
        }

        //ist ein neuer Eintrag erreicht?
        public bool NeuerEintrag(System.Windows.Window fenster)
        {
            string tempName = string.Empty;
            //wenn die aktuelle Punktzahl größer ist als der letzte Eintrag der Liste, 
            //wird der letzte Eintrag der Liste überschrieben und die Liste neu sortiert
            if (punkte > bestenliste[anzahl - 1].GetPunkte())
            {
                //den Namen beschaffen
                NameDialog neuerName = new NameDialog
                {
                    //den "Eigentümer" setzen
                    Owner = fenster
                };
                //den Dialog modal anzeigen
                neuerName.ShowDialog();

                if (neuerName.DialogResult == true)
                    tempName = neuerName.LiefereName();
                neuerName.Close();
                bestenliste[anzahl - 1].SetzeEintrag(punkte, tempName);
                Array.Sort(bestenliste);
                //die Daten speichern
                SchreibePunkte();
                return true;
            }
            else
                return false;
        }

        //die Liste ausgeben
        public void ListeAusgeben(System.Windows.Window fenster)
        {
            List<string> eintraege = new List<string>();

            for (int i = 0; i < anzahl; i++)
            {
                eintraege.Add(Convert.ToString(bestenliste[i].GetPunkte()));
                eintraege.Add(bestenliste[i].GetName());
            }
            //Liste anzeigen
            Bestenliste listeAnzeige = new Bestenliste(eintraege)
            {
                //eigentümer setzten
                Owner = fenster
            };

            //dialog antzeigen
            listeAnzeige.ShowDialog();
        }
        //zum Lesen aus der Datei
        void LesePunkte()
        {
            //Zwischen Speichern der daten
            int tempPunkte;
            string tempName;

            //FileStream instanz erzeugen
            using (FileStream fStream = new FileStream(dateiname, FileMode.Open))
            {
                //Binary Reader Instanz
                using (BinaryReader binearDatei = new BinaryReader(fStream))
                {
                    //Einträge lesen und zuweisen
                    for (int i = 0; i < anzahl; i++)
                    {
                        //die punkte
                        tempPunkte = binearDatei.ReadInt32();

                        //den namen
                        tempName = binearDatei.ReadString();

                        //zuweisen
                        bestenliste[i].SetzeEintrag(tempPunkte, tempName);
                    }
                }
            }
        }

        //zum Schreiben in die Datei
        void SchreibePunkte()
        {
            //Instanz Von FileStream
            using (FileStream fStream = new FileStream(dateiname, FileMode.Create))
            {
                //Eine Instanz Von BinaryWriter Erzeugen
                using (BinaryWriter binearDatei = new BinaryWriter(fStream))
                {
                    //Einträge in die Datei schreiben
                    for (int i = 0; i < anzahl; i++)
                    {
                        //Die Punkte
                        binearDatei.Write(bestenliste[i].GetPunkte());

                        //Die namen
                        binearDatei.Write(bestenliste[i].GetName());
                    }
                }
            }
        }
    }
}
