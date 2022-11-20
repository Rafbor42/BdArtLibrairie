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
    public class InfoLivreBox : Dialog
    {
        private Datas mdatas;
        private string strIsbnEan;
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

        public InfoLivreBox(Window ParentWindow, ref Datas datas, string strCode) : this(new Builder("InfoLivreBox.glade"))
        {
            this.TransientFor = ParentWindow;
            this.SetPosition(WindowPosition.CenterOnParent);
            this.Modal = true;
            this.Title = "Info livre";
            //
            mdatas = datas;
            strIsbnEan = strCode;
            UpdateData();
        }

        private InfoLivreBox(Builder builder) : base(builder.GetRawOwnedObject("InfoLivreBox"))
        {
            builder.Autoconnect(this);
            DeleteEvent += delegate { this.Dispose(); };
            //
            Pango.FontDescription tpf = new Pango.FontDescription();
			tpf.Weight = Pango.Weight.Bold;
            txtPrixVente.ModifyFont(tpf);
        }

        private void UpdateData()
        {
            foreach (DataRow row in mdatas.dtTableAlbums.Select("strIsbnEan='" + strIsbnEan + "'"))
            {
                // recherche nom auteur
                foreach (DataRow rowAU in mdatas.dtTableAuteurs.Select("nIdAuteur=" + row["nIdAuteur"].ToString()))
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
                }
            }
        }
    }
}