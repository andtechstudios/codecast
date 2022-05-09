using NaughtyAttributes;
using UnityEngine;

public class InteractiveSandbox : MonoBehaviour
{

	[TextArea(3, 999)]
	public string message;

	[Button]
	public void SendRaw()
	{
		Codecast.WriteLine(message);
	}

	[Button]
	public void Send()
	{
		Codecast.WriteLine(JsonUtility.ToJson(Andtech.Codecast.UnityLogEntry.Log(message)));
	}

	[Button]
	public void SendWarning()
	{
		Codecast.WriteLine(JsonUtility.ToJson(Andtech.Codecast.UnityLogEntry.Warning(message)));
	}

	[Button]
	public void SendError()
	{
		Codecast.WriteLine(JsonUtility.ToJson(Andtech.Codecast.UnityLogEntry.Error(message)));
	}

	[Button]
	public void Clear()
	{
		Codecast.Clear();
	}

	[Button]
	public void Print()
	{
		Debug.Log("Hello world");
	}
}
