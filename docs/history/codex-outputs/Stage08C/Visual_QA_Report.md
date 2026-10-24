# Visual QA

## Actual captures
Screenshots/01_Walk_URP_NoHUD.png and02_Rover_URP_NoHUD.png: actual isolated Editor Play Mode renders during controlled walking/driving. They omit the overlay HUD and touch GUI; they are not proof of hands-on input.

Screenshots/03_LightDust_URP_NoHUD.png,04_Storm_URP_NoHUD.png,05_Clear_URP_NoHUD.png: actual controlled Editor weather renders.

Screenshots/Android_0_clear.png,Android_1_light_dust.png,Android_2_storm.png,Android_3_storm_particles_disabled.png: actual final Android framebuffer captures with mission HUD and touch controls. Android_Live_Touch_UI.png is the earlier diagnostic trial showing the now-fixed screenshot error overlay; do not present it as the clean final state.

Android_Runtime_Preview.mp4: actual10-second screenrecord captured on the emulator after all performance samples completed. It is a stationary runtime/touch-UI visual preview, not hands-on gameplay or proof of driving/gesture behavior. No audio capture. No generated images, artificial composite screenshots or invented footage.

## Observations
Clear view shows ochre textured ground, pale habitats, terrain/delta landmarks, simple astronaut and shadows. Dust states increase haze and reduce distant ground detail; storm particle count is bounded. Mobile HUD and joystick/run/enter/drive/brake controls are visible and readable in the inspected landscape capture. Right-side orbit hint is clear. No pink terrain/material surfaces observed in these views.

Character is rigid primitive limb geometry with authored clips; apparent foot floating/foot placement still needs art refinement. Rover is a kinematic primitive proxy, not a suspension/traction model. Existing static vehicles/structures are decorative; one rover is drivable. Interface is a prototype; physical target sizes, cutouts, accessibility, multitouch ergonomics, foldables and portrait handling remain unverified. Far fog silhouettes against the static sky remain stylized. Weather/resources are synthetic gameplay presentation, not measured Mars conditions.

## Blocked coverage
No complete manual desktop gameplay sequence, RMB orbit/camera-occlusion demonstration, all-target traversal or human Android gesture capture. No actual main-Editor Scene View capture in this stage. See Manual_Gameplay_Verification.md for the mandatory UI-testing stop.
