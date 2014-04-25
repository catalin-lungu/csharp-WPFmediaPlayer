using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace Player
{
    public class Melodie
    {
        string durata;
        string numeMelodie;
        string compozitor;
        string interpret;
        Uri sursa;
        string coperta;

        public string Coperta
        {
            set { this.coperta = value; }
            get { return this.coperta; }
        }
        public Uri URI
        {
            set { this.sursa = value; }
            get { return this.sursa; }
        }
        public string Durata
        {
            set { this.durata = value; }
            get { return this.durata; }
        }
        public string NumeMelodie
        {
            set { this.numeMelodie = value; }
            get { return this.numeMelodie; }
        }
        public string Compozitor
        {
            set { this.compozitor = value; }
            get { return this.compozitor; }
        }
        public string Interpret
        {
            set { this.interpret = value; }
            get { return this.interpret; }
        }
    }
    public class Album
    {
        public static List<Melodie> getSongs(string fileName)
        {
            List<Melodie> lista = new List<Melodie>();
            var songs = from s in XElement.Load(fileName).Elements("melodie") select s;
            foreach (var song in songs)
            {
                Melodie m = new Melodie();
                m.Durata = song.Element("durata").Value;
                m.NumeMelodie = song.Element("titlu_melodie").Value;
                m.Compozitor = song.Element("compozitor").Value;
                m.Interpret = song.Element("interpret").Value;

                try
                {
                    if (song.Element("coperta").Name == "coperta")
                        m.Coperta = fileName.Substring(0,fileName.LastIndexOf('\\')+1) + song.Element("coperta").Value;
                }
                catch
                {
                    m.Coperta = "No image to display";
                }
                lista.Add(m);
            }
            return lista;
        }
    }

    public class Xml
    { 
        public static void getMoreInfo(Melodie m, string xmlPath)
        {
            if (File.Exists(xmlPath))
            {
                List<Melodie> ls = Album.getSongs(xmlPath);
                bool find = false;
                foreach (Melodie mel in ls)
                {
                    if (mel.NumeMelodie == m.NumeMelodie)
                    {
                        m.Durata = mel.Durata;
                        m.Compozitor = mel.Compozitor;
                        m.Interpret = mel.Interpret;
                        m.Coperta = mel.Coperta;
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    m.Interpret = "unknown";
                    m.Compozitor = "unknown";
                }
            }
            else
            {
                m.Interpret = "unknown";
                m.Compozitor = "unknown";
            }
        }
    }
}
