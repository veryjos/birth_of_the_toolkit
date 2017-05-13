#version 130

in vec3 Position;
in vec3 Normal;
in vec4 Color;

out vec4 vColor;

uniform mat4 modelView;

void main() {
  vec3 lightAngle = vec3(1.0, -0.9, 0.9);
  float lightFraction = clamp(dot(Normal, lightAngle), 0, 1);

  vColor = vec4(lightFraction, lightFraction, lightFraction, 1.0);

  gl_Position = modelView * vec4(Position, 1.0);
}
