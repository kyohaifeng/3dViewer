#version 330 core
out vec4 FragColor;

in vec2 TexCoords;
in vec4 vertexColor; // the input variable from the vertex shader (same name and same type)  
uniform sampler2D texture_diffuse1;

void main()
{    
    //FragColor = texture(texture_diffuse1, TexCoords);
    FragColor = vertexColor;
}
