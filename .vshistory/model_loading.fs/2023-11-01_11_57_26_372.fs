#version 330 core
out vec4 outputColor;

in vec2 TexCoords;

uniform sampler2D texture_diffuse1;

void main()
{    
    outputColor = texture(texture_diffuse1, TexCoords);
}
