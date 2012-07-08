﻿using System.Collections;
using Microsoft.Build.Framework;

public class StubBuildEngine : IBuildEngine
{
	public void LogErrorEvent(BuildErrorEventArgs e)
	{
	}

	public void LogWarningEvent(BuildWarningEventArgs e)
	{
	}

	public void LogMessageEvent(BuildMessageEventArgs e)
	{
	}

	public void LogCustomEvent(CustomBuildEventArgs e)
	{
	}

	public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
	{
		return true;
	}

	public bool ContinueOnError
	{
		get { return false; }
	}

	public int LineNumberOfTaskNode
	{
		get { return 0; }
	}

	public int ColumnNumberOfTaskNode
	{
		get { return 0; }
	}

	public string ProjectFileOfTaskNode { get; set; }

}