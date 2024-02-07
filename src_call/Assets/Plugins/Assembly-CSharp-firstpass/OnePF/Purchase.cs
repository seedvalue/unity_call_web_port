namespace OnePF
{
	public class Purchase
	{
		public string ItemType { get; private set; }

		public string OrderId { get; private set; }

		public string PackageName { get; private set; }

		public string Sku { get; private set; }

		public long PurchaseTime { get; private set; }

		public int PurchaseState { get; private set; }

		public string DeveloperPayload { get; private set; }

		public string Token { get; private set; }

		public string OriginalJson { get; private set; }

		public string Signature { get; private set; }

		public string AppstoreName { get; private set; }

		public string Receipt { get; private set; }

		private Purchase()
		{
		}

		public Purchase(string jsonString)
		{
			JSON jSON = new JSON(jsonString);
			ItemType = jSON.ToString("itemType");
			OrderId = jSON.ToString("orderId");
			PackageName = jSON.ToString("packageName");
			Sku = jSON.ToString("sku");
			PurchaseTime = jSON.ToLong("purchaseTime");
			PurchaseState = jSON.ToInt("purchaseState");
			DeveloperPayload = jSON.ToString("developerPayload");
			Token = jSON.ToString("token");
			OriginalJson = jSON.ToString("originalJson");
			Signature = jSON.ToString("signature");
			AppstoreName = jSON.ToString("appstoreName");
			Receipt = jSON.ToString("receipt");
		}

		public static Purchase CreateFromSku(string sku)
		{
			return CreateFromSku(sku, string.Empty);
		}

		public static Purchase CreateFromSku(string sku, string developerPayload)
		{
			Purchase purchase = new Purchase();
			purchase.Sku = sku;
			purchase.DeveloperPayload = developerPayload;
			return purchase;
		}

		public override string ToString()
		{
			return "SKU:" + Sku + ";" + OriginalJson;
		}

		public string Serialize()
		{
			JSON jSON = new JSON();
			jSON["itemType"] = ItemType;
			jSON["orderId"] = OrderId;
			jSON["packageName"] = PackageName;
			jSON["sku"] = Sku;
			jSON["purchaseTime"] = PurchaseTime;
			jSON["purchaseState"] = PurchaseState;
			jSON["developerPayload"] = DeveloperPayload;
			jSON["token"] = Token;
			jSON["originalJson"] = OriginalJson;
			jSON["signature"] = Signature;
			jSON["appstoreName"] = AppstoreName;
			jSON["receipt"] = Receipt;
			return jSON.serialized;
		}
	}
}
