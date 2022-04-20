using DW.RtfWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;

namespace ZpevnikEditor
{
    class PubVar
    {
    }
    public partial class MainWindow : Window
    {
        //public static string url = @"C:\Temp\export.xml";

        public static ObservableCollection<PageSong> psongs;
        //public static string filePath = @"C:\Users\vasek\Desktop\Demo.rtf";
        public static Song vybranySong;
        public static PaperSize VelikostPapiru = PaperSize.A4;
        public class PageSong
        {
            private Song _song;
            private double _pocetStran;
            public Song Song { get; }
            public List<Akord> Akordy
            {
                get
                {
                    List<Akord> _Akordy = new List<Akord>();

                    if (Song.songtext == null)
                        return _Akordy;

                    #region  najdi a zvýrazni akordy 

                    Regex akordStartChar = new Regex(@"\[");
                    foreach (Match akordStart in akordStartChar.Matches(Song.songtext))
                    {

                        for (int i = akordStart.Index + 1; i < akordStart.Index + 10; i++)
                        {
                            if (Song.songtext[i] == ']')
                            {
                                Akord listAddAkord = new Akord(_song);
                                listAddAkord.StartIndex = akordStart.Index;
                                listAddAkord.EndIndex = i;
                                listAddAkord.Value = Song.songtext.Substring(akordStart.Index, i - akordStart.Index + 1);
                                _Akordy.Add(listAddAkord);
                                break;
                            }

                        }
                    }

                    //akordy.fmt = par.addCharFormat(4, 8);
                    //fmt.FgColor = blue;
                    //fmt.BgColor = red;
                    //fmt.FontSize = 18;

                    #endregion


                    return _Akordy;
                }
            }
            public int PocetStran
            {
                get
                {
                    if (Song.songtext == null)
                        return 1;
                    int pocetradku = Song.songtext.Split('\n').Length;

                    foreach (string radek in Song.songtext.Split('\n'))
                    {
                        int aktualniznak;
                        if (VelikostPapiru == PaperSize.A4)
                            aktualniznak = 75;
                        else
                            aktualniznak = 45; //papír je A5

                        while (radek.Length > aktualniznak)
                        {
                            pocetradku += 1;
                            aktualniznak += aktualniznak;
                        }
                    }
                    double dbl = (double)pocetradku / 41;
                    _pocetStran = Math.Ceiling(dbl);
                    return (int)_pocetStran;
                }

                //set
                //{
                //    PocetStran = value;
                //}


            }
            public int[] Strany { get; set; }
            public string StranyString { get; set; }
            public PageSong(Song song)
            {
                Song = song;
                _song = song;
            }
        }
        public class Akord
        {
            public Song Song { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public string Value { get; set; }
            public Akord(Song song)
            {
                Song = song;
            }
        }
        public class Song
        {
            public string author { get; set; }
            public string nazev { get; set; }
            public string Id { get; set; }
            public string lang { get; set; }
            public string songtext { get; set; }
            public string authorID { get; set; }
            public string xmlText { get; set; }

        }
    }
}
