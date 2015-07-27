using UnityEngine;
using System.Collections;
using System;

public static class TransferFunctions
{
	public enum TransferFunction
	{
		None, 
		Sigmoid,
		Linear,
		Gaussian,
		RationalSigmoid,
		NumberOfTypes
	}

	public static TransferFunction GetRandomTransferFunction() {
		int numFunctions = (int)TransferFunction.NumberOfTypes;
		int randFunction = (int)UnityEngine.Random.Range (0f,numFunctions);
		//Random.Range(0f, numFunctions);
		TransferFunction returnFunction = (TransferFunction)randFunction;
		return returnFunction;
	}

	public static float Evaluate(TransferFunction tFunc, float input)
	{
		switch (tFunc)
		{
		case TransferFunction.Sigmoid:
			return sigmoid(input);
			
		case TransferFunction.Linear:
			return linear(input);
			
		case TransferFunction.Gaussian:
			return gaussian(input);
			
		case TransferFunction.RationalSigmoid:
			return rationalsigmoid(input);
			
		case TransferFunction.None:
		default:
			return 0.0f;
		}
		
	}
	public static float EvaluateDerivative(TransferFunction tFunc, float input)
	{
		switch (tFunc)
		{
		case TransferFunction.Sigmoid:
			return sigmoid_derivative(input);
			
		case TransferFunction.Linear:
			return linear_derivative(input);
			
		case TransferFunction.Gaussian:
			return gaussian_derivative(input);
			
		case TransferFunction.RationalSigmoid:
			return rationalsigmoid_derivative(input);
			
		case TransferFunction.None:
		default:
			return 0.0f;
		}
	}
	
	// Transfer Function Definitions 
	private static float sigmoid(float x)
	{
		return 1.0f / (1.0f + (float)Math.Exp(-x));
	}
	private static float sigmoid_derivative(float x)
	{
		return sigmoid(x) * (1f - sigmoid(x));
	}
	
	private static float linear(float x)
	{
		return x;
	}
	private static float linear_derivative(float x)
	{
		return 1.0f;
	}
	
	private static float gaussian(float x)
	{
		return (float)Math.Exp(-(float)Math.Pow(x, 2f));
	}
	private static float gaussian_derivative(float x)
	{
		return -2.0f * x * gaussian(x);
	}
	
	private static float rationalsigmoid(float x)
	{
		return x / (1.0f + (float)Math.Sqrt(1.0f + x * x));
	}
	private static float rationalsigmoid_derivative(float x)
	{
		float val = (float)Math.Sqrt(1.0f + x * x);
		return 1.0f / (val * (1f + val));
	}
}
