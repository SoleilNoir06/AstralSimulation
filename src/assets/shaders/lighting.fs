#version 330

// Input attributes from vertex shader
in vec3 fragPosition;
in vec3 fragNormal;
in vec2 fragTexCoord;

uniform sampler2D texture0;
uniform vec4 colDiffuse;
uniform vec4 ambient;
uniform vec3 viewPos;
uniform vec4 lightCol;

out vec4 pixelColor;

// Main function of the fragment shader program
void main()
{
    // Map UVs
    vec2 uv = vec2(fragTexCoord.y, fragTexCoord.x);

    vec4 texelColor = texture(texture0, uv); // Get texture color
    vec3 lightDot = vec3(0.0);
    vec3 normal = normalize(fragNormal);
    vec3 viewD = normalize(viewPos - fragPosition);
    vec3 specular = vec3(0.0);

    // Light calculations

    vec3 light = vec3(0.0);

    //if (lights[i].type == LIGHT_DIRECTIONAL)
    //{
    //    light = -normalize(lights[i].target - lights[i].position);
    //}

    light = -normalize(fragPosition);

    float NdotL = max(dot(normal, light), 0.0);
    lightDot += lightCol.rgb*NdotL;

    float specCo = 0.0;
    if (NdotL > 0.0) specCo = pow(max(0.0, dot(viewD, reflect(-(light), normal))), 16.0); // 16 refers to shine
    specular += specCo;

    pixelColor = (texelColor*((colDiffuse + vec4(specular, 1.0))*vec4(lightDot, 1.0)));
    pixelColor += texelColor*(ambient)*colDiffuse;

    // Gamma correction
    pixelColor = pow(pixelColor, vec4(1.0/2.2));
}