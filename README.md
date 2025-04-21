# Project Evasion

_A stealth-based game by Beyond the Box_

---

## 🎮 Pitch

**Project Evasion** is a stealth-based game where the player must navigate through four levels while evading detection from guards and surveillance cameras. Each level contains a unique item required to proceed to the next.

---

## 🌍 Setting

Each level has a unique environment based on the mission and target:

- **Level 1**: Public Market  
- **Level 2**: Company Building  
- **Level 3**: Mansion  
- **Level 4**: Government Building  

---

## 🧱 Game Components

### Objects

- Player Character  
- Guards (NPCs)  
- Cameras/Drones  
- Obstacles (walls, crates, furniture)  
- Hiding Spots (bushes, sewers, vents, shadows)  
- Power-ups (3 needed to activate ability)  
- GUI Elements (Time, Ability button, Hide button, Level exit)  

### Attributes

- **Player**: Position, movement speed, visibility, ability cooldown, detection status, lives/checkpoints, movement physics  
- **Guards**: Position, patrol path, vision cone (direction & length), alert level  
- **Cameras/Drones**: Position, rotation speed, vision cone, detection delay  
- **Environment**: Obstacle size/position, line-of-sight blocking  
- **Interactables**: Hiding spots (occupancy, position), power-ups (position, type), buttons (pressed status), level exit (position, state), checkpoints (position, activation)  
- **Game State**: Time remaining, power-up count, ability state, hide interaction status  

---

## 🔗 Relationships

- Player entering a guard’s field of view triggers detection → restart from last checkpoint or level start  
- Players must collect a unique item in each level to continue  
- Players must reach the level exit to progress  
- Each character has a unique special ability activated after collecting 3 power-ups  
- Countdown timer enforces level completion within time limit  
- Life/checkpoint mechanics vary with difficulty:  
  - Easy levels: 1 life → restart on failure  
  - Harder levels: 3 lives or checkpoints for respawn  
- Obstacles block line of sight  
- Hiding spots grant temporary invisibility  
- Cameras/drones alert when detecting player  
- Hide button is usable only near hiding spots  
- Checkpoints save progress

---

## 🕹️ Game Mechanics

- **Controls**:
  - Move: `W`, `A`, `S`, `D`  
  - Hide: `E` (near hiding spots)  
  - Use Ability: `R` (requires 3 power-ups)

- **Character Classes**:
  - Each class has unique base stats and a special ability

- **Goals**:
  - Avoid being detected  
  - Reach level exit before time runs out  
  - Avoid patrolling guards’ and cameras' line of sight  

---

## ⭐ Optional Features

- High Score System  
- Achievement List (e.g., finish with every class, complete game with no deaths)

---

## 👥 Team Members

- **Erick Hambardzumyan**  
  - 2 months Unreal Engine experience  
  - Minimal Unity experience  
  - 2 years C, C++, Java  
  - Strong math & physics background, passion for games

- **Arneh Khachatoorian**  
  - 1–2 years experience in C++, Java, Python  
  - Minimal Unity experience  
  - Passion for video games

---

## 🔧 Division of Labor

- **Erick Hambardzumyan**: Gameplay logic, mechanics, testing, player controls  
- **Arneh Khachatoorian**: Level design, lore, character/class design
