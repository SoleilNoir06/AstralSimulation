# Astral Simulation 🪐⭐

## Description
Welcome to Astral Simulation. Here is a 3D simulation of our solar system. In C# using Raylib_cs based program, you play the role of a probe and can travel through the solar system.

This project is a school project to prepare to diploma work. Obviously, the diploma work must be done alone but this one was a two-person job.

## Authors ✏
- [@SoleilNoir06](https://www.github.com/SoleilNoir06)
- [@FlamingRiot](https://www.github.com/FlamingRiot)

## How it works

### How is calculated data ? ➕➖
Each planet is based on real data from the [Nasa](https://nssdc.gsfc.nasa.gov/planetary/planetfact.html) site web.
Every data is scaled to ensure the proper functioning of the simulation (e.g. Every size and distance from sun are divided by 15'000'000).

### How is the data stored ? 📜
The data is stored in JSON format. You can find the file in AstralSimulation/src/assets/json/solarSystem.json.

## Simulation

### How to use ❓
To open simulation simply open the AstralSimulation/src/astral-simulation.csproj and run the program.
If you're experimented you can do a :
```bash
  docker build
```

### Functionalities ⚙
- Once in the simulation, simply use W, A, S, D to move and left click of the mouse to control the inclination of the probe.  ⚠ The sensitivity is pretty high.
- To go up and down, use keys Space and F.
- You can move planet by planet using the directional arrows ⬅➡.
- Also using the left click, you can select a planet and the probe will travel to it.

### What to see 🚀
Of course as we said earlier, you can travel through the solar system but which data can you see ?
After clicking or travelling to a planet, a window will open with some informations like :
- Name
- Type
- Mass
- Radius
- Volume
- Distance from sun

## Important ‼
In this simulation, the sun is only a shader. Therefore you can't select it or travel to it and see informations about it.

## Corrections to do 🔧
- Adjust size of sun. At the moment the sun is the same size wherever the probe is.
- Adjust Saturn's rings. At the moment its rings are square.
- Correct Uranus rotation. Uranus rotates in the opposite direction to the other planets.