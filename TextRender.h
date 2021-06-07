#pragma once
#include "Common.h"
#include <ft2build.h>
#include FT_FREETYPE_H 
#include "Shader.h"

class TextRender
{
    struct Character
    {
        unsigned int TextureID;
        glm::ivec2   Size;
        glm::ivec2   Bearing;
        unsigned int Advance;
    };

    std::map<GLchar, Character> Characters;

    FT_Library ft;

    unsigned int VAO, VBO;

public:
    TextRender() {}

    void Init(std::string font_path);

    void RenderText(Shader& shader, std::string text, float x, float y, float scale, glm::vec3 color);
};

