"""Генерация спрайтов труб для головоломки. Запуск: python generate_pipe_sprites.py"""
from PIL import Image
import os

SIZE = 128
PIPE_W = 24
BORDER = 8
OUTER = (80, 100, 120, 255)
INNER = (100, 130, 160, 255)
CENTER = (60, 80, 100, 255)
CLEAR = (0, 0, 0, 0)

def create_base():
    return Image.new("RGBA", (SIZE, SIZE), CLEAR)

def draw_pipe_h(img, y):
    cy = SIZE // 2
    half = PIPE_W // 2
    for x in range(BORDER, SIZE - BORDER):
        for dy in range(-half, half + 1):
            py = cy + dy
            if 0 <= py < SIZE:
                dist = abs(dy)
                if dist < 4:
                    img.putpixel((x, py), CENTER)
                elif dist < half:
                    img.putpixel((x, py), INNER)
                else:
                    img.putpixel((x, py), OUTER)

def draw_pipe_v(img, x):
    cx = SIZE // 2
    half = PIPE_W // 2
    for y in range(BORDER, SIZE - BORDER):
        for dx in range(-half, half + 1):
            px = cx + dx
            if 0 <= px < SIZE:
                dist = abs(dx)
                if dist < 4:
                    img.putpixel((px, y), CENTER)
                elif dist < half:
                    img.putpixel((px, y), INNER)
                else:
                    img.putpixel((px, y), OUTER)

def pipe_straight_h():
    img = create_base()
    draw_pipe_h(img, SIZE // 2)
    return img

def pipe_straight_v():
    img = create_base()
    draw_pipe_v(img, SIZE // 2)
    return img

def pipe_corner():
    img = create_base()
    cx, cy = SIZE // 2, SIZE // 2
    half = PIPE_W // 2
    for y in range(BORDER, cy + half + 1):
        for dx in range(-half, half + 1):
            px = cx + dx
            if 0 <= px < SIZE:
                dist = abs(dx)
                c = CENTER if dist < 4 else (INNER if dist < half else OUTER)
                img.putpixel((px, y), c)
    for x in range(cx - half, SIZE - BORDER):
        for dy in range(-half, half + 1):
            py = cy + dy
            if 0 <= py < SIZE:
                dist = abs(dy)
                c = CENTER if dist < 4 else (INNER if dist < half else OUTER)
                img.putpixel((x, py), c)
    return img

def pipe_t():
    img = create_base()
    cx, cy = SIZE // 2, SIZE // 2
    half = PIPE_W // 2
    for y in range(BORDER, cy + half + 1):
        for dx in range(-half, half + 1):
            px = cx + dx
            if 0 <= px < SIZE:
                dist = abs(dx)
                c = CENTER if dist < 4 else (INNER if dist < half else OUTER)
                img.putpixel((px, y), c)
    for x in range(BORDER, SIZE - BORDER):
        for dy in range(-half, half + 1):
            py = cy + dy
            if 0 <= py < SIZE:
                dist = abs(dy)
                c = CENTER if dist < 4 else (INNER if dist < half else OUTER)
                img.putpixel((x, py), c)
    return img

def pipe_cross():
    img = create_base()
    draw_pipe_h(img, SIZE // 2)
    draw_pipe_v(img, SIZE // 2)
    return img

def main():
    out_dir = os.path.join(os.path.dirname(__file__), "Assets", "Sprites", "PipePuzzle")
    os.makedirs(out_dir, exist_ok=True)
    pipe_straight_h().save(os.path.join(out_dir, "pipe_straight_h.png"))
    pipe_straight_v().save(os.path.join(out_dir, "pipe_straight_v.png"))
    pipe_corner().save(os.path.join(out_dir, "pipe_corner.png"))
    pipe_t().save(os.path.join(out_dir, "pipe_t.png"))
    pipe_cross().save(os.path.join(out_dir, "pipe_cross.png"))
    print("Спрайты сохранены в", out_dir)

if __name__ == "__main__":
    main()
