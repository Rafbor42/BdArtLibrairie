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
using System.Diagnostics;

namespace BdArtLibrairie
{
    class MainWindow : Window
    {
        Datas datas;
        private double dblTotalVentes, dblPourcentAuteur, dblPartAuteur;
        private double dblTotalLibrairie, dblTotalMediatheques, dblTotalAFacturer;
        private double dblTotalCB, dblTotalCheques, dblTotalEspeces;
        private Int16 nQteMediatheques, nQteLibrairie, nQteOfferts;
        private ListStore lsListeAuteurs = new ListStore(typeof(string));

        [UI] private MenuItem mnuFichierExportAlbums = null;
        [UI] private MenuItem mnuFichierExportFichiers = null;
        [UI] private MenuItem mnuFichierQuitter = null;
        [UI] private MenuItem mnuFichierResetVentes = null;
        [UI] private CheckMenuItem mnuAffichageTout = null;
        [UI] private MenuItem mnuAffichageVente = null;
        [UI] private MenuItem mnuAideApropos = null;
        //
        [UI] private Button btnNouvelleVente = null;
        [UI] private Button btnReset = null;
        [UI] private Button btnFindPrinter = null;
        [UI] private Button btnFindUsbDevice = null;
        [UI] private Button btnInfoErreurQtes = null;
        [UI] private CheckButton chkAFacturer = null;
        [UI] private CheckButton chkUseDialogForTicketPrint = null;
        [UI] private CheckButton chkUseFgColor = null;
        //
        [UI] private ComboBoxText cbListeLieuVente = null;
        [UI] private ComboBoxText cbListeAuteurs = null;
        //
        [UI] private Entry txtTotalVentes = null;
        [UI] private Entry txtTotalCB = null;
        [UI] private Entry txtQteOffert = null;
        [UI] private Entry txtPourcentAuteur = null;
        [UI] private Entry txtTotalLibrairie = null;
        [UI] private Entry txtTotalCheques = null;
        [UI] private Entry txtQteLibrairie = null;
        [UI] private Entry txtQteMediatheques = null;
        [UI] private Entry txtPartAuteur = null;
        [UI] private Entry txtTotalMediatheques = null;
        [UI] private Entry txtTotalEspeces = null;
        [UI] private Entry txtTotalAFacturer = null;
        [UI] private Entry txtInfo = null;
        [UI] private Entry txtPassword = null;// invisible
        [UI] private SearchEntry txtCodeIsbnEan = null;
        [UI] private Entry txtPrinterFilePath = null;
        [UI] private Entry txtUsbDevicePath = null;
        [UI] private Entry txtNombreTickets = null;
        [UI] private Entry txtTempo = null;
        [UI] private TextView txtPathResult = null;
        //
        [UI] private TreeView trvVentes = null;
        [UI] private TreeView trvAlbums = null;
        [UI] private TreeView trvAuteurs = null;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            // redéfinition du symbole décimal
//			CultureInfo ci = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
//			ci.NumberFormat.NumberDecimalSeparator = ".";
//			Thread.CurrentThread.CurrentCulture = ci;
            //
            builder.Autoconnect(this);
            this.Title = "Librairie BD'Art";
            //
            string strMsg = string.Empty;
            datas = new Datas(this);
            InitTrvVentes();
            InitTrvAlbums();
            InitTrvAuteurs();
            //
            // txtInfo.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtPartAuteur.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtPourcentAuteur.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtQteLibrairie.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtQteMediatheques.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtQteOffert.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtTotalAFacturer.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtTotalCB.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtTotalCheques.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtTotalEspeces.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtTotalLibrairie.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtTotalMediatheques.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtTotalVentes.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            // txtPathResult.ModifyBg(StateType.Normal, new Gdk.Color(220,220,220));
            //
            Pango.FontDescription tpf = new Pango.FontDescription();
			tpf.Weight = Pango.Weight.Bold;
            btnNouvelleVente.ModifyFont(tpf);
            //
            Global.LireConfigLocal(ref strMsg);
			if (strMsg != string.Empty)
                Global.ShowMessage("BdArtLibrairie, lecture configuration:", strMsg, this);
            // events
            btnNouvelleVente.Clicked += OnBtnNouvelleVenteClicked;
            btnReset.Clicked += OnBtnResetClicked;
            chkAFacturer.Clicked += OnChkAFacturerClicked;
            btnFindPrinter.Clicked += OnBtnFindPrinterClicked;
            btnFindUsbDevice.Clicked += OnBtnFindUsbDeviceClicked;
            btnInfoErreurQtes.Clicked += OnBtnInfoErreurQtesClicked;
            chkUseDialogForTicketPrint.Active = Global.UseDialogForTicketPrint;
            chkUseDialogForTicketPrint.Clicked += OnChkUseDialogForTicketPrintClicked;
            chkUseFgColor.Active = Global.UseFgColor;
            chkUseFgColor.Clicked += OnChkUseFgColorClicked;
            //
            cbListeLieuVente.Changed += OnCbListeLieuVenteChanged;
            cbListeAuteurs.Changed += OnCbListeAuteursChanged;
            //
            txtCodeIsbnEan.Activated += OnCodeIsbnEanActivated;
            txtCodeIsbnEan.FocusGrabbed += OnCodeIsbnEanFocusGrabbed;
            txtPassword.FocusOutEvent += OnTxtPasswordFocusOut;
            txtPassword.Activated += OnTxtPasswordActivated;

