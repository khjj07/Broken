#define ROUNDING_PREC 0.999
#define PIXELSIZE 5.0
inline void PixelClipAlpha_float(float4 pos, float alpha_in, out float alpha_out)
{
	alpha_in = clamp(round(alpha_in), 0.0, 1.0);
	float xfactor = step(fmod(abs(floor(pos.x)), PIXELSIZE), ROUNDING_PREC);
	float yfactor = step(fmod(abs(floor(pos.y - PIXELSIZE)), PIXELSIZE), ROUNDING_PREC);
	alpha_out = alpha_in * xfactor * yfactor * alpha_in;
}