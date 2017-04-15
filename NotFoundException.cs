using System;

public class NotFoundException : Exception
{
	public string errInfo = "";

	public NotFoundException(string msg)
	{
		this.errInfo = msg;
	}
}
