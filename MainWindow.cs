/*  Copyright (c) Raphael Borrelli (@Rafbor)

    Ce fichier fait partie de BdArtLibrairie.

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
using System.IO;

namespace BdArtLibrairie
{
    class MainWindow : Window
    {
        Datas datas;
        private string strMsg = string.Empty;
        private double dblTotalVentes, dblPourcentAuteur, dblPartAuteur;
        private double dblTotalLibrairie, dblTotalMediatheques, dblTotalAFacturer;
        private double dblTotalCB, dblTotalCheques, dblTotalEspeces;
        private Int16 nQteMediatheques, nQteLibrairie, nQteOfferts;
        private ListStore lsListeAuteurs = new ListStore(typeof(string));

        [UI] private MenuItem mnuFichierExportAlbums = null;
        [UI] private MenuItem mnuFichierExportFichiers = null;
        [UI] private MenuItem mnuFichierQuitter = null;
        [UI] private MenuItem mnuFichierResetVentes = null;
        [UI] private MenuItem mnuFichierRecharger = null;
        [UI] private MenuItem mnuFichierReTelecharger = null;
        [UI] private MenuItem mnuFichierPurgerSauve = null;
        [UI] private CheckMenuItem mnuAffichageTout = null;
        [UI] private MenuItem mnuAffichageVente = null;
        [UI] private MenuItem mnuAffichageAuteur = null;
        [UI] private MenuItem mnuAffichageAlbum = null;
        [UI] private MenuItem mnuAffichageBilanVentes = null;
        [UI] private MenuItem mnuAideApropos = null;
        //
        [UI] private Button btnNouvelleVente = null;
        [UI] private Button btnReset = null;
        [UI] private Button btnFindPrinter = null;
        [UI] private Button btnFindUsbDevice = null;
        [UI] private Button btnInfoErreurQtes = null;
        [UI] private Button btnAjouterAuteur = null;
        [UI] private Button btnSupprimerAuteur = null;
        [UI] private Button btnDetailsAuteur = null;
        [UI] private Button btnAjouterAlbum = null;
        [UI] private Button btnSupprimerAlbum = null;
        [UI] private Button btnDetailsAlbum = null;
        [UI] private Button btnDetailsVente = null;
        [UI] private Button btnBilanVentes = null;
        [UI] private Button btnErreurEcartVentes = null;
        [UI] private Button btnOuvrirDossierFichiers = null;
        [UI] private CheckButton chkAFacturer = null;
        [UI] private CheckButton chkUseDialogForTicketPrint = null;
        [UI] private CheckButton chkAppliquerCss = null;
        [UI] private CheckButton chkLaunchBaseFile = null;
        [UI] private CheckButton chkJouerSons = null;
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
        [UI] private Entry txtNomFestival = null;
        [UI] private Entry txtPartAuteurDefaut = null;
        [UI] private Entry txtVenteBoxWidth = null;
        [UI] private Entry txtVenteBoxHeight = null;
        [UI] private Entry txtDossierFichiers = null;
        //
        [UI] private TreeView trvVentes = null;
        [UI] private TreeView trvAlbums = null;
        [UI] private TreeView trvAuteurs = null;

        public MainWindow() : this(new Builder("MainWindow.glade"))
        {
            // timer pour afficher les messages d'erreurs au chargement
            // après l'affichage de la fenêtre principale
            GLib.Timeout.Add(1000, new GLib.TimeoutHandler(Update_status));
        }
        private bool Update_status()
        {
            if (strMsg != string.Empty)
            {
                Global.ShowMessage("Erreurs au chargement:", strMsg, this);
            }
            // returning false would terminate the timeout.
            return false;
        }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            // redéfinition du symbole décimal
//			CultureInfo ci = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
//			ci.NumberFormat.NumberDecimalSeparator = ".";
//			Thread.CurrentThread.CurrentCulture = ci;
            //
            builder.Autoconnect(this);
            //
            datas = new Datas(this);
            InitTrvVentes();
            InitTrvAlbums();
            InitTrvAuteurs();
            //
            Pango.FontDescription tpf = new Pango.FontDescription();
			tpf.Weight = Pango.Weight.Bold;
            btnNouvelleVente.ModifyFont(tpf);
            //
            Global.LireConfigLocal(ref strMsg);
			if (strMsg != string.Empty)
                strMsg = "Lecture configuration: " + strMsg + "\n\n";
            // styles css
            InitCss();
            // events
            btnNouvelleVente.Clicked += OnBtnNouvelleVenteClicked;
            btnReset.Clicked += OnBtnResetClicked;
            chkAFacturer.Clicked += OnChkAFacturerClicked;
            btnFindPrinter.Clicked += OnBtnFindPrinterClicked;
            btnFindUsbDevice.Clicked += OnBtnFindUsbDeviceClicked;
            btnInfoErreurQtes.Clicked += OnBtnInfoErreurQtesClicked;
            chkUseDialogForTicketPrint.Active = Global.UseDialogForTicketPrint;
            chkUseDialogForTicketPrint.Clicked += OnChkUseDialogForTicketPrintClicked;
            chkAppliquerCss.Clicked += OnChkAppliquerCssClicked;
            chkLaunchBaseFile.Active = Global.LaunchBaseFile;
            chkLaunchBaseFile.Clicked += OnChkLaunchBaseFileClicked;
            chkJouerSons.Active = Global.JouerSons;
            chkJouerSons.Clicked += OnChkJouerSonsClicked;
            btnAjouterAuteur.Clicked += OnBtnAjouterAuteurClicked;
            btnSupprimerAuteur.Clicked += OnBtnSupprimerAuteurClicked;
            btnAjouterAlbum.Clicked += OnBtnAjouterAlbumClicked;
            btnSupprimerAlbum.Clicked += OnBtnSupprimerAlbumClicked;
            btnDetailsAlbum.Clicked += OnBtnDetailsAlbumClicked;
            btnDetailsAuteur.Clicked += OnBtnDetailsAuteurClicked;
            btnDetailsVente.Clicked += OnBtnDetailsVenteClicked;
            btnBilanVentes.Clicked += OnBtnBilanVentesClicked;
            btnErreurEcartVentes.Clicked += OnBtnErreurEcartVentes;
            btnOuvrirDossierFichiers.Clicked += OnBtnOuvrirDossierFichiers;
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
            txtNombreTickets.Changed += OnTxtNombreTicketsChanged;
            txtTempo.FocusOutEvent += OnTxtTempoFocusOut;
            txtTempo.Changed += OnTxtTempoChanged;
            txtUsbDevicePath.FocusOutEvent += OnTxtUsbDevicePathFocusOut;
            txtUsbDevicePath.Activated += OnTxtUsbDevicePathActivated;
            txtNomFestival.FocusOutEvent += OnTxtNomFestivalFocusOut;
            txtNomFestival.Activated += OnTxtNomFestivalActivated;
            txtPartAuteurDefaut.FocusOutEvent += OnTxtPartAuteurDefautFocusOut;
            txtPartAuteurDefaut.Changed += OnTxtPartAuteurDefautChanged;
            txtVenteBoxWidth.FocusOutEvent += OnTxtVenteBoxWidthFocusOut;
            txtVenteBoxWidth.Changed += OnTxtVenteBoxWidthChanged;
            txtVenteBoxHeight.FocusOutEvent += OnTxtVenteBoxHeightFocusOut;
            txtVenteBoxHeight.Changed += OnTxtVenteBoxHeightChanged;
            //
            DeleteEvent += delegate { OnMnuFichierQuitter(this, new EventArgs()); };
            //
            mnuFichierExportAlbums.Activated += OnMnuFichierExportAlbums;
            mnuFichierExportFichiers.Activated += OnMnuFichierExportFichiers;
            mnuFichierQuitter.Activated += OnMnuFichierQuitter;
            mnuFichierResetVentes.Activated += OnMnuFichierResetVentes;
            mnuFichierRecharger.Activated += OnMnuFichierRecharger;
            mnuFichierReTelecharger.Activated += OnMnuFichierReTelecharger;
            mnuFichierPurgerSauve.Activated += OnMnuFichierPurgerSauve;
            mnuAffichageTout.Toggled += OnMnuAffichageTout;
            mnuAffichageVente.Activated += OnMnuAffichageVente;
            mnuAffichageAuteur.Activated += OnMnuAffichageAuteur;
            mnuAffichageAlbum.Activated += OnMnuAffichageAlbum;
            mnuAffichageBilanVentes.Activated += OnMnuAffichageBilanVentes;
            mnuAideApropos.Activated += OnMnuAideApropos;
            //
            trvVentes.RowActivated += OnTrvVentesRowActivated;
            trvAuteurs.RowActivated += OnTrvAuteursRowActivated;
            trvAlbums.RowActivated += OnTrvAlbumsRowActivated;
            // chargement des fichiers
            string strMsg2 = string.Empty;
            datas.ChargerFichiers(ref strMsg2);
            if (strMsg2 != string.Empty)
            {
                if (strMsg != string.Empty)
                    strMsg += Environment.NewLine + Environment.NewLine;
                strMsg += "Lecture fichiers: " + strMsg2;
            }
            // fichier BdArtLib.odb
            if (!File.Exists(Global.FichierBdArtLibOdb))
                Global.DownloadFile(Global.UriBdArtLibOdb, Global.FichierBdArtLibOdb, this);
            // fichier error.wav
            if (!File.Exists(Global.FichierErrorWav))
                Global.DownloadFile(Global.UriErrorWav, Global.FichierErrorWav, this);
            if (strMsg != string.Empty)
                Global.AfficheInfo(ref txtInfo, "Erreur lors du chargement des fichiers", Global.eCssClasses.InfoColorRed);
            else
                Global.AfficheInfo(ref txtInfo, "Tous les fichiers ont été chargés", Global.eCssClasses.InfoColorBlue);
            //
            InitCbListeLieuVente();
            InitCbListeAuteurs();
            chkAppliquerCss.Active = Global.AppliquerCss;// déclenche l'event si true
            DoCalcul();
            UpdateData();
            datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
            datas.DoFiltreDtTableVentes(cbListeAuteurs.ActiveText);
            mnuAffichageTout.Active = true;
            //
            CheckErreurEcartVentes();
            Global.ConfigModified = false;
        }

        private void OnChkJouerSonsClicked(object sender, EventArgs e)
        {
            Global.JouerSons = chkJouerSons.Active;
            Global.ConfigModified = true;
        }

        private void OnBtnOuvrirDossierFichiers(object sender, EventArgs e)
        {
            // ouverture du dossier dans un process
            try
            {
                Process psBase = new Process();
                psBase.StartInfo.FileName = "xdg-open";
                psBase.StartInfo.Arguments = Global.DossierFichiers;
                //psBase.StartInfo.WorkingDirectory = Global.DossierFichiers;
                if (psBase.Start() == true)// nouveau process créé
                {
                    psBase.WaitForExit(1000);
                }
            }
            catch (Exception ex)
            {
                Global.ShowMessage("Erreur ouverture du dossier:", ex.Message, this);
            }
        }

        private void OnMnuFichierReTelecharger(object sender, EventArgs e)
        {
            if (Global.Confirmation(this, "Retélécharger:", "Voulez-vous vraiment retélécharger le fichier BdArtLib.odb ?") == false)
                return;
            Global.DownloadFile(Global.UriBdArtLibOdb, Global.FichierBdArtLibOdb, this);
            Global.AfficheInfo(ref txtInfo, "Téléchargement de BdArtLib.odb en asynchrone", Global.eCssClasses.InfoColorBlue);
        }

        private void OnTxtVenteBoxHeightChanged(object sender, EventArgs e)
        {
            txtVenteBoxHeight.Changed -= OnTxtVenteBoxHeightChanged;
            Global.CheckValeurInt16(this, sender);
            txtVenteBoxHeight.Changed += OnTxtVenteBoxHeightChanged;
        }

        private void OnTxtVenteBoxHeightFocusOut(object o, FocusOutEventArgs args)
        {
            Int16 nVal;
            txtVenteBoxHeight.FocusOutEvent -= OnTxtVenteBoxHeightFocusOut;
            nVal = Global.GetValueIntOrZero(this, o, true);
            if (nVal < 470) nVal = 470;
            if (nVal > 700) nVal = 700;
            Global.VenteBoxHeight = nVal;
            txtVenteBoxHeight.Text = Global.VenteBoxHeight.ToString();
            txtVenteBoxHeight.FocusOutEvent += OnTxtVenteBoxHeightFocusOut;
            Global.ConfigModified = true;
        }

        private void OnTxtVenteBoxWidthChanged(object sender, EventArgs e)
        {
            txtVenteBoxWidth.Changed -= OnTxtVenteBoxWidthChanged;
            Global.CheckValeurInt16(this, sender);
            txtVenteBoxWidth.Changed += OnTxtVenteBoxWidthChanged;
        }

        private void OnTxtVenteBoxWidthFocusOut(object o, FocusOutEventArgs args)
        {
            Int16 nVal;
            txtVenteBoxWidth.FocusOutEvent -= OnTxtVenteBoxWidthFocusOut;
            nVal = Global.GetValueIntOrZero(this, o, true);
            if (nVal < 720) nVal = 720;
            if (nVal > 1000) nVal = 1000;
            Global.VenteBoxWidth = nVal;
            txtVenteBoxWidth.Text = Global.VenteBoxWidth.ToString();
            txtVenteBoxWidth.FocusOutEvent += OnTxtVenteBoxWidthFocusOut;
            Global.ConfigModified = true;
        }

        private void OnMnuFichierPurgerSauve(object sender, EventArgs e)
        {
            string strMsg = string.Empty;

            // suppression dossier de sauvegarde
            if (Global.Confirmation(this, "Purge sauvegarde", "Etes-vous certain de vouloir purger le dossier de sauvegarde ?") == true)
            {
                datas.PurgerDossierSauve(ref strMsg);
                if (strMsg != string.Empty)
                {
                    Global.ShowMessage("BdArtLibrairie, purge dossier:", strMsg, this);
                    Global.AfficheInfo(ref txtInfo, "Problème lors de la purge du dossier de sauvegarde", Global.eCssClasses.InfoColorRed);
                }
                else
                    Global.AfficheInfo(ref txtInfo, "Le dossier de sauvegarde a été purgé", Global.eCssClasses.InfoColorBlue);
            }
        }

        // Ajout des providers CSS
        private void InitCss()
        {
            // champs non éditables
            txtInfo.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtPartAuteur.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtPourcentAuteur.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtQteLibrairie.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtQteMediatheques.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtQteOffert.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtTotalAFacturer.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtTotalCB.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtTotalCheques.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtTotalEspeces.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtTotalLibrairie.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtTotalMediatheques.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtTotalVentes.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtPathResult.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtDossierFichiers.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            // champs éditables
            txtPrinterFilePath.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtUsbDevicePath.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtNombreTickets.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtTempo.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtNomFestival.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtPartAuteurDefaut.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtCodeIsbnEan.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtVenteBoxWidth.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            txtVenteBoxHeight.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            // listebox
            cbListeAuteurs.Child.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            cbListeLieuVente.Child.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
            // boutons
            btnReset.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
        }

        private void AddCssClass()
        {
            // champs non éditables
            txtInfo.StyleContext.AddClass(Global.eCssClasses.InfoColorBlue.ToString());
            txtPartAuteur.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtPourcentAuteur.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtQteLibrairie.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtQteMediatheques.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtQteOffert.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalAFacturer.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalCB.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalCheques.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalEspeces.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalLibrairie.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalMediatheques.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalVentes.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtPathResult.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtDossierFichiers.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
            // champs éditables
            txtPrinterFilePath.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());
            txtUsbDevicePath.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());
            txtNombreTickets.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());
            txtTempo.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());
            txtNomFestival.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());
            txtPartAuteurDefaut.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());
            txtCodeIsbnEan.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());
            txtVenteBoxWidth.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());
            txtVenteBoxHeight.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());
            // listebox
            if (string.Compare(cbListeAuteurs.ActiveText, "Tous") != 0)
                cbListeAuteurs.Child.StyleContext.AddClass(Global.eCssClasses.ListesColors.ToString());

            if (string.Compare(cbListeLieuVente.ActiveText, "Tous") != 0)
                cbListeLieuVente.Child.StyleContext.AddClass(Global.eCssClasses.ListesColors.ToString());
        }

        // Suppression des classes CSS affectées aux widgets.
        private void RemoveCssClasses()
        {
            // champs non éditables
            // le champ txtInfo contient soit InfoColorBlue, soit InfoColorRed
			if (txtInfo.StyleContext.HasClass(Global.eCssClasses.InfoColorBlue.ToString()) == true)
				txtInfo.StyleContext.RemoveClass(Global.eCssClasses.InfoColorBlue.ToString());
			else
				txtInfo.StyleContext.RemoveClass(Global.eCssClasses.InfoColorRed.ToString());
            //
            txtPartAuteur.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtPourcentAuteur.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtQteLibrairie.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtQteMediatheques.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtQteOffert.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalAFacturer.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalCB.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalCheques.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalEspeces.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalLibrairie.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalMediatheques.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtTotalVentes.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtPathResult.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            txtDossierFichiers.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
            // champs éditables
            txtPrinterFilePath.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
            txtUsbDevicePath.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
            txtNombreTickets.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
            txtTempo.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
            txtNomFestival.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
            txtPartAuteurDefaut.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
            txtCodeIsbnEan.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
            txtVenteBoxWidth.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
            txtVenteBoxHeight.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
            // listbox: contient uniquement la classe ListesColors
            Global.RemoveCssClass(ref cbListeAuteurs, Global.eCssClasses.ListesColors);
            Global.RemoveCssClass(ref cbListeLieuVente, Global.eCssClasses.ListesColors);
        }

        private void OnBtnErreurEcartVentes(object sender, EventArgs e)
        {
            string strMsg = string.Empty;

            datas.ChargerFicErrEcartVentes(ref strMsg);
            if (strMsg != string.Empty)
            {
                Global.ShowMessage("BdArtLibrairie, lecture fichier EcartsVentes:", strMsg, this);
            }
            CheckErreurEcartVentes();
        }

        private void OnBtnBilanVentesClicked(object sender, EventArgs e)
        {
            OnMnuAffichageBilanVentes(sender, e);
        }

        private void OnTxtPartAuteurDefautChanged(object sender, EventArgs e)
        {
            txtPartAuteurDefaut.Changed -= OnTxtPartAuteurDefautChanged;
            Global.CheckValeurs(this, sender);
            txtPartAuteurDefaut.Changed += OnTxtPartAuteurDefautChanged;
        }

        private void OnTxtPartAuteurDefautFocusOut(object o, EventArgs args)
        {
            double dblVal;
            txtPartAuteurDefaut.FocusOutEvent -= OnTxtPartAuteurDefautFocusOut;
            dblVal = Global.GetValueOrZero(this, o, true);
            if (dblVal < 0) dblVal = 0;
            if (dblVal > 100) dblVal = 100;
            Global.PartAuteurDefaut = dblVal;
            txtPartAuteurDefaut.Text = Global.PartAuteurDefaut.ToString();
            txtPartAuteurDefaut.FocusOutEvent += OnTxtPartAuteurDefautFocusOut;
            Global.ConfigModified = true;
        }

        private void OnBtnDetailsAlbumClicked(object sender, EventArgs e)
        {
            OnMnuAffichageAlbum(sender, e);
        }

        private void OnBtnSupprimerAlbumClicked(object sender, EventArgs e)
        {
            TreeIter iter;
            TreePath chemin;
            TreePath[] chemins;
            chemins = trvAlbums.Selection.GetSelectedRows();
            string strCode;
            string strMsg = string.Empty, strMsg2;

            // si aucune ligne sélectionnée
            if (chemins.Length == 0)
            {
                Global.ShowMessage("Album", "Vous devez sélectionner un album", this);
                return;
            }
            //
            chemin = chemins[0];
            if (datas.lstoreAlbums.GetIter(out iter, chemin) == true)
            {
                strCode = datas.lstoreAlbums.GetValue(iter, Convert.ToInt16(Global.eTrvAlbumsCols.CodeIsbnEan)).ToString();
                if (Global.Confirmation(this, "Suppression Album", "Etes-vous certain de vouloir supprimer l'album " + strCode + Environment.NewLine) == false)
                    return;
                // on vérifie d'abord si l'album a déjà été vendu
                foreach(DataRow rowV in datas.dtTableVentes.Select("strIsbnEan='" + strCode + "'"))
                {
                    strMsg += "Vente:" + rowV["nNumero"].ToString() + " Rang:" + rowV["nRang"].ToString() + Environment.NewLine;
                }
                if (strMsg != string.Empty)
                {
                    strMsg2 = "L'album ne peut pas être supprimé car déjà présent dans les ventes suivantes:" + Environment.NewLine + strMsg;
                    Global.ShowMessage("Suppression album", strMsg2, this, MessageType.Error);
                }
                else
                {
                    datas.SupprimerAlbum(iter, strCode);
                    datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
                    datas.EnregistrerFichierAlbums(ref strMsg);
                    if (strMsg != string.Empty)
                    {
                        Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this);
                        Global.AfficheInfo(ref txtInfo, "Problème lors de la mise à jour des albums. Vérifier le fichier", Global.eCssClasses.InfoColorRed);
                    }
                    else
                        Global.AfficheInfo(ref txtInfo, "L'album a été supprimé", Global.eCssClasses.InfoColorBlue);
                }
            }
        }

        private void OnBtnAjouterAlbumClicked(object sender, EventArgs e)
        {
            string strMsg = string.Empty;
            Int16 nIdAuteur=0;

            // il faut préalablement sélectionner l'auteur
            if (cbListeAuteurs.ActiveText == "Tous")
            {
                Global.ShowMessage("Ajout d'un album:", "Vous devez d'abord sélectionner l'auteur de l'album", this);
                return;
            }
            else
            {
                // recherche IdAuteur
                foreach (DataRow rowAU in datas.dtTableAuteurs.Select("strAuteur='" + cbListeAuteurs.ActiveText + "'"))
                    nIdAuteur = Convert.ToInt16(rowAU["nIdAuteur"]);
            }
            //
            AlbumBox albumBox = new AlbumBox(this, ref datas, string.Empty, true, nIdAuteur, cbListeAuteurs.ActiveText);
            albumBox.Run();
            if (albumBox.rResponse == ResponseType.Apply)
            {
                DoCalcul();
                datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
                datas.EnregistrerFichierAlbums(ref strMsg);
                if (strMsg != string.Empty)
                {
                    Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this);
                    Global.AfficheInfo(ref txtInfo, "Problème lors de la mise à jour des albums. Vérifier le fichier", Global.eCssClasses.InfoColorRed);
                }
                else
                    Global.AfficheInfo(ref txtInfo, "L'album a été ajouté", Global.eCssClasses.InfoColorBlue);
            }
        }

        private void OnBtnSupprimerAuteurClicked(object sender, EventArgs e)
        {
            TreeIter iter;
            TreePath chemin;
            TreePath[] chemins;
            chemins = trvAuteurs.Selection.GetSelectedRows();
            string strMsg = string.Empty;
            Int16 nIdAuteur, nNbAlbums=0;

            // si aucune ligne sélectionnée
            if (chemins.Length == 0)
            {
                Global.ShowMessage("Auteur", "Vous devez sélectionner un auteur", this);
                return;
            }
            //
            chemin = chemins[0];
            if (datas.lstoreAuteurs.GetIter(out iter, chemin) == true)
            {
                nIdAuteur = Convert.ToInt16(datas.lstoreAuteurs.GetValue(iter, Convert.ToInt16(Global.eTrvAuteursCols.IdAuteur)));
                if (Global.Confirmation(this, "Suppression Auteur", "Etes-vous certain de vouloir supprimer l'auteur n°" + nIdAuteur.ToString() + Environment.NewLine) == false)
                    return;
                // on vérifie que l'auteur n'a pas d'albums
                foreach (DataRow row in datas.dtTableAlbums.Select("nIdAuteur=" + nIdAuteur.ToString()))
                    nNbAlbums++;
                if (nNbAlbums == 0)
                {
                    datas.SupprimerAuteur(iter, nIdAuteur);                    
                    datas.EnregistrerFichierAuteurs(ref strMsg);
                    if (strMsg != string.Empty)
                    {
                        Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this);
                        Global.AfficheInfo(ref txtInfo, "Problème lors de la mise à jour des auteurs. Vérifier le fichier", Global.eCssClasses.InfoColorRed);
                    }
                    else
                        Global.AfficheInfo(ref txtInfo, "Les auteurs ont été mis à jour", Global.eCssClasses.InfoColorBlue);
                }
                else
                    Global.ShowMessage("Suppression Auteur", "Cet auteur ne peut pas être supprimé car il possède " + nNbAlbums.ToString() + " albums", this);
            }
        }

        private void OnBtnAjouterAuteurClicked(object sender, EventArgs e)
        {
            string strMsg = string.Empty;
            
            Int16 nIdAuteur = datas.GetNewIdAuteur();
            AuteurBox auteurBox = new AuteurBox(this, ref datas, nIdAuteur, true);
            auteurBox.Run();
            if (auteurBox.rResponse == ResponseType.Apply)
            {
                datas.DoRefreshLstoreAuteurs();
                InitCbListeAuteurs();
                datas.EnregistrerFichierAuteurs(ref strMsg);
                if (strMsg != string.Empty)
                {
                    Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this);
                    Global.AfficheInfo(ref txtInfo, "Problème lors de la mise à jour des auteurs. Vérifier le fichier", Global.eCssClasses.InfoColorRed);
                }
                else
                    Global.AfficheInfo(ref txtInfo, "L'auteur a été ajouté", Global.eCssClasses.InfoColorBlue);
            }
        }

        // Double-clic sur ligne album.
        private void OnTrvAlbumsRowActivated(object o, RowActivatedArgs args)
        {
            OnMnuAffichageAlbum(o, args);
        }

        private void OnMnuAffichageAlbum(object sender, EventArgs e)
        {
            TreeIter iter;
            TreePath chemin;
            TreePath[] chemins;
            chemins = trvAlbums.Selection.GetSelectedRows();
            string strCode;

            // si aucune ligne sélectionnée
            if (chemins.Length == 0)
            {
                Global.ShowMessage("Infos album", "Vous devez sélectionner un album", this);
                return;
            }
            chemin = chemins[0];
            if (datas.lstoreAlbums.GetIter(out iter, chemin) == true)
            {
                strCode = datas.lstoreAlbums.GetValue(iter, Convert.ToInt16(Global.eTrvAlbumsCols.CodeIsbnEan)).ToString();
                AlbumBox AlbumBox = new AlbumBox(this, ref datas, strCode);
                AlbumBox.Run();
                if (AlbumBox.rResponse == ResponseType.Apply)
                    AppliquerModifAlbum();
            }
        }

        private void AppliquerModifAlbum()
        {
            string strMsg = string.Empty;

            DoCalcul();
            UpdateData();
            datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
            datas.EnregistrerFichierAlbums(ref strMsg);
            if (strMsg != string.Empty)
            {
                Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this);
                Global.AfficheInfo(ref txtInfo, "Problème lors de la mise à jour des albums. Vérifier le fichier", Global.eCssClasses.InfoColorRed);
            }
            else
                Global.AfficheInfo(ref txtInfo, "Les albums ont été mis à jour", Global.eCssClasses.InfoColorBlue);
            CheckErreurEcartVentes();
        }

        private void OnBtnDetailsAuteurClicked(object sender, EventArgs e)
        {
            OnMnuAffichageAuteur(sender, e);
        }

        // Double-clic sur ligne auteur.
        private void OnTrvAuteursRowActivated(object o, RowActivatedArgs args)
        {
            OnMnuAffichageAuteur(o, args);
        }

        // Affiche dans une boite de dialogue les données liées à l'auteur sélectionné.
        private void OnMnuAffichageAuteur(object sender, EventArgs e)
        {
            TreeIter iter;
            TreePath chemin;
            TreePath[] chemins;
            chemins = trvAuteurs.Selection.GetSelectedRows();
            Int16 nIdAuteur;
            string strMsg = string.Empty;

            // si aucune ligne sélectionnée
            if (chemins.Length == 0)
            {
                Global.ShowMessage("Infos auteur", "Vous devez sélectionner un auteur", this);
                return;
            }
            chemin = chemins[0];
            if (datas.lstoreAuteurs.GetIter(out iter, chemin) == true)
            {
                nIdAuteur = Convert.ToInt16(datas.lstoreAuteurs.GetValue(iter, Convert.ToInt16(Global.eTrvAuteursCols.IdAuteur)));
                AuteurBox auteurBox = new AuteurBox(this, ref datas, nIdAuteur);
			    auteurBox.Run();
                if (auteurBox.rResponse == ResponseType.Apply)
                {
                    datas.DoRefreshLstoreAuteurs();
                    InitCbListeAuteurs();
                    DoCalcul();
                    UpdateData();
                    datas.EnregistrerFichierAuteurs(ref strMsg);
                    if (strMsg != string.Empty)
                    {
                        Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this);
                        Global.AfficheInfo(ref txtInfo, "Problème lors de la mise à jour des auteurs. Vérifier le fichier", Global.eCssClasses.InfoColorRed);
                    }
                    else
                        Global.AfficheInfo(ref txtInfo, "Les auteurs ont été mis à jour", Global.eCssClasses.InfoColorBlue);
                }
            }
        }

        private void OnMnuFichierRecharger(object sender, EventArgs e)
        {
            if (Global.Confirmation(this, "Recharger:", "Voulez-vous vraiment recharger les fichiers de données ?") == false)
				return;
            datas.Init();
            string strMsg = string.Empty;
            datas.ChargerFichiers(ref strMsg);
            if (strMsg != string.Empty)
            {
                strMsg = "Lecture fichiers: " + strMsg;
                Global.ShowMessage("Erreurs au chargement:", strMsg, this);
                Global.AfficheInfo(ref txtInfo, "Problème lors du chargement des fichiers", Global.eCssClasses.InfoColorRed);
            }
            else
                Global.AfficheInfo(ref txtInfo, "Les fichiers ont été chargés", Global.eCssClasses.InfoColorBlue);
            InitCbListeAuteurs();
            DoCalcul();
            UpdateData();
            datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
            datas.DoFiltreDtTableVentes(cbListeAuteurs.ActiveText);
            CheckErreurEcartVentes();
        }

        private void OnChkLaunchBaseFileClicked(object sender, EventArgs e)
        {
            Global.LaunchBaseFile = chkLaunchBaseFile.Active;
            Global.ConfigModified = true;
        }

        private void OnTxtNomFestivalActivated(object sender, EventArgs e)
        {
            OnTxtNomFestivalFocusOut(sender, e);
        }

        private void OnTxtNomFestivalFocusOut(object o, EventArgs args)
        {
            Global.NomFestival = txtNomFestival.Text;
            this.Title = "Librairie " + Global.NomFestival;
            Global.ConfigModified = true;
        }

        private void OnMnuFichierExportFichiers(object sender, EventArgs e)
        {
            string strMsg = string.Empty;

            Global.ExportFichiers(ref strMsg);
            if (strMsg != string.Empty)
            {
                Global.ShowMessage("BdArtLibrairie, copie fichiers:", strMsg, this);
                Global.AfficheInfo(ref txtInfo, "Problème lors de la copie des fichiers sur la clé.", Global.eCssClasses.InfoColorRed);
            }
            else
                Global.AfficheInfo(ref txtInfo, "Les fichiers ont été copiés sur la clé USB", Global.eCssClasses.InfoColorBlue);
        }

        private void OnMnuFichierQuitter(object sender, EventArgs a)
        {
            string strMsg = string.Empty;

            if (Global.ConfigModified == true)
            {
                // écriture de la config locale
                Global.EcrireConfigLocal(ref strMsg);
                if (strMsg != string.Empty)
                    Global.ShowMessage("Erreur écriture config:", strMsg, this);
            }
            //
            Application.Quit();
        }

        // Mise à jour des données.
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
            txtNomFestival.Text = Global.NomFestival;
            this.Title = "Librairie " + Global.NomFestival;
            txtPartAuteurDefaut.Text = Global.PartAuteurDefaut.ToString();
            txtVenteBoxWidth.Text = Global.VenteBoxWidth.ToString();
            txtVenteBoxHeight.Text = Global.VenteBoxHeight.ToString();
            txtDossierFichiers.Text = Global.DossierFichiers;
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
            Global.ShowMessage("Erreur quantités", "Le stock final des albums suivants est négatif:" + Environment.NewLine + Environment.NewLine + datas.ErreurStockAlbums, this);
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
            //
            if (Global.AppliquerCss == true)
            {
                if (string.Compare(cbListeLieuVente.ActiveText, "Tous") == 0)
                    Global.RemoveCssClass(ref cbListeLieuVente, Global.eCssClasses.ListesColors);
                else
                    cbListeLieuVente.Child.StyleContext.AddClass(Global.eCssClasses.ListesColors.ToString());
            }
        }

        private void InitCbListeAuteurs()
        {
            string strNom = string.Empty;

            cbListeAuteurs.Changed -= OnCbListeAuteursChanged;
            lsListeAuteurs.Clear();
            lsListeAuteurs.AppendValues("Tous");
            foreach (DataRow row in datas.dtTableAuteurs.Select("1=1", "strAuteur ASC"))
            {
                lsListeAuteurs.AppendValues(row["strAuteur"].ToString());
            }
            cbListeAuteurs.Model = lsListeAuteurs;
            cbListeAuteurs.Active = 0;

            cbListeAuteurs.Changed += OnCbListeAuteursChanged;
            UpdateCssAuteur();
        }

        private void UpdateCssAuteur()
        {
            if (Global.AppliquerCss == true)
            {
                if (string.Compare(cbListeAuteurs.ActiveText, "Tous") == 0)
                    Global.RemoveCssClass(ref cbListeAuteurs, Global.eCssClasses.ListesColors);
                else
                    cbListeAuteurs.Child.StyleContext.AddClass(Global.eCssClasses.ListesColors.ToString());
            }
        }

        private void OnCbListeAuteursChanged(object sender, EventArgs a)
        {
            txtInfo.Text = string.Empty;
            DoCalcul();
            UpdateData();
            datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
            datas.DoFiltreDtTableVentes(cbListeAuteurs.ActiveText);
            //
            UpdateCssAuteur();
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        // Désactivé: pour supprimer une vente, on passe par VenteBox.
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
                Int16 nNumVente = Convert.ToInt16(datas.lstoreVentes.GetValue(iter, Convert.ToInt16(Global.eTrvVentesCols.Numero)));
                datas.SupprimerVente(nNumVente);
                DoCalcul();
                UpdateData();
                trvVentes.GrabFocus();
                //
                datas.EnregistrerFichiersVentesPaiements(ref strMsg);
                if (strMsg != string.Empty)
                {
                    Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this);
                    Global.AfficheInfo(ref txtInfo, "Problème lors de la mise à jour des ventes. Vérifier les fichiers", Global.eCssClasses.InfoColorRed);
                }
                else
                    Global.AfficheInfo(ref txtInfo, "Les ventes ont été mises à jour", Global.eCssClasses.InfoColorBlue);
            }
        }

        private void OnBtnNouvelleVenteClicked(object sender, EventArgs a)
        {
            string strMsg = string.Empty;

            txtInfo.Text = string.Empty;

            VenteBox venteBox = new VenteBox(this, ref datas, true);
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
            datas.EnregistrerFichiersVentesPaiements(ref strMsg);
            if (strMsg != string.Empty)
            {
                Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this);
                Global.AfficheInfo(ref txtInfo, "Problème lors de l'enregistrement de la vente. Vérifier les fichiers", Global.eCssClasses.InfoColorRed);
            }
            else
                Global.AfficheInfo(ref txtInfo, "La vente a été enregistrée", Global.eCssClasses.InfoColorBlue);
        }

        // Double-clic sur une ligne.
        private void OnTrvVentesRowActivated(object sender, EventArgs a)
        {
            OnMnuAffichageVente(sender, a);
        }

        private void OnBtnDetailsVenteClicked(object sender, EventArgs e)
        {
            OnMnuAffichageVente(sender, e);
        }

        // Affiche dans une boite de dialogue les données liées à la vente sélectionnée.
        // Buts: modifier ou supprimer la vente, réimprimer un ticket de caisse.
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
                VenteBox venteBox = new VenteBox(this, ref datas, false, nNumVente, dtDate);
			    venteBox.Run();
                if (venteBox.rResponse == ResponseType.Apply)
                {
                    DoCalcul();
                    UpdateData();
                    datas.DoFiltreDtTableAlbums(cbListeAuteurs.ActiveText, cbListeLieuVente.ActiveText, chkAFacturer.Active);
                    datas.DoFiltreDtTableVentes(cbListeAuteurs.ActiveText);
                    trvVentes.GrabFocus();
                    //
                    datas.EnregistrerFichiersVentesPaiements(ref strMsg);
                    if (strMsg != string.Empty)
                    {
                        Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this);
                        Global.AfficheInfo(ref txtInfo, "Problème lors de l'enregistrement de la vente. Vérifier les fichiers", Global.eCssClasses.InfoColorRed);
                    }
                    else
                        Global.AfficheInfo(ref txtInfo, "La vente a été mise à jour", Global.eCssClasses.InfoColorBlue);
                }
            }
        }

        private void OnMnuAffichageBilanVentes(object sender, EventArgs e)
        {
            BilanVentesBox bilanBox = new BilanVentesBox(this, ref datas);
            bilanBox.Run();
        }

        private void OnBtnResetClicked(object sender, EventArgs a)
        {
            cbListeLieuVente.Active = 0;
            cbListeAuteurs.Active = 0;
            chkAFacturer.Active = false;
            Global.AfficheInfo(ref txtInfo, "Les filtres ont été réinitialisés", Global.eCssClasses.InfoColorBlue);
        }

        // Interception de la touche Enter
        private void OnCodeIsbnEanActivated(object sender, EventArgs a)
        {
            Entry txtCode = (Entry)sender;
            // Affichage des infos dans une boite de dialogue
            AlbumBox AlbumBox = new AlbumBox(this, ref datas, txtCode.Text);
			AlbumBox.Run();
            if (AlbumBox.rResponse == ResponseType.Apply)
                AppliquerModifAlbum();

            txtCode.Text = string.Empty;
        }

        private void OnCodeIsbnEanFocusGrabbed(object sender, EventArgs a)
        {
            txtInfo.Text = string.Empty;
        }

        private void OnMnuFichierExportAlbums(object sender, EventArgs a)
        {
            string strMsg = string.Empty;

            // Confirmation si un filtre est actif
            if (cbListeAuteurs.ActiveText != "Tous")
            {
                if (Global.Confirmation(this, "Export Albums", "Vous allez exporter la liste d'albums de " + cbListeAuteurs.ActiveText + ". Continuer ?") == false)
                    return;
            }
            datas.ExportAlbums(cbListeAuteurs.ActiveText, ref strMsg);
            if (strMsg != string.Empty)
            {
                Global.ShowMessage("BdArtLibrairie, export albums:", strMsg, this);
                Global.AfficheInfo(ref txtInfo, "Problème lors de l'export des albums. Vérifier les fichiers", Global.eCssClasses.InfoColorRed);
            }
            else
            {
                Global.AfficheInfo(ref txtInfo, "Les albums ont été exportés", Global.eCssClasses.InfoColorBlue);
                if (Global.LaunchBaseFile == true)
                {
                    // lancement de la base de données dans un process
                    try
                    {
                        Process psBase = new Process();
                        psBase.StartInfo.FileName = "xdg-open";
                        psBase.StartInfo.Arguments = "BdArtLib.odb";
                        psBase.StartInfo.WorkingDirectory = Global.DossierFichiers;
                        if (psBase.Start() == true)// nouveau process créé
                        {
                            psBase.WaitForExit(1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        Global.ShowMessage("Erreur lancement BdArtLib.odb:", ex.Message, this);
                    }
                }
            }
        }

        // Affiche ou masque toutes les colonnes des Treeview.
        private void OnMnuAffichageTout(object sender, EventArgs a)
        {
            if (mnuAffichageTout.Active == true)
            {
                trvVentes.Columns[Convert.ToInt16(Global.eTrvVentesCols.Numero)].Visible = true;
                trvVentes.Columns[Convert.ToInt16(Global.eTrvVentesCols.Rang)].Visible = true;
                Global.AfficheInfo(ref txtInfo, "Toutes les colonnes sont affichées", Global.eCssClasses.InfoColorBlue);
            }
            else
            {
                trvVentes.Columns[Convert.ToInt16(Global.eTrvVentesCols.Numero)].Visible = false;
                trvVentes.Columns[Convert.ToInt16(Global.eTrvVentesCols.Rang)].Visible = false;
                Global.AfficheInfo(ref txtInfo, "Certaines colonnes ont été masquées", Global.eCssClasses.InfoColorBlue);
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
            Global.ConfigModified = true;
        }

        private void OnChkAppliquerCssClicked(object sender, EventArgs a)
        {
            Global.AppliquerCss = chkAppliquerCss.Active;
            Global.ConfigModified = true;
            // on applique ou on supprime les classes CSS
            if (Global.AppliquerCss == true)
                AddCssClass();
            else
                RemoveCssClasses();
        }

        private void OnMnuFichierResetVentes(object sender, EventArgs a)
        {
            string strMsg = "Etes-vous certain de vouloir supprimer toutes les ventes ?" + Environment.NewLine;
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
                datas.EnregistrerFichiersVentesPaiements(ref strMsg);
                if (strMsg != string.Empty)
                {
                    Global.ShowMessage("BdArtLibrairie, enregistrer fichiers:", strMsg, this);
                    Global.AfficheInfo(ref txtInfo, "Problème lors de l'enregistrement des ventes. Vérifier les fichiers", Global.eCssClasses.InfoColorRed);
                }
                else
                    Global.AfficheInfo(ref txtInfo, "Toutes les ventes ont été supprimées", Global.eCssClasses.InfoColorBlue);
                strMsg = string.Empty;
                datas.SupprimerFichierEcartVentes(ref strMsg);
                if (strMsg != string.Empty)
                {
                    Global.ShowMessage("BdArtLibrairie, supprimer fichier:", strMsg, this);
                    Global.AfficheInfo(ref txtInfo, "Problème lors de la suppression du fichier EcartsVentes.txt", Global.eCssClasses.InfoColorRed);
                }
                else
                    CheckErreurEcartVentes();
            }
            else
                Global.AfficheInfo(ref txtInfo, "Password incorrect. RAZ des ventes annulé", Global.eCssClasses.InfoColorRed);
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
            Global.ConfigModified = true;
        }

        // Action de la touche Entrée dans le controle.
        private void OnTxtUsbDevicePathActivated(object sender, EventArgs a)
        {
            OnTxtUsbDevicePathFocusOut(sender, a);
        }
        private void OnTxtUsbDevicePathFocusOut(object o, EventArgs a)
        {
            Global.UsbDevicePath = txtUsbDevicePath.Text;
            Global.ConfigModified = true;
        }

        private void OnTxtNombreTicketsChanged(object sender, EventArgs e)
        {
            txtNombreTickets.Changed -= OnTxtNombreTicketsChanged;
            Global.CheckValeurInt16(this, sender);
            txtNombreTickets.Changed += OnTxtNombreTicketsChanged;
        }

        private void OnTxtNombreTicketsFocusOut(object sender, EventArgs a)
        {
            Int16 nVal;
            txtNombreTickets.FocusOutEvent -= OnTxtNombreTicketsFocusOut;
            nVal = Global.GetValueIntOrZero(this, sender, true);
            if (nVal < 1) nVal = 1;
            if (nVal > 3) nVal = 3;
            Global.NombreTickets = nVal;
            UpdateData();
            txtNombreTickets.FocusOutEvent += OnTxtNombreTicketsFocusOut;
            Global.ConfigModified = true;
        }

        private void OnTxtTempoChanged(object sender, EventArgs e)
        {
            txtTempo.Changed -= OnTxtTempoChanged;
            Global.CheckValeurInt16(this, sender);
            txtTempo.Changed += OnTxtTempoChanged;
        }

        private void OnTxtTempoFocusOut(object sender, EventArgs a)
        {
            Int16 nVal;
            txtTempo.FocusOutEvent -= OnTxtTempoFocusOut;
            nVal = Global.GetValueIntOrZero(this, sender, true);
            if (nVal < 0) nVal = 0;
            if (nVal > 300) nVal = 300;
            Global.Tempo = nVal;
            UpdateData();
            txtTempo.FocusOutEvent += OnTxtTempoFocusOut;
            Global.ConfigModified = true;
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
            txtPathResult.Buffer.Clear();
            
            try
            {
                string[] lines = File.ReadAllLines("/proc/mounts");
                foreach (var line in lines)
                {
                    var parts = line.Split(' ');
                    if (parts.Length > 1 && parts[0].Contains("dev/sd"))
                    {
                        // parts[0] est le dispositif, parts[1] est le point de montage  
                        txtPathResult.Buffer.Text += $"{parts[0]}: {parts[1]}" + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
			{
				Global.ShowMessage("Recherche clé USB:", ex.Message, this);
			}
        }

        private void CheckErreurEcartVentes()
        {
            if (File.Exists(System.IO.Path.Combine(Global.DossierFichiers, Global.FichierEcartsVentes)) == true)
                btnErreurEcartVentes.Visible = true;
            else
                btnErreurEcartVentes.Visible = false;
        }
    }
}
