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

namespace BdArtLibrairie
{
    public class AlbumBox : Dialog
    {
        private Datas mdatas;
        private bool bModified;
        public ResponseType rResponse;
        private string strIsbnEan;
        private Int16 nIdAuteur;
        private string strAuteur;
        private bool bNewAlbum, bErreurCode;
        [UI] private Entry txtCodeIsbnEan = null;
        [UI] private Entry txtAuteur = null;
        [UI] private Entry txtTitre = null;
        [UI] private Entry txtPrixVente = null;
        [UI] private Entry txtStockInitial = null;
        [UI] private Entry txtStockFinal = null;
        [UI] private Entry txtVenduLibrairie = null;
        [UI] private Entry txtVenduMediatheque = null;
        [UI] private Entry txtQteOffert = null;
        [UI] private Entry txtQteAFacturer = null;
        [UI] private Entry txtQteTotalVendu = null;
        [UI] private Button btnFermer = null;
        [UI] private Button btnTerminer = null;
        [UI] private Button btnAnnuler = null;
        [UI] private CheckButton chkModifiable = null;

        public AlbumBox(Window ParentWindow, ref Datas datas, string strCode, bool bNew=false, Int16 nId=0, string strNomAuteur="") : this(new Builder("AlbumBox.glade"))
        {
            this.TransientFor = ParentWindow;
            this.SetPosition(WindowPosition.CenterOnParent);
            this.Modal = true;
            this.Title = "Album";
            if (bNew == true)
            {
                this.Title = "Nouvel Album";
                chkModifiable.Visible = false;
            }
            if (Global.AppliquerCss == true)
            {
                // champs non éditables par défaut
                Global.AddCssProvider(ref txtCodeIsbnEan, Global.eCssClasses.EntryNotEditable);
                Global.AddCssProvider(ref txtAuteur, Global.eCssClasses.EntryNotEditable);
                Global.AddCssProvider(ref txtTitre, Global.eCssClasses.EntryNotEditable);
                Global.AddCssProvider(ref txtPrixVente, Global.eCssClasses.EntryNotEditable);
                Global.AddCssProvider(ref txtStockInitial, Global.eCssClasses.EntryNotEditable);
                Global.AddCssProvider(ref txtStockFinal, Global.eCssClasses.EntryNotEditable);
                Global.AddCssProvider(ref txtVenduLibrairie, Global.eCssClasses.EntryNotEditable);
                Global.AddCssProvider(ref txtVenduMediatheque, Global.eCssClasses.EntryNotEditable);
                Global.AddCssProvider(ref txtQteOffert, Global.eCssClasses.EntryNotEditable);
                Global.AddCssProvider(ref txtQteAFacturer, Global.eCssClasses.EntryNotEditable);
                Global.AddCssProvider(ref txtQteTotalVendu, Global.eCssClasses.EntryNotEditable);
            }
            //
            mdatas = datas;
            strIsbnEan = strCode;
            bModified = false;
            bNewAlbum = bNew;
            nIdAuteur = nId;
            strAuteur = strNomAuteur;
            bErreurCode = false;
            UpdateData();
            SetControlesEditable(false);
            // on crée les events après avoir renseignés les champs de données modifiables
            if (bNewAlbum == true)
                chkModifiable.Active = true;
            else
                chkModifiable.Active = false;
            txtTitre.FocusOutEvent += OnTxtTitreFocusOutEvent;
            txtPrixVente.Changed += OnTxtPrixVenteChanged;
            txtPrixVente.FocusOutEvent += OnTxtPrixVenteFocusOutEvent;
            txtStockInitial.Changed += OnTxtStockInitialChanged;
            txtStockInitial.FocusOutEvent += OnTxtStockInitialFocusOutEvent;
            txtCodeIsbnEan.Activated += OnTxtCodeIsbnEanActivated;
            txtCodeIsbnEan.GrabFocus();
        }

