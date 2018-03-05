using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>  {

	private SaveDictionnary data = new SaveDictionnary();
	const string SAVE_KEY = "_SAVE_";

	public void Set<T>(string key, T value){
		setKey (key, value.ToString ());
	}

	public T Get<T>(string key, T defaultValue = default(T)){
		if (data.ContainsKey (key))
			return (T)(System.Convert.ChangeType (getKey (key), typeof(T)));
		else
			return defaultValue;
	}

	void setKey (string key, string value){
		data [key] = value;
		Save ();
	}

	string getKey (string key)
	{
		return data[key].ToString();
	}

	protected override void Init() {
		RetreiveSaves ();
	}

	void RetreiveSaves (){
		string save = PlayerPrefs.GetString (Application.productName + SAVE_KEY, "{}");
		data = JsonUtility.FromJson<SaveDictionnary> (save);
	}

	void Save (){
		PlayerPrefs.SetString (Application.productName + SAVE_KEY, JsonUtility.ToJson(data));
		foreach (KeyValuePair<string,string> toto in data) {
		}
	}
}

[System.Serializable]
public class SaveDictionnary : SerializableDictionary<string, string>{

}

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<TKey> keys = new List<TKey>();

	[SerializeField]
	private List<TValue> values = new List<TValue>();

	// save the dictionary to lists
	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();
		foreach(KeyValuePair<TKey, TValue> pair in this)
		{
			keys.Add(pair.Key);
			values.Add(pair.Value);
		}
	}

	// load dictionary from lists
	public void OnAfterDeserialize()
	{
		this.Clear();

		if(keys.Count != values.Count)
			throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

		for(int i = 0; i < keys.Count; i++)
			this.Add(keys[i], values[i]);
	}
}