            txtPrinterFilePath.FocusOutEvent += OnTxtPrinterFilePathFocusOut;
            txtPrinterFilePath.Activated += OnTxtPrinterFilePathActivated;
            txtNombreTickets.FocusOutEvent += OnTxtNombreTicketsFocusOut;
            txtNombreTickets.Activated += OnTxtNombreTicketsActivated;
            txtTempo.FocusOutEvent += OnTxtTempoFocusOut;
            txtTempo.Activated += OnTxtTempoActivated;
            txtUsbDevicePath.FocusOutEvent += OnTxtUsbDevicePathFocusOut;
            txtUsbDevicePath.Activated += OnTxtUsbDevicePathActivated;
            //
            DeleteEvent += delegate { OnMnuFichierQuitter(this, new EventArgs()); };
            //
            mnuFichierExportAlbums.Activated += OnMnuFichierExportAlbums;
            mnuFichierExportFichiers.Activated += OnMnuFichierExportFichiers;
            mnuFichierQuitter.Activated += OnMnuFichierQuitter;
            mnuFichierResetVentes.Activated += OnMnuFichierResetVentes;
            mnuAffichageTout.Toggled += OnMnuAffichageTout;
            mnuAffichageVente.Activated += OnMnuAffichageVente;
            mnuAideApropos.Activated += OnMnuAideApropos;
            //
            trvVentes.RowActivated += OnTrvVentesRowActivated;
			//
			strMsg = string.Empty;
            // chargement des fichiers
            datas.ChargeFichiers(ref strMsg);
            if (strMsg != string.Empty)
            {
				Global.ShowMessage("BdArtLibrairie, lecture fichiers:", strMsg, this);
                Global.AfficheInfo(txtInfo, "Erreur lors du chargement des fichiers", new Gdk.Color(255,0,0));
            }
            else
                Global.AfficheInfo(txtInfo, "Tous les fichiers ont été correctement chargés", new Gdk.Color(0,0,255));
            //
            InitCbListeLieuVente();
            InitCbListeAuteurs();
            DoCalcul();
            UpdateData();
            datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
            datas.DoFiltreDtTableVentes(cbListeAuteurs.ActiveText);
            //
            Global.ConfigModified = false;
        }

        private void OnMnuFichierExportFichiers(object sender, EventArgs e)
        {
            string strMsg = string.Empty;

            Global.ExportFichiers(ref strMsg);
            if (strMsg != string.Empty)
            {
                Global.ShowMessage("BdArtLibrairie, copie fichiers:", strMsg, this);
                Global.AfficheInfo(txtInfo, "Problème lors de la copie des fichiers sur la clé.", new Gdk.Color(255,0,0));
            }
            else
                Global.AfficheInfo(txtInfo, "Les fichiers ont été copiés la clé USB", new Gdk.Color(0,0,255));
        }

        private void OnMnuFichierQuitter(object sender, EventArgs a)
        {
            Application.Quit();
        }

        // Mise à jour des données.
        // <param name="bVersIHM"></param>
        private void UpdateData()
        {
            // données vers IHM
            txtTotalVentes.Text = Math.Round(dblTotalVentes, 2).ToString();
            txtPartAuteur.Text = Math.Round(dblPartAuteur, 2).ToString();
            txtTotalLibrairie.Text = Math.Round(dblTotalLibrairie, 2).ToString();
            txtTotalMediatheques.Text = Math.Round(dblTotalMediatheques, 2).ToString();
            txtTotalAFacturer.Text = Math.Round(dblTotalAFacturer, 2).ToString();
            txtTotalCB.Text = Math.Round(dblTotalCB, 2).ToString();
            txtTotalCheques.Text = Math.Round(dblTotalCheques, 2).ToString();
            txtTotalEspeces.Text = Math.Round(dblTotalEspeces, 2).ToString();
            txtQteMediatheques.Text = nQteMediatheques.ToString();
            txtQteLibrairie.Text = nQteLibrairie.ToString();
            txtQteOffert.Text = nQteOfferts.ToString();
            if (cbListeAuteurs.ActiveText == "Tous")
                txtPourcentAuteur.Text = "---";
            else
                txtPourcentAuteur.Text = Math.Round(dblPourcentAuteur, 2).ToString();
            txtPrinterFilePath.Text = Global.PrinterFilePath;
            txtNombreTickets.Text = Global.NombreTickets.ToString();
            txtTempo.Text = Global.Tempo.ToString();
            txtUsbDevicePath.Text = Global.UsbDevicePath;
        }

        // Calculs.
        private void DoCalcul()
        {
            string strAuteur = string.Empty;
            TreeIter iter;
            
            InitValeurs();
            if (cbListeAuteurs.ActiveText == "Tous")
            {
                // pour chaque auteur
                if (lsListeAuteurs.GetIterFirst(out iter) == true)
                {
                    do
                    {
                        strAuteur = lsListeAuteurs.GetValue(iter, 0).ToString();
                        if (strAuteur != "Tous")
                            DoCalculAuteur(strAuteur);
                    }
                    while (lsListeAuteurs.IterNext(ref iter) == true);
                }
            }
            else
                DoCalculAuteur(cbListeAuteurs.ActiveText);
            //
            if (datas.ErreurStockAlbums != string.Empty)
                btnInfoErreurQtes.Visible = true;
            else
                btnInfoErreurQtes.Visible = false;
        }