        private void OnTxtCodeIsbnEanActivated(object sender, EventArgs e)
        {
            CheckCodeIsbnEan();
            bModified = true;
            txtTitre.GrabFocus();
        }

        private AlbumBox(Builder builder) : base(builder.GetRawOwnedObject("AlbumBox"))
        {
            builder.Autoconnect(this);
            //DeleteEvent += delegate { this.Dispose(); }; pas de bouton de fermeture
            //
            btnFermer.Clicked += OnBtnFermerClicked;
            btnTerminer.Clicked += OnBtnTerminerClicked;
            btnAnnuler.Clicked += OnBtnAnnulerClicked;
            chkModifiable.Active = false;
            chkModifiable.Clicked += OnChkModifiableClicked;
            //
            Pango.FontDescription tpf = new Pango.FontDescription();
			tpf.Weight = Pango.Weight.Bold;
            txtPrixVente.ModifyFont(tpf);
        }

        private void OnTxtTitreFocusOutEvent(object o, FocusOutEventArgs args) => bModified = true;

        private void OnTxtStockInitialFocusOutEvent(object o, FocusOutEventArgs args)
        {
            Int16 nVal;
            txtStockInitial.FocusOutEvent -= OnTxtStockInitialFocusOutEvent;
            nVal = Global.GetValueIntOrZero(this, o,true);
            if (nVal < 0) nVal = 0;
            txtStockInitial.Text = nVal.ToString();
			txtStockInitial.FocusOutEvent += OnTxtStockInitialFocusOutEvent;
            // mise à jour du stock final
            txtStockFinal.Text = (nVal - Convert.ToInt16(txtQteTotalVendu.Text)).ToString();
            bModified = true;
        }

        private void OnTxtStockInitialChanged(object sender, EventArgs e)
        {
            txtStockInitial.Changed -= OnTxtStockInitialChanged;
            Global.CheckValeurInt16(this, sender);
            txtStockInitial.Changed += OnTxtStockInitialChanged;
            bModified = true;
        }

        private void OnTxtPrixVenteFocusOutEvent(object o, FocusOutEventArgs args)
        {
            txtPrixVente.FocusOutEvent -= OnTxtPrixVenteFocusOutEvent;
			txtPrixVente.Text = Global.GetValueOrZero(this, o, true).ToString();
			txtPrixVente.FocusOutEvent += OnTxtPrixVenteFocusOutEvent;
            bModified = true;
        }

        private void OnTxtPrixVenteChanged(object sender, EventArgs e)
        {
            txtPrixVente.Changed -= OnTxtPrixVenteChanged;
            Global.CheckValeurs(this, sender);
            txtPrixVente.Changed += OnTxtPrixVenteChanged;
            bModified = true;
        }

        private void OnChkModifiableClicked(object sender, EventArgs e)
        {
            if (chkModifiable.Active == true)
                SetControlesEditable();
            else
                SetControlesEditable(false);
        }

        private void OnBtnAnnulerClicked(object sender, EventArgs e)
        {
            if (bModified == true && Global.Confirmation(this, "Quitter", "Toutes les modifications seront perdues. Continuer ?") == false)
                return;
            rResponse = ResponseType.Cancel;
            OnBtnFermerClicked(sender, e);
        }

        private void CheckCodeIsbnEan()
        {
            bModified = true;
            txtCodeIsbnEan.Text = txtCodeIsbnEan.Text.Replace(" ", string.Empty);
            bErreurCode = false;
            // controle 10 caractères
            if (txtCodeIsbnEan.TextLength < 10)
            {
                Global.ShowMessage("Erreur format:", "Le code doit comporter au moins 10 caractères", this);
                bErreurCode = true;
            }
            else
            {
                foreach (Char c in txtCodeIsbnEan.Text)
                {
                    if (Char.IsDigit(c) == false)
                    {
                        Global.ShowMessage("Erreur format:", "Le code ne doit comporter que des chiffres", this);
                        bErreurCode = true;
                        break;
                    }
                }
            }
            if (bNewAlbum == true)
            {
                // controle existence de l'album
                foreach (DataRow row in mdatas.dtTableAlbums.Select("strIsbnEan='" + txtCodeIsbnEan.Text + "'"))
                {
                    if (row.RowState == DataRowState.Deleted)
                        continue;
                    Global.ShowMessage("Erreur doublon:", "Cet album existe déjà", this);
                    bErreurCode = true;
                }
            }
        }

