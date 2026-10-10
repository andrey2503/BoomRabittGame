// Post-proceso minimo de una sola pasada: brillo, contraste, saturacion y vineta.
//
// El proyecto no tiene el paquete de Post Processing, asi que esto lo suple con
// lo esencial. Deliberadamente NO hace bloom: eso exigiria varias pasadas y
// render targets temporales, con mucho mas margen de error.
//
// Vive en Resources/ para que quede incluido en las builds: un shader que solo se
// referencia por codigo no se empaqueta a menos que este en Resources o en la
// lista "Always Included Shaders".
Shader "Hidden/BoomRabitt/ColorGrade"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Brillo ("Brillo", Float) = 1.0
		_Contraste ("Contraste", Float) = 1.06
		_Saturacion ("Saturacion", Float) = 1.12
		_Vineta ("Vineta", Float) = 0.22
		_VinetaSuavidad ("Suavidad vineta", Float) = 0.65
	}

	SubShader
	{
		Cull Off
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Brillo;
			float _Contraste;
			float _Saturacion;
			float _Vineta;
			float _VinetaSuavidad;

			fixed4 frag (v2f_img i) : SV_Target
			{
				fixed4 col = tex2D (_MainTex, i.uv);

				col.rgb *= _Brillo;

				// contraste alrededor del gris medio
				col.rgb = (col.rgb - 0.5) * _Contraste + 0.5;

				// saturacion respecto a la luminancia
				float gris = dot (col.rgb, float3 (0.299, 0.587, 0.114));
				col.rgb = lerp (gris.xxx, col.rgb, _Saturacion);

				// vineta radial: oscurece las esquinas y concentra la atencion
				float2 d = i.uv - 0.5;
				float dist = length (d) * 1.41421356;
				float borde = smoothstep (1.0, 1.0 - _VinetaSuavidad, dist);
				col.rgb *= lerp (1.0 - _Vineta, 1.0, borde);

				col.rgb = saturate (col.rgb);
				return col;
			}
			ENDCG
		}
	}

	Fallback Off
}
