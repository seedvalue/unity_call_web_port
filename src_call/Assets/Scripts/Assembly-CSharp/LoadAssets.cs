using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadAssets : MonoBehaviour
{
	public enum Account
	{
		Blockot = 0,
		GalassiaGames = 1,
		TAGGames = 2
	}

	[Serializable]
	public class AllFeatures
	{
		public string[] URL;

		public string FeatureURL;

		public string[] OpenURL;

		public string FeatureOpenURL;

		public string[] thumbUrl;

		public string[] GameTitle;

		public Texture2D[] LargeGame;

		public GameObject[] LargeImage;

		public Texture2D FeatureTex;

		public Texture2D[] Tex;
	}

	[Serializable]
	public class DownloadableData
	{
		public string _gameTitle;

		public string _url;

		public string _openUrl;

		public int _featureIndex;

		public int _thumbIndex;
	}

	public static LoadAssets instance;

	public Account accountName;

	public bool FetchDataFromLocal;

	public List<ObjectCreator> allObjects;

	public AllFeatures[] allFeatures;

	public int[] ArrayLength;

	public string VersionNumber = string.Empty;

	public int TotalHits;

	public int TotalBytes;

	public Text GameTitleText;

	private string SelectedURL = string.Empty;

	public RawImage Feature;

	private int initializer;

	private List<GameObject> largeGame;

	public Animator anim;

	public GameObject anrdoid;

	public GameObject ios;

	private float targetPosition;

	private WWW www;

	private bool changePosition;

	public GameObject LoadingImage;

	public Animator animImage;

	[HideInInspector]
	public float initialPosition;

	private TextAsset XMLData;

	private List<XmlNodeList> AllGames = new List<XmlNodeList>();

	[HideInInspector]
	public string GamesType = "NewGames";

	private string LocalVersion;

	private string ServerVersion;

	private XmlDocument xmlDocLocal;

	private XmlDocument xmlDocServer;

	public Text DownloadDataTxt;

	public Text HitsTxt;

	public Text versionTxt;

	private int totalCount;

	private int featureArrayIndex;

	public int featureArrayLenght;

	public int thumbArrayLength;

	public int featureIndex;

	public int thumbIndex;

	private int TabClickIndex;

	private bool isDowloading;

	private int allgameindex;

	private int currIndex;

	private int totalListCount;

	private int check;

	public List<DownloadableData> downloadableLinkList;

	private void LoadXMLToList(XmlDocument xmlDoc)
	{
		XmlNodeList xmlNodeList = null;
		XmlNodeList xmlNodeList2 = null;
		XmlNodeList xmlNodeList3 = null;
		XmlNodeList xmlNodeList4 = null;
		XmlNodeList xmlNodeList5 = null;
		string text = string.Empty;
		string empty = string.Empty;
		empty = "Android";
		if (accountName == Account.Blockot)
		{
			text = "Blockot";
		}
		else if (accountName == Account.GalassiaGames)
		{
			text = "Galassia";
		}
		else if (accountName == Account.TAGGames)
		{
			text = "TAG";
		}
		xmlNodeList = xmlDoc.SelectNodes("MoreGames/" + empty + "/" + text + "/AllGames/Action/NewGames/Game");
		xmlNodeList2 = xmlDoc.SelectNodes("MoreGames/" + empty + "/" + text + "/AllGames/Action/OldGames/Game");
		xmlNodeList3 = xmlDoc.SelectNodes("MoreGames/" + empty + "/" + text + "/AllGames/Simulations/NewGames/Game");
		xmlNodeList4 = xmlDoc.SelectNodes("MoreGames/" + empty + "/" + text + "/AllGames/Simulations/OldGames/Game");
		xmlNodeList5 = xmlDoc.SelectNodes("MoreGames/" + empty + "/" + text + "/FeaturedGames/Game");
		AllGames.Add(xmlNodeList);
		AllGames.Add(xmlNodeList2);
		AllGames.Add(xmlNodeList3);
		AllGames.Add(xmlNodeList4);
		AllGames.Add(xmlNodeList5);
	}

	private int GetXMLCount(string gamesType)
	{
		int num = 0;
		switch (gamesType)
		{
		case "AllGames":
			num = 0;
			foreach (XmlNodeList allGame in AllGames)
			{
				num += allGame.Count;
			}
			break;
		case "ActionGameNew":
			num = 0;
			num = AllGames[0].Count;
			break;
		case "ActionGameOld":
			num = 0;
			num = AllGames[1].Count;
			break;
		case "SimulationsNew":
			num = 0;
			num = AllGames[2].Count;
			break;
		case "SimulationOld":
			num = 0;
			num = AllGames[3].Count;
			break;
		case "NewGames":
			num = 0;
			num = AllGames[0].Count;
			num += AllGames[2].Count;
			break;
		case "ActionGames":
			num = 0;
			num = AllGames[0].Count;
			num += AllGames[1].Count;
			break;
		case "SimuationGames":
			num = 0;
			num = AllGames[2].Count;
			num += AllGames[3].Count;
			break;
		case "FeaturedGames":
			num = 0;
			num = AllGames[4].Count;
			break;
		}
		return num;
	}

	private void ShowResults(string gamesType, int index)
	{
		switch (gamesType)
		{
		case "AllGames":
			initializer = 0;
			ProcessGames(AllGames[0], index);
			ProcessGames(AllGames[1], index);
			ProcessGames(AllGames[2], index);
			ProcessGames(AllGames[3], index);
			break;
		case "ActionGameNew":
			initializer = 0;
			ProcessGames(AllGames[0], index);
			break;
		case "ActionGameOld":
			initializer = 0;
			ProcessGames(AllGames[1], index);
			break;
		case "SimulationsNew":
			initializer = 0;
			ProcessGames(AllGames[2], index);
			break;
		case "SimulationOld":
			initializer = 0;
			ProcessGames(AllGames[3], index);
			break;
		case "NewGames":
			initializer = 0;
			ProcessGames(AllGames[0], index);
			ProcessGames(AllGames[2], index);
			break;
		case "ActionGames":
			initializer = 0;
			ProcessGames(AllGames[0], index);
			ProcessGames(AllGames[1], index);
			break;
		case "SimuationGames":
			initializer = 0;
			ProcessGames(AllGames[2], index);
			ProcessGames(AllGames[3], index);
			break;
		case "FeaturedGames":
			initializer = 0;
			ProcessGames(AllGames[4], index);
			break;
		}
	}

	private void ReadFile(string file)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(file);
	}

	public void SetInitialReferences(XmlDocument xmlDoc)
	{
		LoadXMLToList(xmlDoc);
		allFeatures = new AllFeatures[allObjects.Count];
		for (int i = 0; i < allFeatures.Length; i++)
		{
			allFeatures[i] = new AllFeatures();
		}
		ArrayLength = new int[allObjects.Count];
		ArrayLength[0] = GetXMLCount("NewGames");
		ArrayLength[1] = GetXMLCount("ActionGameOld");
		ArrayLength[2] = GetXMLCount("SimulationOld");
		ArrayLength[3] = GetXMLCount("AllGames");
		ArrayLength[4] = GetXMLCount("FeaturedGames");
		for (int j = 0; j < ArrayLength.Length; j++)
		{
			int num = ArrayLength[j];
			allFeatures[j].URL = new string[num];
			allFeatures[j].Tex = new Texture2D[num];
			allFeatures[j].OpenURL = new string[num];
			allFeatures[j].thumbUrl = new string[num];
			allFeatures[j].GameTitle = new string[num];
			allFeatures[j].LargeGame = new Texture2D[num];
			allObjects[j].Size = ArrayLength[j];
			allObjects[j].SetArrayLength();
			allObjects[j].Init();
		}
	}

	private IEnumerator Start()
	{
		if (instance == null)
		{
			instance = this;
		}
		
		Debug.LogError("Скачка ассетов");
		yield return null;
		
		/*
		 
		
		xmlDocLocal = new XmlDocument();
		if (FetchDataFromLocal)
		{
			TextAsset textAsset = Resources.Load("MoreGames_Updated") as TextAsset;
			xmlDocLocal.LoadXml(textAsset.text);
		}
		else
		{
			string url_1 = "http://blockot.com/games/moregames/blockot/MoreGames_Updated.xml";
			www = new WWW(url_1);
			yield return www;
			if (www.error == null)
			{
				if (www.isDone)
				{
					MonoBehaviour.print("Successfully downloaded xml from server...~GameDonar");
					xmlDocServer = new XmlDocument();
					xmlDocLocal.LoadXml(www.text);
					www.Dispose();
				}
			}
			else
			{
				TextAsset textAsset2 = Resources.Load("MoreGames_Updated") as TextAsset;
				xmlDocLocal.LoadXml(textAsset2.text);
				MonoBehaviour.print("getting data from resources...~GameDonar");
			}
			GamesButtonClick(0);
		}
		SetInitialReferences(xmlDocLocal);
		ShowResults("NewGames", 0);
		ShowResults("ActionGameOld", 1);
		ShowResults("SimulationOld", 2);
		ShowResults("FeaturedGames", 4);
		ProcessAllGames();
		if (FetchDataFromLocal)
		{
			gettingDataLocally();
		}
		foreach (ObjectCreator allObject in allObjects)
		{
			allObject.OnRequired(0);
		}
		downLoadDataForTab(0);
		downLoadDataForTab(1);
		downLoadDataForTab(2);
		downLoadDataForTab(4);
		 */
	}

	public void gettingDataLocally()
	{
		for (int i = 0; i < allFeatures.Length; i++)
		{
			if (i == 3)
			{
				continue;
			}
			for (int j = 0; j < allFeatures[i].GameTitle.Length; j++)
			{
				if (CheckFileExistInResources(allFeatures[i].GameTitle[j]))
				{
					Texture2D texture2D = loadTextureFromResouces(allFeatures[i].GameTitle[j]);
					if (allFeatures[i].LargeGame[j] == null)
					{
						allFeatures[i].LargeGame[j] = texture2D;
						allFeatures[i].Tex[j] = texture2D;
						allObjects[i].Change_Large_Image(texture2D, j, i);
						allObjects[i].Change_Small_Icons(texture2D, j);
					}
					for (int k = 0; k < allFeatures[3].GameTitle.Length; k++)
					{
						if (allFeatures[3].GameTitle[k] == allFeatures[i].GameTitle[j] && allFeatures[3].LargeGame[k] == null)
						{
							allFeatures[3].LargeGame[k] = texture2D;
							allFeatures[3].Tex[k] = texture2D;
							allObjects[3].Change_Large_Image(texture2D, k, 3);
							allObjects[3].Change_Small_Icons(texture2D, k);
						}
					}
				}
				else
				{
					Debug.LogError("File Not Found : " + allFeatures[i].GameTitle[j]);
				}
			}
		}
	}

	private bool CheckFileExistInResources(string FileName)
	{
		Texture2D texture2D = Resources.Load("LocalData/" + FileName) as Texture2D;
		if (texture2D != null)
		{
			return true;
		}
		return false;
	}

	private Texture2D loadTextureFromResouces(string FileName)
	{
		return Resources.Load("LocalData/" + FileName) as Texture2D;
	}

	private void ProcessGames(XmlNodeList nodes, int index)
	{
		if (index == 3 || index > 4)
		{
			return;
		}
		foreach (XmlNode node in nodes)
		{
			allFeatures[index].URL[initializer] = node.SelectSingleNode("Large").InnerText;
			allFeatures[index].thumbUrl[initializer] = allFeatures[index].URL[initializer];
			allFeatures[index].OpenURL[initializer] = node.SelectSingleNode("PlayStoreLink").InnerText;
			allFeatures[index].GameTitle[initializer] = node.SelectSingleNode("Title").InnerText;
			initializer++;
		}
	}

	private void ProcessAllGames()
	{
		int num = 0;
		if (num <= allFeatures[3].URL.Length)
		{
		}
		num = 0;
		for (int i = 0; i < allFeatures.Length; i++)
		{
			if (i != 3)
			{
				for (int j = 0; j < allFeatures[i].URL.Length; j++)
				{
					allFeatures[3].thumbUrl[num] = allFeatures[i].thumbUrl[j];
					allFeatures[3].URL[num] = allFeatures[i].URL[j];
					allFeatures[3].OpenURL[num] = allFeatures[i].OpenURL[j];
					allFeatures[3].GameTitle[num] = allFeatures[i].GameTitle[j];
					num++;
				}
			}
		}
	}

	private void LoadAssetInAllGame()
	{
		int num = 0;
		for (int i = 0; i < allFeatures[0].LargeGame.Length; i++)
		{
			allFeatures[3].LargeGame[num] = allFeatures[0].LargeGame[i];
			allFeatures[3].Tex[num] = allFeatures[0].Tex[i];
			allObjects[3].Change_Large_Image(allFeatures[3].LargeGame[num], num, 3);
			allObjects[3].Change_Small_Icons(allFeatures[3].Tex[num], num);
			num++;
		}
		for (int j = 0; j < allFeatures[1].LargeGame.Length; j++)
		{
			allFeatures[3].LargeGame[num] = allFeatures[1].LargeGame[j];
			allFeatures[3].FeatureTex = allFeatures[1].FeatureTex;
			allFeatures[3].Tex[num] = allFeatures[1].Tex[j];
			allObjects[3].Change_Large_Image(allFeatures[3].LargeGame[num], num, 3);
			allObjects[3].Change_Small_Icons(allFeatures[3].Tex[num], num);
			num++;
		}
		for (int k = 0; k < allFeatures[2].LargeGame.Length; k++)
		{
			allFeatures[3].LargeGame[num] = allFeatures[2].LargeGame[k];
			allFeatures[3].Tex[num] = allFeatures[2].Tex[k];
			allObjects[3].Change_Large_Image(allFeatures[3].LargeGame[num], num, 3);
			allObjects[3].Change_Small_Icons(allFeatures[3].Tex[num], num);
			num++;
		}
	}

	public void DeleteData()
	{
		for (int i = 0; i < allFeatures[3].URL.Length; i++)
		{
			if (CheckFileExists(allFeatures[3].GameTitle[i] + "_Large"))
			{
				File.Delete(Application.persistentDataPath + "/" + allFeatures[3].GameTitle[i] + "_Large");
			}
		}
	}

	public void ReloadScene()
	{
		SceneManager.LoadScene(0);
	}

	private IEnumerator GetDataFromLinks()
	{
		while (featureIndex < featureArrayLenght)
		{
			string url = allFeatures[featureIndex].URL[thumbIndex];
			thumbArrayLength = allFeatures[featureIndex].URL.Length;
			Texture2D texture = new Texture2D(1, 1);
			MonoBehaviour.print("Downloading....");
			WWW www = new WWW(url);
			yield return www;
			if (www.error != null)
			{
				Debug.LogError("Error: " + www.error + " ~GameDonar");
				www.Dispose();
				MonoBehaviour.print("Trying to reload: File : " + url);
				yield return null;
			}
			if (www.isDone)
			{
				www.LoadImageIntoTexture(texture);
				TotalBytes += www.bytesDownloaded;
				TotalHits++;
				float dataInKB = (float)TotalBytes * 1f / 1024f;
				float dataInMb = dataInKB / 1024f;
				DownloadDataTxt.text = dataInMb.ToString("00.00") + " MB " + dataInKB.ToString("00.00") + " KB";
				HitsTxt.text = TotalHits + "Hit(s)";
				allFeatures[3].LargeGame[thumbIndex] = texture;
				allFeatures[3].Tex[thumbIndex] = texture;
				allObjects[3].Change_Large_Image(texture, thumbIndex, 3);
				allObjects[3].Change_Small_Icons(texture, thumbIndex);
				allFeatures[featureIndex].LargeGame[thumbIndex] = texture;
				allFeatures[featureIndex].Tex[thumbIndex] = texture;
				allObjects[featureIndex].Change_Large_Image(texture, thumbIndex, 3);
				allObjects[featureIndex].Change_Small_Icons(texture, thumbIndex);
				thumbIndex++;
				if (thumbIndex >= thumbArrayLength)
				{
					thumbIndex = 0;
					featureIndex++;
					yield return null;
				}
				www.Dispose();
			}
		}
		MonoBehaviour.print("downloading Completed....");
		MonoBehaviour.print("UI Completed....");
		PlayerPrefs.SetInt("AllGameDataDownloaded", 0);
		StopAllCoroutines();
	}

	private bool CheckFileExists(string fileName)
	{
		try
		{
			if (File.Exists(Application.persistentDataPath + "/" + fileName))
			{
				return true;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	private void GamesButtonClick(int ffIndex)
	{
		MonoBehaviour.print(featureIndex + " preddedasdfhsgjdhasd");
		for (int i = 0; i < allFeatures[ffIndex].URL.Length; i++)
		{
			MonoBehaviour.print("Feature : " + ffIndex + "\nThumb index : " + i);
			getDataByTab(ffIndex, i);
		}
		if (downloadableLinkList.Count > 0 && !isDowloading)
		{
			StopAllCoroutines();
			StartCoroutine(downloadingQueData());
			isDowloading = true;
		}
	}

	private void getDataByTab(int fIndex, int tIndex)
	{
		if (downloadableLinkList.Count > 0)
		{
			int num = 0;
			if (num <= 0)
			{
				DownloadableData downloadableData = new DownloadableData();
				downloadableData._gameTitle = allFeatures[fIndex].GameTitle[tIndex];
				downloadableData._featureIndex = fIndex;
				downloadableData._thumbIndex = tIndex;
				downloadableData._url = allFeatures[fIndex].URL[tIndex];
				downloadableData._openUrl = allFeatures[fIndex].OpenURL[tIndex];
				downloadableLinkList.Add(downloadableData);
			}
		}
		else
		{
			DownloadableData downloadableData2 = new DownloadableData();
			downloadableData2._gameTitle = allFeatures[fIndex].GameTitle[tIndex];
			downloadableData2._featureIndex = fIndex;
			downloadableData2._thumbIndex = tIndex;
			downloadableData2._url = allFeatures[fIndex].URL[tIndex];
			downloadableData2._openUrl = allFeatures[fIndex].OpenURL[tIndex];
			downloadableLinkList.Add(downloadableData2);
		}
	}

	private IEnumerator downloadingQueData()
	{
		Debug.LogError("ТУТ АССЕТЫ КАКИЕ ТО КАЧАЮТСЯ ! чекнуть потом");
		
		/*
		totalListCount = downloadableLinkList.Count;
		currIndex = 0;
		while (totalListCount > 0)
		{
			string downloadableLink = downloadableLinkList[currIndex]._url;
			Texture2D texture2 = new Texture2D(1, 1);
			MonoBehaviour.print("Downloading....");
			WWW www = new WWW(downloadableLink);
			yield return www;
			if (www.error != null)
			{
				Debug.LogError("Error: " + www.error + " ~GameDonar");
				www.Dispose();
				MonoBehaviour.print("Trying to reload: File : ");
				yield return null;
			}
			if (!www.isDone)
			{
				continue;
			}
			MonoBehaviour.print("done..");
			texture2 = www.texture;
			for (int i = 0; i < allFeatures[3].URL.Length; i++)
			{
				if (allFeatures[3].URL[i] == downloadableLinkList[currIndex]._url && allFeatures[3].LargeGame[i] == null)
				{
					allFeatures[3].LargeGame[i] = texture2;
					allFeatures[3].Tex[i] = texture2;
					allObjects[3].Change_Large_Image(texture2, i, 3);
					allObjects[3].Change_Small_Icons(texture2, i);
				}
			}
			if (allFeatures[downloadableLinkList[currIndex]._featureIndex].LargeGame[downloadableLinkList[currIndex]._thumbIndex] == null)
			{
				allFeatures[downloadableLinkList[currIndex]._featureIndex].LargeGame[downloadableLinkList[currIndex]._thumbIndex] = texture2;
				allFeatures[downloadableLinkList[currIndex]._featureIndex].Tex[downloadableLinkList[currIndex]._thumbIndex] = texture2;
				allObjects[downloadableLinkList[currIndex]._featureIndex].Change_Large_Image(texture2, downloadableLinkList[currIndex]._thumbIndex, downloadableLinkList[currIndex]._featureIndex);
				allObjects[downloadableLinkList[currIndex]._featureIndex].Change_Small_Icons(texture2, downloadableLinkList[currIndex]._thumbIndex);
			}
			downloadableLinkList[currIndex] = new DownloadableData();
			if (downloadableLinkList.Contains(downloadableLinkList[currIndex]))
			{
				downloadableLinkList.Remove(downloadableLinkList[currIndex]);
			}
			totalListCount = downloadableLinkList.Count;
			www.Dispose();
		}
		MonoBehaviour.print("downloading Completed....");
		MonoBehaviour.print("UI Completed....");
		isDowloading = false;
		StopAllCoroutines();
		*/
		StopAllCoroutines();
		yield return null;
	}

	public void downloadLinks()
	{
	}

	public void downLoadDataForTab(int TabClickIndex)
	{
		if (!FetchDataFromLocal)
		{
			GamesButtonClick(TabClickIndex);
		}
	}

	private IEnumerator GetGamesLinks(int index, int panel)
	{
		string url = allFeatures[panel].URL[index];
		Texture2D texture = new Texture2D(1, 1);
		WWW www = new WWW(url);
		TotalHits++;
		yield return www;
		DownloadDataTxt.text = www.bytesDownloaded.ToString();
		TotalBytes = www.bytesDownloaded;
		www.LoadImageIntoTexture(texture);
		allFeatures[panel].LargeGame[index] = texture;
		allFeatures[panel].Tex[index] = texture;
		allObjects[panel].Change_Large_Image(texture, index, panel);
		allObjects[panel].Change_Small_Icons(texture, index);
		totalCount++;
		if (totalCount >= allFeatures[3].LargeGame.Length)
		{
			PlayerPrefs.SetInt("AllGameDataDownloaded", 0);
		}
	}

	public void OnFeatureClick(int panel, int index)
	{
		Application.OpenURL(allFeatures[panel].OpenURL[index]);
	}

	private Texture2D LoadTextureToFile(string fileName)
	{
		Texture2D texture2D = new Texture2D(1, 1);
		Debug.Log("pATH" + Application.persistentDataPath + "/" + fileName);
		byte[] data = File.ReadAllBytes(Application.persistentDataPath + "/" + fileName);
		texture2D.LoadImage(data);
		return texture2D;
	}

	private void WriteTexturesToFile(Texture2D texture, string filename)
	{
		try
		{
			File.WriteAllBytes(Application.persistentDataPath + "/" + filename, texture.EncodeToPNG());
			Debug.Log("pATH" + Application.persistentDataPath + "/" + filename);
		}
		catch
		{
		}
	}

	public void CloseApp()
	{
		Application.Quit();
		MonoBehaviour.print("QuitApp");
	}

	private void SaveAllData()
	{
		int num = 0;
		for (int i = 0; i < allFeatures.Length; i++)
		{
			for (int j = 0; j < allFeatures[i].LargeGame.Length; j++)
			{
				WriteTexturesToFile(allFeatures[i].LargeGame[j], allFeatures[i].GameTitle[j] + "_Large_" + num);
				num++;
			}
		}
	}

	private void LoadAllData()
	{
		try
		{
			for (int i = 0; i < allFeatures.Length; i++)
			{
				for (int j = 0; j < allFeatures[i].LargeGame.Length; j++)
				{
					allFeatures[i].LargeGame[j] = LoadTextureToFile(allFeatures[i].GameTitle[j] + "_Large");
					allFeatures[i].Tex[j] = allFeatures[i].LargeGame[j];
					allObjects[i].Change_Small_Icons(allFeatures[i].Tex[j], j);
					allObjects[i].Change_Large_Image(allFeatures[i].LargeGame[j], j, 0);
				}
			}
		}
		catch (Exception)
		{
			Debug.Log("Files not found");
		}
	}

	private void LoadDataInToLayouts()
	{
		for (int i = 0; i < allFeatures.Length; i++)
		{
			if (i != 3)
			{
				for (int j = 0; j < allFeatures[i].thumbUrl.Length; j++)
				{
					StartCoroutine(GetGamesLinks(j, i));
				}
			}
		}
	}

	public void closeforAds()
	{
		Debug.Log("closeforAds");
		/*
		if(IntegrationManager.Instance)IntegrationManager.Instance.OnMoreGames();
		else Debug.LogError("IntegrationManager.Instance == NULL");
		*/
	}
}
