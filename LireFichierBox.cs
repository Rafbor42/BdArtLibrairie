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
using System.IO;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace BdArtLibrairie
{
    public class LireFichierBox : Dialog
    {
        private Datas mdatas;
        [UI] private Button btnFermer = null;
        [UI] private Button btnSupprimerFichier = null;
        [UI] private TextView tvAffichage = null;

        public LireFichierBox(Window ParentWindow, Datas datas, string strData) : this(new Builder("LireFichierBox.glade"))
        {
            this.TransientFor = ParentWindow;
            this.SetPosition(WindowPosition.CenterOnParent);
            this.Modal = true;
            this.Title = "Ecart Ventes";
            //
            mdatas = datas;
            tvAffichage.Buffer.Text = strData;
        }

        private LireFichierBox(Builder builder) : base(builder.GetRawOwnedObject("LireFichierBox"))
        {
            builder.Autoconnect(this);
            //
            btnFermer.Clicked += OnBtnFermerClicked;
            btnSupprimerFichier.Clicked += OnBtnSupprimerFichierClicked;
        }

        private void OnBtnSupprimerFichierClicked(object sender, EventArgs e)
        {
            string strMsg = string.Empty;

            if (Global.Confirmation(this, "Supprimer Fichier", "Voulez-vous vraiment supprimer le fichier EcartsVentes.txt ?") == true)
            {
                mdatas.SupprimerFichierEcartVentes(ref strMsg);
                if (strMsg != string.Empty)
                {
                    Global.ShowMessage("BdArtLibrairie, supprimer fichier:", strMsg, this);
                }
                else
                    tvAffichage.Buffer.Text = string.Empty;
                
            }
        }

        private void OnBtnFermerClicked(object sender, EventArgs e) => this.Dispose();
    }
}