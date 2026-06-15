# Unity Liver 3D Asset

Files:
- realistic_liver_unity.glb: Main Unity-importable GLB scene with separated anatomical parts.
- realistic_liver_unity.obj: OBJ fallback.
- realistic_liver_unity.stl: Mesh fallback for inspection/printing workflows.
- liver_albedo.png / liver_normal.png / liver_roughness.png: procedural PBR texture maps.

Unity usage:
1. Drag the .glb file into Assets/Models.
2. Drag texture PNG files into Assets/Textures.
3. Create a material using URP/Lit or Standard shader.
4. Assign liver_albedo as Base Map, liver_normal as Normal Map, liver_roughness to Smoothness/Roughness workflow depending on render pipeline.
5. Scale in Unity as needed. The mesh is centered near origin and designed for MR close inspection.

Anatomical notes:
- Includes asymmetric right/left lobe morphology, caudate region/IVC groove, gallbladder fossa, porta hepatis region, portal vein, hepatic artery, bile duct, and visceral surface impressions.
- This is a procedural approximation for educational MR prototyping, not a diagnostic medical segmentation or certified anatomical model.
