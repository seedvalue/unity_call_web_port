using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnePF
{
	public class Inventory
	{
		private Dictionary<string, SkuDetails> _skuMap = new Dictionary<string, SkuDetails>();

		private Dictionary<string, Purchase> _purchaseMap = new Dictionary<string, Purchase>();

		public Inventory(string json)
		{
			JSON jSON = new JSON(json);
			foreach (object item in (List<object>)jSON.fields["purchaseMap"])
			{
				List<object> list = (List<object>)item;
				string key = list[0].ToString();
				Purchase value = new Purchase(list[1].ToString());
				_purchaseMap.Add(key, value);
			}
			foreach (object item2 in (List<object>)jSON.fields["skuMap"])
			{
				List<object> list2 = (List<object>)item2;
				string key2 = list2[0].ToString();
				SkuDetails value2 = new SkuDetails(list2[1].ToString());
				_skuMap.Add(key2, value2);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{purchaseMap:{");
			foreach (KeyValuePair<string, Purchase> item in _purchaseMap)
			{
				stringBuilder.Append("\"" + item.Key + "\":{" + item.Value.ToString() + "},");
			}
			stringBuilder.Append("},");
			stringBuilder.Append("skuMap:{");
			foreach (KeyValuePair<string, SkuDetails> item2 in _skuMap)
			{
				stringBuilder.Append("\"" + item2.Key + "\":{" + item2.Value.ToString() + "},");
			}
			stringBuilder.Append("}}");
			return stringBuilder.ToString();
		}

		public SkuDetails GetSkuDetails(string sku)
		{
			if (!_skuMap.ContainsKey(sku))
			{
				return null;
			}
			return _skuMap[sku];
		}

		public Purchase GetPurchase(string sku)
		{
			if (!_purchaseMap.ContainsKey(sku))
			{
				return null;
			}
			return _purchaseMap[sku];
		}

		public bool HasPurchase(string sku)
		{
			return _purchaseMap.ContainsKey(sku);
		}

		public bool HasDetails(string sku)
		{
			return _skuMap.ContainsKey(sku);
		}

		public void ErasePurchase(string sku)
		{
			if (_purchaseMap.ContainsKey(sku))
			{
				_purchaseMap.Remove(sku);
			}
		}

		public List<string> GetAllOwnedSkus()
		{
			return _purchaseMap.Keys.ToList();
		}

		public List<string> GetAllOwnedSkus(string itemType)
		{
			List<string> list = new List<string>();
			foreach (Purchase value in _purchaseMap.Values)
			{
				if (value.ItemType == itemType)
				{
					list.Add(value.Sku);
				}
			}
			return list;
		}

		public List<Purchase> GetAllPurchases()
		{
			return _purchaseMap.Values.ToList();
		}

		public List<SkuDetails> GetAllAvailableSkus()
		{
			return _skuMap.Values.ToList();
		}

		public void AddSkuDetails(SkuDetails d)
		{
			_skuMap.Add(d.Sku, d);
		}

		public void AddPurchase(Purchase p)
		{
			_purchaseMap.Add(p.Sku, p);
		}
	}
}
