# Paeezan Technical Interview

# Assignment Document

This assignment is about making a prototype meant to showcase how you approach building a small but complete online game slice from scratch. You’ll implement a **simple multiplayer game**, client and server.   
We’re evaluating your ability to choose sensible frameworks, design clean client/server boundaries, keep gameplay logic consistent between peers, and ship something runnable with clear docs and a Dockerized server. Prioritize correctness, clarity, and resilience over visuals. readable code, and a concise README explaining your decisions matter more than polish.

## **Gameplay**

* **Mode:** Real-time **1v1**. Each player has one tower; the first tower to reach 0 health loses.

* **Game Arena:** Single straight lane; top-down or side view is fine.

* **Units:** We should have at least **2** units. A **melee** unit and a **ranged** unit. Players deploy them via **cards** with **cooldowns** (no mana/energy)  
  * Bonus: implement mana system.

* **Behavior:** Units march toward the opponent’s tower and damage it when in reach.  
  * Bonus: units can also target and fight enemy units.

* **Match Join:** No matchmaking needed. One player creates a room and gets a code and the other player can join the battle with that code.

* **UI (minimal):** Login/Register → Create room/Join Room → Battle scene with:

  * Your/Enemy tower HP  
  * Two card buttons with visible cooldowns  
  * Basic match status (e.g., “Waiting for opponent”, “You Win/Lose”)

## **Requirements**

* **Client:** Unity (LTS of your choice). Render the battle and send player actions. Keep things simple and readable.  
  * (Bonus): although there is no requirement for visuals and sounds, it is appreciated if the game visuals and effects are something acceptable 

* **Server:** Any stack/language. Responsibilities:  
  * **Room:** Create/Join room by a short code.  
  * **Realtime:** Provide a lightweight realtime channel between the two players (e.g., WebSocket) to exchange game messages.  
  * (Bonus) **Accounts:** Register/Login  
  * (Bonus) **Persistence:** Store a **win count per user**. Saved in a database.

* **Config:** Put unit/tower stats and card cooldowns in a small config file (e.g., JSON) so they’re easy to tweak.

* **Docker:** Server must be **dockerized** (Dockerfile). A docker-compose file for one-command start is appreciated.

* **Quality:** Clear README, sensible folder structure, small architecture note (max 1 page), basic logging.

## **What to Submit**

* A zip file containing:  
  1- link to your **git repository** with:  
  * `/client` — Unity project.  
  * `/server` — backend code \+ **Dockerfile** (and `docker-compose.yml` if used).  
  * `README.md` — how to run server (Docker) and client locally with two players, API brief, and the short architecture note.  
  * `config/` — unit/tower stats and card cooldowns.  
  * `demo/` — a short screen capture showing the game.

  2- An **apk or windows build** of the client.

* We will **clone** and run locally. Please make sure the repo builds cleanly and includes any necessary seed data or scripts.

## **Notes**

* Feel free to use AI tools for development.