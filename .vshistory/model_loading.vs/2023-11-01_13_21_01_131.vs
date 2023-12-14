#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec2 TexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    TexCoords = aTexCoords;    
    // gl_Position = projection * view * model * vec4(aPos.X + 41000,aPos.Y + 57000,aPos.Z, 1.0);
    // note that we read the multiplication from right to left
    gl_Position =  vec4(aPos.x + 41000,aPos.y + 57000,aPos.z, 1.0) * model * view * projection;
}
