using UnityEngine;

public static class Log
{
	#region Error
	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void ErrorFormat(UnityEngine.Object context, string template, params object[] args)
	{
		var message = string.Format(template, args);
		Error(context, message);
	}

	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void ErrorFormat(string template, params object[] args)
	{
		var message = string.Format(template, args);
		Error(message);
	}

	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Error(object message)
	{
		Debug.LogError(string.Concat("<color=grey>[»]</color> ", message));
	}

	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Error(UnityEngine.Object context, object message)
	{
		Debug.LogError(string.Concat("<color=grey>[»]</color> ", message), context);
	}
	#endregion

	#region Warning
	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void WarningFormat(UnityEngine.Object context, string template, params object[] args)
	{
		var message = string.Format(template, args);
		Warning(context, message);
	}

	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void WarningFormat(string template, params object[] args)
	{
		var message = string.Format(template, args);
		Warning(message);
	}

	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Warning(object message)
	{
		Debug.LogWarning(string.Concat("<color=grey>[»]</color> ", message));
	}

	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Warning(UnityEngine.Object context, object message)
	{
		Debug.LogWarning(string.Concat("<color=grey>[»]</color> ", message), context);
	}
	#endregion

	#region Message
	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void InfoFormat(UnityEngine.Object context, string template, params object[] args)
	{
		var message = string.Format(template, args);
		Info(context, message);
	}

	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void InfoFormat(string template, params object[] args)
	{
		var message = string.Format(template, args);
		Info(message);
	}

	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Info(object message)
	{
		Debug.Log(string.Concat("<color=grey>[»]</color> ", message));
	}

	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Info(UnityEngine.Object context, object message)
	{
		Debug.Log(string.Concat("<color=grey>[»]</color> ", message), context);
	}
	#endregion

	#region Verbose
	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void VerboseFormat(UnityEngine.Object context, string template, params object[] args)
	{
		var message = string.Format(template, args);
		Verbose(context, message);
	}

	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void VerboseFormat(string template, params object[] args)
	{
		var message = string.Format(template, args);
		Verbose(message);
	}

	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Verbose(object message)
	{
		Debug.Log(string.Concat("<color=grey>[»]</color> ", message));
	}

	[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void Verbose(UnityEngine.Object context, object message)
	{
		Debug.Log(string.Concat("<color=grey>[»]</color> ", message), context);
	}
	#endregion
}