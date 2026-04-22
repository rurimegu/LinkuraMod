shader_type canvas_item;
render_mode unshaded;  // blend_mix (default) — blend_add is broken inside CanvasGroups

uniform float progress : hint_range(-0.2, 1.2) = -0.2;

// --- Tunable parameters ---
// Sweep speed : controlled from C# — tween duration in DoGlintAsync (currently 0.55s)
// Core width  : core_half — half-width of the sharp bright line in UV space (0..1)
// Glow width  : glow_half — half-width of the soft outer halo
// Peak alpha  : core peaks at 1.0, glow peaks at glow_strength
uniform float core_half : hint_range(0.005, 0.1) = 0.04;
uniform float glow_half : hint_range(0.02, 0.3) = 0.15;
uniform float glow_strength : hint_range(0.1, 1.0) = 0.6;

void fragment() {
	float d = (UV.x + UV.y) * 0.5;
	float dist = abs(d - progress);

	float core = clamp(1.0 - dist / core_half, 0.0, 1.0);
	core = core * core;

	float glow = clamp(1.0 - dist / glow_half, 0.0, 1.0);
	glow = glow * glow * glow_strength;

	float alpha = min(core + glow, 1.0);
	COLOR = vec4(1.0, 1.0, 1.0, alpha);
}
