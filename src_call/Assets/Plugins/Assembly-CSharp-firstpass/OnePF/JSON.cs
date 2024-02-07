using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace OnePF
{
	public class JSON
	{
		private sealed class _JSON
		{
			private sealed class Parser : IDisposable
			{
				private enum TOKEN
				{
					NONE = 0,
					CURLY_OPEN = 1,
					CURLY_CLOSE = 2,
					SQUARED_OPEN = 3,
					SQUARED_CLOSE = 4,
					COLON = 5,
					COMMA = 6,
					STRING = 7,
					NUMBER = 8,
					TRUE = 9,
					FALSE = 10,
					NULL = 11
				}

				private const string WHITE_SPACE = " \t\n\r";

				private const string WORD_BREAK = " \t\n\r{}[],:\"";

				private StringReader json;

				private char PeekChar
				{
					get
					{
						return Convert.ToChar(json.Peek());
					}
				}

				private char NextChar
				{
					get
					{
						return Convert.ToChar(json.Read());
					}
				}

				private string NextWord
				{
					get
					{
						StringBuilder stringBuilder = new StringBuilder();
						while (" \t\n\r{}[],:\"".IndexOf(PeekChar) == -1)
						{
							stringBuilder.Append(NextChar);
							if (json.Peek() == -1)
							{
								break;
							}
						}
						return stringBuilder.ToString();
					}
				}

				private TOKEN NextToken
				{
					get
					{
						EatWhitespace();
						if (json.Peek() == -1)
						{
							return TOKEN.NONE;
						}
						switch (PeekChar)
						{
						case '{':
							return TOKEN.CURLY_OPEN;
						case '}':
							json.Read();
							return TOKEN.CURLY_CLOSE;
						case '[':
							return TOKEN.SQUARED_OPEN;
						case ']':
							json.Read();
							return TOKEN.SQUARED_CLOSE;
						case ',':
							json.Read();
							return TOKEN.COMMA;
						case '"':
							return TOKEN.STRING;
						case ':':
							return TOKEN.COLON;
						case '-':
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							return TOKEN.NUMBER;
						default:
							switch (NextWord)
							{
							case "false":
								return TOKEN.FALSE;
							case "true":
								return TOKEN.TRUE;
							case "null":
								return TOKEN.NULL;
							default:
								return TOKEN.NONE;
							}
						}
					}
				}

				private Parser(string jsonString)
				{
					json = new StringReader(jsonString);
				}

				public static JSON Parse(string jsonString)
				{
					using (Parser parser = new Parser(jsonString))
					{
						return parser.ParseValue() as JSON;
					}
				}

				public void Dispose()
				{
					json.Dispose();
					json = null;
				}

				private JSON ParseObject()
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					JSON jSON = new JSON();
					jSON.fields = dictionary;
					json.Read();
					while (true)
					{
						switch (NextToken)
						{
						case TOKEN.COMMA:
							continue;
						case TOKEN.NONE:
							return null;
						case TOKEN.CURLY_CLOSE:
							return jSON;
						}
						string text = ParseString();
						if (text == null)
						{
							return null;
						}
						if (NextToken != TOKEN.COLON)
						{
							return null;
						}
						json.Read();
						dictionary[text] = ParseValue();
					}
				}

				private List<object> ParseArray()
				{
					List<object> list = new List<object>();
					json.Read();
					bool flag = true;
					while (flag)
					{
						TOKEN nextToken = NextToken;
						switch (nextToken)
						{
						case TOKEN.NONE:
							return null;
						case TOKEN.SQUARED_CLOSE:
							flag = false;
							break;
						default:
						{
							object item = ParseByToken(nextToken);
							list.Add(item);
							break;
						}
						case TOKEN.COMMA:
							break;
						}
					}
					return list;
				}

				private object ParseValue()
				{
					TOKEN nextToken = NextToken;
					return ParseByToken(nextToken);
				}

				private object ParseByToken(TOKEN token)
				{
					switch (token)
					{
					case TOKEN.STRING:
						return ParseString();
					case TOKEN.NUMBER:
						return ParseNumber();
					case TOKEN.CURLY_OPEN:
						return ParseObject();
					case TOKEN.SQUARED_OPEN:
						return ParseArray();
					case TOKEN.TRUE:
						return true;
					case TOKEN.FALSE:
						return false;
					case TOKEN.NULL:
						return null;
					default:
						return null;
					}
				}

				private string ParseString()
				{
					StringBuilder stringBuilder = new StringBuilder();
					json.Read();
					bool flag = true;
					while (flag)
					{
						if (json.Peek() == -1)
						{
							flag = false;
							break;
						}
						char nextChar = NextChar;
						switch (nextChar)
						{
						case '"':
							flag = false;
							break;
						case '\\':
							if (json.Peek() == -1)
							{
								flag = false;
								break;
							}
							nextChar = NextChar;
							switch (nextChar)
							{
							case '"':
							case '/':
							case '\\':
								stringBuilder.Append(nextChar);
								break;
							case 'b':
								stringBuilder.Append('\b');
								break;
							case 'f':
								stringBuilder.Append('\f');
								break;
							case 'n':
								stringBuilder.Append('\n');
								break;
							case 'r':
								stringBuilder.Append('\r');
								break;
							case 't':
								stringBuilder.Append('\t');
								break;
							case 'u':
							{
								StringBuilder stringBuilder2 = new StringBuilder();
								for (int i = 0; i < 4; i++)
								{
									stringBuilder2.Append(NextChar);
								}
								stringBuilder.Append((char)Convert.ToInt32(stringBuilder2.ToString(), 16));
								break;
							}
							}
							break;
						default:
							stringBuilder.Append(nextChar);
							break;
						}
					}
					return stringBuilder.ToString();
				}

				private object ParseNumber()
				{
					string nextWord = NextWord;
					if (nextWord.IndexOf('.') == -1)
					{
						long result;
						long.TryParse(nextWord, out result);
						return result;
					}
					double result2;
					double.TryParse(nextWord, out result2);
					return result2;
				}

				private void EatWhitespace()
				{
					while (" \t\n\r".IndexOf(PeekChar) != -1)
					{
						json.Read();
						if (json.Peek() == -1)
						{
							break;
						}
					}
				}
			}

			private sealed class Serializer
			{
				private StringBuilder builder;

				private Serializer()
				{
					builder = new StringBuilder();
				}

				public static string Serialize(JSON obj)
				{
					Serializer serializer = new Serializer();
					serializer.SerializeValue(obj);
					return serializer.builder.ToString();
				}

				private void SerializeValue(object value)
				{
					if (value == null)
					{
						builder.Append("null");
					}
					else if (value is string)
					{
						SerializeString(value as string);
					}
					else if (value is bool)
					{
						builder.Append(value.ToString().ToLower());
					}
					else if (value is JSON)
					{
						SerializeObject(value as JSON);
					}
					else if (value is IDictionary)
					{
						SerializeDictionary(value as IDictionary);
					}
					else if (value is IList)
					{
						SerializeArray(value as IList);
					}
					else if (value is char)
					{
						SerializeString(value.ToString());
					}
					else
					{
						SerializeOther(value);
					}
				}

				private void SerializeObject(JSON obj)
				{
					SerializeDictionary(obj.fields);
				}

				private void SerializeDictionary(IDictionary obj)
				{
					bool flag = true;
					builder.Append('{');
					foreach (object key in obj.Keys)
					{
						if (!flag)
						{
							builder.Append(',');
						}
						SerializeString(key.ToString());
						builder.Append(':');
						SerializeValue(obj[key]);
						flag = false;
					}
					builder.Append('}');
				}

				private void SerializeArray(IList anArray)
				{
					builder.Append('[');
					bool flag = true;
					foreach (object item in anArray)
					{
						if (!flag)
						{
							builder.Append(',');
						}
						SerializeValue(item);
						flag = false;
					}
					builder.Append(']');
				}

				private void SerializeString(string str)
				{
					builder.Append('"');
					char[] array = str.ToCharArray();
					char[] array2 = array;
					foreach (char c in array2)
					{
						switch (c)
						{
						case '"':
							builder.Append("\\\"");
							continue;
						case '\\':
							builder.Append("\\\\");
							continue;
						case '\b':
							builder.Append("\\b");
							continue;
						case '\f':
							builder.Append("\\f");
							continue;
						case '\n':
							builder.Append("\\n");
							continue;
						case '\r':
							builder.Append("\\r");
							continue;
						case '\t':
							builder.Append("\\t");
							continue;
						}
						int num = Convert.ToInt32(c);
						if (num >= 32 && num <= 126)
						{
							builder.Append(c);
						}
						else
						{
							builder.Append("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
						}
					}
					builder.Append('"');
				}

				private void SerializeOther(object value)
				{
					if (value is float || value is int || value is uint || value is long || value is double || value is sbyte || value is byte || value is short || value is ushort || value is ulong || value is decimal)
					{
						builder.Append(value.ToString());
					}
					else
					{
						SerializeString(value.ToString());
					}
				}
			}

			public static JSON Deserialize(string json)
			{
				if (json == null)
				{
					return null;
				}
				return Parser.Parse(json);
			}

			public static string Serialize(JSON obj)
			{
				return Serializer.Serialize(obj);
			}
		}

		public Dictionary<string, object> fields = new Dictionary<string, object>();

		public object this[string fieldName]
		{
			get
			{
				if (fields.ContainsKey(fieldName))
				{
					return fields[fieldName];
				}
				return null;
			}
			set
			{
				if (fields.ContainsKey(fieldName))
				{
					fields[fieldName] = value;
				}
				else
				{
					fields.Add(fieldName, value);
				}
			}
		}

		public string serialized
		{
			get
			{
				return _JSON.Serialize(this);
			}
			set
			{
				JSON jSON = _JSON.Deserialize(value);
				if (jSON != null)
				{
					fields = jSON.fields;
				}
			}
		}

		public JSON()
		{
		}

		public JSON(string jsonString)
		{
			serialized = jsonString;
		}

		public string ToString(string fieldName)
		{
			if (fields.ContainsKey(fieldName))
			{
				return Convert.ToString(fields[fieldName]);
			}
			return string.Empty;
		}

		public int ToInt(string fieldName)
		{
			if (fields.ContainsKey(fieldName))
			{
				return Convert.ToInt32(fields[fieldName]);
			}
			return 0;
		}

		public long ToLong(string fieldName)
		{
			if (fields.ContainsKey(fieldName))
			{
				return Convert.ToInt64(fields[fieldName]);
			}
			return 0L;
		}

		public float ToFloat(string fieldName)
		{
			if (fields.ContainsKey(fieldName))
			{
				return Convert.ToSingle(fields[fieldName]);
			}
			return 0f;
		}

		public bool ToBoolean(string fieldName)
		{
			if (fields.ContainsKey(fieldName))
			{
				return Convert.ToBoolean(fields[fieldName]);
			}
			return false;
		}

		public JSON ToJSON(string fieldName)
		{
			if (!fields.ContainsKey(fieldName))
			{
				fields.Add(fieldName, new JSON());
			}
			return (JSON)this[fieldName];
		}

		public static implicit operator Vector2(JSON value)
		{
			return new Vector3(Convert.ToSingle(value["x"]), Convert.ToSingle(value["y"]));
		}

		public static explicit operator JSON(Vector2 value)
		{
			JSON jSON = new JSON();
			jSON["x"] = value.x;
			jSON["y"] = value.y;
			return jSON;
		}

		public static implicit operator Vector3(JSON value)
		{
			return new Vector3(Convert.ToSingle(value["x"]), Convert.ToSingle(value["y"]), Convert.ToSingle(value["z"]));
		}

		public static explicit operator JSON(Vector3 value)
		{
			JSON jSON = new JSON();
			jSON["x"] = value.x;
			jSON["y"] = value.y;
			jSON["z"] = value.z;
			return jSON;
		}

		public static implicit operator Quaternion(JSON value)
		{
			return new Quaternion(Convert.ToSingle(value["x"]), Convert.ToSingle(value["y"]), Convert.ToSingle(value["z"]), Convert.ToSingle(value["w"]));
		}

		public static explicit operator JSON(Quaternion value)
		{
			JSON jSON = new JSON();
			jSON["x"] = value.x;
			jSON["y"] = value.y;
			jSON["z"] = value.z;
			jSON["w"] = value.w;
			return jSON;
		}

		public static implicit operator Color(JSON value)
		{
			return new Color(Convert.ToSingle(value["r"]), Convert.ToSingle(value["g"]), Convert.ToSingle(value["b"]), Convert.ToSingle(value["a"]));
		}

		public static explicit operator JSON(Color value)
		{
			JSON jSON = new JSON();
			jSON["r"] = value.r;
			jSON["g"] = value.g;
			jSON["b"] = value.b;
			jSON["a"] = value.a;
			return jSON;
		}

		public static implicit operator Color32(JSON value)
		{
			return new Color32(Convert.ToByte(value["r"]), Convert.ToByte(value["g"]), Convert.ToByte(value["b"]), Convert.ToByte(value["a"]));
		}

		public static explicit operator JSON(Color32 value)
		{
			JSON jSON = new JSON();
			jSON["r"] = value.r;
			jSON["g"] = value.g;
			jSON["b"] = value.b;
			jSON["a"] = value.a;
			return jSON;
		}

		public static implicit operator Rect(JSON value)
		{
			return new Rect((int)Convert.ToByte(value["left"]), (int)Convert.ToByte(value["top"]), (int)Convert.ToByte(value["width"]), (int)Convert.ToByte(value["height"]));
		}

		public static explicit operator JSON(Rect value)
		{
			JSON jSON = new JSON();
			jSON["left"] = value.xMin;
			jSON["top"] = value.yMax;
			jSON["width"] = value.width;
			jSON["height"] = value.height;
			return jSON;
		}

		public T[] ToArray<T>(string fieldName)
		{
			if (fields.ContainsKey(fieldName) && fields[fieldName] is IEnumerable)
			{
				List<T> list = new List<T>();
				foreach (object item in fields[fieldName] as IEnumerable)
				{
					if (list is List<string>)
					{
						(list as List<string>).Add(Convert.ToString(item));
					}
					else if (list is List<int>)
					{
						(list as List<int>).Add(Convert.ToInt32(item));
					}
					else if (list is List<float>)
					{
						(list as List<float>).Add(Convert.ToSingle(item));
					}
					else if (list is List<bool>)
					{
						(list as List<bool>).Add(Convert.ToBoolean(item));
					}
					else if (list is List<Vector2>)
					{
						(list as List<Vector2>).Add((JSON)item);
					}
					else if (list is List<Vector3>)
					{
						(list as List<Vector3>).Add((JSON)item);
					}
					else if (list is List<Rect>)
					{
						(list as List<Rect>).Add((JSON)item);
					}
					else if (list is List<Color>)
					{
						(list as List<Color>).Add((JSON)item);
					}
					else if (list is List<Color32>)
					{
						(list as List<Color32>).Add((JSON)item);
					}
					else if (list is List<Quaternion>)
					{
						(list as List<Quaternion>).Add((JSON)item);
					}
					else if (list is List<JSON>)
					{
						(list as List<JSON>).Add((JSON)item);
					}
				}
				return list.ToArray();
			}
			return new T[0];
		}
	}
}