        private void OnBtnInfoErreurQtesClicked(object sender, EventArgs e)
        {
            Global.ShowMessage("Erreur quantités", "Le stock final des albums suivants est négatif:\r\n\r\n" + datas.ErreurStockAlbums, this);
        }

        private void DoCalculAuteur(string strAuteur)
        {
            string strIsbnEan = string.Empty;
            double dblPrixVente;
            Int16 nIdAuteur=0;

            // recherche IdAuteur
            foreach (DataRow rowAU in datas.dtTableAuteurs.Select("strAuteur='" + strAuteur + "'"))
            {
                nIdAuteur = Convert.ToInt16(rowAU["nIdAuteur"]);
                dblPourcentAuteur = Convert.ToDouble(rowAU["dblPourcentage"]);
            }
            // pour chaque ligne de vente
            foreach (DataRow rowV in datas.dtTableVentes.Rows)
            {
                if (rowV.RowState == DataRowState.Deleted)
					continue;
                //
                strIsbnEan = rowV["strIsbnEan"].ToString();
                foreach (DataRow rowA in datas.dtTableAlbums.Select("strIsbnEan='" + strIsbnEan + "' AND nIdAuteur=" + nIdAuteur.ToString()))
                {
                    dblPrixVente = Convert.ToDouble(rowA["dblPrixVente"]);

                    // on n'ajoute tout sauf Offerts
                    if (string.Compare(rowV["strPaiement"].ToString(), Global.eMoyenPaiement.Offert.ToString()) != 0)
                    {
                        dblTotalVentes += dblPrixVente;
                        dblPartAuteur += dblPrixVente * dblPourcentAuteur / 100;

                        // sélection dans la table Paiements
                        foreach (DataRow rowP in datas.dtTablePaiements.Select("nNumeroVente=" + Convert.ToInt16(rowV["nNumero"]).ToString()))
                        {
                            if (rowP.RowState == DataRowState.Deleted)
                                continue;
                            // ne pas ajouter les AFacturer
                            if (string.Compare(rowV["strPaiement"].ToString(), Global.eMoyenPaiement.AFacturer.ToString()) != 0)
                            {
                                dblTotalCB += dblPrixVente * Convert.ToDouble(rowP["dblPourcentCB"]) / 100;
                                dblTotalCheques += dblPrixVente * Convert.ToDouble(rowP["dblPourcentCheque"]) / 100;
                                dblTotalEspeces += dblPrixVente * Convert.ToDouble(rowP["dblPourcentEspeces"]) / 100;
                            }
                        }
                        //
                        if (string.Compare(rowV["strPaiement"].ToString(), Global.eMoyenPaiement.AFacturer.ToString()) == 0)
                            dblTotalAFacturer += dblPrixVente;
                        else
                        {
                            if (string.Compare(rowV["strLieu"].ToString(), Global.eListeLieuVente.Librairie.ToString()) == 0)
                                dblTotalLibrairie += dblPrixVente;
                            else if (string.Compare(rowV["strLieu"].ToString(), Global.eListeLieuVente.Médiathèque.ToString()) == 0)
                                dblTotalMediatheques += dblPrixVente;
                        }
                    }
                }
                // update qté vendues
                datas.UpdateAlbumsQteVendues(nIdAuteur, rowV, ref nQteLibrairie, ref nQteMediatheques, ref nQteOfferts);
            }
        }

        private void InitValeurs()
        {
            dblTotalVentes = 0;
            dblTotalLibrairie = dblTotalMediatheques = dblTotalAFacturer = 0;
            dblTotalCB = dblTotalCheques = dblTotalEspeces = 0;
            nQteMediatheques = nQteLibrairie = nQteOfferts = 0;
            dblPartAuteur = dblPourcentAuteur = 0;
            datas.RazTableAlbums();
            datas.ErreurStockAlbums = string.Empty;
        }

