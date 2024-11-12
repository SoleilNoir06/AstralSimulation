#version 330

// Input attributes from vertex shader
in vec3 fragPosition;
in vec3 fragNormal;
in vec2 fragTexCoord;

uniform sampler2D texture0;
uniform vec4 colDiffuse;

out vec4 pixelColor;

// Main function of the fragment shader program
void main()
{
    // Map UVs
    vec2 uv = vec2(fragTexCoord.y, fragTexCoord.x);

    pixelColor = texture(texture0, uv); // Final pixel color calculation
}