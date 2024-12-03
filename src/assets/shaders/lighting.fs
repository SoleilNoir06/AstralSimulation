#version 330

// Input attributes from vertex shader
in vec3 fragPosition;
in vec3 fragNormal;
in vec2 fragTexCoord;

uniform sampler2D texture0;
uniform vec3 viewPos;
uniform vec4 lightColor;

out vec4 pixelColor;

// Lighting function
vec4 lighting()
{
    vec4 ambient = vec4(0.2); // Constant ambient lighting level

    vec3 view = normalize(viewPos - fragPosition);
    vec3 light = normalize(-fragPosition);

    float incidentAngle = max(dot(fragNormal, light), 0.0);

    vec4 diffuse = vec4(lightColor.rgb * incidentAngle * 2, 1.0);

    return  mix(-ambient, diffuse, incidentAngle);
}

// Main function of the fragment shader program
void main()
{
    // Map UVs
    vec2 uv = vec2(fragTexCoord.y, fragTexCoord.x);

    vec4 texelColor = texture(texture0, uv);

    pixelColor = mix(lighting(), texelColor, 0.5);
}