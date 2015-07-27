using UnityEngine;
using System.Collections;

public class TestGeneric<T> {

	public T refValue;

	public TestGeneric(T inValue) {
		refValue = inValue;
	}
}
