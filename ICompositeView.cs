using System;
using System.Collections.Generic;

public interface ICompositeView<T>
{
	List<T> GetChildren();

	void AddChild(T item);

	void RemoveChild(T item);

	void Clear();
}
