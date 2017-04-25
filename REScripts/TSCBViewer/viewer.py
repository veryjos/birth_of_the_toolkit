import pygame

import math
import os
import sys
import struct

from Util import BinaryReader, Camera
from BoTWHeightmap import Heightmap, Config

size = [1024, 768]
camera = Camera(size)
camera.SetLodLevel(32.0)

# Initialize pygame
pygame.init()

screen = pygame.display.set_mode(size)

# Run a loop until done rendering
done = False
clock = pygame.time.Clock()

# Create a new binary reader
filename = sys.argv[1]

f = open(filename, 'rb')
breader = BinaryReader(f, ">")

# Create a new heightmap
heightmap = Heightmap(breader)

currentLodLevel = 0
while not done:
    clock.tick(30)
    pygame.event.pump()

    # move the camera
    speed = 20
    zoomSpeed = 1
    keyTransform = {
        pygame.K_w: (0, -1, 0),
        pygame.K_a: (-1, 0, 0),
        pygame.K_s: (0, 1, 0),
        pygame.K_d: (1, 0, 0),
        pygame.K_q: (0, 0, -0.05),
        pygame.K_e: (0, 0, 0.05)
    }

    keysDown = pygame.key.get_pressed()
    for k in keyTransform:
        if keysDown[k]:
            t = keyTransform[k]
            camera.Translate((t[0] * speed, t[1] * speed))
            camera.ZoomDelta(t[2] * zoomSpeed)

    keyLodLevel = {
        pygame.K_1: 32.0 / pow(2, 0),
        pygame.K_2: 32.0 / pow(2, 1),
        pygame.K_3: 32.0 / pow(2, 2),
        pygame.K_4: 32.0 / pow(2, 3),
        pygame.K_5: 32.0 / pow(2, 4),
        pygame.K_6: 32.0 / pow(2, 5),
        pygame.K_7: 32.0 / pow(2, 6),
        pygame.K_8: 32.0 / pow(2, 7),
        pygame.K_9: 32.0 / pow(2, 8),
        pygame.K_0: "ALL"
    }

    for k in keyLodLevel:
        if keysDown[k]:
            l = keyLodLevel[k]
            camera.SetLodLevel(l)

    for ev in pygame.event.get():
        if ev.type == pygame.KEYDOWN:
            if ev.key == pygame.K_t:
                Config.draw_textures = not Config.draw_textures
            elif ev.key == pygame.K_c:
                Config.draw_coverage = not Config.draw_coverage
            elif ev.key == pygame.K_g:
                Config.draw_grid = not Config.draw_grid
            elif ev.key == pygame.K_o:
                Config.draw_overdraw = not Config.draw_overdraw
        elif ev.type == pygame.MOUSEBUTTONDOWN:
            clickCoords = pygame.mouse.get_pos()

            heightmap.unk1Collection.CheckClick(camera, clickCoords)

    screen.fill((0, 0, 0))

    heightmap.Draw(screen, camera)

    pygame.display.flip()

f.close()
