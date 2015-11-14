Shader "Example/Diffuse ShadowColor" 
{
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_MainTex ("Texture", 2D) = "white" { }
		_ShadowColor ("ShadowColor", Color) = (0,0,1,1) //UI追加
	}

	SubShader {
		Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }

		Cull off
		CGPROGRAM
		//#pragma surface surf Lambert //Lambert を　SimpleLambert　等に変更すると　half4 LightingSimpleLambert　以下が記述できる
		#pragma surface surf SimpleLambert
		half4 _ShadowColor; //とりあえず書く　
		half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
		half NdotL = dot (s.Normal, lightDir);
		half4 c;
		//変更前 ↓
		//c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
		//変更後 ↓
		c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * 2);
		c.rgb *= NdotL * min((atten + _ShadowColor.rgb), 1); //atten が影の色
		//c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * min((atten + _ShadowColor.rgb), 1) * 2); //ReceiveShadow無し
		c.a = s.Alpha;
		return c;
	}

	struct Input {
		float2 uv_MainTex;
	};


	sampler2D _MainTex;
	fixed4 _Color;
	void surf (Input IN, inout SurfaceOutput o) {
		o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
		
	}
	ENDCG
	}
	Fallback "Diffuse"
}

