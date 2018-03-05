using UnityEngine;

public interface IRange<T> where T : struct
{
	T GetRandomValue (bool minInclusive = true, bool maxInclusive = true);	

	T GetMagnitude (bool inclusive = true);

	T Lerp(float ratio);

	float GetRatio(T value);

	T GetValueFromOtherRange (IRange<T> otherRange, T otherValue);
} 

[System.Serializable]
public struct IntRange : IRange<int>{

	public int min;
	public int max;

	public int GetRandomValue(bool minInclusive = true, bool maxInclusive = true)
	{
		return Random.Range (min + (minInclusive ? 0 : 1), max + (maxInclusive ? 1 : 0));
	}

	public int GetMagnitude (bool inclusive = true)
	{
		return Mathf.Max(0, (max - min) - (inclusive ? 0 : 2)); 
	}

	public IntRange(int min, int max)
	{
		this.min = min;
		this.max = max;
	}

	public int Lerp (float ratio){
		return (int)Mathf.Lerp (min, max, ratio);
	}

	public float GetRatio(int value){
		return (value - min) / (float)(max - min);
	}

	public int GetValueFromOtherRange (IRange<int> otherRange, int otherValue )
	{
		return Lerp(otherRange.GetRatio(otherValue));
	}

	public int GetValueFromOtherRange (IRange<float> otherRange, float otherValue )
	{
		return Lerp(otherRange.GetRatio(otherValue));
	}
}

[System.Serializable]
public struct FloatRange : IRange<float>{

	public float min;
	public float max;

	public float GetRandomValue(bool minInclusive = true, bool maxInclusive = true)
	{
		return Random.Range (min + (minInclusive ? 0 : 1), max + (maxInclusive ? 0 : -1));
	}

	public float GetMagnitude (bool inclusive = true)
	{
		return Mathf.Max(0, (max - min) - (inclusive ? 0 : 2)); 
	}

	public FloatRange(float min, float max)
	{
		this.min = min;
		this.max = max;
	}

	public float Lerp (float ratio){
		return Mathf.Lerp (min, max, ratio);
	}

	public float GetRatio(float value){
		return (value - min) / (float)(max - min);
	}

	public float GetValueFromOtherRange (IRange<int> otherRange, int otherValue )
	{
		return Lerp(otherRange.GetRatio(otherValue));
	}

	public float GetValueFromOtherRange (IRange<float> otherRange, float otherValue )
	{
		return Lerp(otherRange.GetRatio(otherValue));
	}
}