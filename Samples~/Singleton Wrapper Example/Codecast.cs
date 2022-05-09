using UnityEngine;


public static class Codecast
{
	
	public static void WriteLine(object message)
	{
		#if UNITY_EDITOR
		Andtech.Codecast.Editor.Codecast.WriteLine(message);
		#endif
	}

	public static void Write(object message)
	{
#if UNITY_EDITOR
		Andtech.Codecast.Editor.Codecast.Write(message);
#endif
	}

	public static void Clear()
	{
#if UNITY_EDITOR
		Andtech.Codecast.Editor.Codecast.Clear();
#endif
	}
}
