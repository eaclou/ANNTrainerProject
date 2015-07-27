Shader "Custom/shad_UICurAgentGraphBrainDiagram"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "black" {}
		_DataTex ("Data Texture", 2D) = "black" {}
		_CurDataTex ("Current Data Texture", 2D) = "black" {}
		_ZoomFactorX ("ZoomFactorX", Range(0.01,2)) = 1.0
		_ZoomFactorY ("ZoomFactorY", Range(0.01,2)) = 1.0
		_ValueMultiplier ("ValueMultiplier", Float) = 1.0	
		_TotalNodes ("TotalNodes", Float) = 2.0	
		_TotalBiases ("TotalBiases", Float) = 2.0	
		_TotalWeights ("TotalWeights", Float) = 2.0		
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
			#pragma target 4.0
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
			uniform fixed _ValueMultiplier;
			uniform fixed _TotalNodes;
			uniform fixed _TotalBiases;
			uniform fixed _TotalWeights;
			uniform sampler2D _MainTex;
			uniform sampler2D _DataTex;
			uniform sampler2D _CurDataTex;
			uniform float4 _DataTex_ST; 
			
			float distanceSquared(half2 v, half2 w) {
				float distSquared = (w.x - v.x)*(w.x - v.x) + (w.y - v.y)*(w.y - v.y);
				return distSquared;
			}
			
			float distanceToLine(half2 v, half2 w, half2 p) {
				float distToLine = 0;
				const float l2 = distanceSquared(v, w);
				if(l2 == 0.0) {  // v == w case
					return distance(p, v);
				}
				// Consider the line extending the segment, parameterized as v + t (w - v).
  				// We find projection of point p onto the line. 
  				// It falls where t = [(p-v) . (w-v)] / |w-v|^2
  				const float t = dot(p - v, w - v) / l2;
  				if (t < 0.0) return distance(p, v);       // Beyond the 'v' end of the segment
  				else if (t > 1.0) return distance(p, w);  // Beyond the 'w' end of the segment
  				const half2 projection = v + t * (w - v);  // Projection falls on the segment
  				return distance(p, projection);
			}
			
			void drawDisk(half2 pos, half2 center, float radius, half4 col, inout half4 pix) {
				if( length(pos-center) < radius) {
					pix = col;
				}
			}
			
			void drawLine(half2 pos, half2 start, half2 end, float thickness, half4 col, inout half4 pix) {
				if(distanceToLine(start, end, pos) < thickness) {
					pix = col;
				}
			}
			
			

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				//OUT.texcoord = IN.texcoord;
				OUT.texcoord = TRANSFORM_TEX( IN.texcoord, _DataTex );
