Shader "Custom/shad_UIFitnessGraphAgentsLastGen"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "black" {}
		_FitnessTex ("Fitness Texture", 2D) = "black" {}
		_ZoomFactorX ("ZoomFactorX", Range(0.01,2)) = 1.0
		_ZoomFactorY ("ZoomFactorY", Range(0.01,2)) = 1.0
		_WarpFactorX ("ZoomFactorX", Range(0.10,4)) = 1.0
		_WarpFactorY ("ZoomFactorY", Range(0.10,4)) = 1.0	
		_NumAgents ("NumAgents", Int) = 10	
		_GenAvgScore ("GenAvgScore", Float) = 0.5
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			uniform fixed4 _Color;
			uniform fixed _ZoomFactorX;
			uniform fixed _ZoomFactorY;
			uniform fixed _WarpFactorX;
			uniform fixed _WarpFactorY;
			uniform fixed _NumAgents;
			uniform fixed _GenAvgScore;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _FitnessTex;

			fixed4 frag(v2f IN) : SV_Target
			{
			    // TEMPORARY!!!!!
				//fixed tempZoomX = 0.5;  // Replace with _ZoomFactorX !!
				//fixed tempZoomY = 0.2;  // Replace with _ZoomFactorY !!!
				
				
				float lineWidth = 0.005 * _ZoomFactorY;	
				float gridLineWidthX = 0.002 * _ZoomFactorX;	
				float gridLineWidthY = 0.002 * _ZoomFactorY;	
				float gridDivisionsX = _NumAgents;
				float gridDivisionsY = 10;	
				float gridLineSpacingX = 1 / gridDivisionsX;
				float gridLineSpacingY = 1 / gridDivisionsY;
				
				half2 finalCoords = IN.texcoord;
				float initialScore = tex2D(_FitnessTex, 0).x;
				//float latestScore = tex2D(_FitnessTex, 1).x;
				float pivotY = _GenAvgScore;
				if((pivotY + (_ZoomFactorY * 0.5)) > 1) {
					pivotY = 1 - (_ZoomFactorY * 0.5);
				}
				if((pivotY - (_ZoomFactorY * 0.5)) < 0) {
					pivotY = (_ZoomFactorY * 0.5);
				}
				finalCoords.x = (finalCoords.x * _ZoomFactorX) + (1 - _ZoomFactorX);
				finalCoords.y = (finalCoords.y * _ZoomFactorY) + (pivotY - (_ZoomFactorY * 0.5));
				//latestScore = (latestScore * _ZoomFactorY) + (latestScore - (_ZoomFactorY * 0.5));
				// color.x is the Raw Fitness VALUE (0-1), baked into red channel
				// color.y is the weighted Fitness VALUE (0-1), baked into green channel
				
				
				
				
				
				
				
				float agentIndex = floor(finalCoords.x * _NumAgents);
				float columnWidth = 1.0 / _NumAgents;
				float samplePos = (columnWidth / 2.0) + (columnWidth * agentIndex);
				
				half4 color = tex2D(_FitnessTex, samplePos) * IN.color;
				clip (color.a - 0.01);
				half4 bgColor = half4(0.21, 0.21, 0.21, 1.0);
				half4 onColor = half4(0.24, 0.33, 0.26, 1.0);
				half4 gridColor = half4(0.14, 0.14, 0.14, 1.0);
				half4 rawColor = half4(0.24, 0.33, 0.26, 1.0);
				half4 weightedColor = half4(0.31, 0.31, 0.44, 1.0);
				
				half4 pixColor = bgColor;
				
				float distRaw = abs(finalCoords.y - color.x);
				float distWeighted = abs(finalCoords.y - color.y);
				
				// Y-Axis:
				if(finalCoords.y < color.x) {  // if vertical position is less than fitnessRawScore:
					pixColor = rawColor;
				}
				if(finalCoords.y < color.y) {  // if vertical position is less than fitnessRawScore:
					pixColor = weightedColor;
				}
				
				
				
				//if(finalCoords.y > initialScore) {
				//	if(finalCoords.y < latestScore) {	
				//		pixColor = onColor;
				//	}
				//}
				
				// Create GRIDLINES:
				for(int i = 0; i < gridDivisionsX; i++) {
					float linePos = i * gridLineSpacingX;
					float gridDistX = abs(finalCoords.x - linePos);
					if(gridDistX < gridLineWidthX) {
						pixColor = gridColor;
					}
				}
				for(int i = 0; i < gridDivisionsY; i++) {
				float linePos = i * gridLineSpacingY;
					float gridDistY = abs(finalCoords.y - linePos);
					if(gridDistY < gridLineWidthY) {
						pixColor = gridColor;
					}
				}
				
				if(distWeighted < lineWidth) {
					pixColor = weightedColor;
				}
				if(distRaw < lineWidth) {
					pixColor = half4(1.0, 1.0, 1.0, 1.0);
				}
				float distZero = abs(finalCoords.y - 0);
				if(distZero < lineWidth) {
					pixColor = half4(0.8, 0.1, 0.1, 1.0);
				}
				float distOne = abs(finalCoords.y - 1.0);
				if(distOne < lineWidth) {
					pixColor = half4(0.1, 0.8, 0.1, 1.0);
				}
				float distAvg = abs(finalCoords.y - _GenAvgScore);	
				if(distAvg < lineWidth) {
					pixColor = half4(0.1, 0.1, 0.8, 1.0);
				}
							
				//float distLastScore = abs(finalCoords.y - latestScore);
				//if(distLastScore < lineWidth) {
				//	pixColor = half4(0.1, 0.1, 0.8, 1.0);
				//}
				
				//return float4(0.5, 0.5, 0.5, 1.0);
				return pixColor;
			}
		ENDCG
		}
	}
}
