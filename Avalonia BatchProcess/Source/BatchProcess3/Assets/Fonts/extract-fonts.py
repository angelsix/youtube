#!/usr/bin/env python3

import sys
from fontTools.ttLib import TTFont
from fontTools.varLib.instancer import instantiateVariableFont

def extract_font_instances(variable_font_path, new_font_name, output_dir):
    
    """ Extract all common weight fonts from inside the variable font """
    font = TTFont(variable_font_path)

    instances = [
        ("Thin", {"wght": 100}),
        ("Extralight", {"wght": 200}),
        ("Light", {"wght": 300}),
        ("Regular", {"wght": 400}),
        ("Medium", {"wght": 500}),
        ("Semibold", {"wght": 600}),
        ("Bold", {"wght": 700}),
        ("ExtraBold", {"wght": 800}),
        ("Black", {"wght": 900}),
    ]
    
    for name, location in instances:
        static_font = instantiateVariableFont(font, location)
        
        output_path = f"{output_dir}/{new_font_name}-{name}.ttf"
        static_font.save(output_path)
        
    return True

if __name__ == "__main__":
    
    if len(sys.argv) < 4:
        print("Usage: python3 extract-fonts.py <variable_font_path> <new_font_name> <output_dir>")
        print("Example: python3 extract-fonts.py NotoSansJP.ttf NotoSansJP .")

        sys.exit(1)
        
    extract_font_instances(sys.argv[1], sys.argv[2], sys.argv[3])