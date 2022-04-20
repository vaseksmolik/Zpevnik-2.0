using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZpevnikEditor
{
    public partial class MainWindow : Window
    {
        public PageSong VytvorPrazdnouStranku()
        {
            PageSong prazdnaStrana = new PageSong(new Song() { songtext = "", author = "", authorID = "", Id = "", lang = "", nazev = "", xmlText = "" }) { StranyString = "" };
            return prazdnaStrana;
        }
        public PageSong VytvorTitulniStranku()
        {
            PageSong prazdnaStrana = VytvorPrazdnouStranku();
            prazdnaStrana.Song.nazev = "TITULNÍ STRANA";
            return prazdnaStrana;
        }

        public PageSong VytvorObsahPodleAutoru()
        {
            PageSong obsah = VytvorPrazdnouStranku();

            obsah.Song.nazev = "Obsah";
            obsah.Song.author = "řazení podle autorů";

            foreach (PageSong pageSong in psongs.OrderBy(o=>o.Song.author))
            {
                obsah.Song.songtext += pageSong.StranyString;

                string tecky = "";
                for (int i = 0; i < 30- pageSong.StranyString.Length; i++)
                {
                    tecky += "..";
                }
                //obsah.Song.songtext += tecky;
                obsah.Song.songtext += "\t\t";
                obsah.Song.songtext += pageSong.Song.author + " - " + pageSong.Song.nazev + "\n";

            }
            return obsah;
        }
}
}
