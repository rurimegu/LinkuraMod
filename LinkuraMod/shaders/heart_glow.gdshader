shader_type canvas_item;

uniform float pulse_speed : hint_range(0.5, 5.0) = 2.5;
uniform float glow_low    : hint_range(0.2, 1.5) = 0.7;
uniform float glow_high   : hint_range(1.0, 4.0) = 1.9;
uniform vec3  glow_tint   : source_color = vec3(1.0, 0.72, 0.84);
uniform float base_alpha  : hint_range(0.0, 1.0) = 0.8;

void fragment() {
	vec4 tex = texture(TEXTURE, UV);
	float t = (sin(TIME * pulse_speed) + 1.0) * 0.5;
	float intensity = mix(glow_low, glow_high, t);
	COLOR = vec4(tex.rgb * glow_tint * intensity, tex.a * intensity * base_alpha);
}
