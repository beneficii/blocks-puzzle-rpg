import os
from PIL import Image
from collections import defaultdict

def get_colors_from_image(image_path):
    """Extract unique colors from a PNG image."""
    with Image.open(image_path) as img:
        img = img.convert("RGBA")  # Ensure consistent color format
        colors = img.getcolors(img.width * img.height)
        unique_colors = {color[1] for color in colors} if colors else set()
    return unique_colors

def traverse_and_collect_colors(directory):
    """Recursively traverse directory to collect colors from PNG files."""
    all_colors = set()
    for root, _, files in os.walk(directory):
        for file in files:
            if file.lower().endswith(".png"):
                file_path = os.path.join(root, file)
                all_colors.update(get_colors_from_image(file_path))
    return all_colors

def create_color_image(colors, output_path, tile_size=10):
    """Create an image showing each unique color as a tile."""
    num_colors = len(colors)
    grid_size = int(num_colors**0.5) + 1  # Determine grid dimensions
    image_size = (grid_size * tile_size, grid_size * tile_size)
    
    # Create a new blank image
    color_image = Image.new("RGBA", image_size, (255, 255, 255, 0))
    draw = Image.new("RGBA", (tile_size, tile_size))

    # Draw each color as a tile
    colors = list(colors)
    for i, color in enumerate(colors):
        row, col = divmod(i, grid_size)
        for x in range(tile_size):
            for y in range(tile_size):
                draw.putpixel((x, y), color)
        color_image.paste(draw, (col * tile_size, row * tile_size))
    
    color_image.save(output_path)

if __name__ == "__main__":
    output_path = "all_colors.png"
    all_colors = traverse_and_collect_colors(".")
    create_color_image(all_colors, output_path)
    print(f"All colors image created: {output_path}")