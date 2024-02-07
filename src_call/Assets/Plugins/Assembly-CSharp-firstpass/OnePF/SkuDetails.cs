namespace OnePF
{
	public class SkuDetails
	{
		public string ItemType { get; private set; }

		public string Sku { get; private set; }

		public string Type { get; private set; }

		public string Price { get; private set; }

		public string Title { get; private set; }

		public string Description { get; private set; }

		public string Json { get; private set; }

		public string CurrencyCode { get; private set; }

		public string PriceValue { get; private set; }

		public SkuDetails(string jsonString)
		{
			JSON jSON = new JSON(jsonString);
			ItemType = jSON.ToString("itemType");
			Sku = jSON.ToString("sku");
			Type = jSON.ToString("type");
			Price = jSON.ToString("price");
			Title = jSON.ToString("title");
			Description = jSON.ToString("description");
			Json = jSON.ToString("json");
			CurrencyCode = jSON.ToString("currencyCode");
			PriceValue = jSON.ToString("priceValue");
			ParseFromJson();
		}

		private void ParseFromJson()
		{
			if (!string.IsNullOrEmpty(Json))
			{
				JSON jSON = new JSON(Json);
				if (string.IsNullOrEmpty(PriceValue))
				{
					float num = jSON.ToFloat("price_amount_micros");
					PriceValue = (num / 1000000f).ToString();
				}
				if (string.IsNullOrEmpty(CurrencyCode))
				{
					CurrencyCode = jSON.ToString("price_currency_code");
				}
			}
		}

		public override string ToString()
		{
			return string.Format("[SkuDetails: type = {0}, SKU = {1}, title = {2}, price = {3}, description = {4}, priceValue={5}, currency={6}]", ItemType, Sku, Title, Price, Description, PriceValue, CurrencyCode);
		}
	}
}
