Shader "Hidden/LowFoveatedRendering" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
	}

		CGINCLUDE
#include "UnityCG.cginc"

		sampler2D _MainTex, _frTex;
	float4 _MainTex_TexelSize;


	float _minX_1, _minX_2, _maxX_1, _maxX_2, _minY_1, _minY_2, _maxY_1, _maxY_2;

	struct VertexData {
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct Interpolators {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	Interpolators VertexProgram(VertexData v) {
		Interpolators i;
		i.pos = UnityObjectToClipPos(v.vertex);
		i.uv = v.uv;
		return i;
	}
	ENDCG

		SubShader
		{
			Cull Off
			ZTest Always
			ZWrite Off

		
		Pass { // 0 Horizontal Pass Low 1/4 
			CGPROGRAM
				#pragma vertex VertexProgram
				#pragma fragment FragmentProgram

				half4 FragmentProgram(Interpolators i) : SV_Target
				{
					half4 source = tex2D(_MainTex, i.uv);
					half4 fr = tex2D(_frTex, i.uv);

					if (i.pos.x <= _minX_2 * 2 && i.pos.y > _maxY_1)
					{
						half frStrength = smoothstep(_minX_2 * 1.5f, _minX_2 * 2, i.pos.x);
						half3 color = lerp(source.rgb, fr.rgb, frStrength + fr.a - frStrength * fr.a);
						return half4(color, source.a);
					}

					else if (i.pos.x >= _maxX_1 - _minX_2 && i.pos.y > _maxY_2)
					{
						half frStrength = smoothstep(_maxX_1 - _minX_2 * .5f, _maxX_1 - _minX_2, i.pos.x);
						half3 color = lerp(source.rgb, fr.rgb, frStrength + fr.a - frStrength * fr.a);
						return half4(color, source.a);
					}

					else if (i.pos.x <= _minX_2 && i.pos.y < _minY_2)
					{
						half frStrength = smoothstep(_minX_2 * .5f, _minX_2, i.pos.x);
						half3 color = lerp(source.rgb, fr.rgb, frStrength + fr.a - frStrength * fr.a);
						return half4(color, source.a);
					}

					else if (i.pos.x >= _maxX_1 && i.pos.y < _minY_2)
					{
						half frStrength = smoothstep(_maxX_1 + _minX_2 * .5f, _maxX_1, i.pos.x);
						half3 color = lerp(source.rgb, fr.rgb, frStrength + fr.a - frStrength * fr.a);
						return half4(color, source.a);
					}

					else
					{
						return source;
					}
				}
			ENDCG
			}

		
			Pass { // 1 Vertical Pass Low 1/4
				CGPROGRAM
				#pragma vertex VertexProgram
				#pragma fragment FragmentProgram

				half4 FragmentProgram(Interpolators i) : SV_Target
				{
					half4 source = tex2D(_MainTex, i.uv);
					half4 fr = tex2D(_frTex, i.uv);
					if (i.pos.y >= _maxY_1 && (i.pos.x <= _minX_2 * 2 || i.pos.x >= _maxX_1 - _minX_2))
					{
						half frStrength = smoothstep(_maxY_1 + _minY_2 * .5f, _maxY_1, i.pos.y);
						half3 color = lerp(source.rgb, fr.rgb, frStrength + fr.a - frStrength * fr.a);
						return half4(color, source.a);
					}
					else if (i.pos.y <= _minY_2 && (i.pos.x <= _minX_2 || i.pos.x >= _maxX_1))
					{
						half frStrength = smoothstep(_minY_2 * .5f, _minY_2, i.pos.y);
						half3 color = lerp(source.rgb, fr.rgb, frStrength + fr.a - frStrength * fr.a);
						return half4(color, source.a);
					}
					else
					{
						return source;
					}
				}
				ENDCG
			}
		
			
			Pass { // 2 Horizontal Pass Low 1/2
				CGPROGRAM
				#pragma vertex VertexProgram
				#pragma fragment FragmentProgram

				half4 FragmentProgram(Interpolators i) : SV_Target
				{
					half4 source = tex2D(_MainTex, i.uv);
					half4 fr = tex2D(_frTex, i.uv);

					if (i.pos.x <= _minX_2 && (i.pos.y <= _maxY_1 && i.pos.y >= _minY_2))
					{
						half frStrength = smoothstep(_minX_2 * .5f, _minX_2, i.pos.x);
						half3 color = lerp(source.rgb, fr.rgb, frStrength + fr.a - frStrength * fr.a);
						return half4(color, source.a);
					}
					else if (i.pos.x >= _maxX_1 && (i.pos.y <= _maxY_1 && i.pos.y >= _minY_2))
					{
						half frStrength = smoothstep(_maxX_1 + _minX_2 * .5f, _maxX_1, i.pos.x);
						half3 color = lerp(source.rgb, fr.rgb, frStrength + fr.a - frStrength * fr.a);
						return half4(color, source.a);
					}
					else
					{
						return source;
					}
				}
				ENDCG
			}

			
			Pass { // 3 Vertical Pass Low 1/2
				CGPROGRAM
				#pragma vertex VertexProgram
				#pragma fragment FragmentProgram

				half4 FragmentProgram(Interpolators i) : SV_Target
				{
					half4 source = tex2D(_MainTex, i.uv);
					half4 fr = tex2D(_frTex, i.uv);
					if (i.pos.y >= _maxY_1 && (i.pos.x >= _minX_2 && i.pos.x <= _maxX_1))
					{
						half frStrength = smoothstep(_maxX_1 + _minX_2 * .5f, _maxX_1, i.pos.y);
						half3 color = lerp(source.rgb, fr.rgb, frStrength + fr.a - frStrength * fr.a);
						return half4(color, source.a);
					}
					else if (i.pos.y <= _minY_2 && ((i.pos.x >= _minX_2 && i.pos.x <= _minX_2 * 2) || (i.pos.x >= _maxX_1 - _minX_2 || i.pos.x <= _maxX_1)))
					{
						half frStrength = smoothstep(_minY_2 * .5f, _minY_2, i.pos.y);
						half3 color = lerp(source.rgb, fr.rgb, frStrength + fr.a - frStrength * fr.a);
						return half4(color, source.a);
					}
					else
					{
						return source;
					}
				}
				ENDCG
			}

		}