        private void InitTrvAlbums()
        {
            trvAlbums.Model = datas.lstoreAlbums;
            //
            TreeViewColumn colIsbnEan = new TreeViewColumn();
            colIsbnEan.Title = "ISBN / EAN";
            TreeViewColumn colAuteur = new TreeViewColumn();
            colAuteur.Title = "Auteur";
            TreeViewColumn colTitre = new TreeViewColumn();
            colTitre.Title = "Titre";
            TreeViewColumn colPrixVente = new TreeViewColumn();
            colPrixVente.Title = "Prix vente (€)";
            TreeViewColumn colStockInitial = new TreeViewColumn();
            colStockInitial.Title = "Stock initial";
            TreeViewColumn colQteVenduLibrairie = new TreeViewColumn();
            colQteVenduLibrairie.Title = "Vendu librairie";
            TreeViewColumn colQteVenduMediat = new TreeViewColumn();
            colQteVenduMediat.Title = "Vendu médiat.";
            TreeViewColumn colQteOffert = new TreeViewColumn();
            colQteOffert.Title = "Qté offert";
            TreeViewColumn colStockFinal = new TreeViewColumn();
            colStockFinal.Title = "Stock final";
            TreeViewColumn colQteAfacturer = new TreeViewColumn();
            colQteAfacturer.Title = "A facturer";
            TreeViewColumn colQteTotalVendu = new TreeViewColumn();
            colQteTotalVendu.Title = "Total vendu";
            TreeViewColumn colPrixTotal = new TreeViewColumn();
            colPrixTotal.Title = "Prix total (€)";
            //
            trvAlbums.AppendColumn(colIsbnEan);
            trvAlbums.AppendColumn(colAuteur);
            trvAlbums.AppendColumn(colTitre);
            trvAlbums.AppendColumn(colPrixVente);
            trvAlbums.AppendColumn(colStockInitial);
            trvAlbums.AppendColumn(colQteVenduLibrairie);
            trvAlbums.AppendColumn(colQteVenduMediat);
            trvAlbums.AppendColumn(colQteOffert);
            trvAlbums.AppendColumn(colStockFinal);
            trvAlbums.AppendColumn(colQteAfacturer);
            trvAlbums.AppendColumn(colQteTotalVendu);
            trvAlbums.AppendColumn(colPrixTotal);
            //
            CellRendererText cellIsbnEan = new CellRendererText();
            colIsbnEan.PackStart(cellIsbnEan, true);
            colIsbnEan.AddAttribute(cellIsbnEan, "text", Convert.ToInt16(Global.eTrvAlbumsCols.CodeIsbnEan));

            CellRendererText cellAuteur = new CellRendererText();
            colAuteur.PackStart(cellAuteur, true);
            colAuteur.AddAttribute(cellAuteur, "text", Convert.ToInt16(Global.eTrvAlbumsCols.Auteur));

            CellRendererText cellTitre = new CellRendererText();
            colTitre.PackStart(cellTitre, true);
            colTitre.AddAttribute(cellTitre, "text", Convert.ToInt16(Global.eTrvAlbumsCols.Titre));

            CellRendererText cellPrixVente = new CellRendererText();
            colPrixVente.PackStart(cellPrixVente, true);
            colPrixVente.AddAttribute(cellPrixVente, "text", Convert.ToInt16(Global.eTrvAlbumsCols.PrixVente));

            CellRendererText cellStockInitial = new CellRendererText();
            colStockInitial.PackStart(cellStockInitial, true);
            colStockInitial.AddAttribute(cellStockInitial, "text", Convert.ToInt16(Global.eTrvAlbumsCols.StockInitial));

            CellRendererText cellQteVenduLibrairie = new CellRendererText();
            colQteVenduLibrairie.PackStart(cellQteVenduLibrairie, true);
            colQteVenduLibrairie.AddAttribute(cellQteVenduLibrairie, "text", Convert.ToInt16(Global.eTrvAlbumsCols.QteVenduLibrairie));

            CellRendererText cellQteVenduMediat = new CellRendererText();
            colQteVenduMediat.PackStart(cellQteVenduMediat, true);
            colQteVenduMediat.AddAttribute(cellQteVenduMediat, "text", Convert.ToInt16(Global.eTrvAlbumsCols.QteVenduMediatheque));

            CellRendererText cellQteOffert = new CellRendererText();
            colQteOffert.PackStart(cellQteOffert, true);
            colQteOffert.AddAttribute(cellQteOffert, "text", Convert.ToInt16(Global.eTrvAlbumsCols.QteOffert));

            CellRendererText cellStockFinal = new CellRendererText();
            colStockFinal.PackStart(cellStockFinal, true);
            colStockFinal.AddAttribute(cellStockFinal, "text", Convert.ToInt16(Global.eTrvAlbumsCols.StockFinal));

            CellRendererText cellQteAfacturer = new CellRendererText();
            colQteAfacturer.PackStart(cellQteAfacturer, true);
            colQteAfacturer.AddAttribute(cellQteAfacturer, "text", Convert.ToInt16(Global.eTrvAlbumsCols.QteAfacturer));

            CellRendererText cellQteTotalVendu = new CellRendererText();
            colQteTotalVendu.PackStart(cellQteTotalVendu, true);
            colQteTotalVendu.AddAttribute(cellQteTotalVendu, "text", Convert.ToInt16(Global.eTrvAlbumsCols.QteTotalVendu));

            CellRendererText cellPrixTotal = new CellRendererText();
            colPrixTotal.PackStart(cellPrixTotal, true);
            colPrixTotal.AddAttribute(cellPrixTotal, "text", Convert.ToInt16(Global.eTrvAlbumsCols.PrixTotal));
        }

