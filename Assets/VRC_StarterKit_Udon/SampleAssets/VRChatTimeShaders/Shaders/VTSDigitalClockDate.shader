Shader "VTS/DigitalClockDate"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_EmissionColor ("Emission", Color) = (0,0,0,1)
		_TexChars ("Characters", 2D) = "white" {}
		_TexWP ("WebPanelRender", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
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

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			const float2 uv = IN.uv_TexChars;
			const float2 uvchar = {frac(uv.x*10), uv.y};

			const float2 x1 = {1.0/8, 0};
			const float2 y1 = {0, 1.0/8};

			const int3   sec0 = round(tex2D(_TexWP, x1*3.5+y1*0.5).rgb),   sec1 = round(tex2D(_TexWP, x1*2.5+y1*0.5).rgb);
			const int3   min0 = round(tex2D(_TexWP, x1*5.5+y1*0.5).rgb),   min1 = round(tex2D(_TexWP, x1*4.5+y1*0.5).rgb);
			const int3  hour0 = round(tex2D(_TexWP, x1*7.5+y1*0.5).rgb),  hour1 = round(tex2D(_TexWP, x1*6.5+y1*0.5).rgb);
			const int3  year0 = round(tex2D(_TexWP, x1*7.5+y1*1.5).rgb),  year1 = round(tex2D(_TexWP, x1*6.5+y1*1.5).rgb), year2 = round(tex2D(_TexWP, x1*5.5+y1*1.5).rgb);
			const int3 month0 = round(tex2D(_TexWP, x1*4.5+y1*1.5).rgb), month1 = round(tex2D(_TexWP, x1*3.5+y1*1.5).rgb);
			const int3  date0 = round(tex2D(_TexWP, x1*2.5+y1*1.5).rgb),  date1 = round(tex2D(_TexWP, x1*1.5+y1*1.5).rgb);

			const int sec    =  sec0.r +  sec0.g*2 +  sec0.b*4 +  sec1.r*8 +  sec1.g*16 +  sec1.b*32 + _Time.y;
			const int minute =  min0.r +  min0.g*2 +  min0.b*4 +  min1.r*8 +  min1.g*16 +  min1.b*32 + sec/60.0;
			const int hour   = hour0.r + hour0.g*2 + hour0.b*4 + hour1.r*8 + hour1.g*16 + hour1.b*32 + minute/60.0;

			const float startYear =  year0.r + year0.g*2 + year0.b*4 + year1.r*8 + year1.g*16 + year1.b*32 + year2.r*64 + year2.g*128 + year2.b*256 + 1900;
			const int startMonth  = month0.r +month0.g*2 +month0.b*4 +month1.r*8 +month1.g*16 +month1.b*32;
			const int startDate   =  date0.r + date0.g*2 + date0.b*4 + date1.r*8 + date1.g*16 + date1.b*32 - 1;

			const bool isLeapYear = (fmod(startYear,400) == 0) || (fmod(startYear,4) == 0 && (fmod(startYear,100) != 0));
			const int leapOffset = (isLeapYear ? 13 : 0);

			int MONTH_DAYS[26] = {0,31,59,90,120,151,181,212,243,273,304,334,365,
				0,31,60,91,121,152,182,213,244,274,305,335,366};

			const int startTotalDates = startDate + MONTH_DAYS[startMonth+leapOffset];
			int totalDates = startTotalDates + hour/24.0;

			int year = startYear;
			if(totalDates >= MONTH_DAYS[12+leapOffset]) {
				totalDates -= MONTH_DAYS[12+leapOffset];
				year += 1;
			}

			int date = 0;
			int month = 0;
			for(; month < 12; month++) {
				if(totalDates < MONTH_DAYS[month+1+leapOffset]) {
					date = totalDates - MONTH_DAYS[month+leapOffset];
					break;
				}
			}
			month += 1;
			date += 1;

			const int digit = uv.x*10;
			int ymdd;
			if(digit < 2) {
				ymdd = (digit == 0 ? (year + 0.1)/1000 : fmod((year + 0.1)/100, 10.0));
			} else {
				const int ymdWhich = (digit-2)/8.0*3;
				const int ymd = (ymdWhich == 0 ? year : (ymdWhich == 1 ? month : date));
				ymdd = fmod((uint)(fmod((digit-2.0), 3.0) == 0 ? ymd * 0.1 : ymd) + 0.1, 10.0);
			}

			float2 uv0 = {(floor(fmod(ymdd + 0.1, 8.0))+uvchar.x)/8, 1-(floor((ymdd + 0.1)/8.0)+1-uvchar.y)/2};
			const float2 uvslash = {(uvchar.x-4.0)/8, uvchar.y/2};
			uv0 = (digit == 4 || digit == 7) ? uvslash : uv0;

			fixed4 c = tex2Dgrad(_TexChars, uv0, ddx(uv) / float2(1.25, 2.0), ddy(uv) / float2(1.25, 2.0));
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