        private void OnBtnTerminerClicked(object sender, EventArgs e)
        {
            rResponse = ResponseType.Close;
            CheckCodeIsbnEan();
            if (bErreurCode == false)
            {
                if (bModified == true)
                {
                    rResponse = ResponseType.Apply;
                    if (bNewAlbum == true)
                    {
                        mdatas.AjouteAlbum( txtCodeIsbnEan.Text,
                                            nIdAuteur,
                                            txtTitre.Text,
                                            Convert.ToDouble(txtPrixVente.Text),
                                            Convert.ToInt16(txtStockInitial.Text),
                                            Convert.ToInt16(txtStockFinal.Text));
                    }
                    else
                        UpdateData(false);
                }
                OnBtnFermerClicked(sender, e);
            }
        }

        private void OnBtnFermerClicked(object sender, EventArgs e) => this.Dispose();

        private void UpdateData(bool bVersIHM = true)
        {
            double dblAncienPrix, dblNouveauPrix, dblEcart;
            string strMsg = string.Empty, strMsg2, strMsg3 = string.Empty;
            Int16 nCount;

            if (bNewAlbum == true && bVersIHM == true)
            {
                txtCodeIsbnEan.Text = strIsbnEan;
                txtAuteur.Text = strAuteur;
                txtPrixVente.Text = "0";
                txtStockInitial.Text = "0";
                txtStockFinal.Text = "0";
                txtVenduLibrairie.Text = "0";
                txtVenduMediatheque.Text = "0";
                txtQteOffert.Text = "0";
                txtQteAFacturer.Text = "0";
                txtQteTotalVendu.Text = "0";
                return;
            }
            foreach (DataRow row in mdatas.dtTableAlbums.Select("strIsbnEan='" + strIsbnEan + "'"))
            {
                // recherche nom auteur
                foreach (DataRow rowAU in mdatas.dtTableAuteurs.Select("nIdAuteur=" + row["nIdAuteur"].ToString()))
                {
                    if (bVersIHM == true)
                    {
                        txtCodeIsbnEan.Text = strIsbnEan;
                            txtAuteur.Text = rowAU["strAuteur"].ToString();
                        txtTitre.Text = row["strTitre"].ToString();
                        txtPrixVente.Text = Convert.ToDouble(row["dblPrixVente"]).ToString();
                        txtStockInitial.Text = Convert.ToInt16(row["nStockInitial"]).ToString();
                        txtStockFinal.Text = Convert.ToInt16(row["nStockFinal"]).ToString();
                        txtVenduLibrairie.Text = Convert.ToInt16(row["nQteVenduLibrairie"]).ToString();
                        txtVenduMediatheque.Text = Convert.ToInt16(row["nQteVenduMediatheque"]).ToString();
                        txtQteOffert.Text = Convert.ToInt16(row["nQteOffert"]).ToString();
                        txtQteAFacturer.Text = Convert.ToInt16(row["nQteAfacturer"]).ToString();
                        txtQteTotalVendu.Text = Convert.ToInt16(row["nQteTotalVendu"]).ToString();
                    }
                    else
                    {
                        // mise à jour des champs éditables
                        row["strTitre"] = txtTitre.Text;
                        row["nStockInitial"] = Convert.ToInt16(txtStockInitial.Text);
                        // on vérifie d'abord si l'album a déjà été vendu avec l'ancien prix
                        dblAncienPrix = Convert.ToDouble(row["dblPrixVente"]);
                        dblNouveauPrix = Convert.ToDouble(txtPrixVente.Text);
                        row["dblPrixVente"] = dblNouveauPrix;
                        if (dblAncienPrix != dblNouveauPrix)
                        {
                            nCount = 0;
                            foreach(DataRow rowV in mdatas.dtTableVentes.Select("strIsbnEan='" + strIsbnEan + "'"))
                            {
                                strMsg += "Vente:" + rowV["nNumero"].ToString() + " Rang:" + rowV["nRang"].ToString() + Environment.NewLine;
                                nCount++;
                            }
                            if (strMsg != string.Empty)
                            {
                                dblEcart = (dblAncienPrix - dblNouveauPrix) * nCount;
                                strMsg2 = DateTime.Now.ToString() + ": L'album:" + Environment.NewLine; 
                                strMsg2 += strIsbnEan + ": " + txtTitre.Text + " de " + txtAuteur.Text + Environment.NewLine;
                                strMsg2 += "est déjà présent dans les ventes suivantes:" + Environment.NewLine + strMsg;
                                strMsg2 += "Le montant réel des recettes sera différent de celui calculé par l'application." + Environment.NewLine;
                                strMsg3 = string.Format("Ancien prix: {0:0.00}€, Nouveau prix: {1:0.00}€" + Environment.NewLine, dblAncienPrix, dblNouveauPrix);
                                strMsg2 += strMsg3;
                                strMsg3 = string.Format("Qté vendue: {0}, Ecart: {1:0.00}€" + Environment.NewLine, nCount, dblEcart);
                                strMsg2 += strMsg3;
                                Global.ShowMessage("Vente albums", strMsg2, this, MessageType.Warning);
                                strMsg = string.Empty;
                                mdatas.EnregistrerFichierEcartsVentes(ref strMsg, strMsg2);
                                if (strMsg != string.Empty)
                                {
                                    Global.ShowMessage("BdArtLibrairie, enregistrer fichier EcartsVentes:", strMsg, this);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetControlesEditable(bool bEdit = true)
        {
            if (bEdit == true)
            {
                btnAnnuler.Visible = true;
                btnTerminer.Visible = true;
                btnFermer.Visible = false;
                //
                if (bNewAlbum == true)
                {
                    txtCodeIsbnEan.IsEditable = true;
                    if (Global.AppliquerCss == true)
                    {
                        txtCodeIsbnEan.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
                        txtCodeIsbnEan.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());
                    }
                }
                txtTitre.IsEditable = true;
                txtPrixVente.IsEditable = true;
                txtStockInitial.IsEditable = true;
                if (Global.AppliquerCss == true)
                {
                    txtTitre.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
                    txtTitre.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());

                    txtPrixVente.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
                    txtPrixVente.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());

                    txtStockInitial.StyleContext.RemoveClass(Global.eCssClasses.EntryNotEditable.ToString());
                    txtStockInitial.StyleContext.AddClass(Global.eCssClasses.EntryEditable.ToString());
                }
            }
            else
            {
                btnAnnuler.Visible = false;
                btnTerminer.Visible = false;
                btnFermer.Visible = true;
                //
                txtCodeIsbnEan.IsEditable = false;
                txtTitre.IsEditable = false;
                txtPrixVente.IsEditable = false;
                txtStockInitial.IsEditable = false;

                if (Global.AppliquerCss == true)
                {
                    txtCodeIsbnEan.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
                    txtCodeIsbnEan.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());

                    txtTitre.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
                    txtTitre.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());

                    txtPrixVente.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
                    txtPrixVente.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());

                    txtStockInitial.StyleContext.RemoveClass(Global.eCssClasses.EntryEditable.ToString());
                    txtStockInitial.StyleContext.AddClass(Global.eCssClasses.EntryNotEditable.ToString());
                }
            }
        }
    }
}