#version 330 core
out vec4 outputColor;

in vec2 TexCoords;

uniform sampler2D texture_diffuse1;

void main()
{    
    outputColor = vec4(0.0,0.0,1.0, 1.0);
}
