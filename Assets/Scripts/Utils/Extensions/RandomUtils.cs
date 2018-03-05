using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUtils {

	public static bool Boolean()
	{
		return Random.Range(0, 2) == 1;
	}

	public static int Sign()
	{
		return Boolean() ? 1 : -1;
	}
}
