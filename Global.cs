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
using System.Configuration;
using Gtk;
using System.IO;
using System.IO.Compression;

namespace BdArtLibrairie
{
	public static class Global
	{
		public enum eMoyenPaiement
		{
			Vendu,
			Offert,
			AFacturer
		}

		public enum eListeLieuVente
		{
			Librairie,
			Médiathèque
		}

		public enum eTrvAuteursCols
		{
			IdAuteur=0,
			Auteur,
			Pourcentage
		}

		public enum eTrvAlbumsCols
		{
			CodeIsbnEan=0,
			Auteur,
			Titre,
			PrixVente,
			StockInitial,
			QteVenduLibrairie,
			QteVenduMediatheque,
			QteOffert,
			StockFinal,
			QteAfacturer,
			QteTotalVendu,
			PrixTotal
		}

		public enum eTrvVentesCols
		{
			Numero=0,
			Rang,
			Date,
			CodeIsbnEan,
			Auteur,
			Titre,
			PrixVente,
			QteVendu,
			Lieu,
			Paiement
		}

		public enum eTrvListeLivresCols
		{
			CodeIsbnEan=0,
			Auteur,
			Titre,
			PrixVente,
			QteVendu,
			LieuVente,
			Paiement
		}

		public static string m_strMsgFileFormatNotOk = "Format du fichier non conforme";
        private static bool bConfigModified;
		private static bool bImprimerTickets;
		private static string strFichierAlbums;
		private static string strFichierVentes;
		private static string strFichierVentesWoExt;
		private static string strFichierPaiements;
		private static string strFichierPaiementsWoExt;
		private static string strFichierAuteurs;
		private static string strDossierFichiers;
        private static string strAppStartupPath;
		private static string strPrinterFilePath;
		private static string strUsbDevicePath;
		private static Int16 nTempo;
		private static Int16 nNombreTickets;
		private static bool bUseFgColor;
		private static bool bUseDialogForTicketPrint;
        public static bool ConfigModified {
			get { return bConfigModified; }
			set { bConfigModified = value; }
		}

        public static string FichierAlbums { get => strFichierAlbums; set => strFichierAlbums = value; }
        public static string FichierVentes { get => strFichierVentes; set => strFichierVentes = value; }
		public static string FichierPaiements { get => strFichierPaiements; set => strFichierPaiements = value; }
        public static string AppStartupPath { get => strAppStartupPath; set => strAppStartupPath = value; }
        public static string FichierAuteurs { get => strFichierAuteurs; set => strFichierAuteurs = value; }
        public static string DossierFichiers { get => strDossierFichiers; set => strDossierFichiers = value; }
        public static string FichierVentesWoExt { get => strFichierVentesWoExt; set => strFichierVentesWoExt = value; }
        public static string FichierPaiementsWoExt { get => strFichierPaiementsWoExt; set => strFichierPaiementsWoExt = value; }
        public static string PrinterFilePath { get => strPrinterFilePath; set => strPrinterFilePath = value; }
		public static string UsbDevicePath { get => strUsbDevicePath; set => strUsbDevicePath = value; }
        public static short Tempo { get => nTempo; set => nTempo = value; }
        public static short NombreTickets { get => nNombreTickets; set => nNombreTickets = value; }
        public static bool ImprimerTickets { get => bImprimerTickets; set => bImprimerTickets = value; }
        public static bool UseDialogForTicketPrint { get => bUseDialogForTicketPrint; set => bUseDialogForTicketPrint = value; }
        public static bool UseFgColor { get => bUseFgColor; set => bUseFgColor = value; }

        /// <summary>
        /// Constructeur.
        /// </summary>
        static Global()
		{
			InitParams();
		}

        /// <summary>
		/// Initialisation des paramètres.
		/// </summary>
		static void InitParams()
		{
            ConfigModified = false;
			ImprimerTickets = true;
			FichierAlbums = "Albums.csv";
			FichierVentes = "Ventes.csv";
			FichierPaiements = "Paiements.csv";
			FichierAuteurs = "Auteurs.csv";
			FichierVentesWoExt = "Ventes";
			FichierPaiementsWoExt = "Paiements";
			// params écrasés par fichier de config
			PrinterFilePath = "/dev/usb/lp0";
			Tempo = 2000;
			NombreTickets = 2;
			UseFgColor = false;
			UseDialogForTicketPrint = true;
			UsbDevicePath = "/media/raf/4429-4124";
			//
			AppStartupPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			DossierFichiers = AppStartupPath + Path.DirectorySeparatorChar + "Fichiers";
			// création du dossier Fichiers, si pas présent
			if (!Directory.Exists(DossierFichiers))
				Directory.CreateDirectory(DossierFichiers);
        }

        /// <summary>
		/// Récupère les paramètres de l'application dans le fichier de configuration.
		/// </summary>
		public static void LireConfigLocal(ref string strMsg)
		{
        	try
        	{
        		PrinterFilePath = ConfigurationManager.AppSettings["PrinterFilePath"];
				Tempo = Convert.ToInt16(ConfigurationManager.AppSettings["Tempo"]);
				NombreTickets = Convert.ToInt16(ConfigurationManager.AppSettings["NombreTickets"]);
				UseFgColor = Convert.ToBoolean(ConfigurationManager.AppSettings["UseFgColor"]);
				UseDialogForTicketPrint = Convert.ToBoolean(ConfigurationManager.AppSettings["UseDialogForTicketPrint"]);
				UsbDevicePath = ConfigurationManager.AppSettings["UsbDevicePath"];
			}
        	catch (Exception ex)
			{
				strMsg += ex.Message + "\r\n";
			}
		}

