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
using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using System.Runtime.InteropServices;
using System.Threading;

namespace BdArtLibrairie
{
    public class VenteBox : Dialog
    {
        private Datas mdatas;
        private Int16 nNumeroVente;
        private DateTime dtDate;
        private double dblPrixTotal, dblPartCB, dblPartCheque, dblPartEspeces;
        private Gdk.Color CouleurFondTxt;
        private BasePrinter printer;
        public ResponseType rResponse;
        [UI] private Label lblLieuVente = null;
        [UI] private Label lblStatutPaiement = null;
        [UI] private Entry txtCoutTotal = null;
        [UI] private Entry txtPartCB = null;
        [UI] private Entry txtPartCheque = null;
        [UI] private Entry txtPartEspeces = null;
        [UI] private Button btnTerminer = null;
        [UI] private Entry txtInfo = null;
        [UI] private Button btnModifier = null;
        [UI] private Button btnSupprimer = null;
        [UI] private Button btnImprimerTicket = null;
        [UI] private Button btnFermer = null;
        [UI] private Button btnChercheAlbum = null;
        [UI] private SearchEntry txtAjouteAlbum = null;
        [UI] private Button btnAnnuler = null;
        [UI] private CheckButton chkPartCB = null;
        [UI] private CheckButton chkPartCheque = null;
        [UI] private CheckButton chkPartEspeces = null;
        [UI] private TreeView trvListeLivres = null;
        [UI] private ComboBoxText cbListeLieuVente = null;
        [UI] private ComboBoxText cbListeStatutPaiement = null;
        [UI] private CheckButton chkImprimerTicket = null;

        public VenteBox(Window ParentWindow, ref Datas datas, Int16 nNumVente=0, DateTime dtDateVente=new DateTime()) : this(new Builder("VenteBox.glade"))
        {
            this.TransientFor = ParentWindow;
            this.SetPosition(WindowPosition.CenterOnParent);
            this.Modal = true;
            this.Title = "Nouvelle vente";
            txtInfo.Visible = false;
            //
            mdatas = datas;
            nNumeroVente = nNumVente;
            dtDate = dtDateVente;
            dblPrixTotal = dblPartCB = dblPartCheque = dblPartEspeces = 0;
            InitCbListeLieuVente();
            InitcbListeStatutPaiement();
            InitTrvVentes();
            UpdateData();
            InitPrinter();
            //
            chkImprimerTicket.Active = Global.ImprimerTickets;
            txtAjouteAlbum.GrabFocus();
        }

        private VenteBox(Builder builder) : base(builder.GetRawOwnedObject("VenteBox"))
        {
            builder.Autoconnect(this);
            //DeleteEvent += delegate { OnBtnFermerClicked(this, new EventArgs()); };// pas de bouton de fermeture
            //
            txtAjouteAlbum.Activated += OnAjouteAlbumActivated;
            //
            btnTerminer.Clicked += OnBtnTerminerClicked;
            btnAnnuler.Clicked += OnBtnAnnulerClicked;
            btnSupprimer.Clicked += OnBtnSupprimerClicked;
            btnModifier.Clicked += OnBtnModifierClicked;
            btnImprimerTicket.Clicked += OnBtnImprimerTicketClicked;
            btnImprimerTicket.Visible = false;
            btnFermer.Clicked += OnBtnFermerClicked;
            btnFermer.Visible = false;
            btnChercheAlbum.Clicked += OnBtnChercheAlbumClicked;
            //
            cbListeLieuVente.Changed += OnCbListeLieuVenteChanged;
            cbListeStatutPaiement.Changed += OnCbListeStatutPaiementChanged;
            //
            chkPartCB.Clicked += OnchkPartCBClicked;
            chkPartCheque.Clicked += OnchkPartChequeClicked;
            chkPartEspeces.Clicked += OnchkPartEspecesClicked;
            chkImprimerTicket.Active = true;
            chkImprimerTicket.Clicked += OnChkImprimerTicketClicked;
            //
            txtPartCB.Changed += OnTxtPartCBChanged;
            txtPartCheque.Changed += OnTxtPartChequeChanged;
            txtPartEspeces.Changed += OnTxtPartEspecesChanged;
            txtPartCB.FocusOutEvent += OnTxtPartCBFocusOutEvent;
            txtPartCheque.FocusOutEvent += OnTxtPartChequeFocusOutEvent;
            txtPartEspeces.FocusOutEvent += OnTxtPartEspecesFocusOutEvent;
            //
            // txtCoutTotal.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtPartCB.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtPartCheque.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtPartEspeces.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtInfo.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            //
            Pango.FontDescription tpf = new Pango.FontDescription();
			tpf.Weight = Pango.Weight.Bold;
            // txtCoutTotal.ModifyFont(tpf);
        }

