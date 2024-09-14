Shader "VTS/DigitalClockDay"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_EmissionColor ("Emission", Color) = (0,0,0,1)
		_TexChars ("Characters", 2D) = "white" {}
		_TexWP ("WebPanelRender", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		[Enum(Off, 0, On, 1)] _JP ("Japanese", Float) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		ZWrite Off
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:fade
		#pragma target 3.0

		struct Input
		{
			float2 uv_TexChars;
		};

		sampler2D _TexChars, _TexWP;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _EmissionColor;
		uint _JP;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			const float2 uv = IN.uv_TexChars;

			const float2 x1 = {1.0/8, 0};
			const float2 y1 = {0, 1.0/8};

			const int3   sec0 = round(tex2D(_TexWP, x1*3.5+y1*0.5).rgb),   sec1 = round(tex2D(_TexWP, x1*2.5+y1*0.5).rgb);
			const int3   min0 = round(tex2D(_TexWP, x1*5.5+y1*0.5).rgb),   min1 = round(tex2D(_TexWP, x1*4.5+y1*0.5).rgb);
			const int3  hour0 = round(tex2D(_TexWP, x1*7.5+y1*0.5).rgb),  hour1 = round(tex2D(_TexWP, x1*6.5+y1*0.5).rgb);
			const int3   day0 = round(tex2D(_TexWP, x1*0.5+y1*1.5).rgb);

			const int sec    =  sec0.r +  sec0.g*2 +  sec0.b*4 +  sec1.r*8 +  sec1.g*16 +  sec1.b*32 + _Time.y;
			const int minute =  min0.r +  min0.g*2 +  min0.b*4 +  min1.r*8 +  min1.g*16 +  min1.b*32 + sec/60.0;
			const int hour   = hour0.r + hour0.g*2 + hour0.b*4 + hour1.r*8 + hour1.g*16 + hour1.b*32 + minute/60.0;
			const int day    =  day0.r +  day0.g*2 +  day0.b*4 + hour/24.0;

			float2 uv0 = {5/8.0 + (uv.x + (_JP?1:0))/16, (uv.y+6-floor(fmod(day + 0.1, 7.0)))/14};
			fixed4 c = tex2Dgrad(_TexChars, uv0, ddx(uv) / float2(16.0, 14.0), ddy(uv) / float2(16.0, 14.0));
			o.Albedo = c.rgb;
			o.Emission = (_EmissionColor*((c.r + c.g + c.b)/3));
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Transparent/Diffuse"
}