        private void InitTrvVentes()
        {
            trvVentes.Model = datas.lstoreVentes;
            //
            TreeViewColumn colNumero = new TreeViewColumn();
            colNumero.Title = "Numéro";
            TreeViewColumn colRang = new TreeViewColumn();
            colRang.Title = "Rang";
            TreeViewColumn colDate = new TreeViewColumn();
            colDate.Title = "Date et heure";
            TreeViewColumn colIsbnEan = new TreeViewColumn();
            colIsbnEan.Title = "ISBN / EAN";
                // dttable albums
                TreeViewColumn colAuteur = new TreeViewColumn();
                colAuteur.Title = "Auteur";
                TreeViewColumn colTitre = new TreeViewColumn();
                colTitre.Title = "Titre";
                TreeViewColumn colPrixVente = new TreeViewColumn();
                colPrixVente.Title = "Prix vente (€)";
                // fin dttable albums
            TreeViewColumn colQteVendu = new TreeViewColumn();
            colQteVendu.Title = "Qté vendu";
            TreeViewColumn colLieu = new TreeViewColumn();
            colLieu.Title = "Lieu";
            TreeViewColumn colPaiement = new TreeViewColumn();
            colPaiement.Title = "Paiement";
            //
            trvVentes.AppendColumn(colNumero);
            trvVentes.AppendColumn(colRang);
            trvVentes.AppendColumn(colDate);
            trvVentes.AppendColumn(colIsbnEan);
            trvVentes.AppendColumn(colAuteur);
            trvVentes.AppendColumn(colTitre);
            trvVentes.AppendColumn(colPrixVente);
            trvVentes.AppendColumn(colQteVendu);
            trvVentes.AppendColumn(colLieu);
            trvVentes.AppendColumn(colPaiement);
            //
            CellRendererText cellNumero = new CellRendererText();
            colNumero.PackStart(cellNumero, true);
            colNumero.AddAttribute(cellNumero, "text", Convert.ToInt16(Global.eTrvVentesCols.Numero));
            colNumero.Visible = false;
            
            CellRendererText cellRang = new CellRendererText();
            colRang.PackStart(cellRang, true);
            colRang.AddAttribute(cellRang, "text", Convert.ToInt16(Global.eTrvVentesCols.Rang));
            colRang.Visible = false;

            CellRendererText cellDate = new CellRendererText();
            colDate.PackStart(cellDate, true);
            colDate.AddAttribute(cellDate, "text", Convert.ToInt16(Global.eTrvVentesCols.Date));
            
            CellRendererText cellIsbnEan = new CellRendererText();
            colIsbnEan.PackStart(cellIsbnEan, true);
            colIsbnEan.AddAttribute(cellIsbnEan, "text", Convert.ToInt16(Global.eTrvVentesCols.CodeIsbnEan));
            colIsbnEan.Visible = false;

            CellRendererText cellAuteur = new CellRendererText();
            colAuteur.PackStart(cellAuteur, true);
            colAuteur.AddAttribute(cellAuteur, "text", Convert.ToInt16(Global.eTrvVentesCols.Auteur));

            CellRendererText cellTitre = new CellRendererText();
            colTitre.PackStart(cellTitre, true);
            colTitre.AddAttribute(cellTitre, "text", Convert.ToInt16(Global.eTrvVentesCols.Titre));

            CellRendererText cellPrixVente = new CellRendererText();
            colPrixVente.PackStart(cellPrixVente, true);
            colPrixVente.AddAttribute(cellPrixVente, "text", Convert.ToInt16(Global.eTrvVentesCols.PrixVente));

            CellRendererText cellQteVendu = new CellRendererText();
            colQteVendu.PackStart(cellQteVendu, true);
            colQteVendu.AddAttribute(cellQteVendu, "text", Convert.ToInt16(Global.eTrvVentesCols.QteVendu));

            CellRendererText cellLieu = new CellRendererText();
            colLieu.PackStart(cellLieu, true);
            colLieu.AddAttribute(cellLieu, "text", Convert.ToInt16(Global.eTrvVentesCols.Lieu));

            CellRendererText cellPaiement = new CellRendererText();
            colPaiement.PackStart(cellPaiement, true);
            colPaiement.AddAttribute(cellPaiement, "text", Convert.ToInt16(Global.eTrvVentesCols.Paiement));
        }

        private void InitTrvAuteurs()
        {
            trvAuteurs.Model = datas.lstoreAuteurs;
            //
            TreeViewColumn colIdAuteur = new TreeViewColumn();
            colIdAuteur.Title = "Id";
            TreeViewColumn colAuteur = new TreeViewColumn();
            colAuteur.Title = "Auteur";
            TreeViewColumn colPourcentage = new TreeViewColumn();
            colPourcentage.Title = "Pourcentage";
            //
            trvAuteurs.AppendColumn(colIdAuteur);
            trvAuteurs.AppendColumn(colAuteur);
            trvAuteurs.AppendColumn(colPourcentage);
            //
            CellRendererText cellIdAuteur = new CellRendererText();
            colIdAuteur.PackStart(cellIdAuteur, true);
            colIdAuteur.AddAttribute(cellIdAuteur, "text", Convert.ToInt16(Global.eTrvAuteursCols.IdAuteur));

            CellRendererText cellAuteur = new CellRendererText();
            colAuteur.PackStart(cellAuteur, true);
            colAuteur.AddAttribute(cellAuteur, "text", Convert.ToInt16(Global.eTrvAuteursCols.Auteur));

            CellRendererText cellPourcentage = new CellRendererText();
            colPourcentage.PackStart(cellPourcentage, true);
            colPourcentage.AddAttribute(cellPourcentage, "text", Convert.ToInt16(Global.eTrvAuteursCols.Pourcentage));
        }

