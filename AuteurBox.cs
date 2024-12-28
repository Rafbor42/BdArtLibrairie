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
    public class AuteurBox : Dialog
    {
        private Datas mdatas;
        private Int16 nIdAuteur; 
        private bool bModified;
        private bool bNewAuteur;
        public ResponseType rResponse;
        [UI] private Entry txtIdAuteur = null;
        [UI] private Entry txtPrenomAuteur = null;
        [UI] private Entry txtNomAuteur = null;
        [UI] private Entry txtPourcentage = null;
        [UI] private Button btnFermer = null;
        [UI] private Button btnTerminer = null;
        [UI] private Button btnAnnuler = null;
        [UI] private CheckButton chkModifiable = null;

        public AuteurBox(Window ParentWindow, ref Datas datas, Int16 nId, bool bNew = false) : this(new Builder("AuteurBox.glade"))
        {
            this.TransientFor = ParentWindow;
            this.SetPosition(WindowPosition.CenterOnParent);
            this.Modal = true;
            this.Title = "Auteur";
            if (bNew == true)
            {
                this.Title = "Nouvel Auteur";
                chkModifiable.Visible = false;
            }
            //
            mdatas = datas;
            nIdAuteur = nId;
            bModified = false;
            bNewAuteur = bNew;
            UpdateData();
            if (bNewAuteur == true)                
                chkModifiable.Active = true;
            else
                chkModifiable.Active = false;
            // on crée les events après avoir renseignés les champs de données modifiables
            txtPourcentage.Changed += OnTxtPourcentageChanged;
            txtPourcentage.FocusOutEvent += OnTxtPourcentageFocusOutEvent;
            txtPrenomAuteur.Changed += OnTxtPrenomAuteurChanged;
            txtPrenomAuteur.FocusOutEvent += OnTxtPrenomAuteurFocusOutEvent;
            txtNomAuteur.Changed += OnTxtNomAuteurChanged;
            txtNomAuteur.FocusOutEvent += OnTxtNomAuteurFocusOutEvent;
            txtPrenomAuteur.GrabFocus();
        }

        private AuteurBox(Builder builder) : base(builder.GetRawOwnedObject("AuteurBox"))
        {
            builder.Autoconnect(this);
            //DeleteEvent += delegate { this.Dispose(); }; pas de bouton de fermeture
            //
            btnFermer.Clicked += OnBtnFermerClicked;
            btnTerminer.Clicked += OnBtnTerminerClicked;
            btnAnnuler.Clicked += OnBtnAnnulerClicked;
            SetControlesEditable(false);
            //
            chkModifiable.Clicked += OnChkModifiableClicked;
        }

        private void OnTxtNomAuteurFocusOutEvent(object o, FocusOutEventArgs args) => bModified = true;

        private void OnTxtNomAuteurChanged(object sender, EventArgs e) => bModified = true;

        private void OnTxtPrenomAuteurFocusOutEvent(object o, FocusOutEventArgs args) => bModified = true;

        private void OnTxtPrenomAuteurChanged(object sender, EventArgs e) => bModified = true;

        private void OnChkModifiableClicked(object sender, EventArgs e)
        {
            if (chkModifiable.Active == true)
                SetControlesEditable();
            else
                SetControlesEditable(false);
        }

        private void OnTxtPourcentageFocusOutEvent(object o, FocusOutEventArgs args)
        {
            txtPourcentage.FocusOutEvent -= OnTxtPourcentageFocusOutEvent;
			txtPourcentage.Text = Global.GetValueOrZero(this, o, true).ToString();
			txtPourcentage.FocusOutEvent += OnTxtPourcentageFocusOutEvent;
            bModified = true;
        }

        private void OnTxtPourcentageChanged(object sender, EventArgs e)
        {
            txtPourcentage.Changed -= OnTxtPourcentageChanged;
            Global.CheckValeurs(this, sender);
            txtPourcentage.Changed += OnTxtPourcentageChanged;
            bModified = true;
        }

        private void OnBtnAnnulerClicked(object sender, EventArgs e)
        {
            if (bModified == true && Global.Confirmation(this, "Quitter", "Toutes les modifications seront perdues. Continuer ?") == false)
                return;
            rResponse = ResponseType.Cancel;
            OnBtnFermerClicked(sender, e);
        }

        private void OnBtnTerminerClicked(object sender, EventArgs e)
        {
            string strAuteur;
            rResponse = ResponseType.Close;
            if (bModified == true)
            {
                rResponse = ResponseType.Apply;
                if (bNewAuteur == true)
                {
                    // controle existence auteur
                    strAuteur = txtNomAuteur.Text + " " + txtPrenomAuteur.Text;
                    foreach (DataRow row in mdatas.dtTableAuteurs.Select("strAuteur='" + strAuteur + "'"))
                    {
                        if (row.RowState == DataRowState.Deleted)
                            continue;
                        Global.ShowMessage("Erreur doublon:", "Cet auteur existe déjà", this);
                        return;
                    }
                    mdatas.AjouteAuteur(Convert.ToInt16(nIdAuteur));
                }
                // recherche auteur
                foreach (DataRow rowAU in mdatas.dtTableAuteurs.Select("nIdAuteur=" + nIdAuteur.ToString()))
                {
                    rowAU["dblPourcentage"] = Convert.ToDouble(txtPourcentage.Text);
                    rowAU["strPrenomAuteur"] = txtPrenomAuteur.Text;
                    rowAU["strNomAuteur"] = txtNomAuteur.Text;
                    // mise à jour champ strAuteur
                    rowAU["strAuteur"] = rowAU["strNomAuteur"].ToString() + " " + rowAU["strPrenomAuteur"].ToString();
                }
            }
            OnBtnFermerClicked(sender, e);
        }

        private void OnBtnFermerClicked(object sender, EventArgs e) => this.Dispose();

        private void UpdateData()
        {
            txtIdAuteur.Text = nIdAuteur.ToString();
            if (bNewAuteur == true)
                txtPourcentage.Text = Global.PartAuteurDefaut.ToString();
            // recherche infos auteur
            foreach (DataRow rowAU in mdatas.dtTableAuteurs.Select("nIdAuteur=" + nIdAuteur.ToString()))
            {
                txtPrenomAuteur.Text = rowAU["strPrenomAuteur"].ToString();
                txtNomAuteur.Text = rowAU["strNomAuteur"].ToString();
                txtPourcentage.Text = rowAU["dblPourcentage"].ToString();
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
                txtPourcentage.IsEditable = true;
                txtPrenomAuteur.IsEditable = true;
                txtNomAuteur.IsEditable = true;
            }
            else
            {
                btnAnnuler.Visible = false;
                btnTerminer.Visible = false;
                btnFermer.Visible = true;
                //
                txtPourcentage.IsEditable = false;
                txtPrenomAuteur.IsEditable = false;
                txtNomAuteur.IsEditable = false;
            }
        }
    }
}