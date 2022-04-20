using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Collections;
using DW.RtfWriter;
using System.Collections.ObjectModel;
using BorderStyle = DW.RtfWriter.BorderStyle;
using Color = DW.RtfWriter.Color;

namespace ZpevnikEditor
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            //openFileDialog1.Filter = "Database files (*.xml)";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFileName = openFileDialog1.FileName;
                List<Song> lsongs = NactiXml(selectedFileName);
                //Dg_seznamSongu.ItemsSource = lsongs;
                psongs = NactiPageSongs(lsongs);
                OcislujStrany();
                Dg_seznamSongu.ItemsSource = psongs;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OcislujStrany();
        }


        private void Dgrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }

        private void Btn_Tiskni(object sender, RoutedEventArgs e)
        {
            var y = GetDataGridRows(Dg_seznamSongu);

            RtfDocument doc = UdelejRtfDokument(y);
            System.Windows.Forms.SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            string filePath = saveFileDialog.FileName;
            doc.save(filePath);


            DialogResult mbox = System.Windows.Forms.MessageBox.Show("Hotovo\nDokument uložen na : " + filePath + "\n", "", MessageBoxButtons.OKCancel);
            if (mbox == System.Windows.Forms.DialogResult.OK)
            {
                var p = new Process { StartInfo = { FileName = filePath } };
                p.Start();

            }

        }

        private void Btn_Debug(object sender, RoutedEventArgs e)
        {
            //CreateXml(GetDataGridRows(Dg_seznamSongu));
            System.Windows.Forms.MessageBox.Show((Dg_seznamSongu.SelectedItem as Song).nazev);

        }
        public Song _selectedItem;
        private void Dg_seznamSongu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //RTb_obsah.AppendText();
            //RTb_obsah.
            //if (_selectedItem != null)
            //{
            //    Dg_seznamSongu.SelectedItem = _selectedItem as object;
            //    Dg_seznamSongu.ScrollIntoView(Dg_seznamSongu.ItemContainerGenerator.ContainerFromItem(vybranySong as object) as DataGridRow);
            //    _selectedItem = null;
            //}
            //else
            //{
            if (Dg_seznamSongu.SelectedItem == null)
            {
                RTb_obsah.Text = "";
                RTb_obsah.DataContext = "";
                Tbx_Autor.DataContext = "";
                Tbx_NazevPisne.DataContext = "";

                return;
            }

            vybranySong = (Dg_seznamSongu.SelectedItem as PageSong).Song;

            //Tbx_NazevPisne.Text = ((Dg_seznamSongu.SelectedItem as Song).nazev);
            //Tbx_Autor.Text = ((Dg_seznamSongu.SelectedItem as Song).author);
            RTb_obsah.Text = ((Dg_seznamSongu.SelectedItem as PageSong).Song.songtext);
            RTb_obsah.DataContext = vybranySong;
            Tbx_Autor.DataContext = vybranySong;
            Tbx_NazevPisne.DataContext = vybranySong;

            //TextRange obsah = new TextRange(nadpis.End,);
            //RTb_obsah.AppendText();
            //}
            //if (_selectedItem != null)
            //{
            //    SelectRow(_selectedItem);
            //}
        }

        private void RTb_obsah_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                //vybranySong.songtext = e.Handled.ToString();
                if ((e.Source as System.Windows.Controls.TextBox).Text != (vybranySong.songtext))
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Změnil jste text písně, přejete si ho uložit?", "", MessageBoxButtons.YesNo);
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                        ((e.Source as System.Windows.Controls.TextBox).DataContext as Song).songtext = (e.Source as System.Windows.Controls.TextBox).Text;
                    //else if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
                    //{
                    //    //SelectRow((e.Source as System.Windows.Controls.TextBox).DataContext as Song);
                    //    _selectedItem = (e.Source as System.Windows.Controls.TextBox).DataContext as Song;
                    //}
                }

            }
            catch (Exception w)
            {
                System.Windows.Forms.MessageBox.Show(w.Message);
            }
        }

        private void Btn_OdeberAkordy(object sender, RoutedEventArgs e)
        {
            PageSong _pageSong = Dg_seznamSongu.SelectedItem as PageSong;
            //(Dg_seznamSongu.SelectedItem as PageSong).Song.songtext = OdeberAkordy(_pageSong);
            //RTb_obsah.Text = (Dg_seznamSongu.SelectedItem as PageSong).Song.songtext;
            RTb_obsah.Text = OdeberAkordy(_pageSong);
        }

        private void Btn_VlozRadky(object sender, RoutedEventArgs e)
        {
            foreach (PageSong pageSong in psongs)
            {
                if (pageSong != null && pageSong.Song != null)
                    if (pageSong.Strany.Count() >= 2)
                    {
                        if (pageSong.Strany.First() % 2 != 0)
                        {
                            //List<PageSong> IStoList = Dg_seznamSongu.ItemsSource.Cast<PageSong>().ToList();
                            //IStoList.Insert(Dg_seznamSongu.ItemsSource.Cast<PageSong>().ToList().IndexOf(pageSong), VytvorPrazdnouStranku());
                            //Dg_seznamSongu.ItemsSource = IStoList;
                            psongs.Insert(psongs.IndexOf(pageSong), VytvorPrazdnouStranku());
                        }
                    }
            }
        }

        private void Btn_SmazNuloveRadky(object sender, RoutedEventArgs e)
        {
            List<PageSong> list = Dg_seznamSongu.ItemsSource.Cast<PageSong>().ToList();
            list.RemoveAll(ps => (ps.Song.Id == null || (ps.Song.nazev == "" && ps.Song.songtext == "")));
            Dg_seznamSongu.ItemsSource = list;
            Dg_seznamSongu.Items.Refresh();
        }
        public bool existujeTitulniStrana = false;

        private void Btn_VlozTitulniStranu(object sender, RoutedEventArgs e)
        {
            if (existujeObsahPodleAutoru == true)
                psongs.Remove(psongs.Where(w => w.Song.nazev == "TITULNÍ STRANA").FirstOrDefault());

            psongs.Insert(0, VytvorTitulniStranku());
            existujeTitulniStrana = true;
        }

        public bool existujeObsahPodleAutoru = true;

        private void Btn_VlozitObsahPodleAutoru(object sender, RoutedEventArgs e)
        {
            PageSong zastupnyPageSong = VytvorPrazdnouStranku();

            zastupnyPageSong.Song.nazev = "Obsah";
            zastupnyPageSong.Song.author = "řazení podle autorů";

            psongs.Insert(0, zastupnyPageSong);

            //OcislujStrany();

            if (existujeObsahPodleAutoru == true)
                psongs.Remove(psongs.Where(w => (w.Song.author == "řazení podle autorů" && w.Song.nazev == "Obsah")).FirstOrDefault());

            if (existujeTitulniStrana == true)
            {
                psongs.Insert(1, VytvorObsahPodleAutoru());
            }
            else
            {
                psongs.Insert(0, VytvorObsahPodleAutoru());
            }
            existujeObsahPodleAutoru = true;

        }
        public static OpenFileDialog openFileDialog;
        private void Btn_NactiXML(object sender, RoutedEventArgs e)
        {
            try
            {

                openFileDialog = new System.Windows.Forms.OpenFileDialog();
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.ShowDialog();
                string url = openFileDialog.FileName;

                List<Song> lsongs = NactiXml(url);
                //Dg_seznamSongu.ItemsSource = lsongs;
                OcislujStrany();
                psongs = NactiPageSongs(lsongs);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        //public void SelectRow(Song song)
        //{
        //    //Dg_seznamSongu.ScrollIntoView(Dg_seznamSongu.ItemContainerGenerator.ContainerFromItem(vybranySong as object) as DataGridRow);
        //    DataGridRow dgrow = Dg_seznamSongu.ItemContainerGenerator.ContainerFromItem(vybranySong as object) as DataGridRow;
        //    dgrow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        //    Dg_seznamSongu.SelectedItem = song;
        //    _selectedItem = null;
        //    dgrow.Focus();

        //    //_selectedItem = (e.Source as System.Windows.Controls.TextBox).DataContext as Song;
        //    //SelectedDg_row = (Dg_seznamSongu.ItemContainerGenerator.ContainerFromItem(vybranySong as object) as DataGridRow);
        //}
    }
}
