Shader "Hidden/FoveatedRendering" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
	}

	CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex, _frTex;
		float4 _MainTex_TexelSize;


		float _minX, _maxX, _minY, _maxY;

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

		SubShader{
			Cull Off
			ZTest Always
			ZWrite Off

			pass { // 0 Horizontal
				CGPROGRAM
					#pragma vertex VertexProgram
					#pragma fragment FragmentProgram

				half4 FragmentProgram(Interpolators i) : SV_Target {
					if (i.pos.x < _minX || i.pos.x > _maxX)
						return tex2D(_MainTex, i.uv) * half4(.5,0,0,0);
					return tex2D(_MainTex, i.uv);
				}
				ENDCG
			}

			pass {// 1 Vertical
			CGPROGRAM
					#pragma vertex VertexProgram
					#pragma fragment FragmentProgram

				half4 FragmentProgram(Interpolators i) : SV_Target {
					if (i.pos.y < _minY ||  i.pos.y > _maxY)
						return tex2D(_MainTex, i.uv) * half4(0,0,.5,0);
					return tex2D(_MainTex, i.uv);
				}
				ENDCG
			}

			pass
			{ // 2 combinePass horizontal
			CGPROGRAM
				#pragma vertex VertexProgram
				#pragma fragment FragmentProgram


				half4 FragmentProgram(Interpolators i) : SV_Target {

					half4 source = tex2D(_MainTex, i.uv);
					half4 fr = tex2D(_frTex, i.uv);

					if (i.pos.x < _minX + 30) {
						half frStrength = smoothstep( _minX * .5f, _minX, i.pos.x);
						half3 color = lerp(
							source.rgb, fr.rgb,
							frStrength + fr.a - frStrength * fr.a
						);
						return half4(color, source.a);
						//return fr;
					}
					else if (i.pos.x > _maxX - 30)
					{
						
						half frStrength = smoothstep(_maxX, _maxX + _minX * .5f, i.pos.x);
						half3 color = lerp(
							source.rgb, fr.rgb,
							frStrength + fr.a - frStrength * fr.a
						);
						
						return half4(color, source.a);
						//return fr;
					}
					else
					{
						return source;
					}

				}
			ENDCG
			}

			pass { // pass 3 combinePass vertical
				CGPROGRAM
				#pragma vertex VertexProgram
				#pragma fragment FragmentProgram

				half4 FragmentProgram(Interpolators i) : SV_Target {

					half4 source = tex2D(_MainTex, i.uv);
					half4 fr = tex2D(_frTex, i.uv);
					if (i.pos.y < _minY + 30)
					{
						half frStrength = smoothstep(_minY  * .5f, _minY, i.pos.y);
						half3 color = lerp(
							source.rgb, fr.rgb,
							frStrength + fr.a - frStrength * fr.a
						);
						return half4(color, source.a);
					}
					else if (i.pos.y > _maxY - 30 && i.pos.x < _minX + _minX * .25f && i.pos.x > _maxX - _minX * .25f)
					{
						half frStrength = smoothstep(_maxY, _maxY + _minY * .5f, i.pos.y);
						half3 color = lerp(
							source.rgb, fr.rgb,
							frStrength + fr.a - frStrength * fr.a
						);
						return half4(color, source.a);
					}
					else
					{
						return source;
					}
				}
				ENDCG
			}

			pass { // 4 Combine side Corners
				CGPROGRAM
				#pragma vertex VertexProgram
				#pragma fragment FragmentProgram
				half4 FragmentProgram(Interpolators i) : SV_Target{
					half4 source = tex2D(_MainTex, i.uv);
					half4 fr = tex2D(_frTex, i.uv);
					if (i.pos.x <= _minX + 30 && (i.pos.y <= _minY || i.pos.y >= _maxY))
					{
						half frStrength = smoothstep(_minX * .5f, _minX, i.pos.x);
						half3 color = lerp(
							source.rgb, fr.rgb,
							frStrength + fr.a - frStrength * fr.a
						);
						return half4(color, source.a);
					}
					else if (i.pos.x >= _maxX - 30 && (i.pos.y <= _minY || i.pos.y >= _maxY))
					{
						half frStrength = smoothstep(_maxX, _maxX + _minX * .5f, i.pos.x);
						half3 color = lerp(
							source.rgb, fr.rgb,
							frStrength + fr.a - frStrength * fr.a
						);
						return half4(color, source.a);
					}
					else
					{
						return source;
					}
				}
				ENDCG
			}

			pass { // 5 Combine top/bottom Corners
				CGPROGRAM
				#pragma vertex VertexProgram
				#pragma fragment FragmentProgram
				half4 FragmentProgram(Interpolators i) : SV_Target{
					half4 source = tex2D(_MainTex, i.uv);
					half4 fr = tex2D(_frTex, i.uv);
					if (i.pos.y <= _minY + 30 && (i.pos.x <= _minX || i.pos.x >= _maxX))
					{
						half frStrength = smoothstep(_minY  * .5f, _minY, i.pos.y);
						half3 color = lerp(
							source.rgb, fr.rgb,
							frStrength + fr.a - frStrength * fr.a
						);
						return half4(color, source.a);
					}
					else if (i.pos.y >= _maxY + 30 & (i.pos.x <= _minX || i.pos.x >= _maxX))
					{
						half frStrength = smoothstep(_maxY, _maxY + _minY * .5f, i.pos.y);
						half3 color = lerp(
							source.rgb, fr.rgb,
							frStrength + fr.a - frStrength * fr.a
						);
						return half4(color, source.a);
					}
					else {
						return source;
}
				}
				ENDCG
			}
		}
}