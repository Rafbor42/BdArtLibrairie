/* Ce fichier fait partie de BdArtLibrairie.

    BdArtLibrairie est un logiciel libre: vous pouvez le redistribuer ou le modifier
    suivant les termes de la GNU General Public License telle que publiée par
    la Free Software Foundation, soit la version 3 de la licence, soit
    (à votre gré) toute version ultérieure.

    BdArtLibrairie est distribué dans l'espoir qu'il sera utile,
    mais SANS AUCUNE GARANTIE; sans même la garantie tacite de
    QUALITÉ MARCHANDE ou d'ADÉQUATION à UN BUT PARTICULIER. Consultez la
    GNU General Public License pour plus de détails.

    Vous devez avoir reçu une copie de la GNU General Public License
    en même temps que BdArtLibrairie. Si ce n'est pas le cas, consultez <https://www.gnu.org/licenses>.

    -----------------------------------------------------------------

    This file is part of BdArtLibrairie.

    BdArtLibrairie is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    BdArtLibrairie is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with BdArtLibrairie.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Data;

namespace BdArtLibrairie
{
    public class SelectAlbumBox : Dialog
    {
        private Datas mdatas;
        private ListStore lsListeAuteurs = new ListStore(typeof(string));
        private string strLieuVente, strStatutPaiement;
        public ResponseType rResponse;
        [UI] private ComboBoxText cbListeAuteurs = null;
        [UI] private Entry txtRechercheTitre = null;
        [UI] private TreeView trvAlbums = null;
        [UI] private Button btnOk = null;
        [UI] private Button btnAnnuler = null;

        public SelectAlbumBox(Window ParentWindow, ref Datas datas, string strLieuDeVente, string strStatutDePaiement) : this(new Builder("SelectAlbumBox.glade"))
        {
            this.TransientFor = ParentWindow;
            this.SetPosition(WindowPosition.CenterOnParent);
            this.Modal = true;
            this.Title = "Recherche album";
            //
            mdatas = datas;
            strLieuVente = strLieuDeVente;
            strStatutPaiement = strStatutDePaiement;
            //
            InitTrvAlbums();
            InitCbListeAuteurs();
            //
            mdatas.DoFiltreDtTableAlbumsMini(cbListeAuteurs.ActiveText, txtRechercheTitre.Text);
            //
            txtRechercheTitre.GrabFocus();
        }

        private SelectAlbumBox(Builder builder) : base(builder.GetRawOwnedObject("SelectAlbumBox"))
        {
            builder.Autoconnect(this);
            DeleteEvent += delegate { this.Dispose(); };
            //
            cbListeAuteurs.Changed += OnCbListeAuteursChanged;
            txtRechercheTitre.Changed += OnTxtRechercheTitreChanged;
            btnAnnuler.Clicked += OnBtnAnnulerClicked;
            btnOk.Clicked += OnBtnOkClicked;
            trvAlbums.RowActivated += OnTrvAlbumsRowActivated;
        }

        private void OnTxtRechercheTitreChanged(object sender, EventArgs e)
        {
            mdatas.DoFiltreDtTableAlbumsMini(cbListeAuteurs.ActiveText, txtRechercheTitre.Text);
        }

        private void InitCbListeAuteurs()
        {
            cbListeAuteurs.Changed -= OnCbListeAuteursChanged;

            lsListeAuteurs.AppendValues("Tous");
            foreach (DataRow row in mdatas.dtTableAuteurs.Rows)
            {
                lsListeAuteurs.AppendValues(row["strAuteur"].ToString());
            }
            cbListeAuteurs.Model = lsListeAuteurs;
            cbListeAuteurs.Active = 0;

            cbListeAuteurs.Changed += OnCbListeAuteursChanged;
        }

        private void InitTrvAlbums()
        {
            trvAlbums.Model = mdatas.lstoreAlbumsMini;
            //
            TreeViewColumn colIsbnEan = new TreeViewColumn();
            colIsbnEan.Title = "ISBN / EAN";
            TreeViewColumn colAuteur = new TreeViewColumn();
            colAuteur.Title = "Auteur";
            TreeViewColumn colTitre = new TreeViewColumn();
            colTitre.Title = "Titre";
            //
            trvAlbums.AppendColumn(colIsbnEan);
            trvAlbums.AppendColumn(colAuteur);
            trvAlbums.AppendColumn(colTitre);
            //
            CellRendererText cellIsbnEan = new CellRendererText();
            colIsbnEan.PackStart(cellIsbnEan, true);
            colIsbnEan.AddAttribute(cellIsbnEan, "text", Convert.ToInt16(Global.eTrvAlbumsCols.CodeIsbnEan));
            colIsbnEan.Visible = false;

            CellRendererText cellAuteur = new CellRendererText();
            colAuteur.PackStart(cellAuteur, true);
            colAuteur.AddAttribute(cellAuteur, "text", Convert.ToInt16(Global.eTrvAlbumsCols.Auteur));

            CellRendererText cellTitre = new CellRendererText();
            colTitre.PackStart(cellTitre, true);
            colTitre.AddAttribute(cellTitre, "text", Convert.ToInt16(Global.eTrvAlbumsCols.Titre));
        }

        private void OnCbListeAuteursChanged(object sender, EventArgs a)
        {
           mdatas.DoFiltreDtTableAlbumsMini(cbListeAuteurs.ActiveText, txtRechercheTitre.Text);
           txtRechercheTitre.GrabFocus();
        }

        // Double-clic sur une ligne.
        private void OnTrvAlbumsRowActivated(object sender, EventArgs a)
        {
            OnBtnOkClicked(sender, a);
        }

        private void OnBtnOkClicked(object sender, EventArgs e)
        {
            rResponse = ResponseType.Ok;
            TreeIter iter;
            TreePath chemin;
            TreePath[] chemins;
            chemins = trvAlbums.Selection.GetSelectedRows();
            string strCode;

            // si une ligne sélectionnée
            if (chemins.Length > 0)
            {
                chemin = chemins[0];
                if (mdatas.lstoreAlbumsMini.GetIter(out iter, chemin) == true)
                {
                    strCode = mdatas.lstoreAlbumsMini.GetValue(iter, Convert.ToInt16(Global.eTrvAlbumsCols.CodeIsbnEan)).ToString();
                    // ajout du livre dans lstoreUneVente
                    foreach (DataRow row in mdatas.dtTableAlbums.Select("strIsbnEan='" + strCode + "'"))
                    {
                        // recherche strAuteur et ajout dans lstoreUneVente
                        foreach (DataRow rowAU in mdatas.dtTableAuteurs.Select("nIdAuteur=" + row["nIdAuteur"].ToString()))
                        {	
                            mdatas.lstoreUneVente.AppendValues(
                                strCode,
                                    rowAU["strAuteur"].ToString(),
                                row["strTitre"].ToString(),
                                Convert.ToDouble(row["dblPrixVente"]).ToString(),
                                "1",
                                strLieuVente,
                                strStatutPaiement
                            );
                        }
                    }
                }
            }
            Exit();
        }

        private void OnBtnAnnulerClicked(object sender, EventArgs a)
        {
            rResponse = ResponseType.Cancel;
            Exit();
        }

        private void Exit()
        {
            mdatas.lstoreAlbumsMini.Clear();
            this.Dispose();
        }
    }
}