        /// <summary>
		/// Ecriture des paramètres dans le fichier de configuration.
		/// </summary>
		public static void EcrireConfigLocal(ref string strMsg)
		{
			// try
			// {
			// 	XmlElement element;
	        // 	XmlDocument xmlDoc = new XmlDocument();
			// 	var conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
 			// 	xmlDoc.Load(conf.FilePath);
				
			// 	//foreach (XmlElement element in xmlDoc.DocumentElement)
			// 	for (int i = 0; i < xmlDoc.DocumentElement.ChildNodes.Count; i++)
			// 	{
			// 		element = (XmlElement)xmlDoc.DocumentElement.ChildNodes[i];
			// 	    if (element.Name.Equals("appSettings"))
			// 	    {
			// 	    	foreach (XmlNode node in element.ChildNodes)
			// 	        {
			// 	            if (node.Attributes[0].Value.Equals("SylvainAimes"))
			// 	            	node.Attributes[1].Value = SylvainAimes.ToString();
			// 	            //
			// 	    	}
			// 	    }
			// 	}
			// 	xmlDoc.Save(conf.FilePath);
			// 	ConfigurationManager.RefreshSection("appSettings");
			// }
			// catch (Exception ex)
			// {
			// 	strMsg += ex.Message + "\r\n";
			// }
		}

		// Affiche un message dans une boite de dialogue.
        public static void ShowMessage(string strTitle, string strMsg, Window pParent, MessageType mType=MessageType.Error)
        {
            MessageDialog md = new MessageDialog(pParent, DialogFlags.DestroyWithParent, mType, ButtonsType.Close, strTitle);
            md.SecondaryText = strMsg;
            md.Run();
            md.Dispose();
        }

		// Controle du format de la valeur saisie dans la zone de texte.
		// NB: sur les appels à cette méthode, pour éviter les appels en boucle
		// il faut désactiver l'évenement TextChanged sur le controle source.
        public static bool CheckValeurs(Window pParent, object sender)
		{
			Entry txtBox = (Entry)sender;
			try
			{
				double dblValue;
				
				// on remplace le point par la virgule
				txtBox.Text = txtBox.Text.Replace('.',',');
				// on tente une conversion en double
				dblValue = Convert.ToDouble(txtBox.Text);
				//
				return true;
			}
			catch// (Exception ex)
			{
				if (txtBox.Text != string.Empty)
					ShowMessage("Erreur", "Erreur format", pParent);
			}
			return false;
		}

        public static double GetValueOrZero(Window pParent, object sender, bool bShowMessage=false)
		{
			double dblValue = 0;
			Entry txtBox = (Entry)sender;
			
			try
			{
				// on remplace le point par la virgule
				txtBox.Text = txtBox.Text.Replace('.',',');
				// on tente une conversion en double
				dblValue = Math.Round(Convert.ToDouble(txtBox.Text), 2);
			}
			catch (Exception)
			{
				if (bShowMessage == true)
				{
					// seulement si saisie incorrecte
					if (txtBox.Text != string.Empty)
					{
						ShowMessage("Erreur", txtBox.Text + ": le nombre saisi n'est pas dans le bon format.", pParent);
					}
				}
			}
			return dblValue;
		}

		public static void AfficheInfo(Entry txtInfo, string strMsg, Gdk.Color color)
		{
			txtInfo.Text = string.Empty;
			// on change la couleur du texte, ne pas faire si utilisation d'un thème dark
			if (UseFgColor == true)
				txtInfo.ModifyFg(StateType.Normal, color);
			txtInfo.Text = strMsg;
			txtInfo.Show();
		}

		// Demande confirmation à l'utilisateur.
        public static bool Confirmation(Window pParent, string strCaption, string strMsg)
		{
			MessageDialog md = new MessageDialog(pParent, DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.YesNo, strCaption);
            md.SecondaryText = strMsg;
            ResponseType result = (ResponseType)md.Run();
            md.Dispose();
            if (result == ResponseType.Yes)
				return true;
			else
				return false;
		}

		public static bool CheckValeurInt16(Window pParent, object sender)
		{
			Entry txtBox = (Entry)sender;
			try
			{
				Int16 nValue;
				// on tente une conversion en Int16
				nValue = Convert.ToInt16(txtBox.Text);
				//
				return true;
			}
			catch// (Exception ex)
			{
				if (txtBox.Text != string.Empty)
					ShowMessage("Erreur", "Erreur format", pParent);
			}
			return false;
		}

		public static void ExportFichiers(ref string strMsg)
        {
			string strFileName = "BdArtFiles.zip";
			string strZipFile = Path.Combine(Path.GetTempPath(), strFileName);
			string strDestFile = Path.Combine(UsbDevicePath, strFileName);

			try
			{
				// suppression préléminaire
				if (File.Exists(strZipFile))
					File.Delete(strZipFile);
				// création du zip
				ZipFile.CreateFromDirectory(Global.DossierFichiers, strZipFile);
				// copie sur clé
				File.Copy(strZipFile, strDestFile, true);
			}
            catch (Exception e)
			{
				if (strMsg != String.Empty)
					strMsg += "\r\n";
				strMsg = e.Message + "\r\n";
			}
        }
    }
}