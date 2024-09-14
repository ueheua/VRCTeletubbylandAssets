Shader "VTS/DigitalClock"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_EmissionColor ("Emission", Color) = (0,0,0,1)
		_TexChars ("Characters", 2D) = "white" {}
		_TexWP ("WebPanelRender", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		[Enum(Off, 0, On, 1)] _ColonBlink ("ColonBlink", Float) = 0.0
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
		uint _ColonBlink;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			const float2 uv = IN.uv_TexChars;
			const float2 uvchar = {frac(uv.x*8), uv.y};

			const float2 x1 = {1.0/8, 0};
			const float2 y1 = {0, 1.0/8};

			const int hmsWhich = uv.x/(1.0/8*3);

			const int3   sec0 = round(tex2D(_TexWP, x1*3.5+y1*0.5).rgb),   sec1 = round(tex2D(_TexWP, x1*2.5+y1*0.5).rgb);
			const int3   min0 = round(tex2D(_TexWP, x1*5.5+y1*0.5).rgb),   min1 = round(tex2D(_TexWP, x1*4.5+y1*0.5).rgb);
			const int3  hour0 = round(tex2D(_TexWP, x1*7.5+y1*0.5).rgb),  hour1 = round(tex2D(_TexWP, x1*6.5+y1*0.5).rgb);

			const float secf    =  sec0.r +  sec0.g*2 +  sec0.b*4 +  sec1.r*8 +  sec1.g*16 +  sec1.b*32 + _Time.y;
			const float minutef =  min0.r +  min0.g*2 +  min0.b*4 +  min1.r*8 +  min1.g*16 +  min1.b*32 + secf/60.0;
			const float hourf   = hour0.r + hour0.g*2 + hour0.b*4 + hour1.r*8 + hour1.g*16 + hour1.b*32 + minutef/60.0;

			const int sec    = fmod(secf, 60.0);
			const int minute = fmod(minutef, 60.0);
			const int hour   = fmod(hourf, 24.0);

			const int hms = (hmsWhich == 0 ? hour : (hmsWhich == 1 ? minute : sec));
			const int hmsd = ((uint)fmod(uv.x*8, 3.0)) == 0 ? (hms + 0.1)/10.0 : fmod(hms + 0.1, 10.0);

			float2 uv0 = {(floor(fmod(hmsd + 0.1, 8.0))+uvchar.x)/8, 1-(floor((hmsd + 0.1)/8.0)+1-uvchar.y)/2};
			const float2 uvcolon = {uvchar.x/8-(5.0+floor(fmod(_Time.y, 2.0))*_ColonBlink)/8, uvchar.y/2};
			uv0 = ((uint)fmod(uv.x*8, 3.0)) == 2 ? uvcolon : uv0;

			fixed4 c = tex2Dgrad(_TexChars, uv0, ddx(uv) / float2(1.0, 2.0), ddy(uv) / float2(1.0, 2.0));
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
