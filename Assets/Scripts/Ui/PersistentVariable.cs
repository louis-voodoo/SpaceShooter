using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentVariable<T> {

	bool _init = false;
	T _value;
	string _key;
	T _defaultValue;

	public PersistentVariable(string playerPrefKey, T defaultValue) {
		_init = false;
		_key = playerPrefKey;
		_defaultValue = defaultValue;
	}
	
	public T Value {
		get {
			if (_init == false) {
				_value = SaveManager.Instance.Get<T>(_key, _defaultValue);
				_init = true;
			}
			return (_value);
		}
		set {
			_value = value;
			SaveManager.Instance.Set(_key, _value);
		}
	}
}
