#version 330

in vec2 fragTexCoord;

uniform sampler2D texture0; // Render texture

uniform vec2 resolution; // Dimensions de l'écran
uniform vec2 lightPosition; // Position de la lumière (soleil)
uniform vec3 lightColor; // Couleur de la lumière
uniform float intensity; // Intensité du glow

out vec4 pixelColor;

// Main function of the fragment shader program
void main()
{
    //vec2 uv = fragTexCoord.xy / resolution;
    //vec2 toLight = uv - lightPosition / resolution;
    //float dist = length(toLight);

    //float glow = exp(-dist * intensity) * 1.5;

    //pixelColor = vec4(lightColor * glow, 1.0);*/

    vec4 texelColor = texture(texture0, fragTexCoord);

    pixelColor = texelColor;
}