        private void InitCbListeLieuVente()
        {
            cbListeLieuVente.Changed -= OnCbListeLieuVenteChanged;

            cbListeLieuVente.AppendText("Tous");
            cbListeLieuVente.AppendText(Global.eListeLieuVente.Librairie.ToString());
            cbListeLieuVente.AppendText(Global.eListeLieuVente.Médiathèque.ToString());
            cbListeLieuVente.Active = 0;

            cbListeLieuVente.Changed += OnCbListeLieuVenteChanged;
        }
        private void OnCbListeLieuVenteChanged(object sender, EventArgs a)
        {
            txtInfo.Text = string.Empty;
            string strItem = cbListeLieuVente.ActiveText;
            datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
            // on masque ou affiche certaines colonnes
            if (string.Compare(cbListeLieuVente.ActiveText, "Tous") == 0)
            {
                trvAlbums.Columns[Convert.ToInt16(Global.eTrvAlbumsCols.QteVenduMediatheque)].Visible = true;
                trvAlbums.Columns[Convert.ToInt16(Global.eTrvAlbumsCols.QteVenduLibrairie)].Visible = true;
            }
            else if (string.Compare(cbListeLieuVente.ActiveText, Global.eListeLieuVente.Librairie.ToString()) == 0)
            {
                trvAlbums.Columns[Convert.ToInt16(Global.eTrvAlbumsCols.QteVenduMediatheque)].Visible = false;
                trvAlbums.Columns[Convert.ToInt16(Global.eTrvAlbumsCols.QteVenduLibrairie)].Visible = true;
            }
            else
            {
                trvAlbums.Columns[Convert.ToInt16(Global.eTrvAlbumsCols.QteVenduLibrairie)].Visible = false;
                trvAlbums.Columns[Convert.ToInt16(Global.eTrvAlbumsCols.QteVenduMediatheque)].Visible = true;
            }
        }

        private void InitCbListeAuteurs()
        {
            string strNom = string.Empty;

            cbListeAuteurs.Changed -= OnCbListeAuteursChanged;

            lsListeAuteurs.AppendValues("Tous");
            foreach (DataRow row in datas.dtTableAuteurs.Rows)
            {
                lsListeAuteurs.AppendValues(row["strAuteur"].ToString());
            }
            cbListeAuteurs.Model = lsListeAuteurs;
            cbListeAuteurs.Active = 0;

            cbListeAuteurs.Changed += OnCbListeAuteursChanged;
        }

        private void OnCbListeAuteursChanged(object sender, EventArgs a)
        {
            txtInfo.Text = string.Empty;
            DoCalcul();
            UpdateData();
            datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
            datas.DoFiltreDtTableVentes(cbListeAuteurs.ActiveText);
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        // Désactivé: TODO réfléchir à l'utilité de cette fonction, il faut aussi
        // supprimer / modifier la ligne liée dans la table Paiements...
        private void OnBtnSupprimerVenteClicked(object sender, EventArgs a)
        {
            TreeIter iter;
            TreePath chemin;
            TreePath[] chemins;
            chemins = trvVentes.Selection.GetSelectedRows();
            string strMsg = string.Empty;

            // si aucune ligne sélectionnée
            if (chemins.Length == 0)
            {
                Global.ShowMessage("Suppression", "Vous devez sélectionner une ligne à supprimer", this);
                return;
            }
            if (Global.Confirmation(this, "Suppression:", "Voulez-vous vraiment supprimer la ligne sélectionnée ?") == false)
				return;
            chemin = chemins[0];
            if (datas.lstoreVentes.GetIter(out iter, chemin) == true)
            {
                datas.SupprimerVente(iter);
                DoCalcul();
                UpdateData();
                trvVentes.GrabFocus();
                //
                datas.EnregistrerFichiers(ref strMsg);
                if (strMsg != string.Empty)
                {
                    Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this);
                    Global.AfficheInfo(txtInfo, "Problème lors de la mise à jour des ventes. Vérifier les fichiers", new Gdk.Color(255,0,0));
                }
                else
                    Global.AfficheInfo(txtInfo, "Les ventes ont été mises à jour", new Gdk.Color(0,0,255));
            }
        }

