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

namespace BdArtLibrairie
{
    public class StatistiquesBox : Dialog
    {
        private Datas mdatas;
        [UI] private Entry txtTotalAlbums = null;
        [UI] private Entry txtTotalPrix = null;
        [UI] private Entry txtTotalCommissions = null;
        [UI] private TreeView trvStatsAlbums = null;
        [UI] private TreeView trvStatsPrix = null;
        [UI] private TreeView trvStatsCommissions = null;
        [UI] private Button btnFermer = null;

        public StatistiquesBox(Window ParentWindow, ref Datas datas) : this(new Builder("StatistiquesBox.glade"))
        {
            this.TransientFor = ParentWindow;
            this.SetPosition(WindowPosition.CenterOnParent);
            this.Modal = true;
            this.Title = "Statistiques des ventes";
            //
            mdatas = datas;
            InitTrvStatsAlbums();
            InitTrvStatsPrix();
            InitTrvStatsCommissions();
            UpdateData();
        }

        private StatistiquesBox(Builder builder) : base(builder.GetRawOwnedObject("StatistiquesBox"))
        {
            builder.Autoconnect(this);
            DeleteEvent += delegate { OnBtnFermerClicked(this, new EventArgs()); };
            //
            btnFermer.Clicked += OnBtnFermerClicked;
        }

        private void OnBtnFermerClicked(object sender, EventArgs a)
        {
            mdatas.lstoreStatsAlbums.Clear();
            mdatas.lstoreStatsPrix.Clear();
            mdatas.lstoreStatsCommissions.Clear();
            this.Dispose();
        }

        private void InitTrvStatsAlbums()
        {
            trvStatsAlbums.Model = mdatas.lstoreStatsAlbums;
            //
            TreeViewColumn colAuteur = new TreeViewColumn();
            colAuteur.Title = "Auteur";
            TreeViewColumn colQteVendus = new TreeViewColumn();
            colQteVendus.Title = "Albums vendus";
            //
            trvStatsAlbums.AppendColumn(colAuteur);
            trvStatsAlbums.AppendColumn(colQteVendus);
            //
            CellRendererText cellAuteur = new CellRendererText();
            colAuteur.PackStart(cellAuteur, true);
            colAuteur.AddAttribute(cellAuteur, "text", 0);

            CellRendererText cellQteVendus = new CellRendererText();
            colQteVendus.PackStart(cellQteVendus, true);
            colQteVendus.AddAttribute(cellQteVendus, "text", 1);
        }

        private void InitTrvStatsPrix()
        {
            trvStatsPrix.Model = mdatas.lstoreStatsPrix;
            //
            TreeViewColumn colAuteur = new TreeViewColumn();
            colAuteur.Title = "Auteur";
            TreeViewColumn colPrix = new TreeViewColumn();
            colPrix.Title = "Ventes (€)";
            //
            trvStatsPrix.AppendColumn(colAuteur);
            trvStatsPrix.AppendColumn(colPrix);
            //
            CellRendererText cellAuteur = new CellRendererText();
            colAuteur.PackStart(cellAuteur, true);
            colAuteur.AddAttribute(cellAuteur, "text", 0);

            CellRendererText cellPrix = new CellRendererText();
            colPrix.PackStart(cellPrix, true);
            colPrix.AddAttribute(cellPrix, "text", 1);
        }

        private void InitTrvStatsCommissions()
        {
            trvStatsCommissions.Model = mdatas.lstoreStatsCommissions;
            //
            TreeViewColumn colAuteur = new TreeViewColumn();
            colAuteur.Title = "Auteur";
            TreeViewColumn colCommissions = new TreeViewColumn();
            colCommissions.Title = "Part BD'Art (€)";
            //
            trvStatsCommissions.AppendColumn(colAuteur);
            trvStatsCommissions.AppendColumn(colCommissions);
            //
            CellRendererText cellAuteur = new CellRendererText();
            colAuteur.PackStart(cellAuteur, true);
            colAuteur.AddAttribute(cellAuteur, "text", 0);

            CellRendererText cellCommissions = new CellRendererText();
            colCommissions.PackStart(cellCommissions, true);
            colCommissions.AddAttribute(cellCommissions, "text", 1);
        }

        private void UpdateData()
        {
            Int16 nQteVendus = 0;
            double dblPrix = 0;
            double dblCommissions = 0;
            mdatas.CalculStatistiquesVentes(ref nQteVendus, ref dblPrix, ref dblCommissions);
            txtTotalAlbums.Text = nQteVendus.ToString();
            txtTotalPrix.Text = Math.Round(dblPrix, 2).ToString();
            txtTotalCommissions.Text = Math.Round(dblCommissions, 2).ToString();
        }
    }
}