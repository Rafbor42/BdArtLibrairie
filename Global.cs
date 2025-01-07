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
using System.Xml;
using System.Text;
using Gtk;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

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

		public enum eCssClasses
		{
			EntryEditable,
			EntryNotEditable,
			InfoColorRed,
			InfoColorBlue
		}

		public static string m_strMsgFileFormatNotOk = "Format du fichier non conforme";
        private static bool bConfigModified;
		private static bool bImprimerTickets;
		private static string strFichierAlbums;
		private static string strFichierAlbumsWoExt;
		private static string strFichierVentes;
		private static string strFichierVentesWoExt;
		private static string strFichierPaiements;
		private static string strFichierPaiementsWoExt;
		private static string strFichierAuteurs;
		private static string strFichierAuteursWoExt;
		private static string strDossierFichiers;
		private static string strDossierSauve;
        private static string strAppStartupPath;
		private static string strPrinterFilePath;
		private static string strUsbDevicePath;
		private static string strNomFestival;
		private static string strFichierEcartsVentes;
		private static string strFichierEcartsVentesWoExt;
		private static Int16 nTempo;
		private static Int16 nNombreTickets;
		private static bool bAppliquerCss;
		private static bool bUseDialogForTicketPrint;
		private static string strFichierConfigLocal;
		private static bool bLaunchBaseFile;
		private static double dblPartAuteurDefaut;
		private static CssProvider cssProvider;
        public static bool ConfigModified {	get => bConfigModified; set => bConfigModified = value; }
        public static string FichierAlbums { get => strFichierAlbums; set => strFichierAlbums = value; }
        public static string FichierVentes { get => strFichierVentes; set => strFichierVentes = value; }
		public static string FichierPaiements { get => strFichierPaiements; set => strFichierPaiements = value; }
        public static string AppStartupPath { get => strAppStartupPath; set => strAppStartupPath = value; }
        public static string FichierAuteurs { get => strFichierAuteurs; set => strFichierAuteurs = value; }
        public static string DossierFichiers { get => strDossierFichiers; set => strDossierFichiers = value; }
        public static string FichierVentesWoExt { get => strFichierVentesWoExt; set => strFichierVentesWoExt = value; }
        public static string FichierPaiementsWoExt { get => strFichierPaiementsWoExt; set => strFichierPaiementsWoExt = value; }
		public static string FichierAuteursWoExt { get => strFichierAuteursWoExt; set => strFichierAuteursWoExt = value; }
		public static string FichierAlbumsWoExt { get => strFichierAlbumsWoExt; set => strFichierAlbumsWoExt = value; }
        public static string PrinterFilePath { get => strPrinterFilePath; set => strPrinterFilePath = value; }
		public static string UsbDevicePath { get => strUsbDevicePath; set => strUsbDevicePath = value; }
        public static short Tempo { get => nTempo; set => nTempo = value; }
        public static short NombreTickets { get => nNombreTickets; set => nNombreTickets = value; }
        public static bool ImprimerTickets { get => bImprimerTickets; set => bImprimerTickets = value; }
        public static bool UseDialogForTicketPrint { get => bUseDialogForTicketPrint; set => bUseDialogForTicketPrint = value; }
        public static bool AppliquerCss { get => bAppliquerCss; set => bAppliquerCss = value; }
        public static string FichierConfigLocal { get => strFichierConfigLocal; set => strFichierConfigLocal = value; }
		public static string NomFestival { get => strNomFestival; set => strNomFestival = value; }
		public static bool LaunchBaseFile { get => bLaunchBaseFile; set => bLaunchBaseFile = value; }
        public static double PartAuteurDefaut { get => dblPartAuteurDefaut; set => dblPartAuteurDefaut = value; }
        public static string FichierEcartsVentes { get => strFichierEcartsVentes; set => strFichierEcartsVentes = value; }
        public static string FichierEcartsVentesWoExt { get => strFichierEcartsVentesWoExt; set => strFichierEcartsVentesWoExt = value; }
        public static CssProvider ProviderCss { get => cssProvider; set => cssProvider = value; }
        public static string DossierSauve { get => strDossierSauve; set => strDossierSauve = value; }

        /// <summary>
        /// Constructeur.
        /// </summary>
        static Global()
		{
			ProviderCss = new CssProvider();
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
			FichierAuteursWoExt = "Auteurs";
			FichierAlbumsWoExt = "Albums";
			FichierEcartsVentes = "EcartsVentes.txt";
			FichierEcartsVentesWoExt = "EcartsVentes";
			DossierSauve = "Sauve";
			// params écrasés par fichier de config
			PrinterFilePath = "/dev/usb/lp0";
			Tempo = 2000;
			NombreTickets = 2;
			AppliquerCss = false;
			UseDialogForTicketPrint = true;
			UsbDevicePath = "/media/raf/4429-4124";
			FichierConfigLocal = "app.config";
			NomFestival = "BD'Art";
			LaunchBaseFile = true;
			PartAuteurDefaut = 90;
			//
			AppStartupPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			//DossierFichiers = AppStartupPath + Path.DirectorySeparatorChar + "Fichiers";
			DossierFichiers = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bdartlibrairie");
			// création du dossier Fichiers, si pas présent
			if (!Directory.Exists(DossierFichiers))
				Directory.CreateDirectory(DossierFichiers);
			//
			// classes css, vérifier la correspondance avec la liste enum
			string cssdata = ".EntryEditable {background-color: rgb(255, 255, 255); color: rgb(20, 20, 20);}";
            cssdata += ".EntryNotEditable {background-color: rgb(240, 240, 240); color: rgb(50, 50, 50);}";
            cssdata += ".InfoColorRed {color: rgb(255, 0, 0);}";
            cssdata += ".InfoColorBlue {color: rgb(0, 0, 255);}";
            ProviderCss.LoadFromData(cssdata);
        }

		// Téléchargement d'un fichier.
		// D'après la réponse de Tony dans https://stackoverflow.com/questions/45711428/download-file-with-webclient-or-httpclient
		public static async void DownloadFile(Uri uriSource, string strFileDest, Window pWindow)
		{
			try
			{
				using (var client = new HttpClient()) // WebClient
				{
					await client.DownloadFileTaskAsync(uriSource, strFileDest);
				}
			}
			catch(Exception e)
			{
				ShowMessage("Téléchargement", "Fichier " + uriSource.ToString() + Environment.NewLine + e.Message, pWindow);
			}
		}
		public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string strFileDest)
		{
			using (var s = await client.GetStreamAsync(uri))
			{
				using (var fs = new FileStream(strFileDest, FileMode.CreateNew))
				{
					await s.CopyToAsync(fs);
				}
			}
		}

        /// <summary>
		/// Récupère les paramètres de l'application dans le fichier de configuration.
		/// </summary>
		public static void LireConfigLocal(ref string strMsg)
		{
			XmlTextReader reader = null;
        	try
        	{
				// config au niveau utilisateur
				reader = new XmlTextReader(Path.Combine(DossierFichiers, FichierConfigLocal));
				while (reader.Read())
				{
					if (reader.IsStartElement())
					{
						switch (reader.Name)
						{
							case "PrinterFilePath":
								PrinterFilePath = reader.GetAttribute("value");
								break;
							case "Tempo":
								Tempo = Convert.ToInt16(reader.GetAttribute("value"));
								break;
							case "NombreTickets":
								NombreTickets = Convert.ToInt16(reader.GetAttribute("value"));
								break;
							case "AppliquerCss":
								AppliquerCss = Convert.ToBoolean(reader.GetAttribute("value"));
								break;
							case "UseDialogForTicketPrint":
								UseDialogForTicketPrint = Convert.ToBoolean(reader.GetAttribute("value"));
								break;
							case "UsbDevicePath":
								UsbDevicePath = reader.GetAttribute("value");
								break;
							case "NomFestival":
								NomFestival = reader.GetAttribute("value");
								break;
							case "LaunchBaseFile":
								LaunchBaseFile = Convert.ToBoolean(reader.GetAttribute("value"));
								break;
							case "PartAuteurDefaut":
								PartAuteurDefaut = Convert.ToDouble(reader.GetAttribute("value"));
								break;
						}
					}
				}
			}
        	catch (FileNotFoundException)
			{
				// le fichier sera créé par l'application
			}
        	catch (Exception ex)
			{
				strMsg += ex.Message + Environment.NewLine;
			}
			finally
			{
				if (reader != null)
					reader.Close();
			}
		}

        /// <summary>
		/// Ecriture des paramètres utilisateur dans le fichier de configuration.
		/// </summary>
		public static void EcrireConfigLocal(ref string strMsg)
		{
			XmlTextWriter writer = null;
			try
			{
				writer = new XmlTextWriter(Path.Combine(DossierFichiers, FichierConfigLocal), Encoding.UTF8);
				writer.Formatting = Formatting.Indented;

				writer.WriteStartDocument(true);
				writer.WriteStartElement("configuration");
					writer.WriteStartElement("userSettings");
						writer.WriteStartElement("PrinterFilePath");
						writer.WriteAttributeString("value", PrinterFilePath);
						writer.WriteEndElement();

						writer.WriteStartElement("Tempo");
						writer.WriteAttributeString("value", Tempo.ToString());
						writer.WriteEndElement();

						writer.WriteStartElement("NombreTickets");
						writer.WriteAttributeString("value", NombreTickets.ToString());
						writer.WriteEndElement();

						writer.WriteStartElement("AppliquerCss");
						writer.WriteAttributeString("value", AppliquerCss.ToString());
						writer.WriteEndElement();

						writer.WriteStartElement("UseDialogForTicketPrint");
						writer.WriteAttributeString("value", UseDialogForTicketPrint.ToString());
						writer.WriteEndElement();

						writer.WriteStartElement("UsbDevicePath");
						writer.WriteAttributeString("value", UsbDevicePath);
						writer.WriteEndElement();

						writer.WriteStartElement("NomFestival");
						writer.WriteAttributeString("value", NomFestival);
						writer.WriteEndElement();

						writer.WriteStartElement("LaunchBaseFile");
						writer.WriteAttributeString("value", LaunchBaseFile.ToString());
						writer.WriteEndElement();

						writer.WriteStartElement("PartAuteurDefaut");
						writer.WriteAttributeString("value", PartAuteurDefaut.ToString());
						writer.WriteEndElement();
					writer.WriteEndElement();
				writer.WriteEndElement();
				writer.WriteEndDocument();
			}
			catch (Exception ex)
			{
				strMsg += ex.Message + Environment.NewLine;
			}
			finally
			{
				if (writer != null)
					writer.Close();
			}
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
		// NB: sur les appels à cette méthode à partir de l'évenement TextChanged, pour éviter les
		// appels en boucle, il faut désactiver l'évenement TextChanged sur le controle source.
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
					ShowMessage("Erreur", txtBox.Text + ": format incorrect.", pParent);
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
						ShowMessage("Erreur", txtBox.Text + ": format incorrect.", pParent);
					}
				}
			}
			return dblValue;
		}

		public static Int16 GetValueIntOrZero(Window pParent, object sender, bool bShowMessage=false)
		{
			Int16 nValue = 0;
			Entry txtBox = (Entry)sender;
			
			try
			{
				// on tente une conversion en short
				nValue = Convert.ToInt16(txtBox.Text);
			}
			catch (Exception)
			{
				if (bShowMessage == true)
				{
					// seulement si saisie incorrecte
					if (txtBox.Text != string.Empty)
					{
						ShowMessage("Erreur", txtBox.Text + ": format incorrect.", pParent);
					}
				}
			}
			return nValue;
		}

		public static void AfficheInfo(ref Entry txtInfo, string strMsg, eCssClasses eClass)
		{
			txtInfo.Text = string.Empty;
			// on change la couleur du texte, ne pas faire si utilisation d'un thème dark
			if (AppliquerCss == true)
			{
				// le champ contient une seule classe css
				// si déjà présente, on ne fait rien
				if (txtInfo.StyleContext.HasClass(eClass.ToString()) == false)
				{
					if (eClass == eCssClasses.InfoColorBlue)
						txtInfo.StyleContext.RemoveClass(eCssClasses.InfoColorRed.ToString());
					else
						txtInfo.StyleContext.RemoveClass(eCssClasses.InfoColorBlue.ToString());
					txtInfo.StyleContext.AddClass(eClass.ToString());
				}
			}
			txtInfo.Text = strMsg;
			txtInfo.Show();
		}

		// Ajoute le Css provider ainsi que la classe passée en paramètre.
		public static void AddCssProvider(ref Entry txtChamp, eCssClasses eClass)
		{
			txtChamp.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
			txtChamp.StyleContext.AddClass(eClass.ToString());
		}

		public static void AddCssProvider(ref SearchEntry txtChamp, eCssClasses eClass)
		{
			txtChamp.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
			txtChamp.StyleContext.AddClass(eClass.ToString());
		}

		public static void AddCssProvider(ref TextView txtChamp, eCssClasses eClass)
		{
			txtChamp.StyleContext.AddProvider(Global.ProviderCss, Gtk.StyleProviderPriority.User);
			txtChamp.StyleContext.AddClass(eClass.ToString());
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
					ShowMessage("Erreur", txtBox.Text + ": format incorrect.", pParent);
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
					strMsg += Environment.NewLine;
				strMsg = e.Message + Environment.NewLine;
			}
        }
    }
}