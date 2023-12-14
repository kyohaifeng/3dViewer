#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec2 TexCoords;
out vec4 vertexColor; // specify a color output to the fragment shader

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    TexCoords = aTexCoords;    
    // gl_Position = projection * view * model * vec4(aPos, 1.0);
    // note that we read the multiplication from right to left
    gl_Position =  vec4(aPos, 1.0) * model * view * projection;
    vertexColor = vec4(0.5, 0.0, 0.0, 1.0); // set the output variable to a dark-red color
}
