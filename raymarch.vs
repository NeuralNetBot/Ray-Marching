#version 330 core

layout (location = 0) in vec3 a_position;
layout (location = 1) in vec2 aTexCoords;

void main()
{
    gl_Position = vec4(a_position, 1);
}