        private void OnBtnChercheAlbumClicked(object sender, EventArgs e)
        {
            SelectAlbumBox selectAlbumBox = new SelectAlbumBox(this, ref mdatas, cbListeLieuVente.ActiveText, cbListeStatutPaiement.ActiveText);
			selectAlbumBox.Run();
            // si Ok, recalcul des prix
            if (selectAlbumBox.rResponse == ResponseType.Ok)
                UpdateData();
            //
            txtAjouteAlbum.GrabFocus();
        }

        private void OnBtnFermerClicked(object sender, EventArgs a)
        {
            mdatas.lstoreUneVente.Clear();
            Exit();
        }

        private void InitTrvVentes()
        {
            if (nNumeroVente == 0)
            {
                mdatas.lstoreUneVente.Clear();
                nNumeroVente = mdatas.GetNewNumVente();
            }
            else
            {
                mdatas.GetLstoreUneVente(nNumeroVente);
            }
            trvListeLivres.Model = mdatas.lstoreUneVente;
            //
            TreeViewColumn colIsbnEan = new TreeViewColumn();
            colIsbnEan.Title = "ISBN / EAN";
            // dttable albums
                TreeViewColumn colAuteur = new TreeViewColumn();
                colAuteur.Title = "Auteur";
                TreeViewColumn colTitre = new TreeViewColumn();
                colTitre.Title = "Titre";
                TreeViewColumn colPrixVente = new TreeViewColumn();
                colPrixVente.Title = "Prix vente (€)";
                TreeViewColumn colQteVendu = new TreeViewColumn();
            // fin dttable albums
            colQteVendu.Title = "Qté vendu";
            TreeViewColumn colLieu = new TreeViewColumn();
            colLieu.Title = "Lieu";
            TreeViewColumn colPaiement = new TreeViewColumn();
            colPaiement.Title = "Paiement";
            //
            trvListeLivres.AppendColumn(colIsbnEan);
            trvListeLivres.AppendColumn(colAuteur);
            trvListeLivres.AppendColumn(colTitre);
            trvListeLivres.AppendColumn(colPrixVente);
            trvListeLivres.AppendColumn(colQteVendu);
            trvListeLivres.AppendColumn(colLieu);
            trvListeLivres.AppendColumn(colPaiement);
            //
            CellRendererText cellIsbnEan = new CellRendererText();
            colIsbnEan.PackStart(cellIsbnEan, true);
            colIsbnEan.AddAttribute(cellIsbnEan, "text", Convert.ToInt16(Global.eTrvListeLivresCols.CodeIsbnEan));
            colIsbnEan.Visible = false;

            CellRendererText cellAuteur = new CellRendererText();
            colAuteur.PackStart(cellAuteur, true);
            colAuteur.AddAttribute(cellAuteur, "text", Convert.ToInt16(Global.eTrvListeLivresCols.Auteur));

            CellRendererText cellTitre = new CellRendererText();
            colTitre.PackStart(cellTitre, true);
            colTitre.AddAttribute(cellTitre, "text", Convert.ToInt16(Global.eTrvListeLivresCols.Titre));

            CellRendererText cellPrixVente = new CellRendererText();
            colPrixVente.PackStart(cellPrixVente, true);
            colPrixVente.AddAttribute(cellPrixVente, "text", Convert.ToInt16(Global.eTrvListeLivresCols.PrixVente));

            CellRendererText cellQteVendu = new CellRendererText();
            colQteVendu.PackStart(cellQteVendu, true);
            colQteVendu.AddAttribute(cellQteVendu, "text", Convert.ToInt16(Global.eTrvListeLivresCols.QteVendu));

            CellRendererText cellLieu = new CellRendererText();
            colLieu.PackStart(cellLieu, true);
            colLieu.AddAttribute(cellLieu, "text", Convert.ToInt16(Global.eTrvListeLivresCols.LieuVente));
            //colLieu.Visible = false;

            CellRendererText cellPaiement = new CellRendererText();
            colPaiement.PackStart(cellPaiement, true);
            colPaiement.AddAttribute(cellPaiement, "text", Convert.ToInt16(Global.eTrvListeLivresCols.Paiement));
            //colPaiement.Visible = false;
        }

