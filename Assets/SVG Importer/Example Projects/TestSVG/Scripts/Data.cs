using System.Collections.Generic;
using System;

public class Response<T> 
{
    public List<T> list;
}

[Serializable]
public class FillDataItem 
{
	public int n;
	public string c;
	public List<int> p;
	public List<int> t;
}