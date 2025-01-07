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
using System.Data;
using System.Text;
using System.IO;
using Gtk;
using System.Linq;

namespace BdArtLibrairie
{
	public class Datas
	{
		public Window pParentWindow=null;
		public DataTable dtTableVentes, dtTableAlbums, dtTablePaiements, dtTableAuteurs;
		public ListStore lstoreVentes, lstoreAlbums, lstoreUneVente, lstoreAuteurs, lstoreAlbumsMini;
		public ListStore lstoreStatsAlbums, lstoreStatsPrix, lstoreStatsCommissions;
		protected StreamReader strmReader = StreamReader.Null;
		protected StreamWriter strmWriter = StreamWriter.Null;
		private string strPremLigneAlbums = "Code ISBN / EAN;IdAuteur;Titre;Prix vente;Stock initial";
		private string  strPremLigneVentes = "Numéro;Rang;Date;Code ISBN / EAN;Quantité;Lieu;Paiement";
		private string  strPremLignePaiements = "NuméroVente;PourcentCB;PourcentChèque;PourcentEspèces";
		private string  strPremLigneAuteurs = "IdAuteur;Prénom auteur;Nom auteur;Pourcentage";
		private string strErreurStockAlbums;

        public string ErreurStockAlbums { get => strErreurStockAlbums; set => strErreurStockAlbums = value; }


