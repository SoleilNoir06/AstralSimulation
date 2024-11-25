#version 330

in vec2 fragTexCoord;

uniform sampler2D texture0;
uniform sampler2D shineTexture;

out vec4 finalColor;

void main()
{
    vec4 texelColor = texture(texture0, fragTexCoord);

    vec2 reducedUV = fragTexCoord;
    vec4 shineCol = texture(shineTexture, reducedUV);
    //texelColor = mix(texelColor, shineCol, 0.5);

    finalColor = shineCol;
}