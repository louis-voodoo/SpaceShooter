using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventExtension {

	public static void Raise(this Action eEvent)
	{
		if (eEvent != null)
		{
			eEvent();
		}
	}

	public static void RaiseEvent<T>(this Action<T> eEvent, T parameter)
	{
		if (eEvent != null)
		{
			eEvent(parameter);
		}
	}
}