        private void GetPartsPaiementVente()
        {
            // sélection dans la table Paiements
            foreach (DataRow rowP in mdatas.dtTablePaiements.Select("nNumeroVente=" + nNumeroVente.ToString()))
            {
                if (rowP.RowState == DataRowState.Deleted)
                    continue;
                //
                dblPartCB = dblPrixTotal * Convert.ToDouble(rowP["dblPourcentCB"]) / 100;
                dblPartCheque = dblPrixTotal * Convert.ToDouble(rowP["dblPourcentCheque"]) / 100;
                dblPartEspeces = dblPrixTotal * Convert.ToDouble(rowP["dblPourcentEspeces"]) / 100;
            }
        }

        private void UpdateData(bool bVersIHM = true)
        {
            TreeIter iter;
            double dblPrix;
            string strLieuVente, strPaiement;
            ListStore lsListe = new ListStore(typeof(string));

            if (bVersIHM == true)
            {
                dblPrix = 0;
                strLieuVente = string.Empty;
                strPaiement = string.Empty;
                dblPrixTotal = 0;
                //
                // calcul cout total
                if (mdatas.lstoreUneVente.GetIterFirst(out iter) == true)
                {
                    do
                    {
                        strPaiement = mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.Paiement)).ToString();
                        dblPrix = Convert.ToDouble(mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.PrixVente)));
                        // ignorer cout si Offert ou si AFacturer
                        if (string.Compare(strPaiement, Global.eMoyenPaiement.Offert.ToString()) != 0
                        && string.Compare(strPaiement, Global.eMoyenPaiement.AFacturer.ToString()) != 0)
                            dblPrixTotal += dblPrix;
                        strLieuVente = mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.LieuVente)).ToString();
                    }
                    while (mdatas.lstoreUneVente.IterNext(ref iter) == true);
                }
                txtCoutTotal.Text = Math.Round(dblPrixTotal, 2).ToString();
                //
                // répartitions des parts
                // si une ligne existe dans la table Paiements (seulement dans le cas de l'affichage
                // d'une vente existante), on récupère les parts de paiements
                GetPartsPaiementVente();
                txtPartCB.Changed -= OnTxtPartCBChanged;
                txtPartCheque.Changed -= OnTxtPartChequeChanged;
                txtPartEspeces.Changed -= OnTxtPartEspecesChanged;
                txtPartCB.Text = Math.Round(dblPartCB, 2).ToString();
                txtPartCheque.Text = Math.Round(dblPartCheque, 2).ToString();
                txtPartEspeces.Text = Math.Round(dblPartEspeces, 2).ToString();
                txtPartCB.Changed += OnTxtPartCBChanged;
                txtPartCheque.Changed += OnTxtPartChequeChanged;
                txtPartEspeces.Changed += OnTxtPartEspecesChanged;
            }
            else
            {
                dblPartCB = Convert.ToDouble(txtPartCB.Text);
                dblPartCheque = Convert.ToDouble(txtPartCheque.Text);
                dblPartEspeces = Convert.ToDouble(txtPartEspeces.Text);
            }
        }

        private void InitCbListeLieuVente()
        {
            cbListeLieuVente.AppendText(Global.eListeLieuVente.Librairie.ToString());
            cbListeLieuVente.AppendText(Global.eListeLieuVente.Médiathèque.ToString());
            cbListeLieuVente.Active = 0;
        }

        private void InitcbListeStatutPaiement()
        {
            cbListeStatutPaiement.AppendText(Global.eMoyenPaiement.Vendu.ToString());
            cbListeStatutPaiement.AppendText(Global.eMoyenPaiement.Offert.ToString());
            cbListeStatutPaiement.AppendText(Global.eMoyenPaiement.AFacturer.ToString());
            cbListeStatutPaiement.Active = 0;
        }

        private void OnAjouteAlbumActivated(object sender, EventArgs a)
        {
            Entry txtCode = (Entry)sender;
            // ajout du livre dans lstoreUneVente
            foreach (DataRow row in mdatas.dtTableAlbums.Select("strIsbnEan='" + txtCode.Text + "'"))
            {
                // recherche strAuteur et ajout dans lstoreUneVente
                foreach (DataRow rowAU in mdatas.dtTableAuteurs.Select("nIdAuteur=" + row["nIdAuteur"].ToString()))
                {	
                    mdatas.lstoreUneVente.AppendValues(
                        txtCode.Text,
                            rowAU["strAuteur"].ToString(),
                        row["strTitre"].ToString(),
                        Convert.ToDouble(row["dblPrixVente"]).ToString(),
                        "1",
                        cbListeLieuVente.ActiveText,
                        cbListeStatutPaiement.ActiveText
                    );
                }
            }
            txtCode.Text = string.Empty;
            UpdateData();
        }

        private void OnBtnModifierClicked(object sender, EventArgs a)
        {
            TreeIter iter;
            TreePath chemin;
            TreePath[] chemins;
            chemins = trvListeLivres.Selection.GetSelectedRows();

            // si aucune ligne sélectionnée
            if (chemins.Length == 0)
            {
                Global.ShowMessage("Modification", "Vous devez sélectionner une ligne à modifier", this);
                return;
            }
            if (Global.Confirmation(this, "Modification:", "Voulez-vous vraiment modifier le lieu et le statut de la ligne sélectionnée ?") == false)
				return;
            chemin = chemins[0];
            if (mdatas.lstoreUneVente.GetIter(out iter, chemin) == true)
            {
                mdatas.ModifierUneVente(iter, cbListeLieuVente.ActiveText, cbListeStatutPaiement.ActiveText);
                UpdateData();
            }
        }

        private void OnBtnSupprimerClicked(object sender, EventArgs a)
        {
            TreeIter iter;
            TreePath chemin;
            TreePath[] chemins;
            chemins = trvListeLivres.Selection.GetSelectedRows();

            // si aucune ligne sélectionnée
            if (chemins.Length == 0)
            {
                Global.ShowMessage("Suppression", "Vous devez sélectionner une ligne à supprimer", this);
                return;
            }
            if (Global.Confirmation(this, "Suppression:", "Voulez-vous vraiment supprimer la ligne sélectionnée ?") == false)
				return;
            chemin = chemins[0];
            if (mdatas.lstoreUneVente.GetIter(out iter, chemin) == true)
            {
                mdatas.SupprimerUneVente(iter);
                UpdateData();
            }
        }

        private void OnBtnTerminerClicked(object sender, EventArgs a)
        {
            double dblCumul=0;
            TreeIter iter;
            int nCount = 0, nCountOffert = 0, nCountAFacturer = 0;
            string strPaiement = string.Empty;
            double dblPourcentCB=0, dblPourcentCheque=0, dblPourcentEspeces=0;

            // si pas d'élément
            if (mdatas.lstoreUneVente.GetIterFirst(out iter) == false)
            {
                rResponse = ResponseType.Cancel;
                Exit();
                return;
            }
            else
            {
                do
                {
                    nCount++;
                    strPaiement = mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.Paiement)).ToString();
                    if (string.Compare(strPaiement, Global.eMoyenPaiement.Offert.ToString()) == 0)
                        nCountOffert++;
                    else if (string.Compare(strPaiement, Global.eMoyenPaiement.AFacturer.ToString()) == 0)
                        nCountAFacturer++;
                }
                while (mdatas.lstoreUneVente.IterNext(ref iter) == true);
            }
            if (mdatas.lstoreUneVente.GetIterFirst(out iter) == true)
            {
                // si pas seulement livres Offerts ou AFacturer
                if (nCount != nCountOffert && nCount != nCountAFacturer && nCount != (nCountOffert + nCountAFacturer))
                {
                    // les parts CB, Chèque et Espèces doivent être renseignés
                    dblCumul = dblPartCB + dblPartCheque + dblPartEspeces;
                    if (dblCumul == 0)
                    {
                        Global.ShowMessage("Nouvelle vente", "Vous devez renseigner les montants en CB, Chèque, Espèces", this);
                        return;
                    }
                    else if (dblCumul != dblPrixTotal)
                    {
                        Global.ShowMessage("Nouvelle vente", "Les cumuls CB, Chèque, Espèces (=" + dblCumul.ToString() + "€) ne correspondent pas au prix total", this);
                        return;
                    }
                }
            }
            rResponse = ResponseType.Apply;
            // ajout des livres de la vente dans la table Ventes
            dtDate = DateTime.Now;
            // pour éviter la division par 0
            if (dblPrixTotal != 0)
            {
                dblPourcentCB = 100*dblPartCB/dblPrixTotal;
                dblPourcentCheque = 100*dblPartCheque/dblPrixTotal;
                dblPourcentEspeces = 100*dblPartEspeces/dblPrixTotal;
            }
            mdatas.AjouteVenteLivre(nNumeroVente, dblPourcentCB, dblPourcentCheque, dblPourcentEspeces, dtDate);
            //
            if (chkImprimerTicket.Active == true)
            {
                Gdk.Cursor cursor = this.Window.Cursor;
                this.Window.Cursor = new Gdk.Cursor(Gdk.CursorType.CoffeeMug);
                txtInfo.Visible = true;
                Global.AfficheInfo(txtInfo, "Impression des tickets de caisse...", new Gdk.Color(0,0,255));
                //txtInfo.ShowAll();
                try
                {
                    if (ImprimerTicket(dtDate) == true)
                    {
                        if (Global.UseDialogForTicketPrint == false)
                            // on laisse l'impression se terminer avant l'appel à printer.Dispose()
                            Thread.Sleep(Global.Tempo);
                    }
                }
                catch (Exception e)
                {
                    Global.ShowMessage("Impression tickets", e.Message, this);
                }
                finally
                {
                    this.Window.Cursor = cursor;
                }
            }
            Exit();
        }

        private void Exit()
        {
            if (printer != null)
                printer.Dispose();
            this.Dispose();
        }

        private void OnBtnImprimerTicketClicked(object sender, EventArgs a)
        {
            if (printer == null && Global.UseDialogForTicketPrint == false)
                return;
            Gdk.Cursor cursor = this.Window.Cursor;
            this.Window.Cursor = new Gdk.Cursor(Gdk.CursorType.CoffeeMug);
            txtInfo.Visible = true;
            bool bResult = true;
            Global.AfficheInfo(txtInfo, "Impression des tickets de caisse...", new Gdk.Color(0,0,255));
            try
            {
                bResult = ImprimerTicket(dtDate);
            }
            catch (Exception e)
            {
                Global.ShowMessage("Impression tickets", e.Message, this);
                Global.AfficheInfo(txtInfo, "Problème d'impression", new Gdk.Color(255,0,0));
                bResult = false;
            }
            finally
            {
                this.Window.Cursor = cursor;
                if (bResult == true)
                {
                    txtInfo.Text = string.Empty;
                    txtInfo.Visible = false;
                }
            }
        }

        private void OnBtnAnnulerClicked(object sender, EventArgs a)
        {
            TreeIter iter;

            rResponse = ResponseType.Cancel;
            // si pas d'élément
            if (mdatas.lstoreUneVente.GetIterFirst(out iter) == false)
            {
                Exit();
            }
            else if(Global.Confirmation(this, "Quitter", "Toutes les données seront perdues. Continuer ?") == true)
            {
                // on vide le listStore
                mdatas.lstoreUneVente.Clear();
                Exit();
            }
        }

        private void OnchkPartCBClicked(object sender, EventArgs a)
        {
            if (chkPartCB.Active == true)
            {
                txtPartCB.IsEditable = true;
                // txtPartCB.ModifyBg(StateType.Normal, new Gdk.Color(255,255,255));
                txtPartCB.CanFocus = true;
                if (dblPartCheque + dblPartEspeces == 0)
                    dblPartCB = dblPrixTotal;
            }
            else
            {
                txtPartCB.IsEditable = false;
                txtPartCB.CanFocus = false;
                // txtPartCB.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
                dblPartCB = 0;
            }
            UpdateData();
        }
        private void OnchkPartChequeClicked(object sender, EventArgs a)
        {
            if (chkPartCheque.Active == true)
            {
                txtPartCheque.IsEditable = true;
                txtPartCheque.CanFocus = true;
                // txtPartCheque.ModifyBg(StateType.Normal, new Gdk.Color(255,255,255));
                if (dblPartCB + dblPartEspeces == 0)
                    dblPartCheque = dblPrixTotal;
            }
            else
            {
                txtPartCheque.IsEditable = false;
                txtPartCheque.CanFocus = false;
                // txtPartCheque.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
                dblPartCheque = 0;
            }
            UpdateData();
        }
        private void OnchkPartEspecesClicked(object sender, EventArgs a)
        {
            if (chkPartEspeces.Active == true)
            {
                txtPartEspeces.IsEditable = true;
                txtPartEspeces.CanFocus = true;
                // txtPartEspeces.ModifyBg(StateType.Normal, new Gdk.Color(255,255,255));
                if (dblPartCB + dblPartCheque == 0)
                    dblPartEspeces = dblPrixTotal;
            }
            else
            {
                txtPartEspeces.IsEditable = false;
                txtPartEspeces.CanFocus = false;
                // txtPartEspeces.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
                dblPartEspeces = 0;
            }
            UpdateData();
        }

        // Controle du texte entré dans les zones de textes.
        private void OnTxtPartCBChanged(object sender, EventArgs a)
        {
            txtPartCB.Changed -= OnTxtPartCBChanged;
            Global.CheckValeurs(this, sender);
            txtPartCB.Changed += OnTxtPartCBChanged;
        }

        private void OnTxtPartChequeChanged(object sender, EventArgs a)
        {
            txtPartCheque.Changed -= OnTxtPartChequeChanged;
            Global.CheckValeurs(this, sender);
            txtPartCheque.Changed += OnTxtPartChequeChanged;
        }

        private void OnTxtPartEspecesChanged(object sender, EventArgs a)
        {
            txtPartEspeces.Changed -= OnTxtPartEspecesChanged;
            Global.CheckValeurs(this, sender);
            txtPartEspeces.Changed += OnTxtPartEspecesChanged;
        }

        private void OnTxtPartCBFocusOutEvent(object sender, EventArgs a)
        {
            txtPartCB.FocusOutEvent -= OnTxtPartCBFocusOutEvent;
			txtPartCB.Text = Global.GetValueOrZero(this, sender, true).ToString();
			txtPartCB.FocusOutEvent += OnTxtPartCBFocusOutEvent;
            UpdateData(false);
        }

        private void OnTxtPartChequeFocusOutEvent(object sender, EventArgs a)
        {
            txtPartCheque.FocusOutEvent -= OnTxtPartChequeFocusOutEvent;
			txtPartCheque.Text = Global.GetValueOrZero(this, sender, true).ToString();
			txtPartCheque.FocusOutEvent += OnTxtPartChequeFocusOutEvent;
            UpdateData(false);
        }

        private void OnTxtPartEspecesFocusOutEvent(object sender, EventArgs a)
        {
            txtPartEspeces.FocusOutEvent -= OnTxtPartEspecesFocusOutEvent;
			txtPartEspeces.Text = Global.GetValueOrZero(this, sender, true).ToString();
			txtPartEspeces.FocusOutEvent += OnTxtPartEspecesFocusOutEvent;
            UpdateData(false);
        }

        private void OnCbListeLieuVenteChanged(object sender, EventArgs a)
        {
            txtAjouteAlbum.GrabFocus();
        }

        private void OnCbListeStatutPaiementChanged(object sender, EventArgs a)
        {
            txtAjouteAlbum.GrabFocus();
        }

        private void OnChkImprimerTicketClicked(object sender, EventArgs a)
        {
            if (chkImprimerTicket.Active == true)
                Global.ImprimerTickets = true;
            else
                Global.ImprimerTickets = false;
            txtAjouteAlbum.GrabFocus();
        }

        private string GetTicketTest(DateTime dtDate)
        {
            TreeIter iter;
            string strIsbnEan, strPaiement, strLieuVente, strTitre;
            double dblPrixVente;
            string strResult=string.Empty;
            string strEndLine = "\r\n";
            int nLargeurTot = 42;
            int nLongueurTitreTot = nLargeurTot - 7 - 1;
            int nLongueurTitre = nLongueurTitreTot - 2;
            string strSeparator = new String('-', nLargeurTot) + strEndLine;

            if (mdatas.lstoreUneVente.GetIterFirst(out iter) == true)
            {
                strResult = "Librairie BD'Art - Vente n°" + nNumeroVente.ToString() + strEndLine;
                strResult += dtDate.ToString() + strEndLine;
                strResult += strSeparator;
                do
                {
                    strIsbnEan = mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.CodeIsbnEan)).ToString();
                    strTitre = mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.Titre)).ToString();
                    if (strTitre.Length > nLongueurTitre)
                        strTitre = strTitre.Substring(0,nLongueurTitre);
                    strPaiement = mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.Paiement)).ToString();
                    dblPrixVente = Convert.ToDouble(mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.PrixVente)));
                    strLieuVente = mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.LieuVente)).ToString();
                    //
                    strResult += strIsbnEan + strEndLine;
                    strResult += strTitre.PadRight(nLongueurTitreTot, '.') + string.Format("{0,7:F2}€", dblPrixVente) + strEndLine;
                    strResult += "Lieu: " + strLieuVente + " - Statut: " + strPaiement + strEndLine;
                }
                while (mdatas.lstoreUneVente.IterNext(ref iter) == true);
                //
                strResult += strSeparator;
                strResult += ("Total (" + "CB:" + txtPartCB.Text + "|Ch:" + txtPartCheque.Text + "|Es:" + txtPartEspeces.Text + ")").PadRight(nLongueurTitreTot,'.') + string.Format("{0,7:F2}€", Math.Round(dblPrixTotal, 2)) + strEndLine;
            }
            return strResult;
        }

        private void InitPrinter()
        {
            printer = null;
            if (Global.UseDialogForTicketPrint == true)
                return;
            //
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    printer = new SerialPrinter(portName: "COM5", baudRate: 115200);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    printer = new FilePrinter(filePath: Global.PrinterFilePath);
                else
                {
                    Global.ShowMessage("Impression tickets:", "Votre OS n'est pas pris en charge", this);
                    return;
                }
            }
            catch(Exception e)
            {
                Global.ShowMessage("Initialisation imprimante:", e.Message, this);
            }
            finally
            {
                if (printer == null)
                {
                    txtInfo.Visible = true;
                    Global.AfficheInfo(txtInfo, "L'imprimante n'est pas disponible", new Gdk.Color(255, 0, 0));
                }
            }
        }

        private bool ImprimerTicket(DateTime dtDate)
        {
            if (Global.UseDialogForTicketPrint == true)
            {
            //    Console.Write(GetTicketTest(dtDate));
                Global.ShowMessage("Ticket:", GetTicketTest(dtDate), this, MessageType.Info);
                return true;
            }
            //
            if (printer == null)
            {
                return false;
            }
            ICommandEmitter e;
            TreeIter iter;
            string strIsbnEan, strPaiement, strLieuVente, strTitre;
            double dblPrixVente;
            int nLargeurTot = 47;
            int nLongueurTitreTot = nLargeurTot - 7 - 1;
            int nLongueurTitre = nLongueurTitreTot - 2;
            string strSeparator = new String('-', nLargeurTot);
            var EURO = new byte[] { 0xD5 };// €
        //    var NUM = new byte[] { 0xF8 };// °
            //
            e = new EPSON();
            printer.Write(e.Initialize());
            //
            // NombreTickets à imprimer
            for (Int16 n = 0; n < Global.NombreTickets; n++)
            {
                if (mdatas.lstoreUneVente.GetIterFirst(out iter) == true)
                {
                    printer.Write(
                            ByteSplicer.Combine(
                                e.CodePage(CodePage.ISO8859_15_LATIN9),
                                e.LeftAlign(),
                                e.SetStyles(PrintStyle.Bold),
                                e.Print("Librairie BD'Art - Vente n°"),
                                e.PrintLine(nNumeroVente.ToString()),
                                e.SetStyles(PrintStyle.None),
                                e.PrintLine(dtDate.ToString()),
                                e.SetStyles(PrintStyle.None),
                                e.PrintLine(strSeparator)
                            )
                    );
                    do
                    {
                        strIsbnEan = mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.CodeIsbnEan)).ToString();
                        strTitre = mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.Titre)).ToString();
                        if (strTitre.Length > nLongueurTitre)
                            strTitre = strTitre.Substring(0,nLongueurTitre);
                        strPaiement = mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.Paiement)).ToString();
                        dblPrixVente = Convert.ToDouble(mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.PrixVente)));
                        strLieuVente = mdatas.lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.LieuVente)).ToString();
                        printer.Write(
                            ByteSplicer.Combine(
                                e.LeftAlign(),
                                e.SetBarcodeHeightInDots(320),
                                e.SetBarWidth(BarWidth.Default),
                                e.SetBarLabelFontB(true),
                                e.SetBarLabelPosition(BarLabelPrintPosition.Below),
                                e.PrintBarcode(BarcodeType.JAN13_EAN13, strIsbnEan),
                                e.Print(strTitre.PadRight(nLongueurTitreTot, '.') + string.Format("{0,7:F2}", dblPrixVente)),
                                e.CodePage(CodePage.PC858_EURO),
                                EURO,
                                e.CodePage(CodePage.ISO8859_15_LATIN9),
                                e.PrintLine(),
                                e.SetStyles(PrintStyle.FontB),
                                e.PrintLine("Lieu: " + strLieuVente + " - Statut: " + strPaiement),
                                e.SetStyles(PrintStyle.None)
                            )
                        );
                    }
                    while (mdatas.lstoreUneVente.IterNext(ref iter) == true);
                    //
                    printer.Write(
                            ByteSplicer.Combine(
                                e.LeftAlign(),
                                e.PrintLine(strSeparator),
                                e.SetStyles(PrintStyle.Bold),
                                e.Print(("Total (" + "CB:" + txtPartCB.Text + "|Ch:" + txtPartCheque.Text + "|Es:" + txtPartEspeces.Text + ")").PadRight(nLongueurTitreTot,'.') + string.Format("{0,7:F2}", Math.Round(dblPrixTotal, 2))),
                                e.CodePage(CodePage.PC858_EURO),
                                EURO,
                                e.CodePage(CodePage.ISO8859_15_LATIN9),
                                e.PrintLine(),
                                e.PrintLine(),
                                e.SetStyles(PrintStyle.None),
                                e.PrintLine("Merci pour votre achat !"),
                                e.FeedLines(3),
                                e.FullCutAfterFeed(2)
                            )
                    );
                }
            }
            return true;
        }

        public void AfficherMasquerBoutons()
        {
            this.Title = "Détails vente";
            btnModifier.Visible = false;
            btnSupprimer.Visible = false;
            btnTerminer.Visible = false;
            btnAnnuler.Visible = false;
            chkImprimerTicket.Visible = false;
            chkPartCB.Sensitive = false;
            chkPartCheque.Sensitive = false;
            chkPartEspeces.Sensitive = false;
            txtAjouteAlbum.Visible = false;
            cbListeStatutPaiement.Visible = false;
            cbListeLieuVente.Visible = false;
            lblStatutPaiement.Visible = false;
            lblLieuVente.Visible = false;
            btnChercheAlbum.Visible = false;
            //
            btnImprimerTicket.Visible = true;
            btnFermer.Visible = true;
            //
            //this.Deletable = true;// pour afficher le bouton de fermeture
            // code EAN ex.: 9782723433785
        }
    }
}