# Stargate VR
Javier Cabero @ Copyright 2024

This is a learning project to get started with Unity and OpenXR. The goal is to find the star in the dungeon. The main technical idea was to do random level generation in Unity and the essential pieces that make a VR game. This helped me understand the scope and effort involved in each of the areas. I've followed [Black Whale Studios](https://www.youtube.com/watch?v=0rL_YrqG7lU) tutorials on Youtube for the set up and basic game mechanics with the XR Interaction Toolkit. Using ChatGPT for Unity scripts improved a lot my understanding of the framework and writing the MonoBehavior scripts. All assets were free stuff I found in Unity Asset Store.

# Game features

### A fade-in starting screen
A gong plays while the background music begins. Darkness fades to the view of the initial dungeon room. Few doors remain open for the player to choose their path. It's possible to see what's in each room ahead, so a player would probably choose the weapon room first before the monster one.

https://github.com/reivash/stargate-vr/assets/3762851/4c66652a-1d80-4252-94c5-16b865982b37

<img width="525" alt="image" src="https://github.com/reivash/stargate-vr/assets/3762851/f8d6f1dc-f1d3-44fd-bfdc-a9a987b42bf6">

### Auto generated level
Few settings determine the number of monster, weapon and puzzle rooms.

<img width="613" alt="image" src="https://github.com/reivash/stargate-vr/assets/3762851/bfa494c5-6716-491d-9c0b-ac89fac7f037">

Originally, the level was made as the player walked through it by expanding 3x3 sections around them though later an algorithm for a fixed level layout generation on startup was added. This improves performance.

![dynamic-dungeon-expansion](https://github.com/reivash/stargate-vr/assets/3762851/055b2734-8b2e-40cf-b2af-40234433a85a)

### Random decoration
<img width="606" alt="image" src="https://github.com/reivash/stargate-vr/assets/3762851/14e1986b-1816-4fe7-a10f-36e645dd4a03">
<img width="608" alt="image" src="https://github.com/reivash/stargate-vr/assets/3762851/8d22cba6-ebde-42ba-95f0-85178f4f4460">

### Combat system
A simple combat system was implemented:
- The player can pick up an axe and successfully hit enemies. The axe plays a sound when picked up and the enemies complain about the hit.
- The player can get hit but recovers if undamaged. The screen will turn red on and off for signaling it.
- Enemies can be killed. A death animation is triggered when their health reaches below 0.
- Enemies are implemented with Unity agents and require a NavMesh to navigate. The NavMesh is implemented through an invisible floor that's fixed below the generated dungeon.

https://github.com/reivash/stargate-vr/assets/3762851/7f0c5803-5ce2-4739-8774-a082466af374

https://github.com/reivash/stargate-vr/assets/3762851/87210e9a-dab9-4aec-8935-856e837314c5

https://github.com/reivash/stargate-vr/assets/3762851/2e9cdec9-c9fe-4cad-88ae-f99b43bc7e6c

### Reaching the stargate 
Reaching the stargate is the goal of the game. A victory screen and music will appear when doing so. 

https://github.com/reivash/stargate-vr/assets/3762851/50a84a1f-94d7-435b-b744-4de9604eb9af

# Set up
You can test this game by downloading Unity and OpenXR. The configuration I followed can be seen in this video. https://www.youtube.com/watch?v=0rL_YrqG7lU