#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				OUT.color = IN.color * _Color;
				return OUT;
			}

			

			fixed4 frag(v2f IN) : SV_Target
			{		    
				
				half2 finalCoords = IN.texcoord;
				float pivotY = 0.5;
				if((pivotY + (_ZoomFactorY * 0.5)) > 1) {
					pivotY = 1 - (_ZoomFactorY * 0.5);
				}
				if((pivotY - (_ZoomFactorY * 0.5)) < 0) {
					pivotY = (_ZoomFactorY * 0.5);
				}
				finalCoords.x = (finalCoords.x * _ZoomFactorX) + (1 - _ZoomFactorX); // hugs right-side of graph
				finalCoords.y = (finalCoords.y * _ZoomFactorY) + (pivotY - (_ZoomFactorY * 0.5));
				
					
				
				
				half4 sampleValues = tex2D(_DataTex, finalCoords);
				half4 currentBrainValues = tex2D(_CurDataTex, finalCoords);
				half4 negHue = half4(1.0, 0.75, 0.9, 1.0);
				half4 posHue = half4(0.75, 1.0, 0.9, 1.0);
				float brightness = ((sampleValues.r - 0.5) * _ValueMultiplier) + 0.5;
				float curValueBrightness = ((currentBrainValues.r - 0.5) * 1) + 0.5;
				//half4 brightness = half4(sampleValues.r, sampleValues.r, sampleValues.r, 1.0);
				half4 pixColor = half4(brightness, brightness, brightness, 1.0);
				half4 pixColorTick = half4(curValueBrightness, curValueBrightness, curValueBrightness, 1.0);
				
				half4 finalColor;
				if(brightness > 0.51) {
					pixColor = posHue * pixColor;
				}
				if(brightness < 0.49) {
					pixColor = negHue * pixColor;
				}
				if(curValueBrightness > 0.51) {
					pixColorTick = posHue * pixColorTick;
				}
				if(curValueBrightness < 0.49) {
					pixColorTick = negHue * pixColorTick;
				}
				
				if(finalCoords.y < 0.5) {
					finalColor = pixColor * 0.2;
				}
				else {
					finalColor = pixColorTick * 0.2;
				}
				
				if(_TotalNodes > 2.0) {
				
					//float layerFraction = tex2D(_CurDataTex, half2(1.0/(_TotalNodes*2), 0.0)).g;	// get number of total Layers
					float layerFraction = 0.5;	// get number of total Layers
					
					float weightsPlusBiases = _TotalBiases + _TotalWeights;
					float prevLayerNodeFraction = 0.5;
					// Iterate over layers:
					float nodeIndex = 0;
					float biasIndex = 0;
					float weightIndex = 0;
					for(float i = 0.0; i < 1.00; i+=layerFraction) {  // iterate over all the layers
						//float nodeFraction = tex2D(_CurDataTex, half2(nodeIndex/_TotalNodes + 1.0/(_TotalNodes*2), 0.5)).b; // number of nodes in this layer
						float nodeFraction = 0.5; // number of nodes in this layer
						for(float j = 0.0; j < 1.00; j += nodeFraction) {  // iterate over all Nodes for this layer:
							
							//half4 texValue = tex2D(_CurDataTex, half2(nodeIndex/_TotalNodes + 1.0/(_TotalNodes*2), 0.5));  // grab information for this node
							//float posX = texValue.b - (nodeFraction)*0.5;
							//float posY = texValue.g - (layerFraction)*0.5;
							
							if(i > 0.0) {  // NOT INPUT LAYER
								// WEIGHT LINES:
								
								for(float x = 0.0; x < 60; x++) {
									//float weightSamplePosX = _TotalBiases/weightsPlusBiases + (weightIndex/_TotalWeights)/weightsPlusBiases + 1.0/(weightsPlusBiases*2);
									//half4 weightValue = tex2D(_DataTex, half2(weightSamplePosX, 0.5));
									//drawLine(finalCoords, half2(posX, posY), half2(x/6 + prevLayerNodeFraction*0.5, i - layerFraction*0.5), 0.005, half4(weightValue.r, weightValue.r, weightValue.r, 1.0), finalColor);
									//weightIndex++;
									//if(weightIndex > 30) {
									//	break;
									//}
								}
								//for(float k = 0.0; k < 1.00; k+=prevLayerNodeFraction) { // For all nodes in the previous layer, connect line from this node j									
								//	drawLine(finalCoords, half2(posX, posY), half2(k + prevLayerNodeFraction*0.5, i - layerFraction*0.5), 0.005, half4(weightValue.r, weightValue.r, weightValue.r, 1.0), finalColor);
								//	weightIndex++;								
								//}
								
								// BIAS:
								//half4 biasValue = tex2D(_DataTex, half2((biasIndex/_TotalBiases)/weightsPlusBiases + 1.0/(weightsPlusBiases*2), 0.5));  // grab information for this node
								//drawDisk(finalCoords, half2(posX, posY), 0.065, half4(biasValue.r, biasValue.r, biasValue.r, 1.0), finalColor);	
								//biasIndex++;						
							}
							
							// value:
							//drawDisk(finalCoords, half2(posX, posY), 0.05, half4(texValue.r, texValue.r, texValue.r, 1.0), finalColor);
							
							nodeIndex++;
						}
						prevLayerNodeFraction = nodeFraction;
					}
				}
				
				
				
				
				//return float4(0.5, 0.5, 0.5, 1.0);
				return finalColor;
			}
		ENDCG
		}
	}
}