        /// <summary>
        /// Constructeur.
        /// </summary>
        public Datas(Window ParentWindow)
		{
			pParentWindow = ParentWindow;
			//
			dtTableAlbums = new DataTable("Albums");
			dtTableAlbums.Columns.Add("strIsbnEan", typeof(String));// ISBN-10 ou ISBN-13 ou EAN
			dtTableAlbums.Columns.Add("nIdAuteur", typeof(Int16));
			dtTableAlbums.Columns.Add("strTitre", typeof(String));
			dtTableAlbums.Columns.Add("dblPrixVente", typeof(Double));
			dtTableAlbums.Columns.Add("nStockInitial", typeof(Int16));
			dtTableAlbums.Columns.Add("nQteVenduLibrairie", typeof(Int16));
			dtTableAlbums.Columns.Add("nQteVenduMediatheque", typeof(Int16));
			dtTableAlbums.Columns.Add("nQteOffert", typeof(Int16));
			dtTableAlbums.Columns.Add("nStockFinal", typeof(Int16));
			dtTableAlbums.Columns.Add("nQteAfacturer", typeof(Int16));
			dtTableAlbums.Columns.Add("nQteTotalVendu", typeof(Int16));
			dtTableAlbums.Columns.Add("dblPrixTotal", typeof(Double));
			dtTableAlbums.PrimaryKey = new DataColumn[] { dtTableAlbums.Columns["strIsbnEan"] };
			//
			dtTableVentes = new DataTable("Ventes");
			dtTableVentes.Columns.Add("nNumero", typeof(Int16));
			dtTableVentes.Columns.Add("nRang", typeof(Int16));
			dtTableVentes.Columns.Add("dtDate", typeof(DateTime));
			dtTableVentes.Columns.Add("strIsbnEan", typeof(String));
			dtTableVentes.Columns.Add("nQteVendu", typeof(Int16));
			dtTableVentes.Columns.Add("strLieu", typeof(String));// Librairie ou Médiathèque
			dtTableVentes.Columns.Add("strPaiement", typeof(String));// A facturer, Offert
			dtTableVentes.PrimaryKey = new DataColumn[] { dtTableVentes.Columns["nNumero"], dtTableVentes.Columns["nRang"] };
			//
			dtTablePaiements = new DataTable("Paiements");
			dtTablePaiements.Columns.Add("nNumeroVente", typeof(Int16));
			dtTablePaiements.Columns.Add("dblPourcentCB", typeof(Double));
			dtTablePaiements.Columns.Add("dblPourcentCheque", typeof(Double));
			dtTablePaiements.Columns.Add("dblPourcentEspeces", typeof(Double));
			dtTablePaiements.PrimaryKey = new DataColumn[] { dtTablePaiements.Columns["nNumeroVente"] };
			//
			dtTableAuteurs = new DataTable("Auteurs");
			dtTableAuteurs.Columns.Add("nIdAuteur", typeof(Int16));
			dtTableAuteurs.Columns.Add("strPrenomAuteur", typeof(string));
			dtTableAuteurs.Columns.Add("strNomAuteur", typeof(string));
			dtTableAuteurs.Columns.Add("strAuteur", typeof(string));// champ conservé après ajout des 2 précédents pour éviter de modifier trop de code
			dtTableAuteurs.Columns.Add("dblPourcentage", typeof(double));
			dtTableAuteurs.PrimaryKey = new DataColumn[] { dtTableAuteurs.Columns["nIdAuteur"] };
			//
			// IsbnEan, auteur, titre, PrixVente, StockInitial, QtéVenduLibrairie, QteVenduMediat, QteOffert, StockFinal, QteAfacturer, QteTotalVendu, PrixTotal
			lstoreAlbums = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));
			// numéro, rang, date+heure, IsbnEan, auteur, titre, PrixVente, Qté vendu, Lieu, Paiement
			lstoreVentes = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));
			// IsbnEan, auteur, titre, PrixVente, Qté vendu, Lieu, Paiement
			lstoreUneVente = new ListStore(typeof(string), typeof(string), typeof(string),typeof(string), typeof(string), typeof(string), typeof(string));
			// IdAuteur, auteur, pourcentage
			lstoreAuteurs = new ListStore(typeof(string), typeof(string), typeof(string));
			// IsbnEan, auteur, titre
			lstoreAlbumsMini = new ListStore(typeof(string), typeof(string), typeof(string));
			//
			lstoreStatsAlbums = new ListStore(typeof(string), typeof(string));
			lstoreStatsPrix = new ListStore(typeof(string), typeof(string));
			lstoreStatsCommissions = new ListStore(typeof(string), typeof(string));
			//
			Init();
		}

		public void Init()
		{
			dtTableAlbums.Clear();
			dtTableAuteurs.Clear();
			dtTableVentes.Clear();
			dtTablePaiements.Clear();
			lstoreAlbums.Clear();
			lstoreVentes.Clear();
			lstoreAuteurs.Clear();
			strErreurStockAlbums = string.Empty;
		}

		/// <summary>
		/// Retourne le status de la table passée en paramètre.
		/// </summary>
		/// <returns></returns>
		public DataRowState GetTableState(ref DataTable dtTable)
		{
			DataRowState eStatus = DataRowState.Unchanged;

			foreach (DataRow row in dtTable.Rows)
			{
				// si ligne ajoutée
				if (row.RowState == DataRowState.Added)
				{
					eStatus = DataRowState.Added;
				}
				// priorité: si ligne modifiée ou supprimée
				if (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Deleted)
				{
					eStatus = DataRowState.Modified;
					break;
				}
			}
			return eStatus;
		}

		public void ChargerFichiers(ref string strMsg)
		{
			bool bContinu = false;
			//
			try
			{
				// fichier Auteur existe ?
				if (File.Exists(Path.Combine(Global.DossierFichiers, Global.FichierAuteurs)) == false)
				{
					FileStream fs = new FileStream(Path.Combine(Global.DossierFichiers, Global.FichierAuteurs),	FileMode.Create, FileAccess.Write);
					strmWriter = new StreamWriter(fs, Encoding.UTF8);

					// création du fichier
					strmWriter.WriteLine(strPremLigneAuteurs);
					bContinu = true;
				}
				else
				{
					FileStream fs = new FileStream(Path.Combine(Global.DossierFichiers, Global.FichierAuteurs), FileMode.Open, FileAccess.Read);
					strmReader = new StreamReader(fs, Encoding.UTF8);

					// lecture du fichier
					if (LireFichierAuteurs())
						bContinu = true;
				}
			}
			catch (Exception e)
			{
				if (strMsg != String.Empty)
					strMsg += Environment.NewLine;
				strMsg += e.Message + Environment.NewLine;
				bContinu = false;
			}
			finally
			{
				if (!strmReader.Equals(StreamReader.Null))
					strmReader.Close();
				if (!strmWriter.Equals(StreamWriter.Null))
					strmWriter.Close();
			}
			//
			if (bContinu == true)
			{
				bContinu = false;
				try
				{
					// fichier Album existe ?
					if (File.Exists(Path.Combine(Global.DossierFichiers, Global.FichierAlbums)) == false)
					{
						FileStream fs = new FileStream(Path.Combine(Global.DossierFichiers, Global.FichierAlbums),	FileMode.Create, FileAccess.Write);
						strmWriter = new StreamWriter(fs, Encoding.UTF8);

						// création du fichier
						strmWriter.WriteLine(strPremLigneAlbums);
						bContinu = true;
					}
					else
					{
						FileStream fs = new FileStream(Path.Combine(Global.DossierFichiers, Global.FichierAlbums), FileMode.Open, FileAccess.Read);
						strmReader = new StreamReader(fs, Encoding.UTF8);

						// lecture du fichier
						if (LireFichierAlbums())
							bContinu = true;
					}
				}
				catch (Exception e)
				{
					if (strMsg != String.Empty)
						strMsg += Environment.NewLine;
					strMsg += e.Message + Environment.NewLine;
					bContinu = false;
				}
				finally
				{
					if (!strmReader.Equals(StreamReader.Null))
						strmReader.Close();
					if (!strmWriter.Equals(StreamWriter.Null))
					strmWriter.Close();
				}
			}
			//
			if (bContinu == true)
			{
				bContinu = false;
				try
				{
					// fichier Ventes existe ?
					if (File.Exists(Path.Combine(Global.DossierFichiers, Global.FichierVentes)) == false)
					{
						FileStream fs = new FileStream(Path.Combine(Global.DossierFichiers, Global.FichierVentes),	FileMode.Create, FileAccess.Write);
						strmWriter = new StreamWriter(fs, Encoding.UTF8);

						// création du fichier
						strmWriter.WriteLine(strPremLigneVentes);
						bContinu = true;
					}
					else
					{
						FileStream fs = new FileStream(Path.Combine(Global.DossierFichiers, Global.FichierVentes), FileMode.Open, FileAccess.Read);
						strmReader = new StreamReader(fs, Encoding.UTF8);

						// lecture du fichier
						if (LireFichierVentes())
							bContinu = true;
					}
				}
				catch (Exception e)
				{
					if (strMsg != String.Empty)
						strMsg += Environment.NewLine;
					strMsg += e.Message + Environment.NewLine;
					bContinu = false;
				}
				finally
				{
					if (!strmReader.Equals(StreamReader.Null))
						strmReader.Close();
					if (!strmWriter.Equals(StreamWriter.Null))
						strmWriter.Close();
				}
			}
			//
			if (bContinu == true)
			{
				try
				{
					// fichier Paiements existe ?
					if (File.Exists(Path.Combine(Global.DossierFichiers, Global.FichierPaiements)) == false)
					{
						FileStream fs = new FileStream(Path.Combine(Global.DossierFichiers, Global.FichierPaiements),	FileMode.Create, FileAccess.Write);
						strmWriter = new StreamWriter(fs, Encoding.UTF8);

						// création du fichier
						strmWriter.WriteLine(strPremLignePaiements);
						bContinu = true;
					}
					else
					{
						FileStream fs = new FileStream(Path.Combine(Global.DossierFichiers, Global.FichierPaiements), FileMode.Open, FileAccess.Read);
						strmReader = new StreamReader(fs, Encoding.UTF8);

						// lecture du fichier
						LireFichierPaiements();
					}
				}
				catch (Exception e)
				{
					if (strMsg != String.Empty)
						strMsg += Environment.NewLine;
					strMsg += e.Message + Environment.NewLine;
				}
				finally
				{
					if (!strmReader.Equals(StreamReader.Null))
						strmReader.Close();
					if (!strmWriter.Equals(StreamWriter.Null))
						strmWriter.Close();
				}
			}
		}

		/// <summary>
		/// Lecture des lignes du fichier Auteurs et remplissage du dataTable.
		/// </summary>
		private bool LireFichierAuteurs()
		{
			string 		strLigne;
			string [] 	strSplit;
			DataRow 	dtRow;
			int j = 0;

			try
			{
				// 1ère ligne
				strmReader.ReadLine();
				// lignes suivantes
				while ( (strLigne = strmReader.ReadLine()) != null )
				{
					strSplit = strLigne.Split(new Char [] {';'});
					if (strSplit.Length < 4)
					{
						throw new Exception("Fichier AUTEURS, ligne " + (j + 1).ToString() + ", " + Global.m_strMsgFileFormatNotOk);
					}
					dtRow = dtTableAuteurs.NewRow();
					dtRow["nIdAuteur"]		= strSplit[0];
					dtRow["strPrenomAuteur"]= strSplit[1];
					dtRow["strNomAuteur"]	= strSplit[2];
					dtRow["strAuteur"]		= strSplit[2] + " " + strSplit[1];// nom prénom
					dtRow["dblPourcentage"]	= Convert.ToDouble(strSplit[3]);//.Replace(',','.'));
					//
					dtTableAuteurs.Rows.Add(dtRow);
					dtTableAuteurs.Rows[j++].AcceptChanges();
					//
					lstoreAuteurs.AppendValues(strSplit[0], strSplit[2] + " " + strSplit[1], strSplit[3]);
				}
			}
			catch (Exception)
			{
				throw;
			}
			return true;
		}

        /// <summary>
        /// Lecture des lignes du fichier Albums et remplissage du dataTable.
        /// </summary>
        private bool LireFichierAlbums()
		{
			string 		strLigne;
			string [] 	strSplit;
			DataRow 	dtRow;
			int j = 0;

			try
			{
				// 1ère ligne
				strmReader.ReadLine();
				// lignes suivantes
				while ( (strLigne = strmReader.ReadLine()) != null )
				{
					strSplit = strLigne.Split(new Char [] {';'});
					//if (strSplit.Length < 12) ancienne version du fichier
					if (strSplit.Length < 5)
					{
						throw new Exception("Fichier ALBUMS, ligne " + (j + 1).ToString() + ", " + Global.m_strMsgFileFormatNotOk);
					}
					dtRow = dtTableAlbums.NewRow();
					dtRow["strIsbnEan"]		= strSplit[0];
					dtRow["nIdAuteur"]		= Convert.ToInt16(strSplit[1]);
					dtRow["strTitre"]		= strSplit[2];
					dtRow["dblPrixVente"]	= Convert.ToDouble(strSplit[3]);//.Replace(',','.'));
					dtRow["nStockInitial"]	= Convert.ToInt16(strSplit[4]);
					dtRow["nQteVenduLibrairie"]		= 0;
					dtRow["nQteVenduMediatheque"]	= 0;
					dtRow["nQteOffert"]		= 0;
					dtRow["nStockFinal"]	= Convert.ToInt16(strSplit[4]); //=stock initial
					dtRow["nQteAfacturer"]	= 0;
					dtRow["nQteTotalVendu"]	= 0;
					dtRow["dblPrixTotal"]	= 0;
					//
					dtTableAlbums.Rows.Add(dtRow);
					dtTableAlbums.Rows[j++].AcceptChanges();
					//
					// recherche nom auteur et ajout ligne dans lstoreAlbums
					foreach (DataRow rowAU in dtTableAuteurs.Select("nIdAuteur=" + strSplit[1]))
					{
						lstoreAlbums.AppendValues(
							strSplit[0],
							rowAU["strAuteur"].ToString(),
							strSplit[2],
							strSplit[3],
							strSplit[4],
							"0",
							"0",
							"0",
							strSplit[4], //=stock initial
							"0",
							"0",
							"0"
						);
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
			return true;
		}

		/// <summary>
		/// Lecture des lignes du fichier Ventes et remplissage du dataTable.
		/// </summary>
		private bool LireFichierVentes()
		{
			string 		strLigne;
			string [] 	strSplit;
			DataRow 	dtRow;
			int j = 0;
			string strIsbnEan;

			try
			{
				// 1ère ligne
				strmReader.ReadLine();
				// lignes suivantes
				while ( (strLigne = strmReader.ReadLine()) != null )
				{
					strSplit = strLigne.Split(new Char [] {';'});
					if (strSplit.Length < 7)
					{
						throw new Exception("Fichier VENTES, ligne " + (j + 1).ToString() + ", " + Global.m_strMsgFileFormatNotOk);
					}
					dtRow = dtTableVentes.NewRow();
					dtRow["nNumero"]		= Convert.ToInt16(strSplit[0]);
					dtRow["nRang"]			= Convert.ToInt16(strSplit[1]);
					dtRow["dtDate"]			= Convert.ToDateTime(strSplit[2]);
					dtRow["strIsbnEan"]		= strSplit[3];
					dtRow["nQteVendu"]		= Convert.ToInt16(strSplit[4]);
					dtRow["strLieu"]		= strSplit[5];
					dtRow["strPaiement"]	= strSplit[6];
					//
					dtTableVentes.Rows.Add(dtRow);
					dtTableVentes.Rows[j++].AcceptChanges();
					//
					strIsbnEan = dtRow["strIsbnEan"].ToString();
					foreach (DataRow rowA in dtTableAlbums.Select("strIsbnEan='" + strIsbnEan + "'"))
					{
						// recherche nom auteur et ajout ligne dans lstoreVentes
						foreach (DataRow rowAU in dtTableAuteurs.Select("nIdAuteur=" + rowA["nIdAuteur"].ToString()))
						{
							lstoreVentes.AppendValues(
								dtRow["nNumero"].ToString(),
								dtRow["nRang"].ToString(),
								Convert.ToDateTime(dtRow["dtDate"]).ToString(),
								strIsbnEan,
									rowAU["strAuteur"].ToString(),
									rowA["strTitre"].ToString(),
									rowA["dblPrixVente"].ToString(),
								dtRow["nQteVendu"].ToString(),
								dtRow["strLieu"].ToString(),
								dtRow["strPaiement"].ToString());
						}
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
			return true;
		}

		/// <summary>
		/// Lecture des lignes du fichier Paiements et remplissage du dataTable.
		/// </summary>
		private bool LireFichierPaiements()
		{
			string 		strLigne;
			string [] 	strSplit;
			DataRow 	dtRow;
			int j = 0;

			try
			{
				// 1ère ligne
				strmReader.ReadLine();
				// lignes suivantes
				while ( (strLigne = strmReader.ReadLine()) != null )
				{
					strSplit = strLigne.Split(new Char [] {';'});
					if (strSplit.Length < 4)
					{
						throw new Exception("Fichier PAIEMENTS, ligne " + (j + 1).ToString() + ", " + Global.m_strMsgFileFormatNotOk);
					}
					dtRow = dtTablePaiements.NewRow();
					dtRow["nNumeroVente"] = Convert.ToInt16(strSplit[0]);
					dtRow["dblPourcentCB"] = Convert.ToDouble(strSplit[1]);
					dtRow["dblPourcentCheque"] = Convert.ToDouble(strSplit[2]);
					dtRow["dblPourcentEspeces"] = Convert.ToDouble(strSplit[3]);
					//
					dtTablePaiements.Rows.Add(dtRow);
					dtTablePaiements.Rows[j++].AcceptChanges();
				}
			}
			catch (Exception)
			{
				throw;
			}
			return true;
		}

		public void GetLstoreUneVente(Int16 nNumVente)
		{
			string strIsbnEan;

			lstoreUneVente.Clear();
			foreach (DataRow rowV in dtTableVentes.Select("nNumero=" + nNumVente.ToString()))
			{
				if (rowV.RowState == DataRowState.Deleted)
						continue;

				strIsbnEan = rowV["strIsbnEan"].ToString();
				foreach (DataRow rowA in dtTableAlbums.Select("strIsbnEan='" + strIsbnEan + "'"))
				{
					// recherche nom auteur et ajout ligne dans lstoreUneVente
					foreach (DataRow rowAU in dtTableAuteurs.Select("nIdAuteur=" + rowA["nIdAuteur"].ToString()))
					{
						lstoreUneVente.AppendValues(
							strIsbnEan,
							rowAU["strAuteur"].ToString(),
							rowA["strTitre"].ToString(),
							rowA["dblPrixVente"].ToString(),
							rowV["nQteVendu"].ToString(),
							rowV["strLieu"].ToString(),
							rowV["strPaiement"].ToString()
							);
					}
				}
			}

		}

		public Int16 GetNewNumVente()
		{
			Int16 nNum = 0;

			// recherche du plus grand numéro attribué
			foreach (DataRow row in dtTableVentes.Rows)
			{
				if (Convert.ToInt16(row["nNumero"]) > nNum)
					nNum = Convert.ToInt16(row["nNumero"]);
			}
			return ++nNum;
		}

		public void AjouteVenteLivre(Int16 nNumeroVente, double dblPourcentCB, double dblPourcentCheque, double dblPourcentEspeces, DateTime date)
		{
			TreeIter iter;
			int nRang = 1;
			string strIsbnEan;

			if (lstoreUneVente.GetIterFirst(out iter) == true)
            {
                do
                {
                    DataRow dtRow = dtTableVentes.NewRow();
					dtRow["nNumero"]		= nNumeroVente;
					dtRow["nRang"]			= nRang++;
					dtRow["dtDate"]			= date;
					dtRow["strIsbnEan"]		= lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.CodeIsbnEan)).ToString();
					dtRow["nQteVendu"]		= Convert.ToInt16(lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.QteVendu)).ToString());
					dtRow["strLieu"]		= lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.LieuVente)).ToString();
					dtRow["strPaiement"]	= lstoreUneVente.GetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.Paiement)).ToString();
					dtTableVentes.Rows.Add(dtRow);
					//
					strIsbnEan = dtRow["strIsbnEan"].ToString();
					foreach (DataRow rowA in dtTableAlbums.Select("strIsbnEan='" + strIsbnEan + "'"))
					{
						// recherche nom auteur et ajout ligne dans lstoreVentes
						foreach (DataRow rowAU in dtTableAuteurs.Select("nIdAuteur=" + rowA["nIdAuteur"].ToString()))
						{
							lstoreVentes.AppendValues(
								dtRow["nNumero"].ToString(),
								dtRow["nRang"].ToString(),
								Convert.ToDateTime(dtRow["dtDate"]).ToString(),
								strIsbnEan,
									rowAU["strAuteur"].ToString(),
									rowA["strTitre"].ToString(),
									rowA["dblPrixVente"].ToString(),
								dtRow["nQteVendu"].ToString(),
								dtRow["strLieu"].ToString(),
								dtRow["strPaiement"].ToString()
								);
						}
					}
                }
                while (lstoreUneVente.IterNext(ref iter) == true);

				// ajout dans dtTablePaiements
				DataRow newP = dtTablePaiements.NewRow();
				newP["nNumeroVente"] = nNumeroVente;
				newP["dblPourcentCB"] = dblPourcentCB;
				newP["dblPourcentCheque"] = dblPourcentCheque;
				newP["dblPourcentEspeces"] = dblPourcentEspeces;
				dtTablePaiements.Rows.Add(newP);
            }
		}

		public void DoFiltreDtTableVentes(string strAuteur)
		{
			string strQuery;
			Int16 nIdAuteur=0;
			string strIsbnEan;

			lstoreVentes.Clear();
			foreach (DataRow dtRow in dtTableVentes.Select("1=1", "nNumero DESC, nRang ASC"))
			{
				if (dtRow.RowState == DataRowState.Deleted)
						continue;

				strIsbnEan = dtRow["strIsbnEan"].ToString();
				//
				if (string.Compare(strAuteur, "Tous") == 0)
					strQuery = "strIsbnEan='" + strIsbnEan + "'";
				else
				{
					// recherche IdAuteur
					foreach (DataRow rowAU in dtTableAuteurs.Select("strAuteur='" + strAuteur + "'"))
						nIdAuteur = Convert.ToInt16(rowAU["nIdAuteur"]);
					strQuery = "strIsbnEan='" + strIsbnEan + "' AND nIdAuteur=" + nIdAuteur.ToString();
				}
				//
				foreach (DataRow row in dtTableAlbums.Select(strQuery))
				{
					// recherche strAuteur et ajout dans lstoreVentes
					foreach (DataRow rowAU in dtTableAuteurs.Select("nIdAuteur=" + row["nIdAuteur"].ToString()))
					{	
						lstoreVentes.AppendValues(
							dtRow["nNumero"].ToString(),
							dtRow["nRang"].ToString(),
							Convert.ToDateTime(dtRow["dtDate"]).ToString(),
							strIsbnEan,
								rowAU["strAuteur"].ToString(),
								row["strTitre"].ToString(),
								row["dblPrixVente"].ToString(),
							dtRow["nQteVendu"].ToString(),
							dtRow["strLieu"].ToString(),
							dtRow["strPaiement"].ToString()
						);
					}
				}
			}
		}

		// Filtre les lignes de la table AlbumsMini selon certains critères
		public void DoFiltreDtTableAlbumsMini(string strAuteur, string strMotTitre)
		{
			string strQuery;
			string strQueryTitre = "strTitre LIKE '%" + strMotTitre + "%'";
			Int16 nIdAuteur=0;

			if (string.Compare(strAuteur, "Tous") == 0)
			{
				if (strMotTitre != string.Empty)
					strQuery = strQueryTitre;
				else
					strQuery = "1=1";
			}
			else
			{
				// recherche IdAuteur
				foreach (DataRow rowAU in dtTableAuteurs.Select("strAuteur='" + strAuteur + "'"))
					nIdAuteur = Convert.ToInt16(rowAU["nIdAuteur"]);
				strQuery = "nIdAuteur=" + nIdAuteur.ToString();
				if (strMotTitre != string.Empty)
					strQuery += " AND " + strQueryTitre;
			}
			//
			lstoreAlbumsMini.Clear();
			foreach (DataRow dtRow in dtTableAlbums.Select(strQuery, "nIdAuteur ASC"))
			{
				// recherche strAuteur et ajout dans lstoreAlbumsMini
				foreach (DataRow rowAU in dtTableAuteurs.Select("nIdAuteur=" + dtRow["nIdAuteur"].ToString()))
				{				
					lstoreAlbumsMini.AppendValues(
						dtRow["strIsbnEan"].ToString(),
							rowAU["strAuteur"].ToString(),
						dtRow["strTitre"].ToString());
				}
			}
		}

		// Filtre les lignes de la table Albums selon certains critères.
		public void DoFiltreDtTableAlbums(string strAuteur, string strLieuVente, bool bAFacturer)
		{
			string strQuery;
			Int16 nIdAuteur=0;

			if (string.Compare(strAuteur, "Tous") == 0)
			{
				if (bAFacturer == false)
					strQuery = "1=1";
				else
					strQuery = "nQteAfacturer > 0";
			}
			else
			{
				// recherche IdAuteur
				foreach (DataRow rowAU in dtTableAuteurs.Select("strAuteur='" + strAuteur + "'"))
					nIdAuteur = Convert.ToInt16(rowAU["nIdAuteur"]);
				strQuery = "nIdAuteur=" + nIdAuteur.ToString();
				//
				if (bAFacturer == true)
					strQuery += " AND nQteAfacturer > 0";
			}
			//
			if (string.Compare(strLieuVente, "Tous") != 0)
			{
				if (string.Compare(strLieuVente, Global.eListeLieuVente.Librairie.ToString()) == 0)
					strQuery += " AND nQteVenduLibrairie > 0";
				else
					strQuery += " AND nQteVenduMediatheque > 0";
			}
			//
			lstoreAlbums.Clear();
			foreach (DataRow dtRow in dtTableAlbums.Select(strQuery, "nIdAuteur ASC"))
			{
				// recherche strAuteur et ajout dans lstoreAlbums
				foreach (DataRow rowAU in dtTableAuteurs.Select("nIdAuteur=" + dtRow["nIdAuteur"].ToString()))
				{				
					lstoreAlbums.AppendValues(
						dtRow["strIsbnEan"].ToString(),
							rowAU["strAuteur"].ToString(),
						dtRow["strTitre"].ToString(),
						dtRow["dblPrixVente"].ToString(),
						dtRow["nStockInitial"].ToString(),
						dtRow["nQteVenduLibrairie"].ToString(),
						dtRow["nQteVenduMediatheque"].ToString(),
						dtRow["nQteOffert"].ToString(),
						dtRow["nStockFinal"].ToString(),
						dtRow["nQteAfacturer"].ToString(),
						dtRow["nQteTotalVendu"].ToString(),
						dtRow["dblPrixTotal"].ToString()
					);
				}
			}

		}

		public bool EnregistrerFichiersVentesPaiements(ref string strMsg)
		{
			bool bContinu = false;
			FileMode eMode;
			// fichier Ventes
			try
			{
				if (GetTableState(ref dtTableVentes) == DataRowState.Unchanged)
					return true;
				// ligne ajoutée
				else if (GetTableState(ref dtTableVentes) == DataRowState.Added)
					eMode = FileMode.Append;
				// ligne modifiée ou supprimée, il faut réécrire le fichier
				else
				{
					eMode = FileMode.Create;
					DoCopieFichierSauve(Global.FichierVentes, Global.FichierVentesWoExt);
				}

				FileStream fs = new FileStream(
					Path.Combine(Global.DossierFichiers, Global.FichierVentes),
					eMode, FileAccess.Write);
				//
				strmWriter = new StreamWriter(fs, Encoding.UTF8);
				// écriture du fichier
				if (GetTableState(ref dtTableVentes) == DataRowState.Added)
				{
					if (EcrireFichierVentesAppend() == true)
						bContinu = true;
				}
				else
				{
					if (EcrireFichierVentesComplet() == true)
						bContinu = true;
				}
			}
			catch (IOException e)
			{
				if (strMsg != String.Empty)
					strMsg += Environment.NewLine;
				strMsg += e.Message + Environment.NewLine;
				bContinu = false;
			}
			finally
			{
				if (!strmWriter.Equals(StreamWriter.Null))
					strmWriter.Close();
			}
			//
			if (bContinu == true)
			{
				dtTableVentes.AcceptChanges();
				//
				// fichier Paiements
				try
				{
					if (GetTableState(ref dtTablePaiements) == DataRowState.Unchanged)
						return true;
					// ligne ajoutée
					else if (GetTableState(ref dtTablePaiements) == DataRowState.Added)
						eMode = FileMode.Append;
					// ligne modifiée ou supprimée, il faut réécrire le fichier
					else
					{
						eMode = FileMode.Create;
						DoCopieFichierSauve(Global.FichierPaiements, Global.FichierPaiementsWoExt);
					}

					FileStream fs = new FileStream(
						Path.Combine(Global.DossierFichiers, Global.FichierPaiements),
						eMode, FileAccess.Write);
					//
					strmWriter = new StreamWriter(fs, Encoding.UTF8);
					// écriture du fichier
					if (GetTableState(ref dtTablePaiements) == DataRowState.Added)
					{
						if (EcrireFichierPaiementsAppend() == true)
							bContinu = true;
					}
					else
					{
						if (EcrireFichierPaiementsComplet() == true)
							bContinu = true;
					}
				}
				catch (IOException e)
				{
					if (strMsg != String.Empty)
						strMsg += Environment.NewLine;
					strMsg += e.Message + Environment.NewLine;
					bContinu = false;
				}
				finally
				{
					if (!strmWriter.Equals(StreamWriter.Null))
						strmWriter.Close();
				}
				if (bContinu == true)
					dtTablePaiements.AcceptChanges();
			}
			return bContinu;
		}

		private bool EcrireFichierVentesAppend()
		{
			string strLigne;

			try
			{
				foreach (DataRow row in dtTableVentes.Rows)
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					if (row.RowState == DataRowState.Added)
					{
						strLigne = row["nNumero"].ToString() + ";";
						strLigne += row["nRang"].ToString() + ";";
						strLigne += row["dtDate"].ToString() + ";";
						strLigne += row["strIsbnEan"].ToString() + ";";
						strLigne += row["nQteVendu"].ToString() + ";";
						strLigne += row["strLieu"].ToString() + ";";
						strLigne += row["strPaiement"].ToString();
						strmWriter.WriteLine(strLigne);
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
			return true;
		}

		private bool EcrireFichierVentesComplet()
		{
			string strLigne;

			try
			{
				strmWriter.WriteLine(strPremLigneVentes);
				foreach (DataRow row in dtTableVentes.Rows)
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					// on écrit toutes les lignes
					strLigne = row["nNumero"].ToString() + ";";
					strLigne += row["nRang"].ToString() + ";";
					strLigne += row["dtDate"].ToString() + ";";
					strLigne += row["strIsbnEan"].ToString() + ";";
					strLigne += row["nQteVendu"].ToString() + ";";
					strLigne += row["strLieu"].ToString() + ";";
					strLigne += row["strPaiement"].ToString();
					strmWriter.WriteLine(strLigne);
				}
			}
			catch (Exception)
			{
				throw;
			}			
			return true;
		}

		private bool EcrireFichierPaiementsAppend()
		{
			string strLigne;

			try
			{
				foreach (DataRow row in dtTablePaiements.Rows)
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					if (row.RowState == DataRowState.Added)
					{
						strLigne = row["nNumeroVente"].ToString() + ";";
						strLigne += row["dblPourcentCB"].ToString() + ";";
						strLigne += row["dblPourcentCheque"].ToString() + ";";
						strLigne += row["dblPourcentEspeces"].ToString();
						strmWriter.WriteLine(strLigne);
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
			return true;
		}

		private bool EcrireFichierPaiementsComplet()
		{
			string strLigne;

			try
			{
				strmWriter.WriteLine(strPremLignePaiements);
				foreach (DataRow row in dtTablePaiements.Rows)
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					// on écrit toutes les lignes
					strLigne = row["nNumeroVente"].ToString() + ";";
					strLigne += row["dblPourcentCB"].ToString() + ";";
					strLigne += row["dblPourcentCheque"].ToString() + ";";
					strLigne += row["dblPourcentEspeces"].ToString();
					strmWriter.WriteLine(strLigne);
				}
			}
			catch (Exception)
			{
				throw;
			}
			return true;
		}

		// RAZ des valeurs à recalculer d'après les ventes, de la table Albums.
		public void RazTableAlbums()
		{
			foreach (DataRow rowA in dtTableAlbums.Rows)
			{
				rowA["nQteVenduLibrairie"] = 0;
				rowA["nQteVenduMediatheque"] = 0;
				rowA["nQteOffert"] = 0;
				rowA["nStockFinal"] = Convert.ToInt16(rowA["nStockInitial"]);
				rowA["nQteAfacturer"] = 0;
				rowA["nQteTotalVendu"] = 0;
				rowA["dblPrixTotal"] = 0;
			}
		}

		// Update des qtés vendues dans la table Albums.
		public void UpdateAlbumsQteVendues(Int16 nIdAuteur, DataRow rowV, ref Int16 nQteLibrairie, ref Int16 nQteMediatheques, ref Int16 nQteOfferts)
		{
			string strIsbnEan;
			Int16 nQteVendu;

			nQteVendu = Convert.ToInt16(rowV["nQteVendu"]);
			strIsbnEan = rowV["strIsbnEan"].ToString();
			foreach (DataRow rowA in dtTableAlbums.Select("strIsbnEan='" + strIsbnEan + "' AND nIdAuteur=" + nIdAuteur.ToString()))
			{
				rowA["nStockFinal"] = Convert.ToInt16(rowA["nStockFinal"]) - nQteVendu;
				// si stock final < 0
				if (Convert.ToInt16(rowA["nStockFinal"]) < 0)
					ErreurStockAlbums += strIsbnEan + "\t" + rowA["strTitre"] + Environment.NewLine;
				//
				if (string.Compare(rowV["strPaiement"].ToString(), Global.eMoyenPaiement.Offert.ToString()) == 0)
				{
					nQteOfferts += nQteVendu;
					rowA["nQteOffert"] = Convert.ToInt16(rowA["nQteOffert"]) + nQteVendu;
				}
				else
				{
					// total vendus
					rowA["nQteTotalVendu"] = Convert.ToInt16(rowA["nQteTotalVendu"]) + nQteVendu;
					rowA["dblPrixTotal"] = Math.Round(Convert.ToDouble(rowA["dblPrixVente"]) * Convert.ToInt16(rowA["nQteTotalVendu"]), 2);
					//
					if (string.Compare(rowV["strPaiement"].ToString(), Global.eMoyenPaiement.AFacturer.ToString()) == 0)
					{
						rowA["nQteAfacturer"] = Convert.ToInt16(rowA["nQteAfacturer"]) + nQteVendu;
					}
					//
					if (string.Compare(rowV["strLieu"].ToString(), Global.eListeLieuVente.Librairie.ToString()) == 0)
					{
						nQteLibrairie += nQteVendu;
						rowA["nQteVenduLibrairie"] = Convert.ToInt16(rowA["nQteVenduLibrairie"]) + nQteVendu;
					}
					else if (string.Compare(rowV["strLieu"].ToString(), Global.eListeLieuVente.Médiathèque.ToString()) == 0)
					{
						nQteMediatheques += nQteVendu;
						rowA["nQteVenduMediatheque"] = Convert.ToInt16(rowA["nQteVenduMediatheque"]) + nQteVendu;
					}
				}
			}
		}

		public void RAZVentesPaiements()
		{
			// Ne pas utiliser Clear car le statut des tables est aussi réinitialisé et n'entraine pas la sauvegarde
			// complète de la table dans la méthode EnregistrerFichiers
			// dtTablePaiements.Clear();
			// dtTableVentes.Clear();
			foreach (DataRow rowP in dtTablePaiements.Rows)
				rowP.Delete();
			foreach (DataRow rowV in dtTableVentes.Rows)
				rowV.Delete();
			lstoreVentes.Clear();
		}

		// Modifie le lieu et le statut paiement de la ligne passée en paramètre.
		public void ModifierUneVente(TreeIter iter, string strLieuVente, string strPaiement)
		{
			lstoreUneVente.SetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.LieuVente), strLieuVente);
			lstoreUneVente.SetValue(iter, Convert.ToInt16(Global.eTrvListeLivresCols.Paiement), strPaiement);
		}

		// Supprime définitivement dans le listStore temporaire, la vente dont la clé est passée en paramètre.
		public void SupprimerUneVente(TreeIter iter)
		{
			lstoreUneVente.Remove(ref iter);
		}

		// Supprime définitivement dans le dataTable, la vente dont la clé est passée en paramètre.
		public void SupprimerVente(Int16 nNumVente)
		{
			// suppression de la table Paiements
			foreach (DataRow row in dtTablePaiements.Select("nNumeroVente=" + nNumVente.ToString()))
			{
				if (row.RowState == DataRowState.Deleted)
					continue;
				row.Delete();
			}
			// suppression de la table Ventes
			foreach (DataRow row in dtTableVentes.Select("nNumero=" + nNumVente.ToString()))
			{
				if (row.RowState == DataRowState.Deleted)
					continue;
				row.Delete();
			}
		}

		// Méthode utilisée pour l'export des données Albums.
		public void ExportAlbums(string strAuteur, ref string strMsg)
		{
			// nom de fichier unique quelque soit le filtre actif pour ouverture dans LO_Base
			string strNomFichier = "Albums_export.csv";
			string strLigne, strFiltre="1=1";
			Int16 nIdAuteur;
			//
			try
			{
				FileStream fs = new FileStream(
					Path.Combine(Global.DossierFichiers, strNomFichier),
					FileMode.Create, FileAccess.Write);
				strmWriter = new StreamWriter(fs, Encoding.UTF8);
				//
				strmWriter.WriteLine("Code Isbn/Ean;IdAuteur;Titre;Prix vente;Stock initial;Vendu librairie;Vendu médiat.;Offert;Stock final;A facturer;Total vendu;Prix total;Nom auteur;Pourcentage;Part auteur;Part librairie");
				//
				if (strAuteur.Trim() != "Tous")
				{
					// recherche IdAuteur
					foreach (DataRow rowAU in dtTableAuteurs.Select("strAuteur='" + strAuteur + "'"))
					{
						nIdAuteur = Convert.ToInt16(rowAU["nIdAuteur"]);
						strFiltre = "nIdAuteur=" + nIdAuteur.ToString();
					}
				}
				// pour chaque album
				foreach (DataRow row in dtTableAlbums.Select(strFiltre))
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					// champs nécessaires pour BdArtLib.odb
					strLigne = row["strIsbnEan"].ToString() + ";";
					strLigne += row["nIdAuteur"].ToString() + ";";
					strLigne += row["strTitre"].ToString() + ";";
					strLigne += row["dblPrixVente"].ToString() + ";";
					strLigne += row["nStockInitial"].ToString() + ";";
					strLigne += row["nQteVenduLibrairie"].ToString() + ";";
					strLigne += row["nQteVenduMediatheque"].ToString() + ";";
					strLigne += row["nQteOffert"].ToString() + ";";
					strLigne += row["nStockFinal"].ToString() + ";";
					strLigne += row["nQteAfacturer"].ToString() + ";";
					strLigne += row["nQteTotalVendu"].ToString() + ";";
					strLigne += row["dblPrixTotal"].ToString() + ";";
					// champs supplémentaires pour exploitation dans un tableur
					// nom auteur, pourcentage, part auteur, part librairie
					foreach (DataRow rowAU in dtTableAuteurs.Select("nIdAuteur=" + row["nIdAuteur"].ToString()))
					{
						strLigne += rowAU["strAuteur"].ToString() + ";";
						strLigne += rowAU["dblPourcentage"].ToString() + ";";
						strLigne += Math.Round(Convert.ToDouble(row["dblPrixVente"]) * Convert.ToInt16(row["nQteTotalVendu"]) * (Convert.ToDouble(rowAU["dblPourcentage"])) / 100, 2) + ";";
						strLigne += Math.Round(Convert.ToDouble(row["dblPrixVente"]) * Convert.ToInt16(row["nQteTotalVendu"]) * (100 - Convert.ToDouble(rowAU["dblPourcentage"])) / 100, 2) + ";";
					}
					strmWriter.WriteLine(strLigne);
				}
			}
			catch (IOException e)
			{
				if (strMsg != String.Empty)
					strMsg += Environment.NewLine;
				strMsg += e.Message + Environment.NewLine;
			}
			finally
			{
				if (!strmWriter.Equals(StreamWriter.Null))
					strmWriter.Close();
			}
		}

		public void CalculStatistiquesVentes(ref Int16 nQteTotalVendus, ref double dblTotalPrix, ref double dblTotalCommissions)
		{
			lstoreStatsAlbums.Clear();
			lstoreStatsPrix.Clear();
			lstoreStatsCommissions.Clear();
			Int16 nQteVendus;
			double dblPrix;
			double dblCommissions;

			DataTable dtTableTemp = new DataTable("Temp");
			dtTableTemp.Columns.Add("strAuteur", typeof(String));
			dtTableTemp.Columns.Add("nQteVendus", typeof(Int16));
			dtTableTemp.Columns.Add("dblPrix", typeof(Double));
			dtTableTemp.Columns.Add("dblCommissions", typeof(Double));
			//
			dtTableTemp.Clear();
			// pour chaque auteur
			foreach (DataRow rowAU in dtTableAuteurs.Rows)
			{
				if (rowAU.RowState == DataRowState.Deleted)
					continue;
				nQteVendus = 0;
				dblPrix = 0;
				dblCommissions = 0;
				// pour chaque album de l'auteur
				foreach (DataRow dtRow in dtTableAlbums.Select("nIdAuteur=" + rowAU["nIdAuteur"].ToString()))
				{
					nQteVendus += Convert.ToInt16(dtRow["nQteTotalVendu"]);
					dblPrix += Convert.ToDouble(dtRow["dblPrixVente"]) * Convert.ToInt16(dtRow["nQteTotalVendu"]);
					dblCommissions += Convert.ToDouble(dtRow["dblPrixVente"]) * Convert.ToInt16(dtRow["nQteTotalVendu"]) * (100 - Convert.ToDouble(rowAU["dblPourcentage"])) / 100;
				}
				DataRow dtNewRow = dtTableTemp.NewRow();
				dtNewRow["strAuteur"] = rowAU["strAuteur"].ToString();
				dtNewRow["nQteVendus"] = nQteVendus;
				dtNewRow["dblPrix"] = dblPrix;
				dtNewRow["dblCommissions"] = dblCommissions;
				dtTableTemp.Rows.Add(dtNewRow);
				nQteTotalVendus += nQteVendus;
				dblTotalPrix += dblPrix;
				dblTotalCommissions += dblCommissions;
			}
			// tri et remplissage des ListStore
			foreach (DataRow dtRow in dtTableTemp.Select("1=1", "nQteVendus DESC"))
			{
				lstoreStatsAlbums.AppendValues(dtRow["strAuteur"].ToString(), dtRow["nQteVendus"].ToString());
			}
			//
			foreach (DataRow dtRow in dtTableTemp.Select("1=1", "dblPrix DESC"))
			{
				lstoreStatsPrix.AppendValues(dtRow["strAuteur"].ToString(), Math.Round(Convert.ToDouble(dtRow["dblPrix"]), 2).ToString());
			}
			//
			foreach (DataRow dtRow in dtTableTemp.Select("1=1", "dblCommissions DESC"))
			{
				lstoreStatsCommissions.AppendValues(dtRow["strAuteur"].ToString(), Math.Round(Convert.ToDouble(dtRow["dblCommissions"]), 2).ToString());
			}
		}

        public void DoRefreshLstoreAuteurs()
        {
			lstoreAuteurs.Clear();
            foreach (DataRow row in dtTableAuteurs.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
					continue;
				lstoreAuteurs.AppendValues(	row["nIdAuteur"].ToString(),
											row["strAuteur"].ToString(),
											row["dblPourcentage"].ToString());
			}
        }

        internal void EnregistrerFichierAuteurs(ref string strMsg)
        {
            FileMode eMode;
			// fichier Auteurs
			try
			{
				if (GetTableState(ref dtTableAuteurs) == DataRowState.Unchanged)
					return;
				// ligne ajoutée
				else if (GetTableState(ref dtTableAuteurs) == DataRowState.Added)
					eMode = FileMode.Append;
				// ligne modifiée ou supprimée, il faut réécrire le fichier
				else
				{
					eMode = FileMode.Create;
					DoCopieFichierSauve(Global.FichierAuteurs, Global.FichierAuteursWoExt);
				}

				FileStream fs = new FileStream(
					Path.Combine(Global.DossierFichiers, Global.FichierAuteurs),
					eMode, FileAccess.Write);
				//
				strmWriter = new StreamWriter(fs, Encoding.UTF8);
				// écriture du fichier
				if (GetTableState(ref dtTableAuteurs) == DataRowState.Added)
				{
					if (EcrireFichierAuteursAppend() == true)
					{
						dtTableAuteurs.AcceptChanges();
						Console.WriteLine("Fichier Auteurs: ligne ajoutée");
					}
				}
				else
				{
					if (EcrireFichierAuteursComplet() == true)
					{
						dtTableAuteurs.AcceptChanges();
						Console.WriteLine("Fichier Auteurs: réécrit");
					}
				}
			}
			catch (IOException e)
			{
				if (strMsg != String.Empty)
					strMsg += Environment.NewLine;
				strMsg += e.Message + Environment.NewLine;
			}
			finally
			{
				if (!strmWriter.Equals(StreamWriter.Null))
					strmWriter.Close();
			}
        }

        private bool EcrireFichierAuteursComplet()
        {
			string strLigne;

			try
			{
				strmWriter.WriteLine(strPremLigneAuteurs);
				foreach (DataRow row in 	dtTableAuteurs.Rows)
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					// on écrit toutes les lignes
					strLigne = row["nIdAuteur"].ToString() + ";";
					strLigne += row["strPrenomAuteur"].ToString() + ";";
					strLigne += row["strNomAuteur"].ToString() + ";";
					strLigne += row["dblPourcentage"].ToString();
					strmWriter.WriteLine(strLigne);
				}
			}
			catch (Exception)
			{
				throw;
			}
			return true;	
		}

        private bool EcrireFichierAuteursAppend()
        {
			string strLigne;

			try
			{
				foreach (DataRow row in dtTableAuteurs.Rows)
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					if (row.RowState == DataRowState.Added)
					{
						strLigne = row["nIdAuteur"].ToString() + ";";
						strLigne += row["strPrenomAuteur"].ToString() + ";";
						strLigne += row["strNomAuteur"].ToString() + ";";
						strLigne += row["dblPourcentage"].ToString();
						strmWriter.WriteLine(strLigne);
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
			return true;
		}

        private void DoCopieFichierSauve(string strNomFichier, string strNomFichierWoExt, string strExt=".csv")
        {
			string strFichier = Path.Combine(Global.DossierFichiers, strNomFichier);
			string strDirSauve = Path.Combine(Global.DossierFichiers, Global.DossierSauve);
			string strFichierSauve = string.Empty;

			try
			{
				// création du dossier de sauvegarde, si pas présent
				if (!Directory.Exists(strDirSauve))
					Directory.CreateDirectory(strDirSauve);

				// incrémentation du n° de fichier dans le nom
				string[] allFiles = Directory.GetFiles(strDirSauve).Select(filename => Path.GetFileNameWithoutExtension(filename)).ToArray();
				string tempFileName = strNomFichierWoExt;
				int count = 1;
				while (allFiles.Contains(tempFileName))
				{
					tempFileName = String.Format("{0}({1})", strNomFichierWoExt, count++); 
				}
				strFichierSauve = Path.Combine(strDirSauve, tempFileName) + strExt;
				File.Copy(strFichier, strFichierSauve);
			}
			catch (Exception e)
			{
				Global.ShowMessage("Erreur copie du fichier " + strNomFichierWoExt, e.Message, pParentWindow);
			}
		}

        internal void EnregistrerFichierAlbums(ref string strMsg)
        {
            FileMode eMode;
			try
			{
				if (GetTableState(ref dtTableAlbums) == DataRowState.Unchanged)
					return;
				// ligne ajoutée
				else if (GetTableState(ref dtTableAlbums) == DataRowState.Added)
					eMode = FileMode.Append;
				// ligne modifiée ou supprimée, il faut réécrire le fichier
				else
				{
					eMode = FileMode.Create;
					DoCopieFichierSauve(Global.FichierAlbums, Global.FichierAlbumsWoExt);
				}

				FileStream fs = new FileStream(
					Path.Combine(Global.DossierFichiers, Global.FichierAlbums),
					eMode, FileAccess.Write);
				//
				strmWriter = new StreamWriter(fs, Encoding.UTF8);
				// écriture du fichier
				if (GetTableState(ref dtTableAlbums) == DataRowState.Added)
				{
					if (EcrireFichierAlbumsAppend() == true)
					{
						dtTableAlbums.AcceptChanges();
						Console.WriteLine("Fichier Albums: ligne ajoutée");
					}
				}
				else
				{
					if (EcrireFichierAlbumsComplet() == true)
					{
						dtTableAlbums.AcceptChanges();
						Console.WriteLine("Fichier Albums: réécrit");
					}
				}
			}
			catch (IOException e)
			{
				if (strMsg != String.Empty)
					strMsg += Environment.NewLine;
				strMsg += e.Message + Environment.NewLine;
			}
			finally
			{
				if (!strmWriter.Equals(StreamWriter.Null))
					strmWriter.Close();
			}
        }

        private bool EcrireFichierAlbumsComplet()
        {
            string strLigne;

			try
			{
				strmWriter.WriteLine(strPremLigneAlbums);
				foreach (DataRow row in dtTableAlbums.Rows)
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					// on écrit toutes les lignes
					strLigne = row["strIsbnEan"].ToString() + ";";
					strLigne += row["nIdAuteur"].ToString() + ";";
					strLigne += row["strTitre"].ToString() + ";";
					strLigne += row["dblPrixVente"].ToString() + ";";
					strLigne += row["nStockInitial"].ToString();
					strmWriter.WriteLine(strLigne);
				}
			}
			catch (Exception)
			{
				throw;
			}
			return true;
        }

        private bool EcrireFichierAlbumsAppend()
        {
            string strLigne;

			try
			{
				foreach (DataRow row in dtTableAlbums.Rows)
				{
					if (row.RowState == DataRowState.Deleted)
						continue;
					if (row.RowState == DataRowState.Added)
					{
						strLigne = row["strIsbnEan"].ToString() + ";";
						strLigne += row["nIdAuteur"].ToString() + ";";
						strLigne += row["strTitre"].ToString() + ";";
						strLigne += row["dblPrixVente"].ToString() + ";";
						strLigne += row["nStockInitial"].ToString();
						strmWriter.WriteLine(strLigne);
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
			return true;
        }

		public void SupprimerAuteur(TreeIter iter, Int16 nIdAuteur)
		{
			foreach (DataRow row in dtTableAuteurs.Select("nIdAuteur=" + nIdAuteur.ToString()))
			{
				if (row.RowState == DataRowState.Deleted)
					continue;
				row.Delete();
			}
			// lstoreVentes.Remove(ref iter);// /!\ ne pas supprimer du ListStore sinon erreur:
			// gtk_list_store_remove: assertion 'iter_is_valid (iter, list_store)' failed
			// on recharge le ListStore
			DoRefreshLstoreAuteurs();
		}

        internal short GetNewIdAuteur()
        {
            // détermination de la clé
			Int16 nKey = 0;
			foreach(DataRow row in dtTableAuteurs.Rows)
			{
				if (row.RowState == DataRowState.Deleted)
						continue;
				if (Convert.ToInt16(row["nIdAuteur"]) > nKey)
					nKey = Convert.ToInt16(row["nIdAuteur"]);
			}
			return ++nKey;
        }

		public void SupprimerAlbum(TreeIter iter, string strCode)
		{
			foreach (DataRow row in dtTableAlbums.Select("strIsbnEan='" + strCode + "'"))
			{
				if (row.RowState == DataRowState.Deleted)
					continue;
				row.Delete();
			}
			// lstoreVentes.Remove(ref iter);// /!\ ne pas supprimer du ListStore sinon erreur:
			// gtk_list_store_remove: assertion 'iter_is_valid (iter, list_store)' failed
			// on recharge le ListStore après l'appel de cette méthode
		}

        internal void AjouteAlbum(string strCode, short nIdAuteur, string strTitre, double dblPrixVente, short nStockInital, short nStockFinal)
        {
            DataRow row = dtTableAlbums.NewRow();
			row["strIsbnEan"] = strCode;
			row["nIdAuteur"] = nIdAuteur;
			row["strTitre"] = strTitre;
			row["dblPrixVente"] = dblPrixVente;
			row["nStockInitial"] = nStockInital;
			row["nStockFinal"] = nStockFinal;
			row["nQteVenduLibrairie"] = 0;
			row["nQteVenduMediatheque"] = 0;
			row["nQteOffert"] = 0;
			row["nQteAfacturer"] = 0;
			row["nQteTotalVendu"] = 0;
			dtTableAlbums.Rows.Add(row);
        }

        internal void AjouteAuteur(short nIdAuteur)
        {
            DataRow rowAU = dtTableAuteurs.NewRow();
			rowAU["nIdAuteur"] = nIdAuteur;
			dtTableAuteurs.Rows.Add(rowAU);
        }

		public void EnregistrerFichierEcartsVentes(ref string strMsg, string strMsg2)
		{
			FileMode eMode;
			try
			{
				if (File.Exists(Path.Combine(Global.DossierFichiers, Global.FichierEcartsVentes)) == true)
					eMode = FileMode.Append;
				else
					eMode = FileMode.Create;
				
				FileStream fs = new FileStream(
					Path.Combine(Global.DossierFichiers, Global.FichierEcartsVentes),
					eMode, FileAccess.Write);
				
				strmWriter = new StreamWriter(fs, Encoding.UTF8);
				strmWriter.WriteLine(strMsg2);
			}
			catch (IOException e)
			{
				if (strMsg != String.Empty)
					strMsg += Environment.NewLine;
				strMsg += e.Message + Environment.NewLine;
			}
			finally
			{
				if (!strmWriter.Equals(StreamWriter.Null))
					strmWriter.Close();
			}
		}

        internal void ChargerFicErrEcartVentes(ref string strMsg)
        {
			string strLigne, strContenu = string.Empty;

            // fichier EcartsVentes existe ?
			if (File.Exists(Path.Combine(Global.DossierFichiers, Global.FichierEcartsVentes)) == false)
			{
				Global.ShowMessage("Charger fichier", "Le fichier EcartsVentes n'a pas été trouvé.", pParentWindow);
			}
			else
			{
				try
				{
					FileStream fs = new FileStream(Path.Combine(Global.DossierFichiers, Global.FichierEcartsVentes), FileMode.Open, FileAccess.Read);
					strmReader = new StreamReader(fs, Encoding.UTF8);

					while ( (strLigne = strmReader.ReadLine()) != null )
					{
						strContenu += strLigne + Environment.NewLine;
					}
					//Global.ShowMessage("Ecarts Ventes", strContenu, pParentWindow);
					LireFichierBox lireBox = new LireFichierBox(pParentWindow, this, strContenu);
					lireBox.Run();
				}
				catch (Exception e)
				{
					if (strMsg != String.Empty)
						strMsg += Environment.NewLine;
					strMsg += e.Message + Environment.NewLine;
				}
				finally
				{
					if (!strmReader.Equals(StreamReader.Null))
						strmReader.Close();
				}
			}
		}

        internal void SupprimerFichierEcartVentes(ref string strMsg)
        {
			string strFilename = Path.Combine(Global.DossierFichiers, Global.FichierEcartsVentes);

            if (File.Exists(strFilename) == true)
			{
				DoCopieFichierSauve(Global.FichierEcartsVentes, Global.FichierEcartsVentesWoExt, ".txt");
				try
				{
					File.Delete(strFilename);
				}
				catch (Exception e)
				{
					if (strMsg != String.Empty)
						strMsg += Environment.NewLine;
					strMsg += e.Message + Environment.NewLine;
				}
			}
        }

        internal void PurgerDossierSauve(ref string strMsg)
        {
            string strDirSauve = Path.Combine(Global.DossierFichiers, Global.DossierSauve);

			try
			{
				if (Directory.Exists(strDirSauve) == true)
					Directory.Delete(strDirSauve, true);
			}
			catch (Exception e)
			{
				if (strMsg != String.Empty)
					strMsg += Environment.NewLine;
				strMsg += e.Message + Environment.NewLine;
			}
        }
    }
}