        private void OnBtnNouvelleVenteClicked(object sender, EventArgs a)
        {
            string strMsg = string.Empty;

            txtInfo.Text = string.Empty;

            VenteBox venteBox = new VenteBox(this, ref datas);
			venteBox.Run();
            if (venteBox.rResponse == ResponseType.Cancel)
                return;
            //
            DoCalcul();
            UpdateData();
            datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
            datas.DoFiltreDtTableVentes(cbListeAuteurs.ActiveText);
            trvVentes.GrabFocus();
            //
            datas.EnregistrerFichiers(ref strMsg);
            if (strMsg != string.Empty)
            {
                Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this);
                Global.AfficheInfo(txtInfo, "Problème lors de l'enregistrement de la vente. Vérifier les fichiers", new Gdk.Color(255,0,0));
            }
            else
                Global.AfficheInfo(txtInfo, "La vente a été enregistrée", new Gdk.Color(0,0,255));
        }

        // Double-clic sur une ligne.
        private void OnTrvVentesRowActivated(object sender, EventArgs a)
        {
            OnMnuAffichageVente(sender, a);
        }

        // Affiche dans une boite de dialogue les données liées à la vente sélectionnée.
        // But: réimprimer un ticket de caisse.
        private void OnMnuAffichageVente(object sender, EventArgs a)
        {
            TreeIter iter;
            TreePath chemin;
            TreePath[] chemins;
            chemins = trvVentes.Selection.GetSelectedRows();
            Int16 nNumVente;
            DateTime dtDate;

            // si aucune ligne sélectionnée
            if (chemins.Length == 0)
            {
                Global.ShowMessage("Détails vente", "Vous devez sélectionner une ligne de vente", this);
                return;
            }
            chemin = chemins[0];
            if (datas.lstoreVentes.GetIter(out iter, chemin) == true)
            {
                nNumVente = Convert.ToInt16(datas.lstoreVentes.GetValue(iter, Convert.ToInt16(Global.eTrvVentesCols.Numero)));
                dtDate = Convert.ToDateTime(datas.lstoreVentes.GetValue(iter, Convert.ToInt16(Global.eTrvVentesCols.Date)));
                VenteBox venteBox = new VenteBox(this, ref datas, nNumVente, dtDate);
                venteBox.AfficherMasquerBoutons();
			    venteBox.Run();
            }
        }

        private void OnBtnResetClicked(object sender, EventArgs a)
        {
            cbListeLieuVente.Active = 0;
            cbListeAuteurs.Active = 0;
            chkAFacturer.Active = false;
            Global.AfficheInfo(txtInfo, "Les filtres ont été réinitialisés", new Gdk.Color(0,0,255));
        }

        // Interception de la touche Enter
        private void OnCodeIsbnEanActivated(object sender, EventArgs a)
        {
            Entry txtCode = (Entry)sender;
            // Affichage des infos dans une boite de dialogue
            InfoLivreBox infolivreBox = new InfoLivreBox(this, ref datas, txtCode.Text);
			infolivreBox.ShowAll();

            txtCode.Text = string.Empty;
        }

        private void OnCodeIsbnEanFocusGrabbed(object sender, EventArgs a)
        {
            txtInfo.Text = string.Empty;
        }

        private void OnMnuFichierExportAlbums(object sender, EventArgs a)
        {
            string strMsg = string.Empty;

            datas.ExportAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active, ref strMsg);
            if (strMsg != string.Empty)
            {
                Global.ShowMessage("BdArtLibrairie, export albums:", strMsg, this);
                Global.AfficheInfo(txtInfo, "Problème lors de l'export des albums. Vérifier les fichiers", new Gdk.Color(255,0,0));
            }
            else
                Global.AfficheInfo(txtInfo, "Les albums ont été exportés", new Gdk.Color(0,0,255));
        }

        // Affiche ou masque toutes les colonnes des Treeview.
        private void OnMnuAffichageTout(object sender, EventArgs a)
        {
            if (mnuAffichageTout.Active == true)
            {
                trvVentes.Columns[Convert.ToInt16(Global.eTrvVentesCols.Numero)].Visible = true;
                trvVentes.Columns[Convert.ToInt16(Global.eTrvVentesCols.Rang)].Visible = true;
                Global.AfficheInfo(txtInfo, "Toutes les colonnes sont affichées", new Gdk.Color(0,0,255));
            }
            else
            {
                trvVentes.Columns[Convert.ToInt16(Global.eTrvVentesCols.Numero)].Visible = false;
                trvVentes.Columns[Convert.ToInt16(Global.eTrvVentesCols.Rang)].Visible = false;
                Global.AfficheInfo(txtInfo, "Certaines colonnes ont été masquées", new Gdk.Color(0,0,255));
            }
        }

        private void OnMnuAideApropos(object sender, EventArgs a)
        {
            AboutBox aboutBox = new AboutBox(this);
            aboutBox.Run();
            aboutBox.Destroy();
        }
        
        private void OnChkAFacturerClicked(object sender, EventArgs a)
        {
            datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
        }

        private void OnChkUseDialogForTicketPrintClicked(object sender, EventArgs a)
        {
            Global.UseDialogForTicketPrint = chkUseDialogForTicketPrint.Active;
        }

        private void OnChkUseFgColorClicked(object sender, EventArgs a)
        {
            Global.UseFgColor = chkUseFgColor.Active;
        }

        private void OnMnuFichierResetVentes(object sender, EventArgs a)
        {
            string strMsg = "Etes-vous certain de vouloir supprimer toutes les ventes ?\r\n";
            strMsg += "\r\nSi oui, vous devrez saisir un mot de passe pour que l'action soit effectuée";
            if (Global.Confirmation(this, "RAZ ventes", strMsg))
            {
                txtPassword.Visible = true;
                txtPassword.Text = string.Empty;
                txtPassword.GrabFocus();
            }
        }

        // Action de la touche Entrée dans le controle.
        private void OnTxtPasswordActivated(object sender, EventArgs a)
        {
            OnTxtPasswordFocusOut(sender, a);
        }

        private void OnTxtPasswordFocusOut(object sender, EventArgs a)
        {
            string strMsg = string.Empty;

            if (String.Compare(txtPassword.Text, "Password01") == 0)
            {
                datas.RAZVentesPaiements();
                DoCalcul();
                UpdateData();
                datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
                datas.DoFiltreDtTableVentes(cbListeAuteurs.ActiveText);
                datas.EnregistrerFichiers(ref strMsg);
                if (strMsg != string.Empty)
                {
                    Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this, MessageType.Question);
                    Global.AfficheInfo(txtInfo, "Problème lors de l'enregistrement des ventes. Vérifier les fichiers", new Gdk.Color(255,0,0));
                }
                else
                    Global.AfficheInfo(txtInfo, "Toutes les ventes ont été supprimées", new Gdk.Color(0,0,255));
            }
            else
                Global.AfficheInfo(txtInfo, "Password incorrect. RAZ des ventes annulé", new Gdk.Color(255,0,0));
            //
            txtPassword.Visible = false;
        }

        // Action de la touche Entrée dans le controle.
        private void OnTxtPrinterFilePathActivated(object sender, EventArgs a)
        {
            OnTxtPrinterFilePathFocusOut(sender, a);
        }
        private void OnTxtPrinterFilePathFocusOut(object sender, EventArgs a)
        {
            Global.PrinterFilePath = txtPrinterFilePath.Text;
            btnFindPrinter.GrabFocus();
        }

        // Action de la touche Entrée dans le controle.
        private void OnTxtUsbDevicePathActivated(object sender, EventArgs a)
        {
            OnTxtUsbDevicePathFocusOut(sender, a);
        }
        private void OnTxtUsbDevicePathFocusOut(object o, EventArgs a)
        {
            Global.UsbDevicePath = txtUsbDevicePath.Text;
            btnFindUsbDevice.GrabFocus();
            //Global.ShowMessage("", Global.UsbDevicePath, this);
        }

        private void OnTxtNombreTicketsActivated(object sender, EventArgs a)
        {
            OnTxtNombreTicketsFocusOut(sender, a);
        }
        private void OnTxtNombreTicketsFocusOut(object sender, EventArgs a)
        {
            Int16 nVal = Global.NombreTickets;
            txtNombreTickets.FocusOutEvent -= OnTxtNombreTicketsFocusOut;
            if (Global.CheckValeurInt16(this, txtNombreTickets) == true)
            {
                nVal = Convert.ToInt16(txtNombreTickets.Text);
                if (nVal < 1)
                    nVal = 1;
                if (nVal > 3)
                    nVal = 3;
                Global.NombreTickets = nVal;
                UpdateData();
                btnFindPrinter.GrabFocus();
            }
            txtNombreTickets.FocusOutEvent += OnTxtNombreTicketsFocusOut;
        }

        private void OnTxtTempoActivated(object sender, EventArgs a)
        {
            OnTxtTempoFocusOut(sender, a);
        }
        private void OnTxtTempoFocusOut(object sender, EventArgs a)
        {
            Int16 nVal = Global.Tempo;
            txtTempo.FocusOutEvent -= OnTxtTempoFocusOut;
            if (Global.CheckValeurInt16(this, txtTempo) == true)
            {
                nVal = Convert.ToInt16(txtTempo.Text);
                if (nVal < 0)
                    nVal = 0;
                if (nVal > 5000)
                    nVal = 5000;
                Global.Tempo = nVal;
                UpdateData();
                btnFindPrinter.GrabFocus();
            }
            txtTempo.FocusOutEvent += OnTxtTempoFocusOut;
        }

        // Liste les périphériques usb situés dans /dev/usb.
        // Permet de savoir si l'imprimante s'est bien montée en lp1 (défaut).
        private void OnBtnFindPrinterClicked(object sender, EventArgs a)
        {
            Process psExec = null;
            txtPathResult.Buffer.Clear();
            try
            {
                ProcessStartInfo myInfo = new ProcessStartInfo();
                myInfo.WorkingDirectory = "/dev/usb";
                myInfo.CreateNoWindow = true;
                myInfo.FileName = "ls";
                myInfo.Arguments = "";
                myInfo.RedirectStandardOutput = true;
                myInfo.UseShellExecute = false;
                psExec = new Process();
	     		psExec.StartInfo = myInfo;
	     		psExec.Start();
                txtPathResult.Buffer.Text = psExec.StandardOutput.ReadToEnd();
                psExec.WaitForExit(5000); // 5 secondes maxi pour que le process se termine
            }
            catch (Exception ex)
			{
				Global.ShowMessage("Recherche imprimante:", ex.Message, this);
			}
            finally
			{
                try
				{
					if (psExec != null && psExec.HasExited == true)
						psExec.Close();
				}
				catch (System.InvalidOperationException)
				{
					// no process associated
				}
            }
        }

        // Lance la commande permettant d'identifier le point de montage de la clé USB.
        private void OnBtnFindUsbDeviceClicked(object sender, EventArgs a)
        {
            Process psExec = null;
            txtPathResult.Buffer.Clear();
            try
            {
                ProcessStartInfo myInfo = new ProcessStartInfo();
                myInfo.CreateNoWindow = true;
                myInfo.WorkingDirectory = Global.AppStartupPath;
                myInfo.FileName = "findusb.sh";
                myInfo.Arguments = "";
                myInfo.RedirectStandardOutput = true;
                myInfo.UseShellExecute = false;
                psExec = new Process();
	     		psExec.StartInfo = myInfo;
	     		psExec.Start();
                txtPathResult.Buffer.Text = psExec.StandardOutput.ReadToEnd();
                psExec.WaitForExit(5000); // 5 secondes maxi pour que le process se termine
            }
            catch (Exception ex)
			{
				Global.ShowMessage("Recherche clé USB:", ex.Message, this);
			}
            finally
			{
                try
				{
					if (psExec != null && psExec.HasExited == true)
						psExec.Close();
				}
				catch (System.InvalidOperationException)
				{
					// no process associated
				}
            }
        }
    }
}
