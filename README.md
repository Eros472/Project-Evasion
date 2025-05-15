# Project Evasion

_A stealth-based game by Beyond the Box_

---

## 🎮 Pitch

**Project Evasion** is a stealth-based game where the player must navigate through two levels while evading detection from unique enemies.

---

## 🌍 Setting

Each level has a unique environment based on the mission and target:

- **Level 1**: Town  
- **Level 2**: Mystic Forest 

---

## 🧱 Game Components

### Objects

- Player Character  
- Enemies (NPCs)    
- Obstacles (walls, crates, projectiles)  
- Portal- to move to next level
- Weapons (dagger, bow and arrow)

### Attributes

- **Player**: Position, movement speed, visibility, detection status, healthbar, inventory, animations
- **Enemies**: Position, patrol path, vision cone (direction & length), alert level, attacking ability, projectile-based attacks 
- **Environment**: Dodging mechanics, evasion of detection zones
- **Interactables**: Level exit (position, state), weapon pickups, inventory slot options

---

## 🔗 Relationships

- Player entering a guard’s field of view triggers detection → attacks player
- Players must reach the level exit to progress   

---

## 🕹️ Game Mechanics

- **Controls**:
  - Move: `W`, `A`, `S`, `D`  
  - Crouch: `LCTRL`  
  - Sprint: `LSHIFT` 

- **Character Classes**:
  - Each class has unique base stats and a special ability

- **Goals**:
  - Avoid being detected  
  - Reach level exit
  - Avoid projectiles from evasion-based enemies and patrolling enemies' line of sight 

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

- **Erick Hambardzumyan**: Gameplay logic, mechanics, testing, player controls, animations, level design, sprite design, debugging, creativity
- **Arneh Khachatoorian**: Story lore, debugging gameplay logic, final boss design, health and inventory system, main menu design
