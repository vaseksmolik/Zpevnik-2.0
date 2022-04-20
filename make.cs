using DW.RtfWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

using System.Collections.ObjectModel;

using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace ZpevnikEditor
{
    public partial class MainWindow : Window
    {

        public void CreateXml(List<Song> listSongu)
        {
            XDocument doc = new XDocument();
            //doc.Declaration = new XDeclaration("1.0", "UTF-8", "yes");
            XElement root = new XElement("root");
            foreach (Song song in listSongu)
            {
                root.Add(
                    new XElement("song",
                      new XElement("ID", song.Id),
                      new XElement("lang", song.lang),
                      new XElement("songtext", song.songtext),
                      new XElement("author", song.author),
                      new XElement("authorID", song.authorID),
                      new XElement("groupname", ""),
                      new XElement("title", song.nazev)
                      ));

            }
            doc.Add(root);
            doc.Save(@"C:\Users\vasek\Desktop\document.xml");


        }

        public ObservableCollection<PageSong> NactiPageSongs(List<Song> listSongu)
        {
            ObservableCollection<PageSong> retCol = new ObservableCollection<PageSong>();
            foreach (Song song in listSongu)
            {
                retCol.Add(new PageSong(song));
            }
            return retCol;
        }

        //public List<PageSong> NactiPageSongs(List<Song> listSongu)
        //{
        //    List<PageSong> retList = new List<PageSong>();
        //    foreach (Song song in listSongu)
        //    {
        //        retList.Add(new PageSong(song));
        //    }
        //    return retList;
        //}

        public List<Song> NactiXml(string inputUrl)
        {
            try
            {

                List<Song> songList = new List<Song>();


                XmlDocument doc = new XmlDocument();
                doc.Load(inputUrl);

                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    Song song = new Song();
                    song.xmlText = node.OuterXml;

                    foreach (XmlNode underNode in node.ChildNodes)
                    {
                        if (underNode == null)
                            goto pokracuj;

                        switch (underNode.Name)
                        {
                            case "ID":
                                song.Id = underNode.InnerText;
                                break;
                            case "lang":
                                song.lang = underNode.InnerText;
                                break;
                            case "songtext":
                                song.songtext = underNode.InnerText;
                                break;
                            case "author":
                                song.author = underNode.InnerText;
                                break;
                            case "authorId":
                                song.authorID = underNode.InnerText;
                                break;
                            case "title":
                                song.nazev = underNode.InnerText;
                                break;
                            default:
                                break;
                        }
                    }
                    songList.Add(song);
                pokracuj:
                    continue;
                }




                //Song song = new Song();
                //switch (reader.Name)
                //{
                //    case "author":

                //        XElement el = XNode.ReadFrom(reader) as XElement;
                //        if (el != null)
                //        {
                //            //System.Windows.MessageBox.Show(el.FirstNode.ToString().Substring(0,el.FirstNode.ToString().Length-3).Substring(9));
                //            song.Autor = el.FirstNode.ToString().Substring(0, el.FirstNode.ToString().Length - 3).Substring(9);
                //            song.Obsah = "null";
                //            songList.Add(song);
                //        }

                //        break;
                //    case "songtext":

                //    default:
                //        break;
                //}
                return songList;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return null;
            }
        }


        public void OcislujStrany()
        {
            int pocatecniStrana = 1;
            int strana = pocatecniStrana;
            //List<PageSong> listStran = (GetDataGridRows(Dg_seznamSongu));
            //listStran.RemoveAll(q => q == null);
            //foreach (PageSong pagesong in listStran)

            for (int indexPageSongu = 0; indexPageSongu < psongs.Count(); indexPageSongu++)
            {
                PageSong pagesong = psongs[indexPageSongu];

                pagesong.StranyString = "";
                pagesong.Strany = new int[pagesong.PocetStran];
                for (int i = 0; i < pagesong.PocetStran; i++)
                {
                    pagesong.Strany[i] = strana;
                    pagesong.StranyString += strana.ToString() + ", ";
                    strana++;
                }
                pagesong.StranyString = pagesong.StranyString.Substring(0, pagesong.StranyString.Length - 2);
            }
            Dg_seznamSongu.Items.Refresh();
        }

        public List<PageSong> GetDataGridRows(System.Windows.Controls.DataGrid grid)
        {
            IEnumerable<PageSong> itemsSource = grid.ItemsSource as IEnumerable<PageSong>;

            //IEnumerable<Song> itemsSource = itemsSourcePageSongs.Select(w => w.Song);
            if (null == itemsSource) return null;

            //List<Song> prerovnaneSongy = new List<Song>(itemsSource.Count());
            PageSong[] prerovnaneSongy = new PageSong[itemsSource.Count()];
            foreach (var item in itemsSource)
            {
                //try
                //{
                if (item != null || item.Song != null)
                {
                    DataGridRow row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    //System.Windows.Forms.MessageBox.Show((row.Item as Song).nazev + " - " + row.GetIndex().ToString());
                    if (row != null)
                        prerovnaneSongy.SetValue(row.Item as PageSong, row.GetIndex());
                }
                //}
                //catch
                //{
                //    System.Windows.Forms.MessageBox.Show((grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow).GetIndex().ToString());
                //}
            }
            return prerovnaneSongy.ToList();

            //List<Song> songsPrerovnaneSongy = prerovnaneSongy.ToList().Select(w=>w.Song).ToList();
            //return songsPrerovnaneSongy;
        }


        public static RtfDocument UdelejRtfDokument(List<PageSong> listPageSongu)
        {
            // Create document by specifying paper size and orientation, 
            // and default language.
            var doc = new RtfDocument(VelikostPapiru, PaperOrientation.Portrait, 0);
            // Create fonts and colors for later use
            var arial = doc.createFont("Arial");
            var courier = doc.createFont("Courier New");
            var red = doc.createColor(new Color("ff0000"));
            var blue = doc.createColor(new Color(0, 0, 255));

            // Don't instantiate RtfTable, RtfParagraph, and RtfImage objects by using
            // ``new'' keyword. Instead, use add* method in objects derived from 
            // RtfBlockList class. (See Demos.)
            RtfParagraph par;
            // Don't instantiate RtfCharFormat by using ``new'' keyword, either. 
            // An addCharFormat method are provided by RtfParagraph objects.
            RtfCharFormat fmt;



            foreach (PageSong pagesong in listPageSongu)
            {
                Song song = pagesong.Song;

                // ==========================================================================
                // Demo 1: Font Setting
                // ==========================================================================
                // If you want to use Latin characters only, it is as simple as assigning
                // ``Font'' property of RtfCharFormat objects. If you want to render Far East 
                // characters with some font, and Latin characters with another, you may 
                // assign the Far East font to ``Font'' property and the Latin font to 
                // ``AnsiFont'' property. This Demo contains Traditional Chinese characters.
                // (Note: non-Latin characters are unicoded so you don't have to be worried.)
                par = doc.addParagraph();
                par.DefaultCharFormat.AnsiFont = arial;
                par.addCharFormat().FontStyle.addStyle(FontStyleFlag.Bold);
                par.DefaultCharFormat.FontSize = 18;
                par.LineSpacing = (float)25;
                par.Alignment = Align.Left;
                par.setText(song.nazev);
                par.StartNewPage = true;


                par = doc.addParagraph();
                par.DefaultCharFormat.AnsiFont = arial;
                par.DefaultCharFormat.FontStyle.addStyle(FontStyleFlag.Italic);
                par.DefaultCharFormat.FontSize = 14;
                par.Alignment = Align.Left;
                par.setText(song.author + Environment.NewLine);

                par = doc.addParagraph();
                par.DefaultCharFormat.Font = arial;
                par.DefaultCharFormat.FontSize = 12;
                par.LineSpacing = (float)15;
                par.Alignment = Align.Left;
                par.setText(song.songtext);

                foreach (Akord akord in pagesong.Akordy)
                {
                    fmt = par.addCharFormat(akord.StartIndex, akord.EndIndex);
                    fmt.FontStyle.addStyle(FontStyleFlag.Bold);
                    fmt.FontSize = 14;
                }


                //// ==========================================================================
                //// Demo 2: Character Formatting
                //// ==========================================================================
                //par = doc.addParagraph();
                //par.DefaultCharFormat.Font = times;
                //par.Alignment = Align.Left;

                //par.setText(song.songtext);

                //// Besides setting default character formats of a paragraph, you can specify
                //// a range of characters to which formatting is applied. For convenience,
                //// let's call it range formatting. The following section sets formatting 
                //// for the 4th, 5th, ..., 8th characters in the paragraph. (Note: the first
                //// character has an index of 0)
                //fmt = par.addCharFormat(4, 8);
                //fmt.FgColor = blue;
                //fmt.BgColor = red;
                //fmt.FontSize = 18;
                //// Sets another range formatting. Note that when range formatting overlaps, 
                //// the latter formatting will overwrite the former ones. In the following, 
                //// formatting for the 8th chacacter is overwritten.
                //fmt = par.addCharFormat(8, 10);
                //fmt.FontStyle.addStyle(FontStyleFlag.Bold);
                //fmt.FontStyle.addStyle(FontStyleFlag.Underline);
                //fmt.Font = courier;



            }
            // ==========================================================================
            // Demo 4: Header and Footer
            // ==========================================================================
            // You may use ``Header'' and ``Footer'' properties of RtfDocument objects to
            // specify information to be displayed in the header and footer of every page,
            // respectively.
            par = doc.Footer.addParagraph();
            par.setText("Strana: /");
            par.Alignment = Align.Left;
            par.DefaultCharFormat.FontSize = 15;
            // You may insert control words, including page number, total pages, date and
            // time, into the header and/or the footer. 
            par.addControlWord(7, RtfFieldControlWord.FieldType.Page);
            par.addControlWord(8, RtfFieldControlWord.FieldType.NumPages);
            // Here we also add some text in header.
            par = doc.Header.addParagraph();
            par.Alignment = Align.FullyJustify;
            par.setText("Zpěvník");

            return doc;



        }
        public string OdeberAkordy(PageSong pageSong)
        {
            string _songtext = pageSong.Song.songtext;
            int pocetOdstranenychZnaku = 0;
            foreach (Akord akord in pageSong.Akordy)
            {
                int index = akord.StartIndex - pocetOdstranenychZnaku;
                int counter = akord.EndIndex - akord.StartIndex + 1;
                int pocetpismen = _songtext.Count();
                if (index < pocetpismen)
                    _songtext = _songtext.Remove(index, counter);
                pocetOdstranenychZnaku += akord.EndIndex - akord.StartIndex + 1;
            }
            return _songtext;
        }

    }
}
